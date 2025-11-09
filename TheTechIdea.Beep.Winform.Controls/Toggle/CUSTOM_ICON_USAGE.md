# BeepToggle Custom Icon Usage Guide

## Overview
The `IconCustom` toggle style allows developers to specify any two SVG images for the ON and OFF states using the `OnIconPath` and `OffIconPath` properties.

## Basic Usage

### Example 1: Using SvgsUI Static Properties
```csharp
// Create toggle with custom airplane icons
var toggle = new BeepToggle
{
    ToggleStyle = ToggleStyle.IconCustom,
    OnIconPath = SvgsUI.Navigation,     // Airplane icon when ON
    OffIconPath = SvgsUI.Navigation2,   // Different airplane icon when OFF
    OnColor = Color.Blue,
    OffColor = Color.Gray,
    Size = new Size(60, 30)
};
```

### Example 2: Using File Paths
```csharp
// Create toggle with custom SVG files
var toggle = new BeepToggle
{
    ToggleStyle = ToggleStyle.IconCustom,
    OnIconPath = @"C:\Icons\play.svg",
    OffIconPath = @"C:\Icons\pause.svg",
    OnColor = Color.Green,
    OffColor = Color.Red
};
```

### Example 3: Using Embedded Resources
```csharp
// Create toggle with embedded resource SVGs
var toggle = new BeepToggle
{
    ToggleStyle = ToggleStyle.IconCustom,
    OnIconPath = "MyApp.Resources.Icons.sun.svg",
    OffIconPath = "MyApp.Resources.Icons.moon.svg",
    OnColor = Color.Orange,
    OffColor = Color.DarkBlue
};
```

## Common Use Cases

### Play/Pause Control
```csharp
var mediaToggle = new BeepToggle
{
    ToggleStyle = ToggleStyle.IconCustom,
    OnIconPath = SvgsUI.Pause,
    OffIconPath = SvgsUI.Play,
    OnColor = Color.FromArgb(244, 67, 54),  // Red when playing
    OffColor = Color.FromArgb(76, 175, 80)  // Green when paused
};
```

### Dark Mode Toggle
```csharp
var themeToggle = new BeepToggle
{
    ToggleStyle = ToggleStyle.IconCustom,
    OnIconPath = SvgsUI.Moon,   // Dark mode ON (assuming Moon icon exists)
    OffIconPath = SvgsUI.Sun,   // Light mode OFF (assuming Sun icon exists)
    OnColor = Color.FromArgb(33, 33, 33),
    OffColor = Color.FromArgb(255, 193, 7)
};
```

### WiFi Toggle
```csharp
var wifiToggle = new BeepToggle
{
    ToggleStyle = ToggleStyle.IconCustom,
    OnIconPath = SvgsUI.Wifi,       // WiFi on (assuming Wifi icon exists)
    OffIconPath = SvgsUI.WifiOff,   // WiFi off (assuming WifiOff icon exists)
    OnColor = Color.Green,
    OffColor = Color.Gray
};
```

### Bookmark Toggle
```csharp
var bookmarkToggle = new BeepToggle
{
    ToggleStyle = ToggleStyle.IconCustom,
    OnIconPath = SvgsUI.Bookmark,   // Filled bookmark
    OffIconPath = SvgsUI.Bookmark,  // Same icon but different opacity/color
    OnColor = Color.FromArgb(255, 193, 7),  // Gold when bookmarked
    OffColor = Color.FromArgb(189, 189, 189) // Gray when not bookmarked
};
```

## Available SvgsUI Icons (Examples)

### Navigation & Actions
- `SvgsUI.Play`, `SvgsUI.Pause`, `SvgsUI.FastForward`, `SvgsUI.Rewind`
- `SvgsUI.SkipBack`, `SvgsUI.SkipForward`
- `SvgsUI.RefreshCw`, `SvgsUI.RefreshCcw`

### Alerts & Indicators
- `SvgsUI.Bell`, `SvgsUI.BellOff`
- `SvgsUI.AlertCircle`, `SvgsUI.AlertTriangle`
- `SvgsUI.CheckCircle`, `SvgsUI.XCircle`

### Communication
- `SvgsUI.Mail`, `SvgsUI.MessageCircle`, `SvgsUI.MessageSquare`
- `SvgsUI.Phone`, `SvgsUI.PhoneCall`, `SvgsUI.PhoneMissed`

### Files & Folders
- `SvgsUI.File`, `SvgsUI.FileText`, `SvgsUI.FilePlus`
- `SvgsUI.Folder`, `SvgsUI.FolderPlus`, `SvgsUI.FolderMinus`

### Media
- `SvgsUI.Image`, `SvgsUI.Film`, `SvgsUI.Camera`
- `SvgsUI.Volume`, `SvgsUI.VolumeX`, `SvgsUI.Volume1`, `SvgsUI.Volume2`

### Editing
- `SvgsUI.Edit`, `SvgsUI.Edit2`, `SvgsUI.Edit3`
- `SvgsUI.Copy`, `SvgsUI.Trash`, `SvgsUI.Trash2`

### Security
- `SvgsUI.Lock`, `SvgsUI.Unlock`, `SvgsUI.Shield`, `SvgsUI.Key`

## Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `ToggleStyle` | `ToggleStyle` | Must be set to `ToggleStyle.IconCustom` | - |
| `OnIconPath` | `string` | SVG path for ON state (SvgsUI property, file path, or resource) | Empty (fallback to Check icon) |
| `OffIconPath` | `string` | SVG path for OFF state (SvgsUI property, file path, or resource) | Empty (fallback to X icon) |
| `OnColor` | `Color` | Color tint for icon when ON | Green |
| `OffColor` | `Color` | Color tint for icon when OFF | Gray |

## Features

✅ **Automatic SVG Tinting**: Icons are automatically colored based on `OnColor` and `OffColor`
✅ **Multiple Path Types**: Supports SvgsUI properties, file paths, and embedded resources
✅ **Fallback Icons**: Uses Check/X icons if paths are not set
✅ **Caching**: Uses `StyledImagePainter` with built-in caching for performance
✅ **Disabled State**: Automatically shows gray icons when disabled
✅ **Smooth Animation**: Inherits all toggle animation features

## Notes

1. **SVG Files Only**: Only SVG images are supported (not PNG, JPG, etc.)
2. **Icon Sizing**: Icons are automatically sized to fit within the thumb (60% of thumb size)
3. **Color Override**: Icon colors are overridden by `OnColor` and `OffColor` properties
4. **Performance**: Icons are cached by `StyledImagePainter` for optimal performance

## Explore Available Icons

To see all available SvgsUI icons, check:
```
C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\IconsManagement\SvgsUI.cs
```

Or use IntelliSense by typing `SvgsUI.` in your code editor.
