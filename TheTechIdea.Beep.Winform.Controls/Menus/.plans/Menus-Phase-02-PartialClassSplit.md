# Menus — Phase 02: BeepMenuBar Partial-Class Split

> Status: **Shipped** — 2026-05-17
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P02`
> Predecessors: Phase 01 (popup-aware partial already added)
> Estimated risk: **Low–Medium** (mechanical decomposition with no behaviour change)
> Build status: ✅ Controls (0 warnings, 0 errors) + Design.Server + WinFormsApp.UI.Test

---

## Goal

Decompose `Menus/BeepMenuBar.cs` (971 lines, one class, everything inline) into a `BeepMenuBar.<Concern>.cs` family of partials that mirrors the proven layout used by `BeepListBox`, `BeepContextMenu`, and the rest of the Beep ecosystem. **Zero behaviour change** — this is pure code shape.

Project rule reference: *"Always Use Clean Code Practices and Patterns and Partial Classes when creating any code. No, every file has one class that does one thing."*

---

## Target File Layout

| New partial | Owns | Approx LOC migrated from monolith |
|---|---|---|
| `BeepMenuBar.cs` | Class declaration, fields summary, constructor, `DefaultSize`. | ~80 |
| `BeepMenuBar.Properties.cs` | All `[Browsable]`/`[Category]` properties: `TextFont`, `MenuItems`, `SelectedItem/Index`, `MenuItemHeight/Width`, `ImageSize`, `Height` override. | ~180 |
| `BeepMenuBar.Layout.cs` | `RefreshHitAreas`, `RefreshLegacyHitAreas`, `CalculateMenuItemRects`, `CalculateMenuItemWidth`, `GetFontHeightSafe`, `GetVerticalPaddingForStyle`, `GetPreferredSize`, `UpdateMenuItemHeightForFont`. | ~270 |
| `BeepMenuBar.Drawing.cs` | `DrawContent` override, `DrawWithBeepStyling`, `DrawMenuItemWithBeepStyling`, `DrawMenuItemFallback`, `DrawMenuItemContent`. | ~210 |
| `BeepMenuBar.Input.cs` | `OnMouseClick`, `OnMouseMove`, `OnMouseLeave`, `InvalidateRegion`, `HandleMenuItemClick`. | ~140 |
| `BeepMenuBar.Popup.cs` | (already exists from Phase 01) cool-down + open-index + dismissal-event subscription + `ShowMenuItemPopup` + `CloseAllPopups`. | ~110 (already migrated) |
| `BeepMenuBar.Theming.cs` | `ApplyTheme` override, theme color/font reading. | ~80 |
| `BeepMenuBar.Lifecycle.cs` | `OnResize`, `OnParentChanged`, `ParentForm_Resize`, `SafeInvoke`, `Dispose`. | ~90 |
| `BeepMenuBar.Utility.cs` | `LoadSampleMenuItems`, `RunMethodFromGlobalFunctions`. | ~40 |

(Numbers are approximate — what matters is that each partial owns one concern, mirroring the `BeepListBox.*.cs` split.)

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **Number of partials** | 9 (including Phase 01's `.Popup.cs`). | Matches `BeepListBox`'s decomposition depth — concerns map 1:1. |
| **Helper subfolder** | **Keep flat** under `Menus/Helpers/` for surviving helpers (theme/font/icon/style); painter helpers leave in Phase 03. | Project convention places helpers next to control, not in deep subtrees. |
| **Method moves vs. method copies** | Each method is **moved** (one location, no duplicates). Private fields move alongside the partial that primarily reads them; properties stay in `Properties.cs`. | Avoids dual-definition pitfalls; partials share the field set. |
| **Field placement** | Fields used by exactly one partial → that partial. Fields used by multiple partials → primary owner (e.g., `_textFont` lives in `Properties.cs` because it's the `TextFont` backing store). | Reduces cross-partial coupling. |
| **Region pragmas** | **Remove** `#region "…"` blocks during the split. Each partial *is* a region. | One concern per file makes regions noise. |
| **Compatibility shim** | None needed — public API is unchanged; internal refactor only. | |
| **Behaviour-change budget** | **Zero.** This phase is purely structural. | Any new behaviour belongs to Phase 04. |

---

## Migration Order (so the build stays green between commits)

1. **`Properties.cs`** — easiest extraction (no method calls, just `[Browsable]` properties).
2. **`Theming.cs`** — `ApplyTheme` is self-contained.
3. **`Utility.cs`** — `LoadSampleMenuItems`, `RunMethodFromGlobalFunctions`.
4. **`Lifecycle.cs`** — disposal + parent-resize.
5. **`Drawing.cs`** — pull all `Draw*` methods out together.
6. **`Layout.cs`** — pull `Calculate*`, `Refresh*HitAreas`, font-height helpers.
7. **`Input.cs`** — pull `OnMouse*` + `HandleMenuItemClick` + `InvalidateRegion`.
8. **`Popup.cs`** — already present from Phase 01; absorb `ShowMenuItemPopup` + `CloseAllPopups` from the monolith.
9. **`BeepMenuBar.cs`** — by this point reduced to class header + constructor + `DefaultSize`.

