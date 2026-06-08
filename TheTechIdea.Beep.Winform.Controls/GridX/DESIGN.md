# BeepGridPro — Design Document

_Visual and interaction design rationale. For the layer-by-layer
architecture see [DESIGN_ARCHITECTURE.md](./DESIGN_ARCHITECTURE.md).
For the public API see [README.md](./README.md)._

## 1. Design Goals

The grid is designed to be:

1. **Self-contained.** A `BeepGridPro` instance renders its own
   header, rows, scrollbars, filter panel, navigator, and toolbar. No
   child controls are required except the on-demand search editor.
2. **Painter-pluggable.** The look and feel of the header, navigator,
   and filter panel is selected by the `GridStyle` property and
   resolved through a factory. New styles ship as new painter classes
   without touching the core.
3. **Layout-pluggable.** Per-style padding, border radius, and header
   background are selected by `ApplyLayoutPreset` so a single
   `BeepGridStyle` can have several layout variants.
4. **Data-source agnostic.** `BindingSource`, `DataTable`,
   `DataView`, `DataSet+DataMember`, `IEnumerable<T>`, root objects with
   collection properties, and UoW are all first-class. Selection of
   binding strategy happens inside `GridDataHelper` and is opaque to
   the rest of the grid.
5. **Virtualization-aware.** Both row and column virtualization are
   first-class. The grid handles 100 K+ rows with on-demand
   materialization.
6. **Testable.** Helpers are independently constructible with a
   mock-friendly surface; integration tests live in `GridX/Testing/`.

## 2. Visual Design

### 2.1 The control surface

The grid surface is divided into stacked regions. Top to bottom, on a
grid with the toolbar + top-filter-panel + column-headers + navigator
all enabled:

```
┌─────────────────────────────────────────────────────────────────┐
│ Unified toolbar (Phase 18)                ─┐
│ ┌─[Title]─┬─[New][Edit][Del]─┬─[🔍]──┬─[⚙][⛭][✕]─┬─[↥][↧][🖨]┐│
│ └─────────┴──────────────────┴───────┴─────────────┴────────────┘│
│ Top filter panel (legacy, hidden when toolbar is enabled)        │
│ Column headers                                                     │
│ Sticky columns (Sel, RowNum, RowID)                               │
│ ┌─Sel─┬─#─┬─ID─┐┌────────── headers ──────────┐                 │
│ │  ☑ │ 1│  1││ Name   │ Email   │ Region  │   │                 │
│ ├────┼──┼───┤├─────────┼─────────┼──────────┤                 │
│ │  ☑ │ 2│  2││ Alice  │ a@x.com │ EMEA    │   │                 │
│ │  ☐ │ 3│  3││ Bob    │ b@x.com │ APAC    │   │                 │
│ │  ☐ │ 4│  4││ Carol  │ c@x.com │ NA      │   │                 │
│ ├────┴──┴───┴└────────────────────────────────┘                 │
│ Navigator (paged or external)                                     │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 The unified toolbar

The toolbar is the user-facing **command surface** for the grid. It
follows the convention used by DevExpress XtraGrid, Telerik
RadGridView, and AG-Grid:

```
[Title]   [+ New] [✎ Edit] [🗑 Delete]   |   [🔍 Search...]   |   [⚙ Filter] [⛭ Advanced] [✕]   |   [↑ Import] [↓ Export] [🖨 Print]
```

**Sections, left to right:**

| Section | Element | Visible | Default action |
|---|---|---|---|
| **Title** | `GridTitle` text | When `ShowGridTitle = true` | — |
| **Actions** | New / Edit / Delete | When toolbar button visible | `Navigator.InsertNew` / `Edit.BeginEdit` / `Navigator.DeleteCurrent` |
| **Search** | Search icon + flexible box | Always when toolbar is shown | `ApplyQuickFilter` on commit |
| **Filter** | Filter / Advanced / Clear | Always when toolbar is shown | `ShowFilterDialog` / `ShowAdvancedFilterDialog` / `ClearFilter` |
| **Export** | Import / Export / Print | When toolbar button visible | `ToolbarAction` event (host implements) |

**Visual rules:**

- Section dividers are 1 px vertical lines using
  `ToolbarSeparatorColor`.
- Action buttons (New / Edit / Delete) show **icon + text label**; the
  rest are **icon-only**.
- Hover state: rounded background with 4 px radius in
  `ToolbarButtonHoverBackColor`.
- Pressed state: same shape with `ToolbarButtonPressedBackColor`.
- The Clear filter button only renders when `IsFilterActive` is
  true, and shows a badge with the active criteria count.
- The filter icon tints to the accent colour when a filter is
  active; otherwise 60 % opacity.
- The search icon tints to the accent colour when the search box has
  focus; otherwise the toolbar foreground colour.
- Search box border tints to the accent colour on focus.
- All sizes are DPI-scaled. 16 px icon at 100 % DPI; multiplies by
  `ToolbarState.DpiScale`.

**Behaviour rules:**

- The toolbar and the legacy top filter panel are mutually exclusive.
  Enabling `ShowToolbar` hides the filter panel; enabling
  `ShowTopFilterPanel` hides the toolbar.
- The grid title is opt-out via `ShowGridTitle = false`.
- The search box placeholder is configurable via `SearchPlaceholder`.
- The keyboard shortcut `Ctrl+F` focuses the search box. `Escape`
  cancels an active search.
- Action buttons are visible by default. Use
  `SetToolbarButtonVisible(key, false)` to hide individual buttons
  (e.g. disable Delete when no row is selected).
- When the toolbar is narrower than the minimum, action / export
  buttons flow into an overflow menu surfaced by a chevron button.

### 2.3 Per-style look

The `GridStyle` enum drives every visual decision:

- Header painter (`MaterialHeaderPainter`, `FluentHeaderPainter`, …)
- Navigator painter
- Filter-panel painter (legacy)
- Padding, font, border-radius, alternating-row stripe
- Default sort direction and indicator visibility
- Default grid line style

The default style is `Bootstrap`. The painter factories resolve the
class on every paint so changing `GridStyle` at runtime takes effect
on the next frame.

### 2.4 Layout presets

`ApplyLayoutPreset(GridLayoutPreset)` adjusts padding, border-radius,
and header background on top of the painter. The legacy 12 are
property-wired; the 11 modern classes (`Material3SurfaceLayout`,
`Fluent2CardLayout`, `TailwindDashboardLayout`, …) are applied
directly via the `IGridLayoutPreset` overload.

## 3. Interaction Design

### 3.1 The cell as the unit of interaction

The grid treats the cell as the smallest unit of focus, hover, and
selection. Group headers and summary rows are non-editable
extensions; system columns (`Sel`, `RowNum`, `RowID`) are sticky and
always visible.

### 3.2 Click → Hover → Press → Commit

Every interactive surface follows the same state machine:

1. **Click** — set the active cell and update selection.
2. **Hover** — paint a subtle background tint over the row or header.
3. **Press** — paint a darker tint while the mouse button is held
   over the cell.
4. **Commit** — on mouse-up inside the same cell, run the action
   (toggle checkbox, fire `ItemClicked`, start edit if double-click).

Toolbar buttons follow the same state machine. The `IsHovered` and
`IsPressed` keys are tracked in `BeepGridToolbarState`.

### 3.3 Selection semantics

`BeepRowConfig.IsSelected` (set by the leading checkbox column) is the
canonical row-selection flag. The active cell is tracked separately
by `GridSelectionHelper`. The two are independent so a row can be
selected without changing the active cell, and vice versa.

The `SelectionMode` strategy pattern is implemented but not yet
wired into all input handlers. The current behaviour matches
`CellSelectionStrategy`: a click sets the active cell; a click on a
row's checkbox toggles `IsSelected` without moving the active cell.

### 3.4 Edit flow

Click → select cell → press F2 or Enter → `Edit.BeginEdit` creates
the appropriate `IGridEditor` (resolved by `BeepColumnType`) → the
editor overlays the cell → commit fires `CellValueChanged` and
`RequestAutoSize(AutoSizeTriggerSource.EditCommit)` → editor is
disposed.

`Esc` cancels; clicking away also commits (the editor's `LostFocus`
event fires `RequestEndEdit(true)`).

### 3.5 Filter flow

Two filter surfaces coexist by design:

- **Excel-style** — click the funnel on a column header to open
  the per-column popup. Driven by `GridSortFilterHelper`.
- **Advanced** — click the sliders button in the toolbar (or call
  `ShowAdvancedFilterDialog`) to open the multi-criteria dialog.
  Driven by `ActiveFilter`.

The toolbar's search box is the third entry point. It applies a
free-text `ApplyQuickFilter(text, column?)` to the active filter
pipeline and is the cheapest path for "I just want to find this row".

### 3.6 Group flow

`GroupBy(column)` adds a `GroupDescriptor`. The grid:

1. Re-runs the sort + filter pipeline to ensure ordering.
2. Splits the rows into `GridGroup` buckets.
3. Inserts `GridGroupHeaderRow` and `GridGroupSummaryRow` into the
   virtual row stream.
4. Repaints.

Clicking a group header toggles collapse. The summary row's
aggregation values are computed at group-build time and stored in
`GridGroupSummaryRow` so per-row paint does not need to re-aggregate.

### 3.7 Drag and drop

Drag-reorder is enabled on non-system, non-sticky, visible columns
with `AllowReorder = true`. The drag visual is rendered as a 2 px
vertical guide at the candidate drop position; the drop is committed
on mouse-up.

## 4. UX Patterns Adopted

| Pattern | Source | Where in BeepGridPro |
|---|---|---|
| Single toolbar with actions + search + filter + export | DevExpress XtraGrid, Telerik RadGridView | Phase 18 unified toolbar |
| Sticky system columns (Sel / RowNum / RowID) | BeepSimpleGrid legacy | `EnsureSystemColumns` |
| Excel-style column header filter popup | Microsoft DataGridView | `BeepGridFilterPopup` + `GridSortFilterHelper` |
| Per-cell active focus with focus ring | Universal | `GridFocusManager.DrawFocusIndicator` |
| Ctrl+F focuses search | Universal | `BeepGridPro.Input.OnKeyDown` |
| F2 to edit, Esc to cancel, Enter to commit | Standard | `GridEditHelper` + editor event handlers |
| Overflow menu for narrow toolbars | Material Design, AG-Grid | `BeepGridToolbarState.HasOverflowItems` |
| Group header chevron for collapse/expand | Excel pivot, AG-Grid grouping | `GridGroupHeaderRenderer` |
| Badge for active filter count | Standard | `BeepGridToolbarPainter.PaintBadge` |
| Color-coded filter icon when active | Telerik | `BeepGridToolbarPainter.PaintFilterSection` |

## 5. UX Anti-Patterns Avoided

- **No "filter panel as a real WinForms control with child controls
  fighting z-order."** The unified toolbar is fully owner-drawn.
- **No emoji icons in the toolbar.** All icons are SVG paths tinted
  with the toolbar foreground colour, matching the rest of the
  BeepForms icon system.
- **No "always-show horizontal scrollbar just in case."** The custom
  scrollbars only render when needed.
- **No "all-rows virtualization off by default."** `EnableVirtualization`
  defaults to false to keep the common case (≤ 10 K rows) fast, but
  the grid is designed to switch to virtual mode without code
  changes.
- **No "you must call X before Y to make filtering work."** Both
  filter systems can be applied to an unbound grid; the grid
  attaches itself to whatever source is set.

## 6. Accessibility

The grid is keyboard-navigable end-to-end (see
`GridKeyboardNavigator`). The accessibility object is a
`GridAccessibleObject` with rows and cells. Screen readers announce
column caption, cell value, and selection state.

The toolbar is also keyboard-navigable: `Tab` walks the buttons in
left-to-right order; `Enter` activates the focused button; `Esc`
defocuses the search box.

## 7. Theming

`BeepGridPro` inherits from `BaseControl` and uses the global
`IBeepTheme` system. `GridThemeHelper.ApplyTheme()` re-themes all
renderers and the toolbar state. Toolbar colours have
grid-property-level overrides that the host can set in code or in the
designer. When a property is not set, the painter falls back to the
active theme's `GridBackColor` / `GridHeaderForeColor` / `AccentColor`.

## 8. Open UX Questions

1. **Should the filter dropdown also list saved filters / recent
   searches?** A `FilterHistory` property on `BeepGridPro` is a
   possible Phase 22 addition.
2. **Should the toolbar be customizable (drag-reorder buttons,
   add/remove)?** The `ToolbarButtonItem` model already supports
   arbitrary keys and labels, so a `ToolbarDefinition` API would be
   additive. Not currently planned.
3. **Should the search box support regex or wildcard syntax?** The
   current `ApplyQuickFilter` is substring-only. A `SearchMode` enum
   could add Contains / StartsWith / Regex.

## 9. Future Phases (UI/UX)

| Phase | Title | UX impact |
|---|---|---|
| 19 | Per-button tooltips | Hovering a toolbar button shows a tooltip with the action + shortcut |
| 20 | Unified filter engine | Both `ActiveFilter` and `SortFilter` collapse into one `FilterEngine` |
| 21 | Remove obsolete types | `BeepFilterRow` and `BeepQuickFilterBar` are deleted |
| 22 | Filter history dropdown | Quick access to last 10 filters |
| 23 | Per-column quick filter | Filter funnel in the unified toolbar opens the Excel popup for the last-clicked column |
| 24 | Column visibility menu | "Choose columns…" lets the user hide / show columns from the toolbar |
| 25 | Status bar | Bottom bar showing row count, selected count, and sum of numeric columns |

---

## See Also

- [README.md](./README.md) — public surface overview
- [DESIGN_ARCHITECTURE.md](./DESIGN_ARCHITECTURE.md) — layer-by-layer
  architecture
- [Claude.md](./Claude.md) — code-level invariants
- [Enhancements/PHASE_018_UnifiedToolbar.md](./Enhancements/PHASE_018_UnifiedToolbar.md)
  — toolbar rationale
