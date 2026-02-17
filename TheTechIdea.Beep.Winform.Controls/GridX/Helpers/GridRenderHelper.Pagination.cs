using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Pagination-related methods for grid navigation and page calculations
    /// </summary>
    internal partial class GridRenderHelper
    {
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
            if (pageSize <= 0) pageSize = 10;
            
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
            if (pageSize <= 0) pageSize = 10;
            
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
            if (pageSize <= 0) pageSize = 10;
            
            return System.Math.Max(1, (int)System.Math.Ceiling(totalRecords / (double)pageSize));
        }

        /// <summary>
        /// Gets the range of page numbers to display in pagination controls
        /// </summary>
        public (int startPage, int endPage) GetVisiblePageRange(BeepGridPro grid, int maxVisiblePages = 5)
        {
            int currentPage = GetCurrentPage(grid);
            int totalPages = GetTotalPages(grid);
            return CalculatePageRange(currentPage, totalPages, maxVisiblePages);
        }

        /// <summary>
        /// Calculates the range of page numbers to display
        /// </summary>
        public (int startPage, int endPage) CalculatePageRange(int currentPage, int totalPages, int maxVisiblePages = 5)
        {
            if (totalPages <= maxVisiblePages)
                return (1, totalPages);
            
            int halfVisible = maxVisiblePages / 2;
            int startPage = System.Math.Max(1, currentPage - halfVisible);
            int endPage = System.Math.Min(totalPages, startPage + maxVisiblePages - 1);
            
            if (endPage - startPage < maxVisiblePages - 1)
                startPage = System.Math.Max(1, endPage - maxVisiblePages + 1);
            
            return (startPage, endPage);
        }

        /// <summary>
        /// Gets the first row index (0-based) for a specific page
        /// </summary>
        public int GetPageStartRow(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;
            
            int pageSize = GetVisibleRowCapacity();
            if (pageSize <= 0) pageSize = 10;
            
            return (pageNumber - 1) * pageSize;
        }

        /// <summary>
        /// Gets the last row index (0-based) for a specific page
        /// </summary>
        public int GetPageEndRow(int pageNumber, int totalRecords)
        {
            int startRow = GetPageStartRow(pageNumber);
            int pageSize = GetVisibleRowCapacity();
            if (pageSize <= 0) pageSize = 10;
            
            int endRow = startRow + pageSize - 1;
            return Math.Min(endRow, totalRecords - 1);
        }

        /// <summary>
        /// Gets the row range for a specific page
        /// </summary>
        public (int startRow, int endRow) GetPageRowRange(int pageNumber, int totalRecords)
        {
            int startRow = GetPageStartRow(pageNumber);
            int endRow = GetPageEndRow(pageNumber, totalRecords);
            return (startRow, endRow);
        }

        /// <summary>
        /// Navigates the grid to a specific page
        /// </summary>
        public void GoToPage(BeepGridPro grid, int pageNumber)
        {
            if (grid?.Data?.Rows == null || grid.Selection == null) return;
            
            int totalPages = GetTotalPages(grid);
            pageNumber = System.Math.Max(1, System.Math.Min(pageNumber, totalPages));
            
            int targetRow = GetPageStartRow(pageNumber);
            targetRow = System.Math.Max(0, System.Math.Min(targetRow, grid.Data.Rows.Count - 1));
            
            int currentColumn = Math.Max(0, grid.Selection.ColumnIndex);
            grid.Selection.SelectCell(targetRow, currentColumn);
            
            grid.Scroll?.SetVerticalIndex(targetRow);
            grid.SafeInvalidate();
        }

        /// <summary>
        /// Gets pagination information as a formatted string
        /// </summary>
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
        /// Gets simple pagination info
        /// </summary>
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
        public string GetPagePaginationInfo(BeepGridPro grid)
        {
            int currentPage = GetCurrentPage(grid);
            int totalPages = GetTotalPages(grid);
            return $"Page {currentPage} of {totalPages}";
        }

        /// <summary>
        /// Checks if a specific page number is valid
        /// </summary>
        public bool IsValidPage(BeepGridPro grid, int pageNumber)
        {
            int totalPages = GetTotalPages(grid);
            return pageNumber >= 1 && pageNumber <= totalPages;
        }

        /// <summary>
        /// Checks if there is a previous page
        /// </summary>
        public bool HasPreviousPage(BeepGridPro grid)
        {
            return GetCurrentPage(grid) > 1;
        }

        /// <summary>
        /// Checks if there is a next page
        /// </summary>
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
                GoToPage(grid, currentPage - 1);
        }

        /// <summary>
        /// Navigates to the next page
        /// </summary>
        public void GoToNextPage(BeepGridPro grid)
        {
            int currentPage = GetCurrentPage(grid);
            int totalPages = GetTotalPages(grid);
            if (currentPage < totalPages)
                GoToPage(grid, currentPage + 1);
        }

        /// <summary>
        /// Sets current page (API compatibility)
        /// </summary>
        public void SetCurrentPage(int pageNumber)
        {
            // Page jump textbox removed; nothing to set
        }

        /// <summary>
        /// Gets current page size
        /// </summary>
        public int GetCurrentPageSize()
        {
            return 10;
        }

        /// <summary>
        /// Gets jump to page
        /// </summary>
        public int GetJumpToPage()
        {
            return 1;
        }

        /// <summary>
        /// Triggers page first navigation
        /// </summary>
        public void TriggerPageFirst() => _grid.Navigator?.MoveFirst();

        /// <summary>
        /// Triggers page previous navigation
        /// </summary>
        public void TriggerPagePrev() => _grid.Navigator?.MovePrevious();

        /// <summary>
        /// Triggers page next navigation
        /// </summary>
        public void TriggerPageNext() => _grid.Navigator?.MoveNext();

        /// <summary>
        /// Triggers page last navigation
        /// </summary>
        public void TriggerPageLast() => _grid.Navigator?.MoveLast();

        /// <summary>
        /// Triggers go to page
        /// </summary>
        public void TriggerGoToPage()
        {
            int page = GetJumpToPage();
            if (page == 1)
                _grid.Navigator?.MoveFirst();
            else
                _grid.Navigator?.MoveLast();
        }

        /// <summary>
        /// Focuses page jump
        /// </summary>
        public void FocusPageJump() { }

        /// <summary>
        /// Focuses page size
        /// </summary>
        public void FocusPageSize() { }

        /// <summary>
        /// Sets up paging event handlers
        /// </summary>
        public void SetupPagingEventHandlers(Action<int> onPageSizeChanged, Action<int> onPageJump, 
            Action onFirstPage, Action onPrevPage, Action onNextPage, Action onLastPage)
        {
            // Page size selector removed; nothing to wire here
        }
    }
}
