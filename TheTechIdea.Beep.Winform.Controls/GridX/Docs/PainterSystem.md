# BeepGridPro Painter System

## Overview
The BeepGridPro painter system provides a flexible, extensible architecture for rendering grid headers and navigation areas with different visual styles. This system separates visual rendering from business logic and enables custom styling without modifying core grid code.

## Architecture

### Core Interfaces

#### `IPaintGridHeader`
Responsible for rendering column headers with different visual styles.

```csharp
public interface IPaintGridHeader
{
    void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid);
    void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column, int columnIndex, BeepGridPro grid);
    void RegisterHeaderHitAreas(BeepGridPro grid);
    string StyleName { get; }
}
```

#### `IPaintGridNavigation`
Responsible for rendering navigation/toolbar area with different layouts.

```csharp
public interface IPaintGridNavigation
{
    void PaintNavigation(Graphics g, Rectangle navigationRect, BeepGridPro grid);
    void RegisterNavigationHitAreas(BeepGridPro grid);
    void UpdatePageInfo(int currentPage, int totalPages, int totalRecords);
    int GetPreferredHeight();
    string StyleName { get; }
}
```

### Event System

#### `GridPainterEventManager`
Centralized event management for painter interactions.

```csharp
// Register custom event handler
grid.PainterEvents.RegisterEvent("ClickedSortIcon", (sender, args) =>
{
    Console.WriteLine($"Sort clicked on column {args.ColumnIndex}");
});

// Trigger event from custom painter
grid.PainterEvents.TriggerEvent("NavCustomAction", 
    new GridPainterEventArgs { Data = customData });
```

#### Standard Events

**Header Events:**
- `ClickedSortIcon` - Sort icon clicked
- `ClickedFilterIcon` - Filter icon clicked
- `ClickedHeaderCell` - Header cell clicked
- `HeaderCellDoubleClick` - Header double-clicked
- `ResizeColumn` - Column being resized

**Navigation Events:**
- `NavFirst`, `NavPrevious`, `NavNext`, `NavLast` - Navigation
- `NavInsert`, `NavDelete`, `NavSave`, `NavCancel` - CRUD operations
- `NavQuery`, `NavFilter`, `NavPrint` - Utility actions
- `NavPageSizeChanged`, `NavPageJump` - Paging

## Using Painters

### Setting Painters

```csharp
// Set header painter
grid.HeaderPainter = new DefaultGridHeaderPainter();

// Set navigation painter
grid.NavigationPainter = new DefaultGridNavigationPainter();

// Or use custom implementations
grid.HeaderPainter = new MyCustomHeaderPainter();
grid.NavigationPainter = new MinimalNavigationPainter();
```

### Custom Event Handlers

```csharp
// Custom sort handling
grid.PainterEvents.RegisterEvent(GridPainterEvents.ClickedSortIcon, (sender, args) =>
{
    var column = grid.Data.Columns[args.ColumnIndex];
    
    // Custom sort logic
    MyCustomSort(column);
    
    // Mark as handled to prevent default behavior
    args.Handled = true;
});

// Custom filter UI
grid.PainterEvents.RegisterEvent(GridPainterEvents.ClickedFilterIcon, (sender, args) =>
{
    var column = grid.Data.Columns[args.ColumnIndex];
    ShowMyCustomFilterDialog(column);
    args.Handled = true;
});

// Custom navigation action
grid.PainterEvents.RegisterEvent(GridPainterEvents.NavQuery, (sender, args) =>
{
    OpenAdvancedQueryBuilder();
});
```

## Creating Custom Painters

### Custom Header Painter

```csharp
public class MaterialHeaderPainter : IPaintGridHeader
{
    public string StyleName => "Material";

    public void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid)
    {
        // Custom header background with Material Design elevation
        using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
        {
            g.FillRectangle(brush, headerRect);
        }

        // Draw elevation shadow
        using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
        {
            g.FillRectangle(shadowBrush, 
                new Rectangle(headerRect.X, headerRect.Bottom - 4, 
                             headerRect.Width, 4));
        }

        // Paint individual cells
        for (int i = 0; i < grid.Data.Columns.Count; i++)
        {
            var col = grid.Data.Columns[i];
            if (!col.Visible) continue;
            
            if (i < grid.Layout.HeaderCellRects.Length)
            {
                PaintHeaderCell(g, grid.Layout.HeaderCellRects[i], col, i, grid);
            }
        }
    }

    public void PaintHeaderCell(Graphics g, Rectangle cellRect, 
        BeepColumnConfig column, int columnIndex, BeepGridPro grid)
    {
        // Material Design header cell styling
        // ... custom rendering logic
    }

    public void RegisterHeaderHitAreas(BeepGridPro grid)
    {
        // Register interactive areas
        // Use grid.AddHitArea() to define clickable regions
    }
}
```

### Custom Navigation Painter

