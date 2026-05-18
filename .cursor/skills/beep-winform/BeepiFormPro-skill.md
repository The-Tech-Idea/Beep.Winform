# BeepiFormPro Skill

## Overview
`BeepiFormPro` is the main modern form class in Beep.Winform. It is a full-custom-chrome `Form` with zero native title bar (`FormBorderStyle.None`), a painter-driven caption bar, rounded corners, DPI awareness, theme synchronization, backdrop effects, and a keyboard-accessible caption button system.

**Namespace:** `TheTechIdea.Beep.Winform.Controls.Forms.ModernForm`  
**Designer:** `[Designer(typeof(BeepiFormProDesigner))]`  
**Implements:** `Form`, `IFormStyle`

---

## Quick Setup

```csharp
var form = new BeepiFormPro
{
    FormStyle     = FormStyle.Material,
    Theme         = "DarkBlue",
    UseThemeColors = true,
    ShowCaptionBar = true,
    ShowMinMaxButtons = true,
    ShowCloseButton   = true
};
form.Show();
```

---

## Enums

### `FormStyle`
Each value maps to a dedicated `IFormPainter` via `PaintersFactory`:

| Value | Painter |
|-------|---------|
| Modern | ModernFormPainter |
| Minimal | MinimalFormPainter |
| Material | MaterialFormPainter |
| MaterialYou | MaterialYouFormPainter |
| Fluent | FluentFormPainter |
| MacOS | MacOSFormPainter |
| Glass | GlassFormPainter |
| Glassmorphism | GlassmorphismFormPainter |
| Metro | MetroFormPainter |
| Metro2 | Metro2FormPainter |
| GNOME / KDE / ArcLinux / Ubuntu | Linux DE painters |
| Dracula / Nord / NordDark / OneDark / GruvBox / Solarized / Tokyo | Dark theme painters |
| Brutalist / Retro / Cartoon / ChatBubble / Cyberpunk / Neon / Holographic | Creative painters |
| Shadcn / RadixUI / NextJS / Linear | Web-framework-inspired painters |
| Terminal | TerminalFormPainter |
| NeoMorphism / Paper / iOS | Specialty painters |
| Custom | CustomFormPainter (customizable) |

### `BackdropEffect`
`None` | `Mica` | `Acrylic` | `MicaAlt` | `Blur`

### `AntiAliasMode`
`None` | `Low` | `High` (default) | `Ultra`

### `RenderingQuality`
`Auto` (default) | `Performance` | `Balanced` | `Quality` | `Ultra`

### `AdaptiveLayoutMode`
`Auto` | `Compact` | `Comfortable` | `Spacious`

### `FocusIndicatorStyle`
`None` | `Subtle` (default) | `Prominent` | `HighContrast`

### `RegionDock`
`Caption` | `Bottom` | `Left` | `Right` | `ContentOverlay`

### `SystemButtonsSide`
`Right` (default) | `Left`  — controls which side the system buttons (close/min/max) appear.

---

## Key Properties

### Core / Style

| Property | Type | Default | Category |
|----------|------|---------|----------|
| `FormStyle` | `FormStyle` | Modern | — |
| `Theme` | `string` | "DefaultTheme" | Beep Theme |
| `UseThemeColors` | `bool` | true | Beep Theme |
| `BorderColor` | `Color` | LightGray | — |

### Caption Bar

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowCaptionBar` | `bool` | true | **Show or hide the entire caption bar** |
| `CaptionHeight` | `int` | 32 | Height of the caption bar in px |
| `ShowCloseButton` | `bool` | true | Show ✕ button |
| `ShowMinMaxButtons` | `bool` | true | Show min/max buttons |
| `ShowThemeButton` | `bool` | false | Show theme-picker button |
| `ShowStyleButton` | `bool` | false | Show style-picker button |
| `ShowProfileButton` | `bool` | false | Show profile button |
| `ShowCustomActionButton` | `bool` | false | Show a custom action button |
| `ShowSearchBox` | `bool` | false | Show inline search box |
| `SearchText` | `string` | "" | Current search box content |

### Effects

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShadowEffect` | `ShadowEffect` | blur=10, offsetY=2 | Drop shadow config |
| `CornerRadius` | `CornerRadius` | 8 all | Rounded corner config |
| `AntiAliasMode` | `AntiAliasMode` | High | AA quality |
| `RenderingQuality` | `RenderingQuality` | Auto | Override AA/compositing |
| `BackdropEffect` | `BackdropEffect` | None | Mica / Acrylic / Blur |
| `PaintOverContentArea` | `bool` | false | Paint effects over child controls |

