# BeepDialogManager — v3 Enhancement Plan (Research-Informed)

**Target directory:** `TheTechIdea.Beep.Winform.Controls/DialogsManagers/`
**Plan folder:** `TheTechIdea.Beep.Winform.Controls/DialogsManagers/plans/`
**Predecessor plans:** `DIALOG_REVIEW_AND_ENHANCEMENT_PLAN.md` (v2, Phases 0-12), `DIALOG_ENHANCEMENT_PLAN.md` (v1, UI/UX focus), `DIALOG_SYSTEM_FIX_PLAN.md` (Phases 0-2 details)
**Date:** 2026-07-04
**Research conducted:** GitHub search for WinForms dialog managers; analysis of Ookii.Dialogs.WinForms (★685), dotnet/winforms built-in TaskDialog, 5cover/Dialogs (★3); full codebase audit of `BeepDialogManager` (Core/File/Input/Notifications/Progress) + `BeepDialogForm` (1167 lines) + models/helpers

---

## 0. Research Findings

### 0.1 GitHub Search — Competitor Analysis

| Library | Stars | Architecture | Key Features |
|---|---|---|---|
| **Ookii.Dialogs.WinForms** | 685★ | Native ComCtl32 wrapper. `TaskDialogButton` typed objects. Collection-based buttons. | Expander, verification checkbox, footer, command links, progress |
| **dotnet/winforms TaskDialog** | built-in (.NET 5+) | Page-based navigation (`Navigate(page)`). `TaskDialogPage` with `Created`/`Destroyed` lifecycle. | Multi-page nav, marquee progress, `AllowCloseDialog=false`, shield icons |
| **5cover/Dialogs** | 3★ | `Page` + `NextPageSelector` delegate. Immutable-by-default pages. Command links as button style. | Multi-page nav, next-page-selector pattern |

### 0.2 Beep's Strengths (vs. competition — NO changes planned)

| Strength | Beep | Competition |
|---|---|---|
| Async-first API | `ShowAsync` + `CancellationToken` | ❌ All blocking/synchronous |
| Fluent configuration | `DialogConfig` with `Builder` | ❌ Property-based only |
| Theming | Light/dark via `BeepThemesManager` | ❌ OS-native only |
| Backdrop overlay | `DialogBackdropForm` with click policy | ❌ None |
| Animations | Fade/slide/zoom with easing + reduced-motion | ❌ None |
| Platform abstraction | WF/WPF/Blazor via `IDialogManager` | ❌ WinForms-only |
| Input memory | `StoreRecentInput`/`GetRecentInputs` | ❌ None |
| State persistence | `DialogStateStore` (size/position) | ❌ None |
| Toast notifications | `BeepNotificationManager` integration | ❌ None |

### 0.3 Beep's Architecture Issues (from full audit)

| # | Issue | File | Severity |
|---|---|---|---|
| A | No multi-page navigation — all competitors have it | `Core.cs`, `BeepDialogForm.cs` | **Critical gap** |
| B | Buttons are `BeepDialogButtons[]` (flat enums) — competitors use typed `Button` objects with `Enabled`, `Visible`, etc. | `DialogConfig.cs`, `BeepDialogForm.cs` | **Architecture gap** |
| C | No marquee (indeterminate) progress bar | `Progress.cs` | **Medium** |
| D | Expander/details section exists in `BeepDialogForm` but not configurable from `DialogConfig` | `BeepDialogForm.cs`, `DialogConfig.cs` | **Medium** |
| E | No footer area for attribution text | `DialogConfig.cs`, `BeepDialogForm.cs` | **Low** |
| F | No verification checkbox ("Don't ask again") | `DialogConfig.cs`, `BeepDialogForm.cs` | **Low** |
| G | No lifecycle events (`Created`/`Destroyed`) on dialog | `BeepDialogForm.cs` | **Low** |
| H | `Control.BeginInvoke` marshalling instead of `TaskScheduler` | `Core.cs:ShowAsync` | **Papercut** |
| I | `CreateDialog()` property-setting order dependency | `Core.cs:CreateDialog` | **Papercut** |
| J | `Application.OpenForms[0]` fallback when no host form set | `Core.cs` (multiple spots) | **Papercut** |
| K | No `IDisposable` on singleton manager | `Core.cs` | **Papercut** |

