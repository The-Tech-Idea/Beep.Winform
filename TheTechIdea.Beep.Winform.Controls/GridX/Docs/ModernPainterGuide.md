# Modern Painter Guide - Using BeepStyling for Grid Headers and Navigation

## Overview
The GridX painter system now integrates with `BeepStyling` to provide consistent, modern visual styles across all grid components. This guide shows how to create and use painters that leverage the 25+ built-in control styles.

## Integration with BeepStyling

### Core Concepts

1. **ControlStyle Property**: Inherited from `BaseControl`, determines the visual style (Material3, iOS15, Bootstrap, etc.)
2. **UseThemeColors**: Boolean flag to use theme colors vs. custom colors
3. **BeepStyling Static Class**: Central styling system that handles all visual rendering

### How It Works

```csharp
// In your painter's PaintNavigation or PaintHeaders method:
var theme = GetTheme(grid);

// Set BeepStyling context
BeepStyling.CurrentTheme = theme;
BeepStyling.UseThemeColors = grid.UseThemeColors;

// Paint background with current control style
BeepStyling.PaintStyleBackground(g, rect, grid.ControlStyle);
```

## Available Control Styles

### Material Design Family
- `Material3` - Latest Material Design (default)
- `MaterialYou` - Dynamic color Material Design
- `AntDesign` - Ant Design system
- `ChakraUI` - Chakra UI styling

### Apple Design Family
- `iOS15` - Modern iOS design
- `MacOSBigSur` - macOS Big Sur style

### Microsoft Design Family
- `Fluent2` - Microsoft Fluent Design 2.0
- `Windows11Mica` - Windows 11 Mica material
- `Bootstrap` - Bootstrap 5 framework

### Modern Web Frameworks
- `TailwindCard` - Tailwind CSS cards
- `StripeDashboard` - Stripe dashboard style
- `VercelClean` - Vercel clean design
- `NotionMinimal` - Notion-inspired minimal
- `FigmaCard` - Figma card design
- `DiscordStyle` - Discord UI style

### Special Effects
- `Neumorphism` - Soft UI with dual shadows
- `GlassAcrylic` - Frosted glass effect
- `DarkGlow` - Dark theme with glow
- `GradientModern` - Modern gradients
- `PillRail` - Pill-shaped elements

### Minimal Styles
- `Minimal` - Clean minimal design

## Creating Modern Painters

### Example: Material Design Header Painter

