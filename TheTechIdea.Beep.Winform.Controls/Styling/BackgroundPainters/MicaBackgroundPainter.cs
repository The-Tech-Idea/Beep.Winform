using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Mica background painter - Windows 11 signature material effect
    /// Subtle noise texture with desktop-tint simulation and depth gradient
    /// </summary>
    public static class MicaBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Mica base color
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Windows11Mica);

            // Apply state adjustment
            Color stateColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                baseColor, state, BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = Rectangle.Truncate(path.GetBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Create unique signature for cached mica bitmap
            string signature = $"mica_{stateColor.ToArgb()}_{bounds.Width}x{bounds.Height}";

            // Get or create cached mica texture
            var bmp = PaintersFactory.GetOrCreateRaster(signature, 
                () => GenerateMicaBitmap(bounds.Size, stateColor));

            // Draw cached mica texture clipped by path
            using (var texture = new TextureBrush(bmp, WrapMode.Clamp))
            {
                g.FillPath(texture, path);
            }

            // Subtle depth gradient overlay
            RectangleF fBounds = path.GetBounds();
            var gradient = PaintersFactory.GetLinearGradientBrush(
                fBounds,
                Color.FromArgb(6, 255, 255, 255),
                Color.FromArgb(3, 0, 0, 0),
                LinearGradientMode.Vertical);
            g.FillPath(gradient, path);
        }

        /// <summary>
        /// Legacy overload without state
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, path, style, theme, useThemeColors, ControlState.Normal);
        }

        private static Bitmap GenerateMicaBitmap(Size size, Color baseColor)
        {
            var bmp = new Bitmap(Math.Max(1, size.Width), Math.Max(1, size.Height));
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(baseColor);
                
                // Subtle deterministic noise
                var rand = new Random(12345);
                using (var pen = new Pen(Color.FromArgb(8, Color.White)))
                {
                    int noiseCount = Math.Max(10, (size.Width * size.Height) / 600);
                    for (int i = 0; i < noiseCount; i++)
                    {
                        int x = rand.Next(size.Width);
                        int y = rand.Next(size.Height);
                        g.DrawEllipse(pen, x, y, 1, 1);
                    }
                }

                // Soft light vignette for depth
                using (var brush = new LinearGradientBrush(
                    new Rectangle(0, 0, size.Width, size.Height),
                    Color.FromArgb(8, Color.White),
                    Color.FromArgb(0, Color.White),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, 0, 0, size.Width, size.Height);
                }
            }
            return bmp;
        }
    }
}