---

## 1. Predecessor Audit (v2 — what's shipped vs. pending)

From `plans/DIALOG_REVIEW_AND_ENHANCEMENT_PLAN.md` and `plans/MASTER_TODO.md`:

| Phase | Title | Status |
|---|---|---|
| 0 | Diagnose "no controls + app stuck" root cause | ✅ Done |
| 1 | `ShowDialogInternal`: uncomment `dialog.ApplyTheme()` | ✅ Done |
| 2 | `BeepDialogForm` constructor / order-of-operations | ✅ Done |
| 3 | `ShowAsync` `onUiThread` + deadlock-safe fallback | ✅ Done |
| 4 | Toast → `BeepNotificationManager` | ✅ Done |
| 5 | Progress → `BeepProgressBar` | ✅ Done |
| 6 | CommandPalette: chips/shortcuts/icons/recent | ✅ Done |
| 7 | 3-zone layout + semantic buttons | ⬜ Pending (rolled into v3 Phase 7) |
| 8 | Modern preset factory methods | ⬜ Pending (v3 Phase 8) |
| 9 | Inline validation in input dialogs | ⬜ Pending (v3 Phase 9) |
| 10 | `DialogBackdropForm` frosted overlay | ⬜ Pending (v3 Phase 10) |
| 11 | Long-message "Show more" button | ⬜ Pending (v3 Phase 11) |
| 12 | Master todo tracker | ✅ Done |

v3 picks up Phases 7-11 from v2 and adds 6 **new** research-driven phases (13-18).

---

## 2. Phase 7 — 3-Zone Layout + Semantic Buttons + Expander

**Carried from v2 Phase 7 + v1 Phase 1.**
**Research-backed:** All three competitors (Ookii, dotnet, 5cover) have expander/details section.

**File:** `Forms/BeepDialogForm.cs`

### 7.1 Three-zone structure
```
┌─────────────────────────────────────────┐  _headerPanel (BeepPanel, Dock=Top)
│  [Icon]  Title                     [×]  │  ← BeepImage + BeepLabel + CloseButton
├─────────────────────────────────────────┤  _bodyPanel (BeepPanel, Dock=Fill, scrollable)
│  Message text (BeepLabel)               │
│                                         │
│  [▸ Show details]                       │  ← _detailsToggleButton (already exists)
│    Detail text                          │  ← _detailsLabel (already exists)
├─────────────────────────────────────────┤  _buttonPanel (BeepPanel, Dock=Bottom, 56px)
│           [Secondary]  [Primary]        │  ← _leftButton / _middleButton / _rightButton
└─────────────────────────────────────────┘
```

### 7.2 Semantic button styling
Map `DialogType` / `DialogPreset` → `BeepButton` styling:

| DialogType/Preset | Primary button | Secondary button |
|---|---|---|
| Information / Input | Primary | Ghost |
| Warning | Warning | Ghost |
| Error / Danger / Destructive | Danger | Ghost |
| Question | Primary | Secondary |

### 7.3 Expander exposure
Add to `DialogConfig`:
- `string? DetailsText` — when set, shows the expander toggle + detail text
- `bool DetailsExpanded` — initial expand state (default false)

**Existing wiring in `BeepDialogForm`:**
- `_detailsToggleButton` — line 258 (already created, hidden by default)
- `_detailsLabel` — line 267 (already created, hidden by default)
- Toggle logic is already in-place; just needs `DialogConfig` exposure

**Test:** Warning dialog with `DetailsText = "Stack trace..."` shows "Show details" button. Clicking it expands the detail panel. Buttons render Danger + Ghost colors.

---

## 3. Phase 8 — Modern Preset Factory Methods

**Carried from v2 Phase 8.**

**File:** `Models/DialogConfig.cs`

Add static factory methods:

| Method | Buttons | Icon | Primary button style |
|---|---|---|---|
| `CreateDestructive(title, msg)` | `Delete, Cancel` | Danger | Danger |
| `CreateUnsavedChanges()` | `Save, Don't Save, Cancel` | Warning | Primary |
| `CreateUpdate(version, notes)` | `Update, Later` | Info | Primary |
| `CreateOnboarding(steps)` | `Next, Skip` | Info | Primary |
| `CreateRating(title)` | `Submit, Skip` | Info | Primary |
| `CreateSearch(placeholder)` | `OK, Cancel` | Search | Primary |

Each preset wires `IconType`, `Buttons`, `DefaultButton`, `PresetIntent`, and a body layout hint.

**Test:** `CreateDestructive("Delete", "Are you sure?")` → Danger icon, "Delete" (Danger) + "Cancel" (Ghost), no accidental close.

---

## 4. Phase 9 — Inline Validation in Input Dialogs

**Carried from v2 Phase 9.**

**File:** `Forms/BeepDialogForm.cs`

### 9.1 TextChanged → Validator
When `DialogType = GetInputString` and `InputValidator` is set:
- Subscribe `_inputBox.TextChanged` → run `InputValidator(text)`
- Show inline error in `_validationLabel` (already exists) with the error color from theme
- Disable primary button while validation fails (`_rightButton.Enabled = false`)

### 9.2 Validation fail feedback
Use `DialogMotionEngine.ShakeDialog` (already exists for `ShowValidation`) on the first failed submit. Subsequent TYPING dismisses the shake.

**Test:** `InputValidator` returns `"Email invalid"` → OK is disabled, label shows error. Clicking OK shakes the dialog.

---

## 5. Phase 10 — `DialogBackdropForm` Frosted Overlay

**Carried from v2 Phase 10.**

**File:** `Forms/DialogBackdropForm.cs`

Current state: `DimOnly` and `DimWithBlur` use `OnPaint` override to draw a semi-transparent black rectangle. Per Rule 2, replace with a child `BeepPanel` whose `DrawContent` calls `StyledImagePainter.PaintFrostedOverlay(g, rect, blurRadius, tint)`.

The `DimWithNoise` path stays as `OnPaint` (special-purpose noise texture rendering). The bare `Form` status of `DialogBackdropForm` is acceptable per Rule 1 exception.

**Test:** Opening a `ShowBackdrop = true` dialog draws a frosted/blurred tint over the host form.

---

## 6. Phase 11 — Long-Message "Show More" Button

**Carried from v2 Phase 11.**

**File:** `Forms/BeepDialogForm.cs`

`ReflowBodyContent` already truncates the message label at 200px tall. Add a "Show more" `BeepButton` that:
1. Appears when `TextRenderer.MeasureText` proves the message overflows the 200px cap
2. On click: expands `_messageLabel` to its full `GetPreferredSize` height
3. Replaces its text with "Show less" for a second toggle

**Test:** A 500-character message shows a "Show more" button. Clicking it expands the form. Clicking "Show less" collapses it back.

---

## 7. Phase 13 — **NEW** Multi-Page Navigation (Research-Driven)

**Research backing:** All three competitors (Ookii, dotnet, 5cover) support navigating between pages while the dialog is open. This is the single biggest architecture gap vs competition.

**Files:** NEW `Models/DialogPage.cs`, modify `Forms/BeepDialogForm.cs` + `BeepDialogManager.Core.cs`

### 13.1 `DialogPage` model
```csharp
public class DialogPage
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string? IconPath { get; set; }
    public BeepDialogButtons[] Buttons { get; set; } = { BeepDialogButtons.Next };
    public Action<BeepDialogForm>? OnCreated { get; set; }
    public Action<BeepDialogForm>? OnDestroyed { get; set; }
}
```

### 13.2 `DialogConfig` extension
```csharp
public IReadOnlyList<DialogPage>? Pages { get; set; }  // when set, activates multi-page mode
```

### 13.3 `BeepDialogForm.Navigate(page)`
Replaces the current page's content (title, message, icon, buttons) without closing the form. Fires `OnDestroyed` on the leaving page, then `OnCreated` on the new page.

### 13.4 Wizard mode
When `Pages.Count > 1`:
- "Next" button navigates to next page (last page uses "Finish")
- "Back" button navigates to previous page (first page hides Back)
- Page indicator: "Step 2 of 4" in the header

