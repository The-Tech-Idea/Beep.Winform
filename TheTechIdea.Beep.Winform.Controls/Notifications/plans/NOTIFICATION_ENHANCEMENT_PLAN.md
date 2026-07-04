# BeepNotification System – Phase-2 Enhancement Plan (research-informed)
**Target directory:** `TheTechIdea.Beep.Winform.Controls/Notifications/`
**Plan folder:** `TheTechIdea.Beep.Winform.Controls/Notifications/plans/`
**Predecessor plan:** `../../plans/enhancmentnotifications.md` (v1, 95% implemented)
**Predecessor summary:** `../NOTIFICATION_ENHANCEMENT_SUMMARY.md`
**Standards:** Figma Material 3 / Fluent 2 / DevExpress AlertControl / **Win 11 Toast UX Guidance**
**Skills applied:** `beep-winform`, `beep-dpi-fonts`

---

## 0  Predecessor Audit – What Already Exists

See master TODO § "Predecessor Audit". Foundation (NotificationLayout / VisualStyle / painter factory / font + DPI / StyledImagePainter) is fully shipped. This v2 plan picks up the remaining work plus new gaps surfaced by the **open-source + MS-doc research** summarized below.

---

## 1  Research Findings (input → design decisions)

### 1.1  Microsoft – Notifications UX Guidance
Source: `learn.microsoft.com/en-us/windows/apps/develop/notifications/app-notifications/app-notifications-ux-guidance`

| Recommendation | Adopted in Beep plan? |
|---|---|
| "Notifications should not be noisy" — suppress when over-quota | ✅ Phase 4.1 (queue only non-Critical when over capacity), Phase 4.5 (DND respects min priority) |
| Respect **Focus Sessions** (Win 11) | 🆕 NEW gap G27 — Phase 5.6 |
| "Respond to the user's intent" — button click launches in context | ✅ Already supported by `NotificationAction.OnClick`; logged in v1 |
| "Provide a consistent Notification Center experience" — auto-clear when underlying data is consumed | 🆕 NEW gap G28 — Phase 7.6 |
| Max 5 actions, max 5 inputs (XML schema) | ✅ Already enforced in `NotificationAction[]` size limit (add explicit `MaxActions=5` constant) |
| `HintMaxLines` / `HintWrap` on text | 🆕 NEW gap G29 — Phase 2.3 (`BeepNotification.Message` rendering) |
| Clear old toasts from Notification Center automatically | 🆕 NEW gap G30 — Phase 8.5 (toast replacement via `Tag`+`Group`) |

### 1.2  Microsoft – App Notification Content Schema
Source: `learn.microsoft.com/en-us/windows/apps/develop/notifications/app-notifications/app-notifications-schema`

| Schema field | Adopted in Beep plan? |
|---|---|
| `ToastScenario`: Default / Reminder / Alarm / IncomingCall | ✅ Phase 8.3 — exact mapping table |
| `ToastVisual` → `BindingGeneric` → `Children` (AdaptiveText / AdaptiveImage / AdaptiveGroup / AdaptiveProgressBar) | ✅ Phase 8.2 — direct 1:1 with `NotificationData` |
| `ToastActionsCustom` (custom buttons), `ToastActionsSnoozeAndDismiss` (system-handled snooze+dismiss) | 🆕 NEW gap G31 — add `NotificationAction.Style = Default / Snooze / Dismiss` |
| `ToastAudio`: Src (ms-appx / ms-resource), Loop, Silent | ✅ Phase 6.1 — bundled `.wav` resources map to `ms-appx:///{type}.wav` |
| `ToastHeader` (groups in Notification Center) | ✅ Maps to `NotificationData.GroupKey` |
| `AdaptiveProgressBar` (Title / Value / ValueStringOverride / Status) | ✅ Maps to `NotificationData.ProgressValue + ProgressText` |
| `BindableString` (`<binding>` template) | 🆕 NEW gap G32 — optional binding template for live updates |
| `ToastPeople` (assignment to contacts) | Out of scope |
| Max 3 lines of text in top-level `BindingGeneric.Children` | 🆕 NEW gap G29 — clamp to 3 lines |
| `ActivationType`: Foreground / Background / Protocol | ✅ Phase 5.6 — `OnClick` callback uses Foreground semantics |

