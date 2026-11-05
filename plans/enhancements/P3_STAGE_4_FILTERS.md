# Phase 3 — Stage 4: Filters

> Parent plan: [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md) ·
> Prerequisite: [P3_STAGE_1_INTERACTION_SYSTEM.md](P3_STAGE_1_INTERACTION_SYSTEM.md)

## 1. Goal

Replace the parallel filter surfaces and state stores with one type-aware filter
experience: column menu → Excel-style value list or advanced builder → chips →
query. Search becomes one more condition in the same model.

## 2. Current behavior and files

- `BeepGridPro.Filtering.cs`: `ActiveFilter`, `ApplyQuickFilter`,
  `ShowAdvancedFilterDialog`, `AddFilterCriterion`, `ClearFilter`,
  `FilterApplied`, `FilterCleared`, `_filteredRowIndices` (removed), and the
  active-filter chips row.
- `GridSortFilterHelper.cs`: per-column `contains` and `in` dictionaries,
  source-sort/filter attempts, local visibility fallback, and `ClearFilters`.
- `Filters/BeepGridFilterPopup.cs`: a value-list popup with sort/clear/filter
  events.
- `Helpers/GridDialogHelper.FilteringDialogs.cs`: the “Filter by Column” panel,
  modal wrapper, and Apply/Clear logic.
- `Helpers/GridDialogHelper.cs`: `ShowFilterDialog(preferredColumnName,
  preferredFilterText)` entry point.
- `Painters/BaseFilterPanelPainter.cs` and family: the legacy top filter panel.
- `BeepGridProFilterExtensions.cs`: the header `MouseClick` popup path that
  bypasses the menu.
- `GridRenderHelper.Rendering.cs`: active-filter chips row and hit rectangles.

## 3. Target behavior

### 3.1 One filter model

All entry points write the same query model:

```
GridQuery.Filters:
  GridFilterGroup (AND/OR)
    GridFilterCondition (column, operator, values)
```

`ActiveFilter`, `SortFilter`, search, advanced criteria, Excel values, and chips
are projections or adapters over this model. No feature writes `row.IsVisible`
directly except the single query application step.

### 3.2 Type-aware operators

| Column type | Operators |
|---|---|
| Text | contains, not contains, equals, not equals, starts with, ends with, is empty, is not empty |
| Numeric | equals, not equals, greater, greater or equal, less, less or equal, between, is empty, is not empty |
| Date/DateTime | equals, not equals, before, after, between, is empty, is not empty |
| Boolean | is true, is false, is empty |
| Advanced | in list, not in list, regex |

Operator availability is driven by `BeepColumnConfig.DataType`, not by a global
list. Operators are localized through display names, not glyphs alone.

### 3.3 Excel-style value list

- Opens from the column menu’s Filter command.
- Header: column caption, search-within-values field, sort A→Z / Z→A.
- Body: virtualized checklist with counts, Select all, and a “(Blanks)” entry
  when null/empty values exist.
- Footer: Apply, Cancel, Clear.
- Keyboard: type-to-search, Up/Down, Space toggles, Enter applies, Esc cancels.
- Applying creates an `In` condition with the selected values; blank creates
  `IsNull`/`IsNotNull` as appropriate.

### 3.4 Advanced builder

Reuse the existing `BeepFilter` control but align it to the contract:

- Group logic AND/OR at the top.
- Add condition, remove condition, duplicate, reorder.
- Column → operator → value editor appropriate to the type.
- Between has two editors; In has a chip/list editor.
- Validation inline: empty required value, invalid number/date, invalid regex.
- Preview count: “Matches 128 rows” before Apply.
- Buttons: Apply, Reset, Cancel.
- Keyboard focus trap and Esc cancel.

### 3.5 Active filter chips

- One chip per condition, not per column.
- Chip text: `Column operator value` (localized).
- Each chip has an edit action (click) and a remove action (x).
- “Clear all” clears search, column filters, and advanced filters.
- Overflow: show `+N more` with a menu rather than wrapping into the data area.
- Chips are focusable and deletable from the keyboard.
- The chips row participates in layout height and scrollbar range (already
  reserved by `ActiveFilterBarRect`).

