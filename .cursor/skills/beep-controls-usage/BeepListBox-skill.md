# BeepListBox Skill

## Overview
`BeepListBox` is a modern list box control with painter methodology, 10+ visual styles, search, checkboxes, and theme integration. It extends `BaseControl` with advanced styling capabilities.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `ListBoxType` | `ListBoxType` | Visual style (Standard, Material, Minimal, etc.) |
| `ListItems` | `BindingList<SimpleItem>` | Collection of items |
| `SelectedItem` | `SimpleItem` | Currently selected item |
| `SelectedItems` | `List<SimpleItem>` | Multiple selected items (checkbox mode) |
| `SelectedIndex` | `int` | Index of selected item |
| `ShowSearch` | `bool` | Show/hide search box |
| `ShowCheckBox` | `bool` | Show/hide checkboxes |
| `ShowImage` | `bool` | Show/hide item icons |
| `ShowHilightBox` | `bool` | Show/hide hover highlights |
| `AllowMultipleSelection` | `bool` | Enable multiple selection |
| `MenuItemHeight` | `int` | Height of each item (DPI-scaled) |
| `ImageSize` | `int` | Size of item icons (DPI-scaled) |
| `TextFont` | `Font` | Font for item text |
| `Theme` | `ThemeEnum` | Theme selection for styling |

## ListBoxType Visual Styles
- `Standard` - Classic list appearance
- `Material` - Material Design outlined style
- `Minimal` - Ultra-minimal with accent line
- `Outlined` - Traditional outlined list
- `Borderless` - Clean, minimal style
- `RadioSelection` - Radio-button style selection
- `CategoryChips` - Pill-shaped category tags
- `Grouped` - Section headers with groups
- `Searchable` - Integrated search with filtering
- `TeamMembers` - Avatar-style member list

## Data Model
```csharp
public class SimpleItem
{
    public string IDField { get; set; }
    public string DisplayField { get; set; }
    public string ImagePath { get; set; }
    public object Tag { get; set; }
    public bool Disabled { get; set; }
    public List<SimpleItem> Children { get; set; }
}
```

## Usage Examples

### Basic ListBox
```csharp
var listBox = new BeepListBox
{
    ListBoxType = ListBoxType.Standard,
    ShowImage = true,
    UseThemeColors = true
};

listBox.ListItems.Add(new SimpleItem { DisplayField = "Item 1", IDField = "1" });
listBox.ListItems.Add(new SimpleItem { DisplayField = "Item 2", IDField = "2" });
```

### With Checkboxes (Multi-Select)
```csharp
var listBox = new BeepListBox
{
    ShowCheckBox = true,
    AllowMultipleSelection = true,
    ListBoxType = ListBoxType.Material
};

listBox.CheckBoxChanged += (s, e) =>
{
    var checkedItems = listBox.SelectedItems;
    Console.WriteLine($"{checkedItems.Count} items checked");
};
```

### With Search
```csharp
var listBox = new BeepListBox
{
    ShowSearch = true,
    ListBoxType = ListBoxType.Searchable
};
// Automatically filters as user types
```

### Handle Selection
```csharp
listBox.SelectedItemChanged += (s, e) =>
{
    var selected = listBox.SelectedItem;
    Console.WriteLine($"Selected: {selected?.DisplayField}");
};
```

### With Icons
```csharp
listBox.ShowImage = true;
listBox.ImageSize = 24;
listBox.ListItems.Add(new SimpleItem 
{ 
    DisplayField = "Settings",
    ImagePath = "GFX/icons/settings.svg"
});
```

### Grouped Style
```csharp
var listBox = new BeepListBox
{
    ListBoxType = ListBoxType.Grouped
};
// Section headers with grouped items
```

## Events
| Event | Description |
|-------|-------------|
| `SelectedItemChanged` | Fired when selection changes |
| `CheckBoxChanged` | Fired when checkbox state changes |
| `ItemClicked` | Fired when item is clicked |
| `SearchTextChanged` | Fired when search text changes |

## Architecture Features
- **Partial Class Structure**: Core, Properties, Events, Methods, Drawing
- **Helper System**: BeepListBoxHelper, LayoutHelper, HitTestHelper
- **Painter Pattern**: 10+ pluggable visual painters
- **DPI Awareness**: Automatic scaling for high-DPI displays
- **Theme Integration**: Uses `BeepThemesManager` for colors

## Related Controls
- `BeepComboBox` - Dropdown with popup list
- `BeepTree` - Hierarchical tree view
