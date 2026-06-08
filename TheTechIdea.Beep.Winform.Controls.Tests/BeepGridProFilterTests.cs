using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepGridProFilterTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProFilterTests()
        {
            _grid = new BeepGridPro();
        }

        public void Dispose()
        {
            _grid.Dispose();
        }

        private void BindSampleData()
        {
            var data = new List<GridTestPerson>
            {
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 },
                new() { Name = "Charlie", Age = 35 },
                new() { Name = "David", Age = 25 },
                new() { Name = "Eve", Age = 40 },
                new() { Name = "alice", Age = 28 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();
        }

        [Fact]
        public void QuickFilter_ByName_HidesNonMatchingRows()
        {
            BindSampleData();

            _grid.ApplyQuickFilter("alice");

            Assert.True(_grid.IsFiltered);
            Assert.Equal(2, _grid.FilteredRowCount);
            Assert.NotNull(_grid.ActiveFilter);
            Assert.NotNull(_grid.FilteredRowIndices);
            Assert.Equal(2, _grid.FilteredRowIndices.Count);
        }

        [Fact]
        public void QuickFilter_BySpecificColumn_CorrectCount()
        {
            BindSampleData();

            _grid.ApplyQuickFilter("alice", columnName: "Name");

            Assert.True(_grid.IsFiltered);
            Assert.Equal(2, _grid.FilteredRowCount);
        }

        [Fact]
        public void QuickFilter_NoMatches_ReturnsEmpty()
        {
            BindSampleData();

            _grid.ApplyQuickFilter("xyz_nomatch");

            Assert.True(_grid.IsFiltered);
            Assert.Equal(0, _grid.FilteredRowCount);
        }

        [Fact]
        public void ClearFilter_RestoresAllRowsVisible()
        {
            BindSampleData();

            _grid.ApplyQuickFilter("alice");
            Assert.True(_grid.IsFiltered);

            _grid.ClearFilter();

            Assert.False(_grid.IsFiltered);
            Assert.Equal(6, _grid.FilteredRowCount);
            Assert.Null(_grid.ActiveFilter);
        }

        [Fact]
        public void QuickFilter_EmptySearchText_ClearsFilter()
        {
            BindSampleData();
            _grid.ApplyQuickFilter("alice");
            Assert.True(_grid.IsFiltered);

            _grid.ApplyQuickFilter("");

            Assert.False(_grid.IsFiltered);
            Assert.Equal(6, _grid.FilteredRowCount);
        }

        [Fact]
        public void ClearFilter_WhenNotFiltered_DoesNotThrow()
        {
            BindSampleData();

            _grid.ClearFilter();

            Assert.False(_grid.IsFiltered);
        }

        [Fact]
        public void FilteredRowIndices_Null_WhenNotFiltered()
        {
            BindSampleData();

            Assert.Null(_grid.FilteredRowIndices);
        }

        [Fact]
        public void IsFiltered_False_WhenFilterCleared()
        {
            BindSampleData();
            _grid.ApplyQuickFilter("alice");

            _grid.ClearFilter();

            Assert.False(_grid.IsFiltered);
        }

        [Fact]
        public void ActiveFilter_Null_AfterClear()
        {
            BindSampleData();
            _grid.ApplyQuickFilter("alice");

            _grid.ClearFilter();

            Assert.Null(_grid.ActiveFilter);
        }

        [Fact]
        public void FilterApplied_Event_Fires_OnApplyQuickFilter()
        {
            BindSampleData();
            int eventFiredCount = 0;

            _grid.FilterApplied += (s, e) => eventFiredCount++;

            _grid.ApplyQuickFilter("alice");

            Assert.Equal(1, eventFiredCount);
        }

        [Fact]
        public void FilterCleared_Event_Fires_OnClearFilter()
        {
            BindSampleData();
            _grid.ApplyQuickFilter("alice");
            int fired = 0;

            _grid.FilterCleared += (s, e) => fired++;

            _grid.ClearFilter();

            Assert.Equal(1, fired);
        }
    }
}
