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
        public IBeepTheme OldTheme { get; set; }
        public IBeepTheme NewTheme { get; set; }
    }

  
}
