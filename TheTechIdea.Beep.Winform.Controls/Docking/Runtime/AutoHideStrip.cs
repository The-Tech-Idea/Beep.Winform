using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

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
        private const int TabSize      = 22;   // thickness of the strip
        private const int TabMinLength = 60;   // minimum tab length along the strip
        private const int TabPadding   = 8;    // horizontal padding inside each tab

        // ── Fields ─────────────────────────────────────────────────────────
        private readonly DockPosition _edge;
        private readonly List<DockPanel> _panels      = new List<DockPanel>();
        private readonly List<Rectangle> _tabRects    = new List<Rectangle>();  // cached per tab
        private DockPanel _activePanel;   // tab that is currently selected (slide shown)
        private AutoHideSlidePanel _slidePanel;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

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
            if (hostForm != null)
            {
                hostForm.Controls.Add(_slidePanel);
                hostForm.Controls.Add(this);
                BringToFront();
            }
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>The edge this strip is anchored to.</summary>
        public DockPosition Edge => _edge;

        /// <summary>Read-only list of auto-hidden panels registered on this strip.</summary>
        public IReadOnlyList<DockPanel> Panels => _panels.AsReadOnly();

        /// <summary>The panel whose slide-out is currently visible (null when collapsed).</summary>
        public DockPanel ActivePanel => _activePanel;

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
            Invalidate();
        }

        /// <summary>
        /// Collapses the currently visible slide panel.
        /// </summary>
        public void CollapseSlide()
        {
            if (_activePanel == null) return;
            _slidePanel.Hide();
            _activePanel = null;
            Invalidate();
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
            _tabRects.Clear();
            bool horizontal = (_edge == DockPosition.Top || _edge == DockPosition.Bottom);
            int pos = 2;  // start offset

            foreach (var p in _panels)
            {
                int titleLen;
                using (var g = CreateGraphics())
                using (var font = new Font("Segoe UI", 8f))
                    titleLen = (int)g.MeasureString(p.Title, font).Width + TabPadding * 2;
                titleLen = Math.Max(titleLen, TabMinLength);

                Rectangle r = horizontal
                    ? new Rectangle(pos, 0, titleLen, TabSize)
                    : new Rectangle(0, pos, TabSize, titleLen);

                _tabRects.Add(r);
                pos += (horizontal ? titleLen : titleLen) + 2;
            }
        }

        // ── Painting ─────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Strip background
            using (var bg = new SolidBrush(_themeColors.AutoHideStripBackColor))
                g.FillRectangle(bg, ClientRectangle);

            bool horizontal = (_edge == DockPosition.Top || _edge == DockPosition.Bottom);

            for (int i = 0; i < _panels.Count && i < _tabRects.Count; i++)
            {
                var panel = _panels[i];
                var rect  = _tabRects[i];
                bool isActive = (panel == _activePanel);

                // Tab background
                Color tabBack = isActive
                    ? _themeColors.AutoHideActiveTabBackColor
                    : _themeColors.AutoHideTabBackColor;

                using (var brush = new SolidBrush(tabBack))
                    g.FillRectangle(brush, rect);

                // Tab border
                using (var pen = new Pen(_themeColors.TabBorderColor))
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                // Tab title — rotate 90° for Left/Right edges, straight for Top/Bottom
                using (var font  = new Font("Segoe UI", 8f))
                using (var brush = new SolidBrush(isActive ? _themeColors.ActiveTabForeColor : _themeColors.InactiveTabForeColor))
                {
                    var sf = new StringFormat
                    {
                        Alignment     = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming      = StringTrimming.EllipsisCharacter
                    };

                    if (horizontal)
                    {
                        g.DrawString(panel.Title, font, brush, rect, sf);
                    }
                    else
                    {
                        // Rotate text 90° for vertical edges (Left / Right)
                        var state = g.Save();
                        g.TranslateTransform(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
                        g.RotateTransform(-90f);
                        var rotRect = new RectangleF(-rect.Height / 2f, -rect.Width / 2f,
                                                      rect.Height, rect.Width);
                        g.DrawString(panel.Title, font, brush, rotRect, sf);
                        g.Restore(state);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateTabs();
        }

        // ── Mouse — click tab to show/hide slide panel ────────────────────────

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int i = 0; i < _tabRects.Count; i++)
            {
                if (_tabRects[i].Contains(e.Location))
                {
                    ShowPanel(_panels[i]);
                    return;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _slidePanel?.Dispose();
            base.Dispose(disposing);
        }
    }
}
