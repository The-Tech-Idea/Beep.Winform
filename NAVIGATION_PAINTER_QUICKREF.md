# Navigation Painter Quick Reference

## Quick Style Selection Guide

### Choose Your Style Based On:

**🎨 Visual Preference**
- **Flat & Modern**: Tailwind, Minimal, AntDesign
- **Professional**: Telerik, AGGrid, DataTables
- **Distinctive**: Material, Fluent, Card
- **Traditional**: Standard, Bootstrap
- **Space-Saving**: Compact, Minimal

**📏 Height Requirements**
- **Compact** (28px): When vertical space is limited
- **Minimal** (32px): Clean, minimal footprint
- **Standard** (40px): Traditional desktop sizing
- **Telerik/AGGrid** (44px): Professional balance
- **Tailwind/AntDesign/Bootstrap** (46-48px): Modern web sizing
- **Fluent** (52px): Rich visual experience
- **Material/DataTables** (50-56px): Generous spacing
- **Card** (60px): Maximum visual impact

**🎯 Use Case**
| Use Case | Recommended Style |
|----------|-------------------|
| Enterprise LOB App | AntDesign, Telerik |
| Windows 11 App | Fluent |
| Web-like Interface | Bootstrap, DataTables |
| Modern SaaS App | Material, Tailwind |
| Dense Data Display | Compact, Minimal |
| Advanced Data Grid | AGGrid, Telerik |
| Dashboard/Analytics | Card, Material |
| Traditional Desktop | Standard |

## One-Liner Setup Examples

```csharp
// Material Design (Google style)
grid.NavigationStyle = navigationStyle.Material;

// Bootstrap (Web table style)
grid.NavigationStyle = navigationStyle.Bootstrap;

// Fluent (Windows 11 style)
grid.NavigationStyle = navigationStyle.Fluent;

// Compact (DevExpress style)
grid.NavigationStyle = navigationStyle.Compact;

// Tailwind (Modern flat)
grid.NavigationStyle = navigationStyle.Tailwind;
```

## Common Patterns

### Default Setup (Recommended)
```csharp
beepGridPro1.UsePainterNavigation = true; // Enable modern painters
beepGridPro1.NavigationStyle = navigationStyle.Standard; // Default style
```

### Match Your Application Theme
```csharp
// Windows 11 app
beepGridPro1.NavigationStyle = navigationStyle.Fluent;

// Web-inspired app
beepGridPro1.NavigationStyle = navigationStyle.Bootstrap;

// Enterprise app
beepGridPro1.NavigationStyle = navigationStyle.AntDesign;

// Compact dashboard
beepGridPro1.NavigationStyle = navigationStyle.Compact;
```

### Runtime Style Switching
```csharp
private void ApplyStyleFromComboBox()
{
    beepGridPro1.NavigationStyle = cmbStyle.SelectedItem switch
    {
        "Material Design" => navigationStyle.Material,
        "Bootstrap" => navigationStyle.Bootstrap,
        "Fluent Design" => navigationStyle.Fluent,
        "Compact" => navigationStyle.Compact,
        _ => navigationStyle.Standard
    };
}
```

## Visual Comparison

### Icon vs Text Buttons
- **Icons Only**: Material, Fluent, Compact, Telerik, Card
- **Text Only**: Standard, DataTables
- **Mixed**: Bootstrap, Minimal, AntDesign, AGGrid, Tailwind

