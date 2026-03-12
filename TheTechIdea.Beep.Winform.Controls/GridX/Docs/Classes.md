# BeepGridPro Class Map

## Top-Level Control

### `BeepGridPro`
Main public control. Owns helper instances, public properties, public methods, and events.

Primary responsibilities:
- expose binding, selection, CRUD, filtering, sizing, and clipboard APIs
- instantiate all helpers in the constructor
- coordinate theme and style application
- forward public methods to the relevant helper

Key files:
- `BeepGridPro.cs`
- `BeepGridPro.Properties.cs`
- `BeepGridPro.Navigation.cs`
- `BeepGridPro.Filtering.cs`
- `BeepGridPro.Dialogs.cs`
- `BeepGridPro.ContextMenu.cs`
- `BeepGridPro.ClipboardOps.cs`
- `BeepGridPro.Events.cs`

## Helper Classes

### `GridDataHelper`
Handles:
- `DataSource` / `DataMember` resolution
- `AutoGenerateColumns()`
- `EnsureSystemColumns()`
- `RefreshRows()`
- row object to cell value synchronization
- editor value normalization and write-back

Important detail:
- `AutoGenerateColumns()` clears and rebuilds the column collection.

### `GridLayoutHelper`
Calculates:
- `TopFilterRect`
- `HeaderRect`
- `RowsRect`
- navigator rect
- header cell rectangles
- cell bounds for visible rows

Important detail:
- `IsCalculating` prevents recursive layout work.

### `GridRenderHelper`
Draws:
- background
- top filter panel
- headers
- rows and cells
- focus state
- header sort/filter hit rectangles
- top-panel chip and action hit rectangles

Important detail:
- this helper is the source of most hit-test rectangles consumed by `GridInputHelper`.

### `GridInputHelper`
Handles:
- top filter panel clicks
- sort and filter header icon clicks
- select-all and row checkbox clicks
- column resize
- row resize
- column reorder drag delegation
- keyboard shortcuts
- edit start

Important detail:
- any click inside `TopFilterRect` is intentionally handled and returned early to avoid falling through into cell selection.

### `GridSelectionHelper`
Tracks:
- `RowIndex`
- `ColumnIndex`
- selected cell rect

Responsibilities:
- active-cell focus
- localized invalidation for selection changes
- keeping the active row visible in the viewport

### `GridScrollHelper`
Tracks scroll offsets and visible row calculations.

### `GridScrollBarsHelper`
Paints and updates custom scrollbars instead of using child `ScrollBar` controls.

### `GridSortFilterHelper`
Owns the helper-level sort/filter pipeline:
- source-level sort via `BindingSource` or `IBindingList`
- source-level filter via `BindingSource.Filter` or `IBindingListView`
- local row reorder fallback
- local visibility filtering fallback

Important detail:
- this is not the same state as `BeepGridPro.ActiveFilter`.

### `GridEditHelper`
Manages editor activation, commit/cancel flow, and overlay editor positioning.

### `GridNavigatorHelper`
Bridges:
- owner-drawn navigator
- optional external `BeepBindingNavigator`
- `BindingSource`
- UOW lifecycle calls

### `GridSizingHelper`
Handles:
- `AutoResizeColumnsToFitContent()`
- `AutoSizeRowsToFitContent()`
- `BestFitColumn()`
- `BestFitVisibleColumns()`
- width clamping against `MinWidth` and `MaxWidth`

### `GridDialogHelper`
Owns filter dialogs, column config dialogs, editor dialogs, and inline criterion editor menus.

### `GridUnitOfWorkBinder`
Synchronizes UOW-backed `Units` collections with the grid and rebinds on UOW lifecycle events.

### `GridColumnReorderHelper`
Implements drag-reorder for visible, non-sticky, non-system columns.

## Supporting Types

### `BeepColumnConfig`
Column metadata type.

Important members:
- identity: `ColumnName`, `ColumnCaption`
- layout: `Width`, `MinWidth`, `MaxWidth`, `FillWeight`, `DisplayOrder`, `Sticked`
- behavior: `Visible`, `ReadOnly`, `AllowReorder`, `AllowSort`, `AllowFilter`, `AllowAutoSize`
- state: `IsSorted`, `SortDirection`, `IsFiltered`, `Filter`
- editor/config: `CellEditor`, `Items`, `EnumSourceType`
- system flags: `IsSelectionCheckBox`, `IsRowNumColumn`, `IsRowID`, `IsUnbound`

### `BeepGridColumnConfigCollection`
`BindingList<BeepColumnConfig>` with column property-change forwarding.

### `BeepRowConfig`
Row view model.

Important members:
- `RowData` / `RowDataObject`
- `Cells`
- `IsSelected`
- `IsVisible`
- `Height`
- `RowCheckRect`

### `BeepCellConfig`
Cell view model.

Important members:
- `CellValue`
- `ColumnName`
- `ColumnIndex`
- `Rect`
- `IsEditable`
- `IsReadOnly`
- `CellEditor`

### `BeepGridCurrentRow`
Read-only convenience wrapper so callers can use:
```csharp
grid.CurrentRow?.Cells["Name"]?.Value
```

## Adapters and Extensions

### `BeepGridProAdapter`
Thin adapter that exposes `BeepGridPro` through the `BeepSimpleGridLike` abstraction for reuse with Excel popup helpers.

### `BeepGridProFilterExtensions`
Adds the Excel-style popup filter entry point without changing the main control API shape.