### 3.6 Behavior rules

- Apply once per change; do not rebuild rows for every keystroke inside a filter
  editor.
- Empty value means “no condition” and removes the filter, not “match empty”.
- Null/empty semantics are explicit: text `IsEmpty` matches null and whitespace;
  numeric `IsEmpty` matches null/DBNull.
- Clearing a column filter leaves other columns and search intact.
- Filters survive sort, group, and page changes; paging resets to page 1.
- Filter state can be saved/loaded with schema versioning and validation.
- Virtual/server sources receive one `GridQuery`; no local window-only
  filtering unless the source explicitly falls back.

### 3.7 Remove duplicates

- Delete the header `MouseClick` popup path.
- Delete `ExcelFilterHelper` if still unused.
- Remove the simple “Filter by Column” dialog from the header path; keep
  `ShowFilterDialog(...)` as a compatibility command that opens the new value
  list/editor.
- Remove the legacy top filter panel from the default UI after the chips row and
  toolbar cover the same actions; keep it only behind an explicit compatibility
  flag if a host requires it.

## 4. Tasks

| # | Task | Status |
|---|------|--------|
| 4.1 | Make `GridQuery` the only filter state; adapt `ActiveFilter`/`SortFilter` to it | ⬜ |
| 4.2 | Add type-aware operator catalog and validation | ⬜ |
| 4.3 | Rebuild the Excel value list on the shared popup contract | ⬜ |
| 4.4 | Align the advanced builder to the shared group/condition model | ⬜ |
| 4.5 | Implement chips as per-condition editable/removable items | ✅ |
| 4.6 | Add chip overflow (`+N more`) and clear-all | ✅ |
| 4.7 | Implement null/empty semantics consistently across types | ⬜ |
| 4.8 | Remove the header popup path and unused filter helpers | ✅ |
| 4.9 | Route `ShowFilterDialog`/`ShowAdvancedFilterDialog` to the new surfaces | ⬜ |
| 4.10 | Add saved-filter versioning and validation | ✅ |
| 4.11 | Push a single query to `IGridQuerySource`; remove window-only surprises | ⬜ |
| 4.12 | Add accessible names, focus trap, and keyboard paths for menu/popup/builder/chips | ⬜ |
| 4.13 | Add operator, null, chips, clear, saved-filter, and virtual tests | ⬜ |
| 4.14 | Add performance tests: one evaluation per apply, no per-keystroke rebuild | ⬜ |
| 4.15 | Visual QA popup/builder/chips in light/dark/high-contrast at 3 DPIs | ⬜ |
| 4.16 | Update GridX docs and migration notes | ⬜ |

## 5. Acceptance criteria

- Searching/filtering writes one query; `IsVisible` is written in one place.
- Each column type offers the correct operators and editors.
- Excel value list supports search, counts, blanks, select all, keyboard.
- Advanced builder supports AND/OR groups and preview count.
- Chips are per condition, editable, removable, and overflow safely.
- Clearing one condition never clears unrelated conditions.
- Virtual sources filter across the full source.
- No duplicate filter popup/header path remains.

## 6. Verification

- Parity tests between old `ActiveFilter` cases and the new query engine.
- Operator matrix tests for every supported type.
- Null/empty matrix tests.
- Chip edit/remove/clear-all tests.
- Keyboard-only menu → popup → apply → chip → clear walkthrough.
- Virtual source filter test and performance budget test.
- Bitmap fixtures for popup, builder, and chips.

## 7. Risks

- Unifying the two pipelines changes edge-case semantics. Freeze the current
  behavior in parity tests before deleting the old state.
- Value-list counts over very large sources may need a server-side count/limit.
  Define the fallback and label it in the UI.
- Saved filters can outlive a schema. Validate column names and operator values
  on load and report rejected conditions.
