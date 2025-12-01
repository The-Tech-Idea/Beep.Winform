using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Metrices.Helpers
{
    /// <summary>
    /// Centralized theme color management for MetricTile controls
    /// Provides consistent color retrieval based on theme
    /// </summary>
    public static class MetricTileThemeHelpers
    {
        #region Tile Colors

        /// <summary>
        /// Get background color for metric tile
        /// Priority: Custom color > Theme DashboardBackColor > Theme CardBackColor > Default White
        /// </summary>
        public static Color GetTileBackColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.DashboardBackColor != Color.Empty)
                    return theme.DashboardBackColor;

                if (theme.CardBackColor != Color.Empty)
                    return theme.CardBackColor;

                if (theme.BackColor != Color.Empty)
                    return theme.BackColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Get color for tile title text
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
        /// Get color for metric value text
        /// Priority: Custom color > Theme CardTitleForeColor > Theme PrimaryTextColor > Default Black
        /// </summary>
        public static Color GetMetricValueColor(
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
        /// Get color for delta text
        /// Priority: Custom color > Theme CardSubTitleForeColor > Theme SecondaryTextColor > Default Gray
        /// </summary>
        public static Color GetDeltaColor(
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
                    return Color.FromArgb(180, theme.CardTextForeColor);
            }

            return Color.FromArgb(128, 128, 128); // Gray
        }

        /// <summary>
        /// Get color for icon
        /// Priority: Custom color > Theme PrimaryTextColor > Theme ForeColor > Default Black
        /// </summary>
        public static Color GetIconColor(
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
        /// Get gradient colors for tile background
        /// </summary>
        public static (Color startColor, Color endColor) GetGradientColors(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customStart = null,
            Color? customEnd = null)
        {
            if (customStart.HasValue && customEnd.HasValue)
                return (customStart.Value, customEnd.Value);

            if (useThemeColors && theme != null)
            {
                Color start = theme.GradientStartColor != Color.Empty 
                    ? theme.GradientStartColor 
                    : GetTileBackColor(theme, useThemeColors, null);
                
                Color end = theme.GradientEndColor != Color.Empty 
                    ? theme.GradientEndColor 
                    : GetTileBackColor(theme, useThemeColors, null);

                if (customStart.HasValue)
                    start = customStart.Value;
                if (customEnd.HasValue)
                    end = customEnd.Value;

                return (start, end);
            }

            // Default gradient
            Color defaultStart = Color.FromArgb(255, 235, 228, 255); // Light pink-lavender
            Color defaultEnd = Color.FromArgb(255, 215, 233, 255); // Light lavender

            if (customStart.HasValue)
                defaultStart = customStart.Value;
            if (customEnd.HasValue)
                defaultEnd = customEnd.Value;

            return (defaultStart, defaultEnd);
        }

        #endregion

        #region Bulk Theme Application

        /// <summary>
        /// Apply theme colors to a metric tile control
        /// Updates all color properties based on theme
        /// </summary>
        public static void ApplyThemeColors(
            BeepMetricTile tile,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (tile == null || theme == null)
                return;

            // Apply background and gradient colors
            tile.BackColor = GetTileBackColor(theme, useThemeColors, null);
            var (startColor, endColor) = GetGradientColors(theme, useThemeColors, null, null);
            tile.GradientStartColor = startColor;
            tile.GradientEndColor = endColor;
        }

        /// <summary>
        /// Get all theme colors for a metric tile
        /// Returns a tuple of (backColor, titleColor, metricValueColor, deltaColor, iconColor, gradientStart, gradientEnd)
        /// </summary>
        public static (Color backColor, Color titleColor, Color metricValueColor, Color deltaColor, Color iconColor, Color gradientStart, Color gradientEnd) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customBackColor = null,
            Color? customTitleColor = null,
            Color? customMetricValueColor = null,
            Color? customDeltaColor = null,
            Color? customIconColor = null,
            Color? customGradientStart = null,
            Color? customGradientEnd = null)
        {
            var (gradientStart, gradientEnd) = GetGradientColors(theme, useThemeColors, customGradientStart, customGradientEnd);

            return (
                GetTileBackColor(theme, useThemeColors, customBackColor),
                GetTitleColor(theme, useThemeColors, customTitleColor),
                GetMetricValueColor(theme, useThemeColors, customMetricValueColor),
                GetDeltaColor(theme, useThemeColors, customDeltaColor),
                GetIconColor(theme, useThemeColors, customIconColor),
                gradientStart,
                gradientEnd
            );
        }

        #endregion
    }
}