### 1.3  Open Source – `leandrovip/Vip.Notification` (28★ on GitHub)
Source: `github.com/leandrovip/Vip.Notification/blob/master/source/Vip.Notification/frmAlert.cs`

#### Patterns to BORROW
- **`protected override bool ShowWithoutActivation => true;`** — prevents the notification from stealing focus from the user's active app. 🆕 NEW gap G33 — add to `BeepNotification` (currently uses `TopMost` which can sometimes steal focus on first show)
- **Single-Timer state machine with `AlertAction { Start, Wait, Close }`** — we already do this in `BeepNotificationAnimator`, validating the architecture. ✓
- **Naming open forms `alert1..alert9` and querying `Application.OpenForms[name]`** — manual stack positioning. We track `_activeNotifications` so we don't need this, but the principle (`use the form collection for stacking, not local state`) is documented in the plan.
- **Static factory methods on a manager class** — `Alert.ShowSucess("msg")`, `Alert.ShowWarning("msg")` etc. We already have `ShowSuccess`/`ShowWarning`/`ShowError`/`ShowInfo`. ✓ Also good idea to expose `AlertType` enum (we have `NotificationType`).

#### Gaps in Vip we should NOT inherit
- **No DPI scaling** — uses raw `Width`, `Height` constants. ❌ We use `DpiScalingHelper` everywhere.
- **No theme support** — hard-coded `Color.SeaGreen`, `Color.RoyalBlue`, etc. ❌ Our painters are theme-aware.
- **No accessibility** — no `AccessibleName`/`AccessibleRole`. We address via Phase 5.1.
- **No Focus-stealing prevention beyond `ShowWithoutActivation`** — we add `TopMost=false on Show, true on Focus restore`, etc.
- **No `OwnerControl` for window grouping** — notifications are ownerless. We add Phase 5.7 — optional `OwnerForm` so positioning snaps to monitor of active window.
- **No persistence** — gone after dismiss.
- **`Timer.Interval = 1` for animation** — wasteful, ~1000 ticks/sec. ❌ We use 60 FPS / 1000/60 = 16 ms. Note in plan: do NOT follow this anti-pattern.
- **`Opacity += 0.1` per tick of 1ms** — finishes in 10 ms (no animation visible). ❌ Vip has no real animation. We do proper easing.
- **Manual `Left--` slide without easing** — looks janky. ❌ We use `ApplyEasing(t)` (cubic-out) in `AnimateSlide`.
- **`BackColor = Color.FromArgb(...)`** — designer lost ability to theme. ❌ We use `NotificationThemeHelpers.GetColorsForType(type, theme, ...)`.
- **No keyboard support** — `Esc` not handled. We already do (Phase 5.3).

#### Recommended Vip-derived API for backward parity
```csharp
// In addition to current Show/ShowWithActions, expose:
public static class BeepNotificationAlert
{
    public static void ShowSuccess(string message, int ms = 5000);
    public static void ShowWarning(string message, int ms = 8000);
    public static void ShowError(string message, int ms = 0);
    public static void ShowInfo(string message, int ms = 5000);
    public static void ShowCustom(string message, Image image, Color color, int ms = 5000);
}
```
🆕 NEW gap G34 — Phase 4.6 (parity API for users coming from Vip.Notification / similar libs).

### 1.4  Open Source – `OceanAirdrop/WinformsHTMLToastNotification` (49★)
Source: `github.com/OceanAirdrop/WinformsHTMLToastNotification`

Pattern observed: renders notifications inside a `WebBrowser` control (legacy IE engine) — **NOT ADOPTED**. Modern Win11 toast is the right escape path; embedded IE is deprecated and security-flagged. Plan explicitly excludes this approach (mentioned in Out of Scope).

