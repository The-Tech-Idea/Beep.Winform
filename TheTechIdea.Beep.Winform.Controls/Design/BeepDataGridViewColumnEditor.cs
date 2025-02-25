using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Design.Forms;
using TheTechIdea.Beep.Winform.Controls.Grid;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class BeepDataGridViewColumnEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null && context.Instance is BeepDataGridView beepGrid)
            {
                try
                {
                    using (BeepDataGridViewColumnCollectionDialog dialog = new BeepDataGridViewColumnCollectionDialog(beepGrid))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            beepGrid.Refresh();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening column editor: {ex.Message}", "Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return value;
        }
    }
}
