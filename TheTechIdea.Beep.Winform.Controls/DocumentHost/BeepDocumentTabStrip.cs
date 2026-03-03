using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Figma-quality tab strip for <see cref="BeepDocumentHost"/>.
    /// Supports Chrome, VS Code, Underline and Pill visual styles with DPI scaling,
    /// animated active-indicator, scroll overflow, middle-click-to-close and drag reorder.
    /// Inherits <see cref="Control"/> directly — no BaseControl overhead.
    /// </summary>
    [ToolboxItem(true)]
    [Description("Beep themed tab strip — use inside BeepDocumentHost.")]
    public partial class BeepDocumentTabStrip : Control
    {
        // ─────────────────────────────────────────────────────────────────────
        // State  (DPI helpers / constants / properties / events → partial files)
        // ─────────────────────────────────────────────────────────────────────

        private readonly List<BeepDocumentTab> _tabs = new List<BeepDocumentTab>();
        private IBeepTheme _theme;
        private string _themeName = string.Empty;

        // Scroll
        private int  _scrollOffset;
        private bool _showScrollButtons;
        private int  _totalTabsWidth;

        // Mouse
        private int  _hoverTabIndex = -1;
        private bool _hoverClose;
        private bool _hoverAdd;
        private bool _hoverScrollLeft;
        private bool _hoverScrollRight;

        // Drag
        private bool  _dragging;
        private int   _dragStartTab = -1;
        private Point _dragStartPoint;

        // Drag-to-float
        private bool  _dragFloating;
        private Form? _dragFloatGhost;

        // Geometry
        private Rectangle _addButtonRect;
        private Rectangle _scrollLeftRect;
        private Rectangle _scrollRightRect;
        private Rectangle _overflowButtonRect;     // ▾ overflow drop-down
        private bool      _hoverOverflow;

        // Active indicator slide animation
        private Rectangle _indicatorCurrent = Rectangle.Empty;
        private Rectangle _indicatorTarget  = Rectangle.Empty;
        private bool      _animating;
        private readonly System.Windows.Forms.Timer _animTimer;
        private const int AnimStep = 16;

        // ── Performance: batch-add mode ───────────────────────────────────────
        // While true, AddTab skips CalculateTabLayout + Invalidate.  EndBatchAdd
        // does a single CalculateTabLayout + Invalidate for the whole batch.
        private bool _paintBatch;
        private int  _pendingActivateIndex = -1;

        // Tooltip
        private readonly ToolTip _tooltip = new ToolTip { AutoPopDelay = 4000, InitialDelay = 600 };
        private int _lastTooltipIndex = -1;

        // Active tab  (properties are in BeepDocumentTabStrip.Properties.cs)
        private int _activeTabIndex = -1;

        // Cached clip rect for the scrollable tab area (set in CalculateTabLayout)
        private Rectangle _tabArea = Rectangle.Empty;

        // ─────────────────────────────────────────────────────────────────────
        // Constructor  (properties / events → BeepDocumentTabStrip.Properties.cs)
        // ─────────────────────────────────────────────────────────────────────

        public BeepDocumentTabStrip()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint            |
                ControlStyles.ResizeRedraw         |
                ControlStyles.Selectable,
                true);

            TabStop = true;

            // Timers are always safe to create pre-handle
            _animTimer      = new System.Windows.Forms.Timer { Interval = AnimStep };
            _animTimer.Tick += OnAnimTimerTick;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Handle created — deferred theme init (like BeepButton/BaseControl)
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Load theme now that we have a real window handle
            try
            {
                _theme     = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
                             ?? BeepThemesManager.GetDefaultTheme();
                _themeName = BeepThemesManager.CurrentThemeName ?? string.Empty;
            }
            catch { /* stay null */ }

            if (_theme == null)
                try { _theme = BeepThemesManager.GetDefaultTheme(); } catch { }

            // Subscribe to future theme changes (once, runtime only)
            BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;  // guard re-entry
            BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;

            // Now that DPI is available, size ourselves correctly
            Height = TabHeight;  // Sprint 18.3: density-aware
            ApplyDensityFont();
            ApplyThemeColors();

            // Apply high-contrast overrides now that the handle and DPI are ready
            ApplyHighContrastTheme();

            // Sprint 18.1: register for native WM_TOUCH messages
            RegisterTouchInput();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Theme helpers
        // ─────────────────────────────────────────────────────────────────────

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
            => ThemeName = e.NewThemeName;

        private void ApplyThemeColors()
        {
            if (_theme == null) return;
            BackColor = _theme.PanelBackColor;
            ForeColor = _theme.ForeColor;
        }

        /// <summary>
        /// Applies a font whose point size matches the current <see cref="TabDensity"/> setting.
        /// Called during handle creation and whenever <c>TabDensity</c> changes.
        /// </summary>
        private void ApplyDensityFont()
        {
            float pt = _tabDensity switch
            {
                TabDensityMode.Compact => 11f,
                TabDensityMode.Dense   => 10f,
                _                      => 12f    // Comfortable
            };

            var existing = Font;
            if (Math.Abs(existing.SizeInPoints - pt) < 0.01f) return;  // already correct

            Font = new System.Drawing.Font(
                existing.FontFamily, pt, existing.Style, System.Drawing.GraphicsUnit.Point);
        }



        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            CalculateTabLayout();
            Invalidate();
        }

        private void OnAnimTimerTick(object? sender, EventArgs e)
        {
            if (!_animating) { _animTimer.Stop(); return; }
            int lx = Lerp(_indicatorCurrent.X, _indicatorTarget.X, 0.25f);
            int lw = Lerp(_indicatorCurrent.Width, _indicatorTarget.Width, 0.25f);
            _indicatorCurrent = new Rectangle(lx, _indicatorCurrent.Y, lw, _indicatorCurrent.Height);
            if (Math.Abs(_indicatorCurrent.X - _indicatorTarget.X) < 2 &&
                Math.Abs(_indicatorCurrent.Width - _indicatorTarget.Width) < 2)
            {
                _indicatorCurrent = _indicatorTarget;
                _animating = false;
                _animTimer.Stop();
            }
            if (!_tabArea.IsEmpty) Invalidate(_tabArea); else Invalidate();
        }

        private static int Lerp(int from, int to, float t) =>
            from + (int)((to - from) * t);

        // ─────────────────────────────────────────────────────────────────────
        // Dispose
        // ─────────────────────────────────────────────────────────────────────

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                _animTimer?.Dispose();
                _tabAnimTimer?.Dispose();
                _tooltip?.Dispose();
                _tooltipHoverTimer?.Dispose();
                _richTooltipPopup?.Dispose();
                _badgeAnimTimer?.Dispose();
                _longPressTimer?.Dispose();   // Sprint 18.1

                if (_dragFloatGhost != null)
                {
                    _dragFloatGhost.Close();
                    _dragFloatGhost.Dispose();
                    _dragFloatGhost = null;
                }

                // Dispose any cached tab icons loaded from file
                foreach (var img in _iconCache.Values) img?.Dispose();
                _iconCache.Clear();
            }
            base.Dispose(disposing);
        }
    }
}
