using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Lovs.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in LOV controls.
    /// Both overloads delegate to <see cref="BeepThemesManager.ToFont"/> with DPI scaling.
    /// </summary>
    public static class LovFontHelpers
    {
        // ── Theme-aware (preferred) ───────────────────────────────────────

        /// <summary>
        /// Returns the main field font from the supplied theme's
        /// <see cref="IBeepTheme.LabelSmall"/> typography slot, with DPI scaling.
        /// Falls back to a 9 pt regular font if the theme or slot is null.
        /// </summary>
        public static Font GetLovFontFromTheme(IBeepTheme? theme)
        {
            var style = theme?.LabelSmall
                     ?? new TypographyStyle { FontSize = 9f };
            return BeepThemesManager.ToFont(style, applyDpiScaling: true);
        }

        /// <summary>
        /// Returns the KEY badge font (bold, caption-size) from the theme's
        /// <see cref="IBeepTheme.CaptionStyle"/>, with DPI scaling.
        /// </summary>
        public static Font GetBadgeFontFromTheme(IBeepTheme? theme)
        {
            var style = theme?.CaptionStyle
                     ?? new TypographyStyle { FontSize = 8f, FontWeight = FontWeight.Bold };

            // Ensure bold weight for the badge label
            var boldStyle = new TypographyStyle
            {
                FontFamily  = style.FontFamily,
                FontSize    = style.FontSize,
                FontWeight  = FontWeight.Bold,
                FontStyle   = FontStyle.Regular
            };
            return BeepThemesManager.ToFont(boldStyle, applyDpiScaling: true);
        }
    }
}
