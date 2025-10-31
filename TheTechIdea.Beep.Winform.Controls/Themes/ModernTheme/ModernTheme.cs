using System;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme : DefaultBeepTheme
    {
        public ModernTheme()
        {
            ThemeName   = "ModernTheme";
            ThemeGuid   = Guid.NewGuid().ToString();
            IsDarkTheme = false;
            FontName    = "Segoe UI Variable";
            FontSize    = 12.0f;

            ApplyColorPalette();
            ApplyTypography();
            ApplyCore();
        }
    }
}