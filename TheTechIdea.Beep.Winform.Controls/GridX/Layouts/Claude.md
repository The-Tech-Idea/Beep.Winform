# Claude.md - Layouts Folder Guide

## Quick Reference

**Purpose**: Structural presets that configure grid dimensions, spacing, and visual properties (no colors)

**File Count**: 17 files (12 presets + 5 infrastructure)

**Key Pattern**: Strategy pattern via IGridLayoutPreset interface

## File Structure

```
Layouts/
├── Interface
│   └── IGridLayoutPreset.cs              - Layout preset contract
│
├── Enums
│   └── GridLayoutPreset.cs               - Enum of built-in presets
│
├── Infrastructure
│   ├── LayoutCommon.cs                   - Common alignment logic
│   ├── GridLayoutPresetExtensions.cs     - Extension methods
│   └── docs/                             - Documentation folder
│
└── Layout Implementations (12 presets)
    ├── DefaultTableLayoutHelper.cs       - Standard layout (24px rows)
    ├── CleanTableLayoutHelper.cs         - Clean, minimal (26px)
    ├── DenseTableLayoutHelper.cs         - Maximum density (20px)
    ├── StripedTableLayoutHelper.cs       - Zebra striping (24px)
    ├── BorderlessTableLayoutHelper.cs    - No borders (24px)
    ├── HeaderBoldTableLayoutHelper.cs    - Bold headers (24px)
    ├── MaterialHeaderTableLayoutHelper.cs - Material Design (28px)
    ├── CardTableLayoutHelper.cs          - Card-based (28px)
    ├── ComparisonTableLayoutHelper.cs    - Product comparison (26px)
    ├── MatrixSimpleTableLayoutHelper.cs  - Simple matrix (24px)
    ├── MatrixStripedTableLayoutHelper.cs - Striped matrix (24px)
    └── PricingTableLayoutHelper.cs       - Pricing table (28px)
```

## Core Interface

```csharp
public interface IGridLayoutPreset
{
    void Apply(BeepGridPro grid);
}
```

## Implementation Pattern

### Standard Implementation

```csharp
public sealed class MyLayoutHelper : IGridLayoutPreset
{
    public void Apply(BeepGridPro grid)
    {
        if (grid == null) return;

        // 1. DIMENSIONS
        grid.RowHeight = 26;
        grid.ColumnHeaderHeight = 30;
        grid.ShowColumnHeaders = true;

        // 2. GRID LINES
        grid.Render.ShowGridLines = true;
        grid.Render.GridLineStyle = DashStyle.Solid; // or Dot, Dash, etc.

        // 3. ROW STYLING
        grid.Render.ShowRowStripes = false; // Zebra striping

        // 4. HEADER STYLING
        grid.Render.UseHeaderGradient = false;
        grid.Render.UseHeaderHoverEffects = true;
        grid.Render.UseBoldHeaderText = false;
        grid.Render.HeaderCellPadding = 3;

        // 5. ADVANCED EFFECTS
        grid.Render.UseElevation = false; // Shadow/depth
        grid.Render.CardStyle = false;    // Card appearance

        // 6. ALIGNMENT HEURISTICS
        LayoutCommon.ApplyAlignmentHeuristics(grid);
    }
}
```

### Key Properties by Category

#### Dimensions
```csharp
grid.RowHeight = 24;              // Row height in pixels (18-40 typical)
grid.ColumnHeaderHeight = 26;      // Header height in pixels (22-36 typical)
grid.ShowColumnHeaders = true;     // Show/hide header row
```

#### Grid Lines
```csharp
grid.Render.ShowGridLines = true;              // Enable/disable borders
grid.Render.GridLineStyle = DashStyle.Solid;   // Solid, Dot, Dash, DashDot, etc.
```

#### Row Appearance
```csharp
grid.Render.ShowRowStripes = true;  // Alternating row colors (zebra stripes)
```

#### Header Appearance
```csharp
grid.Render.UseHeaderGradient = true;      // Gradient background
grid.Render.UseHeaderHoverEffects = true;  // Highlight on hover
grid.Render.UseBoldHeaderText = true;      // Bold font
grid.Render.HeaderCellPadding = 4;         // Padding in pixels (1-8)
```

#### Advanced Effects
```csharp
grid.Render.UseElevation = true;  // Shadow/depth effect (Material Design)
grid.Render.CardStyle = true;     // Card-based appearance
```

## Layout Presets by Purpose

### General Purpose

#### Default (Standard)
```csharp
// Use When: General-purpose tables, standard data display
RowHeight: 24px
HeaderHeight: 26px
GridLines: Solid
Stripes: No
HeaderGradient: No
Use Case: Default for most scenarios
```

