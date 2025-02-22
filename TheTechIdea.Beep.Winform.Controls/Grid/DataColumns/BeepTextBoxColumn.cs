using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using System.ComponentModel; // Ensure correct namespace for BeepTextBox

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepTextBoxColumn : DataGridViewColumn
    {
        public BeepTextBoxColumn() : base(new BeepTextBoxCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepTextBoxCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepTextBoxEditingControl); // Use BeepTextBox for editing
        public override Type ValueType => typeof(string); // Store text as a string
        public override object DefaultNewRowValue => string.Empty; // Default to empty

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepTextBoxEditingControl control)
            {
                control.Text = initialFormattedValue?.ToString() ?? string.Empty;
            }
        }
    }
    [ToolboxItem(false)]
    public class BeepTextBoxEditingControl : BeepTextBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepTextBoxEditingControl()
        {
            this.Size = new Size(200, 30); // Default size
            this.BackColor = Color.White;

            // Handle text change event
            this.TextChanged += BeepTextBox_TextChanged;
        }

        private void BeepTextBox_TextChanged(object sender, EventArgs e)
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
