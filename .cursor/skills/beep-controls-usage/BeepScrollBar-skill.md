# BeepScrollBar Skill

## Overview
Themed scroll bar control for custom scrolling implementations.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Orientation` | `Orientation` | Horizontal or Vertical |
| `Value` | `int` | Current scroll position |
| `Minimum` | `int` | Minimum value |
| `Maximum` | `int` | Maximum value |
| `LargeChange` | `int` | Page scroll amount |
| `SmallChange` | `int` | Arrow scroll amount |

## Events
| Event | Description |
|-------|-------------|
| `Scroll` | Scroll position changed |
| `ValueChanged` | Value updated |

## Usage
```csharp
var scrollBar = new BeepScrollBar
{
    Orientation = Orientation.Vertical,
    Maximum = 1000,
    LargeChange = 100
};
scrollBar.Scroll += (s, e) => ScrollContent(scrollBar.Value);
```
