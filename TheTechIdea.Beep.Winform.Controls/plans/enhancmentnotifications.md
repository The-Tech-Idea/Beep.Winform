# BeepNotification System – Enhancement Plan
**Target directory:** `TheTechIdea.Beep.Winform.Controls/Notifications/`
**Standards:** Figma Material 3 / Fluent Design 2 / DevExpress AlertControl patterns
**Skills applied:** `beep-winform`, `beep-dpi-fonts`

---

## 1  Goals & Motivation

| Gap | Enhancement |
|-----|-------------|
| Painters (`Standard`, `Compact`, `Prominent`, `Banner`, `Toast`) share nearly identical hollow stubs | Replace with **16 fully-differentiated, production-quality painters**, one per style |
| `PaintTitle` / `PaintMessage` create `new Font(...)` inline | All fonts → `BeepFontManager.ToFont(theme.XxxFont)` in `ApplyTheme()` |
| Hardcoded pixel values throughout `PainterBase`, `Manager`, `Animator` | Replace with `DpiScalingHelper.ScaleValue(n, ownerControl)` everywhere |
| Icon rendering falls back to raw `iconPath` string with no theme tint | Use `StyledImagePainter.PaintWithTint` + SVG paths from `IconsManagement` |
| `NotificationData.EmbeddedImage` is a raw `Image` object | Replace with `ImagePath` (string) property; load/paint via `StyledImagePainter` |
| `NotificationLayout` is 5 variants | Add 11 more: **Elevated, Snackbar, InlineAlert, Callout, Chip, Timeline, MediaRich, Actionsheet, StatusBar, Overlay, FullWidth** (DevExpress ribbon bar style) |
| No `BeepControlStyle` variant per notification | Add `NotificationVisualStyle` enum (16 values) mapped to painters |
| `BeepNotificationHistory` has no live-filter / sort | Add filter bar, sort, and badge count refresh |
| `BeepNotificationGroup` uses a plain `Form` | Refactor to use `StyledImagePainter`-backed group icon and DPI-aware layout |
| No accessible `AutomationPeer` / ARIA-like metadata | Add `AccessibleName`/`AccessibleRole` on every rendered region |

---

## 2  New Enum: `NotificationVisualStyle`

```csharp
// File: NotificationModels.cs  (extend existing file)
public enum NotificationVisualStyle
{
    Material3,        // Google Material Design 3 – surface + tonal fill
    iOS15,            // Apple HIG – frosted glass, pill corners
    AntDesign,        // Alibaba Ant Design – lightweight border + icon band
    Fluent2,          // Microsoft Fluent 2 – layered acrylic
    MaterialYou,      // Dynamic color, large icon, gradient bar
    Windows11Mica,    // Win11 acrylic Mica
    MacOSBigSur,      // macOS BigSur sheet
    ChakraUI,         // Chakra solid left-bar accent
    TailwindCard,     // Tailwind CSS card style
    NotionMinimal,    // Notion text-first minimal
    Minimal,          // Zero-decoration, text only
    VercelClean,      // Vercel dark clean
    StripeDashboard,  // Stripe rounded card + colour dot
    DarkGlow,         // Dark background with colored-glow border
    DiscordStyle,     // Discord dark sidebar strip
    GradientModern    // Gradient header + white body
}
```

---

## 3  Extended `NotificationLayout` Enum

```csharp
// Append to existing NotificationLayout enum
public enum NotificationLayout
{
    // --- existing ---
    Standard, Compact, Prominent, Banner, Toast,

    // --- new ---
    /// <summary>Elevated card with drop shadow, large icon in circle badge</summary>
    Elevated,
    /// <summary>Material Snackbar — single-line, bottom-anchored, optional action link</summary>
    Snackbar,
    /// <summary>Inline alert inside form body (not floating)</summary>
    InlineAlert,
    /// <summary>Speech-bubble callout anchored to a control</summary>
    Callout,
    /// <summary>Chip-sized dismissible tag (like a browser permission pill)</summary>
    Chip,
    /// <summary>Timeline entry — icon left strip, multi-line body, timestamp right</summary>
    Timeline,
    /// <summary>Rich media — image thumbnail left, text right, action below</summary>
    MediaRich,
    /// <summary>Action sheet — full-width slide-up panel (mobile-inspired)</summary>
    ActionSheet,
    /// <summary>Thin status-bar strip at top/bottom of host form</summary>
    StatusBar,
    /// <summary>Semi-transparent overlay covering host form</summary>
    Overlay,
    /// <summary>100% host-form-width inline banner</summary>
    FullWidth
}
```

