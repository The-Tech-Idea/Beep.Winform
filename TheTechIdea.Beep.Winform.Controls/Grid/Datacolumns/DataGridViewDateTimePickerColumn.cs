using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    // Custom DataGridView Column that uses a DateTimePicker as its editing control
    [ToolboxItem(false)]
    public class BeepDataGridViewDateTimePickerColumn : DataGridViewColumn
    {
        public BeepDataGridViewDateTimePickerColumn() : base(new DataGridViewDateTimePickerCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a DataGridViewDateTimePickerCell.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewDateTimePickerCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewDateTimePickerCell");
                }
                base.CellTemplate = value;
            }
        }
    }

    // Custom DataGridView Cell that uses a DateTimePicker as its editing control
    public class DataGridViewDateTimePickerCell : DataGridViewTextBoxCell
    {
        public DataGridViewDateTimePickerCell() : base()
        {
            // Use a default date format
            this.Style.Format = "d"; // Short date pattern
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            // Get the editing control as a DateTimePicker
            if (this.DataGridView.EditingControl is DateTimePicker editingControl)
            {
                // Set the format of the DateTimePicker
                editingControl.Format = DateTimePickerFormat.Short;
                if (this.Value is DateTime dateTimeValue)
                {
                    editingControl.Value = dateTimeValue;
                }
                else
                {
                    editingControl.Value = DateTime.Now;
                }
            }
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that DataGridView will use
                return typeof(DataGridViewDateTimePickerEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Set the value type to DateTime
                return typeof(DateTime);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // The default value for a new row is the current date/time
                return DateTime.Now;
            }
        }
    }

    // Custom editing control that is displayed when editing a cell
    public class DataGridViewDateTimePickerEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;

        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        public object EditingControlFormattedValue
        {
            get => this.Value.ToShortDateString();
            set
            {
                if (value is string stringValue)
                {
                    this.Value = DateTime.TryParse(stringValue, out var dateValue) ? dateValue : DateTime.Now;
                }
            }
        }

        public Cursor EditingPanelCursor => Cursors.Default;

        public bool RepositionEditingControlOnValueChange => false;

        public int EditingControlRowIndex { get; set; }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the navigation keys.
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation is needed.
        }

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }

        protected override void OnValueChanged(EventArgs eventargs)
        {
            // Notify the DataGridView that the value has been changed
            this.EditingControlValueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }
}
