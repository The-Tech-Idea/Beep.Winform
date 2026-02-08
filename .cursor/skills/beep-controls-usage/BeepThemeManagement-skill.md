# BeepThemeManagement Skill

## Overview
Static manager for themes, form styles, and global appearance across all Beep controls.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
```

---

## BeepThemesManager (Static)

### Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `CurrentThemeName` | `string` | Active theme name |
| `CurrentTheme` | `IBeepTheme` | Active theme object |
| `CurrentStyle` | `FormStyle` | Active form style |

### Key Methods
| Method | Description |
|--------|-------------|
| `GetTheme(name)` | Get theme by name |
| `GetThemeNames()` | List all theme names |
| `GetThemes()` | List all IBeepTheme objects |
| `SetCurrentTheme(name/theme)` | Change active theme |
| `SetCurrentStyle(style)` | Change form style |
| `ThemeExists(name)` | Check if theme exists |
| `AddTheme(theme)` | Add custom theme |
| `RemoveTheme(theme)` | Remove custom theme |
| `SaveTheme(theme, path)` | Save to XML file |
| `LoadTheme(path)` | Load from XML file |
| `ToFont(TypographyStyle)` | Create Font from style |

### Events
- `ThemeChanged` - Fires when theme changes
- `FormStyleChanged` - Fires when form style changes

---

## FormStyle Enum (32 Styles)
```csharp
Modern, Minimal, MacOS, Fluent, Material, Cartoon, ChatBubble, Glass,
Metro, Metro2, GNOME, NeoMorphism, Glassmorphism, Brutalist, Retro,
Cyberpunk, Nordic, iOS, Ubuntu, KDE, ArcLinux, Dracula, Solarized,
OneDark, GruvBox, Nord, Tokyo, Paper, Neon, Holographic, Terminal, Custom
```

---

## Built-in Themes (50+)
`DefaultTheme`, `DarkTheme`, `LightTheme`, `OceanTheme`, `ForestTheme`, `NeonTheme`, `MaterialDesignTheme`, `NeumorphismTheme`, `GlassmorphismTheme`, `FlatDesignTheme`, `CyberpunkNeonTheme`, `DraculaTheme`, `NordTheme`, `TokyoTheme`, `FluentTheme`, `iOSTheme`, `MacOSTheme`, `UbuntuTheme`, `KDETheme`, and many more.

---

## Usage Examples

### Set Theme by Name
```csharp
BeepThemesManager.SetCurrentTheme("DarkTheme");
```

### Set Theme by Object
```csharp
var theme = new MyCustomTheme();
BeepThemesManager.SetCurrentTheme(theme);
```

### Listen for Theme Changes
```csharp
BeepThemesManager.ThemeChanged += (s, e) => {
    var oldTheme = e.OldThemeName;
    var newTheme = e.NewThemeName;
    // Update UI
};
```

### Set Form Style
```csharp
BeepThemesManager.SetCurrentStyle(FormStyle.Fluent);
// Automatically selects matching theme
```

### Get Available Themes
```csharp
var themes = BeepThemesManager.GetThemeNames();
foreach (var name in themes)
    comboBox.Items.Add(name);
```

---

## IBeepTheme Interface (Key Properties)
- `ThemeName`, `ThemeGuid`, `IsDarkTheme`
- `BackColor`, `ForeColor`, `AccentColor`, `PrimaryColor`, `SecondaryColor`
- `BorderColor`, `BorderRadius`
- `FontFamily`, Typography styles
- `ButtonBackColor/ForeColor/HoverColors`
- `CardBackColor`, `PanelBackColor`, `SurfaceColor`
- `SuccessColor`, `ErrorColor`, `WarningColor`, `InfoColor`

## Related
- `BeepFontManager` - Font management
- `BeepTheme` - Base theme class
