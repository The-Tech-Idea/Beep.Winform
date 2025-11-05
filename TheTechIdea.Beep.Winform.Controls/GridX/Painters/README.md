# Painters - Visual Rendering System

## Overview

The Painters folder contains all visual rendering implementations for BeepGridPro, primarily focused on navigation bar painting. The painter pattern allows runtime style switching and makes it easy to add new visual styles without modifying core grid code.

## Architecture

### Painter Pattern Benefits

1. **Separation of Concerns**: Visual rendering is isolated from business logic
2. **Runtime Flexibility**: Switch styles dynamically without recreating controls
3. **Easy Extension**: Add new styles by implementing interfaces
4. **Theme Integration**: All painters receive theme information for consistent styling
5. **Testability**: Painters can be tested in isolation

## Core Interfaces

### INavigationPainter

The primary interface for navigation bar painters. All navigation painters must implement this interface.

```csharp
public interface INavigationPainter
{
    navigationStyle Style { get; }           // Style identifier
    int RecommendedHeight { get; }           // Recommended height for this style
    int RecommendedMinWidth { get; }         // Minimum width needed
    
    // Main painting method
    void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme);
    
    // Paint individual elements
    void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
        NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme);
    void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
        int totalRecords, IBeepTheme theme);
    
    // Layout calculation
    NavigationLayout CalculateLayout(Rectangle availableBounds, int totalRecords, 
        bool showCrudButtons);
    
    // Helper methods
    string GetButtonContent(NavigationButtonType buttonType);
    string GetButtonTooltip(NavigationButtonType buttonType);
}
```

### IPaintGridHeader

Interface for column header painters (future implementation - currently integrated in helper).

```csharp
public interface IPaintGridHeader
{
    void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid);
    void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column, 
        int columnIndex, BeepGridPro grid);
    string StyleName { get; }
    void RegisterHeaderHitAreas(BeepGridPro grid);
}
```

### IPaintGridNavigation

Future interface for grid-level navigation painting strategies.

## Navigation Painters

### 1. StandardNavigationPainter
**Style**: `navigationStyle.Standard`  
**Height**: 32px  
**Characteristics**:
- Simple button-based layout
- Left: CRUD buttons (Add, Delete, Save, Cancel)
- Center: Navigation buttons with position indicator
- Right: Utility buttons (Filter, Export)
- Minimal styling, functional focus

### 2. MaterialNavigationPainter
**Style**: `navigationStyle.Material`  
**Height**: 56px  
**Characteristics**:
- Material Design principles
- Flat design with subtle shadows
- Icon-based circular buttons
- Ripple effects on hover/click
- Accent colors for primary actions
- Elevation and depth

### 3. BootstrapNavigationPainter
**Style**: `navigationStyle.Bootstrap`  
**Height**: 40px  
**Characteristics**:
- Bootstrap 5 inspired
- Button groups with borders
- Primary/secondary/danger color scheme
- Rounded corners (4px radius)
- Consistent spacing (8px)

### 4. TailwindNavigationPainter
**Style**: `navigationStyle.Tailwind`  
**Height**: 44px  
**Characteristics**:
- Tailwind CSS utility-first approach
- Clean, modern aesthetic
- Subtle shadows and borders
- Ring focus indicators
- Transition effects

### 5. FluentNavigationPainter
**Style**: `navigationStyle.Fluent`  
**Height**: 48px  
**Characteristics**:
- Microsoft Fluent Design
- Acrylic-like backgrounds
- Reveal effects on hover
- Depth and lighting
- Smooth animations

### 6. AGGridNavigationPainter
**Style**: `navigationStyle.AGGrid`  
**Height**: 36px  
**Characteristics**:
- Enterprise grid focused
- Compact, efficient layout
- Pagination controls prominent
- Page size dropdown
- "Go to page" input field
- Professional appearance

### 7. DataTablesNavigationPainter
**Style**: `navigationStyle.DataTables`  
**Height**: 48px  
**Characteristics**:
- jQuery DataTables inspired
- "Show X entries" dropdown
- "Showing 1 to 10 of 100 entries" text
- Simple numbered pagination
- Search box integration