```csharp
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters.Header
{
    /// <summary>
    /// Material Design inspired header painter with elevation and clean layout
    /// </summary>
    public class MaterialHeaderPainter : IPaintGridHeader
    {
        private const int ToolbarHeight = 56; // Material standard app bar height
        private const int ButtonHeight = 36;  // Material button height
        private const int IconSize = 24;      // Material icon size
        private const int Padding = 16;       // Material spacing unit
        private const int Spacing = 8;        // Half spacing unit

        public string StyleName => "Material";

        public void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid)
        {
            if (g == null || grid == null || headerRect.Width <= 0 || headerRect.Height <= 0)
                return;

            var theme = GetTheme(grid);

            // Use BeepStyling for background (respects grid.ControlStyle)
            BeepStyling.CurrentTheme = theme;
            BeepStyling.UseThemeColors = grid.UseThemeColors;
            BeepStyling.PaintStyleBackground(g, headerRect, grid.ControlStyle);

            // Material elevation shadow (subtle)
            DrawMaterialElevation(g, headerRect, 2);

            // Layout: [Title] ................. [Search] [Filter] [Add]
            int y = headerRect.Top + (headerRect.Height - ButtonHeight) / 2;
            
            // Left: Title
            DrawTitle(g, new Rectangle(headerRect.Left + Padding, y, 200, ButtonHeight), 
                     "Data Table", theme);

            // Right: Action buttons
            int rightX = headerRect.Right - Padding;
            rightX = DrawFabButton(g, rightX, y, "+ Add", grid, theme);
            rightX = DrawIconButton(g, rightX - Spacing, y, "filter_list", grid, theme);
            rightX = DrawSearchBox(g, rightX - Spacing, y, grid, theme);
        }

        public void PaintHeaderCell(Graphics g, Rectangle cellRect, 
                                   Models.BeepColumnConfig column, 
                                   int columnIndex, BeepGridPro grid)
        {
            // Headers handled by grid itself
        }

        public void RegisterHeaderHitAreas(BeepGridPro grid)
        {
            // Registered during painting
        }

        private void DrawMaterialElevation(Graphics g, Rectangle rect, int elevation)
        {
            // Material Design elevation shadows
            int offset = elevation;
            int blur = elevation * 2;
            
            using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            {
                var shadowRect = new Rectangle(rect.X, rect.Y + offset, 
                                              rect.Width, rect.Height);
                g.FillRectangle(shadowBrush, shadowRect);
            }
        }

        private void DrawTitle(Graphics g, Rectangle rect, string title, IBeepTheme theme)
        {
            using (var font = new Font("Roboto", 20, FontStyle.Medium))
            using (var brush = new SolidBrush(theme.ForeColor))
            {
                g.DrawString(title, font, brush, rect, 
                    new StringFormat { LineAlignment = StringAlignment.Center });
            }
        }

        private int DrawFabButton(Graphics g, int rightX, int y, string text, 
                                 BeepGridPro grid, IBeepTheme theme)
        {
            int width = 100;
            var rect = new Rectangle(rightX - width, y, width, ButtonHeight);

            // Material FAB style - rounded corners, primary color
            using (var path = CreateRoundedRect(rect, ButtonHeight / 2))
            using (var brush = new SolidBrush(theme.PrimaryColor))
            using (var font = new Font("Roboto", 10, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            {
                g.FillPath(brush, path);
                
                var textRect = new Rectangle(rect.X + IconSize, rect.Y, 
                                            rect.Width - IconSize, rect.Height);
                g.DrawString(text, font, textBrush, textRect,
                    new StringFormat { 
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Center 
                    });
            }

            RegisterButtonHitArea(grid, "AddNew", rect, GridPainterEvents.NavInsert);
            return rightX - width;
        }

        private int DrawIconButton(Graphics g, int rightX, int y, string icon, 
                                   BeepGridPro grid, IBeepTheme theme)
        {
            int size = 40; // Touch target size (Material minimum 48dp, but 40px here)
            var rect = new Rectangle(rightX - size, y, size, size);

            // Material icon button - circular ripple area
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                
                // Subtle background on hover (would track hover state)
                using (var brush = new SolidBrush(Color.FromArgb(10, theme.ForeColor)))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw icon (simplified - real implementation would use Material Icons)
            using (var font = new Font("Material Icons", 18))
            using (var brush = new SolidBrush(theme.ForeColor))
            {
                g.DrawString(icon, font, brush, rect,
                    new StringFormat { 
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Center 
                    });
            }

            RegisterButtonHitArea(grid, "Filter", rect, GridPainterEvents.NavFilter);
            return rightX - size;
        }

        private int DrawSearchBox(Graphics g, int rightX, int y, 
                                 BeepGridPro grid, IBeepTheme theme)
        {
            int width = 240;
            var rect = new Rectangle(rightX - width, y, width, ButtonHeight);

            // Material filled text field style
            using (var path = CreateRoundedRect(rect, 4))
            using (var brush = new SolidBrush(Color.FromArgb(15, theme.ForeColor)))
            using (var font = new Font("Roboto", 10))
            using (var textBrush = new SolidBrush(Color.FromArgb(150, theme.ForeColor)))
            {
                g.FillPath(brush, path);
                
                // Search icon + placeholder
                var textRect = new Rectangle(rect.X + Padding + IconSize, rect.Y, 
                                            rect.Width - Padding - IconSize, rect.Height);
                g.DrawString("Search...", font, textBrush, textRect,
                    new StringFormat { LineAlignment = StringAlignment.Center });
            }

            RegisterButtonHitArea(grid, "Search", rect, "Search");
            return rightX - width;
        }

        private GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }

        private void RegisterButtonHitArea(BeepGridPro grid, string name, 
                                          Rectangle rect, string eventName)
        {
            grid.AddHitArea(name, rect, null, () =>
            {
                grid.PainterEvents?.TriggerEvent(eventName, 
                    new GridPainterEventArgs { EventName = eventName });
            });
        }

        private IBeepTheme GetTheme(BeepGridPro grid)
        {
            return grid.Theme != null 
                ? BeepThemesManager.GetTheme(grid.Theme) 
                : BeepThemesManager.GetDefaultTheme();
        }
    }
}
```

