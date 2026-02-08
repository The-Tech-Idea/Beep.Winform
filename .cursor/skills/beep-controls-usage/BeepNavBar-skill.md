# BeepNavBar Skill

## Overview
`BeepNavBar` is a modern navigation bar with 15+ style painters, horizontal/vertical orientation, and selection tracking.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.NavBars;
using TheTechIdea.Beep.Winform.Controls.Models;
```

## Painter Styles (15+)
- **Material**: Material3, MaterialYou
- **Fluent**: Fluent2, Windows11Mica
- **Apple**: MacOSBigSur, iOS15
- **Modern**: Discord, Stripe, Vercel, Notion, Tailwind
- **Effects**: DarkGlow, GradientModern, ChakraUI, AntDesign

## NavBarOrientation
```csharp
public enum NavBarOrientation
{
    Horizontal = 0,  // Top/bottom bar
    Vertical = 1     // Side bar
}
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Items` | `BindingList<SimpleItem>` | Nav items |
| `SelectedItem` | `SimpleItem` | Selected item |
| `Orientation` | `NavBarOrientation` | Horizontal/Vertical |
| `ItemWidth` | `int` | Item width (80) |
| `ItemHeight` | `int` | Item height (48) |
| `AccentColor` | `Color` | Accent color |
| `UseThemeColors` | `bool` | Use theme colors |
| `EnableShadow` | `bool` | Shadow effects |
| `CornerRadius` | `int` | Corner radius (8) |

## Usage Examples

### Horizontal NavBar (Top)
```csharp
var navbar = new BeepNavBar
{
    Orientation = NavBarOrientation.Horizontal,
    Dock = DockStyle.Top,
    ItemHeight = 50
};

navbar.Items.Add(new SimpleItem { Text = "Home", ImagePath = "home.svg" });
navbar.Items.Add(new SimpleItem { Text = "Products", ImagePath = "products.svg" });
navbar.Items.Add(new SimpleItem { Text = "About", ImagePath = "about.svg" });

navbar.ItemClicked += item => Console.WriteLine($"Clicked: {item.Text}");
```

### Bottom NavBar
```csharp
var navbar = new BeepNavBar
{
    Orientation = NavBarOrientation.Horizontal,
    Dock = DockStyle.Bottom,
    ItemHeight = 60
};
```

### Vertical NavBar
```csharp
var navbar = new BeepNavBar
{
    Orientation = NavBarOrientation.Vertical,
    Dock = DockStyle.Left,
    ItemWidth = 100
};
```

## Events
| Event | Description |
|-------|-------------|
| `ItemClicked` | Navigation item clicked |
| `PropertyChanged` | Property value changed |

## Related Controls
- `BeepSideBar` - Collapsible sidebar
- `BeepBottomNavBar` - Specialized bottom bar
