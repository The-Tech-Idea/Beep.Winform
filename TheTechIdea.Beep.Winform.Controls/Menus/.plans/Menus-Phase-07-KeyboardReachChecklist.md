# Menus — Phase 07 Keyboard Reach Checklist

> Companion to `Menus-Phase-07-AccessibilityKeyboard.md`.
> Captures every menu interaction and confirms it is reachable from the keyboard alone.

## Legend

| Symbol | Meaning |
|---|---|
| ✅ | Reachable + verified shipped in the named phase. |
| ➡  | Reachable, but the verification is part of a later phase (e.g. Phase 09 runtime demo). |
| ☐ | Not yet reachable — must be addressed before the next minor release. |

## Top-level menubar (`BeepMenuBar`)

| Interaction | Mouse | Keyboard | Shipped in |
|---|---|---|---|
| Activate menubar | Click top-level item | `Alt` (toggle) | ✅ Phase 04A |
| Move highlight to next top-level | Hover next item | `Right` arrow | ✅ Phase 04A |
| Move highlight to previous top-level | Hover prev item | `Left` arrow | ✅ Phase 04A |
| Open popup at highlighted item | Click | `Down` or `Enter` | ✅ Phase 04A |
| Activate top-level command (no children) | Click | `Enter` | ✅ Phase 04A |
| Open by mnemonic | n/a | `Alt+letter` | ✅ Phase 04A |
| Deactivate menubar (no popup up) | Click outside | `Esc` or second `Alt` | ✅ Phase 04A |
| Close active popup AND deactivate menubar | Click outside | `Esc` | ✅ Phase 04A |
| Toggle close same popup | Click same item | `Enter` again (closes popup, activation drops to ActiveNoPopup) | ✅ Phase 04A |
| Hover-swap between top-level popups | Hover next top-level while a popup is up | `Left`/`Right` arrow when activation is `ActiveWithPopup` | ✅ Phase 04A/04B |
| Visible keyboard focus | n/a | Focus rectangle on highlighted top-level when activation ≠ `Inactive` | ✅ Phase 07-C |
| Announce activation to AT | n/a | `AccessibleEvents.Focus` fires from `SetActivation` and `MoveHighlight` | ✅ Phase 07-E |
| Announce popup open to AT | n/a | `AccessibleEvents.SystemMenuPopupStart` fires from `ShowMenuItemPopup` + non-blocking | ✅ Phase 07-E |
| Announce popup close to AT | n/a | `AccessibleEvents.SystemMenuPopupEnd` fires from `OnContextMenuDismissed` | ✅ Phase 07-E |
| Expose top-level item bounds to AT | n/a | `MenuItemAccessibleObject.Bounds = RectangleToScreen(layoutRect)` | ✅ Phase 07-B |
| Expose mnemonic shortcut to AT | n/a | `MenuItemAccessibleObject.KeyboardShortcut` returns `"Alt+X"` | ✅ Phase 07-B |
| Strip `&` from AT announcements | n/a | `MenuItemAccessibleObject.Name` strips `&` and unescapes `&&` | ✅ Phase 07-B |

## Inside an open popup (`BeepContextMenu`)

