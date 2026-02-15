# Sorting and Filtering

Built-in sorting
- Click a header to toggle ascending/descending if the column `AllowSort` is true.
- Visual sort indicator arrows show on the sorted column when `GridRenderHelper.ShowSortIndicators` is enabled (default).
- Programmatically: `beepGridPro1.ToggleColumnSort(colIndex)` or `beepGridPro1.SortFilter.Sort(columnName, SortDirection.Ascending)`.

Simple contains filter
- `GridSortFilterHelper.Filter(columnName, contains)` performs a case-insensitive substring filter on the in-memory rows.

Top filter panel
- The modern top filter panel is shown by default. Toggle it with `ShowTopFilterPanel`.
- Set `GridTitle` to display a custom title in the toolbar.
- In `AGGrid` and `DataTables` navigation styles, the toolbar includes Search, Filters, active filter count, and Clear action buttons.
- Filter panel painter selection is now synchronized with `GridStyle` presets via factory mapping.
- Style-specific painter classes (`Material`, `Bootstrap`, `Fluent`, `AntDesign`, `Compact`, `Minimal`, `Telerik`, `Tailwind`) provide dedicated visual tokens.
- Clicking a filter chip/tag opens an inline criterion popup editor (operator + value/value2 + case-sensitive) that updates `ActiveFilter` directly.

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
