# BeepiFormPro Painters

This folder contains 32 specialized painter classes that provide unique visual styles for `BeepiFormPro` forms.

## Overview

The painter pattern enables complete separation of rendering logic from form behavior. Each painter implements the `IFormPainter` interface and provides distinct visual styles for:
- Form background with custom effects (gradients, textures, shadows)
- Caption bar with style-specific decorations
- Window borders with unique shapes and effects
- System buttons (Close, Maximize, Minimize) with custom rendering
- Theme/Style toggle buttons (when enabled)
- Custom button shapes and visual identities per painter

## Architecture

### Core Interfaces

All painters implement these three interfaces:

#### 1. IFormPainter (Core Painting)
```csharp
public interface IFormPainter
{
    void PaintBackground(Graphics g, BeepiFormPro owner);
    void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect);
    void PaintBorders(Graphics g, BeepiFormPro owner);
    void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect);
    
    ShadowEffect GetShadowEffect(BeepiFormPro owner);
    CornerRadius GetCornerRadius(BeepiFormPro owner);
    AntiAliasMode GetAntiAliasMode(BeepiFormPro owner);
    bool SupportsAnimations { get; }
}
```

#### 2. IFormPainterMetricsProvider (Layout & Hit Areas)
```csharp
public interface IFormPainterMetricsProvider
{
    FormPainterMetrics GetMetrics(BeepiFormPro owner);
    void CalculateLayoutAndHitAreas(BeepiFormPro owner);
}
```

#### 3. IFormNonClientPainter (Optional - Non-Client Area)
```csharp
public interface IFormNonClientPainter
{
    void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness);
}
```

## ⚠️ CRITICAL: CalculateLayoutAndHitAreas Implementation

**This method is THE MOST IMPORTANT for functionality.** It:
1. ✅ Calculates layout positions for all UI elements
2. ✅ Registers hit areas for interactive buttons
3. ✅ Makes buttons clickable and responsive
4. ✅ Ensures proper spacing to prevent overlap
5. ✅ Supports theme/style button visibility toggling

### Standard Implementation Pattern

```csharp
public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
{
    var layout = new PainterLayoutInfo();
    var metrics = GetMetrics(owner);
    
    int captionHeight = Math.Max(metrics.CaptionHeight, 
        (int)(owner.Font.Height * metrics.FontHeightMultiplier));
    owner._hits.Clear();
    
    // 1. Caption drag area
    layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
    owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
    
    int buttonWidth = metrics.ButtonWidth;
    int buttonX = owner.ClientSize.Width - buttonWidth;
    
    // 2. Standard window buttons (right to left)
    layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
    owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
    buttonX -= buttonWidth;
    
    layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
    owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
    buttonX -= buttonWidth;
    
    layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
    owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
    buttonX -= buttonWidth;
    
    // 3. Style button (if enabled)
    if (owner.ShowStyleButton)
    {
        layout.StyleButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
        owner._hits.RegisterHitArea("style", layout.StyleButtonRect, HitAreaType.Button);
        buttonX -= buttonWidth;
    }
    
    // 4. Theme button (if enabled)
    if (owner.ShowThemeButton)
    {
        layout.ThemeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
        owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
        buttonX -= buttonWidth;
    }
    
    // 5. Icon and title positioning
    int iconX = metrics.IconLeftPadding;
    int iconY = (captionHeight - metrics.IconSize) / 2;
    layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
    if (owner.ShowIcon && owner.Icon != null)
    {
        owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
    }
    
    int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
    int titleWidth = buttonX - titleX - metrics.ButtonSpacing;
    layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
    
    owner.CurrentLayout = layout;
}
```

## 🎨 Complete Painter Catalog (32 Painters)

All painters implement `IFormPainter`, `IFormPainterMetricsProvider`, and `IFormNonClientPainter`.

