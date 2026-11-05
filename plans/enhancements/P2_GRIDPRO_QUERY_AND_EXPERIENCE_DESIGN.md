# Phase 2: BeepGridPro query, layout, and experience enhancements — Design Document

## 0. Problem Statement

Phase 1 stabilizes the foundation the grid stands on. Phase 2 addresses the
next layer: the grid currently has several parallel sources of truth and no
shared way to read the view it is showing.

- Filtering and sorting are split across `BeepGridPro.Filtering.cs`
  (`ActiveFilter`) and `GridSortFilterHelper` (per-column contains/in). Both
  write `row.IsVisible` and neither reads the other.
- `Data.Rows` is the only view the renderer and exporters know. In virtual
  mode it holds only the materialized window, so filter, sort, and export see
  a subset of the data without saying so.
- Grouping is a third state owned by `GridGroupEngine`; export and virtual
  paging do not consume it.
- Column layout (order, width, sticky, visibility) lives only in memory on
  `BeepColumnConfig`; every host implements its own persistence.
- There is no way to express conditional formatting, no pinned aggregate
  footer, and export is synchronous, uncancellable, and UI-thread bound.

The five enhancements in this phase share one dependency: a single view model
that describes the current result of filter + sort + group + page. Everything
else consumes that view.

## 1. Goals

1. Provide one query pipeline (`filter → group → sort → page → view`) that is
   the only path any consumer uses to read rows and columns.
2. Make the pipeline virtualization-aware so windowed data sources participate
   in the same contract as in-memory lists.
3. Persist and restore per-user column layout (order, width, sticky,
   visibility) without host-specific glue.
4. Add conditional formatting rules and a pinned aggregate footer driven by the
   same view model.
5. Add asynchronous export with progress and cancellation, exposed through the
   existing export engine and plugin model.

## 2. Design

### Phase 2.A — Unified query pipeline

**Issue.** Filter, sort, and group each mutate the grid independently. A
filtered row set is represented by `row.IsVisible`; a sort reorders
`Data.Rows`; grouping is separate again. `_filteredRowIndices` is positional,
so it goes stale as soon as the view is reordered.

**Files likely involved.**
- `BeepGridPro.Filtering.cs`
- `Helpers/GridSortFilterHelper.cs`
- `Grouping/GridGroupEngine.cs`
- `Controllers/GridDataController.cs`
- `Helpers/GridDataHelper.cs`
- `BeepGridPro.cs` (virtualization entry points)
- `Virtualization/GridRowVirtualizer.cs`
- `Export/GridExportEngine.cs` and `Export/Grid*Exporter.cs`

**Change pattern.**

1. Add a UI-free query layer under `GridX/Query/`:
   - `GridQuery`: filter groups, sort descriptors, group descriptors, page
     offset, page size, and the source identity revision.
   - `GridQueryResult`: materialized rows, total count (pre-page), and the
     effective column projection.
   - `GridViewRow`: stable row identity, original item, cell values keyed by
     column name, group key path, and display index.
   - `GridQueryEngine`: composes filter, group, sort, and page in that order.
     No controls, no painting, no timers.
   - Reuse `Filtering.FilterCriteria` / `FilterOperator` where the shape is
     already right; only add `GridFilterGroup` for the group-level AND/OR and
     the serialization-friendly projection.
2. `GridDataController` owns a `GridView` built from `GridQueryEngine`. The
   renderer and exporters read `GridView.Rows`, not `Data.Rows`.
   `Data.Rows` becomes a projection maintained for compatibility with the
   existing editor and navigator paths.
3. Replace the two filter implementations with adapters that write
   `GridQuery.Filters` and re-run the pipeline:
   - `BeepGridPro.ActiveFilter`, `ApplyQuickFilter`, and the advanced dialog
     produce query criteria.
   - `GridSortFilterHelper.Filter` and `FilterIn` produce the same criteria.
   - The source-sort fast path is retained as an execution strategy inside
     `GridQueryEngine`, not as a parallel state machine.
4. Route `GridGroupEngine` through the pipeline. Group descriptors become
   `GridQuery.Groups`; the engine only materializes group headers and summary
   values from `GridQueryResult`.