#### Clean (Minimal)
```csharp
// Use When: Modern, clean UIs, professional dashboards
RowHeight: 26px
HeaderHeight: 28px
GridLines: Solid
Stripes: No
HeaderGradient: No
HeaderPadding: 3px (generous)
Use Case: Professional applications, clean design
```

### High Density

#### Dense (Compact)
```csharp
// Use When: Space-constrained, data-heavy screens
RowHeight: 20px (smallest)
HeaderHeight: 24px
GridLines: Solid
Stripes: No
HeaderPadding: 1px (minimal)
Use Case: Admin panels, data analysis, maximize visible rows
```

### Visual Enhancement

#### Striped (Zebra)
```csharp
// Use When: Wide tables, many columns, readability important
RowHeight: 24px
HeaderHeight: 26px
GridLines: Solid
Stripes: Yes (key feature)
Use Case: Improving row tracking, wide tables, reports
```

#### Borderless (Minimal)
```csharp
// Use When: Modern UIs, embedded grids, card layouts
RowHeight: 24px
HeaderHeight: 26px
GridLines: No (key feature)
Stripes: No
Use Case: Clean interfaces, embedded views
```

### Header Emphasis

#### HeaderBold (Emphasis)
```csharp
// Use When: Important tables, primary data displays
RowHeight: 24px
HeaderHeight: 28px
GridLines: Solid
BoldHeader: Yes (key feature)
HeaderPadding: 3px
Use Case: Emphasized headers, reports, key tables
```

#### MaterialHeader (Modern)
```csharp
// Use When: Material Design apps, modern UIs
RowHeight: 28px (taller)
HeaderHeight: 32px (taller)
GridLines: Solid
HeaderGradient: Yes (key feature)
Elevation: Yes (key feature)
HeaderPadding: 4px (generous)
Use Case: Material Design applications, modern aesthetic
```

### Specialty Layouts

#### Card (Elevated)
```csharp
// Use When: Dashboard cards, modern UIs
RowHeight: 28px
HeaderHeight: 32px
GridLines: No
HeaderGradient: Yes
Elevation: Yes (key feature)
CardStyle: Yes (key feature)
Use Case: Dashboard widgets, card-based layouts
```

#### ComparisonTable (Features)
```csharp
// Use When: Product comparisons, feature matrices
RowHeight: 26px
HeaderHeight: 30px
GridLines: Solid
HeaderGradient: Yes
BoldHeader: Yes
Use Case: Product specs, feature comparison
```

#### MatrixSimple / MatrixStriped
```csharp
// Use When: Data matrices, correlation tables
RowHeight: 24px
HeaderHeight: 26px
GridLines: Solid
Stripes: (Simple: No, Striped: Yes)
CenterAlignment: Yes (auto-applied)
Use Case: Matrix data, cross-reference tables
```

#### PricingTable
```csharp
// Use When: Pricing comparisons, subscription tiers
RowHeight: 28px
HeaderHeight: 32px
GridLines: Solid
HeaderGradient: Yes
BoldHeader: Yes
Elevation: Yes
CenterAlignment: Yes (auto-applied)
Use Case: Pricing pages, package features
```

## LayoutCommon Helper

### ApplyAlignmentHeuristics

Intelligently sets column alignments:

```csharp
public static void ApplyAlignmentHeuristics(BeepGridPro grid)
{
    // Rules:
    // 1. System columns (checkbox, row#, ID) → Center
    // 2. First visible non-system column → Left
    // 3. Status/Action/Price/Qty columns → Center (by name)
    // 4. All other columns → Left (default)
}
```

**Detected Center-Aligned Columns** (case-insensitive):
- Contains "status", "state"
- Contains "action", "actions"
- Contains "price", "total"
- Contains "qty", "quantity"

**Override if Needed**:
```csharp
// After applying layout
grid.LayoutPreset = GridLayoutPreset.Default;

// Override specific column
var priceColumn = grid.GetColumnByName("Price");
priceColumn.CellTextAlignment = ContentAlignment.MiddleRight;
```

## Application Methods

### Method 1: Via Enum (Recommended)
```csharp
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
```

Internally calls:
```csharp
IGridLayoutPreset impl = preset switch
{
    GridLayoutPreset.MaterialHeader => new MaterialHeaderTableLayoutHelper(),
    // ... etc
    _ => new DefaultTableLayoutHelper()
};
grid.ApplyLayoutPreset(impl);
```

### Method 2: Direct Instance
```csharp
var layout = new MaterialHeaderTableLayoutHelper();
grid.ApplyLayoutPreset(layout);
```

### Method 3: Custom Implementation
```csharp
public class MyCustomLayout : IGridLayoutPreset
{
    public void Apply(BeepGridPro grid)
    {
        // Custom configuration
    }
}

grid.ApplyLayoutPreset(new MyCustomLayout());
```