### 8. AntDesignNavigationPainter
**Style**: `navigationStyle.AntDesign`  
**Height**: 42px  
**Characteristics**:
- Ant Design system
- Clean, enterprise-friendly
- Bordered buttons
- Icon + text labels
- Subtle hover effects
- Blue accent colors

### 9. TelerikNavigationPainter
**Style**: `navigationStyle.Telerik`  
**Height**: 38px  
**Characteristics**:
- Telerik UI inspired
- Professional grid appearance
- Gradient buttons
- Separator bars between groups
- Consistent spacing

### 10. CompactNavigationPainter
**Style**: `navigationStyle.Compact`  
**Height**: 28px  
**Characteristics**:
- Minimal space usage
- Smaller buttons and text
- Icon-only mode
- Dense layouts
- Perfect for space-constrained UIs

### 11. MinimalNavigationPainter
**Style**: `navigationStyle.Minimal`  
**Height**: 36px  
**Characteristics**:
- Absolute minimal styling
- Text-based buttons
- No borders or backgrounds
- Focus on content
- Clean and simple

### 12. CardNavigationPainter
**Style**: `navigationStyle.Card`  
**Height**: 52px  
**Characteristics**:
- Card-based design
- Elevated appearance
- Rounded corners
- Shadow effects
- Modern, friendly look

### 13. None
**Style**: `navigationStyle.None`  
**Height**: 0px  
**Characteristics**:
- No navigation bar
- Grid extends to bottom
- Useful for embedding or custom navigation

## Base Classes

### BaseNavigationPainter

Abstract base class providing common functionality for all navigation painters.

**Common Methods**:
```csharp
// Geometry helpers
protected GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
protected GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius,
    bool topLeft, bool topRight, bool bottomLeft, bool bottomRight)

// Color helpers
protected Color GetButtonColor(NavigationButtonState state, IBeepTheme theme)
protected Color GetTextColor(NavigationButtonState state, IBeepTheme theme)

// Drawing helpers
protected void DrawCenteredText(Graphics g, string text, Font font, Color color, Rectangle bounds)
protected void DrawButtonShadow(Graphics g, Rectangle bounds, Color shadowColor)
protected Size CalculateButtonSize(int availableHeight, bool isSquare = true)
```

**Button Content Defaults**:
- First: ⏮ (First track symbol)
- Previous: ◀ (Left arrow)
- Next: ▶ (Right arrow)
- Last: ⏭ (Last track symbol)
- Add: + (Plus)
- Delete: ✕ (Cross)
- Save: ✓ (Checkmark)
- Cancel: ✖ (Heavy cross)

## NavigationLayout Class

The `NavigationLayout` class describes the complete layout of navigation elements.

### Button Rectangles
```csharp
Rectangle FirstButtonRect
Rectangle PreviousButtonRect
Rectangle NextButtonRect
Rectangle LastButtonRect
Rectangle AddNewButtonRect
Rectangle DeleteButtonRect
Rectangle SaveButtonRect
Rectangle CancelButtonRect
Rectangle EditButtonRect
Rectangle RefreshButtonRect
```

### Information Display Areas
```csharp
Rectangle PositionIndicatorRect      // "1 of 100"
Rectangle PageInfoRect               // "Page 1 of 10"
Rectangle RecordCountRect            // "100 records"
Rectangle StatusTextRect             // Status messages
```

### Pagination Controls
```csharp
Rectangle PageSizeComboRect          // "Show 10 entries"
Rectangle PageNumberInputRect        // Page number textbox
Rectangle GoToPageButtonRect         // "Go" button
Rectangle[] PageNumberRects          // [1] [2] [3] [4] [5]
```

### Additional Features
```csharp
Rectangle QuickSearchRect            // Search textbox
Rectangle FilterButtonRect           // Filter icon/button
Rectangle ColumnsButtonRect          // Column chooser
Rectangle ExportButtonRect           // Export data
Rectangle PrintButtonRect            // Print grid
Rectangle SettingsButtonRect         // Settings/preferences
Rectangle MoreActionsButtonRect      // Additional actions menu
Rectangle HelpButtonRect             // Help/info
```

### Layout Sections
```csharp
Rectangle LeftSectionRect            // Left group of controls
Rectangle CenterSectionRect          // Center group
Rectangle RightSectionRect           // Right group
Rectangle TopSectionRect             // Top row (for two-row layouts)
Rectangle BottomSectionRect          // Bottom row
```