### 1.5  Open Source – `Tr1sma/WindowsToastNotifyApi`
Source: `github.com/Tr1sma/WindowsToastNotifyApi`

Pattern: thin C# wrapper around `Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder` for .NET Console/WPF/WinForms. The wrapping pattern is informative — we can do the same over a now-newer `Microsoft.Windows.AppNotifications` package (post-2024 split). See 1.6.

### 1.6  Microsoft – modern Win11 toast path
Source: developer.microsoft.com/windows / Visual Studio Search "Microsoft.Windows.AppNotifications"

| Old (archived 2026-02-25) | New |
|---|---|
| `Microsoft.Toolkit.Uwp.Notifications` (WCT 7.x, archived) | `Microsoft.Windows.SDK.Contracts` + `Windows.UI.Notifications` direct |
| `ToastContentBuilder` | `AppNotificationBuilder` (`Microsoft.Windows.AppNotifications`) |
| `ToastNotificationManagerCompat.CreateToastNotifier()` | `AppNotificationManager.Default.Register()` |
| Apps had to be MSIX-packaged with Start menu shortcut | **Win32 unpackaged apps now supported** via COM activator in `Microsoft.WindowsAppSDK` 1.4+ |

🆕 NEW gap G35 — Plan Phase 8 should target `Microsoft.WindowsAppSDK` 1.4+ AppNotifications instead of the archived Toolkit package. Implementation note: requires `<UseWindowsAppSDK>true</UseWindowsAppSDK>` in the .csproj (or a separate satellite project that wraps the COM call).

### 1.7  Beep-Ecosystem Decisions (synthesized)

| Decision | Rationale |
|---|---|
| Adopt `ShowWithoutActivation = true` on `BeepNotification` | Borrowed from Vip — prevents focus theft |
| Reject `WebBrowser`-based rendering | Security + age of pattern |
| Reject `Opacity +/-= 0.1` per-tick animation | Looks broken; we already use easing |
| Adopt `BindableString` template for live updates (Phase 7.7) | Maps `BeepNotificationManager.UpdateProgress(Id, value)` to Toast XML `<binding>` |
| Adopt `ToastHeader.Id` ↔ `NotificationData.GroupKey` mapping | One-to-one with existing group API |
| Reject pre-Win11 fallback (WCT 7.x Microsoft.Toolkit.Uwp) | Archived; fall back to in-app toast only when Windows App SDK not present |

---

## 2  Remaining Gaps Found in Audit (Jan 2026, +research deltas)

