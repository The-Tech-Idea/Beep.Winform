// BeepDocumentDockOverlay.cs
// Semi-transparent docking target overlay shown when a BeepDocumentFloatWindow
// is dragged near a BeepDocumentHost.
//
// Five diamond targets are rendered: Centre (merge/re-dock), Left, Right, Top, Bottom.
// Currently only Centre is active; the edge zones are shown but disabled (grey-tinted).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    // ─────────────────────────────────────────────────────────────────────────
    // DockZone enum
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Identifies a dock drop zone on the overlay.</summary>
    public enum DockZone
    {
        /// <summary>Cursor is not over any zone.</summary>
        None,
        /// <summary>Merge / dock-back to the host as a tab.</summary>
        Centre,
        /// <summary>Split the host and dock to the left half.</summary>
        Left,
        /// <summary>Split the host and dock to the right half.</summary>
        Right,
        /// <summary>Split the host and dock to the top half.</summary>
        Top,
        /// <summary>Split the host and dock to the bottom half.</summary>
        Bottom
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Event args
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Carries the chosen <see cref="DockZone"/> when a drop occurs.</summary>
    public class DockZoneEventArgs : EventArgs
    {
        public DockZone Zone       { get; }
        public string   DocumentId { get; }

        public DockZoneEventArgs(DockZone zone, string documentId)
        {
            Zone       = zone;
            DocumentId = documentId;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Overlay form
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Borderless, semi-transparent overlay drawn on top of a <see cref="BeepDocumentHost"/>
    /// during a drag-to-dock operation.  Call <see cref="ShowOverlay"/> /
    /// <see cref="HideOverlay"/> and use <see cref="HitTest"/> to resolve the drop.
    /// </summary>
    internal sealed class BeepDocumentDockOverlay : Form
    {
        // ── Layout constants ─────────────────────────────────────────────────
        private const int DiamondR   = 28;   // logical: half-width of each diamond target
        private const int EdgeOffset = 40;   // logical: distance from edge to edge-zone centre

        // ── State ────────────────────────────────────────────────────────────
        private DockZone _highlighted = DockZone.None;

        // ── Colours ──────────────────────────────────────────────────────────
        private static readonly Color ColNormal      = Color.FromArgb(100, 55, 135, 210);   // blue
        private static readonly Color ColHighlight   = Color.FromArgb(200, 55, 135, 210);   // bright blue
        private static readonly Color ColDisabled    = Color.FromArgb(60,  140, 140, 140);  // grey
        private static readonly Color ColBorder      = Color.FromArgb(200, 255, 255, 255);

        // ─────────────────────────────────────────────────────────────────────
        // Constructor
        // ─────────────────────────────────────────────────────────────────────
        public BeepDocumentDockOverlay()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.UserPaint, true);

            FormBorderStyle     = FormBorderStyle.None;
            ShowInTaskbar       = false;
            TopMost             = true;
            StartPosition       = FormStartPosition.Manual;
            BackColor           = Color.Black;   // key colour for layered window
            TransparencyKey     = Color.Black;
            Opacity             = 1.0;           // individual shapes control their own alpha
        }

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Position the overlay over <paramref name="hostScreenBounds"/> and make it visible.
        /// </summary>
        public void ShowOverlay(Rectangle hostScreenBounds)
        {
            Bounds = hostScreenBounds;
            _highlighted = DockZone.None;
            Invalidate();
            if (!Visible) Show();
        }

        /// <summary>Hide the overlay.</summary>
        public void HideOverlay()
        {
            if (Visible) Hide();
        }

        /// <summary>
        /// Update the highlighted zone based on the current screen cursor position.
        /// Call this from the float window's MouseMove handler.
        /// </summary>
        public void UpdateHighlight(Point screenPt)
        {
            DockZone zone = HitTest(screenPt);
            if (zone == _highlighted) return;
            _highlighted = zone;
            Invalidate();
        }

        /// <summary>
        /// Returns which zone (if any) the screen point falls inside.
        /// </summary>
        public DockZone HitTest(Point screenPt)
        {
            Point local = PointToClient(screenPt);
            foreach (DockZone zone in new[] { DockZone.Centre, DockZone.Left, DockZone.Right, DockZone.Top, DockZone.Bottom })
            {
                Point centre = ZoneCentre(zone);
                int   r      = ScaleL(DiamondR);
                // Diamond bounds as a square rotated 45° — use Manhattan distance
                int   dx     = Math.Abs(local.X - centre.X);
                int   dy     = Math.Abs(local.Y - centre.Y);
                if (dx + dy <= r)
                    return zone;
            }
            return DockZone.None;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Painting
        // ─────────────────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (DockZone zone in new[] { DockZone.Centre, DockZone.Left, DockZone.Right, DockZone.Top, DockZone.Bottom })
                DrawZone(g, zone);
        }

        private void DrawZone(Graphics g, DockZone zone)
        {
            bool  isActive    = true;   // all 5 zones are active
            bool  isHighlight = zone == _highlighted;
            int   r           = ScaleL(DiamondR);
            Point c           = ZoneCentre(zone);

            // ── Semi-transparent preview rectangle when this zone is highlighted ──
            if (isHighlight && zone != DockZone.None)
            {
                var preview = ZonePreviewRect();
                using var previewBrush = new SolidBrush(Color.FromArgb(70, 55, 135, 210));
                using var previewPen   = new Pen(Color.FromArgb(200, 55, 135, 210), 2f);
                g.FillRectangle(previewBrush, preview);
                g.DrawRectangle(previewPen,   preview);
            }

            Point[] diamond =
            {
                new Point(c.X,     c.Y - r),
                new Point(c.X + r, c.Y    ),
                new Point(c.X,     c.Y + r),
                new Point(c.X - r, c.Y    ),
            };

            Color fill = isHighlight ? ColHighlight
                       : isActive    ? ColNormal
                       :               ColDisabled;

            using (var brush = new SolidBrush(fill))
                g.FillPolygon(brush, diamond);

            using (var pen = new Pen(ColBorder, 1.5f))
                g.DrawPolygon(pen, diamond);

            DrawZoneIcon(g, c, zone, isActive);
        }

        private static void DrawZoneIcon(Graphics g, Point centre, DockZone zone, bool active)
        {
            Color  iconCol = active ? Color.White : Color.FromArgb(120, 200, 200, 200);
            int    s       = 7;   // half-size of icon strokes

            using var pen = new Pen(iconCol, 2f);

            switch (zone)
            {
                case DockZone.Centre:
                    // Two overlapping tab rectangles
                    g.DrawRectangle(pen, centre.X - s, centre.Y - s / 2, s, s);
                    g.DrawRectangle(pen, centre.X,     centre.Y - s / 2, s, s);
                    break;
                case DockZone.Left:
                    g.DrawLine(pen, centre.X - s, centre.Y, centre.X + s, centre.Y);
                    g.DrawLine(pen, centre.X,     centre.Y - s, centre.X - s, centre.Y);
                    g.DrawLine(pen, centre.X,     centre.Y + s, centre.X - s, centre.Y);
                    break;
                case DockZone.Right:
                    g.DrawLine(pen, centre.X - s, centre.Y, centre.X + s, centre.Y);
                    g.DrawLine(pen, centre.X,     centre.Y - s, centre.X + s, centre.Y);
                    g.DrawLine(pen, centre.X,     centre.Y + s, centre.X + s, centre.Y);
                    break;
                case DockZone.Top:
                    g.DrawLine(pen, centre.X, centre.Y + s, centre.X, centre.Y - s);
                    g.DrawLine(pen, centre.X - s, centre.Y, centre.X, centre.Y - s);
                    g.DrawLine(pen, centre.X + s, centre.Y, centre.X, centre.Y - s);
                    break;
                case DockZone.Bottom:
                    g.DrawLine(pen, centre.X, centre.Y - s, centre.X, centre.Y + s);
                    g.DrawLine(pen, centre.X - s, centre.Y, centre.X, centre.Y + s);
                    g.DrawLine(pen, centre.X + s, centre.Y, centre.X, centre.Y + s);
                    break;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Geometry helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Returns the client-space centre of a given zone target.</summary>
        private Point ZoneCentre(DockZone zone)
        {
            int cx = Width  / 2;
            int cy = Height / 2;
            int e  = ScaleL(EdgeOffset);

            return zone switch
            {
                DockZone.Centre => new Point(cx, cy),
                DockZone.Left   => new Point(e,          cy),
                DockZone.Right  => new Point(Width  - e, cy),
                DockZone.Top    => new Point(cx,          e),
                DockZone.Bottom => new Point(cx,  Height - e),
                _               => new Point(cx, cy)
            };
        }

        /// <summary>
        /// Returns the translucent preview rectangle (in client coords) that shows
        /// where a panel will land if dropped on the currently highlighted zone.
        /// </summary>
        private Rectangle ZonePreviewRect()
        {
            int w = Width;
            int h = Height;
            int t = ScaleL(4);   // thin margin so the preview doesn't overlap the diamond

            return _highlighted switch
            {
                DockZone.Centre => new Rectangle(t,       t,       w - 2*t, h - 2*t),
                DockZone.Left   => new Rectangle(t,       t,       w/2 - t, h - 2*t),
                DockZone.Right  => new Rectangle(w/2,     t,       w/2 - t, h - 2*t),
                DockZone.Top    => new Rectangle(t,       t,       w - 2*t, h/2 - t),
                DockZone.Bottom => new Rectangle(t,       h/2,     w - 2*t, h/2 - t),
                _               => Rectangle.Empty
            };
        }

        private int ScaleL(int logical) =>
            (int)Math.Round(logical * (DeviceDpi / 96f));

        // ── No-activate ──────────────────────────────────────────────────────
        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = unchecked((int)0x08000000);
                const int WS_EX_TOOLWINDOW = 0x00000080;
                var cp   = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                return cp;
            }
        }
    }
}