### Window

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DrawCustomWindowBorder` | `bool` | true | Custom-painted border (DevExpress-style) |
| `CustomBorderThickness` | `int` | 1 | Non-client border thickness (DIP) |

### Animation

| Property | Type | Default |
|----------|------|---------|
| `EnableAnimations` | `bool` | true |
| `AnimationDuration` | `int` | 200 ms |
| `EnableMicroInteractions` | `bool` | true |
| `HoverAnimationDuration` | `int` | 150 ms |
| `FocusTransitionDuration` | `int` | 200 ms |
| `EnableSmoothResize` | `bool` | true |
| `WindowStateTransitionDuration` | `int` | 300 ms |

### Accessibility

| Property | Type | Default |
|----------|------|---------|
| `HighContrastMode` | `bool` | false |
| `ScreenReaderSupport` | `bool` | true |
| `FocusIndicatorStyle` | `FocusIndicatorStyle` | Subtle |

### Layout / Performance

| Property | Type | Default |
|----------|------|---------|
| `AdaptiveLayoutMode` | `AdaptiveLayoutMode` | Auto |
| `EnableSmartInvalidation` | `bool` | true |
| `FormPainterMetrics` | `FormPainterMetrics` | lazy-loaded | Override painter metrics |
| `CurrentLayout` | `PainterLayoutInfo` | set by painter | Read-only layout geometry |
| `ActivePainter` | `IFormPainter` | from FormStyle | Swappable at runtime |

---

## Events

| Event | Args | Description |
|-------|------|-------------|
| `OnFormLoad` | `EventArgs` | Form loaded |
| `OnFormShown` | `EventArgs` | Form shown |
| `OnFormClose` | `EventArgs` | Form closing |
| `PreClose` | `FormClosingEventArgs` | Cancelable pre-close |
| `RegionClick` | `RegionEventArgs` | Custom region clicked |
| `RegionHover` | `RegionEventArgs` | Custom region hovered |
| `ThemeButtonClicked` | `EventArgs` | Theme button in caption |
| `StyleButtonClicked` | `EventArgs` | Style button in caption |
| `ProfileButtonClicked` | `EventArgs` | Profile button in caption |
| `SearchBoxTextChanged` | `string` | Search text changed |

---

## Public Methods

| Method | Description |
|--------|-------------|
| `ApplyTheme()` | Re-apply current theme to form and all child `IBeepUIComponent` controls |
| `ApplyTheme(string themeName)` | Apply a named theme |
| `InvalidateLayout()` | Force layout recalculation + clear cached border path |
| `AddRegion(FormRegion region)` | Add a custom painted region |
| `ClearRegions()` | Remove all custom regions |
| `GetRegisteredHitAreasSnapshot()` | Diagnostics — current hit areas |
| `AddChildExternalDrawing(control, handler, layer)` | Register a child to draw on the form surface |
| `ClearChildExternalDrawing(control)` | Remove external drawing for a child |
| `ClearAllChildExternalDrawing()` | Remove all external drawings |

---

## Architecture

### Rendering Pipeline (`OnPaintBackground`)
```
SetupGraphicsQuality()
→ EnsureLayoutCalculated()          (lazy; only when dirty)
→ ApplyBackdropEffect()             (if BackdropEffect != None)
→ ActivePainter.PaintBackground()
→ ActivePainter.PaintCaption()      (if ShowCaptionBar)
→ ActivePainter.PaintBorders()
→ PaintRegions()                    (custom FormRegion list)
→ PaintKeyboardCaptionFocusIndicator()
```

### Layout Caching
Layout is recalculated only when `_layoutDirty` is true OR when `ClientSize`, `FormStyle`, or any `Show*` caption property changes. This dramatically reduces per-frame work.

### `PainterLayoutInfo` (read via `CurrentLayout`)
```csharp
Rectangle CaptionRect, ContentRect, BottomRect, LeftRect, RightRect;
Rectangle LeftZoneRect, CenterZoneRect, RightZoneRect;  // within caption
Rectangle IconRect, TitleRect;
Rectangle MinimizeButtonRect, MaximizeButtonRect, CloseButtonRect;
Rectangle ThemeButtonRect, StyleButtonRect, CustomActionButtonRect;
Rectangle ProfileButtonRect, MailButtonRect, SearchBoxRect;
Padding SafeContentInsets;   // safe margins near rounded corners
```

---

## `FormRegion` — Custom Regions

```csharp
var region = new FormRegion
{
    Id              = "my-badge",
    Dock            = RegionDock.Caption,
    Bounds          = new Rectangle(200, 0, 80, 32),  // offset within caption
    IsInteractive   = true,
    IsEnabled       = true,
    AccessibleName  = "Notifications badge",
    OnPaint         = (g, rect) =>
    {
        using var br = new SolidBrush(Color.Red);
        g.FillEllipse(br, rect);
    }
};
form.AddRegion(region);
```

`RegionDock` mapping when painting:
- `Caption` → offset within `CaptionRect`
- `Bottom` → bottom 24px strip
- `Left` / `Right` → side strips below caption
- `ContentOverlay` → `ContentRect`

---

## `IFormPainter` — Custom Painter

```csharp
public class MyFormPainter : IFormPainter
{
    public bool SupportsAnimations => false;

