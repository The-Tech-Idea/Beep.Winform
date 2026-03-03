# BeepCheckBox Enhancement Plan
**Reference direction:** Latest Figma component patterns + modern web UI checkbox/switch standards (Material, Fluent, iOS, accessible web forms)  
**Target area:** `TheTechIdea.Beep.Winform.Controls/CheckBoxes`  
**Last updated:** 2026-02-27

---

## Goal

Upgrade the `BeepCheckBox` family to a modern, consistent, and highly accessible interaction model with:
- strong state clarity (`unchecked`, `checked`, `indeterminate`, `hover`, `focus`, `disabled`),
- unified painter behavior across all styles,
- predictable keyboard and hit-target ergonomics,
- DPI-safe typography and icon rendering,
- cleaner performance and reduced redundant repaints.

---

## Current Observations

From review of `CheckBoxes`:
- Architecture is already strong: generic core (`BeepCheckBox<T>`), painter strategy (`ICheckBoxPainter`), style/theme helper stack.
- Icon pipeline already uses `StyledImagePainter` and `SvgsUI` fallback logic via `CheckBoxIconHelpers`.
- Font pipeline currently uses `CheckBoxFontHelpers` + `BeepFontManager`, but should be unified with `BeepThemesManager.ToFont(...)` for strict theme/DPI consistency.
- There are many painter variants; behavior parity is likely uneven for edge states (focus-visible, indeterminate weight, disabled contrast, text/icon alignment).
- `CheckBoxSize`, spacing, and text placement are configurable, but responsive/tokenized sizing can be made stricter across styles.

---

## UX/UI Standards to Apply

1. **State expressiveness first**
   - Every style must clearly differentiate `unchecked`, `checked`, `indeterminate`, `hover`, `focus`, `pressed`, `disabled`.
   - Never rely on color alone; use stroke/fill/shape/opacity changes too.

2. **Web and desktop parity**
   - Match modern web form semantics:
     - clear focus ring,
     - keyboard toggle (`Space`) and deterministic tab behavior,
     - minimum practical pointer target.

3. **Figma spacing rhythm**
   - Enforce 4/8/12/16 spacing scale for icon box, label gap, padding, and row rhythm.
   - Keep label baseline visually centered to checkbox glyph.

4. **Typography and legibility**
   - Label text must remain readable in all themes and DPI levels.
   - Disabled text should be visibly de-emphasized but still legible.

5. **Painter consistency**
   - All style painters should share semantic layout metrics and state interpretation.
   - Style-specific visuals are allowed, behavior must remain consistent.

---

## Implementation Constraints (Mandatory)

- **Font sourcing**
  - Always resolve runtime text fonts using `BeepThemesManager.ToFont(...)`.
  - Do not use raw `control.Font` as the primary source for checkbox sizing/measure loops.
  - Keep font mapping centralized in checkbox font helpers.

- **Icons**
  - Resolve check/indeterminate icons through `IconsManagement` (`SvgsUI`, `Svgs`) and helper methods.
  - Paint icons through `StyledImagePainter` only (no ad-hoc raster fallback painting unless absolutely required).
  - Keep icon tint/state colors theme-driven.

- **Painter architecture**
  - Keep event/state detection in control/layout layer.
  - Keep painters rendering-only, no control state mutation in painter methods.

- **Theme consistency**
  - Use theme tokens first (`IBeepTheme`), then style fallback tokens.
  - Avoid hardcoded arbitrary colors in render paths.

- **Legacy state-value compatibility**
  - Add explicit state mapping properties for `unchecked`, `checked`, `indeterminate`.
  - Keep backward compatibility by mapping these to legacy `UncheckedValue` / `CheckedValue` behavior.
  - `indeterminate` must have deterministic mapping rules for legacy consumers (configurable or documented fallback).

---

## Enhancement Backlog

## Phase 1 - Foundation and Visual System (P0)

