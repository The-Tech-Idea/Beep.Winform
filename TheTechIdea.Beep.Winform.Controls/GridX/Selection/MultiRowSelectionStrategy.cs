using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Selection
{
    /// <summary>
    /// Multi-row selection strategy: Ctrl+click toggles individual rows,
    /// Shift+click selects a contiguous range, normal click clears and selects one row.
    /// </summary>
    internal sealed class MultiRowSelectionStrategy : ISelectionStrategy
    {
        public static readonly MultiRowSelectionStrategy Instance = new();

        private MultiRowSelectionStrategy() { }

        private const string AnchorKey = "MultiRowAnchor";

        public void HandleCellClick(SelectionContext context, int row, int col, bool ctrlPressed, bool shiftPressed)
        {
            if (row < 0 || row >= context.RowCount) return;

            if (ctrlPressed)
            {
                // Toggle row selection without clearing others
                bool newState = !context.Grid.Data.Rows[row].IsSelected;
                context.SetRowSelected(row, newState);
                context.SetActiveCell(row, col);
                if (newState)
                    SetAnchor(context, row);
                context.RaiseRowSelectionChanged(row);
            }
            else if (shiftPressed)
            {
                // Range select from anchor to clicked row
                var anchor = GetAnchor(context);
                if (anchor.HasValue)
                {
                    int r0 = Math.Min(anchor.Value, row);
                    int r1 = Math.Max(anchor.Value, row);

                    context.ClearAllRowSelection();
                    for (int r = r0; r <= r1; r++)
                        context.SetRowSelected(r, true);
                    context.SetActiveCell(row, col);
                    context.RaiseRowSelectionChanged(row);
                }
                else
                {
                    ClearAndSelect(context, row, col);
                }
            }
            else
            {
                ClearAndSelect(context, row, col);
            }
        }

        public void HandleColumnHeaderClick(SelectionContext context, int col, bool ctrlPressed, bool shiftPressed)
        {
            // In row-select mode, clicking a column header does nothing
        }

        public void HandleKeyboardNavigation(SelectionContext context, int deltaRow, int deltaCol)
        {
            var s = context.Selection;
            int newRow = s.RowIndex + deltaRow;
            int newCol = s.ColumnIndex + deltaCol;

            newRow = Math.Max(0, Math.Min(newRow, context.RowCount - 1));
            newCol = Math.Max(0, Math.Min(newCol, context.ColumnCount - 1));

            context.ClearAllRowSelection();
            context.SetRowSelected(newRow, true);
            context.SetActiveCell(newRow, newCol);
            SetAnchor(context, newRow);
            context.RaiseRowSelectionChanged(newRow);
        }

        public void ClearSelection(SelectionContext context)
        {
            context.ClearAllRowSelection();
            context.ClearAllCellSelection();
            context.StrategyState.Remove(AnchorKey);
        }

        private static void ClearAndSelect(SelectionContext context, int row, int col)
        {
            context.ClearAllRowSelection();
            context.ClearAllCellSelection();
            context.SetRowSelected(row, true);
            context.SetActiveCell(row, col);
            SetAnchor(context, row);
            context.RaiseRowSelectionChanged(row);
        }

        private static void SetAnchor(SelectionContext context, int row)
        {
            context.StrategyState[AnchorKey] = row;
        }

        private static int? GetAnchor(SelectionContext context)
        {
            if (context.StrategyState.TryGetValue(AnchorKey, out var obj) && obj is int row)
                return row;
            return null;
        }
    }
}
