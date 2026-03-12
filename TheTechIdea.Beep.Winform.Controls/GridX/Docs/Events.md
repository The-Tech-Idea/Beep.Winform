# Events and Integration

## Main Public Events

### `RowSelectionChanged`
```csharp
grid.RowSelectionChanged += (s, e) =>
{
    int rowIndex = e.RowIndex;
    var row = e.Row;
};
```

What raises it:
- row checkbox toggle
- header select-all / clear-all checkbox action

What it does not mean:
- it is not fired for every active-cell highlight move

Special case:
- `RowIndex = -1` is used for bulk selection changes

## `SelectionChanged`
```csharp
grid.SelectionChanged += (s, e) =>
{
    var current = grid.CurrentRow;
};
```

This is the lightweight sibling event raised alongside `RowSelectionChanged`.

## `CellValueChanged`
```csharp
grid.CellValueChanged += (s, e) =>
{
    var cell = e.Cell;
};
```

Raised when:
- an editor commit succeeds through the grid edit flow

Important side effect:
- `OnCellValueChanged(...)` also requests auto-size when `AutoSizeTriggerMode` is `OnEditCommit`

## `SaveCalled`
```csharp
grid.SaveCalled += (s, e) =>
{
    // persist or respond to save action
};
```

Raised by:
- the grid save flow

Use this when a screen needs to react after a save request or completion path.

## `ColumnReordered`
```csharp
grid.ColumnReordered += (s, e) =>
{
    int columnIndex = e.ColumnIndex;
    int oldOrder = e.OldDisplayOrder;
    int newOrder = e.NewDisplayOrder;
};
```

Raised when:
- `GridColumnReorderHelper` completes a drag reorder

Restrictions already enforced before the event:
- system columns are excluded
- sticky columns are excluded
- columns with `AllowReorder = false` are excluded

## `GridContextMenuItemSelected`
```csharp
grid.GridContextMenuItemSelected += (s, e) =>
{
    if (e.Action == "export_csv")
    {
        e.Cancel = true;
        e.RefreshGrid = false;
        // custom export
    }
};
```

Useful members:
- `SelectedItem`
- `Action`
- `CurrentRow`
- `SelectedRows`
- `CurrentRowIndex`
- `Cancel`
- `RefreshGrid`

## Filter Events

### `FilterApplied`
```csharp
grid.FilterApplied += (s, e) =>
{
    var config = e.FilterConfiguration;
    int matches = e.MatchingRowCount;
};
```

### `FilterCleared`
```csharp
grid.FilterCleared += (s, e) =>
{
};
```

Important:
- these events belong to the `ActiveFilter` / quick-filter pipeline
- Excel-style popup filtering through `SortFilter` does not raise these events

## Integration Tips
- Use `SelectedRows` or `SelectedRowIndices` when responding to checkbox row selection.
- Use `CurrentRow` when responding to active-cell or focus-driven workflows.
- Use `GridContextMenuItemSelected` to override or extend built-in context menu commands cleanly.
- If you add new editor commit paths, make sure they still route through `OnCellValueChanged(...)`.
