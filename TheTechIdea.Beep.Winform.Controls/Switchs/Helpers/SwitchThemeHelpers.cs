using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers
{
    public static class SwitchThemeHelpers
    {
        public static Color GetSwitchBackgroundColor(
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

            return Color.Transparent;
        }

        public static Color GetTrackBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false,
            bool isHovered = false,
            bool isPressed = false,
            Color? customOnColor = null,
            Color? customOffColor = null)
        {
            if (useThemeColors && theme != null)
            {
                Color baseColor = isOn
                    ? (customOnColor.HasValue ? customOnColor.Value : GetThemeSuccessColor(theme))
                    : (customOffColor.HasValue ? customOffColor.Value : GetThemeOffColor(theme));

                if (isPressed)
                    return ColorUtils.ShiftLuminance(baseColor, -0.08f);
                if (isHovered)
                    return ColorUtils.ShiftLuminance(baseColor, isOn ? -0.05f : 0.05f);

                return baseColor;
            }

            Color fallback = isOn
                ? Color.FromArgb(76, 175, 80)
                : Color.FromArgb(189, 189, 189);

            if (isPressed)
                return ColorUtils.ShiftLuminance(fallback, -0.08f);
            if (isHovered)
                return ColorUtils.ShiftLuminance(fallback, isOn ? -0.05f : 0.05f);

            return fallback;
        }

        public static Color GetThumbColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false,
            bool isHovered = false,
            bool isDisabled = false)
        {
            if (isDisabled)
                return ColorUtils.ShiftLuminance(Color.White, 0.1f);

            Color baseThumb = Color.White;

            if (useThemeColors && theme != null)
            {
                if (theme.SurfaceColor != Color.Empty)
                {
                    float luminance = GetLuminance(theme.SurfaceColor);
                    baseThumb = luminance > 0.5f ? Color.White : Color.FromArgb(240, 240, 240);
                }
            }

            if (isHovered)
                return ColorUtils.ShiftLuminance(baseThumb, -0.03f);

            return baseThumb;
        }

        public static Color GetTrackBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isOn)
                {
                    if (theme.SuccessColor != Color.Empty)
                        return ColorUtils.ShiftLuminance(theme.SuccessColor, -0.1f);
                }
                else
                {
                    if (theme.BorderColor != Color.Empty)
                        return theme.BorderColor;
                }
            }

            return isOn
                ? Color.FromArgb(66, 165, 70)
                : Color.FromArgb(224, 224, 224);
        }

        public static Color GetLabelTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false,
            bool isActive = false,
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
                if (isActive)
                {
                    if (isOn && theme.SuccessColor != Color.Empty)
                        return theme.SuccessColor;
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
                else
                {
                    if (theme.DisabledForeColor != Color.Empty)
                        return theme.DisabledForeColor;
                }
            }

            if (isActive)
                return isOn ? Color.FromArgb(76, 175, 80) : Color.FromArgb(33, 37, 41);

            return Color.FromArgb(180, 180, 180);
        }

        public static Color GetThumbShadowColor(
            IBeepTheme theme,
            bool useThemeColors,
            int elevation = 2)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ShadowColor != Color.Empty)
                    return Color.FromArgb(Math.Min(255, elevation * 20), theme.ShadowColor);
            }

            return Color.FromArgb(Math.Min(255, elevation * 20), Color.Black);
        }

        public static Color GetFocusRingColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.PrimaryColor != Color.Empty)
                    return Color.FromArgb(80, theme.PrimaryColor);
            }

            return Color.FromArgb(80, Color.FromArgb(76, 175, 80));
        }

        public static (Color switchBg, Color trackBg, Color thumb, Color border, Color labelText, Color shadow, Color focusRing) GetSwitchColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false,
            bool isHovered = false,
            bool isPressed = false,
            bool isFocused = false,
            bool isDisabled = false,
            bool isLabelActive = false)
        {
            return (
                GetSwitchBackgroundColor(theme, useThemeColors),
                GetTrackBackgroundColor(theme, useThemeColors, isOn, isHovered, isPressed),
                GetThumbColor(theme, useThemeColors, isOn, isHovered, isDisabled),
                GetTrackBorderColor(theme, useThemeColors, isOn),
                GetLabelTextColor(theme, useThemeColors, isOn, isLabelActive, isDisabled),
                GetThumbShadowColor(theme, useThemeColors, 2),
                isFocused ? GetFocusRingColor(theme, useThemeColors) : Color.Transparent
            );
        }

        public static Color GetContrastColor(Color backgroundColor, IBeepTheme theme = null)
        {
            float luminance = GetLuminance(backgroundColor);

            if (theme != null)
            {
                return luminance > 0.4f
                    ? (theme.ForeColor != Color.Empty ? theme.ForeColor : Color.FromArgb(28, 27, 31))
                    : (theme.IsDarkTheme ? Color.FromArgb(249, 250, 251) : Color.White);
            }

            return luminance > 0.4f
                ? Color.FromArgb(28, 27, 31)
                : Color.FromArgb(249, 250, 251);
        }

        #region Helper Methods

        private static float GetLuminance(Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            r = r <= 0.03928f ? r / 12.92f : (float)Math.Pow((r + 0.055f) / 1.055f, 2.4f);
            g = g <= 0.03928f ? g / 12.92f : (float)Math.Pow((g + 0.055f) / 1.055f, 2.4f);
            b = b <= 0.03928f ? b / 12.92f : (float)Math.Pow((b + 0.055f) / 1.055f, 2.4f);

            return 0.2126f * r + 0.7152f * g + 0.0722f * b;
        }

        private static Color GetThemeSuccessColor(IBeepTheme theme)
        {
            if (theme.SuccessColor != Color.Empty)
                return theme.SuccessColor;
            if (theme.PrimaryColor != Color.Empty)
                return theme.PrimaryColor;
            if (theme.CheckBoxBackColor != Color.Empty)
                return theme.CheckBoxBackColor;
            return Color.FromArgb(76, 175, 80);
        }

        private static Color GetThemeOffColor(IBeepTheme theme)
        {
            if (theme.SecondaryColor != Color.Empty)
                return theme.SecondaryColor;
            if (theme.SurfaceColor != Color.Empty)
                return theme.SurfaceColor;
            return Color.FromArgb(189, 189, 189);
        }

        #endregion
    }
}
