# BeepChart Skill

## Overview
`BeepChart` is a comprehensive charting control with 12+ chart types, axis configuration, viewport zoom/pan, tooltips, animations, and painter-based rendering.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Charts;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;
```

## ChartType (12+)
```csharp
public enum ChartType
{
    Line,           // Line chart
    Bar,            // Bar/column chart
    Pie,            // Pie chart
    Area,           // Area chart
    Scatter,        // Scatter plot
    Donut,          // Donut chart
    Spline,         // Smooth line
    Stacked,        // Stacked bar
    Combination,    // Multiple types
    Gauge,          // Gauge/meter
    Radar,          // Radar/spider
    Waterfall       // Waterfall chart
}
```

## AxisType
```csharp
public enum AxisType
{
    Numeric,    // Numbers
    Category,   // Text labels
    DateTime    // Date/time values
}
```

## Key Properties

### Title Properties
| Property | Type | Description |
|----------|------|-------------|
| `ShowTitle` | `bool` | Show chart title |
| `ChartTitle` | `string` | Title text |
| `ChartValue` | `string` | Main value display |
| `ChartSubtitle` | `string` | Subtitle text |
| `ChartTitleFont` | `Font` | Title font |

### Data Properties
| Property | Type | Description |
|----------|------|-------------|
| `DataSeries` | `List<ChartDataSeries>` | Data series |
| `LegendLabels` | `List<string>` | Legend items |
| `ChartDefaultSeriesColors` | `List<Color>` | Series colors |

### Axis Properties
| Property | Type | Description |
|----------|------|-------------|
| `BottomAxisType` | `AxisType` | X-axis type |
| `LeftAxisType` | `AxisType` | Y-axis type |
| `XAxisTitle` | `string` | X-axis label |
| `YAxisTitle` | `string` | Y-axis label |
| `XLabelAngle` | `float` | X-label rotation |

### Viewport Properties
| Property | Type | Description |
|----------|------|-------------|
| `ViewportXMin/XMax` | `float` | X viewport range |
| `ViewportYMin/YMax` | `float` | Y viewport range |

### Appearance Properties
| Property | Type | Description |
|----------|------|-------------|
| `ChartType` | `ChartType` | Chart type |
| `ShowLegend` | `bool` | Show legend |
| `LegendPlacement` | `LegendPlacement` | Legend position |
| `SurfaceStyle` | `ChartSurfaceStyle` | Card style |
| `AccentColor` | `Color` | Accent color |
| `Stacked` | `bool` | Stack series |
| `SmoothLines` | `bool` | Smooth line curves |
| `ShowMarkers` | `bool` | Show data markers |

## Usage Example
```csharp
var chart = new BeepChart
{
    ChartType = ChartType.Line,
    ChartTitle = "Monthly Revenue",
    ShowLegend = true,
    SmoothLines = true
};

chart.DataSeries.Add(new ChartDataSeries
{
    Name = "Revenue",
    DataPoints = new List<ChartDataPoint>
    {
        new ChartDataPoint { X = 1, Y = 100 },
        new ChartDataPoint { X = 2, Y = 150 },
        new ChartDataPoint { X = 3, Y = 120 }
    }
});
```

## Related Controls
- `BeepChartWidget` - Dashboard chart widget
