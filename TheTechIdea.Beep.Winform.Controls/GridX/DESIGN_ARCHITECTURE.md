# BeepGridPro — Design Architecture

_Authoritative reference for the layer-by-layer structure of `BeepGridPro`._

## 1. Layered Model

`BeepGridPro` is organized into five layers, with strict downward
dependencies (lower layers do not know about higher layers):

```
┌────────────────────────────────────────────────────────────────┐
│  L5  Public surface       BeepGridPro + .Properties / .Events  │
│                              .Dialogs / .Filtering / .Input    │
├────────────────────────────────────────────────────────────────┤
│  L4  Behaviour helpers    GridInputHelper, GridSelectionHelper │
│                           GridNavigatorHelper, GridEditHelper   │
│                           GridSortFilterHelper,                  │
│                           GridSizingHelper, GridDialogHelper    │
│                           GridColumnReorderHelper,              │
│                           GridKeyboardNavigator,                │
│                           GridFocusManager                     │
├────────────────────────────────────────────────────────────────┤
│  L3  Layout & geometry    GridLayoutHelper                    │
├────────────────────────────────────────────────────────────────┤
│  L2  Data + paint         GridDataHelper, GridRenderHelper     │
│                           GridScrollHelper, GridScrollBarsHelper│
│                           GridDataController                   │
├────────────────────────────────────────────────────────────────┤
│  L1  Models + utilities   BeepColumnConfig / BeepRowConfig /   │
│                           BeepCellConfig                       │
│                           Toolbar/BeepGridToolbarState          │
│                           Toolbar/BeepGridToolbarPainter        │
│                           Filtering/FilterEditorHelper          │
│                           Export/*                              │
│                           Painters/* (header / navigation /     │
│                             filter panel)                       │
│                           Layouts/* (per-style padding / radius)│
│                           Selection/* (strategy pattern)        │
│                           Grouping/*                            │
│                           Virtualization/*                      │
│                           Accessibility/*                       │
└────────────────────────────────────────────────────────────────┘
```

The public surface depends on every lower layer. Lower layers never
reference the public surface. This is enforced by:

- All helpers take `BeepGridPro` in their constructor but only call
  a small, curated subset of its properties.
- Helpers cross-talk through `_grid` (the back-reference) plus typed
  parameters and events; no helper references another's private state.
- The helper is the integration point: `BeepGridPro` constructs them
  in its constructor and is the only object that wires them together.

## 2. Helper Composition

```csharp
// BeepGridPro.cs:80 (excerpt)
public BeepGridPro() : base()
{
    Layout                = new GridLayoutHelper(this);
    Data                  = new GridDataHelper(this);
    DataController         = new GridDataController(this);
    Render                 = new GridRenderHelper(this);
    Selection              = new GridSelectionHelper(this);
    Input                  = new GridInputHelper(this);
    Scroll                 = new GridScrollHelper(this);
    ScrollBars             = new GridScrollBarsHelper(this);
    SortFilter             = new GridSortFilterHelper(this);
    Edit                   = new GridEditHelper(this);
    ThemeHelper            = new GridThemeHelper(this);
    Navigator              = new GridNavigatorHelper(this);
    NavigatorPainter       = new GridNavigationPainterHelper(this);
    KeyboardNavigator      = new GridKeyboardNavigator(this);
    FocusManager          = new GridFocusManager(this);
    _uowBinder             = new GridUnitOfWorkBinder(this);
    Sizing                 = new GridSizingHelper(this);
    Dialog                 = new GridDialogHelper(this);
    Clipboard              = new GridClipboardHelper(this);
    ColumnReorder          = new GridColumnReorderHelper(this);
    _toolbarPainter        = new BeepGridToolbarPainter(this);
    _filterEditor          = new FilterEditorHelper(this);
    GroupEngine            = new GridGroupEngine(this);
}
```

## 3. The Render Pipeline

`DrawContent(Graphics)` is the single entry point for painting:

