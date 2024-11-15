using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    // DataGridViewSliderColumn class
    [ToolboxItem(true)]
    public class BeepDataGridViewSliderColumn : DataGridViewColumn
    {
        // Constructor
        public BeepDataGridViewSliderColumn() : base(new DataGridViewSliderCell())
        {
            this.CellTemplate = new DataGridViewSliderCell();
            this.Minimum = 0;  // Default minimum value
            this.Maximum = 100; // Default maximum value
        }

        // Minimum value for the slider
        public int Minimum { get; set; }

        // Maximum value for the slider
        public int Maximum { get; set; }

        // Override Clone method to copy properties
        public override object Clone()
        {
            var clone = (BeepDataGridViewSliderColumn)base.Clone();
            clone.Minimum = this.Minimum;
            clone.Maximum = this.Maximum;
            return clone;
        }
    }

    // DataGridViewSliderCell class
    public class DataGridViewSliderCell : DataGridViewTextBoxCell
    {
        public DataGridViewSliderCell()
        {
            this.Style.Format = "N0"; // Default to numeric with no decimals.
        }

        // Initialize the slider editing control with appropriate values
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            DataGridViewSliderEditingControl ctl = DataGridView.EditingControl as DataGridViewSliderEditingControl;
            if (ctl != null)
            {
                // Get the owning column and set the min and max values
                var owningColumn = this.OwningColumn as BeepDataGridViewSliderColumn;
                if (owningColumn != null)
                {
                    ctl.Minimum = owningColumn.Minimum;
                    ctl.Maximum = owningColumn.Maximum;
                }

                // Set the slider value
                if (this.Value == null || this.Value == DBNull.Value)
                {
                    ctl.Value = ctl.Minimum; // Set to the minimum value if no value exists.
                }
                else
                {
                    ctl.Value = Convert.ToInt32(this.Value); // Set to the cell's current value.
                }
            }
        }

        // Return the type of the editing control that DataGridView uses for this cell
        public override Type EditType => typeof(DataGridViewSliderEditingControl);

        // Return the type of the value stored in the cell
        public override Type ValueType => typeof(int);

        // Default value for new rows
        public override object DefaultNewRowValue => 0;
    }

    // DataGridViewSliderEditingControl class
    public class DataGridViewSliderEditingControl : TrackBar, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        public DataGridViewSliderEditingControl()
        {
            this.TickStyle = TickStyle.None;
            this.Minimum = 0;  // Default minimum
            this.Maximum = 100; // Default maximum
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property
        public object EditingControlFormattedValue
        {
            get => this.Value.ToString();
            set
            {
                if (value is string strValue && int.TryParse(strValue, out int result))
                {
                    this.Value = result;
                }
            }
        }

        // Implements the IDataGridViewEditingControl.GetEditingControlFormattedValue method
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        // Applies the style to the editing control
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex property
        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey method
        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            // Let the TrackBar handle the keys listed.
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit method
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation is needed for the TrackBar control
        }

        // Implements the IDataGridViewEditingControl.RepositionEditingControlOnValueChange property
        public bool RepositionEditingControlOnValueChange => false;

        // Implements the IDataGridViewEditingControl.EditingControlDataGridView property
        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        // Implements the IDataGridViewEditingControl.EditingControlValueChanged property
        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }

        // Implements the IDataGridViewEditingControl.EditingPanelCursor property
        public Cursor EditingPanelCursor => base.Cursor;

        // Handles value change event
        protected override void OnValueChanged(EventArgs eventargs)
        {
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }
}
