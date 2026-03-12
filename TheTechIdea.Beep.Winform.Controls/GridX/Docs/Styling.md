# Styling and Theming

## `GridStyle`

`GridStyle` applies a built-in appearance preset through `BeepGridPro.ApplyGridStyle()`.

### Implemented branches
| Style | Effect |
|-------|--------|
| `Default` | theme defaults, grid lines, hover effects |
| `Clean` | subtle borders, minimal spacing |
| `Bootstrap` | striped rows, clean borders |
| `Material` | larger heights, elevation, header gradient |
| `Flat` | no grid lines, flatter header treatment |
| `Compact` | tighter row/header spacing |
| `Corporate` | professional gradient header, bold header text |
| `Minimal` | very light styling, no sort indicator emphasis |
| `Card` | elevated card-like rows and headers |
| `Borderless` | no visible grid lines |

### Important gap
`BeepGridStyle.Modern` exists in `TheTechIdea.Beep.Vis.Modules`, but `ApplyGridStyle()` does not currently implement a branch for it.

## `NavigationStyle`

The navigator painter is selected independently through:
```csharp
grid.NavigationStyle = navigationStyle.Material;
grid.UsePainterNavigation = true;
```

Available values include:
- `None`
- `Standard`
- `Material`
- `Compact`
- `Minimal`
- `Bootstrap`
- `Fluent`
- `AntDesign`
- `Telerik`
- `AGGrid`
- `DataTables`
- `Card`
- `Tailwind`

`ApplyGridStyle()` also assigns a matching `NavigationStyle` for the built-in style presets.

## `LayoutPreset`

### Property-based support today
`BeepGridPro.ApplyLayoutPreset(GridLayoutPreset)` currently switches these values:
- `Default`
- `Clean`
- `Dense`
- `Striped`
- `Borderless`
- `HeaderBold`
- `MaterialHeader`
- `Card`
- `ComparisonTable`
- `MatrixSimple`
- `MatrixStriped`
- `PricingTable`

### Enum values that need direct class application
The enum also contains newer values such as:
- `Material3Surface`
- `Material3Compact`
- `Material3List`
- `Fluent2Standard`
- `Fluent2Card`
- `TailwindProse`
- `TailwindDashboard`
- `AGGridAlpine`
- `AGGridBalham`
- `AntDesignStandard`
- `AntDesignCompact`
- `DataTablesStandard`

Apply those directly with the extension overload:
```csharp
grid.ApplyLayoutPreset(new Material3SurfaceLayout());
```

## Focus Styling

Focus-related properties are forwarded into `GridRenderHelper`:
```csharp
grid.UseDedicatedFocusedRowStyle = true;
grid.FocusedRowBackColor = Color.Empty;
grid.ShowFocusedCellFill = true;
grid.FocusedCellFillOpacity = 36;
grid.ShowFocusedCellBorder = true;
grid.FocusedCellBorderWidth = 2f;
```

Behavior:
- empty override colors fall back to theme-derived values
- focused-row styling is separate from checkbox selection
- focused-cell fill and border can be enabled independently

## Header Icons

```csharp
grid.SortIconVisibility = HeaderIconVisibility.HoverOnly;
grid.FilterIconVisibility = HeaderIconVisibility.Hidden;
```

Rules:
- sort icons support `Always`, `HoverOnly`, `Hidden`
- filter icons support `Always`, `HoverOnly`, `Hidden`
- when `ShowTopFilterPanel` is `true`, the render path suppresses header filter icon areas

## Column Appearance

Per-column customization uses `BeepColumnConfig`:
```csharp
var col = grid.GetColumnByName("Status");
col.UseCustomColors = true;
col.ColumnBackColor = Color.WhiteSmoke;
col.ColumnForeColor = Color.DarkSlateGray;
col.ColumnHeaderBackColor = Color.Navy;
col.ColumnHeaderForeColor = Color.White;
```

Relevant members:
- `UseCustomColors`
- `ColumnBackColor`
- `ColumnForeColor`
- `ColumnBorderColor`
- `ColumnHeaderBackColor`
- `ColumnHeaderForeColor`
- `CellTextAlignment`
- `HeaderTextAlignment`

## Theme Integration

`BeepGridPro` inherits from `BaseControl`, so theme application is still rooted in the Beep theme system:
```csharp
grid.Theme = "MaterialDark";
grid.ApplyTheme();
```

`ApplyTheme()`:
1. calls the base implementation
2. applies `GridThemeHelper`
3. reapplies `GridStyle`
4. invalidates the control

Practical implication:
- custom hard-coded colors can be overwritten or visually conflict after theme changes
- prefer theme-driven settings unless a column or state truly requires explicit colors
