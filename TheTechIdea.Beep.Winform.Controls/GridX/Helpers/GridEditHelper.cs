using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using static TheTechIdea.Beep.Winform.Controls.BeepControl;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridEditHelper
    {
        private readonly BeepGridPro _grid;
        private Control _editorControl;
        private BeepCellConfig _editingCell;
        private object _originalValue;
        private bool _suppressLostFocus;
        private bool _isEndingEdit;
        private IBeepUIComponent _currenteditorUIcomponent;
        public GridEditHelper(BeepGridPro grid) { _grid = grid; }

        public void BeginEdit()
        {
            // Debug output
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
                _grid.Update();
                // Re-get the cell after layout recalculation
                cell = _grid.Data.Rows[r].Cells[c];
                System.Diagnostics.Debug.WriteLine($"BeginEdit: After recalc, Cell Rect={cell.Rect}");
                
                // If still invalid, cannot edit
                if (cell.Rect.Width <= 0 || cell.Rect.Height <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("BeginEdit: Cell Rect still invalid after recalculation");
                    return;
                }
            }
            
            System.Diagnostics.Debug.WriteLine("BeginEdit: Creating editor control...");
            cell = _grid.Data.Rows[r].Cells[c];
            

            // Remove previous editor from host - ALWAYS clear completely
            if (_editorControl != null)
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: Cleaning up previous editor");
                try
                {
                    _editorControl.KeyDown -= OnEditorKeyDown;
                    _editorControl.LostFocus -= OnEditorLostFocus;
                    if (_currenteditorUIcomponent is BeepComboBox oldCb)
                        oldCb.PopupClosed -= OnComboPopupClosed;
                    
                    // Remove from form, parent, or grid controls
                    var form = _grid.FindForm();
                    if (form != null && form.Controls.Contains(_editorControl))
                        form.Controls.Remove(_editorControl);
                    else if (_grid.Parent != null && _grid.Parent.Controls.Contains(_editorControl))
                        _grid.Parent.Controls.Remove(_editorControl);
                    else if (_grid.Controls.Contains(_editorControl))
                        _grid.Controls.Remove(_editorControl);
                    
                    _editorControl.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"BeginEdit: Error cleaning up previous editor: {ex.Message}");
                }
                finally { _editorControl = null; }
            }



            // Create editor
            System.Diagnostics.Debug.WriteLine($"BeginEdit: Calling CreateEditorForColumn for column type: {col.ColumnType}");
            _currenteditorUIcomponent = CreateEditorForColumn(col);
            System.Diagnostics.Debug.WriteLine($"BeginEdit: CreateEditorForColumn returned: {_currenteditorUIcomponent?.GetType().Name ?? "NULL"}");
            
            if (!(_currenteditorUIcomponent is Control editor))
            {
                System.Diagnostics.Debug.WriteLine("BeginEdit: Editor is not a Control, exiting");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"BeginEdit: Editor created successfully: {editor.GetType().Name}");
            _editorControl = editor as Control;
            _editingCell = cell;
            _originalValue = cell.CellValue;
            _isEndingEdit = false;

            // Configure editor with explicit visibility settings
            // Apply BaseControl-specific properties if editor is a BaseControl
            if (_editorControl is BaseControl baseEditor)
            {
                baseEditor.Theme = _grid.Theme;
                baseEditor.IsChild = true;
                baseEditor.ShowAllBorders = false;
            }
            
            _editorControl.Visible = true;
            _editorControl.TabStop = true;
            _editorControl.Enabled = true;

            // Set explicit background and foreground colors for ALL control types
            _editorControl.BackColor = Color.White;
            _editorControl.ForeColor = Color.Black;

            // Populate items and configure specific control types
            var itemsToUse = GetFilteredItems(col, cell);
            if (_currenteditorUIcomponent is BeepComboBox combo)
            {
                combo.ListItems.Clear();
                foreach (var item in itemsToUse) combo.ListItems.Add(item);
                combo.GridMode = false;
                combo.PopupClosed += OnComboPopupClosed;
                combo.BackColor = Color.White;
                combo.ForeColor = Color.Black;
                combo.BorderStyle = BorderStyle.FixedSingle; // Ensure visible border
            }
            else if (_currenteditorUIcomponent is BeepListBox listBox)
            {
                listBox.ListItems.Clear();
                foreach (var item in itemsToUse) listBox.ListItems.Add(item);
                listBox.GridMode = false;
                listBox.BackColor = Color.White;
                listBox.ForeColor = Color.Black;
                listBox.BorderStyle = BorderStyle.FixedSingle;
            }
           
            else if (_currenteditorUIcomponent is BeepTextBox st)
            {
               // st.ShowAllBorders = false;
               // st.IsFrameless = true;
                st.GridMode = true;
                st.BackColor = Color.White;
                st.ForeColor = Color.Black;
                st.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (_currenteditorUIcomponent is BeepCheckBoxBool checkBool)
            {
                checkBool.BackColor = Color.White;
                checkBool.ForeColor = Color.Black;
                checkBool.GridMode = true;
                checkBool.CheckBoxSize = Math.Min(cell.Rect.Width - 4, cell.Rect.Height - 4);
            }
            else if (_currenteditorUIcomponent is BeepCheckBoxChar checkChar)
            {
                checkChar.BackColor = Color.White;
                checkChar.ForeColor = Color.Black;
                checkChar.GridMode = true;
                checkChar.CheckBoxSize = Math.Min(cell.Rect.Width - 4, cell.Rect.Height - 4);
            }
            else if (_currenteditorUIcomponent is BeepCheckBoxString checkString)
            {
                checkString.BackColor = Color.White;
                checkString.ForeColor = Color.Black;
                checkString.GridMode = true;
                checkString.CheckBoxSize = Math.Min(cell.Rect.Width - 4, cell.Rect.Height - 4);
            }

            if (_editorControl is IBeepUIComponent ic)
                ic.SetValue(cell.CellValue);

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
            // Add small padding for visual alignment with cell borders
            int padding = 2; // Padding from cell borders
            var editorRect = new Rectangle(
                rect.X + padding,
                rect.Y + padding,
                rect.Width - (padding * 2),
                rect.Height - (padding * 2)
            );

            // Ensure minimum size for usability
            if (editorRect.Width < 10) editorRect.Width = Math.Max(10, rect.Width - 4);
            if (editorRect.Height < 10) editorRect.Height = Math.Max(10, rect.Height - 4);

            // Find the parent Form and add the editor there to avoid clipping issues
            var parentForm = _grid.FindForm();
            System.Diagnostics.Debug.WriteLine($"BeginEdit: ParentForm found: {parentForm != null}, Type: {parentForm?.GetType().Name}");
            
            if (parentForm != null)
            {
                // Convert grid-relative coordinates to form-relative coordinates
                var screenPoint = _grid.PointToScreen(editorRect.Location);
                var formPoint = parentForm.PointToClient(screenPoint);
                var formRect = new Rectangle(formPoint, editorRect.Size);
                
                System.Diagnostics.Debug.WriteLine($"BeginEdit: Grid rect: {editorRect}");
                System.Diagnostics.Debug.WriteLine($"BeginEdit: Screen point: {screenPoint}");
                System.Diagnostics.Debug.WriteLine($"BeginEdit: Form point: {formPoint}");
                System.Diagnostics.Debug.WriteLine($"BeginEdit: Form rect: {formRect}");
                
                // Remove from grid if it was added there
                if (_grid.Controls.Contains(_editorControl))
                {
                    System.Diagnostics.Debug.WriteLine("BeginEdit: Removing editor from grid");
                    _grid.Controls.Remove(_editorControl);
                }
                
                // Add to form
                if (!parentForm.Controls.Contains(_editorControl))
                {
                    System.Diagnostics.Debug.WriteLine("BeginEdit: Adding editor to form");
                    parentForm.Controls.Add(_editorControl);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("BeginEdit: Editor already in form controls");
                }
                
                _editorControl.Bounds = formRect;
                System.Diagnostics.Debug.WriteLine($"BeginEdit: Editor bounds set to: {_editorControl.Bounds}");
            }
            else
            {
                // Fallback: add to grid if no form found
                System.Diagnostics.Debug.WriteLine("BeginEdit: No parent form, adding to grid");
                if (!_grid.Controls.Contains(_editorControl))
                    _grid.Controls.Add(_editorControl);
                
                _editorControl.Bounds = editorRect;
            }
            
            _editorControl.Anchor = AnchorStyles.None; // No anchoring - we handle positioning manually
            
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
                        
                        // Handle ComboBox popup after focus is established
                        if (_currenteditorUIcomponent is BeepComboBox cb)
                        {
                           // try { cb.IsPopupOpen = true; } catch { }
                        }
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

        private void OnComboPopupClosed(object sender, EventArgs e)
        {
            // Don't end edit immediately when popup closes to prevent disposal race
            if (!_isEndingEdit && _currenteditorUIcomponent is BeepComboBox combo)
            {
                // Check if the grid still has a valid handle before attempting BeginInvoke
                if (_grid != null && !_grid.IsDisposed && _grid.IsHandleCreated)
                {
                    try
                    {
                        _grid.BeginInvoke(new Action(() =>
                        {
                            if (!_isEndingEdit && combo != null && !combo.IsDisposed)
                            {
                                EndEdit(true);
                            }
                        }));
                    }
                    catch (InvalidOperationException)
                    {
                        // Handle has been destroyed during the call - end edit synchronously
                        if (!_isEndingEdit && combo != null && !combo.IsDisposed)
                        {
                            EndEdit(true);
                        }
                    }
                }
                else
                {
                    // Grid is disposed or handle not created - end edit synchronously
                    if (!_isEndingEdit && combo != null && !combo.IsDisposed)
                    {
                        EndEdit(true);
                    }
                }
            }
        }

        private void OnGridPaintReposition(object sender, PaintEventArgs e)
        {
            if (_editingCell != null && _editorControl != null && !_editorControl.IsDisposed)
            {
                try
                {
                    var rect = _editingCell.Rect;
                    
                    // Check if cell is still visible (not scrolled out of view)
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        // Calculate editor bounds with padding (grid-relative)
                        int padding = 2;
                        var editorRect = new Rectangle(
                            rect.X + padding,
                            rect.Y + padding,
                            rect.Width - (padding * 2),
                            rect.Height - (padding * 2)
                        );
                        
                        // Ensure minimum size
                        if (editorRect.Width < 10) editorRect.Width = Math.Max(10, rect.Width - 4);
                        if (editorRect.Height < 10) editorRect.Height = Math.Max(10, rect.Height - 4);
                        
                        // Convert to form-relative if editor is on form
                        var parentForm = _grid.FindForm();
                        if (parentForm != null && parentForm.Controls.Contains(_editorControl))
                        {
                            var formLocation = parentForm.PointToClient(_grid.PointToScreen(editorRect.Location));
                            editorRect = new Rectangle(formLocation, editorRect.Size);
                        }
                        
                        // Only update if position changed to avoid flicker
                        if (_editorControl.Bounds != editorRect)
                        {
                            _editorControl.Bounds = editorRect;
                            _editorControl.BringToFront();
                        }
                    }
                    else
                    {
                        // Cell scrolled out of view - end editing
                        EndEdit(true);
                    }
                }
                catch { }
            }
        }

        private void OnGridMovedOrSized(object sender, EventArgs e)
        {
            if (_editingCell != null && _editorControl != null && !_editorControl.IsDisposed)
            {
                try
                {
                    var rect = _editingCell.Rect;
                    
                    // Check if cell is still visible
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        // Calculate editor bounds with padding (grid-relative)
                        int padding = 2;
                        var editorRect = new Rectangle(
                            rect.X + padding,
                            rect.Y + padding,
                            rect.Width - (padding * 2),
                            rect.Height - (padding * 2)
                        );
                        
                        // Ensure minimum size
                        if (editorRect.Width < 10) editorRect.Width = Math.Max(10, rect.Width - 4);
                        if (editorRect.Height < 10) editorRect.Height = Math.Max(10, rect.Height - 4);
                        
                        // Convert to form-relative if editor is on form
                        var parentForm = _grid.FindForm();
                        if (parentForm != null && parentForm.Controls.Contains(_editorControl))
                        {
                            var formLocation = parentForm.PointToClient(_grid.PointToScreen(editorRect.Location));
                            editorRect = new Rectangle(formLocation, editorRect.Size);
                        }
                        
                        _editorControl.Bounds = editorRect;
                        _editorControl.BringToFront();
                    }
                    else
                    {
                        // Cell scrolled out of view - end editing
                        EndEdit(true);
                    }
                }
                catch { }
            }
        }

        private void OnGridMouseWheel(object sender, MouseEventArgs e)
        {
            // Reposition editor when scrolling
            OnGridPaintReposition(sender, null);
        }

        public void EndEdit(bool commit)
        {
            if (_editingCell == null || _editorControl == null || _isEndingEdit) return;
            
            _isEndingEdit = true;

            try
            {
                if (commit && _editorControl is IBeepUIComponent ic && !_editorControl.IsDisposed)
                {
                    var rawValue = ic.GetValue();
                    var valueToAssign = NormalizeEditorValue(rawValue, _grid.Data.Columns[_grid.Selection.ColumnIndex]);
                    CommitValueToData(valueToAssign);
                }

                // Detach all event handlers
                _editorControl.KeyDown -= OnEditorKeyDown;
                _editorControl.LostFocus -= OnEditorLostFocus;
                if (_currenteditorUIcomponent is BeepComboBox combo)
                {
                    combo.PopupClosed -= OnComboPopupClosed;
                   // try { combo.IsPopupOpen = false; } catch { }
                }

                // Detach grid event handlers
                _grid.Paint -= OnGridPaintReposition;
                _grid.Resize -= OnGridMovedOrSized;
                _grid.MouseWheel -= OnGridMouseWheel;

                // Remove editor from form, parent, or grid controls
                var parentForm = _grid.FindForm();
                if (parentForm != null && parentForm.Controls.Contains(_editorControl))
                    parentForm.Controls.Remove(_editorControl);
                else if (_grid.Parent != null && _grid.Parent.Controls.Contains(_editorControl))
                    _grid.Parent.Controls.Remove(_editorControl);
                else if (_grid.Controls.Contains(_editorControl))
                    _grid.Controls.Remove(_editorControl);
                
                _editorControl.Visible = false;

                // Dispose editor asynchronously to prevent blocking
                var editorToDispose = _editorControl;
                _grid.BeginInvoke(new Action(() => { 
                    try { 
                        editorToDispose?.Dispose(); 
                    } catch { } 
                }));
            }
            catch { }
            finally
            {
                _editorControl = null;
                _editingCell = null;
                _isEndingEdit = false;
                _grid.SafeInvalidate();
            }
        }

        private System.Collections.Generic.List<SimpleItem> GetFilteredItems(BeepColumnConfig column, BeepCellConfig cell)
        {
            var baseItems = column?.Items ?? new System.Collections.Generic.List<SimpleItem>();
            if (baseItems == null || baseItems.Count == 0) return new System.Collections.Generic.List<SimpleItem>();

            if (!string.IsNullOrEmpty(column.ParentColumnName))
            {
                object parentValue = cell?.ParentCellValue;
                if (cell?.FilterdList != null && cell.FilterdList.Count > 0)
                {
                    return cell.FilterdList;
                }
                if (parentValue != null)
                {
                    return baseItems.Where(i => i.ParentValue?.ToString() == parentValue.ToString()).ToList();
                }
            }
            return baseItems.ToList();
        }

        private IBeepUIComponent CreateEditorForColumn(BeepColumnConfig col)
        {
            // Create fresh instances like BeepSimpleGrid.CreateCellControlForEditing
            IBeepUIComponent editor = col.CellEditor switch
            {
                BeepColumnType.Text => new BeepTextBox { IsChild = true,  GridMode = true },
                BeepColumnType.CheckBoxBool => new BeepCheckBoxBool { IsChild = true, GridMode = true },
                BeepColumnType.CheckBoxChar => new BeepCheckBoxChar { IsChild = true, GridMode = true },
                BeepColumnType.CheckBoxString => new BeepCheckBoxString { IsChild = true, GridMode = true },
                BeepColumnType.ComboBox => new BeepComboBox { IsChild = true, GridMode = false },
                BeepColumnType.DateTime => new BeepDatePicker { IsChild = true, GridMode = true },
                BeepColumnType.Image => new BeepImage { IsChild = true },
                BeepColumnType.Button => new BeepButton { IsChild = true },
                BeepColumnType.ProgressBar => new BeepProgressBar { IsChild = true },
                BeepColumnType.NumericUpDown => new BeepNumericUpDown { IsChild = true, GridMode = true },
                BeepColumnType.Radio => new BeepRadioGroup { IsChild = true, GridMode = true },
                BeepColumnType.ListBox => new BeepListBox { IsChild = true, GridMode = false },
                BeepColumnType.ListOfValue => new BeepListofValuesBox { IsChild = true, GridMode = false },
                _ => new BeepTextBox { IsChild = true, GridMode = true },
            };

            return editor;
        }

        private void OnEditorLostFocus(object sender, EventArgs e)
        {
            if (_suppressLostFocus || _isEndingEdit) return;
            
            // For ComboBox, don't end edit if popup is open
            if (sender is BeepComboBox combo )
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

        private object NormalizeEditorValue(object rawValue, BeepColumnConfig col)
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

            if (rawValue is SimpleItem si)
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
    }
}