5. Route export through the pipeline. `GridExportEngine` exports
   `GridView.Rows` using `ExportOptions` (which already controls visibility,
   display order, and hidden rows), so filtered/sorted/grouped state is
   exported exactly as displayed.
6. Route virtualization through the pipeline. In virtual mode,
   `GridRowVirtualizer` asks the query source for the requested page/window
   instead of reading `Data.Rows`.
7. Delete the duplicate `_filteredRowIndices` state. Public
   `FilteredRowIndices` is derived from `GridView.Rows` (display indices), and
   row identity is carried by `GridViewRow.RowId`.

**Acceptance criteria.**
- Filter, sort, and group applied in any order produce the same result.
- `FilteredRowIndices` remains correct after a sort.
- Export of a filtered, sorted, grouped grid matches the rendered order.
- Virtual mode filter/sort return results from the full source, not the
  materialized window.
- No consumer reads `Data.Rows` to decide what is displayed.

### Phase 2.B — Layout persistence per user

**Issue.** Column order (`DisplayOrder`), width, `Sticked`, and `Visible` are
runtime-only. Hosts reimplement save/restore, and `AutoGenerateColumns()`
wipes them on rebind.

**Files likely involved.**
- `Models/BeepColumnConfig.cs`
- `Models/BeepColumnConfigCollection`
- `BeepGridPro.Properties.cs` (`Columns`)
- `Helpers/GridDataHelper.cs` (`AutoGenerateColumns`)
- new `GridX/LayoutState/` folder

**Change pattern.**

1. Add `GridLayoutState` (versioned DTO): grid key, schema revision, and a list
   of `GridColumnLayout` entries keyed by `ColumnName` with `DisplayOrder`,
   `Width`, `Sticked`, and `Visible`. System columns (`Sel`, `RowNum`,
   `RowID`) are excluded.
2. Add `IGridLayoutStateStore` with `Load(key)` / `Save(key, state)` /
   `Delete(key)`. Ship `FileGridLayoutStateStore` (JSON, atomic
   write-to-temp-then-replace) as the default; hosts can supply a database or
   settings-backed store.
3. Add `BeepGridPro.SaveLayoutState(string key)`,
   `LoadLayoutState(string key)`, and `ResetLayoutState(string key)`. A key is
   `{application}/{user}/{gridName}` by convention; the grid never invents a
   user identity.
4. Apply state by column name, not index:
   - Unknown columns are ignored.
   - New columns are appended in their generated position.
   - Widths are clamped to `MinWidth`/`MaxWidth`.
   - Missing entries keep the current value.
5. Apply the state inside `AutoGenerateColumns()` after column generation so a
   rebind no longer discards a user's layout. Do not re-apply it on every
   layout pass.
6. Mark the state dirty on `ColumnPropertyChanged` for the four persisted
   fields; save explicitly, or via an optional debounced auto-save owned by the
   host.

**Acceptance criteria.**
- Save, rebind, restore reproduces order, width, sticky, and visibility.
- Removing a column from the model does not break restore.
- New columns appear with their generated defaults.
- System columns are never persisted.
- A corrupt or version-mismatched state file is ignored and reported, not
  applied partially.

### Phase 2.C — Server-side paging, sort, and filter contract

**Issue.** `IVirtualDataSource` exposes only `TotalRowCount` and
`GetRows(start, count)`. A remote source cannot express a filter, sort, or
page request, so the grid must materialize everything to query it.

**Files likely involved.**
- `Virtualization/IVirtualDataSource.cs`
- `Virtualization/GridVirtualDataSource.cs`
- `Virtualization/GridRowVirtualizer.cs`
- `GridX/Query/` (shared `GridQuery` / `GridQueryResult`)

**Change pattern.**

1. Add `IGridQuerySource` as a capability interface (it does not replace
   `IVirtualDataSource`):
   - `Task<GridQueryResult> QueryAsync(GridQuery query, CancellationToken ct)`
   - `long TotalCount { get; }`
   - `event EventHandler? QueryResultInvalidated`
2. `GridRowVirtualizer` detects `IGridQuerySource` first and falls back to the
   existing window contract for in-memory sources.
3. `GridQuery` is serialization-friendly (no delegates, no UI types) so a
   transport adapter can translate it to SQL, OData, or a service call.
4. Add a query revision/generation number. A response for an older revision is
   discarded, so fast typing cannot apply out-of-order results.
