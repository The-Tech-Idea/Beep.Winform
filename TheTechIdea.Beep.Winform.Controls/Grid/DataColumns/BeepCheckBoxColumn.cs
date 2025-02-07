using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using System.ComponentModel; // Ensure correct namespace for BeepCheckBox<bool>

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepCheckBoxColumn : DataGridViewColumn
    {
        public BeepCheckBoxColumn() : base(new BeepCheckBoxCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepCheckBoxCell : DataGridViewCheckBoxCell
    {
        public override Type EditType => typeof(BeepCheckBoxEditingControl); // Use BeepCheckBox as editing control
        public override Type ValueType => typeof(bool); // Store checkbox value as bool
        public override object DefaultNewRowValue => false; // Default to unchecked

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepCheckBoxEditingControl control)
            {
                if (initialFormattedValue is bool checkedValue)
                {
                    control.CurrentValue = checkedValue;
                }
                else
                {
                    control.CurrentValue = false; // Default unchecked
                }
            }
        }
    }

    public class BeepCheckBoxEditingControl : BeepCheckBox<bool>, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepCheckBoxEditingControl()
        {
            this.Size = new Size(20, 20);
            this.CheckedValue = true;
            this.UncheckedValue = false;
            this.CurrentValue = false;

            // Handle state change to notify grid
            this.StateChanged += BeepCheckBox_StateChanged;
        }

        private void BeepCheckBox_StateChanged(object sender, EventArgs e)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.CurrentValue;
            set
            {
                if (value is bool boolValue)
                {
                    this.CurrentValue = boolValue;
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.CurrentValue;

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
