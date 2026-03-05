# BeepDialogManager — UI/UX Enhancement Plan
**Date:** 2026-03-05  
**Reference products:** Linear, Figma, Shadcn/UI, Vercel, Radix UI, Notion, Stripe  
**Constraint:** All UI uses Beep controls only (`BeepiFormPro`, `BeepButton`, `BeepLabel`, `BeepTextBox`, `BeepComboBox`, `BeepImage`, `BeepProgressBar`, `BeepPanel`, `BeepNotification`). All image rendering uses `StyledImagePainter`. No bare `Form`-hosted raw GDI drawing except `DialogBackdropForm`.

---

## Current State Summary

| Area | Current | Problem |
|---|---|---|
| `BeepDialogModal` body | `CaptionTextBox` + 3 `BeepButton`s | Static layout, fixed size, no scroll, no detail zone |
| Icon rendering | `BeepImage.ImagePath = Svgs.xxx` | Always same size, no theme-tinted ring/badge treatment |
| Button zone | Left/Middle/Right fixed positions | No primary/danger/ghost differentiation |
| Progress dialog | Raw GDI arc drawn in `BeepDialogManager.Progress.cs` | Not using `BeepProgressBar` |
| Toast / notifications | Raw `Form` + `Label` created in code | Not using `BeepNotification` / `BeepNotificationManager` |
| Input dialog | `InputTextBox` always visible | Fixed — now hidden for non-input types |
| Command palette | `BeepCommandPaletteDialog` → `BeepiFormPro` | Needs keyboard shortcut chips, category grouping |
| Confirmation / destructive | Single-style for all severities | No color-coded button danger/warning styling |
| No: detail expander | — | No "Show details" collapsible zone |
| No: step/progress wizard | Uses `WizardManager` ✓ | Dialog-native progress steps not available |

---

## Phase 1 — `BeepDialogModal` Layout Overhaul

### 1.1 — Three-zone layout (header / body / footer)

Replace the current two-panel layout with a proper three-zone structure:

```
┌─────────────────────────────────────────┐  panel_header (BeepiFormPro title zone)
│  [Icon]  Title                     [×]  │  ← BeepImage (40×40) + BeepLabel (title) + CloseButton
├─────────────────────────────────────────┤
│                                         │  panel_body (BeepPanel, Dock=Fill, scrollable, ShowTitle=false, ShowTitleLine=false)
│  Message text (BeepLabel, multiline)    │  ← BeepLabel wrapping
│                                         │
│  [▸ Show details]  (collapsible)        │  ← BeepButton toggle + BeepPanel expansion (ShowTitle=false, ShowTitleLine=false)
│    Stack trace / detail text            │
│                                         │
│  [InputTextBox]  (GetInputString only)  │  ← BeepTextBox
│  [SelectComboBox] (GetInputFromList)    │  ← BeepComboBox
├─────────────────────────────────────────┤
│           [Secondary]  [Primary]        │  panel_footer (BeepPanel, Dock=Bottom, 56px, ShowTitle=false, ShowTitleLine=false)
└─────────────────────────────────────────┘
```

**Changes to `BeepDialogModal.cs`:**
- Add `_detailsPanel` (`BeepPanel`, `Visible=false`, `ShowTitle=false`, `ShowTitleLine=false`, auto-height)
- Add `_expandButton` (`BeepButton`, `HideText=false`, text="Show details ▸")
- `_expandButton.Click` → toggles `_detailsPanel.Visible`, resizes form height
- Use `BeepLabel` for `CaptionTextBox` current role (read-only display label, not a textbox)
- Keep `InputTextBox` (`BeepTextBox`) only for `GetInputString`
- Keep `SelectFromListComboBox` (`BeepComboBox`) only for `GetInputFromList`

### 1.2 — Icon treatment with `StyledImagePainter`

**Rule: never override `OnPaint` in a `BeepiFormPro` subclass.** Instead, wrap the icon in a `BeepPanel` that acts as the ring container. The panel's own paint cycle handles the badge background via its inherited theming machinery.

Replace the plain `DialogImage` sitting directly on `panel1` with:

```
panel1
└── _iconContainer  (BeepPanel, 48×48, BorderRadius=12, UseThemeColors=true, ShowTitle=false, ShowTitleLine=false)
    └── DialogImage (BeepImage, Dock=Fill, ApplyThemeOnImage=true)
```

In `ApplyTheme()` — assign the semantic ring color to `_iconContainer.BackColor` and let `BeepPanel` render it:

```csharp
// In BeepDialogModal.ApplyTheme() — no OnPaint override
var ringColor = dialogType switch {
    DialogType.Error       => _currentTheme.ErrorColor,
    DialogType.Warning     => _currentTheme.WarningColor,
    DialogType.Information => _currentTheme.AccentColor,
    DialogType.Question    => _currentTheme.PrimaryColor,
    _                      => _currentTheme.PanelColor
};
_iconContainer.BackColor = Color.FromArgb(30, ringColor); // low-opacity tint
_iconContainer.Theme = Theme;
_iconContainer.ApplyTheme();
DialogImage.ImagePath = ResolveDialogIconPath(DialogType);
DialogImage.ApplyThemeOnImage = true;
DialogImage.Theme = Theme;
DialogImage.ApplyTheme();
```

