# BeepTooltip Skill

## Overview
All Beep controls inherit tooltip support from `BaseControl`. Tooltips are rich, styled, and theme-integrated with 7 types, placement control, and animations.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
```

## ToolTipType (7 types)
```csharp
public enum ToolTipType
{
    Default,   // Standard tooltip
    Info,      // Blue - Information
    Success,   // Green - Success
    Warning,   // Yellow - Warning
    Error,     // Red - Error
    Help,      // ? icon - Help
    Custom     // Custom styling
}
```

## ToolTipPlacement
```csharp
public enum ToolTipPlacement
{
    Auto,    // Automatic best position
    Top,     // Above control
    Bottom,  // Below control
    Left,    // Left of control
    Right    // Right of control
}
```

## ToolTipAnimation
```csharp
public enum ToolTipAnimation
{
    None,      // No animation
    Fade,      // Fade in/out
    Slide,     // Slide in
    Scale      // Scale up
}
```

## Key Properties (on any Beep control)
| Property | Type | Description |
|----------|------|-------------|
| `TooltipText` | `string` | Tooltip content |
| `TooltipTitle` | `string` | Title/header |
| `TooltipType` | `ToolTipType` | Semantic type |
| `TooltipIconPath` | `string` | Custom icon |
| `EnableTooltip` | `bool` | Enable/disable (true) |
| `TooltipDuration` | `int` | Duration ms (3000) |
| `TooltipPlacement` | `ToolTipPlacement` | Position (Auto) |
| `TooltipAnimation` | `ToolTipAnimation` | Animation (Fade) |
| `TooltipShowArrow` | `bool` | Show arrow (true) |
| `TooltipShowShadow` | `bool` | Show shadow (true) |
| `TooltipFollowCursor` | `bool` | Follow cursor (false) |
| `TooltipShowDelay` | `int` | Delay ms (500) |
| `TooltipClosable` | `bool` | User can close (false) |
| `TooltipMaxSize` | `Size?` | Max dimensions |
| `TooltipFont` | `Font` | Custom font |
| `TooltipUseThemeColors` | `bool` | Use theme (true) |
| `TooltipUseControlStyle` | `bool` | Match ControlStyle (true) |

## Usage Examples

### Basic Tooltip
```csharp
var button = new BeepButton
{
    Text = "Save",
    TooltipText = "Save the current document",
    TooltipPlacement = ToolTipPlacement.Top
};
```

### Rich Tooltip
```csharp
var button = new BeepButton
{
    TooltipTitle = "Save Document",
    TooltipText = "Click to save changes to the document",
    TooltipType = ToolTipType.Info,
    TooltipAnimation = ToolTipAnimation.Slide,
    TooltipShowArrow = true
};
```

### Notification Methods
```csharp
// Show temporary notifications from any control
button.ShowSuccess("File saved successfully!");
button.ShowError("Failed to save file");
button.ShowWarning("File is large");
button.ShowInfo("Auto-saving enabled");

// Custom notification
button.ShowNotification("Custom message", ToolTipType.Info, 2000);
```

## ToolTipManager
```csharp
// Programmatic tooltip control
ToolTipManager.Instance.SetTooltip(control, "Text", config);
await ToolTipManager.Instance.ShowTooltipAsync(config);
ToolTipManager.Instance.RemoveTooltip(control);
```

## Features
- Theme integration via `ApplyTheme()`
- ControlStyle matching (Material3, Fluent2, etc.)
- Auto-positioning to stay on screen
- Rich content with title, icon, text
- Semantic types with appropriate colors

## Related Controls
- All Beep controls inherit tooltip properties