```
┌──────────────────────────┐
│  DrawContent (override)  │
└──────────┬───────────────┘
           │
           ▼
   ┌──────────────────┐
   │ Layout.EnsureCalc│ ──► recalculate when scroll/rows/visibility changed
   └──────┬───────────┘
          │
          ▼
   ┌──────────────────┐
   │ Render.Draw       │ ──► Header (sticky + scrolling), rows, filter
   │                   │     panel, navigator, toolbar, focus, hover
   └──────┬───────────┘
          │
          ▼
   ┌──────────────────┐
   │ ScrollBars.Draw    │ ──► Custom-painted vertical + horizontal bars
   └──────────────────┘
```

`GridRenderHelper.Draw` is itself a small orchestrator that delegates
to:

- `_painterHelper.DrawTopFilterPanel(g)` — top filter panel
- `DrawColumnHeaders(g)` — column header painter
- `DrawRows(g)` — data rows (with virtualization)
- `DrawGroupHeaders(g)` — group expand/collapse headers
- `DrawSummaryRows(g)` — group aggregation summary rows
- `_toolbarPainter.Paint(g, ...)` — unified toolbar
- `_navigatorPainter.DrawNavigator(g)` — owner-drawn navigator

### Render rule

> Never add a child control for cells, headers, navigator, scrollbars,
> filter panel, or toolbar (the search-editor is the only exception,
> and it is *activated on demand* and hidden after commit).
> All visuals are painted in `GridRenderHelper` or one of the painter
> classes.

## 4. The Input Pipeline

`OnMouseDown` / `OnMouseMove` / `OnMouseWheel` / `OnKeyDown` are
overridden in `BeepGridPro.Input.cs`. They call into
`GridInputHelper.HandleXxx(...)` which performs hit testing and routes
events to the appropriate helper:

```
OnMouseDown
   │
   ├──► HandleToolbarMouseDown (toolbar buttons + search)
   ├──► HandleFilterPanelMouseDown (legacy top filter panel)
   ├──► HandleGroupHeaderClick (group expand/collapse)
   ├──► HandleColumnHeaderClick (sort + reorder + filter icon)
   ├──► HandleDataCellClick (selection + double-click edit)
   └──► HandleExpanderClick (row expander)
```

`OnKeyDown` is intercepted by `ProcessDialogKey` so `Tab` and
`Shift+Tab` route through `GridInputHelper.HandleKeyDown` instead of
moving focus out of the grid. `Ctrl+F` is handled at the control level
in `OnKeyDown` to focus the toolbar search box.

## 5. Data Flow

`GridDataHelper` is the only helper that talks to the `DataSource`. It
is responsible for:

1. Detecting the source type (`DataTable`, `BindingSource`,
   `IEnumerable<T>`, root object, UoW).
2. Resolving the schema and creating `BeepColumnConfig` instances via
   `AutoGenerateColumns()`.
3. Materializing rows into `BeepRowConfig` and cells into
   `BeepCellConfig`.
4. Subscribing to `INotifyCollectionChanged` (and the special
   `BindingSource` events) for live updates.
5. Re-syncing row visibility when `ActiveFilter` is applied
   (`ApplyActiveFilter`).
6. Forwarding cell edits back to the source via
   `UpdateCellValue`.

The data flow diagram:

```
DataSource
   │
   ▼
GridDataHelper.Bind() ── AutoGenerateColumns() ── CreateColumns
   │                                                  │
   │                                                  ▼
   │                                            BeepColumnConfig
   │
   ▼
INotifyCollectionChanged / BindingSource.ListChanged
   │
   ▼
RefreshRows() ── CreateOrUpdateRow ── BeepRowConfig
                                          │
                                          ▼
                                    BeepCellConfig (per cell)
```

## 6. Filter Pipeline

Two parallel filter systems exist by design:

