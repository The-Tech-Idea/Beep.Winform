using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing scrolling functionality for BeepSimpleGrid
    /// Handles scrollbar management, smooth scrolling, and scroll position tracking
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Scrollbar Management

        /// <summary>
        /// Updates scrollbar visibility and properties
        /// </summary>
        private void UpdateScrollBars()
        {
            if (!IsHandleCreated || DesignMode)
            {
                return;
            }

            if (_scrollBarsUpdatePending) return;

            _scrollBarsUpdatePending = true;

            if (IsHandleCreated && !IsDisposed)
            {
                BeginInvoke(new Action(() =>
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

        /// <summary>
        /// Core scrollbar update logic
        /// </summary>
        private void UpdateScrollBarsCore()
        {
            if (DesignMode)
                return;
            if (_verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            if (_fullData == null || !_fullData.Any())
            {
                _verticalScrollBar.Visible = false;
                _horizontalScrollBar.Visible = false;
                return;
            }

            int totalRowHeight = (_fullData.Count * RowHeight) + (_showaggregationRow ? bottomagregationPanelHeight : 0);
            int visibleHeight = gridRect.Height;
            int visibleRowCount = GetVisibleRowCount();
            int aggregationRows = _showaggregationRow ? 1 : 0;

            int maxOffset = Math.Max(0, _fullData.Count - visibleRowCount);

            if (_showVerticalScrollBar && _fullData.Count >= visibleRowCount)
            {
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = maxOffset + visibleRowCount;
                _verticalScrollBar.LargeChange = visibleRowCount;
                _verticalScrollBar.SmallChange = 1;
                _verticalScrollBar.Value = Math.Min(_dataOffset, maxOffset);
                _verticalScrollBar.Visible = true;
            }
            else
            {
                if (_verticalScrollBar.Visible)
                {
                    _verticalScrollBar.Visible = false;
                    _dataOffset = 0;
                    FillVisibleRows();
                }
            }

            int totalColumnWidth = Columns.Where(o => o.Visible).Sum(col => col.Width);
            int visibleColumnCount = Columns.Count(o => o.Visible);
            int borderWidth = 1;
            int totalBorderWidth = visibleColumnCount > 0 ? (visibleColumnCount - 1) * borderWidth : 0;
            totalColumnWidth += totalBorderWidth;
            int visibleWidth = gridRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);

            bool horizontalScrollNeeded = _showHorizontalScrollBar && totalColumnWidth > visibleWidth;
            if (horizontalScrollNeeded)
            {
                int maxXOffset = Math.Max(0, totalColumnWidth - visibleWidth);
                _horizontalScrollBar.Minimum = 0;
                _horizontalScrollBar.Maximum = totalColumnWidth;
                _horizontalScrollBar.LargeChange = visibleWidth;
                _horizontalScrollBar.SmallChange = Columns.Where(c => !c.Sticked && c.Visible).Min(c => c.Width) / 2;
                _horizontalScrollBar.Value = Math.Max(0, Math.Min(_xOffset, maxXOffset));
                _horizontalScrollBar.Visible = true;
            }
            else
            {
                if (_horizontalScrollBar.Visible)
                {
                    _horizontalScrollBar.Visible = false;
                    _xOffset = 0;
                }
            }
            UpdateHeaderLayout();
        }

        /// <summary>
        /// Positions scrollbars in their correct locations
        /// </summary>
        private void PositionScrollBars()
        {
            if (_verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            var drawingBounds = DrawingRect;
            int scrollBarWidth = _verticalScrollBar.Width;
            int scrollBarHeight = _horizontalScrollBar.Height;

            // Position vertical scrollbar
            if (_verticalScrollBar.Visible)
            {
                _verticalScrollBar.Location = new Point(
                    drawingBounds.Right - scrollBarWidth,
                    gridRect.Top
                );
                _verticalScrollBar.Height = gridRect.Height;
            }

            // Position horizontal scrollbar
            if (_horizontalScrollBar.Visible)
            {
                _horizontalScrollBar.Location = new Point(
                    drawingBounds.Left,
                    drawingBounds.Bottom - scrollBarHeight - (_showNavigator ? navigatorPanelHeight : 0) - (_showFooter ? footerPanelHeight : 0) - (_showaggregationRow ? bottomagregationPanelHeight : 0)
                );
                _horizontalScrollBar.Width = drawingBounds.Width - (_verticalScrollBar.Visible ? scrollBarWidth : 0);
            }
        }

        /// <summary>
        /// Updates cached horizontal metrics for scrolling
        /// </summary>
        private void UpdateCachedHorizontalMetrics()
        {
            int visibleColumnCount = Columns.Count(o => o.Visible);
            int borderWidth = 1;
            int totalBorderWidth = visibleColumnCount > 0 ? (visibleColumnCount - 1) * borderWidth : 0;
            _cachedTotalColumnWidth = Columns.Where(o => o.Visible).Sum(col => col.Width) + totalBorderWidth;
            _cachedMaxXOffset = Math.Max(0, _cachedTotalColumnWidth - (gridRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0)));
        }

        #endregion

        #region Smooth Scrolling

        /// <summary>
        /// Starts smooth scroll animation to target offset
        /// </summary>
        private void StartSmoothScroll(int targetOffset)
        {
            if (_fullData == null || _fullData.Count == 0) return;

            int visibleRowCount = GetVisibleRowCount();
            int maxOffset = Math.Max(0, _fullData.Count - visibleRowCount);
            targetOffset = Math.Max(0, Math.Min(targetOffset, maxOffset));

            if (targetOffset == _dataOffset) return;

            _scrollTargetVertical = targetOffset;

            if (_scrollTimer == null)
            {
                _scrollTimer = new Timer();
                _scrollTimer.Interval = 16; // ~60 FPS
                _scrollTimer.Tick += ScrollTimer_Tick;
            }

            _scrollTimer.Stop();
            _scrollTimer.Start();
        }

        /// <summary>
        /// Scroll timer tick event for smooth animation
        /// </summary>
        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            bool updated = false;

            // Vertical smooth scroll
            if (_dataOffset != _scrollTargetVertical)
            {
                int delta = _scrollTargetVertical - _dataOffset;
                int step = Math.Max(1, Math.Abs(delta) / _scrollStep);

                if (delta > 0)
                {
                    _dataOffset = Math.Min(_dataOffset + step, _scrollTargetVertical);
                }
                else
                {
                    _dataOffset = Math.Max(_dataOffset - step, _scrollTargetVertical);
                }

                if (_verticalScrollBar.Visible)
                {
                    _verticalScrollBar.Value = Math.Max(_verticalScrollBar.Minimum,
                        Math.Min(_dataOffset, _verticalScrollBar.Maximum - _verticalScrollBar.LargeChange + 1));
                }

                updated = true;
            }

            if (!updated)
            {
                if (_scrollTimer != null)
                {
                    _scrollTimer.Stop();
                    _scrollTimer.Dispose();
                    _scrollTimer = null;
                }
            }
            else
            {
                UpdateCellPositions();
                if (Math.Abs(_dataOffset - _scrollTargetVertical) > 0)
                {
                    FillVisibleRows();
                }
                else
                {
                    Invalidate(gridRect);
                }
            }
        }

        /// <summary>
        /// Scrolls by a delta amount
        /// </summary>
        private void ScrollBy(int delta)
        {
            int newOffset = _dataOffset + delta;
            int maxOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
            newOffset = Math.Max(0, Math.Min(newOffset, maxOffset));
            if (newOffset != _dataOffset)
            {
                StartSmoothScroll(newOffset);
            }
        }

        #endregion

        #region Scrollbar Event Handlers

        /// <summary>
        /// Handles vertical scrollbar scroll event
        /// </summary>
        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            // Immediately apply the new value without animation for direct clicks
            _dataOffset = _verticalScrollBar.Value;

            // Stop any ongoing animations first
            if (_scrollTimer != null && _scrollTimer.Enabled)
            {
                _scrollTimer.Stop();
            }

            // Update UI
            FillVisibleRows();
            UpdateCellPositions();
            MoveEditorIn(); // Move editor if active
            Invalidate();
        }

        /// <summary>
        /// Handles horizontal scrollbar scroll event
        /// </summary>
        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            // Immediately apply the new value without animation for direct clicks
            _xOffset = _horizontalScrollBar.Value;

            // Stop any ongoing animations first
            if (_scrollTimer != null && _scrollTimer.Enabled)
            {
                _scrollTimer.Stop();
            }

            // Update UI
            UpdateCellPositions();
            MoveEditorIn(); // Move editor if active
            Invalidate();
        }

        #endregion

        #region Cell Position Updates

        /// <summary>
        /// Updates cell positions after scrolling
        /// </summary>
        private void UpdateCellPositions()
        {
            if (Rows == null || Rows.Count == 0)
            {
                return;
            }
            // Clear cache when positions need to be recalculated
            _cellBounds.Clear();
            // yOffset is now 0 since Rows only contains visible rows
            int currentY = gridRect.Top;

            // Update positions for all visible rows
            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var row = Rows[rowIndex];
                row.UpperY = currentY;

                int x = gridRect.Left;
                int stickyWidthTotal = _stickyWidth;
                int scrollableX = x + stickyWidthTotal - _xOffset;

                for (int colIndex = 0; colIndex < Columns.Count; colIndex++)
                {
                    if (Columns[colIndex].Visible)
                    {
                        var cell = row.Cells[colIndex];
                        if (Columns[colIndex].Sticked)
                        {
                            cell.X = x;
                            x += Columns[colIndex].Width;
                        }
                        else
                        {
                            cell.X = scrollableX;
                            scrollableX += Columns[colIndex].Width;
                        }
                        cell.Y = row.UpperY;
                        cell.Width = Columns[colIndex].Width;
                        cell.Height = row.Height;
                    }
                }

                currentY += row.Height;
            }
        }

        /// <summary>
        /// Updates header layout for select all checkbox
        /// </summary>
        private void UpdateHeaderLayout()
        {
            if (_showCheckboxes && _showColumnHeaders && _columns.Any(c => c.IsSelectionCheckBox))
            {
                var selColumn = _columns.First(c => c.IsSelectionCheckBox);
                int colIndex = _columns.IndexOf(selColumn);
                int x = gridRect.X + (colIndex == 0 ? 0 : _columns.Take(colIndex).Sum(c => c.Width)) - _xOffset;
                int y = columnsheaderPanelRect.Y + (columnsheaderPanelRect.Height - _selectAllCheckBox.CheckBoxSize) / 2;

                x = Math.Max(columnsheaderPanelRect.Left, Math.Min(x, columnsheaderPanelRect.Right - _selectAllCheckBox.CheckBoxSize));
                y = Math.Max(columnsheaderPanelRect.Top, Math.Min(y, columnsheaderPanelRect.Bottom - _selectAllCheckBox.CheckBoxSize));

                _selectAllCheckBox.Location = new Point(x + (_selectionColumnWidth - _selectAllCheckBox.CheckBoxSize) / 2, y);
                _selectAllCheckBox.Size = new Size(_selectAllCheckBox.CheckBoxSize, _selectAllCheckBox.CheckBoxSize);
                _selectAllCheckBox.Visible = true;
                _selectAllCheckBox.BringToFront();
            }
            else
            {
                _selectAllCheckBox.Visible = false;
            }
        }

        #endregion
    }
}
