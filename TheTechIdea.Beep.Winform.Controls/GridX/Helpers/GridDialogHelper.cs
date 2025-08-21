using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridDialogHelper
    {
        private readonly BeepGridPro _grid;
        private Form _editorDialog;
        private Form _filterDialog;
        private BeepControl _currentEditor;

        public GridDialogHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        /// <summary>
        /// Shows an editor dialog for the specified cell
        /// </summary>
        public void ShowEditorDialog(BeepCellConfig cell)
        {
            if (cell == null || !cell.IsEditable || _grid.ReadOnly)
                return;

            var column = _grid.Data.Columns[cell.ColumnIndex];
            if (column.ReadOnly)
                return;

            // Close any existing editor dialog
            CloseEditorDialog();

            // Create editor control
            _currentEditor = CreateEditorForColumn(column);
            if (_currentEditor == null)
                return;

            // Set initial value
            if (_currentEditor is IBeepUIComponent uiComponent)
            {
                uiComponent.SetValue(cell.CellValue);
            }

            // Create and configure dialog
            _editorDialog = CreateEditorDialog(column.ColumnCaption ?? column.ColumnName, _currentEditor);
            _editorDialog.StartPosition = FormStartPosition.CenterParent;

            // Show dialog and handle result
            var result = _editorDialog.ShowDialog(_grid);
            
            if (result == DialogResult.OK && _currentEditor is IBeepUIComponent finalComponent)
            {
                // Get the new value and update the cell
                var newValue = finalComponent.GetValue();
                
                // Update the cell value
                cell.CellValue = newValue;
                cell.IsDirty = true;
                
                // Update the data source
                _grid.Data.UpdateCellValue(cell, newValue);
                
                // Notify of changes
                _grid.OnCellValueChanged(cell);
                
                // Refresh the grid
                _grid.Invalidate();
            }

            CloseEditorDialog();
        }

        /// <summary>
        /// Shows a filter dialog for the grid
        /// </summary>
        public void ShowFilterDialog()
        {
            // Close any existing filter dialog
            CloseFilterDialog();

            // Create filter panel
            var filterPanel = CreateFilterPanel();

            // Create and configure dialog
            _filterDialog = CreateFilterDialog(filterPanel);
            _filterDialog.StartPosition = FormStartPosition.CenterParent;

            // Show dialog
            _filterDialog.ShowDialog(_grid);

            CloseFilterDialog();
        }

        /// <summary>
        /// Shows a search dialog for the grid
        /// </summary>
        public void ShowSearchDialog()
        {
            // Create search panel
            var searchPanel = CreateSearchPanel();

            // Create and configure dialog
            using var searchDialog = CreateSearchDialog(searchPanel);
            searchDialog.StartPosition = FormStartPosition.CenterParent;

            // Show dialog
            searchDialog.ShowDialog(_grid);
        }

        /// <summary>
        /// Shows a column configuration dialog
        /// </summary>
        public void ShowColumnConfigDialog()
        {
            // Create column config panel
            var configPanel = CreateColumnConfigPanel();

            // Create and configure dialog
            using var configDialog = CreateColumnConfigDialog(configPanel);
            configDialog.StartPosition = FormStartPosition.CenterParent;

            // Show dialog
            var result = configDialog.ShowDialog(_grid);

            if (result == DialogResult.OK)
            {
                // Apply column changes
                ApplyColumnChanges(configPanel);
                _grid.Invalidate();
            }
        }

        /// <summary>
        /// Closes the editor dialog if open
        /// </summary>
        public void CloseEditorDialog()
        {
            if (_editorDialog != null)
            {
                _editorDialog.Close();
                _editorDialog.Dispose();
                _editorDialog = null;
            }

            if (_currentEditor != null)
            {
                _currentEditor.Dispose();
                _currentEditor = null;
            }
        }

        /// <summary>
        /// Closes the filter dialog if open
        /// </summary>
        public void CloseFilterDialog()
        {
            if (_filterDialog != null)
            {
                _filterDialog.Close();
                _filterDialog.Dispose();
                _filterDialog = null;
            }
        }

        private BeepControl CreateEditorForColumn(BeepColumnConfig column)
        {
            BeepControl editor = column.CellEditor switch
            {
                BeepColumnType.Text => new BeepTextBox { IsChild = false, IsFrameless = false, ShowAllBorders = true },
                BeepColumnType.CheckBoxBool => new BeepCheckBoxBool { IsChild = false },
                BeepColumnType.CheckBoxChar => new BeepCheckBoxChar { IsChild = false },
                BeepColumnType.CheckBoxString => new BeepCheckBoxString { IsChild = false },
                BeepColumnType.ComboBox => new BeepComboBox { IsChild = false, ListItems = new System.ComponentModel.BindingList<SimpleItem>(column.Items ?? new List<SimpleItem>()) },
                BeepColumnType.DateTime => new BeepDatePicker { IsChild = false },
                BeepColumnType.Image => new BeepImage { IsChild = false },
                BeepColumnType.Button => new BeepButton { IsChild = false },
                BeepColumnType.ProgressBar => new BeepProgressBar { IsChild = false },
                BeepColumnType.NumericUpDown => new BeepNumericUpDown { IsChild = false },
                BeepColumnType.Radio => new BeepRadioButton { IsChild = false },
                BeepColumnType.ListBox => new BeepListBox { IsChild = false, ListItems = new System.ComponentModel.BindingList<SimpleItem>(column.Items ?? new List<SimpleItem>()) },
                BeepColumnType.ListOfValue => new BeepListofValuesBox { IsChild = false, ListItems = column.Items ?? new List<SimpleItem>() },
                _ => new BeepTextBox { IsChild = false, IsFrameless = false, ShowAllBorders = true }
            };

            editor.Theme = _grid.Theme;
            editor.Size = new Size(300, 30);
            editor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            return editor;
        }

        private Form CreateEditorDialog(string title, Control editor)
        {
            var dialog = new Form
            {
                Text = $"Edit {title}",
                Size = new Size(400, 150),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.CenterParent
            };

            // Create OK/Cancel buttons
            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Size = new Size(75, 25),
                Location = new Point(240, 80)
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Size = new Size(75, 25),
                Location = new Point(320, 80)
            };

            // Position editor
            editor.Location = new Point(10, 20);
            editor.Size = new Size(dialog.Width - 40, 30);

            // Add controls
            dialog.Controls.Add(editor);
            dialog.Controls.Add(okButton);
            dialog.Controls.Add(cancelButton);

            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;

            return dialog;
        }

        private Panel CreateFilterPanel()
        {
            var panel = new Panel
            {
                Size = new Size(500, 200),
                BorderStyle = BorderStyle.None
            };

            // Create filter controls
            var filterLabel = new Label
            {
                Text = "Filter by Column:",
                Location = new Point(10, 15),
                Size = new Size(100, 20)
            };

            var columnCombo = new ComboBox
            {
                Location = new Point(120, 12),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Populate column combo
            columnCombo.Items.Add("All Columns");
            foreach (var col in _grid.Data.Columns.Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn))
            {
                columnCombo.Items.Add(col.ColumnCaption ?? col.ColumnName);
            }
            columnCombo.SelectedIndex = 0;

            var filterTextLabel = new Label
            {
                Text = "Filter Text:",
                Location = new Point(10, 50),
                Size = new Size(100, 20)
            };

            var filterTextBox = new TextBox
            {
                Location = new Point(120, 47),
                Size = new Size(200, 25),
                Name = "FilterTextBox"
            };

            var applyButton = new Button
            {
                Text = "Apply Filter",
                Location = new Point(120, 85),
                Size = new Size(100, 30)
            };

            var clearButton = new Button
            {
                Text = "Clear Filter",
                Location = new Point(230, 85),
                Size = new Size(100, 30)
            };

            // Add event handlers
            applyButton.Click += (s, e) => ApplyFilter(columnCombo.SelectedItem?.ToString(), filterTextBox.Text);
            clearButton.Click += (s, e) => ClearFilter();

            // Add controls to panel
            panel.Controls.AddRange(new Control[] {
                filterLabel, columnCombo, filterTextLabel, filterTextBox, applyButton, clearButton
            });

            return panel;
        }

        private Panel CreateSearchPanel()
        {
            var panel = new Panel
            {
                Size = new Size(400, 150),
                BorderStyle = BorderStyle.None
            };

            var searchLabel = new Label
            {
                Text = "Search for:",
                Location = new Point(10, 15),
                Size = new Size(80, 20)
            };

            var searchTextBox = new TextBox
            {
                Location = new Point(100, 12),
                Size = new Size(200, 25),
                Name = "SearchTextBox"
            };

            var searchButton = new Button
            {
                Text = "Search",
                Location = new Point(100, 50),
                Size = new Size(80, 30)
            };

            var searchNextButton = new Button
            {
                Text = "Find Next",
                Location = new Point(190, 50),
                Size = new Size(80, 30)
            };

            // Add event handlers
            searchButton.Click += (s, e) => PerformSearch(searchTextBox.Text, true);
            searchNextButton.Click += (s, e) => PerformSearch(searchTextBox.Text, false);

            panel.Controls.AddRange(new Control[] {
                searchLabel, searchTextBox, searchButton, searchNextButton
            });

            return panel;
        }

        private Panel CreateColumnConfigPanel()
        {
            var panel = new Panel
            {
                Size = new Size(600, 400),
                BorderStyle = BorderStyle.None
            };

            // Create column list
            var columnListBox = new CheckedListBox
            {
                Location = new Point(10, 30),
                Size = new Size(200, 300),
                Name = "ColumnListBox"
            };

            // Populate column list
            foreach (var col in _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID))
            {
                columnListBox.Items.Add(col.ColumnCaption ?? col.ColumnName, col.Visible);
            }

            var columnLabel = new Label
            {
                Text = "Visible Columns:",
                Location = new Point(10, 10),
                Size = new Size(100, 20)
            };

            // Add width controls
            var widthLabel = new Label
            {
                Text = "Column Width:",
                Location = new Point(230, 30),
                Size = new Size(100, 20)
            };

            var widthNumeric = new NumericUpDown
            {
                Location = new Point(230, 55),
                Size = new Size(100, 25),
                Minimum = 20,
                Maximum = 500,
                Name = "WidthNumeric"
            };

            var applyWidthButton = new Button
            {
                Text = "Apply Width",
                Location = new Point(230, 90),
                Size = new Size(100, 30)
            };

            // Add event handlers
            columnListBox.SelectedIndexChanged += (s, e) =>
            {
                if (columnListBox.SelectedIndex >= 0)
                {
                    var selectedCol = _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList()[columnListBox.SelectedIndex];
                    widthNumeric.Value = selectedCol.Width;
                }
            };

            applyWidthButton.Click += (s, e) =>
            {
                if (columnListBox.SelectedIndex >= 0)
                {
                    var selectedCol = _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList()[columnListBox.SelectedIndex];
                    selectedCol.Width = (int)widthNumeric.Value;
                }
            };

            panel.Controls.AddRange(new Control[] {
                columnLabel, columnListBox, widthLabel, widthNumeric, applyWidthButton
            });

            return panel;
        }

        private Form CreateFilterDialog(Panel filterPanel)
        {
            var dialog = new Form
            {
                Text = "Filter Data",
                Size = new Size(520, 280),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            filterPanel.Location = new Point(10, 10);
            dialog.Controls.Add(filterPanel);

            var closeButton = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(420, 210),
                Size = new Size(75, 25)
            };

            dialog.Controls.Add(closeButton);
            dialog.AcceptButton = closeButton;

            return dialog;
        }

        private Form CreateSearchDialog(Panel searchPanel)
        {
            var dialog = new Form
            {
                Text = "Search Data",
                Size = new Size(420, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            searchPanel.Location = new Point(10, 10);
            dialog.Controls.Add(searchPanel);

            var closeButton = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(320, 130),
                Size = new Size(75, 25)
            };

            dialog.Controls.Add(closeButton);
            dialog.AcceptButton = closeButton;

            return dialog;
        }

        private Form CreateColumnConfigDialog(Panel configPanel)
        {
            var dialog = new Form
            {
                Text = "Column Configuration",
                Size = new Size(620, 480),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            configPanel.Location = new Point(10, 10);
            dialog.Controls.Add(configPanel);

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(450, 420),
                Size = new Size(75, 25)
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(530, 420),
                Size = new Size(75, 25)
            };

            dialog.Controls.AddRange(new Control[] { okButton, cancelButton });
            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;

            return dialog;
        }

        private void ApplyFilter(string columnName, string filterText)
        {
            // Implement filter logic here
            // This would call the existing filter functionality in the grid
            if (string.IsNullOrWhiteSpace(filterText))
            {
                ClearFilter();
                return;
            }

            // Apply filter through the grid's existing filter mechanism
            // You would implement this based on your grid's filter functionality
        }

        private void ClearFilter()
        {
            // Clear any applied filters
            // This would call the existing clear filter functionality in the grid
        }

        private void PerformSearch(string searchText, bool fromStart)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return;

            // Implement search logic here
            // This would search through the grid data and highlight/select matching cells
        }

        private void ApplyColumnChanges(Panel configPanel)
        {
            var columnListBox = configPanel.Controls.OfType<CheckedListBox>().FirstOrDefault(c => c.Name == "ColumnListBox");
            if (columnListBox == null)
                return;

            var visibleColumns = _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList();

            for (int i = 0; i < columnListBox.Items.Count && i < visibleColumns.Count; i++)
            {
                visibleColumns[i].Visible = columnListBox.GetItemChecked(i);
            }
        }

        public void Dispose()
        {
            CloseEditorDialog();
            CloseFilterDialog();
        }
    }
}