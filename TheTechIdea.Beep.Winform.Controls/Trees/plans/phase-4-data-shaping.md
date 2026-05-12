# Phase 4: Data Shaping & Analysis

## Goal
Add data shaping capabilities: sorting, advanced filtering, incremental search, and summaries to enable users to analyze hierarchical data effectively.

## Current State Analysis
BeepTree has basic filtering (`FilterText` property) and search (`SearchNodes` method). No sorting, no advanced filtering UI, no summaries or aggregates.

## Research Insights

### DevExpress TreeList
- High-speed multi-column sorting
- Summaries & Data Aggregates (Sum, Count, Average, Min, Max, Custom)
- Instant Search & Incremental Search
- Query Builder / Filter Criteria Editor
- Excel Inspired Filtering (filter popup per column)
- Auto Filter Row (filter row above data)
- Filter Panel & MRU Filters
- Conditional Formatting (data bars, icons, color scales)

### Telerik RadTreeView
- Sorting with `SortOrder` property
- Programmatic filtering
- Search with `Find` method

### Syncfusion TreeViewAdv
- Sorting nodes dynamically
- Filtering support

## Tasks

### 4.1 Sorting
- [ ] Add `SortColumn` and `SortOrder` properties
- [ ] Single column sort:
  - [ ] Click column header to sort ascending
  - [ ] Click again to sort descending
  - [ ] Click third time to clear sort
  - [ ] Visual indicator (up/down arrow in header)
- [ ] Multi-column sort (Ctrl+Click headers):
  - [ ] Primary sort, secondary sort, etc.
  - [ ] Show sort priority numbers in headers
- [ ] Sort modes:
  - [ ] `Standard` (string/numeric/date comparison)
  - [ ] `Custom` (via `CompareNodes` event)
  - [ ] `Natural` (human-friendly string sort: Item2 < Item10)
- [ ] Sort scope:
  - [ ] `Global` - sort all visible nodes
  - [ ] `Level` - sort nodes within same parent only
  - [ ] `Group` - sort within expanded groups
- [ ] Preserve expand/collapse state after sort
- [ ] Async sort for large datasets (background thread)

### 4.2 Advanced Filtering
- [ ] Excel-style column filter popup:
  - [ ] Checklist of unique values
  - [ ] Search within values
  - [ ] (Select All) option
  - [ ] Sort A-Z / Z-A in popup
- [ ] Auto Filter Row:
  - [ ] Row above all nodes with filter inputs per column
  - [ ] Filter operators per cell (=, <>, <, >, contains, starts with, etc.)
  - [ ] Visual distinction from data rows
- [ ] Filter Criteria Builder:
  - [ ] Visual query builder (AND/OR conditions)
  - [ ] Field picker, operator picker, value input
  - [ ] Save/load filter criteria
  - [ ] MRU (Most Recently Used) filters list
- [ ] Filter Panel:
  - [ ] Show active filters as chips/tags
  - [ ] Click to edit, X to remove
  - [ ] Clear All button
- [ ] Filter types:
  - [ ] Text filters (contains, starts with, ends with, equals)
  - [ ] Numeric filters (equals, less than, greater than, between)
  - [ ] Date filters (today, this week, this month, between)
  - [ ] Custom filter (predicate delegate)

### 4.3 Search & Find
- [ ] Incremental search (type letters to jump to matching node):
  - [ ] `IncrementalSearch` property (enable/disable)
  - [ ] `SearchDelay` property (ms to wait before searching)
  - [ ] Highlight matching text in nodes
  - [ ] Wrap around option
- [ ] Find Panel (Ctrl+F):
  - [ ] Search box overlay at top of tree
  - [ ] Find next / previous buttons
  - [ ] Match case option
  - [ ] Match whole word option
  - [ ] Show "X of Y matches" indicator
  - [ ] Close with Escape
- [ ] Programmatic search:
  - [ ] `FindNode(string text)` - first match
  - [ ] `FindAll(string text)` - all matches
  - [ ] `FindNode(Func<SimpleItem, bool> predicate)` - already exists, enhance

### 4.4 Summaries & Aggregates
- [ ] Footer row at bottom of tree:
  - [ ] Show summary values per column
  - [ ] Multiple summaries per column (e.g., Sum and Count)
- [ ] Summary types:
  - [ ] `Sum`
  - [ ] `Count`
  - [ ] `Average`
  - [ ] `Min`
  - [ ] `Max`
  - [ ] `Custom` (via event)
- [ ] Group summaries:
  - [ ] Show summary for each expanded parent node
  - [ ] Example: folder shows total size of children
- [ ] Summary formatting:
  - [ ] Format string support
  - [ ] Alignment
  - [ ] Visual style (bold, color)

### 4.5 Conditional Formatting
- [ ] Format rules:
  - [ ] `FormatCondition` class (Expression, Rule, Style)
  - [ ] Predefined rules:
    - [ ] Greater than / Less than
    - [ ] Between
    - [ ] Contains
    - [ ] Top N items
    - [ ] Duplicate values
  - [ ] Custom rule (predicate)
- [ ] Format styles:
  - [ ] Background color
  - [ ] Foreground color
  - [ ] Font (bold, italic, size)
  - [ ] Icon sets (arrows, traffic lights, ratings)
  - [ ] Data bars (horizontal bar proportional to value)
  - [ ] Color scales (gradient from min to max)
- [ ] Rule priority and stop-if-true

## Success Criteria
- Sort 10k nodes by multiple columns < 100ms
- Filter panel shows active filters as removable chips
- Find panel highlights all matches with navigation
- Footer shows correct sums/averages for numeric columns
- Conditional formatting applies 5 rules to 10k nodes < 200ms

## Dependencies
- Phase 1: Foundation & Stability
- Phase 2: Data Binding & Multi-Column (sorting/filtering need columns)

## Estimated Effort
4-6 days

## Research References
- DevExpress TreeList: Sorting, summaries, filter builder, conditional formatting
- Telerik RadTreeView: Sorting, programmatic filtering
- Syncfusion TreeViewAdv: Dynamic sorting
