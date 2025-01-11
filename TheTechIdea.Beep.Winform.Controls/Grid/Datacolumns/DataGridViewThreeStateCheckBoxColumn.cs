using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    [ToolboxItem(false)]
    public class BeepDataGridViewThreeStateCheckBoxColumn : DataGridViewCheckBoxColumn
    {
        public BeepDataGridViewThreeStateCheckBoxColumn()
        {
            this.CellTemplate = new DataGridViewThreeStateCheckBoxCell();
            this.ThreeState = true; // Enable three states
        }

        public override object Clone()
        {
            BeepDataGridViewThreeStateCheckBoxColumn clone = (BeepDataGridViewThreeStateCheckBoxColumn)base.Clone();
            return clone;
        }
    }

    public class DataGridViewThreeStateCheckBoxCell : DataGridViewCheckBoxCell
    {
        public DataGridViewThreeStateCheckBoxCell()
        {
            this.ThreeState = true; // Enable three states
        }

        // Determines the value type of the cell (CheckState rather than bool to allow 3 states)
        public override Type ValueType => typeof(CheckState);

        // Default value for a new row, set to Indeterminate state
        public override object DefaultNewRowValue => CheckState.Indeterminate;

        // Ensures the correct toggling between three states
        protected override bool SetValue(int rowIndex, object value)
        {
            // If the value is a valid CheckState or null, proceed with setting it
            if (value is CheckState || value == null)
            {
                base.SetValue(rowIndex, value ?? CheckState.Indeterminate);
                return true;
            }
            return false;
        }

        // Formats the value to be displayed in the cell
        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value == null)
            {
                return CheckState.Indeterminate;
            }

            // If the value is already a CheckState, return it
            if (value is CheckState)
            {
                return value;
            }

            // Try to interpret the value as a bool and convert it
            bool boolValue;
            if (bool.TryParse(value.ToString(), out boolValue))
            {
                return boolValue ? CheckState.Checked : CheckState.Unchecked;
            }

            return CheckState.Indeterminate;
        }

        // Handles mouse click event to toggle between the three states
        protected override void OnClick(DataGridViewCellEventArgs e)
        {
            DataGridView grid = this.DataGridView;
            if (grid != null && e.RowIndex >= 0 && e.RowIndex < grid.Rows.Count)
            {
                DataGridViewRow row = grid.Rows[e.RowIndex];
                object cellValue = row.Cells[e.ColumnIndex].Value;

                // Current state of the checkbox
                CheckState currentState = cellValue == null ? CheckState.Indeterminate : (CheckState)cellValue;

                // Toggle to the next state
                CheckState nextState;
                switch (currentState)
                {
                    case CheckState.Checked:
                        nextState = CheckState.Unchecked;
                        break;
                    case CheckState.Unchecked:
                        nextState = CheckState.Indeterminate;
                        break;
                    case CheckState.Indeterminate:
                    default:
                        nextState = CheckState.Checked;
                        break;
                }

                // Set the next state
                row.Cells[e.ColumnIndex].Value = nextState;
            }

            base.OnClick(e);
        }
    }
}