**Test:** `ShowMultiPage([page1, page2, page3])` opens a 3-page wizard. Next/Back navigates between pages. Finish dismisses.

---

## 8. Phase 14 — **NEW** Typed Button Objects (Research-Driven)

**Research backing:** All three competitors use typed button objects with `Enabled`, `Visible`, `AllowCloseDialog`, etc. — Beep's flat `BeepDialogButtons[]` enum array is the limitation.

**Files:** NEW `Models/DialogButton.cs`, modify `Models/DialogConfig.cs` + `Forms/BeepDialogForm.cs`

### 14.1 `DialogButton` model
```csharp
public class DialogButton
{
    public string Text { get; set; }
    public string Id { get; set; }              // "ok", "cancel", "delete", "yes", "no", "save", ...
    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public bool IsPrimary { get; set; }
    public bool IsGhost { get; set; }
    public bool IsDanger { get; set; }
    public string? ToolTip { get; set; }
    public Action<BeepDialogForm>? OnClick { get; set; }
}
```

### 14.2 `DialogConfig` dual-mode
```csharp
// Legacy enum (kept for backward compat)
public BeepDialogButtons[] Buttons { get; set; }

// New typed model (takes precedence when both set)
public DialogButton[]? TypedButtons { get; set; }
```

### 14.3 Internal conversion
`BeepDialogManager.CreateDialog` converts `BeepDialogButtons[]` → `DialogButton[]` internally. The form only deals with `DialogButton[]`.

**Test backward compat:** `new DialogConfig { Buttons = BeepDialogButtons.YesNo }` still produces Yes + No buttons. `new DialogConfig { TypedButtons = [new DialogButton { Text = "Custom Ok" }] }` produces a custom-labeled single button.

---

## 9. Phase 15 — **NEW** Marquee Progress Bar (Research-Driven)

**Research backing:** All three competitors support indeterminate (marquee) progress.

**File:** `BeepDialogManager.Progress.cs` + `Forms/BeepProgressDialog.cs`

### 15.1 `ProgressHandle` extension
```csharp
public void MakeIndeterminate()   → sets `ProgressPainterKind.DotsLoader`
public void MakeDeterminate()     → back to `ProgressPainterKind.LinearBadge`
```

### 15.2 `BeepProgressDialog` support
The existing `BeepProgressBar` with `ProgressPainterKind.DotsLoader` for indeterminate → `LinearBadge` for determinate.

**Test:** `var h = Progress("Downloading", "...", indeterminate: true);` shows a scrolling dots animation. `h.Update(50, "50%");` switches to a determinate bar at 50%.

---

## 10. Phase 16 — **NEW** Footer Area (Research-Driven)

**Research backing:** Ookii and dotnet both have a footer/text area below the button panel.

**File:** `Models/DialogConfig.cs` + `Forms/BeepDialogForm.cs`

### 16.1 `DialogConfig` extension
```csharp
public string? FooterText { get; set; }      // e.g. "Note: You can disable this later in Settings."
public string? FooterIconPath { get; set; }   // optional info/warning icon
```

### 16.2 Rendering
A `BeepPanel` (Dock=Bottom, auto-height) below `_buttonPanel` containing a `BeepLabel` + optional `BeepImage`. The panel is hidden when `FooterText` is null.

**Test:** `new DialogConfig { FooterText = "Tip: press Esc to cancel." }` shows a footer label below the button row.

---

## 11. Phase 17 — **NEW** Verification Checkbox (Research-Driven)

**Research backing:** All three competitors provide a "Don't ask again" / verification checkbox.

**File:** `Models/DialogConfig.cs` + `Forms/BeepDialogForm.cs`

### 17.1 `DialogConfig` extension
```csharp
public string? VerificationText { get; set; }    // e.g. "Don't show this again"
public bool VerificationChecked { get; set; }     // initial state
```

### 17.2 Rendering
A `BeepCheckBox` in `_bodyPanel`, docked below `_messageLabel`. Label shows `VerificationText`.

### 17.3 `DialogReturn` extension
```csharp
public bool WasVerificationChecked { get; set; }  // read after dialog closes
```

