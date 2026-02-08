# BeepImage Skill

## Overview
`BeepImage` replaces standard `PictureBox` with SVG support, theme-aware rendering, and enhanced image loading.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Images;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `ImagePath` | `string` | Image file path (PNG, JPG, SVG) |
| `Image` | `Image` | Direct image reference |
| `UseThemeColors` | `bool` | Use theme colors for SVGs |
| `SizeMode` | `ImageSizeMode` | Stretch, Center, Zoom, etc. |

## Usage Examples

### Basic Image
```csharp
var img = new BeepImage
{
    ImagePath = "images/logo.png",
    Size = new Size(100, 100)
};
```

### SVG Image
```csharp
var img = new BeepImage
{
    ImagePath = "GFX/icons/settings.svg",
    UseThemeColors = true
};
```

### Scaled Image
```csharp
var img = new BeepImage
{
    ImagePath = "images/photo.jpg",
    SizeMode = ImageSizeMode.Zoom
};
```

## Events
| Event | Description |
|-------|-------------|
| `ImageLoaded` | Image loaded successfully |
| `ImageError` | Error loading image |
| `Click` | Image clicked |

## Migration from PictureBox
```csharp
// Before
PictureBox pb = new PictureBox { Image = myImage };

// After
BeepImage img = new BeepImage { ImagePath = "path/to/image" };
```

## Related Controls
- `BeepShape` - Geometric shapes
