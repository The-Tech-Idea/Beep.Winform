using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Font helpers for menu bar controls. All fonts resolved via BeepThemesManager.ToFont().
    /// </summary>
    public static class MenuFontHelpers
    {
        public static Font GetMenuItemFont(BeepControlStyle controlStyle, IBeepTheme theme = null)
        {
            if (theme?.MenuItemUnSelectedFont != null)
                return BeepThemesManager.ToFont(theme.MenuItemUnSelectedFont, true);
            if (theme?.LabelFont != null)
                return BeepThemesManager.ToFont(theme.LabelFont, true);
            return BeepThemesManager.ToFont(theme?.BodyMedium) ?? SystemFonts.DefaultFont;
        }

        public static Font GetMenuBarFont(BeepControlStyle controlStyle)
        {
            return BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme?.BodyMedium) ?? SystemFonts.DefaultFont;
        }

        public static void ApplyFontTheme(BeepControlStyle controlStyle, IBeepTheme theme = null) { }
    }
}
