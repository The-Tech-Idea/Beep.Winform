using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Helpers
{
    internal static class BeepLabelFontHelpers
    {
        public static Font GetHeaderFont(BeepLabel owner, IBeepTheme theme)
        {
            if (owner?.TextFont != null)
            {
                return owner.TextFont;
            }

            if (owner?.UseThemeFont == true && theme?.LabelFont != null)
            {
                return BeepThemesManager.ToFont(theme.LabelFont);
            }

            if (theme?.LabelFont != null)
            {
                return BeepThemesManager.ToFont(theme.LabelFont);
            }

            return SystemFonts.MessageBoxFont;
        }

        public static Font GetSubHeaderFont(BeepLabel owner, IBeepTheme theme, Font headerFont)
        {
            if (owner?.SubHeaderFont != null)
            {
                return owner.SubHeaderFont;
            }

            if (theme?.SmallText != null)
            {
                return BeepThemesManager.ToFont(theme.SmallText);
            }

            if (headerFont == null)
            {
                return SystemFonts.MessageBoxFont;
            }

            float smallerSize = Math.Max(6f, headerFont.Size - 1.5f);
            return BeepThemesManager.ToFont(headerFont.FontFamily.Name, smallerSize, FontWeight.Normal, FontStyle.Regular);
        }
    }
}
