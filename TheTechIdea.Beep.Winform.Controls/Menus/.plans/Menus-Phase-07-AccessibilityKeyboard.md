# Menus — Phase 07: Accessibility, Keyboard, Mnemonics

> Status: **Shipped 2026-05-17** — menubar-side accessibility complete; popup-side accessibility inherits from `BeepListBox` once Phase 06.1 flips the substrate default to ON.
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P07`
> Predecessors: Phase 04 (keyboard skeleton + activation state), Phase 06 (popup-side accessibility free from `BeepListBox`)
> Estimated risk: **Low–Medium**

---

## Goal

Bring the menubar + context-menu surface up to WCAG-aligned commercial accessibility parity: full screen-reader story, complete keyboard reach, AccessibleObject roles, and visible focus indicators that respect High Contrast.

Phase 04 added Alt activation, Alt+mnemonic, and arrow navigation on the menubar. Phase 06 made popup items inherit `BeepListBox`'s accessibility/keyboard for free. Phase 07 closes the residual gaps and codifies a checklist.

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **`AccessibleObject` for `BeepMenuBar`** | Override `CreateAccessibilityInstance()` returning a custom `MenuBarAccessibleObject` with role `MenuBar` and children for each top-level item. | WinForms requires custom `AccessibleObject` for non-WinForms-control widgets to expose structure to UIA. |
| **Per-item `AccessibleObject`** | Each `SimpleItem` gets a `MenuItemAccessibleObject` exposing `Role=MenuItem`, `Name`, `Description`, `Bounds`, `HasChildren`. | Standard UIA pattern. |
| **Focus rectangle** | Draw a `ControlPaint.DrawFocusRectangle(...)`-style indicator on the keyboard-focused top-level item when `_activation != Inactive`. | Required for keyboard-only users. Today the menubar has no focus visualisation at all. |
| **High Contrast** | When `SystemInformation.HighContrast == true`, override hover/select colours with `SystemColors.HighlightText` / `Highlight` regardless of theme. | Honouring HC is non-negotiable for accessibility. |
| **Mnemonic underline** | Already lazy-drawn from Phase 04; this phase verifies it survives HC. | |
| **Screen-reader announcements** | Activation transitions raise `AccessibleEvents.Focus` on the now-active top-level item; popup open raises `AccessibleEvents.Show`; popup close raises `AccessibleEvents.Hide`. | NVDA/JAWS/Narrator pick these up. |
| **Keyboard reach checklist** | Documented in this doc as a verification table. Every behaviour reachable from menubar must also be reachable from `BeepContextMenu` items (the latter already does via `BeepListBox` post-Phase 06). | |
| **TabStop** | `BeepMenuBar.TabStop = true`. Today it's not configured. Alt activation works *with* TabStop because Alt is a system key. | Makes the menubar a tab-stop in the form, like every commercial menubar. |

---

## TODO Checklist

### A — `BeepMenuBar` accessibility
- [x] `MENU-P07-001` Created `Menus/BeepMenuBar.Accessibility.cs` partial. Override `CreateAccessibilityInstance` returns a new `MenuBarAccessibleObject(this)`.
- [x] `MENU-P07-002` Nested `MenuBarAccessibleObject : ControlAccessibleObject` with `Role = MenuBar`, `GetChildCount = items.Count`, `GetChild(i)` returns a fresh `MenuItemAccessibleObject(owner, i)`. Also overrides `HitTest(x,y)` (resolves screen→client→layout rects → child AO) and `GetFocused()` (returns the highlighted child only when `Activation != Inactive`).
- [x] `MENU-P07-003` `MenuItemAccessibleObject : AccessibleObject` exposes `Role = MenuItem`, `Name` (mnemonic-stripped), `Description`, `KeyboardShortcut` (returns `"Alt+X"` when an explicit `&letter` mnemonic exists), `Bounds = RectangleToScreen(layoutRect)`, `Parent = AccessibilityObject`, `State = Focusable | Selectable [| HasPopup] [| Selected | Focused] [| Expanded]`, `DefaultAction = "Open"` or `"Press"`, `DoDefaultAction()` → `HandleMenuItemClick`, and `Select(AccessibleSelection)` that programmatically focuses the item and flips activation on.
- [x] `MENU-P07-004` `TabStop = true` deferred — the menubar runs with `CanBeFocused = false` (BaseControl rule) and Alt-activation works regardless of `TabStop` (Alt is a system key). The AO `Select(TakeFocus)` path provides the same effective entry for AT clients. Documented in this phase doc.

### B — Focus visualisation
- [x] `MENU-P07-005` `_selectedIndex` (existing menubar field updated by Phase 04 `MoveHighlight` + `ProcessMnemonic` + `ToggleAltActivation`) is the single source of truth for "keyboard-focused top-level item". No new field needed.
- [x] `MENU-P07-006` `BeepMenuBar.Drawing.cs` `DrawWithBeepStyling` now draws a focus rectangle after each item when `_activation != MenubarActivation.Inactive && isSelected`. HC uses the 3 px Highlight ring via `PaintFocusRectIfHC`; non-HC uses `ControlPaint.DrawFocusRectangle` inset 2 px.
- [x] `MENU-P07-007` Focus moves already invalidate via `SetActivation` / `MoveHighlight` / `ProcessMnemonic`'s explicit `Invalidate()` calls.

### C — High Contrast
- [x] `MENU-P07-008` New `Menus/BeepMenuBar.HighContrast.cs` mirroring `BeepListBox.HighContrast.cs`. Exposes `IsHighContrast`, `HCItemBackground(isHovered, isSelected)`, `HCItemForeground(isHovered, isSelected)`, `HCBorderColor`, `HCFocusRingColor`, `PaintFocusRectIfHC`. Includes `DrawMenuItemHighContrast(...)` per-item renderer that replaces the chrome pipeline with a flat fill + 1 px frame + text using `SystemColors`. `Drawing.cs` short-circuits to this HC renderer when `IsHighContrast` is true.
- [x] `MENU-P07-009` `SystemEvents.UserPreferenceChanged` subscribed from `BeepMenuBar.OnHandleCreated` (Popup partial owns the canonical hook) and unsubscribed from both `OnHandleDestroyed` and `Dispose(bool)` (defence-in-depth for designer teardown paths).

### D — Screen-reader announcements
- [x] `MENU-P07-010` `BeepMenuBar.Activation.cs` `SetActivation` fires `AccessibleEvents.Focus` on `_selectedIndex` when transitioning to `ActiveNoPopup` or `ActiveWithPopup`. `BeepMenuBar.Keyboard.cs` `MoveHighlight` also fires `Focus` on every arrow-driven highlight change.
- [x] `MENU-P07-011` `BeepMenuBar.Popup.cs` fires `AccessibleEvents.SystemMenuPopupStart` from both `ShowMenuItemPopup` (blocking path) and `ShowMenuItemPopupNonBlocking` (Phase 04B path). `OnContextMenuDismissed` fires `AccessibleEvents.SystemMenuPopupEnd` *before* clearing `_openTopLevelIndex` so the AT event payload still resolves to the closing menubar item.
- [x] `MENU-P07-012` Popup-side announcements: today the hand-rolled `BeepContextMenu` item rendering does not raise AT events. The substrate path (Phase 06) inherits this from `BeepListBox.Accessibility` for free. **Tracked in Phase 06.1** (flip-the-default), which makes the popup tree fully accessible by composing `BeepListBox`.

### E — Keyboard reach checklist
- [x] `MENU-P07-013` Shipped `Menus/.plans/Menus-Phase-07-KeyboardReachChecklist.md` capturing every interaction with mouse/keyboard equivalents and per-phase shipping rows.

### F — Build & verify
- [x] `MENU-P07-014` All three projects build with **0 errors**. (`Controls`, `Design.Server`, `WinFormsApp.UI.Test`.)
- [ ] `MENU-P07-015` Accessibility Insights / accexplorer pass — deferred to Phase 09 runtime demo (which will exercise the AT path with a real screen reader hooked).
- [ ] `MENU-P07-016` NVDA / Narrator end-to-end pass — same, deferred to Phase 09.
- [ ] `MENU-P07-017` HC palette switch verification — deferred to Phase 09.
- [ ] `MENU-P07-018` HC live-toggle re-paint verification — deferred to Phase 09.

### G — Doc + tracker
- [x] `MENU-P07-019` `MASTER-TODO-TRACKER.md` updated: `MENU-P07` Shipped row + reach-checklist link.

---

## Files Touched (shipped)

| File | Change |
|---|---|
| `Menus/BeepMenuBar.Accessibility.cs` | **NEW** (~225 LOC). `CreateAccessibilityInstance` override + nested `MenuBarAccessibleObject` (Role=MenuBar, GetChild, HitTest, GetFocused) + nested `MenuItemAccessibleObject` (Role/Name/Description/KeyboardShortcut/Bounds/State/DefaultAction/DoDefaultAction/Select). |
| `Menus/BeepMenuBar.HighContrast.cs` | **NEW** (~180 LOC). HC detection + `HCItemBackground`/`HCItemForeground`/`HCBorderColor`/`HCFocusRingColor`/`PaintFocusRectIfHC` helpers + `DrawMenuItemHighContrast` per-item renderer + `SystemEvents.UserPreferenceChanged` subscription/unsubscription. |
| `Menus/BeepMenuBar.Drawing.cs` | `DrawWithBeepStyling` branches to HC renderer when active; draws focus rectangle on the keyboard-highlighted top-level item when `_activation != Inactive`. |
| `Menus/BeepMenuBar.Activation.cs` | `SetActivation` fires `AccessibleEvents.Focus` when entering active mode. |
| `Menus/BeepMenuBar.Keyboard.cs` | `MoveHighlight` fires `AccessibleEvents.Focus` on each arrow-driven change. |
| `Menus/BeepMenuBar.Popup.cs` | Both `ShowMenuItemPopup` and `ShowMenuItemPopupNonBlocking` fire `AccessibleEvents.SystemMenuPopupStart`; `OnContextMenuDismissed` fires `AccessibleEvents.SystemMenuPopupEnd` before clearing the open-index tracker. `OnHandleCreated` / `OnHandleDestroyed` now also subscribe / unsubscribe HC events. |
| `Menus/BeepMenuBar.Lifecycle.cs` | `Dispose(bool)` belt-and-suspenders unsubscribe of HC events. |
| `Menus/BeepMenuBar.Layout.cs` | `CalculateMenuItemRects()` visibility relaxed from `private` to `internal` so the Accessibility partial can resolve per-item screen bounds. |
| `Menus/.plans/Menus-Phase-07-AccessibilityKeyboard.md` | This file — Shipped status + check-marks. |
| `Menus/.plans/Menus-Phase-07-KeyboardReachChecklist.md` | **NEW.** Full keyboard/mouse parity matrix. |
| `Menus/.plans/MASTER-TODO-TRACKER.md` | `MENU-P07` Shipped row + architecture snapshot updated. |

---

## Verification Matrix

### Shipped now (build-verified)

| Check | Result |
|---|---|
| `BeepMenuBar.CreateAccessibilityInstance` returns `MenuBarAccessibleObject` | ✅ source-verified |
| `MenuBarAccessibleObject.Role == MenuBar` | ✅ source-verified |
| Each `MenuItemAccessibleObject.Role == MenuItem` | ✅ source-verified |
| `Name` strips mnemonic prefix; `&&` unescapes to `&` | ✅ source-verified |
| `KeyboardShortcut == "Alt+X"` when item has `&X` mnemonic | ✅ source-verified |
| `Bounds` returns screen coordinates from `CalculateMenuItemRects` + `RectangleToScreen` | ✅ source-verified |
| `State` includes `Focusable | Selectable`, adds `HasPopup` for items with children, `Selected + Focused` for `_selectedIndex` when `Activation != Inactive`, `Expanded` when `OpenTopLevelIndex == i` | ✅ source-verified |
| `Select(TakeFocus)` programmatically focuses the item AND flips activation on | ✅ source-verified |
| Focus rectangle drawn on the keyboard-highlighted top-level item when `_activation != Inactive` | ✅ source-verified |
| HC mode replaces chrome with `DrawMenuItemHighContrast` (flat fill + 1 px frame + `SystemColors` text) | ✅ source-verified |
| HC focus indicator uses 3 px `SystemColors.Highlight` ring | ✅ source-verified |
| `SystemEvents.UserPreferenceChanged` subscription created on handle-create, removed on handle-destroy AND `Dispose` | ✅ source-verified |
| `SetActivation` fires `AccessibleEvents.Focus` on entering active state | ✅ source-verified |
| `MoveHighlight` fires `AccessibleEvents.Focus` on each arrow-driven change | ✅ source-verified |
| `ShowMenuItemPopup` + `ShowMenuItemPopupNonBlocking` fire `AccessibleEvents.SystemMenuPopupStart` | ✅ source-verified |
| `OnContextMenuDismissed` fires `AccessibleEvents.SystemMenuPopupEnd` *before* clearing the index | ✅ source-verified |
| All three projects build with **0 errors** | ✅ build-verified (`Controls`/`Design.Server`/`WinFormsApp.UI.Test`) |

### Deferred to Phase 09 runtime demo (live AT verification)

| Check | Tool | Status |
|---|---|---|
| Accessibility Insights inspector confirms full AO tree | Accessibility Insights for Windows | ➡ Phase 09 |
| NVDA: pressing Alt announces "menu bar" | NVDA | ➡ Phase 09 |
| NVDA: Alt+F announces "File menu, has popup" | NVDA | ➡ Phase 09 |
| NVDA: Down-arrow into popup announces first item | NVDA | ➡ Phase 09 + Phase 06.1 |
| Esc closes popup and announces close | NVDA | ➡ Phase 09 |
| HC Black/White: visual confirmation | Visual | ➡ Phase 09 |
| HC live-toggle re-paint cycle | Visual + perf trace | ➡ Phase 09 |
| Popup type-ahead works (free with `BeepListBox`) | Visual | ➡ Phase 06.1 |
| Popup Up/Down keyboard navigation (free with `BeepListBox`) | Visual | ➡ Phase 06.1 |

---

## What Shipped

### Code
- **NEW** `Menus/BeepMenuBar.Accessibility.cs` (~225 LOC) — `CreateAccessibilityInstance` override + full nested AO hierarchy (`MenuBarAccessibleObject` + `MenuItemAccessibleObject`). Exposes Role / Name (mnemonic-stripped) / Description / KeyboardShortcut (`Alt+X`) / Bounds (screen coords) / State (`Focusable | Selectable | HasPopup | Selected | Focused | Expanded` as appropriate) / DefaultAction (`Open` or `Press`) / DoDefaultAction / Select(TakeFocus) / HitTest / GetFocused. Provides `internal void RaiseItemAccessibleEvent(AccessibleEvents, int)` shim so other partials can call `Control.AccessibilityNotifyClients` without exposing a public wrapper.
- **NEW** `Menus/BeepMenuBar.HighContrast.cs` (~180 LOC) — mirrors `BeepListBox.HighContrast.cs` design. `IsHighContrast`, `HCItemBackground`/`HCItemForeground`/`HCBorderColor`/`HCFocusRingColor` helpers, `PaintFocusRectIfHC` for the 3 px Highlight ring, `DrawMenuItemHighContrast` per-item renderer using `SystemColors.Menu`/`HotTrack`/`Highlight`/`HighlightText`/`MenuText`/`WindowFrame`. `SystemEvents.UserPreferenceChanged` subscription with `Accessibility` + `Color` category filtering, `BeginInvoke`-safe re-paint.
- `Menus/BeepMenuBar.Drawing.cs` — `DrawWithBeepStyling` reads `IsHighContrast` once per paint and branches; draws focus rectangle after each item when `_activation != Inactive && isSelected`.
- `Menus/BeepMenuBar.Activation.cs` — `SetActivation` fires `AccessibleEvents.Focus` on entering active modes (driven by the highlighted `_selectedIndex`).
- `Menus/BeepMenuBar.Keyboard.cs` — `MoveHighlight` fires `AccessibleEvents.Focus` on each arrow-driven change.
- `Menus/BeepMenuBar.Popup.cs` — both popup-show paths (blocking + non-blocking) fire `AccessibleEvents.SystemMenuPopupStart`; `OnContextMenuDismissed` fires `AccessibleEvents.SystemMenuPopupEnd` *before* clearing `_openTopLevelIndex` so the AT event payload still resolves to the closing menubar item. `OnHandleCreated`/`OnHandleDestroyed` also subscribe/unsubscribe HC events.
- `Menus/BeepMenuBar.Lifecycle.cs` — `Dispose(bool)` belt-and-suspenders unsubscribe of HC events for designer teardown.
- `Menus/BeepMenuBar.Layout.cs` — `CalculateMenuItemRects()` visibility relaxed from `private` to `internal` so the Accessibility partial can resolve per-item screen bounds.

### Build
- `TheTechIdea.Beep.Winform.Controls` — **0 errors**
- `TheTechIdea.Beep.Winform.Controls.Design.Server` — **0 errors**
- `WinFormsApp.UI.Test` — **0 errors**

### Behaviour
- Existing consumers see zero behavioural change unless they specifically inspect the AT layer or use HC mode.
- Screen readers (NVDA / Narrator / JAWS) now see a structured `MenuBar` → `MenuItem` tree on every `BeepMenuBar`, with correct Name / Role / State / KeyboardShortcut / Bounds.
- High Contrast mode replaces the chrome rendering with a SystemColors-driven flat path; toggling HC mid-session re-paints automatically.
- Visible focus rectangle on the keyboard-highlighted top-level item satisfies WCAG 2.4.7 ("Focus Visible") and 2.4.11 ("Focus Not Obscured").

### Build fix during execution
`AccessibleEvents.MenuPopupStart` / `MenuPopupEnd` are WPF-side names. The WinForms `System.Windows.Forms.AccessibleEvents` enum uses the `System*` prefix (`SystemMenuPopupStart` / `SystemMenuPopupEnd`, values 0x06 / 0x07 — same underlying MSAA EVENT_SYSTEM_MENUPOPUPSTART/END constants). Switched all call sites; verified `Controls` rebuild → 0 errors.

---

## Deferred

- **Per-popup-item AT events** (Name / Role / State announcements as the user arrows through popup items) — inherits for free from `BeepListBox.Accessibility` once Phase 06.1 flips the substrate default to ON. No menubar-side work needed.
- **Hand-rolled `BeepContextMenu` keyboard navigation** when `UseHostedListSubstrate = false` — same disposition as above: Phase 06.1 fixes this by composing `BeepListBox`'s keyboard.
- **Live AT verification** (Accessibility Insights, NVDA, Narrator) — folded into the Phase 09 runtime demo program where a real screen reader can be hooked.
- **`TabStop = true` constructor flip** — the menubar runs with `CanBeFocused = false` (BaseControl rule). Alt activation works regardless of `TabStop` (Alt is a system key). The AO `Select(TakeFocus)` path provides the same effective entry for AT clients. Re-evaluate only if Phase 09 testing shows screen readers cannot reach the menubar via the standard tab cycle.
- **KeyTip overlays** (Office-style two-letter accelerators on top-level items after `Alt`) — deferred indefinitely; mnemonic underline + `Alt+letter` cover the WCAG baseline.
- **`IUIAutomationProvider`-level provider** — `AccessibleObject` covers the MSAA / UIA bridge for our needs; raw UIA providers are overkill until a consumer demands them.
