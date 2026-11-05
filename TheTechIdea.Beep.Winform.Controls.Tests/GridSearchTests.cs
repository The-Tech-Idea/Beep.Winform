using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridSearchTests
    {
        [Fact]
        public void Search_ScopeAndMatchCount_AreApplied()
        {
            using var grid = CreateGrid();

            grid.SearchColumnName = "Name";
            grid.ApplyQuickFilter("Ali");

            Assert.Equal(1, grid.SearchMatchCount);
            grid.Selection.Clear();
            Assert.True(grid.MoveToNextSearchMatch());
            Assert.Equal(0, grid.Selection.RowIndex);
        }

        [Fact]
        public void Search_MovesToNextVisibleMatch()
        {
            using var grid = CreateGrid();
            grid.ApplyQuickFilter("a");
            Assert.True(grid.SearchMatchCount >= 2);

            Assert.True(grid.MoveToNextSearchMatch());
            int first = grid.Selection.RowIndex;
            Assert.True(grid.MoveToNextSearchMatch());

            Assert.NotEqual(first, grid.Selection.RowIndex);
        }

        [Fact]
        public void ClearToolbarSearch_KeepsColumnFilters()
        {
            using var grid = CreateGrid();
            grid.SortFilter.Filter("Name", "Alice");
            grid.ApplyQuickFilter("Alice");

            grid.ClearToolbarSearch();

            Assert.Equal(0, grid.SearchMatchCount);
            var visible = Assert.Single(grid.Rows, r => r.IsVisible);
            Assert.Equal("Alice", visible.Cells.First(c => c.ColumnName == "Name").CellValue);
        }

        [Fact]
        public void VirtualSearch_UsesTheQuerySourceTotal()
        {
            using var grid = CreateGrid();
            grid.EnableVirtualization = true;

            grid.ApplyQuickFilter("Alice");

            Assert.Equal(1, grid.SearchMatchCount);
            Assert.Single(grid.Rows);
        }

        [Fact]
        public void Search_UpdatesAccessibleDescription()
        {
            using var grid = CreateGrid();

            grid.ApplyQuickFilter("Alice");
            Assert.Contains("1 matching rows", grid.AccessibleDescription);

            grid.ClearFilter();
            Assert.Contains("3 rows", grid.AccessibleDescription);
        }

        [Fact]
        public void PageNavigation_UpdatesAccessibleDescription()
        {
            using var grid = CreateGrid();
            grid.PageSize = 1;

            grid.GoToPage(2);

            Assert.Contains("page 2 of 3", grid.AccessibleDescription);
            Assert.Contains("page size 1", grid.AccessibleDescription);
        }

        [Fact]
        public void AccessibleObject_ExposesRoleAndLiveDescription()
        {
            using var grid = CreateGrid();
            grid.UpdateAccessibleDescription();

            Assert.Equal(AccessibleRole.Table, grid.AccessibilityObject.Role);
            Assert.Contains("3 rows", grid.AccessibilityObject.Description);

            grid.ApplyQuickFilter("Alice");
            Assert.Equal(1, grid.AccessibilityObject.GetChildCount());
            Assert.Equal("Row 1", grid.AccessibilityObject.GetChild(0)?.Name);

            grid.ApplyQuickFilter("Charlie");
            Assert.Null(grid.AccessibilityObject.GetChild(0)?.Navigate(AccessibleNavigation.Down));

            grid.SelectCell(2, 1);
            Assert.NotNull(grid.AccessibilityObject.GetFocused());
        }

        [Fact]
        public void QuickSearch_AndsWithColumnFilter()
        {
            using var grid = CreateGrid();
            grid.SortFilter.Filter("Name", "Ali");

            Assert.True(grid.IsFiltered);
            Assert.Equal(1, grid.FilteredRowCount);

            grid.ApplyQuickFilter("Alice");

            var visible = Assert.Single(grid.Data.Rows, r => r.IsVisible);
            Assert.Equal("Alice", ((SearchPerson)visible.RowData).Name);
        }

        [Fact]
        public void ApplyingFilter_MovesToFirstVisibleRow()
        {
            using var grid = CreateGrid();
            grid.PageSize = 1;

            grid.ApplyQuickFilter("Charlie");

            Assert.Equal(2, grid.Selection.RowIndex);
            Assert.Equal(2, grid.Layout.RowMetrics.FirstVisibleRow(0));
            Assert.Equal(2, grid.Scroll.FirstVisibleRowIndex);
            Assert.Equal(2, grid.Render.GetLastRenderableRowIndex(grid.Data.Rows, 200, 2, 0));
        }

        [Fact]
        public void FilteredPagination_UsesVisibleRowCount()
        {
            using var grid = CreateGrid();
            grid.PageSize = 1;
            Assert.Equal(3, grid.TotalPages);

            grid.ApplyQuickFilter("Charlie");

            Assert.Equal(1, grid.TotalPages);
            Assert.Equal(1, grid.CurrentPage);
        }

        [Fact]
        public void FilteredPagination_GoToPageUsesVisibleOrdinal()
        {
            using var grid = CreateGrid();
            grid.PageSize = 1;

            grid.ApplyQuickFilter("a");
            Assert.Equal(2, grid.TotalPages);

            grid.GoToPage(2);

            Assert.Equal(2, grid.CurrentPage);
            Assert.Equal(2, grid.Selection.RowIndex);
        }

        [Fact]
        public void ClearingSearch_KeepsColumnFilter()
        {
            using var grid = CreateGrid();
            grid.SortFilter.Filter("Name", "Ali");
            grid.ApplyQuickFilter("Alice");

            grid.ClearToolbarSearch();

            Assert.True(grid.IsFiltered);
            Assert.Equal(1, grid.FilteredRowCount);
            var visible = Assert.Single(grid.Data.Rows, r => r.IsVisible);
            Assert.Equal("Alice", ((SearchPerson)visible.RowData).Name);
        }

        [Fact]
        public void EmptyQuickSearch_KeepsColumnFilter()
        {
            using var grid = CreateGrid();
            grid.SortFilter.Filter("Name", "Ali");
            grid.ApplyQuickFilter("Alice");

            grid.ApplyQuickFilter(string.Empty);

            Assert.True(grid.IsFiltered);
            Assert.Single(grid.Data.Rows, r => r.IsVisible);
        }

        private static BeepGridPro CreateGrid()
        {
            var grid = new BeepGridPro();
            grid.DataSource = new List<SearchPerson>
            {
                new() { Name = "Alice", Amount = 10 },
                new() { Name = "Bob", Amount = 20 },
                new() { Name = "Charlie", Amount = 30 }
            };
            grid.AutoGenerateColumns();
            return grid;
        }

        private sealed class SearchPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
        }
    }
}
