# Phase 4: DataTable Cell-Level Fast Refresh

**Priority:** P0 | **Track:** Data & Binding | **Status:** Pending

## Objective

When a cell in a DataTable-bound grid is edited, update only that cell instead of triggering a full rebind (O(n) cost per keystroke).

## Problem

`DataRowView` does not implement `INotifyPropertyChanged`. The `canFastRefresh` check in `BindingSource_ListChanged` always fails for DataTable edits, causing every cell edit to trigger a full `Data.Bind + InitializeData + Layout.Recalculate`.

## Implementation Steps

### Step 1: Detect DataRowView in ListChanged Handler

In `GridNavigatorHelper.cs`, `BindingSource_ListChanged`:

```csharp
if (e.ListChangedType == ListChangedType.ItemChanged && e.NewIndex >= 0 && e.NewIndex < _grid.Rows.Count)
{
    var row = _grid.Rows[e.NewIndex];
    if (row.RowData is DataRowView drv && e.PropertyDescriptor != null)
    {
        var col = _grid.Columns.FirstOrDefault(c =>
            string.Equals(c.ColumnName, e.PropertyDescriptor.Name, StringComparison.OrdinalIgnoreCase));
        if (col != null)
        {
            var cell = row.Cells.FirstOrDefault(c => c.ColumnIndex == col.Index);
            if (cell != null && drv.DataView?.Table?.Columns.Contains(col.ColumnName) == true)
            {
                cell.CellValue = drv.Row[col.ColumnName];
                _grid.InvalidateRow(e.NewIndex);
                return;  // Skip full rebind
            }
        }
    }
    // Fallback: full rebind
    // ... existing code ...
}
```

## Acceptance Criteria

- [ ] DataTable cell edit updates only that cell visually
- [ ] No full rebind occurs on single cell edit
- [ ] Fallback to full rebind works if column lookup fails
- [ ] Rapid edits do not cause visual glitches
- [ ] Underlying DataRow is updated correctly

## Rollback Plan

Remove the DataRowView fast path block from `BindingSource_ListChanged`, reverting to the existing full rebind behavior.

## Files to Modify

- `Helpers/GridNavigatorHelper.cs`
