using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// A transparent, always-on-top overlay form that shows five docking guide diamonds
    /// (Left / Right / Top / Bottom / Fill) while the user is dragging a floating panel.
    /// The hovered diamond is highlighted; the active target is returned via <see cref="HitTest"/>.
    ///
    /// Design notes (follows DockPanelSuite DockPanel.DockDragHandler.DockIndicator pattern):
    /// - FormBorderStyle = None, transparent = true, TopMost = true
    /// - Five guide diamonds: Centre-Left, Centre-Right, Centre-Top, Centre-Bottom, Centre-Fill
    /// - Each diamond is 32×32 px, arranged in a plus (+) shape
    /// - Hovered diamond drawn with accent highlight
    ///
    /// Reference files:
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.DockDragHandler.cs
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.DragHandler.cs
    ///   Krypton.Docking\DockingDragManager.cs (concept)
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class DockingGuideOverlay : Form
    {
        // ── Diamond geometry (mirrors DockPanelSuite PaneIndicator layout) ──
        private const int DiamondSize   = 32;
        private const int DiamondSpacing = 36;  // centre-to-centre distance

        // ── Fields ──────────────────────────────────────────────────────────
        private Rectangle _leftRect;
        private Rectangle _rightRect;
        private Rectangle _topRect;
        private Rectangle _bottomRect;
        private Rectangle _fillRect;
        private DockPosition? _hoveredTarget;

        // ── Constructor ──────────────────────────────────────────────────────

        public DockingGuideOverlay()
        {
            FormBorderStyle     = FormBorderStyle.None;
            ShowInTaskbar       = false;
            TopMost             = true;
            BackColor           = Color.Lime;          // TransparencyKey colour — never drawn
            TransparencyKey     = Color.Lime;
            AllowTransparency   = true;
            StartPosition       = FormStartPosition.Manual;
            Size                = new Size(DiamondSpacing * 3, DiamondSpacing * 3);

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint, true);

            LayoutDiamonds();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// The guide target currently under the mouse, or null if not over any diamond.
        /// Updated by <see cref="HitTest"/>.
        /// </summary>
        public DockPosition? ActiveTarget => _hoveredTarget;

        /// <summary>
        /// Centres the overlay on the given host-form client area and shows it.
        /// </summary>
        public void ShowOver(Form hostForm)
        {
            if (hostForm == null) return;
            var centre = hostForm.RectangleToScreen(
                new Rectangle(hostForm.ClientRectangle.Width  / 2 - Width  / 2,
                              hostForm.ClientRectangle.Height / 2 - Height / 2, Width, Height));
            Location = centre.Location;
            if (!Visible) Show(hostForm);
        }

        /// <summary>
        /// Tests whether the given screen point is over a guide diamond.
        /// Updates <see cref="ActiveTarget"/> and repaints the overlay.
        /// </summary>
        /// <param name="screenPt">Cursor position in screen coordinates.</param>
        /// <returns>The hovered <see cref="DockPosition"/>, or null.</returns>
        public DockPosition? HitTest(Point screenPt)
        {
            var clientPt = PointToClient(screenPt);
            DockPosition? hit = null;

            if      (_leftRect.Contains(clientPt))   hit = DockPosition.Left;
            else if (_rightRect.Contains(clientPt))  hit = DockPosition.Right;
            else if (_topRect.Contains(clientPt))    hit = DockPosition.Top;
            else if (_bottomRect.Contains(clientPt)) hit = DockPosition.Bottom;
            else if (_fillRect.Contains(clientPt))   hit = DockPosition.Fill;

            if (hit != _hoveredTarget)
            {
                _hoveredTarget = hit;
                Invalidate();
            }
            return hit;
        }

        // ── Layout ───────────────────────────────────────────────────────────

        private void LayoutDiamonds()
        {
            int cx = Width  / 2 - DiamondSize / 2;
            int cy = Height / 2 - DiamondSize / 2;

            _fillRect   = new Rectangle(cx, cy, DiamondSize, DiamondSize);
            _leftRect   = new Rectangle(cx - DiamondSpacing, cy, DiamondSize, DiamondSize);
            _rightRect  = new Rectangle(cx + DiamondSpacing, cy, DiamondSize, DiamondSize);
            _topRect    = new Rectangle(cx, cy - DiamondSpacing, DiamondSize, DiamondSize);
            _bottomRect = new Rectangle(cx, cy + DiamondSpacing, DiamondSize, DiamondSize);
        }

        // ── Painting ─────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            // Background is transparent via TransparencyKey — only paint the diamonds
            DrawDiamond(e.Graphics, _leftRect,   DockPosition.Left,   "◄");
            DrawDiamond(e.Graphics, _rightRect,  DockPosition.Right,  "►");
            DrawDiamond(e.Graphics, _topRect,    DockPosition.Top,    "▲");
            DrawDiamond(e.Graphics, _bottomRect, DockPosition.Bottom, "▼");
            DrawDiamond(e.Graphics, _fillRect,   DockPosition.Fill,   "▪");
        }

        private void DrawDiamond(Graphics g, Rectangle r, DockPosition target, string arrow)
        {
            bool isHot = (_hoveredTarget == target);

            Color back   = isHot ? Color.FromArgb(0, 122, 204) : Color.FromArgb(45, 45, 48);
            Color border = isHot ? Color.White                  : Color.FromArgb(100, 100, 100);
            Color fore   = Color.White;

            // Diamond shape path (rotated square)
            var path = DiamondPath(r);

            using (var fill = new SolidBrush(back))
                g.FillPath(fill, path);
            using (var pen = new Pen(border, 1.5f))
                g.DrawPath(pen, path);

            using (var font  = new Font("Wingdings 3", 9f))
            using (var brush = new SolidBrush(fore))
            {
                var sf = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(arrow, SystemFonts.SmallCaptionFont, brush, r, sf);
            }
        }

        private static GraphicsPath DiamondPath(Rectangle r)
        {
            int mx = r.X + r.Width  / 2;
            int my = r.Y + r.Height / 2;
            var path = new GraphicsPath();
            path.AddPolygon(new[]
            {
                new Point(mx,         r.Top),
                new Point(r.Right,    my),
                new Point(mx,         r.Bottom),
                new Point(r.Left,     my)
            });
            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutDiamonds();
        }
    }
}
