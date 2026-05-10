# GitHub Source Benchmark Findings - BeepCheckBox

## Goal

Ground the `CheckBoxes` commercialization plan in real WinForms source code from mature UI libraries, not only internal architecture preferences.

This note summarizes the specific source patterns observed in external repositories and translates them into concrete implications for `BeepCheckBox<T>`.

## Benchmarked Repositories

### 1. MaterialSkin

Repository: `IgnaceMaes/MaterialSkin`

Relevant source surfaces reviewed:

- `MaterialSkin/Controls/MaterialCheckbox.cs`
- `MaterialSkin/MaterialSkinManager.cs`

Observed product patterns:

- Checkbox interaction is split into separate animation concerns rather than one blended state machine. The control maintains one animation for checked-state transition and another for ripple feedback.
- Layout is recalculated into cached geometry (`_boxOffset`, `_boxRectangle`) on size changes, and paint logic consumes that geometry instead of recomputing every rectangle ad hoc.
- Preferred sizing is explicit and tied to text measurement plus style-specific vertical affordance. Auto-size behavior is not left to default WinForms assumptions.
- Theme lookup for checkbox colors is centralized in the skin manager. The checkbox asks the theme system for unchecked and disabled colors instead of hardcoding its own palette rules.
- Pointer affordance is scoped to the interactive region. Cursor changes only when the pointer is inside the actual check area.

Why this matters for BeepCheckBox:

- `BeepCheckBox` should separate state-transition animation, hover/focus feedback, and optional ripple behavior instead of letting painters or event handlers blend them implicitly.
- Rectangle calculation should become a reusable layout contract that is cached and reused across paint, hit test, and grid mode.
- Auto-size and preferred-size behavior need to be part of the public product contract, not a side effect of painter math.
- Theme colors should continue to come from helpers and `IBeepTheme`, but the contract should specify which states are token-driven and which can be style overrides.
- Interactive hit regions should be explicit so switch and button styles do not accidentally widen or narrow pointer semantics unpredictably.

### 2. ReaLTaiizor

Repository: `Taiizor/ReaLTaiizor`

Relevant source surfaces reviewed:

- `src/ReaLTaiizor/Controls/CheckBox/MaterialCheckBox.cs`
- `src/ReaLTaiizor/Controls/Switch/MaterialSwitch.cs`
- `src/ReaLTaiizor/Controls/CheckBox/PoisonCheckBox.cs`

Observed product patterns:

- DPI scale factors are treated as first-class rendering inputs. The control caches scale values, recomputes geometry on size changes, and measures text through scale-aware helpers.
- `ReadOnly` is implemented as an interaction policy, not a visual-only flag. Input handlers preserve the previous check state when read-only mode is active.
- Hover, ripple, and checked animations are modeled separately. This keeps focus, hover, and toggle feedback composable across checkbox and switch variants.
- The switch implementation remains a checkbox-derived control. It changes appearance without changing semantic identity.
- The Poison family uses a three-stage paint pipeline: background, custom paint, foreground. That creates extensibility without forcing every consumer to replace the whole control.
- Focus cues are treated as a deliverable surface. The library has explicit focus rectangle behavior and keyboard-state awareness instead of assuming default WinForms visuals are enough.

Why this matters for BeepCheckBox:

- DPI and auto-size behavior should be validated per style, especially for `Switch` and `Button` styles where geometry diverges the most.
- `ReadOnly` needs an explicit contract for keyboard, mouse, and data-binding scenarios. It should prevent state mutation deterministically.
- Switch and button variants should remain semantically checkbox controls in accessibility, events, and binding.
- A layered paint contract would make `BeepCheckBox` easier to extend for diagnostics, overlays, and host-specific styling without copying painter logic.
- Focus visuals cannot be treated as polish. They need a stable product contract and validation matrix.

### 3. SunnyUI

Repository: `yhuse/SunnyUI`

Relevant source surfaces reviewed:

- `SunnyUI/Controls/UICheckBox.cs`
- `SunnyUI/Controls/UICheckBoxGroup.cs`
- `SunnyUI/Controls/UIControl.cs`

Observed product patterns:

- The checkbox is built on a reusable base paint pipeline. `UIControl` separates fill, border, and foreground responsibilities and exposes style token application through `SetStyleColor`.
- Control sizing is productized with explicit properties such as `CheckBoxSize` and `ImageInterval`.
- The checkbox exposes both `ValueChanged` and `CheckedChanged`, which makes domain-value consumers and classic WinForms consumers equally well served.
- `ReadOnly` is enforced in the click path, not only hinted at visually.
- Auto-size behavior is continuously maintained when text changes.
- The library treats checkbox groups as a first-class adjacent product. Group layout caches rectangles, supports columns, and keeps hover state explicit.

