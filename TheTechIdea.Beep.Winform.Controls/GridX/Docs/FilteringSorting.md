# Sorting and Filtering

## The Important Split
`BeepGridPro` currently exposes two different filtering systems.

## 1. Public `ActiveFilter` Pipeline
Owned by `BeepGridPro.Filtering.cs`.

### Main API
```csharp
grid.ActiveFilter = config;
grid.ShowAdvancedFilterDialog();
grid.ApplyQuickFilter("smith");
grid.ClearFilter();
grid.AddFilterCriterion(criterion);
grid.RemoveFilterCriterion("LastName");
```

### How it works
- Stores a `FilterConfiguration` in `ActiveFilter`.
- Evaluates rows through `FilterEngine<ExpandoObject>`.
- Sets `BeepRowConfig.IsVisible` for matching/non-matching rows.
- Raises:
  - `FilterApplied`
  - `FilterCleared`

### Advanced filter dialog
`ShowAdvancedFilterDialog()`:
- opens `BeepiFormPro`
- hosts a `BeepFilter`
- builds available fields from visible non-system columns
- writes the chosen `FilterConfiguration` back into `ActiveFilter`

### Quick filter
`ApplyQuickFilter(searchText, columnName)`:
- clears the filter if the text is empty
- searches one column or all columns
- stores a lightweight `FilterConfiguration` for display/state
- updates row visibility directly

## 2. Helper `SortFilter` Pipeline
Owned by `GridSortFilterHelper.cs`.

### Internal API
This path is implemented by the internal `GridSortFilterHelper`, not by a public consumer-facing property on `BeepGridPro`.

### How it works
Sort path:
1. commit any pending edit
2. try source-level sort through `IBindingList.ApplySort(...)`
3. try `BindingSource.Sort`
4. try reordering the underlying list when possible
5. in UOW mode, stop before local row reorder if source sort fails
6. otherwise reorder local rows

Filter path:
1. commit any pending edit
2. build contains-filter and in-filter dictionaries
3. try source-level filtering through `IBindingListView.Filter`
4. try `BindingSource.Filter`
5. fall back to local `row.IsVisible`

### Public event implication
This helper path does not raise the public `FilterApplied` / `FilterCleared` events from `BeepGridPro`.

## Header Sorting

### User interaction
- clicking a sortable header toggles sort
- clicking a sort icon toggles sort
- `ToggleColumnSort(columnIndex)` flips between ascending and descending

### Column state updated
`ToggleColumnSort(...)`:
- clears previous `IsSorted` / `ShowSortIcon` flags
- sets the new column's sort state
- calls the internal `GridSortFilterHelper.Sort(...)`

## Top Filter Panel

The top panel is shown when:
```csharp
grid.ShowTopFilterPanel = true;
```

Key behaviors:
- search action opens/activates inline quick search
- advanced action opens `ShowAdvancedFilterDialog()`
- clear-all clears both `SortFilter` helper filters and `ActiveFilter` when needed
- per-column chip click opens `ShowInlineCriterionEditor(...)`
- per-column clear icon clears that column's criterion and helper filter state

Important note:
- when the top filter panel is shown, header filter icons are intentionally suppressed during rendering

## Inline Quick Search

### Controls used
- `BeepComboBox` for column choice
- `BeepTextBox` for search text

### Trigger points
```csharp
grid.ShowSearchDialog();
```

### Behavior
- if already live, focuses the text box
- if not live, activates and positions the real controls over the painted placeholders
- choosing `All Columns` searches across all visible data columns
- typing calls `ApplyQuickFilter(...)`
- pressing `Escape` hides the live quick-search control surface

## Excel-Style Popup Filtering

Enable the extension:
```csharp
grid.EnableExcelFilter();
```

Runtime note:
- `BeepGridPro` already calls `EnableExcelFilter()` in its constructor outside design mode
- calling it manually is still safe if you want the hookup to be explicit in a screen

This hooks mouse clicks and shows `BeepGridFilterPopup` for header interactions.

Popup actions map internally to:
- `SortRequested` -> `GridSortFilterHelper.Sort(...)`
- `ClearRequested` -> `GridSortFilterHelper.ClearFilters()`
- `FilterApplied` -> `GridSortFilterHelper.FilterIn(...)`

## Saving and Loading Filter Configurations

Available only for the `ActiveFilter` pipeline:
```csharp
grid.SaveFilterConfiguration(path);
grid.LoadFilterConfiguration(path);
var saved = grid.GetSavedFilterConfigurations(folder);
```

These serialize `FilterConfiguration` as JSON.

## Practical Guidance
- Use the `ActiveFilter` pipeline when you want advanced dialog-driven filtering and public filter events.
- Use the `SortFilter` pipeline when you want header popup filtering or binding-source-backed filtering.
- Avoid mixing both pipelines in the same user flow unless you also define how the UI resolves conflicting state.
