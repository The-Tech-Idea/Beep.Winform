# Phase 3 — Stage 3: Unified search

> Parent plan: [P3_HEADER_SEARCH_NAV_DESIGN.md](P3_HEADER_SEARCH_NAV_DESIGN.md) ·
> Prerequisite: [P3_STAGE_1_INTERACTION_SYSTEM.md](P3_STAGE_1_INTERACTION_SYSTEM.md)

## 1. Goal

Provide one search experience with a clear scope, predictable commit rules,
live results, match count, and keyboard support. Remove the three parallel
search surfaces.

## 2. Current behavior and files

- `Toolbar/BeepGridToolbarState.cs` owns the search box rectangle and the
  `SearchText` snapshot.
- `Toolbar/BeepGridToolbarPainter.cs` paints the search icon, placeholder, and
  focus state.
- `Filtering/FilterEditorHelper.cs` creates the on-demand `BeepTextBox`, commits
  on Enter/lost focus, cancels on Esc, and resizes with the toolbar.
- `BeepGridPro.Input.cs` routes Ctrl+F, `/`, F3, and Esc.
- `BeepGridPro.Filtering.cs` implements `ApplyQuickFilter`, which searches all
  visible non-system columns with one OR group and rebuilds a `GridView`.
- `BeepGridPro.Dialogs.cs` owns a second inline `BeepComboBox` + `BeepTextBox`
  search in the legacy top filter panel.
- `GridDialogHelper.CreateSearchPanel` owns a third modal search with
  Search/Next buttons and its own row-selection logic.

## 3. Target behavior

### 3.1 One search surface

- The toolbar search is canonical. The legacy top-panel search is removed from
  the normal UI; the modal Search dialog becomes an alias that focuses the
  toolbar search or is removed after host compatibility review.
- The search box has:
  - search icon
  - text editor
  - scope selector (All columns / one column)
  - clear button when text exists
  - match count / state text
  - next/previous buttons when a query is active

### 3.2 Query semantics

- Default: case-insensitive literal contains across all visible, non-system
  columns.
- Scope: all columns or one column; the scope is visible in the box.
- Options: case-sensitive and diacritic-insensitive toggles in the search
  options menu.
- No regex in quick search; regex remains an advanced filter operator.
- Tokens separated by spaces are AND by default; support a “match any token”
  option.
- Empty/whitespace clears the search and restores the prior filters.

### 3.3 Live behavior

- Debounce 250 ms; Enter commits immediately; Esc cancels the in-flight query
  and clears the text only when the box is focused and empty-state rules allow.
- While typing, do not rebuild the grid on every keystroke; update the query
  once per debounce when the text changes.
- Show “No matches” inside the grid empty state with a Clear search action.
- Show the match count next to the box and announce it.
- Highlight the matched substring in visible cells with a theme-safe highlight
  (foreground/background pair, no hard-coded yellow).
- Next/previous navigates matches and selects the matching cell.

### 3.4 Search and filters

- Search is a condition in the shared `GridQuery` model, not a separate
  `IsVisible` mutation.
- The active-filter chips row includes a search chip (`Search: "foo"`) with an
  `x`; clearing it clears the search box.
- Clearing all filters clears search and column filters together.
- Search + column filters combine with AND. The chips row shows both.
- Search persists across sort/group/page changes; paging resets to the first
  page when the query changes.

### 3.5 Virtualization and large data

- In virtual mode, search goes through `IGridQuerySource` when available. It
  must not search only the materialized window.
- If the source cannot search server-side, disable the search box with an
  explanatory tooltip rather than returning misleading local results.
- Debounce/cancellation must prevent out-of-order results (reuse query
  revisions).

### 3.6 Keyboard and accessibility

- Ctrl+F focuses the box. `/` and F3 focus it when no editor is active.
- Enter commits / moves to next match; Shift+Enter previous; Esc clears or
  cancels.
- Tab moves from the box to scope, clear, and next/previous in logical order.
- Screen reader name: “Search grid”; description: current scope and result
  count; live announcement on count change.
- Focus ring visible; placeholder contrast at least 4.5:1.

### 3.7 Host API

Keep:

```csharp
grid.ApplyQuickFilter("text");
grid.ApplyQuickFilter("text", "CustomerName");
grid.ClearFilter();
```

Add if needed:

```csharp
grid.SearchScope = SearchScope.AllColumns; // or a column name
grid.SearchOptions = new GridSearchOptions { CaseSensitive = false, MatchAllTokens = true };
```

All existing calls route to the unified component.

## 4. Tasks

| # | Task | Status |
|---|------|--------|
| 3.1 | Add `GridSearchController` with scope, options, debounce, revision, and result count | ⬜ |
| 3.2 | Extend the search box layout for scope, clear, count, and next/previous | ✅ |
| 3.3 | Remove/route the legacy top-panel inline search | ✅ |
| 3.4 | Remove/route the modal Search dialog to the unified search | ✅ |
| 3.5 | Implement token AND/OR and case/diacritic options | ⬜ |
| 3.6 | Implement debounce + cancellation + stale-result discard | ✅ |
| 3.7 | Add no-results state with Clear search | ✅ |
| 3.8 | Add match count + next/previous + selected-cell navigation | ✅ |
| 3.9 | Add visible-cell match highlighting from the query view | ✅ |
| 3.10 | Add search chip + clear-all integration | ✅ |
| 3.11 | Route virtual search through `IGridQuerySource`; disable when unavailable | ✅ |
| 3.12 | Add accessible names/announcements and keyboard tests | ⬜ |
| 3.13 | Add query, debounce, count, highlight, and virtualization tests | ⬜ |
| 3.14 | Visual QA search box at all densities/themes/DPI | ⬜ |

## 5. Acceptance criteria

- One visible search surface; no duplicate search text/edit state.
- Typing updates results once per debounce, never once per keystroke.
- Match count and no-results state are visible and announced.
- Search participates in the chips row and Clear all.
- Virtual search never searches only the window.
- Keyboard-only search works end to end.

## 6. Verification

- Unit tests for token parsing, scope, count, and revision handling.
- Integration tests: type → debounce → view → count → next/prev → clear.
- Virtual source test proving full-source search.
- Screen-reader walkthrough and bitmap fixture.

## 7. Risks

- Live search on very large in-memory grids can be expensive. Debounce and the
  query engine’s single evaluation are required; add a result cap/label if
  needed.
- Removing the modal Search dialog may affect hosts. Keep a compatibility alias
  that focuses the toolbar search rather than a second implementation.
