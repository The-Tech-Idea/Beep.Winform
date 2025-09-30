using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    [Obsolete("ControlPaintHelper is deprecated. Use IBaseControlPainter implementations for layout and drawing. This helper remains only for shared utilities (rounded paths, gradients) during transition.")]
    internal static class ControlPaintHelper
    {
        // Removed instance state. Only utilities remain.

        // Modern Gradient Methods (utility overloads)
        public static void DrawSubtleGradient(Graphics g, Rectangle rect, Color baseColor, float angleDegrees = 0f)
        {
            Color color1 = baseColor;
            float brightness = baseColor.GetBrightness();
            const float subtleFactor = 0.05f;

            Color color2 = brightness > 0.5f
                ? Color.FromArgb(Math.Max(0, baseColor.R - (int)(255 * subtleFactor)), Math.Max(0, baseColor.G - (int)(255 * subtleFactor)), Math.Max(0, baseColor.B - (int)(255 * subtleFactor)))
                : Color.FromArgb(Math.Min(255, baseColor.R + (int)(255 * subtleFactor)), Math.Min(255, baseColor.G + (int)(255 * subtleFactor)), Math.Min(255, baseColor.B + (int)(255 * subtleFactor)));

            float angleRadians = (float)(angleDegrees * Math.PI / 180f);
            using (var gradientBrush = CreateAngledGradientBrush(rect, color1, color2, angleRadians))
            {
                var blend = new ColorBlend
                {
                    Colors = new Color[] { color1, BlendColors(color1, color2, 0.5f), color2 },
                    Positions = new float[] { 0.0f, 0.3f, 1.0f }
                };
                gradientBrush.InterpolationColors = blend;
                FillShape(g, gradientBrush, rect, 0, false);
            }
        }

        public static void DrawLinearGradient(Graphics g, Rectangle rect, Color startColor, Color endColor, LinearGradientMode direction, List<GradientStop> stops = null)
        {
            using (var gradientBrush = new LinearGradientBrush(rect, startColor, endColor, direction))
            {
                if (stops != null && stops.Count > 1)
                {
                    ApplyGradientStops(gradientBrush, stops);
                }
                FillShape(g, gradientBrush, rect, 0, false);
            }
        }

        public static void DrawRadialGradient(Graphics g, Rectangle rect, Color centerColor, Color edgeColor, PointF center, float radiusFactor = 0.7f)
        {
            using (var path = new GraphicsPath())
            {
                float radius = Math.Max(rect.Width, rect.Height) * radiusFactor;
                path.AddEllipse(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                using (var gradientBrush = new PathGradientBrush(path))
                {
                    gradientBrush.CenterColor = centerColor;
                    gradientBrush.SurroundColors = new Color[] { edgeColor };
                    gradientBrush.CenterPoint = center;
                    FillShape(g, gradientBrush, rect, 0, false);
                }
            }
        }

        public static void DrawConicGradient(Graphics g, Rectangle rect, Func<float, Color> colorByHue, float startAngleDegrees)
        {
            var center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            int segments = 36;
            for (int i = 0; i < segments; i++)
            {
                float startAngle = (i * 360f / segments) + startAngleDegrees;
                float hue = (startAngle % 360f) / 360f;
                Color segmentColor = colorByHue(hue);
                using var segmentBrush = new SolidBrush(Color.FromArgb(100, segmentColor));
                g.FillPie(segmentBrush, rect, startAngle, 360f / segments);
            }
        }

        public static void DrawMeshGradient(Graphics g, Rectangle rect, Color baseColor, int gridSize = 3)
        {
            Color[,] colorGrid = new Color[gridSize, gridSize];
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    float brightness = 0.7f + (0.3f * ((x + y) / (float)(gridSize * 2)));
                    colorGrid[x, y] = ModifyColorBrightness(baseColor, brightness);
                }
            }

            float cellWidth = rect.Width / (float)(gridSize - 1);
            float cellHeight = rect.Height / (float)(gridSize - 1);

            for (int x = 0; x < gridSize - 1; x++)
            {
                for (int y = 0; y < gridSize - 1; y++)
                {
                    var cellRect = new RectangleF(rect.X + x * cellWidth, rect.Y + y * cellHeight, cellWidth * 1.5f, cellHeight * 1.5f);
                    using var cellBrush = new LinearGradientBrush(cellRect, colorGrid[x, y], colorGrid[x + 1, y + 1], LinearGradientMode.ForwardDiagonal);
                    g.FillRectangle(cellBrush, cellRect);
                }
            }
        }

        public static void ApplyGlassmorphism(Graphics g, Rectangle rect, float opacity)
        {
            using var glassBrush = new SolidBrush(Color.FromArgb((int)(255 * opacity), Color.White));
            var random = new Random(42);
            for (int i = 0; i < rect.Width * rect.Height / 1000; i++)
            {
                int x = random.Next(rect.X, rect.X + rect.Width);
                int y = random.Next(rect.Y, rect.Y + rect.Height);
                using var noiseBrush = new SolidBrush(Color.FromArgb(random.Next(5, 15), Color.White));
                g.FillRectangle(noiseBrush, x, y, 1, 1);
            }
            FillShape(g, glassBrush, rect, 0, false);
        }

        // Helper Methods
        private static void FillShape(Graphics g, Brush brush, Rectangle rect, int radius = 0, bool rounded = false)
        {
            if (rounded && radius > 0)
            {
                using var path = GetRoundedRectPath(rect, radius);
                g.FillPath(brush, path);
            }
            else
            {
                g.FillRectangle(brush, rect);
            }
        }

        private static LinearGradientBrush CreateAngledGradientBrush(Rectangle rect, Color color1, Color color2, float angleRadians)
        {
            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            var start = new PointF(
                rect.X + rect.Width * (0.5f - cos * 0.5f),
                rect.Y + rect.Height * (0.5f - sin * 0.5f));

            var end = new PointF(
                rect.X + rect.Width * (0.5f + cos * 0.5f),
                rect.Y + rect.Height * (0.5f + sin * 0.5f));

            return new LinearGradientBrush(start, end, color1, color2);
        }

        public static Color ModifyColorBrightness(Color color, float brightness)
        {
            return Color.FromArgb(color.A, (int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
        }

        public static Color BlendColors(Color color1, Color color2, float amount)
        {
            return Color.FromArgb(
                (int)(color1.R + (color2.R - color1.R) * amount),
                (int)(color1.G + (color2.G - color1.G) * amount),
                (int)(color1.B + (color2.B - color1.B) * amount));
        }

        public static Color ColorFromHSV(float hue, float saturation, double brightness)
        {
            hue = hue * 360f;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            double v = brightness;
            double p = brightness * (1 - saturation);
            double q = brightness * (1 - f * saturation);
            double t = brightness * (1 - (1 - f) * saturation);

            switch (hi)
            {
                case 0: return Color.FromArgb(255, (int)(v * 255), (int)(t * 255), (int)(p * 255));
                case 1: return Color.FromArgb(255, (int)(q * 255), (int)(v * 255), (int)(p * 255));
                case 2: return Color.FromArgb(255, (int)(p * 255), (int)(v * 255), (int)(t * 255));
                case 3: return Color.FromArgb(255, (int)(p * 255), (int)(q * 255), (int)(v * 255));
                case 4: return Color.FromArgb(255, (int)(t * 255), (int)(p * 255), (int)(v * 255));
                default: return Color.FromArgb(255, (int)(v * 255), (int)(p * 255), (int)(q * 255));
            }
        }

        private static void ApplyGradientStops(LinearGradientBrush brush, List<GradientStop> stops)
        {
            var sortedStops = stops.OrderBy(s => s.Position).ToList();
            var blend = new ColorBlend
            {
                Colors = sortedStops.Select(s => s.Color).ToArray(),
                Positions = sortedStops.Select(s => s.Position).ToArray()
            };
            brush.InterpolationColors = blend;
        }

        public static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
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

        /// <summary>
        /// Creates a DPI-scaled font based on the control's scaling factor
        /// </summary>
        /// <param name="baseFont">Base font to scale</param>
        /// <param name="scaleFactor">DPI scale factor (1.0 = 100%, 1.25 = 125%, etc.)</param>
        /// <returns>Scaled font</returns>
        public static Font GetScaledFont(Font baseFont, float scaleFactor = 1.0f)
        {
            if (baseFont == null) return SystemFonts.DefaultFont;
            if (Math.Abs(scaleFactor - 1.0f) < 0.01f) return baseFont;

            try
            {
                float scaledSize = baseFont.Size * scaleFactor;
                // Ensure minimum readable size
                scaledSize = Math.Max(6f, scaledSize);
                return new Font(baseFont.FontFamily, scaledSize, baseFont.Style, baseFont.Unit);
            }
            catch
            {
                return baseFont; // Return original if scaling fails
            }
        }

        /// <summary>
        /// Creates a DPI-scaled font with specified size
        /// </summary>
        /// <param name="fontFamily">Font family</param>
        /// <param name="baseSize">Base font size</param>
        /// <param name="style">Font style</param>
        /// <param name="scaleFactor">DPI scale factor</param>
        /// <returns>Scaled font</returns>
        public static Font GetScaledFont(FontFamily fontFamily, float baseSize, FontStyle style, float scaleFactor = 1.0f)
        {
            try
            {
                float scaledSize = baseSize * scaleFactor;
                scaledSize = Math.Max(6f, scaledSize);
                return new Font(fontFamily, scaledSize, style);
            }
            catch
            {
                return SystemFonts.DefaultFont;
            }
        }

        /// <summary>
        /// Creates a DPI-scaled font from font family name
        /// </summary>
        /// <param name="fontFamilyName">Font family name</param>
        /// <param name="baseSize">Base font size</param>
        /// <param name="style">Font style</param>
        /// <param name="scaleFactor">DPI scale factor</param>
        /// <returns>Scaled font</returns>
        public static Font GetScaledFont(string fontFamilyName, float baseSize, FontStyle style, float scaleFactor = 1.0f)
        {
            try
            {
                float scaledSize = baseSize * scaleFactor;
                scaledSize = Math.Max(6f, scaledSize);
                return new Font(fontFamilyName, scaledSize, style);
            }
            catch
            {
                return SystemFonts.DefaultFont;
            }
        }
    }
}
