# BeepiFormPro Painters

This folder contains painter classes for customizing the visual appearance of `BeepiFormPro` forms.

## Overview

The painter pattern allows complete separation of rendering logic from form behavior. Each painter implements the `IFormPainter` interface and provides distinct visual styles for:
- Form background
- Caption bar
- Window borders

## Architecture

### IFormPainter Interface

The core interface that all form painters must implement:

```csharp
public interface IFormPainter
{
    void PaintBackground(Graphics g, BeepiFormPro owner);
    void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect);
    void PaintBorders(Graphics g, BeepiFormPro owner);
}
```

### Available Painters

#### 1. MinimalFormPainter
- **Style**: Clean, minimal design
- **Background**: Solid color using default style
- **Caption**: Simple 2px underline with primary color
- **Borders**: 1px solid border
- **Use Case**: Simple, distraction-free interfaces

#### 2. MaterialFormPainter
- **Style**: Material Design 3
- **Background**: Solid Material3 background
- **Caption**: 4px vertical accent bar on left edge
- **Borders**: 1px solid border
- **Use Case**: Modern Material Design applications

#### 3. GlassFormPainter
- **Style**: Glass effect with transparency
- **Background**: Semi-transparent (240 alpha) with gradient overlay
- **Caption**: Gradient bar (160-120 alpha) with highlight line
- **Borders**: Double border (2px primary + 1px inner highlight)
- **Use Case**: Modern, visually rich applications with depth

## Usage

### Setting a Painter

```csharp
var form = new BeepiFormPro();

// Use minimal painter
form.ActivePainter = new MinimalFormPainter();

// Use Material Design painter
form.ActivePainter = new MaterialFormPainter();

// Use glass effect painter
form.ActivePainter = new GlassFormPainter();
```

### Multiple Painters

You can register multiple painters and switch between them:

```csharp
form.Painters.Add(new MinimalFormPainter());
form.Painters.Add(new MaterialFormPainter());
form.Painters.Add(new GlassFormPainter());

// Switch painters dynamically
form.ActivePainter = form.Painters[1]; // Material
form.Invalidate(); // Trigger repaint
```

## Creating Custom Painters

To create a custom painter:

1. Create a new class in this folder
2. Implement `IFormPainter` interface
3. Use XML documentation for each method
4. Follow the established naming pattern: `[Style]FormPainter`

### Example Custom Painter

```csharp
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Custom form painter with gradient background.
    /// </summary>
    internal sealed class GradientFormPainter : IFormPainter
    {
        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var bg = StyleColors.GetBackground(style);
            var primary = StyleColors.GetPrimary(style);
            
            using var brush = new LinearGradientBrush(
                owner.ClientRectangle,
                bg,
                Color.FromArgb(255, 
                    Math.Min(bg.R + 20, 255),
                    Math.Min(bg.G + 20, 255),
                    Math.Min(bg.B + 20, 255)),
                90f);
            g.FillRectangle(brush, owner.ClientRectangle);
        }
        
        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            // Your caption painting logic
        }
        
        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            // Your border painting logic
        }
    }
}
```

## Design Guidelines

### Background Painting
- Always use `owner.ClientRectangle` for full form area
- Access colors via `StyleColors` static methods
- Dispose of brushes and pens properly (use `using` statements)
- Consider performance - avoid creating objects in paint loops

### Caption Painting
- Use provided `captionRect` parameter
- Inflate rectangles for text padding: `captionRect.Inflate(-8, 0)`
- Use `TextRenderer.DrawText` for text rendering with proper flags
- Caption height is managed by layout manager (typically 32-40px)

### Border Painting
- Remember to adjust rectangle size: `r.Width -= 1; r.Height -= 1`
- Use `Inflate()` for inset/outset borders
- Draw from outer to inner for layered borders
- Standard border width: 1-2px

### Graphics Quality
```csharp
// Set anti-aliasing for smooth rendering
g.SmoothingMode = SmoothingMode.AntiAlias;
g.PixelOffsetMode = PixelOffsetMode.HighQuality;
```

### Color Access
```csharp
// Always get colors from StyleColors
var bg = StyleColors.GetBackground(style);
var fg = StyleColors.GetForeground(style);
var primary = StyleColors.GetPrimary(style);
var surface = StyleColors.GetSurface(style);
var border = StyleColors.GetBorder(style);
```

## Integration with BeepiFormPro

The form automatically calls painter methods during OnPaint:

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    // ... layout and hit testing ...
    
    ActivePainter?.PaintBackground(e.Graphics, this);
    ActivePainter?.PaintCaption(e.Graphics, this, _layout.CaptionRect);
    ActivePainter?.PaintBorders(e.Graphics, this);
    
    // ... region painting ...
}
```

## Style Coordination

Painters work with:
- **BeepStyling**: Global style management
- **StyleColors**: Color palette access
- **FormStyle enum**: High-level form appearance modes
- **BeepControlStyle enum**: Detailed control styling

## Performance Considerations

1. **Object Reuse**: Consider caching brushes/pens for frequently used painters
2. **Conditional Rendering**: Use `owner.FormStyle` to skip unnecessary painting
3. **Graphics State**: Save and restore graphics state if modifying transforms
4. **Double Buffering**: Form uses `OptimizedDoubleBuffer` - don't duplicate

## Testing

When creating new painters:
1. Test with different form sizes and DPI scales
2. Verify all FormStyle modes work correctly
3. Check caption bar with and without system buttons
4. Test maximize/restore state appearance
5. Verify proper color application in light/dark themes

## Future Enhancements

Potential features for future painters:
- Shadow effects (drop shadows, inner shadows)
- Blur effects (backdrop blur, motion blur)
- Animation support (transitions between states)
- Theme-aware painters (auto-switch light/dark)
- Platform-specific painters (Windows 11, macOS style)

## Related Files

- `BeepiFormPro.Core.cs` - Core form properties and painter management
- `BeepiFormPro.Drawing.cs` - Main OnPaint method that invokes painters
- `BeepiFormPro.Managers.cs` - Layout manager that calculates caption rect
- `../Helpers/` - Layout and hit testing helper classes
