# BeepFontManagement Skill

## Overview
Static font manager for loading, caching, and accessing fonts across the application.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.FontManagement;
```

---

## BeepFontManager (Static)

### Default Font Properties
| Property | Type | Description |
|----------|------|-------------|
| `DefaultFontName` | `string` | Default font family |
| `DefaultFontSize` | `float` | Default size (9.0f) |
| `DefaultFontStyle` | `FontStyle` | Default style |
| `AppFontName` | `string` | Application font |
| `AppFontSize` | `float` | Application font size |

### Pre-built Font Properties
| Property | Usage |
|----------|-------|
| `DefaultFont` | General purpose |
| `ButtonFont` | Button text |
| `LabelFont` | Label text |
| `HeaderFont` | Section headers (+2 size, Bold) |
| `TitleFont` | Titles (+4 size, Bold) |
| `MenuFont` | Menu items |
| `TooltipFont` | Tooltips (-1 size) |
| `StatusBarFont` | Status bar text |
| `DialogFont` | Dialog text |
| `MonospaceFont` | Code (Consolas) |

### Font Collections
- `SystemFonts` - System-installed fonts
- `CustomFonts` - Private/embedded fonts
- `AllFonts` - Combined list
- `FontConfigurations` - Font metadata

---

## Key Methods

### Get Fonts
```csharp
Font font = BeepFontManager.GetFont("Segoe UI", 12f, FontStyle.Bold);
Font cached = BeepFontManager.GetCachedFont("Arial", 10f); // Use in painters
Font elementFont = BeepFontManager.GetFontForElement(UIElementType.Header);
```

### Embedded Fonts
```csharp
Font embedded = BeepFontManager.GetEmbeddedFont("Roboto", 12f, FontStyle.Regular);
bool available = BeepFontManager.IsEmbeddedFontAvailable("Cairo");
var families = BeepFontManager.GetEmbeddedFontFamilies();
var styles = BeepFontManager.GetEmbeddedFontStyles("Roboto");
```

### Management
```csharp
await BeepFontManager.Initialize(); // Load all fonts at startup
BeepFontManager.SetDefaultFont("Inter", 10f);
BeepFontManager.ScaleFonts(1.25f); // Scale all fonts by 125%
bool exists = BeepFontManager.IsFontAvailable("CustomFont");
BeepFontManager.ClearFontCache(); // Call on theme change
BeepFontManager.Cleanup(); // Dispose fonts
```

---

## UIElementType Enum
```csharp
Default, Button, Label, Header, Title, Menu, Tooltip, StatusBar, Dialog, Code
```

---

## Embedded Font Families (26+)
Located in `Fonts/` directory:
- **Sans-Serif**: Inter, Roboto, Open_Sans, Montserrat, Nunito
- **Arabic**: Cairo, Tajawal
- **Display**: Bebas_Neue, Exo_2, Orbitron, Rajdhani
- **Monospace**: Fira_Code, JetBrains_Mono, Cascadia_Code, Space_Mono
- **Accessibility**: Atkinson_Hyperlegible, OpenDyslexic, Lexend

---

## BeepFontPaths (Static)

Provides embedded resource paths for fonts:
```csharp
string path = BeepFontPaths.GetFontPath("Roboto", "Bold");
var families = BeepFontPaths.GetFontFamilyNames();
var styles = BeepFontPaths.GetFontFamilyStyles("Cairo");
```

---

## Usage Examples

### Initialize at Startup
```csharp
// In Program.cs or Form_Load
await BeepFontManager.Initialize();
```

### Use in Custom Painting
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    // Use cached fonts to avoid allocations
    var titleFont = BeepFontManager.GetCachedFont("Segoe UI", 14f, FontStyle.Bold);
    e.Graphics.DrawString("Title", titleFont, Brushes.Black, 10, 10);
}
```

### Get Font from Theme
```csharp
var theme = BeepThemesManager.CurrentTheme;
var font = BeepFontManager.ToFont(theme.TitleStyle);
```

## Related
- `BeepThemesManager` - Theme management
- `FontListHelper` - Low-level font operations
