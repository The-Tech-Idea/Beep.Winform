# BeepGridPro

`BeepGridPro` is the owner-drawn data grid control in `GridX`. It binds to
many data source types, paints its own headers/rows/scrollbars/filters, and
exposes selection, sorting, filtering, grouping, virtualization, and
exporting through a helper-driven architecture that is easier to extend than
the older monolithic grid implementations.

---

## Table of Contents

1. [What It Does](#what-it-does)
2. [Quick Start](#quick-start)
3. [Public Surface](#public-surface)
4. [Architecture](#architecture)
5. [Binding Model](#binding-model)
6. [Filtering](#filtering)
7. [The Unified Toolbar](#the-unified-toolbar)
8. [Grouping & Summary Rows](#grouping--summary-rows)
9. [Virtualization](#virtualization)
10. [Export Engine](#export-engine)
11. [Selection Modes](#selection-modes)
12. [Editor Framework](#editor-framework)
13. [Styling & Layout Presets](#styling--layout-presets)
14. [User Interaction](#user-interaction)
15. [Design-Time Support](#design-time-support)
16. [Accessibility & Keyboard](#accessibility--keyboard)
17. [Known Gaps & Roadmap](#known-gaps--roadmap)
18. [Related Docs](#related-docs)

---

## What It Does

- Binds to `BindingSource`, `DataTable`, `DataView`, `DataSet + DataMember`,
  `IEnumerable<T>`, root objects with collection properties, and UoW
  (`IUnitofWork` / `IUnitOfWorkWrapper`) sources.
- Draws its own headers, sticky columns, rows, filter panel, navigator,
  custom scrollbars, and the unified toolbar.
- Provides top-panel filtering, an inline quick search, advanced filter
  dialogs, Excel-style popup filtering, and per-column inline criteria.
- Supports editing, clipboard copy/cut/paste, column resize, row resize,
  drag-to-reorder, freeze columns, aggregation in summary rows, and
  row-level grouping with collapse/expand.
- Row and column virtualization handle very large datasets with on-demand
  materialization.
- Export engine produces CSV / JSON / HTML natively, with pluggable
  Excel/PDF exporters discovered at runtime.

---

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
    ShowToolbar = true,                          // unified toolbar (default)
    GridTitle = "Customers"
};

grid.DataSource = customersBindingSource;
grid.AutoGenerateColumns();
grid.EnableExcelFilter();
```

> **Runtime note:** The constructor wires `EnableExcelFilter()` outside
> design mode. Calling it explicitly is optional and safe.

---

## Public Surface

### Data & Selection
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

### CRUD & Navigation
```csharp
grid.MoveFirst();
grid.MovePrevious();
grid.MoveNext();
grid.MoveLast();
grid.InsertNew();       // Navigator.InsertNew — works in UoW mode too
grid.DeleteCurrent();
grid.Save();
grid.Cancel();
```

### Filtering & Dialogs
```csharp
grid.ShowFilterDialog();
grid.ShowAdvancedFilterDialog();
grid.ShowInlineCriterionEditor("CustomerName");
grid.ShowSearchDialog();
grid.ShowColumnConfigDialog();
grid.ApplyQuickFilter("john");
grid.ClearFilter();
```

### Sizing & Layout
```csharp
grid.AutoResizeColumnsToFitContent();
grid.AutoSizeRowsToFitContent();
grid.BestFitColumn(columnIndex);
grid.BestFitVisibleColumns();
grid.SetColumnWidth("Name", 180);
```

### Grouping
```csharp
grid.GroupBy("Region");
grid.GroupBy(new[] { "Region", "Country" });
grid.Ungroup();
grid.ToggleGroup("EMEA");
grid.ExpandAllGroups();
grid.CollapseAllGroups();
```

### Export
```csharp
grid.ExportToCsv("customers.csv");
grid.ExportToJson("customers.json");
grid.ExportToHtml("customers.html");
grid.ExportToExcel("customers.xlsx");   // requires Excel plugin
grid.ExportToPdf("customers.pdf");       // requires PDF plugin
```

### Virtualization
```csharp
grid.EnableVirtualization   = true;     // row virtualization
grid.EnableColumnVirtualization = true; // column virtualization
grid.VirtualRowCount         // long
```

### Unified Toolbar
```csharp
grid.ShowToolbar         = true;
grid.ShowGridTitle        = true;
grid.GridTitle            = "Customers";
grid.SearchPlaceholder    = "Search...";
grid.SetToolbarButtonVisible("delete", false);
grid.FocusToolbarSearch();   // bound to Ctrl+F
grid.ToolbarAction += (s, e) => Console.WriteLine(e.Action);
```

---

## Architecture

`BeepGridPro` is a thin coordinator. Real work is delegated to helper
classes, each of which owns one concern. Helpers are constructed in
`BeepGridPro`'s constructor and stored as properties.

| Helper | Responsibility |
|---|---|
| `GridLayoutHelper` | Header, rows, filter panel, navigator, toolbar, and cell-rectangle calculation |
| `GridDataHelper` | Binding, column generation, row materialization, cell value updates |
| `GridRenderHelper` | Painting rows, headers, sticky columns, focus, hover, virtualization, **toolbar** |
| `GridInputHelper` | Mouse, keyboard, resize, reorder, checkbox, filter-panel, and **toolbar** interaction |
| `GridSelectionHelper` | Active cell tracking and viewport sync |
| `GridScrollHelper` / `GridScrollBarsHelper` | Scroll position and custom scrollbar painting |
| `GridSortFilterHelper` | Binding-source sort/filter fallback, local visibility filtering |
| `GridEditHelper` | Overlay/in-place editor lifecycle; public `IsEditing` flag |
| `GridNavigatorHelper` | Owner-drawn/external navigator actions and CRUD flows |
| `GridSizingHelper` | Auto-size and best-fit logic |
| `GridDialogHelper` | Filter, column config, and editor dialogs |
| `GridClipboardHelper` | Copy/cut/paste buffer |
| `GridColumnReorderHelper` | Drag-reorder columns |
| `GridKeyboardNavigator` | Arrow / Tab / PgUp / PgDn / Home / End navigation |
| `GridFocusManager` | Tracks focus and draws focus indicator |
| `GridUnitOfWorkBinder` | UoW-to-grid synchronization |
| `GridThemeHelper` | Per-style theme colour application |
| `GridNavigationPainterHelper` | Owner-drawn navigator rendering |
| `GridColumnHeadersPainterHelper` | Per-style header rendering |
| `GridColumnVirtualizer` | Per-window horizontal virtualization |
| `GridRowVirtualizer` | On-demand row materialization |
| `GridExportEngine` | CSV / JSON / HTML + pluggable Excel/PDF |
| `GridEditorFactory` | Resolves `IGridEditor` by `BeepColumnType` |
| `GridGroupEngine` | Group descriptors and re-grouping |
| `GridGroupHeaderRenderer` | Group header painter |
| `GridGroupSummaryRow` | Per-group aggregation values |

### Painter System

Header, navigator, and filter-panel visuals are pluggable. See
[`Painters/`](./Painters/) for the painter families:

- **Header painters** — `MaterialHeaderPainter`, `FluentHeaderPainter`,
  `BootstrapHeaderPainter`, `DataTablesHeaderPainter`, `AGGridHeaderPainter`,
  `TelerikHeaderPainter`, `TailwindHeaderPainter`, `AntDesignHeaderPainter`,
  `CardHeaderPainter`, `CompactHeaderPainter`, `MinimalHeaderPainter`,
  `StandardHeaderPainter`. Resolved by `HeaderPainterFactory` based on
  `GridStyle`.
- **Navigation painters** — same set, resolved by
  `NavigationPainterFactory`.
- **Filter-panel painters** — `MaterialFilterPanelPainter`,
  `FluentFilterPanelPainter`, etc. Resolved by `FilterPanelPainterFactory`.

### Layout Presets

A separate `Layouts/` system provides table-layout heuristics (padding,
border radius, header background) that stack on top of the painter
system. 11 modern preset classes are shipped (`Material3SurfaceLayout`,
`Fluent2CardLayout`, `TailwindDashboardLayout`, `AGGridAlpineLayout`, …)
plus the legacy 12.

### Toolbar (Phase 18)

The unified toolbar replaces `BeepFilterRow` and `BeepQuickFilterBar`.
It is fully owner-drawn (no child controls except the lazy search text
editor). See [The Unified Toolbar](#the-unified-toolbar).

---

## Binding Model

### Standard mode

```csharp
grid.DataSource = source;
grid.DataMember = "Customers";         // optional
grid.AutoGenerateColumns();
grid.EnsureSystemColumns();           // adds Sel, RowNum, RowID
```

### UoW mode

```csharp
grid.Uow = myUnitOfWork;              // must implement IUnitofWork or IUnitOfWorkWrapper
```

- `Units` becomes the authoritative source; the last regular `DataSource`
  is retained only for fallback.
- `Uow` setter casts to `IUnitofWork` and `IUnitOfWorkWrapper` only. There
  is no automatic `UnitofWork<T>` wrapping inside `BeepGridPro`; wrap
  before assignment when needed.
- `WrapperEventForwarded` exposes synthetic lifecycle events for
  wrapper-only UoW modes.

### Live updates

`GridDataHelper` subscribes to `INotifyCollectionChanged` when the
resolved data implements it (and does not implement `IBindingList`).
Add/remove/clear on an `ObservableCollection<T>` reflects in the grid
without a manual `RefreshGrid()`. `ClearDataSource()` unsubscribes.

---

## Filtering

There are **two** distinct filtering paths in the current code:

### `ActiveFilter` path
- Used by `ApplyQuickFilter`, `ShowAdvancedFilterDialog`,
  `AddFilterCriterion`, `ClearFilter`, the unified toolbar search box,
  and the legacy inline quick search.
- Marks rows visible/hidden via `BeepRowConfig.IsVisible`.
- Raises `FilterApplied` and `FilterCleared` events.
- Drives the top filter panel and the unified toolbar search box.

### `SortFilter` helper path
- Used by `GridSortFilterHelper` and the Excel-style popup extension.
- Tries to push sort/filter into the underlying `BindingSource` first.
- Falls back to local row reorder or row visibility when necessary.
- Does **not** raise the public `FilterApplied`/`FilterCleared` events.

Treat these as separate mechanisms when extending or documenting.

### Toolbar search behavior

- Search commits on **Enter**, cancels on **Escape**, and commits when
  the editor loses focus.
- The placeholder text is configurable via
  `grid.SearchPlaceholder`.
- Press **Ctrl+F** at any time to focus the search box.

---

## The Unified Toolbar

The unified toolbar (Phase 18) is the default filter + action surface
for `BeepGridPro`. It is enabled by default and replaces the legacy
filter panel. The toolbar and `ShowTopFilterPanel` are mutually
exclusive — enabling one disables the other.

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│ [📋 Customers]  [+ New] [✏ Edit] [🗑 Delete] │ [🔍 Search...] │      │
│  Title             Actions                     Search              │
│                                                  │ [⚙ Filter] [⛭ Adv]│[✕]
│                                                  │       Filter      │
│                                                  │   [↥][↧][↤][↦]    │
│                                                  │   Import Export  │
│                                                  │   [↥][↧]  [🖨]   │
│                                                  │            Print  │
└────────────────────────────────────────────────────────────────────┘
```

### Sections (left → right)

| Section | Elements | Width strategy |
|---|---|---|
| **Title** | `GridTitle` (configurable, toggle via `ShowGridTitle`) | Fixed, up to 25 % of toolbar width |
| **Actions** | New, Edit, Delete | Icon + text label, fixed |
| **Search** | Search icon + flexible-width box | Flexible — fills remaining space |
| **Filter** | Filter button, Advanced button, Clear button (when active) | Icon-only, fixed |
| **Export** | Import, Export, Print | Icon-only, fixed |

### Behaviour

- **Action buttons** are visible by default. Set
  `grid.SetToolbarButtonVisible("delete", false)` to hide individual
  buttons (e.g. when nothing is selected).
- **Filter button** opens the Excel-style filter dialog
  (`ShowFilterDialog`).
- **Advanced button** opens the multi-criteria advanced filter dialog
  (`ShowAdvancedFilterDialog`).
- **Clear filter button** appears only when a filter is active, with a
  badge showing the active criteria count.
- **Search box** activates an on-demand `BeepTextBox` editor on click;
  commits on Enter, cancels on Escape, commits on focus loss.
- **Hover/pressed** states draw rounded backgrounds matching the toolbar
  colour scheme.
- **Overflow**: when the toolbar is narrower than the actions + exports
  + filter + search minimums, action and export buttons flow into an
  overflow menu (chevron button) which surfaces them as a context menu.

### Keyboard

- **Ctrl+F** — focus the search box.
- **Escape** while search has focus — commit and unfocus.

### Customization

```csharp
grid.ToolbarBackColor             = Color.FromArgb(248, 249, 250);
grid.ToolbarForeColor             = Color.FromArgb(33, 37, 41);
grid.ToolbarPlaceholderColor      = Color.FromArgb(150, 150, 150);
grid.ToolbarSearchBackColor       = Color.White;
grid.ToolbarSearchFocusBackColor  = Color.FromArgb(240, 245, 255);
grid.ToolbarBorderColor           = Color.FromArgb(200, 200, 200);
grid.ToolbarButtonHoverBackColor  = Color.FromArgb(230, 235, 240);
grid.ToolbarButtonPressedBackColor = Color.FromArgb(210, 220, 230);
grid.ToolbarSeparatorColor        = Color.FromArgb(220, 220, 220);
```

### Events

```csharp
grid.ToolbarAction += (s, e) =>
{
    switch (e.Action)
    {
        case "add":     // user clicked the New button
        case "edit":    // user clicked the Edit button
        case "delete":  // user clicked the Delete button
        case "import":  // user clicked the Import button
        case "export":  // user clicked the Export button
        case "print":   // user clicked the Print button
        default: break;
    }
};
```

> **Note:** The Add and Delete buttons fire no public event because they
> are bound directly to `Navigator.InsertNew` and
> `Navigator.DeleteCurrent`. Subscribe to `RowAdded`, `RowDeleted`, and
> the UoW lifecycle events for fine-grained tracking.

### Deprecation

`BeepFilterRow` and `BeepQuickFilterBar` are marked `[Obsolete]` and
will be removed in a future version. Migrate to the unified toolbar.

---

## Grouping & Summary Rows

`GridGroupEngine` manages descriptors, collapse state, and re-grouping
after sort/filter operations.

```csharp
grid.GroupBy("Region");
grid.GroupBy(new[] { "Region", "Country" }, descending: false);
```

- Sorting a **group column** updates the descriptor and reorders groups.
- Sorting a **non-group column** calls `GroupEngine.SortWithinGroups()`,
  preserving the group structure.
- `ApplyLocalSort()` is bypassed entirely when
  `_grid.GroupEngine.IsGrouped` is true.

### Summary rows

Configure aggregation per column:

```csharp
foreach (var col in grid.Data.Columns)
{
    col.AggregationType = AggregationType.Sum; // Sum, Average, Count, Min, Max, First, Last, DistinctCount
}
```

Summary rows participate in total height and per-row height-before
calculations, and render in a dedicated pass after both sticky and
scrolling column passes. They appear only when the group is expanded.

---

## Virtualization

### Row virtualization (`EnableVirtualization`)

- When enabled, `Data.Rows` contains only the visible window, not all
  rows. `FirstVisibleRowIndex` returns `0` because `Data.Rows[0]` is
  always the first visible row.
- `GridRowVirtualizer.PublishToGrid()` materializes the window.
- `SyncGridRowCount()` triggers layout recalculation.
- Use `GridVirtualDataSource.FromList()`,
  `FromDataTable()`, or `FromDataView()` factories.

### Column virtualization (`EnableColumnVirtualization`)

- Tracks a horizontal window of scrolling (non-sticky) columns.
- `DrawRows()` and `DrawSummaryRowContent()` iterate only
  `FirstScrollingVisibleIndex`..`LastScrollingVisibleIndex`.
- Sticky columns are never virtualized (always rendered).

---

## Export Engine

CSV, JSON, and HTML are built into the main assembly. Excel and PDF
are plugin-based to avoid heavy NuGet dependencies.

```csharp
grid.ExportEngine.DiscoverPlugins();   // call once at app start
if (grid.ExportEngine.IsAvailable(GridExportFormat.Excel))
    grid.ExportToExcel("report.xlsx");
```

- `GridExportEngine.DiscoverPlugins()` scans
  `AppDomain.CurrentDomain.GetAssemblies()` for `IGridExporter`
  implementations. A real plugin assembly (e.g.
  `TheTechIdea.Beep.Winform.Controls.GridX.Export.Excel`) replaces the
  stub at runtime.
- Stubs (`GridExcelExporterStub`, `GridPdfExporterStub`) are registered
  with `IsAvailable = false` and surface in UI as grayed-out items.
- Exports honour `IsVisible` and the column display order, and skip
  hidden rows.

---

## Selection Modes

Selection mode is implemented as a strategy:

- `CellSelectionStrategy` — single active cell (default)
- `RowSelectionStrategy` — full-row selection via the leading checkbox
- `MultiRowSelectionStrategy` — Ctrl/Shift multi-row
- `MultiCellSelectionStrategy` — Ctrl/Shift multi-cell
- `ColumnSelectionStrategy` — column header click selects entire column

`SelectionMode` is the public property; `GridSelectionHelper` swaps the
strategy in response.

> **Note:** Despite the strategy switch, most current interaction logic
> still centres on active-cell focus plus checkbox row selection. The
> strategies are ready for use but the input handlers haven't all been
> updated to take advantage.

---

## Editor Framework

Editor classes encapsulate creation, setup, value access, and event
handling for each `BeepColumnType`:

- `BeepGridTextEditor`
- `BeepGridNumericEditor`
- `BeepGridMaskedEditor`
- `BeepGridComboBoxEditor`
- `BeepGridCheckBoxEditor`
- `BeepGridDateDropDownEditor`
- `BeepGridGenericEditor`

Resolution is by `GridEditorFactory` (`IGridEditor Resolve(BeepColumnType)`).
Register a custom editor:

```csharp
GridEditorFactory.Register(BeepColumnType.Text, new MyCustomTextEditor());
```

`GridEditHelper.BeginEdit()` is the main entry point from input
handlers. `OnCellValueChanged()` triggers the public `CellValueChanged`
event and also calls `RequestAutoSize(AutoSizeTriggerSource.EditCommit)`.

---

## Styling & Layout Presets

### `GridStyle`

`ApplyGridStyle()` implements:

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
- `Modern`

### `LayoutPreset`

`ApplyLayoutPreset(GridLayoutPreset)` wires the legacy preset set. New
preset classes can be applied directly:

```csharp
grid.ApplyLayoutPreset(new Material3SurfaceLayout());
```

`ApplyLayoutPreset(IGridLayoutPreset)` accepts any preset that
implements the interface.

---

## User Interaction

- Double-click a column border to best-fit one column.
- Press `Ctrl` while double-clicking a column border to best-fit all
  visible columns.
- Drag a non-system, non-sticky header to change `DisplayOrder`.
- Click the unified toolbar's search box to filter, advanced button for
  multi-criteria filter, filter button for Excel-style column popup.
- `Ctrl+C` / `Ctrl+X` / `Ctrl+V` for clipboard actions.
- `F2` or `Enter` to begin editing when the active cell is editable.
- `Ctrl+F` to focus the toolbar search box.
- `Esc` to cancel an active editor or toolbar search.
- `Tab` / `Shift+Tab` move between cells; intercept by
  `ProcessDialogKey` so focus does not leave the grid.
- Arrow keys, Home, End, PageUp, PageDown navigate the grid.

---

## Design-Time Support

The design server project exposes a `BeepGridProDesigner` smart-tag
surface with:

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

---

## Accessibility & Keyboard

`BeepGridPro.CreateAccessibilityInstance()` returns a
`GridAccessibleObject` (root, `Role = Table`).

- `GridRowAccessibleObject` (`Role = Row`) is a child of the root; its
  children are cells.
- `GridCellAccessibleObject` (`Role = Cell`) exposes `Name` (column
  caption), `Value` (cell text), `Bounds`, and `State`
  (Selected/Focused).
- Navigation: `Next` / `Previous` move between cells; `Up` / `Down` /
  `Left` / `Right` navigate the grid.
- `HitTest` maps screen coordinates to the cell accessible object under
  the cursor.

Full keyboard navigation is provided by `GridKeyboardNavigator` (Tab,
arrows, Home, End, PageUp, PageDown, Enter, Escape, F2). `Ctrl+F`
focuses the toolbar search box.

---

## Known Gaps & Roadmap

| Gap | Plan |
|---|---|
| `BeepGridStyle.Modern` is the latest style; verify its render in all themes. | Visual QA pass |
| The `LayoutPreset` enum is fully wired but some presets share the same default style heuristic. | Add per-preset painter params |
| Toolbar button tooltips are not yet shown. | Phase 19: per-button `ToolTip` member + auto-binding |
| `SelectionMode` strategy switch is implemented but most input handlers still use the legacy focus/checkbox path. | Phase 10 follow-up |
| Filtering has two parallel pipelines (`ActiveFilter` vs `SortFilter`). | Phase 20: unify the engines |
| `BeepFilterRow` and `BeepQuickFilterBar` are still defined; the toolbar supersedes them but the obsolete classes remain. | Phase 21: remove obsolete types |

---

## Related Docs

- [Claude.md](./Claude.md) — implementation guide (authoritative for
  code facts)
- [DESIGN_ARCHITECTURE.md](./DESIGN_ARCHITECTURE.md) — layer-by-layer
  architecture overview
- [DESIGN.md](./DESIGN.md) — visual + interaction design rationale
- [ENHANCEMENT_PLAN.md](./ENHANCEMENT_PLAN.md) — phased plan, current
  status
- [FOLDER_PLAN.md](./FOLDER_PLAN.md) — folder layout
- [TODO_TRACKER.md](./TODO_TRACKER.md) — per-task status
- [Enhancements/PHASE_018_UnifiedToolbar.md](./Enhancements/PHASE_018_UnifiedToolbar.md)
  — toolbar design rationale
- [Docs/](./Docs/) — additional reference docs (class index, events,
  filtering, painters)
- [Painters/](./Painters/) — header / navigator / filter-panel painters
- [Layouts/](./Layouts/) — layout presets
