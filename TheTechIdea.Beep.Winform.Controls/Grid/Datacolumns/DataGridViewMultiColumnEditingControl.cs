using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepDataGridViewMultiColumnColumn : DataGridViewColumn
    {
        public BeepDataGridViewMultiColumnColumn() : base(new DataGridViewMultiColumnCell())
        {
            // Additional initialization if needed
        }
    }

    // Custom DataGridView cell that uses the multi-column editing control
    public class DataGridViewMultiColumnCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(DataGridViewMultiColumnEditingControl);
        public override Type ValueType => typeof(string);  // The value will be a string
        public override object DefaultNewRowValue => "";   // Default value for new rows is an empty string

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is DataGridViewMultiColumnEditingControl editingControl)
            {
                editingControl.CurrentCell = editingControl.Rows[0].Cells[0];  // Set default cell selection
                if (this.Value != null)
                {
                    // Set initial value in the editing control
                    foreach (DataGridViewRow row in editingControl.Rows)
                    {
                        if (row.Cells[0].Value?.ToString() == this.Value.ToString())
                        {
                            editingControl.CurrentCell = row.Cells[0];  // Select corresponding row
                            break;
                        }
                    }
                }
            }
        }
    }

    // Custom editing control with multi-column support
    public class DataGridViewMultiColumnEditingControl : DataGridView, IDataGridViewEditingControl
    {
        public DataGridView EditingControlDataGridView { get; set; }
        public object EditingControlFormattedValue
        {
            get => CurrentRow?.Cells[0].Value?.ToString(); // Return the selected row's first column value
            set => CurrentCell.Value = value;
        }
        public int EditingControlRowIndex { get; set; }
        public bool EditingControlValueChanged { get; set; }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            // Handle navigation keys for the DataGridView
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
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
            if (selectAll && CurrentCell != null)
            {
                // You could select the current value or leave as is
                CurrentCell.Selected = true;
            }
        }

        public Cursor EditingPanelCursor => Cursors.Default;
        public bool RepositionEditingControlOnValueChange => false;

        // Initialize the DataGridView with predefined columns and rows
        public DataGridViewMultiColumnEditingControl()
        {
            // Set editing control properties
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.ReadOnly = true;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.MultiSelect = false;
            this.RowHeadersVisible = false;
            this.BorderStyle = BorderStyle.None;

            // Define columns
            Columns.Add("Value", "Value");
            Columns.Add("Description", "Description");

            // Add sample rows, replace with your data
            Rows.Add("1", "Item One");
            Rows.Add("2", "Item Two");
            Rows.Add("3", "Item Three");
        }

        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            base.OnCellClick(e);
            EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);  // Notify that the cell has changed
        }
    }
}
