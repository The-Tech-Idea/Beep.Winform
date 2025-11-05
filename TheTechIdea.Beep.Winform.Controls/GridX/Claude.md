# Claude.md - AI Assistant Guide for BeepGridPro

## Purpose

This document helps AI assistants (like Claude) understand the BeepGridPro codebase structure, architectural decisions, and development patterns. Use this as a quick reference when modifying or extending BeepGridPro functionality.

## Quick Architecture Overview

```
BeepGridPro = Main Control (inherits BaseControl)
    ├── Helpers (21 classes) - Business logic, state, calculations
    ├── Painters (17 classes) - Visual rendering strategies
    ├── Layouts (17 classes) - Structural presets
    ├── Models - Data structures (in parent Controls.Models)
    ├── Adapters - Data source adapters (future)
    └── Filters - Filter implementations (future)
```

## Key Architectural Patterns

### 1. Helper Pattern (not Dependency Injection)
- Each helper is instantiated directly in BeepGridPro constructor
- Helpers reference `_grid` (the parent BeepGridPro instance)
- Helpers call each other via `_grid.HelperName`
- **No IoC container**, **no interfaces** for helpers

```csharp
// In BeepGridPro constructor:
Data = new GridDataHelper(this);
Render = new GridRenderHelper(this);
Selection = new GridSelectionHelper(this);

// In helpers:
internal class GridSelectionHelper
{
    private readonly BeepGridPro _grid;
    public GridSelectionHelper(BeepGridPro grid) => _grid = grid;
    
    public void SelectCell(int row, int col)
    {
        // Access other helpers via _grid
        _grid.Data.Rows[row].IsSelected = true;
        _grid.Render.InvalidateRow(row);
    }
}
```

### 2. Painter Pattern (Strategy Pattern)
- Painters implement interfaces (INavigationPainter, IPaintGridHeader)
- Created by factories (NavigationPainterFactory, HeaderPainterFactory)
- Can be swapped at runtime
- **Stateless** - receive all context as parameters

```csharp
// Creating a painter
var painter = NavigationPainterFactory.CreatePainter(navigationStyle.Material);

// Using a painter
painter.PaintNavigation(g, bounds, grid, theme);
```

### 3. Layout Preset Pattern
- Implements IGridLayoutPreset interface
- Configures grid properties via `Apply(BeepGridPro grid)`
- Applied once, not continuously called
- **Structural only** - no colors (colors come from themes)

### 4. Owner-Drawn Everything
- No child controls for grid cells, headers, or navigation
- Everything drawn in `DrawContent(Graphics g)`
- Hit testing via rectangles stored in helpers
- Performance-optimized with double buffering

## Critical Code Paths

### Rendering Pipeline

```
OnPaint(PaintEventArgs e)
    └── DrawContent(Graphics g)  [overridden from BaseControl]
        ├── UpdateDrawingRect()
        ├── Layout.EnsureCalculated()
        │   └── GridLayoutHelper.Recalculate()
        │       ├── Calculate HeaderRect
        │       ├── Calculate RowsRect  
        │       ├── Calculate NavigatorRect
        │       └── LayoutCells() - All cell rectangles
        │
        ├── Render.Draw(g)
        │   ├── Draw background
        │   ├── Draw visible rows
        │   │   └── For each cell: Draw background + content
        │   ├── GridColumnHeadersPainterHelper.DrawColumnHeaders(g)
        │   └── GridNavigationPainterHelper.DrawNavigatorArea(g)
        │
        └── ScrollBars.DrawScrollBars(g)
```

### Input Handling

```
Mouse Click
    └── GridInputHelper.HandleMouseDown(e)
        ├── Check if in HeaderRect
        │   ├── Hit sort icon → SortFilter.Sort()
        │   ├── Hit filter icon → Dialog.ShowFilterDialog()
        │   ├── Hit select-all → Selection.SelectAll()
        │   └── Hit column border → Start resize
        │
        ├── Check if in cell
        │   ├── Hit checkbox → Toggle row selection
        │   └── Hit cell → Selection.SelectCell()
        │
        ├── Check if in NavigatorRect
        │   ├── Check hit areas from painters
        │   └── Execute action (MoveFirst, Save, etc.)
        │
        └── Check if in scrollbar
            └── ScrollBars.HandleMouseDown()
```

### Data Binding

```
DataSource property set
    └── GridDataHelper.Bind(dataSource)
        ├── Detect data type (BindingList, List, DataTable, etc.)
        ├── AutoGenerateColumns() if needed
        ├── EnsureSystemColumns() (checkbox, row number, row ID)
        ├── InitializeData() - Create BeepRowConfig for each row
        └── Subscribe to IBindingList.ListChanged
            └── On change: RefreshRows() + Invalidate()
```

### Scrolling

