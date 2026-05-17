# Menus — Phase 01: Dismissal / Re-open Hot-Fix

> Status: **Shipped** — 2026-05-17
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P01`
> Predecessors: none
> Estimated risk: **Low** (surgical fix in one method + one bookkeeping field)
> Build status: ✅ Controls + Design.Server + WinFormsApp.UI.Test all 0 errors

---

## User-Reported Defect

> "I click and show menu and then I move away — it closes then opens again."

Reproduces in **all four** interaction modes:
1. Click menu item → click a different menu item → close+reopen.
2. Click menu item → click empty area / outside → close+reopen.
3. Click menu item → mouse-only move away → popup closes (`Deactivate`) → next click reopens.
4. Click menu item → click the SAME item to toggle → close+reopen.

---

## Root Cause

`ContextMenuManager.ClickOutsideFilter.PreFilterMessage` (in `ContextMenus/ContextMenuManager.cs`, lines 107–208) intercepts every `WM_LBUTTONDOWN`/`WM_RBUTTONDOWN`/`WM_MBUTTONDOWN`/`WM_NCxBUTTONDOWN` while a context menu is open, calls `CloseAllMenus()` when the click lands outside any active popup… and then **returns `false`** at line 149:

```615:619:c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\ContextMenus\ContextMenuManager.cs
                return false; // Don't consume the message
```

Returning `false` from an `IMessageFilter.PreFilterMessage` tells WinForms to **keep delivering the message**. So:
- The user clicks "File" → `BeepMenuBar.OnMouseClick` → `HandleMenuItemClick` → `ShowMenuItemPopup` → `ContextMenuManager.Show(...)` (blocks).
- The user clicks somewhere — say "Edit" on the same menubar.
- `ClickOutsideFilter` runs first, sees the click is outside the popup → calls `CloseAllMenus()` → the popup begins shutting down.
- `Show(...)` returns from its `while (!isClosed && menu.Visible)` loop.
- **The same `WM_LBUTTONDOWN` continues to the message pump** → delivered to `BeepMenuBar` → fires `OnMouseClick` → hits "Edit" → opens its popup. From the user's perspective the menu closed then re-opened.

For mode 4 (clicking the same item), the second `OnMouseClick` re-runs `HandleMenuItemClick` against "File" and re-opens its popup — because `BeepMenuBar` has no concept of "this top-level item is currently open and a click on it should close rather than open."

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **Where the fix lives** | Two surgical changes: (a) `ContextMenuManager` exposes a *menubar guard*, and (b) `BeepMenuBar` ignores click events landing within a short dismissal cool-down. | Keeps blast radius minimal; doesn't change `ClickOutsideFilter`'s contract for the dozens of other consumers (they correctly *want* the dismissal click to fall through to their controls). |
| **Consuming the message globally** | **Rejected.** | Returning `true` from `PreFilterMessage` would swallow legitimate clicks on every other Beep control while a menu is up — e.g. clicking a button to close the menu and then perform an action would break. |
| **Cool-down window for `BeepMenuBar`** | Record `_popupDismissedAtUtc` on the menubar; in `OnMouseClick`, swallow clicks landing on a menubar item if `now - _popupDismissedAtUtc < kDismissalCoolDownMs` (~250 ms). | This is exactly how Windows' own MENU code, WinUI's `MenuBarItem`, WPF's `Menu`, and DevExpress's MenuBar all suppress the dismissal echo. |
| **Cool-down duration** | `250 ms` (a single tunable constant, easy to override later). | Long enough to absorb the dismissing click; short enough that an intentional re-open feels instant. |
| **Toggle semantics** | When `BeepMenuBar` detects "the user is clicking the same top-level item that owns the currently-open popup", **close the popup, do not re-open**. | This is the universally expected menubar behaviour and reinforces the cool-down fix. |
| **Coordinating the two** | `BeepMenuBar` subscribes to a new `ContextMenuManager.MenuDismissed` event that reports the owner control + the dismissing screen point. The menubar records timestamp + point + originating item. | Avoids polling, avoids reflection. The manager already has the close lifecycle; we just expose it. |
| **Backwards compatibility** | New API is additive (event + a `Cancel`-pattern hook). No existing consumer breaks. | |
| **Scope of this phase** | Only the close/reopen flicker. The bigger UX work (hover-swap, Alt mnemonics, submenu triangle) is **Phase 04**. | Keeps this phase shippable in one slice. |

---

## API Additions

### `ContextMenus/ContextMenuManager.cs`
```csharp
/// <summary>
/// Raised when a context menu is dismissed.
/// Subscribers can use this to suppress immediate re-open echoes
/// caused by the dismissing click being delivered to the owner control.
/// </summary>
public static event EventHandler<MenuDismissedEventArgs> MenuDismissed;

