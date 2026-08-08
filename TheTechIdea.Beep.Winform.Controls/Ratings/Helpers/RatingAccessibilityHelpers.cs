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