### Color Schemes
- **Blue Accents**: Material (#2196F3), Bootstrap (#007bff), AntDesign (#1890FF), Telerik (#007BFF), DataTables (#337AB7)
- **Indigo Accents**: Card (#6366F1), Tailwind (#6366F1)
- **Windows Accent**: Fluent (system accent color)
- **Neutral**: Standard (system colors), Compact (grays), Minimal (minimal color)

### Layout Patterns
- **Left-Center-Right**: Standard, AntDesign, DataTables
- **Center-Focused**: Minimal, Bootstrap, Tailwind
- **Card Sections**: Card (3 separate cards)
- **Inline Compact**: Compact (all controls in single line)
- **Page Numbers**: Bootstrap, AntDesign, AGGrid, DataTables, Minimal, Tailwind

## Properties Summary

```csharp
// BeepGridPro Properties
public navigationStyle NavigationStyle { get; set; }  // Choose style
public bool UsePainterNavigation { get; set; }        // Enable/disable painters

// GridNavigationPainterHelper Properties
public navigationStyle NavigationStyle { get; set; }  // Current style
public bool UsePainterNavigation { get; set; }        // Painter mode toggle

// Read-only
public int GetRecommendedNavigatorHeight()           // Get height for current style
```

## Designer Support

In Visual Studio Designer:

1. Select BeepGridPro control
2. Find "NavigationStyle" in Properties window (Appearance category)
3. Choose from dropdown:
   - None
   - Standard ⭐
   - Material
   - Compact
   - Minimal
   - Bootstrap
   - Fluent
   - AntDesign
   - Telerik
   - AGGrid
   - DataTables
   - Card
   - Tailwind

## Troubleshooting

**Navigation not showing?**
```csharp
// Ensure painter navigation is enabled
beepGridPro1.UsePainterNavigation = true;

// Ensure navigator area has space in layout
// Check Layout.NavigatorRect is not empty
```

**Want legacy button navigation?**
```csharp
beepGridPro1.UsePainterNavigation = false;
```

**Style not updating?**
```csharp
// Force refresh after style change
beepGridPro1.NavigationStyle = navigationStyle.Material;
beepGridPro1.Invalidate();
beepGridPro1.Refresh();
```

## Performance Tips

- Painter instances are cached - style switching is fast
- Layout calculated once per paint cycle
- Hit areas registered efficiently
- No external dependencies - pure GDI+ rendering

## Common Customizations

### Change Navigation Height
```csharp
// Get recommended height for style
int height = NavigationPainterFactory.GetRecommendedHeight(navigationStyle.Material);

// Use in layout calculations
// (GridLayoutHelper will use this automatically when painters are enabled)
```

### Hide Navigation
```csharp
// Set style to None (still uses Standard painter)
beepGridPro1.NavigationStyle = navigationStyle.None;

// Or use legacy mode and hide
beepGridPro1.UsePainterNavigation = false;
// (legacy mode has its own visibility controls)
```

## API Quick Reference

```csharp
// Factory methods
var painter = NavigationPainterFactory.CreatePainter(navigationStyle.Material);
int height = NavigationPainterFactory.GetRecommendedHeight(navigationStyle.Material);
int minWidth = NavigationPainterFactory.GetRecommendedMinWidth(navigationStyle.Material);

// Painter interface
painter.PaintNavigation(g, bounds, grid, theme);
painter.PaintButton(g, buttonRect, buttonType, state, component, theme);
painter.PaintPositionIndicator(g, rect, currentPos, total, theme);
var layout = painter.CalculateLayout(availableBounds, totalRecords, showCrud);

// Grid integration
grid.ClearHitList();  // Clear existing hit areas
grid.AddHitArea(name, rect, component, action);  // Register clickable area
```

## Style Selection Decision Tree

```
Need traditional Windows look?
└─ Yes → Standard

Need minimal vertical space?
├─ Ultra-compact → Compact (28px)
└─ Minimal → Minimal (32px)

Building Windows 11 app?
└─ Yes → Fluent

Need web-like table?
├─ Classic pagination → Bootstrap, DataTables
└─ Modern → Tailwind, AntDesign

Need professional/enterprise?
└─ Yes → AntDesign, Telerik, AGGrid

Want distinctive/modern?
├─ Cards → Card
└─ Material → Material

Default/Balanced?
└─ Standard, Bootstrap, AntDesign
```

## Complete Style List

| # | Style | Height | Inspiration | Key Feature |
|---|-------|--------|-------------|-------------|
| 1 | Standard | 40px | Windows Forms | Classic 3D buttons |
| 2 | Material | 56px | Google Material | Circular hovers, shadows |
| 3 | Bootstrap | 48px | Bootstrap | Numbered pagination |
| 4 | Compact | 28px | DevExpress | Ultra-compact spacing |
| 5 | Minimal | 32px | Minimalist | No backgrounds |
| 6 | Fluent | 52px | Windows 11 | Acrylic, gradients |
| 7 | AntDesign | 48px | Ant Design | Clean borders |
| 8 | Telerik | 44px | Telerik/Kendo | Professional gradients |
| 9 | AGGrid | 44px | AG Grid | Page size dropdown |
| 10 | DataTables | 50px | jQuery DataTables | Connected buttons |
| 11 | Card | 60px | Modern cards | 3 card sections |
| 12 | Tailwind | 46px | Tailwind CSS | Flat design |

---
**Ready to use!** Just set `NavigationStyle` property and enjoy your new navigation! 🎉
