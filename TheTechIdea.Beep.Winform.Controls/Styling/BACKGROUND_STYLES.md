# Background Painting Styles

Comprehensive documentation for how each design system renders backgrounds in the Beep WinForm styling system.

## Style-Specific Background Implementations

### 1. **Material3** & **MaterialYou**
**Method:** `PaintMaterialBackground()`
- **Base:** Solid color from StyleColors.GetBackground()
- **Effect:** Top highlight line (2px, 15 alpha white)
- **Purpose:** Creates material elevation appearance
- **Visual:** Clean with subtle depth indicator

---

### 2. **iOS15**
**Method:** `PaintiOSBackground()`
- **Base:** Solid color
- **Overlay:** Subtle translucent white (10 alpha)
- **Shadow:** Bottom 1px shadow (20 alpha black)
- **Purpose:** iOS blur/translucent effect
- **Visual:** Light, airy appearance with depth

---

### 3. **Fluent2**
**Method:** `PaintSolidBackground()`
- **Base:** Solid color from StyleColors.GetBackground()
- **Effect:** None (clean and simple)
- **Purpose:** Microsoft Fluent clean design
- **Visual:** Flat, professional

---

### 4. **Minimal**
**Method:** `PaintSolidBackground()`
- **Base:** Solid color (pure white or black)
- **Effect:** None
- **Purpose:** Ultra-minimal design
- **Visual:** Completely flat, no effects

---

### 5. **AntDesign**
**Method:** `PaintSolidBackground()`
- **Base:** Solid color
- **Effect:** None
- **Purpose:** Clean, systematic design
- **Visual:** Simple and functional

---

### 6. **Windows11Mica**
**Method:** `PaintMicaBackground()`
- **Base:** Solid color
- **Gradient:** Very subtle vertical gradient (8 alpha white to 4 alpha black)
- **Purpose:** Mica material effect (noise texture simplified)
- **Visual:** Sophisticated, modern Windows 11 look

---

### 7. **MacOSBigSur**
**Method:** `PaintMacOSBackground()`
- **Base:** Solid color
- **Gradient:** Subtle vertical gradient (12 alpha white to 5 alpha black)
- **Purpose:** macOS system appearance
- **Visual:** Polished, depth with subtle gradient

---

### 8. **ChakraUI**
**Method:** `PaintSolidBackground()`
- **Base:** Solid color
- **Effect:** None
- **Purpose:** Chakra UI clean design
- **Visual:** Simple, accessible

---

### 9. **TailwindCard**
**Method:** `PaintTailwindBackground()`
- **Base:** Solid color
- **Ring:** Subtle 1px outline (8 alpha black)
- **Purpose:** Tailwind ring utility effect
- **Visual:** Clean with subtle definition

---

### 10. **NotionMinimal**
**Method:** `PaintSolidBackground()`
- **Base:** Solid color
- **Effect:** None
- **Purpose:** Notion clean aesthetic
- **Visual:** Minimal, content-focused

---

### 11. **VercelClean**
**Method:** `PaintSolidBackground()`
- **Base:** Solid color
- **Effect:** None
- **Purpose:** Vercel clean design
- **Visual:** Ultra-clean, modern

---

### 12. **StripeDashboard**
**Method:** `PaintStripeBackground()`
- **Base:** Solid color
- **Gradient:** Very subtle vertical gradient (3 alpha white to 3 alpha black)
- **Purpose:** Stripe polished dashboard look
- **Visual:** Professional with subtle polish

---

### 13. **DarkGlow**
**Method:** `PaintGlowBackground()`
- **Base:** Dark solid color
- **Glow:** Inner glow effect with 3 progressive rings
- **Alpha:** Progressive 30 alpha fade using primary color
- **Purpose:** Neon/cyberpunk glow effect
- **Visual:** Dark with neon inner glow

---

### 14. **DiscordStyle**
**Method:** `PaintDiscordBackground()`
- **Base:** Solid dark color (Blurple theme)
- **Effect:** None (base state)
- **Purpose:** Discord dark theme
- **Visual:** Clean dark, simple

---

### 15. **GradientModern**
**Method:** `PaintGradientBackground()`
- **Gradient:** Vertical linear gradient
- **Colors:** Primary color to Secondary color
- **Purpose:** Modern gradient aesthetic
- **Visual:** Smooth color transition

---

### 16. **GlassAcrylic**
**Method:** `PaintGlassBackground()`
- **Base:** Semi-transparent white (180 alpha)
- **Gradient:** Subtle vertical gradient (60 alpha to 20 alpha white)
- **Border:** Border highlight (100 alpha white, 1px)
- **Purpose:** Frosted glass/glassmorphism effect
- **Visual:** Translucent, frosted glass appearance

---

### 17. **Neumorphism**
**Method:** `PaintNeumorphismBackground()`
- **Base:** Solid color
- **Gradient:** Subtle light-to-dark gradient on top half
- **Effect:** Light edge to dark edge (5% variation)
- **Purpose:** Soft embossed 3D effect
- **Visual:** Soft, raised appearance

