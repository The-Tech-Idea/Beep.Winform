# BeepSwitch Skill

## Overview
`BeepSwitch` is an on/off toggle switch with 4 painter styles, icons, drag-to-toggle, animations, and data binding.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
```

## Painter Styles
- **Material3** - Material Design 3
- **Fluent2** - Microsoft Fluent 2
- **iOS** - Apple iOS style
- **Minimal** - Clean minimal

## SwitchOrientation
```csharp
public enum SwitchOrientation
{
    Horizontal,  // Left/right toggle (default)
    Vertical     // Up/down toggle
}
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Checked` | `bool` | On (true) or Off (false) |
| `Orientation` | `SwitchOrientation` | Toggle direction |
| `OnLabel` | `string` | Label for On state |
| `OffLabel` | `string` | Label for Off state |
| `OnImagePath` | `string` | Image for On state |
| `OffImagePath` | `string` | Image for Off state |
| `OnIconName` | `string` | Icon library name for On |
| `OffIconName` | `string` | Icon library name for Off |
| `DragToToggleEnabled` | `bool` | Enable drag toggle (true) |

## Usage Examples

### Basic Switch
```csharp
var toggle = new BeepSwitch
{
    Checked = false,
    OnLabel = "On",
    OffLabel = "Off"
};

toggle.CheckedChanged += (s, e) =>
{
    Console.WriteLine($"Switched: {toggle.Checked}");
};
```

### With Icons
```csharp
var toggle = new BeepSwitch();
toggle.UseCheckmarkIcons();  // check/close icons
// or
toggle.UsePowerIcons();      // power on/off
// or
toggle.UseLightIcons();      // lightbulb on/off
```

### Custom Icons
```csharp
var toggle = new BeepSwitch
{
    OnIconName = "wifi",
    OffIconName = "wifi_off"
};
```

## Events
| Event | Description |
|-------|-------------|
| `CheckedChanged` | State changed |

## Features
- Animated toggle transitions
- Drag-to-toggle support
- Data binding properties
- Theme integration

## Related Controls
- `BeepCheckBox` - Standard checkbox
- `BeepRadioGroup` - Radio buttons
