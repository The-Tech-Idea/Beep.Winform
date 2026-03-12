# BeepGridPro

`BeepGridPro` is the owner-drawn grid control in `GridX`. The control keeps data, rendering, scrolling, editing, navigation, filtering, and sizing in separate helpers, which makes it easier to extend than the older single-file grid implementations.

## What It Does
- Binds to `BindingSource`, `DataTable`, `DataView`, `DataSet` + `DataMember`, `IEnumerable<T>`, and root objects with collection properties.
- Supports `IUnitofWork` and `IUnitOfWorkWrapper` as an alternate binding mode.
- Draws its own headers, rows, sticky columns, navigator, and scrollbars.
- Provides top-panel filtering, inline quick search, advanced filter dialogs, and Excel-style popup filtering.
- Supports editing, clipboard copy/cut/paste, column resize, row resize, and drag reorder.

## Quick Start
```csharp
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.GridX;

var grid = new BeepGridPro
{
    Dock = DockStyle.Fill,
    GridStyle = BeepGridStyle.Bootstrap,
    ShowNavigator = true,
    ShowTopFilterPanel = true,
    GridTitle = "Customers"
};

grid.DataSource = customersBindingSource;
grid.AutoGenerateColumns();
grid.EnableExcelFilter();
```

Runtime note:
- the constructor already wires `EnableExcelFilter()` outside design mode
- calling it explicitly is optional and safe

## Core Architecture
`BeepGridPro` is mostly an orchestrator. Real work is delegated to helper classes:

| Helper | Responsibility |
|--------|----------------|
| `GridDataHelper` | Binding, column generation, row materialization, cell value updates |
| `GridLayoutHelper` | Header, rows, filter panel, navigator, and cell rectangle calculation |
| `GridRenderHelper` | Painting rows, headers, filter panel, focus state, and hit-test maps |
| `GridInputHelper` | Mouse, keyboard, resize, reorder, checkbox, and filter-panel interaction |
| `GridSelectionHelper` | Active cell tracking and viewport sync |
| `GridScrollHelper` / `GridScrollBarsHelper` | Scroll position and custom scrollbar painting |
| `GridSortFilterHelper` | Binding-source sort/filter fallback logic and local visibility filtering |
| `GridEditHelper` | Overlay and in-place editor lifecycle |
| `GridNavigatorHelper` | Owner-drawn/external navigator actions and CRUD flows |
| `GridSizingHelper` | Auto-size and best-fit logic |
| `GridDialogHelper` | Filter, column config, and editor dialogs |
| `GridUnitOfWorkBinder` | UOW-to-grid synchronization |

## Binding Model

### Standard mode
- Set `DataSource`.
- Optionally set `DataMember`.
- Call `AutoGenerateColumns()` if you want columns inferred from the source.
- `EnsureSystemColumns()` keeps `Sel`, `RowNum`, and `RowID` available.

### UOW mode
- Assign `Uow` to an `IUnitofWork` or `IUnitOfWorkWrapper`.
- The current implementation does not auto-wrap arbitrary runtime `UnitofWork<T>` objects inside `BeepGridPro`; wrap them before assignment when needed.
- When UOW mode is active, `Units` becomes the authoritative source and the last regular `DataSource` is only retained for fallback.

## Important Public Surface

### Data and selection
```csharp
grid.DataSource = source;
grid.DataMember = "Customers";
grid.Columns
grid.Rows
grid.CurrentRow
grid.CurrentRowIndex
grid.SelectedRows
grid.SelectedRowIndices
grid.SelectCell(rowIndex, columnIndex);
```

### CRUD and navigation
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

### Filtering and dialogs
```csharp
grid.ShowFilterDialog();
grid.ShowAdvancedFilterDialog();
grid.ShowInlineCriterionEditor("CustomerName");
grid.ShowSearchDialog();
grid.ShowColumnConfigDialog();
grid.ApplyQuickFilter("john");
grid.ClearFilter();
```

### Sizing and layout
```csharp
grid.AutoResizeColumnsToFitContent();
grid.AutoSizeRowsToFitContent();
grid.BestFitColumn(columnIndex);
grid.BestFitVisibleColumns();
grid.SetColumnWidth("Name", 180);
```

## Filtering Behavior
There are two distinct filtering paths in the current code:

### `ActiveFilter` path
- Used by `ApplyQuickFilter`, `ShowAdvancedFilterDialog`, `AddFilterCriterion`, and `ClearFilter`.
- Marks rows visible or hidden with `BeepRowConfig.IsVisible`.
- Raises `FilterApplied` and `FilterCleared`.
- Drives the top filter panel and inline quick search.

### `SortFilter` helper path
- Used by `GridSortFilterHelper` and the Excel-style popup extension.
- Tries to push sort/filter into the underlying `BindingSource` first.
- Falls back to local row reorder or row visibility when necessary.
- Does not raise the public `FilterApplied`/`FilterCleared` events from `BeepGridPro`.

Treat these as separate mechanisms when documenting or extending the grid.

## Styling Notes

### `GridStyle`
`ApplyGridStyle()` currently implements:
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

`BeepGridStyle.Modern` exists in the shared enum but does not have its own branch in `ApplyGridStyle()`.

### `LayoutPreset`
`GridLayoutPreset` exposes many values, but the property-based switch currently wires only the legacy preset set:
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

Newer layout classes such as `Material3SurfaceLayout` and `AGGridAlpineLayout` can still be applied directly with the extension overload:
```csharp
grid.ApplyLayoutPreset(new Material3SurfaceLayout());
```

## User Interaction Details
- Double-click a column border to best-fit one column.
- Press `Ctrl` while double-clicking a column border to best-fit all visible columns.
- Drag a non-system, non-sticky header to change `DisplayOrder`.
- Click the top filter panel for search, advanced filter, clear-all, or per-column inline criteria.
- Use `Ctrl+C`, `Ctrl+X`, and `Ctrl+V` for clipboard actions.
- Use `F2` or `Enter` to begin editing when the active cell is editable.

## Design-Time Support
The design server project exposes a `BeepGridProDesigner` smart-tag surface with:
- quick configuration presets
- row-height presets
- auto-size actions
- filter panel settings
- header icon settings
- focus styling settings

Quick actions include:
- `Best Fit Visible Columns (Fast)`
- `Best Fit Visible Columns (All Rows)`
- `Auto-Size Columns Now`

## Known Implementation Gaps
- `SelectionMode` is exposed publicly, but most current interaction logic still centers on active-cell focus plus checkbox row selection.
- `BeepGridStyle.Modern` is present in the enum but not handled in `ApplyGridStyle()`.
- The `LayoutPreset` enum is ahead of the property switch; newer layout classes must be applied directly.

## Related Docs
- [Claude.md](./Claude.md)
- [Docs/README.md](./Docs/README.md)
- [Painters/README.md](./Painters/README.md)
- [Layouts/README.md](./Layouts/README.md)
