using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Popovers
{
    public class BeepPopover : Form
    {
        public BeepPopover()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Size = new Size(320, 220);
            Deactivate += (s, e) => Close();
        }

        public void Attach(Control anchor)
        {
            if (anchor == null) return;
            Location = PopoverLayoutHelper.GetSmartLocation(anchor, Size);
        }
    }
}
