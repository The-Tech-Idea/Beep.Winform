# Phase 4 - Binding, Grid Mode, And Designer Workflows

Priority: High  
Status: Not Started  
Depends on: Phase 1 and Phase 2

## Objective

Make the control predictable in real host environments: data-bound forms, grid cells, designer usage, wrapper-based authoring, and property-grid editing.

## Problem Statement

Commercial controls are not judged only by how they paint. They are judged by how cleanly they serialize, how safely they behave in designers, how easy they are to bind, and how consistently they behave in data grids and editors. The current `CheckBoxes` surface already has wrapper types and grid mode, which means this phase should formalize and harden those workflows.

## Scope

- generic binding and value mapping in real forms
- wrapper-type behavior in design-time and runtime scenarios
- serialization and reset behavior for visual and value properties
- grid mode parity and hosting expectations
- designer smart tags, presets, and discoverability
- sample host scenarios for common use cases

## Benchmark Anchors

- Krypton treats designer action lists, property metadata, and command/state synchronization as part of the control product.
- SunnyUI treats grouped checkbox scenarios as a first-class host surface rather than a later extension.
- ReaLTaiizor exposes both checkbox and switch variants through the same semantic family, which is a useful guardrail for Beep wrappers and presets.

## Deliverables

- `BCHK-P4-001` Define supported binding patterns for `BeepCheckBox<T>` and wrapper types, including tri-state expectations.
- `BCHK-P4-002` Audit serialization visibility and default-value behavior so property-grid changes persist predictably.
- `BCHK-P4-003` Harden grid-mode rules and document which features are fully supported, simplified, or intentionally excluded when hosted in grid/editor scenarios.
- `BCHK-P4-004` Review and strengthen design-time actions, presets, and property grouping so common authoring flows are easy to discover and benchmarked against mature toolkit ergonomics.
- `BCHK-P4-005` Add sample scenarios for bool, char, string, indeterminate, switch style, button style, and grid-hosted usage.
- `BCHK-P4-006` Produce a design-time safety checklist covering initialization, DPI, theme switching, and serializer round-trips.

## Recommended Work Breakdown

1. Review wrapper types and property attributes from a designer-author perspective.
2. Validate binding behavior against real data models, not just direct property toggling.
3. Decide whether grid mode is a first-class host surface or a simplified compatibility path.
4. Improve smart-tag and preset discoverability so style selection does not require deep property-grid knowledge.
5. Add focused sample forms that can serve as both docs and manual QA hosts.
6. Validate grouped, grid-hosted, and wrapper-driven scenarios against the same state/layout contract used by the main control surface.

## File Focus

- `CheckBoxes/BeepCheckBox.cs`
- `CheckBoxes/BeepCheckBox.IBeepComponent.cs`
- `CheckBoxes/BeepCheckBox.Drawing.cs`
- designer registration and designer action-list files
- sample hosts in `Beep.Winform.Sample` or equivalent validation surface

## Acceptance Criteria

- Bound usage is documented and validated for supported wrappers.
- Designer serialization round-trips cleanly for core appearance and state properties.
- Grid mode has an explicit support policy.
- Smart tags and presets support the main authoring workflow.
- Grouped and grid-hosted scenarios do not rely on undocumented special-case behavior.
- Sample hosts exist for the major product scenarios.

## Out Of Scope

- deep performance instrumentation
- release packaging and public documentation set

## GitHub Project Mapping

Epic: `Phase 4 - Binding, Grid Mode, and Designer Workflows`  
Suggested labels: `area:checkbox`, `phase:4`, `type:designer`, `type:binding`, `priority:p1`

Recommended issue split:

- binding contract and examples
- serialization audit
- grid-mode parity
- designer ergonomics
- sample host coverage

## Exit Evidence

- sample host list committed
- design-time checklist attached to the epic
- tracker updated with grid and designer readiness notes