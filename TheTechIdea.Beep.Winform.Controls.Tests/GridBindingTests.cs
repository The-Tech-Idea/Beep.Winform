using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.GridX;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridBindingTests
    {
        [Fact]
        public void PocoList_WritesBackThroughSetCellValue()
        {
            var people = new List<Person> { new() { Name = "Alice", Amount = 10 } };
            using var grid = Bind(people);

            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.True(grid.SetCellValue(0, "Amount", "42"));

            Assert.Equal("Alicia", people[0].Name);
            Assert.Equal(42, people[0].Amount);
            Assert.Equal("Alicia", grid.GetCellValue(0, grid.GetColumnByName("Name")!.Index));
        }

        [Fact]
        public void UpdateCellValue_WritesThroughBinding()
        {
            var people = new List<Person> { new() { Name = "Alice", Amount = 10 } };
            using var grid = Bind(people);
            var column = grid.GetColumnByName("Name")!;
            var cell = grid.Rows[0].Cells.First(c => c.ColumnName == "Name");

            grid.Data.UpdateCellValue(cell, "Alicia");

            Assert.Equal("Alicia", people[0].Name);
            Assert.Equal("Alicia", cell.CellValue);
        }

        [Fact]
        public void SinglePoco_IsExposedAsOneRowAndWritesBack()
        {
            var person = new Person { Name = "Alice", Amount = 10 };
            using var grid = new BeepGridPro();
            grid.DataSource = person;
            grid.AutoGenerateColumns();

            Assert.Single(grid.Rows);
            Assert.True(grid.SetCellValue(0, "Amount", 42));
            Assert.Equal(42, person.Amount);
        }

        [Fact]
        public void DataTable_SupportsSchemaAndWritesBack()
        {
            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Amount", typeof(int));
            table.Rows.Add("Alice", 10);

            using var grid = new BeepGridPro();
            grid.DataSource = table;
            grid.AutoGenerateColumns();

            Assert.NotNull(grid.GetColumnByName("Name"));
            Assert.NotNull(grid.GetColumnByName("Amount"));
            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", table.Rows[0]["Name"]);
        }

        [Fact]
        public void VirtualizedDataSet_UsesNormalizedTableView()
        {
            using var dataSet = new DataSet();
            var table = dataSet.Tables.Add("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");

            using var grid = new BeepGridPro { EnableVirtualization = true };
            grid.DataMember = "People";
            grid.DataSource = dataSet;

            Assert.NotNull(grid.VirtualDataSource);
            Assert.NotNull(grid.GetColumnByName("Name"));
        }

        [Fact]
        public void DataSetWithDataMember_BindsSelectedTable()
        {
            using var dataSet = new DataSet();
            var table = dataSet.Tables.Add("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");

            using var grid = new BeepGridPro();
            grid.DataMember = "People";
            grid.DataSource = dataSet;
            grid.AutoGenerateColumns();

            Assert.Single(grid.Rows);
            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", table.Rows[0]["Name"]);
        }

        [Fact]
        public void DataRow_IsExposedAsOneRowAndWritesBack()
        {
            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");
            var row = table.Rows[0];

            using var grid = new BeepGridPro();
            grid.DataSource = row;
            grid.AutoGenerateColumns();

            Assert.Single(grid.Rows);
            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", row["Name"]);
        }

        [Fact]
        public void DataTableColumnAddition_RebuildsSchema()
        {
            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");

            using var grid = new BeepGridPro();
            grid.DataSource = table;
            grid.AutoGenerateColumns();

            table.Columns.Add("Age", typeof(int));

            Assert.NotNull(grid.GetColumnByName("Age"));
        }

        [Fact]
        public void DataTableColumnRemoval_PrunesFilterState()
        {
            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(int));
            table.Rows.Add("Alice", 10);

            using var grid = new BeepGridPro();
            grid.DataSource = table;
            grid.AutoGenerateColumns();
            grid.SortFilter.Filter("Age", "1");

            table.Columns.Remove("Age");

            Assert.Null(grid.GetColumnByName("Age"));
            Assert.DoesNotContain("Age", grid.SortFilter.GetActiveColumnCriteria().Select(c => c.ColumnName));
            Assert.Single(grid.Rows, row => row.IsVisible);
        }

        [Fact]
        public void DataReader_IsMaterializedIntoAView()
        {
            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");

            using var reader = table.CreateDataReader();
            using var grid = new BeepGridPro();
            grid.DataSource = reader;
            grid.AutoGenerateColumns();

            Assert.Single(grid.Rows);
            Assert.NotNull(grid.GetColumnByName("Name"));
        }

        [Fact]
        public void DataSourceReplacement_RebuildsChangedSchema()
        {
            using var first = new DataTable("People");
            first.Columns.Add("Name", typeof(string));
            first.Rows.Add("Alice");
            using var second = new DataTable("Orders");
            second.Columns.Add("Total", typeof(int));
            second.Rows.Add(42);

            using var grid = new BeepGridPro();
            grid.DataSource = first;
            grid.AutoGenerateColumns();
            Assert.NotNull(grid.GetColumnByName("Name"));

            grid.DataSource = second;

            Assert.Null(grid.GetColumnByName("Name"));
            Assert.NotNull(grid.GetColumnByName("Total"));
            Assert.Equal(42, grid.GetCellValue(0, grid.GetColumnByName("Total")!.Index));
        }

        [Fact]
        public void DataMemberPropertyChange_RebuildsSchema()
        {
            using var dataSet = new DataSet();
            dataSet.Tables.Add("People").Columns.Add("Name", typeof(string));
            dataSet.Tables.Add("Orders").Columns.Add("Total", typeof(int));

            using var grid = new BeepGridPro();
            grid.DataMember = "People";
            grid.DataSource = dataSet;
            grid.AutoGenerateColumns();
            Assert.NotNull(grid.GetColumnByName("Name"));

            grid.DataMember = "Orders";

            Assert.Null(grid.GetColumnByName("Name"));
            Assert.NotNull(grid.GetColumnByName("Total"));
        }

        [Fact]
        public void BindingSourceDataMemberChange_RebuildsSchema()
        {
            using var dataSet = new DataSet();
            dataSet.Tables.Add("People").Columns.Add("Name", typeof(string));
            dataSet.Tables.Add("Orders").Columns.Add("Total", typeof(int));
            using var source = new BindingSource { DataSource = dataSet, DataMember = "People" };
            using var grid = new BeepGridPro();
            grid.DataSource = source;
            grid.AutoGenerateColumns();
            Assert.NotNull(grid.GetColumnByName("Name"));

            source.DataMember = "Orders";

            Assert.Null(grid.GetColumnByName("Name"));
            Assert.NotNull(grid.GetColumnByName("Total"));
        }

        [Fact]
        public void BindingSourceReplacement_RebuildsSchema()
        {
            using var first = new DataTable("People");
            first.Columns.Add("Name", typeof(string));
            first.Rows.Add("Alice");
            using var second = new DataTable("Orders");
            second.Columns.Add("Total", typeof(int));
            second.Rows.Add(42);

            using var source = new BindingSource { DataSource = first };
            using var grid = new BeepGridPro();
            grid.DataSource = source;
            grid.AutoGenerateColumns();
            Assert.NotNull(grid.GetColumnByName("Name"));

            source.DataSource = second;

            Assert.Null(grid.GetColumnByName("Name"));
            Assert.NotNull(grid.GetColumnByName("Total"));
            Assert.Equal(42, grid.GetCellValue(0, grid.GetColumnByName("Total")!.Index));
        }

        [Fact]
        public void DataViewManagerWithDataMember_BindsSelectedTable()
        {
            using var dataSet = new DataSet();
            var table = dataSet.Tables.Add("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");
            var manager = new DataViewManager(dataSet);

            using var grid = new BeepGridPro();
            grid.DataMember = "People";
            grid.DataSource = manager;
            grid.AutoGenerateColumns();

            Assert.Single(grid.Rows);
            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", table.Rows[0]["Name"]);
        }

        [Fact]
        public void BindingSourceWithDataMember_BindsAndWritesBack()
        {
            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");

            using var source = new BindingSource
            {
                DataSource = table,
                DataMember = string.Empty
            };
            using var grid = new BeepGridPro();
            grid.DataSource = source;
            grid.AutoGenerateColumns();

            Assert.Single(grid.Rows);
            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", table.Rows[0]["Name"]);
        }

        [Fact]
        public void SortAndFilter_WorkOnPocoAndDictionaryRows()
        {
            var people = new List<Person>
            {
                new() { Name = "Bob", Amount = 20 },
                new() { Name = "Alice", Amount = 10 }
            };
            using (var grid = Bind(people))
            {
                grid.SortFilter.Sort("Name", SortDirection.Ascending);
                Assert.Equal("Alice", ((Person)grid.Rows[0].RowData).Name);
                Assert.Equal("Bob", people[0].Name);

                grid.SortFilter.Filter("Name", "Bo");
                Assert.Single(grid.Rows, row => row.IsVisible);
            }

            var dictionaries = new List<Dictionary<string, object?>>
            {
                new(StringComparer.OrdinalIgnoreCase) { ["Name"] = "Bob" },
                new(StringComparer.OrdinalIgnoreCase) { ["Name"] = "Alice" }
            };
            using (var grid = Bind(dictionaries))
            {
                grid.SortFilter.Sort("Name", SortDirection.Descending);
                Assert.Equal("Bob", ((Dictionary<string, object?>)grid.Rows[0].RowData)["Name"]);

                grid.SortFilter.Filter("Name", "Ali");
                Assert.Single(grid.Rows, row => row.IsVisible);
            }
        }

        [Fact]
        public void DictionaryRows_UseKeysAsFieldsAndWriteBack()
        {
            var rows = new List<Dictionary<string, object?>>
            {
                new(StringComparer.OrdinalIgnoreCase) { ["Name"] = "Alice", ["Amount"] = 10 }
            };

            using var grid = Bind(rows);

            Assert.NotNull(grid.GetColumnByName("Name"));
            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", rows[0]["Name"]);
        }

        [Fact]
        public void TypedDictionaryRows_BindAndWrite()
        {
            var rows = new List<Dictionary<string, int>>
            {
                new() { ["Amount"] = 10 }
            };

            using var grid = Bind(rows);

            Assert.NotNull(grid.GetColumnByName("Amount"));
            Assert.Equal(10, grid.GetCellValue(0, grid.GetColumnByName("Amount")!.Index));
            Assert.True(grid.SetCellValue(0, "Amount", 42));
            Assert.Equal(42, rows[0]["Amount"]);
        }

        [Fact]
        public void DictionaryColumns_UnionKeysAcrossRows()
        {
            var rows = new List<Dictionary<string, object?>>
            {
                new(StringComparer.OrdinalIgnoreCase) { ["Name"] = "Alice" },
                new(StringComparer.OrdinalIgnoreCase) { ["Name"] = "Bob", ["Amount"] = 20 }
            };

            using var grid = Bind(rows);

            Assert.NotNull(grid.GetColumnByName("Amount"));
            Assert.Null(grid.GetCellValue(0, grid.GetColumnByName("Amount")!.Index));
            Assert.True(rows[1].TryGetValue("Amount", out var rawAmount));
            Assert.Equal(20, rawAmount);
            Assert.True(rows[1] is IDictionary<string, object?>);
            Assert.Equal(20, grid.Data.Binding!.GetValue(rows[1], "Amount"));
            var amountCell = grid.Rows[1].Cells.First(c => c.ColumnName == "Amount");
            Assert.Equal(20, amountCell.CellValue);
        }

        [Fact]
        public void HeterogeneousObjects_UnionTheirFields()
        {
            var rows = new List<object>
            {
                new Person { Name = "Alice", Amount = 10 },
                new ExtendedPerson { Name = "Bob", Amount = 20, Extra = "VIP" }
            };
            using var grid = Bind(rows);

            Assert.NotNull(grid.GetColumnByName("Extra"));
            var extra = grid.GetColumnByName("Extra")!;
            Assert.Equal("VIP", grid.GetCellValue(1, extra.Index));
        }

        [Fact]
        public void UnboundColumnAddedAfterBind_IsEditableLocally()
        {
            var people = new List<Person> { new() { Name = "Alice", Amount = 10 } };
            using var grid = Bind(people);

            var column = new TheTechIdea.Beep.Winform.Controls.Models.BeepColumnConfig
            {
                ColumnName = "Note",
                ColumnCaption = "Note",
                Width = 100,
                IsUnbound = true,
                Visible = true
            };
            grid.Columns.Add(column);
            grid.Data.SyncRowCellsToColumns();

            if (!grid.Rows[0].Cells.Any(c => c.ColumnName == "Note"))
                throw new InvalidOperationException($"Columns=[{string.Join(",", grid.Columns.Select(c => c.ColumnName))}] Cells=[{string.Join(",", grid.Rows[0].Cells.Select(c => c.ColumnName))}]");
            Assert.True(grid.SetCellValue(0, "Note", "local"));
            Assert.Equal("local", grid.GetCellValue(0, column.Index));
            Assert.Equal("Alice", people[0].Name);
        }

        [Fact]
        public void PublicFields_AreBoundAndWritable()
        {
            var rows = new List<FieldPerson> { new() { Name = "Alice", Amount = 10 } };
            using var grid = Bind(rows);

            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", rows[0].Name);
        }

        [Fact]
        public void ExpandoObject_IsTreatedAsDictionary()
        {
            dynamic row = new ExpandoObject();
            row.Name = "Alice";
            row.Amount = 10;
            var rows = new List<object> { row };

            using var grid = Bind(rows);

            Assert.NotNull(grid.GetColumnByName("Name"));
            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", ((IDictionary<string, object?>)row)["Name"]);
        }

        [Fact]
        public void ObservableCollection_RefreshesRows()
        {
            var rows = new ObservableCollection<Person> { new() { Name = "Alice", Amount = 10 } };
            using var grid = Bind(rows);

            rows.Add(new Person { Name = "Bob", Amount = 20 });

            Assert.Equal(2, grid.Rows.Count);
        }

        [Fact]
        public void SortAndFilter_WorkOnBindingListAndDataTable()
        {
            var bindingList = new BindingList<Person>
            {
                new() { Name = "Bob", Amount = 20 },
                new() { Name = "Alice", Amount = 10 }
            };
            using (var grid = Bind(bindingList))
            {
                grid.SortFilter.Sort("Name", SortDirection.Ascending);
                Assert.Equal("Alice", ((Person)grid.Rows[0].RowData).Name);

                grid.SortFilter.Filter("Name", "Bo");
                Assert.Single(grid.Rows, row => row.IsVisible);
            }

            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Bob");
            table.Rows.Add("Alice");
            using (var grid = Bind(table))
            {
                grid.SortFilter.Sort("Name", SortDirection.Ascending);
                Assert.Equal("Alice", grid.GetCellValue(0, grid.GetColumnByName("Name")!.Index));

                grid.SortFilter.Filter("Name", "Bo");
                Assert.Single(grid.Rows, row => row.IsVisible);
            }
        }

        [Fact]
        public void CollectionChanges_ReapplySortAndFilter()
        {
            var rows = new ObservableCollection<Person>
            {
                new() { Name = "Bob", Amount = 20 },
                new() { Name = "Alice", Amount = 10 }
            };
            using var grid = Bind(rows);
            grid.SortFilter.Sort("Name", SortDirection.Ascending);
            grid.SortFilter.Filter("Name", "A");

            rows.Add(new Person { Name = "Aaron", Amount = 5 });

            Assert.Equal(2, grid.Rows.Count(row => row.IsVisible));
            Assert.Equal("Aaron", ((Person)grid.Rows.First(row => row.IsVisible).RowData).Name);
        }

        [Fact]
        public void InotifyPropertyChanged_UpdatesCell()
        {
            var person = new NotifyingPerson { Name = "Alice", Amount = 10 };
            using var grid = Bind(new List<NotifyingPerson> { person });

            person.Name = "Alicia";

            var column = grid.GetColumnByName("Name")!;
            Assert.Equal("Alicia", grid.GetCellValue(0, column.Index));
        }

        [Fact]
        public void NullableProperty_CanBeCleared()
        {
            var rows = new List<NullablePerson> { new() { Amount = 10 } };
            using var grid = Bind(rows);

            Assert.True(grid.SetCellValue(0, "Amount", null));
            Assert.Null(rows[0].Amount);
        }

        [Fact]
        public void TypeConverterValues_AreAccepted()
        {
            var rows = new List<ConvertPerson> { new() { Location = new System.Drawing.Point(1, 2) } };
            using var grid = Bind(rows);

            Assert.True(grid.SetCellValue(0, "Location", "5, 6"));
            Assert.Equal(new System.Drawing.Point(5, 6), rows[0].Location);
        }

        [Fact]
        public void PrimitiveList_HasValueColumnAndWritesBackBySourceIndex()
        {
            var values = new List<int> { 3, 1, 2 };
            using var grid = Bind(values);

            var valueColumn = grid.GetColumnByName("Value");
            Assert.NotNull(valueColumn);

            grid.SortFilter.Sort("Value", SortDirection.Ascending);
            Assert.Equal(1, grid.GetCellValue(0, valueColumn!.Index));

            Assert.True(grid.SetCellValue(0, "Value", 9));
            Assert.Equal(new[] { 3, 9, 2 }, values);
        }

        [Fact]
        public void BindingListOfScalars_HasValueColumn()
        {
            var values = new BindingList<int> { 3, 1, 2 };
            using var grid = Bind(values);

            Assert.NotNull(grid.GetColumnByName("Value"));
            grid.SortFilter.Sort("Value", SortDirection.Ascending);
            Assert.Equal(1, grid.GetCellValue(0, grid.GetColumnByName("Value")!.Index));
        }

        [Fact]
        public void StringArray_IsBindable()
        {
            var values = new[] { "Bob", "Alice" };
            using var grid = Bind(values);

            Assert.NotNull(grid.GetColumnByName("Value"));
            grid.SortFilter.Filter("Value", "Ali");
            Assert.Single(grid.Rows, row => row.IsVisible);
        }

        [Fact]
        public void AsyncEnumerable_IsMaterialized()
        {
            using var grid = new BeepGridPro();
            grid.DataSource = GetAsyncRows();
            grid.AutoGenerateColumns();

            Assert.Equal(2, grid.Rows.Count);
            Assert.NotNull(grid.GetColumnByName("Name"));
        }

        private static async IAsyncEnumerable<Person> GetAsyncRows()
        {
            yield return new Person { Name = "Alice", Amount = 10 };
            await Task.Yield();
            yield return new Person { Name = "Bob", Amount = 20 };
        }

        [Fact]
        public void QueryableSource_IsMaterialized()
        {
            var rows = new[] { new Person { Name = "Alice", Amount = 10 } }.AsQueryable();
            using var grid = Bind(rows);

            Assert.Single(grid.Rows);
            Assert.NotNull(grid.GetColumnByName("Name"));
        }

        [Fact]
        public void ReadOnlyProperty_RejectsWrite()
        {
            var rows = new List<ReadOnlyPerson> { new("Alice", 10) };
            using var grid = Bind(rows);

            Assert.False(grid.SetCellValue(0, "Amount", 42));
            Assert.Equal(10, rows[0].Amount);
        }

        private static BeepGridPro Bind(object source)
        {
            var grid = new BeepGridPro { DataSource = source };
            grid.AutoGenerateColumns();
            return grid;
        }

        private sealed class NullablePerson
        {
            public int? Amount { get; set; }
        }

        private sealed class ConvertPerson
        {
            public System.Drawing.Point Location { get; set; }
        }

        private sealed class ExtendedPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
            public string Extra { get; set; } = string.Empty;
        }

        private sealed class FieldPerson
        {
            public string Name = string.Empty;
            public int Amount;
        }

        private sealed class Person
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
        }

        private sealed class NotifyingPerson : INotifyPropertyChanged
        {
            private string _name = string.Empty;
            private int _amount;

            public string Name
            {
                get => _name;
                set
                {
                    if (_name == value) return;
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }

            public int Amount
            {
                get => _amount;
                set
                {
                    if (_amount == value) return;
                    _amount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        private sealed class ReadOnlyPerson
        {
            public ReadOnlyPerson(string name, int amount)
            {
                Name = name;
                Amount = amount;
            }

            public string Name { get; set; }
            public int Amount { get; }
        }
    }
}
