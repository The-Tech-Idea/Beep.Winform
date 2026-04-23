using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Handles full keyboard navigation for <see cref="BeepGridPro"/>:
    /// arrow keys, Tab, Home/End, PageUp/PageDown, and selection visibility.
    /// </summary>
    public sealed class GridKeyboardNavigator
    {
        private readonly BeepGridPro _grid;

        public GridKeyboardNavigator(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// Process a key press for cell navigation and editing initiation.
        /// Sets <paramref name="e.Handled"/> = true when the key was consumed.
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            if (_grid.Dialog.HasActiveInlineOverlay)
                return;

            // F2 or Enter starts editing immediately
            if ((e.KeyCode == Keys.F2 || e.KeyCode == Keys.Enter) && !_grid.ReadOnly)
            {
                if (_grid.Selection.HasSelection)
                {
                    var cell = _grid.Data.Rows[_grid.Selection.RowIndex].Cells[_grid.Selection.ColumnIndex];
                    var col = _grid.Data.Columns[_grid.Selection.ColumnIndex];
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
                }
                e.Handled = true;
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
                case Keys.Tab:
                    if (e.KeyCode == Keys.Tab && e.Shift)
                    {
                        c--;
                        if (c < 0)
                        {
                            c = _grid.Data.Columns.Count - 1;
                            r = Math.Max(0, r - 1);
                        }
                    }
                    else
                    {
                        c++;
                        if (c >= _grid.Data.Columns.Count)
                        {
                            c = 0;
                            r = Math.Min(_grid.Data.Rows.Count - 1, r + 1);
                        }
                    }
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
            e.Handled = true;
        }

        /// <summary>
        /// Scrolls the grid so the current selection is within the visible viewport.
        /// </summary>
        public void EnsureSelectionVisible()
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
    }
}