### Metrics
```csharp
int ButtonSpacing = 4                // Space between buttons
int GroupSpacing = 12                // Space between groups
int Padding = 8                      // Edge padding
Size ButtonSize                      // Standard button size
int TotalHeight                      // Calculated total height
int TotalWidth                       // Calculated total width
bool IsCompact                       // Compact mode flag
bool IconOnly                        // Icon-only mode flag
int CurrentPage                      // Current page number (1-based)
int TotalPages                       // Total page count
int VisiblePageButtons = 5           // Number of page buttons to show
```

## NavigationPainterFactory

Factory class for creating painter instances.

```csharp
public static class NavigationPainterFactory
{
    public static INavigationPainter CreatePainter(navigationStyle style)
    {
        return style switch
        {
            navigationStyle.Material => new MaterialNavigationPainter(),
            navigationStyle.Bootstrap => new BootstrapNavigationPainter(),
            navigationStyle.Tailwind => new TailwindNavigationPainter(),
            navigationStyle.Fluent => new FluentNavigationPainter(),
            navigationStyle.AGGrid => new AGGridNavigationPainter(),
            navigationStyle.DataTables => new DataTablesNavigationPainter(),
            navigationStyle.AntDesign => new AntDesignNavigationPainter(),
            navigationStyle.Telerik => new TelerikNavigationPainter(),
            navigationStyle.Compact => new CompactNavigationPainter(),
            navigationStyle.Minimal => new MinimalNavigationPainter(),
            navigationStyle.Card => new CardNavigationPainter(),
            _ => new StandardNavigationPainter()
        };
    }
    
    public static int GetRecommendedHeight(navigationStyle style)
    {
        var painter = CreatePainter(style);
        return painter.RecommendedHeight;
    }
}
```

## Enums

### navigationStyle
```csharp
public enum navigationStyle
{
    None,           // No navigation bar
    Standard,       // Basic functional style
    Material,       // Material Design
    Bootstrap,      // Bootstrap inspired
    Tailwind,       // Tailwind CSS inspired
    Fluent,         // Microsoft Fluent
    AGGrid,         // AG Grid inspired
    DataTables,     // jQuery DataTables inspired
    AntDesign,      // Ant Design system
    Telerik,        // Telerik UI inspired
    Compact,        // Space-efficient
    Minimal,        // Minimal styling
    Card           // Card-based design
}
```

### NavigationButtonType
```csharp
public enum NavigationButtonType
{
    First,          // Go to first record
    Previous,       // Go to previous record
    Next,           // Go to next record
    Last,           // Go to last record
    AddNew,         // Add new record
    Delete,         // Delete current record
    Save,           // Save changes
    Cancel,         // Cancel changes
    Edit,           // Edit mode toggle
    Refresh,        // Refresh data
    Filter,         // Open filter dialog
    Export,         // Export data
    Print,          // Print grid
    Settings,       // Grid settings
    Help            // Help/info
}
```

### NavigationButtonState
```csharp
public enum NavigationButtonState
{
    Normal,         // Default state
    Hovered,        // Mouse hovering
    Pressed,        // Mouse down
    Disabled        // Button disabled
}
```

## Creating a Custom Navigation Painter

### Step 1: Create Painter Class

```csharp
public class CustomNavigationPainter : BaseNavigationPainter
{
    public override navigationStyle Style => navigationStyle.Custom;
    public override int RecommendedHeight => 44;
    public override int RecommendedMinWidth => 500;
    
    public override void PaintNavigation(Graphics g, Rectangle bounds, 
        BeepGridPro grid, IBeepTheme theme)
    {
        // 1. Clear hit areas
        grid.ClearHitList();
        
        // 2. Draw background
        using (var brush = new SolidBrush(theme.GridHeaderBackColor))
        {
            g.FillRectangle(brush, bounds);
        }
        
        // 3. Calculate layout
        var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);
        
        // 4. Paint buttons with hit areas
        PaintButtonWithHitArea(g, grid, layout.FirstButtonRect, 
            NavigationButtonType.First, "First", () => grid.MoveFirst(), theme);
        // ... more buttons
        
        // 5. Paint position indicator
        PaintPositionIndicator(g, layout.PositionIndicatorRect,
            grid.Selection.RowIndex + 1, grid.Data.Rows.Count, theme);
    }
    
    public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
        int totalRecords, bool showCrudButtons)
    {
        var layout = new NavigationLayout();
        
        // Calculate button positions, sizes, etc.
        int x = availableBounds.Left + 8;
        int y = availableBounds.Top + 8;
        int buttonWidth = 32;
        int buttonHeight = 28;
        
        layout.FirstButtonRect = new Rectangle(x, y, buttonWidth, buttonHeight);
        x += buttonWidth + 4;
        // ... more calculations
        
        return layout;
    }
    
    // Implement other abstract methods...
}
```

