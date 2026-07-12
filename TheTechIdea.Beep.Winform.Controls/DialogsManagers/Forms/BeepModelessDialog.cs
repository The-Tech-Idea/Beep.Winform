using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public class BeepModelessDialog : BeepiFormPro
    {
        public BeepModelessDialog()
        {
            ShowInTaskbar = false;
            TopMost = true;
            // Skill § 1: modeless dialog size flows from a BeepLayoutMetrics token; DPI-aware.
            Helpers.DialogHelpers.FitFormToContent(this);
            StartPosition = FormStartPosition.Manual;
        }

        public void PositionRelativeToOwner(Form owner, DialogPosition position)
        {
            if (owner == null) return;
            // Skill § 1: corner offset = ContainerPadding (12 px at 96 DPI, scales with DPI).
            int cornerOffset = BeepLayoutMetrics.ContainerPadding.All.ScaleValue(this);
            Location = position switch
            {
                DialogPosition.TopLeft => new Point(owner.Left + cornerOffset, owner.Top + cornerOffset),
                DialogPosition.TopRight => new Point(owner.Right - Width - cornerOffset, owner.Top + cornerOffset),
                DialogPosition.BottomLeft => new Point(owner.Left + cornerOffset, owner.Bottom - Height - cornerOffset),
                DialogPosition.BottomRight => new Point(owner.Right - Width - cornerOffset, owner.Bottom - Height - cornerOffset),
                _ => new Point(owner.Left + (owner.Width - Width) / 2, owner.Top + (owner.Height - Height) / 2)
            };
        }
    }
}
