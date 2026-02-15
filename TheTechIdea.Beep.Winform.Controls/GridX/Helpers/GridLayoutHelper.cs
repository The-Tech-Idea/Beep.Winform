using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridLayoutHelper
    {
        private readonly BeepGridPro _grid;
        
        // Scrollbar dimensions (must match GridScrollBarsHelper constants)
        private const int SCROLLBAR_WIDTH = 15;
        private const int SCROLLBAR_HEIGHT = 15;
        
        public int RowHeight { get; set; } = 25;
        public int ColumnHeaderHeight { get; set; } = 28;
        public bool ShowColumnHeaders { get; set; } = true;
        public Rectangle HeaderRect { get; private set; }
        public Rectangle TopFilterRect { get; private set; } = Rectangle.Empty;
        public Rectangle ColumnsHeaderRect { get; private set; }
        public Rectangle FooterRect { get; private set; }
        public Rectangle NavigatorRect { get; private set; } = Rectangle.Empty;
        public Rectangle RowsRect { get; private set; }
        public Rectangle RowsRectWithScrollbars { get; private set; } // Full area including scrollbar space
        public Rectangle SelectAllCheckRect { get; private set; } = Rectangle.Empty;
        public Rectangle[] HeaderCellRects { get; private set; } = System.Array.Empty<Rectangle>();
        public int NavigatorHeight { get; set; } = 36;
        public int TopFilterHeight { get; set; } = 34;
        public bool ShowTopFilterPanel { get; set; } = true;
        
        public int CheckBoxColumnWidth { get; set; } = 30; // Add checkbox column width like BeepSimpleGrid
     
        public bool IsCalculating { get; private set; }
        public int HoveredHeaderColumnIndex { get; internal set; }

        private int _lastScrollOffset = -1;
        private int _lastRowCount = -1;

        public GridLayoutHelper(BeepGridPro grid) { _grid = grid; }

        public void EnsureCalculated()
        {
            // Recalculate if layout is empty OR if scroll position/row count changed
            bool needsRecalc = (HeaderRect == Rectangle.Empty && RowsRect == Rectangle.Empty);
            
            if (!needsRecalc)
            {
                // Check if scroll position changed
                int currentOffset = _grid.Scroll?.VerticalOffset ?? 0;
                int currentRowCount = _grid.Data?.Rows?.Count ?? 0;
                
                if (currentOffset != _lastScrollOffset || currentRowCount != _lastRowCount)
                {
                    needsRecalc = true;
                }
            }
            
            if (needsRecalc)
            {
                Recalculate();
            }
        }

        public void Recalculate()
        {
            if (IsCalculating) return;
            IsCalculating = true;
            _grid.UpdateDrawingRect();
            Rectangle r = Rectangle.Inflate(_grid.DrawingRect, -_grid.BorderThickness, -_grid.BorderThickness); // Account for border thickness
            try
            {
                // FIRST: Calculate heights from painters (font-aware) before computing layout regions
                RecalculateHeightsFromPainters();
                
                // Validate the drawing rectangle
                if (r.Width <= 0 || r.Height <= 0)
                {
                    HeaderRect = Rectangle.Empty;
                    TopFilterRect = Rectangle.Empty;
                    RowsRect = Rectangle.Empty;
                    NavigatorRect = Rectangle.Empty;
                    HeaderCellRects = System.Array.Empty<Rectangle>();
                    return;
                }
                
                // Pre-calculate if scrollbars will be needed to reserve space
                // Check if vertical scrollbar is needed (rough estimate)
                int totalRowHeight = 0;
                if (_grid.Data?.Rows != null)
                {
                    for (int i = 0; i < _grid.Data.Rows.Count; i++)
                    {
                        var row = _grid.Data.Rows[i];
                        totalRowHeight += row.Height > 0 ? row.Height : RowHeight;
                    }
                }
                
                // Check if horizontal scrollbar is needed
                int totalColumnWidth = _grid.Data?.Columns?.Where(c => c.Visible).Sum(c => Math.Max(20, c.Width)) ?? 0;
                
                int availableHeight = r.Height - (ShowColumnHeaders ? ColumnHeaderHeight : 0);
                if (_grid.ShowNavigator && NavigatorHeight > 0)
                {
                    availableHeight -= NavigatorHeight;
                }
                
                bool needsVerticalScrollbar = totalRowHeight > availableHeight;
                bool needsHorizontalScrollbar = totalColumnWidth > r.Width;
                
                // Reserve space for scrollbars
                int scrollbarWidth = needsVerticalScrollbar ? SCROLLBAR_WIDTH : 0;
                int scrollbarHeight = needsHorizontalScrollbar ? SCROLLBAR_HEIGHT : 0;
                
                int top = r.Top;
                int topFilterHeight = (ShowTopFilterPanel && ShowColumnHeaders)
                    ? Math.Min(TopFilterHeight, Math.Max(0, r.Height))
                    : 0;

                if (topFilterHeight > 0)
                {
                    int filterWidth = Math.Max(0, r.Width - scrollbarWidth);
                    TopFilterRect = new Rectangle(r.Left, top, filterWidth, topFilterHeight);
                    top += topFilterHeight;
                }
                else
                {
                    TopFilterRect = Rectangle.Empty;
                }

                if (ShowColumnHeaders && r.Height > ColumnHeaderHeight)
                {
                    // Header should not extend into scrollbar area
                    int headerWidth = Math.Max(0, r.Width - scrollbarWidth);
                    HeaderRect = new Rectangle(r.Left, top, headerWidth, ColumnHeaderHeight);
                    top += ColumnHeaderHeight;
                }
                else
                {
                    HeaderRect = Rectangle.Empty;
                }

                int bottomReserve = 0;
                if (_grid.ShowNavigator && NavigatorHeight > 0)
                {
                    // Clamp navigator height to available space
                    int actualNavHeight = Math.Min(NavigatorHeight, Math.Max(0, r.Height - (ShowColumnHeaders ? ColumnHeaderHeight : 0)));
                    if (actualNavHeight > 0)
                    {
                        bottomReserve = actualNavHeight;
                        NavigatorRect = new Rectangle(r.Left, r.Bottom - actualNavHeight, r.Width, actualNavHeight);
                    }
                    else
                    {
                        NavigatorRect = Rectangle.Empty;
                    }
                }
                else
                {
                    NavigatorRect = Rectangle.Empty;
                }

                // Calculate RowsRect with scrollbar space reserved
                int rowsHeight = Math.Max(0, r.Height - (top - r.Top) - bottomReserve - scrollbarHeight);
                int rowsWidth = Math.Max(0, r.Width - scrollbarWidth);
                
                // Store the full area (with scrollbar space) for scrollbar positioning
                int fullRowsHeight = Math.Max(0, r.Height - (top - r.Top) - bottomReserve);
                int fullRowsWidth = r.Width;
                RowsRectWithScrollbars = new Rectangle(r.Left, top, fullRowsWidth, fullRowsHeight);
                
                // Store the reduced area (without scrollbar space) for grid content
                if (rowsHeight > 0 && rowsWidth > 0)
                {
                    RowsRect = new Rectangle(r.Left, top, rowsWidth, rowsHeight);
                }
                else
                {
                    RowsRect = Rectangle.Empty;
                }

                LayoutCells();
                
                // Position filter panel embedded controls
                PositionFilterPanelControls();
                
                // Track scroll position and row count for change detection
                _lastScrollOffset = _grid.Scroll?.VerticalOffset ?? 0;
                _lastRowCount = _grid.Data?.Rows?.Count ?? 0;
            }
            finally
            {
                IsCalculating = false;
            }
        }

        private void LayoutCells()
        {
            if (_grid.Data.Columns.Count == 0)
            {
                HeaderCellRects = System.Array.Empty<Rectangle>();
                return;
            }

            //// Ensure system columns are present and visible/invisible as needed exactly like BeepSimpleGrid
            //var selColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            //if (_grid.ShowCheckBox && selColumn != null)
            //{
            //    selColumn.Visible = true;
            //}
            //else if (selColumn != null)
            //{
            //    selColumn.Visible = false;
            //}

            // Calculate sticky columns width exactly like BeepSimpleGrid
            // Group columns by sticky/unpinned and order by DisplayOrder
            var stickyColumns = _grid.Data.Columns
                .Where(c => c.Visible && c.Sticked)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
            var unpinnedColumns = _grid.Data.Columns
                .Where(c => c.Visible && !c.Sticked)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
            
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, RowsRect.Width); // Prevent overflow

            int unpinnedStartX = RowsRect.Left + stickyWidth - _grid.Scroll.HorizontalOffset;

            HeaderCellRects = new Rectangle[_grid.Data.Columns.Count];
            int px = RowsRect.Left;
            int ux = unpinnedStartX;
            
            // Calculate sticky columns first (in display order)
            foreach (var col in stickyColumns)
            {
                int i = _grid.Data.Columns.IndexOf(col);
                if (i < 0) continue;
                
                int w = Math.Max(20, col.Width);
                
                if (w > 0 && HeaderRect.Height > 0 && px >= 0)
                {
                    int maxWidth = Math.Max(1, RowsRect.Right - px);
                    w = Math.Min(w, maxWidth);
                    HeaderCellRects[i] = new Rectangle(px, HeaderRect.Top, w, HeaderRect.Height);
                }
                else
                {
                    HeaderCellRects[i] = Rectangle.Empty;
                }
                
                px += w;
            }
            
            // Calculate unpinned columns (in display order)
            foreach (var col in unpinnedColumns)
            {
                int i = _grid.Data.Columns.IndexOf(col);
                if (i < 0) continue;
                
                int w = Math.Max(20, col.Width);
                
                if (w > 0 && HeaderRect.Height > 0 && ux >= 0)
                {
                    int maxWidth = Math.Max(1, RowsRect.Right - ux);
                    w = Math.Min(w, maxWidth);
                    HeaderCellRects[i] = new Rectangle(ux, HeaderRect.Top, w, HeaderRect.Height);
                }
                else
                {
                    HeaderCellRects[i] = Rectangle.Empty;
                }
                
                ux += w;
            }
            
            // Set empty rectangles for invisible columns
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                if (!_grid.Data.Columns[i].Visible)
                {
                    HeaderCellRects[i] = Rectangle.Empty;
                }
            }

            // Calculate SelectAllCheckRect exactly like BeepSimpleGrid
            if (_grid.ShowCheckBox && ShowColumnHeaders && _grid.SelectionCheckBoxColumn != null)
            {
                int size = CheckBoxColumnWidth - 8;
                SelectAllCheckRect = new Rectangle(RowsRect.Left + 4, HeaderRect.Top + (HeaderRect.Height - size) / 2, size, size);
            }
            else
            {
                SelectAllCheckRect = Rectangle.Empty;
            }

            if (_grid.Data.Rows.Count == 0)
                return;

            // Calculate visible row range with variable row heights
            int availableHeight = RowsRect.Height;
            int startPixelOffset = _grid.Scroll.VerticalOffset;
            int startRow = _grid.Scroll.FirstVisibleRowIndex;
            
            // FIX: Find the actual visible rows based on pixel offset and available height
            int currentPixelOffset = 0;
            int actualStartRow = 0;
            
            // Find the first visible row based on pixel offset
            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (currentPixelOffset + rowHeight > startPixelOffset)
                {
                    actualStartRow = i;
                    break;
                }
                currentPixelOffset += rowHeight;
            }
            
            // Calculate how many rows fit in the available height
            int endRow = actualStartRow;
            int usedHeight = 0;
            
            // Account for partial visibility of first row
            if (actualStartRow < _grid.Data.Rows.Count)
            {
                var firstRow = _grid.Data.Rows[actualStartRow];
                int firstRowHeight = firstRow.Height > 0 ? firstRow.Height : _grid.RowHeight;
                int firstRowVisibleHeight = firstRowHeight - (startPixelOffset - currentPixelOffset);
                
                if (firstRowVisibleHeight > 0)
                {
                    usedHeight += firstRowVisibleHeight;
                    endRow = actualStartRow;
                    
                    // Add more rows if they fit
                    for (int i = actualStartRow + 1; i < _grid.Data.Rows.Count && usedHeight < availableHeight; i++)
                    {
                        var row = _grid.Data.Rows[i];
                        int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                        
                        if (usedHeight + rowHeight > availableHeight)
                            break;
                            
                        usedHeight += rowHeight;
                        endRow = i;
                    }
                }
            }

            // Calculate the Y position accounting for partial row visibility
            int startRowPixelOffset = 0;
            for (int i = 0; i < actualStartRow; i++)
            {
                var row = _grid.Data.Rows[i];
                startRowPixelOffset += row.Height > 0 ? row.Height : RowHeight;
            }
            
            int y = RowsRect.Top - (startPixelOffset - startRowPixelOffset);

            int[] xmap = new int[_grid.Data.Columns.Count];
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
                xmap[i] = HeaderCellRects[i].X;

            for (int r = actualStartRow; r <= endRow && y < RowsRect.Bottom; r++)
            {
                if (r >= _grid.Data.Rows.Count) break;
                
                var row = _grid.Data.Rows[r];
                int h = row.Height > 0 ? row.Height : RowHeight; // Use individual row height
                
                // Calculate checkbox rect exactly like BeepSimpleGrid
                if (_grid.ShowCheckBox && _grid.SelectionCheckBoxColumn != null && _grid.SelectionCheckBoxColumn.Visible)
                {
                    int cbSize = Math.Min(h - 6, CheckBoxColumnWidth - 6);
                    row.RowCheckRect = new Rectangle(RowsRect.Left + 4, y + (h - cbSize) / 2, cbSize, cbSize);
                }
                else
                {
                    row.RowCheckRect = Rectangle.Empty;
                }

                for (int c = 0; c < row.Cells.Count && c < _grid.Data.Columns.Count; c++)
                {
                    var cell = row.Cells[c];
                    var col = _grid.Data.Columns[c];
                    if (!col.Visible) continue;
                    
                    int w = Math.Max(20, col.Width);
                    cell.Rect = new Rectangle(xmap[c], y, w, h);
                }
                y += h; // Use individual row height for positioning
            }
        }

        /// <summary>
        /// Recalculate heights from painters (font-aware) before computing layout regions
        /// </summary>
        private void RecalculateHeightsFromPainters()
        {
            // Ensure fonts are initialized before calculating heights
            FontManagement.FontListHelper.EnsureFontsLoaded();
            
            // Calculate column header height from painter (font-aware)
            var headerPainter = new GridColumnHeadersPainterHelper(_grid);
            ColumnHeaderHeight = headerPainter.CalculateHeaderHeight();

            // Calculate row height from data font (font-aware) using safe height method
            if (_grid.Font != null)
            {
                int baseFontHeight = FontManagement.FontListHelper.GetFontHeightSafe(_grid.Font, _grid);
                int cellPadding = 2; // Default padding
                RowHeight = baseFontHeight + (cellPadding * 2) + 4; // 4px for comfortable spacing
            }

            // Calculate navigator height if enabled (font-aware)
            if (_grid.ShowNavigator && _grid.NavigatorPainter != null)
            {
                NavigatorHeight = _grid.NavigatorPainter.GetRecommendedNavigatorHeight();
            }
            else if (!_grid.ShowNavigator)
            {
                NavigatorHeight = 0;
            }
        }
        
        /// <summary>
        /// Positions the filter panel embedded controls (search box and column combo)
        /// </summary>
        private void PositionFilterPanelControls()
        {
            if (_grid.FilterPanelSearchBox == null || _grid.FilterPanelColumnCombo == null)
            {
                return;
            }
            
            // Always keep controls visible when ShowTopFilterPanel is enabled
            if (!ShowTopFilterPanel)
            {
                // Hide controls if filter panel is not shown
                _grid.FilterPanelSearchBox.Visible = false;
                _grid.FilterPanelColumnCombo.Visible = false;
                return;
            }
            
            // If TopFilterRect is not calculated yet, use default positioning
            if (TopFilterRect == Rectangle.Empty)
            {
                // Keep controls visible with default positioning until layout is ready
                return;
            }
            
            // Suspend layout to prevent flicker during repositioning
            _grid.SuspendLayout();
            try
            {
                // Position controls in the right portion of the filter panel
                // Leave left side for filter chips/icons painted by the painter
                
                int controlHeight = 24;
                int comboWidth = 120;
                int searchMinWidth = 200;
                int padding = 6;
                int rightMargin = 12;
                
                // Calculate available space on the right side
                int availableWidth = TopFilterRect.Width - rightMargin;
                
                // Start from right edge
                int rightEdge = TopFilterRect.Right - rightMargin;
                
                // Position search box (right side)
                int searchWidth = Math.Max(searchMinWidth, availableWidth - comboWidth - padding);
                int searchX = rightEdge - searchWidth;
                int searchY = TopFilterRect.Top + (TopFilterRect.Height - controlHeight) / 2;
                
                _grid.FilterPanelSearchBox.Bounds = new Rectangle(searchX, searchY, searchWidth, controlHeight);
                _grid.FilterPanelSearchBox.Visible = true;
                
                // Position combo box (to the left of search box)
                int comboX = searchX - padding - comboWidth;
                int comboY = TopFilterRect.Top + (TopFilterRect.Height - controlHeight) / 2;
                
                if (comboWidth > 0 && comboX >= TopFilterRect.Left)
                {
                    _grid.FilterPanelColumnCombo.Bounds = new Rectangle(comboX, comboY, comboWidth, controlHeight);
                    _grid.FilterPanelColumnCombo.Visible = true;
                }
                else
                {
                    _grid.FilterPanelColumnCombo.Visible = false;
                }
                
                // Populate column combo if empty
                if (_grid.FilterPanelColumnCombo.ListItems.Count == 0 && _grid.Data?.Columns != null)
                {
                    _grid.PopulateFilterPanelColumnCombo();
                }
            }
            finally
            {
                // Resume layout
                _grid.ResumeLayout(false); // false = don't force immediate layout
            }
        }
    }
}
