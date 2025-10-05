# Beep WinForm Painter Architecture

## Overview
Complete refactored painting system with separate painter classes for all rendering operations. Each painter type is organized into its own folder with classes grouped by style family.

## Folder Structure
```
Styling/
├── BackgroundPainters/      (10 classes)
├── BorderPainters/          (6 classes)
├── TextPainters/            (4 classes)
├── ButtonPainters/          (5 classes)
├── Colors/                  (StyleColors helper)
├── Spacing/                 (StyleSpacing helper)
├── Borders/                 (StyleBorders helper)
├── Shadows/                 (StyleShadows helper)
└── Typography/              (StyleTypography helper)
```

---

## 1. BackgroundPainters (10 Classes)

### MaterialBackgroundPainter.cs
**Styles**: Material3, MaterialYou  
**Effect**: Elevation-based highlight overlay  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### iOSBackgroundPainter.cs
**Styles**: iOS15  
**Effect**: Translucent overlay (15 alpha white)  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### MacOSBackgroundPainter.cs
**Styles**: MacOSBigSur  
**Effect**: Subtle vertical gradient (5% lighter at top)  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### MicaBackgroundPainter.cs
**Styles**: Windows11Mica  
**Effect**: Very subtle gradient (2% darker at bottom)  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### GlowBackgroundPainter.cs
**Styles**: DarkGlow  
**Effect**: Neon inner glow (3 rings: 80α, 40α, 20α at 1px, 3px, 6px inset)  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### GradientBackgroundPainter.cs
**Styles**: GradientModern  
**Effect**: Vertical gradient (primary → 30% darker)  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### GlassBackgroundPainter.cs
**Styles**: GlassAcrylic  
**Effect**: Frosted glass with 3 layers (base 50α + highlight 15α + shine 25α)  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### NeumorphismBackgroundPainter.cs
**Styles**: Neumorphism  
**Effect**: Soft embossed look (10px blur, 50α white top-left, 80α black bottom-right)  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### WebFrameworkBackgroundPainter.cs
**Styles**: Bootstrap, TailwindCard, DiscordStyle, StripeDashboard, FigmaCard  
**Methods**: 
- `PaintBootstrap(...)` - Simple solid
- `PaintTailwind(...)` - Subtle gradient (5% darker at bottom)
- `PaintDiscord(...)` - Simple solid
- `PaintStripe(...)` - Very subtle gradient (3% lighter at top)
- `PaintFigma(...)` - Simple solid

