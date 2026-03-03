# BeepProgressBar Enhancement Plan
**Reference direction:** Latest Figma progress indicators + modern web/desktop UI standards (Material, Fluent, iOS)  
**Target area:** `TheTechIdea.Beep.Winform.Controls/ProgressBars`  
**Last updated:** 2026-02-27

---

## Goal

Upgrade `BeepProgressBar` and all progress painters to deliver:
- clear progress semantics (`idle`, `in-progress`, `paused/indeterminate`, `completed`, `error`),
- consistent typography/icons and theme parity across painter kinds,
- strong keyboard/focus/accessibility behavior,
- DPI-safe rendering and predictable autosize/layout behavior,
- efficient targeted invalidation and cache hygiene.

---

## Implementation Constraints (Mandatory)

- Always resolve runtime fonts via `BeepThemesManager.ToFont(...)`.
- Use `IconsManagement` (`SvgsUI`/`Svgs`) and `StyledImagePainter` for icon drawing.
- Keep event/state mutation in control layer; painters remain rendering-only.
- Prefer theme tokens first; avoid hardcoded render colors unless fallback.
- Keep painter behavior parity while preserving visual identity.

---

## Backlog

## Phase 1 - Foundation and Tokens (P0)

### PRG-01: Typography unification
Status: Completed
- Migrate `ProgressBarFontHelpers` to `BeepThemesManager.ToFont(...)`.
- Ensure `ApplyTheme()` assigns text fonts through helper.
- Validate display modes (`Percentage`, `TaskProgress`, `CenterPercentage`) for clipping.

### PRG-02: Icon pipeline unification
Status: Completed
- Standardize icon resolution via `ProgressBarIconHelpers` with `SvgsUI` + `Svgs` fallback.
- Ensure all icon painters rely on helper + `StyledImagePainter`.

### PRG-03: Shared painter interaction semantics
Status: Completed
- Normalize hover/focus/disabled semantics across painter kinds.
- Ensure focus visuals are keyboard-friendly and theme-driven.

---

## Phase 2 - Layout, DPI and Accessibility (P0)

### PRG-04: Layout and autosize reliability
Status: Completed
- Validate bar/text metrics under DPI changes.
- Ensure style/size mode transitions don’t cause clipping.

### PRG-05: Keyboard and accessibility parity
Status: Completed
- Improve keyboard interaction for step-based painters.
- Keep accessibility metadata current with progress/value state.

### PRG-06: Hit target and focus ring consistency
Status: Completed
- Ensure focus affordance is consistent for all interactive painter kinds.
- Align hit areas to visual elements.

---

## Phase 3 - Performance and Reliability (P1)

### PRG-07: Targeted invalidation
Status: Completed
- Reduce broad `Invalidate()` calls where practical.
- Invalidate only affected regions for text/progress updates.

### PRG-08: Cache hygiene
Status: Completed
- Improve measurement/resource cache invalidation on theme/font/style changes.
- Prevent stale cache reuse after DPI/style transitions.

### PRG-09: Timer lifecycle hardening
Status: Completed
- Ensure animation/transition timers are consistently started/stopped/disposed.
- Avoid unnecessary redraw cycles when animations are disabled.

---

## Phase 4 - Painter Family Polish (P2)

### PRG-10: Linear family polish
Status: Completed
- Harmonize `Linear`, `LinearBadge`, `LinearTrackerIcon`, `ArrowStripe`, `ArrowHeadAnimated`.

### PRG-11: Step family polish
Status: Completed
- Harmonize `StepperCircles`, `ChevronSteps` labels, spacing, and interactive feedback.

### PRG-12: Ring family polish
Status: Completed
- Harmonize `Ring`, `DottedRing`, `RingCenterImage`, `RadialSegmented`.

---

## File-by-File Focus

- `ProgressBars/BeepProgressBar.cs`
  - Core state transitions, theme application, accessibility integration, invalidation policy.
- `ProgressBars/BeepProgressBar.Core.cs`
  - Painter registry, painter lifecycle, parameter semantics.
- `ProgressBars/Helpers/ProgressBarFontHelpers.cs`
  - Typography migration to `BeepThemesManager.ToFont(...)`.
- `ProgressBars/Helpers/ProgressBarIconHelpers.cs`
  - Icon resolution and `StyledImagePainter` usage consistency.
- `ProgressBars/Painters/*.cs`
  - State parity, focus/hover/disabled behavior, style polish.

---

## Validation Checklist

- Build passes for `TheTechIdea.Beep.Winform.Controls.csproj`.
- No new linter issues in modified ProgressBars files.
- Fonts across display modes follow theme typography without clipping.
- Icon painters resolve gracefully with `SvgsUI`/`Svgs` fallback.
- Interactive painters maintain clear hover/focus/press behavior.
- No regressions in animation/timer behavior.
