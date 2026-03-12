# BeepGridPro Skill

## Overview
`BeepGridPro` is the owner-drawn grid in `GridX`. It is built from helper classes instead of one monolithic control and combines:
- binding-source and plain object/list data binding
- optional `IUnitofWork` / `IUnitOfWorkWrapper` integration
- custom painting, scrolling, selection, editing, and clipboard flows
- an owner-drawn navigator plus optional external `BeepBindingNavigator`
- a top filter panel with inline quick search controls
- auto-size, best-fit, sticky columns, and column drag reorder

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Vis.Modules;
```

## Binding Modes

### Regular data binding
- Set `DataSource` to a `BindingSource`, `DataTable`, `DataView`, `DataSet` plus `DataMember`, `IEnumerable<T>`, or a root object that exposes a collection property named by `DataMember`.
- `Navigator.BindTo(...)` is used behind the scenes, so move/add/delete/save/cancel operations flow through a `BindingSource` when one is available.
- Setting `DataSource = null` clears rows, clears non-system columns, resets navigator binding, and clears selection.

### UOW binding
- `Uow` accepts:
  - `IUnitofWork`
  - `IUnitOfWorkWrapper`
- The current implementation only casts to those two contracts. It does not auto-wrap arbitrary runtime `UnitofWork<T>` instances inside `BeepGridPro`; wrap them before assignment if needed.
- When `Uow` is active, the grid binds to `Units` and keeps the last regular `DataSource` value only as a fallback for when `Uow` is cleared.
- `GridUnitOfWorkBinder` refreshes rows on list changes and UOW lifecycle events without using runtime reflection.

## Key Properties

### Data surface
| Property | Type | Notes |
|----------|------|-------|
| `DataSource` | `object?` | Main non-UOW binding entry point |
| `DataMember` | `string` | Used for `DataSet`, `DataViewManager`, or root-object property binding |
| `Uow` | `object?` | Must be an `IUnitofWork` or `IUnitOfWorkWrapper` |
| `Columns` | `BeepGridColumnConfigCollection` | Column definitions, including system columns |
| `Rows` | `BindingList<BeepRowConfig>` | Current row view models |
| `CurrentRow` | `BeepGridCurrentRow?` | Supports `grid.CurrentRow?.Cells["Name"]?.Value` |
| `CurrentRowIndex` | `int` | Current active row index |
| `SelectedRows` | `IReadOnlyList<BeepRowConfig>` | Checkbox-selected rows |
| `SelectedRowIndices` | `IReadOnlyList<int>` | Checkbox-selected row indices |

### Layout and appearance
| Property | Type | Default |
|----------|------|---------|
| `RowHeight` | `int` | `25` |
| `ColumnHeaderHeight` | `int` | `28` |
| `ShowColumnHeaders` | `bool` | `true` |
| `ShowNavigator` | `bool` | `true` |
| `ShowTopFilterPanel` | `bool` | `true` |
| `UseInlineQuickSearch` | `bool` | `true` |
| `TopFilterPanelHeight` | `int` | `34` |
| `GridTitle` | `string` | `"Grid"` |
| `GridStyle` | `BeepGridStyle` | runtime field starts as `Bootstrap` |
| `NavigationStyle` | `navigationStyle` | `Standard` |
| `UsePainterNavigation` | `bool` | `true` |
| `LayoutPreset` | `GridLayoutPreset` | `Default` |

### Behavior
| Property | Type | Default |
|----------|------|---------|
| `AllowUserToResizeColumns` | `bool` | `true` |
| `AllowUserToResizeRows` | `bool` | `false` |
| `AllowColumnReorder` | `bool` | `true` |
| `ReadOnly` | `bool` | `false` |
| `MultiSelect` | `bool` | `true` |
| `ShowCheckBox` | `bool` | `false` |
| `SelectionMode` | `BeepGridSelectionMode` | `FullRowSelect` |

### Focus styling
| Property | Type | Default |
|----------|------|---------|
| `UseDedicatedFocusedRowStyle` | `bool` | `true` |
| `FocusedRowBackColor` | `Color` | theme-derived when empty |
| `ShowFocusedCellFill` | `bool` | `true` |
| `FocusedCellFillColor` | `Color` | theme-derived when empty |
| `FocusedCellFillOpacity` | `int` | `36` |
| `ShowFocusedCellBorder` | `bool` | `true` |
| `FocusedCellBorderColor` | `Color` | theme-derived when empty |
| `FocusedCellBorderWidth` | `float` | `2f` |

### Header icons and auto-size
| Property | Type | Default |
|----------|------|---------|
| `SortIconVisibility` | `HeaderIconVisibility` | `Always` |
| `FilterIconVisibility` | `HeaderIconVisibility` | `Hidden` |
| `AutoSizeColumnsMode` | `DataGridViewAutoSizeColumnsMode` | `None` |
| `AutoSizeTriggerMode` | `AutoSizeTriggerMode` | `OnDataBind` |
| `AutoSizeDebounceMilliseconds` | `int` | `120` |
| `AutoSizeRowsToContent` | `bool` | `false` |
| `RowAutoSizePadding` | `int` | `2` |
| `UseDpiAwareRowHeights` | `bool` | `true` |

## System Columns
`EnsureSystemColumns()` keeps three internal columns available:
- `Sel`: sticky checkbox column, hidden unless `ShowCheckBox = true`
- `RowNum`: sticky row-number column, visible
- `RowID`: sticky hidden identity/index column

These are unbound helper columns. Best-fit and most auto-size logic skip them.

## Column Configuration
Important `BeepColumnConfig` members:
- `ColumnName`, `ColumnCaption`, `Visible`, `ReadOnly`
- `Width`, `MinWidth`, `MaxWidth`, `FillWeight`, `AllowAutoSize`
- `DisplayOrder`, `AllowReorder`, `Sticked`
- `AllowSort`, `AllowFilter`, `IsSorted`, `SortDirection`, `IsFiltered`, `Filter`
- `CellEditor` for editor selection
- `Items`, `EnumSourceType`, numeric/date/image metadata for specialized editors

Notes:
- `Width` is clamped to `MinWidth` and `MaxWidth`.
- Sticky columns (`Sticked = true`) stay pinned on the left and are excluded from reorder moves.
- `HeaderTextAlignment` and `CellTextAlignment` currently default to `ContentAlignment.MiddleLeft`.

## Public Methods

### Data and layout
```csharp
grid.AutoGenerateColumns();
grid.EnsureSystemColumns();
grid.RefreshGrid();
grid.RefreshData();
grid.ClearGrid();
grid.SetColumnWidth("Name", 180);
```

### Selection and navigation
```csharp
grid.SelectCell(rowIndex, columnIndex);
grid.MoveFirst();
grid.MovePrevious();
grid.MoveNext();
grid.MoveLast();
grid.InsertNew();
grid.DeleteCurrent();
grid.Save();
grid.Cancel();
```

### Editing, dialogs, and clipboard
```csharp
grid.ShowCellEditor();
grid.ShowFilterDialog();
grid.ShowAdvancedFilterDialog();
grid.ShowInlineCriterionEditor("CustomerName");
grid.ShowSearchDialog();
grid.ShowColumnConfigDialog();
grid.CopyToClipboard();
grid.CutToClipboard();
grid.PasteFromClipboard();
grid.CopyCellToClipboard();
```

### Filtering and sizing
```csharp
grid.ToggleColumnSort(columnIndex);
grid.ApplyQuickFilter("john");
grid.ClearFilter();
grid.AddFilterCriterion(criterion);
grid.RemoveFilterCriterion("CustomerName");
grid.AutoResizeColumnsToFitContent();
grid.AutoSizeRowsToFitContent();
grid.BestFitColumn(columnIndex, includeHeader: true, allRows: false);
grid.BestFitVisibleColumns(includeHeader: true, allRows: false);
```

## Filtering Model
`BeepGridPro` currently has two filtering paths:

### 1. ActiveFilter / quick filter path
- Driven by `ActiveFilter`, `ShowAdvancedFilterDialog()`, `ApplyQuickFilter(...)`, `AddFilterCriterion(...)`, and `ClearFilter()`.
- Works by setting `BeepRowConfig.IsVisible`.
- Raises `FilterApplied` and `FilterCleared`.
- Powers the top filter panel and inline quick search.

### 2. SortFilter helper path
- Driven by the internal `SortFilter` helper (`GridSortFilterHelper`) used by GridX internals and the Excel-style popup extension.
- Tries source-level `BindingSource` sort/filter first, then falls back to local row visibility or local row reorder.
- Used by the Excel-style filter popup extension.
- Does not raise the public `FilterApplied` / `FilterCleared` events from `BeepGridPro`.

Use one path consistently inside a screen to avoid confusing state.

## Layout and Style Notes

### Grid styles
`ApplyGridStyle()` has concrete branches for:
- `Default`
- `Clean`
- `Bootstrap`
- `Material`
- `Flat`
- `Compact`
- `Corporate`
- `Minimal`
- `Card`
- `Borderless`

`BeepGridStyle.Modern` exists in the enum but does not have a dedicated `ApplyGridStyle()` branch in the current implementation.

### Layout presets
The `GridLayoutPreset` enum contains many values, but `BeepGridPro.ApplyLayoutPreset(GridLayoutPreset)` currently switches only:
- `Default`
- `Clean`
- `Dense`
- `Striped`
- `Borderless`
- `HeaderBold`
- `MaterialHeader`
- `Card`
- `ComparisonTable`
- `MatrixSimple`
- `MatrixStriped`
- `PricingTable`

For newer layout classes such as `Material3SurfaceLayout`, `Fluent2StandardLayout`, `TailwindProseLayout`, `AGGridAlpineLayout`, `AntDesignStandardLayout`, and `DataTablesStandardLayout`, apply the class directly:
```csharp
grid.ApplyLayoutPreset(new Material3SurfaceLayout());
```

## User Interactions
- Double-click a column border to best-fit one column.
- `Ctrl` + double-click a column border to best-fit all visible columns.
- Drag a non-sticky, non-system header to reorder columns.
- Click row or header checkboxes to change row selection.
- Use `Ctrl+C`, `Ctrl+X`, and `Ctrl+V` for clipboard operations.
- `F2` or `Enter` starts editing when the current cell is editable.

## Events
| Event | Notes |
|-------|-------|
| `RowSelectionChanged` | Checkbox row selection change or bulk select |
| `SelectionChanged` | Lightweight active-selection event |
| `CellValueChanged` | Raised after editor commit |
| `SaveCalled` | Raised when save flow is invoked |
| `ColumnReordered` | Raised after drag reorder |
| `GridContextMenuItemSelected` | Can cancel built-in action or request refresh |
| `FilterApplied` | Only for the `ActiveFilter` / quick-filter path |
| `FilterCleared` | Only for the `ActiveFilter` / quick-filter path |

## Usage Examples

### Basic binding
```csharp
var grid = new BeepGridPro
{
    Dock = DockStyle.Fill,
    GridStyle = BeepGridStyle.Bootstrap,
    ShowNavigator = true,
    ShowTopFilterPanel = true
};

