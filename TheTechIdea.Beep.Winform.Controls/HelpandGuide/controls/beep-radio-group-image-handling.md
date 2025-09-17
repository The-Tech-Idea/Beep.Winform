# Beep Radio Group Image Handling Documentation

## Overview

All Beep Radio Group renderers now support comprehensive image handling through the `SimpleItem.ImagePath` property, similar to how BeepListBox handles images. This document explains the implementation details and usage patterns.

## Key Features

### ? **MaxImageSize Property**
- **Property**: `MaxImageSize` with default value of `24x24` pixels
- **Type**: `Size` (Width, Height)
- **Range**: Minimum 16px, Maximum 64px (recommended)
- **Inheritance**: Available on both `BeepRadioGroupAdvanced` and `BeepHierarchicalRadioGroup`

### ? **IImageAwareRenderer Interface**
- **Purpose**: Standardizes image handling across all renderers
- **Implementation**: All renderers now implement this interface
- **Properties**: 
  - `MaxImageSize { get; set; }` - Controls the maximum size of rendered images

### ? **Automatic Size Calculation**
- **Dynamic sizing**: Item height automatically adjusts to accommodate images
- **Layout calculation**: Text and image spacing is properly calculated
- **Minimum heights**: Each renderer ensures minimum heights accommodate both text and images

## Renderer-Specific Implementation

### 1. **FlatRadioRenderer**
```csharp
// Image size calculated from MaxImageSize
private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);

// Size measurement includes image
public Size MeasureItem(SimpleItem item, Graphics graphics)
{
    // Account for image if present
    if (!string.IsNullOrEmpty(item.ImagePath))
    {
        width += IconSize + ComponentSpacing;
        height = Math.Max(height, IconSize + ItemPadding);
    }
}
```

### 2. **CircularRadioRenderer**
```csharp
// Traditional radio with 24px default images
private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);

// Proper spacing between radio button, image, and text
private void DrawContent(Graphics graphics, SimpleItem item, ...)
{
    int currentX = radioArea.Right + ComponentSpacing;
    
    if (!string.IsNullOrEmpty(item.ImagePath))
    {
        // Draw image with proper positioning
        var iconRect = new Rectangle(currentX, ..., IconSize, IconSize);
        _imageRenderer.ImagePath = item.ImagePath;
        _imageRenderer.Draw(graphics, iconRect);
        currentX += IconSize + ComponentSpacing;
    }
}
```

### 3. **MaterialRadioRenderer**
```csharp
// Material Design 3 compliant image handling
// Uses state layer size for proper material alignment
private const int StateLayerSize = 40;
private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
```

### 4. **CardRadioRenderer**
```csharp
// Card-style with elevated appearance
// Images integrated into card layout with proper padding
private const int ItemPadding = 16;
private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
```

### 5. **ChipRadioRenderer**
```csharp
// Pill/chip style with compact layout
// Images positioned at the start of the chip content
private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
```

## Usage Examples

### Basic Usage
```csharp
var radioGroup = new BeepHierarchicalRadioGroup
{
    MaxImageSize = new Size(32, 32), // Custom image size
    RenderStyle = RadioGroupRenderStyle.Flat
};

var item = new SimpleItem
{
    Text = "Security Settings",
    SubText = "Configure security options",
    ImagePath = "shield.svg" // Will be rendered at 32x32
};
```

### Image Path Types Supported
```csharp
// Local file paths
item.ImagePath = @"C:\Icons\security.png";

// Relative paths
item.ImagePath = @"Icons\shield.svg";

// Embedded resources (via BeepImage)
item.ImagePath = "shield"; // Looks in embedded resources

// Icon font references
item.ImagePath = "fa-shield"; // Font Awesome style
```

### Dynamic Image Size Control
```csharp
// Runtime image size adjustment
private void OnImageSizeChanged(object sender, EventArgs e)
{
    var newSize = new Size(24, 24);
    radioGroup.MaxImageSize = newSize; // Automatically updates all renderers
}
```

## Implementation Details

### Image Rendering Flow
1. **Property Assignment**: `item.ImagePath` contains the image reference
2. **Size Calculation**: `IconSize` property calculates actual render size from `MaxImageSize`
3. **Layout Measurement**: `MeasureItem()` accounts for image dimensions in total item size
4. **Rendering**: `BeepImage` component handles actual image loading and drawing
5. **Positioning**: Images positioned between selector and text with proper spacing

### BeepImage Integration
```csharp
// All renderers use consistent BeepImage usage
_imageRenderer.ImagePath = item.ImagePath;
_imageRenderer.Draw(graphics, iconRect);
```

### Size Constraints
- **Minimum**: 16x16 pixels (readable at small sizes)
- **Default**: 24x24 pixels (optimal for most use cases)
- **Maximum**: 64x64 pixels (recommended maximum for performance)
- **Square**: Width and height are kept proportional using `Math.Min(width, height)`

## Theme Integration

### Theme-Aware Images
- **SVG Support**: SVG images can adapt to theme colors when supported by BeepImage
- **Color Adaptation**: Theme changes automatically update image rendering context
- **Consistent Styling**: Images maintain visual consistency with the selected theme

### Font Integration
- **Size Relationship**: Image sizes work harmoniously with theme font sizes
- **Spacing**: Component spacing follows theme guidelines
- **Alignment**: Images align properly with text baselines

## Performance Considerations

### Efficient Rendering
- **Cached Images**: BeepImage handles image caching internally
- **Size Optimization**: MaxImageSize prevents oversized images from impacting performance
- **Layout Calculation**: Image dimensions are calculated once during layout

### Memory Management
- **Shared Instances**: Single BeepImage instance per renderer
- **Proper Disposal**: Images are properly disposed when renderers are disposed
- **Resource Management**: Embedded resources are efficiently managed

## Comparison with BeepListBox

### Similarities
- **ImagePath Property**: Uses same `SimpleItem.ImagePath` property
- **BeepImage Renderer**: Uses same underlying image rendering component
- **Size Control**: Similar MaxImageSize concept for consistent sizing
- **Theme Integration**: Same level of theme awareness

### Differences
- **Layout Context**: Radio groups have selector elements (radio buttons/checkboxes)
- **Spacing**: Different component spacing requirements
- **Size Constraints**: Different default and recommended sizes
- **Multiple Renderers**: Radio groups support multiple visual styles

## Best Practices

### Image Selection
- **Consistent Style**: Use consistent icon style across all items (outlined, filled, etc.)
- **Appropriate Size**: Choose source images that scale well to target sizes
- **Format Choice**: Prefer SVG for scalable icons, PNG for detailed images
- **Theme Compatibility**: Consider how images work with both light and dark themes

### Performance Optimization
- **Reasonable Sizes**: Keep MaxImageSize reasonable (16-32px for most cases)
- **Image Caching**: Reuse image paths when possible for better caching
- **Format Efficiency**: Use appropriate image formats for content type

### User Experience
- **Meaningful Icons**: Use icons that clearly represent the option
- **Accessibility**: Ensure images supplement, don't replace, descriptive text
- **Consistency**: Maintain consistent image sizes across related controls
- **Loading States**: Handle missing or loading images gracefully

This comprehensive image handling system ensures that all radio group renderers provide consistent, high-quality image support while maintaining the flexibility and performance characteristics expected from the Beep control suite.