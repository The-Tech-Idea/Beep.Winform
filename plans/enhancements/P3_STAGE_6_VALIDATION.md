# Phase 3 — Stage 6: Integration, validation, and rollout

> Parent plan: [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md) ·
> Prerequisites: Stages 1–5

## 1. Goal

Prove that the revised chrome works as one system: visual consistency, keyboard
and screen-reader access, correct DPI/theme behavior, no duplicate actions, and
no regression in sort/filter/search/paging.

## 2. Workstreams

### 2.1 End-to-end scenarios

Run each scenario in light, dark, and high-contrast themes:

1. Load 10,000 rows; focus header; sort ascending/descending/none.
2. Shift-click a second header; verify multi-sort badge and order.
3. Open column menu with mouse and keyboard; apply a column filter.
4. Search globally; verify debounce, match count, next/previous, clear.
5. Add a second filter; verify chips, AND behavior, clear one, clear all.
6. Open advanced builder; add AND/OR group; preview count; apply/cancel.
7. Reorder and resize a column; verify layout state marks dirty.
8. Pin/unpin a column; verify sticky divider and scroll behavior.
9. Hide a column from the menu; verify header/body/export alignment.
10. Page through results; change page size; go to page.
11. Change search/filter; verify page resets to 1.
12. Tab through header, search, chips, pager; verify visible focus and Enter/Space.
13. Use a screen reader to inspect header state, search count, chip text, pager.
14. Toggle virtualization; search/filter/page through the query source.

### 2.2 Focused automated tests

- `GridChromeMetrics` breakpoints/density.
- Header geometry and hit-zone separation.
- Sort cycle, multi-sort, menu actions.
- Search debounce/cancel/stale result/count/highlight.
- Filter operator matrix, null/empty matrix, chips, clear-all.
- Navigation status/page reset/page-size rules.
- Duplicate-action regression: header click must not open a popup.

### 2.3 Visual fixtures

Render off-screen at 100 %, 125 %, and 150 % DPI:

- header band for each header painter
- toolbar/search box for each state (empty, typed, focused, no results)
- filter menu, value list, advanced builder, chips row
- navigation bar for each navigation painter, compact and full
- high-contrast theme

Compare the affected regions, not the whole window. A fixture should fail when a
geometry or state token changes unexpectedly.

### 2.4 Accessibility audit

- Keyboard-only pass for every scenario above.
- Screen-reader names/states for header, search, chips, pager.
- Contrast check for text, icons, focus ring, placeholder, disabled states.
- Touch-density target check (44 px).
- Reduced-motion check for menu/chip animations.

### 2.5 Performance budget

Record before/after for:

- typing 10 characters in search (should do one query per debounce, not ten)
- applying/removing a filter chip
- sorting a header
- changing page size
- opening/closing the column menu
- painting the header band and footer after a repaint

Target: no full row rebuild for chrome-only changes; no O(n) scan in a
mouse-move or paint path; no duplicate query evaluation.

### 2.6 Documentation and migration

- Update `GridX/README.md` for the new search/filter/header/nav APIs.
- Update `GridX/Claude.md` invariants: one filter model, one search surface,
  one header controller, one nav renderer.
- Record removed duplicate entry points and compatibility aliases.
- Add a short host migration note if a public behavior changed (for example,
  `ShowToolbar = false` no longer auto-enables the legacy filter panel).

## 3. Tasks

| # | Task | Status |
|---|------|--------|
| 6.1 | Run the 14 end-to-end scenarios in all three themes | ⬜ |
| 6.2 | Run keyboard-only and screen-reader audits | ⬜ |
| 6.3 | Run the visual fixture matrix at 100/125/150 % DPI | ⬜ |
| 6.4 | Run contrast and touch-target checks | ⬜ |
| 6.5 | Run the performance budget comparisons | ⬜ |
| 6.6 | Run the focused xunit suite and duplicate-action regression tests | ⬜ |
| 6.7 | Update README, Claude.md, and migration notes | ⬜ |
| 6.8 | Review remaining gaps and record what was not verified | ⬜ |

## 4. Completion gate

Phase 3 is complete only when:

- every stage's acceptance criteria is met,
- the end-to-end scenarios pass in all themes,
- keyboard and screen-reader audits pass,
- visual fixtures are stable,
- the focused test suite passes,
- and the documentation reflects the actual API and removed paths.

## 5. Exit report

Record:

- tested revision and dirty-tree state
- exact commands and results
- bitmap fixture paths
- accessibility findings
- performance before/after table
- remaining limitations and follow-up items

## 6. Risks

| Risk | Mitigation |
|---|---|
| Visual drift across 12 header/nav painters | Shared metrics + per-painter fixtures |
| Host code relying on removed duplicate entry points | Keep public command aliases; document behavior changes |
| Large-grid search/filter cost | Debounce, one query per change, server pushdown |
| High-contrast focus invisible | Tokenized focus ring and contrast assertions |
| Test project blocked by unrelated Integrated dependency | Use a linked GridX test project until the dependency is repaired |
