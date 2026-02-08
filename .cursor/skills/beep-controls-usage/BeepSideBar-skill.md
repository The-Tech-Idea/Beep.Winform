# BeepSideBar Skill

## Overview
`BeepSideBar` is a modern sidebar with 20+ visual style painters, collapse/expand animations, hierarchical items, and selection tracking.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.SideBar;
using TheTechIdea.Beep.Winform.Controls.Models;
```

## Painter Styles (20+)
- **Material**: Material3, MaterialYou
- **Fluent**: Fluent2, Windows11Mica  
- **Apple**: MacOSBigSur, iOS15
- **Modern**: Discord, Stripe, Vercel, Notion, Tailwind
- **Effects**: Glassmorphism, Neumorphic, DarkGlow, Cyberpunk
- **Others**: AntDesign, ChakraUI, PillRail, FinSet, Universal

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Items` | `BindingList<SimpleItem>` | Menu items |
| `SelectedItem` | `SimpleItem` | Selected item |
| `IsCollapsed` | `bool` | Collapse state |
| `ExpandedWidth` | `int` | Expanded width (200) |
| `CollapsedWidth` | `int` | Collapsed width (64) |
| `ItemHeight` | `int` | Item height (44) |
| `AccentColor` | `Color` | Accent color |
| `UseThemeColors` | `bool` | Use theme colors |
| `EnableRailShadow` | `bool` | Shadow effects |
| `ChromeCornerRadius` | `int` | Corner radius (10) |
| `DefaultItemImagePath` | `string` | Fallback icon |
| `UseExpandCollapseIcon` | `bool` | Use icon for toggle |
| `ExpandIconPath` | `string` | Expand icon |
| `CollapseIconPath` | `string` | Collapse icon |
| `ClickTogglesExpansion` | `bool` | Click toggles children |
| `ClickTogglesExpansionMode` | `ClickTogglesExpansionMode` | Toggle behavior |

## ClickTogglesExpansionMode
```csharp
public enum ClickTogglesExpansionMode
{
    ToggleThenSelect,  // Toggle first, then select
    SelectThenToggle,  // Select first, then toggle
    ToggleOnly         // Only toggle, no select
}
```

## Usage Examples

### Basic SideBar
```csharp
var sidebar = new BeepSideBar
{
    ExpandedWidth = 220,
    CollapsedWidth = 60,
    ItemHeight = 48
};

sidebar.Items.Add(new SimpleItem { Text = "Home", ImagePath = "home.svg" });
sidebar.Items.Add(new SimpleItem 
{
    Text = "Settings",
    ImagePath = "settings.svg",
    Children = new List<SimpleItem>
    {
        new SimpleItem { Text = "Profile" },
        new SimpleItem { Text = "Security" }
    }
});

sidebar.ItemClicked += item => Console.WriteLine($"Clicked: {item.Text}");
```

### With Collapse
```csharp
sidebar.IsCollapsed = false; // Expanded
sidebar.CollapseStateChanged += isCollapsed =>
{
    Console.WriteLine($"Collapsed: {isCollapsed}");
};
sidebar.Toggle(); // Toggle collapse state
```

## Events
| Event | Description |
|-------|-------------|
| `ItemClicked` | Item clicked |
| `ItemExpansionChanged` | Parent expanded/collapsed |
| `ItemExpansionChanging` | Before expansion (cancelable) |
| `CollapseStateChanged` | Sidebar collapsed/expanded |

## Related Controls
- `BeepAccordionMenu` - Accordion-style menu
- `BeepNavBar` - Horizontal navigation
