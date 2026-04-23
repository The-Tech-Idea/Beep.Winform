# Phase 6: UoW PostCommit/PostUpdate Cell Sync

**Priority:** P1 | **Track:** Data & Binding | **Status:** Pending

## Objective

After a UoW commit or update, ensure the grid reflects server-generated values (PKs, timestamps) and updated entity properties.

## Problem

`HandleUowPostCommit` only calls `SafeInvalidate()` — does not re-read cell values. After a commit that persists server-generated values (e.g., identity PKs), the grid won't show those new values. `HandleUowPostChange` for `PostUpdate/PostEdit` has the same issue.

## Implementation Steps

### Step 1: Fix PostCommit

In `GridUnitOfWorkBinder.cs`:

```csharp
private void HandleUowPostCommit(object sender, UnitofWorkParams e)
{
    // Full re-read to capture server-generated values
    RefreshBinding();
}
```

### Step 2: Fix PostUpdate/PostEdit

```csharp
private void HandleUowPostChange(object sender, UnitofWorkParams e)
{
    if (e.EventAction == EventAction.PostUpdate || e.EventAction == EventAction.PostEdit)
    {
        // Try targeted row sync if index hint is available
        if (e.DirtyColumns != null && e.RowIndex >= 0 && e.RowIndex < _grid.Rows.Count)
        {
            var row = _grid.Rows[e.RowIndex];
            // Re-read values from the entity for dirty columns
            foreach (var colName in e.DirtyColumns)
            {
                var col = _grid.Columns.FirstOrDefault(c =>
                    string.Equals(c.ColumnName, colName, StringComparison.OrdinalIgnoreCase));
                if (col != null)
                {
                    var cell = row.Cells.FirstOrDefault(c => c.ColumnIndex == col.Index);
                    if (cell != null)
                    {
                        cell.CellValue = _grid.Data.GetPropertyValue(row.RowData, colName);
                    }
                }
            }
            _grid.InvalidateRow(e.RowIndex);
            return;
        }
        // Fallback: full repaint
        _grid.SafeInvalidate();
        return;
    }
    RefreshBinding();
}
```

## Acceptance Criteria

- [ ] UoW commit with server-generated PK shows new PK value in grid
- [ ] UoW update shows updated property values
- [ ] Targeted sync works when column hints are available
- [ ] Fallback to full repaint works when hints are not available
- [ ] No infinite refresh loops from post-change events

## Rollback Plan

Revert `HandleUowPostCommit` and `HandleUowPostChange` to their original implementations.

## Files to Modify

- `Helpers/GridUnitOfWorkBinder.cs`
