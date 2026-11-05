# Phase 3 — Stage 1: Interaction system and shared chrome contract

> Parent plan: [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md)

## 1. Goal

Give headers, search, filters, and navigation one shared visual and interaction
contract before changing their layouts. This prevents each painter from
inventing its own spacing, state colors, focus behavior, and responsive rules.

## 2. Current behavior to replace

- Painters use their own constants (`RecommendedHeight`, `BUTTON_SIZE`,
  `SPACING`, `HeaderCellPadding`) and their own fallback colors.
- Header input lives in `GridInputHelper`; header menu actions live in
  `BeepGridPro.ContextMenu`; a second header `MouseClick` handler lives in
  `BeepGridProFilterExtensions`.
- Search has three separate implementations: toolbar (`FilterEditorHelper`),
  top filter panel (`BeepGridPro.Dialogs` inline controls), and modal search
  (`GridDialogHelper.CreateSearchPanel`).
- Navigation has a painter path and a legacy `BeepButton` path in
  `GridNavigationPainterHelper`.
- There is no shared definition of focus, hover, active, disabled, or density.

## 3. Design

### 3.1 Add `GridChromeMetrics`

One internal class that resolves, from the current theme and DPI:

- density: `Compact`, `Standard`, `Comfortable`, `Touch`
- header height, nav height, toolbar height, chip height
- horizontal/vertical padding, gap, icon size, minimum hit target
- focus-ring thickness and offset
- resize-handle width (6 px), menu-hit width, sort-slot width
- responsive breakpoints (`<480`, `480–767`, `768–1199`, `>=1200`)
- motion durations and reduced-motion behavior

Painters and layout helpers consume this class. They may choose a different
presentation, but not different geometry rules.

### 3.2 Add `GridChromeState`

One state model:

- `Normal`, `Hover`, `Focused`, `Pressed`, `Active`, `Disabled`, `Error`
- resolves background, foreground, border, focus ring, and icon tint from
  `BeepThemesManager` semantic slots
- explicit host overrides remain separate from themed values, as required by
  the control library rules

### 3.3 One focus and keyboard contract

Define and document:

| Surface | Tab order | Keys |
|---|---|---|
| Header | header band is one tab stop | Left/Right move column focus; Enter/Space sort; Alt+Down/Menu opens column menu; Esc leaves |
| Toolbar search | search is one tab stop | Ctrl+F, `/`, F3 focus; Enter commits/next; Shift+Enter previous; Esc clears/cancels |
| Filter menu/popup | menu is modal within the grid | Up/Down, Enter, Esc, type-to-search |
| Filter chips | each chip and its remove button | Left/Right, Delete removes, Enter edits |
| Navigator | pager controls are tab stops | Enter/Space activate; Home/End jump; PgUp/PgDn page |

### 3.4 One responsive contract

- **≥1200 px:** full labels, title, search scope, all actions, full pager.
- **768–1199 px:** search shrinks first; action labels collapse to icons; page
  numbers reduce; title truncates.
- **480–767 px:** title hides; actions go to overflow; search keeps a minimum
  editable width; pager switches to compact.
- **<480 px:** toolbar keeps search + overflow; header keeps sort + menu;
  navigator keeps first/prev/next/last and status only.
- Nothing clips mid-button; overflow is explicit.

### 3.5 Accessibility contract

- Header cells expose name, sort state, filter state, and menu availability.
- Search exposes its scope and result count.
- Chips expose the condition text and a remove action.
- Pager controls expose names such as “First page”, “Next page”, and current
  page/total.
- Focus ring visible at 4.5:1 contrast; touch density uses 44 px targets.
- Live announcements for result count, filter applied, and page changed.

## 4. Tasks

| # | Task | Status |
|---|------|--------|
| 1.1 | Add `GridChromeMetrics` and `GridChromeState`; migrate header, toolbar, search, filter-panel, and navigation painters to them | ⬜ |
| 1.2 | Define density modes and default metric values; add DPI scaling helpers | ✅ |
| 1.3 | Define state tokens and remove per-painter literal colors except semantic theme fallbacks | ⬜ |
| 1.4 | Define the keyboard/focus map and implement the shared focus-ring drawing helper | ⬜ |
| 1.5 | Define responsive breakpoints and overflow order for toolbar, header, chips, and pager | ⬜ |
| 1.6 | Add the accessibility contract helpers (name/state/announcement builders) | ⬜ |
| 1.7 | Remove duplicate header `MouseClick` filtering entry point; route header actions through one controller | ✅ |
| 1.8 | Inventory and delete unused duplicate chrome code (`ExcelFilterHelper` if unused, duplicate search panels, duplicate nav renderer after parity) | ⬜ |
| 1.9 | Normalize all chrome icons through `BeepImage`/`StyledImagePainter` tinting | ⬜ |
| 1.10 | Add a deterministic chrome fixture (bitmap render + geometry assertions) for later stages | ⬜ |

## 5. Acceptance criteria

- Header, toolbar, search, filter panel, and navigator read height, spacing,
  state colors, and breakpoints from one metrics/state layer.
- Keyboard focus order is documented and testable; focus ring is visible in
  light, dark, and high-contrast themes.
- There is exactly one handler for each header action.
- The unused duplicate chrome code is removed or explicitly routed.
- Chrome fixture renders deterministically at 100/125/150 % DPI.

## 6. Verification

- Unit tests for metric resolution and breakpoint selection.
- Geometry assertions for header/toolbar/chips/pager at each density.
- Bitmap fixture at three DPI values.
- Keyboard walkthrough: Tab into header, sort, open menu, move to search,
  filter, chips, pager, and back to the grid.

## 7. Risks

- Existing painter styles may look slightly different after shared padding is
  applied. Treat that as intended and verify each style visually.
- Host overrides of toolbar colors must remain intact. Track “explicit” colors
  separately from theme-resolved colors.