5. Cancellation is end-to-end: a new query cancels the previous token, and the
   virtualizer ignores cancelled results.
6. `GridVirtualDataSource` implements `IGridQuerySource` in memory so tests and
   in-memory grids exercise the same path as remote sources.

**Acceptance criteria.**
- A remote implementation can page, sort, filter, and group without loading
  the full set.
- Out-of-order responses never replace newer results.
- Cancelling a query does not leave the grid on a spinner or partial window.
- In-memory and remote sources are interchangeable behind `IGridQuerySource`.

### Phase 2.D — Conditional formatting and pinned aggregate footer

**Issue.** There is no rule model for row/cell styling, and aggregates exist
only inside group summary rows. Users cannot pin a totals row at the bottom of
the grid.

**Files likely involved.**
- `Models/BeepColumnConfig.cs` (aggregation already exists)
- `Helpers/GridRenderHelper.cs` and `GridRenderHelper.Rendering.cs`
- `Helpers/GridLayoutHelper.cs` (new `FooterRect`)
- `Grouping/GridGroupSummaryRow.cs` / aggregate engine
- new `GridX/Formatting/` and `GridX/Footer/` folders
- `BeepGridPro.Properties.cs`

**Change pattern.**

1. Add `ConditionalFormatRule`:
   - target (`AllCells`, `Column`, `Row`)
   - column name(s)
   - operator and value(s) (reuse the filter operator vocabulary)
   - `GridCellStyle` (background, foreground, font style)
   - `Priority`, `StopIfTrue`, `IsEnabled`
2. Add `GridConditionalFormatEngine` that evaluates rules once per
   `GridQueryResult` revision and returns a per-cell style map. Rendering looks
   up the map; it does not evaluate rules during paint.
3. Define precedence explicitly: focused/selected state wins over conditional
   formatting; conditional formatting wins over style/theme defaults. Expose the
   precedence as a documented constant, not an accident of paint order.
4. Add `GridFooterConfig` with per-column `AggregationType`, a footer height,
   and `ShowFooter`. Aggregates are computed over the filtered view (not the
   current page) using the same aggregate engine as group summaries.
5. Add `FooterRect` to `GridLayoutHelper`, pinned between the rows and the
   navigator. It participates in row viewport height, scrollbar range, and
   hit-testing the same way the header does.
6. Render the footer in `GridRenderHelper` in its own pass, including sticky
   columns, horizontal scroll offset, and `FocusedCell` exclusion.
7. Expose `grid.Footer`, `grid.ConditionalFormats`, `grid.ShowFooter`, and
   `grid.FooterHeight`, plus a designer smart-tag verb to edit the aggregate
   columns.

**Acceptance criteria.**
- A rule can recolor a cell/row without changing the underlying value.
- Multiple rules resolve deterministically by priority and `StopIfTrue`.
- The footer shows one aggregate per configured column and updates on
  filter/sort/data change.
- Footer is pinned while rows scroll and never overlaps the navigator.
- Footer aggregate scope is the filtered view, not the current page.

### Phase 2.E — Async export with progress and cancellation

**Issue.** `IGridExporter.Export(BeepGridPro grid, Stream, ExportOptions)`
requires the grid and runs synchronously on the UI thread. Large exports freeze
the UI and cannot be cancelled.

**Files likely involved.**
- `Export/IGridExporter.cs`
- `Export/GridExportEngine.cs`
- `Export/GridCsvExporter.cs`, `GridJsonExporter.cs`, `GridHtmlExporter.cs`
- `Export/GridExcelExporterStub.cs`, `GridPdfExporterStub.cs`
- `BeepGridPro.cs` export methods
- `BeepGridPro.ContextMenu.cs` export menu

**Change pattern.**

1. Add `IAsyncGridExporter`:
   - `Task ExportAsync(GridExportSnapshot snapshot, Stream output, ExportOptions? options, IProgress<GridExportProgress>? progress, CancellationToken ct)`
2. Add `GridExportSnapshot`: a UI-thread-created, immutable copy of columns,
   rows, options, title, and timestamp. It contains no `Control` references, so
   export never touches the grid from a background thread.
