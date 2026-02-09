using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing mouse event handling for BeepSimpleGrid
    /// Handles mouse clicks, movements, hover effects, and column/row resizing
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Mouse Movement

        /// <summary>
        /// Handles mouse move events for hover effects and resize cursors
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _lastMousePos = e.Location;

            // Check if we're resizing
            if (_resizingColumn)
            {
                int delta = e.X - _lastMousePos.X;
                if (_resizingIndex >= 0 && _resizingIndex < Columns.Count)
                {
                    var column = Columns[_resizingIndex];
                    column.Width = Math.Max(20, column.Width + delta);
                    UpdateCachedHorizontalMetrics();
                    UpdateScrollBars();
                    Invalidate();
                }
                return;
            }

            if (_resizingRow)
            {
                int delta = e.Y - _lastMousePos.Y;
                if (_resizingIndex >= 0 && _resizingIndex < Rows.Count)
                {
                    int newHeight = Math.Max(20, Rows[_resizingIndex].Height + delta);
                    Rows[_resizingIndex].Height = newHeight;
                    _rowHeights[_resizingIndex + _dataOffset] = newHeight;
                    UpdateScrollBars();
                    Invalidate();
                }
                return;
            }

            // Check for resize cursor near column borders
            if (IsNearColumnBorder(e.Location, out int columnIndex))
            {
                Cursor = Cursors.SizeWE;
                return;
            }

            // Check for resize cursor near row borders
            if (IsNearRowBorder(e.Location, out int rowIndex))
            {
                Cursor = Cursors.SizeNS;
                return;
            }

            // Reset cursor
            Cursor = Cursors.Default;

            // Handle cell hover
            var cell = GetCellAtLocation(e.Location);
            if (cell != _hoveredCell)
            {
                var previousCell = _hoveredCell;
                _hoveredCell = cell;

                if (previousCell != null)
                {
                    CellMouseLeave?.Invoke(this, new BeepCellEventArgs(previousCell));
                    InvalidateCell(previousCell);
                }

                if (_hoveredCell != null)
                {
                    CellMouseEnter?.Invoke(this, new BeepCellEventArgs(_hoveredCell));
                    InvalidateCell(_hoveredCell);
                }
            }

            // Handle row hover
            if (cell != null)
            {
                int rowIndex2 = cell.RowIndex;
                if (rowIndex2 >= 0 && rowIndex2 < Rows.Count)
                {
                    var row = Rows[rowIndex2];
                    if (row != _hoveredRow)
                    {
                        _hoveredRow = row;
                        _hoveredRowIndex = rowIndex2;
                        Invalidate();
                    }
                }
            }

            // Handle column header hover
            if (_showColumnHeaders && columnsheaderPanelRect.Contains(e.Location))
            {
                int xRelative = e.Location.X - columnsheaderPanelRect.Left + _xOffset;
                int currentX = 0;

                for (int i = 0; i < Columns.Count; i++)
                {
                    var column = Columns[i];
                    if (!column.Visible) continue;

                    if (xRelative >= currentX && xRelative < currentX + column.Width)
                    {
                        _hoveredColumnHeaderIndex = i;
                        
                        // Check if hovering over sort icon
                        if (_showSortIcons && sortIconBounds.Count > i)
                        {
                            Rectangle iconRect = sortIconBounds[i];
                            if (iconRect.Contains(e.Location))
                            {
                                _hoveredSortIconIndex = i;
                                Cursor = Cursors.Hand;
                            }
                            else
                            {
                                _hoveredSortIconIndex = -1;
                            }
                        }

                        // Check if hovering over filter icon
                        if (ShowFilterIcon && filterIconBounds.Count > i)
                        {
                            Rectangle iconRect = filterIconBounds[i];
                            if (iconRect.Contains(e.Location))
                            {
                                _hoveredFilterIconIndex = i;
                                Cursor = Cursors.Hand;
                            }
                            else
                            {
                                _hoveredFilterIconIndex = -1;
                            }
                        }

                        Invalidate(columnsheaderPanelRect);
                        return;
                    }
                    currentX += column.Width;
                }
            }
        }

        /// <summary>
        /// Handles mouse leave events
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredCell != null)
            {
                var previousCell = _hoveredCell;
                _hoveredCell = null;
                CellMouseLeave?.Invoke(this, new BeepCellEventArgs(previousCell));
                InvalidateCell(previousCell);
            }

            _hoveredRow = null;
            _hoveredRowIndex = -1;
            _hoveredColumnHeaderIndex = -1;
            _hoveredSortIconIndex = -1;
            _hoveredFilterIconIndex = -1;
            Cursor = Cursors.Default;
            Invalidate();
        }

        #endregion

        #region Mouse Clicks

        /// <summary>
        /// Handles mouse click events
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Check for sort icon click
            if (_hoveredSortIconIndex >= 0 && _showSortIcons)
            {
                HandleSortIconClick(_hoveredSortIconIndex);
                return;
            }

            // Check for filter icon click
            if (_hoveredFilterIconIndex >= 0 && ShowFilterIcon)
            {
                HandleFilterIconClick(_hoveredFilterIconIndex);
                return;
            }

            // Handle cell click
            var cell = GetCellAtLocation(e.Location);
            if (cell != null)
            {
                SelectCell(cell.RowIndex, cell.ColumnIndex);
                CellClick?.Invoke(this, new BeepCellEventArgs(cell));

                // Handle checkbox column clicks
                var column = Columns[cell.ColumnIndex];
                if (column.IsSelectionCheckBox)
                {
                    var wrapper = _fullData[cell.RowIndex + _dataOffset] as DataRowWrapper;
                    if (wrapper != null)
                    {
                        int rowID = wrapper.RowID;
                        bool isSelected = _persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID];
                        _persistentSelectedRows[rowID] = !isSelected;

                        if (!isSelected)
                        {
                            if (!_selectedRows.Contains(cell.RowIndex + _dataOffset))
                                _selectedRows.Add(cell.RowIndex + _dataOffset);
                        }
                        else
                        {
                            _selectedRows.Remove(cell.RowIndex + _dataOffset);
                        }

                        UpdateRowsSelection();
                        RaiseSelectedRowsChanged();
                    }
                }

                // Double-click handling
                if (e.Clicks == 2)
                {
                    CellDoubleClick?.Invoke(this, new BeepCellEventArgs(cell));
                    if (!ReadOnly && cell.IsEditable)
                    {
                        ShowCellEditor(cell, e.Location);
                    }
                }
            }
        }

        /// <summary>
        /// Handles mouse down events for resize operations
        /// </summary>
        private void BeepGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsNearColumnBorder(e.Location, out int columnIndex))
            {
                _resizingColumn = true;
                _resizingIndex = columnIndex;
                _lastMousePos = e.Location;
                return;
            }

            if (IsNearRowBorder(e.Location, out int rowIndex))
            {
                _resizingRow = true;
                _resizingIndex = rowIndex;
                _lastMousePos = e.Location;
                return;
            }

            var cell = GetCellAtLocation(e.Location);
            if (cell != null)
            {
                CellMouseDown?.Invoke(this, new BeepCellEventArgs(cell));
            }
        }

        /// <summary>
        /// Handles mouse up events to end resize operations
        /// </summary>
        private void BeepGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (_resizingColumn)
            {
                _resizingColumn = false;
                _resizingIndex = -1;
                UpdateCachedHorizontalMetrics();
                UpdateScrollBars();
                Invalidate();
            }

            if (_resizingRow)
            {
                _resizingRow = false;
                _resizingIndex = -1;
                UpdateScrollBars();
                Invalidate();
            }

            var cell = GetCellAtLocation(e.Location);
            if (cell != null)
            {
                CellMouseUp?.Invoke(this, new BeepCellEventArgs(cell));
            }
        }

        #endregion

        #region Resize Detection

        /// <summary>
        /// Checks if the mouse is near a column border for resizing
        /// </summary>
        private bool IsNearColumnBorder(Point location, out int columnIndex)
        {
            if (!columnsheaderPanelRect.Contains(location))
            {
                columnIndex = -1;
                return false;
            }

            int xRelative = location.X - columnsheaderPanelRect.Left + _xOffset;
            int currentX = 0;

            for (int i = 0; i < Columns.Count; i++)
            {
                var column = Columns[i];
                if (!column.Visible) continue;

                int columnRight = currentX + column.Width;
                if (Math.Abs(xRelative - columnRight) <= _resizeMargin)
                {
                    columnIndex = i;
                    return true;
                }
                currentX += column.Width;
            }

            columnIndex = -1;
            return false;
        }

        /// <summary>
        /// Checks if the mouse is near a row border for resizing
        /// </summary>
        private bool IsNearRowBorder(Point location, out int rowIndex)
        {
            int yRelative = location.Y - gridRect.Top;
            if (yRelative < 0)
            {
                rowIndex = -1;
                return false;
            }

            int currentY = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                int rowHeight = Rows[i].Height;
                int rowBottom = currentY + rowHeight;

                if (yRelative >= currentY && yRelative <= rowBottom)
                {
                    if (Math.Abs(yRelative - rowBottom) <= _resizeMargin)
                    {
                        rowIndex = i;
                        return true;
                    }
                    if (i == 0 && Math.Abs(yRelative - currentY) <= _resizeMargin)
                    {
                        rowIndex = -1;
                        return false;
                    }
                    rowIndex = -1;
                    return false;
                }

                currentY += rowHeight;
            }

            rowIndex = -1;
            return false;
        }

        #endregion

        #region Mouse Wheel

        /// <summary>
        /// Handles mouse wheel for scrolling (already defined in base, this is the implementation)
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_verticalScrollBar.Visible)
            {
                int delta = e.Delta > 0 ? -_verticalScrollBar.SmallChange : _verticalScrollBar.SmallChange;
                StartSmoothScroll(_dataOffset + delta);
            }
        }

        #endregion

        // Cell events (CellClick, CellDoubleClick, CellMouseEnter, etc.) are declared in BeepSimpleGrid.Events.cs
    }

    #region Event Args

    ///// <summary>
    ///// Event arguments for cell events
    ///// </summary>
    //public class BeepCellEventArgs : EventArgs
    //{
    //    public BeepCellConfig Cell { get; }
    //    public int RowIndex => Cell?.RowIndex ?? -1;
    //    public int ColumnIndex => Cell?.ColumnIndex ?? -1;
    //    public object CellValue => Cell?.CellValue;
    //    public bool Cancel { get; set; }
    //    public Graphics Graphics { get; set; }

    //    public BeepCellEventArgs(BeepCellConfig cell)
    //    {
    //        Cell = cell;
    //    }
    //}

    /// <summary>
    /// Event arguments for cell selection events
    /// </summary>
    public class BeepCellSelectedEventArgs : EventArgs
    {
        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public BeepCellConfig Cell { get; }

        public BeepCellSelectedEventArgs(int rowIndex, int columnIndex, BeepCellConfig cell)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Cell = cell;
        }
    }

    /// <summary>
    /// Event arguments for row selection events
    /// </summary>
    public class BeepRowSelectedEventArgs : EventArgs
    {
        public int RowIndex { get; }
        public BeepRowConfig Row { get; }

        public BeepRowSelectedEventArgs(int rowIndex, BeepRowConfig row)
        {
            RowIndex = rowIndex;
            Row = row;
        }
    }

    /// <summary>
    /// Hit test result for navigation and interactive elements
    /// </summary>
    internal struct HitTestResult
    {
        public string Name { get; set; }
        public Rectangle TargetRect { get; set; }
        public bool IsValid { get; set; }
    }



    public partial class BeepSimpleGrid
    {
        #region Hit Test Support

        private Dictionary<string, Rectangle> _hitAreas = new Dictionary<string, Rectangle>();

        /// <summary>
        /// Adds an interactive hit area for button or icon clicking
        /// </summary>
        private void AddHitArea(string name, Rectangle rect)
        {
            if (_hitAreas.ContainsKey(name))
            {
                _hitAreas[name] = rect;
            }
            else
            {
                _hitAreas.Add(name, rect);
            }
        }

        /// <summary>
        /// Performs hit testing to find which interactive element was clicked
        /// </summary>
        private bool HitTest(Point location, out HitTestResult result)
        {
            result = default;

            foreach (var hitArea in _hitAreas)
            {
                if (hitArea.Value.Contains(location))
                {
                    result = new HitTestResult
                    {
                        Name = hitArea.Key,
                        TargetRect = hitArea.Value,
                        IsValid = true
                    };
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears all registered hit areas
        /// </summary>
        private void ClearHitAreas()
        {
            _hitAreas.Clear();
        }

        #endregion
    }

    #endregion
}