| # | Painter Name | Button Style | Location | Visual Identity | Status |
|---|---|---|---|---|---|
| 1 | **ArcLinuxFormPainter** | Hexagons (6-sided) | RIGHT | Arch Linux-inspired geometric design | ✅ |
| 2 | **BrutalistFormPainter** | Sharp rectangles | RIGHT | Neo-brutalist, no anti-aliasing, thick borders | ✅ |
| 3 | **CartoonFormPainter** | Comic style | RIGHT | Bold outlines, comic book effects | ✅ |
| 4 | **ChatBubbleFormPainter** | Speech bubbles | RIGHT | Chat/messaging aesthetic | ✅ |
| 5 | **CustomFormPainter** | Customizable | RIGHT | Base painter for custom implementations | ✅ |
| 6 | **CyberpunkFormPainter** | Neon glow (multi-layer) | RIGHT | Cyberpunk with scan lines, intense neon | ✅ |
| 7 | **DraculaFormPainter** | Vampire fangs (curved triangles) | RIGHT | Dracula theme colors, gothic style | ✅ |
| 8 | **FluentFormPainter** | Acrylic reveal | RIGHT | Microsoft Fluent Design with acrylic effects | ✅ |
| 9 | **GlassFormPainter** | Glass transparency | RIGHT | Classic glass/aero effect | ✅ |
| 10 | **GlassmorphismFormPainter** | Frosted circles with hatching | RIGHT | Modern glassmorphism with texture | ✅ |
| 11 | **GNOMEFormPainter** | Pill-shaped | RIGHT | GNOME/Adwaita desktop environment style | ✅ |
| 12 | **GruvBoxFormPainter** | 3D beveled Win95-style | RIGHT | Retro Gruvbox color scheme | ✅ |
| 13 | **HolographicFormPainter** | Rainbow chevrons/arrows | RIGHT | Iridescent holographic effects | ✅ |
| 14 | **iOSFormPainter** | Traffic light circles | LEFT | iOS-style colored circles (red/yellow/green) | ✅ |
| 15 | **KDEFormPainter** | Breeze gradient | RIGHT | KDE Plasma desktop style | ✅ |
| 16 | **MacOSFormPainter** | Traffic lights with 3D | LEFT | macOS Big Sur style with highlights | ✅ |
| 17 | **MaterialFormPainter** | Material circles | RIGHT | Google Material Design 3 with elevation | ✅ |
| 18 | **Metro2FormPainter** | Modern Metro | RIGHT | Updated Windows Metro style | ✅ |
| 19 | **MetroFormPainter** | Classic Metro | RIGHT | Original Windows Metro design | ✅ |
| 20 | **MinimalFormPainter** | Minimalist | RIGHT | Clean, minimal design | ✅ |
| 21 | **ModernFormPainter** | Beveled rectangles | RIGHT | Contemporary beveled buttons | ✅ |
| 22 | **NeoMorphismFormPainter** | Embossed rectangles | RIGHT | Soft UI with dual shadows (light/dark) | ✅ |
| 23 | **NeonFormPainter** | Star-shaped (5-pointed) | RIGHT | Vibrant neon with multi-color glow | ✅ |
| 24 | **NordFormPainter** | Rounded triangles | RIGHT | Nord theme with frost gradients | ✅ |
| 25 | **NordicFormPainter** | Minimalist rectangles | RIGHT | Scandinavian design aesthetic | ✅ |
| 26 | **OneDarkFormPainter** | Octagons (8-sided) | RIGHT | Atom One Dark editor theme | ✅ |
| 27 | **PaperFormPainter** | Double-border circles | RIGHT | Material paper design with rings | ✅ |
| 28 | **RetroFormPainter** | 3D beveled (Win95) | RIGHT | 80s/90s retro computing with scan lines | ✅ |
| 29 | **SolarizedFormPainter** | Diamonds (rotated squares) | RIGHT | Solarized color scheme | ✅ |
| 30 | **TokyoFormPainter** | Neon cross/plus shapes | RIGHT | Tokyo Night theme with glow | ✅ |
| 31 | **UbuntuFormPainter** | Unity pill-shaped | LEFT | Ubuntu Unity desktop style | ✅ |
| 32 | **Windows11FormPainter** | Square Mica | RIGHT | Windows 11 with Mica effect | ✅ |
| 33 | **TerminalFormPainter** | ASCII rectangles | RIGHT | Console/Terminal with monospace and scanlines | ✅ |

## 🎯 BeepFormStyle Enum Mapping

The `BeepFormStyle` enum (in `BeepFormStyle.cs`) maps to painter implementations:

| BeepFormStyle | Painter Implementation | Notes |
|---|---|---|
| `Classic` | Uses system default | No custom painter |
| `Modern` | `ModernFormPainter` | Contemporary design |
| `Metro` | `MetroFormPainter` | Windows Metro |
| `Glass` | `GlassFormPainter` | Aero glass effect |
| `Office` | `CustomFormPainter` or theme-based | Professional look |
| `ModernDark` | Theme variant of `ModernFormPainter` | Dark mode |
| `Material` | `MaterialFormPainter` | Material Design 3 |
| `Minimal` | `MinimalFormPainter` | Minimalist |
| `Gnome` | `GNOMEFormPainter` | GNOME/Adwaita |
| `Kde` | `KDEFormPainter` | KDE Plasma |
| `Cinnamon` | Could use `GNOMEFormPainter` variant | Not yet implemented |
| `Elementary` | Could use theme-based | Not yet implemented |
| `Fluent` | `FluentFormPainter` | Microsoft Fluent |
| `NeoBrutalist` | `BrutalistFormPainter` | Neo-brutalist |
| `Neon` | `NeonFormPainter` | Vibrant neon |
| `Retro` | `RetroFormPainter` | 80s/90s retro |
| `Gaming` | `CyberpunkFormPainter` or custom | Gaming aesthetic |
| `Corporate` | Theme-based | Professional |
| `Artistic` | `CartoonFormPainter` or custom | Creative |
| `HighContrast` | Theme-based | Accessibility |
| `Soft` | `NeoMorphismFormPainter` | Soft UI |
| `Industrial` | `BrutalistFormPainter` | Industrial design |
| `Windows` | `Windows11FormPainter` | Windows 11 |
| `Terminal` | `TerminalFormPainter` | Console/terminal aesthetic |
| `Custom` | `CustomFormPainter` | User-defined |

## 🔧 Usage

### Basic Setup
```csharp
// Enable theme/style buttons
form.ShowThemeButton = true;
form.ShowStyleButton = true;

// Set painter
form.SetPainter(new FluentFormPainter());
```

### Custom Button Behavior
The painter automatically registers hit areas. Button clicks are handled by `BeepiFormPro`:
- **Theme Button**: Cycles through available themes
- **Style Button**: Cycles through available painters/styles

## 📋 Painter Responsibilities

### PaintBackground
- Paints base background color/gradient
- NO clipping applied at this stage
- Should cover entire client rectangle

### PaintCaption
- Paints caption bar decorations
- Paints custom button shapes (unique to each painter)
- Renders title text
- Calls `owner._iconRegion?.OnPaint` for icon

### PaintBorders
- Paints form borders with unique styles
- Uses `owner.BorderShape` for consistent shape

### PaintWithEffects
- **Orchestrates** the entire painting process
- Applies shadows, clipping, and effects
- Calls other paint methods in correct order

### CalculateLayoutAndHitAreas
- **MOST CRITICAL METHOD**
- Calculates all button positions
- Registers hit areas for interactivity
- Determines title text width

## ✅ Implementation Status

**Current Status**: 33/33 painters fully implemented (100%)

All painters:
- ✅ Implement all required interfaces
- ✅ Have proper `CalculateLayoutAndHitAreas` implementation
- ✅ Register theme/style buttons correctly
- ✅ Have unique visual identities
- ✅ Support hit area testing
- ✅ Handle DPI scaling via metrics

## 🧪 Testing Checklist

For each painter:
- [ ] Buttons are clickable (hit areas work)
- [ ] Theme/style buttons appear when enabled
- [ ] Title text doesn't overlap buttons
- [ ] Buttons render correctly at different DPI
- [ ] Form resizes properly
- [ ] Maximize/restore state works
- [ ] Unique button style renders correctly
- [ ] Hover states work (if implemented)
- [ ] Border shape is correct
- [ ] Shadow effects render properly

## 📚 Related Files

- `BeepiFormPro.Core.cs` - Core form properties
- `BeepiFormPro.Drawing.cs` - Main OnPaint orchestration
- `BeepiFormPro.Managers.cs` - Layout management
- `FormPainterMetrics.cs` - Sizing metrics
- `PainterLayoutInfo.cs` - Layout structure
- `IFormPainter.cs` - Core interface definitions
- `BeepFormStyle.cs` - Style enumeration
- `skinplan.md` - Architecture and development plan

---

**Last Updated**: 2025-10-20  
**Version**: 2.1  
**Progress**: 100% Complete (33/33 painters)