---

## 4  Model Changes

### 4.1 `NotificationData` – additions

```csharp
// Add to NotificationData class
/// <summary>Visual style mapper to the 16-painter system.</summary>
public NotificationVisualStyle VisualStyle { get; set; } = NotificationVisualStyle.Material3;

/// <summary>
/// Path to an embedded image rendered in the notification body.
/// Always a STRING (resolved by StyledImagePainter). Replaces EmbeddedImage.
/// </summary>
public string? EmbeddedImagePath { get; set; }

/// <summary>Corner radius override (0 = use style default).</summary>
public int CornerRadiusOverride { get; set; } = 0;

/// <summary>
/// Optional anchor Control for Callout layout — the notification
/// positions itself relative to this control's screen rectangle.
/// </summary>
[System.ComponentModel.Browsable(false)]
public System.Windows.Forms.Control? AnchorControl { get; set; }

/// <summary>Show a thin left-bar accent stripe (ChakraUI / Stripe style).</summary>
public bool ShowAccentStripe { get; set; } = false;

/// <summary>Accent stripe color (null = uses type color).</summary>
public System.Drawing.Color? AccentStripeColor { get; set; }
```

> **Breaking change:** Remove `public Image? EmbeddedImage { get; set; }` — use `EmbeddedImagePath` (string) instead.

### 4.2 Default icon paths → `SvgsUI` + `Svgs`

Replace the raw string key map in `GetDefaultIconForType`:

```csharp
// NotificationModels.cs
public static string GetDefaultIconForType(NotificationType type)
{
    return type switch
    {
        NotificationType.Success => IconsManagement.SvgsUI.CheckCircle,
        NotificationType.Warning => IconsManagement.SvgsUI.AlertTriangle,
        NotificationType.Error   => IconsManagement.SvgsUI.XCircle,
        NotificationType.Info    => IconsManagement.SvgsUI.InfoCircle,
        NotificationType.System  => IconsManagement.Svgs.Settings,
        _                        => IconsManagement.SvgsUI.Bell,
    };
}
```

---

## 5  Font Pattern (MANDATORY for all painters)

Every painter / base class **must** source fonts via `BeepFontManager.ToFont`. No `new Font(...)` inline:

```csharp
// NotificationPainterBase.cs — fields
protected Font _titleFont;
protected Font _messageFont;
protected Font _buttonFont;
protected Font _captionFont;

// Called by BeepNotification.ApplyTheme() and passed into painters:
internal void RefreshFonts(IBeepTheme theme)
{
    _titleFont   = BeepFontManager.ToFont(theme.TitleSmall);   // or TitleMedium for Prominent
    _messageFont = BeepFontManager.ToFont(theme.BodyMedium);
    _buttonFont  = BeepFontManager.ToFont(theme.ButtonStyle);
    _captionFont = BeepFontManager.ToFont(theme.CaptionStyle);
}
```

`BeepNotification.cs` — update `ApplyTheme()`:

```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    var theme = BeepThemesManager.CurrentTheme;
    if (theme == null) return;

    // Theme fonts set once, passed to painter
    if (_painter is NotificationPainterBase pb)
    {
        pb.RefreshFonts(theme);
        pb.OwnerControl = this;
    }
    Invalidate();
}
```

---

## 6  DPI Scaling Pattern (MANDATORY everywhere)

```csharp
// CORRECT — in painters/helpers, never cache a float scale
int pad   = DpiScalingHelper.ScaleValue(12, OwnerControl);
int icon  = DpiScalingHelper.ScaleValue(24, OwnerControl);
int strip = DpiScalingHelper.ScaleValue(4, OwnerControl);
Size sz   = DpiScalingHelper.ScaleSize(new Size(350, 80), OwnerControl);

// WRONG — do not do any of these:
// int pad = 12;
// float dpi = 1f; int pad = (int)(12 * dpi);
```

