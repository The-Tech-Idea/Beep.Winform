# BeepPanel Skill

## Overview
`BeepPanel` is a container control with title, shapes, collapsible behavior, elevation, scrolling, and state management.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## PanelShape (4 shapes)
| Value | Description |
|-------|-------------|
| `Rectangle` | Standard rectangular panel |
| `RoundedRectangle` | Rounded corners |
| `Ellipse` | Elliptical/circular |
| `Custom` | Custom GraphicsPath |

## PanelTitleStyle (7 styles)
| Value | Description |
|-------|-------------|
| `GroupBox` | Title breaks border (like GroupBox) |
| `Above` | Title above with line |
| `Below` | Title at bottom |
| `Left` | Title on left side |
| `Right` | Title on right side |
| `Overlay` | Title inside at top |
| `TopHeader` | Solid header bar |

## PanelState (4 states)
| Value | Description |
|-------|-------------|
| `Normal` | Display content |
| `Loading` | Show loading skeleton |
| `Empty` | Show empty message |
| `Error` | Show error message |

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `TitleText` | `string` | Panel title |
| `ShowTitle` | `bool` | Show/hide title |
| `TitleStyle` | `PanelTitleStyle` | Title display style |
| `TitleAlignment` | `ContentAlignment` | Title position |
| `TitleGap` | `int` | Gap around title text |
| `PanelShape` | `PanelShape` | Shape style |
| `CustomShapePath` | `GraphicsPath` | Custom shape |
| `ShowTitleLine` | `bool` | Line below title |
| `TitleLineColor` | `Color` | Line color |
| `State` | `PanelState` | Current state |
| `EmptyStateMessage` | `string` | Empty state text |
| `Collapsible` | `bool` | Enable collapse |
| `IsCollapsed` | `bool` | Collapsed state |
| `CollapseAnimationDuration` | `int` | Animation ms |
| `Elevation` | `int` | Shadow depth (0-5) |
| `ElevationOnHover` | `int` | Hover elevation |
| `StickyHeader` | `bool` | Fixed header on scroll |
| `AutoScroll` | `bool` | Enable scrollbars |

## Usage Examples

### Basic Panel
```csharp
var panel = new BeepPanel
{
    TitleText = "Settings",
    TitleStyle = PanelTitleStyle.GroupBox,
    PanelShape = PanelShape.RoundedRectangle
};
```

### Collapsible Panel
```csharp
var panel = new BeepPanel
{
    TitleText = "Details",
    Collapsible = true,
    CollapseAnimationDuration = 300
};

// Toggle collapse
panel.IsCollapsed = !panel.IsCollapsed;
```

### Elevated Card Panel
```csharp
var card = new BeepPanel
{
    PanelShape = PanelShape.RoundedRectangle,
    Elevation = 2,
    ElevationOnHover = 4
};
```

### State Management
```csharp
panel.State = PanelState.Loading;
// Load data...
panel.State = PanelState.Normal;
// Or if empty:
panel.State = PanelState.Empty;
panel.EmptyStateMessage = "No items found";
```

## Methods
| Method | Description |
|--------|-------------|
| `Collapse(animate)` | Collapse panel |
| `Expand(animate)` | Expand panel |
