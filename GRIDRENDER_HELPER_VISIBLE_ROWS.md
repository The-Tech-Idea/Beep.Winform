# GridRenderHelper - Visible Row Calculation Methods

## Overview
GridRenderHelper now provides public methods for calculating visible row capacity and indices. These methods are essential for pagination calculations in navigation painters.

## New Public Methods

### 1. GetVisibleRowCapacity()
```csharp
public int GetVisibleRowCapacity()
```
**Purpose:** Gets the number of rows that can fit in the current viewport (RowsRect height / RowHeight).
This is the "page size" for pagination calculations.

**Returns:** Integer representing how many rows can fit in the visible area (minimum 1, default 10 if layout not ready)

**Usage:**
```csharp
// In a navigation painter
int pageSize = grid.Render.GetVisibleRowCapacity();
int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
```

### 2. GetFirstVisibleRowIndex()
```csharp
public int GetFirstVisibleRowIndex()
```
**Purpose:** Gets the index of the first fully or partially visible row.

**Returns:** Zero-based row index of the first visible row

**Usage:**
```csharp
int firstRow = grid.Render.GetFirstVisibleRowIndex();
```

### 3. GetLastVisibleRowIndex()
```csharp
public int GetLastVisibleRowIndex()
```
**Purpose:** Gets the index of the last visible row (may be partially visible).

**Returns:** Zero-based row index of the last visible row

**Usage:**
```csharp
int lastRow = grid.Render.GetLastVisibleRowIndex();
int visibleRange = lastRow - firstRow + 1;
```

### 4. GetActualVisibleRowCount()
```csharp
public int GetActualVisibleRowCount()
```
**Purpose:** Gets the actual count of visible rows (handles variable row heights).

**Returns:** Actual number of rows currently visible (accounts for partial visibility and variable heights)

**Usage:**
```csharp
int actualVisible = grid.Render.GetActualVisibleRowCount();
// Use when you need precise visible row count with variable row heights
```

## BeepGridPro Property

### VisibleRowCapacity
```csharp
[Browsable(false)]
public int VisibleRowCapacity => Render?.GetVisibleRowCapacity() ?? 10;
```
**Purpose:** Convenient property to access visible row capacity directly from grid instance.

**Usage:**
```csharp
// Easy access from grid
int pageSize = grid.VisibleRowCapacity;

// Use in pagination calculations
int currentPage = (grid.Selection.RowIndex / grid.VisibleRowCapacity) + 1;
int totalPages = (int)Math.Ceiling(grid.Data.Rows.Count / (double)grid.VisibleRowCapacity);
```

## Pagination Calculation Examples

### Example 1: Calculate Current Page
```csharp
public int GetCurrentPage(BeepGridPro grid)
{
    if (grid?.Selection == null) return 1;
    int pageSize = grid.VisibleRowCapacity;
    return (grid.Selection.RowIndex / pageSize) + 1;
}
```

### Example 2: Calculate Total Pages
```csharp
public int GetTotalPages(BeepGridPro grid)
{
    if (grid?.Data?.Rows == null) return 1;
    int totalRecords = grid.Data.Rows.Count;
    int pageSize = grid.VisibleRowCapacity;
    return Math.Max(1, (int)Math.Ceiling(totalRecords / (double)pageSize));
}
```

### Example 3: Calculate Page Range for Buttons
```csharp
public (int startPage, int endPage) GetVisiblePageRange(BeepGridPro grid, int maxVisible = 5)
{
    int currentPage = GetCurrentPage(grid);
    int totalPages = GetTotalPages(grid);
    
    // Center current page in visible range
    int startPage = Math.Max(1, currentPage - maxVisible / 2);
    int endPage = Math.Min(totalPages, startPage + maxVisible - 1);
    
    // Adjust if at end
    if (endPage - startPage < maxVisible - 1)
        startPage = Math.Max(1, endPage - maxVisible + 1);
        
    return (startPage, endPage);
}
```

