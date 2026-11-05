# Phase 3 — Stage 2: Header panel

> Parent plan: [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md) ·
> Prerequisite: [P3_STAGE_1_INTERACTION_SYSTEM.md](P3_STAGE_1_INTERACTION_SYSTEM.md)

## 1. Goal

Make the column header predictable, keyboard accessible, and visually aligned
with modern commercial grids: one clear sort target, one menu target, visible
filter state, reliable resize/reorder, and complete column commands.

## 2. Current behavior and files

- `Painters/IPaintGridHeader.cs` exposes `HeaderCellLayout` with text, sort,
  menu, and sort-hit rectangles, plus `PaintHeaders`, `PaintHeaderCell`,
  `PaintSortIndicator`, `PaintFilterIcon`, and `RegisterHeaderHitAreas`.
- `Painters/BaseHeaderPainter.cs` and 12 style painters own geometry and
  painting. Each style has its own padding and hover rules.
- `Helpers/GridInputHelper.cs` handles header hover, filter-icon clicks, sort
  clicks (including Shift multi-sort), column menu, reorder start, resize, and
  header-row resize.
- `BeepGridPro.ContextMenu.cs` builds the column menu: sort asc/desc, clear
  sort, Filter, Clear Filter.
- `BeepGridProFilterExtensions.cs` attaches a second header `MouseClick`
  handler that opens `BeepGridFilterPopup` for any header hit. This conflicts
  with sorting, resize, and reorder, and its geometry ignores display order,
  sticky columns, horizontal scroll, and clipping.
- `BeepGridPro.Dialogs.cs` exposes `EnableExcelFilter`, which only toggles
  `ShowFilterIcon`; it does not open the value-list popup.

## 3. Target behavior

### 3.1 Header cell anatomy

```
┌──────────────────────────────────────────────┐
│ Caption text                  [sort] [menu] │
│ ↑/↓ + order badge                 funnel/dot │
└──────────────────────────────────────────────┘
```

- Caption occupies remaining width, ellipsized with a tooltip.
- Sort slot is reserved for sortable columns whether or not sorted.
- Menu slot is reserved for columns that offer a menu; the hit target is at
  least 24×24 px even when the glyph is smaller.
- Filter-active state is a dot or count on the menu/funnel glyph.
- Right alignment for numeric/currency columns; left for text; center for
  boolean/system columns.

### 3.2 Sort

- Caption click cycles ascending → descending → none by default.
- Shift-click appends a column to multi-sort; a small badge shows the order.
- `Ctrl` is reserved for other grid actions; do not overload it here.
- Sorting a non-group column while grouped sorts within groups; sorting a group
  column reorders groups. Preserve the existing `GridGroupEngine` behavior.
- Keyboard: focused header + Enter/Space performs the same cycle; Alt+Down
  opens the menu instead of sorting.

### 3.3 Column menu

The menu replaces the current three-item menu and becomes the single header
command surface:

- Sort ascending / Sort descending / Clear sort
- Filter… (opens the value list for this column)
- Clear filter (only when active)
- Auto-size this column
- Pin / Unpin column
- Group by this column / Remove grouping
- Hide column
- Reset columns
- Copy column name

Use separators and disabled states. Destructive actions (Hide, Reset) use the
theme’s warning/error semantic color where appropriate.

### 3.4 Filter affordance

- The funnel does **not** open a popup directly from a header `MouseClick`.
- Header click routing is owned by `GridInputHelper` + a single
  `GridHeaderController`.
- Clicking the funnel opens the column menu with the filter command focused.
- Clicking “Filter…” opens the Excel-style value list (Stage 4) or the
  type-aware filter editor. The simple `Filter by Column` dialog is removed
  from the header path and kept only as a host command alias if needed.

### 3.5 Resize and reorder

- Resize hit target: 6 px, `SizeWE`, visible on hover; double-click best-fits
  one column, Ctrl+double-click fits all visible columns.
- Reorder: drag ghost + insertion marker; sticky columns cannot move into the
  scrolling region; system columns cannot move; Esc cancels.
- During resize/reorder, suppress sort and menu actions.
- Column width/order changes mark the persisted layout dirty (Stage 2 of the
  Phase 2 layout state already tracks this).

### 3.6 Sticky and scrolling headers

- Sticky header cells stay aligned with sticky body cells.
- A divider and shadow separate sticky from scrolling columns.
- Horizontal scroll moves only the scrolling header band.
- Column virtualization must render the same geometry as the full layout.

### 3.7 Header height and density

- Height comes from `GridChromeMetrics` + painter font metrics.
- Compact/standard/comfortable/touch densities are supported.
- Multi-line captions are opt-in; default is single-line ellipsis.
- Header hover only repaints the header band.

### 3.8 Accessibility

- Header cell role, accessible name = caption, sort state, filter state, and
  menu availability.
- Focus ring around the focused header cell, not the whole band.
- Screen reader announces “sorted ascending, position 1 of 2” after sort.
- Resize and reorder expose tooltips and keyboard alternatives where possible.

## 4. Tasks

| # | Task | Status |
|---|------|--------|
| 2.1 | Add `GridHeaderController` and move header hit routing out of ad-hoc handlers | ⬜ |
| 2.2 | Remove `BeepGridProFilterExtensions` header `MouseClick` popup path | ✅ |
| 2.3 | Rebuild `HeaderCellLayout` geometry from shared metrics; reserve sort/menu slots | ✅ |
| 2.4 | Implement caption click sort cycle + Shift multi-sort + order badges | ✅ |
| 2.5 | Implement keyboard header focus, Enter/Space sort, Alt+Down/Menu menu | ✅ |
| 2.6 | Expand column menu to the full command set; add disabled states | ✅ |
| 2.7 | Add filter-active dot/count and menu-focused filter command | ✅ |
| 2.8 | Make funnel open the column menu, not a direct popup | ✅ |
| 2.9 | Add resize handle hover, cursor, double-click best-fit, Ctrl+fit-all | ✅ |
| 2.10 | Add drag ghost + insertion marker; enforce sticky/system rules; Esc cancel | ⬜ |
| 2.11 | Align sticky header with sticky body; add divider/shadow | ⬜ |
| 2.12 | Add header density/multiline/tooltip behavior | ⬜ |
| 2.13 | Add accessible names/states and focus-ring drawing | ⬜ |
| 2.14 | Add header geometry, sort-state, menu-action, resize, reorder, and keyboard tests | ⬜ |
| 2.15 | Visual QA all 12 header painters at 100/125/150 % DPI | ⬜ |

## 5. Acceptance criteria

- A header click never opens a filter popup.
- Sort, menu, resize, and reorder have disjoint hit zones.
- Multi-sort order is visible and correct.
- Column menu exposes sort/filter/autosize/pin/group/hide/reset.
- Filter state is visible on the header and clears from the menu.
- Header is keyboard operable with a visible focus ring.
- Sticky/scroll header geometry matches body cells.
- All header painters share the same geometry contract.

## 6. Verification

- Unit tests for hit-zone geometry and sort cycle.
- Menu action tests for autosize/pin/group/hide/reset.
- Keyboard-only header walkthrough.
- Bitmap fixtures for each header painter.
- Regression: sort + filter + grouping + reorder + resize in one session.

## 7. Risks

- Removing the header `MouseClick` popup may surprise hosts that relied on it.
  Keep `EnableExcelFilter` as a command that opens the menu/filter surface
  intentionally, not as a click hook.
- Painter-specific hover visuals may need per-style tweaks; keep geometry shared
  and presentation local.
