# Phase 8: Dead Code Cleanup

**Priority:** P2 | **Track:** Data & Binding | **Status:** Pending

## Objective

Remove dead code and fix null-safety issues in the data binding pipeline.

## Problem

1. The second `if (resolved is BindingSource bs)` check inside `GetEffectiveEnumerableWithSchema()` is dead code — `ResolveDataForBinding()` already strips `BindingSource`.
2. `ResolveDataForBinding()` can return null when `BindingSource.List` is null AND `BindingSource.DataSource` is null, potentially causing NullReferenceException.

## Implementation Steps

### Step 1: Remove Dead Code

In `GridDataHelper.cs`, `GetEffectiveEnumerableWithSchema()`:

Remove or comment out the second BindingSource check:

```csharp
// REMOVED: Dead code — ResolveDataForBinding() already strips BindingSource
// if (resolved is BindingSource bs)
// {
//     resolved = bs.List ?? bs.DataSource;
// }
```

### Step 2: Fix Null Case in ResolveDataForBinding

```csharp
private object? ResolveDataForBinding(object? data)
{
    if (data is BindingSource bs)
    {
        var list = bs.List;
        if (list != null) return list;
        var ds = bs.DataSource;
        if (ds != null) return ds;
        // Return the BindingSource itself so enumerable path gets empty enumerable
        return bs;
    }
    return data;
}
```

### Step 3: Review Unused Code

- Review all `#if` / `#region` blocks for unused code
- Remove any commented-out blocks that are no longer needed
- Verify no regressions after cleanup

## Acceptance Criteria

- [ ] No dead BindingSource check in `GetEffectiveEnumerableWithSchema()`
- [ ] Null case in `ResolveDataForBinding()` handled safely
- [ ] All existing DataSource types still work
- [ ] No NullReferenceException in edge cases

## Rollback Plan

Restore the removed code blocks from version control.

## Files to Modify

- `Helpers/GridDataHelper.cs`
