using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    /// <summary>
    /// Centralized helper for managing breadcrumb theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps breadcrumb states (normal, hovered, selected, last) to theme colors
    /// </summary>
    public static class BreadcrumbThemeHelpers
    {
        /// <summary>
        /// Gets the text color for a breadcrumb item
        /// Priority: Custom color > Theme LinkColor > Default blue
        /// </summary>
        public static Color GetItemTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isLast,
            bool isHovered,
            Color? customColor = null)
        {
            // Priority 1: Custom color (highest priority)
            if (customColor.HasValue)
            {
                return customColor.Value;
            }

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                if (isLast)
                {
                    // Last item uses foreground color (not a link)
                    return theme.ForeColor != Color.Empty ? theme.ForeColor : theme.ButtonForeColor;
                }
                else
                {
                    // Non-last items use link color
                    return theme.LinkColor != Color.Empty ? theme.LinkColor : theme.PrimaryColor;
                }
            }

            // Priority 3: Default fallback
            return isLast ? Color.Black : Color.FromArgb(0, 102, 204); // Default blue for links
        }

        /// <summary>
        /// Gets the background color for a breadcrumb item when hovered
        /// Priority: Custom color > Theme ButtonHoverBackColor > Default light gray
        /// </summary>
        public static Color GetItemHoverBackColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
            {
                return customColor.Value;
            }

            if (useThemeColors && theme != null)
            {
                return theme.ButtonHoverBackColor != Color.Empty 
                    ? Color.FromArgb(40, theme.ButtonHoverBackColor) 
                    : Color.FromArgb(40, theme.PrimaryColor);
            }

            return Color.FromArgb(40, Color.LightGray); // Default light gray with opacity
        }

        /// <summary>
        /// Gets the background color for a breadcrumb item when selected
        /// Priority: Custom color > Theme ButtonBackColor > Default gray
        /// </summary>
        public static Color GetItemSelectedBackColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
            {
                return customColor.Value;
            }

            if (useThemeColors && theme != null)
            {
                return theme.ButtonBackColor != Color.Empty 
                    ? Color.FromArgb(80, theme.ButtonBackColor) 
                    : Color.FromArgb(80, theme.PrimaryColor);
            }

            return Color.FromArgb(80, Color.Gray); // Default gray with opacity
        }

        /// <summary>
        /// Gets the separator color between breadcrumb items
        /// Priority: Custom color > Theme LabelForeColor with opacity > Default gray
        /// </summary>
        public static Color GetSeparatorColor(
            IBeepTheme theme,
            bool useThemeColors,
            float opacity = 0.5f,
            Color? customColor = null)
        {
            if (customColor.HasValue)
            {
                return Color.FromArgb((int)(255 * opacity), customColor.Value);
            }

            if (useThemeColors && theme != null)
            {
                Color baseColor = theme.LabelForeColor != Color.Empty 
                    ? theme.LabelForeColor 
                    : theme.ForeColor;
                return Color.FromArgb((int)(255 * opacity), baseColor);
            }

            return Color.FromArgb((int)(255 * opacity), Color.Gray); // Default gray with opacity
        }

        /// <summary>
        /// Gets the background color for the breadcrumb control
        /// Priority: Custom color > Theme PanelBackColor > Default white
        /// </summary>
        public static Color GetBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
            {
                return customColor.Value;
            }

            if (useThemeColors && theme != null)
            {
                return theme.PanelBackColor != Color.Empty 
                    ? theme.PanelBackColor 
                    : theme.BackColor;
            }

            return Color.White; // Default white
        }

        /// <summary>
        /// Gets the border color for breadcrumb items (if needed)
        /// Priority: Custom color > Theme ButtonHoverBorderColor > Default gray
        /// </summary>
        public static Color GetItemBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered,
            Color? customColor = null)
        {
            if (customColor.HasValue)
            {
                return customColor.Value;
            }

            if (useThemeColors && theme != null)
            {
                if (isHovered)
                {
                    return theme.ButtonHoverBorderColor != Color.Empty 
                        ? theme.ButtonHoverBorderColor 
                        : theme.BorderColor;
                }
                return theme.BorderColor != Color.Empty ? theme.BorderColor : Color.Transparent;
            }

            return isHovered ? Color.FromArgb(200, Color.Gray) : Color.Transparent;
        }

        /// <summary>
        /// Gets all relevant theme colors for a breadcrumb item in one go
        /// </summary>
        public static (Color textColor, Color hoverBackColor, Color selectedBackColor, Color separatorColor, Color borderColor) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isLast,
            bool isHovered,
            bool isSelected,
            float separatorOpacity = 0.5f)
        {
            Color text = GetItemTextColor(theme, useThemeColors, isLast, isHovered);
            Color hover = GetItemHoverBackColor(theme, useThemeColors);
            Color selected = GetItemSelectedBackColor(theme, useThemeColors);
            Color separator = GetSeparatorColor(theme, useThemeColors, separatorOpacity);
            Color border = GetItemBorderColor(theme, useThemeColors, isHovered);
            
            return (text, hover, selected, separator, border);
        }

        /// <summary>
        /// Applies theme colors to the BeepBreadcrump control properties
        /// </summary>
        public static void ApplyThemeColors(
            BeepBreadcrump breadcrumb,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (breadcrumb == null || theme == null || !useThemeColors) return;

            // Apply background color
            breadcrumb.BackColor = GetBackgroundColor(theme, useThemeColors, breadcrumb.BackColor);
            
            // Apply foreground color (for separators and default text)
            breadcrumb.ForeColor = GetSeparatorColor(theme, useThemeColors, 1.0f, breadcrumb.ForeColor);
        }
    }
}