---

## 7  Icon Painting Pattern

All icon rendering **must use `StyledImagePainter`**:

```csharp
// In any painter — PaintIcon
protected void DrawIcon(Graphics g, Rectangle iconRect, string imagePath, Color tintColor)
{
    if (string.IsNullOrEmpty(imagePath) || iconRect.IsEmpty) return;
    int radius = DpiScalingHelper.ScaleValue(4, OwnerControl);
    StyledImagePainter.PaintWithTint(g, iconRect, imagePath, tintColor, 1f, radius);
}

// Circular badge icon (Elevated, Material3 style)
protected void DrawCircleBadgeIcon(Graphics g, Rectangle iconRect, string imagePath, Color bgColor, Color iconColor)
{
    using var brushBg = new SolidBrush(bgColor);
    g.FillEllipse(brushBg, iconRect);
    int inset = DpiScalingHelper.ScaleValue(4, OwnerControl);
    var innerRect = new Rectangle(iconRect.X + inset, iconRect.Y + inset,
                                   iconRect.Width - inset * 2, iconRect.Height - inset * 2);
    StyledImagePainter.PaintWithTint(g, innerRect, imagePath, iconColor, 1f, 0);
}

// Embedded body image
protected void DrawBodyImage(Graphics g, Rectangle imageRect, string imagePath)
{
    if (string.IsNullOrEmpty(imagePath) || imageRect.IsEmpty) return;
    int radius = DpiScalingHelper.ScaleValue(8, OwnerControl);
    StyledImagePainter.Paint(g, imageRect, imagePath, BeepControlStyle.Material3);
}
```

---

## 8  File Structure After Enhancement

```
Notifications/
├── BeepNotification.Core.cs          ← MODIFY: ApplyTheme font refresh; remove EmbeddedImage
├── BeepNotification.Drawing.cs       ← MODIFY: route through painter's RefreshFonts
├── BeepNotification.Events.cs        ← minor: add AnchorControl Callout positioning
├── BeepNotification.Methods.cs       ← MODIFY: DPI-scaled sizes via helper
├── BeepNotificationAnimator.cs       ← MODIFY: DPI-scale offset/distance values
├── BeepNotificationGroup.cs          ← MODIFY: StyledImagePainter group icon, DPI fonts
├── BeepNotificationHistory.cs        ← MODIFY: filter bar, sort, DPI fonts, SVG icons
├── BeepNotificationManager.cs        ← MODIFY: DPI margin/spacing references, CalloutLayout
├── BeepNotificationSound.cs          ← unchanged
├── NotificationModels.cs             ← MODIFY: new enums, new Data fields, SVG icon map
│
├── Helpers/
│   ├── NotificationLayoutHelper.cs   ← NEW: centralised layout rect calculations (DPI-aware)
│   ├── NotificationThemeHelpers.cs   ← MODIFY: pass IBeepTheme to GetColorsForType
│   └── NotificationStyleHelpers.cs   ← MODIFY: DPI-aware spacing/padding helpers
│
├── Models/
│   └── NotificationRenderOptions.cs  ← MODIFY: add VisualStyle, AccentStripeColor, CornerRadius
│
└── Painters/
    ├── INotificationPainter.cs        ← MODIFY: add RefreshFonts(IBeepTheme), DrawBadgeIcon
    ├── NotificationPainterBase.cs     ← MODIFY: full DPI scaling; BeepFontManager fonts; SVG icons
    ├── NotificationPainterFactory.cs  ← MODIFY: route on both Layout + VisualStyle
    │
    ├── Standard/
    │   ├── Material3NotificationPainter.cs      ← NEW (replaces StandardNotificationPainter)
    │   ├── iOS15NotificationPainter.cs          ← NEW
    │   ├── AntDesignNotificationPainter.cs      ← NEW
    │   ├── Fluent2NotificationPainter.cs        ← NEW
    │   ├── MaterialYouNotificationPainter.cs    ← NEW
    │   ├── Windows11MicaNotificationPainter.cs  ← NEW
    │   ├── MacOSBigSurNotificationPainter.cs    ← NEW
    │   ├── ChakraUINotificationPainter.cs       ← NEW
    │   ├── TailwindCardNotificationPainter.cs   ← NEW
    │   ├── NotionMinimalNotificationPainter.cs  ← NEW
    │   ├── MinimalNotificationPainter.cs        ← NEW
    │   ├── VercelCleanNotificationPainter.cs    ← NEW
    │   ├── StripeDashboardNotificationPainter.cs← NEW
    │   ├── DarkGlowNotificationPainter.cs       ← NEW
    │   ├── DiscordStyleNotificationPainter.cs   ← NEW
    │   └── GradientModernNotificationPainter.cs ← NEW
    │
    ├── BannerNotificationPainter.cs   ← MODIFY: delegates to active VisualStyle painter
    ├── CompactNotificationPainter.cs  ← MODIFY: delegates to active VisualStyle painter
    ├── ProminentNotificationPainter.cs← MODIFY: delegates to active VisualStyle painter
    ├── ToastNotificationPainter.cs    ← MODIFY: delegates to active VisualStyle painter
    │
    └── Layout/
        ├── ElevatedNotificationPainter.cs       ← NEW
        ├── SnackbarNotificationPainter.cs        ← NEW
        ├── InlineAlertNotificationPainter.cs     ← NEW
        ├── CalloutNotificationPainter.cs         ← NEW
        ├── ChipNotificationPainter.cs            ← NEW
        ├── TimelineNotificationPainter.cs        ← NEW
        ├── MediaRichNotificationPainter.cs       ← NEW
        ├── ActionSheetNotificationPainter.cs     ← NEW
        ├── StatusBarNotificationPainter.cs       ← NEW
        ├── OverlayNotificationPainter.cs         ← NEW
        └── FullWidthNotificationPainter.cs       ← NEW
```

