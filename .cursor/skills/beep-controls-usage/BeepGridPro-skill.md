# BeepGridPro Skill

## Overview
`BeepGridPro` is the modern helper-driven grid in `GridX`, with:
- regular `DataSource` mode and UOW mode
- row/cell focus painting controls
- header sort/filter icons with visibility modes
- top filter panel support
- best-fit and auto-size policies
- design-time smart-tag support in the Design Server project

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.GridX;
```

---

## Data Modes

### Regular DataSource mode
- Use `DataSource` (+ optional `DataMember`).
- Navigator operations (`InsertNew`, `DeleteCurrent`, `Save`, `Cancel`) flow through `BindingSource`.

### UOW mode
- Set `Uow` to either:
  - `IUnitofWork` (typed/non-generic UOW contract)
  - `IUnitOfWorkWrapper`
  - runtime `UnitofWork<T>` object (wrapped once by `UnitOfWorkWrapper`)
- The grid stores a reference to the assigned UOW object (no data copy).
- `GridUnitOfWorkBinder` binds `Units` and listens for list/UOW events without runtime reflection.
- While UOW is set, `DataSource` is kept as fallback value but UOW is authoritative.

---

## Key Properties

### Data
| Property | Type | Description |
|----------|------|-------------|
| `DataSource` | `object` | Regular data source mode |
| `DataMember` | `string` | Data member for complex data sources |
| `Uow` | `object` | UOW entry point (`IUnitofWork`, `IUnitOfWorkWrapper`, or `UnitofWork<T>`) |
| `Columns` | `BeepGridColumnConfigCollection` | Column definitions |
| `Rows` | `BindingList<BeepRowConfig>` | Row view model collection |

### Layout
| Property | Type | Default |
|----------|------|---------|
| `RowHeight` | `int` | 25 |
| `ColumnHeaderHeight` | `int` | 28 |
| `ShowColumnHeaders` | `bool` | `true` |
| `ShowNavigator` | `bool` | `true` |
| `ShowTopFilterPanel` | `bool` | `false` |
| `TopFilterPanelHeight` | `int` | `34` |
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

### Header Icon Visibility
| Property | Type | Default | Notes |
|----------|------|---------|-------|
| `SortIconVisibility` | `HeaderIconVisibility` | `Always` | `Always`, `HoverOnly`, `Hidden` |
| `FilterIconVisibility` | `HeaderIconVisibility` | `Always` | `Always`, `HoverOnly`, `Hidden` |

### Focus Painting
| Property | Type | Default |
|----------|------|---------|
| `UseDedicatedFocusedRowStyle` | `bool` | `true` |
| `FocusedRowBackColor` | `Color` | theme-derived |
| `ShowFocusedCellFill` | `bool` | `true` |
| `FocusedCellFillColor` | `Color` | theme-derived |
| `FocusedCellFillOpacity` | `int` | `36` |
| `ShowFocusedCellBorder` | `bool` | `true` |
| `FocusedCellBorderColor` | `Color` | theme-derived |
| `FocusedCellBorderWidth` | `float` | `2f` |

### Auto-Size Policy
| Property | Type | Default |
|----------|------|---------|
| `AutoSizeColumnsMode` | `DataGridViewAutoSizeColumnsMode` | `None` |
| `AutoSizeTriggerMode` | `AutoSizeTriggerMode` | `OnDataBind` |
| `AutoSizeDebounceMilliseconds` | `int` | `120` |
| `AutoSizeRowsToContent` | `bool` | `false` |
| `RowAutoSizePadding` | `int` | `2` |
| `UseDpiAwareRowHeights` | `bool` | `true` |

---

## Column Sizing Contract (BeepColumnConfig)
Use per-column constraints with global auto-size:

| Property | Type | Default | Purpose |
|----------|------|---------|---------|
| `MinWidth` | `int` | `20` | Lower bound for resize/auto-size |
| `MaxWidth` | `int` | `0` | Upper bound (`0` = no max) |
| `FillWeight` | `float` | `1f` | Relative width in Fill mode |
| `AllowAutoSize` | `bool` | `true` | Include/exclude from auto-size and best-fit |

Notes:
- `Width` is clamped to `MinWidth/MaxWidth`.
- Header and cell alignment defaults are now `ContentAlignment.MiddleLeft`.

---

## Navigation + CRUD Behavior
Public methods:
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

### UOW mode CRUD mapping
- `InsertNew()` -> `_uow.New()` or `_uowWrapper.New()`
- `DeleteCurrent()` -> UOW delete of selected/current record
- `Save()` -> `Commit()`
- `Cancel()` -> `Rollback()`

Wrapper mode event forwarding is wired through navigator/binder for post-create/delete/commit/rollback refresh behavior.

---

## Sorting and Filtering
Public surface:
```csharp
grid.ToggleColumnSort(columnIndex);
grid.ShowFilterDialog();
grid.ShowAdvancedFilterDialog();
grid.ApplyQuickFilter("john");
grid.ClearFilter();
```

Additional support:
- Header icons are clickable and visibility-controlled by `SortIconVisibility` / `FilterIconVisibility`.
- Top filter panel can be rendered above headers via `ShowTopFilterPanel`.
- For UOW/`ObservableBindingList`, source-level sort/filter is supported without forcing full data reconstruction.

---

## Auto-Size and Best-Fit
Public methods:
```csharp
grid.AutoResizeColumnsToFitContent();
grid.BestFitColumn(columnIndex, includeHeader: true, allRows: false);
grid.BestFitVisibleColumns(includeHeader: true, allRows: false);
```

User gesture:
- Double-click near a header divider -> best-fit that column.
- `Ctrl + double-click` near divider -> best-fit all visible columns.

Trigger policy (`AutoSizeTriggerMode`):
- `Manual`
- `OnDataBind`
- `OnEditCommit`
- `OnSortFilter`
- `AlwaysDebounced`

---

## Design-Time (Design Server)
`BeepGridProDesigner` smart tags expose grouped sections:
- `Auto-Size`
- `Filtering UI`
- `Header Icons`
- `Focus Styling`

Quick design-time actions include:
- best fit visible columns (fast/all rows)
- auto-size columns now

---

## Events
| Event | Description |
|-------|-------------|
| `RowSelectionChanged` | Active/checkbox row selection changed |
| `CellValueChanged` | Inline editor committed a cell change |
| `SaveCalled` | Save flow requested/completed |
| `ColumnReordered` | Column drag reorder completed |
| `GridContextMenuItemSelected` | Context-menu action invoked |
| `FilterApplied` | Advanced/quick filter applied |
| `FilterCleared` | Active filter cleared |

---

## Usage Examples

### Basic regular source
```csharp
var grid = new BeepGridPro
{
    GridStyle = BeepGridStyle.Bootstrap,
    LayoutPreset = GridLayoutPreset.Striped,
    ShowNavigator = true
};
grid.DataSource = customers;
```

### UOW mode
```csharp
grid.Uow = myUnitOfWork;   // reference assignment, no copy
grid.InsertNew();          // calls UOW New()
grid.DeleteCurrent();      // deletes selected/current row in UOW
grid.Save();               // Commit()
grid.Cancel();             // Rollback()
```

### Focus + filter UI polish
```csharp
grid.UseDedicatedFocusedRowStyle = true;
grid.ShowFocusedCellBorder = true;
grid.SortIconVisibility = HeaderIconVisibility.HoverOnly;
grid.FilterIconVisibility = HeaderIconVisibility.Always;
grid.ShowTopFilterPanel = true;
grid.TopFilterPanelHeight = 36;
```

### Auto-size policy
```csharp
grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
grid.AutoSizeTriggerMode = AutoSizeTriggerMode.OnSortFilter;
grid.AutoSizeRowsToContent = true;
grid.BestFitVisibleColumns(includeHeader: true, allRows: false);
```

---

## Related
- `GridUnitOfWorkBinder` (UOW bridge, no reflection)
- `GridNavigatorHelper` (CRUD/navigation behavior)
- `GridRenderHelper` (painting orchestration)
- `BeepColumnConfig` (per-column sizing/focus metadata)