```
                       ActiveFilter path           SortFilter path
                       ─────────────────           ────────────────
Public API            ApplyQuickFilter             (none — internal)
                      ShowAdvancedFilterDialog
                      AddFilterCriterion
                      ClearFilter
                      Toolbar search box

State                 BeepRowConfig.IsVisible      BindingSource.Sort / Filter
                                                      or local visibility fallback

Events raised         FilterApplied                (none)
                      FilterCleared

Pipeline
                      ActiveFilter (per-grid)     SortFilter (per-helper)
                          │                            │
                          ▼                            ▼
                      UpdateRowsVisibility        ApplySortOrFilter
                          │                            │
                          └────────► RecalculateLayout ◄┘
```

The two systems share the `BeepRowConfig.IsVisible` flag for the
"fallback" case, but they are independent in code. Any new filtering
feature should pick one and stay in its lane.

## 7. The Unified Toolbar (Phase 18)

The toolbar is a separate subsystem with its own layout + paint state.
It is not a child control of the grid — it is owner-drawn inside the
`ToolbarRect` computed by `GridLayoutHelper`.

```
BeepGridPro (host)
  ├── _toolbarPainter  : BeepGridToolbarPainter
  ├── _filterEditor    : FilterEditorHelper   (on-demand BeepTextBox)
  └── _toolbarState    : BeepGridToolbarState (model)

Layout: ToolbarRect   = top 0..ToolbarHeight × ClientWidth
Paint:  GridRenderHelper.Rendering.cs:38  →  ToolbarPainter.Paint(g, rect, state)
Input:  GridInputHelper.cs:986  →  HitTest → HandleToolbarButtonClick
```

`FilterEditorHelper` is the *only* place where a real child control
(an on-demand `BeepTextBox`) is created for the toolbar. The control
is added to `BeepGridPro.Controls`, sized to `SearchBoxRect`, focused
on click, and hidden on commit/cancel. The `LostFocus` event triggers
`CommitSearch()` so the search text is always applied even if the user
clicks away.

## 8. Virtualization Architecture

```
IVirtualDataSource (factory)
   │  • FromList(IEnumerable)
   │  • FromDataTable(DataTable, columnNames)
   │  • FromDataView(DataView, columnNames)
   │
   ▼
GridRowVirtualizer  ── UpdateWindow(scroll, viewport, rowHeight)
   │                       │
   │                       ▼
   │                  PublishToGrid() ── Data.Rows.Clear + AddRange(window)
   │
   ▼
GridColumnVirtualizer ── UpdateWindow(horizontalScroll, viewportWidth)
   │                          │
   │                          ▼
   │                    FirstScrollingVisibleIndex / LastScrollingVisibleIndex
   │
   ▼
GridRenderHelper.DrawRows  ── iterate only visible window
   │
   ▼
GridScrollHelper           ── adjust virtual scroll positions
```

When virtualization is active, `Data.Rows` contains **only the visible
window**, not the full source. `FirstVisibleRowIndex` always returns 0
in virtual mode because `Data.Rows[0]` is the first visible row. The
total logical count is exposed via `VirtualRowCount`.

## 9. Selection Strategy

The selection subsystem uses the **strategy pattern**:

```
BeepGridPro
   │
   ▼
GridSelectionHelper  ── holds ISelectionStrategy
                              │
                              ▼
                  CellSelectionStrategy   (default)
                  RowSelectionStrategy
                  MultiCellSelectionStrategy
                  MultiRowSelectionStrategy
                  ColumnSelectionStrategy
```

The strategy is responsible for translating `OnMouseDown` /
`OnKeyDown` events into selection changes. The default strategy is
`CellSelectionStrategy`, which only updates the active cell. Checkbox
row selection is *separate* from the strategy — it lives in
`BeepRowConfig.IsSelected` and is set by `GridInputHelper` directly
when the leading column checkbox is clicked.

> **Migration note:** Most input handlers were written before the
> strategy pattern was introduced, so the legacy single-cell focus
> + checkbox-row-selection behaviour is still the dominant
> implementation. The strategy classes are correct and unit-tested;
> the migration of the input handlers is tracked as a Phase 10
> follow-up.

