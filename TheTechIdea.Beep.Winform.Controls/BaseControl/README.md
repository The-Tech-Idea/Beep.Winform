# BaseControl Architecture and Painting Pipeline

This document explains how `BaseControl` renders and how to extend it safely. It summarizes the painter-first design for consistent rendering across different visual styles.

## Overview

BaseControl uses a **Painter Strategy Pattern** to handle all visual rendering. This architecture separates outer styling (borders, shadows, backgrounds) from inner content, allowing for maximum flexibility and consistency across different visual styles.

## Structure

- **`BaseControl`** (partial): Core behavior, theming, events, drawing entry points
- **`Painters/`**: Strategy pattern implementations for different visual styles
- **`Helpers/`**: Supporting classes for effects, hit testing, DPI scaling, etc.

### Core Files
```
BaseControl/
??? BaseControl.cs                    # Main control class
??? BaseControl.Events.cs             # Event handling and routing
??? BaseControl.Methods.cs            # Core methods and painter management
??? BaseControl.Properties.cs         # Properties and painter selection
??? BaseControl.Material.cs           # Material Design integration
??? README.md                         # This documentation
??? Helpers/
    ??? Painters/
    ?   ??? IBaseControlPainter.cs     # Painter interface
    ?   ??? ClassicBaseControlPainter.cs
    ?   ??? MaterialBaseControlPainter.cs
    ?   ??? CardBaseControlPainter.cs
    ?   ??? NeoBrutalistBaseControlPainter.cs
    ?   ??? ReadingCardBaseControlPainter.cs
    ?   ??? ButtonBaseControlPainter.cs
    ?   ??? ShortcutCardBaseControlPainter.cs
    ?   ??? GlassmorphismBaseControlPainter.cs
    ?   ??? NeumorphismBaseControlPainter.cs
    ?   ??? MinimalistBaseControlPainter.cs
    ??? ControlEffectHelper.cs         # Effects and ripple animations
    ??? ControlHitTestHelper.cs        # Hit area management
    ??? ControlExternalDrawingHelper.cs # Overlays and badges
    ??? ControlDpiHelper.cs            # DPI scaling support
    ??? ControlDataBindingHelper.cs    # Data binding logic
```

## Render Flow

The rendering pipeline follows this sequence:

```
OnPaint ? DrawContent(Graphics g) ? Painter Strategy
                ?
1. EnsurePainter()              # Select appropriate painter
2. _painter.UpdateLayout()      # Calculate layout rectangles  
3. _painter.Paint()             # Render outer styling
4. _painter.UpdateHitAreas()    # Register interactive areas
5. DrawingRect available        # For inheriting controls
```

## Painter Selection

Use the `PainterKind` property to select visual styles:

```csharp
public enum BaseControlPainterKind
{
    Auto,           // Defaults to Classic
    Classic,        // Traditional WinForms
    Material,       // Material Design 3
    Card,           // Clean card design
    NeoBrutalist,   // Bold, high-contrast
    ReadingCard,    // Content-focused card
    Button,         // Button styling
    ShortcutCard,   // Keyboard shortcut card
    Glassmorphism,  // Modern glass effects
    Neumorphism,    // Soft 3D effects
    Minimalist      // Clean web design
}
```

## Available Painters

### Traditional Styles

#### 1. Classic Painter
- **Style**: Traditional WinForms appearance
- **Features**: Configurable borders, shadows, gradients, state colors
- **Theme Integration**: Full theme support with fallbacks
- **Best For**: Legacy applications, backward compatibility
- **Layout**: Respects all border/shadow properties

```csharp
baseControl.PainterKind = BaseControlPainterKind.Classic;
baseControl.ShowAllBorders = true;
baseControl.BorderThickness = 2;
baseControl.ShowShadow = true;
```

