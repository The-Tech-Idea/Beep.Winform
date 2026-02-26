using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Holographic border painter - iridescent gradient outline.
    /// </summary>
    public static class HolographicBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float width = StyleBorders.GetBorderWidth(style);
            if (width <= 0f) return path;
            RectangleF bounds = path.GetBounds();

            using (var gradient = new LinearGradientBrush(bounds, Color.Magenta, Color.Cyan, LinearGradientMode.Horizontal))
            {
                gradient.InterpolationColors = new ColorBlend
                {
                    Colors = new[]
                    {
                        Color.FromArgb(180, 255, 0, 200),
                        Color.FromArgb(180, 255, 200, 0),
                        Color.FromArgb(180, 0, 255, 150),
                        Color.FromArgb(180, 100, 150, 255),
                        Color.FromArgb(180, 255, 0, 200)
                    },
                    Positions = new[] { 0f, 0.25f, 0.5f, 0.75f, 1f }
                };

                var savedPixel = g.PixelOffsetMode;
                using (var pen = new Pen(gradient, width))
                {
                    pen.LineJoin = LineJoin.Round;
                    pen.Alignment = PenAlignment.Inset;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.None;
                    g.DrawPath(pen, path);
                }
                g.PixelOffsetMode = savedPixel;
            }

            return BorderPainterHelpers.CreateStrokeInsetPath(path, width);
        }
    }
}
