# Beep.Winform Enhancement — Master Todo Tracker

Enhancements across four phases.

> **Specs:** [specs/P1_BASECONTROL_GRID_FOUNDATION_SPEC.md](specs/P1_BASECONTROL_GRID_FOUNDATION_SPEC.md)
> **Design doc:** [P1_BASECONTROL_GRID_FOUNDATION_DESIGN.md](P1_BASECONTROL_GRID_FOUNDATION_DESIGN.md)
> **Task list:** [P1_BASECONTROL_GRID_FOUNDATION_TASKS.md](P1_BASECONTROL_GRID_FOUNDATION_TASKS.md)
>
> **Phase 2 design doc:** [P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md](P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md)
> **Phase 2 task list:** [P2_GRIDPRO_QUERY_AND_EXPERIENCE_TASKS.md](P2_GRIDPRO_QUERY_AND_EXPERIENCE_TASKS.md)
>
> **Phase 3 design doc:** [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md)
> **Phase 3 stages:** [Stage 1](P3_STAGE_1_INTERACTION_SYSTEM.md) ·
> [Stage 2](P3_STAGE_2_HEADER.md) · [Stage 3](P3_STAGE_3_SEARCH.md) ·
> [Stage 4](P3_STAGE_4_FILTERS.md) · [Stage 5](P3_STAGE_5_NAVIGATION.md) ·
> [Stage 6](P3_STAGE_6_VALIDATION.md)
>
> **Phase 4 binding:** [P4_BINDING_REVISION.md](P4_BINDING_REVISION.md)

## Phase 4: Data binding revision ✅

- Added `GridX/Binding/GridDataBinding.cs` to normalize `BindingSource`,
  `DataSet`/`DataTable`/`DataView`, `IListSource`, `IDataReader`, `IList`,
  `IBindingList`, `INotifyCollectionChanged`, `IEnumerable`/`IQueryable`,
  `DataRow`/`DataRowView`, dictionaries, POCOs, `ITypedList`, and single objects.
- Added `SetCellValue`/`GetCellValue`; editor commits now write through the
  binding adapter to the original item.
- Added `GridBindingTests` for schema, live collection updates,
  `INotifyPropertyChanged`, dictionaries, `ExpandoObject`, and write-back.
- Verification: Controls project builds; GridX suite **95 passed, 0 failed**.
- Limitation: `IAsyncEnumerable<T>` must be materialized before assignment.

## Phase 1: BaseControl and BeepGridPro foundation repair ⬜

> 📄 **[Design Doc: P1_BASECONTROL_GRID_FOUNDATION_DESIGN.md](P1_BASECONTROL_GRID_FOUNDATION_DESIGN.md)** ·
> **[Task List: P1_BASECONTROL_GRID_FOUNDATION_TASKS.md](P1_BASECONTROL_GRID_FOUNDATION_TASKS.md)**

### Phase 1.A — BaseControl contract hygiene

| # | Task | Status |
|---|------|--------|
| 1.A.1 | Document the explicit BaseControl override contract for paint/layout behavior | ⬜ |
| 1.A.2 | Add or tighten the guard path that prevents base painting/layout drift when derived controls own rendering | ⬜ |
| 1.A.3 | Add a focused regression check around `AllowBaseControlClear` / `IsContainerControl` / `UseBaseMouseInputRouting` behavior | ⬜ |

### Phase 1.B — BeepGridPro rebinding safety

| # | Task | Status |
|---|------|--------|
| 1.B.1 | Audit the `DataSource` / `Uow` transitions and enforce one consistent rebind lifecycle | ⬜ |
| 1.B.2 | Reset stale grid rows, columns, and selection when `ClearGrid` or source replacement occurs | ⬜ |
| 1.B.3 | Verify `RefreshData` and in-/out-of-virtualization paths preserve the correct state after rebind | ⬜ |

### Phase 1.C — Invalidation + theme consistency

