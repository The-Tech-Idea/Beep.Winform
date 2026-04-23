# Phase 7: Deduplicate Position/Current Events

**Priority:** P1 | **Track:** Data & Binding | **Status:** Pending

## Objective

Eliminate redundant double-selection updates when navigating through a BindingSource-bound grid.

## Problem

`BindingSource_CurrentChanged` and `BindingSource_PositionChanged` do identical selection sync work. Both fire on every navigation step, causing double repaint and potential selection flicker.

## Implementation Steps

### Step 1: Create Unified Method

In `GridNavigatorHelper.cs`:

```csharp
private void SyncSelectionFromPosition()
{
    if (_bindingSource == null) return;
    int position = _bindingSource.Position;
    if (position >= 0 && position < _grid.Rows.Count)
    {
        if (!_grid.Selection.HasSelection || _grid.Selection.RowIndex != position)
            _grid.SelectCell(position, _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0);
    }
}
```

### Step 2: Update Event Handlers

```csharp
private void BindingSource_CurrentChanged(object? sender, EventArgs e)  => SyncSelectionFromPosition();
private void BindingSource_PositionChanged(object? sender, EventArgs e) => SyncSelectionFromPosition();
```

## Acceptance Criteria

- [ ] Navigation through BindingSource updates selection exactly once per step
- [ ] No selection flicker during navigation
- [ ] Keyboard navigation works correctly
- [ ] Programmatic position changes work correctly

## Rollback Plan

Revert the two event handlers to their original separate implementations.

## Files to Modify

- `Helpers/GridNavigatorHelper.cs`
