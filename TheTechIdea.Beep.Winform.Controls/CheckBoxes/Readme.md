# BeepCheckBox

## Overview

`BeepCheckBox<T>` is the checkbox family under `TheTechIdea.Beep.Winform.Controls.CheckBoxes`.

The control already provides:

- `BaseControl` integration
- explicit checkbox contract properties: `Checked`, `CheckState`, `ThreeState`, and `ReadOnly`
- explicit checkbox events: `CheckedChanged` and `CheckStateChanged`
- generic value mapping with wrapper types for bool, char, and string usage
- multiple painter-driven visual styles
- helper-based theme, font, icon, and style resolution
- grid-oriented drawing support
- design-time support and smart-tag integration

## Current Contract Highlights

- `Checked` exposes the mainstream boolean checkbox semantic.
- `CheckState` exposes the full `Unchecked/Checked/Indeterminate` state.
- `ThreeState` defaults to `true` to preserve existing runtime behavior, but it can now be disabled for classic two-state toggling.
- `AutoCheck` defaults to `true`, and can be disabled when callers need click/focus behavior without automatic state mutation.
- `MouseHitMode` defaults to `WholeControl`, but callers can switch to `CheckBoxGlyph` when only clicks and hover affordance inside the painted checkbox area should toggle state.
- `ReadOnly` now blocks keyboard and mouse toggling while keeping the control enabled.
- Generic mapping through `CurrentValue`, `CheckedValue`, `UncheckedValue`, and explicit state values remains available on top of the classic checkbox surface.
- `BoundProperty` now defaults to `CurrentValue`, which aligns the checkbox with the repo's integrated field-binding infrastructure.
- Consumers that need visual-state semantics can still opt into `State`, `Checked`, or `CheckState` explicitly by setting `BoundProperty`.
- `MinimumAutoSizeWidth` makes any extra width floor explicit; the control no longer hides a hardcoded 100px minimum inside `GetPreferredSize()`.
- The default `Material3` instance and later `CheckBoxStyle` changes also apply the mapped `ControlStyle` while `SyncControlStyleWithCheckBoxStyle` remains enabled.
- `SyncControlStyleWithCheckBoxStyle` defaults to `true`, but callers can disable it when they want to preserve a custom `ControlStyle` while changing only the checkbox painter/style family.
- Direct `ControlStyle` changes now act as one-way overrides: they preserve the current `CheckBoxStyle` and turn off automatic control-style sync until `SyncControlStyleWithCheckBoxStyle` is re-enabled.
- The default `Material3` instance and later `CheckBoxStyle` changes reapply recommended `CheckBoxSize`, `Spacing`, `MinimumHitTargetSize`, and `MinimumAutoSizeWidth` metrics while `SyncLayoutMetricsWithStyle` remains enabled.
- `SyncLayoutMetricsWithStyle` defaults to `true`, but callers can disable it when they want to preserve custom layout metrics while changing only the visual checkbox style.
- Direct `CheckBoxSize`, `Spacing`, `MinimumHitTargetSize`, and `MinimumAutoSizeWidth` edits only turn layout sync off when they diverge from the current style's recommended values, which keeps preset and "recommended size" workflows stable.
- Style preset actions now also assign recommended `MinimumHitTargetSize` and `MinimumAutoSizeWidth` values for touch-oriented or horizontal variants such as `iOS`, `Switch`, and `Button`.
- Smart-tag designer actions now expose key behavior and sizing properties, including `ControlStyle`, `Checked`, `CheckState`, `ThreeState`, `AutoCheck`, `MouseHitMode`, `ReadOnly`, `BoundProperty`, `MinimumHitTargetSize`, `MinimumAutoSizeWidth`, `SyncLayoutMetricsWithStyle`, and `SyncControlStyleWithCheckBoxStyle`.
- Theme/font ownership is now explicit for design-time safety: theme-created fonts are tracked as internally owned and disposed safely, while externally assigned `TextFont` instances are never disposed by the control.
- `ApplyTheme()` now respects `UseThemeFont` so property-grid assigned fonts are preserved unless theme-font mode is intentionally enabled.
- `ApplyTheme()` also guards the `_beepImage` path against null during host/designer initialization churn.
- Lightweight debug diagnostics are available through `BeepCheckBoxDiagnostics` (`InvalidationCount`, `ToggleCount`, `CacheRebuildCount`, `PainterFetchCount`) to support regression checks without release-build overhead.

## Main Source Files

- `BeepCheckBox.cs`
- `BeepCheckBox.Drawing.cs`
- `BeepCheckBox.Events.cs`
- `BeepCheckBox.Methods.cs`
- `BeepCheckBox.IBeepComponent.cs`

Supporting architecture lives under:

- `Helpers/`
- `Models/`
- `Painters/`

## Productization Status

Implementation enhancements already completed are summarized in:

- `CHECKBOX_ENHANCEMENT_SUMMARY.md`

Commercialization planning is tracked in:

- `.plans/00-overview-gap-matrix.md`
- `.plans/08-phase1-current-api-audit.md`
- `.plans/todo-tracker.md`
- `.plans/github-project-playbook.md`

The plan is now grounded in source-backed benchmark findings from mature WinForms libraries. That benchmark reference is documented in:

- `.plans/07-github-source-benchmark-findings.md`

## Benchmark Sources Used For Planning

- `IgnaceMaes/MaterialSkin`
- `Taiizor/ReaLTaiizor`
- `yhuse/SunnyUI`
- `Krypton-Suite/Standard-Toolkit`

These repositories were used to extract concrete product patterns for:

- checkbox state contracts
- preferred-size and auto-size behavior
- DPI-aware layout
- switch-as-checkbox semantics
- focus and accessibility behavior
- designer ergonomics
- grouped and grid-hosted checkbox scenarios

## Current Planning Priorities

1. Finish Phase 1 state/value harmonization after adding the classic checkbox contract surface.
2. Move layout and rendering toward a shared metrics contract.
3. Harden accessibility, focus, and read-only behavior.
4. Make designer, binding, grid, and sample workflows predictable.
5. Capture performance, diagnostics, QA, and release evidence.

## Recommended Entry Points

- Start with `BeepCheckBox.cs` for control contract and public surface.
- Use `BeepCheckBox.Drawing.cs` and `Painters/` for rendering behavior.
- Use `.plans/07-github-source-benchmark-findings.md` before changing contract, layout, input, or designer behavior.