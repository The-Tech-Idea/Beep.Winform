using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers
{
    /// <summary>
    /// Maps vertical-table states onto the theme's grid slots. One slot, one return.
    /// </summary>
    /// <remarks>
    /// This carried the full anti-pattern the Calendar review removed by user directive: a
    /// <c>useThemeColors</c> flag, <c>!= Color.Empty</c> guards, <c>ShiftLuminance</c> blends and a
    /// literal Tailwind-ish fallback palette — while reading GENERIC slots even though the theme
    /// declares a dedicated Grid* family. There is always a theme; the slots are assigned as the
    /// theme defines them, and a wrong-looking colour is the theme's bug, fixed in the theme.
    /// </remarks>
    public static class VerticalTableThemeHelpers
    {
        /// <summary>The current theme - painters read slots off this directly.</summary>
        public static IBeepTheme Cur => BeepThemesManager.CurrentTheme;

        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        public static Color GetTableBackgroundColor(IBeepTheme theme, Color? customColor = null)
            => customColor ?? T(theme).GridBackColor;

        public static Color GetHeaderBackgroundColor(IBeepTheme theme,
            bool isSelected = false, bool isFeatured = false, Color? customColor = null)
        {
            if (customColor.HasValue) return customColor.Value;
            var t = T(theme);
            return isFeatured || isSelected ? t.GridHeaderSelectedBackColor : t.GridHeaderBackColor;
        }

        public static Color GetCellBackgroundColor(IBeepTheme theme,
            bool isHovered = false, bool isSelected = false, bool isAlternate = false, Color? customColor = null)
        {
            if (customColor.HasValue) return customColor.Value;
            var t = T(theme);
            if (isSelected) return t.GridRowSelectedBackColor;
            if (isHovered) return t.GridRowHoverBackColor;
            if (isAlternate) return t.AltRowBackColor;
            return t.GridBackColor;
        }

        public static Color GetBorderColor(IBeepTheme theme,
            bool isSelected = false, bool isFeatured = false, Color? customColor = null)
        {
            if (customColor.HasValue) return customColor.Value;
            var t = T(theme);
            return isFeatured || isSelected ? t.GridHeaderSelectedBorderColor : t.GridLineColor;
        }

        public static Color GetHeaderTextColor(IBeepTheme theme,
            bool isSelected = false, bool isFeatured = false)
        {
            var t = T(theme);
            return isFeatured || isSelected ? t.GridHeaderSelectedForeColor : t.GridHeaderForeColor;
        }

        public static Color GetCellTextColor(IBeepTheme theme, bool isSelected = false)
        {
            var t = T(theme);
            return isSelected ? t.GridRowSelectedForeColor : t.GridForeColor;
        }

        /// <summary>Elevation alpha over the theme's shadow colour.</summary>
        public static Color GetShadowColor(IBeepTheme theme, int elevation = 4)
            => Color.FromArgb(Math.Min(255, elevation * 10), T(theme).ShadowColor);

        public static (Color tableBg, Color headerBg, Color cellBg, Color borderColor, Color headerText, Color cellText, Color shadow) GetThemeColors(
            IBeepTheme theme,
            bool isHeaderSelected = false, bool isHeaderFeatured = false,
            bool isCellHovered = false, bool isCellSelected = false, bool isCellAlternate = false)
        {
            return (
                GetTableBackgroundColor(theme),
                GetHeaderBackgroundColor(theme, isHeaderSelected, isHeaderFeatured),
                GetCellBackgroundColor(theme, isCellHovered, isCellSelected, isCellAlternate),
                GetBorderColor(theme, isHeaderSelected, isHeaderFeatured),
                GetHeaderTextColor(theme, isHeaderSelected, isHeaderFeatured),
                GetCellTextColor(theme, isCellSelected),
                GetShadowColor(theme, 4)
            );
        }
    }
}
