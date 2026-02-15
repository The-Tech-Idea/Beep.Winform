using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridDialogHelper
    {
        private readonly BeepGridPro _grid;
        private Form _editorDialog;
        private Form _filterDialog;
        private BeepControl _currentEditor;
        private IBeepUIComponent _currenteditorUIcomponent;
        private int _lastSearchRow = -1;
        private int _lastSearchCol = -1;
        private string _lastSearchQuery = string.Empty;
        private string _currentSearchFilter = string.Empty; // Tracks the last applied search filter for persistence
        internal bool HasActiveInlineOverlay =>
            (_inlineCriterionMenu?.Visible ?? false);
        private void CloseInlineCriterionMenu()
        {
            CloseInlineCriterionMenuCore();
        }
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
                _grid.SafeInvalidate();
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
        public void ShowFilterDialog(string? preferredColumnName = null, string? preferredFilterText = null)
        {
            // Close any existing filter dialog
            CloseFilterDialog();

            // Create filter panel
            var filterPanel = CreateFilterPanel(preferredColumnName, preferredFilterText);

            // Create and configure dialog
            _filterDialog = CreateFilterDialog(filterPanel);
            _filterDialog.StartPosition = FormStartPosition.CenterParent;
            ApplyDpiScaling(_filterDialog);

            // Show dialog
            _filterDialog.ShowDialog(_grid);

            CloseFilterDialog();
        }

        /// <summary>
        /// <summary>
        /// Shows a search dialog for the grid
        /// </summary>
        public void ShowSearchDialog()
        {
            using var searchDialog = new Form
            {
                Text = "Quick Search",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(720, 120),
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var quickSearchFilter = new BeepFilter
            {
                Dock = DockStyle.Fill,
                FilterStyle = FilterStyle.QuickSearch,
                DisplayMode = FilterDisplayMode.AlwaysVisible,
                AutoApply = true,
                ShowActionButtons = false,
                IsChild = true,
                IsFrameless = true,
                Theme = _grid.Theme,
                EntityStructure = CreateQuickSearchEntityStructure()
            };

            var initialFilter = new FilterConfiguration("Quick Search")
            {
                IsActive = !string.IsNullOrWhiteSpace(_currentSearchFilter)
            };
            initialFilter.Criteria.Add(new FilterCriteria
            {
                ColumnName = "All Columns",
                Operator = FilterOperator.Contains,
                Value = _currentSearchFilter,
                IsEnabled = true
            });
            quickSearchFilter.ActiveFilter = initialFilter;

            ContextMenuStrip? activeContextMenu = null;

            void CloseActiveContextMenu()
            {
                if (activeContextMenu == null)
                {
                    return;
                }

                try
                {
                    if (!activeContextMenu.IsDisposed)
                    {
                        activeContextMenu.Close();
                        activeContextMenu.Dispose();
                    }
                }
                catch (ObjectDisposedException)
                {
                }
                finally
                {
                    activeContextMenu = null;
                }
            }

            quickSearchFilter.FieldSelectionRequested += (_, e) =>
            {
                if (e.Index < 0 || e.Index >= quickSearchFilter.ActiveFilter.Criteria.Count)
                {
                    return;
                }

                CloseActiveContextMenu();
                var menu = new ContextMenuStrip();
                activeContextMenu = menu;

                foreach (var column in quickSearchFilter.AvailableColumns)
                {
                    var selectedColumn = column.ColumnName;
                    var display = string.IsNullOrWhiteSpace(column.DisplayName) ? selectedColumn : column.DisplayName;
                    var item = new ToolStripMenuItem(display);
                    item.Click += (_, __) =>
                    {
                        var config = quickSearchFilter.ActiveFilter.Clone();
                        if (config.Criteria.Count == 0)
                        {
                            config.Criteria.Add(new FilterCriteria
                            {
                                ColumnName = selectedColumn,
                                Operator = FilterOperator.Contains,
                                Value = string.Empty,
                                IsEnabled = true
                            });
                        }
                        else
                        {
                            config.Criteria[0].ColumnName = selectedColumn;
                        }

                        quickSearchFilter.ActiveFilter = config;
                    };
                    menu.Items.Add(item);
                }

                menu.Closed += (_, __) =>
                {
                    if (ReferenceEquals(activeContextMenu, menu))
                    {
                        activeContextMenu = null;
                    }

                    if (!menu.IsDisposed)
                    {
                        menu.Dispose();
                    }
                };

                menu.Show(quickSearchFilter, new Point(e.Bounds.Left, e.Bounds.Bottom));
            };

            quickSearchFilter.FilterChanged += (_, __) =>
            {
                if (quickSearchFilter.ActiveFilter.Criteria.Count == 0)
                {
                    _currentSearchFilter = string.Empty;
                    _grid.ClearFilter();
                    return;
                }

                var criterion = quickSearchFilter.ActiveFilter.Criteria[0];
                var text = criterion.Value?.ToString() ?? string.Empty;
                var columnName = criterion.ColumnName;

                _currentSearchFilter = text;

                if (string.Equals(columnName, "All Columns", StringComparison.OrdinalIgnoreCase))
                {
                    columnName = null;
                }

                _grid.ApplyQuickFilter(text, columnName);
            };

            quickSearchFilter.FilterCanceled += (_, __) => searchDialog.Close();

            searchDialog.FormClosed += (_, __) => CloseActiveContextMenu();
            searchDialog.Controls.Add(quickSearchFilter);
            searchDialog.Shown += (_, __) =>
            {
                quickSearchFilter.ApplyTheme();
                quickSearchFilter.Refresh();
            };

            searchDialog.ShowDialog(_grid);
        }

        private EntityStructure CreateQuickSearchEntityStructure()
        {
            var entity = new EntityStructure
            {
                EntityName = "GridQuickSearch",
                Fields = new List<EntityField>()
            };

            entity.Fields.Add(new EntityField
            {
                FieldName = "All Columns",
                Originalfieldname = "All Columns",
                Fieldtype = typeof(string).FullName,
                AllowDBNull = true,
                IsKey = false
            });

            foreach (var column in _grid.Data.Columns.Where(c => c != null && c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn))
            {
                entity.Fields.Add(new EntityField
                {
                    FieldName = column.ColumnName,
                    Originalfieldname = string.IsNullOrWhiteSpace(column.ColumnCaption) ? column.ColumnName : column.ColumnCaption,
                    Fieldtype = column.DataType?.FullName ?? typeof(string).FullName,
                    AllowDBNull = true,
                    IsKey = false
                });
            }

            return entity;
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
                _grid.SafeInvalidate();
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
            CloseInlineCriterionMenu();
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
                BeepColumnType.Radio => new BeepRadioGroup { IsChild = false },
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
