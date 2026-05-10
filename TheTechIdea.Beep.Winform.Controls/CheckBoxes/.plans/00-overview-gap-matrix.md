# BeepCheckBox Commercialization Plan - Overview And Gap Matrix

## Purpose

This planning set resets the `CheckBoxes` roadmap from "enhanced" to "commercially shippable".
The code already has a strong foundation: `BaseControl` inheritance, generic value support, multiple painters, helper-based theme and font integration, DPI handling, and design-time scaffolding. What it does not yet have is a phased execution plan that treats the control like a product surface with explicit contracts, quality gates, release governance, and GitHub delivery discipline.

## Benchmark Families

- Source-backed benchmark baseline:
   - `IgnaceMaes/MaterialSkin`: cached checkbox geometry, explicit preferred sizing, separate check/ripple animation concerns, centralized theme lookup
   - `Taiizor/ReaLTaiizor`: DPI-aware checkbox and switch implementations, read-only interaction enforcement, layered paint contracts, focus/ripple separation
   - `yhuse/SunnyUI`: reusable base paint pipeline, style token application, explicit size/spacing properties, grouped checkbox surface
   - `Krypton-Suite/Standard-Toolkit`: explicit `Checked`/`CheckState`/`ThreeState` contract, renderer-driven preferred size, focus overrides, designer action lists
- Product-shell surfaces: settings dialogs, property grids, data grids, and setup wizards
- Accessibility bar: WCAG 2.2 AA-aligned behavior for keyboard, focus, and high contrast

Benchmark detail doc: `07-github-source-benchmark-findings.md`

## Current Architecture Snapshot

- `BeepCheckBox<T>` already inherits from `BaseControl`
- bool, char, and string wrappers already exist for mainstream use cases
- drawing is split into partial classes and painter implementations
- helper classes centralize theme, font, icon, and style concerns
- multiple visual styles already exist: Material3, Modern, Classic, Minimal, iOS, Fluent2, Switch, Button
- the control already carries state/value mapping fields, DPI reapplication, keyboard focus visibility, and a grid-mode drawing path

## Current Diagnosis

1. The public contract is richer than the documented product contract.
   The control exposes tri-state behavior, generic value mapping, legacy mapping flags, and wrapper types, but there is no formal statement of which semantics are stable and which are compatibility behavior.

2. Layout and rendering are still too tightly coupled.
   Rectangle calculation and painter invocation happen directly inside the draw path, which makes style parity and regression testing harder than it should be.

3. Grid mode behaves like a sibling product.
   The separate grid drawing path is valuable, but it increases the risk of divergence in metrics, colors, focus behavior, and future feature support.

4. Accessibility is partially present but not yet product-grade.
   Keyboard focus and minimum hit-target intent already exist, but commercial products require a complete contract for role, state, naming, focus visuals, high-contrast behavior, and screen-reader predictability.

5. Designer and binding workflows need stronger governance.
   Generic value mapping, serialization visibility, wrapper defaults, and grid/editor hosting all need a cleaner operational model so the control behaves predictably across runtime and design time.

6. Reliability and release operations are undocumented.
   Caches, invalidation rules, painter reuse, and QA evidence are not yet represented in a delivery program with trackers, release criteria, and GitHub Project discipline.

## Gap Matrix

| Capability Family | Current State | Product Gap / Risk | Target State | Phase |
| --- | --- | --- | --- | --- |
| API and state semantics | Generic values and tri-state plumbing already exist | Compatibility flags and mapping rules can remain ambiguous for consumers | One authoritative state/value contract with migration notes | [Phase 1](./01-phase1-contracts-state-and-api.md) |
| Layout and painter contract | Painter system exists and draws multiple styles | Layout math is still too embedded in the control draw path | Shared metrics and render context with style parity rules | [Phase 2](./02-phase2-layout-rendering-and-painter-contract.md) |
| Interaction and accessibility | Keyboard focus visibility and hover-aware painting exist | No full shipping contract for keyboard, automation, focus, and high contrast | Accessibility-first behavior with explicit interaction rules | [Phase 3](./03-phase3-input-accessibility-and-ux.md) |
| Binding, grid, and designer workflows | Wrapper types, grid mode, and design-time hooks exist | Runtime/design-time semantics can drift and grid mode can lag | Deterministic designer, binding, and host-surface behavior | [Phase 4](./04-phase4-binding-grid-and-designer-workflows.md) |
| Performance and reliability | Cache dictionaries and dirty-region refresh logic exist | No measurable lifecycle or stress-test program yet | Measured cache safety, repaint discipline, and stress gates | [Phase 5](./05-phase5-performance-reliability-and-diagnostics.md) |
| Documentation and shipping operations | Enhancement summary exists | No phase plan, project tracker, GitHub project playbook, or release gate | Product-ready roadmap, tracker, sample plan, QA plan, and release process | [Phase 6](./06-phase6-documentation-samples-and-release.md) |

## Phase Index

- [Phase 1 - Contracts, State, and API](./01-phase1-contracts-state-and-api.md)
- [Phase 2 - Layout, Rendering, and Painter Contract](./02-phase2-layout-rendering-and-painter-contract.md)
- [Phase 3 - Input, Accessibility, and UX Reliability](./03-phase3-input-accessibility-and-ux.md)
- [Phase 4 - Binding, Grid Mode, and Designer Workflows](./04-phase4-binding-grid-and-designer-workflows.md)
- [Phase 5 - Performance, Reliability, and Diagnostics](./05-phase5-performance-reliability-and-diagnostics.md)
- [Phase 6 - Documentation, Samples, and Release Readiness](./06-phase6-documentation-samples-and-release.md)
- [GitHub Source Benchmark Findings](./07-github-source-benchmark-findings.md)
- [Phase 1 Current API Audit](./08-phase1-current-api-audit.md)
- [Todo Tracker](./todo-tracker.md)
- [GitHub Project Playbook](./github-project-playbook.md)

## Commercial Product Standards

- Stabilize contract behavior before adding more styles or visual polish
- Keep runtime and design-time behavior aligned
- Treat accessibility and keyboard support as first-class, not cleanup work
- Prefer explicit metrics and token-based layout over ad hoc geometry
- Any change to contract, layout, input, or designer behavior should either follow a benchmark pattern or document an intentional divergence
- Require measurable performance and resource-lifecycle evidence
- Ship with documentation, sample coverage, QA evidence, and migration notes

## Delivery Model

- One GitHub Project for the full CheckBox commercialization program
- One epic per phase, backed by task-level child issues from the tracker
- Every phase exits with code, docs, QA notes, and a demonstrable host scenario
- No phase closes without updated plan status and release evidence in the tracker