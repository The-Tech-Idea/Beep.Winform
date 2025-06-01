using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules.ThemeTypes;

namespace TheTechIdea.Beep.Vis.Modules.ThemeTypes
{
    public partial class WinterTheme : BeepTheme
    {
        public WinterTheme() {


            // Set base properties
            ThemeName = "WinterTheme";
            ThemeGuid = Guid.NewGuid().ToString();

            // Set primary colors
            BackColor = Color.FromArgb(240, 245, 255);
            ForeColor = Color.FromArgb(20, 30, 40);

            // The properties defined in partial files will be initialized automatically
            // through their property initializers, but if you need to override any
            // of those values or set additional values, you can do it here

            // Configure theme based on type
            IsDarkTheme = false;
        }
    }
}