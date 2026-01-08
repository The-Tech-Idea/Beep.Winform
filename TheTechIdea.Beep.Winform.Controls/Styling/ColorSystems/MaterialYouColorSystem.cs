using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using static TheTechIdea.Beep.Winform.Controls.Styling.Helpers.ColorAccessibilityHelper;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ColorSystems
{
    /// <summary>
    /// Material You color system implementation
    /// Generates dynamic, accessible color palettes with automatic contrast adjustment
    /// </summary>
    public static class MaterialYouColorSystem
    {
        /// <summary>
        /// Generate Material You color palette with accessibility compliance
        /// </summary>
        /// <param name="seedColor">Seed color (typically from wallpaper or user preference)</param>
        /// <param name="isDarkMode">Whether to generate dark mode palette</param>
        /// <param name="ensureAccessibility">Whether to ensure WCAG AA compliance</param>
        /// <returns>Material You color palette</returns>
        public static MaterialYouPalette GenerateMaterialYouPalette(Color seedColor, bool isDarkMode = false, bool ensureAccessibility = true)
        {
            var basePalette = DynamicColorGenerator.GeneratePalette(seedColor, isDarkMode);
            
            var palette = new MaterialYouPalette
            {
                Primary = basePalette.Primary,
                OnPrimary = basePalette.OnPrimary,
                PrimaryContainer = basePalette.Tonal200,
                OnPrimaryContainer = basePalette.Tonal800,
                Secondary = basePalette.Secondary,
                OnSecondary = basePalette.OnSecondary,
                SecondaryContainer = GenerateTonalVariant(basePalette.Secondary, 0.2f),
                OnSecondaryContainer = GenerateTonalVariant(basePalette.Secondary, -0.2f),
                Tertiary = GenerateTertiaryColor(seedColor),
                OnTertiary = GetContrastColor(GenerateTertiaryColor(seedColor)),
                Error = basePalette.Error,
                OnError = basePalette.OnError,
                ErrorContainer = GenerateTonalVariant(basePalette.Error, 0.2f),
                OnErrorContainer = GenerateTonalVariant(basePalette.Error, -0.2f),
                Background = basePalette.Background,
                OnBackground = basePalette.OnBackground,
                Surface = basePalette.Surface,
                OnSurface = basePalette.OnSurface,
                SurfaceVariant = GenerateTonalVariant(basePalette.Surface, isDarkMode ? 0.1f : -0.05f),
                OnSurfaceVariant = GetContrastColor(GenerateTonalVariant(basePalette.Surface, isDarkMode ? 0.1f : -0.05f)),
                Outline = GenerateOutlineColor(basePalette.Surface, isDarkMode),
                OutlineVariant = GenerateTonalVariant(GenerateOutlineColor(basePalette.Surface, isDarkMode), 0.1f),
                
                // Add tonal colors from base palette for state variations
                Tonal100 = basePalette.Tonal100,
                Tonal300 = basePalette.Tonal300,
                Tonal400 = basePalette.Tonal400,
                Tonal700 = basePalette.Tonal700
            };

            // Ensure accessibility compliance if requested
            if (ensureAccessibility)
            {
                EnsureAccessibility(palette);
            }

            return palette;
        }

        /// <summary>
        /// Ensure all colors meet WCAG AA contrast requirements
        /// </summary>
        private static void EnsureAccessibility(MaterialYouPalette palette)
        {
            // Ensure text colors meet contrast on their backgrounds
            palette.OnPrimary = ColorAccessibilityHelper.EnsureContrastRatio(
                palette.OnPrimary, palette.Primary, ColorAccessibilityHelper.WCAG_AA_Normal);
            
            palette.OnSecondary = ColorAccessibilityHelper.EnsureContrastRatio(
                palette.OnSecondary, palette.Secondary, ColorAccessibilityHelper.WCAG_AA_Normal);
            
            palette.OnBackground = ColorAccessibilityHelper.EnsureContrastRatio(
                palette.OnBackground, palette.Background, ColorAccessibilityHelper.WCAG_AA_Normal);
            
            palette.OnSurface = ColorAccessibilityHelper.EnsureContrastRatio(
                palette.OnSurface, palette.Surface, ColorAccessibilityHelper.WCAG_AA_Normal);
        }

        /// <summary>
        /// Generate tertiary color (accent color)
        /// </summary>
        private static Color GenerateTertiaryColor(Color seedColor)
        {
            var (h, s, l) = RgbToHsl(seedColor.R, seedColor.G, seedColor.B);
            h = (h + 0.33) % 1.0; // Shift hue by 120 degrees for tertiary
            return HslToRgb(h, s, l, seedColor.A);
        }

        /// <summary>
        /// Generate tonal variant
        /// </summary>
        private static Color GenerateTonalVariant(Color color, float adjustment)
        {
            var (h, s, l) = RgbToHsl(color.R, color.G, color.B);
            l = Math.Max(0, Math.Min(1, l + adjustment));
            return HslToRgb(h, s, l, color.A);
        }

        /// <summary>
        /// Generate outline color (for borders)
        /// </summary>
        private static Color GenerateOutlineColor(Color surfaceColor, bool isDarkMode)
        {
            if (isDarkMode)
            {
                // Dark mode: lighter outline
                return ColorAccessibilityHelper.LightenColor(surfaceColor, 0.3f);
            }
            else
            {
                // Light mode: darker outline
                return ColorAccessibilityHelper.DarkenColor(surfaceColor, 0.3f);
            }
        }

        /// <summary>
        /// Get contrasting color
        /// </summary>
        private static Color GetContrastColor(Color backgroundColor)
        {
            double luminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255.0;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        // HSL conversion helpers (reuse from DynamicColorGenerator or implement here)
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

        private static Color HslToRgb(double h, double s, double l, int alpha)
        {
            double r, g, b;

            if (s == 0)
            {
                r = g = b = l;
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

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
            return p;
        }
    }

    /// <summary>
    /// Material You color palette structure
    /// </summary>
    public class MaterialYouPalette
    {
        public Color Primary { get; set; }
        public Color OnPrimary { get; set; }
        public Color PrimaryContainer { get; set; }
        public Color OnPrimaryContainer { get; set; }
        public Color Secondary { get; set; }
        public Color OnSecondary { get; set; }
        public Color SecondaryContainer { get; set; }
        public Color OnSecondaryContainer { get; set; }
        public Color Tertiary { get; set; }
        public Color OnTertiary { get; set; }
        public Color Error { get; set; }
        public Color OnError { get; set; }
        public Color ErrorContainer { get; set; }
        public Color OnErrorContainer { get; set; }
        public Color Background { get; set; }
        public Color OnBackground { get; set; }
        public Color Surface { get; set; }
        public Color OnSurface { get; set; }
        public Color SurfaceVariant { get; set; }
        public Color OnSurfaceVariant { get; set; }
        public Color Outline { get; set; }
        public Color OutlineVariant { get; set; }
        
        // Tonal colors for state variations (Material You uses tonal colors for interactive states)
        public Color Tonal100 { get; set; }
        public Color Tonal300 { get; set; }
        public Color Tonal400 { get; set; }
        public Color Tonal700 { get; set; }
    }
}
