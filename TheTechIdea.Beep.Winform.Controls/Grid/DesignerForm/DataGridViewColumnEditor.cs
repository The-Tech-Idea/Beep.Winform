using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DesignerForm
{
    public class DataGridViewColumnEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            // Use modal form for the editor
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (editorService == null)
            {
                MessageBox.Show("Editor service is unavailable.");
                return value;
            }

            // Try to cast the instance as BeepGrid
            if (!(context.Instance is BeepGrid control))
            {
                MessageBox.Show("The instance is not a BeepGrid.");
                return value;
            }

            // Get the DataGridView from the BeepGrid control
            DataGridView dataGridView = control.GridView;

            if (dataGridView == null)
            {
                MessageBox.Show("DataGridView is null.");
                return value;
            }

            // Open the DataGridViewColumnDesignerForm to edit columns
            using (DataGridViewColumnDesignerForm form = new DataGridViewColumnDesignerForm(control))
            {
                if (editorService.ShowDialog(form) == DialogResult.OK)
                {
                    // Handle any updates or modifications made to the DataGridView here
                    // Optionally, refresh the grid or persist changes
                    control.RefreshGrid(); // Example: refresh the grid after changes
                }
            }

            return value; // Return the updated value if necessary
        }
    }
}
