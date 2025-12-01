using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Ratings;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Helpers
{
    /// <summary>
    /// Centralized theme color management for Rating controls
    /// Provides consistent color retrieval based on theme and rating style
    /// </summary>
    public static class RatingThemeHelpers
    {
        #region Rating Colors

        /// <summary>
        /// Get color for filled ratings (selected stars/icons)
        /// Priority: Custom color > Theme StarRatingFillColor > Theme Primary/Success > Default Gold
        /// </summary>
        public static Color GetFilledRatingColor(
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            Color? customColor = null)
        {
            // Priority 1: Custom color (highest priority)
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StarRatingFillColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Style-specific fallbacks
                switch (style)
                {
                    case RatingStyle.Heart:
                        // Hearts typically use red/pink
                        if (theme.ErrorColor != Color.Empty)
                            return theme.ErrorColor;
                        break;
                    case RatingStyle.Thumb:
                        // Thumbs use primary color
                        if (theme.PrimaryColor != Color.Empty)
                            return theme.PrimaryColor;
                        break;
                }

                // General fallbacks
                if (theme.SuccessColor != Color.Empty)
                    return theme.SuccessColor;

                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;

                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
            }

            // Priority 3: Default colors based on style
            return style switch
            {
                RatingStyle.Heart => Color.FromArgb(236, 72, 153), // Pink
                RatingStyle.Thumb => Color.FromArgb(59, 130, 246), // Blue
                RatingStyle.Circle => Color.FromArgb(59, 130, 246), // Blue
                _ => Color.FromArgb(255, 215, 0) // Gold for stars
            };
        }

        /// <summary>
        /// Get color for empty ratings (unselected stars/icons)
        /// Priority: Custom color > Theme StarRatingBackColor > Theme Disabled/Border > Default Gray
        /// </summary>
        public static Color GetEmptyRatingColor(
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            Color? customColor = null)
        {
            // Priority 1: Custom color
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StarRatingBackColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to disabled color
                if (theme.DisabledBackColor != Color.Empty)
                    return theme.DisabledBackColor;

                // Fallback to border color (lighter)
                if (theme.BorderColor != Color.Empty)
                    return ControlPaint.Light(theme.BorderColor, 0.7f);

                // Fallback to secondary color
                if (theme.SecondaryColor != Color.Empty)
                    return theme.SecondaryColor;
            }

            // Priority 3: Default gray
            return Color.FromArgb(200, 200, 200);
        }

        /// <summary>
        /// Get color for hovered ratings
        /// Priority: Custom color > Theme StarRatingHoverForeColor > Theme Accent > Default Orange
        /// </summary>
        public static Color GetHoverRatingColor(
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            Color? customColor = null)
        {
            // Priority 1: Custom color
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StarRatingHoverForeColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to accent color
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;

                // Fallback to warning color (orange-like)
                if (theme.WarningColor != Color.Empty)
                    return theme.WarningColor;

                // Fallback to primary color (lighter)
                if (theme.PrimaryColor != Color.Empty)
                    return ControlPaint.Light(theme.PrimaryColor, 0.2f);
            }

            // Priority 3: Default orange
            return Color.FromArgb(255, 165, 0); // Orange
        }

        /// <summary>
        /// Get color for rating borders
        /// Priority: Custom color > Theme StarRatingBorderColor > Theme Border > Default
        /// </summary>
        public static Color GetRatingBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            Color? customColor = null)
        {
            // Priority 1: Custom color
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StarRatingBorderColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to border color
                if (theme.BorderColor != Color.Empty)
                    return theme.BorderColor;

                // Fallback to secondary text color (subtle)
                if (theme.SecondaryTextColor != Color.Empty)
                    return theme.SecondaryTextColor;
            }

            // Priority 3: Default (subtle gray)
            return Color.FromArgb(130, 130, 130);
        }

        /// <summary>
        /// Get color for rating labels
        /// Priority: Custom color > Theme PrimaryTextColor > Theme ForeColor > Default Black
        /// </summary>
        public static Color GetRatingLabelColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            // Priority 1: Custom color
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                if (theme.PrimaryTextColor != Color.Empty)
                    return theme.PrimaryTextColor;

                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;

                if (theme.CardTextForeColor != Color.Empty)
                    return theme.CardTextForeColor;
            }

            // Priority 3: Default black
            return Color.Black;
        }

        #endregion

        #region Bulk Theme Application

        /// <summary>
        /// Apply theme colors to a rating control
        /// Updates all color properties based on theme and style
        /// </summary>
        public static void ApplyThemeColors(
            BeepStarRating rating,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (rating == null || theme == null)
                return;

            // Apply colors based on rating style
            rating.FilledStarColor = GetFilledRatingColor(
                theme,
                useThemeColors,
                rating.RatingStyle,
                rating.FilledStarColor);

            rating.EmptyStarColor = GetEmptyRatingColor(
                theme,
                useThemeColors,
                rating.RatingStyle,
                rating.EmptyStarColor);

            rating.HoverStarColor = GetHoverRatingColor(
                theme,
                useThemeColors,
                rating.RatingStyle,
                rating.HoverStarColor);

            rating.StarBorderColor = GetRatingBorderColor(
                theme,
                useThemeColors,
                rating.RatingStyle,
                rating.StarBorderColor);

            rating.LabelColor = GetRatingLabelColor(
                theme,
                useThemeColors,
                rating.LabelColor);
        }

        /// <summary>
        /// Get all theme colors for a rating style
        /// Returns a tuple of (filledColor, emptyColor, hoverColor, borderColor, labelColor)
        /// </summary>
        public static (Color filledColor, Color emptyColor, Color hoverColor, Color borderColor, Color labelColor) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            Color? customFilledColor = null,
            Color? customEmptyColor = null,
            Color? customHoverColor = null,
            Color? customBorderColor = null,
            Color? customLabelColor = null)
        {
            return (
                GetFilledRatingColor(theme, useThemeColors, style, customFilledColor),
                GetEmptyRatingColor(theme, useThemeColors, style, customEmptyColor),
                GetHoverRatingColor(theme, useThemeColors, style, customHoverColor),
                GetRatingBorderColor(theme, useThemeColors, style, customBorderColor),
                GetRatingLabelColor(theme, useThemeColors, customLabelColor)
            );
        }

        #endregion
    }
}

