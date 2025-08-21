using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridScrollBarsHelper
    {
        private readonly BeepGridPro _grid;
        private readonly BeepScrollBar _verticalScrollBar;
        private readonly BeepScrollBar _horizontalScrollBar;
        private bool _showVerticalScrollBar = true;
        private bool _showHorizontalScrollBar = true;
        private int _yOffset = 0;
        private int _xOffset = 0;
        private bool _scrollBarsUpdatePending = false;
        private int _cachedTotalColumnWidth = 0;
        private int _cachedMaxXOffset = 0;

        public GridScrollBarsHelper(BeepGridPro grid)
        {
            _grid = grid;

            // Create exactly like BeepSimpleGrid
            _verticalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Theme = _grid.Theme,
                Visible = false,
                IsChild = true,
                Width = 15
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            _grid.Controls.Add(_verticalScrollBar);

            _horizontalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Theme = _grid.Theme,
                Visible = false,
                IsChild = true,
                Height = 15
            };
            _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            _grid.Controls.Add(_horizontalScrollBar);
        }

        public void UpdateBars()
        {
            // Skip in design mode to prevent flickering
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime ||
                !_grid.IsHandleCreated || _grid.IsDisposed)
            {
                return;
            }

            if (_scrollBarsUpdatePending) return;

            _scrollBarsUpdatePending = true;

            // Use BeginInvoke only if handle exists and not in design mode
            if (_grid.IsHandleCreated && !_grid.IsDisposed)
            {
                _grid.BeginInvoke(new Action(() =>
                {
                    _scrollBarsUpdatePending = false;
                    UpdateScrollBarsCore();
                }));
            }
            else
            {
                _scrollBarsUpdatePending = false;
            }
        }

        private void UpdateScrollBarsCore()
        {
            if (_grid.Data.Rows == null || _grid.Data.Columns == null)
                return;

            // Calculate content dimensions with variable row heights
            int totalRowHeight = CalculateTotalContentHeightWithVariableRows();
            int visibleHeight = _grid.Layout.RowsRect.Height;
            
            // For variable row heights, we need to calculate offset differently
            int maxVerticalOffset = Math.Max(0, totalRowHeight - visibleHeight);

            // Update sticky width calculation exactly like BeepSimpleGrid
            var selColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_grid.ShowCheckBox && selColumn != null)
            {
                selColumn.Visible = true;
            }
            else if (selColumn != null)
            {
                selColumn.Visible = false;
            }

            var stickyColumns = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => c.Width);
            stickyWidth = Math.Min(stickyWidth, _grid.Layout.RowsRect.Width); // Prevent overflow

            // Calculate scrollable columns width exactly like BeepSimpleGrid
            int visibleColumnCount = _grid.Data.Columns.Count(o => o.Visible);
            int borderWidth = 1;
            int totalBorderWidth = visibleColumnCount > 0 ? (visibleColumnCount - 1) * borderWidth : 0;
            int totalColumnWidth = _grid.Data.Columns.Where(o => o.Visible).Sum(col => col.Width) + totalBorderWidth;
            int visibleWidth = _grid.Layout.RowsRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);

            // Determine if scrollbars are needed
            bool needsVertical = _showVerticalScrollBar && totalRowHeight > visibleHeight;
            bool needsHorizontal = _showHorizontalScrollBar && totalColumnWidth > visibleWidth;

            // Update vertical scrollbar with pixel-based scrolling for variable row heights
            if (needsVertical)
            {
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = totalRowHeight; // set to content height
                _verticalScrollBar.LargeChange = visibleHeight;
                _verticalScrollBar.SmallChange = _grid.RowHeight; // Use default row height for scroll step

                // Clamp Value to effective upper bound (Maximum - LargeChange)
                int upper = Math.Max(_verticalScrollBar.Minimum, _verticalScrollBar.Maximum - _verticalScrollBar.LargeChange);
                int desired = Math.Max(0, Math.Min(_grid.Scroll.VerticalOffset, upper));
                if (_verticalScrollBar.Value != desired)
                    _verticalScrollBar.Value = desired;

                _verticalScrollBar.Visible = true;
            }
            else
            {
                if (_verticalScrollBar.Visible)
                {
                    _verticalScrollBar.Visible = false;
                    _grid.Scroll.SetVerticalIndex(0);
                    _grid.Scroll.SetVerticalOffset(0);
                }
            }

            // Update horizontal scrollbar exactly like BeepSimpleGrid
            if (needsHorizontal)
            {
                int maxXOffset = Math.Max(0, totalColumnWidth - visibleWidth);
                _horizontalScrollBar.Minimum = 0;
                _horizontalScrollBar.Maximum = totalColumnWidth;
                _horizontalScrollBar.LargeChange = visibleWidth;
                _horizontalScrollBar.SmallChange = _grid.Data.Columns.Where(c => !c.Sticked && c.Visible).DefaultIfEmpty().Min(c => c != null ? c.Width : 20) / 2;
                int desiredX = Math.Max(0, Math.Min(_grid.Scroll.HorizontalOffset, maxXOffset));
                if (_horizontalScrollBar.Value != desiredX)
                    _horizontalScrollBar.Value = desiredX;
                _horizontalScrollBar.Visible = true;
            }
            else
            {
                if (_horizontalScrollBar.Visible)
                {
                    _horizontalScrollBar.Visible = false;
                    _grid.Scroll.SetHorizontalOffset(0);
                }
            }

            PositionScrollBars();
            UpdateCachedHorizontalMetrics();
        }

        private void PositionScrollBars()
        {
            if (_verticalScrollBar == null || _horizontalScrollBar == null) return;

            int verticalScrollWidth = _verticalScrollBar.Width;
            int horizontalScrollHeight = _horizontalScrollBar.Height;
            var rowsRect = _grid.Layout.RowsRect;

            // Position exactly like BeepSimpleGrid
            if (_verticalScrollBar.Visible)
            {
                _verticalScrollBar.Location = new Point(rowsRect.Right - verticalScrollWidth, rowsRect.Top);
                _verticalScrollBar.Height = rowsRect.Height - (_horizontalScrollBar.Visible ? horizontalScrollHeight : 0);
            }

            if (_horizontalScrollBar.Visible)
            {
                _horizontalScrollBar.Location = new Point(rowsRect.Left, rowsRect.Bottom - horizontalScrollHeight);
                _horizontalScrollBar.Width = rowsRect.Width - (_verticalScrollBar.Visible ? verticalScrollWidth : 0);
            }
        }

        private void UpdateCachedHorizontalMetrics()
        {
            int visibleColumnCount = _grid.Data.Columns.Count(o => o.Visible);
            int borderWidth = 1;
            int totalBorderWidth = visibleColumnCount > 0 ? (visibleColumnCount - 1) * borderWidth : 0;
            _cachedTotalColumnWidth = _grid.Data.Columns.Where(o => o.Visible).Sum(col => col.Width) + totalBorderWidth;
            _cachedMaxXOffset = Math.Max(0, _cachedTotalColumnWidth - (_grid.Layout.RowsRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0)));
        }

        private int GetVisibleRowCount()
        {
            if (_grid.Data.Rows == null || _grid.Data.Rows.Count == 0)
            {
                return 0;
            }

            int availableHeight = _grid.Layout.RowsRect.Height;
            if (availableHeight <= 0)
            {
                return 1;
            }

            int visibleCount = availableHeight / _grid.RowHeight;
            return Math.Max(1, Math.Min(visibleCount, _grid.Data.Rows.Count));
        }

        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            // Translate scrollbar value (pixel offset) to grid model
            int pixelOffset = _verticalScrollBar.Value;
            int rowIndex = CalculateRowIndexForPixelOffset(pixelOffset);
            _grid.Scroll.SetVerticalOffset(pixelOffset);
            _grid.Scroll.SetVerticalIndex(rowIndex);
            _grid.Invalidate();
        }

        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            _grid.Scroll.SetHorizontalOffset(_horizontalScrollBar.Value);
            _grid.Invalidate();
        }

        public void SyncFromModel()
        {
            // Sync scrollbar positions with scroll model values
            if (_verticalScrollBar.Visible)
            {
                int upper = Math.Max(_verticalScrollBar.Minimum, _verticalScrollBar.Maximum - _verticalScrollBar.LargeChange);
                int desired = Math.Max(_verticalScrollBar.Minimum, Math.Min(upper, _grid.Scroll.VerticalOffset));
                if (_verticalScrollBar.Value != desired)
                {
                    _verticalScrollBar.Value = desired;
                    _yOffset = desired;
                }
            }
            
            if (_horizontalScrollBar.Visible)
            {
                int maxX = Math.Max(0, _horizontalScrollBar.Maximum - _horizontalScrollBar.LargeChange);
                int desiredX = Math.Max(_horizontalScrollBar.Minimum, Math.Min(maxX, _grid.Scroll.HorizontalOffset));
                if (_horizontalScrollBar.Value != desiredX)
                {
                    _horizontalScrollBar.Value = desiredX;
                    _xOffset = desiredX;
                }
            }
        }

        private int CalculateTotalContentHeight()
        {
            // Calculate total height exactly like BeepSimpleGrid
            return (_grid.Data.Rows?.Count ?? 0) * _grid.RowHeight;
        }

        private int CalculateTotalContentHeightWithVariableRows()
        {
            if (_grid.Data.Rows == null || _grid.Data.Rows.Count == 0)
                return 0;

            // Sum up all individual row heights
            int totalHeight = 0;
            foreach (var row in _grid.Data.Rows)
            {
                totalHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            return totalHeight;
        }

        private int CalculateTotalContentWidth()
        {
            // Calculate total width exactly like BeepSimpleGrid
            int visibleColumnCount = _grid.Data.Columns.Count(o => o.Visible);
            int borderWidth = 1;
            int totalBorderWidth = visibleColumnCount > 0 ? (visibleColumnCount - 1) * borderWidth : 0;
            return _grid.Data.Columns.Where(o => o.Visible).Sum(col => col.Width) + totalBorderWidth;
        }

        private int CalculateRowIndexForPixelOffset(int pixelOffset)
        {
            if (_grid.Data.Rows == null || pixelOffset <= 0)
                return 0;

            int currentOffset = 0;
            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (currentOffset + rowHeight > pixelOffset)
                    return i;
                
                currentOffset += rowHeight;
            }

            return Math.Max(0, _grid.Data.Rows.Count - 1);
        }

        private int CalculatePixelOffsetForRowIndex(int rowIndex)
        {
            if (_grid.Data.Rows == null || rowIndex <= 0)
                return 0;

            int pixelOffset = 0;
            int maxIndex = Math.Min(rowIndex, _grid.Data.Rows.Count);
            
            for (int i = 0; i < maxIndex; i++)
            {
                var row = _grid.Data.Rows[i];
                pixelOffset += row.Height > 0 ? row.Height : _grid.RowHeight;
            }

            return pixelOffset;
        }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            if (_verticalScrollBar.Visible)
            {
                // Wheel delta: positive means scroll up; our model uses smaller pixel offset when scrolling up
                int step = _grid.RowHeight;
                int desired = _verticalScrollBar.Value - (e.Delta / SystemInformation.MouseWheelScrollDelta) * step;
                int upper = Math.Max(_verticalScrollBar.Minimum, _verticalScrollBar.Maximum - _verticalScrollBar.LargeChange);
                int newValue = Math.Max(_verticalScrollBar.Minimum, Math.Min(upper, desired));
                if (newValue != _verticalScrollBar.Value)
                {
                    _verticalScrollBar.Value = newValue;
                    int rowIndex = CalculateRowIndexForPixelOffset(newValue);
                    _grid.Scroll.SetVerticalOffset(newValue);
                    _grid.Scroll.SetVerticalIndex(rowIndex);
                    _grid.Invalidate();
                }
            }
        }
    }
}
