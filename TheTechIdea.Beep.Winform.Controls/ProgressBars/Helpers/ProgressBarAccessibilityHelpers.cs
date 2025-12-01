using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    /// <summary>
    /// Helper class for accessibility features in progress bar controls
    /// Supports screen readers, high contrast mode, reduced motion, and WCAG compliance
    /// </summary>
    public static class ProgressBarAccessibilityHelpers
    {
        #region Windows API for System Settings

        [DllImport("user32.dll")]
        private static extern int SystemParametersInfo(int uAction, int uParam, ref bool lpvParam, int fuWinIni);

        private const int SPI_GETCLIENTAREAANIMATION = 0x1042;
        private const int SPI_GETANIMATION = 0x0048;
        private const int SPI_GETHIGHCONTRAST = 0x0042;

        #endregion

        #region System Detection

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

        #endregion

        #region ARIA Attributes

        /// <summary>
        /// Generate accessible name for screen readers
        /// </summary>
        public static string GenerateAccessibleName(
            BeepProgressBar progressBar,
            string customName = null)
        {
            if (!string.IsNullOrEmpty(customName))
                return customName;

            if (progressBar == null)
                return "Progress bar";

            int percentage = (int)((float)(progressBar.Value - progressBar.Minimum) / 
                Math.Max(1, progressBar.Maximum - progressBar.Minimum) * 100);

            return $"Progress bar, {percentage}% complete";
        }

        /// <summary>
        /// Generate accessible description for screen readers
        /// </summary>
        public static string GenerateAccessibleDescription(
            BeepProgressBar progressBar,
            string customDescription = null)
        {
            if (!string.IsNullOrEmpty(customDescription))
                return customDescription;

            if (progressBar == null)
                return "Shows the current progress";

            int percentage = (int)((float)(progressBar.Value - progressBar.Minimum) / 
                Math.Max(1, progressBar.Maximum - progressBar.Minimum) * 100);

            string description = $"Progress is {percentage}% complete";
            
            if (progressBar.ShowTaskCount && progressBar.TotalTasks > 0)
            {
                description += $". {progressBar.CompletedTasks} of {progressBar.TotalTasks} tasks completed";
            }
            else
            {
                description += $". Value is {progressBar.Value} out of {progressBar.Maximum}";
            }

            return description;
        }

        /// <summary>
        /// Generate accessible value for screen readers
        /// </summary>
        public static string GenerateAccessibleValue(BeepProgressBar progressBar)
        {
            if (progressBar == null)
                return "0%";

            int percentage = (int)((float)(progressBar.Value - progressBar.Minimum) / 
                Math.Max(1, progressBar.Maximum - progressBar.Minimum) * 100);

            return $"{percentage}%";
        }

        /// <summary>
        /// Apply accessibility settings to a progress bar control
        /// Sets ARIA attributes for screen readers
        /// </summary>
        public static void ApplyAccessibilitySettings(
            BeepProgressBar progressBar,
            string accessibleName = null,
            string accessibleDescription = null)
        {
            if (progressBar == null)
                return;

            // Set ARIA attributes for screen readers
            progressBar.AccessibleName = GenerateAccessibleName(progressBar, accessibleName);
            progressBar.AccessibleDescription = GenerateAccessibleDescription(progressBar, accessibleDescription);
            progressBar.AccessibleRole = AccessibleRole.ProgressBar;
           
        }

        #endregion

        #region High Contrast Support

        /// <summary>
        /// Get accessible colors for high contrast mode
        /// Returns (backColor, foreColor, textColor, borderColor)
        /// </summary>
        public static (Color backColor, Color foreColor, Color textColor, Color borderColor) GetHighContrastColors()
        {
            if (!IsHighContrastMode())
            {
                // Return default colors if not in high contrast
                return (
                    Color.FromArgb(240, 240, 240),  // Light gray background
                    Color.FromArgb(52, 152, 219),   // Blue foreground
                    Color.White,                     // White text
                    Color.FromArgb(30, 0, 0, 0)      // Subtle border
                );
            }

            // Use system colors in high contrast mode
            return (
                SystemColors.Window,                 // Background (system window)
                SystemColors.Highlight,             // Foreground (system highlight)
                SystemColors.WindowText,            // Text (system window text)
                SystemColors.WindowFrame            // Border (system window frame)
            );
        }

        /// <summary>
        /// Adjust colors for high contrast mode
        /// </summary>
        public static (Color backColor, Color foreColor, Color textColor, Color borderColor) AdjustColorsForHighContrast(
            Color preferredBackColor,
            Color preferredForeColor,
            Color preferredTextColor,
            Color preferredBorderColor)
        {
            if (!IsHighContrastMode())
            {
                // Return preferred colors if not in high contrast
                return (preferredBackColor, preferredForeColor, preferredTextColor, preferredBorderColor);
            }

            // Use high contrast system colors
            return GetHighContrastColors();
        }

        /// <summary>
        /// Apply high contrast adjustments to progress bar colors
        /// </summary>
        public static void ApplyHighContrastAdjustments(
            BeepProgressBar progressBar,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (progressBar == null || !IsHighContrastMode())
                return;

            // Get high contrast colors
            var (backColor, foreColor, textColor, borderColor) = GetHighContrastColors();

            // Apply to progress bar
            progressBar.BackColor = backColor;
            progressBar.ProgressColor = foreColor;
            progressBar.TextColor = textColor;
            
            // Border color is applied via border pen in ApplyTheme()
        }

        #endregion

        #region WCAG Compliance

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
        public static double GetRelativeLuminance(Color color)
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
        /// Ensure contrast ratio meets WCAG standards
        /// WCAG AA: 4.5:1 for normal text, 3:1 for large text
        /// </summary>
        public static bool EnsureContrastRatio(Color foreColor, Color backColor, double minRatio = 4.5)
        {
            var ratio = CalculateContrastRatio(foreColor, backColor);
            return ratio >= minRatio;
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

        #endregion

        #region Accessible Sizing

        /// <summary>
        /// Get accessible minimum size (larger touch targets for accessibility)
        /// </summary>
        public static Size GetAccessibleMinimumSize(Size normalMinimumSize)
        {
            // Ensure minimum 44x44 pixels for touch targets (WCAG recommendation)
            // But for progress bars, height can be smaller (minimum 8px)
            return new Size(
                Math.Max(normalMinimumSize.Width, 44),  // Minimum width for touch
                Math.Max(normalMinimumSize.Height, 8)    // Minimum height (progress bars can be thin)
            );
        }

        /// <summary>
        /// Get accessible bar height based on ProgressBarSize
        /// Adjusts height for better accessibility in high contrast mode
        /// </summary>
        public static int GetAccessibleBarHeight(ProgressBarSize barSize)
        {
            int baseHeight = barSize switch
            {
                ProgressBarSize.Thin => 4,
                ProgressBarSize.Small => 8,
                ProgressBarSize.Medium => 12,
                ProgressBarSize.Large => 20,
                ProgressBarSize.ExtraLarge => 30,
                _ => 12
            };

            // Increase height slightly in high contrast mode for better visibility
            if (IsHighContrastMode())
            {
                baseHeight = Math.Max(baseHeight + 2, 8); // Minimum 8px in high contrast
            }

            return baseHeight;
        }

        /// <summary>
        /// Get accessible border width (thicker borders for visibility)
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
        /// Get accessible font size (minimum readable size)
        /// </summary>
        public static float GetAccessibleFontSize(float preferredSize)
        {
            // Minimum 10pt for progress bar text (can be smaller than regular text)
            return Math.Max(preferredSize, 10f);
        }

        #endregion

        #region Reduced Motion

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

        #endregion
    }
}

