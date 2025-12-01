using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Ratings;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Helpers
{
    /// <summary>
    /// Helper class for accessibility features in rating controls
    /// Supports screen readers, high contrast mode, reduced motion, and WCAG compliance
    /// </summary>
    public static class RatingAccessibilityHelpers
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
            BeepStarRating rating,
            string customName = null)
        {
            if (!string.IsNullOrEmpty(customName))
                return customName;

            if (rating == null)
                return "Rating";

            string context = !string.IsNullOrEmpty(rating.RatingContext) 
                ? rating.RatingContext 
                : "Rating";

            if (rating.SelectedRating > 0)
            {
                return $"{context}, {rating.SelectedRating} out of {rating.StarCount} stars";
            }

            return $"{context}, not rated";
        }

        /// <summary>
        /// Generate accessible description for screen readers
        /// </summary>
        public static string GenerateAccessibleDescription(
            BeepStarRating rating,
            string customDescription = null)
        {
            if (!string.IsNullOrEmpty(customDescription))
                return customDescription;

            if (rating == null)
                return "Rating control for providing feedback";

            string context = !string.IsNullOrEmpty(rating.RatingContext) 
                ? rating.RatingContext 
                : "Rating";

            string description = $"Rate {context} from 1 to {rating.StarCount} stars";

            if (rating.SelectedRating > 0)
            {
                description += $". Currently rated {rating.SelectedRating} out of {rating.StarCount} stars";
                
                if (rating.AllowHalfStars && rating.PreciseRating > 0)
                {
                    description += $" ({rating.PreciseRating:F1} stars)";
                }
            }
            else
            {
                description += ". Not yet rated";
            }

            if (rating.ShowRatingCount && rating.RatingCount > 0)
            {
                description += $". {rating.RatingCount} {(rating.RatingCount == 1 ? "rating" : "ratings")} total";
            }

            if (rating.ShowAverage && rating.AverageRating > 0)
            {
                description += $". Average rating: {rating.AverageRating:F1} stars";
            }

            if (!rating.ReadOnly)
            {
                description += ". Click a star to set your rating";
            }

            return description;
        }

        /// <summary>
        /// Generate accessible value for screen readers
        /// </summary>
        public static string GenerateAccessibleValue(BeepStarRating rating)
        {
            if (rating == null)
                return "0";

            if (rating.SelectedRating == 0)
                return "Not rated";

            if (rating.AllowHalfStars && rating.PreciseRating > 0)
            {
                return $"{rating.PreciseRating:F1} out of {rating.StarCount}";
            }

            return $"{rating.SelectedRating} out of {rating.StarCount}";
        }

        /// <summary>
        /// Generate accessible name for an individual star
        /// </summary>
        public static string GenerateStarAccessibleName(
            BeepStarRating rating,
            int starIndex,
            bool isFilled)
        {
            if (rating == null)
                return $"Star {starIndex + 1}";

            string context = !string.IsNullOrEmpty(rating.RatingContext) 
                ? rating.RatingContext 
                : "Rating";

            string state = isFilled ? "filled" : "empty";
            return $"Star {starIndex + 1} of {rating.StarCount} for {context}, {state}";
        }

        /// <summary>
        /// Generate accessible description for an individual star
        /// </summary>
        public static string GenerateStarAccessibleDescription(
            BeepStarRating rating,
            int starIndex,
            bool isFilled)
        {
            if (rating == null)
                return $"Star {starIndex + 1}";

            string action = isFilled 
                ? "Click to clear rating or change to a different star" 
                : "Click to rate {starIndex + 1} stars";

            if (rating.AllowHalfStars)
            {
                action += ". Click left side for half star, right side for full star";
            }

            return action;
        }

        /// <summary>
        /// Apply accessibility settings to a rating control
        /// Sets ARIA attributes for screen readers
        /// </summary>
        public static void ApplyAccessibilitySettings(
            BeepStarRating rating,
            string accessibleName = null,
            string accessibleDescription = null)
        {
            if (rating == null)
                return;

            // Set ARIA attributes for screen readers
            rating.AccessibleName = GenerateAccessibleName(rating, accessibleName);
            rating.AccessibleDescription = GenerateAccessibleDescription(rating, accessibleDescription);
        
        
        }

        #endregion

        #region High Contrast Support

        /// <summary>
        /// Get accessible colors for high contrast mode
        /// Returns (filledColor, emptyColor, hoverColor, borderColor, textColor)
        /// </summary>
        public static (Color filledColor, Color emptyColor, Color hoverColor, Color borderColor, Color textColor) GetHighContrastColors()
        {
            if (!IsHighContrastMode())
            {
                // Return default colors if not in high contrast
                return (
                    Color.FromArgb(255, 215, 0),  // Gold for filled
                    Color.FromArgb(200, 200, 200), // Gray for empty
                    Color.FromArgb(255, 165, 0), // Orange for hover
                    Color.FromArgb(130, 130, 130), // Gray for border
                    SystemColors.WindowText       // System text color
                );
            }

            // Use system colors in high contrast mode
            return (
                SystemColors.Highlight,          // Filled (system highlight)
                SystemColors.ControlDark,        // Empty (system control dark)
                SystemColors.HotTrack,           // Hover (system hot track)
                SystemColors.WindowFrame,        // Border (system window frame)
                SystemColors.WindowText          // Text (system window text)
            );
        }

        /// <summary>
        /// Adjust colors for high contrast mode
        /// </summary>
        public static (Color filledColor, Color emptyColor, Color hoverColor, Color borderColor, Color textColor) AdjustColorsForHighContrast(
            Color preferredFilledColor,
            Color preferredEmptyColor,
            Color preferredHoverColor,
            Color preferredBorderColor,
            Color preferredTextColor)
        {
            if (!IsHighContrastMode())
            {
                // Return preferred colors if not in high contrast
                return (preferredFilledColor, preferredEmptyColor, preferredHoverColor, preferredBorderColor, preferredTextColor);
            }

            // Use high contrast system colors
            return GetHighContrastColors();
        }

        /// <summary>
        /// Apply high contrast adjustments to rating colors
        /// </summary>
        public static void ApplyHighContrastAdjustments(
            BeepStarRating rating,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (rating == null || !IsHighContrastMode())
                return;

            // Get high contrast colors
            var (filledColor, emptyColor, hoverColor, borderColor, textColor) = GetHighContrastColors();

            // Apply to rating
            rating.FilledStarColor = filledColor;
            rating.EmptyStarColor = emptyColor;
            rating.HoverStarColor = hoverColor;
            rating.StarBorderColor = borderColor;
            rating.LabelColor = textColor;
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
            // But for rating controls, we need to account for multiple stars
            return new Size(
                Math.Max(normalMinimumSize.Width, 44),
                Math.Max(normalMinimumSize.Height, 44)
            );
        }

        /// <summary>
        /// Get accessible star size (minimum size for individual stars)
        /// </summary>
        public static int GetAccessibleStarSize(int normalStarSize)
        {
            // Ensure minimum 32x32 pixels for individual stars (WCAG recommendation for interactive elements)
            return Math.Max(normalStarSize, 32);
        }

        /// <summary>
        /// Get accessible spacing (more spacing for better visibility)
        /// </summary>
        public static int GetAccessibleSpacing(int normalSpacing)
        {
            if (IsHighContrastMode())
            {
                // Increase spacing in high contrast mode for better visibility
                return Math.Max(normalSpacing + 2, 8);
            }
            return normalSpacing;
        }

        /// <summary>
        /// Get accessible border width (thicker borders for visibility)
        /// </summary>
        public static float GetAccessibleBorderWidth(float normalBorderWidth)
        {
            if (IsHighContrastMode())
            {
                return Math.Max(normalBorderWidth, 2f); // Minimum 2px in high contrast
            }
            return normalBorderWidth;
        }

        /// <summary>
        /// Get accessible font size (minimum readable size)
        /// </summary>
        public static float GetAccessibleFontSize(float preferredSize)
        {
            // Minimum 12pt for accessibility (WCAG recommendation)
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
            {
                return true; // Already disabled
            }

            return IsReducedMotionEnabled();
        }

        /// <summary>
        /// Should glow effects be disabled based on user preferences
        /// </summary>
        public static bool ShouldDisableGlowEffects(bool currentGlowEnabled)
        {
            if (!currentGlowEnabled)
            {
                return true; // Already disabled
            }

            return IsReducedMotionEnabled();
        }

        #endregion
    }
}

