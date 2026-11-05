# Phase 2: BeepGridPro query, layout, and experience enhancements — Task List

> **Design doc:** [P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md](P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md)

## Phase 2.A — Unified query pipeline

| # | Task | Status |
|---|------|--------|
| 2.A.1 | Add `GridQuery`, `GridSortDescriptor`, `GridGroupDescriptor`, `GridFilterGroup`, `GridQueryResult`, `GridViewRow` under `GridX/Query/` | ✅ |
| 2.A.2 | Implement `GridQueryEngine` (filter → group → sort → page) with no UI or control references | ✅ |
| 2.A.3 | Add `GridView` owned by `GridDataController`; make it the read surface for renderer and exporters | ✅ |
| 2.A.4 | Migrate `BeepGridPro.Filtering` (`ActiveFilter`, quick filter, advanced dialog) to query criteria | ✅ |
| 2.A.5 | Migrate `GridSortFilterHelper` contains/in filters and source-sort fast path into the pipeline | ✅ |
| 2.A.6 | Move grouping input from `GridGroupEngine` to `GridQuery.Groups`; keep the engine as materializer only | ✅ |
| 2.A.7 | Delete `_filteredRowIndices`; derive `FilteredRowIndices` from the view | ✅ |
| 2.A.8 | Route `GridExportEngine` through the view model, including group/visibility/order handling | ✅ |
| 2.A.9 | Route `GridRowVirtualizer` through the pipeline for window materialization | ✅ |
| 2.A.10 | Add parity tests: filter/sort/group/export, `FilteredRowIndices` after sort, virtual window results | ✅ |

## Phase 2.B — Layout persistence per user

| # | Task | Status |
|---|------|--------|
| 2.B.1 | Add versioned `GridLayoutState` + `GridColumnLayout` DTOs keyed by column name | ✅ |
| 2.B.2 | Add `IGridLayoutStateStore` and `FileGridLayoutStateStore` with atomic JSON writes | ✅ |
| 2.B.3 | Add `BeepGridPro.SaveLayoutState` / `LoadLayoutState` / `ResetLayoutState` | ✅ |
| 2.B.4 | Apply state by name (unknown ignored, new columns appended, widths clamped, system columns excluded) | ✅ |
| 2.B.5 | Re-apply saved state after `AutoGenerateColumns()` so rebinds do not wipe user layout | ✅ |
| 2.B.6 | Mark state dirty on `DisplayOrder` / `Width` / `Sticked` / `Visible` changes | ✅ |
| 2.B.7 | Add round-trip, removed-column, new-column, and corrupt-file tests | ✅ |

## Phase 2.C — Server-side paging, sort, and filter contract

| # | Task | Status |
|---|------|--------|
| 2.C.1 | Add `IGridQuerySource` (`TotalCount`, `QueryAsync`, `QueryResultInvalidated`) as an additive capability interface | ✅ |
| 2.C.2 | Make `GridQuery` serialization-friendly for transport adapters | ✅ |
| 2.C.3 | Teach `GridRowVirtualizer` to prefer `IGridQuerySource` and fall back to `IVirtualDataSource` | ✅ |
| 2.C.4 | Add query revision/generation so stale responses are discarded | ✅ |
| 2.C.5 | Wire end-to-end cancellation: new query cancels the previous token | ✅ |
| 2.C.6 | Implement `IGridQuerySource` in `GridVirtualDataSource` for in-memory parity | ✅ |
| 2.C.7 | Add tests for paging, sort, filter, stale response, cancellation, and error retention | ✅ |

## Phase 2.D — Conditional formatting and pinned aggregate footer

| # | Task | Status |
|---|------|--------|
| 2.D.1 | Add `ConditionalFormatRule` with target, operator, value, `CellStyle`, priority, `StopIfTrue` | ✅ |
| 2.D.2 | Add `GridConditionalFormatEngine` evaluating once per view revision into a cached style map | ✅ |
| 2.D.3 | Define and document precedence: focused/selected > conditional rule > style/theme | ✅ |
| 2.D.4 | Integrate the style map into cell/row background and foreground resolution | ✅ |
| 2.D.5 | Add `GridFooterConfig` (visible, height, per-column `AggregationType`) | ✅ |
| 2.D.6 | Add `FooterRect` to layout, viewport height, scrollbar range, and hit-testing | ✅ |
| 2.D.7 | Render pinned footer (sticky + scrolling columns) in its own pass | ✅ |
| 2.D.8 | Compute footer aggregates over the filtered view using the shared aggregate engine | ✅ |
| 2.D.9 | Add designer smart-tag verb to edit footer aggregates | ⬜ |
| 2.D.10 | Add precedence, footer scope, scrolling, and update-on-filter tests | ✅ |

## Phase 2.E — Async export with progress and cancellation

