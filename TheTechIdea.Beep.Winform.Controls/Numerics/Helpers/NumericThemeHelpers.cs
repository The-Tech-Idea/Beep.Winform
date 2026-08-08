using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Helpers
{
    /// <summary>
    /// Colour resolution for numeric controls: slot-direct, one slot one return. The
    /// control is a text input, so surfaces borrow the theme's TextBox* family; the
    /// spinner buttons borrow the Button* family (the previous version derived pressed/
    /// hover fills with luminance shifts when those slots exist). High contrast per paint.
    /// </summary>
    public static class NumericThemeHelpers
    {
        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        private static bool HC => SystemInformation.HighContrast;

        /// <summary>Field surface. Custom overrides are caller data (Empty falls through).</summary>
        public static Color GetNumericBackgroundColor(
            IBeepTheme theme,
            bool isHovered = false,
            bool isFocused = false,
            Color? customColor = null)
        {
            if (HC) return SystemColors.Window;
            if (customColor is { } c && c != Color.Empty) return c;

            var t = T(theme);
            if (isFocused) return t.TextBoxSelectedBackColor;
            if (isHovered) return t.TextBoxHoverBackColor;
            return t.TextBoxBackColor;
        }

        /// <summary>Field ink.</summary>
        public static Color GetNumericTextColor(
            IBeepTheme theme,
            bool isHovered = false,
            bool isFocused = false,
            bool isDisabled = false)
        {
            if (HC) return isDisabled ? SystemColors.GrayText : SystemColors.WindowText;

            var t = T(theme);
            if (isDisabled) return t.DisabledForeColor;
            if (isFocused) return t.TextBoxSelectedForeColor;
            if (isHovered) return t.TextBoxHoverForeColor;
            return t.TextBoxForeColor;
        }

        /// <summary>Field outline; focus takes the Selected slot.</summary>
        public static Color GetNumericBorderColor(
            IBeepTheme theme,
            bool isHovered = false,
            bool isFocused = false,
            bool isDisabled = false)
        {
            if (HC) return isFocused ? SystemColors.Highlight : SystemColors.WindowFrame;

            var t = T(theme);
            if (isDisabled) return t.DisabledBorderColor;
            if (isFocused) return t.TextBoxSelectedBorderColor;
            if (isHovered) return t.TextBoxHoverBorderColor;
            return t.TextBoxBorderColor;
        }

        /// <summary>Spinner button fill: the Button* family (pressed takes the Selected slot).</summary>
        public static Color GetButtonBackgroundColor(
            IBeepTheme theme,
            bool isPressed = false,
            bool isHovered = false,
            bool isDisabled = false)
        {
            if (HC) return isPressed ? SystemColors.Highlight : SystemColors.ButtonFace;

            var t = T(theme);
            if (isDisabled) return t.DisabledBackColor;
            if (isPressed) return t.ButtonSelectedBackColor;
            if (isHovered) return t.ButtonHoverBackColor;
            return t.ButtonBackColor;
        }

        /// <summary>Spinner glyph ink.</summary>
        public static Color GetButtonIconColor(
            IBeepTheme theme,
            bool isPressed = false,
            bool isHovered = false,
            bool isDisabled = false)
        {
            if (HC) return isPressed ? SystemColors.HighlightText : SystemColors.ControlText;

            var t = T(theme);
            if (isDisabled) return t.DisabledForeColor;
            if (isPressed) return t.ButtonSelectedForeColor;
            if (isHovered) return t.ButtonHoverForeColor;
            return t.ButtonForeColor;
        }

        /// <summary>Invalid-input ink: the semantic slot.</summary>
        public static Color GetErrorColor(IBeepTheme theme)
        {
            if (HC) return SystemColors.ControlText;
            return T(theme).ErrorColor;
        }

        /// <summary>All colours in one call.</summary>
        public static (Color background, Color text, Color border, Color buttonBg, Color buttonIcon, Color error) GetNumericColors(
            IBeepTheme theme,
            bool isHovered = false,
            bool isFocused = false,
            bool isDisabled = false,
            bool isButtonPressed = false,
            bool isButtonHovered = false)
        {
            return (
                GetNumericBackgroundColor(theme, isHovered, isFocused),
                GetNumericTextColor(theme, isHovered, isFocused, isDisabled),
                GetNumericBorderColor(theme, isHovered, isFocused, isDisabled),
                GetButtonBackgroundColor(theme, isButtonPressed, isButtonHovered, isDisabled),
                GetButtonIconColor(theme, isButtonPressed, isButtonHovered, isDisabled),
                GetErrorColor(theme)
            );
        }
    }
}
