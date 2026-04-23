# Phase 3: ObservableCollection Live Updates

**Priority:** P0 | **Track:** Data & Binding | **Status:** Pending

## Objective

Enable `ObservableCollection<T>` as a DataSource with automatic live updates when items are added, removed, or the collection is cleared.

## Problem

`ObservableCollection<T>` implements `INotifyCollectionChanged` but NOT `IBindingList`. When wrapped by `BindingSource`, mutations are NOT forwarded to `BindingSource.ListChanged`. The grid has no direct `INotifyCollectionChanged` subscription for non-UoW paths. Result: initial data renders, but subsequent mutations are silent.

## Implementation Steps

### Step 1: Add Subscription Field

In `GridDataHelper.cs`:

```csharp
private INotifyCollectionChanged? _subscribedCollectionChanged;
```

### Step 2: Subscribe in Bind()

After `EnsureSystemColumns()`, add:

```csharp
if (resolved is INotifyCollectionChanged incc && resolved is not IBindingList)
{
    if (!ReferenceEquals(_subscribedCollectionChanged, incc))
    {
        if (_subscribedCollectionChanged != null)
            _subscribedCollectionChanged.CollectionChanged -= OnCollectionChanged;
        _subscribedCollectionChanged = incc;
        incc.CollectionChanged += OnCollectionChanged;
    }
}
```

### Step 3: Implement Handler

```csharp
private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
{
    _grid.Data.RefreshRows();
    _grid.Layout.Recalculate();
    _grid.SafeInvalidate();
}
```

### Step 4: Unsubscribe in ClearDataSource()

```csharp
if (_subscribedCollectionChanged != null)
{
    _subscribedCollectionChanged.CollectionChanged -= OnCollectionChanged;
    _subscribedCollectionChanged = null;
}
```

## Acceptance Criteria

- [ ] `ObservableCollection<T>` renders initial data
- [ ] Adding an item updates the grid automatically
- [ ] Removing an item updates the grid automatically
- [ ] Clearing the collection clears the grid
- [ ] No subscription leaks on rebind
- [ ] No double-subscription to same collection

## Rollback Plan

Remove the `_subscribedCollectionChanged` field, subscription logic in `Bind()`, handler, and unsubscribe in `ClearDataSource()`.

## Files to Modify

- `Helpers/GridDataHelper.cs`
