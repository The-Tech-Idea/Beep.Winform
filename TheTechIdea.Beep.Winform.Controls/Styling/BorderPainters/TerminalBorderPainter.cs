using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Terminal border painter — sharp square 2px stroke with crisp
    /// corner bracket accents for the retro terminal / CRT look.
    /// </summary>
    public static class TerminalBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            const float borderWidth = 2f;

            Color baseColor = BorderPainterHelpers.GetColorFromStyleOrTheme(
                theme, useThemeColors, "Border", Color.FromArgb(0, 255, 0));

            Color borderColor = state switch
            {
                ControlState.Disabled => BorderPainterHelpers.WithAlpha(baseColor, 90),
                ControlState.Pressed => BorderPainterHelpers.Darken(baseColor, 0.15f),
                ControlState.Hovered => BorderPainterHelpers.Lighten(baseColor, 0.1f),
                ControlState.Selected => BorderPainterHelpers.WithAlpha(baseColor, 240),
                ControlState.Focused => BorderPainterHelpers.WithAlpha(baseColor, 255),
                _ => BorderPainterHelpers.WithAlpha(baseColor, 200)
            };

            if (isFocused && state != ControlState.Focused && state != ControlState.Disabled)
                borderColor = BorderPainterHelpers.WithAlpha(borderColor, 255);

            var oldSmooth = g.SmoothingMode;
            var oldPixel = g.PixelOffsetMode;

            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.None;

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            RectangleF bounds = path.GetBounds();
            PointF[] pts = path.PathPoints;
            if (pts.Length >= 4)
            {
                float accentLen = Math.Max(4f, Math.Min(10f,
                    Math.Min(bounds.Width, bounds.Height) / 10f));

                Color accentColor = state == ControlState.Disabled
                    ? BorderPainterHelpers.WithAlpha(borderColor, 100)
                    : BorderPainterHelpers.WithAlpha(borderColor, 255);

                using var accentPen = new Pen(accentColor, 1.5f);
                accentPen.StartCap = LineCap.Flat;
                accentPen.EndCap = LineCap.Flat;

                PointF center = new PointF(
                    bounds.X + bounds.Width / 2f,
                    bounds.Y + bounds.Height / 2f);

                DrawCornerBracket(g, accentPen, pts[0], center, accentLen);
                DrawCornerBracket(g, accentPen, pts[1], center, accentLen);
                DrawCornerBracket(g, accentPen, pts[2], center, accentLen);
                DrawCornerBracket(g, accentPen, pts[3], center, accentLen);
            }

            g.SmoothingMode = oldSmooth;
            g.PixelOffsetMode = oldPixel;

            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }

        private static void DrawCornerBracket(Graphics g, Pen pen,
            PointF cornerPt, PointF center, float len)
        {
            float dx = cornerPt.X - center.X;
            float dy = cornerPt.Y - center.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dist < 0.001f) return;

            float nx = dx / dist;
            float ny = dy / dist;

            PointF outerStart = new PointF(cornerPt.X + nx * len, cornerPt.Y + ny * len);
            PointF outerEnd = new PointF(cornerPt.X + nx * (len * 0.4f), cornerPt.Y + ny * (len * 0.4f));

            g.DrawLine(pen, outerStart, outerEnd);
        }
    }
}
