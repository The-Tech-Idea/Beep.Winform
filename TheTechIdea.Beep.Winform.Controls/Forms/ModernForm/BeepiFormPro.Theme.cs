using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        protected IBeepTheme? _currentTheme; // nullable; resolved on demand

        public string ThemeName { get; private set; }

        public IBeepTheme CurrentTheme
        {
            get
            {
                if (_currentTheme == null)
                {
                    // Lazy load the theme based on the ThemeName property
                    _currentTheme = BeepThemesManager.GetTheme(ThemeName);
                    if (_currentTheme == null)
                    {
                        // Fallback to a default theme if not found
                        _currentTheme = BeepThemesManager.GetDefaultTheme();
                    }
                }
                return _currentTheme;
            }
            set
            {
                _currentTheme = value;
                ThemeName = value.ThemeName; // Keep ThemeName in sync
                Invalidate(); // Redraw with new theme
            }
        }

       
        public virtual void ApplyTheme()
        {
            if (_currentTheme == null)
            {
                return;
            }
           
         //   Invalidate();
        }
        public virtual void ApplyTheme(string themeName)
        {
            var theme = BeepThemesManager.GetTheme(themeName);
            if (theme != null)
            {
                CurrentTheme = theme;
               // Invalidate();
            }
            else
            {
                throw new ArgumentException($"Theme '{themeName}' not found.");
            }
        }
    }
}
