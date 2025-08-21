using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

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
        private Rectangle _firstRect, _prevRect, _nextRect, _lastRect, _insertRect, _deleteRect, _saveRect, _cancelRect, _queryRect, _filterRect, _printRect;
        private bool _selectAllChecked = false;

        public GridInputHelper(BeepGridPro grid) { _grid = grid; }

        public void HandleMouseDown(MouseEventArgs e)
        {
            _mouseDown = e.Location;

            // Navigator
            if (_grid.ShowNavigator && HitTestNavigator(e.Location)) return;

            // Header select-all checkbox
            if (_grid.ShowCheckBox && _grid.Layout.SelectAllCheckRect.Contains(e.Location))
            {
                _selectAllChecked = !_selectAllChecked;
                for (int i = 0; i < _grid.Data.Rows.Count; i++)
                    _grid.Data.Rows[i].IsSelected = _selectAllChecked;
                _grid.OnRowSelectionChanged(-1);
                _grid.Invalidate();
                return;
            }

            // Row checkbox or selection checkbox column cell
            if (_grid.ShowCheckBox)
            {
                for (int i = 0; i < _grid.Data.Rows.Count; i++)
                {
                    var row = _grid.Data.Rows[i];
                    if (!row.RowCheckRect.IsEmpty && row.RowCheckRect.Contains(e.Location))
                    {
                        row.IsSelected = !row.IsSelected;
                        _grid.OnRowSelectionChanged(i);
                        _grid.Invalidate();
                        return;
                    }
                }
                int checkCol = _grid.Data.Columns.ToList().FindIndex(c => c.IsSelectionCheckBox && c.Visible);
                if (checkCol >= 0)
                {
                    var (rr, cc) = HitTestCell(e.Location);
                    if (rr >= 0 && cc == checkCol)
                    {
                        var row = _grid.Data.Rows[rr];
                        row.IsSelected = !row.IsSelected;
                        _grid.OnRowSelectionChanged(rr);
                        _grid.Invalidate();
                        return;
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

            // Highlight active cell; do not toggle selection
            var (r, c) = HitTestCell(e.Location);
            if (r >= 0 && c >= 0)
            {
                _grid.Selection.SelectCell(r, c);
                EnsureSelectionVisible();
                _grid.Invalidate();
            }
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (_resizingColumn && _resizingColIndex >= 0)
            {
                int dx = e.X - _mouseDown.X;
                var col = _grid.Data.Columns[_resizingColIndex];
                col.Width = Math.Max(20, col.Width + dx);
                _mouseDown = e.Location;
                _grid.Layout.Recalculate();
                _grid.ScrollBars?.UpdateBars();
                _grid.Invalidate();
                return;
            }

            _grid.Cursor = _grid.Layout.HeaderRect.Contains(e.Location) && HitTestColumnBorder(e.Location) >= 0
                ? Cursors.VSplit
                : Cursors.Default;
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
            if (r >= 0 && c >= 0 && !_grid.ReadOnly)
            {
                var cell = _grid.Data.Rows[r].Cells[c];
                var col = _grid.Data.Columns[c];
                bool isCheckCol = col.IsSelectionCheckBox;

                // Select the cell first (this is the key - always select first)
                _grid.Selection.SelectCell(r, c);
                EnsureSelectionVisible();
                _grid.Invalidate();

                // Handle checkbox clicks immediately
                if (isCheckCol)
                {
                    cell.IsSelected = !cell.IsSelected;
                    _grid.Data.Rows[r].IsSelected = cell.IsSelected;
                    _grid.OnRowSelectionChanged(r);
                    return;
                }

                // For other editable cells, show dialog editor
                if (!cell.IsReadOnly && cell.IsEditable)
                {
                    _grid.Dialog.ShowEditorDialog(cell);
                }
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
                    if (e.Control) { r = 0; _grid.Scroll.SetVerticalIndex(0); _grid.ScrollBars?.SyncFromModel(); }
                    else { c = 0; }
                    break;
                case Keys.End:
                    if (e.Control)
                    {
                        r = Math.Max(0, _grid.Data.Rows.Count - 1);
                        int newStartEnd = Math.Max(0, r - visible + 1);
                        _grid.Scroll.SetVerticalIndex(newStartEnd);
                        _grid.ScrollBars?.SyncFromModel();
                    }
                    else { c = _grid.Data.Columns.Count - 1; }
                    break;
                case Keys.PageUp:
                    r = Math.Max(0, r - visible);
                    _grid.Scroll.SetVerticalIndex(Math.Max(0, _grid.Scroll.FirstVisibleRowIndex - visible));
                    _grid.ScrollBars?.SyncFromModel();
                    break;
                case Keys.PageDown:
                    r = Math.Min(_grid.Data.Rows.Count - 1, r + visible);
                    int maxStart = Math.Max(0, _grid.Data.Rows.Count - visible);
                    int newStart = Math.Min(maxStart, _grid.Scroll.FirstVisibleRowIndex + visible);
                    _grid.Scroll.SetVerticalIndex(newStart);
                    _grid.ScrollBars?.SyncFromModel();
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
                _grid.ScrollBars?.SyncFromModel();
            }
            else if (r >= start + visible)
            {
                _grid.Scroll.SetVerticalIndex(Math.Max(0, r - visible + 1));
                _grid.ScrollBars?.SyncFromModel();
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
            int buttonWidth = 24, buttonHeight = 24, padding = 6, spacing = 4;
            int y = navRect.Top + (navRect.Height - buttonHeight) / 2;
            int leftX = navRect.Left + padding;
            _insertRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            _deleteRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            _saveRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            _cancelRect = new Rectangle(leftX, y, buttonWidth, buttonHeight);

            string recordCounter = (_grid.Data.Rows.Count > 0 && _grid.Selection.RowIndex >= 0) ? $"{_grid.Selection.RowIndex + 1} - {_grid.Data.Rows.Count}" : "0 - 0";
            using var font = new Font(_grid.Font.FontFamily, 9f);
            SizeF textSize = TextRenderer.MeasureText(recordCounter, font);
            float centerX = navRect.Left + (navRect.Width - textSize.Width) / 2f;
            _firstRect = new Rectangle((int)centerX - (buttonWidth * 2) - padding * 2, y, buttonWidth, buttonHeight);
            _prevRect = new Rectangle((int)centerX - buttonWidth - padding, y, buttonWidth, buttonHeight);
            _nextRect = new Rectangle((int)(centerX + textSize.Width + padding), y, buttonWidth, buttonHeight);
            _lastRect = new Rectangle(_nextRect.Right + padding, y, buttonWidth, buttonHeight);

            int rightX = navRect.Right - padding - buttonWidth;
            _printRect = new Rectangle(rightX, y, buttonWidth, buttonHeight); rightX -= buttonWidth + spacing;
            _filterRect = new Rectangle(rightX, y, buttonWidth, buttonHeight); rightX -= buttonWidth + spacing;
            _queryRect = new Rectangle(rightX, y, buttonWidth, buttonHeight);

            if (_insertRect.Contains(p)) { _grid.InsertNew(); return true; }
            if (_deleteRect.Contains(p)) { _grid.DeleteCurrent(); return true; }
            if (_saveRect.Contains(p)) { _grid.Save(); return true; }
            if (_cancelRect.Contains(p)) { _grid.Cancel(); return true; }
            if (_firstRect.Contains(p)) { _grid.MoveFirst(); return true; }
            if (_prevRect.Contains(p)) { _grid.MovePrevious(); return true; }
            if (_nextRect.Contains(p)) { _grid.MoveNext(); return true; }
            if (_lastRect.Contains(p)) { _grid.MoveLast(); return true; }
            return _queryRect.Contains(p) || _filterRect.Contains(p) || _printRect.Contains(p);
        }
    }
}
