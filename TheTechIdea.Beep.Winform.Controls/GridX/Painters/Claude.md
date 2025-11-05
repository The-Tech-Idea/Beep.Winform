# Claude.md - Painters Folder Guide

## Quick Reference

**Purpose**: Visual rendering strategies for navigation bars and headers using Strategy pattern

**File Count**: 17 files (13 navigation painters + 4 infrastructure)

**Key Pattern**: Factory + Strategy - painters are stateless, created by factories

## File Structure

```
Painters/
├── Interfaces
│   ├── INavigationPainter.cs          - Navigation painter contract
│   ├── IPaintGridHeader.cs            - Header painter contract (future)
│   └── IPaintGridNavigation.cs        - Grid navigation contract (future)
│
├── Base Classes
│   └── BaseNavigationPainter.cs       - Common navigation functionality
│
├── Navigation Painters (12 implementations)
│   ├── StandardNavigationPainter.cs   - Basic functional (32px)
│   ├── MaterialNavigationPainter.cs   - Material Design (56px)
│   ├── BootstrapNavigationPainter.cs  - Bootstrap inspired (40px)
│   ├── TailwindNavigationPainter.cs   - Tailwind CSS (44px)
│   ├── FluentNavigationPainter.cs     - Microsoft Fluent (48px)
│   ├── AGGridNavigationPainter.cs     - AG Grid style (36px)
│   ├── DataTablesNavigationPainter.cs - jQuery DataTables (48px)
│   ├── AntDesignNavigationPainter.cs  - Ant Design (42px)
│   ├── TelerikNavigationPainter.cs    - Telerik UI (38px)
│   ├── CompactNavigationPainter.cs    - Space-efficient (28px)
│   ├── MinimalNavigationPainter.cs    - Minimal styling (36px)
│   └── CardNavigationPainter.cs       - Card-based (52px)
│
├── Factories
│   └── NavigationPainterFactory.cs    - Creates painter instances
│
└── Supporting
    └── enums.cs                        - navigationStyle, NavigationButtonType, etc.
```

## Key Interfaces

### INavigationPainter
```csharp
navigationStyle Style { get; }              // Style identifier
int RecommendedHeight { get; }              // Height needed
int RecommendedMinWidth { get; }            // Minimum width
void PaintNavigation(...)                   // Main painting method
NavigationLayout CalculateLayout(...)       // Layout calculation
string GetButtonContent(NavigationButtonType) // Icon/text for button
string GetButtonTooltip(NavigationButtonType) // Tooltip text
```

### NavigationLayout
**Purpose**: Describes calculated positions of all navigation elements

**Key Rectangles**:
- Navigation buttons: FirstButtonRect, PreviousButtonRect, NextButtonRect, LastButtonRect
- CRUD buttons: AddNewButtonRect, DeleteButtonRect, SaveButtonRect, CancelButtonRect
- Indicators: PositionIndicatorRect, PageInfoRect, RecordCountRect
- Pagination: PageSizeComboRect, PageNumberInputRect, GoToPageButtonRect
- Utilities: QuickSearchRect, FilterButtonRect, ExportButtonRect
- Sections: LeftSectionRect, CenterSectionRect, RightSectionRect

**Key Metrics**:
- ButtonSpacing, GroupSpacing, Padding
- TotalHeight, TotalWidth
- IsCompact, IconOnly
- CurrentPage, TotalPages

## Implementation Pattern

### 1. Implement Abstract Methods from BaseNavigationPainter

