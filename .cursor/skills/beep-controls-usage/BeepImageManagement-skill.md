# BeepImageManagement Skill

## Overview
Image loading utilities supporting multiple sources: files, embedded resources, URLs, Base64, and SVG rendering.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.ImageManagement;
```

---

## ImageLoader (Static)

### From File
```csharp
// Load SVG as SvgDocument
SvgDocument svg = ImageLoader.LoadSvg(@"C:\icons\logo.svg");

// Load regular image (PNG, JPG, BMP)
Image img = ImageLoader.LoadRegularImage(@"C:\images\photo.png");

// Load SVG and render to Bitmap
Image bitmap = ImageLoader.LoadSvgFromFile(@"C:\icons\icon.svg");
```

### From Embedded Resource
```csharp
// Returns (isSvg, object) - object is SvgDocument or Image
var result = ImageLoader.LoadFromEmbeddedResource(
    "MyApp.Resources.logo.png", 
    Assembly.GetExecutingAssembly()
);

if (result.isSvg)
    var svg = (SvgDocument)result.result;
else
    var img = (Image)result.result;

// Direct image load
Image img = ImageLoader.LoadImageFromResource("MyApp.Images.icon.png");
```

### From Stream
```csharp
// Handles SVG or regular images based on extension
Image img = ImageLoader.LoadImageFromStream(stream, ".svg");
```

---

## ImageConfiguration Loading

Load from flexible `ImageConfiguration` object:
```csharp
var config = new ImageConfiguration
{
    IsFile = true,
    Path = @"C:\images",
    FileName = "logo.png"
};
Image img = ImageLoader.LoadImageFromConfig(config);
```

### ImageConfiguration Sources
| Property | Description |
|----------|-------------|
| `IsFile` | Load from file system |
| `IsUrl` | Download from URL |
| `IsBase64` | Decode from Base64 string |
| `IsResxEmbedded` | Load from .resx file |
| `IsMemoryStream` | Load from memory stream |
| `AssemblyFullName` | Load from assembly resource |

### URL Loading
```csharp
var config = new ImageConfiguration
{
    IsUrl = true,
    Path = "https://example.com/image.png"
};
Image img = ImageLoader.LoadImageFromConfig(config);
```

### Base64 Loading
```csharp
var config = new ImageConfiguration
{
    IsBase64 = true,
    Path = "iVBORw0KGgoAAAANS..." // Base64 string
};
Image img = ImageLoader.LoadImageFromConfig(config);
```

---

## ImageConverter Utilities

Convert between image formats:
- `ToBitmap(Image)` - Convert to Bitmap
- `ToIcon(Image)` - Convert to Icon
- `ResizeImage(Image, Size)` - Resize maintaining aspect ratio

---

## ImageListHelper

Manage ImageList collections:
```csharp
// Get/create images for controls
ImageList list = ImageListHelper.GetImageList(images, size);
```

---

## ImageType Enum
```csharp
Png, Jpg, Jpeg, Bmp, Gif, Ico, Icon, Svg, Webp, Tiff, Emf, Wmf, Unknown
```

---

## Usage Examples

### Load Icon for Control
```csharp
// From embedded SVG
var svg = ImageLoader.LoadSvg(Svgs.Settings);
beepButton.Image = svg.Draw(24, 24);

// From file
beepImage.Image = ImageLoader.LoadSvgFromFile(@"C:\icons\save.svg");
```

### Dynamic Image Source
```csharp
public Image LoadDynamicImage(ImageConfiguration config)
{
    return ImageLoader.LoadImageFromConfig(config);
}
```

## Related
- `Svgs` - Embedded SVG icon paths
- `BeepImage` - Image display control
