# BeepDock Skill

## Overview
Docking panel system for flexible layout management.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Docks;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `DockStyle` | `DockStyle` | Dock position |
| `Title` | `string` | Panel title |
| `AllowFloat` | `bool` | Enable floating mode |
| `AllowClose` | `bool` | Enable close button |
| `IsCollapsed` | `bool` | Panel collapsed state |

## Events
| Event | Description |
|-------|-------------|
| `Docked` | Panel docked |
| `Floating` | Panel floated |
| `Closed` | Panel closed |

## Usage
```csharp
var dockPanel = new BeepDockPanel
{
    Title = "Properties",
    DockStyle = DockStyle.Right,
    AllowFloat = true
};
dockContainer.Add(dockPanel);
```