### Example: Bootstrap Navigation Painter

```csharp
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters.Navigation
{
    /// <summary>
    /// Bootstrap 5 inspired pagination painter
    /// </summary>
    public class BootstrapNavigationPainter : IPaintGridNavigation
    {
        private const int NavHeight = 50;
        private const int ButtonSize = 36;
        private const int Padding = 12;
        private const int Spacing = 4;

        public string StyleName => "Bootstrap";
        
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _totalRecords = 0;

        public void PaintNavigation(Graphics g, Rectangle navigationRect, BeepGridPro grid)
        {
            if (g == null || grid == null || navigationRect.IsEmpty)
                return;

            var theme = GetTheme(grid);

            // Use BeepStyling for background
            BeepStyling.CurrentTheme = theme;
            BeepStyling.UseThemeColors = grid.UseThemeColors;
            BeepStyling.PaintStyleBackground(g, navigationRect, grid.ControlStyle);

            // Bootstrap border
            using (var pen = new Pen(Color.FromArgb(222, 226, 230)))
            {
                g.DrawLine(pen, navigationRect.Left, navigationRect.Top, 
                          navigationRect.Right, navigationRect.Top);
            }

            DrawPagination(g, navigationRect, grid, theme);
        }

        private void DrawPagination(Graphics g, Rectangle rect, 
                                   BeepGridPro grid, IBeepTheme theme)
        {
            int y = rect.Top + (rect.Height - ButtonSize) / 2;
            int centerX = rect.Left + rect.Width / 2;

            // Calculate pagination width
            int buttonCount = Math.Min(5, _totalPages) + 2; // Pages + First/Last
            int totalWidth = buttonCount * ButtonSize + (buttonCount - 1) * Spacing;
            int startX = centerX - totalWidth / 2;

            // Previous button
            startX = DrawBootstrapButton(g, startX, y, "‹", false, grid, 
                                        GridPainterEvents.NavPrevious, theme);
            startX += Spacing;

            // Page numbers
            for (int i = 1; i <= Math.Min(5, _totalPages); i++)
            {
                bool isActive = i == _currentPage;
                startX = DrawBootstrapButton(g, startX, y, i.ToString(), isActive, 
                                            grid, $"GoToPage{i}", theme);
                startX += Spacing;
            }

            // Next button
            DrawBootstrapButton(g, startX, y, "›", false, grid, 
                              GridPainterEvents.NavNext, theme);

            // Right: Record info
            string info = $"Showing {_currentPage} of {_totalPages} pages";
            using (var font = new Font("Segoe UI", 9))
            using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
            {
                var infoSize = g.MeasureString(info, font);
                var infoRect = new Rectangle(
                    (int)(rect.Right - Padding - infoSize.Width), 
                    y, 
                    (int)infoSize.Width, 
                    ButtonSize);
                
                g.DrawString(info, font, brush, infoRect,
                    new StringFormat { LineAlignment = StringAlignment.Center });
            }
        }

        private int DrawBootstrapButton(Graphics g, int x, int y, string text, 
                                       bool isActive, BeepGridPro grid, 
                                       string eventName, IBeepTheme theme)
        {
            var rect = new Rectangle(x, y, ButtonSize, ButtonSize);

            // Bootstrap button colors
            Color bgColor = isActive 
                ? Color.FromArgb(13, 110, 253)    // btn-primary
                : Color.FromArgb(255, 255, 255);   // btn-light
            
            Color borderColor = Color.FromArgb(13, 110, 253);
            Color textColor = isActive 
                ? Color.White 
                : Color.FromArgb(13, 110, 253);

            // Background
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(borderColor))
            {
                g.DrawRectangle(pen, rect);
            }

            // Text
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var textBrush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, textBrush, rect,
                    new StringFormat { 
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Center 
                    });
            }

            RegisterButtonHitArea(grid, $"Page{text}", rect, eventName);
            return x + ButtonSize;
        }

        public void RegisterNavigationHitAreas(BeepGridPro grid)
        {
            // Registered during painting
        }

        public void UpdatePageInfo(int currentPage, int totalPages, int totalRecords)
        {
            _currentPage = currentPage;
            _totalPages = totalPages;
            _totalRecords = totalRecords;
        }

        public int GetPreferredHeight()
        {
            return NavHeight;
        }

        private void RegisterButtonHitArea(BeepGridPro grid, string name, 
                                          Rectangle rect, string eventName)
        {
            grid.AddHitArea(name, rect, null, () =>
            {
                grid.PainterEvents?.TriggerEvent(eventName, 
                    new GridPainterEventArgs { EventName = eventName });
            });
        }

        private IBeepTheme GetTheme(BeepGridPro grid)
        {
            return grid.Theme != null 
                ? BeepThemesManager.GetTheme(grid.Theme) 
                : BeepThemesManager.GetDefaultTheme();
        }
    }
}
```

