# BeepiForm (Modern WinForms Host)

BeepiForm is a borderless, theme-aware WinForms `Form` that composes several small helpers to deliver a modern window experience: rounded corners, custom caption bar, shadow/glow, animations, Windows backdrops (Mica/Acrylic), ribbon placeholder, and snap hints. It is the base window used by Beep WinForms views.

This document explains how it works, how to use it, and how to customize/skin it (including recipes to emulate macOS and popular Linux desktop looks).


## Highlights
- Fully owner-drawn window with rounded corners and configurable border thickness/color
- Custom caption bar with optional logo and system buttons
- Shadow and glow painters with style presets (Modern, Metro, Glass, Office, Material, Minimal, Classic, Custom)
- Windows 10/11 effects: Acrylic, Mica, System Backdrop, Blur (with safe fallbacks)
- Overlay painter registry to draw extra layers (ribbon placeholder, snap hints, custom visuals)
- DPI-aware layout and hit testing for resize and drag
- Theme integration via `BeepThemesManager`


## Files and Responsibilities
- `BeepiForm.cs`: The main form class. Orchestrates helpers, exposes public properties/events, handles painting and Win32 messages.
- `Styles/BeepFormStyleMetrics.cs`: Defines metrics for each `BeepFormStyle` and the defaults map.
- `Styles/BeepFormStylePresets.cs`: Serializable map of named presets you can load/save or apply at runtime via `ApplyPreset(key)`.
- `Styles/BeepFormStylePresetsLoader.cs`: Helper to load/merge presets from JSON (folder or file).
- `Styles/samples/*.json`: Sample preset collections for macOS, GNOME, KDE.
- `Enums/BeepFormStyle.cs`: Built-in style names.
- `Helpers/FormRegionHelper.cs`: Creates and caches the rounded window region.
- `Helpers/FormOverlayPainterRegistry.cs`: Keeps a list of functions to paint overlays after the base form is rendered.
- `Helpers/FormCaptionBarHelper.cs`: Renders the caption bar (title, optional logo, system buttons) and handles their hit testing. Hosts caption renderer strategy.
- `Helpers/FormHitTestHelper.cs`: Centralizes `WM_NCHITTEST` logic for borders/corners and the caption drag zone.
- `Helpers/FormBackgroundPainter.cs`: Gradient/texture/solid background rendering (used by other forms; BeepiForm currently paints its background inline).
- `Helpers/FormBorderPainter.cs`: Utilities to draw borders respecting radius and thickness (used by other forms; BeepiForm paints borders inline).
- `Helpers/FormAcrylicHelper.cs`: Encapsulates Acrylic/Mica/Backdrop interop with safe fallbacks.
- `Helpers/FormAnimationHelper.cs`: General animation utilities (fade/slide/scale).
- `Caption/ICaptionRenderer.cs`: Strategy interface for caption visuals and hit tests.
- `Caption/CaptionRendererKind.cs`: Enum to pick `Windows`, `MacLike`, `Gnome`, `Kde`.
- `Caption/Renderers/*Renderer.cs`: Concrete implementations.


## Core Concepts

### Helper Composition
BeepiForm composes helpers at runtime (design-time-safe guards are present). Key instances:
- `FormRegionHelper` handles rounded regions and re-applies them on size/state changes.
- `FormCaptionBarHelper` draws the title area and system buttons and exposes logo options.
- `FormHitTestHelper` performs non-client hit testing for resize/drag.
- `FormOverlayPainterRegistry` paints optional overlays such as the ribbon placeholder and snap hints.
- `FormAcrylicHelper` can enable Acrylic/Mica/Tabbed/Transient backdrops on supported Windows versions.

Painting order (simplified):
1) Shadow + Glow
2) Fill window shape (BackColor)
3) Border stroke
4) Overlays (caption, ribbon placeholder, snap hints, etc.)


### Layout and Content Area
The content area excludes extra non-client space (caption/ribbon). Use:
- `DisplayRectangle` or `GetAdjustedClientRectangle()` — returns the usable content rectangle.
- Controls with `DockStyle.Fill` will be sized to this area when `AdjustControls()` is called.


## Public API (selected)

Appearance and Style:
- `Title` (also sets the window `Text`)
- `BorderRadius`, `BorderThickness`, `BorderColor`
- `FormStyle` (`BeepFormStyle`): Modern, Metro, Glass, Office, ModernDark, Material, Minimal, Classic, Custom
- `StylePresets`: load/apply presets at runtime (`ApplyPreset(key)`)
- `CaptionRenderer`: `Windows`, `MacLike`, `Gnome`, `Kde`

Caption Bar:
- `ShowCaptionBar`, `CaptionHeight`, `ShowSystemButtons`, `EnableCaptionGradient`
- Logo: `ShowLogo`, `LogoImagePath`, `LogoSize`, `LogoMargin`, alias `ShowIconInCaption`

Backdrops and Effects:
- `Backdrop` (None, Mica, Acrylic, Tabbed, Transient, Blur)
- `EnableAcrylicForGlass`, `EnableMicaBackdrop`, `UseImmersiveDarkMode`
- Shadow/Glow tuning: `ShadowColor`, `ShadowDepth`, `EnableGlow`, `GlowColor`, `GlowSpread`