grid.DataSource = customersBindingSource;
grid.AutoGenerateColumns();
grid.EnableExcelFilter();
```

Note:
- runtime construction already calls `EnableExcelFilter()` once outside design mode
- calling it again is harmless if you want to be explicit

### UOW binding
```csharp
grid.Uow = myUnitOfWork;   // must already implement IUnitofWork or IUnitOfWorkWrapper
grid.InsertNew();
grid.DeleteCurrent();
grid.Save();               // Commit()
grid.Cancel();             // Rollback()
```

### Focus and sizing
```csharp
grid.UseDedicatedFocusedRowStyle = true;
grid.ShowFocusedCellBorder = true;
grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
grid.AutoSizeTriggerMode = AutoSizeTriggerMode.OnSortFilter;
grid.AutoSizeRowsToContent = true;
grid.BestFitVisibleColumns(includeHeader: true, allRows: false);
```

### Newer layout class
```csharp
grid.GridStyle = BeepGridStyle.Material;
grid.ApplyLayoutPreset(new Material3SurfaceLayout());
```

## Related Types
- `GridDataHelper`
- `GridRenderHelper`
- `GridNavigatorHelper`
- `GridSortFilterHelper`
- `GridSizingHelper`
- `GridUnitOfWorkBinder`
- `BeepGridProAdapter`
