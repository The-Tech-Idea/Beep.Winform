# BeepToggle Skill

## Overview
`BeepToggle` provides Material Design toggle switches as an alternative to checkboxes for on/off states.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Toggle;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `IsToggled` | `bool` | Current toggle state |
| `Text` | `string` | Toggle label |
| `UseThemeColors` | `bool` | Use theme colors |
| `ShowText` | `bool` | Show/hide label |

## Usage Examples

### Basic Toggle
```csharp
var toggle = new BeepToggle
{
    Text = "Dark Mode",
    IsToggled = false,
    UseThemeColors = true
};
```

### Handle State Change
```csharp
toggle.Toggled += (s, e) =>
{
    bool isOn = toggle.IsToggled;
    // Process state change
};
```

### Toggle Without Label
```csharp
var toggle = new BeepToggle
{
    IsToggled = true,
    ShowText = false
};
```

## Events
| Event | Description |
|-------|-------------|
| `Toggled` | Fires when toggle state changes |
| `Click` | Click event |

## Related Controls
- `BeepSwitch` - Alternative switch style
- `BeepCheckBox` - Traditional checkbox
