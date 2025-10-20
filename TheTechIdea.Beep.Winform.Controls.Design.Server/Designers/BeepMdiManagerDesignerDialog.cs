using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.MDI.Designers
{
    internal class BeepMdiManagerDesignerDialog : BeepiFormPro
    {
        public BeepMdiManagerDesignerDialog()
        {
            Text = "Beep MDI Manager";
            Width = 600; Height = 400; StartPosition = FormStartPosition.CenterParent;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepMdiManagerDesignerDialog));
            SuspendLayout();
            // 
            // BeepMdiManagerDesignerDialog
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            ClientSize = new Size(619, 579);
            FormStyle = FormStyle.Modern;
            IsMdiContainer = false;
            Name = "BeepMdiManagerDesignerDialog";
           ResumeLayout(false);

        }
    }
}