## Interaction with Other Systems

### Layout + Theme
```csharp
// Theme provides COLORS
grid.Theme = BeepTheme.MaterialDark;

// Layout provides STRUCTURE
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;

// Result: Material structure with dark colors
```

### Layout + GridStyle
```csharp
// Layout applied FIRST
grid.LayoutPreset = GridLayoutPreset.Default;

// GridStyle applied SECOND (may override)
grid.GridStyle = BeepGridStyle.Material;

// GridStyle.Material may change RowHeight, HeaderGradient, etc.
```

**Best Practice**: Use matching layout + style:
```csharp
// GOOD - consistent
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
grid.GridStyle = BeepGridStyle.Material;

// AVOID - inconsistent
grid.LayoutPreset = GridLayoutPreset.Minimal;
grid.GridStyle = BeepGridStyle.Material; // Conflict!
```

### Manual Overrides Win
```csharp
grid.LayoutPreset = GridLayoutPreset.Dense;  // Sets RowHeight = 20
grid.RowHeight = 32;                          // Override wins
```

## Creating Custom Layouts

### Step 1: Implement Interface

```csharp
public sealed class EnterpriseLayoutHelper : IGridLayoutPreset
{
    public void Apply(BeepGridPro grid)
    {
        if (grid == null) return;

        // Corporate aesthetic
        grid.RowHeight = 26;
        grid.ColumnHeaderHeight = 30;
        grid.ShowColumnHeaders = true;

        // Professional appearance
        grid.Render.ShowGridLines = true;
        grid.Render.GridLineStyle = DashStyle.Solid;
        grid.Render.ShowRowStripes = false;
        grid.Render.UseHeaderGradient = true;
        grid.Render.UseHeaderHoverEffects = true;
        grid.Render.UseBoldHeaderText = true;
        grid.Render.HeaderCellPadding = 4;
        grid.Render.UseElevation = false;
        grid.Render.CardStyle = false;

        // Standard alignment
        LayoutCommon.ApplyAlignmentHeuristics(grid);
        
        // Custom overrides
        foreach (var col in grid.Data.Columns)
        {
            if (col.ColumnName.EndsWith("Date"))
            {
                col.CellTextAlignment = ContentAlignment.MiddleCenter;
            }
        }
    }
}
```

### Step 2: Register (Optional)

Add to enum:
```csharp
public enum GridLayoutPreset
{
    // ... existing
    Enterprise
}
```

Add to switch in BeepGridPro:
```csharp
case GridLayoutPreset.Enterprise:
    return new EnterpriseLayoutHelper();
```

### Step 3: Use

```csharp
// Direct
grid.ApplyLayoutPreset(new EnterpriseLayoutHelper());

// Via enum (if registered)
grid.LayoutPreset = GridLayoutPreset.Enterprise;
```

## Testing Layouts

### Visual Test
```csharp
[TestMethod]
public void MaterialHeader_ShouldApplyCorrectSettings()
{
    var grid = new BeepGridPro();
    var layout = new MaterialHeaderTableLayoutHelper();
    
    layout.Apply(grid);
    
    Assert.AreEqual(28, grid.RowHeight);
    Assert.AreEqual(32, grid.ColumnHeaderHeight);
    Assert.IsTrue(grid.Render.UseHeaderGradient);
    Assert.IsTrue(grid.Render.UseElevation);
}
```

### Snapshot Test
```csharp
[TestMethod]
public void MaterialHeader_RenderMatchesSnapshot()
{
    var grid = CreateTestGrid();
    grid.LayoutPreset = GridLayoutPreset.MaterialHeader;
    
    var bitmap = RenderToBitmap(grid);
    AssertMatchesSnapshot("MaterialHeader", bitmap);
}
```

## Common Patterns

### Dense Data
```csharp
// Maximize visible rows
grid.RowHeight = 20;
grid.ColumnHeaderHeight = 24;
grid.Render.HeaderCellPadding = 1;
grid.Render.ShowRowStripes = false; // Avoid visual clutter
```

### Modern UI
```csharp
// Clean, elevated
grid.RowHeight = 28;
grid.ColumnHeaderHeight = 32;
grid.Render.ShowGridLines = false;
grid.Render.UseHeaderGradient = true;
grid.Render.UseElevation = true;
grid.Render.HeaderCellPadding = 4;
```

### Professional Report
```csharp
// Clear, readable
grid.RowHeight = 24;
grid.ColumnHeaderHeight = 28;
grid.Render.ShowGridLines = true;
grid.Render.GridLineStyle = DashStyle.Solid;
grid.Render.UseBoldHeaderText = true;
grid.Render.ShowRowStripes = true; // Improve readability
```

## Debugging

