using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls; // Ensure correct namespace for BeepLabel

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepLabelColumn : DataGridViewColumn
    {
        public BeepLabelColumn() : base(new BeepLabelCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepLabelCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepLabelEditingControl); // Use BeepLabel for editing
        public override Type ValueType => typeof(string); // Store text as a string
        public override object DefaultNewRowValue => string.Empty; // Default to empty text

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepLabelEditingControl control)
            {
                control.Text = initialFormattedValue?.ToString() ?? string.Empty;
            }
        }
    }

    public class BeepLabelEditingControl : BeepLabel, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepLabelEditingControl()
        {
            this.Size = new Size(200, 30); // Default size
            this.BackColor = Color.Transparent;

            // Handle text change event
            this.TextChanged += BeepLabel_TextChanged;
        }

        private void BeepLabel_TextChanged(object sender, EventArgs e)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.Text;
            set => this.Text = value?.ToString() ?? string.Empty;
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.Text;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.BackColor = dataGridViewCellStyle.BackColor;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
        }

        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) => true;

        public void PrepareEditingControlForEdit(bool selectAll) { }

        public bool RepositionEditingControlOnValueChange => false;

        public Cursor EditingPanelCursor => base.Cursor;

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }
    }
}
