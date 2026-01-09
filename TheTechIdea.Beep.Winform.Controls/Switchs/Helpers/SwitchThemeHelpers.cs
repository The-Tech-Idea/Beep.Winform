using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers
{
    /// <summary>
    /// Centralized helper for managing switch theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps switch states (on, off, hovered, focused) to theme colors
    /// </summary>
    public static class SwitchThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the switch control
        /// Priority: Custom color > Theme Background > Default
        /// </summary>
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

        /// <summary>
        /// Gets the track background color
        /// Priority: Custom color > Theme Success/Secondary > Default
        /// </summary>
        public static Color GetTrackBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false,
            bool isHovered = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isOn)
                {
                    if (theme.SuccessColor != Color.Empty)
                        return theme.SuccessColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                    if (theme.CheckBoxBackColor != Color.Empty)
                        return theme.CheckBoxBackColor;
                }
                else
                {
                    if (isHovered)
                    {
                        if (theme.SurfaceColor != Color.Empty)
                            return ControlPaint.Light(theme.SurfaceColor, 0.1f);
                    }
                    if (theme.SecondaryColor != Color.Empty)
                        return theme.SecondaryColor;
                    if (theme.SurfaceColor != Color.Empty)
                        return theme.SurfaceColor;
                }
            }

            return isOn
                ? Color.FromArgb(76, 175, 80) // Material Green
                : Color.FromArgb(189, 189, 189); // Material Gray
        }

        /// <summary>
        /// Gets the thumb color
        /// </summary>
        public static Color GetThumbColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                // Material switches typically use white thumb
                if (theme.SurfaceColor != Color.Empty)
                {
                    float luminance = GetLuminance(theme.SurfaceColor);
                    return luminance > 0.5f ? Color.White : Color.FromArgb(240, 240, 240);
                }
            }

            return Color.White; // Default white thumb
        }

        /// <summary>
        /// Gets the border color for the track
        /// </summary>
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
                        return ControlPaint.Dark(theme.SuccessColor, 0.1f);
                }
                else
                {
                    if (theme.BorderColor != Color.Empty)
                        return theme.BorderColor;
                }
            }

            return isOn
                ? Color.FromArgb(66, 165, 70) // Darker green
                : Color.FromArgb(224, 224, 224); // Light gray
        }

        /// <summary>
        /// Gets the label text color
        /// </summary>
        public static Color GetLabelTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false,
            bool isActive = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isActive)
                {
                    if (isOn)
                    {
                        if (theme.SuccessColor != Color.Empty)
                            return theme.SuccessColor;
                    }
                    else
                    {
                        if (theme.ForeColor != Color.Empty)
                            return theme.ForeColor;
                    }
                }
                else
                {
                    if (theme.DisabledForeColor != Color.Empty)
                        return theme.DisabledForeColor;
                }
            }

            if (isActive)
            {
                return isOn
                    ? Color.FromArgb(76, 175, 80) // Material Green
                    : Color.FromArgb(33, 37, 41); // Dark gray
            }
            else
            {
                return Color.FromArgb(180, 180, 180); // Disabled gray
            }
        }

        /// <summary>
        /// Gets the shadow color for thumb elevation
        /// </summary>
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

        /// <summary>
        /// Gets all theme colors for a switch in one call
        /// </summary>
        public static (Color switchBg, Color trackBg, Color thumb, Color border, Color labelText, Color shadow) GetSwitchColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false,
            bool isHovered = false,
            bool isLabelActive = false)
        {
            return (
                GetSwitchBackgroundColor(theme, useThemeColors),
                GetTrackBackgroundColor(theme, useThemeColors, isOn, isHovered),
                GetThumbColor(theme, useThemeColors, isOn, isHovered),
                GetTrackBorderColor(theme, useThemeColors, isOn),
                GetLabelTextColor(theme, useThemeColors, isOn, isLabelActive),
                GetThumbShadowColor(theme, useThemeColors, 2)
            );
        }

        #region Helper Methods

        /// <summary>
        /// Calculate relative luminance of a color (for contrast checking)
        /// Returns value between 0 (dark) and 1 (light)
        /// </summary>
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

        #endregion
    }
}
