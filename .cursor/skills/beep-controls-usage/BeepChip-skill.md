# BeepChip Skill

## Overview
`BeepMultiChipGroup` displays a group of selectable chips/tags with 17 visual styles and keyboard navigation.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Chips;
```

## ChipStyle (17 styles)
| Category | Styles |
|----------|--------|
| **Shape** | `Pill`, `Square` |
| **Themed** | `Likeable`, `Ingredient`, `Avatar` |
| **Effects** | `Elevated`, `Shaded`, `Colorful`, `Soft`, `Smooth` |
| **Style** | `Modern`, `Classic`, `Professional`, `HighContrast`, `Minimalist` |
| **Border** | `Dashed`, `Bold` |
| **Default** | `Default` |

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `ChipStyle` | `ChipStyle` | Visual style/painter |
| `Chips` | Collection | Chip items |
| `FocusedIndex` | `int` | Keyboard focused chip |
| `MultiSelect` | `bool` | Allow multiple selection |
| `Removable` | `bool` | Show remove (X) button |
| `UseThemeColors` | `bool` | Use theme colors |

## Usage Examples

### Basic Chip Group
```csharp
var chips = new BeepMultiChipGroup
{
    ChipStyle = ChipStyle.Pill
};

chips.AddChip("JavaScript");
chips.AddChip("Python");
chips.AddChip("C#");
```

### Avatar Chips
```csharp
var teamChips = new BeepMultiChipGroup
{
    ChipStyle = ChipStyle.Avatar
};

teamChips.AddChip("John", imagePath: "john.png");
teamChips.AddChip("Jane", imagePath: "jane.png");
```

### Ingredient Chips (removable)
```csharp
var ingredients = new BeepMultiChipGroup
{
    ChipStyle = ChipStyle.Ingredient,
    Removable = true
};

ingredients.ChipRemoved += (s, e) => 
{
    Console.WriteLine($"Removed: {e.ChipText}");
};
```

### Colorful Tags
```csharp
var tags = new BeepMultiChipGroup
{
    ChipStyle = ChipStyle.Colorful
};
```

## Events
| Event | Description |
|-------|-------------|
| `ChipSelected` | Chip selected |
| `ChipRemoved` | Chip removed |
| `SelectionChanged` | Selection state changed |

## Related Controls
- `BeepChipListBox` - Combined chip + listbox
