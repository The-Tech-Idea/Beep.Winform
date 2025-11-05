# Layouts - Layout Preset System

## Overview

The Layouts folder contains layout preset implementations that define the structural and spacing characteristics of BeepGridPro. Layout presets control dimensions, spacing, borders, stripes, header effects, and other non-color visual aspects. They are completely separate from themes (which control colors) and grid styles (which are runtime-applied).

## Key Concepts

### Layout Presets vs Grid Styles vs Themes

- **Layout Presets**: Structural configuration (spacing, borders, row height, header effects)
- **Grid Styles**: Runtime visual styling (Material, Bootstrap, Compact, etc.) - applied via BeepGridStyle enum
- **Themes**: Color schemes (light, dark, custom) - applied via BeepThemesManager

**Example Flow**:
```csharp
grid.Theme = BeepTheme.MaterialDark;        // Colors from theme
grid.GridStyle = BeepGridStyle.Material;     // Visual effects
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;  // Structure
```

## IGridLayoutPreset Interface

All layout presets implement this simple interface:

```csharp
public interface IGridLayoutPreset
{
    void Apply(BeepGridPro grid);
}
```

The `Apply` method configures the grid's structural properties. Implementation pattern:

```csharp
public sealed class MyLayoutHelper : IGridLayoutPreset
{
    public void Apply(BeepGridPro grid)
    {
        if (grid == null) return;

        // 1. Set dimensions
        grid.RowHeight = 28;
        grid.ColumnHeaderHeight = 32;
        grid.ShowColumnHeaders = true;

        // 2. Configure Render properties
        grid.Render.ShowGridLines = true;
        grid.Render.GridLineStyle = DashStyle.Solid;
        grid.Render.ShowRowStripes = false;
        grid.Render.UseHeaderGradient = true;
        grid.Render.UseHeaderHoverEffects = true;
        grid.Render.UseBoldHeaderText = false;
        grid.Render.HeaderCellPadding = 4;
        grid.Render.UseElevation = false;
        grid.Render.CardStyle = false;

        // 3. Apply alignment heuristics
        LayoutCommon.ApplyAlignmentHeuristics(grid);
    }
}
```

## Available Layout Presets

### 1. Default
**Preset**: `GridLayoutPreset.Default`  
**Class**: `DefaultTableLayoutHelper`

**Characteristics**:
- Row Height: 24px
- Column Header Height: 26px
- Grid lines: Solid
- Row stripes: Disabled
- Header gradient: Disabled
- Header hover: Enabled
- Bold header text: Disabled
- Header padding: 2px
- Elevation: Disabled

**Use Case**: General-purpose grids, standard data tables

---

### 2. Clean
**Preset**: `GridLayoutPreset.Clean`  
**Class**: `CleanTableLayoutHelper`

**Characteristics**:
- Row Height: 26px
- Column Header Height: 28px
- Grid lines: Solid
- Row stripes: Disabled
- Header gradient: Disabled
- Header hover: Enabled
- Bold header text: Disabled
- Header padding: 3px
- Subtle, minimal styling

**Use Case**: Modern applications, clean interfaces, professional dashboards

---

### 3. Dense
**Preset**: `GridLayoutPreset.Dense`  
**Class**: `DenseTableLayoutHelper`

**Characteristics**:
- Row Height: 20px (compact)
- Column Header Height: 24px
- Grid lines: Solid
- Row stripes: Disabled
- Header gradient: Disabled
- Header hover: Disabled
- Bold header text: Disabled
- Header padding: 1px
- Maximum data density

**Use Case**: Data-heavy displays, admin panels, reports, space-constrained UIs

---

### 4. Striped
**Preset**: `GridLayoutPreset.Striped`  
**Class**: `StripedTableLayoutHelper`

**Characteristics**:
- Row Height: 24px
- Column Header Height: 26px
- Grid lines: Solid
- Row stripes: **Enabled** (zebra striping)
- Header gradient: Disabled
- Header hover: Enabled
- Bold header text: Disabled
- Header padding: 2px
- Alternating row colors

**Use Case**: Improved readability for wide tables, data analysis, reports

---

### 5. Borderless
**Preset**: `GridLayoutPreset.Borderless`  
**Class**: `BorderlessTableLayoutHelper`

**Characteristics**:
- Row Height: 24px
- Column Header Height: 26px
- Grid lines: **Disabled**
- Row stripes: Disabled
- Header gradient: Disabled
- Header hover: Disabled
- Bold header text: Disabled
- Header padding: 2px
- Clean, minimal borders

