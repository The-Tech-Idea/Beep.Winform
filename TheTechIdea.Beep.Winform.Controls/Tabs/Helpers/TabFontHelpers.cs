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

        /// <summary>
        /// Measures the height of the given font, falling back to a usable font first.
        /// <para>
        /// No try/catch: <see cref="ResolveSafeFont"/> has already guaranteed a usable font, so
        /// <see cref="TextRenderer.MeasureText(string, Font)"/> has nothing left to throw about. The
        /// previous <c>catch { return ScaleValue(16, …); }</c> silently substituted a hard-coded
        /// 16px for whatever went wrong, so a genuine failure surfaced as tabs that were the wrong
        /// height with nothing recorded anywhere.
        /// </para>
        /// </summary>
        public static int GetSafeFontHeight(Font font, Control ownerControl = null)
        {
            Font safe = ResolveSafeFont(font, ownerControl);
            return Math.Max(1, TextRenderer.MeasureText("Ag", safe).Height);
        }

        /// <summary>
        /// Measures text width with a guaranteed-usable font.
        /// <para>
        /// The previous version caught everything and re-measured with
        /// <see cref="SystemFonts.DefaultFont"/> — which quietly produced a width for a *different*
        /// font than the one the painter would draw with. Measuring with one font and drawing with
        /// another is the exact defect that clipped every label in BeepTree.
        /// </para>
        /// </summary>
        public static int MeasureTextWidthSafe(string text, Font font, Control ownerControl = null)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            Font safe = ResolveSafeFont(font, ownerControl);
            return TextRenderer.MeasureText(text, safe).Width;
        }

        /// <summary>
        /// True when the font can actually be measured.
        /// <para>
        /// This catch is narrow and is genuine handling, not swallowing: GDI+ throws
        /// <see cref="ArgumentException"/> when <see cref="Font.Height"/> is read on a disposed
        /// font, and "is this font usable?" is precisely the question being asked — an unusable
        /// font is the answer, not an error. Any other exception propagates.
        /// </para>
        /// </summary>
        private static bool IsFontUsable(Font font)
        {
            if (font == null) return false;
            try
            {
                return font.Height > 0;
            }
            catch (ArgumentException)
            {
                return false;   // disposed font — that is the answer, not a failure
            }
        }
    }
}
