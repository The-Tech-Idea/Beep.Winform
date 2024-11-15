
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns;


namespace TheTechIdea.Beep.Winform.Controls.Grid.DesignerForm
{
    public class CustomDataGridViewDesigner : ControlDesigner
    {
        public override DesignerVerbCollection Verbs
        {
            get
            {
                return new DesignerVerbCollection
            {
                new DesignerVerb("Add ListViewEditingColumn", OnAddListViewEditingColumn),
                new DesignerVerb("Configure Columns...", OnConfigureColumns)
            };
            }
        }

        private void OnAddListViewEditingColumn(object sender, EventArgs e)
        {
            BeepGrid dataGridView = (BeepGrid)Component;
            dataGridView.Columns.Add(new ListViewEditingColumn());
        }

        private void OnConfigureColumns(object sender, EventArgs e)
        {
            BeepGrid dataGridView = (BeepGrid)Component;
            DataGridViewDesignerForm designerForm = new DataGridViewDesignerForm(dataGridView.GridView);
            designerForm.ShowDialog();
        }
    }
}