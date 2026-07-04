# BeepDialogManager — Review & Enhancement Master Plan

**Date:** 2026-07-04
**Status:** Phase 0 in progress
**Reference products:** Linear, Figma, Shadcn/UI, Vercel, Radix UI, Notion, Stripe, GitHub Copilot dialogs, macOS native dialogs, Beep.Winform.Modules VisualForms.

---

## Symptom (from user)

> "its not working and not showing any control when dialog shown, that make app stuck"

The dialog appears (or the app hangs waiting for one) but no internal controls render. Possible causes, ordered by likelihood:

1. `BeepDialogForm` is hosted by `BeepiFormPro` whose `OnPaintBackground` calls into the painter pipeline; the painter requires `_currentTheme` and `BorderPath` to be non-null. `BeepDialogForm.ApplyTheme()` reads `_headerPanel` (non-null after construction) and forwards theme names to all children. But the dialog's controls are added in the constructor BEFORE the theme is set — so default WinForms colors persist.
2. **`dialog.ApplyTheme()` is commented out** in `BeepDialogManager.Core.cs → CreateDialog` (line 486). Without it, the dialog's children stay on WinForms default colors. On dark themes this can produce controls that are invisible (e.g. white text on white background).
3. **App-stuck on a background thread**: `ShowAsync` runs on the UI thread when the caller is already on UI (good), but on background thread it calls `ShowDialog()` (blocking) inside a `BeginInvoke` — fine. The "app stuck" symptom is the `ShowDialog` modal loop running while the caller's `await` is still pending.

The user said "app stuck" + "no controls". Both 1+2 (invisible controls) and 3 (modal loop) can co-occur. Fix 1+2 first, then 3 if needed.

---

## Phased Plan (per document, with master todo)

Each phase is a single deliverable that can be built, tested, and committed. After each phase: build + 205 tests + manual smoke test of `BeepDialogManager.Instance.Info("Test", "Hello")`.

### Phase 0 — Diagnose (no code change)  **← in progress**
- Read all files: `BeepDialogManager.Core.cs`, `BeepDialogForm.cs`, `BeepDialogManager.Notifications.cs`, `BeepDialogManager.Progress.cs`, `BeepDialogManager.Input.cs`, `BeepDialogManager.File.cs`, plus the helper/forms/command-palette/models directories.
- Identify exact order-of-operations between `BeepiFormPro`'s `ApplyFormStyle → InitializeBuiltInRegions → InitializeComponent` and `BeepDialogForm`'s field initializers + panel adds + `ConfigureForDialogType` call.
- Output: list of root causes, ranked. The expected top 3 are: 1) `ApplyTheme` not called from `CreateDialog`, 2) BackgroundColor / ForeColor not set on the painter's expected paint path, 3) `OnShown` calls `Focus()` on a control that may not yet be visible.

