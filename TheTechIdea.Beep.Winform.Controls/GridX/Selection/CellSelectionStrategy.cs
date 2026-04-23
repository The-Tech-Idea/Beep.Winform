namespace TheTechIdea.Beep.Winform.Controls.GridX.Selection
{
    /// <summary>
    /// Cell-select strategy: clicking selects only the clicked cell.
    /// </summary>
    internal sealed class CellSelectionStrategy : ISelectionStrategy
    {
        public static readonly CellSelectionStrategy Instance = new();

        private CellSelectionStrategy() { }

        public void HandleCellClick(SelectionContext context, int row, int col, bool ctrlPressed, bool shiftPressed)
        {
            if (!context.IsValidCell(row, col)) return;

            // Ctrl+click toggles cell selection in a multi-cell model (future)
            // For now, just move active cell
            context.ClearAllCellSelection();
            context.ClearAllRowSelection();
            context.SetCellSelected(row, col, true);
            context.SetActiveCell(row, col);
            context.RaiseRowSelectionChanged(row);
        }

        public void HandleColumnHeaderClick(SelectionContext context, int col, bool ctrlPressed, bool shiftPressed)
        {
            // No-op for cell select mode on header click
        }

        public void HandleKeyboardNavigation(SelectionContext context, int deltaRow, int deltaCol)
        {
            var s = context.Selection;
            int newRow = s.RowIndex + deltaRow;
            int newCol = s.ColumnIndex + deltaCol;

            newRow = System.Math.Max(0, System.Math.Min(newRow, context.RowCount - 1));
            newCol = System.Math.Max(0, System.Math.Min(newCol, context.ColumnCount - 1));

            if (context.IsValidCell(newRow, newCol))
            {
                context.ClearAllCellSelection();
                context.SetCellSelected(newRow, newCol, true);
                context.SetActiveCell(newRow, newCol);
                context.RaiseRowSelectionChanged(newRow);
            }
        }

        public void ClearSelection(SelectionContext context)
        {
            context.ClearAllCellSelection();
            context.ClearAllRowSelection();
        }
    }
}