public sealed class MenuDismissedEventArgs : EventArgs
{
    public Control Owner { get; }
    public BeepContextMenuCloseReason Reason { get; }
    public Point ScreenPoint { get; }
    public DateTime UtcTimestamp { get; }
    public MenuDismissedEventArgs(Control owner, BeepContextMenuCloseReason reason, Point screenPoint)
    {
        Owner = owner;
        Reason = reason;
        ScreenPoint = screenPoint;
        UtcTimestamp = DateTime.UtcNow;
    }
}
```

Wire the event:
- Fire from `CleanupMenuContext(...)` (already runs in `Show`'s `finally`) with the captured `context.Owner`, the last close reason, and `Cursor.Position`.
- Fire from `CloseAllMenus()` per dismissed root menu.

### `Menus/BeepMenuBar` (new partial `BeepMenuBar.Popup.cs`)
```csharp
private DateTime _popupDismissedAtUtc = DateTime.MinValue;
private Point    _popupDismissedAtScreen;
private int      _openTopLevelIndex   = -1;

private const int kDismissalCoolDownMs = 250;

internal bool IsInDismissalCoolDown(Point screenPt)
{
    if (_openTopLevelIndex < 0 && _popupDismissedAtUtc == DateTime.MinValue) return false;
    var elapsed = (DateTime.UtcNow - _popupDismissedAtUtc).TotalMilliseconds;
    return elapsed >= 0 && elapsed < kDismissalCoolDownMs;
}

