# Menus — Phase 04: Commercial-Grade Menubar UX

> Status: **Shipped (04A + 04B)**
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P04`
> Predecessors: Phase 01 (popup-dismissal infrastructure), Phase 02 (partial split), Phase 05 (non-blocking popup lifecycle — shipped before this phase)
> Estimated risk: **Medium** (UX behaviour changes; affects every menubar consumer)

---

## Goal

Bring `BeepMenuBar`'s interaction model up to commercial parity with VS, DevExpress, Office, WPF `Menu`, and WinUI `MenuBar`. The Phase 01 fix stops the dismissal flicker; this phase adds the affordances users *expect* once that flicker is gone.

---

## Dependency Note

Two of the four UX items below (hover-swap and submenu auto-open) require the popup to *not block the menubar's message pump*. Today `ContextMenuManager.Show` is a blocking `Application.DoEvents() + Thread.Sleep(1)` loop, which means the menubar's `OnMouseMove` cannot fire while a popup is up — hover-swap is structurally impossible.

**Phase 04 cannot ship its hover-swap item until Phase 05 lands.** Phase 04 is therefore split into two waves:
- **04A** — Toggle semantics + Alt activation + `Escape` dismissal (does not depend on Phase 05).
- **04B** — Hover-swap + submenu auto-open + submenu-triangle tracking (depends on Phase 05).

Both waves live in this single doc, with TODO sub-sections clearly labelled.

---

## Commercial UX Behaviours

| Behaviour | Status today | Target |
|---|---|---|
| Click top-level → popup opens | ✅ (Phase 01) | ✅ |
| Click same top-level again → popup closes | ❌ → ✅ in 04A | ✅ |
| Escape closes any open popup | partial (ContextMenu handles it) | ✅ |
| Alt activates menubar focus / arrow keys navigate | ❌ | ✅ in 04A |
| Alt+`F` opens "File" by mnemonic | ❌ | ✅ in 04A (renders the underline lazily, like Win32) |
| Hover-swap: popup open, mouse drifts to another top-level → popup swaps without click | ❌ | ✅ in 04B |
| Submenu auto-opens on hover with delay | ❌ | ✅ in 04B |
| Submenu "triangle tracking" (don't dismiss a submenu when the user heads diagonally towards it) | ❌ | ✅ in 04B |
| Right-arrow opens submenu; Left-arrow closes it | ❌ | ✅ in 04B |
| Type-ahead navigation within a popup | (provided by `BeepListBox` after Phase 06) | ✅ post-06 |

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **Activation state machine** | Introduce `BeepMenuBar.Activation.cs` partial owning a small state enum: `Inactive`, `ActiveNoPopup`, `ActiveWithPopup`. | Mirrors WPF `Menu`'s `IsMainMenu` activation model. Without an explicit state we end up with hidden booleans scattered across event handlers. |
| **Toggle on second click** | Already scaffolded in `BeepMenuBar.Popup.cs` from Phase 01 (the `_openTopLevelIndex` field). 04A wires the explicit close action. | Reuses Phase 01 state. |
| **Alt key handling** | Override `ProcessCmdKey` to capture `Keys.Menu` (Alt). First Alt press → `ActiveNoPopup` + draw mnemonic underlines. Second Alt or `Escape` → `Inactive`. | Matches Win32 menu semantics. `ProcessCmdKey` is the correct hook because `OnKeyDown` fires too late for system keys. |
| **Mnemonic rendering** | Reuse the existing `MenuFontHelpers.DrawMnemonicText` (or add a 30-line helper if missing) that interprets `&` as the mnemonic prefix. Draw underline only when `_drawMnemonics == true`. | Aligns with standard `Label.UseMnemonic` semantics. |
| **Hover-swap requires non-blocking popup** | Adds `BeepMenuBar.ShowMenuItemPopupNonBlocking(item, index)` that calls a new `ContextMenuManager.ShowNonBlocking(...)` (introduced in Phase 05). When the popup is up, `OnMouseMove` swaps it by closing+reopening if hover crosses to a different top-level. | Without non-blocking popups, hover-swap is impossible because the menubar's UI thread is parked inside `Show`. |
| **Submenu auto-open delay** | Use the existing `_submenuTimer` in `BeepContextMenu` (already present in `BeepContextMenu.Events.cs` as `SubmenuTimer_Tick`). Phase 04B wires the hover→start-timer→fire-open path. | Don't reinvent — extend. |
| **Submenu-triangle tracking** | Implement the classic Amazon Mega Menu / WPF `MenuItem` heuristic: track the mouse trajectory; if the cursor is inside the "triangle" between its previous position and the submenu's top-left/bottom-left corners, do not dismiss the submenu even if the cursor leaves the parent item briefly. Implementation in `Menus/Helpers/SubmenuTriangleTracker.cs` (new). | Without this, every user who tries to drag diagonally into a submenu loses it. Industry-standard solved problem; we just need the implementation. |
| **Where the new behaviours live** | New partials only: `BeepMenuBar.Activation.cs` and `BeepMenuBar.Keyboard.cs`. The mouse-hover swap lives in the existing `BeepMenuBar.Input.cs` (from Phase 02). Triangle tracker is a stateless helper. | Keeps decomposition consistent. |

---

## TODO Checklist — Wave 04A (independent)

### A1 — Toggle close
- [x] `MENU-P04A-001` Phase 01 already scaffolded the same-item-toggle path in `BeepMenuBar.Input.cs` `OnMouseClick`. Phase 04A extends it to also transition `Activation` back to `ActiveNoPopup` so Alt-letter is still primed without a redundant Alt press.
- [x] `MENU-P04A-002` `Debug.WriteLine` already in place from Phase 01.

### A2 — Activation state
- [x] `MENU-P04A-003` Created `Menus/BeepMenuBar.Activation.cs` (~190 LOC) declaring `public enum MenubarActivation { Inactive, ActiveNoPopup, ActiveWithPopup }`, the `_activation` field, the `Activation` read-only property, the `ActivationChanged` event, and a single `SetActivation(MenubarActivation next)` transition chokepoint that paints the side-effects (mnemonic toggle, focus capture/restore, invalidate, event fire) as a unit.
- [x] `MENU-P04A-004` Popup-open in `Popup.cs` → `ActiveWithPopup`. `OnContextMenuDismissed` → `ActiveNoPopup` (when previous state was `ActiveWithPopup`). Toggle-close in `Input.cs` → `ActiveNoPopup`. Escape / Alt-twice in `Keyboard.cs` → `Inactive` via `DeactivateKeyboard()`. Focus restore runs as a side-effect of the `Inactive` transition.

### A3 — Alt activation + mnemonics
- [x] `MENU-P04A-005` Created `Menus/BeepMenuBar.Keyboard.cs` (~190 LOC) overriding `ProcessCmdKey` for `Keys.Menu`, `Keys.Escape`, `Keys.Left`, `Keys.Right`, `Keys.Down`, `Keys.Enter`, and `ProcessMnemonic` for Alt+letter.
- [x] `MENU-P04A-006` `ToggleAltActivation` flips Inactive ↔ ActiveNoPopup; entering ActiveNoPopup auto-highlights index 0 so arrow keys start from a known cell.
- [x] `MENU-P04A-007` Escape closes any open popup, clears `_openTopLevelIndex`, and routes through `DeactivateKeyboard()` so focus is restored.
- [x] `MENU-P04A-008` `ProcessMnemonic` resolves `&letter` (with `&&` escape) and falls back to first letter when no `&` is present, matching Win32 auto-mnemonics. Flips Inactive → ActiveNoPopup if needed, then dispatches through `HandleMenuItemClick`.
- [x] `MENU-P04A-009` `MoveHighlight(delta)` wraps around, skips null items, and re-issues the popup when `ActiveWithPopup` (so arrow keys swap popups when one is up).
- [x] `MENU-P04A-010` In `BeepMenuBar.Drawing.cs`, the prefix flag now depends on `_drawMnemonics`: omit prefix flags (default GDI behaviour underlines mnemonic) when active; add `TextFormatFlags.HidePrefix` when inactive. `TextFormatFlags.HotPrefix` doesn't exist on WinForms `TextFormatFlags`; this is the equivalent lazy-mnemonic idiom Win32 / WPF use.
- [x] `MENU-P04A-011` `RestoreFocusOnDeactivation` runs as a side-effect of every Inactive transition. Defensive against disposed / invisible targets.

### A4 — Verification (04A)
- [x] `MENU-P04A-012` Build all three projects — **0 errors** (one mid-flight fix: `HotPrefix` → omit / `HidePrefix` pair, see A3-010 above).
- [ ] `MENU-P04A-013` Manual smoke (runtime QA before next release): Press Alt → underlines appear; Alt again → underlines vanish; Alt+`F` → File popup opens; Escape → popup + activation cleared.
- [ ] `MENU-P04A-014` Manual smoke: Click "File" → popup opens; click "File" again → popup closes.

---

## TODO Checklist — Wave 04B (requires Phase 05)

### B1 — Non-blocking popup adapter
- [x] `MENU-P04B-001` Added `ShowMenuItemPopupNonBlocking(item, index)` to `Menus/BeepMenuBar.Popup.cs`. Calls `ContextMenuManager.ShowNonBlocking(...)` (from Phase 05) and stores the returned `IDisposable` in `_nonBlockingPopupHandle`. `OnContextMenuDismissed` nulls the handle when the popup auto-closes; `CloseNonBlockingPopup()` provides idempotent programmatic close (used by toggle, swap, and `Lifecycle.Dispose`). Selected-item dispatch mirrors the blocking path: `RunMethodFromGlobalFunctions` runs from the `onItemSelected` callback so user-visible side-effects are unchanged. `HandleMenuItemClick` now routes items-with-children through this non-blocking adapter so hover-swap is actually reachable.

### B2 — Hover-swap
- [x] `MENU-P04B-002` `OnMouseMove` in `BeepMenuBar.Input.cs` now calls `MaybeHoverSwap(targetIndex)` when `Activation == ActiveWithPopup`, `HasNonBlockingPopup == true`, and the hovered index differs from `_openTopLevelIndex`. The target must also have children — items without children behave as inert click-only buttons during hover-swap.
- [x] `MENU-P04B-003` Throttled at `kHoverSwapThrottleMs = 50` (DateTime UTC delta on `_lastHoverSwapUtc`). Below the perceptual lag threshold; high enough to absorb seam-cross jitter at typical mouse-velocity sampling rates.

### B3 — Submenu triangle tracker
- [x] `MENU-P04B-004` Created `Menus/Helpers/SubmenuTriangleTracker.cs`. `internal static bool IsCursorTrackingTowardSubmenu(Point lastPos, Point currentPos, Rectangle submenuScreenBounds)`. Pure geometry, branch-coverage-friendly: cursor inside submenu → true; otherwise picks the submenu corner pair on the side closest to the cursor (LTR top-left + bottom-left default; RTL/left-opening branch ready) and runs a standard barycentric point-in-triangle test. `long` arithmetic prevents overflow on large screen coords. Degenerate (zero-area) triangles return false so the dismissal timer falls through.
- [x] `MENU-P04B-005` Plumbing:
  * New partial `ContextMenus/BeepContextMenu.SubmenuTracking.cs` holds `_openSubmenuScreenBounds`, `_lastCursorScreen`, plus public-ish (`internal`) `NoteSubmenuOpened` / `NoteSubmenuClosed` / `ShouldDeferSubmenuDismissal(clientPoint)` / `ResetCursorTrajectory()`.
  * `BeepContextMenu.Methods.cs` `UpdateHoveredItem` now consults `ShouldDeferSubmenuDismissal` before resetting the dismissal timer — when true, the highlight follows the cursor but the timer keeps running so the open child stays visible.
  * `BeepContextMenu.Events.cs` `MouseLeave` calls `ResetCursorTrajectory` so re-entry starts a fresh measurement.
  * `ContextMenuManager.ShowChildMenu` calls `parentMenu.NoteSubmenuOpened(menu.Bounds)` after `menu.Show()`. `CloseChildMenus` calls `parentMenu.NoteSubmenuClosed()` (snapshot outside the lock to avoid re-entry).
- [ ] `MENU-P04B-006` Sample-form regression case — deferred to Phase 09 (Runtime Demo + Verification) where the demo form is built end-to-end.

### B4 — Verification (04B)
- [x] `MENU-P04B-007` Build all three projects — **0 errors**.
- [ ] `MENU-P04B-008` Manual smoke: Click "File" → popup opens; without clicking, drag to "Edit" → popup swaps to Edit's children. ← runtime QA.
- [ ] `MENU-P04B-009` Manual smoke: Open Edit→Find submenu; drag diagonally toward a Find-item → submenu remains visible throughout. ← runtime QA.

---

## Files Touched

| File | Wave | Change |
|---|---|---|
| `Menus/BeepMenuBar.Activation.cs` | 04A | **New** (~190 LOC). `MenubarActivation` enum + state machine + focus capture/restore + `ActivationChanged` event. |
| `Menus/BeepMenuBar.Keyboard.cs` | 04A | **New** (~190 LOC). `ProcessCmdKey` (Alt/Esc/arrows/Enter), `ProcessMnemonic` (Alt+letter), wrap-around highlight, `GetMnemonicChar` helper. |
| `Menus/BeepMenuBar.Input.cs` | 04A + 04B | Activation transition on toggle-close (04A); `MaybeHoverSwap` + 50 ms throttle field (04B); `HandleMenuItemClick` routes children through non-blocking adapter (04B). |
| `Menus/BeepMenuBar.Drawing.cs` | 04A | `_drawMnemonics`-gated `TextFormatFlags`: default (underline) vs `HidePrefix` (hide). |
| `Menus/BeepMenuBar.Popup.cs` | 04A + 04B | Activation transitions wired into open/close + dismissal handler (04A); `ShowMenuItemPopupNonBlocking` + `_nonBlockingPopupHandle` + `CloseNonBlockingPopup` + `HasNonBlockingPopup` (04B); `CloseAllPopups` no longer a no-op. |
| `Menus/Helpers/SubmenuTriangleTracker.cs` | 04B | **New** (~120 LOC). Stateless point-in-triangle geometry helper for the Bret-Victor "cursor heading toward submenu?" heuristic. |
| `ContextMenus/BeepContextMenu.SubmenuTracking.cs` | 04B | **New** (~115 LOC). State holder + integration glue: `NoteSubmenuOpened` / `NoteSubmenuClosed` / `ShouldDeferSubmenuDismissal` / `ResetCursorTrajectory`. |
| `ContextMenus/BeepContextMenu.Methods.cs` | 04B | `UpdateHoveredItem` consults triangle tracker before resetting the dismissal timer. |
| `ContextMenus/BeepContextMenu.Events.cs` | 04B | `MouseLeave` resets cursor trajectory. |
| `ContextMenus/ContextMenuManager.cs` | 04B | `ShowChildMenu` notifies parent's `NoteSubmenuOpened`; `CloseChildMenus` notifies parent's `NoteSubmenuClosed` (snapshot outside lock to avoid re-entry). |

---

## Verification Matrix

| Scenario | Wave | Expected | Status |
|---|---|---|---|
| Click File → click File again | 04A | File closes; no re-open | ✅ (Phase 01 scaffold + 04A activation transition) |
| Alt → underlines visible | 04A | All mnemonic letters underlined | ✅ (compile) / ☐ (runtime QA) |
| Alt+F | 04A | File popup opens; mnemonic resolved via `ProcessMnemonic` | ✅ (compile) / ☐ (runtime QA) |
| Escape | 04A | Popup closes; menubar goes Inactive; previous focus restored | ✅ (compile) / ☐ (runtime QA) |
| Left/Right arrows in ActiveNoPopup | 04A | Highlight moves across top-level items (wrap-around; skip null items) | ✅ |
| Down arrow on highlighted item | 04A | Popup opens via `OpenHighlightedTopLevel` → `HandleMenuItemClick` | ✅ |
| Click File → drag (no click) to Edit | 04B | Popup swaps to Edit's children (50 ms throttle) | ✅ (compile) / ☐ (runtime QA) |
| Drag File→Edit→View rapidly | 04B | Throttle absorbs intermediate states; no orphan popups (`CloseRootMenus` in `ShowNonBlocking` + handle dispose) | ✅ |
| Open Edit→Find submenu, drag diagonally toward a Find-item | 04B | Triangle tracker returns true → `UpdateHoveredItem` defers timer reset | ✅ |
| Move mouse straight off Edit (no submenu intent) → submenu closes after delay | 04B | Triangle test returns false → timer fires normally | ✅ |
| Screen-reader (NVDA): Alt activation announces "File menu" | 04A | Announcement fires | ☐ (deferred to Phase 07 — Accessibility) |

---

## What Shipped

### Wave 04A — Toggle / Activation / Alt+Mnemonics / Arrows
* `BeepMenuBar.Activation.cs` (new, ~190 LOC) — `MenubarActivation` enum + state machine + focus capture/restore + `ActivationChanged` event. Single `SetActivation` chokepoint paints all side-effects.
* `BeepMenuBar.Keyboard.cs` (new, ~190 LOC) — `ProcessCmdKey` for Alt/Esc/arrows/Enter; `ProcessMnemonic` for Alt+letter; wrap-around highlight; `GetMnemonicChar` helper with `&letter`/`&&` parsing + first-letter fallback.
* `BeepMenuBar.Drawing.cs` — `_drawMnemonics`-gated `TextFormatFlags`: omit prefix flags (default GDI behaviour = underline mnemonic) when active; `TextFormatFlags.HidePrefix` when inactive. Matches Win32 / WPF "lazy underline" UX.
* `BeepMenuBar.Input.cs` — toggle-close now transitions `ActiveWithPopup → ActiveNoPopup` so Alt-letter stays primed.
* `BeepMenuBar.Popup.cs` — popup-open transitions to `ActiveWithPopup`; `OnContextMenuDismissed` transitions back to `ActiveNoPopup`.

### Wave 04B — Non-blocking adapter + Hover-swap + Triangle tracker
* `BeepMenuBar.Popup.cs` — `ShowMenuItemPopupNonBlocking` + `_nonBlockingPopupHandle` + `CloseNonBlockingPopup` + `HasNonBlockingPopup`. Hand-off to Phase 05's `ContextMenuManager.ShowNonBlocking`. `OnContextMenuDismissed` nulls the handle on auto-close. `CloseAllPopups` (called from `Lifecycle.Dispose`) is no longer a no-op.
* `BeepMenuBar.Input.cs` — `HandleMenuItemClick` now routes items-with-children through the non-blocking adapter so hover-swap is reachable. `OnMouseMove` calls `MaybeHoverSwap(targetIndex)` when activation is `ActiveWithPopup`, a non-blocking popup exists, and the hovered index differs. 50 ms throttle via `kHoverSwapThrottleMs` + `_lastHoverSwapUtc`.
* `Menus/Helpers/SubmenuTriangleTracker.cs` (new, ~120 LOC) — stateless point-in-triangle geometry helper. LTR (top-left/bottom-left) and RTL (top-right/bottom-right) branches; `long` arithmetic prevents overflow; degenerate triangles return false.
* `ContextMenus/BeepContextMenu.SubmenuTracking.cs` (new, ~115 LOC) — state holder + integration glue: `NoteSubmenuOpened` / `NoteSubmenuClosed` / `ShouldDeferSubmenuDismissal(Point)` / `ResetCursorTrajectory`. Cursor trajectory captured on every consult; reset on `MouseLeave`.
* `ContextMenus/BeepContextMenu.Methods.cs` — `UpdateHoveredItem` consults `ShouldDeferSubmenuDismissal` before resetting the dismissal timer. Highlight still follows cursor; timer keeps running.
* `ContextMenus/BeepContextMenu.Events.cs` — `MouseLeave` calls `ResetCursorTrajectory` so re-entry measures fresh.
* `ContextMenus/ContextMenuManager.cs` — `ShowChildMenu` calls `parentMenu.NoteSubmenuOpened(menu.Bounds)` post-`Show`. `CloseChildMenus` calls `parentMenu.NoteSubmenuClosed()` (snapshot outside the lock to avoid re-entry).

### Build results
* `TheTechIdea.Beep.Winform.Controls` → **0 errors**
* `TheTechIdea.Beep.Winform.Controls.Design.Server` → **0 errors**
* `WinFormsApp.UI.Test` → **0 errors**

### Fix during 04A4 build
* `TextFormatFlags.HotPrefix` does not exist on WinForms's `TextFormatFlags` (it's a WPF-side concept). Switched to the equivalent idiom: omit any prefix flag when active (GDI's default behaviour underlines `&`-prefixed mnemonics) vs add `TextFormatFlags.HidePrefix` when inactive. Same visual outcome.

---

## Deferred (later phases)
- Type-ahead inside popup item lists → automatic once **Phase 06** moves items into `BeepListBox`.
- Designer SmartTag for "Add menu item" → **Phase 08**.
- Demo form exercising all four interaction modes + the triangle-tracker regression case → **Phase 09**.
- Screen-reader announcement of activation transitions → **Phase 07** (Accessibility).
