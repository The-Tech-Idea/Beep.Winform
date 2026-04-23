namespace TheTechIdea.Beep.Winform.Controls.GridX.Selection
{
    /// <summary>
    /// Full-column-select strategy: clicking any cell selects the entire column.
    /// Clicking a column header also selects the entire column.
    /// </summary>
    internal sealed class ColumnSelectionStrategy : ISelectionStrategy
    {
        public static readonly ColumnSelectionStrategy Instance = new();

        private ColumnSelectionStrategy() { }

        public void HandleCellClick(SelectionContext context, int row, int col, bool ctrlPressed, bool shiftPressed)
        {
            if (col < 0 || col >= context.ColumnCount) return;

            context.ClearAllCellSelection();
            context.ClearAllRowSelection();

            // Select all cells in this column
            for (int r = 0; r < context.RowCount; r++)
            {
                context.SetCellSelected(r, col, true);
            }

            context.SetActiveCell(row >= 0 ? row : 0, col);
            context.RaiseRowSelectionChanged(row >= 0 ? row : 0);
        }

        public void HandleColumnHeaderClick(SelectionContext context, int col, bool ctrlPressed, bool shiftPressed)
        {
            HandleCellClick(context, 0, col, ctrlPressed, shiftPressed);
        }

        public void HandleKeyboardNavigation(SelectionContext context, int deltaRow, int deltaCol)
        {
            var s = context.Selection;
            int newCol = s.ColumnIndex + deltaCol;
            newCol = System.Math.Max(0, System.Math.Min(newCol, context.ColumnCount - 1));

            if (newCol >= 0 && newCol < context.ColumnCount)
            {
                context.ClearAllCellSelection();
                for (int r = 0; r < context.RowCount; r++)
                {
                    context.SetCellSelected(r, newCol, true);
                }
                context.SetActiveCell(s.RowIndex >= 0 ? s.RowIndex : 0, newCol);
            }
        }

        public void ClearSelection(SelectionContext context)
        {
            context.ClearAllCellSelection();
            context.ClearAllRowSelection();
        }
    }
}
