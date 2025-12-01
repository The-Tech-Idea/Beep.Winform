using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Testimonials.Helpers
{
    /// <summary>
    /// Centralized theme color management for Testimonial controls
    /// Provides consistent color retrieval based on theme
    /// </summary>
    public static class TestimonialThemeHelpers
    {
        #region Testimonial Colors

        /// <summary>
        /// Get background color for testimonial card
        /// Priority: Custom color > Theme TestimonialBackColor > Theme CardBackColor > Default White
        /// </summary>
        public static Color GetTestimonialBackColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TestimonialBackColor != Color.Empty)
                    return theme.TestimonialBackColor;

                if (theme.CardBackColor != Color.Empty)
                    return theme.CardBackColor;

                if (theme.BackColor != Color.Empty)
                    return theme.BackColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Get color for testimonial text
        /// Priority: Custom color > Theme TestimonialTextColor > Theme CardTextForeColor > Default Black
        /// </summary>
        public static Color GetTestimonialTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TestimonialTextColor != Color.Empty)
                    return theme.TestimonialTextColor;

                if (theme.CardTextForeColor != Color.Empty)
                    return theme.CardTextForeColor;

                if (theme.PrimaryTextColor != Color.Empty)
                    return theme.PrimaryTextColor;

                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.Black;
        }

        /// <summary>
        /// Get color for name text
        /// Priority: Custom color > Theme TestimonialNameColor > Theme CardTitleForeColor > Default Black
        /// </summary>
        public static Color GetNameColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TestimonialNameColor != Color.Empty)
                    return theme.TestimonialNameColor;

                if (theme.CardTitleForeColor != Color.Empty)
                    return theme.CardTitleForeColor;

                if (theme.PrimaryTextColor != Color.Empty)
                    return theme.PrimaryTextColor;

                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.Black;
        }

        /// <summary>
        /// Get color for details text (username, position)
        /// Priority: Custom color > Theme TestimonialDetailsColor > Theme SecondaryTextColor > Default Gray
        /// </summary>
        public static Color GetDetailsColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TestimonialDetailsColor != Color.Empty)
                    return theme.TestimonialDetailsColor;

                if (theme.SecondaryTextColor != Color.Empty)
                    return theme.SecondaryTextColor;

                if (theme.CardTextForeColor != Color.Empty)
                    return Color.FromArgb(180, theme.CardTextForeColor);

                if (theme.CardSubTitleForeColor != Color.Empty)
                    return theme.CardSubTitleForeColor;
            }

            return Color.FromArgb(128, 128, 128); // Gray
        }

        /// <summary>
        /// Get color for rating stars
        /// Priority: Custom color > Theme TestimonialRatingColor > Theme AccentColor > Default Gold
        /// </summary>
        public static Color GetRatingColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TestimonialRatingColor != Color.Empty)
                    return theme.TestimonialRatingColor;

                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;

                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
            }

            return Color.Gold;
        }

        #endregion

        #region Bulk Theme Application

        /// <summary>
        /// Apply theme colors to a testimonial control
        /// Updates all color properties based on theme
        /// </summary>
        public static void ApplyThemeColors(
            BeepTestimonial testimonial,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (testimonial == null || theme == null)
                return;

            // Apply background color
            testimonial.BackColor = GetTestimonialBackColor(theme, useThemeColors, null);
            // Note: Individual child control colors are applied in BeepTestimonial.ApplyTheme()
            // This method provides the color values that should be used
        }

        /// <summary>
        /// Get all theme colors for a testimonial
        /// Returns a tuple of (backColor, testimonialTextColor, nameColor, detailsColor, ratingColor)
        /// </summary>
        public static (Color backColor, Color testimonialTextColor, Color nameColor, Color detailsColor, Color ratingColor) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customBackColor = null,
            Color? customTestimonialTextColor = null,
            Color? customNameColor = null,
            Color? customDetailsColor = null,
            Color? customRatingColor = null)
        {
            return (
                GetTestimonialBackColor(theme, useThemeColors, customBackColor),
                GetTestimonialTextColor(theme, useThemeColors, customTestimonialTextColor),
                GetNameColor(theme, useThemeColors, customNameColor),
                GetDetailsColor(theme, useThemeColors, customDetailsColor),
                GetRatingColor(theme, useThemeColors, customRatingColor)
            );
        }

        #endregion
    }
}

