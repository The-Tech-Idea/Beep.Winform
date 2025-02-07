using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using System.ComponentModel; // Ensure correct namespace for BeepDatePicker

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepDatePickerColumn : DataGridViewColumn
    {
        public BeepDatePickerColumn() : base(new BeepDatePickerCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepDatePickerCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepDatePickerEditingControl); // Use BeepDatePicker for editing
        public override Type ValueType => typeof(DateTime?); // Store nullable DateTime
        public override object DefaultNewRowValue => null; // Default to null

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepDatePickerEditingControl control)
            {
                if (initialFormattedValue is DateTime dateValue)
                {
                    control.SelectedDate = dateValue.ToString();
                }
                else
                {
                    control.SelectedDate = null; // Default to null
                }
            }
        }
    }

    public class BeepDatePickerEditingControl : BeepDatePicker, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepDatePickerEditingControl()
        {
            this.Size = new Size(120, 30);
            this.BackColor = Color.White;

            // Handle date change event
            this.TextChanged += BeepDatePicker_TextChanged;
        }

        private void BeepDatePicker_TextChanged(object sender, EventArgs e)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.SelectedDate;
            set
            {
                if (value is DateTime dateValue)
                {
                    this.SelectedDate = dateValue.ToString();
                }
                else
                {
                    this.SelectedDate = null;
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.SelectedDate;

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
