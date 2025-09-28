# BeepChartWidget Theme Instructions

## Overview
The BeepChartWidget ApplyTheme function should properly apply chart-specific theme properties to ensure consistent styling across different chart types (Bar, Line, Pie, Gauge, Sparkline, Heatmap, Combination).

## Current ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    InitializePainter();
    Invalidate();
}
```

## Required Theme Properties to Apply

### Chart-Specific Colors
- `ChartBackColor` - Background color for chart area
- `ChartTitleColor` - Color for chart titles
- `ChartTextColor` - Color for chart text/labels
- `ChartAxisColor` - Color for axes and grid lines
- `ChartGridLineColor` - Color for grid lines
- `ChartLegendBackColor` - Background for legend
- `ChartLegendTextColor` - Text color for legend
- `ChartLegendShapeColor` - Color for legend shapes

### Typography Styles
- `ChartTitleFont` - Font for chart titles
- `ChartSubTitleFont` - Font for chart subtitles

### Series Colors
- `ChartDefaultSeriesColors` - List of default colors for data series

### General Theme Colors
- `BackColor` - Widget background
- `ForeColor` - Default text color
- `BorderColor` - Border color
- `AccentColor` - Accent color for highlights

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply chart-specific theme colors
    BackColor = _currentTheme.ChartBackColor;
    ForeColor = _currentTheme.ChartTextColor;
    
    // Update accent color from theme
    _accentColor = _currentTheme.AccentColor;
    
    // Update series colors from theme
    if (_currentTheme.ChartDefaultSeriesColors != null && _currentTheme.ChartDefaultSeriesColors.Count > 0)
    {
        _colors.Clear();
        _colors.AddRange(_currentTheme.ChartDefaultSeriesColors);
    }
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Update BackColor to use `ChartBackColor` instead of generic `BackColor`
2. Update ForeColor to use `ChartTextColor` instead of generic `ForeColor`
3. Update `_accentColor` to use theme's `AccentColor`
4. Update `_colors` list with `ChartDefaultSeriesColors` if available
5. Ensure InitializePainter() is called to refresh painter with new theme
6. Ensure Invalidate() is called to trigger repaint

## Testing
- Verify chart background uses ChartBackColor
- Verify text elements use ChartTextColor
- Verify accent elements use AccentColor
- Verify data series use ChartDefaultSeriesColors
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepChartWidget.theme.instructions.md