using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Helpers
{
    /// <summary>
    /// Centralized helper for managing toggle theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps toggle states (ON/OFF) to theme colors
    /// </summary>
    public static class ToggleThemeHelpers
    {
        /// <summary>
        /// Gets the track color when toggle is ON
        /// Priority: Custom color > Theme Success/Primary > Default green
        /// </summary>
        public static Color GetToggleOnColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            // Priority 1: Custom color (highest priority)
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors (from ApplyTheme)
            if (useThemeColors && theme != null)
            {
                // Prefer SuccessColor for ON state (semantic: success/active)
                if (theme.SuccessColor != Color.Empty)
                    return theme.SuccessColor;

                // Fallback to PrimaryColor
                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;

                // Fallback to AccentColor
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
            }

            // Priority 3: Default green color
            return Color.FromArgb(52, 168, 83); // Google Material Green
        }

        /// <summary>
        /// Gets the track color when toggle is OFF
        /// Priority: Custom color > Theme Secondary/Disabled > Default gray
        /// </summary>
        public static Color GetToggleOffColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            // Priority 1: Custom color
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                // Prefer SecondaryColor for OFF state
                if (theme.SecondaryColor != Color.Empty)
                    return theme.SecondaryColor;

                // Fallback to DisabledBackColor
                if (theme.DisabledBackColor != Color.Empty)
                    return theme.DisabledBackColor;

                // Fallback to BorderColor (subtle)
                if (theme.BorderColor != Color.Empty)
                    return ControlPaint.Light(theme.BorderColor, 0.5f);
            }

            // Priority 3: Default gray color
            return Color.FromArgb(189, 189, 189); // Material Gray
        }

        /// <summary>
        /// Gets the thumb color based on toggle state
        /// Priority: Custom color > Theme Surface/Background > Default white
        /// </summary>
        public static Color GetToggleThumbColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn,
            Color? customOnColor = null,
            Color? customOffColor = null)
        {
            // Use state-specific custom colors if provided
            if (isOn && customOnColor.HasValue)
                return customOnColor.Value;
            if (!isOn && customOffColor.HasValue)
                return customOffColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                // Prefer SurfaceColor for thumb (elevated element)
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;

                // Fallback to BackgroundColor (inverted for contrast)
                if (theme.BackgroundColor != Color.Empty)
                {
                    // If background is dark, use light thumb; if light, use white
                    float luminance = GetLuminance(theme.BackgroundColor);
                    return luminance < 0.5f ? Color.White : Color.FromArgb(245, 245, 245);
                }

                // Fallback to ForeColor (inverted)
                if (theme.ForeColor != Color.Empty)
                {
                    float luminance = GetLuminance(theme.ForeColor);
                    return luminance > 0.5f ? Color.Black : Color.White;
                }
            }

            // Priority 3: Default white
            return Color.White;
        }

        /// <summary>
        /// Gets the track color based on current toggle state
        /// </summary>
        public static Color GetToggleTrackColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn,
            Color? customOnColor = null,
            Color? customOffColor = null)
        {
            return isOn
                ? GetToggleOnColor(theme, useThemeColors, customOnColor)
                : GetToggleOffColor(theme, useThemeColors, customOffColor);
        }

        /// <summary>
        /// Gets the text/label color for toggle labels
        /// </summary>
        public static Color GetToggleTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn)
        {
            // Priority 1: Theme colors
            if (useThemeColors && theme != null)
            {
                if (isOn)
                {
                    // ON state: Use white text on colored background (high contrast)
                    Color onColor = GetToggleOnColor(theme, useThemeColors);
                    float luminance = GetLuminance(onColor);
                    return luminance > 0.5f ? Color.Black : Color.White;
                }
                else
                {
                    // OFF state: Use theme foreground or contrasting color
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;

                    Color offColor = GetToggleOffColor(theme, useThemeColors);
                    float luminance = GetLuminance(offColor);
                    return luminance > 0.5f ? Color.Black : Color.White;
                }
            }

            // Priority 2: Default colors
            if (isOn)
            {
                // White text on green background
                return Color.White;
            }
            else
            {
                // Dark text on gray background
                return Color.FromArgb(97, 97, 97);
            }
        }

        /// <summary>
        /// Gets the border color for toggle track
        /// </summary>
        public static Color GetToggleBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn,
            Color? customColor = null)
        {
            // Priority 1: Custom color
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                if (theme.BorderColor != Color.Empty)
                {
                    // Darken border for ON state, lighten for OFF
                    return isOn
                        ? ControlPaint.Dark(theme.BorderColor, 0.2f)
                        : ControlPaint.Light(theme.BorderColor, 0.3f);
                }
            }

            // Priority 3: Default border
            return isOn
                ? Color.FromArgb(40, 40, 40)
                : Color.FromArgb(200, 200, 200);
        }

        /// <summary>
        /// Applies theme colors to a BeepToggle control
        /// Called from ApplyTheme() method
        /// </summary>
        public static void ApplyThemeColors(
            BeepToggle toggle,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (toggle == null || theme == null || !useThemeColors)
                return;

            // Only apply if custom colors haven't been explicitly set
            // We check if colors are still at default values
            bool useDefaultOnColor = toggle.OnColor == Color.FromArgb(52, 168, 83);
            bool useDefaultOffColor = toggle.OffColor == Color.FromArgb(189, 189, 189);
            bool useDefaultThumbColor = toggle.ThumbColor == Color.White;

            if (useDefaultOnColor)
            {
                toggle.OnColor = GetToggleOnColor(theme, useThemeColors);
            }

            if (useDefaultOffColor)
            {
                toggle.OffColor = GetToggleOffColor(theme, useThemeColors);
            }

            if (useDefaultThumbColor)
            {
                toggle.ThumbColor = GetToggleThumbColor(theme, useThemeColors, toggle.IsOn);
                
                // Set state-specific thumb colors if they're at defaults
                if (toggle.OnThumbColor == Color.White)
                {
                    toggle.OnThumbColor = GetToggleThumbColor(theme, useThemeColors, true);
                }
                if (toggle.OffThumbColor == Color.White)
                {
                    toggle.OffThumbColor = GetToggleThumbColor(theme, useThemeColors, false);
                }
            }
        }

        /// <summary>
        /// Gets all theme colors for a toggle in one call
        /// </summary>
        public static (Color onColor, Color offColor, Color thumbColor, Color textColor, Color borderColor) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn,
            Color? customOnColor = null,
            Color? customOffColor = null,
            Color? customThumbColor = null)
        {
            return (
                GetToggleOnColor(theme, useThemeColors, customOnColor),
                GetToggleOffColor(theme, useThemeColors, customOffColor),
                GetToggleThumbColor(theme, useThemeColors, isOn, customThumbColor, customThumbColor),
                GetToggleTextColor(theme, useThemeColors, isOn),
                GetToggleBorderColor(theme, useThemeColors, isOn)
            );
        }

        #region Helper Methods

        /// <summary>
        /// Calculate relative luminance of a color (for contrast checking)
        /// Returns value between 0 (dark) and 1 (light)
        /// </summary>
        private static float GetLuminance(Color color)
        {
            // Convert to relative luminance using WCAG formula
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            // Apply gamma correction
            r = r <= 0.03928f ? r / 12.92f : (float)Math.Pow((r + 0.055f) / 1.055f, 2.4f);
            g = g <= 0.03928f ? g / 12.92f : (float)Math.Pow((g + 0.055f) / 1.055f, 2.4f);
            b = b <= 0.03928f ? b / 12.92f : (float)Math.Pow((b + 0.055f) / 1.055f, 2.4f);

            return 0.2126f * r + 0.7152f * g + 0.0722f * b;
        }

        /// <summary>
        /// Blend two colors together
        /// </summary>
        private static Color BlendColors(Color color1, Color color2, float ratio)
        {
            float r = color1.R * (1 - ratio) + color2.R * ratio;
            float g = color1.G * (1 - ratio) + color2.G * ratio;
            float b = color1.B * (1 - ratio) + color2.B * ratio;
            return Color.FromArgb(color1.A, (int)r, (int)g, (int)b);
        }

        #endregion
    }
}

