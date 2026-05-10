# Phase 1 Current API Audit - BeepCheckBox

## Purpose

This document records the current `BeepCheckBox<T>` contract as implemented in source today.

It is not a target-state spec. It is a source audit used to drive Phase 1 decisions so the final contract is based on actual behavior rather than assumptions.

Primary source files reviewed:

- `CheckBoxes/BeepCheckBox.cs`
- `CheckBoxes/BeepCheckBox.Events.cs`
- `CheckBoxes/BeepCheckBox.Methods.cs`
- `CheckBoxes/BeepCheckBox.Drawing.cs`
- `CheckBoxes/BeepCheckBox.IBeepComponent.cs`

## Current Public Contract As Implemented

### Wrapper defaults

- `BeepCheckBoxBool`
  - `CheckedValue = true`
  - `UncheckedValue = false`
  - `CurrentValue = false`
- `BeepCheckBoxChar`
  - `CheckedValue = 'Y'`
  - `UncheckedValue = 'N'`
  - `CurrentValue = 'N'`
- `BeepCheckBoxString`
  - `CheckedValue = "YES"`
  - `UncheckedValue = "NO"`
  - `CurrentValue = "NO"`

These wrappers imply mainstream two-state product intent, but the generic control does not currently expose a two-state-only interaction mode.

### Core state surface

- Primary state property: `State : CheckBoxState`
- Classic checkbox aliases:
  - `Checked : bool`
  - `CheckState : CheckBoxState`
  - `ThreeState : bool = true`
  - `AutoCheck : bool = true`
  - `ReadOnly : bool`
- Primary mapped-value property: `CurrentValue : T`
- Legacy value pair:
  - `CheckedValue : T`
  - `UncheckedValue : T`
- Explicit state-value pair:
  - `CheckedStateValue : T`
  - `UncheckedStateValue : T`
  - `IndeterminateStateValue : T`
- Compatibility controls:
  - `MapStateValuesToLegacy : bool = true`
  - `UseUncheckedValueForIndeterminate : bool = true`
- Default binding surface:
  - `BoundProperty = "CurrentValue"`
- Events:
  - `CheckedChanged`
  - `CheckStateChanged`
  - `StateChanged`

### Current state-resolution rules

- `State` is the authoritative visual state when the control is toggled directly.
- `CurrentValue` is derived from `State` through `ResolveValueForState(...)`.
- If explicit state values are not set:
  - checked resolves to `CheckedValue`
  - unchecked resolves to `UncheckedValue`
  - indeterminate resolves to `UncheckedValue` by default, or to `CheckedValue` when `UseUncheckedValueForIndeterminate = false`
- `CurrentValue` can also drive `State` through `UpdateStateFromValue()`.

## Important Behavioral Findings

### 1. Tri-state is now explicit, but still the default runtime behavior

`ToggleState()` now respects `ThreeState`.

Default behavior remains:

- `Unchecked -> Checked`
- `Checked -> Indeterminate`
- `Indeterminate -> Unchecked`

When `ThreeState = false`, interaction becomes two-state:

- `Unchecked <-> Checked`

Implication:

- the runtime interaction model is still tri-state by default
- wrapper types with bool, char, and string defaults still inherit mandatory tri-state toggling behavior
- indeterminate is no longer unavoidable, but it is still the default interaction mode unless consumers opt out

### 2. A classic checkbox contract now exists alongside the generic mapper

The control now exposes:

- `Checked`
- `CheckState`
- `ThreeState`
- `ReadOnly`
- `CheckedChanged`
- `CheckStateChanged`

The existing generic mapping surface remains in place:

- `State`
- `CurrentValue`
- value-mapping properties

Implication:

- consumers can now use a standard checkbox-style API without giving up generic mapping
- the public surface is moving closer to the benchmark libraries, even though the underlying state/value mapper still exists

### 3. `State` and `CurrentValue` are now closer, but `SetValue(...)` remains intentionally quiet

`State` setter behavior:

- routes through `SetStateCore(...)`
- updates `_state`
- synchronizes `_currentValue`
- raises `CheckStateChanged`
- raises `CheckedChanged` when the boolean checked semantic changed
- raises `StateChanged`
- invalidates visual region
- calls `RaiseSubmitChanges()`

`Checked` and `CheckState` both flow through the same path.

`CurrentValue` setter behavior:

