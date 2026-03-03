# BeepRadioGroup Enhancement Plan
**Reference direction:** Latest Figma selection controls + modern design systems (Material, Fluent, iOS)  
**Target area:** `TheTechIdea.Beep.Winform.Controls/RadioGroup`  
**Last updated:** 2026-02-27

---

## Goal

Upgrade `BeepRadioGroup` to deliver:
- consistent typography/icon behavior across all renderer styles,
- reliable hover/focus/pressed/disabled state parity,
- robust keyboard and accessibility behavior,
- DPI-safe layout and rendering with predictable autosize,
- targeted redraws and safe lifecycle handling.

---

## Implementation Constraints (Mandatory)

- Always resolve runtime fonts via `BeepThemesManager.ToFont(...)`.
- Use `IconsManagement` (`SvgsUI`/`Svgs`) and `StyledImagePainter` for icon drawing.
- Keep event/state mutation in control helpers; renderers remain rendering-only.
- Prefer theme tokens first; avoid hardcoded colors except style fallback.
- Keep renderer identity while harmonizing interaction semantics.

---

## Backlog

## Phase 1 - Foundation and Tokens (P0)

### RGP-01: Typography unification
Status: Completed
- Migrate `RadioGroupFontHelpers` to `BeepThemesManager.ToFont(...)`.
- Ensure `BeepRadioGroup.ApplyTheme()` applies TextFont through helper.
- Ensure renderer text uses control/theme typography consistently.

### RGP-02: Icon pipeline unification
Status: Completed
- Ensure icon path resolution supports `SvgsUI` + `Svgs` symbol names.
- Keep icon painting centralized through `RadioGroupIconHelpers`.

### RGP-03: Renderer interaction semantics
Status: Completed
- Normalize hover/focus/pressed/disabled semantics across renderers.
- Ensure keyboard focus visuals are clearly visible and theme-driven.

---

## Phase 2 - Layout, DPI and Accessibility (P0)

### RGP-04: Layout and autosize reliability
Status: Completed
- Validate layout helper sizing under style/orientation switches.
- Ensure image+text combinations avoid clipping in all renderers.

### RGP-05: Keyboard and accessibility parity
Status: Completed
- Verify arrow/home/end/space/enter behavior for all orientations.
- Keep accessibility metadata synchronized with selection state.

### RGP-06: Hit target and focus ring consistency
Status: Completed
- Ensure hit areas align exactly to rendered item bounds.
- Harmonize focus indicators and minimum target size.

---

## Phase 3 - Performance and Reliability (P1)

### RGP-07: Targeted invalidation
Status: Completed
- Replace broad invalidation paths where practical.
- Trigger redraw only for affected regions/state changes.

### RGP-08: Cache/layout hygiene
Status: Completed
- Avoid stale measurements when fonts/theme/DPI change.
- Ensure layout helper re-measures safely on renderer switches.

### RGP-09: Lifecycle hardening
Status: Completed
- Validate helper/renderer disposal and event unsubscription.
- Ensure no stale hover/focus state after item/style changes.

---

## Phase 4 - Renderer Family Polish (P2)

### RGP-10: Classic family polish
Status: Completed
- Harmonize `Material`, `Circular`, `Flat`, `Checkbox` visuals/spacing.

### RGP-11: Button family polish
Status: Completed
- Harmonize `Button`, `Toggle`, `Segmented`, `Pill`.

### RGP-12: Surface family polish
Status: Completed
- Harmonize `Card`, `Chip`, `Tile` content hierarchy and state feedback.

---

## File-by-File Focus

- `RadioGroup/BeepRadioGroup.cs`
  - Theme application, renderer/style syncing, value integration.
- `RadioGroup/BeepRadioGroup.Properties.cs`
  - Property-triggered redraw/layout policy and state transitions.
- `RadioGroup/BeepRadioGroup.Drawing.cs`
  - Layout/render orchestration and design-time behavior.
- `RadioGroup/Helpers/RadioGroupFontHelpers.cs`
  - Font migration to `BeepThemesManager.ToFont(...)`.
- `RadioGroup/Helpers/RadioGroupIconHelpers.cs`
  - Icon symbol/path resolution and painter usage consistency.
- `RadioGroup/Renderers/*.cs`
  - Interaction parity, disabled/focus behavior, visual polish.

---

## Validation Checklist

- Build passes for `TheTechIdea.Beep.Winform.Controls.csproj`.
- No new linter issues in modified RadioGroup files.
- Fonts follow theme typography in all renderers without clipping.
- Icon paths resolve correctly from explicit path and symbol names.
- Hover/focus/pressed/disabled behavior is consistent across renderer families.