    public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
    {
        // use FormPainterLayoutHelper.TryBuildStandardRightAlignedCaptionLayout(...)
        // assign owner.CurrentLayout
    }
    public void PaintBackground(Graphics g, BeepiFormPro owner) { ... }
    public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect) { ... }
    public void PaintBorders(Graphics g, BeepiFormPro owner) { ... }
    public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect) { ... }
    public ShadowEffect GetShadowEffect(BeepiFormPro owner) => owner.ShadowEffect;
    public CornerRadius GetCornerRadius(BeepiFormPro owner) => owner.CornerRadius;
    public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner) => owner.AntiAliasMode;
}

// Assign at runtime:
form.ActivePainter = new MyFormPainter();
form.InvalidateLayout();
form.Invalidate();
```

Inside `PaintCaption`, call `owner.PaintBuiltInCaptionElements(g)` to render the icon, system buttons, search box, and toolbar buttons — so your painter only needs to draw the caption background and title text.

---

## `FormPainterMetrics` — Layout Metrics

```csharp
// Override metrics for a specific form instance
form.FormPainterMetrics = new FormPainterMetrics
{
    CaptionHeight   = 48,
    ButtonWidth     = 36,
    BorderRadius    = 12,
    ButtonsPlacement = SystemButtonsSide.Left
};
```

Key fields:
- `CaptionHeight`, `ButtonWidth`, `AuxiliaryButtonWidth`, `IconSize`, `BorderRadius`
- `BorderWidth`, `ResizeBorderWidth`, `AccentBarWidth`
- `ButtonsPlacement` — `Right` (default) or `Left` (macOS-style)
- `ShowThemeButton`, `ShowStyleButton`, `ShowProfileButton`, `ShowSearchButton`

Style-specific defaults (from `FormPainterMetrics.DefaultFor()`):

| FormStyle | CaptionH | ButtonW | Radius | Placement |
|-----------|----------|---------|--------|-----------|
| Modern | 32 | 32 | 8 | Right |
| Minimal | 28 | 28 | 4 | Right |
| Material | 36 | 32 | 8 | Right |
| Fluent | 40 | 36 | 6 | Right |
| MacOS | 40 | 28 | 12 | **Left** |
| Glass | 32 | 32 | 8 | Right |

---

## Disabling the Caption Bar (`ShowCaptionBar = false`)

`ShowCaptionBar` is **fully implemented**. Setting it to `false`:

1. `FormPainterLayoutHelper` sets `CaptionRect = Rectangle.Empty` and expands `ContentRect` to cover the full client area.
2. `OnPaintBackground` skips `PaintCaption`.
3. All caption hit areas (close, min, max, search, etc.) are cleared — they return `false` from `IsKeyboardCaptionTargetCurrentlyVisible`.
4. Keyboard caption focus is cleared.

```csharp
// Full-chrome, no title bar — useful for splash screens, kiosk apps, embedded panels
form.ShowCaptionBar  = false;
form.ShowCloseButton = false;        // also suppress the close hit area
form.ShowMinMaxButtons = false;

