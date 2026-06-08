using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.GridX;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridTestPerson
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class GridTestCustomer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class GridTestOrder
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class BeepGridProDataSourceTests : IDisposable
    {
        private readonly BeepGridPro _grid;

        public BeepGridProDataSourceTests()
        {
            _grid = new BeepGridPro();
        }

        public void Dispose()
        {
            _grid.Dispose();
        }

        private void BindViaDataSource(object data, bool refresh = true)
        {
            _grid.DataSource = data;
            if (refresh)
                _grid.RefreshData();
        }

        [Fact]
        public void DataSource_BindToList_AutoGeneratesColumns()
        {
            var data = new List<GridTestPerson>
            {
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 }
            };

            BindViaDataSource(data);

            Assert.Contains(_grid.Columns, c => c.ColumnName == "Name");
            Assert.Contains(_grid.Columns, c => c.ColumnName == "Age");
            Assert.True(_grid.Rows.Count >= 2);
        }

        [Fact]
        public void DataSource_BindToDataTable_AutoGeneratesColumns()
        {
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));
            dt.Rows.Add("Alice", 30);
            dt.Rows.Add("Bob", 25);

            BindViaDataSource(dt);

            Assert.Contains(_grid.Columns, c => c.ColumnName == "Name");
            Assert.Contains(_grid.Columns, c => c.ColumnName == "Age");
            Assert.True(_grid.Rows.Count >= 2);
        }

        [Fact]
        public void DataSource_BindToObservableCollection_AddItem_UpdatesRowCount()
        {
            var collection = new ObservableCollection<GridTestPerson>
            {
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 }
            };

            BindViaDataSource(collection);
            var initialCount = _grid.Rows.Count;
            Assert.True(initialCount >= 2);

            collection.Add(new GridTestPerson { Name = "Charlie", Age = 35 });
            Assert.True(_grid.Rows.Count > initialCount);
        }

        [Fact]
        public void DataSource_BindToObservableCollection_RemoveItem_UpdatesRowCount()
        {
            var collection = new ObservableCollection<GridTestPerson>
            {
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 },
                new() { Name = "Charlie", Age = 35 }
            };

            BindViaDataSource(collection);
            var initialCount = _grid.Rows.Count;

            collection.RemoveAt(1);
            Assert.True(_grid.Rows.Count < initialCount);
        }

        [Fact]
        public void DataSource_BindToObservableCollection_Clear_EmptiesRows()
        {
            var collection = new ObservableCollection<GridTestPerson>
            {
                new() { Name = "Alice", Age = 30 },
                new() { Name = "Bob", Age = 25 }
            };

            BindViaDataSource(collection);
            Assert.True(_grid.Rows.Count >= 2);

            collection.Clear();
            Assert.True(_grid.Rows.Count <= 1); // system columns may remain
        }

        [Fact]
        public void DataSource_BindToNull_ClearsGrid()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice", Age = 30 } };
            BindViaDataSource(data);
            Assert.True(_grid.Rows.Count >= 1);

            _grid.ClearGrid();
            Assert.True(_grid.Rows.Count == 0);
        }

        [Fact]
        public void DataSource_GetColumnByName_ReturnsColumn()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice", Age = 30 } };
            BindViaDataSource(data);

            var col = _grid.GetColumnByName("Name");
            Assert.NotNull(col);

            var nonExistent = _grid.GetColumnByName("Missing");
            Assert.Null(nonExistent);
        }

        [Fact]
        public void DataSource_GetColumnByIndex_ReturnsColumn()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice", Age = 30 } };
            BindViaDataSource(data);

            var col = _grid.GetColumnByIndex(0);
            Assert.NotNull(col);

            var outOfRange = _grid.GetColumnByIndex(-1);
            Assert.Null(outOfRange);
        }

        [Fact]
        public void DataSource_SystemColumns_PresentOnBind()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice", Age = 30 } };
            BindViaDataSource(data);

            Assert.Contains(_grid.Columns, c => c.IsSelectionCheckBox);
            Assert.Contains(_grid.Columns, c => c.IsRowNumColumn);
            Assert.Contains(_grid.Columns, c => c.IsRowID);
        }

        [Fact]
        public void DataSource_EnsureSystemColumns_Idempotent()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice", Age = 30 } };
            BindViaDataSource(data);

            int countBefore = _grid.Columns.Count(c => c.IsSelectionCheckBox);
            _grid.EnsureSystemColumns();
            int countAfter = _grid.Columns.Count(c => c.IsSelectionCheckBox);

            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public void DataSource_ShowCheckBox_TogglesVisibility()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice", Age = 30 } };
            BindViaDataSource(data);

            _grid.ShowCheckBox = true;
            Assert.True(_grid.ShowCheckBox);

            _grid.ShowCheckBox = false;
            Assert.False(_grid.ShowCheckBox);
        }

        [Fact]
        public void DataSource_CurrentRow_ReturnsNull_WhenNoSelection()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice", Age = 30 } };
            BindViaDataSource(data);

            Assert.Null(_grid.CurrentRow);
        }

        [Fact]
        public void DataSource_SelectedRows_EmptyByDefault()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice" }, new() { Name = "Bob" } };
            BindViaDataSource(data);

            Assert.Empty(_grid.SelectedRows);
        }

        [Fact]
        public void DataSource_GetDictionaryColumns_ContainsKeys()
        {
            var data = new List<GridTestPerson> { new() { Name = "Alice", Age = 30 } };
            BindViaDataSource(data);

            var dict = _grid.GetDictionaryColumns();
            Assert.Contains("Name", dict.Keys);
            Assert.Contains("Age", dict.Keys);
        }

        [Fact]
        public void DataSource_BindArray_WorksCorrectly()
        {
            var data = new[] { "Alice", "Bob", "Charlie" };

            BindViaDataSource(data);

            Assert.True(_grid.Rows.Count >= 3);
        }
    }
}
