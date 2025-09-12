using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public partial class BeepDialogManager
    {
        private BeepToastHost? _toastHost;
        public void ShowToast(BeepToastOptions options)
        {
            if (_toastHost is null)
            {
                _toastHost = new BeepToastHost(_host);
                _host.Controls.Add(_toastHost);
                _toastHost.Bounds = GetTargetDisplayRect();
                _toastHost.BringToFront();
                _host.Resize += (_, __) =>
                {
                    if (_toastHost != null) { _toastHost.Bounds = GetTargetDisplayRect(); _toastHost.Relayout(); }
                };
            }
            _toastHost.Enqueue(options);
        }
    }


}