### Example 4: Navigate to Specific Page
```csharp
public void GoToPage(BeepGridPro grid, int pageNumber)
{
    if (grid == null) return;
    
    int pageSize = grid.VisibleRowCapacity;
    int totalPages = GetTotalPages(grid);
    
    // Clamp to valid range
    pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
    
    // Calculate row index for the start of the page
    int targetRow = (pageNumber - 1) * pageSize;
    targetRow = Math.Max(0, Math.Min(targetRow, grid.Data.Rows.Count - 1));
    
    // Select the first row of the page
    grid.Selection.SelectCell(targetRow, grid.Selection.ColumnIndex);
    grid.Invalidate();
}
```

## Usage in Navigation Painters

### DataTables Painter Example (BEFORE)
```csharp
// ❌ WRONG - Hardcoded page size
int pageSize = 10; 
int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
```

### DataTables Painter Example (AFTER)
```csharp
// ✅ CORRECT - Dynamic page size
int pageSize = grid.VisibleRowCapacity; 
int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
```

### Material Painter Example
```csharp
public override void PaintNavigation(Graphics g, Rectangle navRect, BeepGridPro grid, IBeepTheme theme)
{
    if (grid?.Data?.Rows == null) return;
    
    int totalRecords = grid.Data.Rows.Count;
    int currentRow = grid.Selection?.RowIndex ?? 0;
    int pageSize = grid.VisibleRowCapacity; // Dynamic page size
    
    int currentPage = (currentRow / pageSize) + 1;
    int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
    
    // Draw position: "Page 3 of 18 (Row 27 of 213)"
    string position = $"Page {currentPage} of {totalPages} (Row {currentRow + 1} of {totalRecords})";
    
    // ... draw position string
}
```

## Key Benefits

1. **Dynamic Calculation**: Page size automatically adjusts when grid is resized
2. **Consistent API**: All painters use the same calculation method
3. **Variable Row Heights**: GetActualVisibleRowCount handles variable heights correctly
4. **Easy to Use**: Simple property access via grid.VisibleRowCapacity
5. **Performance**: Cached in GridRenderHelper, efficient calculation

## Implementation Status

### ✅ Completed
- Added `GetVisibleRowCapacity()` to GridRenderHelper
- Added `GetFirstVisibleRowIndex()` to GridRenderHelper
- Added `GetLastVisibleRowIndex()` to GridRenderHelper
- Added `GetActualVisibleRowCount()` to GridRenderHelper
- Added `VisibleRowCapacity` property to BeepGridPro

### ⏳ Next Steps
1. Add shared pagination calculation methods to BaseNavigationPainter
2. Update DataTablesNavigationPainter to use grid.VisibleRowCapacity
3. Update TailwindNavigationPainter to use grid.VisibleRowCapacity
4. Update AGGridNavigationPainter to use grid.VisibleRowCapacity
5. Test pagination with 213 records → should show 18 pages (not 5)

## Testing Scenarios

### Test Case 1: Basic Pagination
- **Setup**: 213 records, 12 rows visible (grid height = 300px, row height = 25px)
- **Expected**: VisibleRowCapacity = 12, TotalPages = 18
- **Verify**: Navigation shows "Page 1 of 18"

### Test Case 2: Grid Resize
- **Action**: Resize grid vertically to show 20 rows
- **Expected**: VisibleRowCapacity = 20, TotalPages = 11
- **Verify**: Pagination recalculates automatically

### Test Case 3: Row Height Change
- **Action**: Change RowHeight from 25 to 30
- **Expected**: VisibleRowCapacity = 10, TotalPages = 22
- **Verify**: Pagination updates correctly

### Test Case 4: Variable Row Heights
- **Setup**: Some rows have custom heights
- **Action**: Use GetActualVisibleRowCount()
- **Expected**: Returns actual count considering variable heights

## Related Files

- `GridRenderHelper.cs` - Contains visible row calculation methods
- `BeepGridPro.cs` - Exposes VisibleRowCapacity property
- `BaseNavigationPainter.cs` - Should contain shared pagination methods (to be added)
- All 12 navigation painters - Should use grid.VisibleRowCapacity instead of hardcoded values

## Notes

- All calculations use integer division with ceiling for page counts
- Minimum page count is always 1 (even with 0 records)
- VisibleRowCapacity minimum is 1 row, default fallback is 10
- GetVisibleRowCount is still private (handles complex variable height calculations internally)
