using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Helpers
{
    /// <summary>
    /// Helper class for accessibility features in toggle controls
    /// Supports screen readers, high contrast mode, reduced motion, and WCAG compliance
    /// </summary>
    public static class ToggleAccessibilityHelpers
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
                // Use SystemInformation for reliable detection
                return SystemInformation.HighContrast;
            }
            catch
            {
                // Fallback: check via API
                try
                {
                    bool highContrast = false;
                    SystemParametersInfo(SPI_GETHIGHCONTRAST, 0, ref highContrast, 0);
                    return highContrast;
                }
                catch
                {
                    // Final fallback: check SystemColors
                    return SystemColors.Control != SystemColors.Window;
                }
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
                // Fallback: check environment variable
                var prefersReducedMotion = Environment.GetEnvironmentVariable("PREFERS_REDUCED_MOTION");
                return prefersReducedMotion == "1" || prefersReducedMotion?.ToLowerInvariant() == "true";
            }
        }

        /// <summary>
        /// Apply accessibility settings to a toggle control
        /// Sets ARIA attributes, handles high contrast, and reduced motion
        /// </summary>
        public static void ApplyAccessibilitySettings(
            Control toggle,
            bool isOn,
            string onText,
            string offText,
            string accessibleName = null,
            string accessibleDescription = null)
        {
            if (toggle == null)
                return;

            // Set ARIA attributes for screen readers
            if (!string.IsNullOrEmpty(accessibleName))
            {
                toggle.AccessibleName = accessibleName;
            }
            else
            {
                // Generate accessible name from state and text
                toggle.AccessibleName = GenerateAccessibleName(isOn, onText, offText);
            }

            if (!string.IsNullOrEmpty(accessibleDescription))
            {
                toggle.AccessibleDescription = accessibleDescription;
            }
            else
            {
                // Generate accessible description from current state
                toggle.AccessibleDescription = GenerateAccessibleDescription(isOn, onText, offText);
            }

            // Set accessible role (toggle is similar to checkbox)
            toggle.AccessibleRole = AccessibleRole.CheckButton;

            // Set accessible value (current state)
         
            // Set accessible state
            if (toggle is BeepToggle beepToggle)
            {
                beepToggle.AccessibleDefaultActionDescription = isOn 
                    ? $"Turn off {offText}" 
                    : $"Turn on {onText}";
            }
        }

        /// <summary>
        /// Generate accessible name for screen readers
        /// </summary>
        public static string GenerateAccessibleName(bool isOn, string onText, string offText)
        {
            string stateText = isOn ? onText : offText;
            return $"Toggle switch, currently {stateText}";
        }

        /// <summary>
        /// Generate accessible description for screen readers
        /// </summary>
        public static string GenerateAccessibleDescription(bool isOn, string onText, string offText)
        {
            if (isOn)
            {
                return $"Toggle is ON ({onText}). Click to turn OFF ({offText}).";
            }
            else
            {
                return $"Toggle is OFF ({offText}). Click to turn ON ({onText}).";
            }
        }

        /// <summary>
        /// Get accessible colors for high contrast mode
        /// Returns system colors that meet high contrast requirements
        /// </summary>
        public static (Color onColor, Color offColor, Color thumbColor, Color textColor) GetHighContrastColors()
        {
            if (!IsHighContrastMode())
            {
                // Return default colors if not in high contrast
                return (
                    Color.FromArgb(52, 168, 83),  // Green for ON
                    Color.FromArgb(189, 189, 189), // Gray for OFF
                    Color.White,                  // White thumb
                    SystemColors.WindowText       // System text color
                );
            }

            // Use system colors in high contrast mode
            return (
                SystemColors.Highlight,          // ON color (system highlight)
                SystemColors.ControlDark,        // OFF color (system control dark)
                SystemColors.Window,             // Thumb color (system window)
                SystemColors.WindowText          // Text color (system window text)
            );
        }

        /// <summary>
        /// Adjust colors for high contrast mode
        /// </summary>
        public static (Color onColor, Color offColor, Color thumbColor, Color textColor) AdjustColorsForHighContrast(
            Color preferredOnColor,
            Color preferredOffColor,
            Color preferredThumbColor,
            Color preferredTextColor)
        {
            if (!IsHighContrastMode())
            {
                // Return preferred colors if not in high contrast
                return (preferredOnColor, preferredOffColor, preferredThumbColor, preferredTextColor);
            }

            // Use high contrast system colors
            return GetHighContrastColors();
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

            if (darker == 0)
                return lighter > 0 ? double.MaxValue : 1.0;

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
            var foreLuminance = GetRelativeLuminance(foreColor);

            // Determine if we need to lighten or darken
            bool needLighter = backLuminance > 0.5; // Light background needs dark text
            bool needDarker = backLuminance <= 0.5; // Dark background needs light text

            // Adjust color
            if (needLighter && foreLuminance < backLuminance)
            {
                // Need darker text for light background
                return DarkenColor(foreColor, 0.7f);
            }
            else if (needDarker && foreLuminance > backLuminance)
            {
                // Need lighter text for dark background
                return LightenColor(foreColor, 1.3f);
            }

            // Fallback: use black or white
            return backLuminance > 0.5 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Darken a color by a factor
        /// </summary>
        private static Color DarkenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, (int)(color.R * factor)),
                Math.Max(0, (int)(color.G * factor)),
                Math.Max(0, (int)(color.B * factor))
            );
        }

        /// <summary>
        /// Lighten a color by a factor
        /// </summary>
        private static Color LightenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R * factor)),
                Math.Min(255, (int)(color.G * factor)),
                Math.Min(255, (int)(color.B * factor))
            );
        }

        /// <summary>
        /// Get accessible border width for high contrast mode (thicker borders for visibility)
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
        public static bool ShouldDisableAnimations(bool currentAnimationEnabled)
        {
            if (!currentAnimationEnabled)
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
        /// Get accessible minimum size (larger touch targets for accessibility)
        /// </summary>
        public static Size GetAccessibleMinimumSize(Size normalMinimumSize)
        {
            // Ensure minimum 44x44 pixels for touch targets (WCAG recommendation)
            return new Size(
                Math.Max(normalMinimumSize.Width, 44),
                Math.Max(normalMinimumSize.Height, 44)
            );
        }

        /// <summary>
        /// Get accessible padding (more padding for better touch targets)
        /// </summary>
        public static int GetAccessiblePadding(int normalPadding)
        {
            // Increase padding slightly for better accessibility
            return normalPadding + 2;
        }

        /// <summary>
        /// Apply high contrast adjustments to toggle colors
        /// </summary>
        public static void ApplyHighContrastAdjustments(
            BeepToggle toggle,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (toggle == null || !IsHighContrastMode())
                return;

            // Get high contrast colors
            var (onColor, offColor, thumbColor, textColor) = GetHighContrastColors();

            // Apply to toggle
            toggle.OnColor = onColor;
            toggle.OffColor = offColor;
            toggle.ThumbColor = thumbColor;

            // Ensure text colors meet contrast requirements
            if (toggle.ShowLabels)
            {
                // Text color will be adjusted in painting
            }
        }

        /// <summary>
        /// Get accessible state description for screen readers
        /// </summary>
        public static string GetAccessibleStateDescription(bool isOn, string onText, string offText, string customDescription = null)
        {
            if (!string.IsNullOrEmpty(customDescription))
                return customDescription;

            return isOn 
                ? $"Enabled, {onText}" 
                : $"Disabled, {offText}";
        }
    }
}