#### 2. Material Painter  
- **Style**: Material Design 3 specifications
- **Features**: Floating labels, elevation shadows, variants (Outlined/Filled/Standard)
- **Theme Integration**: Material-specific theme colors with focus indicators
- **Best For**: Modern applications following Material Design guidelines
- **Layout**: Material Design spacing and elevation

```csharp
baseControl.PainterKind = BaseControlPainterKind.Material;
baseControl.MaterialVariant = MaterialTextFieldVariant.Outlined;
baseControl.LabelText = "Field Label";
baseControl.HelperText = "Helper text";
baseControl.MaterialElevationLevel = 2;
```

### Card Styles

#### 3. Card Painter
- **Style**: Clean card design with subtle elevation
- **Features**: Rounded corners, soft shadows, hover effects
- **Theme Integration**: Card-specific theme colors
- **Best For**: Dashboard widgets, content cards, panels
- **Layout**: Card padding with shadow space

```csharp
baseControl.PainterKind = BaseControlPainterKind.Card;
// Uses theme.CardBackColor, theme.ShadowColor
```

#### 4. Reading Card Painter
- **Style**: Content-focused card with header and settings
- **Features**: Header area, settings icon, content area separation
- **Theme Integration**: Uses theme colors for header and text
- **Best For**: Reading lists, articles, content display
- **Layout**: Header (40px) + content area + padding

```csharp
baseControl.PainterKind = BaseControlPainterKind.ReadingCard;
baseControl.LabelText = "Continue Reading";
// Header with settings icon (three dots)
// Click events: LeadingIconClicked (settings), main Click
```

#### 5. Shortcut Card Painter
- **Style**: Card optimized for keyboard shortcuts and actions  
- **Features**: Clean card background and border
- **Theme Integration**: Theme-based colors
- **Best For**: Help screens, tutorials, shortcut displays
- **Layout**: Simple card with content padding

```csharp
baseControl.PainterKind = BaseControlPainterKind.ShortcutCard;
// Content drawing handled by inheriting controls
```

### Button & Interactive Styles

#### 6. Button Painter
- **Style**: Clean button styling with border and shadow
- **Features**: State-based colors, subtle shadow, rounded corners
- **Theme Integration**: Button-specific theme colors
- **Best For**: Action buttons, interactive elements
- **Layout**: Button padding with shadow offset

```csharp
baseControl.PainterKind = BaseControlPainterKind.Button;
// Uses theme.ButtonBackColor, theme.ButtonHoverBackColor, etc.
```

#### 7. NeoBrutalist Painter
- **Style**: Bold, high-contrast design  
- **Features**: Thick borders, stark shadows, sharp edges, high contrast
- **Theme Integration**: High-contrast theme colors with brutalist modifications
- **Best For**: Bold interfaces, attention-grabbing elements, artistic designs
- **Layout**: Thick borders (4px) + stark shadow (8px offset)

```csharp
baseControl.PainterKind = BaseControlPainterKind.NeoBrutalist;
// Bold, anti-aliased rendering disabled for sharp edges
// High contrast colors from theme
```

### Modern Web Styles

#### 8. Glassmorphism Painter
- **Style**: Modern glass-like appearance
- **Features**: Transparency effects, subtle gradients, inner glow on focus/hover
- **Theme Integration**: Theme colors with glass transparency modifications  
- **Best For**: Modern interfaces, premium designs, overlay elements
- **Layout**: Glass effects with transparency and highlights

```csharp
baseControl.PainterKind = BaseControlPainterKind.Glassmorphism;
// Glass opacity: 15%, border opacity: 30%
// Inner glow on focus/hover states
```

#### 9. Neumorphism Painter
- **Style**: Soft, subtle 3D appearance
- **Features**: Dual shadows (light/dark), pressed/raised states, soft gradients
- **Theme Integration**: Neutral theme colors with light/dark variations
- **Best For**: Soft interfaces, tactile buttons, modern controls
- **Layout**: Dual shadow system (light top-left, dark bottom-right)

