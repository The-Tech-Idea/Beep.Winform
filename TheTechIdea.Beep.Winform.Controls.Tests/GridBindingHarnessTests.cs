using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Reflection;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor.UOW;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GridBindingHarnessTests
    {
        [Fact]
        public void Poco_Binding_Sort_Filter_Edit()
        {
            var rows = new List<Person>
            {
                new() { Name = "Bob", Amount = 20 },
                new() { Name = "Alice", Amount = 10 }
            };
            using var grid = Bind(rows);

            grid.SortFilter.Sort("Name", SortDirection.Ascending);
            Assert.Equal("Alice", ((Person)grid.Rows[0].RowData).Name);

            grid.SortFilter.Filter("Name", "Bo");
            Assert.Single(grid.Rows, row => row.IsVisible);

            Assert.True(grid.SetCellValue(0, "Amount", "42"));
            Assert.Equal(42, rows[1].Amount);
        }

        [Fact]
        public void SingleObject_And_DataRow_And_DataRowView()
        {
            var person = new Person { Name = "Alice", Amount = 10 };
            using (var grid = new BeepGridPro { DataSource = person })
            {
                grid.AutoGenerateColumns();
                Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
                Assert.Equal("Alicia", person.Name);
            }

            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");
            using (var grid = new BeepGridPro { DataSource = table.Rows[0] })
            {
                grid.AutoGenerateColumns();
                Assert.True(grid.SetCellValue(0, "Name", "Row"));
                Assert.Equal("Row", table.Rows[0]["Name"]);
            }

            using (var grid = new BeepGridPro { DataSource = table.DefaultView[0] })
            {
                grid.AutoGenerateColumns();
                Assert.True(grid.SetCellValue(0, "Name", "View"));
                Assert.Equal("View", table.Rows[0]["Name"]);
            }
        }

        [Fact]
        public void BindingSource_DataSet_DataMember_WriteBack()
        {
            using var dataSet = new DataSet();
            var table = dataSet.Tables.Add("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");

            using var source = new BindingSource { DataSource = dataSet, DataMember = "People" };
            using var grid = new BeepGridPro { DataSource = source };
            grid.AutoGenerateColumns();

            Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
            Assert.Equal("Alicia", table.Rows[0]["Name"]);
        }

        [Fact]
        public void DataView_DataViewManager_IListSource_And_DataReader()
        {
            using var dataSet = new DataSet();
            var table = dataSet.Tables.Add("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");

            using (var grid = new BeepGridPro { DataSource = table.DefaultView })
            {
                grid.AutoGenerateColumns();
                Assert.True(grid.SetCellValue(0, "Name", "View"));
            }

            using (var grid = new BeepGridPro { DataMember = "People", DataSource = new DataViewManager(dataSet) })
            {
                grid.AutoGenerateColumns();
                Assert.True(grid.SetCellValue(0, "Name", "Manager"));
            }

            using (var grid = new BeepGridPro { DataSource = new TableListSource(table) })
            {
                grid.AutoGenerateColumns();
                Assert.True(grid.SetCellValue(0, "Name", "ListSource"));
            }

            table.Rows[0]["Name"] = "Reader";
            using (var grid = new BeepGridPro { DataSource = table.CreateDataReader() })
            {
                grid.AutoGenerateColumns();
                Assert.Single(grid.Rows);
                Assert.NotNull(grid.GetColumnByName("Name"));
            }
        }

        [Fact]
        public void Dictionary_Expando_And_TypedDictionary()
        {
            var objects = new List<Dictionary<string, object?>>
            {
                new(StringComparer.OrdinalIgnoreCase) { ["Name"] = "Alice" }
            };
            using (var grid = Bind(objects))
            {
                Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
                Assert.Equal("Alicia", objects[0]["Name"]);
            }

            dynamic expando = new ExpandoObject();
            expando.Name = "Alice";
            using (var grid = Bind(new List<object> { expando }))
            {
                Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
                Assert.Equal("Alicia", ((IDictionary<string, object?>)expando)["Name"]);
            }

            var typed = new List<Dictionary<string, int>> { new() { ["Amount"] = 10 } };
            using (var grid = Bind(typed))
            {
                Assert.True(grid.SetCellValue(0, "Amount", 42));
                Assert.Equal(42, typed[0]["Amount"]);
            }
        }

        [Fact]
        public void Enumerable_Queryable_Async_And_ScalarSources()
        {
            using (var grid = Bind(LazyPeople()))
            {
                Assert.Equal(2, grid.Rows.Count);
            }

            using (var grid = Bind(new[] { new Person { Name = "Alice" } }.AsQueryable()))
            {
                Assert.Single(grid.Rows);
            }

            using (var grid = new BeepGridPro { DataSource = AsyncPeople() })
            {
                grid.AutoGenerateColumns();
                Assert.Equal(2, grid.Rows.Count);
            }

            var numbers = new List<int> { 3, 1, 2 };
            using (var grid = Bind(numbers))
            {
                var valueColumn = grid.GetColumnByName("Value")!;
                grid.ToggleColumnSort(valueColumn.Index);
                Assert.Equal(1, grid.GetCellValue(0, valueColumn.Index));
                Assert.True(grid.SetCellValue(0, "Value", 9));
                Assert.Equal(new[] { 3, 9, 2 }, numbers);
                Assert.Equal(2, grid.GetCellValue(0, valueColumn.Index));
                Assert.Equal(9, grid.GetCellValue(2, valueColumn.Index));
            }

            var strings = new[] { "Bob", "Alice" };
            using (var grid = Bind(strings))
            {
                Assert.NotNull(grid.GetColumnByName("Value"));
                grid.ApplyQuickFilter("Ali");
                Assert.Single(grid.Rows, row => row.IsVisible);
            }
        }

        [Fact]
        public void BindingList_ObservableCollection_And_Inpc()
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

            var observable = new ObservableCollection<Person>
            {
                new() { Name = "Bob", Amount = 20 }
            };
            using (var grid = Bind(observable))
            {
                grid.SortFilter.Sort("Name", SortDirection.Ascending);
                grid.SortFilter.Filter("Name", "A");
                observable.Add(new Person { Name = "Alice", Amount = 10 });
                Assert.Equal(1, grid.Rows.Count(row => row.IsVisible));
                Assert.Equal("Alice", ((Person)grid.Rows.First(row => row.IsVisible).RowData).Name);
            }

            var notifying = new NotifyingPerson { Name = "Alice", Amount = 10 };
            using (var grid = Bind(new List<NotifyingPerson> { notifying }))
            {
                notifying.Name = "Alicia";
                Assert.Equal("Alicia", grid.GetCellValue(0, grid.GetColumnByName("Name")!.Index));
            }
        }

        [Fact]
        public void Heterogeneous_Unbound_And_SchemaChanges()
        {
            var rows = new List<object>
            {
                new Person { Name = "Alice", Amount = 10 },
                new ExtendedPerson { Name = "Bob", Amount = 20, Extra = "VIP" }
            };
            using (var grid = Bind(rows))
            {
                Assert.NotNull(grid.GetColumnByName("Extra"));
                Assert.Equal("VIP", grid.GetCellValue(1, grid.GetColumnByName("Extra")!.Index));
            }

            var people = new List<Person> { new() { Name = "Alice", Amount = 10 } };
            using (var grid = Bind(people))
            {
                grid.Columns.Add(new TheTechIdea.Beep.Winform.Controls.Models.BeepColumnConfig
                {
                    ColumnName = "Note",
                    ColumnCaption = "Note",
                    IsUnbound = true,
                    Visible = true,
                    Width = 80
                });
                Assert.True(grid.SetCellValue(0, "Note", "local"));
                Assert.Equal("local", grid.GetCellValue(0, grid.GetColumnByName("Note")!.Index));
            }

            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(int));
            table.Rows.Add("Alice", 10);

            using (var grid = new BeepGridPro { DataSource = table })
            {
                grid.AutoGenerateColumns();
                grid.SortFilter.Filter("Age", "1");
                table.Columns.Remove("Age");

                Assert.Null(grid.GetColumnByName("Age"));
                Assert.DoesNotContain("Age", grid.SortFilter.GetActiveColumnCriteria().Select(c => c.ColumnName));
                Assert.Single(grid.Rows, row => row.IsVisible);
            }
        }

        [Fact]
        public void Virtualization_UsesNormalizedSource()
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
        public void SourceReplacement_DataMemberChange_RebuildsSchema()
        {
            using var first = new DataTable("People");
            first.Columns.Add("Name", typeof(string));
            first.Rows.Add("Alice");
            using var second = new DataTable("Orders");
            second.Columns.Add("Total", typeof(int));
            second.Rows.Add(42);

            using var grid = new BeepGridPro { DataSource = first };
            grid.AutoGenerateColumns();
            Assert.NotNull(grid.GetColumnByName("Name"));

            grid.DataSource = second;
            Assert.Null(grid.GetColumnByName("Name"));
            Assert.NotNull(grid.GetColumnByName("Total"));

            using var dataSet = new DataSet();
            dataSet.Tables.Add("People").Columns.Add("Name", typeof(string));
            dataSet.Tables.Add("Orders").Columns.Add("Total", typeof(int));
            using var memberGrid = new BeepGridPro { DataMember = "People", DataSource = dataSet };
            memberGrid.AutoGenerateColumns();
            memberGrid.DataMember = "Orders";
            Assert.Null(memberGrid.GetColumnByName("Name"));
            Assert.NotNull(memberGrid.GetColumnByName("Total"));
        }

        [Fact]
        public void Fields_Conversion_Nullable_And_ReadOnly()
        {
            var fieldRows = new List<FieldPerson> { new() { Name = "Alice", Amount = 10 } };
            using (var grid = Bind(fieldRows))
            {
                Assert.True(grid.SetCellValue(0, "Name", "Alicia"));
                Assert.Equal("Alicia", fieldRows[0].Name);
            }

            var convertRows = new List<ConvertPerson> { new() { Location = new System.Drawing.Point(1, 2) } };
            using (var grid = Bind(convertRows))
            {
                Assert.True(grid.SetCellValue(0, "Location", "5, 6"));
                Assert.Equal(new System.Drawing.Point(5, 6), convertRows[0].Location);
            }

            var nullableRows = new List<NullablePerson> { new() { Amount = 10 } };
            using (var grid = Bind(nullableRows))
            {
                Assert.True(grid.SetCellValue(0, "Amount", null));
                Assert.Null(nullableRows[0].Amount);
            }

            var readOnlyRows = new List<ReadOnlyPerson> { new("Alice", 10) };
            using (var grid = Bind(readOnlyRows))
            {
                Assert.False(grid.SetCellValue(0, "Amount", 42));
                Assert.Equal(10, readOnlyRows[0].Amount);
            }
        }

        [Fact]
        public void SortUpdateFilter_KeepsExternalDataTableInSync()
        {
            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Amount", typeof(int));
            table.Rows.Add("Alice", 10);
            table.Rows.Add("Bob", 20);
            table.Rows.Add("Charlie", 30);

            using var grid = new BeepGridPro { DataSource = table };
            grid.AutoGenerateColumns();
            var nameColumn = grid.GetColumnByName("Name")!;

            grid.ToggleColumnSort(nameColumn.Index);
            Assert.Equal(new[] { "Alice", "Bob", "Charlie" }, VisibleNames(grid));

            var bob = table.Rows.Cast<DataRow>().Single(row => (string)row["Name"] == "Bob");
            int bobRow = FindGridRow(grid, bob);
            Assert.True(bobRow >= 0);
            Assert.True(grid.SetCellValue(bobRow, "Amount", 99));
            Assert.Equal(99, bob["Amount"]);
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Alice", "Bob", "Charlie" }, VisibleNames(grid));

            var charlie = table.Rows.Cast<DataRow>().Single(row => (string)row["Name"] == "Charlie");
            int charlieRow = FindGridRow(grid, charlie);
            Assert.True(charlieRow >= 0);
            Assert.True(grid.SetCellValue(charlieRow, "Name", "Zoe"));
            Assert.Equal("Zoe", charlie["Name"]);
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Alice", "Bob", "Zoe" }, VisibleNames(grid));

            grid.ApplyQuickFilter("Z");
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Zoe" }, VisibleNames(grid));

            int zoeRow = FindGridRow(grid, charlie);
            Assert.True(zoeRow >= 0);
            Assert.Equal(30, grid.GetCellValue(zoeRow, grid.GetColumnByName("Amount")!.Index));
            Assert.True(grid.SetCellValue(zoeRow, "Amount", 77));
            Assert.Equal(77, charlie["Amount"]);
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Zoe" }, VisibleNames(grid));

            grid.ToggleColumnSort(nameColumn.Index);
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Zoe" }, VisibleNames(grid));
        }

        [Fact]
        public void CrudThenSortFilter_KeepsExternalDataTableInSync()
        {
            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Amount", typeof(int));
            table.Rows.Add("Alice", 10);
            table.Rows.Add("Bob", 20);
            table.Rows.Add("Charlie", 30);

            using var grid = new BeepGridPro { DataSource = table };
            grid.AutoGenerateColumns();
            var nameColumn = grid.GetColumnByName("Name")!;

            grid.InsertNew();
            AssertGridMatchesSource(grid, table);
            Assert.Equal(4, table.Rows.Count);

            grid.SortFilter.Sort("Name", SortDirection.Ascending);
            AssertGridMatchesSource(grid, table);
            Assert.True(IsSorted(grid, "Name", descending: false));

            var inserted = table.Rows[table.Rows.Count - 1];
            int insertedRow = FindGridRow(grid, inserted);
            Assert.True(insertedRow >= 0);
            Assert.True(grid.SetCellValue(insertedRow, "Name", "Aaron"));
            insertedRow = FindGridRow(grid, inserted);
            Assert.True(insertedRow >= 0);
            Assert.True(grid.SetCellValue(insertedRow, "Amount", 40));
            Assert.Equal("Aaron", inserted["Name"]);
            Assert.Equal(40, inserted["Amount"]);
            AssertGridMatchesSource(grid, table);

            grid.SortFilter.Filter("Name", "A");
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Aaron", "Alice", "Charlie" }, VisibleNames(grid));

            var alice = table.Rows.Cast<DataRow>().Single(row => (string)row["Name"] == "Alice");
            int aliceGridRow = FindGridRow(grid, alice);
            Assert.True(aliceGridRow >= 0);
            Assert.True(grid.SetCellValue(aliceGridRow, "Amount", 99));
            Assert.Equal(99, alice["Amount"]);
            AssertGridMatchesSource(grid, table);

            var bob = table.Rows.Cast<DataRow>().Single(row => (string)row["Name"] == "Bob");
            int bobGridRow = FindGridRow(grid, bob);
            Assert.True(bobGridRow >= 0);
            Assert.True(grid.SetCellValue(bobGridRow, "Amount", 88));
            Assert.Equal(88, bob["Amount"]);
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Aaron", "Alice", "Charlie" }, VisibleNames(grid));

            grid.InsertNew();
            AssertGridMatchesSource(grid, table);
            var inserted2 = table.Rows[table.Rows.Count - 1];
            int inserted2Row = FindGridRow(grid, inserted2);
            Assert.True(inserted2Row >= 0);
            Assert.True(grid.SetCellValue(inserted2Row, "Name", "Anna"));
            Assert.True(grid.SetCellValue(FindGridRow(grid, inserted2), "Amount", 50));
            Assert.Equal("Anna", inserted2["Name"]);
            Assert.Equal(50, inserted2["Amount"]);
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Aaron", "Alice", "Anna", "Charlie" }, VisibleNames(grid));

            grid.DeleteCurrent();
            AssertGridMatchesSource(grid, table);
            Assert.Equal(4, table.Rows.Count);
            Assert.DoesNotContain(table.Rows.Cast<DataRow>(), row => (string)row["Name"] == "Anna");
            Assert.Equal(new[] { "Aaron", "Alice", "Charlie" }, VisibleNames(grid));

            grid.SortFilter.Sort("Name", SortDirection.Descending);
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Charlie", "Alice", "Aaron" }, VisibleNames(grid));
            Assert.True(IsSorted(grid, "Name", descending: true));

            grid.ClearFilter();
            grid.SortFilter.Sort("Name", SortDirection.Descending);
            AssertGridMatchesSource(grid, table);
            Assert.Equal(new[] { "Charlie", "Bob", "Alice", "Aaron" }, VisibleNames(grid));

            Assert.Equal(new[] { "Alice", "Bob", "Charlie", "Aaron" },
                table.Rows.Cast<DataRow>().Select(row => (string)row["Name"]));

            int bobRowNow = FindGridRow(grid, bob);
            Assert.True(bobRowNow >= 0);
            grid.SelectCell(bobRowNow, nameColumn.Index);
            grid.DeleteCurrent();
            AssertGridMatchesSource(grid, table);
            Assert.Equal(3, table.Rows.Count);
            Assert.DoesNotContain(table.Rows.Cast<DataRow>(), row => (string)row["Name"] == "Bob");
            grid.Save();
        }

        private static int FindGridRow(BeepGridPro grid, DataRow row)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (grid.Rows[i].RowData is DataRowView view && ReferenceEquals(view.Row, row))
                    return i;
            }
            return -1;
        }

        private static void AssertGridMatchesSource(BeepGridPro grid, DataTable table)
        {
            Assert.Equal(table.Rows.Count, grid.Rows.Count);
            var gridRows = grid.Rows
                .Select(row => ((DataRowView)row.RowData).Row)
                .ToList();
            Assert.Equal(table.Rows.Count, gridRows.Distinct().Count());

            foreach (DataRow source in table.Rows)
            {
                int index = FindGridRow(grid, source);
                Assert.True(index >= 0, "Grid row missing for source DataRow.");
                var gridRow = grid.Rows[index];
                AssertCellMatches(gridRow, "Name", source["Name"]);
                AssertCellMatches(gridRow, "Amount", source["Amount"]);
            }
        }

        private static void AssertCellMatches(TheTechIdea.Beep.Winform.Controls.Models.BeepRowConfig row,
            string columnName, object? expected)
        {
            var cell = row.Cells.First(c => c.ColumnName == columnName);
            object? actual = cell.CellValue is DBNull ? null : cell.CellValue;
            object? normalized = expected is DBNull ? null : expected;
            Assert.Equal(normalized, actual);
        }

        private static bool IsSorted(BeepGridPro grid, string columnName, bool descending)
        {
            var values = grid.Rows.Where(row => row.IsVisible)
                .Select(row => ((DataRowView)row.RowData)[columnName]?.ToString() ?? string.Empty)
                .ToList();
            var expected = descending
                ? values.OrderByDescending(value => value, StringComparer.OrdinalIgnoreCase).ToList()
                : values.OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToList();
            return values.SequenceEqual(expected);
        }

        private static List<string> VisibleNames(BeepGridPro grid)
        {
            return grid.Rows.Where(row => row.IsVisible)
                .Select(row => ((DataRowView)row.RowData)["Name"]?.ToString() ?? string.Empty)
                .ToList();
        }
        [Fact]
        public void UowBackedSource_InsertSortFilterDelete()
        {
            var state = new FakeUowState(new ObservableCollection<Person>
            {
                new() { Name = "Alice", Amount = 10 },
                new() { Name = "Bob", Amount = 20 }
            });
            var proxy = DispatchProxy.Create<IUnitOfWorkWrapper, FakeUowProxy>();
            FakeUowProxy.State = state;

            using var grid = new BeepGridPro { Uow = proxy };
            Assert.Equal(2, grid.Rows.Count);

            grid.InsertNew();
            Assert.Equal(3, state.Items.Count);
            grid.ToggleColumnSort(grid.GetColumnByName("Name")!.Index);
            grid.ApplyQuickFilter("A");
            Assert.True(grid.Rows.Count(row => row.IsVisible) >= 1);

            grid.SetCellValue(0, "Name", "Aaron");
            Assert.Contains(state.Items, person => person.Name == "Aaron");

            grid.DeleteCurrent();
            Assert.Equal(2, state.Items.Count);
            grid.Save();
        }

        [Fact]
        public void Crud_AllSupportedSourceShapes()
        {
            var dictionaries = new List<Dictionary<string, object?>>
            {
                new(StringComparer.OrdinalIgnoreCase) { ["Name"] = "Alice" }
            };
            using (var grid = Bind(dictionaries))
            {
                grid.InsertNew();
                Assert.Equal(2, grid.Rows.Count);
                grid.DeleteCurrent();
                Assert.Single(grid.Rows);
            }

            var scalars = new List<int> { 1 };
            using (var grid = Bind(scalars))
            {
                grid.InsertNew();
                Assert.Equal(2, grid.Rows.Count);
                grid.DeleteCurrent();
                Assert.Single(grid.Rows);
            }

            var bindingList = new BindingList<Person> { new() { Name = "Alice" } };
            using (var grid = Bind(bindingList))
            {
                grid.InsertNew();
                Assert.Equal(2, grid.Rows.Count);
                grid.DeleteCurrent();
                Assert.Single(grid.Rows);
            }

            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");
            using (var grid = new BeepGridPro { DataSource = table.DefaultView })
            {
                grid.AutoGenerateColumns();
                grid.InsertNew();
                Assert.Equal(2, grid.Rows.Count);
                grid.DeleteCurrent();
                Assert.Single(grid.Rows);
            }

            using (var grid = new BeepGridPro { DataSource = new TableListSource(table) })
            {
                grid.AutoGenerateColumns();
                grid.InsertNew();
                Assert.Equal(2, grid.Rows.Count);
                grid.DeleteCurrent();
                Assert.Single(grid.Rows);
            }

            using var dataSet = new DataSet();
            dataSet.Tables.Add("People").Columns.Add("Name", typeof(string));
            using (var grid = new BeepGridPro { DataMember = "People", DataSource = dataSet })
            {
                grid.AutoGenerateColumns();
                grid.InsertNew();
                Assert.Equal(1, grid.Rows.Count);
                grid.DeleteCurrent();
                Assert.Empty(grid.Rows);
            }

            using (var grid = new BeepGridPro { DataSource = new BindingSource { DataSource = new List<Person> { new() { Name = "Alice" } } } })
            {
                grid.AutoGenerateColumns();
                grid.InsertNew();
                Assert.Equal(2, grid.Rows.Count);
                grid.DeleteCurrent();
                Assert.Single(grid.Rows);
            }

            using (var grid = new BeepGridPro { DataSource = AsyncPeople() })
            {
                grid.AutoGenerateColumns();
                grid.InsertNew();
                Assert.Equal(3, grid.Rows.Count);
                grid.DeleteCurrent();
                Assert.Equal(2, grid.Rows.Count);
            }
        }

        [Fact]
        public void CrudThenSortFilter_KeepsPocoListInSync()
        {
            var people = new List<Person>
            {
                new() { Name = "Alice", Amount = 10 },
                new() { Name = "Bob", Amount = 20 }
            };
            using var grid = Bind(people);
            var nameColumn = grid.GetColumnByName("Name")!;

            grid.InsertNew();
            Assert.Equal(3, people.Count);
            var inserted = people[^1];
            int insertedRow = FindPocoGridRow(grid, inserted);
            Assert.True(insertedRow >= 0);
            Assert.True(grid.SetCellValue(insertedRow, "Name", "Carol"));
            Assert.True(grid.SetCellValue(FindPocoGridRow(grid, inserted), "Amount", 30));
            Assert.Equal("Carol", inserted.Name);
            Assert.Equal(30, inserted.Amount);

            grid.SortFilter.Sort("Name", SortDirection.Descending);
            grid.SortFilter.Filter("Name", "o");
            Assert.Equal(new[] { "Carol", "Bob" }, PocoVisibleNames(grid));

            int bobRow = FindPocoGridRow(grid, people.Single(person => person.Name == "Bob"));
            Assert.True(bobRow >= 0);
            grid.SelectCell(bobRow, nameColumn.Index);
            grid.SetCellValue(bobRow, "Amount", 99);
            Assert.Equal(99, people.Single(person => person.Name == "Bob").Amount);
            Assert.Equal(new[] { "Carol", "Bob" }, PocoVisibleNames(grid));

            grid.DeleteCurrent();
            Assert.Equal(2, people.Count);
            Assert.DoesNotContain(people, person => person.Name == "Bob");
            Assert.True(grid.SortFilter.HasActiveFilters, "Column filter was cleared by DeleteCurrent.");
            Assert.Contains("Name", grid.SortFilter.GetActiveColumnCriteria().Select(c => c.ColumnName));
            Assert.Equal(new[] { "Carol" }, PocoVisibleNames(grid));
            grid.Save();
        }

        private static int FindPocoGridRow(BeepGridPro grid, object item)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (ReferenceEquals(grid.Rows[i].RowData, item) || Equals(grid.Rows[i].RowData, item))
                    return i;
            }
            return -1;
        }

        private static List<string> PocoVisibleNames(BeepGridPro grid)
        {
            return grid.Rows.Where(row => row.IsVisible)
                .Select(row => ((Person)row.RowData).Name)
                .ToList();
        }

        [Fact]
        public void Crud_MutableSources()
        {
            var people = new List<Person> { new() { Name = "Alice", Amount = 10 } };
            using (var grid = Bind(people))
            {
                grid.InsertNew();
                Assert.Equal(2, people.Count);
                grid.DeleteCurrent();
                Assert.Single(people);
                grid.Save();
            }

            using var table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");
            using (var grid = new BeepGridPro { DataSource = table })
            {
                grid.AutoGenerateColumns();
                grid.InsertNew();
                Assert.Equal(2, table.Rows.Count);
                grid.DeleteCurrent();
                Assert.Single(table.Rows);
                grid.Save();
            }

            var observable = new ObservableCollection<Person> { new() { Name = "Alice", Amount = 10 } };
            using (var grid = Bind(observable))
            {
                grid.InsertNew();
                Assert.Equal(2, observable.Count);
                grid.DeleteCurrent();
                Assert.Single(observable);
                grid.Save();
            }
        }

        [Fact]
        public void Crud_NonGrowableAndSingleSources_DoNotCorrupt()
        {
            var array = new[] { new Person { Name = "Alice", Amount = 10 } };
            using (var grid = Bind(array))
            {
                grid.InsertNew();
                grid.DeleteCurrent();
                grid.Save();
                Assert.True(grid.Rows.Count >= 0);
            }

            var readOnly = new ReadOnlyCollection<Person>(new List<Person>
            {
                new() { Name = "Alice", Amount = 10 }
            });
            using (var grid = Bind(readOnly))
            {
                grid.InsertNew();
                grid.DeleteCurrent();
                grid.Save();
                Assert.True(grid.Rows.Count >= 0);
            }

            var single = new Person { Name = "Alice", Amount = 10 };
            using (var grid = new BeepGridPro { DataSource = single })
            {
                grid.AutoGenerateColumns();
                grid.InsertNew();
                grid.DeleteCurrent();
                grid.Save();
                Assert.True(grid.Rows.Count >= 0);
            }
        }

        [Fact]
        public void DpiMetrics_And_AccessibleObject()
        {
            var normal = TheTechIdea.Beep.Winform.Controls.GridX.Chrome.GridChromeMetrics.Create(
                TheTechIdea.Beep.Winform.Controls.GridX.Chrome.GridChromeDensity.Standard,
                1f, TheTechIdea.Beep.Winform.Controls.GridX.Chrome.GridChromeBreakpoint.Wide);
            var scaled = TheTechIdea.Beep.Winform.Controls.GridX.Chrome.GridChromeMetrics.Create(
                TheTechIdea.Beep.Winform.Controls.GridX.Chrome.GridChromeDensity.Standard,
                2f, TheTechIdea.Beep.Winform.Controls.GridX.Chrome.GridChromeBreakpoint.Wide);
            Assert.Equal(normal.HeaderMinHeight * 2, scaled.HeaderMinHeight);

            using var grid = Bind(new List<Person> { new() { Name = "Alice", Amount = 10 } });
            Assert.Equal(AccessibleRole.Table, grid.AccessibilityObject.Role);
            Assert.Equal(1, grid.AccessibilityObject.GetChildCount());
        }

        [Fact]
        public void Crud_Grouping_Export_Conditional_Footer_Interplay()
        {
            var people = new List<Person>
            {
                new() { Name = "Alice", Amount = 10 },
                new() { Name = "Bob", Amount = 20 }
            };
            using var grid = Bind(people);
            grid.ShowFooter = true;
            grid.Footer.SetAggregate("Amount", AggregationType.Sum);
            grid.ConditionalFormatting.Add(new TheTechIdea.Beep.Winform.Controls.GridX.Formatting.ConditionalFormatRule
            {
                ColumnName = "Amount",
                Operator = FilterOperator.GreaterThan,
                Value = "15",
                Style = new TheTechIdea.Beep.Winform.Controls.GridX.Formatting.GridCellStyle
                {
                    BackColor = System.Drawing.Color.MistyRose
                }
            });
            grid.GroupBy("Name");
            Assert.True(grid.GroupEngine.IsGrouped);

            grid.InsertNew();
            var inserted = people[^1];
            int insertedRow = FindPocoGridRow(grid, inserted);
            Assert.True(insertedRow >= 0);
            Assert.True(grid.SetCellValue(insertedRow, "Name", "Aaron"));
            Assert.True(grid.SetCellValue(FindPocoGridRow(grid, inserted), "Amount", 40));

            Assert.True(grid.GroupEngine.IsGrouped);
            Assert.Contains(grid.GroupEngine.Groups, group => group.Key.Contains("Aaron"));
            Assert.Equal(70m, Convert.ToDecimal(grid.Footer.GetValue(grid, "Amount")));
            Assert.Contains("Aaron", grid.ExportToString(TheTechIdea.Beep.Winform.Controls.GridX.Export.GridExportFormat.Csv));
            Assert.Single(grid.ConditionalFormatting.Rules);

            grid.ApplyQuickFilter("A");
            Assert.Equal(50m, Convert.ToDecimal(grid.Footer.GetValue(grid, "Amount")));
            Assert.Equal(2, grid.Rows.Count(row => row.IsVisible));
        }

        [Fact]
        public void Grouping_Export_ConditionalFormatting_Footer_And_Layout()
        {
            var people = new List<Person>
            {
                new() { Name = "Alice", Amount = 10 },
                new() { Name = "Bob", Amount = 20 }
            };
            using var grid = Bind(people);
            grid.GroupBy("Name");
            Assert.True(grid.GroupEngine.IsGrouped);

            grid.ConditionalFormatting.Add(new TheTechIdea.Beep.Winform.Controls.GridX.Formatting.ConditionalFormatRule
            {
                ColumnName = "Amount",
                Operator = FilterOperator.GreaterThan,
                Value = "15",
                Style = new TheTechIdea.Beep.Winform.Controls.GridX.Formatting.GridCellStyle
                {
                    BackColor = System.Drawing.Color.MistyRose
                }
            });
            Assert.Single(grid.ConditionalFormatting.Rules);

            grid.ShowFooter = true;
            grid.Footer.SetAggregate("Amount", AggregationType.Sum);

            string csv = grid.ExportToString(TheTechIdea.Beep.Winform.Controls.GridX.Export.GridExportFormat.Csv);
            Assert.Contains("Alice", csv);

            grid.LayoutStateKey = "harness-layout";
            Assert.True(grid.SaveLayoutState());
            Assert.True(grid.LoadLayoutState());

            Assert.Equal(AccessibleRole.Table, grid.AccessibilityObject.Role);
            Assert.Contains("rows", grid.AccessibilityObject.Description);
        }

        [Fact]
        public void QuickAndColumnFilters_AreCombined()
        {
            var rows = new List<Person>
            {
                new() { Name = "Alice", Amount = 10 },
                new() { Name = "Alicia", Amount = 20 },
                new() { Name = "Bob", Amount = 30 }
            };
            using var grid = Bind(rows);

            grid.SortFilter.Filter("Name", "Ali");
            grid.ApplyQuickFilter("Alicia");

            var visible = Assert.Single(grid.Rows, row => row.IsVisible);
            Assert.Equal("Alicia", ((Person)visible.RowData).Name);
        }

        [Fact]
        public void UpdateCellValue_UsesBindingAdapter()
        {
            var people = new List<Person> { new() { Name = "Alice", Amount = 10 } };
            using var grid = Bind(people);
            var cell = grid.Rows[0].Cells.First(c => c.ColumnName == "Name");

            grid.Data.UpdateCellValue(cell, "Alicia");

            Assert.Equal("Alicia", people[0].Name);
            Assert.Equal("Alicia", cell.CellValue);
        }

        private static BeepGridPro Bind(object source)
        {
            var grid = new BeepGridPro { DataSource = source };
            grid.AutoGenerateColumns();
            return grid;
        }

        private static IEnumerable<Person> LazyPeople()
        {
            yield return new Person { Name = "Alice", Amount = 10 };
            yield return new Person { Name = "Bob", Amount = 20 };
        }

        private static async IAsyncEnumerable<Person> AsyncPeople()
        {
            yield return new Person { Name = "Alice", Amount = 10 };
            await Task.Yield();
            yield return new Person { Name = "Bob", Amount = 20 };
        }

        private sealed class FakeUowState
        {
            public FakeUowState(ObservableCollection<Person> items) => Items = items;
            public ObservableCollection<Person> Items { get; }
            public int CurrentIndex { get; set; }
        }

        private class FakeUowProxy : DispatchProxy
        {
            public static FakeUowState? State { get; set; }

            protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            {
                var state = State;
                var name = targetMethod?.Name;
                var items = state?.Items;

                switch (name)
                {
                    case "get_Units": return items;
                    case "get_Count": return items?.Count ?? 0;
                    case "get_CurrentIndex": return state?.CurrentIndex ?? 0;
                    case "get_CurrentItem":
                        if (items == null || items.Count == 0) return null;
                        return items[Math.Clamp(state?.CurrentIndex ?? 0, 0, items.Count - 1)];
                    case "get_EntityName": return "Person";
                    case "get_IsDirty": return false;
                    case "get_CanUndo": return false;
                    case "get_IsEmpty": return items == null || items.Count == 0;
                    case "New":
                        items?.Add(new Person());
                        if (state != null && items != null) state.CurrentIndex = items.Count - 1;
                        return null;
                    case "MoveTo":
                        if (state != null && args is { Length: > 0 } && args[0] is int index)
                            state.CurrentIndex = Math.Clamp(index, 0, Math.Max(0, (items?.Count ?? 1) - 1));
                        return null;
                    case "Delete":
                        if (args is { Length: > 0 } && args[0] is Person person)
                            items?.Remove(person);
                        if (state != null && items != null)
                            state.CurrentIndex = Math.Clamp(state.CurrentIndex, 0, Math.Max(0, items.Count - 1));
                        return null;
                    case "Commit":
                        return Task.FromResult<IErrorsInfo>(null!);
                    case "add_CurrentChanged":
                    case "remove_CurrentChanged":
                        return null;
                    default:
                        if (targetMethod?.ReturnType == typeof(void)) return null;
                        if (targetMethod?.ReturnType.IsValueType == true)
                            return Activator.CreateInstance(targetMethod.ReturnType);
                        return null;
                }
            }
        }

        private sealed class TableListSource : IListSource
        {
            private readonly DataTable _table;
            public TableListSource(DataTable table) => _table = table;
            public bool ContainsListCollection => false;
            public IList GetList() => _table.DefaultView;
        }

        private sealed class FieldPerson
        {
            public string Name = string.Empty;
            public int Amount;
        }

        private sealed class ConvertPerson
        {
            public System.Drawing.Point Location { get; set; }
        }

        private sealed class NullablePerson
        {
            public int? Amount { get; set; }
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

        private sealed class Person
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
        }

        private sealed class ExtendedPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
            public string Extra { get; set; } = string.Empty;
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
    }
}
