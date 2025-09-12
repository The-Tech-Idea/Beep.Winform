using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public sealed class BeepDialogButton
    {
        public string Text { get; set; } = "OK";
        public BeepDialogResult Result { get; set; } = BeepDialogResult.OK;
        public bool IsDefault { get; set; }
        public bool IsCancel { get; set; }
    }
}