If a ring outline is needed, use a second `BeepPanel` as an outer border layer (set `BorderSize=1`, `BorderColor=ringColor`, `ShowTitle=false`, `ShowTitleLine=false`) wrapping `_iconContainer` — no custom painting required.

### 1.3 — Semantic button styling

Map dialog severity to `BeepButton.ButtonType`:

| Dialog type | Primary button | Secondary button |
|---|---|---|
| `Information` / `GetInput*` | `ButtonType.Primary` | `ButtonType.Ghost` |
| `Warning` | `ButtonType.Warning` | `ButtonType.Ghost` |
| `Error` / destructive preset | `ButtonType.Danger` | `ButtonType.Ghost` |
| `Question` (confirm) | `ButtonType.Primary` | `ButtonType.Secondary` |

Add helper `ApplyButtonSemantics()` called at end of `SetDialogType()`.

---

## Phase 2 — `BeepDialogManager.Progress.cs` — Use `BeepProgressBar`

### Current problem
Progress dialogs are drawn with raw GDI arcs inside a raw `Form`. Zero Beep theming.

### Fix
Replace the internal progress form class with a `BeepiFormPro`-hosted panel containing:

```csharp
// Inside BeepProgressDialog (BeepiFormPro)
private BeepLabel    _titleLabel   = new BeepLabel { UseThemeColors = true };
private BeepLabel    _messageLabel = new BeepLabel { UseThemeColors = true };
private BeepProgressBar _bar = new BeepProgressBar {
    PainterKind = ProgressPainterKind.Linear,  // or Ring for indeterminate
    UseThemeColors = true
};
private BeepButton   _cancelButton = new BeepButton { Text = "Cancel", UseThemeColors = true };
```

- Indeterminate mode → `PainterKind = ProgressPainterKind.DotsLoader` or `ArrowHeadAnimated`
- Determinate mode → `PainterKind = ProgressPainterKind.LinearBadge` (shows % badge)
- `IProgressHandle.Update(int percent, string message)` → sets `_bar.Value = percent; _messageLabel.Text = message`

---

## Phase 3 — `BeepDialogManager.Notifications.cs` — Use `BeepNotificationManager`

### Current problem
Toasts/snackbars are raw `Form` + `Label` created by hand. No grouping, no sound, no history, no priority.

### Fix
Replace all toast/snackbar methods to delegate to `BeepNotificationManager`:

```csharp
// Replace hand-rolled Toast() with:
public void Toast(string message, ToastType type = ToastType.Info, int durationMs = 3000, ...)
{
    var notifType = type switch {
        ToastType.Success => NotificationType.Success,
        ToastType.Error   => NotificationType.Error,
        ToastType.Warning => NotificationType.Warning,
        _                 => NotificationType.Info
    };
    BeepNotificationManager.Show(new NotificationConfig {
        Message  = message,
        Type     = notifType,
        Duration = durationMs,
        Layout   = NotificationLayout.Toast,
        Position = NotificationPosition.TopRight,
        Priority = durationMs == 0 ? NotificationPriority.Critical : NotificationPriority.Normal
    });
}

public void SnackbarUndo(string message, Action undoAction, int durationMs = 5000)
{
    BeepNotificationManager.Show(new NotificationConfig {
        Message    = message,
        ActionText = "UNDO",
        Action     = undoAction,
        Layout     = NotificationLayout.Banner,
        Position   = NotificationPosition.BottomCenter,
        Duration   = durationMs
    });
}
```

`IDisposable ToastLoading(...)` → returns `BeepNotificationManager.ShowPersistent(...)` handle.

---

## Phase 4 — `BeepCommandPaletteDialog` Enhancements

### Current state
`BeepCommandPaletteDialog : BeepiFormPro` — search + list only.

### Add
1. **Category chips row** — `BeepChip` per category above the list. Clicking filters by category.
2. **Keyboard shortcut display** — `CommandAction` gains `Shortcut` string. `BeepListBox` item renders it right-aligned in a `BeepLabel` with muted color.
3. **Icon per action** — `CommandAction.ImagePath` → rendered via `StyledImagePainter.Paint(g, iconRect, action.ImagePath)` in the list item.
4. **Recent items** — last 5 executed actions pinned at top when search is empty.

```csharp
// CommandAction additions
public class CommandAction
{
    public string Text       { get; set; }
    public string Category   { get; set; }   // NEW
    public string Shortcut   { get; set; }   // NEW — e.g. "Ctrl+K"
    public string ImagePath  { get; set; }   // NEW — SVG path
    public Action Action     { get; set; }
}
```

---

## Phase 5 — Modern Preset Additions

Add the following factory methods to `DialogConfig` to match commercial product patterns:

| Preset | Modeled on | Description |
|---|---|---|
| `CreateDestructive(title, msg)` | Linear / Vercel | Red icon, "Delete" (Danger), "Cancel" (Ghost) |
| `CreateUnsavedChanges()` | Figma / VS Code | Three buttons: Save / Discard / Cancel |
| `CreateUpdate(version, notes)` | Linear / Electron | Version badge, changelog BeepLabel, "Update Now" / "Later" |
| `CreateOnboarding(steps)` | Stripe / Notion | Progress dot indicator at top, step content, Prev/Next |
| `CreateRating(title)` | App stores / Figma plugins | Star row using `BeepRating` control |
| `CreateSearch(placeholder)` | Linear command palette | Full-width `BeepTextBox` with instant results list |

---

## Phase 6 — `DialogBackdropForm` — Hosted blur panel via `StyledImagePainter`

`DialogBackdropForm` is `internal sealed` and inherits bare `Form` (not `BeepiFormPro`), so its existing `OnPaint` override is acceptable — it is a special-purpose transparent overlay window, not a Beep UI form.

However, replace the hand-drawn `PaintBlurSimulation` gradient approach with a `BeepPanel` child that owns the frosted rendering:

```csharp
// Add inside DialogBackdropForm constructor:
private BeepPanel _blurPanel;

public DialogBackdropForm()
{
    // ... existing setup ...
    _blurPanel = new BeepPanel {
        Dock = DockStyle.Fill,
        UseThemeColors = false,     // pure overlay — no theme interference
        ShowTitle = false,
        ShowTitleLine = false
    };
    Controls.Add(_blurPanel);
}
```

Override painting inside `_blurPanel` (a `BaseControl` subclass) — add a thin subclass `FrostedOverlayPanel : BeepPanel` that calls `StyledImagePainter` in its own `DrawContent()`:

```csharp
internal class FrostedOverlayPanel : BeepPanel
{
    public float BlurOpacity { get; set; } = 0.12f;

    protected override void DrawContent(Graphics g, Rectangle rect)
    {
        StyledImagePainter.PaintFrostedOverlay(
            g, rect, blurRadius: 12,
            tintColor: Color.FromArgb((int)(BlurOpacity * 255), 0, 0, 0));
    }
}
```

`DialogBackdropForm.OnPaint` is then only used for the `DimOnly` / `DimWithNoise` paths (solid dim + noise dots), which have no Beep equivalent and are fine to own directly.

---

## Phase 7 — Inline Validation in Input Dialogs

When `DialogType = GetInputString` and `config.FieldValidators` contains a `"value"` validator:
- Subscribe to `InputTextBox.TextChanged`
- On each keystroke: run validator, show inline error via `InputTextBox.ErrorText` (if property available) or a `BeepLabel` in error color below the text box
- Disable `LeftButton` (OK) while validation fails
- Enable it when valid

This matches Stripe/Linear form patterns — validation is live, not deferred to submit.

---

## Implementation Order

| Phase | File(s) | Effort | Priority |
|---|---|---|---|
| 1 — Modal layout overhaul | `Forms/BeepDialogModal.cs`, `.Designer.cs` | High | P1 |
| 2 — Progress → BeepProgressBar | `BeepDialogManager.Progress.cs` | Medium | P1 |
| 3 — Notifications → BeepNotificationManager | `BeepDialogManager.Notifications.cs` | Medium | P1 |
| 4 — CommandPalette enhancements | `CommandPalette/BeepCommandPaletteDialog.cs`, `CommandAction.cs` | Medium | P2 |
| 5 — New presets | `Models/DialogConfig.cs` | Low | P2 |
| 6 — Backdrop blur via StyledImagePainter | `Forms/DialogBackdropForm.cs` | Low | P3 |
| 7 — Inline validation | `Forms/BeepDialogModal.cs` | Medium | P2 |

---

## Rules for All Implementation Work

1. **All dialog forms inherit `BeepiFormPro`** — no bare `Form` except `DialogBackdropForm` (special-purpose transparent overlay)
2. **Never override `OnPaint` in a `BeepiFormPro` subclass** — add a `BeepPanel`, `BeepCard`, or `BaseControl`-derived child and override painting there (`DrawContent()` / the control's own paint cycle)
3. **All buttons are `BeepButton`** with `UseThemeColors = true` and appropriate `ButtonType`
4. **All labels are `BeepLabel`** — no raw `Label`
5. **All text inputs are `BeepTextBox`** — no raw `TextBox`
6. **All images via `StyledImagePainter`** or `BeepImage.ImagePath = Svgs.xxx` — never raw `Graphics.DrawImage` in form code
7. **All progress via `BeepProgressBar`** — no raw GDI arc drawing
8. **All toasts/banners via `BeepNotificationManager`** — no raw toast `Form`
9. **All theme colors from `_currentTheme`** — no hard-coded `Color.FromArgb` in dialogs
10. **`ApplyTheme()` propagates to every child control** — call at end of constructor and on theme change
11. **Icon containers use `BeepPanel` with semantic `BackColor` tint** — not a painted ring in form code
12. **`BeepPanel` used as a layout container (not a titled section) must always have `ShowTitle = false` and `ShowTitleLine = false`** — these default to `true` and will render an unwanted title bar and separator line if not explicitly disabled