### Check Applied Values
```csharp
grid.LayoutPreset = GridLayoutPreset.MaterialHeader;

Console.WriteLine($"Row Height: {grid.RowHeight}");
Console.WriteLine($"Header Height: {grid.ColumnHeaderHeight}");
Console.WriteLine($"Grid Lines: {grid.Render.ShowGridLines}");
Console.WriteLine($"Header Gradient: {grid.Render.UseHeaderGradient}");
```

### Verify Recalculation
```csharp
bool recalculated = false;
grid.Layout.Recalculated += () => recalculated = true;

grid.LayoutPreset = GridLayoutPreset.Dense;

Assert.IsTrue(recalculated); // Should trigger recalc
```

## Performance Notes

- **Layout application is fast** - just property assignments
- **No caching needed** - layouts are stateless
- **Recalculation automatic** - triggered by BeepGridPro
- **Apply before binding** for best results (fewer recalcs)

## Common Mistakes

### 1. Trying to Set Colors
```csharp
// WRONG - layouts don't control colors
public void Apply(BeepGridPro grid)
{
    grid.BackColor = Color.White; // NO! Use themes
}

// CORRECT - only structure
public void Apply(BeepGridPro grid)
{
    grid.RowHeight = 24;
    grid.Render.ShowGridLines = true;
}
```

### 2. Forgetting Alignment Heuristics
```csharp
// Incomplete
public void Apply(BeepGridPro grid)
{
    grid.RowHeight = 24;
    // Missing: LayoutCommon.ApplyAlignmentHeuristics(grid);
}

// Complete
public void Apply(BeepGridPro grid)
{
    grid.RowHeight = 24;
    LayoutCommon.ApplyAlignmentHeuristics(grid);
}
```

### 3. Not Checking for Null
```csharp
// Unsafe
public void Apply(BeepGridPro grid)
{
    grid.RowHeight = 24; // NullReferenceException if grid is null
}

// Safe
public void Apply(BeepGridPro grid)
{
    if (grid == null) return;
    grid.RowHeight = 24;
}
```

## Future Enhancements

See `../ENHANCEMENT_PLAN.md` for:
1. **BaseLayoutPreset** abstract class
2. **Layout metadata** (name, description, category)
3. **Painter integration** (GetHeaderPainter, GetNavigationPainter)
4. **Height calculation** per layout
5. **Compatibility checking** (with themes, grid styles)

## Related Files

- **BeepGridPro.cs**: Applies layouts via ApplyLayoutPreset()
- **GridLayoutHelper.cs**: Performs recalculation after layout applied
- **GridRenderHelper.cs**: Uses layout properties for rendering
- **IGridLayoutPreset.cs**: Interface definition
- **GridLayoutPreset.cs**: Enum of built-in presets

## Quick Reference Table

| Preset | RowH | HeaderH | Lines | Stripes | Gradient | Bold | Elevation | Use Case |
|--------|------|---------|-------|---------|----------|------|-----------|----------|
| Default | 24 | 26 | ✓ | ✗ | ✗ | ✗ | ✗ | General purpose |
| Clean | 26 | 28 | ✓ | ✗ | ✗ | ✗ | ✗ | Modern, professional |
| Dense | 20 | 24 | ✓ | ✗ | ✗ | ✗ | ✗ | Maximum density |
| Striped | 24 | 26 | ✓ | ✓ | ✗ | ✗ | ✗ | Wide tables |
| Borderless | 24 | 26 | ✗ | ✗ | ✗ | ✗ | ✗ | Minimal, embedded |
| HeaderBold | 24 | 28 | ✓ | ✗ | ✗ | ✓ | ✗ | Emphasized headers |
| MaterialHeader | 28 | 32 | ✓ | ✗ | ✓ | ✗ | ✓ | Material Design |
| Card | 28 | 32 | ✗ | ✗ | ✓ | ✗ | ✓ | Card-based UI |
| Comparison | 26 | 30 | ✓ | ✗ | ✓ | ✓ | ✗ | Product comparison |
| MatrixSimple | 24 | 26 | ✓ | ✗ | ✗ | ✗ | ✗ | Simple matrix |
| MatrixStriped | 24 | 26 | ✓ | ✓ | ✗ | ✗ | ✗ | Striped matrix |
| PricingTable | 28 | 32 | ✓ | ✗ | ✓ | ✓ | ✓ | Pricing/packages |

## Decision Tree

**Need maximum data density?** → Dense  
**Need improved row tracking?** → Striped or MatrixStriped  
**Material Design app?** → MaterialHeader + Material style  
**Dashboard cards?** → Card  
**Product comparison?** → ComparisonTable  
**Pricing page?** → PricingTable  
**Matrix/correlation data?** → MatrixSimple or MatrixStriped  
**Professional/clean?** → Clean or HeaderBold  
**Default/unsure?** → Default