### Step 2: Add to navigationStyle Enum

```csharp
public enum navigationStyle
{
    // ... existing values
    Custom          // Add your new style
}
```

### Step 3: Register in Factory

```csharp
public static INavigationPainter CreatePainter(navigationStyle style)
{
    return style switch
    {
        // ... existing cases
        navigationStyle.Custom => new CustomNavigationPainter(),
        _ => new StandardNavigationPainter()
    };
}
```

### Step 4: Use Your Painter

```csharp
var grid = new BeepGridPro();
grid.NavigationStyle = navigationStyle.Custom;
grid.UsePainterNavigation = true;
```

## Helper Method: PaintButtonWithHitArea

This is a common pattern used in painters to paint a button and register its hit area:

```csharp
private void PaintButtonWithHitArea(Graphics g, BeepGridPro grid, Rectangle bounds, 
    NavigationButtonType buttonType, string hitAreaName, Action action, IBeepTheme theme)
{
    if (bounds.IsEmpty) return;

    // Register hit area for click handling
    grid.AddHitArea(hitAreaName, bounds, null, action);

    // Paint the button
    PaintButton(g, bounds, buttonType, NavigationButtonState.Normal, null, theme);
}
```

## Theme Integration

All painters receive an `IBeepTheme` parameter which provides consistent colors:

```csharp
theme.GridHeaderBackColor      // Navigation bar background
theme.GridHeaderForeColor      // Text and icon color
theme.ButtonBackColor          // Button background
theme.ButtonForeColor          // Button text/icon
theme.ButtonHoverBackColor     // Hover state
theme.ButtonSelectedBackColor  // Selected/pressed state
theme.GridLineColor            // Border and separator color
theme.AccentColor              // Accent/highlight color
```

## Best Practices

1. **Always clear hit areas** at the start of `PaintNavigation`
2. **Use theme colors** instead of hard-coded colors
3. **Calculate layout first**, then paint
4. **Register hit areas** for all clickable elements
5. **Handle empty rectangles** gracefully
6. **Use anti-aliasing** for smooth graphics
7. **Respect RecommendedHeight** in your layout calculations
8. **Provide meaningful tooltips** in `GetButtonTooltip`
9. **Test with different themes** (light/dark)
10. **Consider compact mode** for narrow layouts

## Performance Considerations

1. **Cache brushes and pens** when possible
2. **Avoid creating fonts** in tight loops
3. **Use StringFormat** for text alignment
4. **Dispose graphics objects** properly
5. **Minimize Graphics.DrawString** calls
6. **Use FillRectangle** instead of FillPolygon when possible

## Future Enhancements

1. **Header Painters**: Implement IPaintGridHeader for custom column header styles
2. **Cell Painters**: Custom cell rendering strategies
3. **Row Painters**: Custom row rendering
4. **Animation Support**: Transitions and effects
5. **Interactive Elements**: Dropdowns, textboxes in navigation
6. **Responsive Layouts**: Adapt to width changes
7. **Accessibility**: ARIA-like features for screen readers

## Related Files

- `enums.cs` - Contains navigationStyle and related enums
- `NavigationPainterFactory.cs` - Factory for creating painters
- `GridNavigationPainterHelper.cs` - Integrates painters with BeepGridPro
- `INavigationPainter.cs` - Main interface definition
- `BaseNavigationPainter.cs` - Base implementation

## See Also

- [../README.md](../README.md) - Main BeepGridPro documentation
- [../Helpers/README.md](../Helpers/README.md) - Helper classes documentation
- [../Layouts/README.md](../Layouts/README.md) - Layout presets documentation
