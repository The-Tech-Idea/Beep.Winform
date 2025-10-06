# Sorting and Filtering

Built-in sorting
- Click a header to toggle ascending/descending if the column `AllowSort` is true.
- Visual sort indicator arrows show on the sorted column when `GridRenderHelper.ShowSortIndicators` is enabled (default).
- Programmatically: `beepGridPro1.ToggleColumnSort(colIndex)` or `beepGridPro1.SortFilter.Sort(columnName, SortDirection.Ascending)`.

Simple contains filter
- `GridSortFilterHelper.Filter(columnName, contains)` performs a case-insensitive substring filter on the in-memory rows.

Excel-style filter popup
- Two ways to enable:
  1) `beepGridPro1.EnableExcelFilter()` extension: hooks grid mouse and shows `BeepGridFilterPopup` when clicking header area.
  2) Use the adapter-based helper `ExcelFilterHelper.ShowForColumn(adapter, columnIndex, screenLocation)` with `BeepGridProAdapter` if you need zero changes to grid internals.

Popup behavior
- Search box to narrow values.
- Select All checkbox.
- Sort Asc/Desc buttons raising `SortRequested`.
- Clear button raising `ClearRequested` (reset rows via `RefreshGrid()`).
- Apply button raising `FilterApplied` with selected values.

Adapter usage
```csharp
var adapter = new BeepGridProAdapter(
    owner: beepGridPro1,
    getColumns: () => beepGridPro1.Columns.ToList(),
    getRows: () => beepGridPro1.Rows,
    sort: (name, dir) => beepGridPro1.SortFilter.Sort(name, dir),
    clear: () => { beepGridPro1.RefreshGrid(); },
    applyInFilter: (name, vals) => {
        var set = new HashSet<string>(vals?.Select(v => v?.ToString() ?? "") ?? Enumerable.Empty<string>());
        var filtered = beepGridPro1.Rows.Where(r => set.Contains(r.RowData?.GetType().GetProperty(name)?.GetValue(r.RowData)?.ToString() ?? "")).ToList();
        beepGridPro1.Rows.Clear();
        for (int i=0;i<filtered.Count;i++){ filtered[i].DisplayIndex=i; beepGridPro1.Rows.Add(filtered[i]); }
        beepGridPro1.Layout.Recalculate();
        beepGridPro1.Invalidate();
    }
);

// Then show popup for a given header cell location
ExcelFilterHelper.ShowForColumn(adapter, columnIndex, screenPoint);
```
