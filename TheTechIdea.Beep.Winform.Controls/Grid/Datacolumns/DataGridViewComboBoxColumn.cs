using System;
using System.Collections.Generic;
using System.ComponentModel;

using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
   
    [ToolboxItem(false)]
    public class BeepDataGridViewComboBoxColumn : System.Windows.Forms.DataGridViewComboBoxColumn
    {
        public string ParentColumn { get; set; } // The parent column for cascading
        public Dictionary<string, List<ColumnLookupList>> CascadingMap { get; set; } // Cascading data map
        public string Query { get; set; } // Query string to retrieve data if needed
        public string ParamterString { get; set; } = "?"; // String value to be used as a parameter in the query
        public List<ColumnLookupList> CurrentValueList { get; set; } // Value list for the ComboBox
        public DataSourceMode DataSourceMode { get; set; } = DataSourceMode.CascadingMap; // Enum to decide whether to use CascadingMap or Query

        public IDataSource QueryDataSource { get; set; }
        public string DataSourceName { get;set; }

        public BeepDataGridViewComboBoxColumn()
        {
            CellTemplate = new DataGridViewExtendedComboBoxCell();
        }

        // This method handles when the parent column value changes for a specific row
        private void OnParentColumnValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || string.IsNullOrEmpty(ParentColumn)) return;

            // Ensure we are working with the same row where the parent value has changed
            var parentCell = this.DataGridView.Rows[e.RowIndex].Cells[ParentColumn];
            var parentValue = parentCell.Value?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(parentValue))
            {
                var currentCell = this.DataGridView.Rows[e.RowIndex].Cells[this.Index] as DataGridViewExtendedComboBoxCell;

                if (currentCell != null && e.RowIndex == currentCell.RowIndex)
                {
                    // Switch between CascadingMap or Query
                    switch (this.DataSourceMode)
                    {
                        case DataSourceMode.CascadingMap:
                            // Handle using CascadingMap
                            if (CascadingMap != null && CascadingMap.ContainsKey(parentValue))
                            {
                                currentCell.UpdateComboBoxItems(CascadingMap[parentValue], e.RowIndex);
                            }
                            break;

                        case DataSourceMode.Query:
                            // Handle using Query
                            if (!string.IsNullOrEmpty(Query))
                            {
                                string query = Query.Replace(ParamterString, parentValue);
                                var list = ExecuteQuery(query); // Implement your method to retrieve data
                                currentCell.UpdateComboBoxItems(list, e.RowIndex);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        // This method ensures that the editing control behaves correctly when shown (setting DropDown style).
        private void OnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox comboBox && this.DataGridView.CurrentCell.OwningColumn == this)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        // Example function to execute a query and return a list of ColumnLookupList rootnodeitems (to be implemented based on your data source)
        private List<ColumnLookupList> ExecuteQuery(string query)
        {
            // Query execution logic here to retrieve the list based on `query`
            // Return a list of ColumnLookupList after executing the query
            
            return (List<ColumnLookupList>)QueryDataSource.GetEntity(Query, null);
        }
    }
    public class DataGridViewExtendedComboBoxCell : DataGridViewComboBoxCell
    {
        public Dictionary<string, List<ColumnLookupList>> CascadingMap { get; set; }
        public int RowIndex { get; set; } // RowIndex property to track the row index

        public override object Clone()
        {
            var clone = (DataGridViewExtendedComboBoxCell)base.Clone();
            return clone;
        }

        public override Type EditType => typeof(DataGridViewExtendedComboBoxEditingControl);

        public override Type ValueType => typeof(int); // Now using the ID (int) as the value

        public override object DefaultNewRowValue => 0; // Default ID is 0

        // Method to update combo box rootnodeitems for the specific row
        public void UpdateComboBoxItems(List<ColumnLookupList> items, int rowIndex)
        {
            if (items == null || DataGridView == null || rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
            {
                return; // Early exit if invalid
            }

            // Check if the row being edited is the same as the rowIndex provided
            if (DataGridView.CurrentCell != null && DataGridView.CurrentCell.RowIndex == rowIndex)
            {
                // Clear the current rootnodeitems and add the new list for this row
                this.Items.Clear();

                // Populate ComboBox with Display (shown) and Value (ID) for each item
                this.Items.AddRange(items.Select(x => new KeyValuePair<int, string>(x.ID, x.Display)).ToArray());

                // Update the currently displayed ComboBox if we're in editing mode
                if (DataGridView.EditingControl is ComboBox editingControl && DataGridView.CurrentCell.RowIndex == rowIndex)
                {
                    editingControl.Items.Clear();
                    foreach (var item in items)
                    {
                        editingControl.Items.Add(new KeyValuePair<int, string>(item.ID, item.Display));
                    }

                    editingControl.DisplayMember = "Value";  // Config the Display value in the ComboBox
                    editingControl.ValueMember = "Key";  // Use the ID as the underlying value
                }
            }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            var control = DataGridView.EditingControl as DataGridViewExtendedComboBoxEditingControl;
            if (control != null)
            {
                control.DropDownStyle = ComboBoxStyle.DropDownList;
                control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                control.AutoCompleteSource = AutoCompleteSource.ListItems;

                // Set the DataBindingSource, DisplayMember, ValueMember if necessary
                if (this.DataSource != null)
                {
                    control.DataSource = this.DataSource;
                    control.DisplayMember = "Value";  // Config the Display value in the ComboBox
                    control.ValueMember = "Key";  // Use the ID as the underlying value
                }
            }
        }
    }
    [ToolboxItem(false)]
    public class DataGridViewExtendedComboBoxEditingControl : ComboBox, IDataGridViewEditingControl
    {
        private DataGridView _dataGridView;
        private bool _valueChanged;
        private int _rowIndex;

        public object EditingControlFormattedValue
        {
            get => this.SelectedItem is KeyValuePair<int, string> selectedItem ? selectedItem.Key : 0;  // Return the ID (Key)
            set
            {
                if (value is int id)
                {
                    var item = this.Items.Cast<KeyValuePair<int, string>>().FirstOrDefault(x => x.Key == id);
                    if (item.Key != 0)
                    {
                        this.SelectedItem = item;
                    }
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get => _rowIndex;
            set => _rowIndex = value;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            // Let the ComboBox handle navigation keys
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

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                this.SelectAll();
            }
        }

        public bool RepositionEditingControlOnValueChange => false;

        public DataGridView EditingControlDataGridView
        {
            get => _dataGridView;
            set => _dataGridView = value;
        }

        public bool EditingControlValueChanged
        {
            get => _valueChanged;
            set => _valueChanged = value;
        }

        public Cursor EditingPanelCursor => base.Cursor;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            _valueChanged = true;
            _dataGridView?.NotifyCurrentCellDirty(true);
        }
    }
}
