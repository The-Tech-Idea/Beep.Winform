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
                    NotificationType.Success => Color.FromArgb(240, 255, 240), // Light green
                    NotificationType.Warning => Color.FromArgb(255, 252, 232), // Light yellow
                    NotificationType.Error => Color.FromArgb(254, 242, 242), // Light red
                    NotificationType.Info => Color.FromArgb(239, 246, 255), // Light blue
                    NotificationType.System => Color.FromArgb(249, 250, 251), // Light gray
                    _ => theme.BackColor
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
                    NotificationType.Success => Color.FromArgb(22, 101, 52), // Dark green
                    NotificationType.Warning => Color.FromArgb(113, 63, 18), // Dark brown
                    NotificationType.Error => Color.FromArgb(127, 29, 29), // Dark red
                    NotificationType.Info => Color.FromArgb(30, 58, 138), // Dark blue
                    NotificationType.System => Color.FromArgb(55, 65, 81), // Dark gray
                    _ => theme.ForeColor
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
                    NotificationType.Success => Color.FromArgb(134, 239, 172), // Green
                    NotificationType.Warning => Color.FromArgb(251, 191, 36), // Yellow
                    NotificationType.Error => Color.FromArgb(252, 165, 165), // Red
                    NotificationType.Info => Color.FromArgb(147, 197, 253), // Blue
                    NotificationType.System => Color.FromArgb(209, 213, 219), // Gray
                    _ => theme.BorderColor
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
                    NotificationType.Success => Color.FromArgb(34, 197, 94), // Green
                    NotificationType.Warning => Color.FromArgb(245, 158, 11), // Orange
                    NotificationType.Error => Color.FromArgb(239, 68, 68), // Red
                    NotificationType.Info => Color.FromArgb(59, 130, 246), // Blue
                    NotificationType.System => Color.FromArgb(107, 114, 128), // Gray
                    _ => theme.AccentColor
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
