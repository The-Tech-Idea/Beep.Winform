# BeepTimePicker Skill

## Overview
Time selection control with analog/digital display and theme support.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Value` | `TimeSpan` | Selected time |
| `Is24Hour` | `bool` | 24-hour or 12-hour format |
| `ShowSeconds` | `bool` | Display seconds |

## Events
| Event | Description |
|-------|-------------|
| `ValueChanged` | Time selection changed |

## Usage
```csharp
var picker = new BeepTimePicker
{
    Value = DateTime.Now.TimeOfDay,
    Is24Hour = true
};
picker.ValueChanged += (s, e) => Console.WriteLine(picker.Value);
```