### Phase 1 — Fix `ShowDialogInternal`: call `ApplyTheme()` and ensure `ShowDialog` runs on UI thread
- File: `DialogsManagers/BeepDialogManager.Core.cs → CreateDialog` (line ~486)
- Uncomment `dialog.ApplyTheme();` so the theme propagates to all children.
- Add `dialog.CurrentTheme = _defaultTheme ?? BeepThemesManager.CurrentTheme;` (or set via the `Theme` setter with proper theme resolution) so `_currentTheme` is non-null inside the dialog.
- Verify `ShowDialog()` is called with an owner that is alive. If `_hostForm` is null, fall back to `Application.OpenForms[0]`. If `OpenForms[0]` is also null, do not call `ShowDialog()` (it would throw) — return a default DialogReturn instead.
- Add a try/catch around `ShowDialog()` that surfaces exceptions via `DialogConfirmed` (no, that's wrong — use a logging hook) and a fallback `DialogReturn{ Submit=false, Cancel=true, ... }`.
- Test: `BeepDialogManager.Instance.Info("Hello", "World")` shows title + message + OK button. `ShowAsync(...).GetAwaiter().GetResult()` from background thread does not deadlock.

### Phase 2 — Fix `BeepDialogForm` constructor / order-of-operations
- File: `DialogsManagers/Forms/BeepDialogForm.cs`
- Move the explicit `BackColor` / `ForeColor` initialization for every Beep control out of the constructor and into `ApplyTheme()` so the designer defaults don't leak.
- After `_initialized = true`, call `ConfigureForDialogType()` only if `_currentTheme != null`; otherwise call it lazily from `OnShown` (post-theme-apply).
- Add `if (!IsHandleCreated) return;` to `OnResize` / `OnShown` guards — WinForms events during construction may fire before the native handle exists.
- Add `BeginInvoke(...)` around the `Focus()` call in `OnShown` so the focus happens after the form is fully realized.
- Test: construct a `BeepDialogForm` with no theme and immediately call `DialogType = DialogType.Warning; DialogButtons = BeepDialogButtons.YesNo;` — no NullReferenceException, no app-stuck.

### Phase 3 — Restore the onUiThread check in `ShowAsync` and make background-thread invocation robust
- File: `DialogsManagers/BeepDialogManager.Core.cs → ShowAsync` (line ~244)
- Restore the `onUiThread` heuristic (already present at line 256–276). Verify it correctly detects the UI thread via `Application.OpenForms[0].InvokeRequired` when `_hostForm` is null.
- Add a deadlock-safe fallback: if `ShowDialog` is called from a non-UI thread (rare path the heuristic missed), throw `InvalidOperationException` with a clear message rather than silently blocking the calling thread.
- Add a `Stopwatch` to measure show-time; if `ShowDialog` takes > 30 seconds in tests, log a warning (not a hard timeout — dialogs may legitimately take longer).
- Test: `await BeepDialogManager.Instance.InfoAsync("a", "b")` from a background task returns within 1 second. `BeepDialogManager.Instance.Info("a", "b")` (sync) blocks correctly until OK is clicked.

### Phase 4 — Replace hand-rolled toast `Form` with `BeepNotificationManager`
- File: `DialogsManagers/BeepDialogManager.Notifications.cs`
- Replace the raw `Form` + `Label` toast with calls to `BeepNotificationManager.Show(new NotificationConfig { ... })` for all `Toast(...)` variants (Info, Success, Warning, Error).
- Map `ToastType → NotificationType` (Success → Success, Error → Error, Warning → Warning, Info → Info).
- Remove the raw form's `Paint` override; use `BeepNotificationManager` which is themed.
- Add a `NotificationResult` extension for actions like "UNDO" → `SnackbarUndo(message, action, durationMs)`.
- Test: `BeepDialogManager.Instance.Toast("Saved", ToastType.Success)` shows a themed top-right toast, no raw form.

### Phase 5 — Replace progress-dialog raw GDI with `BeepProgressBar`
- File: `DialogsManagers/BeepDialogManager.Progress.cs`
- Replace the internal `BeepProgressDialog` Form's raw GDI arc drawing with a hosted `BeepPanel` containing `BeepLabel` + `BeepProgressBar` + `BeepButton` (cancel).
- Use `ProgressPainterKind.LinearBadge` for determinate and `ProgressPainterKind.DotsLoader` for indeterminate.
- Use `BeepiFormPro` for the form, not bare `Form`. Apply the form's `ApplyTheme` automatically.
- Test: `var handle = BeepDialogManager.Instance.Progress("Title", "Msg", indeterminate: true); handle.Close();` shows a themed indeterminate bar.

### Phase 6 — Enhance `BeepCommandPaletteDialog` with chips / shortcuts / icons / recent
- File: `DialogsManagers/CommandPalette/BeepCommandPaletteDialog.cs` + `CommandAction.cs`
- Add `Category`, `Shortcut`, `ImagePath` properties to `CommandAction`.
- Render category `BeepChip` row above the list.
- Render shortcut text right-aligned in `BeepListBox` items via `StyledImagePainter.PaintWithTint`.
- Track last 5 executed actions via `BeepStateStore`-like helper; pin them at top when search is empty.
- Test: `BeepDialogManager.Instance.ShowCommandPalette(actions)` shows a themed palette with categories and keyboard hints.

### Phase 7 — `BeepDialogModal` layout overhaul (3-zone, semantic button styling)
- File: `DialogsManagers/Forms/BeepDialogForm.cs` — but reuses current BeepDialogForm architecture.
- Three-zone structure: header (Dock=Top), body (Dock=Fill, scrollable), footer (Dock=Bottom, 56px).
- Add `_detailsPanel` (`BeepPanel`, `Visible=false`, `ShowTitle=false`, `ShowTitleLine=false`) for the "Show details" expander.
- Add `_expandButton` (`BeepButton`, text="Show details ▸", toggles `_detailsPanel.Visible`).
- Map dialog severity to `BeepButton.ButtonType` via `ApplyButtonSemantics()`: Information/Input → Primary+Ghost, Warning → Warning+Ghost, Error/Danger → Danger+Ghost, Question → Primary+Secondary.
- Test: pressing "Show details" expands a panel that was previously hidden.

### Phase 8 — Modern preset additions to `DialogConfig`
- File: `DialogsManagers/Models/DialogConfig.cs`
- Add `CreateDestructive(title, msg)`, `CreateUnsavedChanges()`, `CreateUpdate(version, notes)`, `CreateOnboarding(steps)`, `CreateRating(title)`, `CreateSearch(placeholder)`.
- Each preset wires the right `IconType`, `Buttons`, `DefaultButton`, `Preset`, and a body layout hint.
- Test: each preset opens the expected UI shape and dismisses cleanly.

### Phase 9 — Inline validation in input dialogs
- File: `DialogsManagers/Forms/BeepDialogForm.cs`
- When `DialogType = GetInputString` and a `InputValidator` is set, subscribe to `_inputBox.TextChanged` and:
  - Show inline error in `_validationLabel` (already exists) with the error color.
  - Disable primary button while validation fails.
  - Use `DialogMotionEngine.ShakeDialog` on failure (already exists for `ShowValidation`).
- Test: `InputValidator` returns "Email invalid" → OK is disabled, label shows error, dialog shakes.

### Phase 10 — `DialogBackdropForm` (frosted overlay) via `StyledImagePainter`
- File: `DialogsManagers/Forms/DialogBackdropForm.cs`
- `DialogBackdropForm` is a special-purpose transparent overlay window (acceptable as bare `Form`); only the `DimWithNoise` paint path can stay as `OnPaint`. The `DimOnly` and `DimWithBlur` paths are replaced with a child `BeepPanel` whose `DrawContent` calls `StyledImagePainter.PaintFrostedOverlay(g, rect, blurRadius, tint)`.
- Test: opening a `ShowBackdrop = true` dialog draws a frosted tint over the host.

### Phase 11 — `DrawContent` thumbnail when long messages overflow
- File: `DialogsManagers/Forms/BeepDialogForm.cs`
- `ReflowBodyContent` already truncates the message label at 200px tall. Add a "Show more" `BeepButton` when the message is truncated to expand the label height to fit.
- Use `TextRenderer.MeasureText` with the same constraints to decide.
- Test: a 1000-character message shows with a "Show more" button, which expands the label and grows the form.

### Phase 12 — Master todo tracker
- The master todo is in this document under "Master Todo Tracker" (updated as phases complete).
- Each phase has its own entry; cross off as completed and link to commit.

---

## Master Todo Tracker

| Phase | Title | Status | Dependencies |
|---|---|---|---|
| 0 | Diagnose | in progress | — |
| 1 | ShowDialogInternal: ApplyTheme + UI-thread fix | pending | 0 |
| 2 | BeepDialogForm constructor / order-of-operations | pending | 1 |
| 3 | ShowAsync onUiThread + deadlock-safe fallback | pending | 1 |
| 4 | Toast → BeepNotificationManager | pending | 1 |
| 5 | Progress → BeepProgressBar | pending | 1 |
| 6 | CommandPalette: chips/shortcuts/icons/recent | pending | 4 |
| 7 | BeepDialogModal 3-zone layout + semantic buttons | pending | 2 |
| 8 | Modern preset factory methods | pending | 1 |
| 9 | Inline validation in input dialogs | pending | 2 |
| 10 | DialogBackdropForm frosted overlay | pending | 4 |
| 11 | Long-message thumbnail with "Show more" | pending | 2 |
| 12 | Master todo tracker (this file) | in progress | — |

---

## Rules (apply to all phases)

1. **All dialog forms inherit `BeepiFormPro`**, not bare `Form`. Exception: `DialogBackdropForm` is a transparent overlay (Phase 10 only).
2. **Never override `OnPaint` in a `BeepiFormPro` subclass**. Use a `BeepPanel` child and override `DrawContent` there.
3. **All Beep controls have `UseThemeColors = true`**.
4. **All buttons are `BeepButton`**, all labels are `BeepLabel`, all text inputs are `BeepTextBox`, all images via `StyledImagePainter`.
5. **All theme colors come from `_currentTheme`** — no hard-coded `Color.FromArgb` in dialogs.
6. **`ApplyTheme()` propagates to every child control** — call at the end of construction AND on theme change.
7. **ApplyTheme guards**: every override must null-guard fields that may not be initialized yet. Use a single `_initialized` flag in each form.
8. **The `BeepDialogManager` singleton** must remain backward-compatible: existing `BeepDialogManager.Instance.Info(...)` calls must continue to work.
9. **`BeepiFormPro` subclasses must NOT override `OnPaint` directly**. The base painter pipeline handles chrome. To customise painting, add a `BeepPanel` (or any `BaseControl` subclass) as a child and override `DrawContent` on that child. This keeps the rounded-region / chrome path intact. Already-followed by `BeepDialogForm` — preserve this rule.

---

## Verification Checklist (each phase)

| Scenario | Expected |
|---|---|
| `BeepDialogManager.Instance.Info("a", "b")` | Title + message + single OK, themed colors, no app-stuck |
| `BeepDialogManager.Instance.Warning("a", "b")` | Same with warning icon + theme |
| `await BeepDialogManager.Instance.InfoAsync("a", "b")` from UI thread | Returns within 1 second |
| `await BeepDialogManager.Instance.InfoAsync("a", "b")` from background thread | Returns when dialog closes; no deadlock |
| Click OK rapidly multiple times | Handler fires exactly once, dialog closes cleanly |
| Dark/Light theme toggle | Next opened dialog reflects new theme |
| Empty config (`new DialogConfig()`) | Throws `ArgumentException` with clear message; doesn't hang |
| 100+ long message | Shows in scrollable body with "Show more" button |
| Preset `CreateDestructive(...)` | Red icon, "Delete" (Danger), "Cancel" (Ghost) |
| Toast with `ToastType.Error` | Red themed toast top-right, no raw form |

---

## Do NOT Change

- `BeepDialogManager` singleton accessor (`Instance`) — drop-in replacement for callers
- `IDialogManager` interface signatures
- `DialogConfig` public API surface (only ADD new methods, don't rename)
- `BeepDialogButtons` enum values
- `DialogType` enum values
- `DialogResult` enum values
- `DialogPreset` enum values