- routes through `SetCurrentValueCore(...)`
- writes `_currentValue`
- resolves `_state` from value-matching rules
- when the resolved state changes, it raises the same checkbox state event sequence used by `State`
- when the resolved state does not change, it refreshes visuals and can still call `RaiseSubmitChanges()`

`SetValue(object value)` behavior:

- temporarily suppresses outward checkbox notifications
- sets `CurrentValue`
- still synchronizes `_state` and visuals
- does not raise `CheckStateChanged`
- does not raise `CheckedChanged`
- does not raise `StateChanged`
- does not call `RaiseSubmitChanges()`

Implication:

- setting `State`, `Checked`, or `CheckState` produces events and submit behavior
- setting `CurrentValue` directly now produces the same outward checkbox contract when it changes state
- the remaining asymmetry is intentional: `SetValue(...)` is now treated as a quiet inbound update path

### 4. Changing mapping properties can also change runtime value

Setting any of these properties calls `UpdateCurrentValue()`:

- `CheckedValue`
- `UncheckedValue`
- `CheckedStateValue`
- `UncheckedStateValue`
- `IndeterminateStateValue`
- `MapStateValuesToLegacy`
- `UseUncheckedValueForIndeterminate`

Implication:

- configuration changes can mutate `CurrentValue` immediately based on the existing `State`
- the control does not clearly separate contract configuration from live value mutation

### 5. `MapStateValuesToLegacy` is a live synchronization switch

When `MapStateValuesToLegacy = true` and explicit state values exist:

- `CheckedStateValue` can overwrite `_checkedValue`
- `UncheckedStateValue` can overwrite `_uncheckedValue`

Implication:

- legacy and explicit state-value surfaces are not independent contracts
- they are partially synchronized compatibility layers
- the current API does not make clear which surface owns the real product contract

### 6. Indeterminate fallback is compatibility-oriented, not semantically explicit

If `IndeterminateStateValue` is not set, indeterminate maps to one of the two legacy values.

Default behavior:

- indeterminate maps to unchecked

Implication:

- indeterminate can be visible in UI while sharing the same domain value as unchecked
- that may be intentional compatibility behavior, but it is not currently documented as such

### 7. Binding/filter behavior now defaults to mapped value and still honors `BoundProperty`

`BoundProperty` now defaults to `"CurrentValue"`.

`ToFilter()` emits:

- `FieldName = BoundProperty`
- `FilterValue = value resolved from BoundProperty`

Current practical behavior:

- when `BoundProperty = "CurrentValue"`, filtering emits the mapped domain value by default
- when `BoundProperty = "State"`, filtering remains state-oriented
- when `BoundProperty = "Checked"`, the filter now emits the boolean checked value
- when `BoundProperty = "CheckState"`, the filter now emits the explicit checkbox state

Implication:

- the default filtering and submit contract now follows the mapped checkbox value rather than the visual state
- this aligns with the repo's integrated control registry, which already treats `BeepCheckBoxBool.CurrentValue` as the default editor binding surface
- consumers can still opt into state-oriented behavior by choosing `State`, `Checked`, or `CheckState` explicitly

### 8. Auto-size width floor is now explicit

`GetPreferredSize()` returns:

- width at least `max(calculatedWidth, MinimumHitTargetSize, MinimumAutoSizeWidth)`
- height at least `max(calculatedHeight, MinimumHitTargetSize)`

Implication:

- there is no longer a hidden hardcoded 100px width floor
- callers can opt into an extra width floor explicitly through `MinimumAutoSizeWidth`
- the default `Material3` baseline and later `CheckBoxStyle` changes now also apply the mapped `ControlStyle` while `SyncControlStyleWithCheckBoxStyle = true`
- callers can disable `SyncControlStyleWithCheckBoxStyle` when style changes should preserve a custom `ControlStyle`
- direct `ControlStyle` changes now act as one-way overrides instead of trying to infer a reverse `CheckBoxStyle` from a lossy mapping
- the default `Material3` baseline and later `CheckBoxStyle` changes now reapply recommended size, spacing, hit-target, and width-floor metrics while `SyncLayoutMetricsWithStyle = true`
- callers can disable `SyncLayoutMetricsWithStyle` when style changes should preserve custom size, spacing, and width-floor overrides
- direct `CheckBoxSize`, `Spacing`, `MinimumHitTargetSize`, and `MinimumAutoSizeWidth` edits now only disable layout sync when they diverge from the active style's recommended values, so preset workflows that assign recommended metrics do not accidentally disable sync
- style preset workflows can now assign explicit `MinimumHitTargetSize` and `MinimumAutoSizeWidth` recommendations instead of leaving touch-target and width-floor policy fully manual
- compact text-hidden and short-label checkboxes can now autosize to their actual content plus hit-target requirements

