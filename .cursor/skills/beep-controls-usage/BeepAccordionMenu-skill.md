# BeepAccordionMenu Skill

## Overview
`BeepAccordionMenu` is a collapsible menu with expandable parent items, 7 visual styles via painters, and smooth animations.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.AccordionMenus;
using TheTechIdea.Beep.Winform.Controls.Models;
```

## AccordionStyle (7 types)
```csharp
public enum AccordionStyle
{
    Classic,      // Traditional style
    Modern,       // Contemporary look
    Minimal,      // Clean, minimal
    Material3,    // Material Design 3
    Fluent2,      // Microsoft Fluent 2
    iOS,          // Apple iOS style
    Custom        // Custom styling
}
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `ListItems` | `BindingList<SimpleItem>` | Menu items |
| `AccordionStyle` | `AccordionStyle` | Visual style |
| `Title` | `string` | Header title |
| `SelectedItem` | `SimpleItem` | Selected item |
| `ItemHeight` | `int` | Parent item height (40) |
| `ChildItemHeight` | `int` | Child item height (30) |
| `IndentationWidth` | `int` | Child indentation (20) |
| `ExpandedWidth` | `int` | Expanded width (200) |
| `CollapsedWidth` | `int` | Collapsed width (64) |
| `AnimationStep` | `int` | Animation speed |
| `AnimationDelay` | `int` | Animation delay (ms) |

## Usage Examples

### Basic Menu
```csharp
var menu = new BeepAccordionMenu
{
    AccordionStyle = AccordionStyle.Material3,
    Title = "Main Menu"
};

menu.ListItems.Add(new SimpleItem { Text = "Dashboard", ImagePath = "dashboard.svg" });
menu.ListItems.Add(new SimpleItem 
{
    Text = "Settings",
    ImagePath = "settings.svg",
    Children = new List<SimpleItem>
    {
        new SimpleItem { Text = "Profile" },
        new SimpleItem { Text = "Security" }
    }
});
```

### With Style
```csharp
var menu = new BeepAccordionMenu
{
    AccordionStyle = AccordionStyle.Fluent2,
    ItemHeight = 48,
    ChildItemHeight = 36,
    IndentationWidth = 24
};
```

## Events
| Event | Description |
|-------|-------------|
| `ItemClick` | Menu item clicked |
| `ToggleClicked` | Toggle button clicked |
| `HeaderExpandedChanged` | Header expand state changed |

## Related Controls
- `BeepSideBar` - Sidebar with collapse support
- `BeepNavBar` - Horizontal navigation