---

## 9  Painter Factory Routing (updated)

```csharp
// NotificationPainterFactory.cs
public static INotificationPainter CreatePainter(
    NotificationLayout layout,
    NotificationVisualStyle style = NotificationVisualStyle.Material3)
{
    // Step 1: Layout-specific painters override visual style
    INotificationPainter? layoutPainter = layout switch
    {
        NotificationLayout.Snackbar    => new SnackbarNotificationPainter(),
        NotificationLayout.InlineAlert => new InlineAlertNotificationPainter(),
        NotificationLayout.Callout     => new CalloutNotificationPainter(),
        NotificationLayout.Chip        => new ChipNotificationPainter(),
        NotificationLayout.Timeline    => new TimelineNotificationPainter(),
        NotificationLayout.MediaRich   => new MediaRichNotificationPainter(),
        NotificationLayout.ActionSheet => new ActionSheetNotificationPainter(),
        NotificationLayout.StatusBar   => new StatusBarNotificationPainter(),
        NotificationLayout.Overlay     => new OverlayNotificationPainter(),
        NotificationLayout.FullWidth   => new FullWidthNotificationPainter(),
        NotificationLayout.Elevated    => new ElevatedNotificationPainter(),
        _                              => null
    };
    if (layoutPainter != null) return layoutPainter;

    // Step 2: Visual style painters for Standard / Compact / Prominent / Banner / Toast
    return style switch
    {
        NotificationVisualStyle.Material3       => new Material3NotificationPainter(),
        NotificationVisualStyle.iOS15           => new iOS15NotificationPainter(),
        NotificationVisualStyle.AntDesign       => new AntDesignNotificationPainter(),
        NotificationVisualStyle.Fluent2         => new Fluent2NotificationPainter(),
        NotificationVisualStyle.MaterialYou     => new MaterialYouNotificationPainter(),
        NotificationVisualStyle.Windows11Mica   => new Windows11MicaNotificationPainter(),
        NotificationVisualStyle.MacOSBigSur     => new MacOSBigSurNotificationPainter(),
        NotificationVisualStyle.ChakraUI        => new ChakraUINotificationPainter(),
        NotificationVisualStyle.TailwindCard    => new TailwindCardNotificationPainter(),
        NotificationVisualStyle.NotionMinimal   => new NotionMinimalNotificationPainter(),
        NotificationVisualStyle.Minimal         => new MinimalNotificationPainter(),
        NotificationVisualStyle.VercelClean     => new VercelCleanNotificationPainter(),
        NotificationVisualStyle.StripeDashboard => new StripeDashboardNotificationPainter(),
        NotificationVisualStyle.DarkGlow        => new DarkGlowNotificationPainter(),
        NotificationVisualStyle.DiscordStyle    => new DiscordStyleNotificationPainter(),
        NotificationVisualStyle.GradientModern  => new GradientModernNotificationPainter(),
        _                                       => new Material3NotificationPainter()
    };
}
```

