# Phase 3: Grid chrome revision — header, search, filters, navigation

## 0. Problem statement

The GridX chrome grew in layers. Headers, the toolbar search, the top filter
panel, the filter dialogs, and the bottom navigator each have their own visual
rules and several actions have more than one entry point. That makes the grid
feel inconsistent and creates real interaction conflicts.

Observed in the current code:

- **Header clicks can open a filter popup.** `BeepGridProFilterExtensions`
  attaches a `MouseClick` handler to the whole control and opens
  `BeepGridFilterPopup` for any header hit. It runs after the header input
  handler, so sorting, resizing, and reordering can be followed by a popup. Its
  x-coordinate walk also ignores display order, sticky columns, horizontal
  scroll, and the clipped header rectangles.
- **Two filter pipelines.** `BeepGridPro.Filtering` owns `ActiveFilter` and the
  chips row. `GridSortFilterHelper` owns the per-column contains/in state. They
  are adapted into one view now, but they still have separate entry points and
  separate lifecycle rules.
- **Three search experiences.** The unified toolbar search, the legacy top
  filter panel’s inline `BeepComboBox` + `BeepTextBox`, and the modal Search
  dialog. They do not share state, debounce policy, scope, or keyboard rules.
- **Header actions are incomplete.** The column menu offers sort, filter, and
  clear, but not autosize, pin/unpin, hide, group, or reset. The filter item
  opens the simple contains dialog rather than the value-list popup.
- **Header accessibility is thin.** Sort state, menu availability, and resize
  affordances are painted, but header cells are not keyboard-focusable and do
  not expose a menu command path from the keyboard.
- **Navigation has two implementations.** Painter-based navigation and the
  legacy `BeepButton` navigator coexist, and some utilities overlap the
  toolbar. There is no single status-bar contract for record count, filtered
  count, page size, or page numbers.
- **No shared chrome tokens.** Each painter chooses its own sizes and states.

## 1. Scope

In scope:

- Column header panel: caption, sort, menu/filter affordance, resize, reorder,
  sticky/frozen columns, density, hover/focus/active states, keyboard access.
- Search: one global search surface, scope, debounce, match count, highlighting,
  next/previous, clear, keyboard, accessibility.
- Filters: one filter model, type-aware operators, Excel-style value list,
  advanced builder, active chips, saved filters, server-side pushdown.
- Navigation: status/record counts, paging, page size, page numbers, CRUD
  grouping, compact/overflow behavior, keyboard, accessibility.
- Shared chrome tokens, metrics, icon usage, focus order, responsive rules,
  and visual verification.

Out of scope:

- Grid data engine, editing, grouping, export, and virtualization internals,
  except where a chrome action has to call them.
- New themes. Chrome uses existing `BeepThemesManager` tokens.
- New designer surfaces except the smart-tag verbs needed for the new actions.

## 2. UX standards to apply

Use the same rules the commercial grids and current web systems converge on.
Do not copy a single product; use the common contract.

### 2.1 Header

Reference behavior: AG Grid, Excel, DevExpress, Telerik, DataTables, Ant Design,
Material 3, Fluent 2.

- A header cell has three zones: **caption**, **sort state**, **menu/filter**.
  The menu zone is always reserved; the sort zone is reserved for sortable
  columns.
- Clicking the caption sorts. Default cycle: ascending → descending → none.
  Shift-click appends to a multi-sort; a numbered badge shows sort order.
- The menu button opens a command menu, not a filter dialog directly. Menu:
  Sort ascending, Sort descending, Clear sort, Filter, Clear filter,
  Auto-size column, Pin/Unpin, Group by this column, Hide column, Reset columns.
- The filter icon shows a dot/count when that column has an active condition.
- Hover reveals secondary affordances; active sort remains visible at rest.
- Resize handle: 6 px hit target, `SizeWE` cursor, live resize, double-click to
  best-fit, Ctrl+double-click to best-fit all visible columns.
- Drag-reorder shows a ghost and an insertion marker; Esc cancels.
- Sticky columns have a clear divider and a subtle shadow over scrolling content.
- Caption truncates with ellipsis and exposes the full text as a tooltip.
- Keyboard: header cells are focusable; Enter/Space sorts; Alt+Down or the
  Menu key opens the column menu; left/right move focus; Esc closes menu.
- Minimum contrast 4.5:1 for caption and icons; focus ring visible in every
  theme; touch density uses at least a 44 px target.

### 2.2 Search

Reference behavior: AG Grid quick filter, DevExpress search panel, Ant Design
Table filter dropdown, Material search, command palettes.

- One global search surface in the toolbar. The legacy top-panel search and the
  modal Search dialog are either removed or route to the same component.
