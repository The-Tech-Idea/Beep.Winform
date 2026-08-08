using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// The single colour-resolution seam for tab rendering: every painter and the control itself
    /// resolve colours here, slot-direct from the theme's Tab* family. One slot, one return —
    /// a selected tab that does not read against its strip is the THEME's bug and is fixed in
    /// the theme's Tab part, not compensated here.
    /// </summary>
    /// <remarks>
    /// High contrast is a colour concern, not a rendering concern. It used to be a second, parallel
    /// paint implementation (<c>BeepTabHeaderHost.PaintHighContrast</c>) that re-derived the tab
    /// background, border, title, dirty marker and close glyph itself - and which nothing ever
    /// called, so high contrast simply did not work at all. Resolving system colours here instead
    /// means the one painter pipeline is correct in both modes, and icons, badges, subtext and
    /// header actions keep working in high contrast rather than being dropped by a reduced
    /// second path.
    /// </remarks>
    public static class TabThemeHelpers
    {
        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        /// <summary>
        /// <see langword="true"/> when Windows is in a high-contrast accessibility theme, in which
        /// case system colours override theme colours <i>and</i> any explicit custom colour.
        /// Custom colours are deliberately overridden: honouring them is what breaks high contrast.
        /// </summary>
        public static bool IsHighContrast => SystemInformation.HighContrast;

        /// <summary>Background of the control and its pages. Custom overrides are caller data (Empty falls through).</summary>
        public static Color GetTabControlBackgroundColor(IBeepTheme theme, Color? customColor = null)
        {
            if (IsHighContrast) return SystemColors.Window;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).TabBackColor;
        }

        /// <summary>Background of the header strip the tabs sit on.</summary>
        public static Color GetHeaderBackgroundColor(IBeepTheme theme, Color? customColor = null)
        {
            if (IsHighContrast) return SystemColors.Control;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).TabBackColor;
        }

        /// <summary>
        /// Tab fill. Selected → TabSelectedBackColor, hover → TabHoverBackColor,
        /// rest → TabBackColor. HC mapping preserved from the deleted
        /// <c>BeepTabHeaderHost.GetHighContrastTabBackground</c>.
        /// </summary>
        public static Color GetTabBackgroundColor(
            IBeepTheme theme,
            bool isSelected = false,
            bool isHovered = false,
            Color? customColor = null)
        {
            if (IsHighContrast)
            {
                if (isSelected) return SystemColors.Highlight;
                if (isHovered) return SystemColors.HotTrack;
                return SystemColors.ButtonFace;
            }

            if (customColor is { } c && c != Color.Empty) return c;

            var t = T(theme);
            if (isSelected) return t.TabSelectedBackColor;
            if (isHovered) return t.TabHoverBackColor;
            return t.TabBackColor;
        }

        /// <summary>
        /// Caption ink. HC mapping preserved from the deleted
        /// <c>BeepTabHeaderHost.GetHighContrastTabForeground</c>.
        /// </summary>
        public static Color GetTabTextColor(
            IBeepTheme theme,
            bool isSelected = false,
            bool isHovered = false)
        {
            if (IsHighContrast)
            {
                if (isSelected || isHovered) return SystemColors.HighlightText;
                return SystemColors.WindowText;
            }

            var t = T(theme);
            if (isSelected) return t.TabSelectedForeColor;
            if (isHovered) return t.TabHoverForeColor;
            return t.TabForeColor;
        }

        /// <summary>
        /// Tab outline. HC mapping preserved from the deleted
        /// <c>BeepTabHeaderHost.GetHighContrastBorderColor</c>.
        /// </summary>
        public static Color GetTabBorderColor(
            IBeepTheme theme,
            bool isSelected = false,
            bool isHovered = false)
        {
            if (IsHighContrast)
            {
                return isSelected ? SystemColors.Highlight : SystemColors.WindowFrame;
            }

            var t = T(theme);
            if (isSelected) return t.TabSelectedBorderColor;
            if (isHovered) return t.TabHoverBorderColor;
            return t.TabBorderColor;
        }

        /// <summary>
        /// Selected-tab underline/indicator. TabSelectedBorderColor, NOT PrimaryColor:
        /// the indicator is a Tab-family accent, and themes like Zen define PrimaryColor
        /// as a neutral brand tone identical to their tab strip - the indicator vanished.
        /// </summary>
        public static Color GetTabIndicatorColor(IBeepTheme theme)
        {
            if (IsHighContrast) return SystemColors.Highlight;
            return T(theme).TabSelectedBorderColor;
        }

        /// <summary>
        /// Fill colour for a status badge. In high contrast every kind collapses to
        /// <see cref="SystemColors.Highlight"/>: high-contrast themes deliberately offer no
        /// semantic palette, and inventing one defeats the point of the mode.
        /// </summary>
        /// <remarks>
        /// The badge kinds used to read <c>Theme.ErrorColor</c> / <c>WarningColor</c> /
        /// <c>SuccessColor</c> directly from the painter, bypassing this seam — so badges were the
        /// one adornment that stayed themed in high contrast, on a background that had switched to
        /// system colours.
        /// </remarks>
        public static Color GetBadgeColor(IBeepTheme theme, BeepTabBadgeKind kind)
        {
            if (IsHighContrast) return SystemColors.Highlight;

            var t = T(theme);
            return kind switch
            {
                BeepTabBadgeKind.Error => t.ErrorColor,
                BeepTabBadgeKind.Warning => t.WarningColor,
                BeepTabBadgeKind.Success => t.SuccessColor,
                _ => t.TabSelectedBorderColor
            };
        }

        /// <summary>Busy (loading) indicator: the Tab family's accent line slot.</summary>
        public static Color GetBusyIndicatorColor(IBeepTheme theme)
        {
            if (IsHighContrast) return SystemColors.ControlText;
            return T(theme).TabSelectedBorderColor;
        }

        /// <summary>
        /// Unsaved-changes dot. In high contrast this is <see cref="SystemColors.ControlText"/> so
        /// the dot stays visible on any background - the mapping preserved from the deleted
        /// <c>GetHighContrastDirtyMarkerColor</c>.
        /// </summary>
        public static Color GetDirtyMarkerColor(IBeepTheme theme)
        {
            if (IsHighContrast) return SystemColors.ControlText;
            return T(theme).TabSelectedBorderColor;
        }

        /// <summary>Gets all theme colors for a tab in one call.</summary>
        public static (Color tabBg, Color border, Color text, Color indicator) GetTabColors(
            IBeepTheme theme,
            bool isSelected = false,
            bool isHovered = false)
        {
            return (
                GetTabBackgroundColor(theme, isSelected, isHovered),
                GetTabBorderColor(theme, isSelected, isHovered),
                GetTabTextColor(theme, isSelected, isHovered),
                GetTabIndicatorColor(theme)
            );
        }
    }
}
