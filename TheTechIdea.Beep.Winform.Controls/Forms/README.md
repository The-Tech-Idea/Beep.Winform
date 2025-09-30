# BeepiForm (Modern WinForms Host)

BeepiForm is a borderless, theme-aware WinForms `Form` that composes a set of helpers to deliver a modern window: rounded corners, custom caption bar, shadow/glow, Windows backdrops (Mica/Acrylic), optional ribbon placeholder, and snap hints.

This guide shows how to use it, customize the caption bar, switch styles (Windows/macOS/GNOME/KDE/Cinnamon/Elementary), and load presets with automatic light/dark selection.


## Highlights
- Fully owner-drawn window (shapes, borders, shadow/glow)
- Custom caption bar with: logo, title, system buttons, and optional Theme/Style buttons
- Caption renderer strategy with built-in renderers:
  - Windows, MacLike, Gnome, Kde, Cinnamon, Elementary
- Style metrics mapped per `BeepFormStyle` (radius, thickness, shadow, glow, caption height)
- Presets loader for macOS/GNOME/KDE/Cinnamon/Elementary (light/dark variants)
- Optional auto-apply preset when changing caption renderer (picks .dark when your theme is dark)
- Windows 10/11 effects: Acrylic, Mica, System Backdrop, Blur
- Overlay painter registry for ribbon placeholder and snap hints
- DPI-aware layout and hit testing
- Theme integration via `BeepThemesManager` (caption gradient, typography, colors)


## Files and Responsibilities
- `Forms/BeepiForm.cs` — main form class; orchestrates helpers, exposes public API, paints and handles Win32.
- `Forms/Helpers/FormCaptionBarHelper.cs` — draws the caption bar (title, buttons, optional logo), hosts renderer strategy, Theme/Style buttons.
- `Forms/Helpers/FormRegionHelper.cs` — rounded regions; reapplies on size/state changes.
- `Forms/Helpers/FormHitTestHelper.cs` — handles non-client hit test (resize and caption drag).
- `Forms/Helpers/FormOverlayPainterRegistry.cs` — registers overlay painters (caption, ribbon, snap hints).
- `Forms/Styles/BeepFormStyleMetrics.cs` — per-style metrics (radius, thickness, shadow, glow...).
- `Forms/Styles/BeepFormStylePresets.cs` — presets collection (key -> metrics).
- `Forms/Styles/BeepFormStylePresetsLoader.cs` — load/merge presets from JSON.
- `Forms/Styles/samples/*.json` — sample presets (macOS/GNOME/KDE/Cinnamon/Elementary).
- `Forms/Caption/ICaptionRenderer.cs` — caption renderer strategy interface.
- `Forms/Caption/CaptionRendererKind.cs` — enum of caption renderers.
- `Forms/Caption/Renderers/*CaptionRenderer.cs` — built-in implementations.
- `Forms/Caption/Design/*` — designer TypeConverter/UITypeEditor for caption renderer.


## Caption renderers and layouts
- **Windows**: right-aligned glyph buttons, left-aligned title.
- **MacLike**: red/yellow/green circular buttons on the left; title centered.
- **Gnome**: flat glyphs on the right; title centered; gradient off by default.
- **Kde**: right-aligned glyphs with subtle hover fills; gradient on by default.
- **Cinnamon**: larger caption and spacing; right-aligned buttons; gradient on by default.
- **Elementary**: thin glyphs, generous header spacing; right-aligned buttons; gradient off by default.

Title alignment is centered for `MacLike` and `Gnome` renderers; left-aligned otherwise. Caption background uses theme gradient tokens: `AppBarGradiantStartColor`, `AppBarGradiantEndColor`, `AppBarGradiantDirection`.

Title font and color come from:
1) Overrides: `TitleFontOverride`, `TitleForeColorOverride` (runtime)
2) Theme: `AppBarTitleStyle` (converted via `BeepThemesManager.ToFont`) and `AppBarTitleForeColor` or `AppBarTitleStyle.TextColor`
3) Fallback: form `Font` and `ForeColor`


## BeepFormStyle (shape metrics)
BeepiForm maps `FormStyle` to metrics (via `ApplyMetrics`):
- Modern, Metro, Glass, Office, ModernDark, Material, Minimal, Classic, Custom
- Linux-like styles: Gnome, Kde, Cinnamon, Elementary
- Additional styles: Fluent, NeoBrutalist

Each style defines defaults in `BeepFormStyleMetricsDefaults.Map`:
- `BorderRadius`, `BorderThickness`, `ShadowDepth`, `EnableGlow`, `GlowSpread`, `CaptionHeight`, `RibbonHeight`, `CaptionButtonSize`.

Examples:
- Gnome: radius 6, thickness 1, shadow off, glow off, caption 34.
- Kde: radius 8, thickness 1, shadow 8, glow on, caption 36.
- Cinnamon: radius 10, thickness 1, shadow 8, glow on, caption 38.
- Elementary: radius 10, thickness 1, minimal shadow, glow off, caption 40.
- NeoBrutalist: radius 0, thickness 3, no shadow/glow.

Note: When the window is maximized, BeepiForm intentionally sets border radius/thickness to 0 to match maximized UX. Test style borders in Normal state.


## Public API (selected)

Appearance and Style:
- `string Title`
- `int BorderRadius`, `int BorderThickness`, `Color BorderColor`
- `BeepFormStyle FormStyle`
- `Color ShadowColor`, `int ShadowDepth`, `bool EnableGlow`, `Color GlowColor`, `float GlowSpread`
- Auto behavior:
  - `bool AutoPickDarkPresets` (default true)
  - `bool AutoApplyRendererPreset` (default true)
