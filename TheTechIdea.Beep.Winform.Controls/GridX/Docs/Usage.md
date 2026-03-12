# Using BeepGridPro

## Binding

### Standard sources
`BeepGridPro` can bind to:
- `BindingSource`
- `DataTable`
- `DataView`
- `DataSet` with `DataMember`
- `DataViewManager` with `DataMember`
- `IEnumerable<T>`
- a root object whose property named by `DataMember` returns a collection

```csharp
grid.DataSource = customersBindingSource;
grid.DataMember = "";
grid.AutoGenerateColumns();
```

### DataMember resolution
`GridDataHelper.ResolveDataForBinding()` does this in order:
1. unwrap `BindingSource`
2. resolve `DataSet`/`DataViewManager` tables by `DataMember`
3. reflect a property on the root object using `DataMember`
4. fall back to the original object

### Clearing data
```csharp
grid.DataSource = null;
```
This clears rows, removes non-system columns, unbinds the navigator, recalculates layout, and clears selection.

## UOW Mode

Assign `Uow` when the screen uses Beep's unit-of-work contracts:
```csharp
grid.Uow = myUnitOfWork;
```

Important:
- `Uow` currently expects `IUnitofWork` or `IUnitOfWorkWrapper`.
- `BeepGridPro` does not auto-wrap arbitrary runtime `UnitOfWork<T>` instances.
- While `Uow` is active, `DataSource` is retained only as fallback state.

CRUD behavior in UOW mode:
- `InsertNew()` -> `New()`
- `DeleteCurrent()` -> `Delete(...)`
- `Save()` -> `Commit()`
- `Cancel()` -> `Rollback()`

## Columns

### Auto-generated columns
`AutoGenerateColumns()`:
- clears current columns
- re-adds system columns
- inspects a `DataTable` schema or item properties
- assigns default editor types and data categories

### System columns
`EnsureSystemColumns()` maintains:
- `Sel`: sticky checkbox column
- `RowNum`: sticky row-number column
- `RowID`: sticky hidden identity column

### Common column settings
```csharp
var nameColumn = grid.GetColumnByName("Name");
nameColumn.Width = 180;
nameColumn.MinWidth = 100;
nameColumn.MaxWidth = 260;
nameColumn.AllowAutoSize = true;
nameColumn.FillWeight = 2f;
nameColumn.Sticked = true;
nameColumn.AllowReorder = false;
```

Useful `BeepColumnConfig` members:
- `ColumnName`, `ColumnCaption`
- `Width`, `MinWidth`, `MaxWidth`, `FillWeight`, `AllowAutoSize`
- `Visible`, `ReadOnly`, `AllowSort`, `AllowFilter`
- `DisplayOrder`, `AllowReorder`, `Sticked`
- `CellEditor`, `Items`, `EnumSourceType`

## Selection

There are two distinct selection concepts:
- active-cell focus, tracked by `GridSelectionHelper`
- row checkbox selection, tracked through `BeepRowConfig.IsSelected`

Programmatic selection:
```csharp
grid.SelectCell(3, 1);
```

Useful properties:
- `CurrentRow`
- `CurrentRowIndex`
- `SelectedRows`
- `SelectedRowIndices`

Example:
```csharp
var city = grid.CurrentRow?.Cells["City"]?.Value?.ToString();
```

## Editing

### Start editing
```csharp
grid.ShowCellEditor();
```

Keyboard shortcuts:
- `F2` starts editing the active editable cell
- `Enter` also starts editing when the cell is editable

### Cell editor mapping
The grid uses `BeepColumnConfig.CellEditor` to decide which editor or renderer to use. Common values include:
- `Text`
- `NumericUpDown`
- `DateTime`
- `CheckBoxBool`
- `ComboBox`
- `Button`
- `ListBox`
- `ListOfValue`
- `ProgressBar`
- `Custom`

Value commits flow through `GridDataHelper.UpdateCellValue()` so both the grid cell and the underlying row object are updated.

## Navigation and CRUD

```csharp
grid.MoveFirst();
grid.MovePrevious();
grid.MoveNext();
grid.MoveLast();
grid.InsertNew();
grid.DeleteCurrent();
grid.Save();
grid.Cancel();
```

External navigator support:
```csharp
grid.AttachNavigator(beepBindingNavigator1, customersBindingSource);
```

The grid can also run with only its owner-drawn navigator.

## Clipboard

```csharp
grid.CopyToClipboard();
grid.CopyToClipboard(includeHeaders: true, visibleColumnsOnly: true);
grid.CutToClipboard();
grid.PasteFromClipboard();
grid.CopyCellToClipboard();
```

Keyboard shortcuts:
- `Ctrl+C`
- `Ctrl+X`
- `Ctrl+V`

## Auto-size and best-fit

### Main API
```csharp
grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
grid.AutoSizeTriggerMode = AutoSizeTriggerMode.OnSortFilter;
grid.AutoResizeColumnsToFitContent();
grid.AutoSizeRowsToContent = true;
grid.AutoSizeRowsToFitContent();
grid.BestFitColumn(2);
grid.BestFitVisibleColumns();
```

### Trigger modes
- `Manual`
- `OnDataBind`
- `OnEditCommit`
- `OnSortFilter`
- `AlwaysDebounced`

### User gestures
- double-click a column border to best-fit one column
- `Ctrl` + double-click a column border to best-fit all visible columns

## Context Menu
Right-click opens built-in commands for:
- copy / copy with headers / cut / paste
- select all / clear selection
- insert / delete
- auto-size columns
- reset column order
- export placeholders for Excel / CSV

`GridContextMenuItemSelected` lets callers cancel the built-in action or request a grid refresh.
