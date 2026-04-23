# Phase 1: Filter Visibility Rendering Fix

**Priority:** P0 | **Track:** Bug Fixes | **Status:** Pending

## Objective

Fix the rendering pipeline so that rows with `IsVisible = false` are not painted and do not consume vertical space in the viewport.

## Problem

`ApplyLocalVisibilityFilter()` in `GridSortFilterHelper.cs` correctly sets `row.IsVisible = false` on filtered-out rows. However, `DrawRows()` in `GridRenderHelper.Rendering.cs` ignores this flag entirely — every row in the viewport range is drawn regardless of visibility state.

## Affected Code Locations

| Location | Issue |
|---|---|
| `GridRenderHelper.Rendering.cs` — `DrawRows()` main loop | Draws every row without checking `IsVisible` |
| `GridRenderHelper.Rendering.cs` — sticky columns loop | Same issue for sticky column rendering |
| `GridRenderHelper.Rendering.cs` — `currentY` height accumulation | Adds height for invisible rows, displacing scroll position |
| `GridRenderHelper.Rendering.cs` — `GetVisibleRowCount()` | Counts invisible rows when calculating viewport capacity |
| `GridScrollHelper.cs` — scroll bar update | May use total row count instead of visible row count |

## Implementation Steps

### Step 1: Main Draw Loop

In `DrawRows()`, add visibility guard at the top of the row iteration:

```csharp
for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
{
    var row = _grid.Data.Rows[r];
    if (!row.IsVisible) continue;  // ADD THIS
    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
    // ... existing draw logic ...
    drawY += rowHeight;  // Only advance for visible rows
}
```

### Step 2: Sticky Columns Loop

Apply the same guard to the sticky columns draw loop in the same file.

### Step 3: Height Accumulation

In the loop that computes `currentY` (total height of rows before the first visible row):

```csharp
for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
{
    var row = _grid.Data.Rows[i];
    if (!row.IsVisible) continue;  // ADD THIS
    totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
}
```

### Step 4: GetVisibleRowCount()

Update the height accumulation inside `GetVisibleRowCount()` to skip invisible rows.

### Step 5: Scroll Bar Update

Verify `ScrollBars.UpdateBars()` calculates scroll range based on visible row height sum, not `_grid.Data.Rows.Count`. Fix if necessary.

### Step 6: Filter Row Verification

Verify that inline filter row text boxes call `_grid.SortFilter.Filter(columnName, text)` on `TextChanged`. If missing, the `_containsFilters` dictionary is never populated.

## Acceptance Criteria

- [ ] Filtered rows are not rendered
- [ ] Filtered rows do not consume vertical space
- [ ] Scroll bar range reflects visible row count
- [ ] Clearing filter restores all rows
- [ ] No regression in unfiltered rendering performance

## Rollback Plan

Revert changes to `GridRenderHelper.Rendering.cs` — the added `if (!row.IsVisible) continue;` guards are isolated and safe to remove individually.

## Files to Modify

- `Helpers/GridRenderHelper.Rendering.cs`
- `Helpers/GridScrollHelper.cs` (possibly)
