# BeepiForm (Modern WinForms Host)

BeepiForm is a borderless, theme-aware WinForms `Form` that composes a set of helpers to deliver a modern window: rounded corners, custom caption bar, shadow/glow, Windows backdrops (Mica/Acrylic), and snap hints.

This guide shows how to use it, customize the caption bar, switch styles, and load presets with automatic light/dark selection.


## Highlights
- Fully owner-drawn window (shapes, borders, shadow/glow)
- **Unified Style System**: Single `BeepFormStyle` enum controls both form appearance AND caption renderer
- Custom caption bar with: logo, title, system buttons, and optional Theme/Style buttons
- Caption renderers automatically selected based on `BeepFormStyle`
- Style metrics mapped per `BeepFormStyle` (radius, thickness, shadow, glow, caption height)
- Presets loader for macOS/GNOME/KDE/Cinnamon/Elementary (light/dark variants)
- Optional auto-apply preset when changing styles
- Windows 10/11 effects: Acrylic, Mica, System Backdrop, Blur
- Overlay painter registry for snap hints
- DPI-aware layout and hit testing
- Theme integration via `BeepThemesManager` (caption gradient, typography, colors)
- 22 distinct visual styles with matching caption renderers


## Unified Style System

**IMPORTANT CHANGE**: `CaptionRendererKind` is now deprecated. Use `BeepFormStyle` for everything.

BeepiForm now uses a **single enum** (`BeepFormStyle`) that controls:
1. Form appearance (borders, shadows, glow, colors)
2. Caption bar renderer (automatically selected)
3. Visual metrics (radius, thickness, caption height)

## Caption renderers and layouts

Caption renderers are **automatically selected** based on `BeepFormStyle`:

### System-Inspired Styles (with matching caption renderers)
- **Modern, Classic, Metro, Glass, Office**: Windows-style caption (right-aligned buttons, left title)
- **Material**: macOS-like caption (left-aligned colored circles, centered title)
- **Gnome**: GNOME/Adwaita-style caption (flat right buttons, centered title, no gradient)
- **Kde**: KDE/Breeze-style caption (right buttons with hover fills, gradient enabled)
- **Cinnamon**: Cinnamon-style caption (larger spacing, right buttons, gradient enabled)
- **Elementary**: Elementary-style caption (thin glyphs, generous spacing, right buttons)

### Modern Visual Styles (with specialized caption renderers)  
- **Neon**: Vibrant cyan/pink neon glow effects with glowing button outlines
- **Retro**: 80s-inspired circular gradient buttons with colorful accents
- **Gaming**: Angular cut-corner buttons with neon green/red accents
- **Corporate**: Professional gray buttons with simple hover effects
- **Artistic**: Creative curved buttons with rainbow color schemes
- **HighContrast**: Black and white accessibility-focused design
- **Soft**: Rounded buttons with gentle blue colors and soft gradients
- **Industrial**: Metallic gradient buttons with thick borders

### Classic Styles (Windows caption renderer)
- **Fluent**: Enhanced shadow/glow with Windows caption
- **NeoBrutalist**: Bold thick borders with Windows caption
- **ModernDark**: Dark theme with Windows caption

Title alignment is centered for `Gnome` and `Material` styles; left-aligned otherwise.

Caption background uses theme gradient tokens: `AppBarGradiantStartColor`, `AppBarGradiantEndColor`, `AppBarGradiantDirection`.


## Public API (selected)

**Unified Style Control**:
- `BeepFormStyle FormStyle` - **Single property controls everything** (form + caption)

Legacy (Deprecated):
- `CaptionRendererKind CaptionRenderer` - **Deprecated: Use FormStyle instead**

Appearance and Style:
- `string Title`
- `int BorderRadius`, `int BorderThickness`, `Color BorderColor`
- `Color ShadowColor`, `int ShadowDepth`, `bool EnableGlow`, `Color GlowColor`, `float GlowSpread`
- Auto behavior:
  - `bool AutoPickDarkPresets` (default true)
  - `bool AutoApplyRendererPreset` (default true)
- Presets: `BeepFormStylePresets StylePresets`, `void ApplyPreset(string key)`

Caption Bar:
- `bool ShowCaptionBar`, `int CaptionHeight`, `bool ShowSystemButtons`, `bool EnableCaptionGradient`
- Logo: `bool ShowLogo`, `string LogoImagePath`, `Size LogoSize`, `Padding LogoMargin` (alias: `ShowIconInCaption`)
- Caption extras:
  - `bool ShowThemeButton`, `bool ShowStyleButton`
  - `string ThemeButtonIconPath`, `string StyleButtonIconPath`
- Caption title overrides (runtime):
  - `Font? TitleFontOverride`, `Color? TitleForeColorOverride`

Backdrops and Effects:
- `BackdropType Backdrop` (None, Mica, Acrylic, Tabbed, Transient, Blur)
- `bool EnableAcrylicForGlass`, `bool EnableMicaBackdrop`, `bool UseImmersiveDarkMode`

Snap Hints:
- `bool ShowSnapHints`

