using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for Windows11 Mica Style
    /// </summary>
    public static class MicaBackgroundPainter
    {
        /// <summary>
        /// Paint Windows11 Mica background
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);

            var bounds = Rectangle.Truncate(path.GetBounds());

            // Create a unique signature using color and bounds so we can cache rendered mica bitmaps
            string signature = $"mica_{bgColor.ToArgb()}_{bounds.Width}x{bounds.Height}";

            // Generate or fetch a rasterized bitmap for the mica effect
            var bmp = PaintersFactory.GetOrCreateRaster(signature, () => GenerateMicaBitmap(bounds.Size, bgColor));

            // Draw the cached raster to the target graphics clipped by the path
            using (var texture = new TextureBrush(bmp, WrapMode.Clamp))
            {
                g.FillPath(texture, path);
            }

            // Very subtle gradient for depth - use cached gradient brush
            RectangleF fBounds = path.GetBounds();
            Color topTint = Color.FromArgb(8, 255, 255, 255);
            Color bottomTint = Color.FromArgb(4, 0, 0, 0);
            var gradient = PaintersFactory.GetLinearGradientBrush(fBounds, topTint, bottomTint, LinearGradientMode.Vertical);
            g.FillPath(gradient, path);
        }

        private static Bitmap GenerateMicaBitmap(Size size, Color baseColor)
        {
            // Simplified mica generator: subtle noise + tint
            var bmp = new Bitmap(Math.Max(1, size.Width), Math.Max(1, size.Height));
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(baseColor);
                var rand = new Random(12345); // deterministic noise
                using (var pen = new Pen(Color.FromArgb(10, Color.White)))
                {
                    for (int i = 0; i < (size.Width * size.Height) / 500; i++)
                    {
                        int x = rand.Next(size.Width);
                        int y = rand.Next(size.Height);
                        g.DrawEllipse(pen, x, y, 1, 1);
                    }
                }

                // Apply a radial soft light vignette for subtle depth
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, size.Width, size.Height), Color.FromArgb(10, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, 0, 0, size.Width, size.Height);
                }
            }
            return bmp;
        }

        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}
