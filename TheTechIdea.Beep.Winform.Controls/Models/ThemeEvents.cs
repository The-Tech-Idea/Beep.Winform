using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    // New event args class that doesn't depend on EnumBeepThemes
    public class ThemeChangeEventArgs : EventArgs
    {
        public string OldThemeName { get; set; }
        public string NewThemeName { get; set; }
        public BeepTheme OldTheme { get; set; }
        public BeepTheme NewTheme { get; set; }
    }

    // Adapter for the old event args for backward compatibility
    public class ThemeChangeEventsArgs : EventArgs
    {
        public EnumBeepThemes OldTheme { get; set; }
        public EnumBeepThemes NewTheme { get; set; }

        // Conversion constructor for easier interop
        public ThemeChangeEventsArgs(ThemeChangeEventArgs args)
        {
            OldTheme = BeepThemesManager_v2.Legacy.GetEnumFromTheme(args.OldThemeName);
            NewTheme = BeepThemesManager_v2.Legacy.GetEnumFromTheme(args.NewThemeName);
        }

        // Default constructor for direct use
        public ThemeChangeEventsArgs() { }
    }
}
