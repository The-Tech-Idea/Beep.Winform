using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Centralized helper for managing tab theme colors
    /// Integrates with ApplyTheme() pattern
    /// Maps tab states (normal, hovered, selected) to theme colors
    /// </summary>
    public static class TabThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the tab control
        /// Priority: Custom color > Theme Background > Default
        /// </summary>
        public static Color GetTabControlBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TabBackColor != Color.Empty)
                    return theme.TabBackColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Gets the header background color
        /// </summary>
        public static Color GetHeaderBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TabBackColor != Color.Empty)
                    return theme.TabBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
            }

            return Color.FromArgb(245, 245, 250);
        }

        /// <summary>
        /// Gets the tab background color
        /// </summary>
        public static Color GetTabBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.TabSelectedBackColor != Color.Empty)
                        return theme.TabSelectedBackColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return ControlPaint.Light(theme.PrimaryColor, 0.95f);
                }
                else if (isHovered)
                {
                    if (theme.TabBackColor != Color.Empty)
                        return ControlPaint.Light(theme.TabBackColor, 0.05f);
                    if (theme.SurfaceColor != Color.Empty)
                        return ControlPaint.Light(theme.SurfaceColor, 0.05f);
                }
                else
                {
                    if (theme.TabBackColor != Color.Empty)
                        return theme.TabBackColor;
                    if (theme.SurfaceColor != Color.Empty)
                        return theme.SurfaceColor;
                }
            }

            if (isSelected)
                return Color.White;
            if (isHovered)
                return Color.FromArgb(250, 250, 250);
            return Color.FromArgb(240, 240, 245);
        }

        /// <summary>
        /// Gets the tab text color
        /// </summary>
        public static Color GetTabTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.TabSelectedForeColor != Color.Empty)
                        return theme.TabSelectedForeColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
                else
                {
                    if (theme.TabForeColor != Color.Empty)
                        return theme.TabForeColor;
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
            }

            return isSelected
                ? Color.FromArgb(33, 37, 41) // Dark gray
                : Color.FromArgb(97, 97, 97); // Medium gray
        }

        /// <summary>
        /// Gets the border color for tabs
        /// </summary>
        public static Color GetTabBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                    if (theme.AccentColor != Color.Empty)
                        return theme.AccentColor;
                }
                else if (isHovered)
                {
                    if (theme.SecondaryColor != Color.Empty)
                        return theme.SecondaryColor;
                }
                else
                {
                    if (theme.BorderColor != Color.Empty)
                        return theme.BorderColor;
                }
            }

            if (isSelected)
                return Color.FromArgb(33, 150, 243); // Material Blue
            if (isHovered)
                return Color.FromArgb(158, 158, 158); // Material Gray
            return Color.FromArgb(224, 224, 224); // Light Gray
        }

        /// <summary>
        /// Gets the underline/indicator color for selected tabs
        /// </summary>
        public static Color GetTabIndicatorColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
            }

            return Color.FromArgb(33, 150, 243); // Material Blue
        }

        /// <summary>
        /// Gets the close button color
        /// </summary>
        public static Color GetCloseButtonColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isHovered)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                else
                {
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
            }

            return isHovered
                ? Color.FromArgb(33, 150, 243) // Material Blue
                : Color.FromArgb(158, 158, 158); // Material Gray
        }

        /// <summary>
        /// Gets all theme colors for a tab in one call
        /// </summary>
        public static (Color tabBg, Color border, Color text, Color indicator) GetTabColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false)
        {
            return (
                GetTabBackgroundColor(theme, useThemeColors, isSelected, isHovered),
                GetTabBorderColor(theme, useThemeColors, isSelected, isHovered),
                GetTabTextColor(theme, useThemeColors, isSelected, isHovered),
                GetTabIndicatorColor(theme, useThemeColors)
            );
        }
    }
}