```csharp
baseControl.PainterKind = BaseControlPainterKind.Neumorphism;
// Creates inset effect when pressed
// Soft shadow system for 3D appearance
```

#### 10. Minimalist Painter
- **Style**: Clean, minimal web design
- **Features**: Subtle borders, minimal effects, clean typography
- **Theme Integration**: Light theme colors with subtle variations
- **Best For**: Professional interfaces, web-like applications, clean designs
- **Layout**: Minimal padding, subtle 1px borders, focus enhancement

```csharp
baseControl.PainterKind = BaseControlPainterKind.Minimalist;
// 1px borders, 2px focus borders
// Very subtle hover effects
```

## Theme Integration

All painters automatically use BaseControl's current theme (`_currentTheme`):

### Color Properties Used
```csharp
// Background colors
theme.BackColor              // General background
theme.ButtonBackColor        // Button backgrounds  
theme.CardBackColor          // Card backgrounds
theme.DisabledBackColor      // Disabled state

// Border colors  
theme.BorderColor            // General borders
theme.FocusBorderColor       // Focus state
theme.HoverBorderColor       // Hover state  
theme.DisabledBorderColor    // Disabled state

// Button state colors
theme.ButtonHoverBackColor   // Button hover
theme.ButtonPressedBackColor // Button pressed
theme.ButtonSelectedBackColor // Button selected

// Text colors
theme.ForeColor              // Primary text
theme.SecondaryTextColor     // Secondary text

// Effects
theme.ShadowColor            // Shadow effects
theme.FocusIndicatorColor    // Focus indicators
```

### Fallback System
Each painter provides sensible defaults if theme colors are unavailable:

```csharp
Color backgroundColor = theme?.BackColor ?? Color.White;
Color borderColor = theme?.BorderColor ?? Color.Gray;
```

## Layout Rectangles

Each painter provides consistent rectangle properties:

```csharp
public interface IBaseControlPainter
{
    Rectangle DrawingRect { get; }  // Content area for derived controls
    Rectangle BorderRect { get; }   // Outer border area
    Rectangle ContentRect { get; }  // Text/content area (icon-adjusted)
}
```

### Rectangle Usage
- **`BorderRect`**: Outer bounds for borders and shadows
- **`DrawingRect`**: Available area for inheriting controls to draw content
- **`ContentRect`**: Text area adjusted for icons and special elements

## Icon Integration

All painters support comprehensive icon integration:

### Icon Properties
```csharp
// Icon paths (SVG or image)
string LeadingIconPath { get; set; }      // Left icon (SVG)
string TrailingIconPath { get; set; }     // Right icon (SVG)  
string LeadingImagePath { get; set; }     // Left icon (image)
string TrailingImagePath { get; set; }    // Right icon (image)

// Icon configuration
int IconSize { get; set; }                // Icon size in pixels
int IconPadding { get; set; }             // Padding around icons
bool ShowClearButton { get; set; }        // Show clear button

// Icon interaction
bool LeadingIconClickable { get; set; }   // Enable leading icon clicks
bool TrailingIconClickable { get; set; }  // Enable trailing icon clicks
```

### Icon Events
```csharp
// Icon click events
public event EventHandler LeadingIconClicked;
public event EventHandler TrailingIconClicked;

// Trigger methods (used by painters)
public void TriggerLeadingIconClick();
public void TriggerTrailingIconClick();
```

## Material Design Integration

When using the Material painter, additional properties become available:

### Material Properties
```csharp
// Variants
MaterialTextFieldVariant MaterialVariant { get; set; }  // Outlined, Filled, Standard

// Visual styling
int MaterialBorderRadius { get; set; }                  // Border radius
bool MaterialShowFill { get; set; }                     // Show fill background
Color MaterialFillColor { get; set; }                   // Fill color
Color MaterialOutlineColor { get; set; }                // Border color  
Color MaterialPrimaryColor { get; set; }                // Primary accent

// Elevation
int MaterialElevationLevel { get; set; }                // 0-5 shadow levels
bool MaterialUseElevation { get; set; }                 // Enable shadows

// Labels
bool FloatingLabel { get; set; }                        // Floating label behavior
string LabelText { get; set; }                          // Field label
string HelperText { get; set; }                         // Helper text

// Validation
string ErrorText { get; set; }                          // Error message
bool HasError { get; set; }                             // Error state
Color ErrorColor { get; set; }                          // Error color

// Presets
MaterialTextFieldStylePreset StylePreset { get; set; }  // Quick style presets
```

### Material Style Presets
```csharp
public enum MaterialTextFieldStylePreset
{
    Default,                    // Standard outlined
    MaterialOutlined,          // Material 3 outlined  
    MaterialFilled,            // Material 3 filled
    MaterialStandard,          // Material 3 standard
    PillOutlined,              // Pill-shaped outlined
    PillFilled,                // Pill-shaped filled
    DenseOutlined,             // Dense outlined
    DenseFilled,               // Dense filled  
    ComfortableOutlined,       // Comfortable outlined
    ComfortableFilled          // Comfortable filled
}
```

## For Derived Controls

### Where to Draw Content
Always draw inside `DrawContent(Graphics g)`:

```csharp
protected override void DrawContent(Graphics g)
{
    base.DrawContent(g); // Calls painter
    
    // Get content area from active painter
    Rectangle contentArea = this.DrawingRect;
    
    // Draw your content within this area
    if (!contentArea.IsEmpty)
    {
        // Your custom drawing code here
        using (var brush = new SolidBrush(ForeColor))
        {
            g.DrawString("Custom Content", Font, brush, contentArea);
        }
    }
}
```

### Best Practices
1. **Never draw borders/shadows manually** - painters handle outer styling
2. **Always use `DrawingRect`** - provided by the active painter  
3. **Respect icon areas** - use `GetContentRect()` for text positioning
4. **Theme integration** - use `_currentTheme` colors when possible
5. **State awareness** - check `IsHovered`, `IsFocused`, etc. for state-based rendering

### Helper Methods
```csharp
// Get painter-provided rectangles
Rectangle GetContentRect();              // Icon-adjusted content area
Rectangle GetAdjustedContentRect();      // Alternative content area

// Material Design helpers (when using Material painter)
Padding GetMaterialStylePadding();       // Material spacing
Size GetMaterialEffectsSpace();          // Space for focus/elevation
Size GetMaterialIconSpace();             // Space needed for icons
```

## Painter Architecture

### IBaseControlPainter Interface
```csharp
public interface IBaseControlPainter
{
    // Layout rectangles (must be implemented)
    Rectangle DrawingRect { get; }
    Rectangle BorderRect { get; }  
    Rectangle ContentRect { get; }
    
    // Core methods
    void UpdateLayout(BaseControl owner);                              // Calculate layout
    void Paint(Graphics g, BaseControl owner);                        // Render styling
    void UpdateHitAreas(BaseControl owner, Action<...> register);     // Register hit areas
    Size GetPreferredSize(BaseControl owner, Size proposedSize);      // Size calculation
}
```

### Painter Responsibilities
1. **Layout Calculation**: Compute all rectangles based on control size/state
2. **Visual Rendering**: Draw backgrounds, borders, shadows, effects  
3. **Hit Area Registration**: Register clickable areas (icons, buttons)
4. **Size Calculation**: Determine preferred/minimum sizes

### Benefits
- **Consistent interface** across all visual styles
- **Clean separation** between layout and content
- **Easy extensibility** - add new styles by implementing `IBaseControlPainter`
- **No duplicate logic** - each painter handles one visual style
- **Automatic theme integration** - all painters use `_currentTheme`
- **State management** - painters respond to hover, focus, pressed states