Ribbon and Snap Hints:
- `ShowRibbonPlaceholder`, `RibbonHeight`, `ShowQuickAccess`
- `ShowSnapHints`

DPI/Behavior:
- `DpiMode` (Framework, Manual)
- `UseHelperInfrastructure` (keep true to use helper pipeline)

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
        Title = "Welcome to StarLand";
        Theme = "DefaultTheme"; // any theme available in BeepThemesManager
        FormStyle = BeepFormStyle.Modern; // try Material, Office, Glass, etc.
        BorderRadius = 12;
        ShowCaptionBar = true;
        CaptionHeight = 40;
        ShowLogo = true;
        LogoImagePath = "Assets\\logo.svg";

        // Enable Windows 11 Mica (safe no-op on older Windows)
        Backdrop = Effects.BackdropType.Mica;

        // Optional ribbon placeholder
        ShowRibbonPlaceholder = true;
        RibbonHeight = 88;

        // Place your main control to fill DisplayRectangle later
        var content = new Panel { BackColor = Color.White, Dock = DockStyle.Fill };
        Controls.Add(content);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        AdjustControls(); // sizes children to the content area beneath caption/ribbon
    }
}
```


## Presets and Custom Metrics
You can save/load named presets that map to `BeepFormStyleMetrics`:

```csharp
// Load sample presets (macOS/GNOME/KDE) at startup
var samples = Path.Combine(AppContext.BaseDirectory, "Styles\\samples");
BeepFormStylePresetsLoader.LoadFromFolder(StylePresets, samples);

// Or a single file
BeepFormStylePresetsLoader.LoadFromFile(StylePresets, Path.Combine(samples, "beepi-presets-macos.json"));

// Apply later
ApplyPreset("macos.light");
```

`BeepFormStyleMetrics` fields: `BorderRadius`, `BorderThickness`, `ShadowDepth`, `EnableGlow`, `GlowSpread`, `CaptionHeight`, `RibbonHeight`, `CaptionButtonSize`.


## Backdrops (Windows 10/11)
- `Backdrop = BackdropType.Mica` enables Mica (Windows 11, safe no-op elsewhere)
- `Backdrop = BackdropType.Acrylic` enables Acrylic blur
- `Backdrop = BackdropType.Blur` for classic blur behind
- `Backdrop = BackdropType.Tabbed` or `Transient` map to Win11 system backdrop types

BeepiForm guards calls at runtime; unsupported OS will simply keep a solid background.


## Extending the Look (Skins/Themes/Overlays)

BeepiForm supports overlays via an internal registry used for caption, ribbon placeholder, and snap hints. For project-level customization you have three options:

1) Subclass BeepiForm and draw custom visuals in `OnPaint` after calling `base.OnPaint(e)`. Keep shapes inside `DisplayRectangle` or the caption area.
2) Contribute a new helper/overlay to the library (preferred) by adding it to `FormOverlayPainterRegistry` in the constructor (similar to how ribbon/snap hints are registered).
3) Create a custom `BeepFormStyle` metrics preset and a theme with your brand colors (no code changes).


## Recipes: Emulate Popular OS Styles

- macOS: preset `macos.light` + `CaptionRenderer = MacLike` + `FormStyle = Material`.
- GNOME: preset `gnome.adwaita.light` + `CaptionRenderer = Gnome` + `FormStyle = Minimal`.
- KDE: preset `kde.breeze.light` + `CaptionRenderer = Kde` + `FormStyle = Modern`.

You can supply matching Beep themes in your app (colors/typography). Example:

```csharp
Theme = "AdwaitaLight";     // a Beep theme you define in your app
FormStyle = BeepFormStyle.Minimal;
CaptionRenderer = CaptionRendererKind.Gnome;
ApplyPreset("gnome.adwaita.light");
```


## Adding Your Own Caption Renderer (Advanced)
The `FormCaptionBarHelper` uses a strategy interface:

- Implement `ICaptionRenderer` in `Caption/Renderers/MyRenderer.cs`.
- Add a value to `CaptionRendererKind` and wire it in `FormCaptionBarHelper.SetRenderer`.
- Your renderer decides layout/painting/hit tests.


## Known Limits and Notes
- WinForms is Windows-only. “macOS/Linux look” here means skinning, not native windows.
- Some backdrop effects require Windows 11. Guards are in place; unsupported calls are no-ops.
- Designer surface skips heavy interop/effects by design (`InDesignHost`).


## Minimal Example

```csharp
ApplicationConfiguration.Initialize();
var form = new BeepiForm
{
    Title = "My App",
    Theme = "DefaultTheme",
    FormStyle = BeepFormStyle.Modern,
    BorderRadius = 10,
    Backdrop = Effects.BackdropType.Mica,
    ShowCaptionBar = true,
    ShowLogo = true,
    LogoImagePath = "Assets\\logo.svg",
    CaptionRenderer = CaptionRendererKind.Windows
};

// Load presets from samples and apply one
BeepFormStylePresetsLoader.LoadFromFolder(form.StylePresets, Path.Combine(AppContext.BaseDirectory, "Styles\\samples"));
form.ApplyPreset("kde.breeze.light");

Application.Run(form);