| # | Gap | Severity | Source | Fix Phase |
|---|-----|----------|--------|-----------|
| G1 | `BeepNotification.Show()` `new void Show()` hides `Form.Show(IWin32Window)` | High | audit | Phase 1.2 ✅ |
| G2 | `OnPaint` override on `BeepiFormPro` | Medium | audit | Phase 5.0 |
| G3 | `BeepNotificationHistory` raw controls | High | audit | Phase 2 |
| G4 | `BeepNotificationGroup` uses `BaseControl` GDI | Medium | audit | Phase 3 |
| G5 | Group header icon is filled circle | Medium | audit | Phase 3 |
| G6 | Manager queues Critical by capacity | High | audit | Phase 4.1 |
| G7 | Schedule cancel by reference equality | Medium | audit | Phase 4.2 |
| G8 | Recurring no catch-up | Medium | audit | Phase 4.3 |
| G9 | Batch ops no `IsDisposed` guard | High | audit | Phase 4.4 |
| G10 | Hardcoded `300px` slide | Low | audit | Phase 4.7 |
| G11 | Sound: sync `SystemSounds`, no volume, no DND mute | Low | audit | Phase 6 |
| G12 | No accessibility metadata | Medium | audit | Phase 5.1 |
| G13 | Manager keyboard nav | Low | audit | Phase 5.4 |
| G14 | History not persisted | Low | audit | Phase 7 |
| G15 | No Win11 toast integration | Future | audit | Phase 8 |
| G16 | No tests | High | audit | Phase 9 |
| G17 | Dispose implemented | done | audit | — |
| G18 | CleanupNotification re-entrancy | Medium | audit | Phase 4.5 |
| G19 | `_painter.OwnerControl` never reset | Low | audit | Phase 2.1 |
| G20 | First-launch theme null → fonts never refresh | Medium | audit | Phase 5.6 |
| G22 | NotificationData setter no theme push | Medium | audit | Phase 5.5 |
| G23 | Manager threading lock | High | audit | Phase 4.4 |
| G24 | HistoryPanel never shown by default | UX | audit | Phase 4.7 |
| G25 | CustomSoundPath extension | Low | audit | Phase 6.6 |
| G26 | Group first-item-wins type | Medium | audit | Phase 3.5 |
| **G27** | **Focus Session detection** | Medium | MS UX guidance | Phase 5.6 |
| **G28** | **Auto-clear notifications when underlying data consumed** | Low | MS UX guidance | Phase 7.6 |
| **G29** | **Clamp message rendering to 3 top-level lines (Toast schema rule)** | Low | MS schema | Phase 2.3 |
| **G30** | **Tag + Group toast for replacement / grouping in Action Center** | Low | MS schema | Phase 8.5 |
| **G31** | **NotificationAction.Style (Default/Snooze/Dismiss)** | Low | MS schema | Phase 2.7 |
| **G32** | **`NotificationTemplate` (BindableString) for live updates** | Medium | MS schema | Phase 7.7 |
| **G33** | **`ShowWithoutActivation = true` to prevent focus theft** | Medium | Vip research | Phase 5.8 |
| **G34** | **`BeepNotificationAlert` static factory for Vip-Notification-style API** | UX | Vip research | Phase 4.6 |
| **G35** | **Target `Microsoft.WindowsAppSDK` 1.4+ `AppNotificationBuilder` (NOT archived Toolkit)** | High | Microsoft | Phase 8.1 |

---

## 3  Phase 1 – Render Hygiene & Resource Cleanup
(see Phase 1 in master TODO — items 1.1–1.5 **done**; 1.6 build/test deferred)

> **Adopts from Vip research**: nothing in Phase 1 — renderer hygiene is Beep-specific.

---

## 4  Phase 2 – `BeepNotificationHistory` → Beep Child Controls
_Adopts: G19 reset `OwnerControl`, G29 max 3-line clamp, G31 action-style enum._

- Replace raw controls → `BeepPanel` / `BeepLabel` / `BeepTextBox` / `BeepComboBox` / `BeepButton` / `BeepListBox` (per repo pattern)
- Body message rendering uses `BeepNotification.Message` truncated at 3 visual lines per MS schema rule (G29) — `TruncateMessageForHistory(notification, maxLines=3)` helper
- `NotificationAction.Style` enum: `Default / Snooze / Dismiss` — Snooze + Dismiss map to MS `ToastActionsSnoozeAndDismiss` (system-handled, zero dev code)
- All Beep children `UseThemeColors = true`

---

## 5  Phase 3 – `BeepNotificationGroup` → Controls-Based Refactor
_Adopts: G4 G5 G26. Update: per user directive (use controls, not painters), the original "switch to painter system" aim is revised to a controls-based refactor that preserves the existing notification painter pipeline while introducing child controls inside `BeepNotificationGroup` for icon/title/badge/etc._

- Drop manual `DrawHeader`/`DrawBadge`/`DrawNotificationItem` — replace header with a `BeepPanel` containing `BeepLabel` (title) + `BeepBadge` (SVG count, G5)
- Items render as `BeepNotification` instances (`Layout=Chip`) hosted inside a `BeepListBox` (Dock=Fill when expanded)
- Mixed-type aware (G26): when items have different types, the badge label reads "Mixed"
- Group expand animation: 200ms height transition (timer driven; `BeepPanel` keeps `Height` interpolated by WinForms layout)
- Group key → future Win11 Toast `Header.Id` mapping hook (Phase 8.5)
- **Do NOT** redirect the painter system; `BeepNotification` keeps its `OnPaint` pipeline. New visuals inside `BeepNotificationGroup` use child controls.

