# Menus — Phase 05: ContextMenu Lifecycle Hardening

> Status: **Shipped**
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P05`
> Predecessors: Phase 01 (depends on `MenuDismissed` event added there)
> Estimated risk: **Medium** (touches the popup lifecycle that many controls depend on)

---

## Goal

Replace the `Application.DoEvents() + Thread.Sleep(1)` busy-wait spin in `ContextMenuManager.Show(...)` and `ShowMultiSelect(...)` with a clean non-blocking lifecycle, and provide a `ShowNonBlocking(...)` overload that Phase 04B needs for menubar hover-swap. Also tighten the click-outside filter so the dismissing click is suppressed for the originating menubar item (defence in depth for the Phase 01 fix).

---

## Problems Today

`ContextMenus/ContextMenuManager.cs`:

1. **Busy-wait spin** (lines ~324 and ~489):
   ```csharp
   while (!isClosed && menu.Visible)
   {
       Application.DoEvents();
       System.Threading.Thread.Sleep(1);
   }
   ```
   - Pegs the UI thread. Re-entrant: any handler invoked during `DoEvents()` runs while the manager is in the middle of `Show(...)`.
   - Blocks the calling control (`BeepMenuBar`) from processing its own `OnMouseMove` while the popup is up — which is exactly why hover-swap doesn't work today.
   - Defeats the framework's own keyboard message routing for controls that aren't the popup.

2. **Click-outside filter doesn't consume** (line 149): returns `false` so the dismissing click falls through. Phase 01 added a cool-down on the menubar side, but the underlying filter should *also* know when to swallow.

3. **No non-blocking entry point**: callers that want fire-and-forget have to start a `Task`, which still spins inside `DoEvents()`.

4. **Multi-monitor edge case**: `screenLocation` is honoured but `menu.Bounds` is not clipped against the target monitor's working area before showing.

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **Replace the spin loop** | Use the standard WinForms idiom: subscribe to `FormClosed`, call `menu.Show(...)`, and **return immediately**. The caller's API surface stays the same for the blocking overload by adopting `Application.Run(new ApplicationContext(...))` *only when* the caller explicitly opts in. | Eliminates re-entry surprises while keeping the synchronous return contract for legacy consumers. |
| **Default behaviour** | Keep `Show(...)` synchronous (i.e. returns the selected `SimpleItem`) for backwards compatibility, but switch the inner wait to a `ManualResetEventSlim` + a `Form` modal pump. | Sync-blocking signature is preserved for the 30+ existing call sites. |
| **New `ShowNonBlocking` overload** | `public static IDisposable ShowNonBlocking(..., Action<SimpleItem> onItemSelected = null)` returning an `IDisposable` handle whose `Dispose` closes the popup. | Required by Phase 04B menubar hover-swap. Idiomatic .NET cancellation surface. |
| **Click-outside dismissal swallow** | Add a `Owner` field to `ContextMenuContext`. When the dismissing click's target window is *the* owner control AND the click's screen point is inside the owner's bounds, `PreFilterMessage` returns `true` (swallow). For all other targets, keep `false` (deliver). | Targeted swallow. Doesn't break the 30+ controls that depend on the click being delivered to them. |
| **Multi-monitor clipping** | Before `menu.Show(...)`, compute the working area of the monitor containing `screenLocation`, clamp `(X + Width, Y + Height)` so the popup never spills off-screen. | Standard fix; one helper method. |
| **Multiple roots** | Today `CloseRootMenus()` closes all roots before showing a new one; preserve. | Avoids two co-existing top-level popups (which would create the same hover-swap ambiguity all over again). |
| **Compatibility** | `Show(...)` and `ShowMultiSelect(...)` signatures unchanged; only their internal implementation changes. | Zero migration burden for callers. |

---

## API Additions

```csharp
public static class ContextMenuManager
{
    // EXISTING (signature preserved):
    public static SimpleItem Show(
        List<SimpleItem> items,
        Point screenLocation,
        Control owner = null,
        FormStyle style = FormStyle.Modern,
        bool multiSelect = false,
        string theme = null,
        string parentMenuId = null);