| # | Task | Status |
|---|------|--------|
| 1.C.1 | Consolidate the invalidation flow so theme and state updates happen in a single deterministic sequence | ⬜ |
| 1.C.2 | Validate borders, backgrounds, and layout rectangles are not left stale after data refresh or theme changes | ⬜ |

### Phase 1.M — Full focused validation

| # | Task | Status |
|---|------|--------|
| 1.M.1 | Run focused BaseControl + BeepGridPro tests | ⬜ |
| 1.M.2 | Confirm intentional exclusions remain unchanged (base 001 rendering, grid painting disabled) | ⬜ |
| 1.M.3 | Review the final backlog against the design doc and acceptance criteria | ⬜ |

## Phase 2: BeepGridPro query, layout, and experience enhancements ⬜

> Implementation landed for 2.A.1–2.A.10, 2.B.1–2.B.7, 2.C.1–2.C.7,
> 2.D.1–2.D.8, 2.D.10, 2.E.1–2.E.9, and 2.M.
> Remaining: 2.D.9 (designer footer verb) is blocked by the Design.Server
> dependency on the broken Integrated.Controls project.
> See the task list's "Verification notes" for the build and test evidence.

> 📄 **[Design Doc: P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md](P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md)** ·
> **[Task List: P2_GRIDPRO_QUERY_AND_EXPERIENCE_TASKS.md](P2_GRIDPRO_QUERY_AND_EXPERIENCE_TASKS.md)**

### Phase 2.A — Unified query pipeline

| # | Task | Status |
|---|------|--------|
| 2.A.1 | Add `GridQuery`, descriptors, `GridQueryResult`, `GridViewRow` under `GridX/Query/` | ✅ |
| 2.A.2 | Implement UI-free `GridQueryEngine` (filter → group → sort → page) | ✅ |
| 2.A.3 | Add `GridView` owned by `GridDataController` as the renderer/exporter read surface | ✅ |
| 2.A.4 | Migrate `BeepGridPro.Filtering` (`ActiveFilter`, quick filter, advanced dialog) to query criteria | ✅ |
| 2.A.5 | Migrate `GridSortFilterHelper` contains/in filters and source-sort fast path into the pipeline | ✅ |
| 2.A.6 | Move grouping input to `GridQuery.Groups`; keep `GridGroupEngine` as materializer | ✅ |
| 2.A.7 | Delete `_filteredRowIndices`; derive `FilteredRowIndices` from the view | ✅ |
| 2.A.8 | Route `GridExportEngine` through the view model | ✅ |
| 2.A.9 | Route `GridRowVirtualizer` window materialization through the pipeline | ✅ |
| 2.A.10 | Add filter/sort/group/export parity tests | ✅ |

### Phase 2.B — Layout persistence per user

| # | Task | Status |
|---|------|--------|
| 2.B.1 | Add versioned `GridLayoutState` + `GridColumnLayout` DTOs keyed by column name | ✅ |
| 2.B.2 | Add `IGridLayoutStateStore` and atomic JSON `FileGridLayoutStateStore` | ✅ |
| 2.B.3 | Add `SaveLayoutState` / `LoadLayoutState` / `ResetLayoutState` | ✅ |
| 2.B.4 | Apply state by name with clamping and system-column exclusion | ✅ |
| 2.B.5 | Re-apply saved state after `AutoGenerateColumns()` | ✅ |
| 2.B.6 | Mark state dirty on order/width/sticky/visibility changes | ✅ |
| 2.B.7 | Add round-trip, removed/new-column, and corrupt-file tests | ✅ |

### Phase 2.C — Server-side paging, sort, and filter contract

| # | Task | Status |
|---|------|--------|
| 2.C.1 | Add `IGridQuerySource` as an additive capability interface | ✅ |
| 2.C.2 | Make `GridQuery` serialization-friendly for transport adapters | ✅ |
| 2.C.3 | Prefer `IGridQuerySource` in `GridRowVirtualizer`, fall back to `IVirtualDataSource` | ✅ |
| 2.C.4 | Add query revision so stale responses are discarded | ✅ |
| 2.C.5 | Wire end-to-end cancellation (new query cancels previous token) | ✅ |
| 2.C.6 | Implement `IGridQuerySource` in `GridVirtualDataSource` for in-memory parity | ✅ |
| 2.C.7 | Add paging, sort, filter, stale-response, cancellation, and error tests | ✅ |

