# P4 — Data Binding Revision

## Scope

Revise `BeepGridPro` binding so one adapter normalizes common .NET data sources,
generates schema/columns consistently, keeps collection and item changes live,
and writes edited values back to the original item.

## Supported sources

| Source | Normalized form | Write-back target |
| --- | --- | --- |
| `BindingSource` | `BindingSource.List` (or its resolved source) | original list item / `DataRowView` |
| `DataSet` + `DataMember` | selected `DataView` | `DataRowView` |
| `DataTable` | `DefaultView` | `DataRowView` |
| `DataView` | itself | `DataRowView` |
| `DataViewManager` | selected/default table view | `DataRowView` |
| `IListSource` | `GetList()` | list item |
| `IDataReader` | loaded `DataTable.DefaultView` | `DataRowView` |
| `IList`, `IBindingList`, `INotifyCollectionChanged` | itself | item / dictionary / property |
| `IEnumerable`, `IQueryable`, `IAsyncEnumerable` | materialized list | item / dictionary / property |
| single object | one-item list | item / dictionary / property |
| `DataRow`, `DataRowView` | row items | row column |
| scalar/enum values | synthetic `Value` field | source list index |
| `IDictionary`, `ExpandoObject` | key-based fields | dictionary key |
| POCO / `ICustomTypeDescriptor` / `ITypedList` | descriptor/property fields | property/descriptor setter |

`IAsyncEnumerable<T>` is materialized synchronously before binding so the same
local sort/filter/write path applies.

## Uniform feature behavior

Sort and filter are evaluated over the grid row view for every source type.
The grid does not disable or skip them because an `IBindingList`/`IBindingListView`
reports `SupportsSorting`/`SupportsFiltering` as false. ADO.NET, POCO, dictionary,
and UoW items all use the same local query path.

## Two-way behavior

- `BeepGridPro.SetCellValue(rowIndex, columnName, value)` writes through the
  adapter, updates the cell, marks the row dirty, and raises `CellValueChanged`.
- Inline editor commits use the same adapter (`GridEditHelper.CommitValueToData`).
- `INotifyPropertyChanged` item updates refresh the matching cell.
- `BindingSource.DataSourceChanged` and `DataMemberChanged` rebuild the adapter and schema.
- Changing `BeepGridPro.DataMember` refreshes the normalized binding and columns.
- Dictionary and `DataRow`/`DataRowView` writes update the original row.
- Read-only properties/columns and failed conversions return `false` and report
  through `GridErrorReporter`; the editor restores the original cell value.
- `RaiseResetItem` asks a supporting `IBindingList` to refresh the edited item
  when the item does not implement `INotifyPropertyChanged`.

## Files

- `GridX/Binding/GridDataBinding.cs` — normalization, schema, read/write access.
- `GridX/Helpers/GridDataHelper.cs` — binds through the adapter, schema columns.
- `GridX/Helpers/GridDataHelper.Rows.cs` — row materialization and item-change sync.
- `GridX/Helpers/GridEditHelper.cs` — edit commit through the adapter.
- `GridX/Controllers/GridDataController.cs` — navigator gets the normalized list.
- `GridX/BeepGridPro.DataAccess.cs` — public `SetCellValue` / `GetCellValue`.

## Verification

- `GridBindingHarnessTests` is the focused grid-only harness. It covers POCO,
  single object, DataRow/DataRowView, BindingSource/DataSet/DataMember, DataView,
  DataViewManager, IListSource, IDataReader, dictionaries, ExpandoObject, scalar
  lists/arrays, IEnumerable, IQueryable, IAsyncEnumerable, BindingList,
  ObservableCollection, INotifyPropertyChanged, heterogeneous objects, public
  fields, TypeConverter values, nullable values, read-only rejection, runtime
  unbound columns, DataTable schema changes, virtualization, sort, column
  filter, quick filter, combined filters, SetCellValue, UpdateCellValue, and
  collection-change reapplication, and DataTable/POCO CRUD→sort→filter→CRUD→sort sequences plus sort→update→filter→update→sort checks that verify the external source still contains the correct inserted, updated, and deleted records.
- Harness project: `C:\Users\f_ald\AppData\Local\Temp\opencode\grid-harness\GridBindingHarness.csproj`.
- Harness result: 23/23 passed. The full repository suite is intentionally not run.
- WinForms app harness: `C:\Users\f_ald\AppData\Local\Temp\opencode\grid-winforms-harness\GridWinFormsHarness.csproj`.
  It creates real forms for POCO, DataTable, dictionary, scalar, heterogeneous,
  async, virtualized, and unbound-column scenarios and writes screenshots to
  `C:\Users\f_ald\AppData\Local\Temp\opencode\winforms-shots`. The app harness also verifies clipboard copy and opening the
  in-place editor. All eleven app scenarios rendered without exceptions; scalar sort-after-edit
  and dictionary write-back were visually confirmed.

## Limitations

- `IAsyncEnumerable<T>` is materialized synchronously; very large async streams block the caller during binding.
- No automatic `IEditableObject` transaction; hosts can cancel via cell events.
- For dictionaries, the first non-null row determines the generated columns.
