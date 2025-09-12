using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    internal static class ThemeAccess
    {
        // 1) Central current-theme getter
        public static BeepTheme GetCurrentTheme(Control anyControlInForm)
        {
            // Primary: global manager
            var t = BeepThemesManager.CurrentTheme;
            if (t != null) return (BeepTheme)t;

            // Fallback: ask the form host (if you keep one there)
            if (anyControlInForm.FindForm() is IBeepModernFormHost host && host.CurrentTheme is BeepTheme formTheme)
                return formTheme;

            // Last resort: a sane default to avoid null refs
            return new BeepTheme(); // or throw; but default keeps the UI alive
        }

        // 2) Panel/Border colors with your “panel != back” preference
        public static Color PanelBack(BeepTheme theme, Color fallback)
            => theme.PanelBackColor.IsEmpty ? (theme.BackColor.IsEmpty ? fallback : theme.BackColor) : theme.PanelBackColor;

        public static Color Border(BeepTheme theme, Color fallback)
            => theme.BorderColor.IsEmpty ? fallback : theme.BorderColor;

        // 3) Text colors with graceful fallbacks
        public static Color TitleColor(BeepTheme theme, Color fallback)
            => !theme.CardTitleForeColor.IsEmpty ? theme.CardTitleForeColor
             : (theme.TitleStyle?.TextColor ?? fallback);

        public static Color BodyColor(BeepTheme theme, Color fallback)
            => theme.BodyStyle?.TextColor ?? fallback;

        public static Color ButtonTextColor(BeepTheme theme, Color fallback)
            => theme.ButtonStyle?.TextColor ?? fallback;

        // 4) TypographyStyle -> Font (typed, no reflection)
        public static Font ToFont(TypographyStyle? t, Font fallback, bool preferBoldIfNoStyle = false)
        {
            if (t == null)
                return preferBoldIfNoStyle ? new Font(fallback, fallback.Style | FontStyle.Bold) : fallback;

            string family = string.IsNullOrWhiteSpace(t.FontFamily) ? fallback.FontFamily.Name : t.FontFamily;
            float size = t.FontSize > 0 ? (float)t.FontSize : fallback.Size;

            // Merge FontStyle from TypographyStyle.FontStyle + weight/underline/strike flags
            var style = t.FontStyle; // already System.Drawing.FontStyle in your model
            if (t.IsUnderlined) style |= FontStyle.Underline;
            if (t.IsStrikeout) style |= FontStyle.Strikeout;

            // If style==Regular and caller wants bold by default (titles/buttons), nudge it
            if (style == FontStyle.Regular && preferBoldIfNoStyle) style |= FontStyle.Bold;

            try { return new Font(family, size, style, GraphicsUnit.Point); }
            catch { return new Font(fallback, style); }
        }

        public static (Font font, Color color) TitleTypography(BeepTheme theme, Font fallbackFont, Color fallbackColor)
            => (ToFont(theme.TitleStyle, fallbackFont, preferBoldIfNoStyle: true), TitleColor(theme, fallbackColor));

        public static (Font font, Color color) BodyTypography(BeepTheme theme, Font fallbackFont, Color fallbackColor)
            => (ToFont(theme.BodyStyle, fallbackFont), BodyColor(theme, fallbackColor));

        public static (Font font, Color color) ButtonTypography(BeepTheme theme, Font fallbackFont, Color fallbackColor)
            => (ToFont(theme.ButtonStyle, fallbackFont, preferBoldIfNoStyle: true), ButtonTextColor(theme, fallbackColor));
    }
}