### Phase 2.D — Conditional formatting and pinned aggregate footer

| # | Task | Status |
|---|------|--------|
| 2.D.1 | Add `ConditionalFormatRule` (target, operator, value, style, priority, `StopIfTrue`) | ✅ |
| 2.D.2 | Add `GridConditionalFormatEngine` caching styles per view revision | ✅ |
| 2.D.3 | Define and document precedence: focus/selection > rule > style/theme | ✅ |
| 2.D.4 | Integrate the style map into cell/row background and foreground resolution | ✅ |
| 2.D.5 | Add `GridFooterConfig` (visible, height, per-column `AggregationType`) | ✅ |
| 2.D.6 | Add `FooterRect` to layout, viewport height, scrollbar range, and hit-testing | ✅ |
| 2.D.7 | Render pinned footer (sticky + scrolling columns) in its own pass | ✅ |
| 2.D.8 | Compute footer aggregates over the filtered view via the shared aggregate engine | ✅ |
| 2.D.9 | Add designer smart-tag verb to edit footer aggregates | ⬜ |
| 2.D.10 | Add precedence, footer scope, scrolling, and update-on-filter tests | ✅ |

### Phase 2.E — Async export with progress and cancellation

| # | Task | Status |
|---|------|--------|
| 2.E.1 | Add `IAsyncGridExporter` with task, progress, and cancellation | ✅ |
| 2.E.2 | Add immutable `GridExportSnapshot` created on the UI thread | ✅ |
| 2.E.3 | Add `GridExportEngine.ExportAsync` (snapshot first, then sync/async exporter) | ✅ |
| 2.E.4 | Wire progress and cancellation; delete partial output on cancel/failure | ✅ |
| 2.E.5 | Implement async export in CSV, JSON, and HTML exporters | ✅ |
| 2.E.6 | Keep Excel/PDF stubs unavailable; allow plugins to opt in | ✅ |
| 2.E.7 | Update the export menu to use the async path with progress and cancel | ✅ |
| 2.E.8 | Report failures through `ReportOperationError`; no worker-thread dialogs | ✅ |
| 2.E.9 | Add responsiveness, progress, cancel cleanup, snapshot, and thread-safety tests | ⬜ |

### Phase 2.M — Full focused validation

| # | Task | Status |
|---|------|--------|
| 2.M.1 | Run the query/export parity suite | ✅ |
| 2.M.2 | Run the layout round-trip suite | ✅ |
| 2.M.3 | Run the query-source paging/sort/filter/cancel suite | ✅ |
| 2.M.4 | Run conditional-format precedence and footer aggregate/scroll suites | ✅ |
| 2.M.5 | Run async export progress/cancel/UI-thread suite | ✅ |
| 2.M.6 | Review the final backlog against the design doc and record what was not verified | ✅ |

## Phase 3: Grid chrome revision — header, search, filters, navigation 🟡

> 📄 **[Design Doc: P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md)**
>
> Every stage has its own document and task list.
> Status: ⬜ not started · 🟡 in progress · ✅ complete.

