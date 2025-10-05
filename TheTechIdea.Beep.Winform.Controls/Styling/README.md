# Beep WinForm Styling System

A comprehensive styling system for Beep WinForm controls with support for 21 distinct design systems.

## Overview

The styling system is organized into **5 helper categories**, each providing static methods that return style-specific values:

- **Colors/** - Color definitions for all visual elements
- **Spacing/** - Padding, margins, and dimensional values
- **Borders/** - Border radius, width, and visual characteristics
- **Shadows/** - Shadow and elevation definitions
- **Typography/** - Font families, sizes, and text styling

## Supported Design Systems

All 21 design systems have **completely distinct values** across all helpers:

1. **Material3** - Google Material Design 3
2. **iOS15** - Apple iOS 15+ design language
3. **Fluent2** - Microsoft Fluent Design 2
4. **Minimal** - Ultra-clean minimal design
5. **AntDesign** - Ant Design system
6. **MaterialYou** - Google Material You (dynamic)
7. **Windows11Mica** - Windows 11 Mica material
8. **MacOSBigSur** - Apple macOS Big Sur
9. **ChakraUI** - Chakra UI design system
10. **TailwindCard** - Tailwind CSS card styles
11. **NotionMinimal** - Notion-inspired minimal
12. **VercelClean** - Vercel clean design
13. **StripeDashboard** - Stripe dashboard aesthetic
14. **DarkGlow** - Dark theme with neon glow
15. **DiscordStyle** - Discord's Blurple theme
16. **GradientModern** - Modern gradient design
17. **GlassAcrylic** - Glass morphism/acrylic
18. **Neumorphism** - Soft UI neumorphic design
19. **Bootstrap** - Bootstrap framework
20. **FigmaCard** - Figma card design
21. **PillRail** - Pill-shaped navigation rail

## Architecture Principles

### No Base Classes
Each style has **completely independent values** - no inheritance or base classes. This ensures:
- Each design system is truly distinct
- No unintended style bleeding
- Maximum maintainability
- Clear, explicit values

### Helper Organization
Helpers are organized by **type** (Colors, Spacing, etc.), not by style. Each helper file contains methods that accept `BeepControlStyle` and return style-specific values using switch expressions.

## Usage Examples

### Using StyleColors

```csharp
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

// Get background color for Material3
Color bg = StyleColors.GetBackground(BeepControlStyle.Material3);
// Returns: Color.FromArgb(245, 245, 250) - Light lavender

// Get primary color for iOS15
Color primary = StyleColors.GetPrimary(BeepControlStyle.iOS15);
// Returns: Color.FromArgb(0, 122, 255) - iOS blue

// Get hover color for DarkGlow
Color hover = StyleColors.GetHover(BeepControlStyle.DarkGlow);
// Returns: Color.FromArgb(168, 85, 247) - Neon purple glow
```

### Available Color Methods

- `GetBackground(style)` - Main background color
- `GetPrimary(style)` - Primary accent color
- `GetSecondary(style)` - Secondary color for less emphasis
- `GetForeground(style)` - Text/icon color
- `GetBorder(style)` - Border color
- `GetHover(style)` - Hover state color
- `GetSelection(style)` - Selection/active state color

### Using StyleSpacing

```csharp
using TheTechIdea.Beep.Winform.Controls.Styling.Spacing;

// Get padding for Fluent2
int padding = StyleSpacing.GetPadding(BeepControlStyle.Fluent2);
// Returns: 12 pixels

// Get icon size for Material3
int iconSize = StyleSpacing.GetIconSize(BeepControlStyle.Material3);
// Returns: 24 pixels

// Get item height for Neumorphism (needs extra space for shadows)
int itemHeight = StyleSpacing.GetItemHeight(BeepControlStyle.Neumorphism);
// Returns: 52 pixels
```

### Available Spacing Methods

- `GetPadding(style)` - Internal padding (8-20px)
- `GetItemSpacing(style)` - Space between items (2-12px)
- `GetIconSize(style)` - Icon dimensions (18-28px)
- `GetIndentationWidth(style)` - Indentation for nested items (12-24px)
- `GetItemHeight(style)` - Default item height (32-52px)

### Using StyleBorders

```csharp
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

// Get border radius for Material3 (large pills)
int radius = StyleBorders.GetRadius(BeepControlStyle.Material3);
// Returns: 28 pixels

// Check if DiscordStyle uses filled style (no borders)
bool isFilled = StyleBorders.IsFilled(BeepControlStyle.DiscordStyle);
// Returns: true

// Get accent bar width for Fluent2
int accentWidth = StyleBorders.GetAccentBarWidth(BeepControlStyle.Fluent2);
// Returns: 4 pixels (vertical bar on selection)
```

### Available Border Methods

- `GetRadius(style)` - Border radius for containers (0-28px)
- `GetSelectionRadius(style)` - Border radius for selected items (0-100px)
- `GetBorderWidth(style)` - Border width (0-1.5f)
- `IsFilled(style)` - Whether style uses filled (true) or outlined (false) design
- `GetAccentBarWidth(style)` - Width of Fluent-style accent bars (0-4px)

### Using StyleShadows

```csharp
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

// Check if Material3 uses shadows
bool hasShadow = StyleShadows.HasShadow(BeepControlStyle.Material3);
// Returns: true

// Get shadow blur for Material3
int blur = StyleShadows.GetShadowBlur(BeepControlStyle.Material3);
// Returns: 12 pixels

// Get shadow color
Color shadowColor = StyleShadows.GetShadowColor(BeepControlStyle.Material3);
// Returns: Color.FromArgb(60, 0, 0, 0) - Soft black shadow

// Check if Neumorphism uses dual shadows
bool dualShadows = StyleShadows.UsesDualShadows(BeepControlStyle.Neumorphism);
// Returns: true (light + dark for soft effect)
```

### Available Shadow Methods

- `HasShadow(style)` - Whether style uses shadows
- `GetShadowBlur(style)` - Blur radius (8-24px)
- `GetShadowSpread(style)` - Spread radius (-2 to 4px)
- `GetShadowOffsetY(style)` - Vertical offset (0-10px)
- `GetShadowOffsetX(style)` - Horizontal offset (0-5px)
- `GetShadowColor(style)` - Shadow color with opacity
- `GetNeumorphismHighlight(style)` - White highlight for neumorphism
- `UsesDualShadows(style)` - Whether style needs dual shadows
- `UsesGlow(style)` - Whether style uses glow effect

### Using StyleTypography

```csharp
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

// Get font family for iOS15
string fontFamily = StyleTypography.GetFontFamily(BeepControlStyle.iOS15);
// Returns: "SF Pro Display, Segoe UI, Arial"

// Get font for Material3
Font font = StyleTypography.GetFont(BeepControlStyle.Material3);
// Returns: Roboto 14pt Regular (or fallback)

// Get letter spacing for Minimal (wider tracking)
float spacing = StyleTypography.GetLetterSpacing(BeepControlStyle.Minimal);
// Returns: 0.2f pixels

// Get active font style (bold for selected items)
FontStyle activeStyle = StyleTypography.GetActiveFontStyle(BeepControlStyle.Material3);
// Returns: FontStyle.Bold
```

### Available Typography Methods

- `GetFontFamily(style)` - Primary font family with fallbacks
- `GetFontSize(style)` - Default font size (13-14pt)
- `GetFontStyle(style)` - Regular text font style
- `GetActiveFontStyle(style)` - Selected/active text font style
- `GetLineHeight(style)` - Line height multiplier (1.375-1.625)
- `GetLetterSpacing(style)` - Letter spacing in pixels (-0.2 to 0.5)
- `IsMonospace(style)` - Whether style uses monospace fonts
- `GetFont(style, size?, fontStyle?)` - Create Font object with fallbacks

## Integration Example

Here's how to integrate the styling system into a painter:

```csharp
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Spacing;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

public class MyCustomPainter
{
    private BeepControlStyle _currentStyle;

    public void Draw(Graphics g, Rectangle bounds)
    {
        // Get all style values
        Color bg = StyleColors.GetBackground(_currentStyle);
        Color primary = StyleColors.GetPrimary(_currentStyle);
        int padding = StyleSpacing.GetPadding(_currentStyle);
        int radius = StyleBorders.GetRadius(_currentStyle);
        Font font = StyleTypography.GetFont(_currentStyle);

        // Calculate content area
        Rectangle contentRect = new Rectangle(
            bounds.X + padding,
            bounds.Y + padding,
            bounds.Width - (padding * 2),
            bounds.Height - (padding * 2)
        );

        // Draw background with rounded corners
        using (var bgBrush = new SolidBrush(bg))
        using (var path = CreateRoundedRectangle(bounds, radius))
        {
            g.FillPath(bgBrush, path);
        }

        // Draw shadow if style uses it
        if (StyleShadows.HasShadow(_currentStyle))
        {
            DrawShadow(g, bounds, _currentStyle);
        }

        // Draw text with style-specific font
        using (var textBrush = new SolidBrush(StyleColors.GetForeground(_currentStyle)))
        {
            g.DrawString("Hello", font, textBrush, contentRect);
        }
    }

    private void DrawShadow(Graphics g, Rectangle bounds, BeepControlStyle style)
    {
        int blur = StyleShadows.GetShadowBlur(style);
        int offsetY = StyleShadows.GetShadowOffsetY(style);
        Color shadowColor = StyleShadows.GetShadowColor(style);
        
        // Draw shadow logic here...
    }
}
```

## Style Characteristics Reference

### Material3
- **Colors**: Purple/lavender theme (RGB 103,80,164)
- **Spacing**: 12px padding, 24px icons, 40px items
- **Borders**: 28px radius (full pills), filled style
- **Shadows**: 12px blur, 4px offset, soft black
- **Typography**: Roboto, 14pt, letter spacing 0.1

### iOS15
- **Colors**: Blue theme (RGB 0,122,255)
- **Spacing**: 16px padding, 22px icons, 44px items
- **Borders**: 12px radius, outlined with subtle border
- **Shadows**: None (flat with blur)
- **Typography**: SF Pro Display, 14pt, negative tracking -0.2

### Fluent2
- **Colors**: Blue theme (RGB 0,120,212)
- **Spacing**: 12px padding, 20px icons, 40px items
- **Borders**: 4px radius, 4px accent bars
- **Shadows**: 8px blur, subtle
- **Typography**: Segoe UI Variable, 14pt

### DarkGlow
- **Colors**: Dark with neon purple (RGB 139,92,246)
- **Spacing**: 10px padding, 20px icons, 40px items
- **Borders**: 12px radius, filled
- **Shadows**: 24px glow, purple color
- **Typography**: JetBrains Mono (monospace), 13pt, wide tracking 0.5

### Neumorphism
- **Colors**: Soft blue-gray (RGB 228,230,235)
- **Spacing**: 20px padding (extra for shadows), 52px items
- **Borders**: 20px radius, no borders
- **Shadows**: Dual shadows (light + dark), 20px blur, 10px offset
- **Typography**: Montserrat, 14pt, wide tracking 0.5

### Minimal
- **Colors**: Pure black/white with gray accent
- **Spacing**: 8px padding, 2px item spacing, 36px items
- **Borders**: 0px radius (square), outlined
- **Shadows**: None (flat)
- **Typography**: Inter, 13pt, wide tracking 0.2

## Best Practices

### 1. Always Use DrawingRect
Use `DrawingRect` from `BaseControl` instead of `ClientRectangle`:

```csharp
// ❌ Wrong
g.FillRectangle(brush, ClientRectangle);

// ✅ Correct
g.FillRectangle(brush, DrawingRect);
```

### 2. Check Style Features Before Using
Not all styles use the same features:

```csharp
// Check before drawing shadows
if (StyleShadows.HasShadow(_currentStyle))
{
    DrawShadow(g, bounds, _currentStyle);
}

// Check for filled vs outlined
if (StyleBorders.IsFilled(_currentStyle))
{
    DrawFilled(g, bounds);
}
else
{
    DrawOutlined(g, bounds);
}
```

### 3. Use Style-Specific Colors
Respect the `UseThemeColors` flag:

```csharp
if (UseThemeColors)
{
    // Use theme colors from ThemesManager
    color = _themesManager.GetColor("Primary");
}
else
{
    // Use style-specific colors
    color = StyleColors.GetPrimary(_currentStyle);
}
```

### 4. Font Fallbacks
The `GetFont()` method handles fallbacks automatically:

```csharp
// This will try fonts in order and fall back to Segoe UI
Font font = StyleTypography.GetFont(BeepControlStyle.iOS15);
// Tries: SF Pro Display → Segoe UI → Arial
```

### 5. Combine Helpers for Complete Styling
Use multiple helpers together for cohesive design:

```csharp
// Complete item rendering
var itemBounds = new Rectangle(x, y, width, 
    StyleSpacing.GetItemHeight(_currentStyle));

using (var bgBrush = new SolidBrush(
    isHovered ? StyleColors.GetHover(_currentStyle) : 
                StyleColors.GetBackground(_currentStyle)))
using (var path = CreateRoundedRectangle(itemBounds, 
    StyleBorders.GetSelectionRadius(_currentStyle)))
{
    g.FillPath(bgBrush, path);
}

var textFont = StyleTypography.GetFont(_currentStyle, 
    fontStyle: isSelected ? StyleTypography.GetActiveFontStyle(_currentStyle) : 
                           StyleTypography.GetFontStyle(_currentStyle));
```

## Extending the System

### Adding New Styles
To add a new style:

1. Add enum value to `BeepControlStyle`
2. Add case to each method in all 5 helper classes
3. Define distinct values for all properties
4. Test against existing controls

### Adding New Properties
To add a new style property:

1. Create new static method in appropriate helper class
2. Add switch expression with values for all 21 styles
3. Add XML documentation
4. Update this README

## File Structure

```
Styling/
├── Colors/
│   └── StyleColors.cs           # 7 color methods
├── Spacing/
│   └── StyleSpacing.cs          # 5 spacing methods
├── Borders/
│   └── StyleBorders.cs          # 5 border methods
├── Shadows/
│   └── StyleShadows.cs          # 9 shadow methods
└── Typography/
    └── StyleTypography.cs       # 8 typography methods
```

## Performance Notes

- All methods use **switch expressions** (compiled to jump tables)
- **No reflection** or dynamic lookups
- **No object allocation** except for Font creation
- **Inline-friendly** for JIT optimization
- Color values are **structs** (no heap allocation)

## License

Part of the Beep WinForm Controls library.
