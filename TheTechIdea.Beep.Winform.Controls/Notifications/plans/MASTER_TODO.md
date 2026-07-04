# BeepNotification – Master TODO Tracker (research-informed v3)
**Plan:** `./NOTIFICATION_ENHANCEMENT_PLAN.md` (sibling, same folder)
**Target:** `TheTechIdea.Beep.Winform.Controls/Notifications/`
**Started:** July 2026
**Baseline tests:** 205 (4 pre-existing flaky excluded)
**Acceptance bar:** all baseline + new tests green; zero `new Font("Segoe UI", …)`; zero `OnPaint` on `BeepiFormPro`-derived (except `BeepNotification` until Phase 5.7); zero raw `Image` in `Notifications/`
**Research sources (v3):** MS UX guidance, MS toast XML schema, Vip.Notification (★28), OceanAirdrop/WinformsHTMLToastNotification (★49), Tr1sma/WindowsToastNotifyApi, WCT/MS app-notifications docs.

---

## Status Legend
- ⬜ Pending
- 🟡 In Progress
- ✅ Done
- ❌ Blocked (with reason)
- 🚫 Cancelled

---

## Predecessor Audit (already shipped — **PAINTER SYSTEM REMOVED 2026-07**)
- ✅ `NotificationVisualStyle` enum (kept for source compat, marked `[Obsolete]`)
- ✅ `NotificationLayout` enum (kept for source compat, marked `[Obsolete]`)
- ✅ `EmbeddedImagePath` (kept in model for source compat — not consumed by rendering)
- ✅ `AnchorControl` (kept on `NotificationData`)
- ✅ `SvgsUI`-based default icon map (still used, painted via `StyledImagePainter` PictureBox.Paint hook)
- 🚫 `NotificationPainterBase.RefreshFonts(theme)` — **DELETED 2026-07**
- ✅ `DpiScalingHelper.ScaleValue` everywhere (`Scaled*` helpers in `BeepNotification.cs`)
- ✅ `StyledImagePainter` icon helpers (still used for SVG fallback in `IconPicture_Paint`)
- 🚫 `BeepNotification.OnPaint` painter pipeline — **REPLACED** by child-control composition (`BeepNotification.cs:BuildChildControls`)
- 🚫 `NotificationPainterFactory` 16+11 routing — **DELETED 2026-07**
- 🚫 27 painter files (`Painters/Standard/*.cs`, `Painters/Layout/*.cs`, `Painters/INotificationPainter.cs`, `Painters/NotificationPainterBase.cs`, `Painters/NotificationPainterFactory.cs`, `Painters/{Compact,Prominent,Banner,Toast,Standard}NotificationPainter.cs`) — **DELETED 2026-07**
- ✅ `NotificationThemeHelpers.GetColorsForType` (still used by `ApplyData` for type colors)
- ✅ `NotificationData.GetDefaultDuration(priority)` (unchanged)
- 🚫 `NotificationLayoutHelpers` — **DELETED 2026-07** (layout is now driven by WinForms docking)
- 🚫 `NotificationStyleHelpers` — **DELETED 2026-07** (sizes/constants moved into `BeepNotification.BuildChildControls` via `DpiScalingHelper.ScaleValue`)

---

## Phase 1 – Render Hygiene & Resource Cleanup
_Gaps: G1, G2, G10, G17, G18_

- ✅ **1.1** `BeepNotification.Dispose(bool)` verified — now lives in consolidated `BeepNotification.cs` (Phase 5.7 simplification)
- ✅ **1.2** `BeepNotification.Show(IWin32Window)` + `ShowDialog(IWin32Window)` overloads — kept in consolidated `BeepNotification.cs`
- ✅ **1.3** `Manager.CleanupNotification` unsubscribe-first — done in `BeepNotificationManager.cs`
- ✅ **1.4** `SnapshotLive(predicate)` helper replaces `_activeNotifications.ToList()` calls; IsDisposed guards on `MarkAll*/DismissAll*/PinAll*/UnpinAll*`
- ✅ **1.5** `ScaledSlide` / `ScaledBounce` DPI-aware offsets in `BeepNotificationAnimator.cs`
- ⬜ **1.6** `dotnet build` + run 205 tests (deferred per user)

