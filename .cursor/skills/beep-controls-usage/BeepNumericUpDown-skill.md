# BeepNumericUpDown Skill

## Overview
Numeric input control with increment/decrement buttons.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Numerics;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Value` | `decimal` | Current value |
| `Minimum` | `decimal` | Minimum value |
| `Maximum` | `decimal` | Maximum value |
| `Increment` | `decimal` | Step amount |
| `DecimalPlaces` | `int` | Decimal precision |

## Events
| Event | Description |
|-------|-------------|
| `ValueChanged` | Value changed |

## Usage
```csharp
var numeric = new BeepNumericUpDown
{
    Value = 50,
    Minimum = 0,
    Maximum = 100,
    Increment = 5,
    DecimalPlaces = 2
};
numeric.ValueChanged += (s, e) => UpdateQuantity(numeric.Value);
```
