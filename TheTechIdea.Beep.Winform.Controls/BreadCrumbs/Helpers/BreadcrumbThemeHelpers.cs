using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    /// <summary>
    /// Breadcrumb colour resolution: one slot, one return (the settled end-state).
    /// The previous version carried a useThemeColors flag, Empty-guard chains and literal
    /// fallbacks (a hardcoded link-blue), and its ApplyThemeColors passed the control's own
    /// BackColor/ForeColor as the custom override - HasValue was always true, so it returned
    /// its input and assigned it back: completely inert while callers believed theming ran.
    /// </summary>
    public static class BreadcrumbThemeHelpers
    {
        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        // Windows high-contrast is a SYSTEM accessibility override, resolved per paint so
        // toggling it applies on the next repaint. It outranks explicit custom colours.
        // (The old design stamped SystemColors into the control's Back/ForeColor instead,
        // which kept the high-contrast palette after the mode was turned off.)
        private static bool HC => BreadcrumbAccessibilityHelpers.IsHighContrastMode();

        /// <summary>Item text: non-last items are links (the theme's Link family); the last item is plain text.</summary>
        public static Color GetItemTextColor(IBeepTheme theme, bool isLast, bool isHovered, Color? customColor = null)
        {
            if (HC) return SystemColors.WindowText;
            if (customColor is { } c && c != Color.Empty) return c;
            var t = T(theme);
            if (isLast) return t.ForeColor;
            return isHovered ? t.HoverLinkColor : t.LinkColor;
        }

        public static Color GetItemHoverBackColor(IBeepTheme theme, Color? customColor = null)
            => HC ? SystemColors.Highlight
             : customColor is { } c && c != Color.Empty ? c : Color.FromArgb(40, T(theme).ButtonHoverBackColor);

        public static Color GetItemSelectedBackColor(IBeepTheme theme, Color? customColor = null)
            => HC ? SystemColors.Highlight
             : customColor is { } c && c != Color.Empty ? c : Color.FromArgb(80, T(theme).ButtonSelectedBackColor);

        public static Color GetSeparatorColor(IBeepTheme theme, float opacity = 0.5f, Color? customColor = null)
        {
            if (HC) return SystemColors.WindowFrame;
            Color baseColor = customColor is { } c && c != Color.Empty ? c : T(theme).LabelForeColor;
            return Color.FromArgb((int)(255 * opacity), baseColor);
        }

        public static Color GetBackgroundColor(IBeepTheme theme, Color? customColor = null)
            => HC ? SystemColors.Window
             : customColor is { } c && c != Color.Empty ? c : T(theme).PanelBackColor;

        public static Color GetItemBorderColor(IBeepTheme theme, bool isHovered, Color? customColor = null)
        {
            if (HC) return SystemColors.WindowFrame;
            if (customColor is { } c && c != Color.Empty) return c;
            var t = T(theme);
            return isHovered ? t.ButtonHoverBorderColor : t.BorderColor;
        }

        /// <summary>All item colours in one call - what the five style painters consume.</summary>
        public static (Color textColor, Color hoverBackColor, Color selectedBackColor, Color separatorColor, Color borderColor) GetThemeColors(
            IBeepTheme theme, bool isLast, bool isHovered, bool isSelected, float separatorOpacity = 0.5f)
            => (GetItemTextColor(theme, isLast, isHovered),
                GetItemHoverBackColor(theme),
                GetItemSelectedBackColor(theme),
                GetSeparatorColor(theme, separatorOpacity),
                GetItemBorderColor(theme, isHovered));
    }
}
