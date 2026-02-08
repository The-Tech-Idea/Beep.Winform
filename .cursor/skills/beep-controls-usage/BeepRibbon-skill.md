# BeepRibbon Skill

## Overview
Ribbon bar control with tabs, groups, and buttons (Office-style).

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Ribbon;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Tabs` | `List<RibbonTab>` | Ribbon tabs |
| `SelectedTab` | `RibbonTab` | Active tab |
| `QuickAccessItems` | `List<RibbonItem>` | Quick access toolbar |
| `IsMinimized` | `bool` | Collapsed state |

## Ribbon Structure
```
Ribbon
├── RibbonTab (Home, Insert, View...)
│   └── RibbonGroup (Clipboard, Font...)
│       └── RibbonButton/RibbonCombo/etc.
```

## Usage
```csharp
var ribbon = new BeepRibbon();
var homeTab = new RibbonTab { Text = "Home" };
var clipboardGroup = new RibbonGroup { Text = "Clipboard" };
clipboardGroup.Items.Add(new RibbonButton { Text = "Paste", ImagePath = "paste.svg" });
clipboardGroup.Items.Add(new RibbonButton { Text = "Copy", ImagePath = "copy.svg" });
homeTab.Groups.Add(clipboardGroup);
ribbon.Tabs.Add(homeTab);
```
