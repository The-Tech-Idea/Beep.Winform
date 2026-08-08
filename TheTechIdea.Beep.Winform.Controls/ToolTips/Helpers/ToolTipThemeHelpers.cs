using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// The single colour-resolution seam for tooltips: slot-direct from the theme's ToolTip*
    /// family, semantic types from the semantic slots. One slot, one return. Config colours
    /// are caller data passed through as custom overrides — they are resolved HERE at paint
    /// time, never stamped into the config (stamping made every tooltip look custom-coloured,
    /// which is why live theme changes never repainted an open tooltip).
    /// </summary>
    public static class ToolTipThemeHelpers
    {
        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        /// <summary>
        /// High contrast overrides theme AND custom colours: honouring them is what breaks
        /// the mode. Tooltips map to the system Info colours.
        /// </summary>
        public static bool IsHighContrast => SystemInformation.HighContrast;

        /// <summary>Fill. Semantic types take their semantic slot; Default takes ToolTipBackColor.</summary>
        public static Color GetToolTipBackColor(IBeepTheme theme, ToolTipType type, Color? customColor = null)
        {
            if (IsHighContrast) return SystemColors.Info;
            if (customColor is { } c && c != Color.Empty) return c;

            var t = T(theme);
            return type switch
            {
                ToolTipType.Success => t.SuccessColor,
                ToolTipType.Warning => t.WarningColor,
                ToolTipType.Error => t.ErrorColor,
                ToolTipType.Info => t.AccentColor,
                ToolTipType.Primary => t.PrimaryColor,
                ToolTipType.Secondary => t.SecondaryColor,
                ToolTipType.Accent => t.AccentColor,
                _ => t.ToolTipBackColor
            };
        }

        /// <summary>
        /// Ink. Default type takes ToolTipForeColor; semantic fills take the WCAG brightness
        /// pick over the resolved fill (the accepted contrast idiom — semantic slots vary
        /// per theme, so neither Black nor White is safe unconditionally).
        /// </summary>
        public static Color GetToolTipForeColor(IBeepTheme theme, ToolTipType type, Color? customColor = null)
        {
            if (IsHighContrast) return SystemColors.InfoText;
            if (customColor is { } c && c != Color.Empty) return c;

            if (type == ToolTipType.Default) return T(theme).ToolTipForeColor;

            Color fill = GetToolTipBackColor(theme, type);
            return fill.GetBrightness() > 0.55f ? Color.Black : Color.White;
        }

        /// <summary>Outline. Semantic types keep their fill colour as the border (border melts into the fill); Default takes ToolTipBorderColor.</summary>
        public static Color GetToolTipBorderColor(IBeepTheme theme, ToolTipType type, Color? customColor = null)
        {
            if (IsHighContrast) return SystemColors.WindowFrame;
            if (customColor is { } c && c != Color.Empty) return c;

            var t = T(theme);
            return type == ToolTipType.Default ? t.ToolTipBorderColor : GetToolTipBackColor(theme, type);
        }

        /// <summary>All three colours in one call; config colours pass through as custom overrides.</summary>
        public static (Color backColor, Color foreColor, Color borderColor) GetThemeColors(
            IBeepTheme theme,
            ToolTipType type,
            Color? customBackColor = null,
            Color? customForeColor = null,
            Color? customBorderColor = null)
        {
            return (
                GetToolTipBackColor(theme, type, customBackColor),
                GetToolTipForeColor(theme, type, customForeColor),
                GetToolTipBorderColor(theme, type, customBorderColor)
            );
        }

        /// <summary>Link ink.</summary>
        public static Color GetToolTipLinkColor(IBeepTheme theme)
        {
            if (IsHighContrast) return SystemColors.HotTrack;
            return T(theme).ToolTipLinkColor;
        }

        /// <summary>Hovered link ink.</summary>
        public static Color GetToolTipLinkHoverColor(IBeepTheme theme)
        {
            if (IsHighContrast) return SystemColors.HotTrack;
            return T(theme).ToolTipLinkHoverColor;
        }

        /// <summary>Drop shadow (the slot already carries its alpha).</summary>
        public static Color GetToolTipShadowColor(IBeepTheme theme)
        {
            return T(theme).ToolTipShadowColor;
        }
    }
}
