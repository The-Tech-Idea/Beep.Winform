using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public class BeepModelessDialog : BeepiFormPro
    {
        public BeepModelessDialog()
        {
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(420, 280);
            StartPosition = FormStartPosition.Manual;
        }

        public void PositionRelativeToOwner(Form owner, DialogPosition position)
        {
            if (owner == null) return;
            Location = position switch
            {
                DialogPosition.TopLeft => new Point(owner.Left + 12, owner.Top + 12),
                DialogPosition.TopRight => new Point(owner.Right - Width - 12, owner.Top + 12),
                DialogPosition.BottomLeft => new Point(owner.Left + 12, owner.Bottom - Height - 12),
                DialogPosition.BottomRight => new Point(owner.Right - Width - 12, owner.Bottom - Height - 12),
                _ => new Point(owner.Left + (owner.Width - Width) / 2, owner.Top + (owner.Height - Height) / 2)
            };
        }
    }
}
