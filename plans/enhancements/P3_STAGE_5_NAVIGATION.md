# Phase 3 — Stage 5: Navigation and status bar

> Parent plan: [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md) ·
> Prerequisite: [P3_STAGE_1_INTERACTION_SYSTEM.md](P3_STAGE_1_INTERACTION_SYSTEM.md)

## 1. Goal

Turn the bottom chrome into one coherent status/navigation bar: clear record
counts, paging, CRUD, page-size control, and predictable compact behavior. Keep
one runtime renderer and one hit-area contract.

## 2. Current behavior and files

- `Helpers/GridNavigationPainterHelper.cs`: painter navigation plus a legacy
  `BeepButton`/`BeepLabel` path, page-info update, compact/full layouts, and
  utility buttons (insert/delete/save/cancel/first/prev/next/last/query/filter/
  print).
- `Painters/INavigationPainter.cs` and `Painters/NavigationLayout.cs`: painter
  contract and rectangles for navigation, CRUD, status, page size, page numbers,
  goto, utilities, settings, help, and more.
- 12 navigation painters (`Material`, `Fluent`, `Bootstrap`, `DataTables`,
  `AGGrid`, `Telerik`, `AntDesign`, `Tailwind`, `Compact`, `Minimal`, `Card`,
  `Standard`).
- `BeepGridPro.Properties.cs`: `ShowNavigator`, `NavigationStyle`,
  `UsePainterNavigation`, and navigator height sync.
- `Layouts/*NavigationPainter` implementations choose their own spacing and
  button content.

## 3. Target behavior

### 3.1 One bar, three zones

```
┌──────────────────────────────────────────────────────────────────────────┐
│ [New][Edit][Delete] │ 1–25 of 1,240 (filtered from 4,800) │ ◀ 1 2 3 … ▶ │ 25 ▾ │
│ CRUD                │ status                              │ paging        │ size │
└──────────────────────────────────────────────────────────────────────────┘
```

- CRUD zone: New, Edit, Delete, Save, Cancel — grouped, with clear disabled
  states. Destructive actions use the error semantic color.
- Status zone: visible range, total, filtered total, selected count. In compact
  mode it collapses to `25 / 1,240`.
- Paging zone: first, previous, numbered pages, next, last, page-size selector,
  go-to-page input. In compact mode it collapses to first/prev/next/last plus
  status.
- Optional utilities: columns, export, print, settings. These should not
  duplicate the toolbar; if both exist, the toolbar wins and the nav utility is
  hidden by default.

### 3.2 Paging states

- No rows: status “No records”; paging disabled.
- One page: page numbers hide; status still shows the range.
- Filtered: status includes `(filtered from N)`.
- Page size choices: 10, 25, 50, 100, All (configurable; `All` is optional).
- Changing page size resets to page 1; changing filter/search resets to page 1.
- Empty page after delete: move to the previous valid page.
- Page numbers show a window around the current page with ellipses.

### 3.3 Compact/responsive order

Drop in this order as width decreases:

1. page numbers → `Page 3 of 12`
2. CRUD labels → icons
3. status range → `25 / 1,240`
4. optional utilities
5. gap/spacing reductions

Never clip a button or let paging collide with status.

### 3.4 Keyboard and accessibility

- Tab order: CRUD → pager → page size → status (status not focusable).
- Enter/Space activate buttons; Home/End jump to first/last page.
- Page-size combo is keyboard operable; go-to-page accepts Enter.
- Names: “First page”, “Previous page”, “Next page”, “Last page”, “Page size”,
  “Go to page”, “New record”, “Delete record”.
- Announce “Page 2 of 12, rows 26–50 of 1,240”.
- Focus ring visible on every control; touch density uses 44 px targets.

### 3.5 Rendering contract

- `INavigationPainter` remains the single painter contract.
- `NavigationLayout` carries all rectangles; painters only choose presentation.
- Delete the legacy `BeepButton`/`BeepLabel` renderer after parity; if retained
  for compatibility, it must consume the same `NavigationLayout`.
- Only one renderer is active: `UsePainterNavigation` becomes a migration flag,
  then is removed.
- Draw only changed regions during hover/press; do not repaint the grid.

### 3.6 Virtualization and async data

- Status uses `VirtualRowCount`/`IGridQuerySource.TotalCount`, not the
  materialized window, when virtualization is active.
- Paging in virtual mode requests the page/window through the query source.
- In async mode, pager buttons disable while a page request is in flight and
  re-enable on completion/error.

### 3.7 Host API

Keep `ShowNavigator`, `MoveFirst/Previous/Next/Last`, and existing CRUD methods.
Add if needed:

```csharp
grid.ShowPageSizeSelector = true;
grid.PageSizeOptions = new[] { 10, 25, 50, 100 };
grid.ShowRecordStatus = true;
grid.ShowPagerUtilities = false;
```

## 4. Tasks

| # | Task | Status |
|---|------|--------|
| 5.1 | Define the three-zone `NavigationLayout` and shared metrics usage | ⬜ |
| 5.2 | Add status text contract (range, total, filtered, selected) | ✅ |
| 5.3 | Add page-size selector and numbered paging with ellipses | ✅ |
| 5.4 | Add go-to-page input and keyboard validation | ✅ |
| 5.5 | Group CRUD separately; add disabled/semantic states | ⬜ |
| 5.6 | Implement compact/responsive drop order | ⬜ |
| 5.7 | Add tab order, focus ring, and accessible names | ⬜ |
| 5.8 | Add page/filter/page-size reset rules | ✅ |
| 5.9 | Route virtual/async paging through `IGridQuerySource` | ⬜ |
| 5.10 | Unify painter and legacy renderers on `NavigationLayout`; delete the loser | ⬜ |
| 5.11 | Remove duplicated nav utilities that the toolbar owns | ⬜ |
| 5.12 | Add hit-area, paging, status, compact, and keyboard tests | ⬜ |
| 5.13 | Visual QA all 12 navigation painters at 3 DPIs/themes | ⬜ |
| 5.14 | Update GridX docs for the nav contract | ⬜ |

## 5. Acceptance criteria

- One renderer is active; painter and legacy geometry cannot disagree.
- Status shows visible range, total, filtered total, and selection when relevant.
- Page size and page numbers work with correct reset behavior.
- Compact mode never clips or overlaps controls.
- Virtual/async paging uses total counts and requests pages correctly.
- Pager is keyboard operable and screen-reader named.

## 6. Verification

- Unit tests for status text and page reset rules.
- Hit-area tests for every pager/CRUD rectangle.
- Virtual source paging test.
- Keyboard walkthrough.
- Bitmap fixtures for each navigation painter.

## 7. Risks

- Removing nav utilities may break hosts that hid the toolbar. Keep them
  configurable and default-off only when the toolbar is visible.
- Page-size changes can be expensive on large grids. Treat as a query change,
  not a local row rebuild.
