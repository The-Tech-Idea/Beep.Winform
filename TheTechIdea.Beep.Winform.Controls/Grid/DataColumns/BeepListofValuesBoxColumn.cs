using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using System.ComponentModel; // Ensure correct namespace for BeepListofValuesBox

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepListofValuesBoxColumn : DataGridViewColumn
    {
        public BeepListofValuesBoxColumn() : base(new BeepListofValuesBoxCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepListofValuesBoxCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepListofValuesBoxEditingControl); // Use BeepListofValuesBox for editing
        public override Type ValueType => typeof(string); // Store selected item key as string
        public override object DefaultNewRowValue => string.Empty; // Default to empty

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepListofValuesBoxEditingControl control)
            {
                if (initialFormattedValue is string selectedKey)
                {
                    control.SelectedKey = selectedKey;
                }
                else
                {
                    control.SelectedKey = string.Empty;
                }
            }
        }
    }
    [ToolboxItem(false)]
    public class BeepListofValuesBoxEditingControl : BeepListofValuesBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepListofValuesBoxEditingControl()
        {
            this.Size = new Size(150, 30); // Default size
            this.BackColor = Color.White;

            // Handle selection change event
            this.OnSelected += BeepListofValuesBox_SelectedIndexChanged;
        }

        private void BeepListofValuesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.SelectedKey;
            set
            {
                if (value is string key)
                {
                    this.SelectedKey = key;
                }
                else
                {
                    this.SelectedKey = string.Empty;
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.SelectedKey;

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
