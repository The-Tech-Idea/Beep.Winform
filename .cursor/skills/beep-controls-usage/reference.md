# Beep Controls Usage - Quick Reference

## Control Mapping

| Standard Control | Beep Control | Key Property |
|-----------------|--------------|--------------|
| `Label` | `BeepLabel` | `SubHeaderText`, `ImagePath` |
| `TextBox` | `BeepTextBox` | `PlaceholderText`, `ShowFloatingLabel` |
| `Button` | `BeepButton` | `IconPath`, `Variant`, `EnableRipple` |
| `CheckBox` | `BeepCheckBox<T>` | `Style`, `UseThemeColors` |
| `ComboBox` | `BeepComboBox` | `PlaceholderText`, `Style` |
| `Panel` | `BeepCard` | `Elevation`, `CornerRadius` |
| `DataGridView` | `BeepSimpleGrid` | `EnableVirtualScrolling` |
| `PictureBox` | `BeepImage` | `ImagePath` (SVG) |

## Essential Properties

```csharp
// Theme (all controls)
UseThemeColors = true;
CurrentTheme = BeepThemesManager.Instance.GetTheme("Material3");

// Style (all controls)
Style = ControlStyle.Material3; // or iOS15, Fluent2, etc.

// Common
IconPath = "path/to/icon.svg";  // SVG preferred
CornerRadius = 8;                // Rounded corners
```

## Quick Migration Pattern

```csharp
// 1. Replace control type
BeepButton btn = new BeepButton(); // Instead of Button

// 2. Set theme properties
btn.UseThemeColors = true;
btn.CurrentTheme = theme;

// 3. Set style
btn.Style = BeepButtonStyle.Material3;

// 4. Use Beep-specific features
btn.IconPath = "icons/save.svg";
btn.EnableRipple = true;
```

## Namespaces

```csharp
using TheTechIdea.Beep.Winform.Controls;              // Core controls
using TheTechIdea.Beep.Winform.Controls.Buttons;      // Buttons
using TheTechIdea.Beep.Winform.Controls.TextFields; // TextBox
using TheTechIdea.Beep.Winform.Controls.Cards;       // Cards/Panels
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;  // ComboBox
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;  // CheckBox
```

## Theme Setup

```csharp
// Form constructor
this.CurrentTheme = BeepThemesManager.Instance.GetTheme("Material3Dark");
ApplyThemeToControls(this);
```

## Common Styles

- `Material3` - Google Material Design 3
- `iOS15` - Apple iOS 15 style
- `Fluent2` - Microsoft Fluent Design 2
- `AntDesign` - Ant Design style
- `MaterialYou` - Material You (Android 12+)
- `Windows11Mica` - Windows 11 Mica effect
- `MacOSBigSur` - macOS Big Sur style
- `ChakraUI` - Chakra UI style
- `TailwindCard` - Tailwind CSS card style
- `NotionMinimal` - Notion minimal style
- `Minimal` - Minimal design
- `VercelClean` - Vercel clean style
- `StripeDashboard` - Stripe dashboard style
- `DarkGlow` - Dark mode with glow
- `DiscordStyle` - Discord-style UI
- `GradientModern` - Modern gradient style
