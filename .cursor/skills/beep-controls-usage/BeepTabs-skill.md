# BeepTabs Skill

## Overview
`BeepTabs` is a fully-customizable tab control with 7 tab styles, painter-based rendering, drag-drop reordering, close buttons, and smooth animations.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## Key Properties

### Style Properties
| Property | Type | Description |
|----------|------|-------------|
| `TabStyle` | `TabStyle` | Visual style (7 types) |
| `Theme` | `string` | Theme name |
| `HeaderHeight` | `int` | Tab header height |
| `HeaderPosition` | `TabHeaderPosition` | Top/Bottom/Left/Right |
| `TabAnimationDuration` | `int` | Animation speed (ms) |

### Behavior Properties
| Property | Type | Description |
|----------|------|-------------|
| `ShowCloseButtons` | `bool` | Show tab close buttons |
| `AllowDrop` | `bool` | Enable drag-drop reorder |
| `SelectTab` | `TabPage` | Select by TabPage reference |
| `SelectTabByIndex` | `int` | Select by index |

### Read-Only
| Property | Type | Description |
|----------|------|-------------|
| `LastTabSelected` | `int` | Last selected tab index |
| `CurrentTheme` | `IBeepTheme` | Active theme object |

## TabStyle Options (7 types)
```csharp
public enum TabStyle
{
    Classic,     // Traditional tab look with borders
    Underline,   // Material-style underline indicator
    Capsule,     // Rounded pill-shaped tabs
    Minimal,     // Flat minimal design
    Segmented,   // iOS-style segmented control
    Card,        // Card-style tabs with shadow
    Button       // Button-style tabs
}
```

## TabHeaderPosition
```csharp
public enum TabHeaderPosition
{
    Top,     // Headers at top (default)
    Bottom,  // Headers at bottom
    Left,    // Headers on left (vertical)
    Right    // Headers on right (vertical)
}
```

## Usage Examples

### Basic Tabs
```csharp
var tabs = new BeepTabs
{
    TabStyle = TabStyle.Underline,
    HeaderHeight = 40,
    ShowCloseButtons = true
};
tabs.TabPages.Add(new TabPage("Tab 1"));
tabs.TabPages.Add(new TabPage("Tab 2"));
```

### Different Positions
```csharp
var tabs = new BeepTabs
{
    TabStyle = TabStyle.Segmented,
    HeaderPosition = TabHeaderPosition.Left,
    HeaderHeight = 100  // Width for vertical
};
```

### Apply Style Preset
```csharp
tabs.SetTabStylePreset(TabStyle.Capsule);
```

### Handle Tab Removal
```csharp
tabs.TabRemoved += (s, e) =>
{
    Console.WriteLine($"Tab '{e.TabPage.Text}' was removed");
};
```

## Events
| Event | Description |
|-------|-------------|
| `TabRemoved` | Fired when a tab is closed |
| `SelectedIndexChanged` | Tab selection changed |

## Features
- **Drag-drop reordering** - Drag tabs to reorder
- **Smooth animations** - Underline and style transitions
- **Close buttons** - Optional per-tab close icons
- **Theme integration** - Automatic theme application
- **DPI scaling** - High-DPI support

## Related Controls
- `BeepCard` - For tabbed card layouts
- `BeepAccordionMenu` - Collapsible sections