## Usage Examples

### Basic Usage
```csharp
// Create control with specific painter
var control = new BaseControl
{
    PainterKind = BaseControlPainterKind.Material,
    Text = "Sample Control",
    Size = new Size(200, 50)
};
```

### Material Design Example  
```csharp
var materialControl = new BaseControl
{
    PainterKind = BaseControlPainterKind.Material,
    MaterialVariant = MaterialTextFieldVariant.Outlined,
    LabelText = "Email Address",
    HelperText = "Enter your email",
    MaterialBorderRadius = 8,
    MaterialElevationLevel = 1,
    LeadingIconPath = "email-icon.svg",
    Size = new Size(300, 60)
};
```

### Modern Web Style Example
```csharp
var modernControl = new BaseControl
{
    PainterKind = BaseControlPainterKind.Glassmorphism,
    Text = "Glass Button", 
    BackColor = Color.FromArgb(100, Color.CornflowerBlue),
    Size = new Size(150, 40)
};
```

### Card Style Example
```csharp
var cardControl = new BaseControl  
{
    PainterKind = BaseControlPainterKind.ReadingCard,
    LabelText = "Continue Reading",
    Size = new Size(320, 180)
};

// Handle settings click
cardControl.LeadingIconClicked += (s, e) => {
    // Show settings menu
};
```

### Brutalist Style Example
```csharp
var boldControl = new BaseControl
{
    PainterKind = BaseControlPainterKind.NeoBrutalist,
    Text = "BOLD ACTION",
    BackColor = Color.Yellow,
    BorderColor = Color.Black,
    Size = new Size(120, 50)
};
```

## Integration Points

- **`PainterKind` property**: Selects the active painter
- **Automatic initialization**: Painters are created and updated automatically  
- **Theme integration**: `_currentTheme` colors applied automatically
- **Hit testing**: Works seamlessly across all painters
- **Event handling**: Standard Click, hover, focus events work with all painters
- **Icon support**: Leading/trailing icons work with all painters
- **DPI scaling**: All painters respect DPI scaling settings

## Backward Compatibility

- **Existing controls continue to work unchanged**
- **Default `PainterKind.Auto`** provides Classic styling
- **All legacy properties preserved** (borders, shadows, etc.)
- **No breaking changes** to existing APIs
- **Gradual adoption** - can be enabled per-control

## Performance Considerations

- **Painter caching**: Painters are reused when `PainterKind` doesn't change
- **Efficient graphics**: Proper resource disposal and high-quality rendering
- **Smart invalidation**: Only redraws when necessary  
- **Layout optimization**: Layout calculations cached until size/state changes

## Extension Guide

To create a custom painter:

1. **Implement `IBaseControlPainter`**
2. **Handle all required rectangles** (`DrawingRect`, `BorderRect`, `ContentRect`)
3. **Use theme colors** from `owner._currentTheme`  
4. **Register hit areas** for interactive elements
5. **Follow the naming convention**: `[Style]BaseControlPainter`

```csharp
internal sealed class CustomBaseControlPainter : IBaseControlPainter
{
    private Rectangle _drawingRect;
    private Rectangle _borderRect;  
    private Rectangle _contentRect;
    
    public Rectangle DrawingRect => _drawingRect;
    public Rectangle BorderRect => _borderRect;
    public Rectangle ContentRect => _contentRect;
    
    public void UpdateLayout(BaseControl owner) { /* Calculate rects */ }
    public void Paint(Graphics g, BaseControl owner) { /* Draw styling */ }
    public void UpdateHitAreas(BaseControl owner, Action<...> register) { /* Register areas */ }
    public Size GetPreferredSize(BaseControl owner, Size proposedSize) { /* Size calculation */ }
}
```

This architecture provides a powerful, flexible, and extensible foundation for creating visually rich and consistent UI controls that can adapt to any design system or visual style.
