using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridInputHelper
    {
        private readonly BeepGridPro _grid;
        private Point _mouseDown;
        private bool _resizingColumn;
        private int _resizingColIndex = -1;
        private int _resizeMargin = 3;

        // Cached nav button rects per paint (computed in render). We recompute on the fly here for simplicity
        private bool _selectAllChecked = false;

        // Track checkbox press to avoid double-toggle between MouseDown/MouseUp
        private bool _pressedOnCheckbox = false;
        private int _pressedRow = -1;
        private int _pressedCol = -1;

        public GridInputHelper(BeepGridPro grid) { _grid = grid; }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (_resizingColumn && _resizingColIndex >= 0)
            {
                int dx = e.X - _mouseDown.X;
                var col = _grid.Data.Columns[_resizingColIndex];
                col.Width = Math.Max(20, col.Width + dx);
                _mouseDown = e.Location;
                _grid.Layout.Recalculate();
                // Custom scrollbars are updated automatically through drawing
                _grid.Invalidate();
                return;
            }

            // Track hover over header to show filter icon
            if (_grid.Layout.ShowColumnHeaders && _grid.Layout.HeaderRect.Contains(e.Location))
            {
                int hoverIndex = -1;
                for (int i = 0; i < _grid.Layout.HeaderCellRects.Length; i++)
                {
                    var r = _grid.Layout.HeaderCellRects[i];
                    if (!r.IsEmpty && r.Contains(e.Location)) { hoverIndex = i; break; }
                }
                if (_grid.Layout.HoveredHeaderColumnIndex != hoverIndex)
                {
                    _grid.Layout.HoveredHeaderColumnIndex = hoverIndex;
                    _grid.Invalidate();
                }
            }
            else if (_grid.Layout.HoveredHeaderColumnIndex != -1)
            {
                _grid.Layout.HoveredHeaderColumnIndex = -1;
                _grid.Invalidate();
            }

            _grid.Cursor = _grid.Layout.HeaderRect.Contains(e.Location) && HitTestColumnBorder(e.Location) >= 0
                ? Cursors.VSplit
                : Cursors.Default;
        }

        public void HandleMouseDown(MouseEventArgs e)
        {
            _mouseDown = e.Location;
            _pressedOnCheckbox = false;
            _pressedRow = _pressedCol = -1;

            // Navigator
            if (_grid.ShowNavigator && HitTestNavigator(e.Location)) return;

            // Header select-all checkbox
            if (_grid.ShowCheckBox && _grid.Layout.SelectAllCheckRect.Contains(e.Location))
            {
                _selectAllChecked = !_selectAllChecked;

                if (_grid.MultiSelect)
                {
                    for (int i = 0; i < _grid.Data.Rows.Count; i++)
                    {
                        _grid.Data.Rows[i].IsSelected = _selectAllChecked;
                        // keep selection column cell in sync if present
                        SyncSelectionCellWithRow(i, _selectAllChecked);
                    }
                    _grid.OnRowSelectionChanged(-1);
                }
                else
                {
                    for (int i = 0; i < _grid.Data.Rows.Count; i++)
                    {
                        _grid.Data.Rows[i].IsSelected = false;
                        SyncSelectionCellWithRow(i, false);
                    }
                    if (_selectAllChecked && _grid.Data.Rows.Count > 0)
                    {
                        _grid.Data.Rows[0].IsSelected = true;
                        SyncSelectionCellWithRow(0, true);
                        _grid.OnRowSelectionChanged(0);
                    }
                    else
                    {
                        _grid.OnRowSelectionChanged(-1);
                    }
                }

                _grid.Invalidate();
                return;
            }

            // Filter icon click in header
            if (_grid.Layout.ShowColumnHeaders && _grid.Layout.HeaderRect.Contains(e.Location))
            {
                int colIdx = _grid.Layout.HoveredHeaderColumnIndex;
                if (colIdx >= 0 && _grid.Render.HeaderFilterIconRects.TryGetValue(colIdx, out var r) && r.Contains(e.Location))
                {
                    _grid.ShowFilterDialog();
                    return;
                }
            }

            // Sort icon click in header
            if (_grid.Layout.ShowColumnHeaders && _grid.Layout.HeaderRect.Contains(e.Location))
            {
                int colIdx = _grid.Layout.HoveredHeaderColumnIndex;
                if (colIdx >= 0 && _grid.Render.HeaderSortIconRects.TryGetValue(colIdx, out var r) && r.Contains(e.Location))
                {
                    _grid.ToggleColumnSort(colIdx);
                    return;
                }
            }

            // General column header click (for sorting)
            if (_grid.Layout.ShowColumnHeaders && _grid.Layout.HeaderRect.Contains(e.Location))
            {
                int colIdx = _grid.Layout.HoveredHeaderColumnIndex;
                if (colIdx >= 0)
                {
                    var column = _grid.Data.Columns[colIdx];
                    if (column != null && column.AllowSort)
                    {
                        _grid.ToggleColumnSort(colIdx);
                        return;
                    }
                }
            }

            // Row checkbox rectangle click tracking (do not toggle here; toggle on MouseUp)
            if (_grid.ShowCheckBox)
            {
                // Track row checkbox area press
                for (int i = 0; i < _grid.Data.Rows.Count; i++)
                {
                    var row = _grid.Data.Rows[i];
                    if (!row.RowCheckRect.IsEmpty && row.RowCheckRect.Contains(e.Location))
                    {
                        _pressedOnCheckbox = true;
                        _pressedRow = i;
                        _pressedCol = -1; // row area
                        break;
                    }
                }

                // Track selection checkbox column cell press
                if (!_pressedOnCheckbox)
                {
                    int checkCol = _grid.Data.Columns.ToList().FindIndex(c => c.IsSelectionCheckBox && c.Visible);
                    if (checkCol >= 0)
                    {
                        var (rr, cc) = HitTestCell(e.Location);
                        if (rr >= 0 && cc == checkCol)
                        {
                            _pressedOnCheckbox = true;
                            _pressedRow = rr;
                            _pressedCol = cc;
                        }
                    }
                }
            }

            // Resize columns in header
            if (_grid.Layout.ShowColumnHeaders && _grid.Layout.HeaderRect.Contains(e.Location))
            {
                var idx = HitTestColumnBorder(e.Location);
                if (idx >= 0 && _grid.AllowUserToResizeColumns)
                {
                    _resizingColumn = true;
                    _resizingColIndex = idx;
                    _grid.Cursor = Cursors.VSplit;
                    return;
                }
            }

            // Highlight active cell; do not toggle selection here
            var (rrow, rcol) = HitTestCell(e.Location);
            if (rrow >= 0 && rcol >= 0)
            {
                _grid.Selection.SelectCell(rrow, rcol);
                EnsureSelectionVisible();
                _grid.Invalidate();
            }
        }

        public void HandleMouseUp(MouseEventArgs e)
        {
            if (_resizingColumn)
            {
                _resizingColumn = false;
                _resizingColIndex = -1;
                _grid.Cursor = Cursors.Default;
                return;
            }

            var (r, c) = HitTestCell(e.Location);

            // If mouse-up is on the same checkbox we pressed, toggle now
            if (_pressedOnCheckbox)
            {
                bool sameTarget = false;
                if (_pressedCol == -1)
                {
                    // Row checkbox area: still within same row rect?
                    var row = (_pressedRow >= 0 && _pressedRow < _grid.Data.Rows.Count) ? _grid.Data.Rows[_pressedRow] : null;
                    sameTarget = row != null && row.RowCheckRect.Contains(e.Location);
                }
                else
                {
                    sameTarget = (r == _pressedRow && c == _pressedCol);
                }

                if (sameTarget)
                {
                    ToggleRowSelection(_pressedRow);
                }

                _pressedOnCheckbox = false;
                _pressedRow = _pressedCol = -1;
                return;
            }

            // Normal cell clicks (not on checkbox)
            if (r >= 0 && c >= 0 && !_grid.ReadOnly)
            {
                var cell = _grid.Data.Rows[r].Cells[c];
                var col = _grid.Data.Columns[c];
                bool isCheckCol = col.IsSelectionCheckBox;

                // Select the cell first
                _grid.Selection.SelectCell(r, c);
                EnsureSelectionVisible();
                _grid.Invalidate();

                // Handle checkbox clicks (should be handled above through press tracking). Guard here for safety.
                if (isCheckCol)
                {
                    ToggleRowSelection(r);
                    return;
                }

                // For other editable cells, show dialog editor
                if (!cell.IsReadOnly && cell.IsEditable)
                {
                    _grid.Dialog.ShowEditorDialog(cell);
                }
            }
        }

        private void ToggleRowSelection(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _grid.Data.Rows.Count) return;

            int checkCol = _grid.Data.Columns.ToList().FindIndex(c => c.IsSelectionCheckBox && c.Visible);
            var row = _grid.Data.Rows[rowIndex];
            bool current = false;

            if (checkCol >= 0)
            {
                var cell = row.Cells[checkCol];
                current = (bool)(cell.CellValue ?? false);
            }
            else
            {
                current = row.IsSelected;
            }

            bool newState;
            if (_grid.MultiSelect)
            {
                newState = !current;
            }
            else
            {
                newState = !current;
                for (int k = 0; k < _grid.Data.Rows.Count; k++)
                {
                    _grid.Data.Rows[k].IsSelected = false;
                    SyncSelectionCellWithRow(k, false);
                }
            }

            row.IsSelected = newState;
            SyncSelectionCellWithRow(rowIndex, newState);
            _grid.OnRowSelectionChanged(rowIndex);
            _grid.Invalidate();
        }

        private void SyncSelectionCellWithRow(int rowIndex, bool state)
        {
            int checkCol = _grid.Data.Columns.ToList().FindIndex(c => c.IsSelectionCheckBox && c.Visible);
            if (checkCol >= 0 && rowIndex >= 0 && rowIndex < _grid.Data.Rows.Count)
            {
                var cell = _grid.Data.Rows[rowIndex].Cells[checkCol];
                cell.CellValue = state; // drives checkbox render
                cell.IsSelected = state; // optional: align cell state
            }
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            // F2 or Enter starts editing immediately (like BeepSimpleGrid)
            if ((e.KeyCode == Keys.F2 || e.KeyCode == Keys.Enter) && !_grid.ReadOnly)
            {
                if (_grid.Selection.HasSelection)
                {
                    var cell = _grid.Data.Rows[_grid.Selection.RowIndex].Cells[_grid.Selection.ColumnIndex];
                    var col = _grid.Data.Columns[_grid.Selection.ColumnIndex];
                    
                    // Only start editing if the cell is editable and not a checkbox
                    if (!cell.IsReadOnly && cell.IsEditable && !col.IsSelectionCheckBox)
                    {
                        _grid.Dialog.ShowEditorDialog(cell);
                        e.Handled = true;
                        return;
                    }
                }
            }

            if (!_grid.Selection.HasSelection)
            {
                if (_grid.Data.Rows.Count > 0 && _grid.Data.Columns.Count > 0)
                {
                    _grid.Selection.SelectCell(0, 0);
                    EnsureSelectionVisible();
                    _grid.Invalidate();
                }
                return;
            }

            int r = _grid.Selection.RowIndex;
            int c = _grid.Selection.ColumnIndex;
            int visible = VisibleRowCount();
            switch (e.KeyCode)
            {
                case Keys.Left: c = Math.Max(0, c - 1); break;
                case Keys.Right: c = Math.Min(_grid.Data.Columns.Count - 1, c + 1); break;
                case Keys.Up: r = Math.Max(0, r - 1); break;
                case Keys.Down: r = Math.Min(_grid.Data.Rows.Count - 1, r + 1); break;
                case Keys.Home:
                    if (e.Control) { r = 0; _grid.Scroll.SetVerticalIndex(0); /* Custom scrollbars updated automatically */ }
                    else { c = 0; }
                    break;
                case Keys.End:
                    if (e.Control)
                    {
                        r = Math.Max(0, _grid.Data.Rows.Count - 1);
                        int newStartEnd = Math.Max(0, r - visible + 1);
                        _grid.Scroll.SetVerticalIndex(newStartEnd);
                        /* Custom scrollbars updated automatically */
                    }
                    else { c = _grid.Data.Columns.Count - 1; }
                    break;
                case Keys.PageUp:
                    r = Math.Max(0, r - visible);
                    _grid.Scroll.SetVerticalIndex(Math.Max(0, _grid.Scroll.FirstVisibleRowIndex - visible));
                    /* Custom scrollbars updated automatically */
                    break;
                case Keys.PageDown:
                    r = Math.Min(_grid.Data.Rows.Count - 1, r + visible);
                    int maxStart = Math.Max(0, _grid.Data.Rows.Count - visible);
                    int newStart = Math.Min(maxStart, _grid.Scroll.FirstVisibleRowIndex + visible);
                    _grid.Scroll.SetVerticalIndex(newStart);
                    /* Custom scrollbars updated automatically */
                    break;
                default:
                    return;
            }

            _grid.Selection.SelectCell(r, c);
            EnsureSelectionVisible();
            _grid.Invalidate();
        }

        private void EnsureSelectionVisible()
        {
            int visible = VisibleRowCount();
            int start = _grid.Scroll.FirstVisibleRowIndex;
            int r = _grid.Selection.RowIndex;
            if (r < 0) return;

            if (r < start)
            {
                _grid.Scroll.SetVerticalIndex(r);
                // Custom scrollbars are updated automatically through drawing
            }
            else if (r >= start + visible)
            {
                _grid.Scroll.SetVerticalIndex(Math.Max(0, r - visible + 1));
                // Custom scrollbars are updated automatically through drawing
            }
        }

        private int VisibleRowCount() => Math.Max(1, _grid.Layout.RowsRect.Height / _grid.RowHeight);

        private (int row, int col) HitTestCell(Point p)
        {
            for (int r = 0; r < _grid.Data.Rows.Count; r++)
            {
                var row = _grid.Data.Rows[r];
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    if (row.Cells[c].Rect.Contains(p)) return (r, c);
                }
            }
            return (-1, -1);
        }

        private int HitTestColumnBorder(Point p)
        {
            if (_grid.Data.Columns.Count == 0) return -1;
            int x = _grid.Layout.HeaderRect.Left;
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                x += _grid.Data.Columns[i].Width;
                if (Math.Abs(p.X - x) <= _resizeMargin) return i;
            }
            return -1;
        }

        private bool HitTestNavigator(Point p)
        {
            var navRect = _grid.Layout.GetType().GetProperty("NavigatorRect")?.GetValue(_grid.Layout) as Rectangle? ?? Rectangle.Empty;
            if (navRect == Rectangle.Empty || !navRect.Contains(p)) return false;

            // Use the centralized hit test system
            if (_grid.HitTest(p))
            {
                // Execute the hit action if one exists
                if (_grid.HitTestControl?.HitAction != null)
                {
                    _grid.HitTestControl.HitAction.Invoke();
                }
                return true;
            }

            return false;
        }
    }
}
