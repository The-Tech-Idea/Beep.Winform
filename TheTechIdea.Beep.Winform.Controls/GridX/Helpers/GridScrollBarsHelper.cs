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

        // Custom scrollbar properties
        private Rectangle _verticalScrollBarRect;
        private Rectangle _horizontalScrollBarRect;
        private Rectangle _verticalThumbRect;
        private Rectangle _horizontalThumbRect;

        private bool _showVerticalScrollBar = true;
        private bool _showHorizontalScrollBar = true;
        private int _verticalOffset = 0;
        private int _horizontalOffset = 0;

        // Cached metrics for performance
        private int _cachedTotalColumnWidth = 0;
        private int _cachedMaxXOffset = 0;

        // Scrollbar interaction states
        private bool _isVerticalThumbDragging = false;
        private bool _isHorizontalThumbDragging = false;
        private Point _lastMousePos;

        // Scrollbar dimensions
        private const int SCROLLBAR_WIDTH = 15;
        private const int SCROLLBAR_HEIGHT = 15;
        private const int MIN_THUMB_SIZE = 20;

        // Colors for custom scrollbars
        private Color _scrollbarTrackColor = Color.FromArgb(240, 240, 240);
        private Color _scrollbarThumbColor = Color.FromArgb(200, 200, 200);
        private Color _scrollbarThumbHoverColor = Color.FromArgb(180, 180, 180);
        private Color _scrollbarThumbPressedColor = Color.FromArgb(160, 160, 160);

        private bool _verticalThumbHovered = false;
        private bool _horizontalThumbHovered = false;

        // Public properties for scrollbar visibility
        public bool IsVerticalScrollBarNeeded
        {
            get
            {
                if (_grid.Data.Rows == null || _grid.Data.Columns == null)
                    return false;

                int totalRowHeight = CalculateTotalContentHeightWithVariableRows();
                int visibleHeight = _grid.Layout.RowsRect.Height;
                return _showVerticalScrollBar && totalRowHeight > visibleHeight;
            }
        }

        public GridScrollBarsHelper(BeepGridPro grid)
        {
            _grid = grid;
            InitializeScrollBars();
        }

        private void InitializeScrollBars()
        {
            // Initialize scrollbar rectangles
            _verticalScrollBarRect = Rectangle.Empty;
            _horizontalScrollBarRect = Rectangle.Empty;
            _verticalThumbRect = Rectangle.Empty;
            _horizontalThumbRect = Rectangle.Empty;
        }

        public void UpdateBars()
        {
            // Skip in design mode to prevent flickering
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime ||
                !_grid.IsHandleCreated || _grid.IsDisposed)
            {
                return;
            }

            UpdateScrollBarsCore();
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

            //// Update sticky width calculation exactly like BeepSimpleGrid
            //var selColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            //if (_grid.ShowCheckBox && selColumn != null)
            //{
            //    selColumn.Visible = true;
            //}
            //else if (selColumn != null)
            //{
            //    selColumn.Visible = false;
            //}

            var stickyColumns = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => c.Width);
            stickyWidth = Math.Min(stickyWidth, _grid.Layout.RowsRect.Width); // Prevent overflow

            // Calculate scrollable columns width exactly like BeepSimpleGrid
            int visibleColumnCount = _grid.Data.Columns.Count(o => o.Visible);
            int borderWidth = 1;
            int totalBorderWidth = visibleColumnCount > 0 ? (visibleColumnCount - 1) * borderWidth : 0;
            int totalColumnWidth = _grid.Data.Columns.Where(o => o.Visible).Sum(col => col.Width) + totalBorderWidth;
            int visibleWidth = _grid.Layout.RowsRect.Width - (_showVerticalScrollBar && maxVerticalOffset > 0 ? SCROLLBAR_WIDTH : 0);

            // Determine if scrollbars are needed
            bool needsVertical = _showVerticalScrollBar && totalRowHeight > visibleHeight;
            bool needsHorizontal = _showHorizontalScrollBar && totalColumnWidth > visibleWidth;

            // Update scrollbar rectangles and thumb positions
            PositionScrollBars(needsVertical, needsHorizontal, maxVerticalOffset, totalRowHeight, visibleHeight, totalColumnWidth, visibleWidth);

            // Update cached metrics
            UpdateCachedHorizontalMetrics(totalColumnWidth, visibleWidth, needsVertical);
        }

        private void PositionScrollBars(bool needsVertical, bool needsHorizontal, int maxVerticalOffset, int totalRowHeight, int visibleHeight, int totalColumnWidth, int visibleWidth)
        {
            var rowsRect = _grid.Layout.RowsRect;

            // Reset scrollbar rectangles
            _verticalScrollBarRect = Rectangle.Empty;
            _horizontalScrollBarRect = Rectangle.Empty;
            _verticalThumbRect = Rectangle.Empty;
            _horizontalThumbRect = Rectangle.Empty;

            // Position vertical scrollbar
            if (needsVertical)
            {
                int scrollbarWidth = SCROLLBAR_WIDTH;
                int scrollbarHeight = rowsRect.Height - (needsHorizontal ? SCROLLBAR_HEIGHT : 0);

                _verticalScrollBarRect = new Rectangle(
                    rowsRect.Right - scrollbarWidth,
                    rowsRect.Top,
                    scrollbarWidth,
                    scrollbarHeight
                );

                // Calculate thumb size and position
                float thumbRatio = (float)visibleHeight / totalRowHeight;
                int thumbHeight = Math.Max(MIN_THUMB_SIZE, (int)(scrollbarHeight * thumbRatio));
                int maxThumbY = scrollbarHeight - thumbHeight;

                float offsetRatio = maxVerticalOffset > 0 ? (float)_grid.Scroll.VerticalOffset / maxVerticalOffset : 0;
                int thumbY = (int)(offsetRatio * maxThumbY);

                _verticalThumbRect = new Rectangle(
                    _verticalScrollBarRect.X,
                    _verticalScrollBarRect.Y + thumbY,
                    scrollbarWidth,
                    thumbHeight
                );
            }

            // Position horizontal scrollbar
            if (needsHorizontal)
            {
                int scrollbarHeight = SCROLLBAR_HEIGHT;
                int scrollbarWidth = rowsRect.Width - (needsVertical ? SCROLLBAR_WIDTH : 0);

                _horizontalScrollBarRect = new Rectangle(
                    rowsRect.Left,
                    rowsRect.Bottom - scrollbarHeight,
                    scrollbarWidth,
                    scrollbarHeight
                );

                // Calculate thumb size and position
                int maxHorizontalOffset = Math.Max(0, totalColumnWidth - visibleWidth);
                float thumbRatio = maxHorizontalOffset > 0 ? (float)visibleWidth / totalColumnWidth : 1;
                int thumbWidth = Math.Max(MIN_THUMB_SIZE, (int)(scrollbarWidth * thumbRatio));
                int maxThumbX = scrollbarWidth - thumbWidth;

                float offsetRatio = maxHorizontalOffset > 0 ? (float)_grid.Scroll.HorizontalOffset / maxHorizontalOffset : 0;
                int thumbX = (int)(offsetRatio * maxThumbX);

                _horizontalThumbRect = new Rectangle(
                    _horizontalScrollBarRect.X + thumbX,
                    _horizontalScrollBarRect.Y,
                    thumbWidth,
                    scrollbarHeight
                );
            }
        }

        private void UpdateCachedHorizontalMetrics(int totalColumnWidth, int visibleWidth, bool needsVertical)
        {
            int scrollbarWidth = needsVertical ? SCROLLBAR_WIDTH : 0;
            _cachedTotalColumnWidth = totalColumnWidth;
            _cachedMaxXOffset = Math.Max(0, totalColumnWidth - (visibleWidth - scrollbarWidth));
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

        public void DrawScrollBars(Graphics g)
        {
            // Draw vertical scrollbar
            if (!_verticalScrollBarRect.IsEmpty)
            {
                // Draw track
                using (var brush = new SolidBrush(_scrollbarTrackColor))
                {
                    g.FillRectangle(brush, _verticalScrollBarRect);
                }

                // Draw thumb
                Color thumbColor = _scrollbarThumbColor;
                if (_isVerticalThumbDragging)
                    thumbColor = _scrollbarThumbPressedColor;
                else if (_verticalThumbHovered)
                    thumbColor = _scrollbarThumbHoverColor;

                using (var brush = new SolidBrush(thumbColor))
                {
                    g.FillRectangle(brush, _verticalThumbRect);
                }

                // Draw border
                using (var pen = new Pen(Color.FromArgb(180, 180, 180)))
                {
                    g.DrawRectangle(pen, _verticalScrollBarRect);
                }
            }

            // Draw horizontal scrollbar
            if (!_horizontalScrollBarRect.IsEmpty)
            {
                // Draw track
                using (var brush = new SolidBrush(_scrollbarTrackColor))
                {
                    g.FillRectangle(brush, _horizontalScrollBarRect);
                }

                // Draw thumb
                Color thumbColor = _scrollbarThumbColor;
                if (_isHorizontalThumbDragging)
                    thumbColor = _scrollbarThumbPressedColor;
                else if (_horizontalThumbHovered)
                    thumbColor = _scrollbarThumbHoverColor;

                using (var brush = new SolidBrush(thumbColor))
                {
                    g.FillRectangle(brush, _horizontalThumbRect);
                }

                // Draw border
                using (var pen = new Pen(Color.FromArgb(180, 180, 180)))
                {
                    g.DrawRectangle(pen, _horizontalScrollBarRect);
                }
            }

            // Fill the corner box when both scrollbars are visible
            if (!_verticalScrollBarRect.IsEmpty && !_horizontalScrollBarRect.IsEmpty)
            {
                Rectangle cornerBox = new Rectangle(
                    _verticalScrollBarRect.X,
                    _horizontalScrollBarRect.Y,
                    SCROLLBAR_WIDTH,
                    SCROLLBAR_HEIGHT
                );

                using (var brush = new SolidBrush(_scrollbarTrackColor))
                {
                    g.FillRectangle(brush, cornerBox);
                }

                // Draw border to match scrollbars
                using (var pen = new Pen(Color.FromArgb(180, 180, 180)))
                {
                    g.DrawRectangle(pen, cornerBox);
                }
            }
        }

        public void HandleMouseMove(Point location)
        {
            bool wasVerticalHovered = _verticalThumbHovered;
            bool wasHorizontalHovered = _horizontalThumbHovered;

            _verticalThumbHovered = _verticalThumbRect.Contains(location);
            _horizontalThumbHovered = _horizontalThumbRect.Contains(location);

            // Update cursor
            if (_verticalThumbHovered || _horizontalThumbHovered)
            {
                _grid.Cursor = Cursors.Hand;
            }
            else if (!_isVerticalThumbDragging && !_isHorizontalThumbDragging)
            {
                _grid.Cursor = Cursors.Default;
            }

            // Handle dragging
            if (_isVerticalThumbDragging)
            {
                HandleVerticalThumbDrag(location);
            }
            else if (_isHorizontalThumbDragging)
            {
                HandleHorizontalThumbDrag(location);
            }

            // Redraw if hover state changed
            if (wasVerticalHovered != _verticalThumbHovered || wasHorizontalHovered != _horizontalThumbHovered)
            {
                _grid.Invalidate();
            }
        }

        public void HandleMouseDown(Point location, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                if (_verticalThumbRect.Contains(location))
                {
                    _isVerticalThumbDragging = true;
                    _lastMousePos = location;
                    _grid.Capture = true;
                }
                else if (_horizontalThumbRect.Contains(location))
                {
                    _isHorizontalThumbDragging = true;
                    _lastMousePos = location;
                    _grid.Capture = true;
                }
                else if (_verticalScrollBarRect.Contains(location))
                {
                    // Page up/down
                    HandleVerticalPageClick(location);
                }
                else if (_horizontalScrollBarRect.Contains(location))
                {
                    // Page left/right
                    HandleHorizontalPageClick(location);
                }
            }
        }

        public void HandleMouseUp(Point location, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                bool wasDragging = _isVerticalThumbDragging || _isHorizontalThumbDragging;
                
                _isVerticalThumbDragging = false;
                _isHorizontalThumbDragging = false;
                _grid.Capture = false;
                _grid.Cursor = Cursors.Default;
                
                // Update scrollbar positions after drag is complete
                if (wasDragging)
                {
                    UpdateBars();
                    _grid.Invalidate();
                }
            }
        }

        private void HandleVerticalThumbDrag(Point location)
        {
            int deltaY = location.Y - _lastMousePos.Y;
            _lastMousePos = location;

            if (!_verticalScrollBarRect.IsEmpty && deltaY != 0)
            {
                float scrollbarHeight = _verticalScrollBarRect.Height - _verticalThumbRect.Height;
                if (scrollbarHeight > 0)
                {
                    float ratio = deltaY / scrollbarHeight;
                    int totalRowHeight = CalculateTotalContentHeightWithVariableRows();
                    int visibleHeight = _grid.Layout.RowsRect.Height;
                    int maxOffset = Math.Max(0, totalRowHeight - visibleHeight);

                    int newOffset = _grid.Scroll.VerticalOffset + (int)(ratio * maxOffset);
                    newOffset = Math.Max(0, Math.Min(maxOffset, newOffset));

                    if (newOffset != _grid.Scroll.VerticalOffset)
                    {
                        _grid.Scroll.SetVerticalOffset(newOffset);
                        int rowIndex = CalculateRowIndexForPixelOffset(newOffset);
                        _grid.Scroll.SetVerticalIndex(rowIndex);
                        _grid.Invalidate();
                    }
                }
            }
        }

        private void HandleHorizontalThumbDrag(Point location)
        {
            int deltaX = location.X - _lastMousePos.X;
            _lastMousePos = location;

            if (!_horizontalScrollBarRect.IsEmpty && deltaX != 0)
            {
                float scrollbarWidth = _horizontalScrollBarRect.Width - _horizontalThumbRect.Width;
                if (scrollbarWidth > 0)
                {
                    float ratio = deltaX / scrollbarWidth;
                    int totalColumnWidth = CalculateTotalContentWidth();
                    int visibleWidth = _grid.Layout.RowsRect.Width - (_verticalScrollBarRect.IsEmpty ? 0 : SCROLLBAR_WIDTH);
                    int maxOffset = Math.Max(0, totalColumnWidth - visibleWidth);

                    int newOffset = _grid.Scroll.HorizontalOffset + (int)(ratio * maxOffset);
                    newOffset = Math.Max(0, Math.Min(maxOffset, newOffset));

                    if (newOffset != _grid.Scroll.HorizontalOffset)
                    {
                        _grid.Scroll.SetHorizontalOffset(newOffset);
                        _grid.Layout.Recalculate(); // Recalculate header positions with new offset
                        _grid.Invalidate();
                    }
                }
            }
        }

        private void HandleVerticalPageClick(Point location)
        {
            if (_verticalThumbRect.Contains(location))
                return; // Already handled by thumb drag

            int totalRowHeight = CalculateTotalContentHeightWithVariableRows();
            int visibleHeight = _grid.Layout.RowsRect.Height;
            int maxOffset = Math.Max(0, totalRowHeight - visibleHeight);
            int pageSize = visibleHeight;

            int newOffset;
            if (location.Y < _verticalThumbRect.Y)
            {
                // Page up
                newOffset = Math.Max(0, _grid.Scroll.VerticalOffset - pageSize);
            }
            else
            {
                // Page down
                newOffset = Math.Min(maxOffset, _grid.Scroll.VerticalOffset + pageSize);
            }

            _grid.Scroll.SetVerticalOffset(newOffset);
            int rowIndex = CalculateRowIndexForPixelOffset(newOffset);
            _grid.Scroll.SetVerticalIndex(rowIndex);

            // Update the scrollbar thumb position to reflect the new offset
            UpdateBars();
            _grid.Invalidate();
        }

        private void HandleHorizontalPageClick(Point location)
        {
            if (_horizontalThumbRect.Contains(location))
                return; // Already handled by thumb drag

            int totalColumnWidth = _cachedTotalColumnWidth;
            int visibleWidth = _grid.Layout.RowsRect.Width - (_showVerticalScrollBar ? SCROLLBAR_WIDTH : 0);
            int pageSize = visibleWidth;

            if (location.X < _horizontalThumbRect.X)
            {
                // Page left
                int newOffset = Math.Max(0, _grid.Scroll.HorizontalOffset - pageSize);
                _grid.Scroll.SetHorizontalOffset(newOffset);
                _grid.Layout.Recalculate(); // Recalculate header positions with new offset
            }
            else
            {
                // Page right
                int maxOffset = Math.Max(0, totalColumnWidth - visibleWidth);
                int newOffset = Math.Min(maxOffset, _grid.Scroll.HorizontalOffset + pageSize);
                _grid.Scroll.SetHorizontalOffset(newOffset);
                _grid.Layout.Recalculate(); // Recalculate header positions with new offset
            }

            _grid.Invalidate();
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
            // Handle mouse wheel scrolling using custom scrollbar logic
            if (!_verticalScrollBarRect.IsEmpty)
            {
                // Calculate scroll step based on row height
                int step = _grid.RowHeight;
                int delta = (e.Delta / SystemInformation.MouseWheelScrollDelta) * step;

                // Calculate new vertical offset
                int totalRowHeight = CalculateTotalContentHeightWithVariableRows();
                int visibleHeight = _grid.Layout.RowsRect.Height;
                int maxOffset = Math.Max(0, totalRowHeight - visibleHeight);

                int newOffset = _grid.Scroll.VerticalOffset - delta;
                newOffset = Math.Max(0, Math.Min(maxOffset, newOffset));

                if (newOffset != _grid.Scroll.VerticalOffset)
                {
                    _grid.Scroll.SetVerticalOffset(newOffset);
                    int rowIndex = CalculateRowIndexForPixelOffset(newOffset);
                    _grid.Scroll.SetVerticalIndex(rowIndex);
                    // Update the scrollbar thumb position after mouse wheel scroll
                    UpdateBars();
                    _grid.Invalidate();
                }
            }
        }
    }
}