## 10. Editor Framework

```
GridEditHelper  ── IGridEditorEvents
   │                   │
   │                   │  callbacks: RequestEndEdit, RequestCancelEdit
   │                   │
   ▼                   ▼
GridEditorFactory.Resolve(BeepColumnType)
   │
   ▼
IGridEditor  (e.g. BeepGridTextEditor)
   │   • CreateControl()
   │   • Setup(control, column, cell, theme)
   │   • GetValue(control)
   │   • SetValue(control, value)
   │   • AttachEvents(control, events)
   │   • DetachEvents(control, events)
   │   • IsPopupOpen(control)
   │   • OnBeginEdit(control)
```

Custom editors register at runtime:

```csharp
GridEditorFactory.Register(BeepColumnType.Text, new MyEditor());
```

The framework returns `null` for unknown column types. The caller
(`GridEditHelper.BeginEdit`) treats `null` as "no editor available" and
silently skips editing.

## 11. Export Subsystem

```
BeepGridPro.ExportEngine (GridExportEngine)
   │  • DiscoverPlugins()  — AppDomain scan for IGridExporter
   │  • IsAvailable(format)
   │
   ▼
Built-in         Pluggable (post-DiscoverPlugins)
─────────        ─────────────────────────────
GridCsvExporter  GridExcelExporterStub      ──►  Real plugin
GridJsonExporter  GridPdfExporterStub        ──►  Real plugin
GridHtmlExporter
```

The grid exposes `ExportToCsv / ExportToJson / ExportToHtml /
ExportToExcel / ExportToPdf` plus `ExportToStream` and
`ExportToString` for in-process use. The Excel and PDF stubs
participate in `IsAvailable` checks so menus can grey them out until a
real plugin is loaded.

## 12. Dependency Rules

These rules keep the architecture stable:

1. **Helpers are stateless across paint cycles.** All per-frame state
   lives in the helper instance fields, never in `BeepGridPro`.
2. **Cross-helper coordination goes through `BeepGridPro`.** Helpers
   do not hold references to each other; they read from `_grid`'s
   helper properties.
3. **Public methods on helpers are explicit.** `internal` helpers
   expose `public` methods only when `BeepGridPro` (or its public
   surface) needs them.
4. **The control does not patch `DrawContent` or `OnMouseDown` for
   one-off features.** New features go into the appropriate helper or
   a new one.
5. **New layout / style / painter classes are pluggable via the
   `IGridLayoutPreset` / `BaseHeaderPainter` / `BaseNavigationPainter`
   / `BaseFilterPanelPainter` interfaces.** They register themselves
   in the corresponding factory and are selected by the `GridStyle`
   property.

## 13. Threading

`BeepGridPro` is **not** thread-safe. All reads and writes to
`Data.Rows`, `Selection.RowIndex`, `ToolbarState.HoveredButtonKey`,
and similar fields must happen on the UI thread. The animation timer
in `BeepRadioGroup` (Pass 1) is a counter-example: `BeepGridPro` does
not own a `System.Windows.Forms.Timer`. Cell editing fires
`CellValueChanged` synchronously from the editor's commit; the host
can dispatch to a background thread from there.

## 14. Memory & Lifetime

`Dispose` is overridden in `BeepGridPro` to:

- Dispose the on-demand search editor via `FilterEditor.Dispose()`.
- Detach the UoW binder.
- Dispose dialog helpers.
- Dispose the focus manager.
- Clear the clipboard cut-cells buffer.
- Dispose the navigator and clear the binding.
- Stop + dispose the debounce timer.
- Drop the virtual data source.

Helpers are not individually `IDisposable` — they are GC-collected
along with the grid. The child-control editors and the on-demand
search text box are the only components that need explicit disposal.

---

## See Also

- [README.md](./README.md) — public surface overview
- [DESIGN.md](./DESIGN.md) — visual / interaction design rationale
- [Claude.md](./Claude.md) — code-level invariants