```
MouseWheel
    └── GridScrollBarsHelper.HandleMouseWheel(e)
        └── GridScrollHelper.SetVerticalOffset(newOffset)
            ├── Clamp to valid range
            ├── Update scroll position
            ├── GridLayoutHelper.Recalculate() - Recalc visible cells
            └── Invalidate() - Trigger redraw
```

## Important Classes and Their Responsibilities

### BeepGridPro (Main Control)
- **Purpose**: Orchestrator, property container, event source
- **Owns**: All helpers, theme, current painters
- **Does NOT**: Render directly, calculate layout, manage data
- **Key Properties**: DataSource, Theme, GridStyle, LayoutPreset, NavigationStyle
- **Key Events**: RowSelectionChanged, CellValueChanged, SaveCalled

### GridLayoutHelper
- **Purpose**: Calculate ALL rectangles (headers, cells, navigation, scrollbars)
- **Key Method**: `Recalculate()` - Called on resize, data change, scroll
- **Key Properties**: HeaderRect, RowsRect, NavigatorRect, HeaderCellRects[]
- **Important**: `IsCalculating` flag prevents recursive recalculation

### GridDataHelper
- **Purpose**: Manage data binding, columns, rows
- **Key Collections**: `Columns` (BeepGridColumnConfigCollection), `Rows` (BindingList<BeepRowConfig>)
- **Key Methods**: `Bind()`, `AutoGenerateColumns()`, `RefreshRows()`, `EnsureSystemColumns()`
- **Important**: Columns are typed configs, Rows contain cells

### GridRenderHelper
- **Purpose**: Coordinate all drawing operations
- **Key Method**: `Draw(Graphics g)` - Main rendering entry point
- **Delegates to**: GridColumnHeadersPainterHelper, GridNavigationPainterHelper
- **Important**: Handles visible row calculation, cell content rendering

### GridSelectionHelper
- **Purpose**: Track selected cells/rows, manage selection state
- **Key Properties**: `RowIndex`, `ColumnIndex`, `HasSelection`
- **Key Methods**: `SelectCell()`, `SelectRow()`, `ClearSelection()`
- **Important**: Fires RowSelectionChanged event

### GridInputHelper
- **Purpose**: Route mouse/keyboard input to appropriate handlers
- **Key Methods**: `HandleMouseDown()`, `HandleMouseMove()`, `HandleKeyDown()`
- **Important**: Hit testing logic lives here

### GridScrollHelper
- **Purpose**: Manage scroll position and visible range
- **Key Properties**: `VerticalOffset`, `HorizontalOffset`, `FirstVisibleRowIndex`
- **Key Methods**: `SetVerticalOffset()`, `SetHorizontalOffset()`
- **Important**: Coordinates with GridLayoutHelper for recalculation

### GridScrollBarsHelper
- **Purpose**: Render custom scrollbars, handle scrollbar interaction
- **Key Methods**: `DrawScrollBars()`, `HandleMouseDown()`, `UpdateBars()`
- **Important**: Custom drawn, not WinForms ScrollBar controls

### GridNavigationPainterHelper
- **Purpose**: Render navigation bar using painters
- **Key Properties**: `UsePainterNavigation`, `NavigationStyle`
- **Key Methods**: `DrawNavigatorArea()`, `GetRecommendedNavigatorHeight()`
- **Important**: Supports both painter-based and legacy button-based navigation

### GridColumnHeadersPainterHelper
- **Purpose**: Render column headers with sort/filter indicators
- **Key Methods**: `DrawColumnHeaders()`, `DrawHeaderCell()`
- **Important**: Handles sticky column rendering, hover effects

### GridNavigatorHelper
- **Purpose**: Navigation actions (First, Previous, Next, Last, CRUD)
- **Key Methods**: `MoveFirst()`, `MoveNext()`, `Save()`, `InsertNew()`
- **Important**: Integrates with IUnitOfWork pattern

## Data Structures

### BeepGridColumnConfigCollection
- Extends `BindingList<BeepColumnConfig>`
- Fired ListChanged events
- Supports design-time editing

### BeepColumnConfig
- **Key Properties**: 
  - `ColumnName`, `ColumnCaption`
  - `Width`, `Visible`, `Sticked` (frozen)
  - `IsSorted`, `SortDirection`, `ShowSortIcon`
  - `ShowFilterIcon`, `IsFiltered`
  - `HeaderTextAlignment`, `CellTextAlignment`
  - `IsSelectionCheckBox`, `IsRowNumColumn`, `IsRowID`

### BeepRowConfig
- **Key Properties**:
  - `Cells` (List<BeepCellConfig>)
  - `IsSelected`, `IsHovered`
  - `Height` (per-row custom height)
  - `RowCheckRect` (checkbox hit area)
  - `OriginalItem` (reference to data object)