- Scope control inside or beside the box: all columns, or one column.
- Debounced live search (200–300 ms) plus immediate Enter. Esc clears the text
  or cancels the in-flight query. A visible clear button is always available
  while the box has text.
- Search is case-insensitive by default. Diacritic-insensitive matching is
  configurable. Quick search is literal, not regex.
- Show match count (`12 matches`) and next/previous navigation. Highlight the
  matched substring in visible cells without changing the cell value.
- No-results state explains the active search and offers Clear.
- Search state is part of the query filter model, not a separate row-visibility
  flag.
- Keyboard: Ctrl+F focuses; `/` and F3 may focus when not editing; Enter
  commits/next; Shift+Enter previous; Esc clears/cancels.
- Async and virtualized sources search through `IGridQuerySource`, not the
  materialized window.
- Screen readers announce the scope, result count, and no-results state.

### 2.3 Filters

Reference behavior: Excel AutoFilter, AG Grid filter menu, DevExpress filter
builder, Telerik filter menu, Ant Design filter dropdown, Material chips.

- One filter model. Search, column filters, advanced criteria, and chips all
  produce the same query criteria.
- Operator list is type-aware:
  - text: contains, does not contain, equals, not equals, starts with, ends with,
    is empty, is not empty
  - numeric/date: equals, not equals, greater/less, between, is empty
  - boolean: is true, is false, is empty
  - advanced: in, not in, regex
- Excel-style value list: search inside values, select all, include blanks,
  show counts, apply/cancel, keyboard navigation, large-list virtualization.
- Advanced builder: AND/OR groups, add/remove/reorder conditions, per-condition
  validation, preview match count, apply/cancel/reset.
- Active filter chips: one chip per condition, editable, removable, with
  “Clear all”; overflow becomes `+N more`; chips are keyboard reachable.
- Empty/null semantics are explicit and consistent between text, numeric, and
  date columns.
- Filtering is debounced and applied once per change; server-side sources get a
  single `GridQuery`.
- Saved filters are versioned and validated on load.
- Search and filters share the chips row and the clear-all command.

### 2.4 Navigation

Reference behavior: AG Grid pagination, DataTables pager, Telerik pager,
Material data table footer, Fluent status bar.

- One bottom chrome bar: record status, paging, CRUD, and optional utilities.
- Status text: `1–25 of 1,240` plus `(filtered from 4,800)` when filters are
  active; selected count appears when multi-select is used.
- Pagination: first, previous, page numbers, next, last, page-size selector, and
  a go-to-page input. Disabled states are explicit.
- CRUD buttons are grouped and separated from paging; destructive actions use
  the theme’s error semantic color.
- Compact mode drops labels, then utilities, then page numbers, in that order.
  Nothing is clipped mid-button.
- Keyboard: PageUp/PageDown/Home/End work in the grid; the pager is tabbable;
  Enter/Space activate; focus ring is visible.
- Every control has an accessible name and current-state description.
- The painter path and legacy path share one layout contract; only one is
  active at runtime.
- Touch density raises targets to 44 px; desktop standard stays 32–36 px.

### 2.5 Cross-cutting visual rules

- 4/8 px spacing grid; no arbitrary pixel values inside painters.
- States: normal, hover, focus, pressed, active/selected, disabled, error.
- Colors resolve from `BeepThemesManager`; literal ARGB is allowed only for
  semantic fallbacks already used by the theme layer.
- Icons render through `BeepImage`/`StyledImagePainter` with tinting, never
  hand-painted glyph paths.
- Focus ring is a real, visible token; never rely on the Windows dotted rectangle.
- DPI scaling comes from `DpiScalingHelper`; no fixed 96-DPI assumptions.
- Motion is limited to hover/press and menu open/close; respect reduced motion.
- Every interactive region is registered as a hit area and has a tooltip.

## 3. Architecture decisions

1. **One chrome interaction contract.** Add a shared `GridChromeMetrics` /
   `GridChromeState` layer for density, spacing, state colors, and breakpoints.
   Painters consume it; they do not invent their own constants.
2. **One query state.** Search and all filters produce `GridQuery.Filters`.
   `ActiveFilter`, `SortFilter`, and the chips row are views over that state.
3. **One header controller.** Header hit testing, keyboard focus, sort, menu,
   resize, and reorder live behind a single controller; `MouseClick` handlers
   must not open popups independently.
4. **One search component.** Toolbar search, column scope, and chips share one
   component. Remove the duplicate top-panel and modal search paths or make them
   adapters to the same component.
5. **One filter surface family.** The column menu opens one menu; the menu opens
   either the value list or the advanced builder. Do not duplicate the simple
   `Filter by Column` dialog as a third path.