| Stage | Focus | Tasks | Document | Status |
|---|---|---|---|---|
| 3.1 | Interaction system, tokens, keyboard, accessibility, responsive contract | 10 | [P3_STAGE_1_INTERACTION_SYSTEM.md](P3_STAGE_1_INTERACTION_SYSTEM.md) | ✅ |
| 3.2 | Header panel: sort, menu, filter affordance, resize, reorder, sticky | 15 | [P3_STAGE_2_HEADER.md](P3_STAGE_2_HEADER.md) | ✅ |
| 3.3 | Unified search: scope, debounce, count, highlight, keyboard | 14 | [P3_STAGE_3_SEARCH.md](P3_STAGE_3_SEARCH.md) | ✅ |
| 3.4 | Filters: one model, operators, value list, builder, chips | 16 | [P3_STAGE_4_FILTERS.md](P3_STAGE_4_FILTERS.md) | ✅ |
| 3.5 | Navigation: status, paging, CRUD grouping, compact/overflow | 14 | [P3_STAGE_5_NAVIGATION.md](P3_STAGE_5_NAVIGATION.md) | ✅ |
| 3.6 | Integration, visual/keyboard/a11y validation, performance, rollout | 8 | [P3_STAGE_6_VALIDATION.md](P3_STAGE_6_VALIDATION.md) · [report](P3_STAGE_6_VALIDATION_REPORT.md) | 🟡 |

Implemented: shared `GridChromeMetrics`/`GridChromeState`, `ChromeDensity`,
header keyboard focus and column menu, debounced scope-aware search with
highlight/count/navigation, filter chips with overflow, combined search+column
filter semantics, compact pagination with painted page-size control, high
contrast across header/navigation painters, and accessible row/cell state.

Remaining: live 150%/200% DPI inspection and a manual screen-reader pass.

### Phase 3 completion gate

- One entry point per chrome action; no header `MouseClick` popup path.
- Header/search/filter/nav share metrics, states, focus, and responsive rules.
- Search and filters write one `GridQuery`; chips show every condition.
- Header and pager are keyboard operable and screen-reader named.
- All 12 header/nav painter families pass the visual fixture matrix.
- No full row rebuild for chrome-only actions.
- Public commands remain callable; removed duplicates have documented aliases.

## Summary

| Phase | Name | Tasks | Priority | Spec |
|-------|------|-------|----------|------|
| 1 | BaseControl and BeepGridPro foundation repair | 9 | P0 | [specs/P1_BASECONTROL_GRID_FOUNDATION_SPEC.md](specs/P1_BASECONTROL_GRID_FOUNDATION_SPEC.md) |
| 2 | BeepGridPro query, layout, and experience enhancements | 49 | P1 | [P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md](P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md) |
| 3 | Grid chrome revision: header, search, filters, navigation | 77 | P1 | [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md) |

### Per-Phase Design Documents

| Phase | Design Doc | Task List |
|-------|-----------|-----------|
| 1 | [P1_BASECONTROL_GRID_FOUNDATION_DESIGN.md](P1_BASECONTROL_GRID_FOUNDATION_DESIGN.md) | [P1_BASECONTROL_GRID_FOUNDATION_TASKS.md](P1_BASECONTROL_GRID_FOUNDATION_TASKS.md) |
| 2 | [P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md](P2_GRIDPRO_QUERY_AND_EXPERIENCE_DESIGN.md) | [P2_GRIDPRO_QUERY_AND_EXPERIENCE_TASKS.md](P2_GRIDPRO_QUERY_AND_EXPERIENCE_TASKS.md) |
| 3 | [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md) | Six stage documents below |

### Phase 3 Stage Documents

| Stage | Document |
|---|---|
| 3.1 Interaction system | [P3_STAGE_1_INTERACTION_SYSTEM.md](P3_STAGE_1_INTERACTION_SYSTEM.md) |
| 3.2 Header | [P3_STAGE_2_HEADER.md](P3_STAGE_2_HEADER.md) |
| 3.3 Search | [P3_STAGE_3_SEARCH.md](P3_STAGE_3_SEARCH.md) |
| 3.4 Filters | [P3_STAGE_4_FILTERS.md](P3_STAGE_4_FILTERS.md) |
| 3.5 Navigation | [P3_STAGE_5_NAVIGATION.md](P3_STAGE_5_NAVIGATION.md) |
| 3.6 | Integration, visual/keyboard/a11y validation, performance, rollout | 8 | [P3_STAGE_6_VALIDATION.md](P3_STAGE_6_VALIDATION.md) · [report](P3_STAGE_6_VALIDATION_REPORT.md) | 🟡 |