| Interaction | Mouse | Keyboard | Shipped in |
|---|---|---|---|
| Move selection down | Hover next item | `Down` arrow | ➡ Phase 06.1 — currently keyboard nav is **only** active when the popup has focus AND the substrate is on; default-off path relies on the menubar's `Esc` to escape. Hosted-list path inherits `BeepListBox.Keyboard.cs` Up/Down/Home/End/Tab navigation. |
| Move selection up | Hover prev item | `Up` arrow | ➡ Phase 06.1 (substrate path) |
| Activate item (no submenu) | Click | `Enter` | ➡ Phase 06.1 (substrate path) — synchronous `ItemClicked` event fires; menu auto-closes per `CloseOnItemClick`. |
| Open submenu on hovered item | Hover for 300ms or click | `Right` arrow | ➡ Phase 06.1 (substrate path) — triggers existing `SubmenuOpening` event. |
| Close submenu / return to parent | Move cursor out / click parent | `Left` arrow or `Esc` | ➡ Phase 06.1 (substrate path) |
| Close entire popup tree | Click outside | `Esc` | ✅ Phase 05 — `Esc` is wired in `BeepContextMenu` form's key handler regardless of substrate. |
| Type-ahead jump to item by prefix | n/a | Letters / digits within open popup | ➡ Phase 06.1 (substrate path) — `BeepListBox` ships type-ahead. |
| Submenu triangle tracking (avoid premature dismissal on diagonal mouse moves) | n/a (mouse only) | n/a | ✅ Phase 04B |
| Search box (when `ShowSearchBox = true`) | Click | `Tab` to box, type to filter | ✅ Phase 05 (form-level) — substrate path keeps it top-docked above the hosted list. |

## Cross-cutting

| Concern | Status | Shipped in |
|---|---|---|
| Custom `AccessibleObject` hierarchy (`MenuBar` root + `MenuItem` children) | Implemented | ✅ Phase 07-B |
| `AccessibleRole.MenuBar` exposed on the bar | Implemented | ✅ Phase 07-B |
| `AccessibleRole.MenuItem` exposed on each top-level | Implemented | ✅ Phase 07-B |
| `AccessibleStates.HasPopup` on items with children | Implemented | ✅ Phase 07-B |
| `AccessibleStates.Focused` reflects current highlight when activation ≠ `Inactive` | Implemented | ✅ Phase 07-B |
| `AccessibleStates.Expanded` reflects `OpenTopLevelIndex` | Implemented | ✅ Phase 07-B |
| `DefaultAction` returns `"Open"` (has children) or `"Press"` (no children) | Implemented | ✅ Phase 07-B |
| `Select(AccessibleSelection.TakeFocus)` programmatically focuses an item and enters keyboard mode | Implemented | ✅ Phase 07-B |
| `HitTest(x,y)` returns the correct child AO for a screen point | Implemented | ✅ Phase 07-B |
| Visible focus indicator that satisfies WCAG 2.4.11 "Focus Not Obscured" | Focus rectangle drawn 2px inset from item rect | ✅ Phase 07-C |
| Visible focus indicator in High Contrast (3 px Highlight ring) | Implemented via `PaintFocusRectIfHC` | ✅ Phase 07-C/D |
| `SystemInformation.HighContrast` is honoured for fill / border / text colours | Implemented via `DrawMenuItemHighContrast` | ✅ Phase 07-D |
| HC toggled mid-session triggers re-paint | `SystemEvents.UserPreferenceChanged` subscription | ✅ Phase 07-D |
| Subscription is cleaned up on `Dispose` + `OnHandleDestroyed` | Implemented | ✅ Phase 07-D |
| Per-popup-item accessibility (Name / Role / State / Bounds) | Inherits `BeepListBox.Accessibility` when substrate is on | ➡ Phase 06.1 (flip the default) |

## Known gaps (explicitly out of scope for this phase)

1. **Hand-rolled `BeepContextMenu` keyboard navigation** — when `UseHostedListSubstrate = false` (the default), the popup does not currently respond to `Up`/`Down`/`Right`/`Left`/`Enter`. The substrate path inherits this from `BeepListBox` for free. **Decision**: rather than implement keyboard nav twice, Phase 06.1 will flip the substrate default to ON and then this row goes ✅ automatically.
2. **KeyTip overlays** (Office 2007-style two-letter accelerators on top-level items after `Alt`). Deferred; current mnemonic underline + `Alt+letter` cover the WCAG baseline.
3. **Caret-tracking screen reader announcements** for the *currently hovered* mouse item. Today only `Focus` (keyboard-driven) is announced. Deferred — most screen readers don't read mouse hovers anyway.
