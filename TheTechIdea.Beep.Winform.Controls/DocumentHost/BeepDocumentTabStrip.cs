using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Helpers;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    [ToolboxItem(true)]
    [Description("Beep themed tab strip — use inside BeepDocumentHost.")]
    public partial class BeepDocumentTabStrip : BaseControl
    {
        private readonly List<BeepDocumentTab> _tabs = new List<BeepDocumentTab>();

        private int  _scrollOffset;
        private bool _showScrollButtons;
        private int  _totalTabsWidth;

        private int  _hoverTabIndex = -1;
        private bool _hoverClose;
        private bool _hoverAdd;
        private bool _hoverScrollLeft;
        private bool _hoverScrollRight;

        private bool  _dragging;
        private int   _dragStartTab = -1;
        private Point _dragStartPoint;

        // Smooth drag-reorder (Phase 3.10):
        // While dragging, we track the "logical insert position" without performing the
        // actual reorder until mouse-up.  The insert-bar is drawn between tabs.
        private int _dragInsertIndex   = -1;   // target insertion slot (0..Count)
        private int _dragCurrentCursorX = 0;   // screen-space X of drag cursor (for insert bar)

        private bool  _dragFloating;
        private Form? _dragFloatGhost;

        private Rectangle _addButtonRect;
        private Rectangle _scrollLeftRect;
        private Rectangle _scrollRightRect;
        private Rectangle _overflowButtonRect;
        private bool      _hoverOverflow;

        private Rectangle _indicatorCurrent = Rectangle.Empty;
        private Rectangle _indicatorTarget  = Rectangle.Empty;
        private bool      _animating;
        private readonly System.Windows.Forms.Timer _animTimer;
        private const int AnimStep = 16;

        private bool _paintBatch;
        private int  _pendingActivateIndex = -1;

        private readonly ToolTip _tooltip = new ToolTip { AutoPopDelay = 4000, InitialDelay = 600 };
        private int _lastTooltipIndex = -1;

        private int _activeTabIndex = -1;
        private Rectangle _tabArea = Rectangle.Empty;

        private ITabStripPainter _tabPainter;
        private TabStripPaintContext _paintContext;
        private readonly DocumentHitTestHelper _hitTestHelper = new DocumentHitTestHelper();
        private readonly DocumentLayoutManager _layoutManager = new DocumentLayoutManager();
        private string _localThemeName = string.Empty;

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

            _animTimer      = new System.Windows.Forms.Timer { Interval = AnimStep };
            _animTimer.Tick += OnAnimTimerTick;
        }

        protected override bool IsContainerControl => false;
        protected override bool AllowBaseControlClear => false;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            try
            {
                if (_currentTheme == null)
                    _currentTheme = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
                             ?? BeepThemesManager.GetDefaultTheme();
                _localThemeName = BeepThemesManager.CurrentThemeName ?? string.Empty;
            }
            catch { }

            if (_currentTheme == null)
                try { _currentTheme = BeepThemesManager.GetDefaultTheme(); } catch { }

            BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
            BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;

            Height = TabHeight;
            ApplyDensityFont();
            ApplyThemeColors();
            ApplyHighContrastTheme();
            RegisterTouchInput();
        }

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
            => ThemeName = e.NewThemeName;

        private void ApplyThemeColors()
        {
            if (_currentTheme == null) return;
            BackColor = _currentTheme.PanelBackColor;
            ForeColor = _currentTheme.ForeColor;
        }

        private void ApplyDensityFont()
        {
            float pt = _tabDensity switch
            {
                TabDensityMode.Compact => 11f,
                TabDensityMode.Dense   => 10f,
                _                      => 12f
            };

            var existing = Font;
            if (Math.Abs(existing.SizeInPoints - pt) < 0.01f) return;

            Font = BeepFontManager.GetCachedFont(
                existing.FontFamily?.Name ?? "Segoe UI", pt, existing.Style);
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

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var painter = GetPainter();
            var ctx = GetPaintContext();

            painter.PaintStripBackground(g, ClientRectangle, ctx);

            if (_tabs.Count == 0)
            {
                painter.PaintEmptyState(g, ClientRectangle, ctx);
                if (_showAddButton && !_addButtonRect.IsEmpty)
                    painter.PaintAddButton(g, _addButtonRect, _hoverAdd, ctx);
                return;
            }

            painter.PaintSeparator(g, ClientRectangle, IsVertical, ctx);

            if (_showScrollButtons)
            {
                painter.PaintScrollButton(g, _scrollLeftRect, true, _hoverScrollLeft, IsVertical, ctx);
                painter.PaintScrollButton(g, _scrollRightRect, false, _hoverScrollRight, IsVertical, ctx);
                if (!IsVertical)
                    painter.PaintOverflowButton(g, _overflowButtonRect, _hoverOverflow, ctx);
            }

            if (_tabGroups.Count > 0 && !IsVertical)
                DrawGroupHeaders(g);

            if (!_tabArea.IsEmpty)
            {
                var clip = g.Clip;
                g.SetClip(_tabArea, CombineMode.Intersect);
                for (int i = 0; i < _tabs.Count; i++)
                {
                    var t = _tabs[i];
                    if (!string.IsNullOrEmpty(t.Group))
                    {
                        var grp = _tabGroups.Find(tg => tg.Id == t.Group);
                        if (grp != null && grp.IsCollapsed) continue;
                    }
                    if (_responsiveMode == TabResponsiveMode.ActiveOnly && !t.IsActive) continue;
                    painter.PaintTab(g, t, i, ctx);
                }
                DrawIndicator(g);
                g.Clip = clip;
            }
            else
            {
                for (int i = 0; i < _tabs.Count; i++)
                {
                    var t = _tabs[i];
                    if (!string.IsNullOrEmpty(t.Group))
                    {
                        var grp = _tabGroups.Find(tg => tg.Id == t.Group);
                        if (grp != null && grp.IsCollapsed) continue;
                    }
                    if (_responsiveMode == TabResponsiveMode.ActiveOnly && !t.IsActive) continue;
                    painter.PaintTab(g, t, i, ctx);
                }
                DrawIndicator(g);
            }

            painter.PaintFocusIndicator(g,
                _activeTabIndex >= 0 && _activeTabIndex < _tabs.Count ? _tabs[_activeTabIndex] : null,
                Focused, ctx);

            if (_showAddButton && !_addButtonRect.IsEmpty)
                painter.PaintAddButton(g, _addButtonRect, _hoverAdd, ctx);

            if (!IsVertical && _tabStyle == DocumentTabStyle.Chrome && _activeTabIndex >= 0 && _activeTabIndex < _tabs.Count)
            {
                var at = _tabs[_activeTabIndex];
                if (!at.TabRect.IsEmpty)
                {
                    Color contentBg = _currentTheme?.BackgroundColor ?? BackColor;
                    using var bridgeBr = new SolidBrush(contentBg);
                    int bLeft  = Math.Max(at.TabRect.Left  + 1, _tabArea.Left);
                    int bRight = Math.Min(at.TabRect.Right - 1, _tabArea.Right);
                    if (bRight > bLeft)
                        g.FillRectangle(bridgeBr, bLeft, Height - 1, bRight - bLeft, 1);
                }
            }

            // Drag insert-bar: drawn last so it is always on top
            if (_dragging && _dragInsertIndex >= 0 && !IsVertical)
                DrawDragInsertBar(g);

            RegisterTabHitAreas();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Drag insert-bar (Phase 3.10)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Draws a 3 px vertical accent bar at the drag-reorder insertion point,
        /// giving the user a clear preview of where the dragged tab will land.
        /// </summary>
        private void DrawDragInsertBar(Graphics g)
        {
            // Determine the X coordinate of the insert slot edge
            int barX;
            if (_dragInsertIndex <= 0)
            {
                // Insert before the first tab
                barX = _tabs.Count > 0 ? _tabs[0].TabRect.Left : _tabArea.Left;
            }
            else if (_dragInsertIndex >= _tabs.Count)
            {
                // Insert after the last tab
                barX = _tabs[_tabs.Count - 1].TabRect.Right;
            }
            else
            {
                // Insert between two tabs — split the gap
                int leftRight  = _tabs[_dragInsertIndex - 1].TabRect.Right;
                int rightLeft  = _tabs[_dragInsertIndex].TabRect.Left;
                barX = (leftRight + rightLeft) / 2;
            }

            Color accentColor = _currentTheme?.PrimaryColor ?? TabStripThemeHelpers.ThemeAwareHighlight();
            int   barH        = Height - 4;
            int   barW        = S(3);

            using var pen = new System.Drawing.Pen(accentColor, barW);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap   = System.Drawing.Drawing2D.LineCap.Round;
            g.DrawLine(pen, barX, 2, barX, barH);
        }

        private ITabStripPainter GetPainter()
        {
            if (_tabPainter == null)
                _tabPainter = TabStripPainterFactory.CreatePainter(_tabStyle);
            return _tabPainter;
        }

        private TabStripPaintContext GetPaintContext()
        {
            if (_paintContext == null)
            {
                _paintContext = new TabStripPaintContext(
                    this, _currentTheme, Font,
                    _tabStyle, _closeMode, _tabColorMode,
                    _tabDensity, _responsiveMode,
                    _hoverTabIndex, _hoverClose,
                    IsVertical, TabRadius);
            }
            return _paintContext;
        }

        private void RegisterTabHitAreas()
        {
            _hitTestHelper.ClearHitAreas();
            for (int i = 0; i < _tabs.Count; i++)
            {
                var tab = _tabs[i];
                if (!tab.TabRect.IsEmpty)
                    _hitTestHelper.RegisterHitArea($"tab_{i}", tab.TabRect, tab);
                if (!tab.CloseRect.IsEmpty)
                    _hitTestHelper.RegisterHitArea($"close_{i}", tab.CloseRect);
            }
            if (!_addButtonRect.IsEmpty)
                _hitTestHelper.RegisterHitArea("addButton", _addButtonRect);
            if (!_scrollLeftRect.IsEmpty)
                _hitTestHelper.RegisterHitArea("scrollLeft", _scrollLeftRect);
            if (!_scrollRightRect.IsEmpty)
                _hitTestHelper.RegisterHitArea("scrollRight", _scrollRightRect);
            if (!_overflowButtonRect.IsEmpty)
                _hitTestHelper.RegisterHitArea("overflowButton", _overflowButtonRect);
        }

        public DocumentHost.Helpers.HitTestResult HitTestTab(Point pt) => _hitTestHelper.HitTest(pt);

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null) return;

            ApplyDensityFont();
            BackColor = _currentTheme.PanelBackColor;
            ForeColor = _currentTheme.ForeColor;

            _paintContext = null;
            _tabPainter = null;

            Invalidate();
        }

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
                _longPressTimer?.Dispose();

                if (_dragFloatGhost != null)
                {
                    _dragFloatGhost.Close();
                    _dragFloatGhost.Dispose();
                    _dragFloatGhost = null;
                }

                foreach (var img in _iconCache.Values) img?.Dispose();
                _iconCache.Clear();

                _tabPainter = null;
                _paintContext = null;
                _hitTestHelper.ClearHitAreas();
            }
            base.Dispose(disposing);
        }
    }

    internal static class TabStripThemeHelpers
    {
        internal static Color ThemeAwareHighlight()
        {
            return SystemInformation.HighContrast ? SystemColors.Highlight : Color.FromArgb(0, 120, 215);
        }

        internal static Color ThemeAwareControlDark()
        {
            return SystemInformation.HighContrast ? SystemColors.ControlDark : Color.FromArgb(70, 70, 75);
        }
    }
}