### BeepCellConfig
- **Key Properties**:
  - `Value` (object)
  - `FormattedValue` (string for display)
  - `Rect` (Rectangle for hit testing)
  - `Column` (reference to BeepColumnConfig)

## Common Tasks

### Adding a New Helper

1. Create class in `/Helpers` folder:
```csharp
internal class GridMyNewHelper
{
    private readonly BeepGridPro _grid;
    
    public GridMyNewHelper(BeepGridPro grid)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
    }
    
    public void DoSomething()
    {
        // Access other helpers via _grid
        var rows = _grid.Data.Rows;
        _grid.Selection.SelectCell(0, 0);
    }
}
```

2. Add property to BeepGridPro:
```csharp
internal GridMyNewHelper MyNewHelper { get; }
```

3. Initialize in constructor:
```csharp
MyNewHelper = new GridMyNewHelper(this);
```

### Adding a New Navigation Painter

1. Create painter class in `/Painters`:
```csharp
public class MyNavigationPainter : BaseNavigationPainter
{
    public override navigationStyle Style => navigationStyle.Custom;
    public override int RecommendedHeight => 40;
    public override int RecommendedMinWidth => 400;
    
    public override void PaintNavigation(Graphics g, Rectangle bounds, 
        BeepGridPro grid, IBeepTheme theme)
    {
        // 1. Clear hit areas
        grid.ClearHitList();
        
        // 2. Draw background
        // 3. Calculate layout
        // 4. Paint buttons with hit areas
        // 5. Paint indicators
    }
    
    public override NavigationLayout CalculateLayout(Rectangle bounds, 
        int totalRecords, bool showCrudButtons)
    {
        // Calculate button positions
        return new NavigationLayout { ... };
    }
    
    // Implement other abstract methods...
}
```

2. Add to enum in `/Painters/enums.cs`:
```csharp
public enum navigationStyle
{
    // ... existing
    Custom
}
```

3. Register in factory:
```csharp
public static INavigationPainter CreatePainter(navigationStyle style)
{
    return style switch
    {
        // ... existing
        navigationStyle.Custom => new MyNavigationPainter(),
        _ => new StandardNavigationPainter()
    };
}
```

### Adding a New Layout Preset

1. Create layout class in `/Layouts`:
```csharp
public sealed class MyLayoutHelper : IGridLayoutPreset
{
    public void Apply(BeepGridPro grid)
    {
        if (grid == null) return;
        
        // Dimensions
        grid.RowHeight = 26;
        grid.ColumnHeaderHeight = 30;
        
        // Visual properties
        grid.Render.ShowGridLines = true;
        grid.Render.ShowRowStripes = true;
        // ... etc
        
        // Alignment
        LayoutCommon.ApplyAlignmentHeuristics(grid);
    }
}
```

2. Add to enum:
```csharp
public enum GridLayoutPreset
{
    // ... existing
    Custom
}
```

3. Register in BeepGridPro:
```csharp
public void ApplyLayoutPreset(GridLayoutPreset preset)
{
    IGridLayoutPreset impl = preset switch
    {
        // ... existing
        GridLayoutPreset.Custom => new MyLayoutHelper(),
        _ => new DefaultTableLayoutHelper()
    };
    this.ApplyLayoutPreset(impl);
}
```

### Modifying Rendering

**Rule**: Never modify `DrawContent` directly. Instead:

1. **For cells**: Modify `GridRenderHelper.DrawCell()`
2. **For headers**: Modify `GridColumnHeadersPainterHelper.DrawHeaderCell()`
3. **For navigation**: Create new painter or modify existing painter
4. **For rows**: Modify `GridRenderHelper.DrawRow()`

### Performance Optimization

1. **Use BeginUpdate/EndUpdate** for bulk changes:
```csharp
grid.BeginUpdate();
try
{
    // Multiple operations
    grid.DataSource = newData;
    grid.LayoutPreset = GridLayoutPreset.Dense;
    grid.AutoResizeColumnsToFitContent();
}
finally
{
    grid.EndUpdate(); // Single invalidation
}
```

2. **Avoid Layout.Recalculate() in tight loops**
3. **Cache Graphics objects** (brushes, pens, fonts) when possible
4. **Use visible row virtualization** - already implemented

## Common Pitfalls

### 1. Recursive Recalculation
**Problem**: Calling `Layout.Recalculate()` triggers events that call `Layout.Recalculate()` again
**Solution**: Check `Layout.IsCalculating` flag

```csharp
private void SafeRecalculate()
{
    if (Layout != null && !Layout.IsCalculating)
        Layout.Recalculate();
}
```

### 2. Modifying Collections During Enumeration
**Problem**: Modifying `Columns` or `Rows` while iterating
**Solution**: Use `.ToList()` or iterate backwards

