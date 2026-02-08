# BeepMenuBar Skill

## Overview
`BeepMenuBar` is a horizontal menu bar using `SimpleItem` data, painter-based per-item rendering, DPI-aware sizing, and popup submenus.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `MenuItems` | `BindingList<SimpleItem>` | Menu item collection |
| `SelectedItem` | `SimpleItem` | Currently selected |
| `SelectedIndex` | `int` | Selected item index |
| `MenuItemHeight` | `int` | Item height (DPI-scaled) |
| `MenuItemWidth` | `int` | Item width (default: 60) |
| `ImageSize` | `int` | Icon size (DPI-scaled) |
| `TextFont` | `Font` | Menu item font |
| `Height` | `int` | Bar height (preserved if set manually) |

## Usage Examples

### Basic MenuBar
```csharp
var menuBar = new BeepMenuBar
{
    Dock = DockStyle.Top,
    UseThemeColors = true
};

menuBar.MenuItems.Add(new SimpleItem { Text = "File" });
menuBar.MenuItems.Add(new SimpleItem { Text = "Edit" });
menuBar.MenuItems.Add(new SimpleItem { Text = "View" });
```

### With Submenus
```csharp
var fileMenu = new SimpleItem
{
    Text = "File",
    Children = new List<SimpleItem>
    {
        new SimpleItem { Text = "New", MethodName = "NewFile" },
        new SimpleItem { Text = "Open", MethodName = "OpenFile" },
        new SimpleItem { Text = "Save", MethodName = "SaveFile" }
    }
};
menuBar.MenuItems.Add(fileMenu);
```

### Handle Selection
```csharp
menuBar.SelectedItemChanged += (s, e) =>
{
    Console.WriteLine($"Selected: {e.SelectedItem?.Text}");
    
    // If MethodName is set, global function is called automatically
};
```

### With Icons
```csharp
menuBar.MenuItems.Add(new SimpleItem
{
    Text = "Settings",
    ImagePath = "settings.svg"
});
```

## Events
| Event | Description |
|-------|-------------|
| `SelectedItemChanged` | Menu item selected |

## Features
- DPI-aware sizing with `DpiScalingHelper`
- BeepStyling integration for per-item painting
- Hover and selected states with theme colors
- Popup submenus via `ShowContextMenu`
- Transparent background support
