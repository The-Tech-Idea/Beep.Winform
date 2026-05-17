# ADR-001 — Styling Stack Contract for Beep Menus

> Status: **Accepted** — ratified by Phase 03 of the Menus & ContextMenus revise/enhance program.
> Scope: `Menus/**` and `ContextMenus/**` inside `TheTechIdea.Beep.Winform.Controls`.
> Authoritative for all future contributions to either folder.
> Cross-reference: `Menus/.plans/MASTER-TODO-TRACKER.md` → "Established Styling Stack".

---

## Context

`BeepMenuBar` and `BeepContextMenu` had accumulated a parallel painter
framework (`IMenuBarPainter`, `MenuBarPainterBase`, `MenuBarContext`,
`MenuBarRenderingHelpers`, plus a competing `MenuBarStyle` enum and its
`MenuStyleHelpers` dispatcher) that was never wired into the production
controls. At the same time, the rest of the Beep ecosystem standardised
on a single chrome-rendering pipeline routed through `BeepStyling` and
the per-style factories. Maintaining two parallel stacks invites drift,
duplicated chrome calls, and visual inconsistency between menus and
every other Beep surface.

## Decision

**There is exactly one styling stack for every Beep control. Menus and
context menus participate in it like everyone else — they do not build
their own painter framework.**

### The contract

| Concern | Authoritative API | How menus consume it |
|---|---|---|
| Per-control visual variant | `BeepControlStyle` (enum) + `BaseControl.ControlStyle` | `BeepMenuBar.Drawing.cs` reads `ControlStyle` and passes it to `BeepStyling.PaintControl`. |
| Form chrome contract | `FormStyle` + `BaseControl.UseFormStylePaint` | Inherited from `BaseControl` — menus opt-in by default policy. |
| Background painting | `BackgroundPainterFactory` (per-style implementations) | Called via `BeepStyling.PaintControl(...)`. |
| Border painting | `BorderPainterFactory` (per-style implementations) | Called via `BeepStyling.PaintControl(...)`. |
| Shadow painting | `ShadowPainterFactory` (per-style implementations) | Called via `BeepStyling.PaintControl(...)`. |
| Theme colour tokens | `IBeepTheme` via `BeepThemesManager.CurrentTheme` | Read through `MenuThemeHelpers`. Never hard-code colours. |
| Theme fonts | `BeepFontManager` (via `MenuFontHelpers`) | `MenuFontHelpers.GetMenuItemFont(style, theme)`. |
| Icons | `StyledImagePainter` (via `BeepStyling.PaintStyleImage`) | Already in `Menus/BeepMenuBar.Drawing.cs::DrawMenuItemContent`. |
| Popup item rendering (selection, hover, keyboard, accessibility, drag, theming) | `BeepListBox` (`BindingList<SimpleItem>`) | `BeepContextMenu` re-hosts its item area inside `BeepListBox` in Phase 06. |

### Prohibitions

The following patterns are **disallowed** inside `Menus/` and `ContextMenus/`:

1. **No per-control painter interface** — no `IMenuBarPainter`,
   `IContextMenuPainter`, `IMenuItemPainter`, or similar. Chrome
   painting always goes through `BeepStyling.PaintControl`.
2. **No "Base Painter" abstractions** — the project rule
   (`.cursor/rules/mycontrolsonly.mdc`) is explicit: *"Always Create
   Distinct Painters. No Base Painter."* Per-style logic lives inside
   each factory implementation, not in a shared base.
3. **No parallel style enums** — `BeepControlStyle` and `FormStyle`
   together describe every visual variant a menu can take. A second
   enum (such as the removed `MenuBarStyle`) is forbidden.
4. **No hand-rolled item-rendering pipelines for popup contents** —
   `BeepListBox` already supports `BindingList<SimpleItem>`, hover,
   selection, keyboard navigation, theming, accessibility, and drag.
   Re-host it; do not duplicate.
5. **No hard-coded colours** — every brush/pen reads from
   `_currentTheme` (via `MenuThemeHelpers` for menus). The only
   exception is the catastrophic-fallback path inside
   `DrawMenuItemFallback`, which exists solely to keep the surface
   visible if `BeepStyling.PaintControl` throws.

### Where the legitimate helpers live

`Menus/Helpers/` houses **thin wrappers only**:

| Helper | Role |
|---|---|
| `MenuFontHelpers.cs` | Translates `(BeepControlStyle, IBeepTheme) → Font` via `BeepFontManager`. |
| `MenuThemeHelpers.cs` | Translates `(IBeepTheme, UseThemeColors) → Color` for menubar fore/back/border/gradient. |
| `MenuIconHelpers.cs` | SVG-name normalisation + standard menu-icon catalogue. |

These wrappers are allowed because they delegate to framework primitives
and never invent new chrome behaviour. New helpers added to this folder
must follow the same rule.

## Consequences

* `BeepMenuBar` and `BeepContextMenu` benefit automatically from any
  new `BeepControlStyle` variant added in `Styling/`. There is no
  "menu hookup" step.
* When a theme adds new colour tokens, menus inherit them through
  `MenuThemeHelpers` — typically one new wrapper method.
* The cost of a new style variant for menus is **zero lines of code in
  `Menus/`** — the work happens in `Styling/<Factory>/` like every
  other control.
* Code review can reject any PR that introduces a new painter interface
  or base painter inside `Menus/` or `ContextMenus/` by citing this
  ADR.

## Status of competing artefacts (as of 2026-05-17)

| Artefact | Status |
|---|---|
| `Menus/Helpers/IMenuBarPainter.cs` | Removed in Phase 03 (`MENU-P03`). |
| `Menus/Helpers/MenuBarPainterBase.cs` | Removed in Phase 03. |
| `Menus/Helpers/MenuBarContext.cs` | Removed in Phase 03. |
| `Menus/Helpers/MenuBarRenderingHelpers.cs` | Removed in Phase 03. |
| `Menus/Helpers/MenuBarStyle.cs` (parallel enum) | Removed in Phase 03. |
| `Menus/Helpers/MenuStyleHelpers.cs` (dispatcher for the dead enum) | Removed in Phase 03. |
| `Menus/plan.md` (aspirational pre-revise plan) | Archived in `.plans/archive/2024-pre-revise-plan.md`. |
| `Menus/*.md` (four post-mortems) | Archived in `.plans/archive/`. |

## See also

* `MASTER-TODO-TRACKER.md` — pinned "Established Styling Stack" table.
* `Menus-Phase-03-StylingStackConsolidation.md` — the phase that ratified this ADR.
* `Styling/BeepStyling.cs` + the three `*PainterFactory.cs` files — the
  actual chrome pipeline this ADR points at.
* `ListBoxs/BeepListBox.*.cs` — the item-rendering substrate Phase 06
  re-hosts inside `BeepContextMenu`.
