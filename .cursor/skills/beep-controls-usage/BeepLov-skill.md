# BeepListofValuesBox Skill

## Overview
`BeepListofValuesBox` (LOV) provides a lookup/picker control for selecting values from a data source with search filtering.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Lovs;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `DataSource` | `object` | Data source for items |
| `DisplayMember` | `string` | Property for display |
| `ValueMember` | `string` | Property for value |
| `SelectedValue` | `object` | Current selected value |
| `SelectedItem` | `object` | Current selected item |
| `SearchEnabled` | `bool` | Enable search filter |
| `MultiSelect` | `bool` | Allow multiple selection |

## Usage Examples

### Basic LOV
```csharp
var lov = new BeepListofValuesBox
{
    DataSource = customers,
    DisplayMember = "Name",
    ValueMember = "Id"
};

lov.SelectedValueChanged += (s, e) =>
{
    var customerId = lov.SelectedValue;
};
```

### With Search
```csharp
var lov = new BeepListofValuesBox
{
    DataSource = products,
    DisplayMember = "ProductName",
    ValueMember = "ProductId",
    SearchEnabled = true
};
```

### Multi-Select
```csharp
var lov = new BeepListofValuesBox
{
    MultiSelect = true
};

// Get selected items
var selectedIds = lov.SelectedValues;
```

## Events
| Event | Description |
|-------|-------------|
| `SelectedValueChanged` | Selection changed |
| `SearchTextChanged` | Search filter changed |

## Related Controls
- `BeepComboBox` - Dropdown selection
- `BeepListBox` - List selection
