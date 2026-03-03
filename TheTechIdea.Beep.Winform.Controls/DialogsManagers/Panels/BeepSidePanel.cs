using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Panels
{
    public class BeepSidePanel : Form
    {
        public bool OpenFromLeft { get; set; }

        public BeepSidePanel()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Width = 400;
        }

        public void AttachToOwner(Form owner)
        {
            if (owner == null) return;
            Height = owner.Height;
            Top = owner.Top;
            Left = OpenFromLeft ? owner.Left : owner.Right - Width;
        }
    }
}
