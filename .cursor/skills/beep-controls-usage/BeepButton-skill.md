# BeepButton Skill

## Overview
`BeepButton` is a Material Design button with splash effects, loading states, popup support, and extensive customization.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Buttons;
```

## Key Properties

### Core Properties
| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string` | Button text |
| `ImagePath` | `string` | Icon path (SVG/PNG) |
| `ButtonType` | `ButtonType` | Visual type |
| `HideText` | `bool` | Hide text (icon-only) |
| `MaxImageSize` | `Size` | Max icon dimensions |

### Layout Properties
| Property | Type | Description |
|----------|------|-------------|
| `TextAlign` | `ContentAlignment` | Text alignment |
| `ImageAlign` | `ContentAlignment` | Image alignment |
| `TextImageRelation` | `TextImageRelation` | Text/image layout |
| `AutoSizeContent` | `bool` | Auto-resize to content |
| `ButtonMinSize` | `Size` | Minimum button size |

### Appearance Properties
| Property | Type | Description |
|----------|------|-------------|
| `BorderRadius` | `int` | Corner radius (default: 8) |
| `BorderSize` | `int` | Border thickness |
| `IsColorFromTheme` | `bool` | Use theme colors |
| `ApplyThemeOnImage` | `bool` | Theme-tint SVG icons |

### Splash Animation Properties
| Property | Type | Description |
|----------|------|-------------|
| `SplashColor` | `Color` | Ripple effect color |
| `SplashSpeed` | `float` | Animation speed (0.01-0.2) |
| `MaxSplashRadius` | `float` | Max ripple radius |
| `SplashColorOpacity` | `float` | Ripple opacity (0-255) |
| `SplashEasingFunction` | `SplashEasingFunction` | Linear, EaseOut, EaseIn, EaseInOut |

### Behavior Properties
| Property | Type | Description |
|----------|------|-------------|
| `IsLoading` | `bool` | Show loading spinner |
| `IsStillButton` | `bool` | Hover persistence |
| `IsChild` | `bool` | Child button styling |

### Popup Mode Properties
| Property | Type | Description |
|----------|------|-------------|
| `PopupMode` | `bool` | Enable dropdown popup |
| `ListItems` | `BindingList<SimpleItem>` | Popup menu items |
| `SelectedItem` | `SimpleItem` | Selected menu item |
| `PopPosition` | `BeepPopupFormPosition` | Popup position |
| `IsPopupOpen` | `bool` | Popup open state |

### Material Design Properties
| Property | Type | Description |
|----------|------|-------------|
| `ButtonLabel` | `string` | Floating label text |
| `ButtonHelperText` | `string` | Helper text below |
| `ButtonErrorText` | `string` | Error message |
| `ButtonHasError` | `bool` | Error state |

## Usage Examples

### Basic Button
```csharp
var btn = new BeepButton
{
    Text = "Save",
    UseThemeColors = true,
    BorderRadius = 8
};
btn.Click += (s, e) => SaveData();
```

### Button with Icon
```csharp
var btn = new BeepButton
{
    Text = "Download",
    ImagePath = "GFX/icons/download.svg",
    TextImageRelation = TextImageRelation.ImageBeforeText,
    ApplyThemeOnImage = true
};
```

### Icon-Only Button
```csharp
var btn = new BeepButton
{
    ImagePath = "GFX/icons/settings.svg",
    HideText = true,
    ButtonMinSize = new Size(40, 40)
};
```

### Loading State
```csharp
btn.IsLoading = true;  // Show spinner
await ProcessAsync();
btn.IsLoading = false; // Hide spinner
```

### Dropdown Button
```csharp
var btn = new BeepButton
{
    Text = "Options",
    PopupMode = true
};
btn.ListItems.Add(new SimpleItem { Text = "Copy", ImagePath = "copy.svg" });
btn.ListItems.Add(new SimpleItem { Text = "Paste", ImagePath = "paste.svg" });
btn.SelectedItemChanged += (s, e) => HandleAction(e.SelectedItem);
```

### Long Press Handler
```csharp
btn.LongPress += (s, e) => ShowContextMenu();
btn.DoubleClickAction += (s, e) => EditItem();
```

## Events
| Event | Description |
|-------|-------------|
| `Click` | Button clicked |
| `LongPress` | Button held for 500ms |
| `DoubleClickAction` | Double-click detected |
| `SelectedItemChanged` | Popup item selected |
| `ImageClicked` | Image area clicked |

## Related Controls
- `BeepChevronButton` - Navigation arrows
- `BeepCircularButton` - FAB-style circular
- `BeepExtendedButton` - Extended features
- `BeepButtonPopList` - Button with popup list
