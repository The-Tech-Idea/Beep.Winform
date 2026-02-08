# BeepBottomNavBar Skill

## Overview
Bottom navigation bar for mobile-style app navigation with multiple styles.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.BottomNavBars;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Items` | `List<BottomBarItem>` | Navigation items |
| `SelectedIndex` | `int` | Selected item index |
| `BottomBarStyle` | `BottomBarStyle` | Visual style |
| `ShowLabels` | `bool` | Display text labels |

## Events
| Event | Description |
|-------|-------------|
| `SelectedItemChanged` | Navigation item selected |

## Usage
```csharp
var navBar = new BottomBar
{
    BottomBarStyle = BottomBarStyle.Material
};
navBar.Items.Add(new BottomBarItem { Text = "Home", ImagePath = "home.svg" });
navBar.Items.Add(new BottomBarItem { Text = "Settings", ImagePath = "settings.svg" });
```
