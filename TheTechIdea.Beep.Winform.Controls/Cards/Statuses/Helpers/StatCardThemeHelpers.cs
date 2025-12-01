using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards.Helpers
{
    /// <summary>
    /// Centralized theme color management for StatCard controls
    /// Provides consistent color retrieval based on theme
    /// </summary>
    public static class StatCardThemeHelpers
    {
        #region Stat Card Colors

        /// <summary>
        /// Get background color for stat card
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
        /// Get color for header text
        /// Priority: Custom color > Theme CardTitleForeColor > Theme PrimaryTextColor > Default Black
        /// </summary>
        public static Color GetHeaderColor(
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
        /// Get color for value text
        /// Priority: Custom color > Theme CardTextForeColor > Theme PrimaryTextColor > Default Black
        /// </summary>
        public static Color GetValueColor(
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
        /// Get color for delta text
        /// Priority: Custom color > Theme CardSubTitleForeColor > Theme SecondaryTextColor > Default Gray
        /// </summary>
        public static Color GetDeltaColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isPositive,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CardSubTitleForeColor != Color.Empty)
                    return theme.CardSubTitleForeColor;

                if (theme.SecondaryTextColor != Color.Empty)
                    return theme.SecondaryTextColor;

                if (theme.CardTextForeColor != Color.Empty)
                    return Color.FromArgb(180, theme.CardTextForeColor);
            }

            return Color.FromArgb(128, 128, 128); // Gray
        }

        /// <summary>
        /// Get color for info text
        /// Priority: Custom color > Theme CardSubTitleForeColor > Theme SecondaryTextColor > Default Gray
        /// </summary>
        public static Color GetInfoColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CardSubTitleForeColor != Color.Empty)
                    return theme.CardSubTitleForeColor;

                if (theme.SecondaryTextColor != Color.Empty)
                    return theme.SecondaryTextColor;

                if (theme.CardTextForeColor != Color.Empty)
                    return Color.FromArgb(160, theme.CardTextForeColor);
            }

            return Color.FromArgb(128, 128, 128); // Gray
        }

        /// <summary>
        /// Get color for trend up icon
        /// Priority: Custom color > Theme AccentColor > Theme PrimaryColor > Default Green
        /// </summary>
        public static Color GetTrendUpColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;

                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
            }

            return Color.FromArgb(0, 150, 0); // Green
        }

        /// <summary>
        /// Get color for trend down icon
        /// Priority: Custom color > Theme ErrorColor > Theme WarningColor > Default Red
        /// </summary>
        public static Color GetTrendDownColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ErrorColor != Color.Empty)
                    return theme.ErrorColor;

                if (theme.WarningColor != Color.Empty)
                    return theme.WarningColor;
            }

            return Color.FromArgb(200, 0, 0); // Red
        }

        #endregion

        #region Bulk Theme Application

        /// <summary>
        /// Apply theme colors to a stat card control
        /// Updates all color properties based on theme
        /// </summary>
        public static void ApplyThemeColors(
            BeepStatCard card,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (card == null || theme == null)
                return;

            // Apply background color
            card.BackColor = GetCardBackColor(theme, useThemeColors, null);
            card.ParentBackColor = GetCardBackColor(theme, useThemeColors, null);
        }

        /// <summary>
        /// Get all theme colors for a stat card
        /// Returns a tuple of (backColor, headerColor, valueColor, deltaColor, infoColor, trendUpColor, trendDownColor)
        /// </summary>
        public static (Color backColor, Color headerColor, Color valueColor, Color deltaColor, Color infoColor, Color trendUpColor, Color trendDownColor) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isTrendingUp,
            Color? customBackColor = null,
            Color? customHeaderColor = null,
            Color? customValueColor = null,
            Color? customDeltaColor = null,
            Color? customInfoColor = null,
            Color? customTrendUpColor = null,
            Color? customTrendDownColor = null)
        {
            return (
                GetCardBackColor(theme, useThemeColors, customBackColor),
                GetHeaderColor(theme, useThemeColors, customHeaderColor),
                GetValueColor(theme, useThemeColors, customValueColor),
                GetDeltaColor(theme, useThemeColors, isTrendingUp, customDeltaColor),
                GetInfoColor(theme, useThemeColors, customInfoColor),
                GetTrendUpColor(theme, useThemeColors, customTrendUpColor),
                GetTrendDownColor(theme, useThemeColors, customTrendDownColor)
            );
        }

        #endregion
    }
}

