# Phase 2 - Layout, Rendering, And Painter Contract

Priority: Critical  
Status: Not Started  
Depends on: Phase 1

## Objective

Turn the current painter system into a stable rendering platform with shared layout metrics, explicit render context, and less divergence between visual styles and grid mode.

## Problem Statement

The control already has a painter factory and style-specific painters, which is a strong start. The risk is that rectangle calculation, spacing logic, style-specific measurements, and grid-mode behavior can drift because too much geometry is still produced inside the control draw path. Commercial controls reduce that risk by making layout rules explicit and reusable.

## Scope

- shared layout metrics for checkbox glyph, text, spacing, padding, and hit target
- render context or metrics object passed consistently to painters
- parity rules across Material3, Modern, Classic, Minimal, iOS, Fluent2, Switch, and Button styles
- clearer contract between full rendering mode and grid mode
- text overflow, alignment, and RTL policy
- visual regression strategy

## Benchmark Anchors

- MaterialSkin caches checkbox geometry and keeps preferred-size calculation explicit.
- ReaLTaiizor resolves scale-aware geometry before paint for both checkbox and switch variants.
- SunnyUI separates fill, border, and foreground responsibilities through a reusable base paint pipeline.
- Krypton treats glyph size and preferred size as renderer/layout concerns, not painter side effects.

## Deliverables

- `BCHK-P2-001` Introduce a shared layout helper or metrics context so checkbox and text rectangles are computed and cached outside painter-specific drawing.
- `BCHK-P2-002` Replace magic geometry and one-off spacing decisions with style tokens or resolved metrics.
- `BCHK-P2-003` Define a style capability matrix so each painter declares what it supports and what must remain consistent across all styles.
- `BCHK-P2-004` Decide which behaviors grid mode must fully match and which are intentionally simplified, especially for switch/button semantics and preferred sizing.
- `BCHK-P2-005` Add RTL, ellipsis, and long-label handling rules for all supported styles.
- `BCHK-P2-006` Produce a visual baseline matrix for every style in unchecked, checked, indeterminate, hovered, focused, and disabled states.

## Recommended Work Breakdown

1. Extract layout calculation from `BeepCheckBox.Drawing.cs` into a reusable helper or metrics object.
2. Make painters consumers of resolved bounds rather than owners of core geometry policy.
3. Decide which style differences are intentional brand differences and which are layout bugs.
4. Align grid mode to the same theme and state semantics even where the drawing remains simplified.
5. Add screenshot-driven validation or host-form validation for every style/state combination.
6. Validate auto-size, preferred-size, and DPI behavior as part of the layout contract rather than treating them as secondary polish.

## File Focus

- `CheckBoxes/BeepCheckBox.Drawing.cs`
- `CheckBoxes/Helpers/CheckBoxStyleHelpers.cs`
- `CheckBoxes/Painters/ICheckBoxPainter.cs`
- `CheckBoxes/Painters/CheckBoxPainterBase.cs`
- `CheckBoxes/Painters/*CheckBoxPainter.cs`

## Acceptance Criteria

- Layout rectangles are produced by one authoritative path.
- Painter inputs are explicit enough that style regression can be reasoned about without reading the full control.
- Grid mode differences are documented and intentionally bounded.
- RTL and long-text behavior are specified and validated.
- Preferred-size, auto-size, and DPI behavior are documented for every painter family that stays in scope.
- Every painter participates in the same state matrix.

## Out Of Scope

- binding refactors
- designer smart-tag work
- deep accessibility automation work beyond render prerequisites

## GitHub Project Mapping

Epic: `Phase 2 - Layout, Rendering, and Painter Contract`  
Suggested labels: `area:checkbox`, `phase:2`, `type:ux`, `type:rendering`, `priority:p0`

Recommended issue split:

- shared metrics/context
- painter capability matrix
- grid parity rules
- text/RTL behavior
- baseline validation assets

## Exit Evidence

- one metrics contract doc or code comment anchor owned by the phase
- side-by-side validation notes for all painter families
- tracker updated with style parity status