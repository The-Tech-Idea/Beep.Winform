# BeepFlyoutMenu Skill

## Overview
`BeepFlyoutMenu` is a dropdown/flyout menu that expands from the control with slide direction options and animations.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
```

## SlideDirection
```csharp
public enum SlideDirection
{
    Bottom,  // Flyout appears below (default)
    Left,    // Flyout appears to the left
    Right    // Flyout appears to the right
}
```

## LabelPosition
```csharp
public enum LabelPosition
{
    Left,   // Label on left, button on right
    Right   // Button on left, label on right
}
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `ListItems` | `BindingList<SimpleItem>` | Menu items |
| `SelectedIndex` | `int` | Selected item index |
| `FlyoutDirection` | `SlideDirection` | Expand direction |
| `LabelPosition` | `LabelPosition` | Label placement |
| `Text` | `string` | Label text |

## Usage Examples

### Basic Flyout Menu
```csharp
var flyout = new BeepFlyoutMenu
{
    Text = "Select Option",
    FlyoutDirection = SlideDirection.Bottom
};

flyout.ListItems.Add(new SimpleItem { Text = "Option 1" });
flyout.ListItems.Add(new SimpleItem { Text = "Option 2" });
flyout.ListItems.Add(new SimpleItem { Text = "Option 3" });

flyout.SelectedIndexChanged += (s, e) =>
{
    Console.WriteLine($"Selected: {flyout.SelectedIndex}");
};

flyout.MenuClicked += (s, e) =>
{
    Console.WriteLine($"Clicked: {e.Data}");
};
```

### Right-Expanding Flyout
```csharp
var flyout = new BeepFlyoutMenu
{
    FlyoutDirection = SlideDirection.Right,
    LabelPosition = LabelPosition.Left
};
```

## Events
| Event | Description |
|-------|-------------|
| `SelectedIndexChanged` | Selection changed |
| `MenuClicked` | Menu item clicked |

## Features
- Animated expand/collapse
- Dynamic icon for direction
- Auto-positions on parent form
- Multiple slide directions
- Theme-aware styling

## Related Controls
- `BeepPopupMenu` - Context-style popups
- `BeepComboBox` - Standard dropdown
