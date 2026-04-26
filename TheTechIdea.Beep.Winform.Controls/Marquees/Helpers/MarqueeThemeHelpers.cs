using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Helpers
{
    public static class MarqueeThemeHelpers
    {
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

            return SystemInformation.HighContrast
                ? ColorUtils.MapSystemColor(SystemColors.Window)
                : Color.Transparent;
        }

        public static Color GetMarqueeBorderColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.BorderColor != Color.Empty)
                    return theme.BorderColor;
            }

            return SystemInformation.HighContrast
                ? ColorUtils.MapSystemColor(SystemColors.WindowFrame)
                : ColorUtils.MapSystemColor(SystemColors.ControlDark);
        }

        public static Color GetMarqueeTextColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return SystemInformation.HighContrast
                ? ColorUtils.MapSystemColor(SystemColors.WindowText)
                : ColorUtils.MapSystemColor(SystemColors.ControlText);
        }

        public static Color GetMarqueeShadowColor(
            IBeepTheme theme,
            bool useThemeColors,
            int elevation = 1)
        {
            if (SystemInformation.HighContrast)
                return Color.Transparent;

            if (useThemeColors && theme != null)
            {
                if (theme.ShadowColor != Color.Empty)
                    return Color.FromArgb(Math.Min(255, elevation * 20), theme.ShadowColor);
            }

            return Color.FromArgb(Math.Min(255, elevation * 20), ColorUtils.MapSystemColor(SystemColors.ControlText));
        }

        public static bool ShouldShowShadow(bool useThemeColors, IBeepTheme theme)
        {
            if (SystemInformation.HighContrast)
                return false;

            if (useThemeColors && theme != null && theme is BeepTheme bt)
                return !bt.IsDarkTheme;

            return true;
        }

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
