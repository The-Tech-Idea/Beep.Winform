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
        private bool _resizingHeaderRow;
        private bool _resizingDataRow;
        private int _resizingRowIndex = -1;
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
            // Don't process mouse move while context menu is active
            
            
            // Handle column reorder drag first (takes priority)
            if (_grid.AllowColumnReorder && _grid.ColumnReorder.HandleMouseMove(e.Location))
            {
                return; // Column reorder is handling the drag
            }

            // Handle column resize
            if (_resizingColumn && _resizingColIndex >= 0)
            {
                int dx = e.X - _mouseDown.X;
                var col = _grid.Data.Columns[_resizingColIndex];
                col.Width = Math.Max(20, col.Width + dx);
                _mouseDown = e.Location;
                _grid.Layout.Recalculate();
                _grid.ScrollBars?.UpdateBars();
                _grid.SafeInvalidate();
                _grid.Cursor = Cursors.SizeWE;
                return;
            }

            // Handle header row height resize
            if (_resizingHeaderRow)
            {
                int dy = e.Y - _mouseDown.Y;
                _grid.ColumnHeaderHeight = Math.Max(22, _grid.ColumnHeaderHeight + dy);
                _mouseDown = e.Location;
                _grid.Layout.Recalculate();
                _grid.SafeInvalidate();
                _grid.Cursor = Cursors.SizeNS;
                return;
            }

            // Handle data row height resize
            if (_resizingDataRow && _resizingRowIndex >= 0)
            {
                int dy = e.Y - _mouseDown.Y;
                var row = _grid.Data.Rows[_resizingRowIndex];
                int newHeight = Math.Max(18, (row.Height > 0 ? row.Height : _grid.RowHeight) + dy);
                row.Height = newHeight;
                _mouseDown = e.Location;
                _grid.Layout.Recalculate();
                _grid.ScrollBars?.UpdateBars();
                _grid.SafeInvalidate();
                _grid.Cursor = Cursors.SizeNS;
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
                    _grid.SafeInvalidate();
                }
            }
            else if (_grid.Layout.HoveredHeaderColumnIndex != -1)
            {
                _grid.Layout.HoveredHeaderColumnIndex = -1;
                _grid.SafeInvalidate();
            }

            // Determine and set cursor based on possible action at this location
            UpdateCursorForLocation(e.Location);
        }

        public void HandleMouseDown(MouseEventArgs e)
        {
            // Don't process mouse down while context menu is active
            
            
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
                    // Invalidate only rows that changed state
                    for (int i = 0; i < _grid.Data.Rows.Count; i++)
                    {
                        bool wasSelected = _grid.Data.Rows[i].IsSelected;
                        _grid.Data.Rows[i].IsSelected = _selectAllChecked;
                        // keep selection column cell in sync if present
                        SyncSelectionCellWithRow(i, _selectAllChecked);
                        
                        if (wasSelected != _selectAllChecked)
                        {
                            _grid.InvalidateRow(i);
                        }
                    }
                    _grid.OnRowSelectionChanged(-1);
                }
                else
                {
                    // Invalidate all rows then select first if needed
                    for (int i = 0; i < _grid.Data.Rows.Count; i++)
                    {
                        bool wasSelected = _grid.Data.Rows[i].IsSelected;
                        _grid.Data.Rows[i].IsSelected = false;
                        SyncSelectionCellWithRow(i, false);
                        
                        if (wasSelected)
                        {
                            _grid.InvalidateRow(i);
                        }
                    }
                    if (_selectAllChecked && _grid.Data.Rows.Count > 0)
                    {
                        _grid.Data.Rows[0].IsSelected = true;
                        SyncSelectionCellWithRow(0, true);
                        _grid.InvalidateRow(0);
                        _grid.OnRowSelectionChanged(0);
                    }
                    else
                    {
                        _grid.OnRowSelectionChanged(-1);
                    }
                }

                // Invalidate the header area for the checkbox itself
                _grid.SafeInvalidate(_grid.Layout.SelectAllCheckRect);
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

            // Column reorder drag - prepare for potential drag (before general header click)
            if (_grid.AllowColumnReorder && _grid.Layout.ShowColumnHeaders && _grid.Layout.HeaderRect.Contains(e.Location))
            {
                int colIdx = _grid.Layout.HoveredHeaderColumnIndex;
                if (colIdx >= 0)
                {
                    // Let reorder helper track the potential drag start
                    _grid.ColumnReorder.HandleMouseDown(e.Location, colIdx);
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

            // Resize columns - allow starting from header or rows area, like BeepSimpleGrid
            int borderIdx = HitTestColumnBorder(e.Location);
            if (borderIdx >= 0 && _grid.AllowUserToResizeColumns)
            {
                _resizingColumn = true;
                _resizingColIndex = borderIdx;
                _grid.Cursor = Cursors.SizeWE;
                return;
            }

            // Resize header row height
            if (_grid.Layout.ShowColumnHeaders && HitTestHeaderRowBorder(e.Location))
            {
                _resizingHeaderRow = true;
                _grid.Cursor = Cursors.SizeNS;
                return;
            }

            // Resize data row height
            int rowBorderIdx = HitTestRowBorder(e.Location);
            if (rowBorderIdx >= 0)
            {
                _resizingDataRow = true;
                _resizingRowIndex = rowBorderIdx;
                _grid.Cursor = Cursors.SizeNS;
                return;
            }

            // Highlight active cell; do not toggle selection here
            var (rrow, rcol) = HitTestCell(e.Location);
            if (rrow >= 0 && rcol >= 0)
            {
                _grid.Selection.SelectCell(rrow, rcol);
                EnsureSelectionVisible();
                // SelectCell already invalidates affected rows, no need for full grid invalidate
            }
        }

        public void HandleMouseUp(MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"HandleMouseUp at location: {e.Location}");
            // Don't process mouse up while context menu is active
            
            
            // Handle column reorder completion first
            if (_grid.AllowColumnReorder && _grid.ColumnReorder.HandleMouseUp(e.Location))
            {
                System.Diagnostics.Debug.WriteLine("Column reorder handled mouse up");
                return; // Column was being dragged, reorder handled it
            }

            // Reset column resizing
            if (_resizingColumn)
            {
                System.Diagnostics.Debug.WriteLine("Ending column resize");
                _resizingColumn = false;
                _resizingColIndex = -1;
                _grid.Cursor = Cursors.Default;
                return;
            }

            // Reset header row resizing
            if (_resizingHeaderRow)
            {
                System.Diagnostics.Debug.WriteLine("Ending header row resize");
                _resizingHeaderRow = false;
                _grid.Cursor = Cursors.Default;
                return;
            }

            // Reset data row resizing
            if (_resizingDataRow)
            {
                System.Diagnostics.Debug.WriteLine("Ending data row resize");
                _resizingDataRow = false;
                _resizingRowIndex = -1;
                _grid.Cursor = Cursors.Default;
                return;
            }

            var (r, c) = HitTestCell(e.Location);
            System.Diagnostics.Debug.WriteLine($"HitTestCell returned: Row={r}, Col={c}");

            // If mouse-up is on the same checkbox we pressed, toggle now
            if (_pressedOnCheckbox)
            {
                System.Diagnostics.Debug.WriteLine("Pressed on checkbox, handling checkbox toggle");
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
            System.Diagnostics.Debug.WriteLine($"Normal cell click. ReadOnly={_grid.ReadOnly}, r={r}, c={c}");
            if (r >= 0 && c >= 0 && !_grid.ReadOnly)
            {
                var cell = _grid.Data.Rows[r].Cells[c];
                var col = _grid.Data.Columns[c];
                bool isCheckCol = col.IsSelectionCheckBox;
                System.Diagnostics.Debug.WriteLine($"Cell found. IsCheckboxCol={isCheckCol}, IsReadOnly={cell.IsReadOnly}, IsEditable={cell.IsEditable}");

                // Select the cell first
                _grid.Selection.SelectCell(r, c);
                EnsureSelectionVisible();
                // SelectCell already invalidates affected rows, no need for full grid invalidate

                // Handle checkbox clicks (should be handled above through press tracking). Guard here for safety.
                if (isCheckCol)
                {
                    System.Diagnostics.Debug.WriteLine("IsCheckboxColumn, toggling row selection");
                    ToggleRowSelection(r);
                    return;
                }

                // For other editable cells, show dialog editor
                if (!cell.IsReadOnly && cell.IsEditable)
                {
                    System.Diagnostics.Debug.WriteLine("Calling BeginEdit...");
                    _grid.Edit.BeginEdit();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Cell is readonly or not editable, skipping BeginEdit");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Not calling BeginEdit: r={r}, c={c}, ReadOnly={_grid.ReadOnly}");
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
                // Only invalidate the toggled row in multi-select mode
                row.IsSelected = newState;
                SyncSelectionCellWithRow(rowIndex, newState);
                _grid.InvalidateRow(rowIndex);
            }
            else
            {
                newState = !current;
                // Invalidate all previously selected rows and the new one
                for (int k = 0; k < _grid.Data.Rows.Count; k++)
                {
                    bool wasSelected = _grid.Data.Rows[k].IsSelected;
                    _grid.Data.Rows[k].IsSelected = false;
                    SyncSelectionCellWithRow(k, false);
                    
                    // Only invalidate rows that changed
                    if (wasSelected)
                    {
                        _grid.InvalidateRow(k);
                    }
                }
                
                row.IsSelected = newState;
                SyncSelectionCellWithRow(rowIndex, newState);
                _grid.InvalidateRow(rowIndex);
            }

            _grid.OnRowSelectionChanged(rowIndex);
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
            // Don't process keyboard input while context menu is active
            
            
            // Handle Ctrl+C (Copy)
            if (e.Control && e.KeyCode == Keys.C)
            {
                _grid.Clipboard.CopyToClipboard();
                e.Handled = true;
                return;
            }

            // Handle Ctrl+X (Cut)
            if (e.Control && e.KeyCode == Keys.X && !_grid.ReadOnly)
            {
                _grid.Clipboard.CutToClipboard();
                e.Handled = true;
                return;
            }

            // Handle Ctrl+V (Paste)
            if (e.Control && e.KeyCode == Keys.V && !_grid.ReadOnly)
            {
                _grid.Clipboard.PasteFromClipboard();
                e.Handled = true;
                return;
            }

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
                        _grid.Edit.BeginEdit();
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
                    // SelectCell already invalidates affected rows, no need for full grid invalidate
                }
                return;
            }

            int r = _grid.Selection.RowIndex;
            int c = _grid.Selection.ColumnIndex;
            int visible = VisibleRowCount();
            switch (e.KeyCode)
            {
                case Keys.Left:
                    c = Math.Max(0, c - 1);
                    break;
                case Keys.Right:
                    c = Math.Min(_grid.Data.Columns.Count - 1, c + 1);
                    break;
                case Keys.Up:
                    r = Math.Max(0, r - 1);
                    break;
                case Keys.Down:
                    r = Math.Min(_grid.Data.Rows.Count - 1, r + 1);
                    break;
                case Keys.Home:
                    if (e.Control)
                    {
                        r = 0;
                        _grid.Scroll.SetVerticalIndex(0);
                    }
                    else
                    {
                        c = 0;
                    }
                    break;
                case Keys.End:
                    if (e.Control)
                    {
                        r = Math.Max(0, _grid.Data.Rows.Count - 1);
                        int newStartEnd = Math.Max(0, r - visible + 1);
                        _grid.Scroll.SetVerticalIndex(newStartEnd);
                    }
                    else
                    {
                        c = _grid.Data.Columns.Count - 1;
                    }
                    break;
                case Keys.PageUp:
                    r = Math.Max(0, r - visible);
                    _grid.Scroll.SetVerticalIndex(Math.Max(0, _grid.Scroll.FirstVisibleRowIndex - visible));
                    break;
                case Keys.PageDown:
                    r = Math.Min(_grid.Data.Rows.Count - 1, r + visible);
                    int maxStart = Math.Max(0, _grid.Data.Rows.Count - visible);
                    int newStart = Math.Min(maxStart, _grid.Scroll.FirstVisibleRowIndex + visible);
                    _grid.Scroll.SetVerticalIndex(newStart);
                    break;
                default:
                    return;
            }

            _grid.Selection.SelectCell(r, c);
            EnsureSelectionVisible();
            // SelectCell already invalidates affected rows, no need for full grid invalidate
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
            }
            else if (r >= start + visible)
            {
                _grid.Scroll.SetVerticalIndex(Math.Max(0, r - visible + 1));
            }
        }

        private int VisibleRowCount() => Math.Max(1, _grid.Layout.RowsRect.Height / _grid.RowHeight);

        private (int row, int col) HitTestCell(Point p)
        {
            // Only check visible rows that are actually rendered on screen
            // This prevents selecting wrong rows when scrolled
            var rowsRect = _grid.Layout.RowsRect;
            if (!rowsRect.Contains(p))
                return (-1, -1);

            // Calculate visible row range based on scroll position
            int firstVisibleRowIndex = _grid.Scroll.FirstVisibleRowIndex;
            int lastVisibleRowIndex = Math.Min(_grid.Data.Rows.Count - 1, 
                firstVisibleRowIndex + VisibleRowCount() + 1); // +1 for partial rows

            // Only check cells in the visible range
            for (int r = firstVisibleRowIndex; r <= lastVisibleRowIndex && r < _grid.Data.Rows.Count; r++)
            {
                var row = _grid.Data.Rows[r];
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    if (row.Cells[c].Rect.Contains(p)) 
                        return (r, c);
                }
            }
            return (-1, -1);
        }

        private int HitTestColumnBorder(Point p)
        {
            // Prefer using the already calculated header cell rects which account for sticky columns and horizontal scroll
            // Check header first
            if (_grid.Layout.ShowColumnHeaders && _grid.Layout.HeaderRect.Contains(p) && _grid.Layout.HeaderCellRects.Length == _grid.Data.Columns.Count)
            {
                for (int i = 0; i < _grid.Layout.HeaderCellRects.Length; i++)
                {
                    var r = _grid.Layout.HeaderCellRects[i];
                    if (r.IsEmpty || !_grid.Data.Columns[i].Visible) continue;
                    // Near the right edge of the header cell
                    if (Math.Abs(p.X - r.Right) <= _resizeMargin)
                        return i;
                }
            }

            // Also allow resizing from the rows area like BeepSimpleGrid
            if (_grid.Layout.RowsRect.Contains(p) && _grid.Layout.HeaderCellRects.Length == _grid.Data.Columns.Count)
            {
                for (int i = 0; i < _grid.Layout.HeaderCellRects.Length; i++)
                {
                    var r = _grid.Layout.HeaderCellRects[i];
                    if (r.IsEmpty || !_grid.Data.Columns[i].Visible) continue;
                    if (Math.Abs(p.X - r.Right) <= _resizeMargin)
                        return i;
                }
            }

            return -1;
        }

        private bool HitTestHeaderRowBorder(Point p)
        {
            // Check if we're near the bottom edge of the header row
            if (!_grid.Layout.ShowColumnHeaders) return false;
            
            var headerRect = _grid.Layout.HeaderRect;
            if (headerRect.IsEmpty) return false;

            // Check if point is within the header horizontally and near the bottom edge
            if (p.X >= headerRect.Left && p.X <= headerRect.Right)
            {
                if (Math.Abs(p.Y - headerRect.Bottom) <= _resizeMargin)
                {
                    return true;
                }
            }

            return false;
        }

        private int HitTestRowBorder(Point p)
        {
            // Check if we're near the bottom edge of any data row
            var rowsRect = _grid.Layout.RowsRect;
            if (!rowsRect.Contains(p)) return -1;

            int firstVisibleRowIndex = _grid.Scroll.FirstVisibleRowIndex;
            int currentY = rowsRect.Top;
            
            // Calculate pixel offset for positioning
            int totalRowsHeight = 0;
            for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            
            int pixelOffset = _grid.Scroll.VerticalOffset;
            currentY = rowsRect.Top - (pixelOffset - totalRowsHeight);

            // Check each visible row
            for (int i = firstVisibleRowIndex; i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                // Check if we're near the bottom edge of this row
                if (Math.Abs(p.Y - (currentY + rowHeight)) <= _resizeMargin)
                {
                    return i;
                }
                
                currentY += rowHeight;
                
                // Stop if we're past the visible area
                if (currentY > rowsRect.Bottom) break;
            }

            return -1;
        }

        private bool HitTestNavigator(Point p)
        {
            var navRect = _grid.Layout.GetType().GetProperty("NavigatorRect")?.GetValue(_grid.Layout) as Rectangle? ?? Rectangle.Empty;
            if (navRect == Rectangle.Empty || !navRect.Contains(p)) return false;

            // Use the centralized hit test system - DO NOT invoke HitAction here
            // The BaseControl's hit test system will handle the click action automatically
            if (_grid.HitTest(p))
            {
                return true;
            }

            return false;
        }

        private void UpdateCursorForLocation(Point p)
        {
            // 1) Column border -> resize cursor (horizontal)
            int borderIdx = HitTestColumnBorder(p);
            if (borderIdx >= 0 && _grid.AllowUserToResizeColumns)
            {
                _grid.Cursor = Cursors.SizeWE;
                return;
            }

            // 2) Header row border -> resize cursor (vertical)
            if (_grid.Layout.ShowColumnHeaders && HitTestHeaderRowBorder(p))
            {
                _grid.Cursor = Cursors.SizeNS;
                return;
            }

            // 3) Data row border -> resize cursor (vertical)
            int rowBorderIdx = HitTestRowBorder(p);
            if (rowBorderIdx >= 0)
            {
                _grid.Cursor = Cursors.SizeNS;
                return;
            }

            // 4) Navigator buttons -> hand cursor
            if (_grid.ShowNavigator)
            {
                var navRect = _grid.Layout.GetType().GetProperty("NavigatorRect")?.GetValue(_grid.Layout) as Rectangle? ?? Rectangle.Empty;
                if (navRect != Rectangle.Empty && navRect.Contains(p))
                {
                    // Light-weight check: if any hit area exists here, treat as clickable
                    if (_grid.HitTest(p) && _grid.HitTestControl?.HitAction != null)
                    {
                        _grid.Cursor = Cursors.Hand;
                        return;
                    }
                }
            }

            // 5) Header icons (filter/sort) and sortable header -> hand cursor
            if (_grid.Layout.ShowColumnHeaders && _grid.Layout.HeaderRect.Contains(p))
            {
                int colIdx = _grid.Layout.HoveredHeaderColumnIndex;
                if (colIdx >= 0)
                {
                    if (_grid.Render.HeaderFilterIconRects.TryGetValue(colIdx, out var fr) && fr.Contains(p))
                    {
                        _grid.Cursor = Cursors.Hand;
                        return;
                    }
                    if (_grid.Render.HeaderSortIconRects.TryGetValue(colIdx, out var sr) && sr.Contains(p))
                    {
                        _grid.Cursor = Cursors.Hand;
                        return;
                    }
                    var column = _grid.Data.Columns[colIdx];
                    if (column != null && column.AllowSort)
                    {
                        _grid.Cursor = Cursors.Hand;
                        return;
                    }
                }
            }

            // 6) Row/selection checkbox -> hand cursor
            if (_grid.ShowCheckBox)
            {
                int checkCol = _grid.Data.Columns.ToList().FindIndex(c => c.IsSelectionCheckBox && c.Visible);
                var (rr, cc) = HitTestCell(p);
                if (rr >= 0 && rr < _grid.Data.Rows.Count)
                {
                    var row = _grid.Data.Rows[rr];
                    if (!row.RowCheckRect.IsEmpty && row.RowCheckRect.Contains(p))
                    {
                        _grid.Cursor = Cursors.Hand;
                        return;
                    }
                    if (cc == checkCol && checkCol >= 0)
                    {
                        _grid.Cursor = Cursors.Hand;
                        return;
                    }
                }
            }

            // 5) Editable cells -> IBeam cursor (text/edit intent), except selection checkbox column
            {
                var (rr, cc) = HitTestCell(p);
                if (rr >= 0 && cc >= 0)
                {
                    var col = _grid.Data.Columns[cc];
                    var cell = _grid.Data.Rows[rr].Cells[cc];
                    if (!_grid.ReadOnly && !cell.IsReadOnly && cell.IsEditable && !col.IsSelectionCheckBox)
                    {
                        _grid.Cursor = Cursors.IBeam;
                        return;
                    }
                }
            }

            // Default
            _grid.Cursor = Cursors.Default;
        }
    }
}
