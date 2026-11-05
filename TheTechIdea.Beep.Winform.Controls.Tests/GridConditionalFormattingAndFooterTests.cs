using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Formatting;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridConditionalFormattingAndFooterTests
    {
        [Fact]
        public void ConditionalRule_AppliesToMatchingCell()
        {
            using var grid = CreateGrid();
            grid.ConditionalFormatting.Add(new ConditionalFormatRule
            {
                ColumnName = "Amount",
                Operator = FilterOperator.GreaterThan,
                Value = "15",
                Style = new GridCellStyle { BackColor = Color.MistyRose }
            });

            var amountColumn = grid.GetColumnByName("Amount")!;
            int rowIndex = grid.Rows.ToList().FindIndex(r =>
                string.Equals(Convert.ToString(r.Cells.First(c => c.ColumnName == "Name").CellValue), "Alice"));

            var style = grid.ConditionalFormatting.Resolve(grid.Rows[rowIndex], rowIndex, amountColumn);

            Assert.NotNull(style);
            Assert.Equal(Color.MistyRose, style!.BackColor);
        }

        [Fact]
        public void Footer_AggregatesTheVisibleRows()
        {
            using var grid = CreateGrid();
            grid.Footer.SetAggregate("Amount", AggregationType.Sum);
            grid.ShowFooter = true;
            grid.RecalculateHeightsFromPainters();

            Assert.Equal(60m, Convert.ToDecimal(grid.Footer.GetValue(grid, "Amount")));
            Assert.False(grid.FooterRect.IsEmpty);
        }

        [Fact]
        public void Footer_UpdatesAfterFilter()
        {
            using var grid = CreateGrid();
            grid.Footer.SetAggregate("Amount", AggregationType.Sum);

            grid.ApplyQuickFilter("Alice");

            Assert.Equal(30m, Convert.ToDecimal(grid.Footer.GetValue(grid, "Amount")));
        }

        [Fact]
        public void SearchText_HighlightsMatchingCells()
        {
            using var grid = CreateGrid();
            grid.ApplyQuickFilter("Alice");

            var row = grid.Rows[0];
            var nameColumn = grid.GetColumnByName("Name")!;
            var nameCell = row.Cells.First(c => c.ColumnName == "Name");
            var result = grid.Render.ApplyConditionalFormatting(
                row, 0, nameColumn, nameCell, Color.White, Color.Black, false, false);

            Assert.Equal(grid.ChromeState.ActiveBackground, result.Back);
        }

        [Fact]
        public void ConditionalRule_DoesNotOverrideSelectedRow()
        {
            using var grid = CreateGrid();
            grid.ConditionalFormatting.Add(new ConditionalFormatRule
            {
                ColumnName = "Amount",
                Operator = FilterOperator.GreaterThan,
                Value = "15",
                Style = new GridCellStyle { BackColor = Color.MistyRose }
            });

            var row = grid.Rows[0];
            var amountColumn = grid.GetColumnByName("Amount")!;
            var amountCell = row.Cells.First(c => c.ColumnName == "Amount");

            var normal = grid.Render.ApplyConditionalFormatting(
                row, 0, amountColumn, amountCell, Color.White, Color.Black, false, false);
            var selected = grid.Render.ApplyConditionalFormatting(
                row, 0, amountColumn, amountCell, Color.White, Color.Black, true, false);

            Assert.Equal(Color.MistyRose, normal.Back);
            Assert.Equal(Color.White, selected.Back);
        }

        [Fact]
        public void Footer_StaysPinnedAboveNavigatorAndOutsideTheRowsViewport()
        {
            using var grid = CreateGrid();
            grid.ShowNavigator = true;
            grid.ShowFooter = true;
            grid.RecalculateHeightsFromPainters();

            Assert.True(grid.Layout.FooterRect.Bottom <= grid.Layout.NavigatorRect.Top);
            Assert.True(grid.Layout.RowsRect.Bottom <= grid.Layout.FooterRect.Top);

            var before = grid.Layout.FooterRect;
            grid.Scroll.SetVerticalOffset(200);
            grid.Layout.Recalculate();

            Assert.Equal(before, grid.Layout.FooterRect);
        }

        private static BeepGridPro CreateGrid()
        {
            var grid = new BeepGridPro
            {
                Size = new Size(640, 420)
            };
            grid.DataSource = new List<FooterPerson>
            {
                new() { Name = "Alice", Amount = 30 },
                new() { Name = "Bob", Amount = 10 },
                new() { Name = "Charlie", Amount = 20 }
            };
            grid.AutoGenerateColumns();
            return grid;
        }

        private sealed class FooterPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
        }
    }
}