---

## 6  Phase 4 – Manager Safety & Lifecycle
_Adopts: G6 G7 G8 G9 G23 G24 G33 G34._

- Critical-priority bypass (G6)
- Schedule by Id (G7)
- Recurring catch-up loop (G8)
- `SnapshotLive` + `lock` on all collections (G9 G23)
- CleanupNotification unsubscribe-first (G18 — Phase 1.3)
- **`ShowWithoutActivation = true` on `BeepNotification`** (G33) — Set via `Form` subclass override on `BeepiFormPro.OnShown` (verify BeepiFormPro allows this; alternate is `SetWindowPos(hwndInsertAfter = HWND_TOPMOST but NOTFOCUSABLE)` via Win32 `ShowWindow` flag combination)
- **Static factory `BeepNotificationAlert`** with `ShowSuccess/ShowWarning/ShowError/ShowInfo/ShowCustom` (G34) — Vip-Notification-style API for users migrating
- Settings toggle `BeepNotificationManager.HistoryPanel.ShowOnFirstDisplay = true` (G24)

---

## 7  Phase 5 – Accessibility, Theming, Focus Respect
_Adopts: G2 G12 G13 G20 G22 G27 G33._

| Sub | Item | Source |
|-----|------|--------|
| 5.1 | `AccessibleName/Description/Role` on `BeepNotification` | audit |
| 5.2 | Custom `AccessibleObject` exposing Icon/Title/Message/Actions/Close | audit |
| 5.3 | `Esc → Dismiss`, `Enter/Space → fire ActionClicked` | audit |
| 5.4 | Manager `PrevNotification`/`NextNotification` (ArrowUp/Down) | audit |
| 5.5 | `NotificationData` setter pushes theme immediately | audit (G22) |
| 5.6 | OnHandleCreated retry `RefreshFonts` when theme was null | audit (G20). **+ NEW**: subscribe `BeepThemesManager.ThemeChanged` and `FocusSessionManager.IsFocusedChanged` (Win11) → suppress non-critical toasts during Focus Session (G27) |
| 5.7 | Move `BeepNotification` render off `OnPaint` onto `BeepPanel` child + `DrawContent` (resolves G2 — long-standing repo pattern) | audit |
| 5.8 | Verify `ShowWithoutActivation` semantics; if not possible via `BeepiFormPro`, document and use `WS_EX_NOACTIVATE` via `CreateParams` override | Vip research (G33) |

---

## 8  Phase 6 – Sound System Modernization
_Adopts: G11 G25._

- `Microsoft.WindowsAppSDK.MediaPlayer` or `System.Media.SoundPlayer` (async)
- `Volume` 0..1 (default 0.5)
- `MuteDuringDND` (default true) respects `BeepNotificationManager.DoNotDisturbMode`
- `PlayAsync(NotificationType, string?) → Task`
- Bundle embedded `.wav` (ms-appx URIs map to `ms-appx:///{type}.wav` for Win11 Toast audio)
- Validate customSoundPath extension (`.wav` only — matches ms-appx URI source constraint)

---

## 9  Phase 7 – Persistence & Templates
_Adopts: G14 G28 G32._

- `PersistenceFilePath` (`%AppData%\TheTechIdea\Beep\notifications.json`, version-tagged `"version": 2`)
- `Save()` / `Load()` via `JsonHelper.Serialize`
- `DebouncedAutoSave` timer (10s)
- Persist `_pinnedData`
- **`NotificationTemplate` class** (G32) — `BindableString`-style; allows live updates to a toast via Tag:
  ```csharp
  public class NotificationTemplate
  {
      public string ToastTag { get; set; }   // maps to Toast.Tag
      public string Group { get; set; }      // maps to Toast.Group
      public NotificationData Initial { get; set; }
      public void UpdateProgress(double value, string status); // MS schema: AdaptiveProgressBar
      public void Remove(); // MS schema: same Tag removes a toast (replacement rule)
  }
  ```
  MS schema rule: "If you use the same Tag for a new toast, the old one is replaced" (G30, G32)