---

## 10  Painter Implementation Templates

### 10.1 Material3NotificationPainter (reference implementation)

```csharp
// Painters/Standard/Material3NotificationPainter.cs
namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Google Material Design 3 notification painter.
    /// Tonal surface fill • 12 dp corner radius • icon in coloured circle badge
    /// Layout: [badge 40dp][v-stack title+message][close 20dp][opt progress bar 4dp]
    /// </summary>
    public sealed class Material3NotificationPainter : NotificationPainterBase
    {
        public override void Draw(Graphics g, Rectangle bounds,
                                  NotificationData data, float progress)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            int pad      = DpiScalingHelper.ScaleValue(12, OwnerControl);
            int iconSz   = DpiScalingHelper.ScaleValue(40, OwnerControl);
            int closeSz  = DpiScalingHelper.ScaleValue(20, OwnerControl);
            int progH    = DpiScalingHelper.ScaleValue(4, OwnerControl);
            int radius   = DpiScalingHelper.ScaleValue(12, OwnerControl);

            var opts = CreateRenderOptions(data);
            var (back, fore, border, iconColor) = GetColorsForType(data.Type, opts);

            // 1. Background (tonal surface)
            Color tonalBack = data.CustomBackColor ?? Color.FromArgb(230,
                Color.FromArgb(back.R, back.G, back.B));
            using (var path = CreateRoundedPath(bounds, radius))
            using (var bgBrush = new SolidBrush(tonalBack))
            {
                g.FillPath(bgBrush, path);
                using (var pen = new Pen(Color.FromArgb(60, border), 1))
                    g.DrawPath(pen, path);
            }

            // 2. Accent stripe (optional)
            if (data.ShowAccentStripe)
            {
                int stripeW = DpiScalingHelper.ScaleValue(4, OwnerControl);
                var stripeColor = data.AccentStripeColor ?? iconColor;
                var stripeRect = new Rectangle(bounds.X, bounds.Y + radius,
                                               stripeW, bounds.Height - radius * 2);
                using (var b = new SolidBrush(stripeColor))
                    g.FillRectangle(b, stripeRect);
            }

            // 3. Icon badge circle
            var iconRect = new Rectangle(bounds.X + pad, bounds.Y + (bounds.Height - iconSz) / 2,
                                         iconSz, iconSz);
            string iconPath = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath
                : NotificationData.GetDefaultIconForType(data.Type);
            DrawCircleBadgeIcon(g, iconRect, iconPath,
                Color.FromArgb(40, iconColor), iconColor);

            // 4. Close button
            var closeRect = new Rectangle(bounds.Right - closeSz - pad,
                                          bounds.Y + (bounds.Height - closeSz) / 2,
                                          closeSz, closeSz);
            if (data.ShowCloseButton)
                PaintCloseButton(g, closeRect, false, data);

            // 5. Text area
            int textX     = iconRect.Right + pad;
            int textRight = data.ShowCloseButton ? closeRect.Left - pad : bounds.Right - pad;
            int textW     = textRight - textX;

            int titleH = DpiScalingHelper.ScaleValue(20, OwnerControl);
            var titleRect = new Rectangle(textX, bounds.Y + pad, textW, titleH);
            PaintTitle(g, titleRect, data.Title, data);

            var msgRect = new Rectangle(textX, titleRect.Bottom + DpiScalingHelper.ScaleValue(2, OwnerControl),
                                        textW, bounds.Height - titleRect.Bottom - pad - (data.ShowProgressBar ? progH + pad : 0));
            PaintMessage(g, msgRect, data.Message, data);

            // 6. Action buttons
            if (data.Actions?.Length > 0)
            {
                int btnH = DpiScalingHelper.ScaleValue(32, OwnerControl);
                var actRect = new Rectangle(textX, bounds.Bottom - pad - btnH, textW, btnH);
                PaintActions(g, actRect, data.Actions, data);
            }

            // 7. Progress bar
            if (data.ShowProgressBar && progress >= 0f)
            {
                var progRect = new Rectangle(bounds.X, bounds.Bottom - progH, bounds.Width, progH);
                PaintProgressBar(g, progRect, progress, data);
            }
        }
    }
}
```

