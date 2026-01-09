using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers
{
    /// <summary>
    /// Centralized helper for managing radio group theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps radio item states (normal, hovered, selected, focused, disabled) to theme colors
    /// </summary>
    public static class RadioGroupThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the radio group
        /// Priority: Custom color > Theme Background > Default
        /// </summary>
        public static Color GetGroupBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Gets the item background color
        /// Priority: Custom color > Theme Surface > Default
        /// </summary>
        public static Color GetItemBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false,
            bool isFocused = false,
            bool isPressed = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isPressed)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return ControlPaint.Light(theme.PrimaryColor, 0.85f);
                }
                else if (isSelected)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return ControlPaint.Light(theme.PrimaryColor, 0.95f);
                }
                else if (isFocused)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return ControlPaint.Light(theme.PrimaryColor, 0.98f);
                }
                else if (isHovered)
                {
                    if (theme.SurfaceColor != Color.Empty)
                        return ControlPaint.Light(theme.SurfaceColor, 0.05f);
                }
                else
                {
                    if (theme.SurfaceColor != Color.Empty)
                        return theme.SurfaceColor;
                    if (theme.BackgroundColor != Color.Empty)
                        return theme.BackgroundColor;
                }
            }

            if (isPressed)
                return Color.FromArgb(240, 240, 250);
            if (isSelected)
                return Color.FromArgb(245, 245, 255);
            if (isFocused)
                return Color.FromArgb(250, 250, 255);
            if (isHovered)
                return Color.FromArgb(250, 250, 250);
            return Color.White;
        }

        /// <summary>
        /// Gets the radio/checkbox indicator color
        /// </summary>
        public static Color GetIndicatorColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isDisabled = false)
        {
            if (isDisabled)
            {
                if (useThemeColors && theme != null && theme.DisabledForeColor != Color.Empty)
                    return theme.DisabledForeColor;
                return Color.FromArgb(180, 180, 180);
            }

            if (isSelected)
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

            if (useThemeColors && theme != null)
            {
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(97, 97, 97); // Material Gray
        }

        /// <summary>
        /// Gets the border color
        /// </summary>
        public static Color GetBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isFocused = false,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isFocused || isSelected)
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

            if (isFocused || isSelected)
                return Color.FromArgb(33, 150, 243); // Material Blue
            if (isHovered)
                return Color.FromArgb(158, 158, 158); // Material Gray
            return Color.FromArgb(224, 224, 224); // Light Gray
        }

        /// <summary>
        /// Gets the text color
        /// </summary>
        public static Color GetTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isDisabled = false)
        {
            if (isDisabled)
            {
                if (useThemeColors && theme != null && theme.DisabledForeColor != Color.Empty)
                    return theme.DisabledForeColor;
                return Color.FromArgb(180, 180, 180);
            }

            if (useThemeColors && theme != null)
            {
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return isSelected
                ? Color.FromArgb(33, 37, 41) // Dark gray
                : Color.FromArgb(73, 80, 87); // Medium gray
        }

        /// <summary>
        /// Gets the ripple/state layer color
        /// </summary>
        public static Color GetStateLayerColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isFocused = false,
            bool isPressed = false,
            int opacity = 12)
        {
            Color baseColor;

            if (useThemeColors && theme != null)
            {
                if (isPressed)
                {
                    baseColor = theme.PrimaryColor != Color.Empty ? theme.PrimaryColor : Color.Black;
                }
                else if (isFocused)
                {
                    baseColor = theme.PrimaryColor != Color.Empty ? theme.PrimaryColor : Color.Black;
                }
                else if (isHovered)
                {
                    baseColor = theme.PrimaryColor != Color.Empty ? theme.PrimaryColor : Color.Black;
                }
                else
                {
                    baseColor = Color.Black;
                }
            }
            else
            {
                baseColor = isPressed || isFocused || isHovered
                    ? Color.FromArgb(33, 150, 243) // Material Blue
                    : Color.Black;
            }

            return Color.FromArgb(Math.Min(255, opacity), baseColor);
        }

        /// <summary>
        /// Gets all theme colors for a radio item in one call
        /// </summary>
        public static (Color itemBg, Color indicator, Color border, Color text, Color stateLayer) GetItemColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false,
            bool isFocused = false,
            bool isPressed = false,
            bool isDisabled = false)
        {
            return (
                GetItemBackgroundColor(theme, useThemeColors, isHovered, isSelected, isFocused, isPressed),
                GetIndicatorColor(theme, useThemeColors, isSelected, isDisabled),
                GetBorderColor(theme, useThemeColors, isSelected, isFocused, isHovered),
                GetTextColor(theme, useThemeColors, isSelected, isDisabled),
                GetStateLayerColor(theme, useThemeColors, isHovered, isFocused, isPressed)
            );
        }
    }
}
