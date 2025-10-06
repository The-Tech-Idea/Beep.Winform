# GridX Painter Quick Reference

## Setting Up Painters

```csharp
// Basic setup
var grid = new BeepGridPro
{
    ControlStyle = BeepControlStyle.Material3,
    UseThemeColors = true,
    Theme = "BusinessProfessional"
};
```

## Changing Styles

```csharp
// At design time or runtime
grid.ControlStyle = BeepControlStyle.iOS15;
grid.Invalidate();
```

## Available Styles

### Material & Modern
- `Material3` - Material Design 3
- `MaterialYou` - Dynamic Material
- `AntDesign` - Ant Design
- `ChakraUI` - Chakra UI
- `TailwindCard` - Tailwind CSS
- `FigmaCard` - Figma design

### Apple
- `iOS15` - iOS 15 style
- `MacOSBigSur` - macOS Big Sur

### Microsoft
- `Fluent2` - Fluent Design
- `Windows11Mica` - Windows 11
- `Bootstrap` - Bootstrap 5

### Special Effects
- `Neumorphism` - Soft UI
- `GlassAcrylic` - Frosted glass
- `DarkGlow` - Dark + glow
- `GradientModern` - Gradients

### Minimal
- `Minimal` - Clean minimal
- `NotionMinimal` - Notion-style
- `VercelClean` - Vercel design

### Other
- `StripeDashboard` - Stripe style
- `DiscordStyle` - Discord UI
- `PillRail` - Pill shapes

## Creating Custom Painters

### Minimal Template

```csharp
public class MyHeaderPainter : IPaintGridHeader
{
    public string StyleName => "Custom";

    public void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid)
    {
        var theme = GetTheme(grid);
        
        // Use BeepStyling for background
        BeepStyling.CurrentTheme = theme;
        BeepStyling.UseThemeColors = grid.UseThemeColors;
        BeepStyling.PaintStyleBackground(g, headerRect, grid.ControlStyle);
        
        // Your custom rendering here
    }

    public void PaintHeaderCell(Graphics g, Rectangle cellRect, 
                               BeepColumnConfig column, int columnIndex, 
                               BeepGridPro grid)
    {
        // Not used - grid handles column headers
    }

    public void RegisterHeaderHitAreas(BeepGridPro grid)
    {
        // Register during painting
    }

    private IBeepTheme GetTheme(BeepGridPro grid)
    {
        return grid.Theme != null 
            ? BeepThemesManager.GetTheme(grid.Theme) 
            : BeepThemesManager.GetDefaultTheme();
    }
}
```

### Adding Buttons

```csharp
// In your Paint method:
var buttonRect = new Rectangle(x, y, width, height);

// Draw button
DrawButton(g, buttonRect, "Add", theme);

// Register hit area
grid.AddHitArea("AddButton", buttonRect, null, () =>
{
    grid.PainterEvents?.TriggerEvent(GridPainterEvents.NavInsert);
});
```

## Event Handling

### Subscribe to Events

```csharp
grid.PainterEvents.RegisterEvent(GridPainterEvents.NavInsert, (s, e) =>
{
    MessageBox.Show("Insert clicked!");
});

grid.PainterEvents.RegisterEvent(GridPainterEvents.NavDelete, (s, e) =>
{
    if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
    {
        grid.DeleteCurrent();
    }
});
```

### Standard Events

**Header Events**
- `ClickedSortIcon`
- `ClickedFilterIcon`
- `ClickedHeaderCell`
- `HeaderCellDoubleClick`
- `ResizeColumn`

**Navigation Events**
- `NavFirst`
- `NavPrevious`
- `NavNext`
- `NavLast`
- `NavInsert`
- `NavDelete`
- `NavSave`
- `NavCancel`
- `NavQuery`
- `NavFilter`
- `NavPrint`
- `NavPageSizeChanged`
- `NavPageJump`

## Common Patterns

### Material Design Button

```csharp
private void DrawMaterialButton(Graphics g, Rectangle rect, string text, 
                                IBeepTheme theme, bool isPrimary = false)
{
    using (var path = CreateRoundedRect(rect, 4))
    {
        var bgColor = isPrimary ? theme.PrimaryColor : Color.FromArgb(250, 250, 250);
        var textColor = isPrimary ? Color.White : theme.ForeColor;
        
        using (var brush = new SolidBrush(bgColor))
        using (var textBrush = new SolidBrush(textColor))
        using (var font = new Font("Roboto", 10))
        {
            g.FillPath(brush, path);
            
            var sf = new StringFormat 
            { 
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center 
            };
            
            g.DrawString(text, font, textBrush, rect, sf);
        }
    }
}
```