## Phase 2 – Refactor `BeepNotificationHistory` to Beep Child Controls (✅ DONE 2026-07)
_Gaps: G3, G19, G29, G31_

- ✅ **2.1** `Panel _headerPanel` → `BeepPanel` (Dock=Top, `UseThemeColors=true`)
- ✅ **2.2** `Label _titleLabel` → `BeepLabel` (`UseThemeColors=true`)
- ✅ **2.3** `TextBox _searchBox` → `BeepTextBox` (`PlaceholderText`, `UseThemeColors=true`)
- ✅ **2.4** `ComboBox _filterCombo`, `_statusFilterCombo` → kept as standard `ComboBox` (BeepComboBox overlay popup doesn't match simple drop-down semantics)
- ✅ **2.5** `Button _clearButton`, `_markAllReadButton` → `BeepButton` (`UseThemeColors=true`)
- ✅ **2.6** `Panel _listPanel` + manual paint → `Panel _itemsHost` (Dock=Fill, `AutoScroll=true`) with per-row `BeepPanel` rows + `BeepLabel` children
- ✅ **2.7** Remove `_scrollBar` (replaced by `AutoScroll=true`). ~~G31 enum~~ (deferred to Phase 8)
- ⬜ **2.8** Sort combo (Newest/Oldest/Type) + BadgeCount `BeepLabel` (deferred)
- ⬜ **2.9** `_painter.OwnerControl` reset (moot — painters deleted)
- ⬜ **2.10** `dotnet build` + run tests (deferred)

## Phase 3 – Refactor `BeepNotificationGroup` to Controls-Based Layout (✅ DONE 2026-07)
_Gaps: G4, G5, G26 — REVISED: controls-based refactor (not painter system) per user directive_

- ✅ **3.1** Drop manual `DrawHeader`/`DrawBadge`/`DrawNotificationItem` + `DrawExpandedContent` + `DrawContent` — all removed
- ✅ **3.2** Header = `BeepPanel` hosting `BeepLabel` (title) + `BeepLabel` (count badge `n` / `99+`) + `BeepLabel` (chevron glyph `\u25B6`/`\u25C0`)
- ✅ **3.3** Items render as per-row `BeepPanel` (Dock=Top) with `BeepLabel` children (timestamp, title, message) inside `_itemsHost` (Dock=Fill)
- ⬜ **3.4** 200ms height-transition expand animation (deferred)
- ✅ **3.5** Mixed-type aware badge (G26): "Mixed" / "N notification types" label when items have different types
- ⬜ **3.6** `dotnet build` + run tests (deferred)

## Phase 4 – Manager Safety & Lifecycle
_Gaps: G6, G7, G8, G9, G23, G24, G34_

- ✅ **4.1** Critical-priority bypass (`if (Count >= Max && Priority != Critical)`) — done in `BeepNotificationManager.cs:270-275`
- ✅ **4.2** `CancelScheduledNotification` matches by Id, not reference equality — done in `BeepNotificationManager.cs:665-669`
- ✅ **4.3** `SchedulerTimer_Tick` catch-up loop + pile-break — done in `BeepNotificationManager.cs:707-735`
- ✅ **4.4** `private readonly object _lock = new()`; lock all six collections (`_activeNotifications`, `_notificationQueue`, `_animators`, `_notificationGroups`, `_scheduledNotifications`, `_templates`) — `Show`, `CleanupNotification`, `DismissNotificationInternal`, `ScheduleNotification`/`ScheduleRecurringNotification`/`CancelScheduledNotification`/`CancelAllScheduledNotifications`/`ScheduledCount`/`SchedulerTimer_Tick`, `RegisterTemplate`/`ShowFromTemplate`/`RemoveTemplate`/`GetTemplateNames`, `TryAddToGroup`/`CreateGroup`/`Group_Dismissed`, `Dispose` — all under `_lock`. NEVER held across event invocations, animator disposal, or `ShowNotificationInternal`.
- ✅ **4.5** Verify unsubscribe-first (already done in 1.3)
- ✅ **4.6** **NEW (G34)**: Static factory `BeepNotificationAlert.ShowSuccess/ShowWarning/ShowError/ShowInfo/ShowCustom` (Vip-Notification parity API) — new file `BeepNotificationAlert.cs`. Returns `BeepNotification` handle for advanced use.
- ✅ **4.7** `BeepNotificationManager.HistoryPanel` — toggle `ShowOnFirstDisplay` (G24) added in `BeepNotificationManager.cs:206-225`
- ⬜ **4.8** `dotnet build` + run tests

## Phase 5 – Accessibility, Theming, Focus Respect
_Gaps: G2, G12, G13, G20, G22, G27, G33_

> **Major pivot 2026-07-04:** The painter system, OnPaint override, and BeepNotificationCanvas intermediary were all REMOVED. `BeepNotification` is now composed of child Beep controls and one `PictureBox`. Phase 5.7 in this section is therefore the work that subsumed the painter pipeline — done by deletion rather than refactor.

- ✅ **5.1** `AccessibleName/Description/Role` on `BeepNotification` — `RefreshAccessibility()` + `TruncateForAccessibility(200)` kept in consolidated `BeepNotification.cs`
- 🚫 **5.2** Custom `AccessibleObject` for child regions — **DELETED 2026-07** (`BeepNotificationAccessibleObject.cs`) along with the painter pipeline. BeepiFormPro's default accessibility shape covers the form itself; child controls supply their own.
- ✅ **5.3** `Esc → Dismiss`, `Enter/Space → fire ActionClicked`, Ctrl+P toggle-pin, Ctrl+M mark-read, keys 1/2/3 action shortcuts — verified at `BeepNotification.cs` key handler (~inside `BeepNotification_KeyDown`)
- ✅ **5.4** Manager `PrevNotification`/`NextNotification` (ArrowUp/Down) — `FocusNext/FocusPrevious/FocusedIndex` in `BeepNotificationManager.cs`
- ✅ **5.5** `NotificationData` setter rebuilds children via `ApplyData()` — done in `BeepNotification.cs:NotificationData setter`
- ✅ **5.6** `BeepThemesManager.ThemeChanged` subscription for late-bound theme refresh (G20); `FocusSessionDetector` reads `UserInteractive` + manager DND for non-critical suppression (G27)
- ✅ **5.7** **REVISE direction change (user pivot): Painter pipeline + OnPaint override REMOVED. BeepNotification now uses child controls (BeepPanel / BeepLabel / BeepButton / BeepProgressBar / PictureBox) docked via standard WinForms layout. Theme flows from `BeepThemesManager.CurrentTheme` via `UseThemeColors = true` on each child. SVG icons still supported via the `PictureBox.Paint` event using `StyledImagePainter`.**
- ✅ **5.8** `BeepNotification.ShowWithoutActivation => true` + `CreateParams { ExStyle |= WS_EX_NOACTIVATE }` — kept in `BeepNotification.cs:Window style` region
- ⬜ **5.9** `dotnet build` + run tests

## Phase 6 – Sound System Modernization

## Phase 6 – Sound System Modernization
_Gaps: G11, G25_

- ✅ **6.1** Replace `SystemSounds` with `MediaPlayer`-based async — `PlayAsync` returns `Task`; `PlaySound` wraps it for backward compat (`BeepNotificationSound.cs`)
- ✅ **6.2** `Volume` (0..1, default 0.5) property added; `value => Math.Max/Min` clamp — done
- ✅ **6.3** `MuteDuringDnd` static (default true) reads `BeepNotificationManager.DoNotDisturbMode` via reflection helper `FieldAccess.TryGetDoNotDisturbMode` — done
- ✅ **6.4** `PlayAsync(NotificationType, string?) → Task` — done
- ⬜ **6.5** Bundle embedded `.wav` resources (success/warning/error/info/system) at `ms-appx:///` URIs (deferred — bundles live with the consuming app)
- ✅ **6.6** Validate `customSoundPath` extension (`.wav` only — matches MS toast audio source constraint); falls through to type-based default with warning if invalid (G25)
- ⬜ **6.7** `dotnet build` + run tests

## Phase 7 – Persistence & Templates
_Gaps: G14, G28, G32_

- ✅ **7.1** `PersistenceFilePath` property on `BeepNotificationHistory` (default `%AppData%\TheTechIdea\Beep\notifications.json`) — done
- ✅ **7.2** `Save()` via `System.Text.Json` — done
- ✅ **7.3** `Load()` re-hydrate on first `AddNotification`; v2 with migration from v1 — done
- ✅ **7.4** `DebouncedAutoSave` timer (10s) — done
- ⬜ **7.5** Persist `_pinnedData`; restore on first `Show`
- ⬜ **7.6** **NEW (G28)**: `BeepNotificationManager.MarkConsumed(notificationId)` — removes toast from Notification Center via AppNotificationManager
- ⬜ **7.7** **NEW (G32)**: `NotificationTemplate { ToastTag, Group, Initial }` class with `UpdateProgress(value, status)` / `Remove()` — enables live toast updates and replacement via Tag
- ⬜ **7.8** `dotnet build` + run tests

## Phase 8 – Win 11 Toast Integration (Microsoft.WindowsAppSDK 1.4+)
_Gaps: G15, G30, G31, G35_

- ⬜ **8.1** **NEW (G35)**: Add `Microsoft.WindowsAppSDK` 1.4+ NuGet ref with `<UseWindowsAppSDK>true</UseWindowsAppSDK>`; **do NOT** use `Microsoft.Toolkit.Uwp.Notifications` (archived 2026-02-25)
- ⬜ **8.2** `BeepNotificationWindowsToastBridge.Register(IBeepTheme theme)` + `ShowToast(NotificationData)` facade
- ⬜ **8.3** `NotificationType → ToastScenario` mapping per plan §10.3 (`Error/Warning → Reminder`, `Critical → Reminder`, else `Default`)
- ⬜ **8.4** `AppNotificationManager.Default.NotificationInvoked` event → forward to `BeepNotificationManager` events → invoke matching `NotificationAction.OnClick`
- ⬜ **8.5** **NEW (G30)**: Toast replacement via `Tag` — when `BeepNotificationManager.Show` called with existing `Id`, remove old then show new (via Phase 7.7 `NotificationTemplate` or direct)
- ⬜ **8.6** `NotificationAction.Style = Snooze|Dismiss` → use `AddSnoozeButton`/`AddDismissButton` (system-handled)
- ⬜ **8.7** `dotnet build` + run tests; gated on Win 10 1809+

## Phase 9 – Tests
_Gaps: G16_

- ⬜ **9.1** `BeepNotificationManagerTests.cs`:
  - `Show_AddsToActiveList`
  - `Critical_BypassesCapacityLimit`
  - `DismissAll_RemovesAll`
  - `DismissAllByType_OnlyRemovesMatchingType`
  - `PinAll_SetsIsPinnedOnAll`
  - `MarkAllRead_SetsIsReadAndTimestamp`
  - `Clear_RemovesAllAndQueue`
  - `ScheduleNotification_FiresAtShowTime`
  - `ScheduleRecurring_FiresAtInterval`
  - `RegisterTemplate_ShowFromTemplate_AppliesCustomizer`
  - `HistoryPanel_AddNotification_IncrementsCount`
  - **NEW**: `BeepNotificationAlert_ShowSuccess_DoesNotStealFocus` (verifies G33)
  - **NEW**: `BeepNotificationAlert_ShowError_ReturnsNotificationHandle`
  - **NEW**: `FocusSession_Active_SuppressesNonCritical` (G27)
- ⬜ **9.2** `BeepNotificationAnimatorTests.cs`
- ⬜ **9.3** `BeepNotificationPainterTests.cs` (Material3 / iOS15 / Fluent2 / AntDesign / ChakraUI / Minimal / DarkGlow)
- ⬜ **9.4** `BeepNotificationHistoryTests.cs`
  - `AddNotification_IncrementsCount`
  - `ClearHistory_RemovesAll`
  - `GetFilteredItems_WithTypeFilter`
  - `GetFilteredItems_WithSearch`
  - `GetFilteredItems_WithStatusFilter`
  - `Save_Then_Load_RoundTrips`
  - **NEW**: `GetHistoryItem_TruncatesMessageAt3Lines` (G29)
- ⬜ **9.5** Integration
  - `Manager_ShowsMultipleNotifications_PositionsCorrectly`
  - `Manager_PlaySound_RespectsSoundEnabled`
  - `BeepNotification_PainterPerLayout_Renders`
  - **NEW (when Phase 8 ships)**: `WindowsToastBridge_ShowToast_RoutesToAppNotificationManager` (mocked)
- ⬜ **9.6** `[STAThread]` on WinForms-instantiating test classes
- ⬜ **9.7** `dotnet build` + run all 205+ tests

---

## Final Verification (post-Phase 9)

- [ ] No `new Font("Segoe UI", …)` in `Notifications/`
- [ ] No raw `Image` constructor in `Notifications/`
- [ ] No `OnPaint` override on `BeepiFormPro`-derived (G2 fixed in Phase 5.7)
- [ ] All Beep child controls `UseThemeColors = true`
- [ ] All pixel values via `DpiScalingHelper`
- [ ] All `_currentTheme` access null-guarded
- [ ] **NEW** `BeepNotification.ShowWithoutActivation` returns `true` (G33) — verified by spy/spy tool
- [ ] **NEW** Focus Session suppression (G27) verified
- [ ] **NEW** Toast audio uses `ms-appx:///Resources/{type}.wav` only (G35 / schema constraint)
- [ ] **NEW** `Microsoft.WindowsAppSDK` 1.4+ referenced; no `Microsoft.Toolkit.Uwp.Notifications` references remain (G35)
- [ ] `dotnet build` clean (zero warnings)
- [ ] All tests green: existing 205 (4 flaky excluded) + new tests

---

## Research Sources

| Source | URL | Used for |
|---|---|---|
| MS Notifications UX Guidance | `learn.microsoft.com/.../app-notifications/app-notifications-ux-guidance` | G27 (Focus Session), G28 (auto-clear), button/input limits |
| MS App Notification Schema | `learn.microsoft.com/.../app-notifications/app-notifications-schema` | Phase 8.3 mapping table, G29 (3-line clamp), G30 (Tag+Group), G31 (Snooze/Dismiss) |
| MS Tiles overview | `learn.microsoft.com/.../tiles-and-notifications/` | Legacy context only |
| WCT (archived) | `github.com/CommunityToolkit/WindowsCommunityToolkit` | Archived 2026-02-25 → moved to WinAppSDK 1.4+ |
| Vip.Notification ★28 | `github.com/leandrovip/Vip.Notification` | G33 (ShowWithoutActivation), G34 (static factory), rejects: no DPI, no theme, no easing |
| OceanAirdrop ★49 | `github.com/OceanAirdrop/WinformsHTMLToastNotification` | REJECTED (WebBrowser/IE) |
| Tr1sma | `github.com/Tr1sma/WindowsToastNotifyApi` | Phase 8 thin-wrapper pattern |
| WinToastr ★1 | `github.com/umairmushtaq109/WinToastr` | Type-based show methods (already present in our Manager) |
| WinFormsToast-Android ★1 | `github.com/kiraa024/WinFormsToast` | Animation reference; we already exceed |

---

## Progress Log

| Date | Phase(s) Done | Notes |
|------|---------------|-------|
| 2026-07-04 | Phase 1 (1.1-1.5) | Code in `BeepNotification.Methods.cs`, `BeepNotificationManager.cs`, `BeepNotificationAnimator.cs`. Build/test (1.6) deferred per user. |
| 2026-07-04 | — | v2 plan first drafted |
| 2026-07-04 | — | Plan + tracker moved to `Notifications/plans/` per user |
| 2026-07-04 | — | **v3 plan**: researched MS docs (UX guidance, schema), Vip.Notification, WinAppSDK shift from archived WCT. Added gaps G27-G35. |
| 2026-07-04 | Phase 4 (4.1, 4.2, 4.3), Phase 5 (5.1, 5.5, 5.8), Phase 6 (6.1-6.4, 6.6) | Critical-priority bypass, Id-based cancel, scheduler catch-up, accessibility (`AccessibleName`/`AccessibleRole` + `RefreshAccessibility()` on NotificationData setter), `WS_EX_NOACTIVATE` (G33 + CreateParams + `ShowWithoutActivation`), async sound + `Volume` + `MuteDuringDnd` + `.wav` extension validation (G25). Build/test still deferred. |
| 2026-07-04 | Phase 4 (4.6, 4.7), Phase 5 (5.2, 5.4, 5.6) | `BeepNotificationAlert` static factory (G34, Vip parity), HistoryPanel `ShowOnFirstDisplay` toggle (G24), `PrevNotification/NextNotification` keyboard nav (G13), `BeepNotificationAccessibleObject` exposing Icon/Title/Message/Actions/Close as `AccessibleRole.Static/PushButton` children, `BeepThemesManager.ThemeChanged` subscription for late-bound theme refresh (G20), `FocusSessionDetector` reading DND + UserInteractive for non-critical suppression (G27). Build/test still deferred. |
| 2026-07-04 | Phase 4 (4.4), Phase 5 (5.3 verify) | Thread-safety `_lock` around all six manager collections — `Show()` enqueue + capacity, `CleanupNotification` dequeue, `DismissNotificationInternal` animator lookup, `ScheduleNotification` / `ScheduleRecurringNotification` / `CancelScheduledNotification` / `CancelAllScheduledNotifications` / `ScheduledCount` / `SchedulerTimer_Tick`, `RegisterTemplate` / `ShowFromTemplate` / `RemoveTemplate` / `GetTemplateNames`, `TryAddToGroup` / `CreateGroup` / `Group_Dismissed`, `Dispose` animator snapshot. NEVER held across event invocations. Esc/Enter/Space/Ctrl+P/Ctrl+M/1/2/3 wiring confirmed at `Events.cs:70-130`. |
| 2026-07-04 | Phase 4 (4.4), Phase 5 (5.3 verify) | Thread-safety `_lock` around all six manager collections — `Show()` enqueue + capacity, `CleanupNotification` dequeue, `DismissNotificationInternal` animator lookup, `ScheduleNotification` / `ScheduleRecurringNotification` / `CancelScheduledNotification` / `CancelAllScheduledNotifications` / `ScheduledCount` / `SchedulerTimer_Tick`, `RegisterTemplate` / `ShowFromTemplate` / `RemoveTemplate` / `GetTemplateNames`, `TryAddToGroup` / `CreateGroup` / `Group_Dismissed`, `Dispose` animator snapshot. NEVER held across event invocations. Esc/Enter/Space/Ctrl+P/Ctrl+M/1/2/3 wiring confirmed at `Events.cs:70-130`. |
| 2026-07-04 | **USER PIVOT / REVISION** | **Painter pipeline + OnPaint override REMOVED** per user instruction. Deleted: 27 painter files, BeepNotificationCanvas.cs, BeepNotificationAccessibleObject.cs, Helpers/NotificationLayoutHelpers.cs, Helpers/NotificationStyleHelpers.cs, Models/NotificationStyleConfig.cs. Consolidated partials into a single BeepNotification.cs using child controls: BeepPanel/BeepLabel/BeepButton/BeepProgressBar/PictureBox/FlowLayoutPanel. [Obsolete] enums kept for source compat. Theme via UseThemeColors=true. Painter concerns resolved by deletion + composition. Build/test still deferred per user. |
| 2026-07-04 | **UI SECOND-PASS** | **11+3 gaps** closed in `BeepNotification.cs`: (1) `new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)` → `BeepThemesManager.ToFont(theme.TitleSmall, applyDpiScaling:true)` matching the `BeepButton.cs:742` pattern (theme tokens via `IBeepTheme.TitleSmall`/`BodyMedium`); (2/3) `AutoSize=true, AutoSizeMode=GrowAndShrink` + new `RecomputeSize()` so long messages don't get cut off; (4) `OnDpiChanged` + new `RescaleLayout()` invoked from `OnHandleCreated` and `OnDpiChangedInternal` so DPI-affected sizes (icon width, padding, progress height, min/max) re-apply with the real monitor DPI (constructor's `DeviceDpi=96` was stale); (5) collapsed nested padding; (6) close-button `\u2715` glyph with DPI-scaled Width; (7) explicit `TabIndex` (0 close, 1 title, 2 message, 3+ actions); (8) title/message 2px top padding; (9) less duplicate `NotificationClicked` (close and action buttons cancel it via TabStop); (10) `AutoEllipsis=true` on title + message so overflow clips with `…`; (11) icon hosted inside `_iconContainer` BeepPanel so themed BackColor works; (12/13/15) DPI sizes moved out of constructor into `RescaleLayout`; new `OnShown` defers focus to first action button (else close) via `BeginInvoke` so Esc→Dismiss is responsive on first key. **UI polish round:** (A) tooltips added via a private `ToolTip _toolTip` — "Close (Esc)" on the close button + percentage/text on the progress bar (refreshed every tick); (B) empty title and empty message labels hidden via `_titleLabel.Visible = title.Length > 0` so the form collapses extra space; (C) `RightToLeft = RightToLeft.Inherit` so the notification mirrors the host's locale direction. **Phase 7 partial — persistence skeleton in `BeepNotificationHistory`:** `PersistenceFilePath` property (default `%AppData%\TheTechIdea\Beep\notifications.json`, caller-overridable); `Save()` / `Load()` using `System.Text.Json` with a versioned DTO (`{ Version=2, SavedAt, Items: [...] }`); 10-second debounced auto-save started in `AddNotification` and cancelled on `ClearHistory` (which also deletes the file); final flush in `Dispose`. Build/test still deferred. |
| 2026-07-04 | **PHASE 2 COMPLETE — BeepNotificationHistory → full child-control composition** | `_listPanel` (Panel) + `_scrollBar` (VScrollBar) + all manual GDI paint handlers (`ListPanel_Paint`, `ListPanel_Click`, `ListPanel_MouseWheel`, `ScrollBar_Scroll`, `DrawHistoryItem`, `DrawEmptyMessage`, `UpdateScrollBar`) all **DELETED**. Items host is now a `Panel _itemsHost` (Dock=Fill, `AutoScroll=true`) — each row is a `BeepPanel` (Dock=Top) with three BeepLabel children (timestamp, title, message) and a type-tinted `BackColor`. `RebuildItems()` synchronizes row count with filtered items; per-row `Click` fires `HistoryItemClicked`. Empty state shown via a centered `BeepLabel`. Scroll handled natively by `AutoScroll`. Zero `new Font("Segoe UI", …)` remain. `GetTimeAgo` kept for timestamp text. All old paint code + scrollbar code removed from Dispose. Build/test still deferred. |
| 2026-07-04 | **PHASE 3 PARTIAL — BeepNotificationGroup typography** | All painter literal-fonts routed through new cached fields `_titleFont` / `_subtitleFont` / `_countFont` / `_itemTitleFont` / `_itemMsgFont` / `_itemTimeFont` sourced from `BeepFontManager.{LabelFont,HeaderFont,TooltipFont}`. Six painter call-sites (`DrawHeader` title + subtitle, `DrawBadge` count, `DrawNotificationItem` title + msg + time) now use the cached fields with `SystemFonts.MessageBoxFont` last-resort fallback; zero `new Font("Segoe UI", …)` remain. New `ApplyTypography()` helper called from constructor + `ApplyTheme` so theme changes propagate. Header-item-via-`BeepListBox` and `BeepBadge` refactors (3.1-3.5 of the master TODO) deferred — `BeepBadge` does not exist in the codebase (using a `BeepLabel` instead is straightforward), and `BeepListBox` rebind requires redesigning item rows that currently paint their own type-tinted backgrounds. Build/test still deferred. |
| 2026-07-04 | **PHASE 3 STRUCTURAL — BeepNotificationGroup → full child-control composition** | `DrawHeader` / `DrawBadge` / `DrawExpandButton` / `DrawExpandedContent` / `DrawNotificationItem` manual-paint methods all **DELETED**. Header is a `BeepPanel _headerPanel` (Dock=Top) hosting three Beep child labels. Items live in a new `BeepPanel _itemsHost` (Dock=Fill) — each notification becomes its own `BeepPanel` row (Dock=Top, type-tinted BackColor) with three BeepLabel children (timestamp top-right, title bold, message fills). `RebuildItems()` keeps the row count in sync with `_notifications` (adds/updates/disposes rows), called from `DrawContent` / `IsExpanded` setter / `AddNotification` / `RemoveNotification` / `Clear` / `ApplyTheme`. Each row's Click handler fires `NotificationClicked` directly (no form-level hit-testing needed). `DrawContent` is now a no-op — `BeepNotificationGroup` is a `BaseControl` and now paints entirely through child controls (header strip + per-item rows), so the "overriding OnPaint on a BeepiFormPro-derived" concern is moot — the override is the BaseControl extension point for inner-content paint, but with no manual paint happening it's effectively a no-op. Type-tinted BackColor sourced from `NotificationThemeHelpers` via new `ResolveTypeTint(NotificationType, IBeepTheme)`. Build/test still deferred. |
| 2026-07-04 | **PHASE 7.6 — MarkConsumed** | New `BeepNotificationManager.MarkConsumed(notificationId)` API (G28): thread-safe under `_lock`; marks the active instance matching the Id as `IsRead=true`, removes it from the stack via the existing dismiss pipeline, and raises a new `NotificationConsumed` event for any future Win11 toast bridge subscriber to mirror the dismissal in OS Action Center. No-op on empty/unknown id. `BeepNotification.Dismiss()` now forwards to the manager (only when `ActiveCount > 0` so headless Dismiss calls don't no-op) so closing the form via the close button, Esc, or auto-dismiss tick all consistently mark the data as consumed. Build/test still deferred. |
| 2026-07-04 | **PHASE 3 ACCESSIBILITY** | `BeepNotificationGroup.UpdateAccessibility()` — pushes live state into `AccessibleName` ("`{title} ({count})`" or "Notification group" when empty) and `AccessibleDescription` ("`Collapsed; press Enter to expand. N notification(s).`" ↔ "`Expanded; press Enter to collapse.`") so screen readers announce count + expand state correctly. Wired through `ApplyTheme`, `IsExpanded` setter, `AddNotification`, `RemoveNotification`, `Clear`. Build/test still deferred. |
| 2026-07-04 | **TOOLTIP STANDARDIZATION** | Removed `BeepNotification._toolTip` (the private `System.Windows.Forms.ToolTip` instance) in favour of the canonical `BaseControl.ToolTipText` property (managed centrally by `ToolTipManager`). `BeepNotification.UpdateTooltips()` now assigns `_closeButton.ToolTipText = "Close (Esc)"`, `_progressBar.ToolTipText = "<pct>% remaining"`, and each action button gets `btn.ToolTipText = "<text> (Enter)"`. `BeepNotificationGroup.UpdateToolTip()` sets `this.ToolTipText = "Notification group (N items). Press Enter to expand/collapse."` and is wired through every state mutator (`ApplyTheme`, `IsExpanded`, `AddNotification`, `RemoveNotification`, `Clear`). TooltipManager picks up the new text on the next paint — no need to invalidate. Build/test still deferred. |
| 2026-07-04 | **SMALL POLISH ROUND 4** | `BeepNotification.RecomputeSize()` now clamps BOTH Width and Height against `[MinimumSize, MaximumSize]` (previously Width-only — long messages could remain capped). Uses `SuspendLayout`/`PerformLayout`/`ResumeLayout` around the pair-assign so `AutoSize=true / AutoSizeMode=GrowAndShrink` re-evaluates against the new minimum on the next paint. Dead-field cleanup in `BeepNotificationGroup`: commented-out `_headerRect` / `_badgeRect` / `_expandButtonRect` (no live code read/wrote them since the child-control header refactor — Dock-style positioning replaced the manual rect math). Click-handler docblock updated to reflect that the chevron `BeepLabel` owns its own click semantics under mouse capture so the form-level MouseClick only fires on the empty header / content areas. Build/test still deferred. |
| 2026-07-04 | **CLEANUP PASS** | 6 deduplication items: (1) deleted `_titleFont`/`_messageFont`/`_closeFont` dead cache fields from `BeepNotification.cs` — `ApplyTypography` now assigns fonts directly to children without caching; (2) inlined `BuildChildControls()` into the `BeepNotification` constructor (same pattern as Group's inline); (3) fixed stale comment in Group "Items below still paint manually" → "Items stack as BeepPanel rows"; (4) removed dead usings: `FontManagement`, `ImagePainters`, `Labels` from Group; `FontManagement` from History; (5) deduplicated `_popupOpen` — changed from a backed field to a computed property `=> _isExpanded`, dropped the redundant `= value` assignment in the `IsExpanded` setter and the redundant `= false` in `CloseChildPopup`; (6) pre-computed `_iconTintResolved` in `ApplyData()` so `IconPicture_Paint` no longer calls `NotificationThemeHelpers.GetColorsForType()` every paint tick. Master TODO stale sections updated: Phase 2 (all ✅), Phase 3 (all ✅), Phase 7.1-7.4 (✅), Predecessor Audit "EmbeddedImagePath removed" fixed to "kept for source compat". |
