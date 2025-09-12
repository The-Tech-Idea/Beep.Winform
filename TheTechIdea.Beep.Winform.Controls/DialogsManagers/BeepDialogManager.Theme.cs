using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;
using TheTechIdea.Beep.Winform.Controls.Managers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public partial class BeepDialogManager : Component
    {
        private bool _themeSubscribed;

    
        // Optional: call this once if you have a BeepFormUIManager on the form
        public void Attach(BeepFormUIManager ui)
        {
            if (ui == null) return;
            ui.OnThemeChanged -= HandleLocalThemeChanged; // avoid duplicates
            ui.OnThemeChanged += HandleLocalThemeChanged; // gives you early per-form signal
        }

        private void HandleGlobalThemeChanged(object? s, ThemeChangeEventArgs e)
            => SafeApplyThemeNow();

        private void HandleLocalThemeChanged(string _)
            => SafeApplyThemeNow();

        private void SafeApplyThemeNow()
        {
            try { _host?.BeginInvoke(new Action(ApplyThemeNow)); }
            catch { /* best-effort */ ApplyThemeNow(); }
        }
        private void ApplyThemeNow()
        {
            IBeepTheme theme = (IBeepTheme)ThemeAccess.GetCurrentTheme(_host);

            foreach (var s in _stack)
            {
                s.Card.ApplyThemeFrom((BeepTheme?)theme);
                s.Overlay.Invalidate();
                s.Card.Invalidate();
            }

            _toastHost?.ApplyThemeFrom((BeepTheme)theme);
            _host.Invalidate(true);
        }

    }
}