### Bootstrap Button

```csharp
private void DrawBootstrapButton(Graphics g, Rectangle rect, string text,
                                 IBeepTheme theme, bool isPrimary = false)
{
    var bgColor = isPrimary 
        ? Color.FromArgb(13, 110, 253)  // btn-primary
        : Color.White;
    var borderColor = Color.FromArgb(13, 110, 253);
    var textColor = isPrimary ? Color.White : borderColor;
    
    using (var brush = new SolidBrush(bgColor))
    using (var pen = new Pen(borderColor))
    using (var textBrush = new SolidBrush(textColor))
    using (var font = new Font("Segoe UI", 9))
    {
        g.FillRectangle(brush, rect);
        g.DrawRectangle(pen, rect);
        
        var sf = new StringFormat 
        { 
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center 
        };
        
        g.DrawString(text, font, textBrush, rect, sf);
    }
}
```

### Icon Button

```csharp
private void DrawIconButton(Graphics g, Rectangle rect, string icon,
                           IBeepTheme theme)
{
    // Circular background
    using (var path = new GraphicsPath())
    {
        path.AddEllipse(rect);
        
        using (var brush = new SolidBrush(Color.FromArgb(10, theme.ForeColor)))
        {
            g.FillPath(brush, path);
        }
    }
    
    // Icon (use icon font or image)
    using (var font = new Font("Material Icons", 18))
    using (var brush = new SolidBrush(theme.ForeColor))
    {
        var sf = new StringFormat 
        { 
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center 
        };
        
        g.DrawString(icon, font, brush, rect, sf);
    }
}
```

### Search Box

```csharp
private void DrawSearchBox(Graphics g, Rectangle rect, IBeepTheme theme)
{
    // Background
    using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
    using (var pen = new Pen(Color.FromArgb(230, 230, 230)))
    {
        g.FillRectangle(brush, rect);
        g.DrawRectangle(pen, rect);
    }
    
    // Placeholder text
    using (var font = new Font("Segoe UI", 9))
    using (var brush = new SolidBrush(Color.Gray))
    {
        g.DrawString("Search...", font, brush, 
            new Rectangle(rect.X + 8, rect.Y, rect.Width - 8, rect.Height),
            new StringFormat { LineAlignment = StringAlignment.Center });
    }
}
```

## Performance Tips

```csharp
// ✅ Cache brushes
private SolidBrush _cachedBrush;

// ✅ Reuse paths
using (var path = CreateRoundedRect(rect, radius))
{
    g.FillPath(brush, path);
    g.DrawPath(pen, path);
}

// ✅ Batch operations
foreach (var item in items)
    g.FillRectangle(brush, item.Rect);
foreach (var item in items)
    g.DrawRectangle(pen, item.Rect);

// ❌ Avoid
foreach (var item in items)
{
    g.FillRectangle(brush, item.Rect);
    g.DrawRectangle(pen, item.Rect); // Context switching
}
```

## Troubleshooting

### Background Not Styled
```csharp
// WRONG
BeepStyling.PaintStyleBackground(g, rect, style);

// RIGHT
BeepStyling.CurrentTheme = theme;
BeepStyling.UseThemeColors = grid.UseThemeColors;
BeepStyling.PaintStyleBackground(g, rect, grid.ControlStyle);
```

### Colors Don't Update
```csharp
// Ensure Invalidate is called
grid.ControlStyle = BeepControlStyle.Material3;
grid.Invalidate(); // Force repaint
```

### Events Not Firing
```csharp
// Check PainterEvents is not null
if (grid.PainterEvents != null)
{
    grid.PainterEvents.TriggerEvent(eventName);
}
// Or use safe navigation
grid.PainterEvents?.TriggerEvent(eventName);
```

## Resources

- [Full Guide](ModernPainterGuide.md)
- [Implementation Plan](PlanHeaderNavigationPainters.md)
- [Integration Summary](BEEPSTYLING_INTEGRATION_SUMMARY.md)
- [BeepStyling Source](../../Styling/BeepStyling.cs)

---

**Last Updated**: October 6, 2025  
**Version**: 1.0
