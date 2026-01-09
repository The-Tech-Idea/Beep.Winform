using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Helpers
{
    /// <summary>
    /// Centralized helper for managing marquee control theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps marquee control states to theme colors
    /// </summary>
    public static class MarqueeThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the marquee control
        /// Priority: Custom color > Theme Background > Default
        /// </summary>
        public static Color GetMarqueeBackgroundColor(
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
        /// Gets the border color for the marquee control
        /// </summary>
        public static Color GetMarqueeBorderColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.BorderColor != Color.Empty)
                    return theme.BorderColor;
            }

            return Color.FromArgb(200, 200, 200);
        }

        /// <summary>
        /// Gets the text color for marquee items
        /// </summary>
        public static Color GetMarqueeTextColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the shadow color for marquee items (if needed)
        /// </summary>
        public static Color GetMarqueeShadowColor(
            IBeepTheme theme,
            bool useThemeColors,
            int elevation = 1)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ShadowColor != Color.Empty)
                    return Color.FromArgb(Math.Min(255, elevation * 20), theme.ShadowColor);
            }

            return Color.FromArgb(Math.Min(255, elevation * 20), Color.Black);
        }

        /// <summary>
        /// Gets all theme colors for a marquee control in one call
        /// </summary>
        public static (Color background, Color border, Color text, Color shadow) GetMarqueeColors(
            IBeepTheme theme,
            bool useThemeColors)
        {
            return (
                GetMarqueeBackgroundColor(theme, useThemeColors),
                GetMarqueeBorderColor(theme, useThemeColors),
                GetMarqueeTextColor(theme, useThemeColors),
                GetMarqueeShadowColor(theme, useThemeColors, 1)
            );
        }
    }
}
