using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.GridX.Editors;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridEditHelper
    {
        private readonly BeepGridPro _grid;
        private readonly GridEditorFactory _editorFactory = new();
        private Control _editorControl;
        private IGridEditor _currentEditor;
        private readonly EditorEvents _editorEvents;
        private BeepCellConfig _editingCell;
        private object _originalValue;
        private bool _suppressLostFocus;
        private bool _isEndingEdit;

        public GridEditHelper(BeepGridPro grid)
        {
            _grid = grid;
            _editorEvents = new EditorEvents(this);
        }

        public void BeginEdit()
        {
            System.Diagnostics.Debug.WriteLine($"BeginEdit called. ReadOnly={_grid.ReadOnly}, HasSelection={_grid.Selection.HasSelection}");

            if (_grid.ReadOnly)
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: Grid is ReadOnly");
                return;
            }

            if (!_grid.Selection.HasSelection)
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: No selection");
                return;
            }

            int r = _grid.Selection.RowIndex;
            int c = _grid.Selection.ColumnIndex;
            System.Diagnostics.Debug.WriteLine($"BeginEdit: Row={r}, Col={c}, RowCount={_grid.Data.Rows.Count}, ColCount={_grid.Data.Columns.Count}");

            if (r < 0 || c < 0 || r >= _grid.Data.Rows.Count || c >= _grid.Data.Columns.Count)
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: Invalid row or column index");
                return;
            }

            var cell = _grid.Data.Rows[r].Cells[c];
            var col = _grid.Data.Columns[c];
            System.Diagnostics.Debug.WriteLine($"BeginEdit: Cell IsReadOnly={cell.IsReadOnly}, IsEditable={cell.IsEditable}, ColVisible={col.Visible}");
            System.Diagnostics.Debug.WriteLine($"BeginEdit: Cell Rect={cell.Rect}");

            if (cell.IsReadOnly || !cell.IsEditable || !col.Visible)
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: Cell is readonly or not editable");
                return;
            }

            // Ensure we have up-to-date cell rectangles before creating/positioning the editor
            if (cell.Rect.Width <= 0 || cell.Rect.Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: Cell Rect is empty, recalculating layout...");
                _grid.UpdateDrawingRect();
                _grid.Layout?.EnsureCalculated();
                _grid.SafeInvalidate();
                cell = _grid.Data.Rows[r].Cells[c];
                System.Diagnostics.Debug.WriteLine($"BeginEdit: After recalc, Cell Rect={cell.Rect}");

                if (cell.Rect.Width <= 0 || cell.Rect.Height <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("BeginEdit: Cell Rect still invalid after recalculation");
                    return;
                }
            }

            System.Diagnostics.Debug.WriteLine("BeginEdit: Creating editor control...");
            cell = _grid.Data.Rows[r].Cells[c];

            // Remove previous editor from host - ALWAYS clear completely
            CleanupPreviousEditor();

            // Resolve editor from factory
            _currentEditor = _editorFactory.Resolve(col.CellEditor);
            System.Diagnostics.Debug.WriteLine($"BeginEdit: Factory resolved editor: {_currentEditor?.GetType().Name ?? "NULL"}");

            _editorControl = _currentEditor?.CreateControl();
            System.Diagnostics.Debug.WriteLine($"BeginEdit: CreateControl returned: {_editorControl?.GetType().Name ?? "NULL"}");

            if (_editorControl == null)
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: Editor control is null, exiting");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"BeginEdit: Editor created successfully: {_editorControl.GetType().Name}");
            _editingCell = cell;
            _originalValue = cell.CellValue;
            _isEndingEdit = false;

            // Apply base control styling
            if (_editorControl is BaseControl baseEditor)
            {
                baseEditor.Theme = _grid.Theme;
                baseEditor.IsChild = true;
                baseEditor.ShowAllBorders = false;
            }

            _editorControl.Visible = true;
            _editorControl.TabStop = true;
            _editorControl.Enabled = true;
            _editorControl.BackColor = Color.White;
            _editorControl.ForeColor = Color.Black;

            // Editor-specific setup via factory pattern
            _currentEditor.Setup(_editorControl, col, cell, _grid.Theme);
            _currentEditor.SetValue(_editorControl, cell.CellValue);
            _currentEditor.AttachEvents(_editorControl, _editorEvents);

            // Get the cell rect - convert to grid-relative coordinates
            var rect = cell.Rect;
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                rect = new Rectangle(Math.Max(0, _grid.Layout.RowsRect.Left + 2),
                                     Math.Max(0, _grid.Layout.RowsRect.Top + 2),
                                     Math.Max(20, col.Width),
                                     Math.Max(18, _grid.RowHeight));
            }

            // Calculate editor bounds to perfectly fit within the cell
            int padding = 2;
            var editorRect = new Rectangle(
                rect.X + padding,
                rect.Y + padding,
                rect.Width - (padding * 2),
                rect.Height - (padding * 2)
            );

            if (editorRect.Width < 10) editorRect.Width = Math.Max(10, rect.Width - 4);
            if (editorRect.Height < 10) editorRect.Height = Math.Max(10, rect.Height - 4);

            // Always host inline editors inside the grid
            if (_editorControl.Parent != _grid)
            {
                _editorControl.Parent?.Controls.Remove(_editorControl);
                _grid.Controls.Add(_editorControl);
            }
            _editorControl.Bounds = editorRect;
            System.Diagnostics.Debug.WriteLine($"BeginEdit: Editor bounds set to: {_editorControl.Bounds}");

            _editorControl.Anchor = AnchorStyles.None;

            // Force the editor to be visible and on top
            System.Diagnostics.Debug.WriteLine($"BeginEdit: Setting editor visible. Current Visible: {_editorControl.Visible}, Enabled: {_editorControl.Enabled}");
            _editorControl.Visible = true;
            _editorControl.Show();
            _editorControl.BringToFront();
            System.Diagnostics.Debug.WriteLine($"BeginEdit: After Show/BringToFront. Visible: {_editorControl.Visible}, Handle created: {_editorControl.IsHandleCreated}");

            // Keep in sync with grid visuals
            _grid.Paint -= OnGridPaintReposition;
            _grid.Paint += OnGridPaintReposition;
            _grid.Resize -= OnGridMovedOrSized;
            _grid.Resize += OnGridMovedOrSized;
            _grid.MouseWheel -= OnGridMouseWheel;
            _grid.MouseWheel += OnGridMouseWheel;

            // CRITICAL: Suppress LostFocus during initial setup
            _suppressLostFocus = true;

            // Attach event handlers AFTER setting suppress flag
            _editorControl.KeyDown += OnEditorKeyDown;
            _editorControl.LostFocus += OnEditorLostFocus;

            // Use BeginInvoke to ensure proper focus timing
            _grid.BeginInvoke(new Action(() =>
            {
                System.Diagnostics.Debug.WriteLine($"BeginEdit: BeginInvoke callback executing. _isEndingEdit={_isEndingEdit}, _editorControl!=null={_editorControl != null}");
                if (!_isEndingEdit && _editorControl != null && !_editorControl.IsDisposed)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"BeginEdit: Setting focus. Editor type: {_editorControl.GetType().Name}");
                        _grid.ActiveControl = _editorControl;
                        _editorControl.Focus();
                        System.Diagnostics.Debug.WriteLine($"BeginEdit: Focus set. Focused: {_editorControl.Focused}, ContainsFocus: {_editorControl.ContainsFocus}");

                        // Notify editor that editing has begun (e.g., open popups)
                        _currentEditor?.OnBeginEdit(_editorControl);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"BeginEdit: Focus exception: {ex.Message}");
                    }

                    // IMPORTANT: Only disable suppress AFTER focus is established
                    _suppressLostFocus = false;
                }
            }));

            _grid.SafeInvalidate();
        }

        private void CleanupPreviousEditor()
        {
            if (_editorControl != null)
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: Cleaning up previous editor");
                try
                {
                    _editorControl.KeyDown -= OnEditorKeyDown;
                    _editorControl.LostFocus -= OnEditorLostFocus;
                    _currentEditor?.DetachEvents(_editorControl, _editorEvents);

                    _editorControl.Parent?.Controls.Remove(_editorControl);
                    _editorControl.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"BeginEdit: Error cleaning up previous editor: {ex.Message}");
                }
                finally
                {
                    _editorControl = null;
                    _currentEditor = null;
                }
            }
        }

        public void EndEdit(bool commit)
        {
            if (_editingCell == null || _editorControl == null || _isEndingEdit) return;

            _isEndingEdit = true;

            try
            {
                if (commit && _currentEditor != null && !_editorControl.IsDisposed)
                {
                    var rawValue = _currentEditor.GetValue(_editorControl);
                    var valueToAssign = NormalizeEditorValue(rawValue, _grid.Data.Columns[_grid.Selection.ColumnIndex]);
                    CommitValueToData(valueToAssign);
                }

                // Detach all event handlers
                _editorControl.KeyDown -= OnEditorKeyDown;
                _editorControl.LostFocus -= OnEditorLostFocus;
                _currentEditor?.DetachEvents(_editorControl, _editorEvents);

                // Detach grid event handlers
                _grid.Paint -= OnGridPaintReposition;
                _grid.Resize -= OnGridMovedOrSized;
                _grid.MouseWheel -= OnGridMouseWheel;

                // Remove editor from whichever parent currently owns it
                _editorControl.Parent?.Controls.Remove(_editorControl);
                _editorControl.Visible = false;

                // Dispose editor asynchronously to prevent blocking
                var editorToDispose = _editorControl;
                var editorInstance = _currentEditor;
                _grid.BeginInvoke(new Action(() =>
                {
                    try { editorToDispose?.Dispose(); }
                    catch { }
                }));
            }
            catch { }
            finally
            {
                _editorControl = null;
                _currentEditor = null;
                _editingCell = null;
                _isEndingEdit = false;
                _grid.SafeInvalidate();
            }
        }

        private void OnEditorLostFocus(object sender, EventArgs e)
        {
            if (_suppressLostFocus || _isEndingEdit) return;

            // Delegate popup-open check to the current editor strategy
            if (_currentEditor?.IsPopupOpen(_editorControl) == true)
                return;

            EndEdit(true);
        }

        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (_isEndingEdit) return;

            if (e.KeyCode == Keys.Enter)
            {
                EndEdit(true);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                EndEdit(false);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Tab)
            {
                EndEdit(true);
                _grid.Input.HandleKeyDown(new KeyEventArgs(Keys.Right));
                _grid.Edit.BeginEdit();
                e.Handled = true;
            }
        }

        private void OnGridPaintReposition(object sender, PaintEventArgs e)
        {
            RepositionEditor();
        }

        private void OnGridMovedOrSized(object sender, EventArgs e)
        {
            RepositionEditor();
        }

        private void OnGridMouseWheel(object sender, MouseEventArgs e)
        {
            RepositionEditor();
        }

        private void RepositionEditor()
        {
            if (_editingCell != null && _editorControl != null && !_editorControl.IsDisposed)
            {
                try
                {
                    var rect = _editingCell.Rect;

                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        int padding = 2;
                        var editorRect = new Rectangle(
                            rect.X + padding,
                            rect.Y + padding,
                            rect.Width - (padding * 2),
                            rect.Height - (padding * 2)
                        );

                        if (editorRect.Width < 10) editorRect.Width = Math.Max(10, rect.Width - 4);
                        if (editorRect.Height < 10) editorRect.Height = Math.Max(10, rect.Height - 4);

                        if (_editorControl.Bounds != editorRect)
                        {
                            if (_editorControl.Parent != _grid)
                            {
                                _editorControl.Parent?.Controls.Remove(_editorControl);
                                _grid.Controls.Add(_editorControl);
                            }
                            _editorControl.Bounds = editorRect;
                        }
                    }
                    else
                    {
                        EndEdit(true);
                    }
                }
                catch { }
            }
        }

        private object NormalizeEditorValue(object rawValue, Models.BeepColumnConfig col)
        {
            var rowIndex = _grid.Selection.RowIndex;
            var dataItem = _grid.Data.Rows[rowIndex].RowData;
            Type targetType = null;

            if (dataItem is DataRowView drv)
            {
                targetType = drv.Row.Table.Columns[col.ColumnName].DataType;
            }
            else if (dataItem != null && !string.IsNullOrEmpty(col.ColumnName))
            {
                var pi = dataItem.GetType().GetProperty(col.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                targetType = pi?.PropertyType;
            }

            if (rawValue is Models.SimpleItem si)
            {
                if (targetType == null)
                {
                    return si.Item ?? si.Value ?? si.Text;
                }
                if (targetType == typeof(string))
                    return si.Text ?? si.Value?.ToString() ?? si.Item?.ToString();
                if (targetType.IsEnum)
                {
                    try { return Enum.Parse(targetType, si.Text, true); } catch { }
                    if (si.Value != null)
                    {
                        try { return Enum.Parse(targetType, si.Value.ToString(), true); } catch { }
                    }
                    return Activator.CreateInstance(targetType);
                }
                if (IsNumericType(targetType))
                {
                    object candidate = si.Value ?? si.Text;
                    try { return System.Convert.ChangeType(candidate, Nullable.GetUnderlyingType(targetType) ?? targetType); } catch { }
                    return Activator.CreateInstance(Nullable.GetUnderlyingType(targetType) ?? targetType);
                }
                if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                {
                    if (DateTime.TryParse(si.Value?.ToString() ?? si.Text, out var dt)) return dt;
                    return default(DateTime);
                }
                return si.Item ?? si.Value ?? si.Text;
            }

            if (targetType != null)
            {
                return ConvertValue(rawValue, targetType);
            }
            return rawValue;
        }

        private bool IsNumericType(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort) ||
                   t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong) ||
                   t == typeof(float) || t == typeof(double) || t == typeof(decimal);
        }

        private void CommitValueToData(object newValue)
        {
            var rowIndex = _grid.Selection.RowIndex;
            var colIndex = _grid.Selection.ColumnIndex;
            var col = _grid.Data.Columns[colIndex];
            var rowCfg = _grid.Data.Rows[rowIndex];
            var dataItem = rowCfg.RowData;

            try
            {
                if (dataItem is DataRowView drv)
                {
                    drv[col.ColumnName] = newValue ?? DBNull.Value;
                }
                else if (dataItem != null && !string.IsNullOrEmpty(col.ColumnName))
                {
                    var pi = dataItem.GetType().GetProperty(col.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (pi != null && pi.CanWrite)
                    {
                        var converted = ConvertValue(newValue, pi.PropertyType);
                        pi.SetValue(dataItem, converted);
                    }
                }

                _editingCell.CellValue = newValue;
                _editingCell.IsDirty = true;
                _grid.OnCellValueChanged(_editingCell);
            }
            catch
            {
                _editingCell.CellValue = _originalValue;
            }
        }

        private object ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                var ut = Nullable.GetUnderlyingType(targetType) ?? targetType;
                return ut.IsValueType ? Activator.CreateInstance(ut) : null;
            }
            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
            try { return System.Convert.ChangeType(value, underlying); }
            catch { return value; }
        }

        /// <summary>
        /// Bridges editor lifecycle events back into the GridEditHelper.
        /// </summary>
        private sealed class EditorEvents : IGridEditorEvents
        {
            private readonly GridEditHelper _helper;
            public EditorEvents(GridEditHelper helper) => _helper = helper;
            public void RequestEndEdit(bool commit) => _helper.EndEdit(commit);
            public void RequestCancelEdit() => _helper.EndEdit(false);
        }
    }
}