### CHK-01: Shared checkbox design tokens
- Normalize checkbox metrics (box size, radius, border width, check stroke, label gap, padding) in one shared token model.
- Ensure all painters consume the same base metrics, then apply style-specific overrides.

### CHK-02: Typography unification
- Update `CheckBoxFontHelpers` to route typography via `BeepThemesManager.ToFont(...)`.
- Support consistent scale by style + DPI.
- Verify label clipping/truncation rules.

### CHK-03: Icon and glyph unification
- Standardize `check`, `indeterminate`, and optional custom icon resolution via `IconsManagement`.
- Ensure all painter variants call a single icon helper contract.

### CHK-04: Label layout alignment
- Improve text baseline centering and spacing for left/right text alignment.
- Add robust behavior when `HideText = true`.

---

## Phase 2 - Interaction and Accessibility (P0)

### CHK-05: State semantics parity
- Ensure all painters honor identical state semantics (`hover`, `focus`, `disabled`, `indeterminate`).
- Add explicit focus ring rendering strategy (theme driven).

### CHK-05A: Legacy state mapping properties
- Add dedicated properties for state-to-value mapping, e.g.:
  - `UncheckedStateValue`
  - `CheckedStateValue`
  - `IndeterminateStateValue`
- Wire mappings so legacy `UncheckedValue`/`CheckedValue` still work exactly as before.
- Define precedence rules between old and new properties to avoid breaking existing forms.
- Ensure generic `BeepCheckBox<T>` remains type-safe for all mapped state values.

### CHK-06: Keyboard and toggle behavior
- Confirm `Space` toggle behavior and deterministic tab focus.
- Add clear visual feedback for keyboard-triggered toggles.

### CHK-07: Hit target compliance
- Enforce minimum practical clickable region around glyph + label.
- Keep pointer and keyboard activation areas synchronized.

### CHK-08: Disabled/contrast pass
- Audit disabled state contrast for box border, fill, check mark, and label.
- Ensure readable but de-emphasized disabled appearance in all themes.

---

## Phase 3 - Performance and Reliability (P1)

### CHK-09: Targeted invalidation
- Replace broad repaints with region invalidation where feasible (state box + text region).
- Avoid invalidating entire control for minor hover/focus transitions.

### CHK-10: Painter cache hygiene
- Improve brush/pen/path cache lifecycle and invalidation keys.
- Ensure no stale cache entries after theme/style/font/dpi changes.

### CHK-11: DPI and autosize reliability
- Validate `AutoSize`, `CheckBoxSize`, and text measure results under DPI changes.
- Ensure `OnDpiChangedBeforeParent` updates all layout-critical metrics predictably.

---

## Phase 4 - Advanced UX Variants (P2)

### CHK-12: Switch style parity
- Align `SwitchCheckBoxPainter` semantics with checkbox semantics (focus/disabled/indeterminate fallback policy).
- Ensure animation and state transitions are smooth and predictable.

### CHK-13: Button-like checkbox parity
- Standardize selected/unselected pressed states in `ButtonCheckBoxPainter`.
- Preserve accessibility cues and keyboard activation visuals.

### CHK-14: Theme family polish pass
- Do a per-style painter polish pass for Material3/Fluent2/Minimal/Modern/iOS/Classic/Switch/Button.
- Keep brand identity while preserving shared behavior semantics.

---

## File-by-File Implementation Map

- `CheckBoxes/BeepCheckBox.cs`
  - Primary state model, properties, theme apply integration, keyboard/focus behavior, size/layout orchestration.
  - Add `unchecked/checked/indeterminate` state mapping properties with legacy compatibility.
- `CheckBoxes/BeepCheckBox.Events.cs`
  - Mouse/keyboard state transitions, submit/toggle event sequencing.
- `CheckBoxes/BeepCheckBox.Drawing.cs`
  - Paint pipeline orchestration, targeted invalidate integration.
- `CheckBoxes/BeepCheckBox.Methods.cs`
  - Layout helpers, autosize and bounds calculations.
  - Add mapping synchronization helpers and legacy fallback logic.
