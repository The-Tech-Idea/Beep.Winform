using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing dialog and filter-related methods for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        #region Dialog Methods
        /// <summary>
        /// Shows an editor dialog for the currently selected cell.
        /// </summary>
        public void ShowCellEditor()
        {
            if (Selection.HasSelection)
            {
                var cell = Data.Rows[Selection.RowIndex].Cells[Selection.ColumnIndex];
                Dialog.ShowEditorDialog(cell);
            }
        }

        /// <summary>
        /// Shows the filter dialog to configure grid filtering.
        /// </summary>
        public void ShowFilterDialog()
        {
            Dialog.ShowFilterDialog();
        }

        /// <summary>
        /// Shows the search dialog to find data in the grid.
        /// </summary>
        public void ShowSearchDialog()
        {
            Dialog.ShowSearchDialog();
        }

        /// <summary>
        /// Shows the column configuration dialog to customize column visibility and settings.
        /// </summary>
        public void ShowColumnConfigDialog()
        {
            Dialog.ShowColumnConfigDialog();
        }
        #endregion

        #region Filter and Sort Methods
        /// <summary>
        /// Enables Excel-like filter functionality by adding filter icons to column headers.
        /// This method can be expanded to add filter dropdowns to column headers.
        /// </summary>
        public void EnableExcelFilter()
        {
            // This method can be expanded to add filter dropdowns to column headers
            // For now, it's a placeholder that ensures the grid is ready for filtering
            foreach (var col in Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID))
            {
                col.ShowFilterIcon = true;
            }
            Invalidate();
        }

        /// <summary>
        /// Toggles the sort direction for the specified column index.
        /// </summary>
        /// <param name="columnIndex">The index of the column to sort.</param>
        public void ToggleColumnSort(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Data.Columns.Count)
                return;

            var column = Data.Columns[columnIndex];
            if (column == null)
                return;

            // Toggle sort direction
            var newDirection = column.SortDirection == SortDirection.Ascending 
                ? SortDirection.Descending 
                : SortDirection.Ascending;

            // Clear previous sort indicators
            foreach (var col in Data.Columns)
            {
                col.IsSorted = false;
                col.ShowSortIcon = false;
            }

            // Set new sort
            column.IsSorted = true;
            column.ShowSortIcon = true;
            column.SortDirection = newDirection;

            // Apply the sort
            SortFilter.Sort(column.ColumnName, newDirection);

            // Refresh the grid
            Invalidate();
        }
        #endregion
    }
}