### SolidBackgroundPainter.cs
**Styles**: Fluent2, ChakraUI, NotionMinimal, Minimal, VercelClean, AntDesign, PillRail  
**Effect**: Simple solid background color  
**Method**: `Paint(Graphics g, Rectangle bounds, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

---

## 2. BorderPainters (6 Classes)

### MaterialBorderPainter.cs
**Styles**: Material3, MaterialYou  
**Border**: Only if NOT filled (checks IsFilled())  
**Width**: 1px  
**Method**: `Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### FluentBorderPainter.cs
**Styles**: Fluent2, Windows11Mica  
**Border**: Optional 1px + 4px accent bar (vertical, left edge, focused only)  
**Special**: Accent bar is Fluent signature  
**Method**: `Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### AppleBorderPainter.cs
**Styles**: iOS15, MacOSBigSur  
**Border**: 1px subtle outlined, clean aesthetic  
**Method**: `Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### MinimalBorderPainter.cs
**Styles**: Minimal, NotionMinimal, VercelClean  
**Border**: Always present (these styles rely on borders for definition)  
**Width**: 1px  
**Method**: `Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### EffectBorderPainter.cs
**Styles**: Neumorphism, GlassAcrylic, DarkGlow  
**Border**: No traditional borders (part of background effect)  
**Special**: DarkGlow gets glow border on focus (2px, 100α primary)  
**Method**: `Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### WebFrameworkBorderPainter.cs
**Styles**: Bootstrap, Tailwind, Stripe, Figma, Discord, AntDesign, Chakra  
**Border**: Standard 1px borders  
**Special**: Tailwind gets animated ring effect (3px pen, 40α primary, +2px offset rect)  
**Helper**: `CreateRoundedRectangle()` for Tailwind ring  
**Method**: `Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

---

## 3. TextPainters (4 Classes)

### MaterialTextPainter.cs
**Styles**: Material3, MaterialYou  
**Font**: Roboto 14pt (from StyleTypography)  
**Font Style**: Bold when focused, regular otherwise  
**Rendering**: ClearTypeGridFit  
**Method**: `Paint(Graphics g, Rectangle bounds, string text, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### AppleTextPainter.cs
**Styles**: iOS15, MacOSBigSur  
**Font**: SF Pro Display 14pt (from StyleTypography)  
**Tracking**: Negative -0.2 (tighter letter spacing)  
**Rendering**: ClearTypeGridFit  
**Method**: `Paint(Graphics g, Rectangle bounds, string text, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### MonospaceTextPainter.cs
**Styles**: DarkGlow  
**Font**: JetBrains Mono 13pt (from StyleTypography)  
**Font Style**: Bold when focused  
**Rendering**: ClearTypeGridFit (monospace optimized)  
**Method**: `Paint(Graphics g, Rectangle bounds, string text, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### StandardTextPainter.cs
**Styles**: Fluent2, Minimal, Bootstrap, Tailwind, Discord, Stripe, Figma, AntDesign, Chakra, Notion, Vercel, Windows11Mica, etc.  
**Font**: Various (Segoe UI Variable, Inter, etc. - from StyleTypography)  
**Font Style**: Bold when focused, regular otherwise  
**Rendering**: ClearTypeGridFit  
**Method**: `Paint(Graphics g, Rectangle bounds, string text, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

**Common Features**:
- StringFormat with Near alignment, Center vertical, EllipsisCharacter trimming
- NoWrap flag
- Theme color override support via GetColor() helper

---

## 4. ButtonPainters (5 Classes)

### MaterialButtonPainter.cs
**Styles**: Material3, MaterialYou  
**Appearance**: Filled buttons with 28px radius  
**Colors**: Secondary background, Foreground arrow  
**Arrow Size**: 4px triangles  
**Method**: `PaintButtons(Graphics g, Rectangle upRect, Rectangle downRect, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### AppleButtonPainter.cs
**Styles**: iOS15, MacOSBigSur  
**Appearance**: Outlined buttons with 6px radius  
**Border**: 1px border color  
**Colors**: Transparent background, Foreground arrow  
**Arrow Size**: 4px triangles  
**Method**: `PaintButtons(Graphics g, Rectangle upRect, Rectangle downRect, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### FluentButtonPainter.cs
**Styles**: Fluent2, Windows11Mica  
**Appearance**: Filled buttons with 4px radius  
**Colors**: Secondary background, Foreground arrow  
**Arrow Size**: 4px triangles  
**Method**: `PaintButtons(Graphics g, Rectangle upRect, Rectangle downRect, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### MinimalButtonPainter.cs
**Styles**: Minimal, NotionMinimal, VercelClean  
**Appearance**: Outlined buttons with 4px radius  
**Border**: 1px border color  
**Colors**: Transparent background, Foreground arrow  
**Arrow Size**: 4px triangles  
**Method**: `PaintButtons(Graphics g, Rectangle upRect, Rectangle downRect, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

### StandardButtonPainter.cs
**Styles**: Bootstrap, Tailwind, Discord, Stripe, Figma, AntDesign, Chakra, GradientModern, GlassAcrylic, Neumorphism, DarkGlow, PillRail  
**Appearance**: Filled + bordered buttons with 6px radius  
**Colors**: Secondary background, Border outline, Foreground arrow  
**Arrow Size**: 4px triangles  
**Method**: `PaintButtons(Graphics g, Rectangle upRect, Rectangle downRect, bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)`  

**Common Features**:
- ArrowDirection enum (Up, Down)
- DrawArrow() helper with SmoothingMode.AntiAlias
- CreateRoundedRectangle() helper for button shapes
- Theme color override support via GetColor() helper

---

## Pattern & Best Practices

### Consistent Signature Pattern
All painters follow this structure:
```csharp
public static class [Family]Painter
{
    public static void Paint[Operation](
        Graphics g,
        Rectangle bounds / string text / Rectangle upRect + downRect,
        [operation-specific params],
        BeepControlStyle style,
        IBeepTheme theme,
        bool useThemeColors
    )
    {
        // Implementation
    }
    
    private static Color GetColor(
        BeepControlStyle style,
        Func<BeepControlStyle, Color> styleColorFunc,
        string themeColorKey,
        IBeepTheme theme,
        bool useThemeColors
    )
    {
        if (useThemeColors && theme != null)
        {
            var themeColor = BeepStyling.GetColor(themeColorKey);
            if (themeColor != Color.Empty)
                return themeColor;
        }
        return styleColorFunc(style);
    }
}
```

### Key Design Principles
1. **Static Methods**: No instances, direct calls
2. **Style Groups**: One class handles multiple related styles
3. **Theme Integration**: All painters accept IBeepTheme + useThemeColors flag
4. **Helper Methods**: Internal GetColor() for theme override, plus operation-specific helpers
5. **Consistent Parameters**: Graphics, bounds, style, theme, useThemeColors always present

### Usage in BeepStyling.cs
```csharp
// Background painting
switch (style)
{
    case BeepControlStyle.Material3:
    case BeepControlStyle.MaterialYou:
        MaterialBackgroundPainter.Paint(g, bounds, style, theme, useThemeColors);
        break;
    case BeepControlStyle.iOS15:
        iOSBackgroundPainter.Paint(g, bounds, style, theme, useThemeColors);
        break;
    // ... etc
}

// Border painting
switch (style)
{
    case BeepControlStyle.Material3:
    case BeepControlStyle.MaterialYou:
        MaterialBorderPainter.Paint(g, bounds, isFocused, path, style, theme, useThemeColors);
        break;
    // ... etc
}

// Text painting
switch (style)
{
    case BeepControlStyle.Material3:
    case BeepControlStyle.MaterialYou:
        MaterialTextPainter.Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        break;
    // ... etc
}

// Button painting
switch (style)
{
    case BeepControlStyle.Material3:
    case BeepControlStyle.MaterialYou:
        MaterialButtonPainter.PaintButtons(g, upRect, downRect, isFocused, style, theme, useThemeColors);
        break;
    // ... etc
}
```

---

## Style to Painter Mapping

### Material Family
- **Styles**: Material3, MaterialYou
- **Background**: MaterialBackgroundPainter (elevation highlight)
- **Border**: MaterialBorderPainter (only if outlined)
- **Text**: MaterialTextPainter (Roboto, bold on focus)
- **Button**: MaterialButtonPainter (filled, 28px radius)

### Apple Family
- **Styles**: iOS15, MacOSBigSur
- **Background**: iOSBackgroundPainter / MacOSBackgroundPainter
- **Border**: AppleBorderPainter (subtle outlined)
- **Text**: AppleTextPainter (SF Pro, negative tracking)
- **Button**: AppleButtonPainter (outlined, 6px radius)

### Fluent Family
- **Styles**: Fluent2, Windows11Mica
- **Background**: SolidBackgroundPainter / MicaBackgroundPainter
- **Border**: FluentBorderPainter (accent bar on focus)
- **Text**: StandardTextPainter (Segoe UI Variable)
- **Button**: FluentButtonPainter (filled, 4px radius)

### Minimal Family
- **Styles**: Minimal, NotionMinimal, VercelClean
- **Background**: SolidBackgroundPainter
- **Border**: MinimalBorderPainter (always present)
- **Text**: StandardTextPainter (Inter font)
- **Button**: MinimalButtonPainter (outlined, 4px radius)

### Effect Family
- **Styles**: Neumorphism, GlassAcrylic, DarkGlow, GradientModern
- **Background**: NeumorphismBackgroundPainter / GlassBackgroundPainter / GlowBackgroundPainter / GradientBackgroundPainter
- **Border**: EffectBorderPainter (glow on focus for DarkGlow)
- **Text**: MonospaceTextPainter (DarkGlow) / StandardTextPainter (others)
- **Button**: StandardButtonPainter (filled + bordered)

### Web Framework Family
- **Styles**: Bootstrap, Tailwind, Discord, Stripe, Figma, AntDesign, Chakra
- **Background**: WebFrameworkBackgroundPainter (various methods)
- **Border**: WebFrameworkBorderPainter (Tailwind ring effect)
- **Text**: StandardTextPainter (various fonts)
- **Button**: StandardButtonPainter (filled + bordered, 6px radius)

---

## Summary Statistics
- **Total Painters**: 25 classes
- **Total Styles Supported**: 21 design systems
- **Folders**: 4 operation types (Background, Border, Text, Button)
- **Helpers**: 5 categories (Colors, Spacing, Borders, Shadows, Typography)
- **Total Helper Methods**: 34 methods across 5 helper classes
- **Architecture**: Static classes, no instances, theme-aware, operation-focused separation

---

## Next Steps
To complete the refactoring:
1. Update `BeepStyling.cs` to delegate border painting to BorderPainters
2. Update `BeepStyling.cs` to delegate text painting to TextPainters
3. Update `BeepStyling.cs` to delegate button painting to ButtonPainters
4. Remove all embedded private methods from `BeepStyling.cs`
5. Test all 21 styles with all painters
6. Document any style-specific edge cases
