using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Basic;

using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls.Filter
{
    public partial class BeepFilter : frm_Addin
    {
        public EntityStructure Entity { get; set; } = new EntityStructure();
        public BeepFilter()
        {
            InitializeComponent();
            this.Savebutton.Click += Savebutton_Click;
            this.Cancelbutton.Click += Cancelbutton_Click;
            dataGridView1.DataError += DataGridView1_DataError;
        }

        private void DataGridView1_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
           e.Cancel = true;
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Savebutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetFilters(EntityStructure entity)
        {
            this.Entity = entity;
            this.fieldNameDataGridViewTextBoxColumn.DataSource = Entity.Fields;
            this.fieldNameDataGridViewTextBoxColumn.DisplayMember = "FieldName";
            this.fieldNameDataGridViewTextBoxColumn.ValueMember = "FieldName";
            filtersBindingSource.DataSource = Entity.Filters;
            filtersBindingSource.ResetBindings(false);
            this.dataGridView1.DataSource = filtersBindingSource;
            this.dataGridView1.Refresh();
        }
       
    }
}
