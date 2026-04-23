# Phase 5: Schema Change Detection

**Priority:** P1 | **Track:** Data & Binding | **Status:** Pending

## Objective

When a `BindingSource.DataSource` is replaced with a different schema, automatically regenerate columns instead of silently keeping the old column definitions.

## Problem

If the BindingSource's underlying data source is changed after bind (e.g., `bs.DataSource = newTable`), `BindingSource_DataSourceChanged` fires and re-executes full bind — but does not re-run `AutoGenerateColumns()`. The new table's columns are missed if user columns already existed.

## Implementation Steps

### Step 1: Implement DetectSchemaChange

In `GridNavigatorHelper.cs`:

```csharp
private bool DetectSchemaChange(BindingSource bs)
{
    var currentNames = _grid.Columns
        .Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
        .Select(c => c.ColumnName)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    var resolved = _grid.Data.ResolveForSchemaCheck(bs);
    if (resolved == null) return false;

    var newNames = GetColumnNames(resolved);
    return !currentNames.SetEquals(newNames);
}

private HashSet<string> GetColumnNames(object data)
{
    var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    if (data is DataTable dt)
    {
        foreach (DataColumn col in dt.Columns)
            names.Add(col.ColumnName);
    }
    else if (data is IEnumerable enumerable)
    {
        var first = enumerable.GetEnumerator();
        if (first.MoveNext() && first.Current != null)
        {
            foreach (var prop in first.Current.GetType().GetProperties())
                names.Add(prop.Name);
        }
    }
    return names;
}
```

### Step 2: Update DataSourceChanged Handler

```csharp
private void BindingSource_DataSourceChanged(object? sender, EventArgs e)
{
    if (IsUowMode) return;
    bool schemaChanged = DetectSchemaChange(_bindingSource);
    if (schemaChanged)
        _grid.Data.AutoGenerateColumns();
    else
        _grid.Data.Bind(_bindingSource);
    _grid.Data.InitializeData();
    _grid.Layout.Recalculate();
    _grid.SafeInvalidate();
}
```

## Acceptance Criteria

- [ ] Replacing BindingSource DataSource with different schema regenerates columns
- [ ] Replacing with same schema does not regenerate columns
- [ ] System columns (Sel, RowNum, RowID) are preserved
- [ ] User column customizations are lost only when schema actually changes

## Rollback Plan

Revert `BindingSource_DataSourceChanged` to its original behavior (always call `Data.Bind()`).

## Files to Modify

- `Helpers/GridNavigatorHelper.cs`
- `Helpers/GridDataHelper.cs` (add `ResolveForSchemaCheck` if needed)