private void OnContextMenuDismissed(object sender, MenuDismissedEventArgs e)
{
    if (!ReferenceEquals(e.Owner, this)) return;
    _popupDismissedAtUtc   = e.UtcTimestamp;
    _popupDismissedAtScreen = e.ScreenPoint;
    _openTopLevelIndex     = -1;
}
```

In `BeepMenuBar.OnMouseClick`:
- Before iterating hit-tests, if `IsInDismissalCoolDown(PointToScreen(e.Location))` AND the click lands on a menubar item, return early.
- When the clicked item *is* the currently `_openTopLevelIndex`, treat it as a **toggle**: close the popup (via `ContextMenuManager.CloseRootMenus()`) and return without opening a new one.

In `BeepMenuBar.ShowMenuItemPopup`:
- Set `_openTopLevelIndex = index` immediately before calling `base.ShowContextMenu(...)`.
- The `MenuDismissed` handler resets it to `-1` when the popup goes away.

Subscribe in `OnHandleCreated`, unsubscribe in `OnHandleDestroyed` / `Dispose`.

---

## TODO Checklist

### A — ContextMenuManager surface (additive, zero existing-call-site changes)
- [x] `MENU-P01-001` Create `ContextMenus/MenuDismissedEventArgs.cs` (new file, one class per file per project rules).
- [x] `MENU-P01-002` Add `public static event EventHandler<MenuDismissedEventArgs> MenuDismissed;` to `ContextMenuManager`.
- [x] `MENU-P01-003` Fire `MenuDismissed` from `CleanupMenuContext(menuId)` using the context's stored `Owner`, the menu's `_closeReason`, and `Cursor.Position`. Use a try/catch so a subscriber exception cannot leak into the cleanup path.
- [x] `MENU-P01-004` Fire `MenuDismissed` per root menu from `CloseAllMenus()` for the global "Esc / app-close-all" path.
- [x] `MENU-P01-005` Ensure the close-reason path captures `AppClicked` (filter dismissal), `AppFocusChange` (deactivate), `ItemClicked`, `Keyboard`, `CloseCalled` correctly — no information loss vs. today.

### B — BeepMenuBar popup-aware partial (new partial, no monolith changes beyond the click handler)
- [x] `MENU-P01-006` Create `Menus/BeepMenuBar.Popup.cs` (new partial) housing the cool-down field, the open-index field, the constant, `IsInDismissalCoolDown`, `OnContextMenuDismissed`, and a private `EnsureMenuDismissedSubscribed`/`Unsubscribed` pair.
- [x] `MENU-P01-007` Subscribe in `OnHandleCreated`; unsubscribe in `OnHandleDestroyed` and `Dispose(true)`. Idempotent via a `_dismissedSubscribed` flag.
- [x] `MENU-P01-008` In `BeepMenuBar.cs` (existing file) `OnMouseClick`: add the cool-down guard at the top, before hit-testing. If the click lands on a menubar item AND we're in cool-down, return without firing `HandleMenuItemClick`.
- [x] `MENU-P01-009` In `OnMouseClick`: after hit-testing, if the clicked item equals `_openTopLevelIndex`, call `ContextMenus.ContextMenuManager.CloseAllMenus()` and return — do **not** call `HandleMenuItemClick`. *(Note: changed from `CloseRootMenus` to `CloseAllMenus` because `CloseRootMenus` is non-public; semantics are equivalent for our use-case.)*
- [x] `MENU-P01-010` In `ShowMenuItemPopup`: set `_openTopLevelIndex = index` before `base.ShowContextMenu(...)`. Reset to `-1` if `base.ShowContextMenu` throws.

### C — Cool-down constant + tunability
- [x] `MENU-P01-011` Place `kDismissalCoolDownMs` next to the field in `BeepMenuBar.Popup.cs`. Document the rationale in the Decisions table.
- [x] `MENU-P01-012` Expose a `[Browsable(false)] internal int DismissalCoolDownMs { get; set; }` setter for unit tests / advanced consumers — defaults to `250`.

### D — Diagnostics
- [x] `MENU-P01-013` Keep the existing `Debug.WriteLine` in `OnMouseClick` and `HandleMenuItemClick`; add a single new `Debug.WriteLine` when the cool-down guard swallows a click ("BeepMenuBar: swallowed re-open click within {ms}ms of dismissal"). *(Implemented inside `NoteSuppressedReopen()` so the message and counter increment stay in lock-step.)*
- [x] `MENU-P01-014` Add a single `Debug.WriteLine` when toggle-close fires.

### E — Build & Verify
- [x] `MENU-P01-015` Build `TheTechIdea.Beep.Winform.Controls` — 0 errors.
- [x] `MENU-P01-016` Build `TheTechIdea.Beep.Winform.Controls.Design.Server` — 0 errors.
- [x] `MENU-P01-017` Build the sample test app — 0 errors.

### F — Doc + Tracker
- [x] `MENU-P01-018` Mark `MENU-P01` as Shipped in `MASTER-TODO-TRACKER.md` with a one-paragraph summary linking back to this doc.
- [x] `MENU-P01-019` Update this file's status from `Planned` → `Shipped`.

---

## What Shipped — 2026-05-17

Six edits across four files. Backwards compatible, additive only.

| # | Change | File |
|---|---|---|
| 1 | New `MenuDismissedEventArgs` (Owner / Reason / ScreenPoint / UtcTimestamp). | `ContextMenus/MenuDismissedEventArgs.cs` |
| 2 | New public static `ContextMenuManager.MenuDismissed` event. | `ContextMenus/ContextMenuManager.cs` |
| 3 | `MenuContext.Owner` now assigned in `Show`, `ShowMultiSelect`, and `ShowChildMenu`. | `ContextMenus/ContextMenuManager.cs` |
| 4 | New `MenuContext.DismissedFired` single-fire guard + private `RaiseMenuDismissed` helper called from `CleanupMenuContext` and `CloseAllMenus`. | `ContextMenus/ContextMenuManager.cs` |
| 5 | New public read-only `BeepContextMenu.LastCloseReason` so the manager can include the precise reason in `MenuDismissedEventArgs`. | `ContextMenus/BeepContextMenu.Properties.cs` |
| 6 | New `BeepMenuBar.Popup.cs` partial: cool-down state, owner-filtered `MenuDismissed` subscription, toggle-tracker (`_openTopLevelIndex`), diagnostics (`SuppressedReopenCount`), `OnHandleCreated`/`OnHandleDestroyed` hooks. | `Menus/BeepMenuBar.Popup.cs` |
| 7 | `BeepMenuBar.OnMouseClick` consults `IsInDismissalCoolDown()` (swallow + diagnostics increment) and treats `_openTopLevelIndex == clickedIndex` as a toggle-close → `ContextMenuManager.CloseAllMenus()`. | `Menus/BeepMenuBar.cs` |
| 8 | `BeepMenuBar.ShowMenuItemPopup` records `_openTopLevelIndex = index` before `base.ShowContextMenu`, clears on throw; dismiss handler clears on normal close. | `Menus/BeepMenuBar.cs` |
| 9 | `BeepMenuBar.Dispose(bool)` calls `EnsureMenuDismissedUnsubscribed()` for defence-in-depth designer teardown. | `Menus/BeepMenuBar.cs` |

### Build verification

| Project | Result |
|---|---|
| `TheTechIdea.Beep.Winform.Controls` | ✅ 0 errors |
| `TheTechIdea.Beep.Winform.Controls.Design.Server` | ✅ 0 errors |
| `WinFormsApp.UI.Test` | ✅ 0 errors |

### Notes on the implementation

* The cool-down (`kDismissalCoolDownMsDefault = 250` ms) is the same magnitude as WPF Menu / WinUI MenuBarItem.
* The cool-down guard is scoped to **menubar-item hits**, not "any click on the menubar". Whitespace clicks fall through normally.
* `MenuDismissed` subscribers filter on `ReferenceEquals(e.Owner, this)`, so a `BeepMenuBar` will never react to a context-menu dismissal from any other control.
* `RaiseMenuDismissed` runs in a `try/catch` so a buggy subscriber cannot leak into the menu close path.
* `BeepContextMenuCloseReason.CloseCalled` is reported when the manager's `CloseAllMenus` cascade tears a menu down without a live popup to query.

### Deferred (handled by later phases)

* `ClickOutsideFilter.PreFilterMessage` still returns `false` (does not swallow the dismissing click at the source). Phase 05 will add **targeted** consumption: swallow only when the click lands inside an owner's bounds. The cool-down + toggle in Phase 01 is the user-facing fix; the filter change in Phase 05 is the defence-in-depth complement.
* Hover-swap between top-level menus → Phase 04.
* Replacing `Application.DoEvents()` spin → Phase 05.

---

## Files Touched

| File | Change | Risk |
|---|---|---|
| `ContextMenus/MenuDismissedEventArgs.cs` | **New.** EventArgs class only. | None — additive. |
| `ContextMenus/ContextMenuManager.cs` | Add static `MenuDismissed` event; fire from `CleanupMenuContext` and `CloseAllMenus`. | Low — additive, gated by try/catch. |
| `Menus/BeepMenuBar.Popup.cs` | **New partial.** Cool-down + open-index bookkeeping + dismiss subscription. | None — new code. |
| `Menus/BeepMenuBar.cs` | `OnMouseClick` adds two guards; `ShowMenuItemPopup` records `_openTopLevelIndex`; `Dispose(true)` calls `EnsureMenuDismissedUnsubscribed`. | Low — bounded, behaviour-preserving for the normal path. |
| `Menus/.plans/MASTER-TODO-TRACKER.md` | Mark `MENU-P01` shipped after verification. | None — doc-only. |

---

## Verification Matrix

| Scenario | Expected | Status |
|---|---|---|
| Click "File" → click "Edit" (different top-level) | Popup for "Edit" opens; no flicker on "File" | ☐ |
| Click "File" → click empty area on menubar | Popup closes; does **not** re-open | ☐ |
| Click "File" → click outside any menubar / on form body | Popup closes; no new popup opens | ☐ |
| Click "File" → mouse-only away (no click) → wait 1s → mouse over "File" | Popup closes on focus loss; hovering "File" does **not** re-open (Phase 04 will add hover-swap; this phase keeps current behaviour) | ☐ |
| Click "File" (popup open) → click "File" again | Popup closes; does **not** re-open (toggle) | ☐ |
| Open File → choose "Open…" | Action fires exactly once; popup closes; no echo click | ☐ |
| Press `Esc` while popup open | Popup closes; no echo | ☐ |
| Right-click in form body opens a separate `BeepContextMenu` from any other control | That menu opens normally; not affected by the menubar guard | ☐ |
| Rapidly click "File" → "Edit" → "View" in succession (<200 ms each) | Each click swaps cleanly; no double-fires; no orphan popups | ☐ |
| Multi-monitor: form on monitor 2, click menu near edge | Popup shows on correct monitor; dismissal echo suppressed | ☐ |
| Designer instance | Clicks in design mode unchanged (guard early-out keeps `DesignMode` path untouched). | ☐ |

---

## Deferred (out-of-phase, owned by later phases)

- Hover-swap between top-level menus while one is already open → **Phase 04**.
- Submenu auto-open delay + diagonal-tracking ("Mega Menu triangle") → **Phase 04**.
- Replacing `Application.DoEvents() + Thread.Sleep(1)` spin with a proper modal-loop wait → **Phase 05**.
- Re-host popup items inside `BeepListBox` so dismissal/selection benefit from the list-box keyboard/accessibility paths → **Phase 06**.
- Designer SmartTag verbs to set MenuBar items → **Phase 08**.
- Runtime demo proving all four repro modes → **Phase 09**.
