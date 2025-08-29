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
        private IBeepUIComponent _currenteditorUIcomponent;
        public GridDialogHelper(BeepGridPro grid)
        {
            _grid = grid;
        }
        BeepCellConfig Currentcell;
        // DPI helpers
        private float GetDpiScale()
        {
            // No-op: let Windows handle DPI (PerMonitorV2)
            return 1f;
        }

        private void ApplyDpiScaling(Form dialog)
        {
            // No-op: avoid manual scaling to prevent double scaling
        }

        private Keys _pendingNavKey = Keys.None;
        private bool _pendingReopenEditor = false;
        private object _originalValue;

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
            Currentcell = cell;
            _pendingNavKey = Keys.None;
            _pendingReopenEditor = false;

            // Create editor control
            _currenteditorUIcomponent = CreateEditorForColumn(column);
            _currentEditor = _currenteditorUIcomponent as BeepControl;
            if (_currentEditor == null)
                return;

            // Keep original value for cancel
            _originalValue = cell.CellValue;

            // Set initial value
            if (_currentEditor is IBeepUIComponent uiComponent)
            {
                uiComponent.SetValue(cell.CellValue);
            }

            // Create a borderless popup aligned to the cell bounds with same size
            _editorDialog = CreateCellOverlayEditorDialog(cell, _currentEditor);


            // Ensure Tab and arrows are treated as input keys by the editor and all its children
            WireEditorKeyHooks(_currentEditor);

            // Monitor clicks on the grid to close editor when user clicks outside the overlay
            AttachOutsideClickMonitor();

            // Commit on deactivate (click outside)
            _editorDialog.Deactivate += (s, e) =>
            {
                if (_editorDialog != null && _editorDialog.Visible)
                {
                    CommitAndClose(cell);
                }
            };

            // When the overlay closes, perform any scheduled navigation
            _editorDialog.FormClosed += (s, e) => { PerformPendingNavigation(); DetachOutsideClickMonitor(); };

            // Show modeless to allow grid interaction after close
            _editorDialog.Show(_grid);

            // Focus the editor
            _editorDialog.BeginInvoke(new Action(() =>
            {
                try { _currentEditor?.Focus(); } catch { }
            }));
        }

        private void WireEditorKeyHooks(Control root)
        {
            if (root == null) return;
            root.PreviewKeyDown -= Editor_PreviewKeyDown;
            root.PreviewKeyDown += Editor_PreviewKeyDown;
            root.KeyDown -= Editor_KeyDown;
            root.KeyDown += Editor_KeyDown;

            // Hook existing children
            foreach (Control child in root.Controls)
            {
                WireEditorKeyHooks(child);
            }

            // Hook future children dynamically
            root.ControlAdded -= Root_ControlAdded;
            root.ControlAdded += Root_ControlAdded;
        }

        private void Root_ControlAdded(object? sender, ControlEventArgs e)
        {
            WireEditorKeyHooks(e.Control);
        }

        private void Editor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _pendingNavKey = Keys.Down;
                _pendingReopenEditor = true;
                e.Handled = true;
                CommitAndClose(Currentcell);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                CancelAndClose(_originalValue);
            }
            else if (e.KeyCode == Keys.Tab)
            {
                _pendingNavKey = (e.Shift ? Keys.Left : Keys.Right);
                _pendingReopenEditor = true;
                e.Handled = true;
                CommitAndClose(Currentcell);
            }
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                _pendingNavKey = e.KeyCode;
                _pendingReopenEditor = true;
                e.Handled = true;
                CommitAndClose(Currentcell);
            }
        }

        private void Editor_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            // Treat Tab and arrow keys as input keys so KeyDown will fire
            if (e.KeyCode == Keys.Tab ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.Up ||
                e.KeyCode == Keys.Down)
            {
                e.IsInputKey = true;
            }
        }

        // Commit/cancel helpers
        void CommitAndClose(BeepCellConfig cell)
        {
            if (_currentEditor is IBeepUIComponent finalComponent)
            {
                var newValue = finalComponent.GetValue();
                cell.CellValue = newValue;
                cell.IsDirty = true;
                _grid.Data.UpdateCellValue(cell, newValue);
                _grid.OnCellValueChanged(cell);
                _grid.Invalidate();
            }
            _editorDialog?.Close();
            // If modeless, the FormClosed handler will run; but also trigger nav now just in case
            PerformPendingNavigation();
        }

        void CancelAndClose(object originalValue)
        {
            if (_currentEditor is IBeepUIComponent revertComponent)
            {
                revertComponent.SetValue(originalValue);
            }
            // prevent any navigation after cancel
            _pendingNavKey = Keys.None;
            _pendingReopenEditor = false;
            _editorDialog?.Close();
        }

        private void PerformPendingNavigation()
        {
            if (_pendingNavKey == Keys.None) return;

            try
            {
                var navKey = _pendingNavKey;
                var reopen = _pendingReopenEditor;
                _pendingNavKey = Keys.None;
                _pendingReopenEditor = false;

                _grid.Input?.HandleKeyDown(new KeyEventArgs(navKey));

                if (reopen)
                {
                    _grid.BeginInvoke(new Action(() =>
                    {
                        try { _grid.ShowCellEditor(); } catch { }
                    }));
                }
            }
            catch { }
        }

        private Form CreateCellOverlayEditorDialog(BeepCellConfig cell, Control editor)
        {
            var dialog = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                TopMost = false,
                BackColor = SystemColors.Window
            };

            // Determine cell rectangle in screen coordinates
            var cellRect = cell.Rect;
            if (cellRect.Width <= 0 || cellRect.Height <= 0)
            {
                cellRect = new Rectangle(0, 0, 200, 30);
            }
            var screenLoc = _grid.PointToScreen(cellRect.Location);

            // Clamp to screen working area
            var screen = Screen.FromControl(_grid);
            var work = screen.WorkingArea;
            int x = Math.Max(work.Left, Math.Min(screenLoc.X, work.Right - cellRect.Width));
            int y = Math.Max(work.Top, Math.Min(screenLoc.Y, work.Bottom - cellRect.Height));

            dialog.Bounds = new Rectangle(x, y, cellRect.Width, cellRect.Height);

            // Fill with editor
            editor.Dock = DockStyle.Fill;
            dialog.Controls.Add(editor);
            dialog.AutoScaleMode = AutoScaleMode.None;

            return dialog;
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
            ApplyDpiScaling(_filterDialog);

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
            ApplyDpiScaling(searchDialog);

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
            ApplyDpiScaling(configDialog);

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

            DetachOutsideClickMonitor();
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

        private IBeepUIComponent CreateEditorForColumn(BeepColumnConfig column)
        {
            IBeepUIComponent editor = column.CellEditor switch
            {
                BeepColumnType.Text => new BeepTextBox { IsChild = false, IsFrameless = false, ShowAllBorders = true },
                BeepColumnType.CheckBoxBool => new BeepCheckBoxBool { IsChild = false ,HideText=true},
                BeepColumnType.CheckBoxChar => new BeepCheckBoxChar { IsChild = false, HideText = true },
                BeepColumnType.CheckBoxString => new BeepCheckBoxString { IsChild = false, HideText = true },
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
            editor.SubmitChanges -= Editor_SubmitChanges;
            editor.SubmitChanges += Editor_SubmitChanges;
            editor.Theme = _grid.Theme;
            Control control = editor as Control;
            control.Size = new Size(300, 30);
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            return editor;
        }

        private void Editor_SubmitChanges(object? sender, BeepComponentEventArgs e)
        {
            CommitAndClose(Currentcell);
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

        // Monitor clicks on the grid to close editor when user clicks outside the overlay
        private void AttachOutsideClickMonitor()
        {
            try
            {
                _grid.MouseDown -= Grid_MouseDown_CloseEditorIfOutside;
                _grid.MouseDown += Grid_MouseDown_CloseEditorIfOutside;
            }
            catch { }
        }

        private void DetachOutsideClickMonitor()
        {
            try { _grid.MouseDown -= Grid_MouseDown_CloseEditorIfOutside; } catch { }
        }

        private void Grid_MouseDown_CloseEditorIfOutside(object? sender, MouseEventArgs e)
        {
            if (_editorDialog == null || !_editorDialog.Visible) return;

            // Translate grid click to screen coords and check if inside editor bounds
            var screenPt = _grid.PointToScreen(e.Location);
            if (!_editorDialog.Bounds.Contains(screenPt))
            {
                // Commit current edit and close. Do not block the event; GridInputHelper will handle selection.
                CommitAndClose(Currentcell);
                // No return; allow the grid's own MouseDown pipeline to process this click normally.
            }
        }

        public void Dispose()
        {
            CloseEditorDialog();
            CloseFilterDialog();
        }
    }
}