# BeepProgressBar Skill

## Overview
`BeepProgressBar` is a progress indicator with 13 painter styles including linear, ring, dotted, segmented, and animated variants.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
```

## ProgressPainterKind (13 styles)
| Value | Description |
|-------|-------------|
| `Linear` | Default horizontal bar |
| `StepperCircles` | Steps with circles and labels |
| `ChevronSteps` | Chevron arrows per step |
| `DotsLoader` | Row of dots (filling/active) |
| `Segmented` | Segmented blocks along line |
| `Ring` | Circular ring |
| `DottedRing` | Circular dotted ring |
| `LinearBadge` | Linear with floating % badge |
| `LinearTrackerIcon` | Linear with moving tracker |
| `ArrowStripe` | Herringbone/arrow stripe bar |
| `RadialSegmented` | Donut with separated segments |
| `RingCenterImage` | Ring with center image/icon |
| `ArrowHeadAnimated` | Bar with animated arrow head |

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `PainterKind` | `ProgressPainterKind` | Visual style |
| `Value` | `int` | Current progress (0-100) |
| `Minimum` | `int` | Minimum value |
| `Maximum` | `int` | Maximum value |
| `ShowPercentage` | `bool` | Display percentage text |
| `UseThemeColors` | `bool` | Use theme colors |

## Usage Examples

### Basic Linear
```csharp
var progress = new BeepProgressBar
{
    PainterKind = ProgressPainterKind.Linear,
    Value = 50,
    UseThemeColors = true
};
```

### Ring Progress
```csharp
var ring = new BeepProgressBar
{
    PainterKind = ProgressPainterKind.Ring,
    Value = 75,
    Size = new Size(100, 100)
};
```

### Dotted Ring with Center Image
```csharp
var progress = new BeepProgressBar
{
    PainterKind = ProgressPainterKind.RingCenterImage,
    Value = 60,
    CenterImage = "logo.png"
};
```

### Stepper Progress
```csharp
var stepper = new BeepProgressBar
{
    PainterKind = ProgressPainterKind.StepperCircles,
    Maximum = 5,  // Number of steps
    Value = 2     // Current step
};
```

## Events
| Event | Description |
|-------|-------------|
| `ValueChanged` | Progress value changed |

## Helpers
- `ProgressBarThemeHelpers` - Theme colors
- `ProgressBarFontHelpers` - Font styling
- `ProgressBarAccessibilityHelpers` - WCAG support