3. `GridExportEngine.ExportAsync(...)` snapshots on the UI thread and resolves
   an async exporter if the format has one. Built-in CSV/JSON/HTML exporters
   implement the async interface directly. A legacy `IGridExporter` plugin
   keeps working through the existing synchronous path on the caller thread
   (unchanged behavior) until it opts in to `IAsyncGridExporter`.
4. Wire progress (`rows processed / total rows`) and cancellation through the
   engine and into the export menu. Cancellation deletes the partial file.
5. Failures are reported through `ReportOperationError` and surface a
   non-blocking error, not a `MessageBox` on a worker thread.
6. Built-in CSV/JSON/HTML exporters implement the async interface directly.
   The Excel/PDF stubs advertise `IAsyncGridExporter` as unavailable until a
   real plugin is loaded.

**Acceptance criteria.**
- Exporting a large grid keeps the UI responsive.
- Progress reports monotonic row counts.
- Cancel stops the export and leaves no partial/corrupt file.
- The snapshot reflects the view at the moment export was requested.
- Existing synchronous exporters and plugins still work.

## 3. API surface

```csharp
public sealed class GridQuery
{
    public List<GridFilterGroup> Filters { get; } = new();
    public List<GridSortDescriptor> Sorts { get; } = new();
    public List<GridGroupDescriptor> Groups { get; } = new();
    public int Offset { get; set; }
    public int PageSize { get; set; } = -1; // -1 = all
    public long Revision { get; set; }
}

public interface IGridQuerySource
{
    long TotalCount { get; }
    Task<GridQueryResult> QueryAsync(GridQuery query, CancellationToken ct);
    event EventHandler? QueryResultInvalidated;
}

public sealed class GridLayoutState
{
    public int Version { get; set; } = 1;
    public string GridKey { get; set; } = string.Empty;
    public List<GridColumnLayout> Columns { get; set; } = new();
}

public interface IGridLayoutStateStore
{
    GridLayoutState? Load(string key);
    void Save(string key, GridLayoutState state);
    void Delete(string key);
}

public sealed class ConditionalFormatRule
{
    public string ColumnName { get; set; } = string.Empty;
    public GridFormatTarget Target { get; set; }
    public FilterOperator Operator { get; set; }
    public string? Value { get; set; }
    public GridCellStyle Style { get; set; }
    public int Priority { get; set; }
    public bool StopIfTrue { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public sealed class GridFooterConfig
{
    public bool Visible { get; set; }
    public int Height { get; set; }
    public Dictionary<string, AggregationType> ColumnAggregates { get; } = new();
}

public interface IAsyncGridExporter
{
    GridExportFormat Format { get; }
    bool IsAvailable { get; }
    Task ExportAsync(
        GridExportSnapshot snapshot,
        Stream output,
        ExportOptions? options,
        IProgress<GridExportProgress>? progress,
        CancellationToken cancellationToken);
}
```

## 4. Files to change