```csharp
// BAD
foreach (var row in grid.Rows)
    grid.Rows.Remove(row); // Exception!

// GOOD
for (int i = grid.Rows.Count - 1; i >= 0; i--)
    grid.Rows.RemoveAt(i);
```

### 3. Not Invalidating After Changes
**Problem**: Changes don't appear on screen
**Solution**: Call `Invalidate()` after state changes

```csharp
public void SetRowHeight(int height)
{
    RowHeight = height;
    Layout.Recalculate();
    Invalidate(); // MUST call this
}
```

### 4. Theme Colors vs Hard-Coded Colors
**Problem**: Hard-coded colors don't adapt to theme changes
**Solution**: Always use theme colors

```csharp
// BAD
using (var brush = new SolidBrush(Color.White))

// GOOD
using (var brush = new SolidBrush(theme.GridBackColor))
```

### 5. Rectangle.Empty Checks
**Problem**: Drawing with empty rectangles causes artifacts
**Solution**: Always check before drawing

```csharp
if (cellRect.IsEmpty || cellRect.Width <= 0 || cellRect.Height <= 0)
    return; // Don't draw
```

## Testing Guidelines

### Unit Test Helpers
```csharp
[TestMethod]
public void Selection_SelectCell_ShouldUpdateRowIndex()
{
    var grid = new BeepGridPro();
    grid.DataSource = GetTestData();
    
    grid.Selection.SelectCell(2, 1);
    
    Assert.AreEqual(2, grid.Selection.RowIndex);
    Assert.AreEqual(1, grid.Selection.ColumnIndex);
}
```

### Integration Test with Rendering
```csharp
[TestMethod]
public void Apply Layout_ShouldRecalculateAndInvalidate()
{
    var grid = new BeepGridPro();
    grid.DataSource = GetTestData();
    bool invalidated = false;
    grid.Invalidated += (s, e) => invalidated = true;
    
    grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
    
    Assert.IsTrue(invalidated);
    Assert.IsTrue(grid.Layout.HeaderRect.Height > 0);
}
```

## Debugging Tips

### 1. Enable Diagnostic Logging
Uncomment `Console.WriteLine` statements in:
- `GridLayoutHelper.Recalculate()`
- `GridRenderHelper.Draw()`
- `GridNavigationPainterHelper.DrawPainterNavigation()`

### 2. Visualize Rectangles
```csharp
// In DrawContent, after all drawing:
using (var pen = new Pen(Color.Red, 2))
{
    g.DrawRectangle(pen, Layout.HeaderRect);
    g.DrawRectangle(pen, Layout.RowsRect);
    g.DrawRectangle(pen, Layout.NavigatorRect);
}
```

### 3. Breakpoint Locations
- `GridInputHelper.HandleMouseDown` - See where clicks are going
- `GridLayoutHelper.LayoutCells` - See cell rectangle calculation
- `GridDataHelper.Bind` - See data binding process
- `GridRenderHelper.Draw` - See what's being rendered

### 4. Check Hit Areas
```csharp
// After navigation painting
var hitAreas = grid.GetHitAreas(); // Not public, but useful in debug
foreach (var area in hitAreas)
{
    Console.WriteLine($"{area.Name}: {area.Rect}");
}
```

## Related Files Reference

- **Main Control**: `GridX/BeepGridPro.cs`
- **Helpers**: `GridX/Helpers/*.cs` (21 files)
- **Painters**: `GridX/Painters/*.cs` (17 files)
- **Layouts**: `GridX/Layouts/*.cs` (17 files)
- **Models**: `../Models/BeepColumnConfig.cs`, `BeepRowConfig.cs`, etc.
- **Enums**: `../Models/enums.cs`, `Painters/enums.cs`, `Layouts/GridLayoutPreset.cs`

## Architecture Diagrams

See `README.md` for detailed flow diagrams of:
- Rendering Pipeline
- Layout Calculation Flow
- Input Handling Flow
- Style Application Flow

## Next Steps for AI Implementation

When implementing the enhancement plan:
1. Start with Phase 1 (Header Painters)
2. Create base class first, then 2-3 example implementations
3. Test thoroughly before moving to Phase 2
4. Maintain backward compatibility at all times
5. Update this Claude.md as architecture evolves

## Questions to Ask When Making Changes

1. **Does this change affect layout?** → Call `Layout.Recalculate()`
2. **Does this change affect rendering?** → Call `Invalidate()`
3. **Does this change affect data?** → Update `GridDataHelper`
4. **Does this change affect selection?** → Update `GridSelectionHelper`
5. **Is this theme-dependent?** → Use theme colors, not hard-coded
6. **Is this backward compatible?** → Keep existing API working
7. **Is this testable?** → Write unit test first

Remember: BeepGridPro is a high-performance, owner-drawn control. Every change should consider rendering performance and maintain the separation of concerns between helpers.
