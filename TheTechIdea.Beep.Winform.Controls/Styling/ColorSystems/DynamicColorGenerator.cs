using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ColorSystems
{
    /// <summary>
    /// Dynamic color generator for Material You-style color systems
    /// Generates accessible color palettes from a base color
    /// </summary>
    public static class DynamicColorGenerator
    {
        /// <summary>
        /// Generate a color palette from a base color
        /// </summary>
        /// <param name="baseColor">Base color to generate palette from</param>
        /// <param name="isDarkMode">Whether to generate dark mode palette</param>
        /// <returns>Generated color palette</returns>
        public static ColorPalette GeneratePalette(Color baseColor, bool isDarkMode = false)
        {
            var palette = new ColorPalette
            {
                Primary = baseColor,
                PrimaryVariant = AdjustColor(baseColor, isDarkMode ? 0.15f : -0.15f),
                Secondary = GenerateComplementaryColor(baseColor),
                SecondaryVariant = AdjustColor(GenerateComplementaryColor(baseColor), isDarkMode ? 0.15f : -0.15f),
                Background = isDarkMode ? Color.FromArgb(18, 18, 18) : Color.FromArgb(255, 255, 255),
                Surface = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.FromArgb(255, 255, 255),
                Error = Color.FromArgb(186, 26, 26),
                OnPrimary = GetContrastColor(baseColor),
                OnSecondary = GetContrastColor(GenerateComplementaryColor(baseColor)),
                OnBackground = isDarkMode ? Color.White : Color.Black,
                OnSurface = isDarkMode ? Color.White : Color.Black,
                OnError = Color.White
            };

            // Generate tonal variants
            palette.Tonal50 = GenerateTonalColor(baseColor, 0.5f);
            palette.Tonal100 = GenerateTonalColor(baseColor, 0.4f);
            palette.Tonal200 = GenerateTonalColor(baseColor, 0.3f);
            palette.Tonal300 = GenerateTonalColor(baseColor, 0.2f);
            palette.Tonal400 = GenerateTonalColor(baseColor, 0.1f);
            palette.Tonal500 = baseColor;
            palette.Tonal600 = GenerateTonalColor(baseColor, -0.1f);
            palette.Tonal700 = GenerateTonalColor(baseColor, -0.2f);
            palette.Tonal800 = GenerateTonalColor(baseColor, -0.3f);
            palette.Tonal900 = GenerateTonalColor(baseColor, -0.4f);

            return palette;
        }

        /// <summary>
        /// Adjust color lightness (positive = lighter, negative = darker)
        /// </summary>
        private static Color AdjustColor(Color color, float adjustment)
        {
            var (h, s, l) = RgbToHsl(color.R, color.G, color.B);
            l = Math.Max(0, Math.Min(1, l + adjustment));
            return HslToRgb(h, s, l, color.A);
        }

        /// <summary>
        /// Generate complementary color
        /// </summary>
        private static Color GenerateComplementaryColor(Color color)
        {
            var (h, s, l) = RgbToHsl(color.R, color.G, color.B);
            h = (h + 0.5) % 1.0; // Shift hue by 180 degrees
            return HslToRgb(h, s, l, color.A);
        }

        /// <summary>
        /// Generate tonal color variant
        /// </summary>
        private static Color GenerateTonalColor(Color color, float lightnessAdjustment)
        {
            var (h, s, l) = RgbToHsl(color.R, color.G, color.B);
            l = Math.Max(0, Math.Min(1, l + lightnessAdjustment));
            return HslToRgb(h, s, l, color.A);
        }

        /// <summary>
        /// Get contrasting color (black or white) for text on colored background
        /// </summary>
        private static Color GetContrastColor(Color backgroundColor)
        {
            // Calculate relative luminance
            double luminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255.0;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Convert RGB to HSL
        /// </summary>
        private static (double h, double s, double l) RgbToHsl(int r, int g, int b)
        {
            double rNorm = r / 255.0;
            double gNorm = g / 255.0;
            double bNorm = b / 255.0;

            double max = Math.Max(Math.Max(rNorm, gNorm), bNorm);
            double min = Math.Min(Math.Min(rNorm, gNorm), bNorm);
            double delta = max - min;

            double l = (max + min) / 2.0;
            double s = 0.0;
            double h = 0.0;

            if (delta != 0)
            {
                s = l < 0.5 ? delta / (max + min) : delta / (2.0 - max - min);

                if (rNorm == max)
                    h = ((gNorm - bNorm) / delta) % 6;
                else if (gNorm == max)
                    h = (bNorm - rNorm) / delta + 2;
                else
                    h = (rNorm - gNorm) / delta + 4;

                h /= 6.0;
            }

            return (h, s, l);
        }

        /// <summary>
        /// Convert HSL to RGB
        /// </summary>
        private static Color HslToRgb(double h, double s, double l, int alpha)
        {
            double r, g, b;

            if (s == 0)
            {
                r = g = b = l; // Achromatic
            }
            else
            {
                double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                double p = 2 * l - q;

                r = HueToRgb(p, q, h + 1.0 / 3.0);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0 / 3.0);
            }

            return Color.FromArgb(
                alpha,
                (int)Math.Round(r * 255),
                (int)Math.Round(g * 255),
                (int)Math.Round(b * 255)
            );
        }

        /// <summary>
        /// Helper for HSL to RGB conversion
        /// </summary>
        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
            return p;
        }

        /// <summary>
        /// Adapt color for light/dark mode
        /// </summary>
        public static Color AdaptForMode(Color color, bool isDarkMode)
        {
            var (h, s, l) = RgbToHsl(color.R, color.G, color.B);
            
            if (isDarkMode)
            {
                // Dark mode: increase lightness slightly, reduce saturation
                l = Math.Min(0.9, l + 0.1f);
                s = Math.Max(0.1f, s - 0.1f);
            }
            else
            {
                // Light mode: decrease lightness slightly
                l = Math.Max(0.1f, l - 0.05f);
            }
            
            return HslToRgb(h, s, l, color.A);
        }
    }

    /// <summary>
    /// Color palette structure
    /// </summary>
    public class ColorPalette
    {
        public Color Primary { get; set; }
        public Color PrimaryVariant { get; set; }
        public Color Secondary { get; set; }
        public Color SecondaryVariant { get; set; }
        public Color Background { get; set; }
        public Color Surface { get; set; }
        public Color Error { get; set; }
        public Color OnPrimary { get; set; }
        public Color OnSecondary { get; set; }
        public Color OnBackground { get; set; }
        public Color OnSurface { get; set; }
        public Color OnError { get; set; }
        
        // Tonal colors (Material You style)
        public Color Tonal50 { get; set; }
        public Color Tonal100 { get; set; }
        public Color Tonal200 { get; set; }
        public Color Tonal300 { get; set; }
        public Color Tonal400 { get; set; }
        public Color Tonal500 { get; set; }
        public Color Tonal600 { get; set; }
        public Color Tonal700 { get; set; }
        public Color Tonal800 { get; set; }
        public Color Tonal900 { get; set; }
    }
}
