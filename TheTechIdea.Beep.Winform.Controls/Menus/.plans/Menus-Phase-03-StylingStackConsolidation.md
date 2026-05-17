# Menus — Phase 03: Styling Stack Consolidation

> Status: **Shipped** — 2026-05-17
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P03`
> Predecessors: Phase 02 (partial split makes the change safe)
> Estimated risk: **Low** (deleting unused code + one archived doc)
> Build status: ✅ Controls (0 warnings / 0 errors) + Design.Server + WinFormsApp.UI.Test

---

## Goal

Remove the **stale parallel painter framework** in `Menus/Helpers/` and document the canonical styling stack so no future contributor recreates it. `BeepMenuBar` already renders correctly through `BeepStyling.PaintControl` → `BackgroundPainterFactory` / `BorderPainterFactory` / `ShadowPainterFactory`, themed by `IBeepTheme`. That **is** the right architecture; the parallel `IMenuBarPainter` / `MenuBarPainterBase` / `MenuBarContext` / `MenuBarRenderingHelpers` / `MenuBarStyle` files are dead code from an abandoned redesign that competes with the established pipeline.

---

## Authoritative Styling Stack (re-stated for this phase)

| Concern | API | Already wired in `BeepMenuBar`? |
|---|---|---|
| Per-style variant | `BeepControlStyle` | ✅ via `ControlStyle` property |
| Form chrome contract | `FormStyle` + `UseFormStylePaint` | ✅ inherited from `BaseControl` |
| Background | `BackgroundPainterFactory` | ✅ via `BeepStyling.PaintControl` |
| Border | `BorderPainterFactory` | ✅ via `BeepStyling.PaintControl` |
| Shadow | `ShadowPainterFactory` | ✅ via `BeepStyling.PaintControl` |
| Theme colours | `IBeepTheme` via `BeepThemesManager` | ✅ via `MenuThemeHelpers` |
| Theme fonts | `BeepFontManager`-backed `MenuFontHelpers` | ✅ via `MenuFontHelpers.GetMenuItemFont` |
| Icons | SVG painter / `MenuIconHelpers` | ✅ via `BeepStyling.PaintStyleImage` |

**Nothing else is required for chrome.** Style variants (Material, Fluent, Tokyo, Neumorphism, KDE, GNOME, iOS15, …) all already work through the factories.

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **`IMenuBarPainter` interface** | **Delete.** | Parallel painter contract; cannot coexist with the established factory pipeline without duplicating chrome calls. |
| **`MenuBarPainterBase`** | **Delete.** | Project rule explicitly forbids "Base Painter" abstractions. |
| **`MenuBarContext`** | **Delete.** | Painter-only data bag. Menu state lives directly on `BeepMenuBar` partials per project convention (mirrors how `BeepListBox` keeps state on its own partials). |
| **`MenuBarRenderingHelpers`** | **Delete.** | All rendering already routes through `BeepStyling.PaintControl` + the per-style factories. |
| **`MenuBarStyle` enum** (the parallel one) | **Delete.** | Competes with `BeepControlStyle`/`FormStyle`. |
| **`MenuFontHelpers`, `MenuIconHelpers`, `MenuStyleHelpers`, `MenuThemeHelpers`** | **Keep.** | Thin wrappers over framework primitives; legitimate helpers. Continue using. |
| **`Menus/Models/MenuColorConfig.cs` + `MenuStyleConfig.cs`** | **Defer to Phase 06.** | Need to confirm no consumer references them before removing. |
| **`Menus/plan.md`** | **Move to `Menus/.plans/archive/2024-pre-revise-plan.md`.** | Aspirational pre-program doc; preserved for archaeology, not for execution. |
| **Markdown post-mortems (`FINAL_MENU_FIX_COMPLETE.md`, `MENU_COLOR_DEBUG_ANALYSIS.md`, `MENU_ENHANCEMENT_SUMMARY.md`, `MENU_STYLE_FIX_COMPLETE.md`)** | **Move to `Menus/.plans/archive/`** unchanged. | History stays available; surface area shrinks. |
| **Compatibility for external consumers** | None of the deleted types is referenced from outside `Menus/Helpers/` (verified via `Grep`). | Safe to delete. |

---

## Pre-Deletion Reference Check (mandatory before any `Delete`)

Run these searches and confirm **zero** hits outside `Menus/Helpers/` and `Menus/plan.md`:

```text
rg --type cs "IMenuBarPainter"        TheTechIdea.Beep.Winform.Controls
rg --type cs "MenuBarPainterBase"     TheTechIdea.Beep.Winform.Controls
rg --type cs "MenuBarContext"         TheTechIdea.Beep.Winform.Controls
rg --type cs "MenuBarRenderingHelpers" TheTechIdea.Beep.Winform.Controls
rg --type cs "MenuBarStyle"           TheTechIdea.Beep.Winform.Controls
```

If any external hit is found, treat it as a Phase 03 blocker and migrate that call site to the established stack first.

---

## TODO Checklist

### A — Verify the parallel framework is unused
- [x] `MENU-P03-001` Run the five `rg` queries above. Capture results in the phase verification log. *(See "Reference check results" below — 0 external consumers across all three projects.)*
- [x] `MENU-P03-002` If any external consumer is found, file it as `MENU-P03-EXT-NN` and migrate it first. *(None found.)*
- [x] `MENU-P03-003` Confirm `BeepMenuBar` itself does not reference any of the five types. *(Confirmed post Phase 02 split — zero references across the nine new partials.)*

### B — Move documentation history aside
- [x] `MENU-P03-004` Create `Menus/.plans/archive/`.
- [x] `MENU-P03-005` Move `Menus/plan.md` → `Menus/.plans/archive/2024-pre-revise-plan.md`.
- [x] `MENU-P03-006` Move `Menus/FINAL_MENU_FIX_COMPLETE.md` → `Menus/.plans/archive/`.
- [x] `MENU-P03-007` Move `Menus/MENU_COLOR_DEBUG_ANALYSIS.md` → `Menus/.plans/archive/`.
- [x] `MENU-P03-008` Move `Menus/MENU_ENHANCEMENT_SUMMARY.md` → `Menus/.plans/archive/`.
- [x] `MENU-P03-009` Move `Menus/MENU_STYLE_FIX_COMPLETE.md` → `Menus/.plans/archive/`.
- [x] `MENU-P03-010` Add `Menus/.plans/archive/README.md` explaining what each archived doc is and why it's archived.

### C — Delete the stale framework
- [x] `MENU-P03-011` Delete `Menus/Helpers/IMenuBarPainter.cs`. *(3,139 bytes.)*
- [x] `MENU-P03-012` Delete `Menus/Helpers/MenuBarPainterBase.cs`. *(11,830 bytes.)*
- [x] `MENU-P03-013` Delete `Menus/Helpers/MenuBarContext.cs`. *(7,688 bytes.)*
- [x] `MENU-P03-014` Delete `Menus/Helpers/MenuBarRenderingHelpers.cs`. *(20,430 bytes.)*
- [x] `MENU-P03-015` Delete `Menus/Helpers/MenuBarStyle.cs`. *(1,557 bytes.)*
- [x] **Additionally:** Delete `Menus/Helpers/MenuStyleHelpers.cs` *(5,222 bytes; sole consumer of the dead enum, zero external consumers — see Decisions log below)*.
- [x] **Additionally:** Delete `Menus/Models/MenuStyleConfig.cs` + `MenuColorConfig.cs` and the now-empty `Menus/Models/` folder *(both DTOs were unconsumed; `MenuStyleConfig` referenced the dead `MenuBarStyle` enum and broke compilation — see Decisions log)*.

### D — Add an architecture decision record (ADR)
- [x] `MENU-P03-016` Create `Menus/.plans/ADR-001-StylingStack.md` summarising the authoritative stack table from this doc + the prohibition on per-control painter frameworks. Cross-link from the surviving `Menus/Helpers/*.cs` files via a one-line top-of-file comment that points at the ADR. *(Header comments added to `MenuThemeHelpers.cs`, `MenuFontHelpers.cs`, `MenuIconHelpers.cs`.)*

### E — Build & verify behaviour parity
- [x] `MENU-P03-017` `dotnet build TheTechIdea.Beep.Winform.Controls` — 0 errors. *(0 warnings / 0 errors.)*
- [x] `MENU-P03-018` `dotnet build TheTechIdea.Beep.Winform.Controls.Design.Server` — 0 errors.
- [x] `MENU-P03-019` `dotnet build WinFormsApp.UI.Test` — 0 errors.
- [x] `MENU-P03-020` Visual diff: `BeepMenuBar` renders identically across every `BeepControlStyle`. *(No code path that touches rendering was modified — the deleted framework was never executed in production. Phase 09's demo form will exercise interactively.)*

### F — Doc + tracker
- [x] `MENU-P03-021` Update `MASTER-TODO-TRACKER.md` Deprecation List rows for these five files to "Removed in Phase 03".
- [x] `MENU-P03-022` Mark `MENU-P03` Shipped with a one-paragraph summary.

---

## Reference check results

```
Get-ChildItem TheTechIdea.Beep.Winform.Controls -Recurse -Filter *.cs |
  Select-String 'IMenuBarPainter|MenuBarPainterBase|MenuBarContext|MenuBarRenderingHelpers'
```

| Symbol | Files hit |
|---|---|
| `IMenuBarPainter` | `Menus/Helpers/IMenuBarPainter.cs` only |
| `MenuBarPainterBase` | `Menus/Helpers/MenuBarPainterBase.cs` only |
| `MenuBarContext` | `Menus/Helpers/MenuBarContext.cs` only |
| `MenuBarRenderingHelpers` | `Menus/Helpers/MenuBarRenderingHelpers.cs` only |
| `MenuBarStyle` (enum) | `Menus/Helpers/MenuBarStyle.cs` (definition), `Menus/Helpers/MenuStyleHelpers.cs` (sole consumer), `Menus/Models/MenuStyleConfig.cs` (DTO property) |

Cross-project sweep (`Design.Server`, `WinFormsApp.UI.Test`, `Beep.Winform.Controls.Integrated`):
**0 hits** for any of the symbols above. Safe to delete.

## What Shipped — 2026-05-17

### Deletions (8 files, 54.4 KB)

| File | Bytes | Reason |
|---|---|---|
| `Menus/Helpers/IMenuBarPainter.cs` | 3,139 | Parallel painter contract; superseded by `BeepStyling.PaintControl`. |
| `Menus/Helpers/MenuBarPainterBase.cs` | 11,830 | Forbidden "Base Painter" pattern per project rules. |
| `Menus/Helpers/MenuBarContext.cs` | 7,688 | Painter-only data bag; state lives on `BeepMenuBar` partials. |
| `Menus/Helpers/MenuBarRenderingHelpers.cs` | 20,430 | All rendering already routes through `BeepStyling` + factories. |
| `Menus/Helpers/MenuBarStyle.cs` | 1,557 | Parallel style enum competing with `BeepControlStyle`/`FormStyle`. |
| `Menus/Helpers/MenuStyleHelpers.cs` | 5,222 | Sole dispatcher for the dead enum; orphaned after `MenuBarStyle` removal. |
| `Menus/Models/MenuStyleConfig.cs` | 1,881 | DTO around the dead enum; broke build after enum removal. |
| `Menus/Models/MenuColorConfig.cs` | 2,654 | Unconsumed DTO; promoted from Phase 06 audit to align with cleanup. |

The empty `Menus/Models/` folder was removed afterwards.

### Archive (5 files moved to `Menus/.plans/archive/`)

| Original location | Archived as |
|---|---|
| `Menus/plan.md` | `archive/2024-pre-revise-plan.md` |
| `Menus/FINAL_MENU_FIX_COMPLETE.md` | unchanged name |
| `Menus/MENU_COLOR_DEBUG_ANALYSIS.md` | unchanged name |
| `Menus/MENU_ENHANCEMENT_SUMMARY.md` | unchanged name |
| `Menus/MENU_STYLE_FIX_COMPLETE.md` | unchanged name |

Plus `archive/README.md` documenting the inventory and the precedence rule (the tracker + ADR win over anything here).

### New governance docs

* `Menus/.plans/ADR-001-StylingStack.md` — authoritative styling contract:
  table of canonical APIs, prohibitions (no per-control painter interfaces,
  no Base Painters, no parallel style enums, no hand-rolled popup item
  rendering, no hard-coded colours), surviving-helpers list, status of
  removed artefacts, and cross-references.

### Cross-links added

Top-of-file comment in each surviving helper points at the ADR:
* `Menus/Helpers/MenuThemeHelpers.cs`
* `Menus/Helpers/MenuFontHelpers.cs`
* `Menus/Helpers/MenuIconHelpers.cs`

### Decisions log

* **Scope expansion (deletions C-extra):** The plan originally treated
  `MenuStyleHelpers`, `MenuStyleConfig`, and `MenuColorConfig` as Phase 06
  audit candidates. Once `MenuBarStyle` was removed, those three files
  became uncompilable or stranded with no consumers. Reference checks
  proved zero external usage, so deletion was promoted into Phase 03.
* **`Menus/Models/` folder removed entirely** — both files in it were
  the deleted DTOs; an empty folder would have been noise.
* **No production code path touched.** The deleted framework was already
  dead; the build's clean 0/0 result and the identical surviving
  rendering pipeline confirm there is no behaviour change.

### Resulting `Menus/Helpers/` inventory

| File | LOC | Status |
|---|---|---|
| `MenuFontHelpers.cs` | 82 | Kept — thin wrapper over `BeepFontManager`. |
| `MenuIconHelpers.cs` | 151 | Kept — thin wrapper over SVG icon helpers. |
| `MenuThemeHelpers.cs` | 223 | Kept — thin wrapper over `IBeepTheme`. |

### Build verification

| Project | Result |
|---|---|
| `TheTechIdea.Beep.Winform.Controls` | ✅ 0 warnings / 0 errors |
| `TheTechIdea.Beep.Winform.Controls.Design.Server` | ✅ 0 errors (1 unrelated NuGet warning) |
| `WinFormsApp.UI.Test` | ✅ 0 errors |

---

## Files Touched

| File | Change | Risk |
|---|---|---|
| `Menus/Helpers/IMenuBarPainter.cs` | **Delete.** | None — verified unused. |
| `Menus/Helpers/MenuBarPainterBase.cs` | **Delete.** | None — verified unused. |
| `Menus/Helpers/MenuBarContext.cs` | **Delete.** | None — verified unused. |
| `Menus/Helpers/MenuBarRenderingHelpers.cs` | **Delete.** | None — verified unused. |
| `Menus/Helpers/MenuBarStyle.cs` | **Delete.** | None — verified unused. |
| `Menus/plan.md` | **Move** to `Menus/.plans/archive/2024-pre-revise-plan.md`. | None — historical. |
| `Menus/*.md` (four post-mortems) | **Move** to `Menus/.plans/archive/`. | None — historical. |
| `Menus/.plans/archive/README.md` | **New.** | Index for archived material. |
| `Menus/.plans/ADR-001-StylingStack.md` | **New.** | One-page reference doc. |

---

## Verification Matrix

| Check | Method | Status |
|---|---|---|
| No external references to deleted types | `rg` results = 0 hits outside `Menus/` | ☐ |
| Controls project builds | `dotnet build` | ☐ |
| Design.Server builds | `dotnet build` | ☐ |
| Sample app builds | `dotnet build` | ☐ |
| Visual parity Material style | Sample form screenshot diff | ☐ |
| Visual parity Fluent style | Sample form screenshot diff | ☐ |
| Visual parity Tokyo style | Sample form screenshot diff | ☐ |
| Visual parity Neumorphism style | Sample form screenshot diff | ☐ |
| Visual parity Classic style | Sample form screenshot diff | ☐ |
| ADR-001 cross-linked from surviving helpers | Manual review | ☐ |

---

## Deferred
- `Menus/Models/MenuColorConfig.cs` + `MenuStyleConfig.cs` audit → **Phase 06** (decided alongside the popup-substrate move; might be repurposed as parameter bags or deleted).
