using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing cell editing functionality for BeepSimpleGrid
    /// Handles cell editors, value updates, and edit mode management
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Cell Selection

        /// <summary>
        /// Selects a cell at the specified row and column indices
        /// </summary>
        public void SelectCell(int rowIndex, int columnIndex)
        {
            if (IsEditorShown)
            {
                CloseCurrentEditor();
            }

            if (rowIndex < 0 || rowIndex >= Rows.Count) return;
            if (columnIndex < 0 || columnIndex >= Columns.Count) return;

            var previousRowIndex = _currentRowIndex;
            var previousCell = _selectedCell;

            _currentRowIndex = rowIndex;
            _selectedColumnIndex = columnIndex;
            _selectedCell = Rows[rowIndex].Cells[columnIndex];

            CurrentRow = Rows[rowIndex];
            CurrentRow.RowData = _fullData[_dataOffset + rowIndex];

            // Only invalidate the changed rows
            if (previousRowIndex != rowIndex)
            {
                if (previousRowIndex >= 0 && previousRowIndex < Rows.Count)
                {
                    InvalidateRowOptimized(previousRowIndex);
                }
                InvalidateRowOptimized(rowIndex);
            }

            CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(_dataOffset + rowIndex, CurrentRow));
            CurrentCellChanged?.Invoke(this, new BeepCellSelectedEventArgs(rowIndex, columnIndex, _selectedCell));

            UpdateRecordNumber();
        }

        /// <summary>
        /// Selects the specified cell
        /// </summary>
        public void SelectCell(BeepCellConfig cell)
        {
            if (cell == null) return;
            _editingRowIndex = cell.RowIndex;
            SelectCell(cell.RowIndex, cell.ColumnIndex);
        }

        /// <summary>
        /// Gets the cell at the specified screen location
        /// </summary>
        private BeepCellConfig GetCellAtLocation(Point location)
        {
            if (!gridRect.Contains(location))
                return null;

            int yRelative = location.Y - gridRect.Top;
            if (yRelative < 0)
                return null;

            // Use binary search for large row counts
            int rowIndex = -1;
            if (Rows.Count > 50)
            {
                rowIndex = BinarySearchRow(yRelative);
            }
            else
            {
                // Linear search for small datasets
                int currentY = 0;
                for (int i = 0; i < Rows.Count; i++)
                {
                    int rowHeight = Rows[i].Height;
                    if (yRelative >= currentY && yRelative < currentY + rowHeight)
                    {
                        rowIndex = i;
                        break;
                    }
                    currentY += rowHeight;
                }
            }

            if (rowIndex == -1 || rowIndex >= Rows.Count)
                return null;

            var row = Rows[rowIndex];
            int xRelative = location.X - gridRect.Left;
            int stickyWidthTotal = _stickyWidth;

            // Find column
            if (xRelative < stickyWidthTotal)
            {
                // In sticky column area
                int currentX = 0;
                for (int i = 0; i < Columns.Count; i++)
                {
                    var column = Columns[i];
                    if (!column.Visible || !column.Sticked) continue;
                    int width = _scaledColumnWidths.ContainsKey(i) ? _scaledColumnWidths[i] : column.Width;
                    if (xRelative >= currentX && xRelative < currentX + width)
                    {
                        return row.Cells[i];
                    }
                    currentX += width;
                }
            }
            else
            {
                // In scrollable column area
                int xAdjusted = xRelative + _xOffset;
                int currentX = stickyWidthTotal;
                for (int i = 0; i < Columns.Count; i++)
                {
                    var column = Columns[i];
                    if (!column.Visible || column.Sticked) continue;
                    int width = _scaledColumnWidths.ContainsKey(i) ? _scaledColumnWidths[i] : column.Width;
                    if (xAdjusted >= currentX && xAdjusted < currentX + width)
                    {
                        return row.Cells[i];
                    }
                    currentX += width;
                }
            }

            return null;
        }

        /// <summary>
        /// Binary search helper for finding row at Y position
        /// </summary>
        private int BinarySearchRow(int yRelative)
        {
            int left = 0;
            int right = Rows.Count - 1;
            int currentY = 0;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                currentY = 0;
                for (int i = 0; i < mid; i++)
                {
                    currentY += Rows[i].Height;
                }

                if (yRelative < currentY)
                {
                    right = mid - 1;
                }
                else if (yRelative >= currentY + Rows[mid].Height)
                {
                    left = mid + 1;
                }
                else
                {
                    return mid;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the rectangle bounds of a cell
        /// </summary>
        private Rectangle GetCellRectangleIn(BeepCellConfig cell)
        {
            if (cell == null) return Rectangle.Empty;
            return cell.Rect;
        }

        #endregion

        #region Cell Editor Management

        /// <summary>
        /// Shows the cell editor for the specified cell
        /// </summary>
        private void ShowCellEditor(BeepCellConfig cell, Point location)
        {
            if (ReadOnly) return;
            if (cell == null || !cell.IsEditable)
                return;

            int colIndex = cell.ColumnIndex;
            if (colIndex < 0 || colIndex >= Columns.Count) return;

            var column = Columns[colIndex];

            // Close any existing editor, committing changes
            EndEdit(true);

            _editingCell = cell;
            _tempcell = cell;
            Size cellSize = new Size(column.Width, cell.Height);

            // Create or reuse editor control from pool
            if (!_columnEditors.TryGetValue(column.ColumnName, out IBeepUIComponent columnEditor))
            {
                columnEditor = CreateCellControlForEditing(cell);
                _columnEditors[column.ColumnName] = columnEditor;
            }
            _editingControl = (BeepControl)columnEditor;
            _editingControl.SetValue(cell.CellValue);

            if (_editingControl == null) return;

            _editingControl.Size = cellSize;
            _editingControl.Location = cell.Rect.Location;
            _editingControl.Theme = Theme;

            // Attach event handlers
            _editingControl.KeyDown -= OnEditorKeyDown;
            _editingControl.KeyDown += OnEditorKeyDown;
            _editingControl.LostFocus -= LostFocusHandler;
            _editingControl.LostFocus += LostFocusHandler;
            _editingControl.TabKeyPressed -= Tabhandler;
            _editingControl.TabKeyPressed += Tabhandler;
            _editingControl.EscapeKeyPressed -= Cancelhandler;
            _editingControl.EscapeKeyPressed += Cancelhandler;

            UpdateCellControl(_editingControl, column, cell, cell.CellValue);

            // Add to grid's Controls collection if not already present
            if (!this.Controls.Contains(_editingControl))
            {
                this.Controls.Add(_editingControl);
            }

            _editingControl.Visible = true;
            _editingControl.BringToFront();
            this.ActiveControl = _editingControl;
            IsEditorShown = true;
        }

        /// <summary>
        /// Moves/updates the editor position based on scrolling
        /// </summary>
        private void MoveEditorIn()
        {
            if (_editingCell == null || _editingControl == null || !IsEditorShown)
            {
                return;
            }

            Rectangle cellRect = GetCellRectangleIn(_editingCell);

            // Adjust for scrolling offsets
            int yOffset = _dataOffset * RowHeight;
            int xOffset = _xOffset;
            cellRect.Y -= yOffset;

            var column = Columns[_editingCell.ColumnIndex];
            if (!column.Sticked)
            {
                cellRect.X -= _xOffset;
            }

            // Define grid bounds
            int gridLeft = 0;
            int gridRight = gridRect.Width;
            int gridTop = 0;
            int gridBottom = gridRect.Height;

            // Define sticky column region
            int stickyWidthTotal = _stickyWidth;
            int stickyLeft = gridRect.Left;
            int stickyRight = gridRect.Left + stickyWidthTotal;

            // Check if editor overlaps sticky region
            bool overlapsStickyRegion = !column.Sticked && stickyWidthTotal > 0 &&
                                        cellRect.X < stickyRight && cellRect.Right > stickyLeft;

            // Check if editor is fully out of view
            bool isFullyOutOfView =
                (cellRect.Right < gridLeft) ||
                (cellRect.Left > gridRight) ||
                (cellRect.Bottom < gridTop) ||
                (cellRect.Top > gridBottom);

            if (isFullyOutOfView || overlapsStickyRegion)
            {
                _editingControl.Visible = false;
            }
            else
            {
                _editingControl.Location = new Point(cellRect.X, cellRect.Y);
                _editingControl.Visible = true;
            }
        }

        /// <summary>
        /// Closes the current editor
        /// </summary>
        private BeepCellConfig CloseCurrentEditor()
        {
            var previouslyEditingCell = _editingCell;
            EndEdit(true);
            return previouslyEditingCell;
        }

        /// <summary>
        /// Begins edit mode programmatically
        /// </summary>
        public void BeginEdit()
        {
            if (_selectedCell != null && !ReadOnly)
            {
                ShowCellEditor(_selectedCell, _selectedCell.Rect.Location);
            }
        }

        #endregion

        #region Cell Value Management

        /// <summary>
        /// Saves the edited value from the editor to the cell
        /// </summary>
        private void SaveEditedValue()
        {
            if (_tempcell == null)
            {
                return;
            }

            object newValue = _tempcell.CellValue;

            // Retrieve PropertyType from the corresponding column
            BeepColumnConfig columnConfig = Columns.FirstOrDefault(c => c.Index == _tempcell.ColumnIndex);
            if (columnConfig == null)
            {
                return;
            }
            Type propertyType = Type.GetType(columnConfig.PropertyTypeName, throwOnError: false) ?? typeof(string);

            // Convert new value to the correct type before comparing
            object convertedNewValue = MiscFunctions.ConvertValueToPropertyType(propertyType, newValue);
            object convertedOldValue = MiscFunctions.ConvertValueToPropertyType(propertyType, _tempcell.CellData);

            // Skip update if the new value is the same as the old value
            if (convertedNewValue != null && convertedOldValue != null && convertedNewValue.Equals(convertedOldValue))
            {
                return;
            }

            // Update cell's stored value
            _selectedCell.OldValue = _tempcell.CellData;
            _selectedCell.CellData = convertedNewValue;
            _selectedCell.CellValue = convertedNewValue;
            _selectedCell.IsDirty = true;

            // Update the corresponding data record
            UpdateDataRecordFromRow(_selectedCell);
            CellValueChanged?.Invoke(this, new BeepCellEventArgs(_selectedCell));

            // Trigger validation if necessary
            _selectedCell.ValidateCell();

            _tempcell = null;
        }

        /// <summary>
        /// Ends the current edit session
        /// </summary>
        private void EndEdit(bool acceptChanges)
        {
            if (_isEndingEdit || _editingControl == null || !IsEditorShown || _editingCell == null)
            {
                return;
            }

            _isEndingEdit = true;
            try
            {
                BeepCellConfig editedCell = _editingCell;

                if (acceptChanges)
                {
                    object newValue = _editingControl.GetValue();

                    BeepColumnConfig columnConfig = Columns[editedCell.ColumnIndex];
                    Type propertyType = Type.GetType(columnConfig.PropertyTypeName, throwOnError: false) ?? typeof(string);

                    object convertedNewValue = MiscFunctions.ConvertValueToPropertyType(propertyType, newValue);
                    object convertedOldValue = MiscFunctions.ConvertValueToPropertyType(propertyType, editedCell.CellData);

                    if (!Equals(convertedNewValue, convertedOldValue))
                    {
                        editedCell.CellValue = convertedNewValue;
                        editedCell.IsDirty = true;
                        if (editedCell.RowIndex < Rows.Count)
                        {
                            Rows[editedCell.RowIndex].IsDirty = true;
                        }
                        UpdateDataRecordFromRow(editedCell);
                    }
                }

                if (this.Controls.Contains(_editingControl))
                {
                    this.Controls.Remove(_editingControl);
                }

                _editingCell = null;
                IsEditorShown = false;
                EditorClosed?.Invoke(this, EventArgs.Empty);

                InvalidateCell(editedCell);
            }
            finally
            {
                _isEndingEdit = false;
            }
        }

        /// <summary>
        /// Cancels the current edit session without saving changes
        /// </summary>
        private void CancelEdit()
        {
            EndEdit(false);
        }

        /// <summary>
        /// Cancels editing and closes editor
        /// </summary>
        private void CancelEditing()
        {
            CloseCurrentEditor();
        }

        #endregion

        #region Editor Event Handlers

        /// <summary>
        /// Handles key down events in the cell editor
        /// </summary>
        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EndEdit(true);
                MoveNextCell();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Tab)
            {
                EndEdit(true);
                MoveNextCell();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CancelEdit();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles lost focus event for editor
        /// </summary>
        private void LostFocusHandler(object? sender, EventArgs e)
        {
            EndEdit(true);
        }

        /// <summary>
        /// Handles cancel (Escape) event
        /// </summary>
        private void Cancelhandler(object? sender, EventArgs e)
        {
            CancelEditing();
        }

        /// <summary>
        /// Handles Tab key press event
        /// </summary>
        private void Tabhandler(object? sender, EventArgs e)
        {
            MoveNextCell();
        }

        #endregion

        #region Cell Control Creation

        /// <summary>
        /// Creates a cell control for drawing (rendering)
        /// </summary>
        private IBeepUIComponent CreateCellControlForDrawing(BeepCellConfig cell)
        {
            var column = Columns[cell.ColumnIndex];

            switch (column.CellEditor)
            {
                case BeepColumnType.Text:
                    return new BeepTextBox { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.CheckBoxBool:
                    return new BeepCheckBoxBool { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true, GridMode = true };
                case BeepColumnType.CheckBoxString:
                    return new BeepCheckBoxString { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true, GridMode = true };
                case BeepColumnType.CheckBoxChar:
                    return new BeepCheckBoxChar { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true, GridMode = true };
                case BeepColumnType.ComboBox:
                    return new BeepComboBox { Theme = Theme, IsChild = true, ListItems = new BindingList<SimpleItem>(column.Items), GridMode = true };
                case BeepColumnType.DateTime:
                    return new BeepDatePicker { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.Image:
                    return new BeepImage { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.Button:
                    return new BeepButton { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.ProgressBar:
                    return new BeepProgressBar { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.NumericUpDown:
                    return new BeepNumericUpDown { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.Radio:
                    return new BeepRadioGroup { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.ListBox:
                    return new BeepListBox { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.ListOfValue:
                    return new BeepListofValuesBox { Theme = Theme, IsChild = true, GridMode = true };
                default:
                    return new BeepTextBox { Theme = Theme, IsChild = true, GridMode = true };
            }
        }

        /// <summary>
        /// Creates a cell control for editing
        /// </summary>
        private IBeepUIComponent CreateCellControlForEditing(BeepCellConfig cell)
        {
            var column = Columns[cell.ColumnIndex];

            switch (column.CellEditor)
            {
                case BeepColumnType.Text:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
                case BeepColumnType.CheckBoxBool:
                    return new BeepCheckBoxBool { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true };
                case BeepColumnType.CheckBoxString:
                    return new BeepCheckBoxString { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true };
                case BeepColumnType.CheckBoxChar:
                    return new BeepCheckBoxChar { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true };
                case BeepColumnType.ComboBox:
                    return new BeepComboBox { Theme = Theme, IsChild = true, ListItems = new BindingList<SimpleItem>(column.Items) };
                case BeepColumnType.DateTime:
                    return new BeepDatePicker { Theme = Theme, IsChild = true };
                case BeepColumnType.Image:
                    return new BeepImage { Theme = Theme, IsChild = true };
                case BeepColumnType.Button:
                    return new BeepButton { Theme = Theme, IsChild = true };
                case BeepColumnType.ProgressBar:
                    return new BeepProgressBar { Theme = Theme, IsChild = true };
                case BeepColumnType.NumericUpDown:
                    return new BeepNumericUpDown { Theme = Theme, IsChild = true };
                case BeepColumnType.Radio:
                    return new BeepRadioGroup { Theme = Theme, IsChild = true };
                case BeepColumnType.ListBox:
                    return new BeepListBox { Theme = Theme, IsChild = true };
                case BeepColumnType.ListOfValue:
                    return new BeepListofValuesBox { Theme = Theme, IsChild = true };
                default:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
            }
        }

        /// <summary>
        /// Updates a cell control with the specified value
        /// </summary>
        private void UpdateCellControl(IBeepUIComponent control, BeepColumnConfig column, BeepCellConfig cell, object value)
        {
            if (control == null) return;

            if (value == null)
            {
                // Handle null values
                switch (control)
                {
                    case BeepTextBox textBox:
                        textBox.Text = string.Empty;
                        break;
                    case BeepCheckBoxBool checkBox:
                        checkBox.State = CheckBoxState.Indeterminate;
                        break;
                    case BeepCheckBoxChar checkBox:
                        checkBox.State = CheckBoxState.Indeterminate;
                        break;
                    case BeepCheckBoxString checkBox:
                        checkBox.State = CheckBoxState.Indeterminate;
                        break;
                    case BeepComboBox comboBox:
                        comboBox.Reset();
                        break;
                    case BeepDatePicker datePicker:
                        datePicker.SelectedDate = null;
                        break;
                    case BeepRadioGroup radioButton:
                        radioButton.Reset();
                        break;
                    case BeepListofValuesBox listBox:
                        listBox.Reset();
                        break;
                    case BeepListBox listBox:
                        listBox.Reset();
                        break;
                    case BeepImage image:
                        image.ImagePath = null;
                        break;
                    case BeepButton button:
                        button.Text = string.Empty;
                        break;
                    case BeepProgressBar progressBar:
                        progressBar.Value = 0;
                        break;
                    case BeepNumericUpDown numericUpDown:
                        numericUpDown.Value = 0;
                        break;
                }
                return;
            }

            // Handle non-null values
            switch (control)
            {
                case BeepTextBox textBox:
                    textBox.Text = value.ToString();
                    if (column != null)
                        textBox.MaskFormat = GetTextBoxMaskFormat(column.ColumnType);
                    break;

                case BeepCheckBoxBool checkBox:
                    checkBox.CurrentValue = (bool)MiscFunctions.ConvertValueToPropertyType(typeof(bool), value);
                    break;

                case BeepCheckBoxChar checkBox:
                    checkBox.CurrentValue = (char)MiscFunctions.ConvertValueToPropertyType(typeof(char), value);
                    break;

                case BeepCheckBoxString checkBox:
                    checkBox.CurrentValue = (string)MiscFunctions.ConvertValueToPropertyType(typeof(string), value);
                    break;

                case BeepComboBox comboBox:
                    comboBox.Reset();
                    if (column.ParentColumnName != null)
                    {
                        BeepColumnConfig parentColumn = Columns.FirstOrDefault(c => c.ColumnName == column.ParentColumnName);
                        if (parentColumn != null)
                        {
                            UpdateCellFromParent(cell);
                            var filterditems = column.Items.Where(i => i.ParentValue?.ToString() == cell.ParentCellValue.ToString()).ToList();
                            comboBox.ListItems = new BindingList<SimpleItem>(filterditems);
                        }
                    }
                    else if (cell.FilterdList.Count > 0)
                    {
                        comboBox.ListItems = new BindingList<SimpleItem>(cell.FilterdList);
                    }
                    else if (cell.ParentCellValue != null)
                    {
                        BeepColumnConfig parentColumn = Columns.FirstOrDefault(c => c.ColumnName == column.ParentColumnName);
                        if (parentColumn != null)
                        {
                            var filterditems = column.Items.Where(i => i.ParentValue?.ToString() == cell.ParentCellValue.ToString()).ToList();
                            comboBox.ListItems = new BindingList<SimpleItem>(filterditems);
                        }
                    }
                    else if (column?.Items != null)
                    {
                        comboBox.ListItems = new BindingList<SimpleItem>(column.Items);
                    }

                    if (cell.CellValue != null)
                    {
                        if (value is SimpleItem simpleItem)
                        {
                            var item = comboBox.ListItems.FirstOrDefault(i => i.Text == simpleItem.Text && i.ImagePath == simpleItem.ImagePath);
                            if (item != null)
                            {
                                comboBox.SelectedItem = item;
                            }
                            else
                                comboBox.SelectedItem = comboBox.ListItems.FirstOrDefault();
                        }
                        else
                        {
                            string stringValue = value.ToString();
                            var item = comboBox.ListItems.FirstOrDefault(i => i.Value?.ToString() == stringValue || i.Text == stringValue);
                            if (item != null)
                            {
                                comboBox.SelectedItem = item;
                            }
                            else
                                comboBox.SelectedItem = comboBox.ListItems.FirstOrDefault();
                        }
                    }
                    break;

                case BeepDatePicker datePicker:
                    if (value is DateTime dateTime)
                    {
                        datePicker.SelectedDate = dateTime.ToString(datePicker.GetCurrentFormat(), datePicker.Culture);
                    }
                    else if (DateTime.TryParse(value.ToString(), datePicker.Culture, DateTimeStyles.None, out DateTime dateValue))
                    {
                        datePicker.SelectedDate = dateValue.ToString(datePicker.GetCurrentFormat(), datePicker.Culture);
                    }
                    else
                    {
                        datePicker.SelectedDate = null;
                    }
                    break;

                case BeepNumericUpDown numericUpDown:
                    numericUpDown.Value = (decimal)MiscFunctions.ConvertValueToPropertyType(typeof(decimal), value);
                    break;

                case BeepButton button:
                    button.Text = value.ToString();
                    break;

                case BeepProgressBar progressBar:
                    progressBar.Value = int.TryParse(value.ToString(), out int progress) ? progress : 0;
                    break;
            }
        }

        /// <summary>
        /// Updates cell value from parent cell (for cascading dropdowns)
        /// </summary>
        private void UpdateCellFromParent(BeepCellConfig cell)
        {
            var column = Columns[cell.ColumnIndex];
            if (column == null) return;
            if (column.ParentColumnName == null) return;
            var parentColumn = Columns.FirstOrDefault(c => c.ColumnName == column.ParentColumnName);
            if (parentColumn == null) return;
            var parentCell = Rows[cell.RowIndex].Cells.FirstOrDefault(c => c.ColumnIndex == parentColumn.Index);
            if (parentCell == null) return;
            if (parentCell.CellValue == null) return;

            cell.ParentCellValue = parentCell.CellValue;
            cell.ParentItem = parentCell.Item;
        }

        /// <summary>
        /// Gets the text box mask format based on column type
        /// </summary>
        private TextBoxMaskFormat GetTextBoxMaskFormat(DbFieldCategory columnType)
        {
            return columnType switch
            {
                DbFieldCategory.Numeric => TextBoxMaskFormat.Numeric,
                DbFieldCategory.Date => TextBoxMaskFormat.Date,
                DbFieldCategory.Timestamp => TextBoxMaskFormat.DateTime,
                DbFieldCategory.Currency => TextBoxMaskFormat.Currency,
                _ => TextBoxMaskFormat.None
            };
        }

        #endregion

        #region Validation

        /// <summary>
        /// Invalidates a column region for redrawing
        /// </summary>
        private void InvalidateColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Columns.Count || !Columns[columnIndex].Visible)
                return;

            int x = gridRect.Left;
            for (int i = 0; i < columnIndex; i++)
            {
                if (Columns[i].Visible)
                    x += Columns[i].Width;
            }

            Rectangle columnRect = new Rectangle(x - _xOffset, gridRect.Top, Columns[columnIndex].Width, gridRect.Height);
            Invalidate(columnRect);
        }

        #endregion
    }
}