### 9. Interaction semantics are now configurable

`OnMouseClick()` toggles when the left mouse button is used on the control while it is enabled, not read-only, and `AutoCheck = true`.

The toggle region is now controlled by `MouseHitMode`:

- `WholeControl` preserves the classic full-row checkbox behavior
- `CheckBoxGlyph` limits mouse toggling to `_lastCheckBoxRect`, the painted checkbox glyph area

`OnKeyDown()` now also blocks `Space`/`Enter` toggling when `ReadOnly = true`, and respects `AutoCheck = false`.

Implication:

- current pointer semantics can now match either classic full-row checkbox behavior or glyph-only hit behavior without changing keyboard semantics
- `MinimumHitTargetSize` still affects preferred size regardless of whether mouse toggling uses the whole control or only the glyph
- hover affordance now follows the active `MouseHitMode`, so glyph-only mode no longer shows a hand cursor over non-toggle regions
- `ReadOnly` is now enforced in user interaction rather than being only an inherited infrastructure concept
- consumers can suppress automatic state mutation without disabling focus or click participation

## Public Surface Missing Compared To Mature Checkbox Contracts

No remaining missing items were identified in the classic checkbox contract set covered by this audit slice.

The remaining work is now about policy and workflow refinement rather than missing first-class checkbox API members.

## Benchmark Comparison Summary

### MaterialSkin

- Exposes a classic checkbox semantic even though rendering is custom
- Keeps preferred size explicit
- Does not make tri-state the unavoidable default interaction path

### ReaLTaiizor

- Exposes checkbox and switch variants with explicit interaction properties
- Enforces `ReadOnly` in input handling
- Separates animation responsibilities instead of mixing them into state mapping

### SunnyUI

- Exposes explicit `Checked` semantics plus `ValueChanged` and `CheckedChanged`
- Enforces `ReadOnly` in click behavior
- Treats sizing properties as productized inputs

### Krypton Toolkit

- Separates `Checked`, `CheckState`, `ThreeState`, and `AutoCheck`
- Uses explicit event flow for `CheckedChanged` and `CheckStateChanged`
- Treats designer and layout behavior as part of the contract

## Phase 1 Decisions Now Required

### Decision 1

Should `BeepCheckBox` become a classic checkbox contract with generic value mapping on top, or remain primarily a generic state/value mapper?

### Decision 2

Should tri-state be:

- always on
- optional via `ThreeState`
- enabled only when indeterminate mapping is explicitly configured

### Decision 3

Should `SetValue(...)` remain a quiet inbound update path, or should all inbound value changes also participate in the public checkbox event contract?

### Decision 4

Should legacy value properties remain public, or should explicit state-value properties become the primary API with legacy behavior treated as compatibility mode?

### Decision 5

Should mapped-value default binding remain the product default, or should the control grow a higher-level policy switch for choosing between value and state semantics?

### Decision 6

Should `MinimumAutoSizeWidth` remain an opt-in layout refinement, or should style presets/designer workflows assign explicit defaults for specific checkbox variants?

## Recommended Phase 1 Outputs

Use this audit to produce:

- a stable state/value contract
- an event sequencing contract
- wrapper positioning guidance
- migration notes for legacy value mapping
- explicit decisions on tri-state, read-only, and binding/filter semantics
- design-time guidance for how smart tags and property-grid workflows should present behavior, binding, and sizing choices

## Short Version

The current control now exposes a mainstream checkbox contract on top of its existing generic value-mapping model.

What it does not yet provide is an explicit checkbox contract explaining:

- whether two-state or three-state behavior should be the default product posture
- whether `State` or `CurrentValue` is the real public source of truth
- how quiet inbound updates through `SetValue(...)` should relate to the public event contract
- how wrappers and designers should present the choice between mapped-value and explicit state binding

That ambiguity is the core Phase 1 problem.