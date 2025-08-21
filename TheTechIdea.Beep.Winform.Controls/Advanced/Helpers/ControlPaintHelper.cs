using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Advanced.Helpers
{
    internal class ControlPaintHelper
    {
        private readonly Control _owner;
        private BeepControlAdvanced OwnerAdv => _owner as BeepControlAdvanced;

        public ControlPaintHelper(Control owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            UpdateRects();
        }

        // Appearance state held in helper to keep owner light
        [Browsable(true)] public bool ShowAllBorders { get; set; } = false;
        [Browsable(true)] public int BorderThickness { get; set; } = 1;
        [Browsable(true)] public int BorderRadius { get; set; } = 8;
        [Browsable(true)] public bool IsRounded { get; set; } = true;

        [Browsable(true)] public bool ShowShadow { get; set; } = false;
        [Browsable(true)] public Color ShadowColor { get; set; } = Color.Black;
        [Browsable(true)] public float ShadowOpacity { get; set; } = 0.25f; // 0..1
        [Browsable(true)] public int ShadowOffset { get; set; } = 3;

        public Rectangle DrawingRect { get; private set; }

        public void UpdateRects()
        {
            int shadow = ShowShadow ? ShadowOffset : 0;
            int border = ShowAllBorders ? BorderThickness : 0;
            var padding = _owner.Padding;
            var w = Math.Max(0, _owner.Width - (shadow * 2 + border * 2 + padding.Left + padding.Right));
            var h = Math.Max(0, _owner.Height - (shadow * 2 + border * 2 + padding.Top + padding.Bottom));

            DrawingRect = new Rectangle(
                shadow + border + padding.Left,
                shadow + border + padding.Top,
                w,
                h);
        }

        public void Draw(Graphics g)
        {
            if (g == null) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Background
            using (var bg = new SolidBrush(_owner.BackColor))
            {
                if (IsRounded)
                {
                    using var path = GetRoundedRectPath(DrawingRect, BorderRadius);
                    g.FillPath(bg, path);
                }
                else
                {
                    g.FillRectangle(bg, DrawingRect);
                }
            }

            // Shadow
            if (ShowShadow && ShadowOpacity > 0)
            {
                var rect = new Rectangle(DrawingRect.X + ShadowOffset, DrawingRect.Y + ShadowOffset, DrawingRect.Width, DrawingRect.Height);
                var alpha = (int)(255 * Math.Clamp(ShadowOpacity, 0f, 1f));
                using var shadow = new SolidBrush(Color.FromArgb(alpha, ShadowColor));
                if (IsRounded)
                {
                    using var path = GetRoundedRectPath(rect, BorderRadius);
                    g.FillPath(shadow, path);
                }
                else
                {
                    g.FillRectangle(shadow, rect);
                }
            }

            // Border
            if (ShowAllBorders && BorderThickness > 0)
            {
                var color = OwnerAdv?.BorderColor ?? (_owner as Control)?.ForeColor ?? Color.Black;
                using var pen = new Pen(color, BorderThickness) { Alignment = PenAlignment.Inset };
                var borderRect = new Rectangle(DrawingRect.X, DrawingRect.Y, DrawingRect.Width, DrawingRect.Height);
                if (IsRounded)
                {
                    using var path = GetRoundedRectPath(borderRect, BorderRadius);
                    g.DrawPath(pen, path);
                }
                else
                {
                    g.DrawRectangle(pen, borderRect);
                }
            }
        }

        internal static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Min(Math.Min(radius * 2, rect.Width), rect.Height);
            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