DPI/Behavior:
- `DpiHandlingMode DpiMode` (Framework, Manual)
- `bool UseHelperInfrastructure`

Events:
- `OnFormLoad`, `OnFormShown`, `PreClose`, `OnFormClose`

Utilities:
- `ToggleMaximize()`, `AdjustControls()`, `DisplayRectangle` override


## Quick Start

```csharp
public class MyWindow : BeepiForm
{
    public MyWindow()
    {
        Title = "Welcome";
        Theme = "DefaultTheme";
        ShowCaptionBar = true;

        // Load presets once
        var samples = Path.Combine(AppContext.BaseDirectory, "Styles\\samples");
        BeepFormStylePresetsLoader.LoadFromFolder(StylePresets, samples);

        // Enable caption extras
        ShowThemeButton = true;
        ShowStyleButton = true;

        // Simply set FormStyle - caption renderer is automatically selected!
        FormStyle = BeepFormStyle.Kde;        // KDE/Breeze style with matching caption
        FormStyle = BeepFormStyle.Neon;       // Vibrant neon effects with glowing caption
        FormStyle = BeepFormStyle.Gaming;     // Angular gaming style with neon caption
        FormStyle = BeepFormStyle.Material;   // macOS-like with circular caption buttons
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        AdjustControls();
    }
}
```


## Recipes (ready-to-use)

**Simple - Just set FormStyle! Caption renderer is automatic.**

### System-Inspired Styles
- **macOS**: `FormStyle = BeepFormStyle.Material` (auto-applies macOS-like caption + presets)
- **GNOME**: `FormStyle = BeepFormStyle.Gnome` (auto-applies GNOME caption + presets)
- **KDE**: `FormStyle = BeepFormStyle.Kde` (auto-applies KDE caption + presets)
- **Cinnamon**: `FormStyle = BeepFormStyle.Cinnamon` (auto-applies Cinnamon caption + presets)
- **Elementary**: `FormStyle = BeepFormStyle.Elementary` (auto-applies Elementary caption + presets)

### Modern Visual Styles
- **Gaming**: `FormStyle = BeepFormStyle.Gaming` - Angular design with neon green caption effects
- **Neon**: `FormStyle = BeepFormStyle.Neon` - Dark background with vibrant cyan glowing caption
- **Industrial**: `FormStyle = BeepFormStyle.Industrial` - Metallic gray with gradient caption buttons
- **Retro**: `FormStyle = BeepFormStyle.Retro` - 80s inspired with circular gradient caption buttons
- **Corporate**: `FormStyle = BeepFormStyle.Corporate` - Clean professional gray design
- **Artistic**: `FormStyle = BeepFormStyle.Artistic` - Creative circular caption buttons with rainbow colors
- **High Contrast**: `FormStyle = BeepFormStyle.HighContrast` - Black/white accessibility design
- **Soft**: `FormStyle = BeepFormStyle.Soft` - Gentle rounded design with blue caption accents

### Classic Styles
- **Modern**: `FormStyle = BeepFormStyle.Modern` - Default modern Windows style
- **Fluent**: `FormStyle = BeepFormStyle.Fluent` - Enhanced shadow and glow effects
- **NeoBrutalist**: `FormStyle = BeepFormStyle.NeoBrutalist` - Bold thick borders
- **Glass**: `FormStyle = BeepFormStyle.Glass` - Transparent glass effects

Tip: When your theme is dark and `AutoPickDarkPresets = true`, the engine will try the `.dark` key automatically.


## Migration from Dual-Enum System

**Old way (deprecated)**:
```csharp
// ? Old - had to set both properties
CaptionRenderer = CaptionRendererKind.Gaming;
FormStyle = BeepFormStyle.Gaming;
```

**New way (recommended)**:
```csharp
// ? New - single property controls everything
FormStyle = BeepFormStyle.Gaming;  // Caption renderer automatically selected!
```

The `CaptionRenderer` property still exists for backward compatibility but is marked as `[Obsolete]`.


## Designer support
- `FormStyle` property provides access to all 22 visual styles in the designer
- Each style automatically selects the appropriate caption renderer
- TypeConverter shows friendly names for all styles
- `CaptionRenderer` property still appears but shows deprecation warning


## Recent Changes
- **?? BREAKING: Unified Style System**: Consolidated `CaptionRendererKind` into `BeepFormStyle`. Single enum now controls everything.
- **Ribbon Removed**: All ribbon-related functionality has been removed to simplify architecture.
- **Enhanced Border Handling**: Fixed issues where style changes didn't properly update border thickness.
- **Automatic Caption Selection**: Caption renderers are now automatically selected based on `FormStyle`.
- **Simplified API**: No more need to coordinate between two separate enums.
- **22 Total Styles**: Complete coverage of all visual variants with matching caption renderers.


## Known Limits
- WinForms is Windows-only. "macOS/Linux look" here is skinning, not native windows.
- Some backdrops require Windows 11. Unsupported calls are no-ops at runtime.
- `CaptionRenderer` property is deprecated but maintained for compatibility.