- Presets: `BeepFormStylePresets StylePresets`, `void ApplyPreset(string key)`

Caption Bar:
- `bool ShowCaptionBar`, `int CaptionHeight`, `bool ShowSystemButtons`, `bool EnableCaptionGradient`
- Logo: `bool ShowLogo`, `string LogoImagePath`, `Size LogoSize`, `Padding LogoMargin` (alias: `ShowIconInCaption`)
- Caption renderer:
  - `CaptionRendererKind CaptionRenderer` (Designer shows friendly names via TypeConverter/UITypeEditor)
- Caption extras:
  - `bool ShowThemeButton`, `bool ShowStyleButton`
  - `string ThemeButtonIconPath`, `string StyleButtonIconPath`
- Caption title overrides (runtime):
  - `Font? TitleFontOverride`, `Color? TitleForeColorOverride`

Backdrops and Effects:
- `BackdropType Backdrop` (None, Mica, Acrylic, Tabbed, Transient, Blur)
- `bool EnableAcrylicForGlass`, `bool EnableMicaBackdrop`, `bool UseImmersiveDarkMode`

Ribbon and Snap Hints:
- `bool ShowRibbonPlaceholder`, `int RibbonHeight`, `bool ShowQuickAccess`
- `bool ShowSnapHints`

DPI/Behavior:
- `DpiHandlingMode DpiMode` (Framework, Manual)
- `bool UseHelperInfrastructure`

Events:
- `OnFormLoad`, `OnFormShown`, `PreClose`, `OnFormClose`

Utilities:
- `ToggleMaximize()`, `AdjustControls()`, `DisplayRectangle` override


## Presets and Dark/Light auto-pick
Load sample presets at startup:

```csharp
var samples = Path.Combine(AppContext.BaseDirectory, "Styles\\samples");
BeepFormStylePresetsLoader.LoadFromFolder(StylePresets, samples);
```

Available samples in `Styles/samples`:
- `beepi-presets-macos.json` (macos.light/dark)
- `beepi-presets-gnome.json` (gnome.adwaita.light/dark)
- `beepi-presets-kde.json` (kde.breeze.light/dark)
- `beepi-presets-cinnamon.json` (cinnamon.mint.light/dark)
- `beepi-presets-elementary.json` (elementary.light/dark)

When `AutoApplyRendererPreset = true`, changing `CaptionRenderer` will:
- Map `FormStyle` to a matching style (Gnome/Kde/Cinnamon/Elementary/Material)
- Auto-pick and apply a preset key; if `AutoPickDarkPresets = true`, picks `.dark` when your theme name contains "dark" or caption/body colors are dark.

You can also apply a preset explicitly:

```csharp
ApplyPreset("kde.breeze.light");
```


## Theme integration (caption)
BeepiForm uses the current theme to draw the caption:
- Background: `AppBarGradiantStartColor`, `AppBarGradiantEndColor`, `AppBarGradiantDirection` (or `AppBarBackColor`)
- Title font: `AppBarTitleStyle` (converted via `BeepThemesManager.ToFont`)
- Title color: `AppBarTitleForeColor` or `AppBarTitleStyle.TextColor`
- System buttons and glyphs: `AppBarButtonForeColor`, `AppBarButtonBackColor` and hover tokens

To override caption typography at runtime, set `TitleFontOverride` and/or `TitleForeColorOverride` on the form’s caption helper (or expose them as form properties).


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

        // Try a renderer and style
        CaptionRenderer = CaptionRendererKind.Kde; // auto-maps to FormStyle.Kde and applies preset
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        AdjustControls();
    }
}
```


## Recipes (ready-to-use)
- macOS: `CaptionRenderer = MacLike`, `FormStyle = Material`, `ApplyPreset("macos.light")`.
- GNOME: `CaptionRenderer = Gnome`, `FormStyle = Gnome`, `ApplyPreset("gnome.adwaita.light")`.
- KDE: `CaptionRenderer = Kde`, `FormStyle = Kde`, `ApplyPreset("kde.breeze.light")`.
- Cinnamon: `CaptionRenderer = Cinnamon`, `FormStyle = Cinnamon`, `ApplyPreset("cinnamon.mint.light")`.
- Elementary: `CaptionRenderer = Elementary`, `FormStyle = Elementary`, `ApplyPreset("elementary.light")`.

Tip: When your theme is dark and `AutoPickDarkPresets = true`, the engine will try the `.dark` key automatically.


## Designer support
- `CaptionRenderer` shows a friendly dropdown (Windows, macOS-like, GNOME, KDE, Cinnamon, Elementary).
- All style metrics are serialized; presets are data-only and can be loaded at runtime.


## Troubleshooting
- “Nothing changes when I switch styles”
  - Ensure presets were loaded (see Presets section).
  - Check the window is not Maximized (borders are intentionally zero in that state).
  - Verify your app references the updated Controls project (not a stale assembly).
  - Ensure `ShowCaptionBar = true` and you’re not drawing a native title bar.
- “Title font/color doesn’t change”
  - Set `AppBarTitleStyle` and/or `AppBarTitleForeColor` in your theme, or use `TitleFontOverride`/`TitleForeColorOverride`.
- “I don’t see Cinnamon/Elementary in menus”
  - Rebuild; they appear in the designer dropdown and in the Style button menu when `ShowStyleButton = true`.


## Known Limits
- WinForms is Windows-only. “macOS/Linux look” here is skinning, not native windows.
- Some backdrops require Windows 11. Unsupported calls are no-ops at runtime.
