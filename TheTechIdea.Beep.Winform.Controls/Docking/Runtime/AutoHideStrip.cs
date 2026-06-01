using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// An edge tab-strip that shows clickable tab buttons for auto-hidden panels.
    /// One instance lives on each edge (Left / Right / Top / Bottom) of the host form.
    /// Clicking a tab triggers the associated <see cref="AutoHideSlidePanel"/> to slide out.
    ///
    /// Design notes (follows DockPanelSuite AutoHideStripBase + Krypton KryptonDockingEdgeAutoHidden):
    /// - Control (not Panel) so painting is fully custom — same as AutoHideStripBase.
    /// - Tabs drawn horizontally (Top/Bottom edge) or vertically (Left/Right edge).
    /// - Tab height / width: 22 px (same as DockPanelSuite theme measures).
    /// - Active panel tab is highlighted with the accent colour.
    ///
    /// Reference files:
    ///   dockpanelsuite-master\WinFormsUI\Docking\AutoHideStripBase.cs
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.AutoHideWindow.cs
    ///   Krypton.Docking\Control Docking\KryptonDockingEdgeAutoHidden.cs (concept only)
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class AutoHideStrip : Control
    {
        // ── Constants (mirrors DockPanelSuite theme measures) ──────────────
        internal const int TabSize = 22;   // thickness of the strip
        private const int HidePollMs = 150;   // slide-back poll interval
        private const int HideGraceTicks = 2; // cursor must be away ~300 ms before collapse

        private static Font StripFont => BeepFontManager.StatusBarFont;

        // ── Fields ─────────────────────────────────────────────────────────
        private readonly DockPosition _edge;
        private readonly List<DockPanel> _panels = new List<DockPanel>();
        private readonly AutoHideStripLayoutManager _layout = new AutoHideStripLayoutManager();
        private DockPanel _activePanel;   // tab that is currently selected (slide shown)
        private AutoHideSlidePanel _slidePanel;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;
        private readonly Timer _hideTimer;
        private int _awayTicks;
        private readonly ToolTip _tabToolTip = new ToolTip();

        /// <summary>Control style driving strip rendering. Set by the manager (used once StripRenderer lands).</summary>
        internal BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        /// <summary>
        /// Raised when the user clicks a strip tab to restore (unpin) the panel back to a docked
        /// group. The manager re-docks the panel and removes it from this strip.
        /// </summary>
        public event EventHandler<DockPanel> PanelRestoreRequested;

        /// <summary>Raised when the slide panel expands or collapses (layout inset may change).</summary>
        internal event EventHandler SlideLayoutChanged;

        /// <summary>Raised when the user drags the slide panel separator to resize it.</summary>
        internal event EventHandler<SeparatorResizeEventArgs> SlideSeparatorResized;

        private void NotifySlideLayoutChanged() => SlideLayoutChanged?.Invoke(this, EventArgs.Empty);

        // ── Constructor ─────────────────────────────────────────────────────

        /// <summary>
        /// Creates an AutoHideStrip anchored to the specified edge.
        /// </summary>
        /// <param name="edge">The edge this strip sits on (Left / Right / Top / Bottom).</param>
        /// <param name="hostForm">The form whose auto-hide state this strip manages.</param>
        public AutoHideStrip(DockPosition edge, Form hostForm)
        {
            _edge = edge;

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.ResizeRedraw, true);

            // Anchor strip to its edge, same thickness as DockPanelSuite TabSize
            ApplyEdgeDock();

            // Create the companion slide panel
            _slidePanel = new AutoHideSlidePanel(_edge);
            _slidePanel.SeparatorResize += (_, e) => SlideSeparatorResized?.Invoke(this, e);
            if (hostForm != null)
            {
                hostForm.Controls.Add(_slidePanel);
                hostForm.Controls.Add(this);
                BringToFront();
            }

            // Poll timer drives slide-back: collapses the peeked panel once the cursor has been
            // away from both the strip and the slide panel for HideGraceTicks ticks.
            _hideTimer = new Timer { Interval = HidePollMs };
            _hideTimer.Tick += OnHidePoll;
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>The edge this strip is anchored to.</summary>
        public DockPosition Edge => _edge;

        /// <summary>Read-only list of auto-hidden panels registered on this strip.</summary>
        public IReadOnlyList<DockPanel> Panels => _panels.AsReadOnly();

        /// <summary>The panel whose slide-out is currently visible (null when collapsed).</summary>
        public DockPanel ActivePanel => _activePanel;

        /// <summary>True while the peek slide panel is expanded.</summary>
        internal bool IsSlideVisible => _slidePanel != null && _slidePanel.Visible;

        /// <summary>Pixels the expanded slide consumes beyond the strip thickness.</summary>
        internal int SlideExtent
        {
            get
            {
                if (!IsSlideVisible)
                    return 0;

                return _edge == DockPosition.Left || _edge == DockPosition.Right
                    ? _slidePanel.Width
                    : _slidePanel.Height;
            }
        }

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.AutoHideStripBackColor;
            _slidePanel?.ApplyDockingTheme(_themeColors);
            Invalidate();
        }

        /// <summary>
        /// Registers a panel as auto-hidden on this edge.
        /// The panel is hidden from the layout and a tab button is added to the strip.
        /// </summary>
        public void AddPanel(DockPanel panel)
        {
            if (panel == null || _panels.Contains(panel)) return;
            _panels.Add(panel);
            panel.Visible = false;   // hide from docked layout
            RecalculateTabs();
            Invalidate();
        }

        /// <summary>
        /// Removes a panel from the auto-hide strip (e.g. when it is re-docked).
        /// </summary>
        public void RemovePanel(DockPanel panel)
        {
            if (panel == null) return;
            if (_activePanel == panel)
                CollapseSlide();
            _panels.Remove(panel);
            RecalculateTabs();
            Invalidate();
        }

        /// <summary>
        /// Shows (slides out) the panel associated with the given tab button.
        /// If a different panel is currently showing, it is first collapsed.
        /// </summary>
        public void ShowPanel(DockPanel panel)
        {
            if (panel == null || !_panels.Contains(panel)) return;
            if (_activePanel == panel)
            {
                CollapseSlide();
                return;
            }
            if (_activePanel != null)
                CollapseSlide();

            _activePanel = panel;
            _slidePanel.Show(panel);
            RecalculateTabs();
            Invalidate();
            NotifySlideLayoutChanged();
        }

        /// <summary>
        /// Hover-peek: slides the panel out (if not already showing) and arms the slide-back poll.
        /// Unlike <see cref="ShowPanel"/> this does not toggle when the same tab is hovered again.
        /// </summary>
        public void PeekPanel(DockPanel panel)
        {
            if (panel == null || !_panels.Contains(panel)) return;
            if (_activePanel == panel)
            {
                _awayTicks = 0;   // keep it open while interacting
                return;
            }

            if (_activePanel != null)
                _slidePanel.Hide();

            _activePanel = panel;
            _slidePanel.Show(panel);
            RecalculateTabs();
            Invalidate();
            NotifySlideLayoutChanged();

            _awayTicks = 0;
            _hideTimer.Start();
        }

        /// <summary>
        /// Collapses the currently visible slide panel.
        /// </summary>
        public void CollapseSlide()
        {
            _hideTimer.Stop();
            if (_activePanel == null) return;
            _slidePanel.Hide();
            _activePanel = null;
            RecalculateTabs();
            Invalidate();
            NotifySlideLayoutChanged();
        }

        // ── Hover-peek / slide-back ───────────────────────────────────────────

        /// <summary>True when the cursor is over the strip or the visible slide panel.</summary>
        private bool IsCursorOverStripOrSlide()
        {
            var p = Control.MousePosition;
            if (RectangleToScreen(ClientRectangle).Contains(p))
                return true;
            if (_slidePanel != null && _slidePanel.Visible && _slidePanel.Width > 0 && _slidePanel.Height > 0)
                return _slidePanel.RectangleToScreen(_slidePanel.ClientRectangle).Contains(p);
            return false;
        }

        private void OnHidePoll(object sender, EventArgs e)
        {
            if (_activePanel == null)
            {
                _hideTimer.Stop();
                return;
            }

            if (IsCursorOverStripOrSlide())
            {
                _awayTicks = 0;
                return;
            }

            if (++_awayTicks >= HideGraceTicks)
                CollapseSlide();
        }

        // ── Layout ───────────────────────────────────────────────────────────

        private void ApplyEdgeDock()
        {
            switch (_edge)
            {
                case DockPosition.Left:
                    Dock   = DockStyle.Left;
                    Width  = TabSize;
                    break;
                case DockPosition.Right:
                    Dock   = DockStyle.Right;
                    Width  = TabSize;
                    break;
                case DockPosition.Top:
                    Dock   = DockStyle.Top;
                    Height = TabSize;
                    break;
                case DockPosition.Bottom:
                    Dock   = DockStyle.Bottom;
                    Height = TabSize;
                    break;
            }
        }

        private void RecalculateTabs()
        {
            bool horizontal = (_edge == DockPosition.Top || _edge == DockPosition.Bottom);

            var models = new List<AutoHideTabModel>(_panels.Count);
            foreach (var p in _panels)
            {
                models.Add(new AutoHideTabModel
                {
                    Title = p.Title,
                    IconPath = p.IconPath,
                    IsActive = (p == _activePanel),
                    Tag = p
                });
            }

            _layout.Compute(horizontal, models, StripFont);
        }

        // ── Painting ─────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var ctx = new DockingPainterContext
            {
                Colors = _themeColors,
                Style = ControlStyle,
                Bounds = ClientRectangle
            };

            DockingPainterFactory.GetRenderers(ControlStyle).AutoHide.Paint(e.Graphics, ctx, _layout);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateTabs();
        }

        // ── Mouse — hover peeks, click restores (unpin) ───────────────────────

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var hit = _layout.HitTest(e.Location);
            if (hit?.Tag is DockPanel panel)
            {
                PeekPanel(panel);
                if (!string.IsNullOrEmpty(panel.Title))
                    _tabToolTip.Show(panel.Title, this, e.X + 12, e.Y + 12, 3000);
            }
            else
            {
                _tabToolTip.Hide(this);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            var hit = _layout.HitTest(e.Location);
            if (hit?.Tag is DockPanel panel)
                PanelRestoreRequested?.Invoke(this, panel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hideTimer?.Stop();
                _hideTimer?.Dispose();
                _slidePanel?.Dispose();
                _tabToolTip?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
