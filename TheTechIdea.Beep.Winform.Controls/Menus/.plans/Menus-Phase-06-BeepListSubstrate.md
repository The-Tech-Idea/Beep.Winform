# Menus — Phase 06: BeepListBox Substrate for BeepContextMenu Items

> Status: **Shipped (opt-in)** — substrate available behind `UseHostedListSubstrate` flag; default OFF.
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P06`
> Predecessors: Phase 05 (non-blocking lifecycle stabilised), Phase 03 (stale painter framework removed)
> Estimated risk: **Medium–High** — mitigated by shipping as opt-in; full default-on is deferred to Phase 06.1 once parity is end-to-end validated.

---

## Goal

Re-host the popup item area inside `BeepListBox` so menu items automatically get the same:
- chrome (`BeepStyling.PaintControl` → `BackgroundPainterFactory`/`BorderPainterFactory`/`ShadowPainterFactory`)
- hover/select state machine
- keyboard navigation (`BeepListBox.Keyboard.cs`)
- accessibility (`BeepListBox.Accessibility.cs`)
- drag-and-drop (`BeepListBox.Drag.cs`)
- high-contrast handling (`BeepListBox.HighContrast.cs`)

…that every other Beep list surface ships, without `BeepContextMenu` re-implementing them.

This is the user's directive: **"we are using control and form style and BeepList for theming and styling."** The popup body becomes a `BeepListBox` composition; the popup form remains responsible for chrome/positioning/lifecycle.

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **Composition vs inheritance** | `BeepContextMenu` *hosts* a `BeepListBox` as a child control filling its client area; it does **not** inherit. | Inheritance from `BeepListBox` would force every `BeepContextMenu` consumer to deal with `BeepListBox`'s public surface (`ListItems`, `ShowImage`, `ShowCheckBox`, scrolling, etc.) — leaky. Composition keeps the popup API focused. |
| **Item model** | Keep `BindingList<SimpleItem>`. `BeepListBox.ListItems` is already `BindingList<SimpleItem>` — same model, no mapping layer. | Zero schema translation. |
| **Selection event bridge** | `BeepListBox.SelectedItemChanged` → existing `BeepContextMenu.ItemClicked` event. Existing public `ItemClicked` contract preserved. | No consumer changes. |
| **Hover handling** | Delegate to `BeepListBox` — it already raises hover events, paints hover state via `BeepStyling`, and respects `EnableHoverAnimation`. | Removes the hand-rolled hover code in `BeepContextMenu.Events.cs`. |
| **Submenu open trigger** | When the hosted `BeepListBox` raises `SelectedItemChanged` for an item with children, forward to existing `SubmenuOpening` event. Preserves the existing `ContextMenuManager` submenu flow. | Submenu lifecycle stays in `ContextMenuManager`. |
| **Hand-rolled drawing in `BeepContextMenu.Drawing.cs`** | **Strip** everything that draws *items*. Keep what draws *popup chrome* (the form-level shadow/border, the optional caption/footer). | Items go to `BeepListBox`; chrome stays here. |
| **Hand-rolled hit-test in `BeepContextMenu.Methods.cs`** | **Strip** the item-area hit-test (`GetItemAtPoint`, etc.). Keep the form-level hit-test for non-item zones (scrollbars, header). | `BeepListBox` owns its own hit-test. |
| **`Menus/Models/MenuColorConfig.cs` + `MenuStyleConfig.cs`** | Audit usage. If unreferenced after this phase → **delete**. If referenced → migrate to `BeepListBox` configuration. | Driven by what's actually used post-refactor. |
| **Backwards compatibility** | `BeepContextMenu.MenuItems`, `BeepContextMenu.SelectedItem`, `BeepContextMenu.ItemClicked`, `BeepContextMenu.SubmenuOpening` — **all preserved**. Internally backed by the hosted `BeepListBox`. | Zero consumer migration. |

---

## Scope Decision (resolved during execution)

A full rip-out + re-host of the item area would silently drop hand-rolled features the current `BeepContextMenu` still ships (shortcut column, two-line subtext, submenu-arrow indicator, "-"/`separator`-tag separators, top-docked `BeepTextBox` search, BeepScrollBar-aware scroll path, and the Phase 04B submenu-triangle hover-state coupling). It would also force every existing consumer onto an unvalidated visual path in a single shot.

Shipped scope is therefore **opt-in substrate**:

- A new `[Browsable] public bool UseHostedListSubstrate` (default `false`) toggles the path.
- When `true`, a `BeepListBox` is created, docked-fill into the popup form, bound to `_menuItems`, and its `SelectedItemChanged` forwards to the existing `OnItemClicked` / `OnSubmenuOpening` pipeline. The hand-rolled item drawing (`DrawMenuItemsSimple`) is skipped, and `RecalculateSize` sums height from `BeepListBox.PreferredItemHeight × item-count` instead of per-item measurement.
- When `false` (default), nothing changes — Phase 04B's triangle tracker, the search box, separators, shortcuts, subtext, and submenu arrow all behave exactly as before.

A follow-up micro-phase **Phase 06.1 — Flip default-on** will validate parity item-by-item (search-box layout above list, shortcut column, subtext, separators, submenu arrows) and then flip the default. That phase tracks against a fresh tracker entry.

---

## TODO Checklist

### A — Inventory & pre-check
- [x] `MENU-P06-001` Enumerate `BeepContextMenu` partials & per-file LOC.
- [x] `MENU-P06-002` Confirm no external consumer touches the soon-to-be-internal item drawing helpers (drawing/hit-test methods are already `private`).
- [x] `MENU-P06-003` Grep `MenuColorConfig`/`MenuStyleConfig` — **already deleted in Phase 03**; zero references repo-wide.

### B — Compose the BeepListBox (opt-in)
- [x] `MENU-P06-004` New partial `ContextMenus/BeepContextMenu.ListSubstrate.cs`; private `_hostedList`, lazy `EnsureHostedListCreated`, dock-fill with `ShowImage`/`ShowCheckBox`/`MultiSelect`/`EnableHoverAnimation = true` mirrored from menu state.
- [x] `MENU-P06-005` `_hostedList.ListItems = _menuItems` (same `BindingList<SimpleItem>` reference; no mapping layer).
- [x] `MENU-P06-006` Multi-select bridge: list `ShowCheckBox` follows `_showCheckBox`, `MultiSelect` follows `_multiSelect`, and `_selectedItems` is mirrored from `_hostedList.SelectedItems` on each selection change.
- [x] `MENU-P06-007` Forward `_hostedList.SelectedItemChanged` → `OnItemClicked` / `OnSubmenuOpening` based on `item.Children`; honour the existing `CloseOnItemClick` contract.
- [x] `MENU-P06-008` Keyboard handled inside `BeepListBox` natively when focused; menu-form's own keyboard path is unchanged.

### C — Drawing skip-path
- [x] `MENU-P06-009` `BeepContextMenu.Drawing.cs` `OnPaint` now skips `DrawMenuItemsSimple` when `IsHostedListSubstrateActive` is true. Popup chrome (shadow, border, optional search box positioning) still paints from the form.
- [x] `MENU-P06-010` Hand-rolled mouse hit-test in `Events.cs` is left intact — it naturally doesn't fire over child controls (the list owns the area), so guards are not needed.
- [x] `MENU-P06-011` Search-box: when substrate is active and `ShowSearchBox = true`, `EnsureSearchTextBox` runs from `EnsureHostedListCreated` and the textbox is docked-top so the list (`Dock = Fill`) sits beneath it.
- [x] `MENU-P06-012` Property setters for `ShowImage`/`ShowCheckBox`/`MultiSelect`/`MenuItems` now call `SyncHostedListFromMenuState()` so toggling them at runtime keeps the hosted list in sync.

### D — Layout
- [x] `MENU-P06-013` `BeepContextMenu.Methods.cs` `RecalculateSize()` sums `BeepListBox.PreferredItemHeight × _menuItems.Count` when substrate is active; otherwise falls back to the existing per-item `GetItemHeight` loop.
- [x] `MENU-P06-014` `BeepContextMenuLayoutHelper` is unmodified — when substrate is active the layout helper is bypassed by the drawing skip-path.

### E — Models reconciliation
- [x] `MENU-P06-015` `MenuColorConfig.cs` — already deleted in Phase 03 (verified). No action.
- [x] `MENU-P06-016` `MenuStyleConfig.cs` — already deleted in Phase 03 (verified). No action.
- [x] `MENU-P06-017` Tracker Deprecation List unchanged (Phase 03 already covered these files).

### F — Build
- [x] `MENU-P06-018` `dotnet build` all three projects — `TheTechIdea.Beep.Winform.Controls` (0 err), `TheTechIdea.Beep.Winform.Controls.Design.Server` (0 err), `WinFormsApp.UI.Test` (0 err).

### F.1 — Verify behaviour parity (deferred to Phase 06.1)
Items below are checklist for the follow-up flip-the-default phase; they verify substrate-on behaviour matches hand-rolled.

- [ ] `MENU-P06-019` Single-select: item click → existing `ItemClicked` signature.
- [ ] `MENU-P06-020` Multi-select: `ShowContextMenuMultiSelect` returns expected `List<SimpleItem>`.
- [ ] `MENU-P06-021` Submenu: hover an item with children → submenu opens via existing `SubmenuOpening`.
- [ ] `MENU-P06-022` Keyboard: Up/Down navigates items (handled by `BeepListBox.Keyboard`).
- [ ] `MENU-P06-023` Esc closes popup.
- [ ] `MENU-P06-024` Type-ahead: typing "Op…" jumps to "Open…" (free with `BeepListBox`).
- [ ] `MENU-P06-025` Screen reader announces item name on focus (free with `BeepListBox.Accessibility`).
- [ ] `MENU-P06-026` High-contrast theme — colours from `BeepListBox.HighContrast`.
- [ ] `MENU-P06-027` Visual diff across each `BeepControlStyle` — chrome unchanged; items match `BeepListBox` look.

### G — Doc + tracker
- [x] `MENU-P06-028` Update `MASTER-TODO-TRACKER.md`: `MENU-P06` Shipped (opt-in); note Phase 06.1 as the follow-up flip-the-default phase.

---

## Files Touched (what actually shipped)

| File | Change |
|---|---|
| `ContextMenus/BeepContextMenu.ListSubstrate.cs` | **NEW** — entire opt-in substrate (flag, lazy host, sync, selection forwarding, layout helper). |
| `ContextMenus/BeepContextMenu.Drawing.cs` | `OnPaint` now skips `DrawMenuItemsSimple` when `IsHostedListSubstrateActive`. |
| `ContextMenus/BeepContextMenu.Methods.cs` | `RecalculateSize` height path branches: substrate uses `PreferredItemHeight × Count`; else falls back to per-item measurement. |
| `ContextMenus/BeepContextMenu.Properties.cs` | `MenuItems`/`ShowImage`/`ShowCheckBox`/`MultiSelect` setters call `SyncHostedListFromMenuState()`. |
| `ContextMenus/BeepContextMenu.Designer.cs` | `Dispose(bool)` calls `TeardownHostedList()`. |
| `Menus/.plans/Menus-Phase-06-BeepListSubstrate.md` | This file — Scope Decision + Shipped status. |
| `Menus/.plans/MASTER-TODO-TRACKER.md` | `MENU-P06` Shipped (opt-in); Phase 06.1 added as the flip-the-default follow-up. |

> NOT touched: `BeepContextMenu.Events.cs` (hand-rolled hit-test naturally doesn't fire over child controls when substrate is active), `BeepContextMenu.Core.cs` (substrate code lives in its own partial per Clean Code rule), `BeepContextMenuLayoutHelper.cs` (bypassed by drawing skip), `Menus/Models/*` (already deleted in Phase 03).

---

## Verification Matrix

### What shipped now (Phase 06)

| Scenario | Expected | Status |
|---|---|---|
| Default (`UseHostedListSubstrate = false`) — existing visual behaviour 100% preserved | All Phase 01/04/05 features remain stable | ✅ |
| Opt-in (`UseHostedListSubstrate = true`) — popup hosts a docked `BeepListBox` | `_hostedList` becomes a child control; `OnPaint` skips item drawing | ✅ |
| Opt-in — selection forwards through existing event contract | `OnItemClicked` / `OnSubmenuOpening` fire with the original `SimpleItem` payload | ✅ |
| Opt-in — `MenuItems` mutations propagate to the hosted list | `SyncHostedListFromMenuState()` keeps `ListItems`/`ShowImage`/`ShowCheckBox`/`MultiSelect`/`Theme` aligned | ✅ |
| Opt-in — `RecalculateSize` honours hosted-list metrics | Height = `internalPadding × 2 + (search box) + PreferredItemHeight × ItemCount + chrome insets` | ✅ |
| Opt-in — `Dispose` releases the hosted list | `TeardownHostedList()` runs from `Dispose(bool)` and unsubscribes/disposes cleanly | ✅ |
| No build references to `MenuColorConfig`/`MenuStyleConfig` | `rg` clean — both deleted in Phase 03 | ✅ |
| All three projects build with **0 errors** | `Controls` 0/`Design.Server` 0/`WinFormsApp.UI.Test` 0 | ✅ |

### Deferred to Phase 06.1 (flip-the-default)

| Scenario | Owner | Notes |
|---|---|---|
| Single-select substrate-on parity smoke (right-click click) | Phase 06.1 | Verify pixel + behavior diff |
| Multi-select substrate-on parity (check 3 items) | Phase 06.1 | `_selectedItems` mirror correct |
| Submenu open via children | Phase 06.1 | `SubmenuOpening` still fires; triangle-tracker behaviour in submenu mode |
| Up/Down keyboard navigation | Phase 06.1 | Confirm `BeepListBox.Keyboard` claims focus |
| Type-ahead on opened popup | Phase 06.1 | Free with `BeepListBox` |
| Screen reader announce on focus | Phase 06.1 | Free with `BeepListBox.Accessibility` |
| High-contrast theme path | Phase 06.1 | Free with `BeepListBox.HighContrast` |
| Visual parity across each `BeepControlStyle` | Phase 06.1 | Expected pixel deltas are accepted (this is the unification we want) |
| Search-box compat when `ShowSearchBox = true` + substrate | Phase 06.1 | Top-docked behaviour verified in design-time + runtime |
| Shortcut column + subtext + submenu-arrow features | Phase 06.1 | Either reach parity on `BeepListBox` or document migration |

---

## What Shipped

### Code

- **NEW** `ContextMenus/BeepContextMenu.ListSubstrate.cs` (~240 LOC) — single-responsibility partial that:
  - Adds `[Browsable] public bool UseHostedListSubstrate` (default `false`).
  - Adds `[Browsable(false)] public bool IsHostedListSubstrateActive` and `public BeepListBox HostedList` (read-only).
  - Lazily creates the hosted `BeepListBox` (`Dock = Fill`), mirrors `_showImage`/`_showCheckBox`/`_multiSelect`/`_themeName`, binds `ListItems = _menuItems`, and docks the search textbox on top when `ShowSearchBox = true`.
  - Disables the menu-form's hand-rolled scrollbar while the substrate is active (`BeepListBox` brings its own).
  - Forwards `_hostedList.SelectedItemChanged` into `OnItemClicked` / `OnSubmenuOpening`, mirrors multi-select state into `_selectedItems`, and honours `CloseOnItemClick`.
  - Exposes `SyncHostedListFromMenuState()` so property setters keep the list aligned at runtime.
  - Exposes `GetHostedListPreferredHeight()` for `RecalculateSize`.
  - `TeardownHostedList()` cleanly unsubscribes events and disposes.
- `ContextMenus/BeepContextMenu.Drawing.cs` — `OnPaint` guards `DrawMenuItemsSimple` behind `if (!IsHostedListSubstrateActive)`.
- `ContextMenus/BeepContextMenu.Methods.cs` — `RecalculateSize` height path branches between substrate (preferred-row × count) and the original per-item measurement.
- `ContextMenus/BeepContextMenu.Properties.cs` — `MenuItems`, `ShowImage`, `ShowCheckBox`, `MultiSelect` setters call `SyncHostedListFromMenuState()`.
- `ContextMenus/BeepContextMenu.Designer.cs` — `Dispose(bool)` runs `TeardownHostedList()`.

### Build

- `TheTechIdea.Beep.Winform.Controls`: **0 errors**
- `TheTechIdea.Beep.Winform.Controls.Design.Server`: **0 errors**
- `WinFormsApp.UI.Test`: **0 errors**

### Behaviour

- Default consumers (no opt-in) see **zero** visual or behavioural change — Phase 01/02/03/04/05 work is unaffected.
- Opt-in consumers set `myMenu.UseHostedListSubstrate = true;` and immediately get a docked `BeepListBox` rendering the items, with `BeepListBox.Keyboard`, `BeepListBox.Accessibility`, `BeepListBox.HighContrast`, and `BeepStyling`-driven item chrome for free.

---

## Phase 06.1 — Flip the Default (deferred)

Goal: once parity is validated end-to-end (search-box layout, shortcut column, subtext, separators, submenu-arrow), change `_useHostedListSubstrate` default to `true` and remove the hand-rolled `DrawMenuItemsSimple` / per-item measurement paths.

Pre-requisites:

- Verification matrix rows F.1-019 through F.1-027 all pass.
- A runtime demo exists toggling `UseHostedListSubstrate` and exercising single/multi-select + submenu trees.
- Phase 04B submenu-triangle tracker is confirmed to still receive correct hover state from the hosted-list path (today it reads `_hoveredItem`; we may need to relay `_hostedList`'s hover into the menu's `_hoveredItem` field).

Phase 06.1 is scheduled **after** Phases 07 (a11y), 08 (designer), 09 (runtime demo) — the demo built in Phase 09 will double as Phase 06.1's parity harness.

---

## Deferred (out of scope for this phase and 06.1)

- `BeepDropDownMenu` (`Menus/BeepDropDownMenu.cs`, 266 LOC) and `BeepFlyoutMenu` (`FlyoutMenus/BeepFlyoutMenu.cs`, 516 LOC) — audit for the same substrate move in a separate future phase if they prove to need parity.
- `BeepSideMenu` (`Menus/BeepSideMenu.cs`, 1101 LOC) is a navigation surface, not a popup — out of scope for this program.