```csharp
public class MyNavigationPainter : BaseNavigationPainter
{
    // Required overrides
    public override navigationStyle Style => navigationStyle.Custom;
    public override int RecommendedHeight => 40;
    public override int RecommendedMinWidth => 400;
    
    public override void PaintNavigation(Graphics g, Rectangle bounds, 
        BeepGridPro grid, IBeepTheme theme)
    {
        // STEP 1: Clear existing hit areas
        grid.ClearHitList();
        
        // STEP 2: Draw background
        using (var bgBrush = new SolidBrush(theme.GridHeaderBackColor))
        {
            g.FillRectangle(bgBrush, bounds);
        }
        
        // STEP 3: Calculate layout
        var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);
        
        // STEP 4: Paint buttons WITH hit area registration
        PaintButtonWithHitArea(g, grid, layout.FirstButtonRect, 
            NavigationButtonType.First, "First", () => grid.MoveFirst(), theme);
        // ... more buttons
        
        // STEP 5: Paint position indicator
        PaintPositionIndicator(g, layout.PositionIndicatorRect,
            grid.Selection.RowIndex + 1, grid.Data.Rows.Count, theme);
    }
    
    public override NavigationLayout CalculateLayout(Rectangle bounds, 
        int totalRecords, bool showCrudButtons)
    {
        var layout = new NavigationLayout
        {
            ButtonSize = new Size(32, 28),
            ButtonSpacing = 4,
            GroupSpacing = 12,
            Padding = 8
        };
        
        // Calculate button positions
        int x = bounds.Left + layout.Padding;
        int y = bounds.Top + (bounds.Height - layout.ButtonSize.Height) / 2;
        
        layout.FirstButtonRect = new Rectangle(x, y, 32, 28);
        x += 32 + layout.ButtonSpacing;
        // ... more calculations
        
        layout.TotalHeight = RecommendedHeight;
        return layout;
    }
    
    public override void PaintButton(Graphics g, Rectangle bounds, 
        NavigationButtonType buttonType, NavigationButtonState state, 
        IBeepUIComponent component, IBeepTheme theme)
    {
        // Paint button visual based on state
        Color bgColor = GetButtonColor(state, theme);
        Color fgColor = GetTextColor(state, theme);
        
        // Draw button background
        using (var brush = new SolidBrush(bgColor))
        {
            g.FillRectangle(brush, bounds);
        }
        
        // Draw button icon/text
        string content = GetButtonContent(buttonType);
        DrawCenteredText(g, content, font, fgColor, bounds);
    }
    
    public override void PaintPositionIndicator(Graphics g, Rectangle bounds, 
        int currentPosition, int totalRecords, IBeepTheme theme)
    {
        string text = $"{currentPosition} of {totalRecords}";
        using (var font = new Font("Segoe UI", 9))
        using (var brush = new SolidBrush(theme.GridHeaderForeColor))
        using (var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        })
        {
            g.DrawString(text, font, brush, bounds, format);
        }
    }
}
```

### 2. Helper Method for Hit Area Registration

**CRITICAL**: All painters must register hit areas for clickable elements!

```csharp
private void PaintButtonWithHitArea(Graphics g, BeepGridPro grid, Rectangle bounds, 
    NavigationButtonType buttonType, string hitAreaName, Action action, IBeepTheme theme)
{
    if (bounds.IsEmpty) return;

    // 1. Register hit area (allows grid to detect clicks)
    grid.AddHitArea(hitAreaName, bounds, null, action);

    // 2. Paint the button visual
    PaintButton(g, bounds, buttonType, NavigationButtonState.Normal, null, theme);
}
```

### 3. Using Base Class Helpers

BaseNavigationPainter provides many useful helpers:

```csharp
// Geometry
var path = CreateRoundedRectangle(bounds, 4);
var path2 = CreateRoundedRectangle(bounds, 4, 
    topLeft: true, topRight: true, bottomLeft: false, bottomRight: false);

// Colors
Color buttonColor = GetButtonColor(NavigationButtonState.Hovered, theme);
Color textColor = GetTextColor(NavigationButtonState.Disabled, theme);

// Drawing
DrawCenteredText(g, "Text", font, color, bounds);
DrawButtonShadow(g, bounds, shadowColor);

// Calculations
Size buttonSize = CalculateButtonSize(availableHeight, isSquare: true);
```

## Common Patterns by Style

### Material Design (56px, Modern)
```csharp
// Characteristics
- Large height (56px)
- Circular icon buttons
- Ripple/elevation effects
- Flat design with shadows
- Icon-only (no text labels)

// Key Visual Elements
- g.SmoothingMode = SmoothingMode.AntiAlias
- Circular hover effects (FillEllipse)
- Material icons (Unicode symbols)
- Subtle shadows for depth
```

### Bootstrap (40px, Familiar)
```csharp
// Characteristics
- Medium height (40px)
- Button groups with borders
- Rounded corners (4px)
- Text + icon labels
- Color-coded (primary, danger, etc.)

// Key Visual Elements
- Rectangle buttons with rounded corners
- Grouped buttons (adjacent borders)
- Clear button states
- Bootstrap color scheme
```

