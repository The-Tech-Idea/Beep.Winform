using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Lovs.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in LOV controls.
    /// Prefer the theme-aware overloads (<see cref="GetLovFontFromTheme"/>,
    /// <see cref="GetBadgeFontFromTheme"/>) which delegate to
    /// <see cref="BeepThemesManager.ToFont"/> with DPI scaling.
    /// The legacy <c>BeepFontManager</c> overloads remain as fallbacks.
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

        // ── Legacy / fallback ─────────────────────────────────────────────

        /// <summary>
        /// Gets the font for LOV text (key and value textboxes).
        /// Prefer <see cref="GetLovFontFromTheme"/> when a live theme instance is available.
        /// </summary>
        public static Font GetLovFont(BeepControlStyle controlStyle)
        {
            float     baseSize  = StyleTypography.GetFontSize(controlStyle);
            string    family    = StyleTypography.GetFontFamily(controlStyle).Split(',')[0].Trim();
            return BeepFontManager.GetFont(family, baseSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets the font for button icons/text at 85 % of the base control size.
        /// </summary>
        public static Font GetButtonFont(BeepControlStyle controlStyle)
        {
            float  baseSize   = StyleTypography.GetFontSize(controlStyle);
            string family     = StyleTypography.GetFontFamily(controlStyle).Split(',')[0].Trim();
            return BeepFontManager.GetFont(family, baseSize * 0.85f, FontStyle.Regular);
        }

        /// <summary>
        /// No-op kept for binary compatibility. Call
        /// <see cref="GetLovFontFromTheme"/> instead.
        /// </summary>
        [System.Obsolete("Use GetLovFontFromTheme(theme) and cache the returned Font on the control.")]
        public static void ApplyFontTheme(BeepControlStyle controlStyle) { }
    }
}
