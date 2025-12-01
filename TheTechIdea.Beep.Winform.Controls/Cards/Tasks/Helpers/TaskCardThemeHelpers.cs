using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Tasks.Helpers
{
    /// <summary>
    /// Centralized theme color management for TaskCard controls
    /// Provides consistent color retrieval based on theme
    /// </summary>
    public static class TaskCardThemeHelpers
    {
        #region Task Card Colors

        /// <summary>
        /// Get background color for task card
        /// Priority: Custom color > Theme DashboardBackColor > Theme CardBackColor > Default White
        /// </summary>
        public static Color GetTaskCardBackColor(
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

                if (theme.TaskCardBackColor != Color.Empty)
                    return theme.TaskCardBackColor;

                if (theme.CardBackColor != Color.Empty)
                    return theme.CardBackColor;

                if (theme.BackColor != Color.Empty)
                    return theme.BackColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Get color for task card title text
        /// Priority: Custom color > Theme TaskCardTitleForeColor > Theme CardTitleForeColor > Default Black
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
                if (theme.TaskCardTitleForeColor != Color.Empty)
                    return theme.TaskCardTitleForeColor;

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
        /// Get color for task card subtitle text
        /// Priority: Custom color > Theme TaskCardSubTitleForeColor > Theme CardSubTitleForeColor > Default Gray
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
                if (theme.TaskCardSubTitleForeColor != Color.Empty)
                    return theme.TaskCardSubTitleForeColor;

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
        /// Get color for metric text
        /// Priority: Custom color > Theme TaskCardMetricTextForeColor > Theme CardTextForeColor > Default Black
        /// </summary>
        public static Color GetMetricTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TaskCardMetricTextForeColor != Color.Empty)
                    return theme.TaskCardMetricTextForeColor;

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
        /// Get color for progress bar fill
        /// Priority: Custom color > Theme TaskCardMetricTextForeColor > Theme AccentColor > Default Blue
        /// </summary>
        public static Color GetProgressBarColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TaskCardMetricTextForeColor != Color.Empty)
                    return theme.TaskCardMetricTextForeColor;

                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;

                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
            }

            return Color.FromArgb(0, 120, 215); // Blue
        }

        /// <summary>
        /// Get color for progress bar background
        /// Priority: Custom color > Theme TaskCardMetricTextForeColor (semi-transparent) > Default Gray
        /// </summary>
        public static Color GetProgressBarBackColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TaskCardMetricTextForeColor != Color.Empty)
                    return Color.FromArgb(80, theme.TaskCardMetricTextForeColor);

                if (theme.SecondaryColor != Color.Empty)
                    return Color.FromArgb(80, theme.SecondaryColor);
            }

            return Color.FromArgb(80, 128, 128, 128); // Semi-transparent gray
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
        /// Get gradient colors for task card background
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
                    : GetTaskCardBackColor(theme, useThemeColors, null);
                
                Color end = theme.GradientEndColor != Color.Empty 
                    ? theme.GradientEndColor 
                    : GetTaskCardBackColor(theme, useThemeColors, null);

                if (customStart.HasValue)
                    start = customStart.Value;
                if (customEnd.HasValue)
                    end = customEnd.Value;

                return (start, end);
            }

            // Default gradient
            Color defaultStart = Color.FromArgb(255, 255, 182, 193); // Light pink
            Color defaultEnd = Color.FromArgb(255, 255, 153, 187); // Darker pink

            if (customStart.HasValue)
                defaultStart = customStart.Value;
            if (customEnd.HasValue)
                defaultEnd = customEnd.Value;

            return (defaultStart, defaultEnd);
        }

        #endregion

        #region Bulk Theme Application

        /// <summary>
        /// Apply theme colors to a task card control
        /// Updates all color properties based on theme
        /// </summary>
        public static void ApplyThemeColors(
            BeepTaskCard card,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (card == null || theme == null)
                return;

            // Apply background and gradient colors
            card.BackColor = GetTaskCardBackColor(theme, useThemeColors, null);
            var (startColor, endColor) = GetGradientColors(theme, useThemeColors, null, null);
            card.GradientStartColor = startColor;
            card.GradientEndColor = endColor;
            card.BorderColor = GetTaskCardBackColor(theme, useThemeColors, null);
            card.ForeColor = GetMetricTextColor(theme, useThemeColors, null);
        }

        /// <summary>
        /// Get all theme colors for a task card
        /// Returns a tuple of (backColor, titleColor, subtitleColor, metricColor, progressColor, progressBackColor, iconColor, gradientStart, gradientEnd)
        /// </summary>
        public static (Color backColor, Color titleColor, Color subtitleColor, Color metricColor, Color progressColor, Color progressBackColor, Color iconColor, Color gradientStart, Color gradientEnd) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customBackColor = null,
            Color? customTitleColor = null,
            Color? customSubtitleColor = null,
            Color? customMetricColor = null,
            Color? customProgressColor = null,
            Color? customProgressBackColor = null,
            Color? customIconColor = null,
            Color? customGradientStart = null,
            Color? customGradientEnd = null)
        {
            var (gradientStart, gradientEnd) = GetGradientColors(theme, useThemeColors, customGradientStart, customGradientEnd);

            return (
                GetTaskCardBackColor(theme, useThemeColors, customBackColor),
                GetTitleColor(theme, useThemeColors, customTitleColor),
                GetSubtitleColor(theme, useThemeColors, customSubtitleColor),
                GetMetricTextColor(theme, useThemeColors, customMetricColor),
                GetProgressBarColor(theme, useThemeColors, customProgressColor),
                GetProgressBarBackColor(theme, useThemeColors, customProgressBackColor),
                GetIconColor(theme, useThemeColors, customIconColor),
                gradientStart,
                gradientEnd
            );
        }

        #endregion
    }
}

