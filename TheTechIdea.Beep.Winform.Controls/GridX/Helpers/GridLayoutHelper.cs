using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridLayoutHelper
    {
        private readonly BeepGridPro _grid;

        // Scrollbar dimensions — single source of truth (GridScrollBarsHelper reads these
        // from the layout helper rather than duplicating the constants).
        public int ScrollbarWidth  { get; internal set; } = BeepLayoutMetrics.GridScrollbarW;
        public int ScrollbarHeight { get; internal set; } = BeepLayoutMetrics.GridScrollbarW;

        private int _rowHeight = BeepLayoutMetrics.GridRowHeight;
        private int _columnHeaderHeight = BeepLayoutMetrics.GridHeaderHeight;

        public int RowHeight
        {
            get => _rowHeight;
            set
            {
                _rowHeight = value;
                RowHeightIsExplicit = true;
            }
        }

        public bool RowHeightIsExplicit { get; private set; }

        public int ColumnHeaderHeight
        {
            get => _columnHeaderHeight;
            set
            {
                _columnHeaderHeight = value;
                ColumnHeaderHeightIsExplicit = true;
            }
        }

        public bool ColumnHeaderHeightIsExplicit { get; private set; }

        internal void ApplyCalculatedRowHeight(int value)
        {
            if (!RowHeightIsExplicit) _rowHeight = value;
        }

        internal void ApplyCalculatedColumnHeaderHeight(int value)
        {
            if (!ColumnHeaderHeightIsExplicit) _columnHeaderHeight = value;
        }

        /// <summary>
        /// Allows a caller that explicitly wants font/DPI-derived heights to
        /// discard the current explicit values before the next recalculation.
        /// </summary>
        internal void ResetExplicitHeights()
        {
            RowHeightIsExplicit = false;
            ColumnHeaderHeightIsExplicit = false;
        }
        public bool ShowColumnHeaders { get; set; } = true;
        public Rectangle HeaderRect { get; private set; }
        public Rectangle TopFilterRect { get; private set; } = Rectangle.Empty;
        public Rectangle ToolbarRect { get; private set; } = Rectangle.Empty;
        public int ToolbarHeight { get; set; } = BeepLayoutMetrics.DataNavigator.Height;
        public Rectangle ColumnsHeaderRect { get; private set; }
        public Rectangle FooterRect { get; private set; } = Rectangle.Empty;
        public Rectangle NavigatorRect { get; private set; } = Rectangle.Empty;
        public Rectangle RowsRect { get; private set; }
        public Rectangle RowsRectWithScrollbars { get; private set; }
        public Rectangle SelectAllCheckRect { get; private set; } = Rectangle.Empty;
        public Rectangle[] HeaderCellRects { get; private set; } = System.Array.Empty<Rectangle>();
        public int NavigatorHeight { get; set; } = BeepLayoutMetrics.DataNavigator.Height;
        public int TopFilterHeight { get; set; } = BeepLayoutMetrics.FieldStandard.Height;
        public bool ShowTopFilterPanel { get; set; } = true;
        public bool ShowActiveFilterBar { get; set; } = true;
        public int ActiveFilterBarHeight { get; set; } = BeepLayoutMetrics.GridActiveFilterBarHeight;
        public Rectangle ActiveFilterBarRect { get; private set; } = Rectangle.Empty;

        public int CheckBoxColumnWidth { get; set; } = (int)(BeepLayoutMetrics.MinTouchTarget * 0.68f); // ~30 px at 96 DPI

        public bool IsCalculating { get; private set; }
        public int HoveredHeaderColumnIndex { get; internal set; }

        private int _lastScrollOffset = -1;
        private int _lastRowCount = -1;

        internal GridRowMetrics RowMetrics { get; }

        public GridLayoutHelper(BeepGridPro grid)
        {
            _grid = grid;
            RowMetrics = new GridRowMetrics(grid);
        }

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
            _grid.ScrollBars?.MarkDirty();
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
                    ToolbarRect = Rectangle.Empty;
                    ActiveFilterBarRect = Rectangle.Empty;
                    RowsRect = Rectangle.Empty;
                    NavigatorRect = Rectangle.Empty;
                    FooterRect = Rectangle.Empty;
                    HeaderCellRects = System.Array.Empty<Rectangle>();
                    return;
                }
                
                // Pre-calculate if scrollbars will be needed to reserve space
                // Check if vertical scrollbar is needed (rough estimate)
                int totalRowHeight = 0;
                if (_grid.EnableVirtualization && _grid.VirtualDataSource != null)
                {
                    long vcount = _grid.RowVirtualizer.TotalRowCount;
                    totalRowHeight = (int)System.Math.Min(vcount * RowHeight, int.MaxValue);
                }
                else if (_grid.Data?.Rows != null)
                {
                    totalRowHeight = RowMetrics.TotalHeight;
                }
                
                // Check if horizontal scrollbar is needed
                int totalColumnWidth = _grid.Data?.Columns?.Where(c => c.Visible).Sum(c => Math.Max(BeepLayoutMetrics.GridColumnMinW, c.Width)) ?? 0;
                
                int availableHeight = r.Height - (ShowColumnHeaders ? ColumnHeaderHeight : 0);
                if (_grid.ShowNavigator && NavigatorHeight > 0)
                {
                    availableHeight -= NavigatorHeight;
                }
                
                bool needsVerticalScrollbar = totalRowHeight > availableHeight;
                bool needsHorizontalScrollbar = totalColumnWidth > r.Width;
                
                // Reserve space for scrollbars
                int scrollbarWidth = needsVerticalScrollbar ? ScrollbarWidth : 0;
                int scrollbarHeight = needsHorizontalScrollbar ? ScrollbarHeight : 0;
                
                int top = r.Top;

                if (_grid.ShowToolbar && ToolbarHeight > 0)
                {
                    int tbHeight = Math.Min(ToolbarHeight, Math.Max(0, r.Height));
                    if (tbHeight > 0)
                    {
                        int tbWidth = Math.Max(0, r.Width - (needsVerticalScrollbar ? ScrollbarWidth : 0));
                        ToolbarRect = new Rectangle(r.Left, top, tbWidth, tbHeight);
                        top += tbHeight;
                    }
                    else
                    {
                        ToolbarRect = Rectangle.Empty;
                    }
                }
                else
                {
                    ToolbarRect = Rectangle.Empty;
                }

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

                // Active filter chips row (DevExpress-style). It appears only while
                // a filter with at least one condition is active — either an
                // ActiveFilter criterion or a per-column SortFilter condition — so
                // the band is reserved lazily rather than always occupying a row.
                bool hasActiveCriteria = (_grid.ActiveFilter?.Criteria?.Count ?? 0) > 0;
                bool showActiveFilterBar = ShowActiveFilterBar
                    && (hasActiveCriteria || _grid.SortFilter.HasActiveFilters);

                int activeFilterBarHeight = showActiveFilterBar
                    ? Math.Min(ActiveFilterBarHeight, Math.Max(0, r.Height))
                    : 0;

                if (activeFilterBarHeight > 0)
                {
                    int filterBarWidth = Math.Max(0, r.Width - scrollbarWidth);
                    ActiveFilterBarRect = new Rectangle(r.Left, top, filterBarWidth, activeFilterBarHeight);
                    top += activeFilterBarHeight;
                }
                else
                {
                    ActiveFilterBarRect = Rectangle.Empty;
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

                // Pinned aggregate footer sits directly above the navigator.
                int footerHeight = 0;
                if (_grid.Footer.Visible && _grid.Footer.Height > 0)
                {
                    int availableForFooter = Math.Max(0, r.Height - (top - r.Top) - bottomReserve);
                    footerHeight = Math.Min(_grid.Footer.Height, availableForFooter);
                    if (footerHeight > 0)
                    {
                        int footerWidth = Math.Max(0, r.Width - scrollbarWidth);
                        FooterRect = new Rectangle(r.Left, r.Bottom - bottomReserve - footerHeight,
                            footerWidth, footerHeight);
                        bottomReserve += footerHeight;
                    }
                    else
                    {
                        FooterRect = Rectangle.Empty;
                    }
                }
                else
                {
                    FooterRect = Rectangle.Empty;
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

                // Update column virtualizer window after layout is known
                if (_grid.EnableColumnVirtualization)
                {
                    _grid.ColumnVirtualizer.UpdateWindow(
                        _grid.Scroll?.HorizontalOffset ?? 0,
                        RowsRect.Width);
                }

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
            
            int stickyWidth = stickyColumns.Sum(c => Math.Max(BeepLayoutMetrics.GridColumnMinW, c.Width));
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
                
                int w = Math.Max(BeepLayoutMetrics.GridColumnMinW, col.Width);
                
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
                
                int w = Math.Max(BeepLayoutMetrics.GridColumnMinW, col.Width);
                
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
            bool virtualMode = _grid.EnableVirtualization && _grid.VirtualDataSource != null;

            int currentPixelOffset;
            int actualStartRow;
            int startRowPixelOffset;

            if (!virtualMode)
            {
                actualStartRow = RowMetrics.FirstVisibleRow(startPixelOffset);
                currentPixelOffset = RowMetrics.TopOfRow(actualStartRow);
                startRowPixelOffset = currentPixelOffset;
            }
            else
            {
                currentPixelOffset = (int)(_grid.RowVirtualizer.WindowStart * RowHeight);
                actualStartRow = 0;
                startRowPixelOffset = 0;

                for (int i = 0; i < _grid.Data.Rows.Count; i++)
                {
                    var row = _grid.Data.Rows[i];
                    if (!row.IsVisible) continue;
                    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;

                    if (currentPixelOffset + rowHeight > startPixelOffset)
                    {
                        actualStartRow = i;
                        startRowPixelOffset = currentPixelOffset;
                        break;
                    }
                    currentPixelOffset += rowHeight;
                }
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
                    
                    // Add more rows if they fit (accounting for group headers)
                    for (int i = actualStartRow + 1; i < _grid.Data.Rows.Count && usedHeight < availableHeight; i++)
                    {
                        var row = _grid.Data.Rows[i];
                        if (!row.IsVisible) continue;
                        int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;

                        if (!(_grid.EnableVirtualization && _grid.VirtualDataSource != null))
                        {
                            int newHeaders = (_grid.GroupEngine?.GetHeaderCountBeforeRow(i + 1) ?? 0) - (_grid.GroupEngine?.GetHeaderCountBeforeRow(i) ?? 0);
                            if (newHeaders > 0)
                            {
                                int hh = newHeaders * (_grid.GroupEngine?.GetHeaderHeight() ?? 0);
                                if (usedHeight + hh > availableHeight)
                                    break;
                                usedHeight += hh;
                            }
                        }
                        
                        if (usedHeight + rowHeight > availableHeight)
                            break;
                            
                        usedHeight += rowHeight;
                        endRow = i;
                    }
                }
            }

            // Y position accounting for partial row visibility and group headers
            int y = RowsRect.Top - (startPixelOffset - startRowPixelOffset);

            int[] xmap = new int[_grid.Data.Columns.Count];
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
                xmap[i] = HeaderCellRects[i].X;

            for (int r = actualStartRow; r <= endRow && y < RowsRect.Bottom; r++)
            {
                if (r >= _grid.Data.Rows.Count) break;

                // Inject group header height for any groups whose first row is at this index
                if (!(_grid.EnableVirtualization && _grid.VirtualDataSource != null))
                {
                    int newHeaders = (_grid.GroupEngine?.GetHeaderCountBeforeRow(r + 1) ?? 0) - (_grid.GroupEngine?.GetHeaderCountBeforeRow(r) ?? 0);
                    if (newHeaders > 0)
                        y += newHeaders * (_grid.GroupEngine?.GetHeaderHeight() ?? 0);
                }
                
                var row = _grid.Data.Rows[r];
                if (!row.IsVisible) continue;
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
                    
                    int w = Math.Max(BeepLayoutMetrics.GridColumnMinW, col.Width);
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
            
            // Get DPI scale factor for consistent scaling
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(_grid);
            
            // Calculate column header height from the cached painter (font-aware and DPI-aware).
            // Previously this allocated a GridColumnHeadersPainterHelper on every layout pass just
            // to read one number.
            ApplyCalculatedColumnHeaderHeight(_grid.HeaderPainter?.CalculateHeaderHeight(_grid) ?? _columnHeaderHeight);

            // Always calculate filter panel height from painter (DPI-aware)
            // This ensures proper height even when toggling panel visibility
            TopFilterHeight = FilterPanelPainterFactory.GetRecommendedHeight(_grid);

            // Calculate row height from data font (font-aware and DPI-aware) using safe height method
            if (_grid.Font != null)
            {
                int baseFontHeight = FontManagement.FontListHelper.GetFontHeightSafe(_grid.Font, _grid);
                int cellPadding = DpiScalingHelper.ScaleValue(BeepLayoutMetrics.GridCellPadding, dpiScale);
                int spacing = DpiScalingHelper.ScaleValue(BeepLayoutMetrics.GridCellPadding, dpiScale);
                ApplyCalculatedRowHeight(System.Math.Max(DpiScalingHelper.ScaleValue(BeepLayoutMetrics.GridRowMinHeight, dpiScale),
                    baseFontHeight + (cellPadding * 2) + spacing));
            }

            // Calculate navigator height if enabled (font-aware and DPI-aware)
            if (_grid.ShowNavigator && _grid.NavigatorPainter != null)
            {
                NavigatorHeight = _grid.NavigatorPainter.GetRecommendedNavigatorHeight();
            }
            else if (!_grid.ShowNavigator)
            {
                NavigatorHeight = 0;
            }
        }
    }
}