### Compact (28px, Dense)
```csharp
// Characteristics
- Small height (28px)
- Icon-only, small buttons
- Minimal spacing
- Dense layout

// Key Visual Elements
- Smaller fonts (7-8pt)
- Reduced padding (2-4px)
- Compact button sizes (20x20)
- No extra spacing
```

### AGGrid/DataTables (36-48px, Enterprise)
```csharp
// Characteristics
- Professional appearance
- Pagination controls prominent
- Page size dropdown
- "Showing X to Y of Z" text
- Numbered pagination buttons

// Key Visual Elements
- Page number display
- Input field for "go to page"
- Dropdown for "rows per page"
- Record range indicator
```

## NavigationLayout Calculation Strategy

### Responsive Layout Approach

```csharp
public override NavigationLayout CalculateLayout(Rectangle bounds, 
    int totalRecords, bool showCrudButtons)
{
    var layout = new NavigationLayout();
    
    // 1. Determine mode based on available width
    bool compact = bounds.Width < 600;
    layout.IsCompact = compact;
    
    // 2. Calculate button size
    layout.ButtonSize = compact ? new Size(24, 24) : new Size(32, 28);
    
    // 3. Calculate total width needed
    int neededWidth = CalculateTotalWidthNeeded(layout.ButtonSize, showCrudButtons);
    
    // 4. Adjust layout if needed
    if (neededWidth > bounds.Width)
    {
        // Switch to icon-only mode
        layout.IconOnly = true;
        layout.ButtonSize = new Size(28, 28);
    }
    
    // 5. Position elements
    PositionLeftSection(layout, bounds);   // CRUD buttons
    PositionCenterSection(layout, bounds); // Navigation + position
    PositionRightSection(layout, bounds);  // Utilities
    
    return layout;
}

private void PositionLeftSection(NavigationLayout layout, Rectangle bounds)
{
    int x = bounds.Left + layout.Padding;
    int y = bounds.Top + (bounds.Height - layout.ButtonSize.Height) / 2;
    
    // Add button rect
    layout.AddNewButtonRect = new Rectangle(x, y, 
        layout.ButtonSize.Width, layout.ButtonSize.Height);
    x += layout.ButtonSize.Width + layout.ButtonSpacing;
    
    // Delete button rect
    layout.DeleteButtonRect = new Rectangle(x, y, 
        layout.ButtonSize.Width, layout.ButtonSize.Height);
    // ... etc
}
```

## Theme Integration

### Always Use Theme Colors

```csharp
// Background
using (var brush = new SolidBrush(theme.GridHeaderBackColor))
    g.FillRectangle(brush, bounds);

// Text/Icons
Color foreColor = theme.GridHeaderForeColor;
Color accentColor = theme.AccentColor;

// Buttons
Color buttonBg = theme.ButtonBackColor;
Color buttonHover = theme.ButtonHoverBackColor;
Color buttonPressed = theme.ButtonSelectedBackColor;

// Borders
Color borderColor = theme.GridLineColor;
```

### State-Based Colors

```csharp
protected Color GetButtonColor(NavigationButtonState state, IBeepTheme theme)
{
    return state switch
    {
        NavigationButtonState.Pressed => 
            ControlPaint.Dark(theme.ButtonBackColor, 0.2f),
        NavigationButtonState.Hovered => 
            ControlPaint.Light(theme.ButtonBackColor, 0.1f),
        NavigationButtonState.Disabled => 
            Color.FromArgb(230, 230, 230),
        _ => theme.ButtonBackColor
    };
}
```

## Testing Painters

### Visual Test
```csharp
var painter = new MaterialNavigationPainter();
var grid = CreateTestGrid();
var theme = BeepThemesManager.GetDefaultTheme();

using (var bmp = new Bitmap(800, 56))
using (var g = Graphics.FromImage(bmp))
{
    painter.PaintNavigation(g, new Rectangle(0, 0, 800, 56), grid, theme);
    bmp.Save("test_material_nav.png");
}
```