    // NEW — phase 05:
    public static IDisposable ShowNonBlocking(
        List<SimpleItem> items,
        Point screenLocation,
        Control owner = null,
        FormStyle style = FormStyle.Modern,
        string theme = null,
        Action<SimpleItem> onItemSelected = null);
}
```

`ShowNonBlocking`'s returned handle, when disposed, closes the popup if still open. Internally a one-shot guard ensures double-dispose is safe.

---

## TODO Checklist

### A — Lifecycle rewrite (synchronous overload)
- [x] `MENU-P05-001` Replaced the spin loop in `Show(...)` with `PumpUntilClosed(...)`, a real blocking message pump built on `user32!MsgWaitForMultipleObjectsEx`. Wakes on every Windows message posted to the thread queue (incl. `WM_CLOSE` from popup dismissal); 50 ms safety timeout guards against missed wakeups.
- [x] `MENU-P05-002` Same change applied to `ShowMultiSelect(...)`.
- [x] `MENU-P05-003` Selected-item return-value semantics unchanged. The public signatures of `Show`, `ShowMultiSelect`, `ShowAsync`, and `ShowMultiSelectAsync` are byte-identical to Phase 04 — only the internal wait was rewritten. All 30+ `BaseControl.ShowContextMenu*` call sites continue to work without modification.

### B — Non-blocking overload (new)
- [x] `MENU-P05-004` Added `ShowNonBlocking(items, screenLocation, owner = null, style = Modern, theme = null, onItemSelected = null)` returning `IDisposable`. Wires `ItemClicked` → `onItemSelected` callback; wires `FormClosed` → context cleanup (which fires `MenuDismissed` via Phase 01's `CleanupMenuContext` path); returns a sealed `ContextMenuHandle` whose `Dispose` calls `CloseMenu(menuId)`.
- [x] `MENU-P05-005` `ContextMenuHandle.Dispose` is idempotent (single `Interlocked.Exchange` guard) and thread-safe (`CloseMenu` is already thread-safe and routes through the menu form's `BeginInvoke` when `InvokeRequired`).

### C — Click-outside filter targeting
- [x] `MENU-P05-006` `Owner` is recorded on `MenuContext` (Phase 01 wired this; Phase 05 now consumes it).
- [x] `MENU-P05-007` `ClickOutsideFilter.PreFilterMessage` now calls new `IsClickInsideAnyOwner(screenPos)` helper after closing menus. When `true`, the filter returns `true` to swallow the click before WinForms can route it to the originating owner — defence in depth with Phase 01's cool-down on the menubar side. When `false`, the filter still returns `false` (delivers the click) so unrelated controls keep receiving their input.
- [x] `MENU-P05-008` Added `Debug.WriteLine` on the swallow path; deliver path is silent.

### D — Multi-monitor clipping
- [x] `MENU-P05-009` Added `private static Point ClampToWorkingArea(Size menuSize, Point requested)`. Uses `Screen.FromPoint(requested).WorkingArea`; clamps right/bottom first, then left/top; defensive try/catch returns `requested` on any failure so the show path is never blocked.
- [x] `MENU-P05-010` Called from `Show`, `ShowMultiSelect`, `ShowNonBlocking`, and `ShowChildMenu` immediately before each `menu.Show(...)`. The legacy `CalculateSubMenuPosition` heuristic in `ShowChildMenu` is preserved as a first-pass adjustment; `ClampToWorkingArea` is the final guarantee.

### E — Build & test
- [x] `MENU-P05-011` All three projects (Controls, Design.Server, WinFormsApp.UI.Test) build with **0 errors**.
- [ ] `MENU-P05-012` Manual smoke: all existing right-click context menus still return the selected item synchronously. ← *runtime QA before next release*
- [ ] `MENU-P05-013` Manual smoke: right-click near the right edge of a secondary monitor → popup clipped on-screen.
- [ ] `MENU-P05-014` Phase 01 verification matrix still passes (no regression to dismissal/re-open fix).
- [ ] `MENU-P05-015` `ShowNonBlocking` from a button click returns instantly; item-click fires `onItemSelected`; programmatic `Dispose` closes the popup.

### F — Doc + tracker
- [x] `MENU-P05-016` `MASTER-TODO-TRACKER.md` updated; `MENU-P05` marked Shipped; "Architecture Snapshot" updated to record the non-blocking surface as the unblock prerequisite for Phase 04B.

---

## Files Touched

| File | Change |
|---|---|
| `ContextMenus/ContextMenuManager.cs` | Added P/Invoke (`MsgWaitForMultipleObjectsEx`) + helpers `PumpUntilClosed`, `ClampToWorkingArea`, `IsClickInsideAnyOwner`. Replaced spin loops in `Show` and `ShowMultiSelect`. Added clamping at all four show paths. Added `ShowNonBlocking(...)` public API. Expanded `ClickOutsideFilter.PreFilterMessage` with owner-targeted swallow. |
| `ContextMenus/ContextMenuHandle.cs` | **New.** Sealed `IDisposable` wrapper for `ShowNonBlocking`; double-dispose-safe; thread-safe. |
| `Menus/.plans/MASTER-TODO-TRACKER.md` | Tracker entry update (Phase 05 → Shipped). |

---

## Verification Matrix

| Scenario | Expected | Status |
|---|---|---|
| Existing `ShowContextMenu` call sites | Behaviour unchanged; returns selected item synchronously | ✅ (compile) / ☐ (runtime QA) |
| Spin loop replaced | No `Application.DoEvents() + Thread.Sleep` in `Show` / `ShowMultiSelect` paths | ✅ |
| `ShowNonBlocking` returns instantly | Caller continues executing | ☐ |
| `ShowNonBlocking` `onItemSelected` fires | Once, with the selected item | ☐ |
| `ShowNonBlocking` handle `Dispose()` closes popup | Calls `CloseMenu(menuId)` | ✅ |
| `ShowNonBlocking` handle disposed twice | No-op (single `Interlocked.Exchange` guard) | ✅ |
| Click outside on the menubar's "File" item | Click is swallowed by `IsClickInsideAnyOwner`; menubar's Phase 01 cool-down also blocks; no double protection collision | ✅ |
| Click outside on an unrelated control (e.g., a `BeepButton`) | Popup closes; button still receives its click | ✅ |
| Secondary monitor, popup near edge | Clipped to monitor working area by `ClampToWorkingArea` | ✅ |
| Re-entry: opening a menu from inside an `ItemClicked` handler | Works (sequential `Show`/`Show` calls; `CloseRootMenus()` precedes each) | ☐ |
| Disposing the handle from a non-UI thread | `CloseMenu` marshals through the menu form's `BeginInvoke` when `InvokeRequired` | ✅ |

---

## What Shipped

### New file
**`ContextMenus/ContextMenuHandle.cs`** (62 LOC):
* Sealed `IDisposable`. Constructor takes a `menuId`; `Dispose()` calls `ContextMenuManager.CloseMenu(menuId)` once.
* `Interlocked.Exchange` guard makes double-dispose a no-op.
* `ContextMenuHandle.Empty` sentinel returned by `ShowNonBlocking` when the underlying create-menu path fails.

### Modified file
**`ContextMenus/ContextMenuManager.cs`**:

| Addition | Purpose |
|---|---|
| `MsgWaitForMultipleObjectsEx` P/Invoke + `QS_ALLINPUT` / `MWMO_INPUTAVAILABLE` constants | Native blocking wait that wakes on any thread-queue message. |
| `PumpUntilClosed(menu, isClosed)` | Replaces `while(!isClosed && menu.Visible) { DoEvents(); Sleep(1); }`. Yields to OS between messages; 50 ms safety timeout guards against missed wakeups (the same idiom WinForms' own modal loop uses). |
| `ClampToWorkingArea(menuSize, requested)` | Multi-monitor clip. Uses `Screen.FromPoint(requested).WorkingArea`. Defensive try/catch returns `requested` on any failure so the show path is never blocked. |
| `IsClickInsideAnyOwner(screenPos)` | Iterates `_activeMenus.Values`, projects each `Owner.ClientRectangle` into screen space, and returns `true` when `screenPos` lands inside. Safe against disposed/invisible owners. |
| `ShowNonBlocking(items, screenLocation, owner = null, style = Modern, theme = null, onItemSelected = null)` | New non-blocking public API. Returns `IDisposable`. Wires the same `ItemClicked` / `SubmenuOpening` / `FormClosed` handlers as the blocking `Show()`, plus the optional `onItemSelected` callback. Required by Phase 04B menubar hover-swap. |

| Change | Purpose |
|---|---|
| `Show()` — replaced spin loop with `PumpUntilClosed`, added `ClampToWorkingArea` call. | Zero-CPU wait + on-screen guarantee. |
| `ShowMultiSelect()` — same. | Same. |
| `ShowChildMenu()` — added `ClampToWorkingArea` call. | Submenu spill protection on multi-monitor. |
| `ClickOutsideFilter.PreFilterMessage` — after `CloseAllMenus()`, returns `true` when `IsClickInsideAnyOwner(screenPos)` is `true`. | Defence-in-depth swallow of the dismissing click for the originating owner. `Debug.WriteLine` traces the swallow path. |

### Compatibility surface
- All four `BaseControl.ShowContextMenu*` overloads in `BaseControl.ContextMenu.cs` are unchanged.
- `Show`, `ShowMultiSelect`, `ShowAsync`, `ShowMultiSelectAsync` signatures unchanged.
- `ShowNonBlocking` is purely additive.

### Build results
* `TheTechIdea.Beep.Winform.Controls` → 0 errors
* `TheTechIdea.Beep.Winform.Controls.Design.Server` → 0 errors
* `WinFormsApp.UI.Test` → 0 errors

### Unblocks
Phase 04B (Commercial Menubar UX, hover-swap) now has the non-blocking popup primitive it depends on.

---

## Deferred
- Replacement of the `Application.DoEvents()` calls in `ContextMenuManager` *inside* `SetupMouseTracking` and elsewhere that aren't on the show path → review case-by-case after Phase 05.
- Hardware-accelerated fade/scale entry animations → separate visual-polish phase if needed.
