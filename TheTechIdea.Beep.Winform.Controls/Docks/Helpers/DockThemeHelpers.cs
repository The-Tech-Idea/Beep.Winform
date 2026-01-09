using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Centralized helper for managing dock control theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps dock control states to theme colors
    /// </summary>
    public static class DockThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the dock control
        /// Priority: Custom color > Theme Panel Background > Default
        /// </summary>
        public static Color GetDockBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null,
            float opacity = 0.85f)
        {
            if (customColor.HasValue)
            {
                if (opacity < 1f)
                    return Color.FromArgb((int)(255 * opacity), customColor.Value);
                return customColor.Value;
            }

            if (useThemeColors && theme != null)
            {
                Color baseColor = Color.Empty;
                if (theme.PanelBackColor != Color.Empty)
                    baseColor = theme.PanelBackColor;
                else if (theme.SurfaceColor != Color.Empty)
                    baseColor = theme.SurfaceColor;
                else if (theme.BackgroundColor != Color.Empty)
                    baseColor = theme.BackgroundColor;

                if (baseColor != Color.Empty)
                {
                    if (opacity < 1f)
                        return Color.FromArgb((int)(255 * opacity), baseColor);
                    return baseColor;
                }
            }

            return Color.FromArgb((int)(255 * opacity), Color.FromArgb(240, 240, 240));
        }

        /// <summary>
        /// Gets the foreground/text color for the dock control
        /// </summary>
        public static Color GetDockForegroundColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.LabelForeColor != Color.Empty)
                    return theme.LabelForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the border color for the dock control
        /// </summary>
        public static Color GetDockBorderColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.BorderColor != Color.Empty)
                    return Color.FromArgb(100, theme.BorderColor);
            }

            return Color.FromArgb(100, 255, 255, 255);
        }

        /// <summary>
        /// Gets the hover color for dock items
        /// </summary>
        public static Color GetDockItemHoverColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.SurfaceColor != Color.Empty)
                    return ControlPaint.Light(theme.SurfaceColor, 0.1f);
            }

            return Color.FromArgb(245, 245, 245);
        }

        /// <summary>
        /// Gets the selected color for dock items
        /// </summary>
        public static Color GetDockItemSelectedColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
            }

            return Color.FromArgb(0, 122, 255); // iOS blue
        }

        /// <summary>
        /// Gets the indicator color for active/running items
        /// </summary>
        public static Color GetIndicatorColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
                if (theme.SuccessColor != Color.Empty)
                    return theme.SuccessColor;
            }

            return Color.FromArgb(0, 122, 255); // iOS blue
        }

        /// <summary>
        /// Gets the separator color
        /// </summary>
        public static Color GetSeparatorColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.BorderColor != Color.Empty)
                    return Color.FromArgb(100, theme.BorderColor);
            }

            return Color.FromArgb(100, 255, 255, 255);
        }

        /// <summary>
        /// Gets the shadow color for dock
        /// </summary>
        public static Color GetShadowColor(
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
        /// Gets all theme colors for a dock control in one call
        /// </summary>
        public static (Color background, Color foreground, Color border, Color hover, Color selected, Color indicator, Color separator, Color shadow) GetDockColors(
            IBeepTheme theme,
            bool useThemeColors,
            float backgroundOpacity = 0.85f,
            int shadowElevation = 2)
        {
            return (
                GetDockBackgroundColor(theme, useThemeColors, null, backgroundOpacity),
                GetDockForegroundColor(theme, useThemeColors),
                GetDockBorderColor(theme, useThemeColors),
                GetDockItemHoverColor(theme, useThemeColors),
                GetDockItemSelectedColor(theme, useThemeColors),
                GetIndicatorColor(theme, useThemeColors),
                GetSeparatorColor(theme, useThemeColors),
                GetShadowColor(theme, useThemeColors, shadowElevation)
            );
        }
    }
}
