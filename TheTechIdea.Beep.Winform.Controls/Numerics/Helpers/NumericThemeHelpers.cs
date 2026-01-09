using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Helpers
{
    /// <summary>
    /// Centralized helper for managing numeric control theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps numeric control states (normal, hovered, focused, disabled) to theme colors
    /// </summary>
    public static class NumericThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the numeric control
        /// Priority: Custom color > Theme TextBox Background > Default
        /// </summary>
        public static Color GetNumericBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isFocused = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isFocused)
                {
                    if (theme.TextBoxSelectedBackColor != Color.Empty)
                        return theme.TextBoxSelectedBackColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return ControlPaint.Light(theme.PrimaryColor, 0.95f);
                }
                if (isHovered)
                {
                    if (theme.TextBoxHoverBackColor != Color.Empty)
                        return theme.TextBoxHoverBackColor;
                }
                if (theme.TextBoxBackColor != Color.Empty)
                    return theme.TextBoxBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return SystemColors.Window;
        }

        /// <summary>
        /// Gets the text/foreground color for the numeric control
        /// </summary>
        public static Color GetNumericTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isFocused = false,
            bool isDisabled = false)
        {
            if (isDisabled)
            {
                if (useThemeColors && theme != null)
                {
                    if (theme.DisabledForeColor != Color.Empty)
                        return theme.DisabledForeColor;
                }
                return Color.Gray;
            }

            if (useThemeColors && theme != null)
            {
                if (isFocused)
                {
                    if (theme.TextBoxSelectedForeColor != Color.Empty)
                        return theme.TextBoxSelectedForeColor;
                }
                if (isHovered)
                {
                    if (theme.TextBoxHoverForeColor != Color.Empty)
                        return theme.TextBoxHoverForeColor;
                }
                if (theme.TextBoxForeColor != Color.Empty)
                    return theme.TextBoxForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the border color for the numeric control
        /// </summary>
        public static Color GetNumericBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isFocused = false,
            bool isDisabled = false)
        {
            if (isDisabled)
            {
                if (useThemeColors && theme != null)
                {
                    if (theme.DisabledForeColor != Color.Empty)
                        return ControlPaint.Light(theme.DisabledForeColor, 0.5f);
                }
                return Color.FromArgb(200, 200, 200);
            }

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
        /// Gets the button background color
        /// </summary>
        public static Color GetButtonBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isPressed = false,
            bool isHovered = false,
            bool isDisabled = false)
        {
            if (isDisabled)
            {
                if (useThemeColors && theme != null)
                {
                    if (theme.DisabledBackColor != Color.Empty)
                        return theme.DisabledBackColor;
                }
                return Color.FromArgb(240, 240, 240);
            }

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
        /// Gets the button icon/text color
        /// </summary>
        public static Color GetButtonIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isPressed = false,
            bool isHovered = false,
            bool isDisabled = false)
        {
            if (isDisabled)
            {
                if (useThemeColors && theme != null)
                {
                    if (theme.DisabledForeColor != Color.Empty)
                        return theme.DisabledForeColor;
                }
                return Color.FromArgb(180, 180, 180);
            }

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
        /// Gets all theme colors for a numeric control in one call
        /// </summary>
        public static (Color background, Color text, Color border, Color buttonBg, Color buttonIcon, Color error) GetNumericColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isFocused = false,
            bool isDisabled = false,
            bool isButtonPressed = false,
            bool isButtonHovered = false)
        {
            return (
                GetNumericBackgroundColor(theme, useThemeColors, isHovered, isFocused),
                GetNumericTextColor(theme, useThemeColors, isHovered, isFocused, isDisabled),
                GetNumericBorderColor(theme, useThemeColors, isHovered, isFocused, isDisabled),
                GetButtonBackgroundColor(theme, useThemeColors, isButtonPressed, isButtonHovered, isDisabled),
                GetButtonIconColor(theme, useThemeColors, isButtonPressed, isButtonHovered, isDisabled),
                GetErrorColor(theme, useThemeColors)
            );
        }
    }
}
