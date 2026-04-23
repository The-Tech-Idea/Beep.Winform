namespace TheTechIdea.Beep.Winform.Controls.GridX.Selection
{
    /// <summary>
    /// Full-row-select strategy: clicking any cell selects the entire row.
    /// Matches the default behavior of most data grids.
    /// </summary>
    internal sealed class RowSelectionStrategy : ISelectionStrategy
    {
        public static readonly RowSelectionStrategy Instance = new();

        private RowSelectionStrategy() { }

        public void HandleCellClick(SelectionContext context, int row, int col, bool ctrlPressed, bool shiftPressed)
        {
            if (row < 0 || row >= context.RowCount) return;

            // Ctrl+click toggles row selection in multi-select mode
            if (ctrlPressed && context.Grid.MultiSelect)
            {
                bool currentlySelected = context.Grid.Data.Rows[row].IsSelected;
                context.SetRowSelected(row, !currentlySelected);
                context.SetActiveCell(row, col);
                context.RaiseRowSelectionChanged(row);
                return;
            }

            // Normal click: clear previous, select this row
            context.ClearAllRowSelection();
            context.ClearAllCellSelection();
            context.SetRowSelected(row, true);
            context.SetActiveCell(row, col);
            context.RaiseRowSelectionChanged(row);
        }

        public void HandleColumnHeaderClick(SelectionContext context, int col, bool ctrlPressed, bool shiftPressed)
        {
            // In full-row mode, clicking a header could select all rows (optional)
            // For now, no-op to avoid surprising the user
        }

        public void HandleKeyboardNavigation(SelectionContext context, int deltaRow, int deltaCol)
        {
            var s = context.Selection;
            int newRow = s.RowIndex + deltaRow;
            newRow = System.Math.Max(0, System.Math.Min(newRow, context.RowCount - 1));

            if (newRow >= 0 && newRow < context.RowCount)
            {
                context.ClearAllRowSelection();
                context.ClearAllCellSelection();
                context.SetRowSelected(newRow, true);
                context.SetActiveCell(newRow, s.ColumnIndex >= 0 ? s.ColumnIndex : 0);
                context.RaiseRowSelectionChanged(newRow);
            }
        }

        public void ClearSelection(SelectionContext context)
        {
            context.ClearAllRowSelection();
            context.ClearAllCellSelection();
        }
    }
}
