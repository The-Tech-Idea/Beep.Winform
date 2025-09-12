using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public enum BeepToastKind { Info, Success, Warning, Error }
    public enum BeepDialogKind { Centered, TopSheet, LeftDrawer, RightDrawer }
    public enum BeepDialogResult { None, Cancel, OK, Yes, No }
}
