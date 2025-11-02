# BeepAdvancedButton

## Overview

BeepAdvancedButton is a sophisticated, highly customizable button control for WinForms applications. It offers multiple visual styles, smooth animations, and rich features inspired by modern UI design systems.

## Features

- **10+ Button Styles**: Solid, Outlined, Icon, Text, Toggle, FAB (Floating Action Button), Ghost, Link, Gradient, and Icon+Text
- **3 Size Presets**: Small (32px), Medium (40px), Large (48px) with automatic sizing
- **Smooth Animations**: Ripple effects, hover transitions, loading spinners
- **SVG Icon Support**: Full integration with BeepImage and ImagePainter for scalable vector graphics
- **Theme Integration**: Automatically adapts to BeepTheme system
- **Accessibility**: Keyboard navigation, focus indicators, ARIA-compliant
- **Custom Painting**: Modular painter architecture for extensibility

## Architecture

The control uses a **modular partial class structure** with dedicated painters for each style:

```
BeepAdvancedButton/
├── BeepAdvancedButton.cs           # Main control class
├── Enums/
│   └── AdvancedButtonEnums.cs      # ButtonStyle, ButtonSize, ButtonState
├── Models/
│   └── AdvancedButtonModels.cs     # PaintContext, Metrics
├── Painters/
│   ├── IAdvancedButtonPainter.cs   # Painter interface
│   ├── BaseButtonPainter.cs        # Common painting logic
│   ├── ButtonPainterFactory.cs     # Factory pattern
│   ├── SolidButtonPainter.cs       # Solid filled buttons
│   ├── OutlinedButtonPainter.cs    # Bordered buttons
│   ├── IconButtonPainter.cs        # Icon-only buttons
│   ├── TextButtonPainter.cs        # Text-only buttons
│   ├── ToggleButtonPainter.cs      # Toggle on/off buttons
│   ├── FABButtonPainter.cs         # Circular floating buttons
│   ├── GhostButtonPainter.cs       # Subtle ghost buttons
│   ├── LinkButtonPainter.cs        # Hyperlink-style buttons
│   ├── GradientButtonPainter.cs    # Gradient fills
│   └── IconTextButtonPainter.cs    # Combined icon+text
└── Helpers/
    └── BeepAdvancedButtonHelper.cs # Layout calculations, utilities
```

## Usage

### Basic Solid Button

```csharp
var button = new BeepAdvancedButton
{
    Text = "Click Me",
    ButtonStyle = AdvancedButtonStyle.Solid,
    ButtonSize = AdvancedButtonSize.Medium,
    SolidBackground = Color.FromArgb(79, 70, 229), // Indigo
    SolidForeground = Color.White
};
button.Click += (s, e) => MessageBox.Show("Button clicked!");
```

### Icon Button with SVG

```csharp
var iconButton = new BeepAdvancedButton
{
    ButtonStyle = AdvancedButtonStyle.Icon,
    ButtonSize = AdvancedButtonSize.Small,
    ImagePath = SvgsUI.Add, // Using strongly-typed path
    Size = new Size(40, 40)
};
```

### Toggle Button

```csharp
var toggleButton = new BeepAdvancedButton
{
    Text = "Toggle Me",
    ButtonStyle = AdvancedButtonStyle.Toggle,
    IsToggled = false
};
toggleButton.Click += (s, e) => 
{
    // IsToggled automatically switches on click
    Debug.WriteLine($"Toggled: {toggleButton.IsToggled}");
};
```

### FAB (Floating Action Button)

```csharp
var fab = new BeepAdvancedButton
{
    ButtonStyle = AdvancedButtonStyle.FAB,
    ButtonSize = AdvancedButtonSize.Large, // 72x72px circle
    ImagePath = SvgsUI.Add,
    ShowShadow = true
};
```

### Icon + Text Button

```csharp
var iconTextButton = new BeepAdvancedButton
{
    Text = "Save",
    ButtonStyle = AdvancedButtonStyle.IconText,
    ImagePath = SvgsUI.Save,
    ButtonSize = AdvancedButtonSize.Medium
};
```

### Loading State

```csharp
button.IsLoading = true; // Shows spinner
await Task.Delay(2000);
button.IsLoading = false; // Back to normal
```

## Button Styles

### Solid
Filled background with text/icon. Most prominent button type.
- **Use for**: Primary actions (Save, Submit, Continue)

### Outlined
Border with transparent or subtle background on hover.
- **Use for**: Secondary actions (Cancel, Back)

### Icon
Icon-only with subtle background on hover.
- **Use for**: Toolbars, compact UIs

### Text
Minimal styling, text-only with subtle hover.
- **Use for**: Tertiary actions, inline links

### Toggle
On/off state with visual feedback.
- **Use for**: Filters, settings switches

### FAB (Floating Action Button)
Circular, elevated button (typically with icon).
- **Use for**: Primary floating actions (Add, Create)

### Ghost
Very subtle, almost invisible until hover.
- **Use for**: Non-intrusive actions

### Link
Hyperlink style with underline.
- **Use for**: Navigation, "Learn more" actions

### Gradient
Linear gradient fill from top to bottom.
- **Use for**: Eye-catching CTAs

### IconText
Optimized layout for icon + text combination.
- **Use for**: Buttons needing both visual and text clarity

## Size Specifications

Based on UI design standards:

| Size   | Height | Padding H | Padding V | Icon Size | Min Width |
|--------|--------|-----------|-----------|-----------|-----------|
| Small  | 32px   | 12px      | 6px       | 16px      | 64px      |
| Medium | 40px   | 16px      | 8px       | 20px      | 80px      |
| Large  | 48px   | 20px      | 12px      | 24px      | 96px      |

