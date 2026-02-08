# BeepComboBox Skill

## Overview
`BeepComboBox` is a Material Design dropdown with 19 visual styles, auto-complete, multi-select chips, search, and theme integration. Uses `SimpleItem` for items with `BindingList<SimpleItem>`.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
```

## Key Properties

### Data Properties
| Property | Type | Description |
|----------|------|-------------|
| `ListItems` | `BindingList<SimpleItem>` | Collection of items |
| `SelectedItem` | `SimpleItem` | Currently selected item |
| `SelectedItems` | `List<SimpleItem>` | Selected items (multi-select) |
| `SelectedIndex` | `int` | Selected item index |
| `SelectedText` | `string` | Text of selected item |
| `SelectedValue` | `object` | Value of selected item |

### Appearance Properties
| Property | Type | Description |
|----------|------|-------------|
| `ComboBoxType` | `ComboBoxType` | Visual style (19 types) |
| `PlaceholderText` | `string` | Placeholder text |
| `TextFont` | `Font` | Display font |
| `DropdownIconPath` | `string` | Custom dropdown icon |

### Behavior Properties
| Property | Type | Description |
|----------|------|-------------|
| `IsEditable` | `bool` | Allow text editing |
| `ShowSearchInDropdown` | `bool` | Show search in popup |
| `AutoComplete` | `bool` | Enable auto-complete |
| `AutoCompleteMode` | `BeepAutoCompleteMode` | None, Prefix, Fuzzy, Full |
| `AutoCompleteMinLength` | `int` | Min chars for auto-complete |
| `AutoCompleteDelay` | `int` | Delay before auto-complete (ms) |
| `MaxSuggestions` | `int` | Max suggestions shown |
| `AllowMultipleSelection` | `bool` | Enable multi-select |
| `MaxDisplayChips` | `int` | Max chips shown (multi-select) |
| `MaxDropdownHeight` | `int` | Max dropdown height |
| `IsLoading` | `bool` | Shows loading spinner |
| `EnableAnimations` | `bool` | Enable animations |
| `AnimationDuration` | `int` | Animation duration (ms) |

## ComboBoxType Visual Styles (19 types)
- `Minimal` - Simple rectangular, minimal border
- `Outlined` - Clear border, rounded corners
- `Rounded` - Prominent border radius
- `MaterialOutlined` - Material Design with floating label
- `Filled` - Filled background with shadow
- `Borderless` - Clean minimal design
- `Standard` - Default Windows style
- `BlueDropdown` - Blue themed
- `GreenDropdown` - Green/success themed
- `Inverted` - Dark background
- `Error` - Red/error styling
- `MultiSelectChips` - Multi-select with chips
- `SearchableDropdown` - Integrated search
- `WithIcons` - Icons next to items
- `Menu` - Menu-style with categories
- `CountrySelector` - With flags
- `SmoothBorder` - Smooth gradient border
- `DarkBorder` - Dark prominent border
- `PillCorners` - Pill-shaped

## Data Model
```csharp
public class SimpleItem
{
    public string Text { get; set; }        // Display text
    public string Value { get; set; }       // Value 
    public string ImagePath { get; set; }   // Icon path
    public object Item { get; set; }        // Underlying object
    public bool Disabled { get; set; }      // Item disabled
}
```

## Usage Examples

### Basic ComboBox
```csharp
var cmb = new BeepComboBox
{
    ComboBoxType = ComboBoxType.MaterialOutlined,
    PlaceholderText = "Select country",
    UseThemeColors = true
};
cmb.ListItems.Add(new SimpleItem { Text = "USA", Value = "US" });
cmb.ListItems.Add(new SimpleItem { Text = "UK", Value = "GB" });
cmb.ListItems.Add(new SimpleItem { Text = "Canada", Value = "CA" });
```

### With Search
```csharp
var cmb = new BeepComboBox
{
    ComboBoxType = ComboBoxType.SearchableDropdown,
    ShowSearchInDropdown = true,
    AutoComplete = true,
    AutoCompleteMode = BeepAutoCompleteMode.Fuzzy
};
```

### Multi-Select with Chips
```csharp
var cmb = new BeepComboBox
{
    ComboBoxType = ComboBoxType.MultiSelectChips,
    AllowMultipleSelection = true,
    MaxDisplayChips = 3
};
cmb.SelectedItemsChanged += (s, e) =>
{
    var selected = cmb.SelectedItems;
    Console.WriteLine($"{selected.Count} items selected");
};
```

### Handle Selection
```csharp
cmb.SelectedItemChanged += (s, e) =>
{
    var item = cmb.SelectedItem;
    Console.WriteLine($"Selected: {item?.Text} = {item?.Value}");
};
```

### With Loading State
```csharp
cmb.IsLoading = true;  // Show spinner
await LoadDataAsync();
cmb.IsLoading = false; // Hide spinner
```

## Events
| Event | Description |
|-------|-------------|
| `SelectedItemChanged` | Single selection changed |
| `SelectedItemsChanged` | Multi-selection changed |
| `DropDownOpened` | Dropdown opened |
| `DropDownClosed` | Dropdown closed |

## Related Controls
- `BeepDropDownCheckBoxSelect` - Dropdown with checkboxes
- `BeepListBox` - Full list control
- `BeepSelect` - Advanced selection control
