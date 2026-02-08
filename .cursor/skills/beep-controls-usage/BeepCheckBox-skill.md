# BeepCheckBox Skill

## Overview
`BeepCheckBox<T>` is a generic checkbox control with Material Design styling, multiple visual styles (8 painters), and support for different value types (bool, char, string).

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
```

## Available Classes
| Class | Description |
|-------|-------------|
| `BeepCheckBox<T>` | Generic checkbox with custom value types |
| `BeepCheckBoxBool` | Boolean checkbox (true/false) |
| `BeepCheckBoxChar` | Character checkbox ('Y'/'N') |
| `BeepCheckBoxString` | String checkbox ("YES"/"NO") |

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `State` | `CheckBoxState` | Checked, Unchecked, or Indeterminate |
| `CheckBoxStyle` | `CheckBoxStyle` | Visual style (Material3, Modern, Classic, etc.) |
| `Text` | `string` | Checkbox label text |
| `CheckBoxSize` | `int` | Size of the checkbox (default: 15) |
| `Spacing` | `int` | Space between checkbox and text (default: 5) |
| `HideText` | `bool` | Hide the text label |
| `TextAlignRelativeToCheckBox` | `TextAlignment` | Position of text (Left/Right) |
| `CheckedValue` | `T` | Value when checked |
| `UncheckedValue` | `T` | Value when unchecked |
| `CurrentValue` | `T` | Current value |
| `ImagePath` | `string` | Custom check mark image (SVG/PNG) |
| `TextFont` | `Font` | Font for the label |
| `AutoSize` | `bool` | Auto-resize based on content |

## CheckBoxStyle Options
- `Material3` - Material Design 3 style
- `Modern` - Modern flat design
- `Classic` - Traditional bordered style
- `Minimal` - Clean minimal style
- `iOS` - iOS-style rounded
- `Fluent2` - Fluent Design 2
- `Switch` - Toggle switch appearance
- `Button` - Button-style appearance

## Usage Examples

### Basic Boolean Checkbox
```csharp
var chk = new BeepCheckBoxBool
{
    Text = "I agree to terms",
    CheckBoxStyle = CheckBoxStyle.Material3,
    UseThemeColors = true
};
// CheckedValue = true, UncheckedValue = false (default)
```

### Character Value Checkbox
```csharp
var chk = new BeepCheckBoxChar
{
    Text = "Active",
    CheckBoxStyle = CheckBoxStyle.Modern
};
// CheckedValue = 'Y', UncheckedValue = 'N' (default)
char value = chk.CurrentValue;
```

### String Value Checkbox
```csharp
var chk = new BeepCheckBoxString
{
    Text = "Enabled",
    CheckBoxStyle = CheckBoxStyle.iOS
};
// CheckedValue = "YES", UncheckedValue = "NO" (default)
string value = chk.CurrentValue;
```

### Custom Generic Checkbox
```csharp
var chk = new BeepCheckBox<int>
{
    Text = "Priority",
    CheckedValue = 1,
    UncheckedValue = 0,
    CurrentValue = 0
};
```

### Handle State Change
```csharp
chk.StateChanged += (s, e) =>
{
    var state = chk.State; // CheckBoxState.Checked/Unchecked/Indeterminate
    var value = chk.CurrentValue;
};
```

### Switch Style Checkbox
```csharp
var toggle = new BeepCheckBoxBool
{
    Text = "Dark Mode",
    CheckBoxStyle = CheckBoxStyle.Switch,
    UseThemeColors = true
};
```

## Events
| Event | Description |
|-------|-------------|
| `StateChanged` | Fires when checkbox state changes |
| `Click` | Click event |

## Related Controls
- `BeepToggle` - Alternative toggle switch
- `BeepSwitch` - Animated switch
- `BeepRadioGroup` - Radio button groups
