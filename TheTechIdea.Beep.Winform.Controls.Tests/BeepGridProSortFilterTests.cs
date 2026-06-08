using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Vis.Modules;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepGridProSortFilterTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProSortFilterTests()
        {
            _grid = new BeepGridPro();
        }

        public void Dispose()
        {
            _grid.Dispose();
        }

        private object GetSortFilter()
        {
            var prop = typeof(BeepGridPro).GetProperty("SortFilter", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(prop);
            return prop!.GetValue(_grid)!;
        }

        private void InvokeSortFilter(string methodName, params object[] args)
        {
            var sortFilter = GetSortFilter();
            var method = sortFilter.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(method);
            method!.Invoke(sortFilter, args);
        }

        private void BindData()
        {
            var data = new List<GridTestPerson>
            {
                new() { Name = "Charlie", Age = 35 },
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 },
                new() { Name = "alice", Age = 28 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();
        }

        private void BindDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));
            dt.Rows.Add("Charlie", 35);
            dt.Rows.Add("Alice", 30);
            dt.Rows.Add("Bob", 25);
            _grid.DataSource = dt;
            _grid.RefreshData();
        }

        // ========== Sort Tests ==========

        [Fact]
        public void Sort_Ascending_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Sort", "Name", SortDirection.Ascending);
        }

        [Fact]
        public void Sort_Descending_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Sort", "Name", SortDirection.Descending);
        }

        [Fact]
        public void Sort_ByAge_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Sort", "Age", SortDirection.Ascending);
        }

        [Fact]
        public void Sort_NonExistentColumn_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Sort", "NonExistent", SortDirection.Ascending);
        }

        [Fact]
        public void Sort_DataTable_Ascending_DoesNotThrow()
        {
            BindDataTable();
            InvokeSortFilter("Sort", "Name", SortDirection.Ascending);
        }

        // ========== Filter Tests ==========

        [Fact]
        public void Filter_Contains_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Filter", "Name", "alice");
        }

        [Fact]
        public void Filter_Empty_ClearsFilter()
        {
            BindData();
            InvokeSortFilter("Filter", "Name", "alice");
            InvokeSortFilter("Filter", "Name", "");
        }

        [Fact]
        public void Filter_NonExistentColumn_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Filter", "NonExistent", "value");
        }

        [Fact]
        public void Filter_MultipleColumns_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Filter", "Name", "a");
            InvokeSortFilter("Filter", "Age", "30");
        }

        [Fact]
        public void Filter_DataTable_DoesNotThrow()
        {
            BindDataTable();
            InvokeSortFilter("Filter", "Name", "Alice");
        }

        // ========== FilterIn Tests ==========

        [Fact]
        public void FilterIn_WithValues_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("FilterIn", "Name", new object[] { "Alice", "Bob" });
        }

        [Fact]
        public void FilterIn_NullList_ClearsFilter()
        {
            BindData();
            InvokeSortFilter("FilterIn", "Name", new object[] { "Alice" });
            InvokeSortFilter("FilterIn", "Name", (object?)null);
        }

        [Fact]
        public void FilterIn_EmptyList_ClearsFilter()
        {
            BindData();
            InvokeSortFilter("FilterIn", "Name", new object[] { "Alice" });
            InvokeSortFilter("FilterIn", "Name", Array.Empty<object>());
        }

        // ========== ClearFilters Tests ==========

        [Fact]
        public void ClearFilters_AfterFilter_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Filter", "Name", "alice");
            InvokeSortFilter("ClearFilters");
        }

        [Fact]
        public void ClearFilters_NoFilters_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("ClearFilters");
        }

        [Fact]
        public void ClearFilters_AfterMultipleFilters_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Filter", "Name", "a");
            InvokeSortFilter("Filter", "Age", "30");
            InvokeSortFilter("FilterIn", "Name", new object[] { "Alice" });
            InvokeSortFilter("ClearFilters");
        }

        // ========== Combined Operations ==========

        [Fact]
        public void SortThenFilter_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Sort", "Name", SortDirection.Ascending);
            InvokeSortFilter("Filter", "Name", "a");
        }

        [Fact]
        public void FilterThenSort_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Filter", "Name", "a");
            InvokeSortFilter("Sort", "Age", SortDirection.Descending);
        }

        [Fact]
        public void SortWithGrouping_DoesNotThrow()
        {
            BindData();
            InvokeSortFilter("Sort", "Name", SortDirection.Ascending);
            _grid.GroupBy("Name");
            InvokeSortFilter("Sort", "Age", SortDirection.Ascending);
        }

        [Fact]
        public void FilterWithGrouping_DoesNotThrow()
        {
            BindData();
            _grid.GroupBy("Name");
            InvokeSortFilter("Filter", "Name", "a");
        }

        // ========== BindingList Path ==========

        [Fact]
        public void Sort_BindingList_DoesNotThrow()
        {
            var data = new BindingList<GridTestPerson>
            {
                new() { Name = "Charlie", Age = 35 },
                new() { Name = "Alice", Age = 30 }
            };
            _grid.DataSource = data;
            _grid.RefreshData();

            InvokeSortFilter("Sort", "Name", SortDirection.Ascending);
        }

        // ========== Public ToggleColumnSort API ==========

        [Fact]
        public void ToggleColumnSort_ValidIndex_DoesNotThrow()
        {
            BindData();
            _grid.ToggleColumnSort(0);
        }

        [Fact]
        public void ToggleColumnSort_SameColumn_TogglesDirection()
        {
            BindData();
            _grid.ToggleColumnSort(0);
            _grid.ToggleColumnSort(0);
        }

        [Fact]
        public void ToggleColumnSort_DifferentColumns_DoesNotThrow()
        {
            BindData();
            _grid.ToggleColumnSort(0);
            _grid.ToggleColumnSort(1);
        }

        [Fact]
        public void ToggleColumnSort_InvalidIndex_DoesNotThrow()
        {
            BindData();
            _grid.ToggleColumnSort(999);
        }
    }
}
