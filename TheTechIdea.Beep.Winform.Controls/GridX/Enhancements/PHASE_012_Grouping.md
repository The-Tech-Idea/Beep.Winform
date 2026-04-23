# Phase 12: Row Grouping

**Priority:** P2 | **Track:** Feature Additions | **Status:** Pending

## Objective

Add row grouping with collapsible group headers and summary rows to BeepGridPro.

## Implementation Steps

### Step 1: Create GroupDescriptor Model

```csharp
// Models/GroupDescriptor.cs
public class GroupDescriptor
{
    public string ColumnName { get; set; }
    public ListSortDirection SortDirection { get; set; }
    public Func<object, string>? DisplayFormatter { get; set; }
    public SummaryType SummaryType { get; set; }
}

public enum SummaryType { None, Count, Sum, Average, Min, Max }
```

### Step 2: Create Grouping Interface

```csharp
// Grouping/IGridGrouper.cs
public interface IGridGrouper
{
    void GroupBy(params GroupDescriptor[] descriptors);
    void Ungroup();
    void ToggleGroupCollapse(int groupIndex);
    bool IsGroupCollapsed(int groupIndex);
    IEnumerable<GroupInfo> GetGroups();
}
```

### Step 3: Create Group Engine

```csharp
// Grouping/GridGroupEngine.cs
public class GridGroupEngine : IGridGrouper
{
    // Core grouping logic:
    // - Partition rows by group key(s)
    // - Maintain collapsed state per group
    // - Integrate with existing sort pipeline
    // - Integrate with existing filter pipeline
}
```

### Step 4: Create Group Header Renderer

```csharp
// Grouping/GridGroupHeaderRenderer.cs
public class GridGroupHeaderRenderer
{
    // Paint group header rows with:
    // - Collapse/expand indicator
    // - Group key value
    // - Row count
    // - Summary value (if configured)
}
```

### Step 5: Create Group Summary Row

```csharp
// Grouping/GridGroupSummaryRow.cs
public class GridGroupSummaryRow
{
    // Calculate and display summary values per group
    // Support: Count, Sum, Average, Min, Max
}
```

### Step 6: Add Public Methods to BeepGridPro

```csharp
public void GroupBy(params GroupDescriptor[] descriptors);
public void Ungroup();
public void ToggleGroupCollapse(int groupIndex);
public bool IsGrouped { get; }
public IEnumerable<GroupInfo> Groups { get; }
```

### Step 7: Integrate with Input Handling

- Click on group header toggles collapse/expand
- Group headers are not selectable as data rows
- Keyboard navigation skips collapsed groups

### Step 8: Integrate with Sort/Filter

- Grouping applies after sorting (sort within groups)
- Filtering applies before grouping (filter then group)
- Collapsed groups are excluded from scroll calculations

## Acceptance Criteria

- [ ] Group by single column works
- [ ] Group by multiple columns works (nested groups)
- [ ] Collapse/expand groups works
- [ ] Summary rows show correct aggregates
- [ ] Grouping works with active filter
- [ ] Grouping works with active sort
- [ ] Collapsed groups excluded from scroll range
- [ ] Group headers render correctly in all styles

## Files to Create

- `Models/GroupDescriptor.cs`
- `Grouping/IGridGrouper.cs`
- `Grouping/GridGroupEngine.cs`
- `Grouping/GridGroupHeaderRenderer.cs`
- `Grouping/GridGroupSummaryRow.cs`

## Files to Modify

- `BeepGridPro.cs` (add grouping methods)
- `Helpers/GridRenderHelper.Rendering.cs` (render group headers)
- `Helpers/GridInputHelper.cs` (handle group header clicks)
- `Helpers/GridScrollHelper.cs` (account for group rows)
