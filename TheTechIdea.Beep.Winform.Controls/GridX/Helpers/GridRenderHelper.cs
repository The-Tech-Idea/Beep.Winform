using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
 
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Helpers; // Svgs
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using ContentAlignment = System.Drawing.ContentAlignment;
using navigationStyle = TheTechIdea.Beep.Winform.Controls.GridX.Painters.navigationStyle;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridRenderHelper
    {
        private readonly BeepGridPro _grid;
        private BeepCheckBoxBool _rowCheck;

        // Cache drawers per column (like BeepSimpleGrid)
        private readonly Dictionary<string, IBeepUIComponent> _columnDrawerCache = new();

        // // Navigator buttons (owner-drawn)
        // private BeepButton _btnFirst;
        // private BeepButton _btnPrev;
        // private BeepButton _btnNext;
        // private BeepButton _btnLast;
        // private BeepButton _btnInsert;
        // private BeepButton _btnDelete;
        // private BeepButton _btnSave;
        // private BeepButton _btnCancel;
        // private BeepButton _btnQuery;
        // private BeepButton _btnFilter;
        // private BeepButton _btnPrint;

        // // Enhanced navigation controls for professional paging
        // private BeepButton _btnPageFirst;
        // private BeepButton _btnPagePrev;
        // private BeepButton _btnPageNext;
        // private BeepButton _btnPageLast;
        // private BeepComboBox _cmbPageSize;
        // private BeepTextBox _txtPageJump;
        // private BeepButton _btnGoToPage;
        // private BeepLabel _lblPageInfo;
        // // Public property to access page info label for hit testing
        // public BeepLabel PageInfoLabel => _lblPageInfo;

        // Store filter icon rectangles for hit-testing
        private readonly Dictionary<int, Rectangle> _headerFilterIconRects = new();
        public Dictionary<int, Rectangle> HeaderFilterIconRects => _headerFilterIconRects;

        // Store sort icon rectangles for hit-testing
        private readonly Dictionary<int, Rectangle> _headerSortIconRects = new();
        public Dictionary<int, Rectangle> HeaderSortIconRects => _headerSortIconRects;

        // Store top filter panel cell/icon rectangles for hit-testing
        private readonly Dictionary<int, Rectangle> _topFilterCellRects = new();
        public Dictionary<int, Rectangle> TopFilterCellRects => _topFilterCellRects;

        private readonly Dictionary<int, Rectangle> _topFilterClearIconRects = new();
        public Dictionary<int, Rectangle> TopFilterClearIconRects => _topFilterClearIconRects;

        private IGridFilterPanelPainter? _filterPanelPainter;
        private navigationStyle _filterPanelPainterStyle = navigationStyle.None;

        public GridRenderHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        // Grid Style properties
        public bool ShowGridLines { get; set; } = true;
        public bool ShowRowStripes { get; set; } = false;
        public System.Drawing.Drawing2D.DashStyle GridLineStyle { get; set; } = System.Drawing.Drawing2D.DashStyle.Solid;
        public bool UseElevation { get; set; } = false;
        public bool CardStyle { get; set; } = false;

        // Advanced header styling properties
        public bool UseHeaderGradient { get; set; } = false;
        public bool ShowSortIndicators { get; set; } = true;
        public HeaderIconVisibility SortIconVisibility { get; set; } = HeaderIconVisibility.Always;
        public HeaderIconVisibility FilterIconVisibility { get; set; } = HeaderIconVisibility.Always;
        public bool UseHeaderHoverEffects { get; set; } = true;
        public bool UseBoldHeaderText { get; set; } = false;
        public int HeaderCellPadding { get; set; } = 2;

        // Focus styling
        public bool UseDedicatedFocusedRowStyle { get; set; } = true;
        public Color FocusedRowBackColor { get; set; } = Color.Empty;
        public bool ShowFocusedCellFill { get; set; } = true;
        public Color FocusedCellFillColor { get; set; } = Color.Empty;
        public int FocusedCellFillOpacity { get; set; } = 36;
        public bool ShowFocusedCellBorder { get; set; } = true;
        public Color FocusedCellBorderColor { get; set; } = Color.Empty;
        public float FocusedCellBorderWidth { get; set; } = 2f;

        internal IBeepTheme Theme => _grid.Theme != null ? BeepThemesManager.GetTheme(_grid.Theme) : BeepThemesManager.GetDefaultTheme();

        #region Visible Row Calculations

        /// <summary>
        /// Gets the number of rows that can fit in the current viewport (RowsRect height / RowHeight)
        /// This is the "page size" for pagination calculations
        /// </summary>
        public int GetVisibleRowCapacity()
        {
            if (_grid?.Layout == null) return 10; // Default fallback
            
            var rowsRect = _grid.Layout.RowsRect;
            if (rowsRect.Height <= 0) return 10;
            
            int rowHeight = _grid.RowHeight;
            if (rowHeight <= 0) rowHeight = 24; // Safety fallback
            
            return System.Math.Max(1, rowsRect.Height / rowHeight);
        }

        /// <summary>
        /// Gets the index of the first fully or partially visible row
        /// </summary>
        public int GetFirstVisibleRowIndex()
        {
            return _grid?.Scroll?.FirstVisibleRowIndex ?? 0;
        }

        /// <summary>
        /// Gets the index of the last visible row (may be partially visible)
        /// </summary>
        public int GetLastVisibleRowIndex()
        {
            if (_grid?.Data?.Rows == null || _grid.Layout == null) return 0;
            
            int firstVisible = GetFirstVisibleRowIndex();
            int capacity = GetVisibleRowCapacity();
            int lastPossible = System.Math.Min(firstVisible + capacity - 1, _grid.Data.Rows.Count - 1);
            
            return System.Math.Max(0, lastPossible);
        }

        /// <summary>
        /// Gets the actual count of visible rows (handles variable row heights)
        /// </summary>
        public int GetActualVisibleRowCount()
        {
            if (_grid?.Data?.Rows == null || _grid.Layout == null) return 0;
            
            var rowsRect = _grid.Layout.RowsRect;
            int firstVisible = GetFirstVisibleRowIndex();
            int verticalOffset = _grid?.Scroll?.VerticalOffset ?? 0;
            
            return GetVisibleRowCount(_grid.Data.Rows, rowsRect.Height, firstVisible, verticalOffset);
        }

        #endregion

        #region Pagination Calculations

        /// <summary>
        /// Gets the current page number based on the selected row index
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <returns>Current page number (1-based)</returns>
        public int GetCurrentPage(BeepGridPro grid)
        {
            if (grid?.Selection == null || grid.Data?.Rows == null) return 1;
            if (grid.Data.Rows.Count == 0) return 1;
            
            int pageSize = GetVisibleRowCapacity();
            if (pageSize <= 0) pageSize = 10; // Safety fallback
            
            int currentRow = System.Math.Max(0, System.Math.Min(grid.Selection.RowIndex, grid.Data.Rows.Count - 1));
            return (currentRow / pageSize) + 1;
        }

        /// <summary>
        /// Gets the total number of pages based on total records and visible row capacity
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <returns>Total number of pages</returns>
        public int GetTotalPages(BeepGridPro grid)
        {
            if (grid?.Data?.Rows == null) return 1;
            
            int totalRecords = grid.Data.Rows.Count;
            if (totalRecords == 0) return 1;
            
            int pageSize = GetVisibleRowCapacity();
            if (pageSize <= 0) pageSize = 10; // Safety fallback
            
            return System.Math.Max(1, (int)System.Math.Ceiling(totalRecords / (double)pageSize));
        }

        /// <summary>
        /// Gets the total number of pages for a specific record count
        /// </summary>
        /// <param name="totalRecords">Total number of records</param>
        /// <returns>Total number of pages</returns>
        public int GetTotalPages(int totalRecords)
        {
            if (totalRecords <= 0) return 1;
            
            int pageSize = GetVisibleRowCapacity();
            if (pageSize <= 0) pageSize = 10; // Safety fallback
            
            return System.Math.Max(1, (int)System.Math.Ceiling(totalRecords / (double)pageSize));
        }

        /// <summary>
        /// Gets the range of page numbers to display in pagination controls
        /// Centers the current page within the visible range
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <param name="maxVisiblePages">Maximum number of page buttons to show</param>
        /// <returns>Tuple of (startPage, endPage) both inclusive and 1-based</returns>
        public (int startPage, int endPage) GetVisiblePageRange(BeepGridPro grid, int maxVisiblePages = 5)
        {
            int currentPage = GetCurrentPage(grid);
            int totalPages = GetTotalPages(grid);
            
            return CalculatePageRange(currentPage, totalPages, maxVisiblePages);
        }

        /// <summary>
        /// Calculates the range of page numbers to display
        /// </summary>
        /// <param name="currentPage">Current page (1-based)</param>
        /// <param name="totalPages">Total number of pages</param>
        /// <param name="maxVisiblePages">Maximum number of page buttons to show</param>
        /// <returns>Tuple of (startPage, endPage) both inclusive and 1-based</returns>
        public (int startPage, int endPage) CalculatePageRange(int currentPage, int totalPages, int maxVisiblePages = 5)
        {
            if (totalPages <= maxVisiblePages)
            {
                // Show all pages if total is less than max
                return (1, totalPages);
            }
            
            // Center current page in visible range
            int halfVisible = maxVisiblePages / 2;
            int startPage = System.Math.Max(1, currentPage - halfVisible);
            int endPage = System.Math.Min(totalPages, startPage + maxVisiblePages - 1);
            
            // Adjust start if we're near the end
            if (endPage - startPage < maxVisiblePages - 1)
            {
                startPage = System.Math.Max(1, endPage - maxVisiblePages + 1);
            }
            
            return (startPage, endPage);
        }

        /// <summary>
        /// Gets the first row index (0-based) for a specific page
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <returns>Row index (0-based) for the first row of the page</returns>
        public int GetPageStartRow(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;
            
            int pageSize = GetVisibleRowCapacity();
            if (pageSize <= 0) pageSize = 10; // Safety fallback
            
            return (pageNumber - 1) * pageSize;
        }

        /// <summary>
        /// Gets the last row index (0-based) for a specific page
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="totalRecords">Total number of records</param>
        /// <returns>Row index (0-based) for the last row of the page</returns>
        public int GetPageEndRow(int pageNumber, int totalRecords)
        {
            int startRow = GetPageStartRow(pageNumber);
            int pageSize = GetVisibleRowCapacity();
            if (pageSize <= 0) pageSize = 10; // Safety fallback
            
            int endRow = startRow + pageSize - 1;
            return System.Math.Min(endRow, totalRecords - 1);
        }

        /// <summary>
        /// Gets the row range for a specific page
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="totalRecords">Total number of records</param>
        /// <returns>Tuple of (startRow, endRow) both 0-based and inclusive</returns>
        public (int startRow, int endRow) GetPageRowRange(int pageNumber, int totalRecords)
        {
            int startRow = GetPageStartRow(pageNumber);
            int endRow = GetPageEndRow(pageNumber, totalRecords);
            return (startRow, endRow);
        }

        /// <summary>
        /// Navigates the grid to a specific page by selecting the first row of that page
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <param name="pageNumber">Page number to navigate to (1-based)</param>
        public void GoToPage(BeepGridPro grid, int pageNumber)
        {
            if (grid?.Data?.Rows == null || grid.Selection == null) return;
            
            int totalPages = GetTotalPages(grid);
            
            // Clamp page number to valid range
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
            
            // Calculate target row index
            int targetRow = GetPageStartRow(pageNumber);
            targetRow = System.Math.Max(0, System.Math.Min(targetRow, grid.Data.Rows.Count - 1));
            
            // Select the first row of the page
            int currentColumn = System.Math.Max(0, grid.Selection.ColumnIndex);
            grid.Selection.SelectCell(targetRow, currentColumn);
            
            // Ensure the row is visible by scrolling
            grid.Scroll?.SetVerticalIndex(targetRow);
            
            grid.SafeInvalidate();
        }

        /// <summary>
        /// Gets pagination information as a formatted string
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <param name="format">Format string: {0}=currentPage, {1}=totalPages, {2}=currentRow, {3}=totalRecords</param>
        /// <returns>Formatted pagination string</returns>
        public string GetPaginationInfo(BeepGridPro grid, string format = "Page {0} of {1} (Row {2} of {3})")
        {
            if (grid?.Data?.Rows == null) return "No data";
            
            int currentPage = GetCurrentPage(grid);
            int totalPages = GetTotalPages(grid);
            int currentRow = grid.Selection?.RowIndex ?? 0;
            int totalRecords = grid.Data.Rows.Count;
            
            return string.Format(format, currentPage, totalPages, currentRow + 1, totalRecords);
        }

        /// <summary>
        /// Gets simple pagination info showing current position
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <returns>String like "27 of 213"</returns>
        public string GetSimplePaginationInfo(BeepGridPro grid)
        {
            if (grid?.Data?.Rows == null) return "0 of 0";
            
            int currentRow = grid.Selection?.RowIndex ?? 0;
            int totalRecords = grid.Data.Rows.Count;
            
            return $"{currentRow + 1} of {totalRecords}";
        }

        /// <summary>
        /// Gets page-based pagination info
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <returns>String like "Page 3 of 18"</returns>
        public string GetPagePaginationInfo(BeepGridPro grid)
        {
            int currentPage = GetCurrentPage(grid);
            int totalPages = GetTotalPages(grid);
            
            return $"Page {currentPage} of {totalPages}";
        }

        /// <summary>
        /// Checks if a specific page number is valid
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <param name="pageNumber">Page number to check (1-based)</param>
        /// <returns>True if the page number is valid</returns>
        public bool IsValidPage(BeepGridPro grid, int pageNumber)
        {
            int totalPages = GetTotalPages(grid);
            return pageNumber >= 1 && pageNumber <= totalPages;
        }

        /// <summary>
        /// Checks if there is a previous page
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <returns>True if previous page exists</returns>
        public bool HasPreviousPage(BeepGridPro grid)
        {
            return GetCurrentPage(grid) > 1;
        }

        /// <summary>
        /// Checks if there is a next page
        /// </summary>
        /// <param name="grid">The grid instance</param>
        /// <returns>True if next page exists</returns>
        public bool HasNextPage(BeepGridPro grid)
        {
            int currentPage = GetCurrentPage(grid);
            int totalPages = GetTotalPages(grid);
            return currentPage < totalPages;
        }

        /// <summary>
        /// Navigates to the first page
        /// </summary>
        public void GoToFirstPage(BeepGridPro grid)
        {
            GoToPage(grid, 1);
        }

        /// <summary>
        /// Navigates to the last page
        /// </summary>
        public void GoToLastPage(BeepGridPro grid)
        {
            int totalPages = GetTotalPages(grid);
            GoToPage(grid, totalPages);
        }

        /// <summary>
        /// Navigates to the previous page
        /// </summary>
        public void GoToPreviousPage(BeepGridPro grid)
        {
            int currentPage = GetCurrentPage(grid);
            if (currentPage > 1)
            {
                GoToPage(grid, currentPage - 1);
            }
        }

        /// <summary>
        /// Navigates to the next page
        /// </summary>
        public void GoToNextPage(BeepGridPro grid)
        {
            int currentPage = GetCurrentPage(grid);
            int totalPages = GetTotalPages(grid);
            if (currentPage < totalPages)
            {
                GoToPage(grid, currentPage + 1);
            }
        }

        #endregion

        // Create or get cached drawer for a given column
        private IBeepUIComponent GetDrawerForColumn(BeepColumnConfig col)
        {
            if (col == null) return null;
            string key = col.ColumnName ?? col.ColumnCaption ?? col.GuidID ?? col.GetHashCode().ToString();
            if (_columnDrawerCache.TryGetValue(key, out var cached) && cached != null)
            {
                return cached;
            }

            IBeepUIComponent drawer = col.CellEditor switch
            {
                BeepColumnType.CheckBoxBool => new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.CheckBoxChar => new BeepCheckBoxChar { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.CheckBoxString => new BeepCheckBoxString { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.ComboBox => new BeepComboBox { IsChild = true, GridMode = true },
                BeepColumnType.DateTime => new BeepDatePicker { IsChild = true, GridMode = true },
                BeepColumnType.Image => new BeepImage { IsChild = true },
                BeepColumnType.Button => new BeepButton { IsChild = true, IsFrameless = true },
                BeepColumnType.ProgressBar => new BeepProgressBar { IsChild = true },
                BeepColumnType.NumericUpDown => new BeepNumericUpDown { IsChild = true, GridMode = true },
                BeepColumnType.Radio => new BeepRadioGroup { IsChild = true, GridMode = true },
                BeepColumnType.ListBox => new BeepListBox { IsChild = true, GridMode = true },
                BeepColumnType.ListOfValue => new BeepListofValuesBox { IsChild = true, GridMode = true },
                BeepColumnType.Text => new BeepTextBox { IsChild = true, GridMode = true, IsFrameless = true, ShowAllBorders = false },
                _ => new BeepTextBox { IsChild = true, GridMode = true, IsFrameless = true, ShowAllBorders = false }
            };

            drawer.Theme = _grid.Theme;
            _columnDrawerCache[key] = drawer;
            return drawer;
        }

        public void Draw(Graphics g)
        {
           /// Console.WriteLine("GridRenderHelper.Draw called.");
            // Validate graphics object and grid state
            if (g == null || _grid == null || _grid.Layout == null)
            {
              //  Console.WriteLine("Draw skipped: Invalid graphics or grid state.");

                return;
            }
              
          //  Console.WriteLine("Drawing grid...");
            var rowsRect = _grid.Layout.RowsRect;
            if (rowsRect.Width <= 0 || rowsRect.Height <= 0)
            {
            //    Console.WriteLine("Draw skipped: Invalid rows rectangle.");
                return;
            }
               
          //  Console.WriteLine($"RowsRect: {rowsRect}");
            // Draw background (exclude child control areas to prevent painting over editors)
            using (var brush = new SolidBrush(Theme?.GridBackColor ?? SystemColors.Window))
            {
                // Save the current clip region
                var originalClip = g.Clip;
                try
                {
                    // Exclude child controls from the clip region to prevent painting over them
                    if (_grid.Controls.Count > 0)
                    {
                        var region = new Region(rowsRect);
                        foreach (Control child in _grid.Controls)
                        {
                            if (child.Visible && child.Bounds.IntersectsWith(rowsRect))
                            {
                                region.Exclude(child.Bounds);
                            }
                        }
                        g.Clip = region;
                        region.Dispose();
                    }
                    
                    g.FillRectangle(brush, rowsRect);
                }
                finally
                {
                    // Restore the original clip region
                    g.Clip = originalClip;
                    originalClip?.Dispose();
                }
            }
          //  Console.WriteLine("Background drawn.");
            // Draw column headers
            if (_grid.ShowColumnHeaders)
            {
                if (_grid.ShowTopFilterPanel)
                {
                    try
                    {
                        DrawTopFilterPanel(g);
                    }
                    catch (Exception)
                    {
                        // Silently ignore top filter panel draw issues
                    }
                }
                else
                {
                    _topFilterCellRects.Clear();
                    _topFilterClearIconRects.Clear();
                }

                try
                {
           //         Console.WriteLine("Drawing column headers...");
                    DrawColumnHeaders(g);
                }
                catch (Exception)
                {
             //       Console.WriteLine("Error drawing column headers.");
                    // Silently handle header drawing errors to prevent crashes
                }
            }

            // Draw data rows
            try
            {
                DrawRows(g);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GridRenderHelper.DrawRows error: {ex.Message}");
                // Log the error but don't crash the control
            }

            // Draw navigator if enabled
            if (_grid.ShowNavigator)
            {
                try
                {
                //    if (_grid.NavigatorPainter.UsePainterNavigation)
                //{
                //    // Use the painter to draw the navigator
                //    _grid.NavigatorPainter.PaintNavigation(g, _grid.Layout.NavigatorRect, _grid.NavigationStyle, _grid.Theme);
                //}
                    DrawNavigatorArea(g);
                }
            catch (Exception)
            {
                // Silently handle navigator drawing errors
            }
            }
            
            // Draw selection indicators
            try
            {
               // Console.WriteLine("Drawing selection indicators...");
               DrawSelectionIndicators(g);
            }
            catch (Exception)
            {
                // Silently handle selection drawing errors
            }

            // Draw column reorder drag feedback
            if (_grid.AllowColumnReorder && _grid.ColumnReorder.IsDragging)
            {
                try
                {
                    _grid.ColumnReorder.DrawDragFeedback(g);
                }
                catch (Exception)
                {
                    // Silently handle drag feedback errors
                }
            }
        }

        private void DrawColumnHeaders(Graphics g)
        {
            var headerRect = _grid.Layout.HeaderRect;
            if (headerRect.Height <= 0 || headerRect.Width <= 0) return;

            // Rebuild icon hit areas every paint so hit-testing matches current layout.
            _headerFilterIconRects.Clear();
            _headerSortIconRects.Clear();

            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Draw bottom border line if grid lines are enabled
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawLine(pen, headerRect.Left, headerRect.Bottom, headerRect.Right, headerRect.Bottom);
                }
            }

            // Optional: draw select-all checkbox visual in header if enabled
            var selColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_grid.ShowCheckBox && selColumn != null && _grid.Layout.SelectAllCheckRect != Rectangle.Empty)
            {
                bool allSelected = _grid.Rows.Count > 0 && _grid.Rows.All(r => r.IsSelected);
                var chk = new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme, CurrentValue = allSelected };
                chk.Draw(g, _grid.Layout.SelectAllCheckRect);
            }

            var stickyColumns = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, headerRect.Width);

            // Define regions
            Rectangle stickyRegion = new Rectangle(headerRect.Left, headerRect.Top, stickyWidth, headerRect.Height);
            Rectangle scrollingRegion = new Rectangle(headerRect.Left + stickyWidth, headerRect.Top, Math.Max(0, headerRect.Width - stickyWidth), headerRect.Height);

            // Draw non-sticky (scrolling) headers in scrolling clip first
            var state1 = g.Save();
            g.SetClip(scrollingRegion);
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                if (!col.Visible || col.Sticked) continue;
                if (i < _grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = _grid.Layout.HeaderCellRects[i];
                    if (cellRect.Width > 0 && cellRect.Height > 0)
                        DrawHeaderCell(g, col, cellRect, i);
                }
            }
            g.Restore(state1);

            // Draw sticky headers on top within sticky clip
            var state2 = g.Save();
            g.SetClip(stickyRegion);
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                if (!col.Visible || !col.Sticked) continue;
                if (i < _grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = _grid.Layout.HeaderCellRects[i];
                    if (cellRect.Width > 0 && cellRect.Height > 0)
                        DrawHeaderCell(g, col, cellRect, i);
                }
            }
            g.Restore(state2);

            // Vertical separator after sticky section
            if (stickyWidth > 0 && ShowGridLines)
            {
                using var pen2 = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);
                pen2.DashStyle = GridLineStyle;
                g.DrawLine(pen2, headerRect.Left + stickyWidth, headerRect.Top, headerRect.Left + stickyWidth, headerRect.Bottom);
            }
        }

        private void DrawTopFilterPanel(Graphics g)
        {
            var panelRect = _grid.Layout.TopFilterRect;
            _topFilterCellRects.Clear();
            _topFilterClearIconRects.Clear();

            if (panelRect.Width <= 0 || panelRect.Height <= 0)
            {
                return;
            }

            var style = _grid.NavigationStyle;
            if (_filterPanelPainter == null || _filterPanelPainterStyle != style)
            {
                _filterPanelPainter = FilterPanelPainterFactory.CreatePainter(style);
                _filterPanelPainterStyle = style;
            }

            if (_filterPanelPainter == null)
            {
                return;
            }

            _filterPanelPainter.PaintFilterPanel(
                g,
                panelRect,
                _grid,
                Theme,
                _topFilterCellRects,
                _topFilterClearIconRects);
        }

        // Draws a simple filter icon (funnel shape)
        private void DrawFilterIcon(Graphics g, Rectangle rect, bool active)
        {
            Color iconColor = active ? Color.DodgerBlue : (Theme?.GridHeaderForeColor ?? SystemColors.ControlText);
            
            using (Pen pen = new Pen(active ? iconColor : Color.FromArgb(180, iconColor), active ? 1.6f : 1.25f))
            using (Brush brush = new SolidBrush(active ? iconColor : Color.FromArgb(120, iconColor)))
            {
                // Draw a more compact filter funnel shape
                int padding = 1;
                int funnelWidth = rect.Width - (padding * 2);
                int funnelHeight = rect.Height - (padding * 2);

                Point[] funnel = {
                    new Point(rect.X + padding, rect.Y + padding),
                    new Point(rect.Right - padding, rect.Y + padding),
                    new Point(rect.X + rect.Width / 2 + 2, rect.Y + funnelHeight / 2),
                    new Point(rect.X + rect.Width / 2 + 2, rect.Bottom - padding),
                    new Point(rect.X + rect.Width / 2 - 2, rect.Bottom - padding),
                    new Point(rect.X + rect.Width / 2 - 2, rect.Y + funnelHeight / 2)
                };

                if (active)
                {
                    g.FillPolygon(brush, funnel);
                }
                else
                {
                    g.FillPolygon(brush, funnel);
                    g.DrawPolygon(pen, funnel);
                }

                // Add a small circle or dot to indicate active filter
                if (active)
                {
                    using (Brush dotBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
                    {
                        int dotSize = 2;
                        g.FillEllipse(dotBrush,
                            rect.X + rect.Width / 2 - dotSize / 2,
                            rect.Y + rect.Height / 2 - dotSize / 2,
                            dotSize, dotSize);
                    }
                }
            }
        }

        // Draws sort indicator arrows (up/down)
        private void DrawSortIndicator(Graphics g, Rectangle rect, TheTechIdea.Beep.Vis.Modules.SortDirection sortDirection)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var color = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;

            using (Pen pen = new Pen(color, 1.5f))
            using (Brush brush = new SolidBrush(color))
            {
                int centerX = rect.X + rect.Width / 2;
                int centerY = rect.Y + rect.Height / 2;
                int arrowSize = Math.Min(rect.Width, rect.Height) / 3; // Smaller arrows

                if (sortDirection == TheTechIdea.Beep.Vis.Modules.SortDirection.Ascending)
                {
                    // Draw up arrow (smaller and more refined)
                    Point[] upArrowAsc = {
                        new Point(centerX, rect.Y + 2),
                        new Point(centerX - arrowSize, centerY + 1),
                        new Point(centerX + arrowSize, centerY + 1)
                    };
                    g.FillPolygon(brush, upArrowAsc);
                }
                else if (sortDirection == TheTechIdea.Beep.Vis.Modules.SortDirection.Descending)
                {
                    // Draw down arrow (smaller and more refined)
                    Point[] downArrowDesc = {
                        new Point(centerX, rect.Bottom - 2),
                        new Point(centerX - arrowSize, centerY - 1),
                        new Point(centerX + arrowSize, centerY - 1)
                    };
                    g.FillPolygon(brush, downArrowDesc);
                }
                else if (sortDirection == TheTechIdea.Beep.Vis.Modules.SortDirection.None)
                {
                    // Draw both arrows (inactive, lighter)
                    using (Pen lightPen = new Pen(Color.FromArgb(100, color), 1))
                    {
                        Point[] upArrowNone = {
                            new Point(centerX, rect.Y + 2),
                            new Point(centerX - arrowSize + 1, centerY),
                            new Point(centerX + arrowSize - 1, centerY)
                        };
                        Point[] downArrowNone = {
                            new Point(centerX, rect.Bottom - 2),
                            new Point(centerX - arrowSize + 1, centerY),
                            new Point(centerX + arrowSize - 1, centerY)
                        };
                        g.DrawPolygon(lightPen, upArrowNone);
                        g.DrawPolygon(lightPen, downArrowNone);
                    }
                }
            }
        }

        private void DrawHeaderCell(Graphics g, BeepColumnConfig column, Rectangle cellRect, int columnIndex)
        {
            // Validate inputs first
            if (g == null || column == null || cellRect.Width <= 0 || cellRect.Height <= 0)
                return;

            // Determine if this header cell is being hovered
            bool isHovered = UseHeaderHoverEffects && _grid.Layout.HoveredHeaderColumnIndex == columnIndex;

            // Background - with gradient support
            if (UseHeaderGradient)
            {
                // Create subtle gradient from theme color to slightly lighter
                var baseColor = Theme?.GridHeaderBackColor ?? SystemColors.Control;
                var lightColor = Color.FromArgb(
                    Math.Min(255, baseColor.R + 20),
                    Math.Min(255, baseColor.G + 20),
                    Math.Min(255, baseColor.B + 20)
                );

                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    cellRect,
                    isHovered ? lightColor : baseColor,
                    isHovered ? baseColor : lightColor,
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }
            else
            {
                // Solid background with hover effect
                var backColor = Theme?.GridHeaderBackColor ?? SystemColors.Control;
                if (isHovered && UseHeaderHoverEffects)
                {
                    // Slightly lighter color for hover
                    backColor = Color.FromArgb(
                        Math.Min(255, backColor.R + 15),
                        Math.Min(255, backColor.G + 15),
                        Math.Min(255, backColor.B + 15)
                    );
                }

                using (var brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }

            // Add elevation effect to header if enabled
            if (UseElevation)
            {
                using (var shadowPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    // Draw subtle shadow at bottom and right
                    g.DrawLine(shadowPen, cellRect.Left + 1, cellRect.Bottom, cellRect.Right - 1, cellRect.Bottom);
                    g.DrawLine(shadowPen, cellRect.Right, cellRect.Top + 1, cellRect.Right, cellRect.Bottom - 1);
                }
            }

            // Text - with bold support and custom padding
            var textColor = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;

            // Create font with bold support - safely resolve with fallback
            var baseFont = GetSafeHeaderFont();
            var font = UseBoldHeaderText ?
                new Font(baseFont.FontFamily, baseFont.Size, FontStyle.Bold) :
                baseFont;

            bool isSystemColumn = column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID;
            bool hasSortArea = ShowSortIndicators && SortIconVisibility != HeaderIconVisibility.Hidden && !isSystemColumn && column.AllowSort;
            bool hasFilterArea = FilterIconVisibility != HeaderIconVisibility.Hidden && !isSystemColumn;

            // Calculate areas with custom padding
            int sortIconSize = hasSortArea ? Math.Min(cellRect.Height - HeaderCellPadding * 2, 14) : 0;
            int filterIconSize = hasFilterArea ? Math.Min(cellRect.Height - HeaderCellPadding * 2, 14) : 0;

            // Calculate positions from right to left
            int rightX = cellRect.Right - HeaderCellPadding;

            // Filter icon area (if shown)
            Rectangle filterIconRect = Rectangle.Empty;
            if (hasFilterArea && filterIconSize > 0)
            {
                filterIconRect = new Rectangle(
                    rightX - filterIconSize,
                    cellRect.Top + HeaderCellPadding,
                    filterIconSize,
                    filterIconSize);
                rightX -= filterIconSize + HeaderCellPadding;
            }

            // Sort icon area
            Rectangle sortIconRect = Rectangle.Empty;
            if (hasSortArea && sortIconSize > 0)
            {
                sortIconRect = new Rectangle(
                    rightX - sortIconSize,
                    cellRect.Top + HeaderCellPadding,
                    sortIconSize,
                    sortIconSize);
                rightX -= sortIconSize + HeaderCellPadding;
            }

            // Text area
            var textRect = new Rectangle(
                cellRect.X + HeaderCellPadding,
                cellRect.Y + HeaderCellPadding,
                Math.Max(1, rightX - cellRect.X - HeaderCellPadding),
                Math.Max(1, cellRect.Height - HeaderCellPadding * 2)
            );

            // Draw text honoring column header alignment
            if (!string.IsNullOrEmpty(text))
            {
                var headerAlign = column.HeaderTextAlignment;
                var flags = GetTextFormatFlagsForAlignment(headerAlign, true);
                TextRenderer.DrawText(g, text, font, textRect, textColor, flags);
            }

            bool showSortIcon = hasSortArea && !sortIconRect.IsEmpty &&
                (SortIconVisibility == HeaderIconVisibility.Always ||
                (SortIconVisibility == HeaderIconVisibility.HoverOnly && (isHovered || (column.IsSorted && column.ShowSortIcon))));

            // Draw sort indicator (including neutral state to show interactive affordance)
            if (showSortIcon)
            {
                var direction = (column.IsSorted && column.ShowSortIcon)
                    ? column.SortDirection
                    : TheTechIdea.Beep.Vis.Modules.SortDirection.None;
                DrawSortIndicator(g, sortIconRect, direction);
                _headerSortIconRects[columnIndex] = sortIconRect;
            }
            else
            {
                _headerSortIconRects.Remove(columnIndex);
            }

            bool showFilterIcon = hasFilterArea && !filterIconRect.IsEmpty &&
                (FilterIconVisibility == HeaderIconVisibility.Always ||
                (FilterIconVisibility == HeaderIconVisibility.HoverOnly && (isHovered || column.IsFiltered)));

            if (showFilterIcon)
            {
                DrawFilterIcon(g, filterIconRect, column.IsFiltered);
                _headerFilterIconRects[columnIndex] = filterIconRect;
            }
            else
            {
                _headerFilterIconRects.Remove(columnIndex);
            }

            // Border - only draw if grid lines are enabled
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawRectangle(pen, cellRect);
                }
            }

            // Clean up font if we created a bold version
            if (UseBoldHeaderText)
            {
                font.Dispose();
            }
        }

        private void DrawRows(Graphics g)
        {
            var rowsRect = _grid.Layout.RowsRect;
            
            // Pre-cache theme colors to avoid repeated lookups
            var gridBackColor = Theme?.GridBackColor ?? SystemColors.Window;
            var gridForeColor = Theme?.GridForeColor ?? SystemColors.WindowText;
            var gridLineColor = Theme?.GridLineColor ?? SystemColors.ControlDark;
            var selectedBackColor = Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor;
            var hoverBackColor = Theme?.GridRowHoverBackColor == Color.Empty ? SystemColors.ControlLight : Theme.GridRowHoverBackColor;
            var altRowBackColor = Theme?.AltRowBackColor ?? Color.FromArgb(250, 250, 250);
            var focusColor = ResolveThemeFocusColor();
            var focusedRowBackColor = ResolveFocusedRowBackColor(hoverBackColor, focusColor);
         
            // Pre-create reusable pens and brushes
            using var gridLinePen = new Pen(gridLineColor);
            gridLinePen.DashStyle = GridLineStyle;
            
            using var shadowPen = UseElevation ? new Pen(Color.FromArgb(30, 0, 0, 0), 1) : null;
            using var cardPen = CardStyle ? new Pen(Color.FromArgb(40, gridLineColor), 1) : null;

            // Calculate sticky regions (don't modify column.Visible during paint - causes flicker!)
            var stickyColumns = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, rowsRect.Width);

            // Calculate Y offset
            int currentY = rowsRect.Top;
            int firstVisibleRowIndex = _grid.Scroll.FirstVisibleRowIndex;

            int totalRowsHeight = 0;
           
            for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            
            int pixelOffset = _grid.Scroll.VerticalOffset;
            currentY = rowsRect.Top - (pixelOffset - totalRowsHeight);

            // Calculate visible rows
            int visibleRowStart = firstVisibleRowIndex;
            int visibleRowEnd = Math.Min(_grid.Data.Rows.Count - 1, 
                visibleRowStart + GetVisibleRowCount(_grid.Data.Rows, rowsRect.Height, visibleRowStart, pixelOffset));

            // Define regions
            Rectangle stickyRegion = new Rectangle(rowsRect.Left, rowsRect.Top, stickyWidth, rowsRect.Height);
            Rectangle scrollingRegion = new Rectangle(rowsRect.Left + stickyWidth, rowsRect.Top, 
                                                     Math.Max(0, rowsRect.Width - stickyWidth), rowsRect.Height);

            // Draw scrolling columns WITH CLIPPING to prevent overflow outside grid bounds
            var scrollCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                                               .Where(x => x.Col.Visible && !x.Col.Sticked)
                                               .ToList();
            
            // Save graphics state and set clipping for scrolling region
            var scrollState = g.Save();
            g.SetClip(scrollingRegion);
            
            int drawY = currentY;
            for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
            {
                var row = _grid.Data.Rows[r];
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                {
                    bool isActiveRow = (_grid.Selection?.RowIndex ?? -1) == r;
                    bool isSelectedRow = row.IsSelected;

                    int x = scrollingRegion.Left - _grid.Scroll.HorizontalOffset;
                    
                    foreach (var sc in scrollCols)
                    {
                        int colW = Math.Max(20, sc.Col.Width);
                        if (x + colW > scrollingRegion.Left && x < scrollingRegion.Right)
                        {
                            if (sc.Index < row.Cells.Count)
                            {
                                var cell = row.Cells[sc.Index];
                                var rect = new Rectangle(x, drawY, colW, rowHeight);
                                cell.Rect = rect;

                                // Determine colors (using cached theme colors)
                                Color back = isSelectedRow ? selectedBackColor : 
                                           isActiveRow ? (UseDedicatedFocusedRowStyle ? focusedRowBackColor : hoverBackColor) :
                                           ShowRowStripes && r % 2 == 1 ? altRowBackColor :
                                           sc.Col.HasCustomBackColor && sc.Col.UseCustomColors ? sc.Col.ColumnBackColor : gridBackColor;

                                Color fore = sc.Col.HasCustomForeColor && sc.Col.UseCustomColors ? sc.Col.ColumnForeColor : gridForeColor;

                                using (var bg = new SolidBrush(back)) 
                                    g.FillRectangle(bg, rect);

                                if (shadowPen != null && !isSelectedRow && !isActiveRow)
                                {
                                    g.DrawLine(shadowPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
                                    g.DrawLine(shadowPen, rect.Right, rect.Top + 1, rect.Right, rect.Bottom - 1);
                                }

                                if (cardPen != null && !isSelectedRow && !isActiveRow)
                                {
                                    g.DrawRectangle(cardPen, rect);
                                }

                                if (sc.Col.IsSelectionCheckBox && _grid.ShowCheckBox)
                                {
                                    _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                                    _rowCheck.CurrentValue = row.IsSelected;
                                    _rowCheck.Draw(g, rect);
                                }
                                else
                                {
                                    DrawCellContent(g, sc.Col, cell, rect, fore, back);
                                }

                                if (ShowGridLines)
                                {
                                    g.DrawRectangle(gridLinePen, rect);
                                }
                            }
                        }
                        x += colW;
                        if (x > scrollingRegion.Right) break;
                    }
                }
                drawY += rowHeight;
            }
            
            // Restore graphics state after scrolling columns
            g.Restore(scrollState);

            // Draw sticky columns WITH CLIPPING on top to prevent overflow
            var stickyCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                                               .Where(x => x.Col.Visible && x.Col.Sticked)
                                               .ToList();
            
            // Save graphics state and set clipping for sticky region
            var stickyState = g.Save();
            g.SetClip(stickyRegion);
            
            int startX = rowsRect.Left;
            foreach (var st in stickyCols)
            {
                int colW = Math.Max(20, st.Col.Width);
                drawY = currentY;
                
                for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
                {
                    var row = _grid.Data.Rows[r];
                    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                    
                    if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                    {
                        if (st.Index < row.Cells.Count)
                        {
                            bool isActiveRow = (_grid.Selection?.RowIndex ?? -1) == r;
                            bool isSelectedRow = row.IsSelected;

                            var cell = row.Cells[st.Index];
                            var rect = new Rectangle(startX, drawY, colW, rowHeight);
                            cell.Rect = rect;

                            // Determine colors (using cached theme colors)
                            Color back = isSelectedRow ? selectedBackColor : 
                                       isActiveRow ? (UseDedicatedFocusedRowStyle ? focusedRowBackColor : hoverBackColor) :
                                       ShowRowStripes && r % 2 == 1 ? altRowBackColor :
                                       st.Col.HasCustomBackColor && st.Col.UseCustomColors ? st.Col.ColumnBackColor : gridBackColor;

                            Color fore = st.Col.HasCustomForeColor && st.Col.UseCustomColors ? st.Col.ColumnForeColor : gridForeColor;

                            using (var bg = new SolidBrush(back)) 
                                g.FillRectangle(bg, rect);

                            if (shadowPen != null && !isSelectedRow && !isActiveRow)
                            {
                                g.DrawLine(shadowPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
                                g.DrawLine(shadowPen, rect.Right, rect.Top + 1, rect.Right, rect.Bottom - 1);
                            }

                            if (cardPen != null && !isSelectedRow && !isActiveRow)
                            {
                                g.DrawRectangle(cardPen, rect);
                            }

                            if (st.Col.IsSelectionCheckBox && _grid.ShowCheckBox)
                            {
                                _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                                _rowCheck.CurrentValue = row.IsSelected;
                                _rowCheck.Draw(g, rect);
                            }
                            else if (st.Col.IsRowNumColumn)
                            {
                                var font = GetSafeCellFont();
                                TextRenderer.DrawText(g, cell.CellValue?.ToString() ?? string.Empty, font, rect, fore,
                                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                            }
                            else
                            {
                                DrawCellContent(g, st.Col, cell, rect, fore, back);
                            }

                            if (ShowGridLines)
                            {
                                g.DrawRectangle(gridLinePen, rect);
                            }
                        }
                    }
                    drawY += rowHeight;
                }
                startX += colW;
            }
            
            // Restore graphics state after sticky columns
            g.Restore(stickyState);
        }

        private Color ResolveThemeFocusColor()
        {
            if (Theme?.FocusIndicatorColor != Color.Empty)
                return Theme.FocusIndicatorColor;

            return _grid?.FocusIndicatorColor ?? SystemColors.Highlight;
        }

        private Color ResolveFocusedRowBackColor(Color hoverBackColor, Color focusColor)
        {
            if (FocusedRowBackColor != Color.Empty)
                return FocusedRowBackColor;

            return BlendColors(hoverBackColor, focusColor, 0.22f);
        }

        private static Color BlendColors(Color baseColor, Color blendColor, float blendFactor)
        {
            blendFactor = Math.Max(0f, Math.Min(1f, blendFactor));
            int r = (int)(baseColor.R + ((blendColor.R - baseColor.R) * blendFactor));
            int g = (int)(baseColor.G + ((blendColor.G - baseColor.G) * blendFactor));
            int b = (int)(baseColor.B + ((blendColor.B - baseColor.B) * blendFactor));
            return Color.FromArgb(255, r, g, b);
        }

        // Cached fallback font for cell rendering - avoids repeated allocation
        private static Font _cellFallbackFont;
        private static Font CellFallbackFont => _cellFallbackFont ??= SystemFonts.DefaultFont;

        /// <summary>
        /// Safely resolves the grid cell font from the current theme with fallback.
        /// </summary>
        private Font GetSafeCellFont()
        {
            try
            {
                var theme = _grid?._currentTheme ?? BeepThemesManager.CurrentTheme;
                if (theme?.GridCellFont != null)
                {
                    var font = BeepThemesManager.ToFont(theme.GridCellFont);
                    if (font != null) return font;
                }
            }
            catch { /* font creation failed - use fallback */ }
            return CellFallbackFont;
        }

        /// <summary>
        /// Safely resolves the grid header font from the current theme with fallback.
        /// </summary>
        private Font GetSafeHeaderFont()
        {
            try
            {
                var theme = _grid?._currentTheme ?? BeepThemesManager.CurrentTheme;
                if (theme?.GridHeaderFont != null)
                {
                    var font = BeepThemesManager.ToFont(theme.GridHeaderFont);
                    if (font != null) return font;
                }
            }
            catch { /* font creation failed - use fallback */ }
            return CellFallbackFont;
        }

        private void DrawCellContent(Graphics g, BeepColumnConfig column, BeepCellConfig cell, Rectangle rect, Color foreColor, Color backColor)
        {
            if (g == null || column == null || cell == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            try
            {
                if (column.IsSelectionCheckBox)
                {
                    _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                    _rowCheck.CurrentValue = (bool)(cell.CellValue ?? false);
                    _rowCheck.Draw(g, rect);
                    return;
                }

                // Optimization: For simple text/numeric columns, use direct text rendering 
                // instead of the heavy control drawer. This mimics BeepSimpleGrid behavior and avoids 
                // issues where the control drawer might not be in a valid invalid state for painting.
                if (column.CellEditor == BeepColumnType.Text || 
                    column.CellEditor == BeepColumnType.Link || 
                    column.CellEditor == BeepColumnType.NumericUpDown ||
                    column.CellEditor == BeepColumnType.DateTime )
                {
                     DrawCellAsText(g, column, cell, rect, foreColor);
                     return;
                }

                // For simple text display (Text columns and numeric display), use direct TextRenderer
                // This is faster and more reliable than going through a full control drawer
                var drawer = GetDrawerForColumn(column);
                if (drawer == null)
                {
                    DrawCellAsText(g, column, cell, rect, foreColor);
                    return;
                }

                // Try drawing with the control drawer
                try
                {
                    drawer.Theme = _grid.Theme;
                    Control control = drawer as Control;
                    if (control != null)
                    {
                        control.BackColor = backColor;
                        control.ForeColor = foreColor;
                        control.Bounds = rect;
                    }

                    // Populate list-based controls BEFORE setting value, so they can resolve SelectedItem
                    if (drawer is BeepComboBox combo)
                    {
                        var items = GetFilteredItems(column, cell);
                        combo.ListItems = new BindingList<SimpleItem>(items);
                    }
                    else if (drawer is BeepListBox listBox)
                    {
                        var items = GetFilteredItems(column, cell);
                        listBox.ListItems = new BindingList<SimpleItem>(items);
                    }
                    else if (drawer is BeepListofValuesBox lov)
                    {
                        var items = GetFilteredItems(column, cell);
                        lov.ListItems = new List<SimpleItem>(items);
                    }

                    if (drawer is IBeepUIComponent ic)
                    {
                        try { ic.SetValue(cell.CellValue); } catch { }
                    }

                    // Draw via component to match BeepSimpleGrid look
                    drawer.Draw(g, rect);
                }
                catch
                {
                    // If the control drawer fails, fall back to plain text rendering
                    DrawCellAsText(g, column, cell, rect, foreColor);
                }
            }
            catch (Exception ex)
            {
                // Last resort: log and skip this cell rather than killing all rows
                System.Diagnostics.Debug.WriteLine($"DrawCellContent error for column '{column.ColumnName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Draws cell value as plain text using TextRenderer - the safest fallback rendering path.
        /// </summary>
        private void DrawCellAsText(Graphics g, BeepColumnConfig column, BeepCellConfig cell, Rectangle rect, Color foreColor)
        {
            string text = cell.CellValue?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(text)) return;

            var font = GetSafeCellFont();
            var textRect = new Rectangle(rect.X + 2, rect.Y + 1, Math.Max(1, rect.Width - 4), Math.Max(1, rect.Height - 2));
            var flags = GetTextFormatFlagsForAlignment(column.CellTextAlignment, true);
            TextRenderer.DrawText(g, text, font, textRect, foreColor, flags);
        }

        private List<SimpleItem> GetFilteredItems(BeepColumnConfig column, BeepCellConfig cell)
        {
            var baseItems = column?.Items ?? new List<SimpleItem>();
            if (baseItems == null || baseItems.Count == 0) return new List<SimpleItem>();

            // Simple parent filtering if configured
            if (!string.IsNullOrEmpty(column.ParentColumnName))
            {
                object parentValue = cell?.ParentCellValue;
                if (cell?.FilterdList != null && cell.FilterdList.Count > 0)
                {
                    return cell.FilterdList;
                }
                if (parentValue != null)
                {
                    return baseItems.Where(i => i.ParentValue?.ToString() == parentValue.ToString()).ToList();
                }
            }
            return baseItems.ToList();
        }

        /// <summary>
        /// Calculate how many rows can fit in the available height starting from a specific row
        /// </summary>
        private int GetVisibleRowCount(BindingList<BeepRowConfig> rows, int availableHeight, int startRow, int pixelOffset)
        {
            if (rows == null || rows.Count == 0) return 0;
            
            int visibleCount = 0;
            int usedHeight = 0;
            
            if (startRow < rows.Count)
            {
                var firstRow = rows[startRow];
                int firstRowHeight = firstRow.Height > 0 ? firstRow.Height : _grid.RowHeight;
                
                int totalOffsetToFirstRow = 0;
                for (int i = 0; i < startRow; i++)
                {
                    var row = rows[i];
                    totalOffsetToFirstRow += row.Height > 0 ? row.Height : _grid.RowHeight;
                }
                
                int firstRowVisibleHeight = firstRowHeight - (pixelOffset - totalOffsetToFirstRow);
                if (firstRowVisibleHeight > 0)
                {
                    usedHeight += firstRowVisibleHeight;
                    visibleCount++;
                }
                
                for (int i = startRow + 1; i < rows.Count && usedHeight < availableHeight; i++)
                {
                    var row = rows[i];
                    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                    
                    if (usedHeight + rowHeight > availableHeight)
                        break;
                        
                    usedHeight += rowHeight;
                    visibleCount++;
                }
            }
            
            return visibleCount;
        }

       
        private void DrawNavigatorArea(Graphics g)
        {
            // Delegate to GridNavigationPainterHelper
            _grid.NavigatorPainter.DrawNavigatorArea(g);
        }

        private void DrawSelectionIndicators(Graphics g)
        {
            if (g == null || _grid?.Selection == null || _grid.Data?.Rows == null)
                return;

            int rowIndex = _grid.Selection.RowIndex;
            int colIndex = _grid.Selection.ColumnIndex;
            if (rowIndex < 0 || rowIndex >= _grid.Data.Rows.Count)
                return;
            if (colIndex < 0 || colIndex >= _grid.Data.Columns.Count)
                return;

            var row = _grid.Data.Rows[rowIndex];
            if (colIndex >= row.Cells.Count)
                return;

            var cell = row.Cells[colIndex];
            if (cell.Rect.IsEmpty || cell.Rect.Width <= 0 || cell.Rect.Height <= 0)
                return;

            var rowsRect = _grid.Layout.RowsRect;
            var focusRect = Rectangle.Intersect(rowsRect, cell.Rect);
            if (focusRect.IsEmpty)
                return;

            var focusColor = ResolveThemeFocusColor();
            var state = g.Save();
            g.SetClip(rowsRect);

            if (ShowFocusedCellFill)
            {
                var fillBase = FocusedCellFillColor == Color.Empty ? focusColor : FocusedCellFillColor;
                int alpha = Math.Max(0, Math.Min(255, FocusedCellFillOpacity));
                using var fillBrush = new SolidBrush(Color.FromArgb(alpha, fillBase.R, fillBase.G, fillBase.B));
                g.FillRectangle(fillBrush, focusRect);
            }

            if (ShowFocusedCellBorder && FocusedCellBorderWidth > 0f)
            {
                var borderColor = FocusedCellBorderColor == Color.Empty ? focusColor : FocusedCellBorderColor;
                using var borderPen = new Pen(borderColor, FocusedCellBorderWidth)
                {
                    Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
                };
                var borderRect = Rectangle.Inflate(focusRect, -1, -1);
                if (borderRect.Width > 0 && borderRect.Height > 0)
                {
                    g.DrawRectangle(borderPen, borderRect);
                }
            }

            g.Restore(state);
        }

        // Professional paging methods
      

        public void SetCurrentPage(int pageNumber)
        {
            // Page jump textbox removed; nothing to set
        }

        public int GetCurrentPageSize()
        {
            // Page size combobox removed — return default
            return 10;
        }

        public int GetJumpToPage()
        {
            // No page jump control; default to 1
            return 1;
        }



        // Public methods for hit testing - connected to actual navigation
        public void TriggerPageFirst() => _grid.Navigator?.MoveFirst();
        public void TriggerPagePrev() => _grid.Navigator?.MovePrevious();
        public void TriggerPageNext() => _grid.Navigator?.MoveNext();
        public void TriggerPageLast() => _grid.Navigator?.MoveLast();
        public void TriggerGoToPage()
        {
            int page = GetJumpToPage();
            // For now, just move to first/last based on page number
            // TODO: Implement actual page-based navigation when paging is added
            if (page == 1)
                _grid.Navigator?.MoveFirst();
            else
                _grid.Navigator?.MoveLast();
        }
    public void FocusPageJump() { /* no-op: page jump removed */ }
    public void FocusPageSize() { /* no-op: page size selector removed */ }

        public void SetupPagingEventHandlers(Action<int> onPageSizeChanged, Action<int> onPageJump, Action onFirstPage, Action onPrevPage, Action onNextPage, Action onLastPage)
        {
            // page size selector removed; nothing to wire here
            // Only wire the main navigation callbacks to the grid navigator
            // The main nav buttons are handled via hit test -> _grid.Navigator actions
            // Keep this method for API compatibility but avoid wiring removed controls.
        }

        private static TextFormatFlags GetTextFormatFlagsForAlignment(ContentAlignment alignment, bool endEllipsis)
        {
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping;
            if (endEllipsis) flags |= TextFormatFlags.EndEllipsis;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right;
                    break;
            }
            return flags;
        }
    }
}