## Properties

### Appearance

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ButtonStyle` | `AdvancedButtonStyle` | `Solid` | Visual style of the button |
| `ButtonSize` | `AdvancedButtonSize` | `Medium` | Size preset (Small/Medium/Large) |
| `Text` | `string` | `"Button"` | Text displayed on button |
| `ImagePath` | `string` | `""` | Path to icon (SVG/PNG/JPG) |
| `IconLeft` | `string` | `""` | Left icon path (for dual icons) |
| `IconRight` | `string` | `""` | Right icon path |
| `BorderRadius` | `int` | `8` | Corner rounding (pixels) |
| `BorderWidth` | `int` | `2` | Border thickness |
| `BorderColor` | `Color` | Indigo | Border color |
| `ShowShadow` | `bool` | `true` | Enable drop shadow |

### Colors

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SolidBackground` | `Color` | Indigo-600 | Background for solid buttons |
| `SolidForeground` | `Color` | White | Text/icon color for solid |
| `HoverBackground` | `Color` | Indigo-700 | Background on hover |
| `HoverForeground` | `Color` | White | Foreground on hover |
| `DisabledBackground` | `Color` | Gray-200 | Background when disabled |
| `DisabledForeground` | `Color` | Gray-400 | Foreground when disabled |

### Behavior

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsToggled` | `bool` | `false` | Toggle state (for Toggle buttons) |
| `IsLoading` | `bool` | `false` | Shows loading spinner |
| `Enabled` | `bool` | `true` | Enable/disable interaction |

## Events

- `Click` - Standard click event
- `MouseEnter` - Mouse hover begins
- `MouseLeave` - Mouse hover ends
- `MouseDown` - Mouse button pressed (starts ripple)
- `MouseUp` - Mouse button released

## Theme Integration

BeepAdvancedButton automatically integrates with BeepThemesManager:

```csharp
// Theme colors are applied automatically
button.Theme = BeepThemesManager.CurrentTheme;
```

The control respects theme properties:
- `PrimaryColor` → `SolidBackground`
- `PrimaryForeColor` → `SolidForeground`
- `BorderColor` → `BorderColor`
- `DisabledBackColor` → `DisabledBackground`

## Animation Features

### Ripple Effect
Material Design-style ripple emanates from click point.
- Automatically triggered on `MouseDown`
- Clips to button bounds
- Fades out smoothly

### Loading Spinner
Rotating arc spinner replaces content when `IsLoading = true`.

### Hover Transitions
Smooth color transitions on hover (handled by painter).

## Extending with Custom Painters

Create a custom painter by implementing `IAdvancedButtonPainter`:

```csharp
public class NeonButtonPainter : BaseButtonPainter
{
    public override void Paint(AdvancedButtonPaintContext context)
    {
        // Custom painting logic
        // Use helper methods from BaseButtonPainter
    }
}

// Register in factory
public static IAdvancedButtonPainter CreatePainter(AdvancedButtonStyle style)
{
    return style switch
    {
        AdvancedButtonStyle.Neon => new NeonButtonPainter(),
        // ... existing styles
    };
}
```

## Best Practices

1. **Use Appropriate Styles**
   - Solid for primary actions
   - Outlined for secondary actions
   - Text/Ghost for tertiary actions

2. **Size Consistency**
   - Use same size for buttons in a group
   - Small: Compact UIs, toolbars
   - Medium: Standard forms
   - Large: Touch interfaces, prominent CTAs

3. **Icon Usage**
   - Use SVG paths from `SvgsUI`, `SvgsDatasources`, etc.
   - Icon-only buttons should have tooltips
   - Keep icon size consistent with button size

4. **Accessibility**
   - Always provide meaningful `Text` (even for icon buttons)
   - Use `TabStop = true` for keyboard navigation
   - Test with high contrast themes

5. **Loading States**
   - Show `IsLoading = true` during async operations
   - Disable button when loading
   - Provide feedback after completion

## Design Inspiration

Based on modern UI design systems:
- Material Design (Google)
- Fluent Design (Microsoft)
- Carbon Design (IBM)
- Ant Design
- Tailwind CSS

## Performance

- Custom painting with `OptimizedDoubleBuffer`
- Efficient ripple animation (~60 FPS)
- SVG rendering cached by ImagePainter
- Minimal GC pressure

## Known Limitations

- FAB buttons are always circular (square bounds required)
- Ripple effect may not perfectly match Material Design spec
- Loading spinner is simple arc (not full Material spinner)
- Shadow blur is approximated (not true Gaussian blur)

## Future Enhancements

- [ ] Icon animation on click
- [ ] Badge/notification dot
- [ ] Button group support
- [ ] Vertical icon+text layout
- [ ] Custom ripple colors
- [ ] Async click handler with auto-loading
- [ ] Tooltip integration
- [ ] Keyboard shortcuts display

## Related Controls

- `BeepButton` - Original button control
- `BeepCircularButton` - Specialized circular button
- `BeepChevronButton` - Button with chevron indicator
- `BeepImage` - Image display control

## Maintenance

**Created**: 2024  
**Last Updated**: 2024  
**Owner**: The Tech Idea Team  
**Status**: Active Development

## Contributing

When contributing to BeepAdvancedButton:
1. Follow the painter pattern for new styles
2. Update this README with new features
3. Add XML documentation to public members
4. Test all button styles and sizes
5. Ensure theme integration works
6. Check accessibility compliance

---

**Note**: This control is part of the Beep.Winform framework and follows BaseControl conventions. Always inherit from BaseControl and use BeepThemesManager for consistency.
