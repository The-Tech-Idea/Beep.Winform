using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Tasks.Helpers
{
    /// <summary>
    /// Helper class for accessibility features in task card controls
    /// Supports screen readers, high contrast mode, reduced motion, and WCAG compliance
    /// </summary>
    public static class TaskCardAccessibilityHelpers
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
                return SystemInformation.HighContrast;
            }
            catch
            {
                try
                {
                    bool highContrast = false;
                    SystemParametersInfo(SPI_GETHIGHCONTRAST, 0, ref highContrast, 0);
                    return highContrast;
                }
                catch
                {
                    return SystemColors.Control != SystemColors.Window;
                }
            }
        }

        /// <summary>
        /// Check if reduced motion is enabled
        /// </summary>
        public static bool IsReducedMotionEnabled()
        {
            try
            {
                bool animationsEnabled = true;
                SystemParametersInfo(SPI_GETCLIENTAREAANIMATION, 0, ref animationsEnabled, 0);
                return !animationsEnabled;
            }
            catch
            {
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
            BeepTaskCard card,
            string customName = null)
        {
            if (!string.IsNullOrEmpty(customName))
                return customName;

            if (card == null)
                return "Task Card";

            string title = !string.IsNullOrEmpty(card.TitleText) 
                ? card.TitleText 
                : "Task";

            return $"{title} - Task Card";
        }

        /// <summary>
        /// Generate accessible description for screen readers
        /// </summary>
        public static string GenerateAccessibleDescription(
            BeepTaskCard card,
            string customDescription = null)
        {
            if (!string.IsNullOrEmpty(customDescription))
                return customDescription;

            if (card == null)
                return "A task card displaying project information and progress";

            string description = $"Task card: {card.TitleText}";
            
            if (!string.IsNullOrEmpty(card.SubtitleText))
            {
                description += $". {card.SubtitleText}";
            }

            if (!string.IsNullOrEmpty(card.MetricText))
            {
                description += $". {card.MetricText}";
            }

            description += $". Progress: {card.ProgressValue:F0}%";

            if (card.AvatarImagePaths != null && card.AvatarImagePaths.Count > 0)
            {
                description += $". {card.AvatarImagePaths.Count} team member{(card.AvatarImagePaths.Count == 1 ? "" : "s")}";
            }

            return description;
        }

        /// <summary>
        /// Apply accessibility settings to a task card control
        /// </summary>
        public static void ApplyAccessibilitySettings(
            BeepTaskCard card,
            string accessibleName = null,
            string accessibleDescription = null)
        {
            if (card == null)
                return;

            card.AccessibleName = GenerateAccessibleName(card, accessibleName);
            card.AccessibleDescription = GenerateAccessibleDescription(card, accessibleDescription);
            card.AccessibleRole = AccessibleRole.Grouping;
          
        }

        #endregion

        #region High Contrast Support

        /// <summary>
        /// Get accessible colors for high contrast mode
        /// </summary>
        public static (Color backColor, Color titleColor, Color subtitleColor, Color metricColor, Color progressColor, Color iconColor) GetHighContrastColors()
        {
            if (!IsHighContrastMode())
            {
                return (
                    Color.White,
                    Color.Black,
                    Color.FromArgb(128, 128, 128),
                    Color.Black,
                    Color.Black,
                    Color.Black
                );
            }

            return (
                SystemColors.Window,
                SystemColors.WindowText,
                SystemColors.GrayText,
                SystemColors.WindowText,
                SystemColors.WindowText,
                SystemColors.WindowText
            );
        }

        /// <summary>
        /// Adjust colors for high contrast mode
        /// </summary>
        public static (Color backColor, Color titleColor, Color subtitleColor, Color metricColor, Color progressColor, Color iconColor) AdjustColorsForHighContrast(
            Color preferredBackColor,
            Color preferredTitleColor,
            Color preferredSubtitleColor,
            Color preferredMetricColor,
            Color preferredProgressColor,
            Color preferredIconColor)
        {
            if (!IsHighContrastMode())
            {
                return (preferredBackColor, preferredTitleColor, preferredSubtitleColor, preferredMetricColor, preferredProgressColor, preferredIconColor);
            }

            return GetHighContrastColors();
        }

        /// <summary>
        /// Apply high contrast adjustments to task card colors
        /// </summary>
        public static void ApplyHighContrastAdjustments(
            BeepTaskCard card,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (card == null || !IsHighContrastMode())
                return;

            var (backColor, titleColor, subtitleColor, metricColor, progressColor, iconColor) = GetHighContrastColors();

            // Apply to card
            card.BackColor = backColor;
            // Text colors are applied in DrawContent()
        }

        #endregion

        #region WCAG Compliance

        /// <summary>
        /// Calculate contrast ratio between two colors
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
        /// Get relative luminance of a color
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
                return foreColor;

            var backLuminance = GetRelativeLuminance(backColor);
            return backLuminance > 0.5 ? Color.Black : Color.White;
        }

        #endregion

        #region Accessible Sizing

        /// <summary>
        /// Get accessible minimum size
        /// </summary>
        public static Size GetAccessibleMinimumSize(Size normalMinimumSize)
        {
            return new Size(
                Math.Max(normalMinimumSize.Width, 150),
                Math.Max(normalMinimumSize.Height, 200)
            );
        }

        /// <summary>
        /// Get accessible font size
        /// </summary>
        public static float GetAccessibleFontSize(float preferredSize)
        {
            return Math.Max(preferredSize, 12f);
        }

        #endregion

        #region Reduced Motion

        /// <summary>
        /// Should animations be disabled based on user preferences
        /// </summary>
        public static bool ShouldDisableAnimations(bool currentAnimationEnabled)
        {
            if (!currentAnimationEnabled)
                return true;

            return IsReducedMotionEnabled();
        }

        #endregion
    }
}

