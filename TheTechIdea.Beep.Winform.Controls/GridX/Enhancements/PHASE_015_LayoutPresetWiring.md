# Phase 15: LayoutPreset Enum-to-Class Wiring

**Priority:** P2 | **Track:** Polish & Quality | **Status:** Pending

## Objective

Wire all `GridLayoutPreset` enum values to their corresponding layout classes in `ApplyLayoutPreset(GridLayoutPreset)`.

## Problem

`GridLayoutPreset` contains modern values such as `Material3Surface`, `Fluent2Standard`, `TailwindProse`, and `AGGridAlpine`. The property-based switch currently wires only the legacy preset group. Newer layout classes must be applied directly with the extension overload.

## Implementation Steps

### Step 1: Extend ApplyLayoutPreset Switch

In `BeepGridPro.Properties.cs` (or the file containing `ApplyLayoutPreset`):

```csharp
public void ApplyLayoutPreset(GridLayoutPreset preset)
{
    IGridLayoutPreset layout = preset switch
    {
        // Existing legacy presets
        GridLayoutPreset.Default => new DefaultTableLayoutHelper(),
        GridLayoutPreset.Clean => new CleanTableLayoutHelper(),
        GridLayoutPreset.Dense => new DenseTableLayoutHelper(),
        GridLayoutPreset.Striped => new StripedTableLayoutHelper(),
        GridLayoutPreset.Borderless => new BorderlessTableLayoutHelper(),
        GridLayoutPreset.HeaderBold => new HeaderBoldTableLayoutHelper(),
        GridLayoutPreset.MaterialHeader => new MaterialHeaderTableLayoutHelper(),
        GridLayoutPreset.Card => new CardTableLayoutHelper(),
        GridLayoutPreset.ComparisonTable => new ComparisonTableLayoutHelper(),
        GridLayoutPreset.MatrixSimple => new MatrixSimpleTableLayoutHelper(),
        GridLayoutPreset.MatrixStriped => new MatrixStripedTableLayoutHelper(),
        GridLayoutPreset.PricingTable => new PricingTableLayoutHelper(),

        // NEW: Modern presets
        GridLayoutPreset.Material3Surface => new Material3SurfaceLayout(),
        GridLayoutPreset.Material3Compact => new Material3CompactLayout(),
        GridLayoutPreset.Material3List => new Material3ListLayout(),
        GridLayoutPreset.Fluent2Standard => new Fluent2StandardLayout(),
        GridLayoutPreset.Fluent2Card => new Fluent2CardLayout(),
        GridLayoutPreset.TailwindProse => new TailwindProseLayout(),
        GridLayoutPreset.TailwindDashboard => new TailwindDashboardLayout(),
        GridLayoutPreset.AGGridAlpine => new AGGridAlpineLayout(),
        GridLayoutPreset.AGGridBalham => new AGGridBalhamLayout(),
        GridLayoutPreset.AntDesignStandard => new AntDesignStandardLayout(),
        GridLayoutPreset.AntDesignCompact => new AntDesignCompactLayout(),
        GridLayoutPreset.DataTablesStandard => new DataTablesStandardLayout(),

        _ => new DefaultTableLayoutHelper()
    };

    this.ApplyLayoutPreset(layout);  // Calls extension overload
}
```

### Step 2: Verify All Enum Values Are Covered

Compare `GridLayoutPreset` enum values against the switch cases. Add any missing mappings.

### Step 3: Test

- Each preset via property assignment
- Each preset via designer property grid
- Switch presets at runtime

## Acceptance Criteria

- [ ] All `GridLayoutPreset` enum values are wired to layout classes
- [ ] Each preset renders correctly when set via property
- [ ] Each preset works in designer property grid
- [ ] Runtime preset switching works cleanly
- [ ] No deprecated or missing layout classes referenced

## Files to Modify

- `BeepGridPro.Properties.cs` (or the file containing `ApplyLayoutPreset`)
- `Layouts/GridLayoutPresetExtensions.cs` (if extension method needs update)