**Test:** `new DialogConfig { VerificationText = "Don't ask again" }` shows a checkbox. After closing, `returnData.WasVerificationChecked` returns the final state.

---

## 12. Phase 18 — **NEW** Threading + Platform Modernisation (Papercut Fixes)

**Research backing:** Internal audit identified architectural hygiene gaps.

**File:** `BeepDialogManager.Core.cs`

### 18.1 Replace `Control.BeginInvoke` with `TaskScheduler`
Capture the UI `TaskScheduler` at `SetHostForm()` time:
```csharp
private TaskScheduler? _uiScheduler;
public void SetHostForm(Form form) { _hostForm = form; _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); }
```
Then in `ShowAsync`: `Task.Factory.StartNew(ShowDialogAction, token, TaskCreationOptions.None, _uiScheduler)`.

### 18.2 Add `IDisposable` to singleton manager
```csharp
public void Dispose()
{
    _backdropForm?.Close(); _backdropForm?.Dispose();
    _activeModalDialog?.Close();
    _dialogStateStore?.Persist();
    _cancellationSources.ForEach(c => c.Cancel());
    _hostForm = null;
}
```

### 18.3 Remove `Application.OpenForms[0]` fallback
Replace with explicit `_hostForm` requirement. Raise `InvalidOperationException("SetHostForm() must be called before using the dialog manager.")` if null.

### 18.4 Guard `BeginInvoke` delegate
```csharp
Action showDialogAction = () =>
{
    if (_hostForm == null || _hostForm.IsDisposed) return;
    ShowDialogInternal(config);
};
```

---

## 13. File Inventory

| File | Action | Phase(s) |
|---|---|---|
| `Models/DialogPage.cs` | **NEW** | 13 |
| `Models/DialogButton.cs` | **NEW** | 14 |
| `Forms/BeepDialogForm.cs` | Modify | 7, 9, 11, 13, 14, 16, 17 |
| `BeepDialogManager.Core.cs` | Modify | 13, 14, 18 |
| `Models/DialogConfig.cs` | Modify | 7, 8, 13, 14, 16, 17 |
| `BeepDialogManager.Progress.cs` | Modify | 15 |
| `Forms/DialogBackdropForm.cs` | Modify | 10 |
| `Forms/BeepProgressDialog.cs` | Modify | 15 |

---

## 14. Rules (inherited from v2, carry forward)

1. All dialog forms inherit `BeepiFormPro` (exception: `DialogBackdropForm` is a transparent overlay)
2. Never override `OnPaint` on a `BeepiFormPro` subclass — use `BeepPanel` child + `DrawContent`
3. All Beep controls `UseThemeColors = true`
4. All theme colors from `_currentTheme` — no hard-coded `Color.FromArgb`
5. `ApplyTheme()` propagates to every child control, called at end of construction + on theme change
6. `BeepDialogManager.Instance.Info(...)` must remain backward-compatible
7. Do NOT rename `IDialogManager` signatures, `DialogConfig` public surface, or existing enum values

---

## 15. Verification Checklist (all phases)

| Scenario | Expected |
|---|---|
| `BeepDialogManager.Instance.Info("a", "b")` | Title + message + OK, themed, no hang |
| `BeepDialogManager.Instance.Warning("a", "b")` | Warning icon, themed, no hang |
| `await BeepDialogManager.Instance.InfoAsync("a", "b")` from UI thread | Returns <1s |
| `await BeepDialogManager.Instance.InfoAsync("a", "b")` from bg thread | Returns on close, no deadlock |
| Multi-page wizard (3 pages) | Next/Back navigates, Finish dismisses |
| `CreateDestructive(...)` | Danger icon, Danger+Ghost buttons |
| Input with failing validator | OK disabled, inline error shown, shake on submit |
| `Progress(..., indeterminate: true)` | Marquee dots → updates to determinate |
| Verification checkbox "Don't ask again" | `returnData.WasVerificationChecked == true` |
| Long message (500 chars) | Shows "Show more", expands on click |
| Footer text "Tip: press Esc" | Shows below button row |
| `Dispose()` called on manager | Backdrop + active dialog disposed, state persisted |