## Using Styled Painters

### Setting Control Style

```csharp
var grid = new BeepGridPro
{
    Dock = DockStyle.Fill,
    Theme = "BusinessProfessional",
    ControlStyle = BeepControlStyle.Material3, // Set the visual style
    UseThemeColors = true                       // Use theme colors
};

// Set custom painters
grid.HeaderPainter = new MaterialHeaderPainter();
grid.NavigationPainter = new BootstrapNavigationPainter();
```

### Changing Style at Runtime

```csharp
// Switch to iOS style
grid.ControlStyle = BeepControlStyle.iOS15;
grid.Invalidate(); // Painters will automatically use new style

// Switch to dark glow effect
grid.ControlStyle = BeepControlStyle.DarkGlow;
grid.UseThemeColors = false; // Use custom colors instead
grid.Invalidate();
```

### Responding to Style Changes

```csharp
// Subscribe to grid property changes
grid.PropertyChanged += (s, e) =>
{
    if (e.PropertyName == nameof(grid.ControlStyle))
    {
        // Custom logic when style changes
        UpdatePainterOptions();
    }
};
```

## Best Practices

### 1. Always Set BeepStyling Context

```csharp
// At the start of your Paint method
BeepStyling.CurrentTheme = theme;
BeepStyling.UseThemeColors = grid.UseThemeColors;
```

### 2. Respect Control Style

```csharp
// Use grid's ControlStyle for backgrounds
BeepStyling.PaintStyleBackground(g, rect, grid.ControlStyle);

// Get style-specific properties
int radius = StyleBorders.GetRadius(grid.ControlStyle);
bool hasShadow = StyleShadows.HasShadow(grid.ControlStyle);
```

### 3. Theme Color Integration

```csharp
// Get theme colors properly
Color primaryColor = grid.UseThemeColors 
    ? theme.PrimaryColor 
    : Color.FromArgb(13, 110, 253); // Bootstrap blue

Color textColor = grid.UseThemeColors
    ? theme.ForeColor
    : Color.FromArgb(33, 37, 41); // Bootstrap dark text
```

### 4. Consistent Spacing

```csharp
// Use spacing constants based on design system
// Material: 8dp grid (8, 16, 24, etc.)
// Bootstrap: 4px increments (4, 8, 12, 16, etc.)
// iOS: 8pt grid
const int MaterialSpacing = 8;
const int BootstrapSpacing = 4;
```

### 5. Responsive Heights

