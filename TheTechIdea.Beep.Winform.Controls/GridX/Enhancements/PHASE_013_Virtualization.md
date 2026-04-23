# Phase 13: Large Dataset Virtualization

**Priority:** P2 | **Track:** Feature Additions | **Status:** Pending

## Objective

Enable handling of very large datasets (100K+ rows) through row and column virtualization. Only visible rows/columns are materialized.

## Implementation Steps

### Step 1: Create Virtual Data Source Interface

```csharp
// Virtualization/IVirtualDataSource.cs
public interface IVirtualDataSource
{
    int VirtualRowCount { get; }
    int VirtualColumnCount { get; }
    object GetCellValue(int rowIndex, string columnName);
    event EventHandler<VirtualDataChangedEventArgs>? DataChanged;
}
```

### Step 2: Create Virtual Data Source Wrapper

```csharp
// Virtualization/GridVirtualDataSource.cs
public class GridVirtualDataSource : IVirtualDataSource
{
    // Wraps existing data source
    // Provides on-demand cell value access
    // Does not materialize all rows upfront
}
```

### Step 3: Create Row Virtualizer

```csharp
// Virtualization/GridRowVirtualizer.cs
public class GridRowVirtualizer
{
    // Manages visible row window
    // Calculates scroll position based on virtual row count
    // Materializes only rows in viewport + buffer
    // Handles variable row heights
}
```

### Step 4: Create Column Virtualizer

```csharp
// Virtualization/GridColumnVirtualizer.cs
public class GridColumnVirtualizer
{
    // Manages visible column window
    // Only renders columns in viewport + buffer
    // Handles horizontal scroll position
}
```

### Step 5: Modify GridRenderHelper

Update rendering pipeline to work with virtualized data:
- Use virtual row count for scroll calculations
- Materialize only visible rows
- Skip off-screen columns

### Step 6: Modify GridScrollHelper

Update scroll handling for virtual positions:
- Calculate scroll range from virtual row count
- Map scroll position to virtual row index
- Handle variable row height estimation

### Step 7: Add Properties to BeepGridPro

```csharp
public bool EnableVirtualization { get; set; }
public int VirtualRowCount { get; set; }
public int RowBuffer { get; set; } = 10;  // Rows to buffer above/below viewport
```

### Step 8: Implement On-Demand Materialization

When scrolling, materialize rows as they enter the viewport buffer:
- Query virtual data source for cell values
- Create `BeepRowConfig` on demand
- Dispose rows that leave the viewport buffer

## Acceptance Criteria

- [ ] 100K rows load without freezing
- [ ] Scrolling is smooth (60fps target)
- [ ] Memory usage stays constant regardless of row count
- [ ] Virtualization works with active filter
- [ ] Virtualization works with grouping
- [ ] Variable row heights work correctly
- [ ] Column virtualization works for wide grids

## Files to Create

- `Virtualization/IVirtualDataSource.cs`
- `Virtualization/GridVirtualDataSource.cs`
- `Virtualization/GridRowVirtualizer.cs`
- `Virtualization/GridColumnVirtualizer.cs`

## Files to Modify

- `Helpers/GridRenderHelper.cs`
- `Helpers/GridRenderHelper.Rendering.cs`
- `Helpers/GridScrollHelper.cs`
- `BeepGridPro.Properties.cs`
