using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Accessibility helpers for tooltips
    /// Supports screen readers, high contrast mode, and reduced motion
    /// </summary>
    public static class ToolTipAccessibilityHelpers
    {
        #region Windows API for System Settings

        [DllImport("user32.dll")]
        private static extern int SystemParametersInfo(int uAction, int uParam, ref bool lpvParam, int fuWinIni);

        private const int SPI_GETCLIENTAREAANIMATION = 0x1042;
        private const int SPI_GETANIMATION = 0x0048;
        private const int SPI_GETHIGHCONTRAST = 0x0042;

        #endregion

        /// <summary>
        /// Check if Windows high contrast mode is enabled
        /// </summary>
        public static bool IsHighContrastMode()
        {
            try
            {
                bool highContrast = false;
                SystemParametersInfo(SPI_GETHIGHCONTRAST, 0, ref highContrast, 0);
                return highContrast;
            }
            catch
            {
                // Fallback: check SystemColors
                return SystemColors.Control != SystemColors.Window;
            }
        }

        /// <summary>
        /// Check if reduced motion is enabled (Windows animation settings)
        /// </summary>
        public static bool IsReducedMotionEnabled()
        {
            try
            {
                bool animationsEnabled = true;
                SystemParametersInfo(SPI_GETCLIENTAREAANIMATION, 0, ref animationsEnabled, 0);
                return !animationsEnabled; // Inverted: if animations disabled, reduced motion is enabled
            }
            catch
            {
                // Fallback: check registry or environment variable
                var prefersReducedMotion = Environment.GetEnvironmentVariable("PREFERS_REDUCED_MOTION");
                return prefersReducedMotion == "1" || prefersReducedMotion?.ToLowerInvariant() == "true";
            }
        }

        /// <summary>
        /// Get high contrast colors from system
        /// </summary>
        public static (Color backColor, Color foreColor, Color borderColor) GetHighContrastColors()
        {
            if (!IsHighContrastMode())
            {
                // Return default colors if not in high contrast
                return (SystemColors.Window, SystemColors.WindowText, SystemColors.WindowFrame);
            }

            // Use system colors in high contrast mode
            return (
                SystemColors.Window,           // Background
                SystemColors.WindowText,       // Foreground
                SystemColors.WindowFrame       // Border
            );
        }

        /// <summary>
        /// Ensure contrast ratio meets WCAG standards
        /// WCAG AA: 4.5:1 for normal text, 3:1 for large text
        /// WCAG AAA: 7:1 for normal text, 4.5:1 for large text
        /// </summary>
        public static bool EnsureContrastRatio(Color foreColor, Color backColor, double minRatio = 4.5)
        {
            var ratio = CalculateContrastRatio(foreColor, backColor);
            return ratio >= minRatio;
        }

        /// <summary>
        /// Calculate contrast ratio between two colors (WCAG formula)
        /// </summary>
        public static double CalculateContrastRatio(Color color1, Color color2)
        {
            var l1 = GetRelativeLuminance(color1);
            var l2 = GetRelativeLuminance(color2);

            var lighter = Math.Max(l1, l2);
            var darker = Math.Min(l1, l2);

            return (lighter + 0.05) / (darker + 0.05);
        }

        /// <summary>
        /// Get relative luminance of a color (WCAG formula)
        /// </summary>
        private static double GetRelativeLuminance(Color color)
        {
            double r = GetColorComponent(color.R);
            double g = GetColorComponent(color.G);
            double b = GetColorComponent(color.B);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        /// <summary>
        /// Convert color component to linear RGB space
        /// </summary>
        private static double GetColorComponent(byte component)
        {
            double c = component / 255.0;
            return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        /// <summary>
        /// Adjust color to meet minimum contrast ratio
        /// </summary>
        public static Color AdjustForContrast(Color foreColor, Color backColor, double minRatio = 4.5)
        {
            if (EnsureContrastRatio(foreColor, backColor, minRatio))
            {
                return foreColor; // Already meets requirement
            }

            // Try to darken or lighten foreground to meet contrast
            var backLuminance = GetRelativeLuminance(backColor);
            var targetLuminance = backLuminance > 0.5 
                ? (backLuminance + 0.05) / minRatio - 0.05  // Darker foreground for light background
                : (backLuminance + 0.05) * minRatio - 0.05;  // Lighter foreground for dark background

            // Adjust color to meet target luminance
            return AdjustLuminance(foreColor, targetLuminance);
        }

        /// <summary>
        /// Adjust color luminance to target value
        /// </summary>
        private static Color AdjustLuminance(Color color, double targetLuminance)
        {
            var currentLuminance = GetRelativeLuminance(color);
            if (Math.Abs(currentLuminance - targetLuminance) < 0.01)
            {
                return color; // Already close enough
            }

            // Simple adjustment: make darker or lighter
            var factor = targetLuminance > currentLuminance ? 1.2 : 0.8;
            
            int r = Math.Min(255, Math.Max(0, (int)(color.R * factor)));
            int g = Math.Min(255, Math.Max(0, (int)(color.G * factor)));
            int b = Math.Min(255, Math.Max(0, (int)(color.B * factor)));

            return Color.FromArgb(color.A, r, g, b);
        }

        /// <summary>
        /// Get accessible colors that meet WCAG AA standards
        /// </summary>
        public static (Color backColor, Color foreColor, Color borderColor) GetAccessibleColors(
            Color preferredBackColor,
            Color preferredForeColor,
            Color preferredBorderColor)
        {
            // Check high contrast mode first
            if (IsHighContrastMode())
            {
                return GetHighContrastColors();
            }

            // Ensure contrast ratios meet WCAG AA (4.5:1)
            var foreColor = EnsureContrastRatio(preferredForeColor, preferredBackColor, 4.5)
                ? preferredForeColor
                : AdjustForContrast(preferredForeColor, preferredBackColor, 4.5);

            var borderColor = EnsureContrastRatio(preferredBorderColor, preferredBackColor, 3.0)
                ? preferredBorderColor
                : AdjustForContrast(preferredBorderColor, preferredBackColor, 3.0);

            return (preferredBackColor, foreColor, borderColor);
        }

        /// <summary>
        /// Get border width for high contrast mode (thicker borders for visibility)
        /// </summary>
        public static int GetAccessibleBorderWidth(int normalBorderWidth)
        {
            if (IsHighContrastMode())
            {
                return Math.Max(normalBorderWidth, 2); // Minimum 2px in high contrast
            }
            return normalBorderWidth;
        }

        /// <summary>
        /// Should animations be disabled based on user preferences
        /// </summary>
        public static bool ShouldDisableAnimations(ToolTipAnimation animation)
        {
            if (animation == ToolTipAnimation.None)
            {
                return true; // Already disabled
            }

            return IsReducedMotionEnabled();
        }

        /// <summary>
        /// Get accessible font size (minimum readable size)
        /// </summary>
        public static float GetAccessibleFontSize(float preferredSize)
        {
            // Minimum 12pt for accessibility (WCAG recommendation)
            return Math.Max(preferredSize, 12f);
        }

        /// <summary>
        /// Get accessible padding (more padding for better touch targets)
        /// </summary>
        public static int GetAccessiblePadding(int normalPadding)
        {
            // Increase padding slightly for better accessibility
            return normalPadding + 2;
        }
    }
}