// Content uses the full form area — no gap at top
// Borders + rounded corners still paint normally
```

**Caveats when caption is hidden:**
- The form has no built-in draggable region. Provide your own drag behavior:
  ```csharp
  form.MouseDown += (s, e) =>
  {
      if (e.Button == MouseButtons.Left)
      {
          ReleaseCapture();
          SendMessage(Handle, 0xA1 /*WM_NCLBUTTONDOWN*/, (IntPtr)2 /*HTCAPTION*/, IntPtr.Zero);
      }
  };
  ```
- The form can still be resized via the 8px resize border (Win32 `WS_SIZEBOX`).
- `ShowCloseButton = true` still works without a caption bar, but the close rectangle stays empty (`CaptionRect` is zero-size), so the button will not appear unless a painter places it outside the caption.

---

## Theme Synchronization

`BeepiFormPro` auto-subscribes to `BeepThemesManager.ThemeChanged` and `FormStyleChanged` at runtime (not in the designer). Setting `Theme` or `FormStyle` via the global manager cascades to all open forms.

```csharp
// Change globally — all BeepiFormPro instances update automatically
BeepThemesManager.SetTheme("NordDark");
BeepThemesManager.SetFormStyle(FormStyle.Nordic);
```

---

## Usage Patterns

### Splash screen (no caption, borderless, rounded)
```csharp
var splash = new BeepiFormPro
{
    FormStyle        = FormStyle.Minimal,
    ShowCaptionBar   = false,
    ShowCloseButton  = false,
    ShowMinMaxButtons = false,
    CornerRadius     = new CornerRadius(16),
    BackdropEffect   = BackdropEffect.Mica,
    StartPosition    = FormStartPosition.CenterScreen
};
splash.ShowDialog();
```

### Tool window with only close button
```csharp
var tool = new BeepiFormPro
{
    FormStyle         = FormStyle.Fluent,
    ShowMinMaxButtons = false,
    ShowThemeButton   = false,
    CaptionHeight     = 28
};
```

### Full modern shell (search + theme + profile)
```csharp
var shell = new BeepiFormPro
{
    FormStyle         = FormStyle.Material,
    ShowSearchBox     = true,
    ShowThemeButton   = true,
    ShowProfileButton = true,
    UseThemeColors    = true
};
shell.SearchBoxTextChanged += (s, text) => DoSearch(text);
shell.ThemeButtonClicked   += (s, e)    => ShowThemePicker();
shell.ProfileButtonClicked += (s, e)    => ShowProfileMenu();
```

### Swap painter at runtime
```csharp
form.ActivePainter = PaintersFactory.GetPainter(FormStyle.Cyberpunk);
form.InvalidateLayout();
form.Invalidate(true);
```

### External drawing (child draws on form surface)
```csharp
// E.g. a label that renders a floating badge on the form background
myLabel.RequestExternalDraw(form, DrawingLayer.AfterAll, (g, rect) =>
{
    using var br = new SolidBrush(Color.OrangeRed);
    g.FillEllipse(br, rect.Right - 12, rect.Top - 6, 12, 12);
});
```

---

## `FormHitAreaNames` Constants
```csharp
FormHitAreaNames.Caption      // "caption"
FormHitAreaNames.Title        // "title"
FormHitAreaNames.Icon         // "icon"
FormHitAreaNames.Close        // "close"
FormHitAreaNames.Maximize     // "maximize"
FormHitAreaNames.Minimize     // "minimize"
FormHitAreaNames.Theme        // "theme"
FormHitAreaNames.Style        // "Style"
FormHitAreaNames.CustomAction // "customAction"
FormHitAreaNames.Search       // "search"
FormHitAreaNames.Profile      // "profile"
```

---

## `PaintersFactory` — Static Cache

```csharp
IFormPainter painter = PaintersFactory.GetPainter(FormStyle.Dracula);
PaintersFactory.ClearCache();                    // force re-create (after theme change)
PaintersFactory.RemoveFromCache(FormStyle.Nord); // force re-create one painter
bool cached = PaintersFactory.IsCached(FormStyle.Tokyo);
```

---

## Related

- `FormPainterLayoutHelper` — reusable layout helpers for `CalculateLayoutAndHitAreas`
- `FormPainterRenderHelper` — reusable drawing helpers for painters
- `BeepiFormProHitAreaManager` — hit-test registration and lookup
- `BeepiFormProInteractionManager` — mouse/keyboard interaction routing
- `BeepiFormProDesigner` — Visual Studio designer support
