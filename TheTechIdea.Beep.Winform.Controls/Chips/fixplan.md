# BeepMultiChipGroup Refactor Plan (Painters + Helpers + Partials)

Goal: align BeepMultiChipGroup with the same architecture used across controls (BaseControl + Painters + Helpers). Keep outer skinning in BaseControl painters; move inner chip rendering to a dedicated painter strategy; split the control into partial classes for maintainability.

## Objectives
- Introduce a painter strategy for chips: IChipGroupPainter.
- Use helpers for layout, hit-testing, and color/theme mapping.
- Keep BeepMultiChipGroup focused on state, events, data binding; delegate drawing to painter.
- Support common UX: selectable chips, closable chips (with X), selected indicator (check), hover/pressed states.
- Future-ready: add multiple painter styles (Filled/Pill, Outlined, Text, Compact).

## Phases

1) Scaffolding (this PR)
- Add interfaces and simple option types under `Chips/Helpers`:
  - `IChipGroupPainter`
  - `ChipRenderOptions`
- Add a default painter under `Chips/Painters/DefaultChipGroupPainter.cs` implementing the current visuals (pill-like) + modern UX.
- Wire BeepMultiChipGroup to use the painter for:
  - Measurement: `MeasureChip`
  - Drawing: `RenderChip`
  - Optional: `RenderGroup` (draw title area)
  - Optional: `GetCloseRect` to place hit-area for close button.
- Add properties to BeepMultiChipGroup for painter selection and UX:
  - `ChipPainterKind` enum with Default (Pill), Outlined, Text (future painters can be plugged later)
  - `ShowCloseOnSelected` (default true)
  - `ShowSelectionCheck` (default true)

2) Extract partials (follow-up)
- Split BeepMultiChipGroup into partials:
  - `BeepMultiChipGroup.Properties.cs` (props/events)
  - `BeepMultiChipGroup.Layout.cs` (layout + measurement)
  - `BeepMultiChipGroup.Drawing.cs` (DrawContent delegations)
  - `BeepMultiChipGroup.HitTest.cs` (hit-areas + mouse)
- Move color/theme helpers to `Chips/Helpers/ChipThemeHelper.cs`.
- Move layout helpers to `Chips/Helpers/ChipLayoutHelper.cs`.

3) Additional painters (optional next iterations)
- `OutlinedChipGroupPainter`
- `TextChipGroupPainter`
- `CompactChipGroupPainter` (smaller height, dense layout)
- Each painter to respect theme + options and only draw inner chips; BaseControl painters handle outer frame/shadow.

4) UX polish
- Keyboard navigation between chips, Delete to clear selected, Esc to clear hover, Ctrl/Cmd multi-select when SelectionMode=Multiple.
- Accessibility: focus ring on chip when navigated with keyboard.

## Acceptance
- Chips are measured and drawn exclusively by the painter.
- Close (X) area is a separate hit-area; clicking it deselects/removes selection for that chip.
- The control uses `DrawingRect` from BaseControl for layout.
- No regressions in selection modes.

## Notes
- Keep all outer border/shadow/background in BaseControl painters (Classic/Material/etc.).
- Painters must not draw outer container; only chips/title inside `DrawingRect`.