- `CheckBoxes/Helpers/CheckBoxFontHelpers.cs`
  - Migrate/align typography sourcing with `BeepThemesManager.ToFont(...)`.
- `CheckBoxes/Helpers/CheckBoxIconHelpers.cs`
  - Centralize icon resolution and StyledImagePainter rendering contract via `IconsManagement`.
- `CheckBoxes/Helpers/CheckBoxThemeHelpers.cs`
  - Ensure state color mapping is consistent and contrast-aware.
- `CheckBoxes/Helpers/CheckBoxStyleHelpers.cs`
  - Standardize style token extraction and fallback.
- `CheckBoxes/Painters/ICheckBoxPainter.cs`
  - Confirm painter contract supports full state semantics and optional focus visuals.
- `CheckBoxes/Painters/CheckBoxPainterBase.cs`
  - Shared helper behavior only (no style-specific duplication).
- `CheckBoxes/Painters/CheckBoxPainterFactory.cs`
  - Validate style mapping completeness and fallback safety.
- `CheckBoxes/Painters/Material3CheckBoxPainter.cs`
  - Baseline modern behavior and state transitions.
- `CheckBoxes/Painters/Fluent2CheckBoxPainter.cs`
  - Fluent semantics and focus ring parity.
- `CheckBoxes/Painters/MinimalCheckBoxPainter.cs`
  - Minimal state contrast clarity.
- `CheckBoxes/Painters/ModernCheckBoxPainter.cs`
  - Modern visual polish and spacing.
- `CheckBoxes/Painters/iOSCheckBoxPainter.cs`
  - iOS-inspired motion/shape semantics with parity.
- `CheckBoxes/Painters/ClassicCheckBoxPainter.cs`
  - Legacy visual style with modern accessibility behavior.
- `CheckBoxes/Painters/SwitchCheckBoxPainter.cs`
  - Toggle-switch semantics and animation consistency.
- `CheckBoxes/Painters/ButtonCheckBoxPainter.cs`
  - Button-like selected states and action clarity.
- `CheckBoxes/Models/CheckBoxStyleConfig.cs`
  - Shared style metrics/tokens.
- `CheckBoxes/Models/CheckBoxColorConfig.cs`
  - Shared state color configuration and fallback structure.

---

## Acceptance Criteria

- Checkbox, switch, and button-like variants all present clear state transitions.
- Focus ring is visible and consistent across styles.
- Fonts are sourced through `BeepThemesManager.ToFont(...)` in runtime rendering.
- Icons resolve through `IconsManagement` and render through `StyledImagePainter`.
- Label alignment remains stable under DPI scaling and different text lengths.
- Disabled states remain readable and clearly non-interactive.
- Reduced unnecessary full-control repaints during hover/focus toggles.
- New `unchecked/checked/indeterminate` mapping properties preserve legacy `UncheckedValue` / `CheckedValue` behavior without regressions.

---

## Validation Checklist

- Visual checks
  - All checkbox styles under light/dark/high-contrast themes.
  - Checked/unchecked/indeterminate with and without label.
  - Left/right text alignment and long label truncation behavior.

- Interaction checks
  - Mouse click/toggle and hover transitions.
  - Keyboard tab focus + space toggle.
  - Focus ring visibility after keyboard navigation.
  - Legacy forms using `CheckedValue` / `UncheckedValue` behave unchanged.
  - New state mapping properties correctly drive `CurrentValue` for all three states.

- Accessibility checks
  - Disabled state readability.
  - Contrast validation for glyph and label in all states.
  - Practical hit target size checks.

- Performance checks
  - Smooth hover/focus transitions without visible flicker.
  - No redundant repaint spikes while rapidly toggling state.

---

## Suggested Execution Order

1. CHK-01, CHK-02, CHK-03, CHK-04  
2. CHK-05, CHK-06, CHK-07, CHK-08  
3. CHK-09, CHK-10, CHK-11  
4. CHK-12, CHK-13, CHK-14

