# BeepRadioGroup Skill

## Overview
Modern radio group control with 11 render styles, multiple selection modes, and flexible layouts.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
```

---

## RadioGroupRenderStyle Enum (11 Styles)
| Style | Description |
|-------|-------------|
| `Material` | Material Design circles |
| `Card` | Elevated card selections |
| `Chip` | Compact chip/tag style |
| `Circular` | Circular buttons |
| `Flat` | Minimal flat design |
| `Checkbox` | Checkbox appearance |
| `Button` | Button-like selections |
| `Toggle` | Toggle switch style |
| `Segmented` | iOS-style segmented control |
| `Pill` | Rounded pill buttons |
| `Tile` | Large tile selections |

---

## Key Properties

### Appearance
| Property | Type | Description |
|----------|------|-------------|
| `RenderStyle` | `RadioGroupRenderStyle` | Visual style |
| `Orientation` | `RadioGroupOrientation` | Vertical/Horizontal |
| `ItemSpacing` | `int` | Space between items |
| `ItemPadding` | `Padding` | Padding around items |
| `MaxImageSize` | `Size` | Image size limit |
| `AutoSizeItems` | `bool` | Auto-size to content |

### Selection
| Property | Type | Description |
|----------|------|-------------|
| `AllowMultipleSelection` | `bool` | Multi-select mode |
| `SelectedValue` | `string` | Single selected value |
| `SelectedItems` | `List<SimpleItem>` | All selected items |

### Data
| Property | Type | Description |
|----------|------|-------------|
| `Items` | `List<SimpleItem>` | Radio items |

---

## SimpleItem Data Model
```csharp
public class SimpleItem
{
    public string Text { get; set; }
    public string SubText { get; set; }
    public string ImagePath { get; set; }
    public object Value { get; set; }
    public bool IsEnabled { get; set; }
}
```

---

## Key Methods

### Item Management
```csharp
radioGroup.AddItem("Option 1"); // Simple text
radioGroup.AddItem("Option 2", Svgs.Check, "Description");
radioGroup.AddItem(new SimpleItem { Text = "Custom", Value = 123 });
radioGroup.RemoveItem(item);
radioGroup.ClearItems();
```

### Selection
```csharp
radioGroup.SelectItem("value");
radioGroup.DeselectItem("value");
radioGroup.ClearSelection();
radioGroup.SelectAll(); // Multi-select only
```

### Custom Renderer
```csharp
radioGroup.RegisterRenderer(RadioGroupRenderStyle.Material, new MyCustomRenderer());
```

---

## Events
- `SelectionChanged` - Selection changed
- `ItemSelectionChanged` - Individual item toggled
- `OnValueChanged` - Value changed (from BaseControl)

---

## Usage Examples

### Basic Setup
```csharp
var radioGroup = new BeepRadioGroup
{
    RenderStyle = RadioGroupRenderStyle.Material,
    Orientation = RadioGroupOrientation.Vertical
};

radioGroup.AddItem("Small");
radioGroup.AddItem("Medium");
radioGroup.AddItem("Large");

radioGroup.SelectionChanged += (s, e) => {
    var selected = radioGroup.SelectedValue;
};
```

### Card Style with Images
```csharp
radioGroup.RenderStyle = RadioGroupRenderStyle.Card;
radioGroup.AddItem("Credit Card", Svgs.CreditCard, "Pay with credit card");
radioGroup.AddItem("PayPal", Svgs.PayPal, "Pay with PayPal");
radioGroup.AddItem("Bitcoin", Svgs.Bitcoin, "Pay with crypto");
```

### Multi-Select Checkboxes
```csharp
radioGroup.RenderStyle = RadioGroupRenderStyle.Checkbox;
radioGroup.AllowMultipleSelection = true;

radioGroup.AddItem("Feature 1");
radioGroup.AddItem("Feature 2");
radioGroup.AddItem("Feature 3");

// Get all selected
var selected = radioGroup.SelectedItems;
```

### Segmented Control
```csharp
radioGroup.RenderStyle = RadioGroupRenderStyle.Segmented;
radioGroup.Orientation = RadioGroupOrientation.Horizontal;

radioGroup.AddItem("Day");
radioGroup.AddItem("Week");
radioGroup.AddItem("Month");
radioGroup.AddItem("Year");
```

### Tile Selection
```csharp
radioGroup.RenderStyle = RadioGroupRenderStyle.Tile;
radioGroup.AutoSizeItems = true;
radioGroup.MaxImageSize = new Size(48, 48);

radioGroup.AddItem("Basic Plan", Svgs.Star, "$9/month");
radioGroup.AddItem("Pro Plan", Svgs.StarFilled, "$19/month");
```

---

## Hierarchical Radio Group

For tree-structured options:
```csharp
var hierarchical = new BeepHierarchicalRadioGroup();
// Supports nested items with parent-child relationships
```

## Related
- `BeepCheckBox` - Single checkbox
- `BeepToggleSwitch` - Toggle control
- `BeepListBox` - List selection