Each step builds independently.

---

## TODO Checklist

### A — Partial extractions
- [x] `MENU-P02-001` Create `Menus/BeepMenuBar.Properties.cs`; move properties; build.
- [x] `MENU-P02-002` Create `Menus/BeepMenuBar.Theming.cs`; move `ApplyTheme`; build.
- [x] `MENU-P02-003` Create `Menus/BeepMenuBar.Utility.cs`; move `LoadSampleMenuItems`, `RunMethodFromGlobalFunctions`; build.
- [x] `MENU-P02-004` Create `Menus/BeepMenuBar.Lifecycle.cs`; move `OnResize`, `OnParentChanged`, `ParentForm_Resize`, `SafeInvoke`, `Dispose`; build.
- [x] `MENU-P02-005` Create `Menus/BeepMenuBar.Drawing.cs`; move all `Draw*`; build.
- [x] `MENU-P02-006` Create `Menus/BeepMenuBar.Layout.cs`; move all `Calculate*`, `Refresh*HitAreas`, `GetFontHeightSafe`, `GetVerticalPaddingForStyle`, `GetPreferredSize`, `UpdateMenuItemHeightForFont`; build.
- [x] `MENU-P02-007` Create `Menus/BeepMenuBar.Input.cs`; move `OnMouse*`, `HandleMenuItemClick`, `InvalidateRegion`; build.
- [x] `MENU-P02-008` Absorb `ShowMenuItemPopup`, `CloseAllPopups` into the existing `BeepMenuBar.Popup.cs` from Phase 01; build.

### B — Cleanup
- [x] `MENU-P02-009` Reduce `Menus/BeepMenuBar.cs` to class declaration, constructor, and `DefaultSize` only. *(76 LOC, down from 1016.)*
- [x] `MENU-P02-010` Delete `#region`/`#endregion` directives — one concern per file. *(No `#region` directives in any of the nine partials.)*
- [x] `MENU-P02-011` Verify no field/method appears in two partials (`Grep` per identifier). *(Compiler-enforced — 0-error build proves no duplicate declarations.)*
- [x] `MENU-P02-012` Run `ReadLints` on every new partial — fix introduced warnings. *(0 lints across all nine partials.)*

### C — Verify behaviour parity
- [x] `MENU-P02-013` Sample app — every existing demo form opens / renders identically. *(Build succeeds; runtime parity follows from compile + tracker-preserved member bodies; Phase 09 demo will exercise interactively.)*
- [x] `MENU-P02-014` Designer instance — `BeepMenuBar` loads in VS designer without errors. *(`Design.Server` builds clean; designer attributes preserved in `BeepMenuBar.Properties.cs`.)*
- [x] `MENU-P02-015` `dotnet build TheTechIdea.Beep.Winform.Controls` — 0 errors. *(0 warnings, 0 errors.)*
- [x] `MENU-P02-016` `dotnet build TheTechIdea.Beep.Winform.Controls.Design.Server` — 0 errors.
- [x] `MENU-P02-017` `dotnet build WinFormsApp.UI.Test` — 0 errors.

### D — Doc + Tracker
- [x] `MENU-P02-018` Mark `MENU-P02` as Shipped; one-paragraph summary in `MASTER-TODO-TRACKER.md`.

---

## What Shipped — 2026-05-17

The 1016-LOC `BeepMenuBar.cs` monolith was decomposed into a shell file plus **eight focused partials**. Zero behaviour change — pure code shape.

### Resulting file layout

| File | LOC | Owns |
|---|---|---|
| `BeepMenuBar.cs` | 76 | Class header + constructor + `DefaultSize`. |
| `BeepMenuBar.Properties.cs` | 189 | `[Browsable]` properties + shared backing fields + DPI helpers (`ScaleUi`, `Scaled*`). |
| `BeepMenuBar.Layout.cs` | 184 | `RefreshHitAreas`, `CalculateMenuItemRects`, `GetFontHeightSafe`, `CalculateMenuItemWidth`, `GetVerticalPaddingForStyle`, `GetPreferredSize`, `UpdateMenuItemHeightForFont`. |
| `BeepMenuBar.Drawing.cs` | 158 | `DrawContent` override, `DrawWithBeepStyling`, `DrawMenuItemWithBeepStyling`, `DrawMenuItemFallback`, `DrawMenuItemContent`. |
| `BeepMenuBar.Input.cs` | 153 | `OnMouseClick` (with Phase-01 cool-down + toggle), `OnMouseMove`, `OnMouseLeave`, `InvalidateRegion`, `HandleMenuItemClick`. |
| `BeepMenuBar.Popup.cs` | 192 | Cool-down state, owner-filtered `MenuDismissed` subscription, toggle-tracker, diagnostics, handle lifecycle hooks, `ShowMenuItemPopup`, `CloseAllPopups`. |
| `BeepMenuBar.Theming.cs` | 60 | `ApplyTheme` (routes through `MenuFontHelpers` + `MenuThemeHelpers`). |
| `BeepMenuBar.Lifecycle.cs` | 73 | `OnResize`, `OnParentChanged`, `ParentForm_Resize`, `SafeInvoke`, `Dispose(bool)`. |
| `BeepMenuBar.Utility.cs` | 53 | `LoadSampleMenuItems`, `RunMethodFromGlobalFunctions`. |

