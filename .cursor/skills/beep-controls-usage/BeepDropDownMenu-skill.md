# BeepDropDownMenu Skill

## Overview
Popup dropdown menu with themed styling and icon support.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Items` | `List<SimpleItem>` | Menu items |
| `ShowIcons` | `bool` | Display item icons |

## Events
| Event | Description |
|-------|-------------|
| `ItemClicked` | Menu item selected |

## Usage
```csharp
var menu = new BeepDropDownMenu();
menu.Items.Add(new SimpleItem { DisplayField = "Copy", ImagePath = "copy.svg" });
menu.Items.Add(new SimpleItem { DisplayField = "Paste", ImagePath = "paste.svg" });
menu.ItemClicked += (s, e) => HandleAction(e.Item);
menu.Show(control, location);
```
