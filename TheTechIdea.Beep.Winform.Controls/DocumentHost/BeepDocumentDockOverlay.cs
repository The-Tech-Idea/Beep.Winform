// BeepDocumentDockOverlay.cs
// Semi-transparent docking target overlay shown when a BeepDocumentFloatWindow
// is dragged near a BeepDocumentHost.
//
// Five diamond targets are rendered: Centre (merge/re-dock), Left, Right, Top, Bottom.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>Identifies a dock drop zone on the overlay.</summary>
    public enum DockZone
    {
        None,
        Centre,
        Left,
        Right,
        Top,
        Bottom
    }

    /// <summary>Carries the chosen <see cref="DockZone"/> when a drop occurs.</summary>
    public class DockZoneEventArgs : EventArgs
    {
        public DockZone Zone { get; }
        public string DocumentId { get; }

        public DockZoneEventArgs(DockZone zone, string documentId)
        {
            Zone = zone;
            DocumentId = documentId;
        }
    }

    internal sealed class BeepDocumentDockOverlay : Form
    {
        private const int DiamondR = 28;
        private const int EdgeOffset = 40;

        private DockZone _highlighted = DockZone.None;
        private IBeepTheme? _currentTheme;

        // Pulse animation — 0.0 to 1.0 cycling at ~25fps
        private System.Windows.Forms.Timer? _pulseTimer;
        private float _pulsePhase;

        private Color _colNormal;
        private Color _colHighlight;
        private Color _colDisabled;
        private Color _colBorder;
        private Color _colPreviewFill;
        private Color _colPreviewBorder;

        public BeepDocumentDockOverlay()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.UserPaint, true);

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Black;
            TransparencyKey = Color.Black;
            Opacity = 1.0;
        }

        private void EnsurePulseTimer()
        {
            if (_pulseTimer != null) return;
            _pulseTimer = new System.Windows.Forms.Timer { Interval = 40 };
            _pulseTimer.Tick += (_, _) =>
            {
                _pulsePhase = (_pulsePhase + 0.06f) % 1.0f;
                Invalidate();
            };
        }

        private void StartPulse()
        {
            EnsurePulseTimer();
            if (!_pulseTimer!.Enabled) _pulseTimer.Start();
        }

        private void StopPulse()
        {
            _pulseTimer?.Stop();
            _pulsePhase = 0f;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pulseTimer?.Stop();
                _pulseTimer?.Dispose();
                _pulseTimer = null;
            }
            base.Dispose(disposing);
        }

        public void ApplyTheme(IBeepTheme theme)
        {
            _currentTheme = theme;
            _colNormal = Color.FromArgb(100, theme.PrimaryColor.R, theme.PrimaryColor.G, theme.PrimaryColor.B);
            _colHighlight = Color.FromArgb(200, theme.PrimaryColor.R, theme.PrimaryColor.G, theme.PrimaryColor.B);
            _colDisabled = Color.FromArgb(60, theme.SecondaryTextColor.R, theme.SecondaryTextColor.G, theme.SecondaryTextColor.B);
            _colBorder = Color.FromArgb(200, 255, 255, 255);
            _colPreviewFill = Color.FromArgb(70, theme.PrimaryColor.R, theme.PrimaryColor.G, theme.PrimaryColor.B);
            _colPreviewBorder = Color.FromArgb(200, theme.PrimaryColor.R, theme.PrimaryColor.G, theme.PrimaryColor.B);
            Invalidate();
        }

        public void ShowOverlay(Rectangle hostScreenBounds)
        {
            Bounds = hostScreenBounds;
            _highlighted = DockZone.None;
            Invalidate();
            if (!Visible) Show();
        }

        public void HideOverlay()
        {
            StopPulse();
            if (Visible) Hide();
        }

        public void UpdateHighlight(Point screenPt)
        {
            DockZone zone = HitTest(screenPt);
            if (zone == _highlighted) return;
            _highlighted = zone;
            if (zone != DockZone.None)
                StartPulse();
            else
                StopPulse();
            Invalidate();
        }

        public DockZone HitTest(Point screenPt)
        {
            Point local = PointToClient(screenPt);
            foreach (DockZone zone in new[] { DockZone.Centre, DockZone.Left, DockZone.Right, DockZone.Top, DockZone.Bottom })
            {
                Point centre = ZoneCentre(zone);
                int r = ScaleL(DiamondR);
                int dx = Math.Abs(local.X - centre.X);
                int dy = Math.Abs(local.Y - centre.Y);
                if (dx + dy <= r)
                    return zone;
            }
            return DockZone.None;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (DockZone zone in new[] { DockZone.Centre, DockZone.Left, DockZone.Right, DockZone.Top, DockZone.Bottom })
                DrawZone(g, zone);
        }

        private void DrawZone(Graphics g, DockZone zone)
        {
            bool isHighlight = zone == _highlighted;
            int r = ScaleL(DiamondR);
            Point c = ZoneCentre(zone);

            if (isHighlight && zone != DockZone.None)
            {
                var preview = ZonePreviewRect();
                using var previewBrush = new SolidBrush(_colPreviewFill);
                using var previewPen = new Pen(_colPreviewBorder, 2f);
                g.FillRectangle(previewBrush, preview);
                g.DrawRectangle(previewPen, preview);

                // Animated expanding/fading pulse ring
                float progress = _pulsePhase;
                int   pulseR   = r + (int)(r * 0.55f * progress);
                int   alpha    = Math.Max(0, (int)(110 * (1f - progress)));
                if (alpha > 0 && pulseR > r)
                {
                    var pCol = Color.FromArgb(alpha,
                        _colHighlight.R, _colHighlight.G, _colHighlight.B);
                    using var pulsePen = new Pen(pCol, 2.5f);
                    Point[] pulseRing =
                    {
                        new Point(c.X, c.Y - pulseR),
                        new Point(c.X + pulseR, c.Y),
                        new Point(c.X, c.Y + pulseR),
                        new Point(c.X - pulseR, c.Y),
                    };
                    g.DrawPolygon(pulsePen, pulseRing);
                }
            }

            Point[] diamond =
            {
                new Point(c.X, c.Y - r),
                new Point(c.X + r, c.Y),
                new Point(c.X, c.Y + r),
                new Point(c.X - r, c.Y),
            };

            Color fill = isHighlight ? _colHighlight : _colNormal;

            using (var brush = new SolidBrush(fill))
                g.FillPolygon(brush, diamond);

            using (var pen = new Pen(_colBorder, 1.5f))
                g.DrawPolygon(pen, diamond);

            DrawZoneIcon(g, c, zone);
        }

        private static void DrawZoneIcon(Graphics g, Point centre, DockZone zone)
        {
            Color iconCol = Color.White;
            int s = 7;

            using var pen = new Pen(iconCol, 2f);

            switch (zone)
            {
                case DockZone.Centre:
                    g.DrawRectangle(pen, centre.X - s, centre.Y - s / 2, s, s);
                    g.DrawRectangle(pen, centre.X, centre.Y - s / 2, s, s);
                    break;
                case DockZone.Left:
                    g.DrawLine(pen, centre.X - s, centre.Y, centre.X + s, centre.Y);
                    g.DrawLine(pen, centre.X, centre.Y - s, centre.X - s, centre.Y);
                    g.DrawLine(pen, centre.X, centre.Y + s, centre.X - s, centre.Y);
                    break;
                case DockZone.Right:
                    g.DrawLine(pen, centre.X - s, centre.Y, centre.X + s, centre.Y);
                    g.DrawLine(pen, centre.X, centre.Y - s, centre.X + s, centre.Y);
                    g.DrawLine(pen, centre.X, centre.Y + s, centre.X + s, centre.Y);
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

        private Point ZoneCentre(DockZone zone)
        {
            int cx = Width / 2;
            int cy = Height / 2;
            int e = ScaleL(EdgeOffset);

            return zone switch
            {
                DockZone.Centre => new Point(cx, cy),
                DockZone.Left => new Point(e, cy),
                DockZone.Right => new Point(Width - e, cy),
                DockZone.Top => new Point(cx, e),
                DockZone.Bottom => new Point(cx, Height - e),
                _ => new Point(cx, cy)
            };
        }

        private Rectangle ZonePreviewRect()
        {
            int w = Width;
            int h = Height;
            int t = ScaleL(4);

            return _highlighted switch
            {
                DockZone.Centre => new Rectangle(t, t, w - 2 * t, h - 2 * t),
                DockZone.Left => new Rectangle(t, t, w / 2 - t, h - 2 * t),
                DockZone.Right => new Rectangle(w / 2, t, w / 2 - t, h - 2 * t),
                DockZone.Top => new Rectangle(t, t, w - 2 * t, h / 2 - t),
                DockZone.Bottom => new Rectangle(t, h / 2, w - 2 * t, h / 2 - t),
                _ => Rectangle.Empty
            };
        }

        private int ScaleL(int logical) =>
            (int)Math.Round(logical * (DeviceDpi / 96f));

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = unchecked((int)0x08000000);
                const int WS_EX_TOOLWINDOW = 0x00000080;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                return cp;
            }
        }
    }
}
