using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Containers.Helpers
{
    internal static class BeepPanelFontHelpers
    {
        public static Font GetTitleFont(BeepPanel owner, IBeepTheme theme)
        {
            if (owner?.TextFont != null)
            {
                return owner.TextFont;
            }

            if (theme?.TitleSmall != null)
            {
                return BeepThemesManager.ToFont(theme.TitleSmall);
            }

            if (theme?.CardHeaderStyle != null)
            {
                return BeepThemesManager.ToFont(theme.CardHeaderStyle);
            }

            if (theme?.LabelFont != null)
            {
                return BeepFontManager.ToFont(theme.LabelFont);
            }

            return SystemFonts.MessageBoxFont;
        }
    }
}
