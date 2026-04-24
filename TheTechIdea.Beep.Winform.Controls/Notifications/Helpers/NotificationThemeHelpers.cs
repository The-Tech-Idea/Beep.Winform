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
                // Use theme colors when available
                return type switch
                {
                    NotificationType.Success => theme.SuccessColor != Color.Empty 
                        ? Lighten(theme.SuccessColor, 0.85f) 
                        : Color.FromArgb(240, 255, 240),
                    NotificationType.Warning => theme.WarningColor != Color.Empty 
                        ? Lighten(theme.WarningColor, 0.85f) 
                        : Color.FromArgb(255, 252, 232),
                    NotificationType.Error => theme.ErrorColor != Color.Empty 
                        ? Lighten(theme.ErrorColor, 0.85f) 
                        : Color.FromArgb(254, 242, 242),
                    NotificationType.Info => theme.PrimaryColor != Color.Empty 
                        ? Lighten(theme.PrimaryColor, 0.85f) 
                        : Color.FromArgb(239, 246, 255),
                    NotificationType.System => theme.SurfaceColor != Color.Empty 
                        ? Lighten(theme.SurfaceColor, 0.5f) 
                        : Color.FromArgb(249, 250, 251),
                    _ => theme.BackColor != Color.Empty ? theme.BackColor : Color.White
                };
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
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R + (255 - color.R) * factor)),
                Math.Min(255, (int)(color.G + (255 - color.G) * factor)),
                Math.Min(255, (int)(color.B + (255 - color.B) * factor)));
        }

        private static Color Darken(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
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
                        : Color.FromArgb(55, 65, 81),
                    _ => theme.ForeColor != Color.Empty ? theme.ForeColor : Color.Black
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
                _ => Color.Black
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
                        : Color.FromArgb(209, 213, 219),
                    _ => theme.BorderColor != Color.Empty ? theme.BorderColor : Color.Gray
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
    }
}
