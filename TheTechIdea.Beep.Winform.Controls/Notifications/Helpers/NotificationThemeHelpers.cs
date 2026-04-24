using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Helpers
{
    /// <summary>
    /// Centralized helper for managing notification theme colors
    /// Provides type-based color schemes with theme integration
    /// </summary>
    public static class NotificationThemeHelpers
    {
        /// <summary>
        /// Gets background color for notification type
        /// </summary>
        public static Color GetBackgroundColor(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (theme != null)
            {
                var baseColor = type switch
                {
                    NotificationType.Success => theme.SuccessColor != Color.Empty ? theme.SuccessColor : Color.FromArgb(34, 197, 94),
                    NotificationType.Warning => theme.WarningColor != Color.Empty ? theme.WarningColor : Color.FromArgb(245, 158, 11),
                    NotificationType.Error => theme.ErrorColor != Color.Empty ? theme.ErrorColor : Color.FromArgb(239, 68, 68),
                    NotificationType.Info => theme.PrimaryColor != Color.Empty ? theme.PrimaryColor : Color.FromArgb(59, 130, 246),
                    NotificationType.System => theme.SurfaceColor != Color.Empty ? theme.SurfaceColor : (theme.IsDarkTheme ? Color.FromArgb(55, 65, 81) : Color.FromArgb(249, 250, 251)),
                    _ => theme.SurfaceColor != Color.Empty ? theme.SurfaceColor : (theme.IsDarkTheme ? Color.FromArgb(31, 41, 55) : Color.White)
                };

                return theme.IsDarkTheme ? Darken(baseColor, 0.7f) : Lighten(baseColor, 0.85f);
            }

            // Default colors
            return type switch
            {
                NotificationType.Success => Color.FromArgb(240, 255, 240),
                NotificationType.Warning => Color.FromArgb(255, 252, 232),
                NotificationType.Error => Color.FromArgb(254, 242, 242),
                NotificationType.Info => Color.FromArgb(239, 246, 255),
                NotificationType.System => Color.FromArgb(249, 250, 251),
                _ => Color.White
            };
        }

        private static Color Lighten(Color color, float factor)
        {
            return Color.FromArgb( 255,
                Math.Min(255, (int)(color.R + (255 - color.R) * factor)),
                Math.Min(255, (int)(color.G + (255 - color.G) * factor)),
                Math.Min(255, (int)(color.B + (255 - color.B) * factor)));
        }

        private static Color Darken(Color color, float factor)
        {
            return Color.FromArgb( 255,
                Math.Max(0, (int)(color.R * factor)),
                Math.Max(0, (int)(color.G * factor)),
                Math.Max(0, (int)(color.B * factor)));
        }

        /// <summary>
        /// Gets foreground/text color for notification type
        /// </summary>
        public static Color GetForegroundColor(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (theme != null)
            {
                return type switch
                {
                    NotificationType.Success => theme.SuccessColor != Color.Empty 
                        ? Darken(theme.SuccessColor, 0.6f) 
                        : Color.FromArgb(22, 101, 52),
                    NotificationType.Warning => theme.WarningColor != Color.Empty 
                        ? Darken(theme.WarningColor, 0.6f) 
                        : Color.FromArgb(113, 63, 18),
                    NotificationType.Error => theme.ErrorColor != Color.Empty 
                        ? Darken(theme.ErrorColor, 0.6f) 
                        : Color.FromArgb(127, 29, 29),
                    NotificationType.Info => theme.PrimaryColor != Color.Empty 
                        ? Darken(theme.PrimaryColor, 0.6f) 
                        : Color.FromArgb(30, 58, 138),
                    NotificationType.System => theme.ForeColor != Color.Empty 
                        ? theme.ForeColor 
                        : (theme.IsDarkTheme ? Color.FromArgb(229, 231, 235) : Color.FromArgb(55, 65, 81)),
                    _ => theme.ForeColor != Color.Empty ? theme.ForeColor : (theme.IsDarkTheme ? Color.FromArgb(229, 231, 235) : Color.FromArgb(31, 41, 55))
                };
            }

            // Default colors
            return type switch
            {
                NotificationType.Success => Color.FromArgb(22, 101, 52),
                NotificationType.Warning => Color.FromArgb(113, 63, 18),
                NotificationType.Error => Color.FromArgb(127, 29, 29),
                NotificationType.Info => Color.FromArgb(30, 58, 138),
                NotificationType.System => Color.FromArgb(55, 65, 81),
                _ => Color.FromArgb(31, 41, 55)
            };
        }

        /// <summary>
        /// Gets border color for notification type
        /// </summary>
        public static Color GetBorderColor(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (theme != null)
            {
                return type switch
                {
                    NotificationType.Success => theme.SuccessColor != Color.Empty 
                        ? theme.SuccessColor 
                        : Color.FromArgb(134, 239, 172),
                    NotificationType.Warning => theme.WarningColor != Color.Empty 
                        ? theme.WarningColor 
                        : Color.FromArgb(251, 191, 36),
                    NotificationType.Error => theme.ErrorColor != Color.Empty 
                        ? theme.ErrorColor 
                        : Color.FromArgb(252, 165, 165),
                    NotificationType.Info => theme.PrimaryColor != Color.Empty 
                        ? theme.PrimaryColor 
                        : Color.FromArgb(147, 197, 253),
                    NotificationType.System => theme.BorderColor != Color.Empty 
                        ? theme.BorderColor 
                        : (theme.IsDarkTheme ? Color.FromArgb(75, 85, 99) : Color.FromArgb(209, 213, 219)),
                    _ => theme.BorderColor != Color.Empty ? theme.BorderColor : (theme.IsDarkTheme ? Color.FromArgb(75, 85, 99) : Color.Gray)
                };
            }

            // Default colors
            return type switch
            {
                NotificationType.Success => Color.FromArgb(134, 239, 172),
                NotificationType.Warning => Color.FromArgb(251, 191, 36),
                NotificationType.Error => Color.FromArgb(252, 165, 165),
                NotificationType.Info => Color.FromArgb(147, 197, 253),
                NotificationType.System => Color.FromArgb(209, 213, 219),
                _ => Color.Gray
            };
        }

        /// <summary>
        /// Gets icon color for notification type
        /// </summary>
        public static Color GetIconColor(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (theme != null)
            {
                return type switch
                {
                    NotificationType.Success => theme.SuccessColor != Color.Empty 
                        ? theme.SuccessColor 
                        : Color.FromArgb(34, 197, 94),
                    NotificationType.Warning => theme.WarningColor != Color.Empty 
                        ? theme.WarningColor 
                        : Color.FromArgb(245, 158, 11),
                    NotificationType.Error => theme.ErrorColor != Color.Empty 
                        ? theme.ErrorColor 
                        : Color.FromArgb(239, 68, 68),
                    NotificationType.Info => theme.PrimaryColor != Color.Empty 
                        ? theme.PrimaryColor 
                        : Color.FromArgb(59, 130, 246),
                    NotificationType.System => theme.SecondaryColor != Color.Empty 
                        ? theme.SecondaryColor 
                        : Color.FromArgb(107, 114, 128),
                    _ => theme.AccentColor != Color.Empty ? theme.AccentColor : Color.Gray
                };
            }

            // Default colors
            return type switch
            {
                NotificationType.Success => Color.FromArgb(34, 197, 94),
                NotificationType.Warning => Color.FromArgb(245, 158, 11),
                NotificationType.Error => Color.FromArgb(239, 68, 68),
                NotificationType.Info => Color.FromArgb(59, 130, 246),
                NotificationType.System => Color.FromArgb(107, 114, 128),
                _ => Color.Gray
            };
        }

        /// <summary>
        /// Gets all colors for a notification type as a tuple
        /// </summary>
        public static (Color BackColor, Color ForeColor, Color BorderColor, Color IconColor) GetColorsForType(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customBackColor = null,
            Color? customForeColor = null,
            Color? customBorderColor = null,
            Color? customIconColor = null)
        {
            return (
                GetBackgroundColor(type, theme, customBackColor),
                GetForegroundColor(type, theme, customForeColor),
                GetBorderColor(type, theme, customBorderColor),
                GetIconColor(type, theme, customIconColor)
            );
        }

        /// <summary>
        /// Calculates relative luminance of a color per WCAG 2.1
        /// </summary>
        public static float GetRelativeLuminance(Color color)
        {
            float RsRGB = color.R / 255.0f;
            float GsRGB = color.G / 255.0f;
            float BsRGB = color.B / 255.0f;

            float R = RsRGB <= 0.03928f ? RsRGB / 12.92f : (float)Math.Pow((RsRGB + 0.055) / 1.055, 2.4);
            float G = GsRGB <= 0.03928f ? GsRGB / 12.92f : (float)Math.Pow((GsRGB + 0.055) / 1.055, 2.4);
            float B = BsRGB <= 0.03928f ? BsRGB / 12.92f : (float)Math.Pow((BsRGB + 0.055) / 1.055, 2.4);

            return 0.2126f * R + 0.7152f * G + 0.0722f * B;
        }

        /// <summary>
        /// Gets a contrast-aware color (dark or light) that meets WCAG AA contrast ratio against the background
        /// </summary>
        public static Color GetContrastColor(Color backgroundColor, Color darkColor, Color lightColor)
        {
            float luminance = GetRelativeLuminance(backgroundColor);
            return luminance > 0.4f ? darkColor : lightColor;
        }

        /// <summary>
        /// Gets a contrast-aware color for text/icons on a given background
        /// Returns a dark color for light backgrounds and a light color for dark backgrounds
        /// </summary>
        public static Color GetContrastColor(Color backgroundColor)
        {
            float luminance = GetRelativeLuminance(backgroundColor);
            return luminance > 0.4f 
                ? Color.FromArgb(28, 27, 31) 
                : Color.FromArgb(249, 250, 251);
        }

        /// <summary>
        /// Gets a contrast-aware color using theme-aware defaults
        /// </summary>
        public static Color GetContrastColor(Color backgroundColor, IBeepTheme theme = null)
        {
            float luminance = GetRelativeLuminance(backgroundColor);
            
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

        /// <summary>
        /// Shifts luminance of a color up or down (safe for both light and dark themes)
        /// </summary>
        public static Color ShiftLuminance(Color color, float amount)
        {
            float h, s, l;
            ColorToHsl(color, out h, out s, out l);
            l = Math.Max(0, Math.Min(1, l + amount));
            return ColorFromHsl(h, s, l);
        }

        private static void ColorToHsl(Color color, out float h, out float s, out float l)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;

            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));
            float delta = max - min;

            l = (max + min) / 2.0f;

            if (delta == 0)
            {
                h = 0;
                s = 0;
            }
            else
            {
                s = l < 0.5f ? delta / (max + min) : delta / (2.0f - max - min);

                if (r == max) h = (g - b) / delta;
                else if (g == max) h = 2.0f + (b - r) / delta;
                else h = 4.0f + (r - g) / delta;

                h /= 6.0f;
                if (h < 0) h += 1.0f;
            }
        }

        private static Color ColorFromHsl(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5f ? l * (1.0f + s) : l + s - l * s;
                float p = 2.0f * l - q;

                r = HueToRgb(p, q, h + 1.0f / 3.0f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0f / 3.0f);
            }

            return Color.FromArgb( 255,
                Math.Max(0, Math.Min(255, (int)(r * 255))),
                Math.Max(0, Math.Min(255, (int)(g * 255))),
                Math.Max(0, Math.Min(255, (int)(b * 255))));
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0) t += 1.0f;
            if (t > 1) t -= 1.0f;

            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }
    }
}