---

### 18. **Bootstrap**
**Method:** `PaintBootstrapBackground()`
- **Base:** Solid color
- **Effect:** None
- **Purpose:** Bootstrap framework simplicity
- **Visual:** Clean, straightforward

---

### 19. **FigmaCard**
**Method:** `PaintFigmaBackground()`
- **Base:** Solid color
- **Border:** Subtle border (0.5px) using border color
- **Purpose:** Figma card design
- **Visual:** Clean with subtle definition

---

### 20. **PillRail**
**Method:** `PaintSolidBackground()`
- **Base:** Solid color
- **Effect:** None
- **Purpose:** Pill-shaped navigation
- **Visual:** Simple, focuses on pill shape

---

## Background Painting Architecture

### Method Flow
```
PaintStyleBackground()
    ↓
Switch on BeepControlStyle
    ↓
Call specific helper method
    ↓
- PaintSolidBackground() - 9 styles
- PaintGradientBackground() - 1 style
- PaintGlassBackground() - 1 style
- PaintNeumorphismBackground() - 1 style
- PaintMaterialBackground() - 2 styles
- PaintMicaBackground() - 1 style
- PaintGlowBackground() - 1 style
- PaintiOSBackground() - 1 style
- PaintMacOSBackground() - 1 style
- PaintBootstrapBackground() - 1 style
- PaintTailwindBackground() - 1 style
- PaintDiscordBackground() - 1 style
- PaintStripeBackground() - 1 style
- PaintFigmaBackground() - 1 style
```

### Helper Methods Used
- `CreateRoundedRectangle()` - All styles
- `StyleColors.GetBackground()` - All styles
- `StyleColors.GetPrimary()` - Gradient, Glow
- `StyleColors.GetSecondary()` - Gradient
- `StyleColors.GetBorder()` - Figma
- `StyleBorders.GetRadius()` - All styles
- `StyleShadows.HasShadow()` - All styles (pre-render)
- `DrawShadow()` - Styles with shadow support

### Color Usage
- **Solid:** 9 styles use simple solid colors
- **Gradient:** 6 styles use gradient overlays or effects
- **Translucent:** 3 styles use transparency (Glass, iOS, Glow)
- **Complex:** 3 styles use multiple layers (Glass, Glow, Neumorphism)

### Performance Notes
- Solid backgrounds: ~3 operations (path, fill, shadow check)
- Gradient backgrounds: ~5-7 operations (path, base fill, gradient, shadow)
- Complex backgrounds: ~10+ operations (multiple layers, clipping)

### Extension Pattern
To add a new background style:
1. Add case to switch statement in `PaintStyleBackground()`
2. Create helper method `Paint[StyleName]Background()`
3. Use StyleColors, StyleBorders helpers for values
4. Keep method in `#region Background Painting Helpers`

## Visual Characteristics Summary

| Style | Type | Layers | Complexity | Effect |
|-------|------|--------|------------|--------|
| Material3/You | Solid+ | 2 | Low | Top highlight |
| iOS15 | Solid+ | 3 | Low | Translucent overlay |
| Fluent2 | Solid | 1 | Minimal | None |
| Minimal | Solid | 1 | Minimal | None |
| AntDesign | Solid | 1 | Minimal | None |
| Windows11Mica | Solid+ | 2 | Low | Subtle gradient |
| MacOSBigSur | Solid+ | 2 | Low | Subtle gradient |
| ChakraUI | Solid | 1 | Minimal | None |
| TailwindCard | Solid+ | 2 | Low | Ring outline |
| NotionMinimal | Solid | 1 | Minimal | None |
| VercelClean | Solid | 1 | Minimal | None |
| StripeDashboard | Solid+ | 2 | Low | Very subtle gradient |
| DarkGlow | Complex | 4+ | High | Neon glow rings |
| DiscordStyle | Solid | 1 | Minimal | None |
| GradientModern | Gradient | 1 | Medium | Vertical gradient |
| GlassAcrylic | Complex | 3 | High | Frosted glass |
| Neumorphism | Complex | 2 | Medium | Embossed effect |
| Bootstrap | Solid | 1 | Minimal | None |
| FigmaCard | Solid+ | 2 | Low | Subtle border |
| PillRail | Solid | 1 | Minimal | None |

## Usage Example

```csharp
// Paint background for current style
BeepStyling.PaintStyleBackground(g, bounds);

// Paint background for specific style
BeepStyling.PaintStyleBackground(g, bounds, BeepControlStyle.GlassAcrylic);

// All helpers automatically respect:
// - UseThemeColors flag
// - CurrentTheme if set
// - Style-specific radius from StyleBorders
// - Shadow rendering from StyleShadows
```

## Theme Integration

All background helpers support theme override:
```csharp
BeepStyling.UseThemeColors = true;
BeepStyling.CurrentTheme = myCustomTheme;

// Will use theme colors if available, fall back to style colors
BeepStyling.PaintStyleBackground(g, bounds);
```
