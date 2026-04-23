using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Selection
{
    /// <summary>
    /// Multi-cell selection strategy: Ctrl+click toggles individual cells,
    /// Shift+click selects a rectangular range, normal click clears and selects one cell.
    /// </summary>
    internal sealed class MultiCellSelectionStrategy : ISelectionStrategy
    {
        public static readonly MultiCellSelectionStrategy Instance = new();

        private MultiCellSelectionStrategy() { }

        private const string AnchorKey = "MultiCellAnchor";

        public void HandleCellClick(SelectionContext context, int row, int col, bool ctrlPressed, bool shiftPressed)
        {
            if (!context.IsValidCell(row, col)) return;

            if (ctrlPressed)
            {
                // Toggle cell selection without clearing others
                var cell = context.Grid.Data.Rows[row].Cells[col];
                bool newState = !cell.IsSelected;
                context.SetCellSelected(row, col, newState);
                context.SetActiveCell(row, col);
                if (newState)
                    SetAnchor(context, row, col);
                context.RaiseRowSelectionChanged(row);
            }
            else if (shiftPressed)
            {
                // Range select from anchor to clicked cell
                var anchor = GetAnchor(context);
                if (anchor.HasValue)
                {
                    int r0 = Math.Min(anchor.Value.Row, row);
                    int r1 = Math.Max(anchor.Value.Row, row);
                    int c0 = Math.Min(anchor.Value.Col, col);
                    int c1 = Math.Max(anchor.Value.Col, col);

                    context.ClearAllCellSelection();
                    for (int r = r0; r <= r1; r++)
                    {
                        for (int c = c0; c <= c1; c++)
                        {
                            if (context.IsValidCell(r, c))
                                context.SetCellSelected(r, c, true);
                        }
                    }
                    context.SetActiveCell(row, col);
                    context.RaiseRowSelectionChanged(row);
                }
                else
                {
                    // No anchor: treat as normal click
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
            // Select all cells in the column
            if (col < 0 || col >= context.ColumnCount) return;

            if (!ctrlPressed && !shiftPressed)
            {
                context.ClearAllCellSelection();
                context.ClearAllRowSelection();
            }

            for (int r = 0; r < context.RowCount; r++)
            {
                if (context.IsValidCell(r, col))
                    context.SetCellSelected(r, col, true);
            }
            SetAnchor(context, 0, col);
        }

        public void HandleKeyboardNavigation(SelectionContext context, int deltaRow, int deltaCol)
        {
            var s = context.Selection;
            int newRow = s.RowIndex + deltaRow;
            int newCol = s.ColumnIndex + deltaCol;

            newRow = Math.Max(0, Math.Min(newRow, context.RowCount - 1));
            newCol = Math.Max(0, Math.Min(newCol, context.ColumnCount - 1));

            if (context.IsValidCell(newRow, newCol))
            {
                context.ClearAllCellSelection();
                context.SetCellSelected(newRow, newCol, true);
                context.SetActiveCell(newRow, newCol);
                SetAnchor(context, newRow, newCol);
                context.RaiseRowSelectionChanged(newRow);
            }
        }

        public void ClearSelection(SelectionContext context)
        {
            context.ClearAllCellSelection();
            context.ClearAllRowSelection();
            context.StrategyState.Remove(AnchorKey);
        }

        private static void ClearAndSelect(SelectionContext context, int row, int col)
        {
            context.ClearAllCellSelection();
            context.ClearAllRowSelection();
            context.SetCellSelected(row, col, true);
            context.SetActiveCell(row, col);
            SetAnchor(context, row, col);
            context.RaiseRowSelectionChanged(row);
        }

        private static void SetAnchor(SelectionContext context, int row, int col)
        {
            context.StrategyState[AnchorKey] = (row, col);
        }

        private static (int Row, int Col)? GetAnchor(SelectionContext context)
        {
            if (context.StrategyState.TryGetValue(AnchorKey, out var obj) && obj is ValueTuple<int, int> t)
                return t;
            return null;
        }
    }
}