> Each of the **16 painters** must implement `Draw()` with its own unique layout geometry and colour logic, following the `UseThemeColors` pattern. **Never share geometry between painters.**

### 10.2 SnackbarNotificationPainter (layout-specific)

```csharp
// Painters/Layout/SnackbarNotificationPainter.cs
/// <summary>
/// Material Snackbar: single-line, full-width dark bar, optional right action link.
/// Recommended height: 48 dp DPI-scaled. No icon, no title, no progress bar.
/// </summary>
public sealed class SnackbarNotificationPainter : NotificationPainterBase { ... }
```

### 10.3 ElevatedNotificationPainter

```csharp
// Painters/Layout/ElevatedNotificationPainter.cs
/// <summary>
/// Elevated card: dp6 Beep shadow, large icon circle (56 dp), title TitleMedium font.
/// DevExpress FlyoutPanel-inspired sizing.
/// </summary>
public sealed class ElevatedNotificationPainter : NotificationPainterBase { ... }
```

---

## 11  `NotificationPainterBase` – Required Font Methods

```csharp
// Add to NotificationPainterBase
public override void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data)
{
    if (titleRect.IsEmpty || string.IsNullOrEmpty(title)) return;
    var colors = GetColorsForType(data.Type, CreateRenderOptions(data));

    // CORRECT: use _titleFont from BeepFontManager — never new Font(...)
    Font f = _titleFont ?? SystemFonts.DefaultFont;
    TextRenderer.DrawText(g, title, f, titleRect, colors.ForeColor,
        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
}

public override void PaintMessage(Graphics g, Rectangle msgRect, string msg, NotificationData data)
{
    if (msgRect.IsEmpty || string.IsNullOrEmpty(msg)) return;
    var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
    Color msgColor = Color.FromArgb(180, colors.ForeColor);

    Font f = _messageFont ?? SystemFonts.DefaultFont;
    TextRenderer.DrawText(g, msg, f, msgRect, msgColor,
        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
}
```

---

## 12  Colour Palette Reference per `NotificationVisualStyle`

| Style | Background | Accent / Stripe | Border | Corner |
|---|---|---|---|---|
| Material3 | Tonal surface `alpha 90%` | Type fill `alpha 40%` | Type fill `alpha 60%` | 12 dp |
| iOS15 | White `alpha 72%` frosted | None | Gray `alpha 30%` | 16 dp |
| AntDesign | Type bg `alpha 10%` | Type color left stripe 4dp | Type color `alpha 40%` | 4 dp |
| Fluent2 | `White 93%` acrylic | Bottom progress only | `Gray 20%` | 8 dp |
| MaterialYou | Dynamic Surface | Top gradient bar 8dp | None | 28 dp pill |
| Windows11Mica | `#FAFAFF alpha 80%` | None | `Gray 15%` | 8 dp |
| MacOSBigSur | Sheet `#F5F5F7 alpha 95%` | None | `#E0E0E0` | 12 dp |
| ChakraUI | `#FFF5F5` / `#F0FFF4` | Left stripe 4dp type | `alpha 30%` | 6 dp |
| TailwindCard | `White` | None | `#E5E7EB` | 8 dp |
| NotionMinimal | `White` | None | `#E9E9E7` | 4 dp |
| Minimal | Transparent | None | `alpha 10%` | 0 |
| VercelClean | `#111` | None | `#333` | 6 dp |
| StripeDashboard | `White` | Color dot 10dp | `#E5E7EB` | 8 dp |
| DarkGlow | `#1A1A2E` | Glow border 2dp | Type glow | 12 dp |
| DiscordStyle | `#36393F` | Left strip 4dp | None | 4 dp |
| GradientModern | Gradient header 48dp + White body | None | `alpha 20%` | 12 dp |

---

## 13  `BeepNotificationHistory` Enhancements