| # | Task | Status |
|---|------|--------|
| 2.E.1 | Add `IAsyncGridExporter` with `Task`, `IProgress<GridExportProgress>`, and `CancellationToken` | ✅ |
| 2.E.2 | Add immutable `GridExportSnapshot` created on the UI thread; no control references | ✅ |
| 2.E.3 | Add `GridExportEngine.ExportAsync` that snapshots first, then runs sync or async exporter | ✅ |
| 2.E.4 | Wire progress and cancellation; delete partial output on cancel/failure | ✅ |
| 2.E.5 | Implement async export in CSV, JSON, and HTML exporters | ✅ |
| 2.E.6 | Keep Excel/PDF stubs unavailable until a real plugin exists; allow plugins to opt in | ✅ |
| 2.E.7 | Update the export menu to use the async path with progress and cancel | ✅ |
| 2.E.8 | Report failures through `ReportOperationError`; never raise UI dialogs on a worker thread | ✅ |
| 2.E.9 | Add responsiveness, monotonic progress, cancel cleanup, snapshot, and thread-safety tests | ✅ |

## Phase 2.M — Full focused validation

| # | Task | Status |
|---|------|--------|
| 2.M.1 | Run the query/export parity suite | ✅ |
| 2.M.2 | Run the layout round-trip suite | ✅ |
| 2.M.3 | Run the query-source paging/sort/filter/cancel suite | ✅ |
| 2.M.4 | Run conditional-format precedence and footer aggregate/scroll suites | ✅ |
| 2.M.5 | Run async export progress/cancel/UI-thread suite | ✅ |
| 2.M.6 | Review the final backlog against the design doc and record what was not verified | ✅ |

## Issue-by-issue backlog summary

1. Filtering, sorting, and grouping are three states with no shared read model; virtualization and export see inconsistent views.
2. Column layout is memory-only and is wiped by column regeneration.
3. `IVirtualDataSource` cannot express paging, sort, or filter, forcing full materialization for remote sources.
4. No conditional formatting rules and no pinned aggregate footer.
5. Export is synchronous, UI-thread bound, and uncancellable.

## Open decisions

1. **Persistence store default.** Recommended: `IGridLayoutStateStore` + JSON file store, with the key supplied by the host. Rejected: grid-managed per-user identity.
2. **Export grouping semantics.** Recommended: flatten groups by default (rows in display order) with an option to include group header rows. Rejected: always emitting group banners, which breaks CSV consumers.
3. **Footer aggregate scope.** Recommended: filtered view. Rejected: current page only, which makes totals change while paging.
4. **Async plugin compatibility.** Recommended: additive `IAsyncGridExporter`; the engine wraps existing sync exporters on `Task.Run` against a snapshot. Rejected: changing `IGridExporter` (breaks plugins).
5. **Conditional formatting precedence.** Recommended: selection/focus wins over rules. Rejected: rules overriding the active-cell indicator, which hides the user's position.
6. **Test project split.** Recommended: move GridX tests into a buildable test project so this phase has real regression gates. The current test project cannot build because of the unrelated `Beep.Winform.Data.Integrated.Controls` reference.

## Verification notes (actual runs)

Observed on the working tree:

- `dotnet build TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj -c Debug` — succeeded.
- An external harness (`gridx-verify`) executed 31 checks against the built assembly:
  - query filter + sort + page + display indices — passed
  - grouping order and within-group sort — passed
  - `GridVirtualDataSource.QueryAsync` filter/page — passed
  - layout store save/load/delete — passed
  - corrupt layout file ignored — passed
  - real `BeepGridPro` bind, column generation, save/load layout, width/order apply — passed
  - `GridExportSnapshot` columns/rows — passed
  - conditional rule matching (`Amount > 15`) — passed
  - footer `Sum` aggregate and reserved `FooterRect` — passed
  - `ExportToCsvAsync` file output — passed
  - cancellation leaves no file — passed
- `GridQueryPipelineTests.cs`, `GridLayoutStateStoreTests.cs`, `GridExportTests.cs`, `GridConditionalFormattingAndFooterTests.cs`, and `GridFoundationTests.cs` were added to the test project.
- A temporary xunit project that links those five files ran **30 tests, all passed**:
  - query pipeline, grouping input, virtual query/cancellation/stale result/failure retention
  - layout round-trip, unknown/new-column apply, corrupt file
  - export snapshot/async/cancel/progress/no-controls, filter-to-export parity
  - conditional formatting precedence, footer aggregate/update/pinned geometry
  - explicit height survival, Card reset, navigation-style survival across theme apply, row-metric offsets/visibility, single-owner BindingList change flow, toolbar filter-panel behavior, case-insensitive property lookup, layout-store failure returns
- The repository test project still cannot build because of the unrelated `Beep.Winform.Data.Integrated.Controls` compile errors; the temporary project is the verification used here.
- Not verified: visual paint output for conditional formatting and footer, live message-pump behavior of the modeless export progress form, and the per-column source-sort fast path in 2.A.5.

