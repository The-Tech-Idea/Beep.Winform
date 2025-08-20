using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using static TheTechIdea.Beep.Winform.Controls.BeepControl;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridEditHelper
    {
        private readonly BeepGridPro _grid;
        private BeepControl _editorControl;
        private BeepCellConfig _editingCell;
        private object _originalValue;
        // Separate editor cache per column (do not reuse drawing cached controls)
        private readonly System.Collections.Generic.Dictionary<string, BeepControl> _editorCache = new();

        public GridEditHelper(BeepGridPro grid) { _grid = grid; }

        public void BeginEdit()
        {
            if (_grid.ReadOnly) return;
            if (!_grid.Selection.HasSelection) return;

            int r = _grid.Selection.RowIndex;
            int c = _grid.Selection.ColumnIndex;
            if (r < 0 || c < 0 || r >= _grid.Data.Rows.Count || c >= _grid.Data.Columns.Count) return;

            var cell = _grid.Data.Rows[r].Cells[c];
            var col = _grid.Data.Columns[c];
            if (cell.IsReadOnly || !cell.IsEditable || !col.Visible) return;

            // Create/Get dedicated editor for this column
            var key = col.ColumnName ?? col.ColumnCaption ?? col.GuidID;
            if (!_editorCache.TryGetValue(key, out var editor) || editor == null)
            {
                editor = CreateEditorForColumn(col);
                _editorCache[key] = editor;
            }

            _editorControl = editor;
            _editingCell = cell;
            _originalValue = cell.CellValue;

            // Prepare editor control bounds and theme
            _editorControl.Theme = _grid.Theme;
            _editorControl.Bounds = cell.Rect;

            // Push current value
            if (_editorControl is IBeepUIComponent ic)
                ic.SetValue(cell.CellValue);

            // Handlers
            _editorControl.KeyDown += OnEditorKeyDown;
            _editorControl.LostFocus += OnEditorLostFocus;

            // Register external drawing overlay
            _grid.AddChildExternalDrawing(_editorControl, (g, rect) =>
            {
                _editorControl.Draw(g, _editingCell.Rect);
            }, DrawingLayer.AfterAll);

            _grid.Invalidate();
        }

        private BeepControl CreateEditorForColumn(BeepColumnConfig col)
        {
            // Create fresh instances for editing context only
            return col.CellEditor switch
            {
                BeepColumnType.CheckBoxBool => new BeepCheckBoxBool { IsChild = true, GridMode = true },
                BeepColumnType.CheckBoxChar => new BeepCheckBoxChar { IsChild = true, GridMode = true },
                BeepColumnType.CheckBoxString => new BeepCheckBoxString { IsChild = true, GridMode = true },
                BeepColumnType.ComboBox => new BeepComboBox { IsChild = true, GridMode = true },
                BeepColumnType.DateTime => new BeepDatePicker { IsChild = true, GridMode = true },
                BeepColumnType.Image => new BeepImage { IsChild = true, GridMode = true },
                BeepColumnType.Button => new BeepButton { IsChild = true, GridMode = true },
                BeepColumnType.ProgressBar => new BeepProgressBar { IsChild = true, GridMode = true },
                BeepColumnType.NumericUpDown => new BeepNumericUpDown { IsChild = true, GridMode = true },
                BeepColumnType.Radio => new BeepRadioButton { IsChild = true, GridMode = true },
                BeepColumnType.ListBox => new BeepListBox { IsChild = true, GridMode = true },
                BeepColumnType.ListOfValue => new BeepListofValuesBox { IsChild = true, GridMode = true },
                _ => new BeepTextBox { IsChild = true, GridMode = true },
            };
        }

        private void OnEditorLostFocus(object sender, EventArgs e)
        {
            EndEdit(true);
        }

        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
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

        public void EndEdit(bool commit)
        {
            if (_editingCell == null || _editorControl == null) return;

            if (commit && _editorControl is IBeepUIComponent ic)
            {
                var newValue = ic.GetValue();
                CommitValueToData(newValue);
            }

            // Unregister overlay and cleanup
            _grid.ClearChildExternalDrawing(_editorControl);
            _editorControl.KeyDown -= OnEditorKeyDown;
            _editorControl.LostFocus -= OnEditorLostFocus;
            _editorControl = null;
            _editingCell = null;

            _grid.Invalidate();
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
                    drv[col.ColumnName] = newValue;
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
                // restore original if commit fails
                _editingCell.CellValue = _originalValue;
            }
        }

        private object ConvertValue(object value, Type targetType)
        {
            if (value == null) return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
            try { return System.Convert.ChangeType(value, underlying); }
            catch { return value; }
        }
    }
}