- **Filter bar** (top): text search + type filter chips using `BeepFontManager.ToFont(theme.CaptionStyle)`
- **Sort**: newest-first / oldest-first / type grouped
- **Badge count**: real-time update label using `DpiScalingHelper.ScaleValue` for pill height
- **Row icon**: `StyledImagePainter.PaintInCircle` for type icon
- **Row font**: `BeepFontManager.ToFont(theme.ListUnSelectedFont)` for message rows

---

## 14  `BeepNotificationManager` – New API

```csharp
// Convenience overload respecting VisualStyle
public void Show(string title, string message,
                 NotificationType type = NotificationType.Info,
                 NotificationVisualStyle style = NotificationVisualStyle.Material3)

// Callout anchored to a WinForms Control
public void ShowCallout(string title, string message, Control anchor,
                        NotificationType type = NotificationType.Info)

// Snackbar (Material do not stack)
public void ShowSnackbar(string message, string? actionText = null,
                         Action? onAction = null, int duration = 3000)

// Status bar (host-form-anchored, 1 line, dismisses old)
public void ShowStatusBar(Form host, string message,
                           NotificationType type = NotificationType.Info,
                           int duration = 5000)
```

---

## 15  Implementation Sequence

```
Phase 1 – Foundation (1 sprint)
  [x] Extend NotificationModels.cs (enums + Data fields)
  [x] Refactor NotificationPainterBase: RefreshFonts, DPI scaling, StyledImagePainter icon helpers
  [x] Update NotificationPainterFactory routing
  [x] Fix BeepNotification.ApplyTheme() font refresh

Phase 2 – 16 Visual-Style Painters (1 sprint)
  [ ] Material3NotificationPainter       (reference)
  [ ] iOS15NotificationPainter
  [ ] AntDesignNotificationPainter
  [ ] Fluent2NotificationPainter
  [ ] MaterialYouNotificationPainter
  [ ] Windows11MicaNotificationPainter
  [ ] MacOSBigSurNotificationPainter
  [ ] ChakraUINotificationPainter
  [ ] TailwindCardNotificationPainter
  [ ] NotionMinimalNotificationPainter
  [ ] MinimalNotificationPainter
  [ ] VercelCleanNotificationPainter
  [ ] StripeDashboardNotificationPainter
  [ ] DarkGlowNotificationPainter
  [ ] DiscordStyleNotificationPainter
  [ ] GradientModernNotificationPainter

Phase 3 – Layout-Specific Painters (1 sprint)
  [ ] ElevatedNotificationPainter
  [ ] SnackbarNotificationPainter
  [ ] InlineAlertNotificationPainter
  [ ] CalloutNotificationPainter
  [ ] ChipNotificationPainter
  [ ] TimelineNotificationPainter
  [ ] MediaRichNotificationPainter
  [ ] ActionSheetNotificationPainter
  [ ] StatusBarNotificationPainter
  [ ] OverlayNotificationPainter
  [ ] FullWidthNotificationPainter

Phase 4 – Manager & History (0.5 sprint)
  [ ] New Manager convenience API
  [ ] History filter bar, sort, badge count
  [ ] Group DPI + icon fixes
  [ ] Animator DPI-scaled offsets
```

---

## 16  Standards Compliance Checklist

| Rule | Source |
|---|---|
| All fonts via `BeepFontManager.ToFont(theme.XxxFont)` | `beep-dpi-fonts` skill §2 |
| All pixel values via `DpiScalingHelper.ScaleValue(n, ownerControl)` | `beep-dpi-fonts` skill §3 |
| All image/icon paths are **strings** — no `Image` objects | `beep-winform` skill §14 |
| All images rendered via `StyledImagePainter.PaintWithTint` / `Paint` | this plan §7 |
| SVG icon keys from `IconsManagement.SvgsUI` / `Svgs` | `IconsManagement/` directory |
| Each painter's `Draw()` is **unique** — no shared geometry | `beep-winform` skill §Painter rules |
| `UseThemeColors` respected in every painter | `beep-winform` skill §UseThemeColors |
| No `_dpiScale` cached field | `beep-dpi-fonts` skill §6 |
| One class per file | `beep-winform` skill §Painter rules |
| No state mutation inside paint methods | `beep-winform` skill §anti-patterns |