### Layout Test
```csharp
[TestMethod]
public void MaterialPainter_CalculateLayout_ShouldReturnValidRects()
{
    var painter = new MaterialNavigationPainter();
    var layout = painter.CalculateLayout(
        new Rectangle(0, 0, 800, 56), 
        totalRecords: 100, 
        showCrudButtons: true);
    
    Assert.IsFalse(layout.FirstButtonRect.IsEmpty);
    Assert.IsFalse(layout.NextButtonRect.IsEmpty);
    Assert.IsTrue(layout.TotalHeight == 56);
}
```

## Debugging Checklist

When a painter isn't working:

1. **Hit areas registered?**
   - Check if `grid.ClearHitList()` is called
   - Check if `grid.AddHitArea()` is called for each button

2. **Bounds empty?**
   - Check if `bounds.IsEmpty` before drawing
   - Check if `bounds.Width > 0 && bounds.Height > 0`

3. **Theme null?**
   - Check if theme parameter is null
   - Provide fallback colors if needed

4. **Layout calculation correct?**
   - Add Console.WriteLine for debugging
   - Visualize rectangles with debug drawing

5. **Graphics state?**
   - Save/restore graphics state if modifying
   - Reset transformations after use

## Performance Tips

1. **Cache static elements**:
```csharp
private static readonly StringFormat CenterFormat = new StringFormat
{
    Alignment = StringAlignment.Center,
    LineAlignment = StringAlignment.Center
};
```

2. **Reuse graphics objects**:
```csharp
// BAD - creates new brush each call
public void PaintButton(...)
{
    using (var brush = new SolidBrush(theme.ButtonBackColor))
        g.FillRectangle(brush, bounds);
}

// GOOD - reuse in single method
public void PaintNavigation(...)
{
    using (var bgBrush = new SolidBrush(theme.GridHeaderBackColor))
    using (var fgBrush = new SolidBrush(theme.GridHeaderForeColor))
    {
        // Paint everything using these brushes
    }
}
```

3. **Avoid expensive operations**:
```csharp
// Avoid creating fonts in tight loops
// Avoid complex paths when rectangles suffice
// Avoid DrawString when DrawImage is better for icons
```

## Common Mistakes

### 1. Forgetting to Clear Hit Areas
```csharp
// BAD - hit areas accumulate
public override void PaintNavigation(...)
{
    // Missing: grid.ClearHitList();
    PaintButtonWithHitArea(...);
}

// GOOD
public override void PaintNavigation(...)
{
    grid.ClearHitList(); // Always first!
    PaintButtonWithHitArea(...);
}
```

### 2. Not Handling Empty Rectangles
```csharp
// BAD - crashes if bounds empty
g.FillRectangle(brush, bounds);

// GOOD
if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
    return;
g.FillRectangle(brush, bounds);
```

### 3. Hard-Coding Colors
```csharp
// BAD
using (var brush = new SolidBrush(Color.White))

// GOOD
using (var brush = new SolidBrush(theme.GridHeaderBackColor))
```

### 4. Not Disposing Graphics Objects
```csharp
// BAD - memory leak
var brush = new SolidBrush(color);
g.FillRectangle(brush, bounds);

// GOOD
using (var brush = new SolidBrush(color))
{
    g.FillRectangle(brush, bounds);
}
```

## Factory Pattern Usage

### Creating Painters
```csharp
// Factory handles caching and creation
var painter = NavigationPainterFactory.CreatePainter(navigationStyle.Material);

// Get recommended height without creating instance
int height = NavigationPainterFactory.GetRecommendedHeight(navigationStyle.Material);
```

### Registering Custom Painters
Currently, factory uses switch statement. To add custom painter:
1. Add to navigationStyle enum
2. Add case to factory
3. Implement painter class

Future: Plugin-based registration system

## Related Files

- **GridNavigationPainterHelper.cs**: Integrates painters with BeepGridPro
- **enums.cs**: Defines navigationStyle and related enums
- **INavigationPainter.cs**: Interface contract
- **BaseNavigationPainter.cs**: Common functionality

## Next Steps (Enhancement Plan)

1. **Header Painters**: Create similar system for column headers
2. **Layout Integration**: Link painters to layout presets
3. **Animation**: Add transition effects
4. **Interactivity**: Add dropdowns, textboxes in navigation
5. **Accessibility**: Add ARIA-like features

See `../ENHANCEMENT_PLAN.md` for detailed roadmap.