Why this matters for BeepCheckBox:

- `BeepCheckBox` should keep painter specialization, but the core paint contract should mirror the same separation of fill, border, glyph, and text responsibilities.
- Size and spacing properties should be exposed as intentional product knobs, with style defaults resolved from helpers rather than hidden magic numbers.
- Event strategy should distinguish state change from mapped domain-value change when that distinction is meaningful.
- Grouped and grid-hosted checkbox scenarios should be treated as related surfaces during commercialization, because that is where layout drift and hit-testing regressions usually emerge.

### 4. Krypton Toolkit

Repository: `Krypton-Suite/Standard-Toolkit`

Relevant source surfaces reviewed:

- `Source/Krypton Components/Krypton.Toolkit/Controls Toolkit/KryptonCheckBox.cs`
- `Source/Krypton Components/Krypton.Toolkit/View Draw/ViewDrawCheckBox.cs`
- `Source/Krypton Components/Krypton.Toolkit/Designers/Action Lists/KryptonCheckBoxActionList.cs`

Observed product patterns:

- The checkbox is treated as a layout composition problem, not only a paint problem. Dedicated view elements own glyph layout, content layout, and preferred-size calculations.
- `Checked`, `CheckState`, `ThreeState`, and `AutoCheck` are separate contracts with explicit event flow.
- Focus overrides are part of the palette system, which keeps focus visuals aligned with the rest of the control theme.
- Orientation and RTL behavior are explicit layout concerns, not late paint-time conditionals.
- Designer action lists are part of the product surface, not an afterthought.
- Command binding is considered part of state propagation, which keeps external command/state models synchronized.

Why this matters for BeepCheckBox:

- `BeepCheckBox` should formalize the relationship between `Checked`, `State`, and mapped values before more styles are added.
- RTL and orientation behavior should live in the layout contract, not only in painter branches.
- Designer ergonomics should be handled as a first-class commercialization stream.
- Grid and host rendering should measure preferred glyph size from shared metrics rather than duplicating style geometry.

## Cross-Repository Patterns That Recur

The same ideas appear in different implementations:

1. Layout is resolved before painting.
2. Preferred size is explicit and style-aware.
3. `ReadOnly` and `ThreeState` are product contracts, not incidental flags.
4. Checkbox and switch variants preserve the same semantic identity.
5. Theme/color lookup is centralized.
6. Focus and accessibility behavior are handled deliberately.
7. Designer experience is part of the control, not external documentation.
8. Grouped and grid-hosted checkbox usage is treated as a real product surface.

## Direct Implications For The BeepCheckBox Plan

### Plan corrections

- The plan should cite actual benchmark repositories and the exact behaviors learned from them.
- Phase tasks should explicitly pull in the external patterns rather than leaving benchmarking as background context.
- Grid mode, switch mode, and button mode should remain inside the same semantic contract, following the approach used by ReaLTaiizor and Krypton.
- Preferred-size, DPI, and layout metrics need to move from implementation detail to acceptance criteria.

### Recommended adoption priorities

#### P1 - Contract first

- Separate visual state, semantic state, and mapped domain value.
- Define `ReadOnly`, `ThreeState`, and legacy value mapping as explicit product contracts.
- Document event ordering for checked-state change versus mapped-value change.

#### P2 - Layout before painter growth

- Introduce a shared metrics or render-context object for glyph bounds, text bounds, spacing, minimum hit target, and RTL orientation.
- Make all painters consumers of resolved geometry.
- Align grid mode to shared metrics wherever possible.

#### P3 - Accessibility and focus as a release gate

- Keep switch and button styles semantically checkable.
- Define focus-ring, high-contrast, and narration behavior explicitly.
- Validate pointer hit area separate from painted ornament.

#### P4 - Designer and host surfaces

- Treat designer smart tags, property metadata, serialization, and sample hosts as part of the product contract.
- Validate grid-hosted and grouped scenarios with the same state and layout rules.

## Anti-Patterns To Avoid

- Adding new styles before the layout and state contracts are stabilized.
- Leaving auto-size behavior to painter-specific geometry.
- Treating switch style as a separate semantic control.
- Letting grid mode diverge silently from the main rendering path.
- Relying on visual disabled/read-only cues without enforcing interaction policy.
- Handling focus and accessibility only after visual work is complete.

## How To Use This Document

- Use this file as the benchmark source of truth for the commercialization plan.
- Update phase docs when a benchmark finding becomes a committed implementation rule.
- Add new repositories only when they contribute a concrete source-backed pattern that changes plan scope or acceptance criteria.