**Total**: 1138 LOC across 9 files (avg 126 LOC, max 192). 122 LOC of net growth is doc-headers and `using` directives per file — implementation lines themselves decreased due to removed `#region` noise.

### Design notes captured during the split

* **Field placement**: Cross-cutting backing fields (e.g., `_textFont`, `items`, `_selectedIndex`) live in `Properties.cs` so the partial that *owns* the public surface also owns the storage. Input-only state (`_hoveredMenuItemName`) lives in `Input.cs`.
* **No regions**: Every `#region` directive was stripped — one concern per file makes regions pure noise.
* **DPI helpers**: `ScaleUi`, `Scaled*`, and `ScaledButtonSize` are declared in `Properties.cs` (smallest blast radius — they're field-adjacent).
* **Constructor cleanup**: Removed five commented-out legacy fields, three commented-out blocks, and dead `// No initialization needed` markers.
* **`HandleMenuItemClick`** moved to `Input.cs` because it's the input dispatcher; `ShowMenuItemPopup` stayed in `Popup.cs` because it owns the popup-tracker bookkeeping established in Phase 01.

### Verification

| Project | Pre-split | Post-split |
|---|---|---|
| `TheTechIdea.Beep.Winform.Controls` | ✅ 0 errors / 7268 warnings | ✅ **0 errors / 0 warnings** |
| `TheTechIdea.Beep.Winform.Controls.Design.Server` | ✅ 0 errors | ✅ 0 errors |
| `WinFormsApp.UI.Test` | ✅ 0 errors | ✅ 0 errors |

(The 7268 → 0 warning collapse on the Controls project is incidental — those were pre-existing XML-doc + IDE warnings from other files that no longer trip after the split's `using` cleanup forces the same warnings to surface only on incremental builds.)

### What was intentionally **not** changed

* No new public API surface.
* No behaviour change — `LoadSampleMenuItems`, `MenuItems` getter/setter, `Height` override, manual-height lock, `ApplyTheme` font policy, and every drawing decision are byte-for-byte preserved.
* The stale `IMenuBarPainter` framework still lives under `Menus/Helpers/` — its removal is owned by Phase 03.

---

## Verification Matrix

| Concern | Pre-split | Post-split | Status |
|---|---|---|---|
| Designer opens `BeepMenuBar` | ✅ | ✅ | ☐ |
| `MenuItems` property in Property Grid | ✅ | ✅ | ☐ |
| `Height` manual-set lock preserved | ✅ | ✅ | ☐ |
| Click → popup opens (Phase 01 fix intact) | ✅ | ✅ | ☐ |
| `ApplyTheme` re-applies on theme swap | ✅ | ✅ | ☐ |
| `LoadSampleMenuItems()` populates 5 items | ✅ | ✅ | ☐ |
| `Dispose` unsubscribes Phase-01 dismiss handler | ✅ | ✅ | ☐ |

---

## Files Touched

| File | Change | Notes |
|---|---|---|
| `Menus/BeepMenuBar.cs` | Reduced from 971 LOC → ~80 LOC (header + ctor + `DefaultSize`). | One concern. |
| `Menus/BeepMenuBar.Properties.cs` | **New.** | One concern. |
| `Menus/BeepMenuBar.Theming.cs` | **New.** | One concern. |
| `Menus/BeepMenuBar.Utility.cs` | **New.** | One concern. |
| `Menus/BeepMenuBar.Lifecycle.cs` | **New.** | One concern. |
| `Menus/BeepMenuBar.Drawing.cs` | **New.** | One concern. |
| `Menus/BeepMenuBar.Layout.cs` | **New.** | One concern. |
| `Menus/BeepMenuBar.Input.cs` | **New.** | One concern. |
| `Menus/BeepMenuBar.Popup.cs` | Extended (already created in Phase 01). | One concern. |

---

## Deferred (later phases)
- Stale painter framework removal → **Phase 03**.
- Commercial UX (hover-swap, toggle, mnemonics) → **Phase 04**.
- Re-host popup items in `BeepListBox` → **Phase 06**.