```csharp
public int GetPreferredHeight()
{
    // Return height based on control style
    return grid.ControlStyle switch
    {
        BeepControlStyle.Material3 => 56,      // Material app bar
        BeepControlStyle.iOS15 => 44,          // iOS toolbar
        BeepControlStyle.Bootstrap => 50,      // Bootstrap navbar
        BeepControlStyle.Minimal => 36,        // Compact
        _ => 48                                // Default
    };
}
```

## Style-Specific Features

### Material Design
- **Elevation**: Subtle shadows (2dp, 4dp, 8dp)
- **Typography**: Roboto font
- **Icons**: Material Icons
- **Colors**: Primary, Secondary, Surface
- **Ripple**: Touch feedback effects

### iOS/macOS
- **Blur**: Translucent backgrounds
- **SF Symbols**: Apple icon set
- **Typography**: SF Pro font
- **Spacing**: 8pt grid
- **Animations**: Spring curves

### Bootstrap
- **Grid System**: 12-column responsive
- **Utilities**: Border, shadow, spacing
- **Typography**: Segoe UI / System font
- **Colors**: Primary, Secondary, Success, Danger
- **Components**: Buttons, badges, alerts

### Neumorphism
- **Dual Shadows**: Light and dark
- **Soft UI**: Pressed/raised effects
- **Monochrome**: Limited color palette
- **Depth**: Subtle 3D appearance

## Performance Tips

### 1. Cache Drawing Objects

```csharp
// Cache brushes and pens
private SolidBrush _cachedPrimaryBrush;

private Brush GetPrimaryBrush(IBeepTheme theme)
{
    if (_cachedPrimaryBrush == null || 
        _cachedPrimaryBrush.Color != theme.PrimaryColor)
    {
        _cachedPrimaryBrush?.Dispose();
        _cachedPrimaryBrush = new SolidBrush(theme.PrimaryColor);
    }
    return _cachedPrimaryBrush;
}
```

### 2. Minimize Path Creation

```csharp
// Reuse paths when possible
using (var path = CreateRoundedRect(rect, radius))
{
    g.FillPath(brush1, path);
    g.DrawPath(pen1, path);
    // Multiple operations on same path
}
```

### 3. Batch Similar Operations

```csharp
// Group all fills together, then all strokes
foreach (var button in buttons)
{
    g.FillRectangle(brush, button.Rect);
}
foreach (var button in buttons)
{
    g.DrawRectangle(pen, button.Rect);
}
```

## Troubleshooting

### Painter Not Showing Styled Background

**Problem**: Background appears flat/default
**Solution**: Ensure BeepStyling context is set before calling PaintStyleBackground

```csharp
// WRONG - Missing context
BeepStyling.PaintStyleBackground(g, rect, style);

// CORRECT - Set context first
BeepStyling.CurrentTheme = theme;
BeepStyling.UseThemeColors = grid.UseThemeColors;
BeepStyling.PaintStyleBackground(g, rect, grid.ControlStyle);
```

### Colors Don't Match Theme

**Problem**: Custom colors instead of theme colors
**Solution**: Check UseThemeColors flag

```csharp
// Ensure UseThemeColors is true for theme integration
grid.UseThemeColors = true;
```

### Style Changes Not Reflecting

**Problem**: Changing ControlStyle doesn't update visuals
**Solution**: Call Invalidate() after style changes

```csharp
grid.ControlStyle = BeepControlStyle.Material3;
grid.Invalidate(); // Force repaint
```

## Complete Example: Custom Ant Design Painter

See the implementation files for complete examples of:
- `MaterialHeaderPainter.cs`
- `AntDesignHeaderPainter.cs`
- `BootstrapNavigationPainter.cs`
- `MinimalNavigationPainter.cs`

---

**Next Steps:**
1. Review the existing painter implementations
2. Test different ControlStyle values
3. Create custom painters for your specific needs
4. Integrate with the PainterEvents system for interactivity

**Related Documentation:**
- [Painter System Overview](PainterSystem.md)
- [Plan: Header Navigation Painters](PlanHeaderNavigationPainters.md)
- [Painter Integration Fix](PAINTER_SYSTEM_INTEGRATION_FIX.md)
- [BeepStyling Documentation](../../Styling/README.md)
