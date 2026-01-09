using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers
{
    /// <summary>
    /// Centralized helper for managing accordion menu theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps accordion menu states to theme colors
    /// </summary>
    public static class AccordionThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the accordion menu control
        /// Priority: Custom color > Theme Side Menu Background > Default
        /// </summary>
        public static Color GetAccordionBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.SideMenuBackColor != Color.Empty)
                    return theme.SideMenuBackColor;
                if (theme.MenuBackColor != Color.Empty)
                    return theme.MenuBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
            }

            return Color.FromArgb(240, 240, 240);
        }

        /// <summary>
        /// Gets the header/title background color
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
                if (theme.SideMenuBackColor != Color.Empty)
                    return theme.SideMenuBackColor;
                if (theme.MenuBackColor != Color.Empty)
                    return theme.MenuBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return Color.FromArgb(240, 240, 240);
        }

        /// <summary>
        /// Gets the foreground/text color for the accordion menu
        /// </summary>
        public static Color GetAccordionForegroundColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.SideMenuForeColor != Color.Empty)
                    return theme.SideMenuForeColor;
                if (theme.MenuForeColor != Color.Empty)
                    return theme.MenuForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the background color for menu items
        /// </summary>
        public static Color GetItemBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.MenuMainItemSelectedBackColor != Color.Empty)
                        return theme.MenuMainItemSelectedBackColor;
                    if (theme.ButtonSelectedBackColor != Color.Empty)
                        return theme.ButtonSelectedBackColor;
                }
                if (isHovered)
                {
                    if (theme.MenuMainItemHoverBackColor != Color.Empty)
                        return theme.MenuMainItemHoverBackColor;
                    if (theme.ButtonHoverBackColor != Color.Empty)
                        return theme.ButtonHoverBackColor;
                }
                // Normal state - transparent or subtle background
                return Color.Transparent;
            }

            // Default colors
            if (isSelected)
                return Color.FromArgb(230, 240, 255);
            if (isHovered)
                return Color.FromArgb(245, 245, 250);
            return Color.Transparent;
        }

        /// <summary>
        /// Gets the foreground/text color for menu items
        /// </summary>
        public static Color GetItemForegroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.MenuMainItemSelectedForeColor != Color.Empty)
                        return theme.MenuMainItemSelectedForeColor;
                    if (theme.ButtonSelectedForeColor != Color.Empty)
                        return theme.ButtonSelectedForeColor;
                }
                if (theme.SideMenuForeColor != Color.Empty)
                    return theme.SideMenuForeColor;
                if (theme.MenuForeColor != Color.Empty)
                    return theme.MenuForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return isSelected ? Color.FromArgb(33, 150, 243) : Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the border color for menu items
        /// </summary>
        public static Color GetItemBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                if (theme.BorderColor != Color.Empty)
                    return Color.FromArgb(50, theme.BorderColor);
            }

            return Color.Transparent;
        }

        /// <summary>
        /// Gets the highlight color for the left border indicator
        /// </summary>
        public static Color GetHighlightColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected || isHovered)
                {
                    if (theme.MenuMainItemHoverForeColor != Color.Empty)
                        return theme.MenuMainItemHoverForeColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                    if (theme.AccentColor != Color.Empty)
                        return theme.AccentColor;
                }
            }

            return isSelected || isHovered ? Color.FromArgb(33, 150, 243) : Color.Transparent;
        }

        /// <summary>
        /// Gets the expand/collapse icon color
        /// </summary>
        public static Color GetExpanderIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isExpanded = false)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.SideMenuForeColor != Color.Empty)
                    return theme.SideMenuForeColor;
                if (theme.MenuForeColor != Color.Empty)
                    return theme.MenuForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(100, 100, 100);
        }

        /// <summary>
        /// Gets the connector line color for child items
        /// </summary>
        public static Color GetConnectorLineColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.BorderColor != Color.Empty)
                    return Color.FromArgb(100, theme.BorderColor);
            }

            return Color.FromArgb(100, 200, 200, 200);
        }

        /// <summary>
        /// Gets the header/title foreground color
        /// </summary>
        public static Color GetHeaderForegroundColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.SideMenuForeColor != Color.Empty)
                    return theme.SideMenuForeColor;
                if (theme.MenuForeColor != Color.Empty)
                    return theme.MenuForeColor;
                if (theme.LabelForeColor != Color.Empty)
                    return theme.LabelForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets all theme colors for an accordion item in one call
        /// </summary>
        public static (Color background, Color foreground, Color border, Color highlight, Color expander) GetItemColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false,
            bool isExpanded = false)
        {
            return (
                GetItemBackgroundColor(theme, useThemeColors, isHovered, isSelected),
                GetItemForegroundColor(theme, useThemeColors, isSelected),
                GetItemBorderColor(theme, useThemeColors, isSelected),
                GetHighlightColor(theme, useThemeColors, isHovered, isSelected),
                GetExpanderIconColor(theme, useThemeColors, isExpanded)
            );
        }
    }
}
