using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Centralized helper for managing menu bar theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps menu bar states (normal, hovered, selected) to theme colors
    /// </summary>
    public static class MenuThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the menu bar control
        /// Priority: Custom color > Theme Menu Background > Default
        /// </summary>
        public static Color GetMenuBarBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
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
        /// Gets the foreground/text color for the menu bar control
        /// </summary>
        public static Color GetMenuBarForegroundColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.MenuForeColor != Color.Empty)
                    return theme.MenuForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the border color for the menu bar control
        /// </summary>
        public static Color GetMenuBarBorderColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.MenuBorderColor != Color.Empty)
                    return theme.MenuBorderColor;
                if (theme.BorderColor != Color.Empty)
                    return theme.BorderColor;
            }

            return Color.FromArgb(200, 200, 200);
        }

        /// <summary>
        /// Gets the background color for a menu item
        /// </summary>
        public static Color GetMenuItemBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.MenuItemSelectedBackColor != Color.Empty)
                        return theme.MenuItemSelectedBackColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                if (isHovered)
                {
                    if (theme.MenuItemHoverBackColor != Color.Empty)
                        return theme.MenuItemHoverBackColor;
                    if (theme.SurfaceColor != Color.Empty)
                        return ControlPaint.Light(theme.SurfaceColor, 0.1f);
                }
                // Normal state - transparent background
                return Color.Transparent;
            }

            if (isSelected)
                return Color.FromArgb(0, 120, 215); // Windows blue
            if (isHovered)
                return Color.FromArgb(245, 245, 245);
            return Color.Transparent;
        }

        /// <summary>
        /// Gets the foreground/text color for a menu item
        /// </summary>
        public static Color GetMenuItemForegroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.MenuItemSelectedForeColor != Color.Empty)
                        return theme.MenuItemSelectedForeColor;
                }
                if (isHovered)
                {
                    if (theme.MenuItemHoverForeColor != Color.Empty)
                        return theme.MenuItemHoverForeColor;
                }
                if (theme.MenuItemForeColor != Color.Empty)
                    return theme.MenuItemForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            if (isSelected)
                return Color.White;
            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the border color for a menu item
        /// </summary>
        public static Color GetMenuItemBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                if (isHovered)
                {
                    if (theme.BorderColor != Color.Empty)
                        return theme.BorderColor;
                }
                if (theme.BorderColor != Color.Empty)
                    return Color.FromArgb(0, theme.BorderColor); // Transparent by default
            }

            return Color.Transparent;
        }

        /// <summary>
        /// Gets gradient start color for menu bar (if using gradient)
        /// </summary>
        public static Color GetMenuBarGradientStartColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.MenuGradiantStartColor != Color.Empty)
                    return theme.MenuGradiantStartColor;
                if (theme.GradientStartColor != Color.Empty)
                    return theme.GradientStartColor;
            }

            return Color.Empty;
        }

        /// <summary>
        /// Gets gradient end color for menu bar (if using gradient)
        /// </summary>
        public static Color GetMenuBarGradientEndColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.MenuGradiantEndColor != Color.Empty)
                    return theme.MenuGradiantEndColor;
                if (theme.GradientEndColor != Color.Empty)
                    return theme.GradientEndColor;
            }

            return Color.Empty;
        }

        /// <summary>
        /// Gets all theme colors for a menu bar in one call
        /// </summary>
        public static (Color background, Color foreground, Color border, Color gradientStart, Color gradientEnd) GetMenuBarColors(
            IBeepTheme theme,
            bool useThemeColors)
        {
            return (
                GetMenuBarBackgroundColor(theme, useThemeColors),
                GetMenuBarForegroundColor(theme, useThemeColors),
                GetMenuBarBorderColor(theme, useThemeColors),
                GetMenuBarGradientStartColor(theme, useThemeColors),
                GetMenuBarGradientEndColor(theme, useThemeColors)
            );
        }

        /// <summary>
        /// Gets all theme colors for a menu item in one call
        /// </summary>
        public static (Color background, Color foreground, Color border) GetMenuItemColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false)
        {
            return (
                GetMenuItemBackgroundColor(theme, useThemeColors, isHovered, isSelected),
                GetMenuItemForegroundColor(theme, useThemeColors, isHovered, isSelected),
                GetMenuItemBorderColor(theme, useThemeColors, isHovered, isSelected)
            );
        }
    }
}
