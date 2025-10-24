using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.ThemeManagement
{
    public class StyleChangeEventArgs
    {
        public FormStyle OldStyle { get; set; }
        public FormStyle NewStyle { get; set; }
        public StyleChangeEventArgs()
        {
            
            
        }
        public StyleChangeEventArgs(FormStyle oldStyle, FormStyle newStyle)
        {
            OldStyle = oldStyle;
            NewStyle = newStyle;
        }
    }
    // New event args class that doesn't depend on EnumBeepThemes
    public class ThemeChangeEventArgs : EventArgs
    {
        public string OldThemeName { get; set; }
        public string NewThemeName { get; set; }
        public IBeepTheme OldTheme { get; set; }
        public IBeepTheme NewTheme { get; set; }

     
        public ThemeChangeEventArgs()
        {
            
        }
        public ThemeChangeEventArgs(string oldThemeName, string newThemeName, IBeepTheme oldTheme, IBeepTheme newTheme, FormStyle oldStyle, FormStyle newStyle)
        {
            OldThemeName = oldThemeName;
            NewThemeName = newThemeName;
            OldTheme = oldTheme;
            NewTheme = newTheme;
          
        }
        public override string ToString()
        {
            return $"Theme changed from {OldThemeName} to {NewThemeName}";
        }
        public string GetChangeSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Theme changed from {OldThemeName} to {NewThemeName}");
           
            return sb.ToString();
        }
        public bool IsThemeChanged()
        {
            return OldThemeName != NewThemeName;
        }
      
        public bool IsAnyChange()
        {
            return IsThemeChanged();
        }
    
    }

  
}