6. **One navigation contract.** `INavigationPainter` and the legacy navigator
   share `NavigationLayout`; only one renderer is active.
7. **No silent behavior changes.** Every existing public command
   (`ApplyQuickFilter`, `ShowFilterDialog`, `ShowAdvancedFilterDialog`,
   `GroupBy`, `SortFilter`) remains callable and delegates to the new path.

## 4. Stage map

| Stage | Document | Focus |
|---|---|---|
| 1 | [P3_STAGE_1_INTERACTION_SYSTEM.md](P3_STAGE_1_INTERACTION_SYSTEM.md) | Shared chrome tokens, states, keyboard, accessibility, responsive contract |
| 2 | [P3_STAGE_2_HEADER.md](P3_STAGE_2_HEADER.md) | Header layout, sort, menu, filter affordance, resize, reorder, sticky |
| 3 | [P3_STAGE_3_SEARCH.md](P3_STAGE_3_SEARCH.md) | Unified search, scope, debounce, highlight, count, keyboard |
| 4 | [P3_STAGE_4_FILTERS.md](P3_STAGE_4_FILTERS.md) | Filter model, operators, value list, advanced builder, chips |
| 5 | [P3_STAGE_5_NAVIGATION.md](P3_STAGE_5_NAVIGATION.md) | Status bar, paging, CRUD grouping, compact/overflow |
| 6 | [P3_STAGE_6_VALIDATION.md](P3_STAGE_6_VALIDATION.md) | Visual, keyboard, accessibility, DPI, performance, regression |

Implementation order is 1 → 2 → 3 → 4 → 5 → 6. Stage 1 is a prerequisite for
the others. Stage 3 and 4 share the query model; implement 3 first, then 4.

## 5. Common acceptance criteria

- One entry point per action; no `MouseClick` handler opens a header popup.
- Header, search, filters, and navigation use the same density and state tokens.
- All interactive chrome is reachable and operable by keyboard.
- Screen readers can name the header cell, sort state, filter state, search
  scope, result count, and pager controls.
- No action is triggered twice by overlapping handlers.
- No full-grid rebuild for a search keystroke, a filter chip removal, a sort
  toggle, or a page-size change.
- Existing public commands keep working; no host code changes are required for
  the default behavior.
- Visual QA passes in light, dark, and high-contrast themes at 100 %, 125 %,
  and 150 % DPI.

## 6. Verification

Per stage:

- focused unit tests for geometry, state, and command routing
- a deterministic visual fixture rendered off-screen and compared for the
  affected regions (header band, search box, chips row, nav bar)
- keyboard-only walkthrough
- screen-reader name/state inspection

Per phase:

- linked xunit suite for GridX
- `dotnet build TheTechIdea.Beep.Winform.Controls`
- manual matrix: light/dark/high-contrast × 100/125/150 % DPI
- regression pass over sort, filter, search, paging, selection, and editing

## 7. Risks

| Risk | Mitigation |
|---|---|
| Removing duplicate entry points breaks host expectations | Keep public commands, route them to the new component, add regression tests |
| Header keyboard support conflicts with grid navigation | Own focus in one header controller; let `Tab` enter and `Esc` leave the header band |
| Filter/search unify changes result semantics | Build parity tests against current `ActiveFilter` and `SortFilter` cases before deleting state |
| Painter proliferation | Use the shared metrics/tokens; per-style painters only choose presentation, not geometry rules |
| DPI and high contrast regressions | Render fixtures at 100/125/150 % and assert contrast/state visibility |
| Navigation duplication | Make `INavigationPainter` the single contract; delete the legacy renderer after parity |

## 8. Out of scope for Phase 3

- Reworking the data engine, grouping engine, editor framework, or exporters.
- New layout presets.
- Changing the existing public `BeepGridStyle` visuals beyond chrome metrics.
- Implementing the designer footer verb (tracked separately in Phase 2).

## 9. Related documents

- [P3_STAGE_1_INTERACTION_SYSTEM.md](P3_STAGE_1_INTERACTION_SYSTEM.md)
- [P3_STAGE_2_HEADER.md](P3_STAGE_2_HEADER.md)
- [P3_STAGE_3_SEARCH.md](P3_STAGE_3_SEARCH.md)
- [P3_STAGE_4_FILTERS.md](P3_STAGE_4_FILTERS.md)
- [P3_STAGE_5_NAVIGATION.md](P3_STAGE_5_NAVIGATION.md)
- [P3_STAGE_6_VALIDATION.md](P3_STAGE_6_VALIDATION.md)
- [GridX README](../../TheTechIdea.Beep.Winform.Controls/GridX/README.md)
- [GridX implementation guide](../../TheTechIdea.Beep.Winform.Controls/GridX/Claude.md)
