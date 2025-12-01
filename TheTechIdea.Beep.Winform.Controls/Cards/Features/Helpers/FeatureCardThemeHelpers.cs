using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Features.Helpers
{
    /// <summary>
    /// Centralized theme color management for FeatureCard controls
    /// Provides consistent color retrieval based on theme
    /// </summary>
    public static class FeatureCardThemeHelpers
    {
        #region Card Colors

        /// <summary>
        /// Get background color for feature card
        /// Priority: Custom color > Theme CardBackColor > Default White
        /// </summary>
        public static Color GetCardBackColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CardBackColor != Color.Empty)
                    return theme.CardBackColor;

                if (theme.BackColor != Color.Empty)
                    return theme.BackColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Get color for card title text
        /// Priority: Custom color > Theme CardTitleForeColor > Theme PrimaryTextColor > Default Black
        /// </summary>
        public static Color GetTitleColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
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
        /// Get color for card subtitle text
        /// Priority: Custom color > Theme CardTitleForeColor > Theme SecondaryTextColor > Default Dark Gray
        /// </summary>
        public static Color GetSubtitleColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CardTitleForeColor != Color.Empty)
                    return Color.FromArgb(180, theme.CardTitleForeColor); // Slightly transparent

                if (theme.SecondaryTextColor != Color.Empty)
                    return theme.SecondaryTextColor;

                if (theme.CardTextForeColor != Color.Empty)
                    return Color.FromArgb(180, theme.CardTextForeColor);
            }

            return Color.FromArgb(128, 128, 128); // Dark gray
        }

        /// <summary>
        /// Get color for bullet point text
        /// Priority: Custom color > Theme CardTextForeColor > Theme PrimaryTextColor > Default Black
        /// </summary>
        public static Color GetBulletPointColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
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
        /// Get color for action icons
        /// Priority: Custom color > Theme PrimaryTextColor > Theme ForeColor > Default Black
        /// </summary>
        public static Color GetActionIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.PrimaryTextColor != Color.Empty)
                    return theme.PrimaryTextColor;

                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;

                if (theme.CardTextForeColor != Color.Empty)
                    return theme.CardTextForeColor;
            }

            return Color.Black;
        }

        /// <summary>
        /// Get color for card icon (large decorative icon)
        /// Priority: Custom color > Theme AccentColor > Theme PrimaryColor > Default Gray
        /// </summary>
        public static Color GetCardIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.AccentColor != Color.Empty)
                    return Color.FromArgb(50, theme.AccentColor); // Semi-transparent

                if (theme.PrimaryColor != Color.Empty)
                    return Color.FromArgb(50, theme.PrimaryColor);

                if (theme.SecondaryColor != Color.Empty)
                    return Color.FromArgb(50, theme.SecondaryColor);
            }

            return Color.FromArgb(128, 128, 128); // Gray
        }

        #endregion

        #region Bulk Theme Application

        /// <summary>
        /// Apply theme colors to a feature card control
        /// Updates all color properties based on theme
        /// </summary>
        public static void ApplyThemeColors(
            BeepFeatureCard card,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (card == null || theme == null)
                return;

            // Apply background color
            card.BackColor = GetCardBackColor(theme, useThemeColors, null);
            card.ParentBackColor = GetCardBackColor(theme, useThemeColors, null);

            // Note: Individual child control colors are applied in BeepFeatureCard.ApplyTheme()
            // This method provides the color values that should be used
        }

        /// <summary>
        /// Get all theme colors for a feature card
        /// Returns a tuple of (backColor, titleColor, subtitleColor, bulletColor, actionIconColor, cardIconColor)
        /// </summary>
        public static (Color backColor, Color titleColor, Color subtitleColor, Color bulletColor, Color actionIconColor, Color cardIconColor) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customBackColor = null,
            Color? customTitleColor = null,
            Color? customSubtitleColor = null,
            Color? customBulletColor = null,
            Color? customActionIconColor = null,
            Color? customCardIconColor = null)
        {
            return (
                GetCardBackColor(theme, useThemeColors, customBackColor),
                GetTitleColor(theme, useThemeColors, customTitleColor),
                GetSubtitleColor(theme, useThemeColors, customSubtitleColor),
                GetBulletPointColor(theme, useThemeColors, customBulletColor),
                GetActionIconColor(theme, useThemeColors, customActionIconColor),
                GetCardIconColor(theme, useThemeColors, customCardIconColor)
            );
        }

        #endregion
    }
}