**Use Case**: Modern UIs, card-based layouts, embedded grids

---

### 6. HeaderBold
**Preset**: `GridLayoutPreset.HeaderBold`  
**Class**: `HeaderBoldTableLayoutHelper`

**Characteristics**:
- Row Height: 24px
- Column Header Height: 28px
- Grid lines: Solid
- Row stripes: Disabled
- Header gradient: Disabled
- Header hover: Enabled
- Bold header text: **Enabled**
- Header padding: 3px
- Emphasized headers

**Use Case**: Important tables, primary data displays, reports

---

### 7. MaterialHeader
**Preset**: `GridLayoutPreset.MaterialHeader`  
**Class**: `MaterialHeaderTableLayoutHelper`

**Characteristics**:
- Row Height: 28px
- Column Header Height: 32px (taller)
- Grid lines: Solid
- Row stripes: Disabled
- Header gradient: **Enabled**
- Header hover: Enabled
- Bold header text: Disabled
- Header padding: 4px (generous)
- Elevation: **Enabled**
- Material Design inspired

**Use Case**: Modern applications, Material Design apps, elevated UI

---

### 8. Card
**Preset**: `GridLayoutPreset.Card`  
**Class**: `CardTableLayoutHelper`

**Characteristics**:
- Row Height: 28px
- Column Header Height: 32px
- Grid lines: Disabled
- Row stripes: Disabled
- Header gradient: **Enabled**
- Header hover: Enabled
- Bold header text: Disabled
- Header padding: 4px
- Elevation: **Enabled**
- Card style: **Enabled**
- Card-based appearance

**Use Case**: Dashboard cards, modern UIs, elevated content

---

### 9. ComparisonTable
**Preset**: `GridLayoutPreset.ComparisonTable`  
**Class**: `ComparisonTableLayoutHelper`

**Characteristics**:
- Row Height: 26px
- Column Header Height: 30px
- Grid lines: Solid
- Row stripes: Disabled
- Header gradient: **Enabled**
- Header hover: Enabled
- Bold header text: **Enabled**
- Header padding: 3px
- Strong header emphasis
- Clear visual hierarchy

**Use Case**: Product comparisons, feature matrices, spec sheets

---

### 10. MatrixSimple
**Preset**: `GridLayoutPreset.MatrixSimple`  
**Class**: `MatrixSimpleTableLayoutHelper`

**Characteristics**:
- Row Height: 24px
- Column Header Height: 26px
- Grid lines: Solid
- Row stripes: Disabled
- Header gradient: Disabled
- Header hover: Enabled
- Bold header text: Disabled
- Header padding: 2px
- Centered cells by default
- Simple matrix display

**Use Case**: Data matrices, cross-reference tables, correlation tables

---

### 11. MatrixStriped
**Preset**: `GridLayoutPreset.MatrixStriped`  
**Class**: `MatrixStripedTableLayoutHelper`

**Characteristics**:
- Row Height: 24px
- Column Header Height: 26px
- Grid lines: Solid
- Row stripes: **Enabled**
- Header gradient: Disabled
- Header hover: Enabled
- Bold header text: Disabled
- Header padding: 2px
- Centered cells by default
- Striped matrix display

**Use Case**: Complex matrices, improved row tracking, data correlation

---

### 12. PricingTable
**Preset**: `GridLayoutPreset.PricingTable`  
**Class**: `PricingTableLayoutHelper`

**Characteristics**:
- Row Height: 28px (taller)
- Column Header Height: 32px
- Grid lines: Solid
- Row stripes: Disabled
- Header gradient: **Enabled**
- Header hover: Enabled
- Bold header text: **Enabled**
- Header padding: 4px
- Elevation: **Enabled**
- Feature-centric layout
- Centered cells

**Use Case**: Pricing comparisons, subscription tiers, package features

## LayoutCommon Class

Shared utility class providing common layout operations.

### ApplyAlignmentHeuristics

Intelligently sets column alignment based on column type and name:

```csharp
public static void ApplyAlignmentHeuristics(BeepGridPro grid)
```

**Rules**:
1. **System columns** (checkbox, row number, row ID): Center aligned
2. **First visible non-system column**: Left aligned
3. **Status/Action/Price/Quantity columns**: Center aligned (detected by name)
4. **All other columns**: Left aligned (default for readability)