| Action | File | Risk |
|--------|------|------|
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Query/GridQuery.cs` | medium |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Query/GridQueryEngine.cs` | high |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Query/GridView.cs` | high |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Controllers/GridDataController.cs` | high |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/BeepGridPro.Filtering.cs` | high |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Helpers/GridSortFilterHelper.cs` | high |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Grouping/GridGroupEngine.cs` | medium |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/LayoutState/GridLayoutState.cs` | low |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/LayoutState/FileGridLayoutStateStore.cs` | low |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/BeepGridPro.Properties.cs` | medium |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Helpers/GridDataHelper.cs` | medium |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Query/IGridQuerySource.cs` | medium |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Virtualization/GridRowVirtualizer.cs` | high |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Virtualization/GridVirtualDataSource.cs` | medium |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Formatting/ConditionalFormatRule.cs` | medium |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Formatting/GridConditionalFormatEngine.cs` | medium |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Footer/GridFooterConfig.cs` | medium |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Helpers/GridLayoutHelper.cs` | high |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Helpers/GridRenderHelper.Rendering.cs` | high |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Export/IAsyncGridExporter.cs` | low |
| New | `TheTechIdea.Beep.Winform.Controls/GridX/Export/GridExportSnapshot.cs` | medium |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Export/GridExportEngine.cs` | medium |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Export/GridCsvExporter.cs` | low |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Export/GridJsonExporter.cs` | low |
| Modify | `TheTechIdea.Beep.Winform.Controls/GridX/Export/GridHtmlExporter.cs` | low |

## 5. Error handling matrix

| Operation | Before | After |
|-----------|--------|-------|
| Query source throws | Unobserved / swallowed catch | Reported through `OperationError`; previous view retained |
| Query cancelled | Partial rows may be published | Cancelled result discarded; previous view retained |
| Corrupt layout state | Host-specific; often crash | Ignored with a reported warning; grid keeps generated defaults |
| Unknown column in layout state | N/A | Ignored by name |
| Conditional rule throws | N/A | Rule is skipped and reported once; cell renders normally |
| Footer aggregation fails | N/A | Footer cell shows `—`; failure reported |
| Export fails mid-write | Partial file remains | Partial file deleted; failure reported |
| Export cancelled | N/A | Partial file deleted; no error dialog |

## 6. Backward compatibility

- `IGridExporter` and `IVirtualDataSource` stay valid; `IAsyncGridExporter`
  and `IGridQuerySource` are additive capability interfaces.
- Public filter/sort method names (`ApplyQuickFilter`, `ClearFilter`,
  `GroupBy`, `SortFilter`) stay, but their internal state is replaced by the
  query pipeline. No second implementation is kept beside it.
- `Data.Rows` remains available as a projection for existing hosts and
  editor/navigator code during the transition; the renderer and exporters stop
  depending on it.
- Layout state is versioned; a future schema bump can migrate rather than
  discard.

## 7. Verification

Per-task gate:
- `dotnet build TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj`
- A focused test that fails before the change and passes after.

Per-phase gate:
- `dotnet test TheTechIdea.Beep.Winform.Controls.Tests --filter "BeepGridPro"`
  (after the test project's unrelated project reference is repaired or the
  GridX tests are split into a buildable project).

Required parity checks:
1. Filter + sort + group + export parity: the exported rows equal the
   rendered order and hidden rows stay hidden.
2. `FilteredRowIndices` after sort.
3. Virtual window query with `IGridQuerySource`: total count, page contents,
   cancellation, stale-response discard.
4. Layout round-trip: save → rebind → restore.
5. Conditional formatting precedence: selected > rule > style.
6. Footer aggregate over filtered view vs page.
7. Async export progress/cancel and UI-thread safety (`Control.InvokeRequired`
   is never true inside the exporter).

## 8. Risks

| Risk | Mitigation |
|------|------------|
| Big-bang rewrite of two filter systems | Migrate behind `GridQueryEngine` with parity tests before deleting old state; one commit per consumer |
| Virtual and in-memory paths diverge | Make `GridVirtualDataSource` implement `IGridQuerySource` and run the same tests on both |
| Grouping semantics change | Keep `GridGroupEngine` as the materializer; only move its input to `GridQuery.Groups` |
| Column identity collisions (duplicate names) | Key by name and reject duplicates at generation time; document as an invariant |
| Background export touching controls | Export only `GridExportSnapshot`; add a test that no `Control` is reachable |
| Conditional formatting cost | Evaluate once per query revision, cache by revision and column |
| Footer changes row viewport math | Add `FooterRect` to the same height/scroll calculations as the toolbar and navigator; test scrollbar range |

## 9. Out of scope

- A visual conditional-format rule designer beyond a basic smart-tag list editor.
- Cross-grid layout sharing or server-side user profiles.
- Remote transport implementations (SQL/OData/HTTP); this phase defines the
  contract and the in-memory implementation only.
- Replacing Excel/PDF exporters with real implementations.
- Changes to `BaseControl` painting contract (Phase 1).

## 10. Sub-task execution order

1. **P2.A** — Query model + engine + view, then migrate filter, sort, group,
   export, and virtualization consumers one at a time.
2. **P2.B** — Layout state DTO + store + apply-on-generate (independent of A).
3. **P2.C** — `IGridQuerySource` + virtualizer integration + in-memory source.
4. **P2.D** — Conditional formatting engine, then footer layout/render.
5. **P2.E** — Snapshot + async interface + engine + built-in exporters + menu.
6. **P2.M** — Parity, layout round-trip, query cancellation, formatting
   precedence, footer scope, and async export validation.

P2.A is the dependency for P2.C, P2.D, and P2.E. P2.B can run in parallel.
