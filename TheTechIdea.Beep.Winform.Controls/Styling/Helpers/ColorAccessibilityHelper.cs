using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Helpers
{
    /// <summary>
    /// Helper class for WCAG accessibility compliance
    /// Provides contrast ratio calculation and automatic color adjustments
    /// </summary>
    public static class ColorAccessibilityHelper
    {
        /// <summary>
        /// WCAG AA contrast ratio for normal text (4.5:1)
        /// </summary>
        public const float WCAG_AA_Normal = 4.5f;

        /// <summary>
        /// WCAG AA contrast ratio for large text (3:1)
        /// </summary>
        public const float WCAG_AA_Large = 3.0f;

        /// <summary>
        /// WCAG AAA contrast ratio for normal text (7:1)
        /// </summary>
        public const float WCAG_AAA_Normal = 7.0f;

        /// <summary>
        /// WCAG AAA contrast ratio for large text (4.5:1)
        /// </summary>
        public const float WCAG_AAA_Large = 4.5f;

        /// <summary>
        /// Calculate relative luminance of a color (WCAG formula)
        /// </summary>
        /// <param name="color">Color to calculate luminance for</param>
        /// <returns>Relative luminance value (0.0 to 1.0)</returns>
        public static double CalculateRelativeLuminance(Color color)
        {
            // Convert RGB to relative luminance
            double r = GetLinearComponent(color.R / 255.0);
            double g = GetLinearComponent(color.G / 255.0);
            double b = GetLinearComponent(color.B / 255.0);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        /// <summary>
        /// Convert color component to linear component for luminance calculation
        /// </summary>
        private static double GetLinearComponent(double component)
        {
            if (component <= 0.03928)
                return component / 12.92;
            return Math.Pow((component + 0.055) / 1.055, 2.4);
        }

        /// <summary>
        /// Calculate contrast ratio between two colors
        /// </summary>
        /// <param name="color1">First color (typically foreground)</param>
        /// <param name="color2">Second color (typically background)</param>
        /// <returns>Contrast ratio (1.0 to 21.0)</returns>
        public static float CalculateContrastRatio(Color color1, Color color2)
        {
            double lum1 = CalculateRelativeLuminance(color1);
            double lum2 = CalculateRelativeLuminance(color2);

            double lighter = Math.Max(lum1, lum2);
            double darker = Math.Min(lum1, lum2);

            if (darker == 0) return 21.0f; // Maximum contrast

            return (float)((lighter + 0.05) / (darker + 0.05));
        }

        /// <summary>
        /// Ensure contrast ratio meets minimum requirement, adjusting color if needed
        /// </summary>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        /// <param name="minRatio">Minimum contrast ratio (default 4.5 for WCAG AA)</param>
        /// <returns>Adjusted foreground color that meets contrast requirement</returns>
        public static Color EnsureContrastRatio(Color foreground, Color background, float minRatio = WCAG_AA_Normal)
        {
            float currentRatio = CalculateContrastRatio(foreground, background);
            if (currentRatio >= minRatio)
            {
                return foreground; // Already meets requirement
            }

            // Determine if we need to lighten or darken
            double foreLum = CalculateRelativeLuminance(foreground);
            double backLum = CalculateRelativeLuminance(background);

            // If background is darker, lighten foreground; if lighter, darken foreground
            bool shouldLighten = backLum < foreLum;

            // Adjust color to meet contrast ratio
            return AdjustColorForContrast(foreground, background, minRatio, shouldLighten);
        }

        /// <summary>
        /// Adjust color to meet minimum contrast ratio
        /// </summary>
        private static Color AdjustColorForContrast(Color color, Color background, float minRatio, bool lighten)
        {
            Color adjusted = color;
            float currentRatio = CalculateContrastRatio(adjusted, background);
            int iterations = 0;
            const int maxIterations = 50;

            // Binary search approach for efficiency
            float adjustment = 0.5f;
            float step = 0.25f;

            while (currentRatio < minRatio && iterations < maxIterations)
            {
                if (lighten)
                {
                    adjusted = LightenColor(adjusted, adjustment);
                }
                else
                {
                    adjusted = DarkenColor(adjusted, adjustment);
                }

                currentRatio = CalculateContrastRatio(adjusted, background);
                
                if (currentRatio < minRatio)
                {
                    adjustment += step;
                }
                else
                {
                    adjustment -= step;
                }

                step *= 0.5f;
                iterations++;
            }

            return adjusted;
        }

        /// <summary>
        /// Lighten a color using HSL for more natural results
        /// </summary>
        public static Color LightenColor(Color color, float amount)
        {
            // Convert to HSL
            var (h, s, l) = RgbToHsl(color.R, color.G, color.B);
            
            // Increase lightness
            l = Math.Min(1.0, l + amount);
            
            // Convert back to RGB
            return HslToRgb(h, s, l, color.A);
        }

        /// <summary>
        /// Darken a color using HSL for more natural results
        /// </summary>
        public static Color DarkenColor(Color color, float amount)
        {
            // Convert to HSL
            var (h, s, l) = RgbToHsl(color.R, color.G, color.B);
            
            // Decrease lightness
            l = Math.Max(0.0, l - amount);
            
            // Convert back to RGB
            return HslToRgb(h, s, l, color.A);
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
        /// Check if contrast ratio meets WCAG AA standard
        /// </summary>
        public static bool MeetsWCAGAA(Color foreground, Color background, bool largeText = false)
        {
            float minRatio = largeText ? WCAG_AA_Large : WCAG_AA_Normal;
            return CalculateContrastRatio(foreground, background) >= minRatio;
        }

        /// <summary>
        /// Check if contrast ratio meets WCAG AAA standard
        /// </summary>
        public static bool MeetsWCAGAAA(Color foreground, Color background, bool largeText = false)
        {
            float minRatio = largeText ? WCAG_AAA_Large : WCAG_AAA_Normal;
            return CalculateContrastRatio(foreground, background) >= minRatio;
        }
    }
}
