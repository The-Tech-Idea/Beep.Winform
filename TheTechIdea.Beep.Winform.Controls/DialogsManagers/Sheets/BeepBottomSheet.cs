using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Sheets
{
    public class BeepBottomSheet : Form
    {
        public BeepBottomSheet()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Height = 360;
        }

        public void AttachToOwner(Form owner)
        {
            if (owner == null) return;
            Width = owner.Width;
            Left = owner.Left;
            Top = owner.Bottom - Height;
        }
    }
}
