using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.MDI.Designers
{
    internal class BeepMdiManagerDesignerDialog : BeepiForm
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
            FormStyle = BeepFormStyle.Office;
            IsMdiContainer = false;
            Name = "BeepMdiManagerDesignerDialog";
            StylePresets.Presets = (Dictionary<string, BeepFormStyleMetrics>)resources.GetObject("BeepMdiManagerDesignerDialog.StylePresets.Presets");
            ResumeLayout(false);

        }
    }
}
