
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepDataGridViewNumericUpDownColumn : DataGridViewColumn
    {
        public BeepDataGridViewNumericUpDownColumn() : base(new DataGridViewNumericUpDownCell())
        {
        }

        public override object Clone()
        {
            BeepDataGridViewNumericUpDownColumn clone = (BeepDataGridViewNumericUpDownColumn)base.Clone();
            return clone;
        }
    }

    public class DataGridViewNumericUpDownCell : DataGridViewTextBoxCell
    {
        public DataGridViewNumericUpDownCell() : base()
        {
            this.Style.Format = "N2"; // Optional: specify number format
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            DataGridViewNumericUpDownEditingControl ctl = DataGridView.EditingControl as DataGridViewNumericUpDownEditingControl;

            if (this.Value == null || this.Value == DBNull.Value)
            {
                ctl.Value = ctl.Minimum;
            }
            else
            {
                ctl.Value = Convert.ToDecimal(this.Value);
            }
        }

        public override Type EditType => typeof(DataGridViewNumericUpDownEditingControl);
        public override Type ValueType => typeof(decimal);
        public override object DefaultNewRowValue => 0m;
    }

    public class DataGridViewNumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public object EditingControlFormattedValue
        {
            get { return this.Value; }
            set { this.Value = Convert.ToDecimal(value); }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        public int EditingControlRowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            return true;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange => false;

        public DataGridView EditingControlDataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        public bool EditingControlValueChanged
        {
            get { return valueChanged; }
            set { valueChanged = value; }
        }

        public Cursor EditingPanelCursor => base.Cursor;

        protected override void OnValueChanged(EventArgs eventargs)
        {
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }

}
