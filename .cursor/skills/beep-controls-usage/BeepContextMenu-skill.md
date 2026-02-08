# BeepContextMenu Skill

## Overview
`BeepContextMenu` is a themed context menu with hierarchical items, WinForms integration, animations, and click-outside handling.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.ContextMenus;
```

## Key Components
| Class | Description |
|-------|-------------|
| `BeepContextMenu` | The popup menu control |
| `ContextMenuManager` | Global menu management |
| `ContextMenuMetrics` | Size/spacing calculations |

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Items` | Collection | Menu items (SimpleItem) |
| `AutoClose` | `bool` | Close on item click |
| `ShowIcons` | `bool` | Display item icons |
| `ShowKeyboardShortcuts` | `bool` | Show shortcuts |

## Usage Examples

### Basic Context Menu
```csharp
var menu = new BeepContextMenu();

menu.Items.Add(new SimpleItem { Text = "Cut", ImagePath = "cut.svg" });
menu.Items.Add(new SimpleItem { Text = "Copy", ImagePath = "copy.svg" });
menu.Items.Add(new SimpleItem { Text = "Paste", ImagePath = "paste.svg" });

menu.ItemClicked += (s, e) =>
{
    Console.WriteLine($"Clicked: {e.Item.Text}");
};

// Show at mouse position
menu.Show(Control.MousePosition);
```

### With Submenus
```csharp
var fileMenu = new SimpleItem { Text = "File" };
fileMenu.Children.Add(new SimpleItem { Text = "New" });
fileMenu.Children.Add(new SimpleItem { Text = "Open" });
fileMenu.Children.Add(new SimpleItem { Text = "Save" });

menu.Items.Add(fileMenu);
```

### Attach to Control
```csharp
myButton.ContextMenuStrip = menu;
// Or use ContextMenuManager
ContextMenuManager.Attach(myButton, menu);
```

## Features
- Hierarchical submenus
- Icon support (PNG, SVG)
- Keyboard shortcuts display
- Click-outside to close
- Theme integration
- Animations

## Related Controls
- `BeepDropDownMenu` - Dropdown button menu
- `BeepFlyoutMenu` - Flyout panel menu