**Example**:
```
| ☑ | #  | Product Name    | Status  | Price  | Actions |
|---|----|-----------------+---------+--------+---------|
| C | C  | L               | C       | C      | C       |

C = Center, L = Left
```

This heuristic improves readability and follows common UI conventions.

## GridLayoutPreset Enum

```csharp
public enum GridLayoutPreset
{
    Default,            // Standard layout
    Clean,              // Clean, minimal
    Dense,              // Maximum density
    Striped,            // Zebra striping
    Borderless,         // No borders
    HeaderBold,         // Bold headers
    MaterialHeader,     // Material Design
    Card,               // Card-based
    ComparisonTable,    // Product comparison
    MatrixSimple,       // Simple matrix
    MatrixStriped,      // Striped matrix
    PricingTable        // Pricing display
}
```

## Applying Layout Presets

### Method 1: Via Enum (Recommended)

```csharp
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
```

This internally calls:
```csharp
public void ApplyLayoutPreset(GridLayoutPreset preset)
{
    IGridLayoutPreset impl = preset switch
    {
        GridLayoutPreset.MaterialHeader => new MaterialHeaderTableLayoutHelper(),
        GridLayoutPreset.Dense => new DenseTableLayoutHelper(),
        // ... etc
        _ => new DefaultTableLayoutHelper()
    };
    this.ApplyLayoutPreset(impl);
}
```

### Method 2: Direct Application

```csharp
var layout = new MaterialHeaderTableLayoutHelper();
grid.ApplyLayoutPreset(layout);
```

### Method 3: Extension Method

```csharp
// Using extension methods from GridLayoutPresetExtensions
grid.ApplyLayoutPreset(new CustomLayoutHelper());
```

## Creating Custom Layout Presets

### Step 1: Create Layout Helper Class

```csharp
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;

namespace MyApp.CustomLayouts
{
    public sealed class MyCustomLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Configure dimensions
            grid.RowHeight = 30;
            grid.ColumnHeaderHeight = 36;
            grid.ShowColumnHeaders = true;

            // Configure visual properties
            grid.Render.ShowGridLines = true;
            grid.Render.GridLineStyle = DashStyle.Dot;
            grid.Render.ShowRowStripes = true;
            grid.Render.UseHeaderGradient = true;
            grid.Render.UseHeaderHoverEffects = true;
            grid.Render.UseBoldHeaderText = true;
            grid.Render.HeaderCellPadding = 6;
            grid.Render.UseElevation = true;
            grid.Render.CardStyle = false;

            // Apply intelligent alignment
            LayoutCommon.ApplyAlignmentHeuristics(grid);
            
            // Custom column-specific overrides
            var statusColumn = grid.Data.Columns
                .FirstOrDefault(c => c.ColumnName == "Status");
            if (statusColumn != null)
            {
                statusColumn.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                statusColumn.CellTextAlignment = ContentAlignment.MiddleCenter;
            }
        }
    }
}
```

### Step 2: Add to Enum (Optional)

If you want to add it to the built-in enum:

```csharp
public enum GridLayoutPreset
{
    // ... existing values
    Custom          // Add your preset
}
```

And register in `ApplyLayoutPreset`:

```csharp
case GridLayoutPreset.Custom:
    return new MyCustomLayoutHelper();
```

### Step 3: Apply Your Layout

```csharp
// Method 1: Direct
grid.ApplyLayoutPreset(new MyCustomLayoutHelper());

// Method 2: Via enum (if registered)
grid.LayoutPreset = GridLayoutPreset.Custom;
```

## Layout Configuration Properties

### Dimensions
```csharp
grid.RowHeight = 24;                    // Row height in pixels
grid.ColumnHeaderHeight = 26;            // Header height in pixels
grid.ShowColumnHeaders = true;           // Show/hide headers
```

### Grid Lines
```csharp
grid.Render.ShowGridLines = true;                        // Enable/disable
grid.Render.GridLineStyle = DashStyle.Solid;             // Solid, Dot, Dash, etc.
```

### Row Appearance
```csharp
grid.Render.ShowRowStripes = true;       // Alternating row colors
```

### Header Styling
```csharp
grid.Render.UseHeaderGradient = true;    // Gradient background
grid.Render.UseHeaderHoverEffects = true;// Hover highlighting
grid.Render.UseBoldHeaderText = true;    // Bold header text
grid.Render.HeaderCellPadding = 4;       // Padding in pixels
```

