using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Font helpers for tab controls. All fonts resolved via BeepThemesManager.ToFont().
    /// Theme-managed fonts are NEVER disposed by consumers.
    /// </summary>
    public static class TabFontHelpers
    {
        /// <summary>Resolve a safe font, falling back to SystemFonts.DefaultFont.</summary>
        public static Font ResolveSafeFont(Font font, Control ownerControl = null)
        {
            if (font != null && IsFontUsable(font)) return font;
            if (ownerControl?.Font != null && IsFontUsable(ownerControl.Font)) return ownerControl.Font;
            return SystemFonts.DefaultFont;
        }

        /// <summary>Get the tab text font from theme typography.</summary>
        public static Font GetTabFont(IBeepTheme theme, bool isSelected = false)
        {
            var typo = isSelected ? theme?.LabelMedium : theme?.LabelFont;
            return BeepThemesManager.ToFont(typo)
                ?? BeepThemesManager.ToFont(theme?.BodyMedium)
                ?? SystemFonts.DefaultFont;
        }

        /// <summary>Get the tab subtext/description font from theme typography.</summary>
        public static Font GetTabSubtextFont(IBeepTheme theme, Control ownerControl = null)
        {
            return BeepThemesManager.ToFont(theme?.BodySmall)
                ?? BeepThemesManager.ToFont(theme?.LabelFont)
                ?? SystemFonts.DefaultFont;
        }

        /// <summary>DPI-safe measurement of font height.</summary>
        public static int GetSafeFontHeight(Font font, Control ownerControl = null)
        {
            Font safe = ResolveSafeFont(font, ownerControl);
            try { return Math.Max(1, TextRenderer.MeasureText("Ag", safe).Height); }
            catch { return DpiScalingHelper.ScaleValue(16, ownerControl); }
        }

        /// <summary>DPI-safe text width measurement.</summary>
        public static int MeasureTextWidthSafe(string text, Font font, Control ownerControl = null)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            Font safe = ResolveSafeFont(font, ownerControl);
            try { return TextRenderer.MeasureText(text, safe).Width; }
            catch { return TextRenderer.MeasureText(text, SystemFonts.DefaultFont).Width; }
        }

        /// <summary>Applies font theme — no-op (fonts are resolved at paint time).</summary>
        public static void ApplyFontTheme(BeepControlStyle controlStyle) { }

        private static bool IsFontUsable(Font font)
        {
            if (font == null) return false;
            try { return font.Height > 0; }
            catch { return false; }
        }
    }
}