- **Auto-clear on user consumption** (G28) — optional `BeepNotificationManager.MarkConsumed(notificationId)` removes from Notification Center (`AppNotificationManager.RemoveAsync(tag)`).

---

## 10  Phase 8 – Win 11 Toast Integration (Windows App SDK 1.4+)
_Adopts: G15 G30 G31 G35._

### 8.1 Choose the modern package
- ~~`Microsoft.Toolkit.Uwp.Notifications`~~ (archived 2026-02-25)
- ✅ **`Microsoft.WindowsAppSDK` 1.4+** with `AppNotifications` namespace (Win32 unpackaged apps supported)

### 8.2 Facade `BeepNotificationWindowsToastBridge`
```csharp
public static class BeepNotificationWindowsToastBridge
{
    private static bool _supported = OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763);

    public static void Register(IBeepTheme theme);
    public static void ShowToast(NotificationData data);

    // Forwarded events from AppNotificationManager
    public static event EventHandler<NotificationEventArgs> Activated;
    public static event EventHandler<NotificationEventArgs> Dismissed;
}
```

### 8.3 Exact schema mapping table
| Beep → Win 11 Toast field |
|---|
| `Title` → `AddText(title)` (top-level line 1) |
| `Message` → `AddText(message)` line 2; **clamped to 3 lines max** (G29) |
| `IconPath` → `AddAppLogoOverride(iconUri)` (ms-appx:///Resources/...) |
| `EmbeddedImagePath` → `AddHeroImage(uri)` |
| `Type = Error/Warning` → `ToastScenario.Reminder` (urgent + pre-expanded) |
| `Type = System` → `AddAttributionText(system)` |
| `Priority = Critical` → `ToastScenario.Reminder` |
| Otherwise → `ToastScenario.Default` |
| `GroupKey` → `Header { Id = groupKey, Title = ... }` (G30 grouping) |
| `Id` → `ToastNotification.Tag` (enables replace-on-update via G32) |
| `Actions[]` → `AddButton(...)` per action; **`Style = Snooze` → `AddSnoozeButton()`**, **`Style = Dismiss` → `AddDismissButton()`** — system-handled, zero client code (G31) |
| `ProgressValue` + `ProgressText` → `AddProgressBar(...)` |
| `CustomSoundPath` → `SetAudio(new ToastAudio { Src = new Uri(...) })` if .wav; else default system sound |

### 8.4 Activation routing
- `AppNotificationManager.Default.NotificationInvoked` → forward to `BeepNotificationManager` event hub → invoke the matching `NotificationAction.OnClick` callback or, if no matching action, focus the host window with the toast `arguments` payload

### 8.5 Auto-replace / auto-clear
- When `BeepNotificationManager.Show` is called with a `NotificationData.Id` that matches an active toast → call `AppNotificationManager.Default.RemoveAsync(tag)` then `ShowAsync(new)` (G32 replacement)
- `BeepNotificationManager.MarkConsumed(id)` (G28) — remove toast

---

## 11  Phase 9 – Tests
_(unchanged from v1 — see master TODO)_

---

## 12  Verification Checklist (per phase)
- [ ] `dotnet build` clean (zero warnings)
- [ ] All 205+ tests pass
- [ ] No `new Font("Segoe UI", …)` in `Notifications/` (grep audit)
- [ ] No raw `Image` constructor in `Notifications/` (grep audit)
- [ ] No `OnPaint` override on `BeepiFormPro`-derived except `BeepNotification` in-scope (G2 fixed Phase 5.7)
- [ ] All Beep child controls `UseThemeColors = true`
- [ ] All pixel values via `DpiScalingHelper`
- [ ] All `_currentTheme` access null-guarded
- [ ] **NEW (G33)**: `BeepNotification.ShowWithoutActivation` returns `true`; verified by spy tool or by clicking another app during toast — focused app stays active
- [ ] **NEW (G27)**: when Windows Focus Session is active, only `Priority >= Critical` notifications show via `AppNotificationManager` (in-app Beep form is still visible)
- [ ] **NEW (G35)**: `Microsoft.WindowsAppSDK` 1.4+ referenced; `<UseWindowsAppSDK>true</UseWindowsAppSDK>` in `<PropertyGroup>`

---

## 13  Out of Scope (this plan)

- Email-style rich-text bodies with embedded forms
- Speech synthesis for visually-impaired users
- Web push notifications
- `WebBrowser`-based HTML rendering (security-flawed, rejected)
- `Microsoft.Toolkit.Uwp.Notifications` (archived, rejected)

---

## 14  References

### Microsoft Docs (read)
- Notifications design basics — `learn.microsoft.com/en-us/windows/apps/develop/notifications/app-notifications/app-notifications-ux-guidance`
- App notification content schema — `learn.microsoft.com/en-us/windows/apps/develop/notifications/app-notifications/app-notifications-schema`
- Tiles for Windows apps — `learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/` (legacy overview)

### Microsoft GitHub
- CommunityToolkit/WindowsCommunityToolkit — `github.com/CommunityToolkit/WindowsCommunityToolkit` — **archived 2026-02-25**. v7 still works but new features moved to `github.com/CommunityToolkit/Windows`
- API design pattern: `ToastContentBuilder`-style fluent API ⇒ adopted as `AppNotificationBuilder` in WinAppSDK 1.4+

### Open Source WinForms
- `leandrovip/Vip.Notification` ★28 — most-popular WinForms toast; patterns borrowed (state machine, `ShowWithoutActivation`, static factory), anti-patterns rejected (no DPI, no theme, no easing, no a11y)
- `OceanAirdrop/WinformsHTMLToastNotification` ★49 — **rejected** (WebBrowser IE engine)
- `Tr1sma/WindowsToastNotifyApi` ★0 (recent 2025-09) — same approach as our Phase 8: thin wrapper over `AppNotificationBuilder`/`ToastContentBuilder`
- `umairmushtaq109/WinToastr` (★1) — type-based Show methods, our Phase 4.6 feature parity
- `idleh4021/winform-toast-notification-android-style` (★0) — Android Material-style slide-in; our existing animation supersedes

### Internal Beep
- Predecessor plan: `../../plans/enhancmentnotifications.md` (v1, 95% done)
- Predecessor summary: `../NOTIFICATION_ENHANCEMENT_SUMMARY.md`
- Master TODO: `MASTER_TODO.md` (sibling, same folder)

---

## 15  Risk & Mitigation

| Risk | Mitigation |
|---|---|
| WinAppSDK 1.4 `AppNotifications` requires Win 10 1809+ | Check `OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763)`; fallback to in-app `BeepNotification` |
| WinAppSDK requires `UseWindowsAppSDK=true` MSBuild property; impacts build if not set | Document in README; have Phase 8.1 verify build separately |
| Toast remove-by-tag (replacement) silently drops old toast | Document in `NotificationTemplate` API; opt-in `Template.AllowReplace` |
| Phase 5.7 moving `OnPaint` to `BeepPanel.DrawContent` requires `BeepiFormPro` patches | BeepFormPro already supports `DrawContent` pattern (used by other dialogs); just remove OnPaint from `BeepNotification` |
| 4 pre-existing flaky tests in the suite + new tests could expose other latent races | Run only the 205 baseline + new tests; mark flaky for re-run |
| Phase 4.6 `BeepNotificationAlert` static factory could mask misuse (callers ignore return type) | Document the **return value is `BeepNotification` handle** so callers can `Dismiss()` or subscribe to events |
