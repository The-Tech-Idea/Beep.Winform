using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public sealed class BeepDialogOptions
    {
        public BeepDialogKind Kind { get; set; } = BeepDialogKind.Centered;
        public bool DismissOnOverlayClick { get; set; } = false;
        public bool EscToClose { get; set; } = true;
        public int BorderRadius { get; set; } = 10;
        public int BorderThickness { get; set; } = 1;
        public int AnimationMs { get; set; } = 220;
        public Size MaxSize { get; set; } = new Size(720, 520);
        public Padding ContentPadding { get; set; } = new Padding(20);
    }
}
