# DataSource Compatibility Guide

This document describes all supported `DataSource` types for `BeepGridPro`, recommended binding patterns, and known gotchas.

## Supported DataSource Types

| Type | Supported | Recommended Pattern | Notes |
|---|---|---|---|
| `BindingSource` | ✅ Yes | `grid.DataSource = bindingSource;` | Full CRUD, sort, filter, position sync |
| `DataTable` / `DataView` | ✅ Yes | `grid.DataSource = dataTable;` | Fast cell-level refresh via `ItemChanged` |
| `List<T>` / `BindingList<T>` | ✅ Yes | `grid.DataSource = list;` | Use `BindingList<T>` for add/remove notifications |
| `ObservableCollection<T>` | ✅ Yes | `grid.DataSource = observableCollection;` | Live updates via `CollectionChanged` (Ph 3) |
| `DataSet` | ✅ Yes | `grid.DataSource = dataSet; grid.DataMember = "TableName";` | `DataMember` required |
| `IEnumerable<T>` | ✅ Yes | `grid.DataSource = enumerable;` | One-way read-only; no CRUD |
| `IUnitofWork` / `IUnitOfWorkWrapper` | ✅ Yes | `grid.Uow = unitOfWork;` | CRUD goes through UOW methods directly |
| `Virtual data (IVirtualDataSource)` | ✅ Yes | `grid.EnableVirtualization = true;` + `CreateVirtualDataSource()` | For 100K+ rows; windowed row materialization (Ph 13) |

## Binding Patterns

### BindingSource (most common)
```csharp
var bs = new BindingSource { DataSource = customerList };
grid.DataSource = bs;
grid.AutoGenerateColumns();
```
- Sorting uses `BindingSource.Sort` or `IBindingList.ApplySort` when available.
- Filtering uses `BindingSource.Filter` or `IBindingListView.Filter` when available.
- Position and `Current` changes are deduplicated to avoid double selection updates (Ph 7).

### DataTable (fast cell refresh)
```csharp
grid.DataSource = dataTable;
grid.AutoGenerateColumns();
```
- Cell edits trigger `ListChanged.ItemChanged` → targeted row repaint (Ph 4).
- Avoids full rebind on every cell change.

### ObservableCollection (live updates)
```csharp
grid.DataSource = observableCollection;
grid.AutoGenerateColumns();
```
- `Add`/`Remove`/`Reset` are handled via `INotifyCollectionChanged` (Ph 3).
- `Replace`/`Move` fall back to full rebind.

### UOW Mode
```csharp
grid.Uow = myUnitOfWork;
grid.AutoGenerateColumns();
```
- `InsertNew()`, `Save()`, `DeleteCurrent()` call UOW methods directly.
- `PostCommit` / `PostUpdate` sync cell values back to the grid (Ph 6).
- The UOW type must implement `IUnitofWork` or `IUnitOfWorkWrapper`.

### Virtual Mode
```csharp
grid.EnableVirtualization = true;
grid.VirtualRowCount = 100000;
grid.VirtualDataSource = GridVirtualDataSource.FromList(largeList);
```
- Only visible rows are materialized into `Data.Rows`.
- Scroll positions map to the virtual window (Ph 13).
- Grouping and filtering are supported but operate on the full virtual dataset.

## DataMember Resolution

`DataMember` is respected for:
- `DataSet` tables (`"TableName"`)
- `DataViewManager` views
- Root-object properties that return `IEnumerable`
- Nested `BindingSource` paths

If `DataMember` is set and the resolved data implements `IBindingList`, the grid binds to the list directly.

## Schema Change Detection (Ph 5)

When `BindingSource.DataSource` is replaced with a different schema:
1. `BindingSource_DataSourceChanged` fires.
2. Column names are compared against the new schema.
3. If the schema changed, `AutoGenerateColumns()` is called automatically.
4. If the schema is the same, only `RefreshRows()` is called.

## Gotchas and Workarounds

### Gotcha: Two filter systems exist
- **Public filter**: `ActiveFilter`, `ApplyQuickFilter`, `ShowAdvancedFilterDialog` — owned by `BeepGridPro.Filtering.cs`.
- **Internal filter**: `SortFilter` pipeline — owned by `GridSortFilterHelper` and used by header popup filtering.
- They share UI but NOT internal state. Do not assume `ActiveFilter` state reflects `SortFilter` state.

### Gotcha: `Uow` does not auto-wrap plain objects
- `Uow` stores `object?`, but only casts to `IUnitofWork` / `IUnitOfWorkWrapper`.
- If your runtime UOW does not implement those interfaces, wrap it externally before assignment.

### Gotcha: `GridLayoutPreset` enum is ahead of property wiring
- Some enum values (e.g., `Material3Surface`, `Fluent2Standard`) exist in the enum but are not wired in `ApplyLayoutPreset(GridLayoutPreset)`.
- Use the extension overload `ApplyLayoutPreset(new Material3SurfaceTableLayoutHelper())` directly.

### Gotcha: `BeepGridStyle.Modern` is not wired
- The enum exists in `TheTechIdea.Beep.Vis.Modules`.
- The `ApplyGridStyle()` switch has no `Modern` branch yet.

### Gotcha: `BindingSource.Sort` with grouping
- When grouping is active, sorting a group column recomputes groups.
- Sorting a non-group column sorts rows **within** each group.
- Global row reordering is suppressed while grouping is active (Ph 12.9).

## Best Practices

1. **Always call `AutoGenerateColumns()` after setting `DataSource`.**
2. **Use `BindingList<T>` instead of `List<T>` when you need add/remove notifications.**
3. **Set `DataMember` before `DataSource` when binding to `DataSet` or root objects.**
4. **Handle `CellValueChanged` to persist edits back to your data layer if not using `BindingSource`.**
5. **For very large datasets, enable virtualization before setting the data source.**

## Troubleshooting

| Symptom | Likely Cause | Fix |
|---|---|---|
| Grid stays empty after `DataSource = ...` | `AutoGenerateColumns()` not called | Call `AutoGenerateColumns()` |
| Edits do not persist | Not using `BindingSource` and no `CellValueChanged` handler | Handle `CellValueChanged` or use `BindingSource` |
| Sort does nothing | `BindingSource` not set or list does not support sorting | Use `BindingList<T>` or enable local sort |
| Filter does nothing | `BindingSource` does not support filtering | Use `DataView` or enable local filter |
| Group headers overlap rows | Grouping applied but scroll helper not updated | Ensure `GridScrollHelper` accounts for `GroupEngine.GetHeaderHeight()` |
