using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.Ratings;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Helpers
{
    /// <summary>
    /// Font element types for rating controls
    /// </summary>
    public enum RatingFontElement
    {
        Label,
        Count,
        Average
    }

    /// <summary>
    /// Centralized font management for Rating controls
    /// Integrates with BeepFontManager and StyleTypography
    /// </summary>
    public static class RatingFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>
        /// Get font for rating labels (text below stars/icons)
        /// </summary>
        public static Font GetLabelFont(
            BeepStarRating rating,
            BeepControlStyle controlStyle,
            RatingStyle ratingStyle = RatingStyle.ClassicStar)
        {
            if (rating == null)
                return GetLabelFont(controlStyle, ratingStyle, 24, null);

            return GetLabelFont(controlStyle, ratingStyle, rating.StarSize, rating.Font);
        }

        /// <summary>
        /// Get font for rating labels (overload with individual properties)
        /// </summary>
        public static Font GetLabelFont(
            BeepControlStyle controlStyle,
            RatingStyle ratingStyle,
            int starSize,
            Font baseFont = null)
        {
            baseFont = baseFont ?? new Font("Segoe UI", 8, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, RatingFontElement.Label);
            
            // Adjust size based on star size if available
            if (starSize > 0)
            {
                // Font size should be proportional to star size (about 25-30% of star size)
                fontSize = Math.Max(7, Math.Min(14, (int)(starSize * 0.28f)));
            }

            FontStyle fontStyle = GetFontStyleForElement(controlStyle, RatingFontElement.Label);

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                // Fallback to base font with adjusted size
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get font for rating count text (e.g., "(5 ratings)")
        /// </summary>
        public static Font GetCountFont(
            BeepStarRating rating,
            BeepControlStyle controlStyle,
            RatingStyle ratingStyle = RatingStyle.ClassicStar)
        {
            if (rating == null)
                return GetCountFont(controlStyle, ratingStyle, null);

            return GetCountFont(controlStyle, ratingStyle, rating.Font);
        }

        /// <summary>
        /// Get font for rating count text (overload with individual properties)
        /// </summary>
        public static Font GetCountFont(
            BeepControlStyle controlStyle,
            RatingStyle ratingStyle,
            Font baseFont = null)
        {
            baseFont = baseFont ?? new Font("Segoe UI", 7, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, RatingFontElement.Count);
            
            // Count text is typically smaller than labels
            fontSize = Math.Max(6, fontSize - 1);

            FontStyle fontStyle = GetFontStyleForElement(controlStyle, RatingFontElement.Count);

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get font for average rating text (e.g., "Avg: 4.5")
        /// </summary>
        public static Font GetAverageFont(
            BeepStarRating rating,
            BeepControlStyle controlStyle,
            RatingStyle ratingStyle = RatingStyle.ClassicStar)
        {
            if (rating == null)
                return GetAverageFont(controlStyle, ratingStyle, null);

            return GetAverageFont(controlStyle, ratingStyle, rating.Font);
        }

        /// <summary>
        /// Get font for average rating text (overload with individual properties)
        /// </summary>
        public static Font GetAverageFont(
            BeepControlStyle controlStyle,
            RatingStyle ratingStyle,
            Font baseFont = null)
        {
            baseFont = baseFont ?? new Font("Segoe UI", 7, FontStyle.Regular);

            int fontSize = GetFontSizeForElement(controlStyle, RatingFontElement.Average);
            
            // Average text is typically smaller than labels
            fontSize = Math.Max(6, fontSize - 1);

            FontStyle fontStyle = GetFontStyleForElement(controlStyle, RatingFontElement.Average);

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get font for rating control based on element type
        /// </summary>
        public static Font GetFontForElement(
            BeepStarRating rating,
            RatingFontElement element,
            BeepControlStyle controlStyle,
            RatingStyle ratingStyle = RatingStyle.ClassicStar)
        {
            return element switch
            {
                RatingFontElement.Label => GetLabelFont(rating, controlStyle, ratingStyle),
                RatingFontElement.Count => GetCountFont(rating, controlStyle, ratingStyle),
                RatingFontElement.Average => GetAverageFont(rating, controlStyle, ratingStyle),
                _ => GetLabelFont(rating, controlStyle, ratingStyle)
            };
        }

        #endregion

        #region Font Size and Style Helpers

        /// <summary>
        /// Get font size for a specific element based on control style
        /// </summary>
        public static int GetFontSizeForElement(BeepControlStyle controlStyle, RatingFontElement element)
        {
            // Base size from StyleTypography
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            // Adjust based on element type
            float elementSize = element switch
            {
                RatingFontElement.Label => baseSize - 1f,      // Labels: slightly smaller than body
                RatingFontElement.Count => baseSize - 2f,     // Count: smaller than labels
                RatingFontElement.Average => baseSize - 2f,    // Average: same as count
                _ => baseSize - 1f
            };

            // Adjust based on control style
            elementSize = AdjustSizeForControlStyle(elementSize, controlStyle);

            return Math.Max(6, (int)Math.Round(elementSize));
        }

        /// <summary>
        /// Get font style for a specific element based on control style
        /// </summary>
        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, RatingFontElement element)
        {
            // Labels can be slightly bold for emphasis
            if (element == RatingFontElement.Label)
            {
                return controlStyle switch
                {
                    BeepControlStyle.Modern => FontStyle.Bold,
                    BeepControlStyle.Material => FontStyle.Regular,
                    BeepControlStyle.Fluent => FontStyle.Regular,
                    BeepControlStyle.Minimal => FontStyle.Bold,
                    _ => FontStyle.Regular
                };
            }

            // Count and average are typically regular
            return FontStyle.Regular;
        }

        /// <summary>
        /// Adjust font size based on control style
        /// </summary>
        private static float AdjustSizeForControlStyle(float baseSize, BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Modern => baseSize * 1.05f,      // Slightly larger for modern
                BeepControlStyle.Material => baseSize,            // Standard for material
                BeepControlStyle.Fluent => baseSize * 0.95f,      // Slightly smaller for fluent
                BeepControlStyle.Minimal => baseSize * 1.1f,      // Larger for classic
                _ => baseSize
            };
        }

        #endregion

        #region Bulk Font Application

        /// <summary>
        /// Apply font theme to a rating control
        /// Updates all font properties based on theme and style
        /// </summary>
        public static void ApplyFontTheme(
            BeepStarRating rating,
            BeepControlStyle controlStyle)
        {
            if (rating == null)
                return;

            // Apply label font if labels are shown
            if (rating.ShowLabels)
            {
                if (rating.LabelFont != null)
                {
                    rating.LabelFont.Dispose();
                }
                rating.LabelFont = GetLabelFont(rating, controlStyle, rating.RatingStyle);
            }

            // Note: Count and Average fonts are used internally in painters
            // They don't need to be stored as properties since they're retrieved on-demand
        }

        #endregion
    }
}