### Advanced Effects
```csharp
grid.Render.UseElevation = true;         // Shadow/depth effect
grid.Render.CardStyle = true;            // Card-based appearance
```

## Layout vs Style Interaction

### How They Work Together

```csharp
// Step 1: Apply theme (colors)
grid.Theme = BeepTheme.MaterialDark;

// Step 2: Apply layout preset (structure)
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
// Sets: row heights, padding, header effects

// Step 3: Apply grid Style (visual effects - optional)
grid.GridStyle = BeepGridStyle.Material;
// May override some layout settings for consistency

// Result: MaterialDark theme + MaterialHeader structure + Material effects
```

### Precedence

1. **Layout Preset**: Applied first, sets base structure
2. **Grid Style**: Applied second, may override for consistency
3. **Manual overrides**: Applied last, highest priority

```csharp
grid.LayoutPreset = GridLayoutPreset.Dense;  // Sets RowHeight = 20
grid.GridStyle = BeepGridStyle.Material;     // May change to 28 for Material look
grid.RowHeight = 32;                         // Manual override wins
```

## Best Practices

### 1. Use Appropriate Presets

Match preset to use case:
- **Dense**: Admin panels, data-heavy screens
- **Card**: Dashboards, modern UIs
- **Striped**: Wide tables, many columns
- **Clean**: Professional applications
- **MaterialHeader**: Material Design apps

### 2. Combine with Themes

```csharp
// Light theme + clean layout
grid.Theme = BeepTheme.LightMode;
grid.LayoutPreset = GridLayoutPreset.Clean;

// Dark theme + material layout
grid.Theme = BeepTheme.DarkMode;
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
```

### 3. Don't Mix Too Many Styles

❌ **Bad**:
```csharp
grid.LayoutPreset = GridLayoutPreset.Minimal;
grid.GridStyle = BeepGridStyle.Material;  // Conflict!
```

✅ **Good**:
```csharp
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
grid.GridStyle = BeepGridStyle.Material;  // Consistent!
```

### 4. Test Responsive Behavior

```csharp
// Ensure layout works with different:
- Window sizes
- DPI settings
- Font sizes
- Theme changes
```

### 5. Use LayoutCommon Helpers

```csharp
// Apply standard alignment heuristics
LayoutCommon.ApplyAlignmentHeuristics(grid);

// Then customize specific columns
var priceColumn = grid.Data.Columns.First(c => c.ColumnName == "Price");
priceColumn.CellTextAlignment = ContentAlignment.MiddleRight;
```

## Performance Considerations

1. **Layout application is lightweight** - Just property assignments
2. **Recalculation triggered automatically** - via `Layout.Recalculate()`
3. **No performance difference** between presets
4. **Apply before binding data** for best results

```csharp
// Good: Apply layout first
grid.LayoutPreset = GridLayoutPreset.Dense;
grid.DataSource = myData;

// Also fine: Apply after (triggers invalidation)
grid.DataSource = myData;
grid.LayoutPreset = GridLayoutPreset.Dense;
```

## Troubleshooting

### Layout not applying?

```csharp
// Force recalculation
grid.Layout.Recalculate();
grid.Invalidate();
```

### Grid Style overriding layout?

```csharp
// Apply layout AFTER grid Style
grid.GridStyle = BeepGridStyle.Material;
grid.LayoutPreset = GridLayoutPreset.Custom;  // Will override Material settings
```

### Custom spacing not working?

```csharp
// Some properties must be set on Render helper
grid.Render.HeaderCellPadding = 10;  // ✅ Correct
grid.HeaderCellPadding = 10;         // ❌ Property doesn't exist
```

## Future Enhancements

See [ENHANCEMENT_PLAN.md](../ENHANCEMENT_PLAN.md) for planned improvements:

1. **Column Header Painters per Layout**: Each layout can have its own header painter
2. **Navigation Painters per Layout**: Each layout can specify navigation style
3. **Per-Layout Calculations**: Column header height and navigation height calculated by layout
4. **Advanced Layout API**: More control over layout behavior
5. **Layout Templates**: Save and load custom layout configurations

## Related Documentation

- [../README.md](../README.md) - Main BeepGridPro documentation
- [../Painters/README.md](../Painters/README.md) - Painter pattern documentation
- [../Helpers/README.md](../Helpers/README.md) - Helper classes documentation
- [ENHANCEMENT_PLAN.md](../ENHANCEMENT_PLAN.md) - Future development roadmap
