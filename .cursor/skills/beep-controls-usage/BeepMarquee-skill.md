# BeepMarquee Skill

## Overview
`BeepMarquee` is a continuous scrolling text/component display, similar to HTML `<marquee>`, with configurable speed and direction.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `ScrollLeft` | `bool` | Scroll direction (true = left) |
| `ScrollSpeed` | `float` | Pixels per tick (default: 2) |
| `ScrollInterval` | `int` | Timer interval ms (default: 30) |
| `ComponentSpacing` | `int` | Space between items (px: 20) |

## Usage Examples

### Basic Marquee
```csharp
var marquee = new BeepMarquee
{
    ScrollLeft = true,
    ScrollSpeed = 3f,
    Dock = DockStyle.Top,
    Height = 50
};

// Add IBeepUIComponent items
marquee.AddMarqueeComponent("news1", newsLabel1);
marquee.AddMarqueeComponent("news2", newsLabel2);
```

### Right-to-Left Scroll
```csharp
var marquee = new BeepMarquee
{
    ScrollLeft = false,  // Scroll right
    ScrollSpeed = 1.5f   // Slower
};
```

### Manage Components
```csharp
// Add component
marquee.AddMarqueeComponent("item1", myComponent);

// Remove component
marquee.RemoveMarqueeComponent("item1");
```

## Features
- Continuous seamless wrap-around
- Timer-driven animation
- Vertical centering of components
- Theme integration via helpers

## Theme Helpers
- `MarqueeFontHelpers.ApplyFontTheme()`
- `MarqueeThemeHelpers.GetMarqueeBackgroundColor()`
