using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Helpers
{
    /// <summary>
    /// Colour resolution for notifications. No Notification* slot family exists, so the
    /// settled shape uses the accepted idioms: the card base is the theme's surface for
    /// EVERY type — the type's identity is carried by the semantic border, the icon, and
    /// an alpha veil (<see cref="GetTypeVeil"/>) — and ink is the theme's ForeColor.
    /// The previous version derived pastel cards with Lighten/Darken and a private HSL
    /// engine, and fell back to a hardcoded Tailwind palette whenever the theme was null
    /// — which it always was, because the toast passed null.
    /// </summary>
    public static class NotificationThemeHelpers
    {
        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        private static bool HC => SystemInformation.HighContrast;

        /// <summary>Card base: the surface slot for every type; identity comes from border/icon/veil.</summary>
        public static Color GetBackgroundColor(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customColor = null)
        {
            if (HC) return SystemColors.Info;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).SurfaceColor;
        }

        /// <summary>Card ink.</summary>
        public static Color GetForegroundColor(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customColor = null)
        {
            if (HC) return SystemColors.InfoText;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).ForeColor;
        }

        /// <summary>Card outline: the semantic slot (Info takes AccentColor — PrimaryColor is not always an accent).</summary>
        public static Color GetBorderColor(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customColor = null)
        {
            if (HC) return SystemColors.WindowFrame;
            if (customColor is { } c && c != Color.Empty) return c;

            var t = T(theme);
            return type switch
            {
                NotificationType.Success => t.SuccessColor,
                NotificationType.Warning => t.WarningColor,
                NotificationType.Error => t.ErrorColor,
                NotificationType.Info => t.AccentColor,
                NotificationType.System => t.BorderColor,
                _ => t.BorderColor
            };
        }

        /// <summary>Icon tint: the semantic slot.</summary>
        public static Color GetIconColor(
            NotificationType type,
            IBeepTheme theme = null,
            Color? customColor = null)
        {
            if (HC) return SystemColors.InfoText;
            if (customColor is { } c && c != Color.Empty) return c;

            var t = T(theme);
            return type switch
            {
                NotificationType.Success => t.SuccessColor,
                NotificationType.Warning => t.WarningColor,
                NotificationType.Error => t.ErrorColor,
                NotificationType.Info => t.AccentColor,
                NotificationType.System => t.SecondaryColor,
                _ => t.AccentColor
            };
        }

        /// <summary>
        /// Soft wash of the type's accent, painted OVER the card base — the accepted
        /// alpha-veil idiom (the Group hand-rolled exactly this at 12%).
        /// </summary>
        public static Color GetTypeVeil(NotificationType type, IBeepTheme theme = null, int alpha = 30)
        {
            return Color.FromArgb(Math.Clamp(alpha, 8, 64), GetIconColor(type, theme));
        }

        /// <summary>All colours for a type in one call; customs pass through.</summary>
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
