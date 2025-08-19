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

        public GridInputHelper(BeepGridPro grid) { _grid = grid; }

        public void HandleMouseDown(MouseEventArgs e)
        {
            _mouseDown = e.Location;

            // Column resize hit test in header
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

            // Select a cell
            var (r, c) = HitTestCell(e.Location);
            if (r >= 0 && c >= 0)
            {
                _grid.Selection.SelectCell(r, c);
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
                _grid.Invalidate();
                return;
            }

            if (_grid.Layout.HeaderRect.Contains(e.Location))
            {
                _grid.Cursor = HitTestColumnBorder(e.Location) >= 0 ? Cursors.VSplit : Cursors.Default;
            }
            else
            {
                _grid.Cursor = Cursors.Default;
            }
        }

        public void HandleMouseUp(MouseEventArgs e)
        {
            _resizingColumn = false;
            _resizingColIndex = -1;
            _grid.Cursor = Cursors.Default;
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (!_grid.Selection.HasSelection)
            {
                if (_grid.Data.Rows.Count > 0 && _grid.Data.Columns.Count > 0)
                {
                    _grid.Selection.SelectCell(0, 0);
                    _grid.Invalidate();
                }
                return;
            }

            int r = _grid.Selection.RowIndex;
            int c = _grid.Selection.ColumnIndex;
            switch (e.KeyCode)
            {
                case Keys.Left:  c = Math.Max(0, c - 1); break;
                case Keys.Right: c = Math.Min(_grid.Data.Columns.Count - 1, c + 1); break;
                case Keys.Up:    r = Math.Max(0, r - 1); break;
                case Keys.Down:  r = Math.Min(_grid.Data.Rows.Count - 1, r + 1); break;
                case Keys.Home:  c = 0; break;
                case Keys.End:   c = _grid.Data.Columns.Count - 1; break;
                case Keys.PageUp:   r = Math.Max(0, r - VisibleRowCount()); break;
                case Keys.PageDown: r = Math.Min(_grid.Data.Rows.Count - 1, r + VisibleRowCount()); break;
                default: return;
            }
            _grid.Selection.SelectCell(r, c);
            _grid.Selection.EnsureVisible();
            _grid.Invalidate();
        }

        private int VisibleRowCount()
        {
            return Math.Max(1, _grid.Layout.RowsRect.Height / _grid.RowHeight);
        }

        private (int row, int col) HitTestCell(Point p)
        {
            for (int r = 0; r < _grid.Data.Rows.Count; r++)
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
            if (_grid.Data.Columns.Count == 0) return -1;
            int x = _grid.Layout.HeaderRect.Left;
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                x += _grid.Data.Columns[i].Width;
                if (Math.Abs(p.X - x) <= _resizeMargin)
                    return i;
            }
            return -1;
        }
    }
}
