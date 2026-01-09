using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Lovs.Helpers
{
    /// <summary>
    /// Centralized helper for managing LOV control theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps LOV control states to theme colors
    /// </summary>
    public static class LovThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the LOV control
        /// Priority: Custom color > Theme Background > Default
        /// </summary>
        public static Color GetLovBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TextBoxBackColor != Color.Empty)
                    return theme.TextBoxBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
            }

            return SystemColors.Window;
        }

        /// <summary>
        /// Gets the foreground/text color for the LOV control
        /// </summary>
        public static Color GetLovForegroundColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.TextBoxForeColor != Color.Empty)
                    return theme.TextBoxForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the border color for the LOV control
        /// </summary>
        public static Color GetLovBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isFocused = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isFocused)
                {
                    if (theme.TextBoxSelectedBorderColor != Color.Empty)
                        return theme.TextBoxSelectedBorderColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                if (isHovered)
                {
                    if (theme.TextBoxHoverBorderColor != Color.Empty)
                        return theme.TextBoxHoverBorderColor;
                }
                if (theme.TextBoxBorderColor != Color.Empty)
                    return theme.TextBoxBorderColor;
                if (theme.BorderColor != Color.Empty)
                    return theme.BorderColor;
            }

            return isFocused
                ? Color.FromArgb(0, 120, 215) // Windows blue
                : Color.FromArgb(200, 200, 200);
        }

        /// <summary>
        /// Gets the button background color for dropdown button
        /// </summary>
        public static Color GetButtonBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isPressed = false)
        {
            if (isPressed)
            {
                if (useThemeColors && theme != null)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return ControlPaint.Dark(theme.PrimaryColor, 0.1f);
                }
                return Color.FromArgb(200, 200, 200);
            }

            if (isHovered)
            {
                if (useThemeColors && theme != null)
                {
                    if (theme.SurfaceColor != Color.Empty)
                        return ControlPaint.Light(theme.SurfaceColor, 0.1f);
                }
                return Color.FromArgb(245, 245, 245);
            }

            if (useThemeColors && theme != null)
            {
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return Color.FromArgb(250, 250, 250);
        }

        /// <summary>
        /// Gets the button icon/text color for dropdown button
        /// </summary>
        public static Color GetButtonIconColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(100, 100, 100);
        }

        /// <summary>
        /// Gets the error/invalid input color
        /// </summary>
        public static Color GetErrorColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ErrorColor != Color.Empty)
                    return theme.ErrorColor;
            }

            return Color.FromArgb(220, 53, 69); // Bootstrap danger red
        }

        /// <summary>
        /// Gets all theme colors for a LOV control in one call
        /// </summary>
        public static (Color background, Color foreground, Color border, Color buttonBg, Color buttonIcon, Color error) GetLovColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isFocused = false,
            bool isButtonHovered = false,
            bool isButtonPressed = false)
        {
            return (
                GetLovBackgroundColor(theme, useThemeColors),
                GetLovForegroundColor(theme, useThemeColors),
                GetLovBorderColor(theme, useThemeColors, isHovered, isFocused),
                GetButtonBackgroundColor(theme, useThemeColors, isButtonHovered, isButtonPressed),
                GetButtonIconColor(theme, useThemeColors),
                GetErrorColor(theme, useThemeColors)
            );
        }
    }
}