```csharp
public class MinimalNavigationPainter : IPaintGridNavigation
{
    public string StyleName => "Minimal";

    public void PaintNavigation(Graphics g, Rectangle navRect, BeepGridPro grid)
    {
        // Minimal navigation with only essential controls
        int y = navRect.Top + (navRect.Height - 24) / 2;
        int x = navRect.Left + 8;

        // Draw only essential navigation buttons
        DrawPrevButton(g, new Rectangle(x, y, 24, 24));
        x += 32;
        DrawNextButton(g, new Rectangle(x, y, 24, 24));
    }

    public void RegisterNavigationHitAreas(BeepGridPro grid)
    {
        // Register hit areas for buttons
    }

    public void UpdatePageInfo(int currentPage, int totalPages, int totalRecords)
    {
        // Optional: update page display
    }

    public int GetPreferredHeight() => 36;
}
```

## Painter Styles Gallery

### Default Style
Standard professional layout with full CRUD and navigation controls.
- Full button set
- Page information
- Hover effects
- Sort and filter indicators

### Minimal Style
Clean, compact layout with essential controls only.
- Prev/Next navigation
- No CRUD buttons
- Minimal visual chrome

### Material Style
Google Material Design aesthetics.
- Elevation shadows
- Ripple effects
- Material colors and typography

### Compact Style
Maximum density for limited space.
- Smaller buttons
- Condensed spacing
- Icon-only mode

### Card Style
Card-based header design.
- Rounded corners
- Card shadows
- Spaced layout

## Hit Area System

### Adding Hit Areas

```csharp
// In your painter's RegisterNavigationHitAreas method
grid.AddHitArea("MyButton", buttonRect, buttonControl, () =>
{
    // Action when clicked
    grid.PainterEvents.TriggerEvent("MyCustomEvent");
});
```

### Hit Area Benefits
- Automatic mouse tracking
- Hover state management
- Click handling
- Tooltip support
- No need for child controls

## Best Practices

### 1. Use BeepStyling
Always respect the current BeepGridStyle and theme colors.

```csharp
var theme = BeepThemesManager.GetTheme(grid.Theme);
var backColor = theme.GridHeaderBackColor;
```

### 2. Register All Interactive Areas
Every clickable region should be registered as a hit area.

```csharp
public void RegisterHeaderHitAreas(BeepGridPro grid)
{
    // Register sort icons
    foreach (var sortIcon in _sortIconRects)
    {
        grid.AddHitArea($"Sort_{sortIcon.Key}", sortIcon.Value, null, () =>
        {
            grid.PainterEvents.TriggerEvent(GridPainterEvents.ClickedSortIcon,
                new GridPainterEventArgs { ColumnIndex = sortIcon.Key });
        });
    }
}
```

### 3. Support All Grid States
Handle sticky columns, scrolling, selection, hover, etc.

```csharp
// Respect sticky columns
var stickyColumns = grid.Data.Columns.Where(c => c.Sticked && c.Visible);
// Draw sticky columns separately with proper clipping
```

### 4. Event Integration
Use the event system for all user interactions.

```csharp
// Don't call grid methods directly from painter
// DON'T: grid.MoveNext();

// DO: Trigger event
grid.PainterEvents.TriggerEvent(GridPainterEvents.NavNext);
```

### 5. Stateless Painting
Painters should be stateless when possible. Store state in grid, not painter.

## Examples

### Complete Custom Painter

```csharp
public class CustomHeaderPainter : IPaintGridHeader
{
    private Dictionary<int, Rectangle> _actionRects = new();

    public string StyleName => "Custom";

    public void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid)
    {
        // Custom header styling
        var theme = BeepThemesManager.GetTheme(grid.Theme);
        
        using (var brush = new LinearGradientBrush(
            headerRect, 
            theme.GridHeaderBackColor,
            Color.FromArgb(theme.GridHeaderBackColor.R + 20,
                          theme.GridHeaderBackColor.G + 20,
                          theme.GridHeaderBackColor.B + 20),
            90f))
        {
            g.FillRectangle(brush, headerRect);
        }

        // Paint cells
        for (int i = 0; i < grid.Data.Columns.Count; i++)
        {
            if (!grid.Data.Columns[i].Visible) continue;
            PaintHeaderCell(g, grid.Layout.HeaderCellRects[i], 
                          grid.Data.Columns[i], i, grid);
        }
    }

    public void PaintHeaderCell(Graphics g, Rectangle cellRect, 
        BeepColumnConfig column, int columnIndex, BeepGridPro grid)
    {
        // Custom cell painting
        // ... your rendering code
        
        // Store interactive regions
        var actionRect = new Rectangle(cellRect.Right - 20, cellRect.Top, 20, cellRect.Height);
        _actionRects[columnIndex] = actionRect;
    }

    public void RegisterHeaderHitAreas(BeepGridPro grid)
    {
        foreach (var kvp in _actionRects)
        {
            int colIndex = kvp.Key;
            grid.AddHitArea($"CustomAction_{colIndex}", kvp.Value, null, () =>
            {
                grid.PainterEvents.TriggerEvent("CustomHeaderAction",
                    new GridPainterEventArgs { ColumnIndex = colIndex });
            });
        }
    }
}
```

## See Also
- [BeepGridStyle Documentation](Styling.md)
- [Event System](Events.md)
- [Theme Management](../ThemeManagement/README.md)
