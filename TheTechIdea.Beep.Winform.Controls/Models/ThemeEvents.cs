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

  
}
