# Phase 1 - Contracts, State, And API

Priority: Critical  
Status: Not Started  
Depends on: None

## Objective

Define one authoritative product contract for `BeepCheckBox<T>` so future work is built on stable semantics instead of implicit behavior.

Current source audit: `08-phase1-current-api-audit.md`

## Why This Phase Comes First

The current control already supports more than a simple boolean toggle. It has tri-state visual state, generic value mapping, wrapper types, and compatibility fields that suggest an evolving API. Commercial products do not leave those semantics to inference. The surface has to be explicit before rendering, designer, and QA work can be trusted.

## Scope

- public property contract for `State`, `CurrentValue`, and value-mapping properties
- wrapper policy for `BeepCheckBoxBool`, `BeepCheckBoxChar`, and `BeepCheckBoxString`
- compatibility rules for legacy mapping behavior
- event/change ordering policy
- serialization and designer visibility policy
- migration and non-breaking change strategy

## Benchmark Anchors

- Krypton treats `Checked`, `CheckState`, `ThreeState`, and `AutoCheck` as separate but coordinated contracts.
- ReaLTaiizor and SunnyUI treat `ReadOnly` as enforced interaction policy, not only a visual hint.
- SunnyUI distinguishes classic checked-state change from value-oriented consumers, while MaterialSkin and ReaLTaiizor keep preferred-size behavior explicit enough that state semantics do not leak into layout.

## Deliverables

- `BCHK-P1-001` Inventory the public API exposed by `BeepCheckBox<T>` and wrapper classes and classify each member as stable, compatibility-only, or candidate for tightening, using `08-phase1-current-api-audit.md` as the source baseline.
- `BCHK-P1-002` Define the canonical tri-state contract for `Checked`, `Unchecked`, and `Indeterminate`, including how each state maps to `CurrentValue` and whether `Checked` is a convenience projection or a separately supported semantic property.
- `BCHK-P1-003` Document and decide the long-term role of `_mapStateValuesToLegacy`, `_useUncheckedValueForIndeterminate`, and any `ReadOnly`-style interaction policy, including whether they remain public-facing behavior, internal compatibility switches, or migration-only paths.
- `BCHK-P1-004` Normalize property metadata: categories, default values, serialization visibility, and naming consistency.
- `BCHK-P1-005` Define deterministic change sequencing for toggle operations, value updates, and any submit/change notifications emitted by the control.
- `BCHK-P1-006` Produce a migration note for existing consumers so wrapper defaults and generic behavior remain predictable.

## Recommended Work Breakdown

1. Audit `BeepCheckBox.cs` for every state/value property and record what changes state, what changes data, and what is display-only.
2. Separate "visual state" from "domain value" in the written contract even if the first code change keeps them in one class.
3. Decide whether indeterminate state is a first-class data value or a visual-only affordance for some wrappers.
4. Review wrapper defaults for bool, char, and string to ensure they are product-safe for serialization, data binding, and design-time authoring.
5. Capture non-breaking rules so future refactors do not silently change toggle semantics.
6. Record where `BeepCheckBox` intentionally matches or diverges from the benchmark patterns for `Checked`/`CheckState`, read-only enforcement, and event ordering.

## File Focus

- `CheckBoxes/BeepCheckBox.cs`
- `CheckBoxes/BeepCheckBox.Events.cs`
- `CheckBoxes/BeepCheckBox.Methods.cs`
- `CheckBoxes/Models/CheckBoxStyleConfig.cs`
- `CheckBoxes/Models/CheckBoxColorConfig.cs`

## Acceptance Criteria

- Every public state/value member has one documented purpose.
- Toggle and value-update sequencing is deterministic and documented.
- Wrapper types have explicit product positioning and safe defaults.
- Compatibility behavior is intentionally preserved, renamed, hidden, or scheduled for deprecation.
- Any intentional divergence from benchmarked checkbox contracts is documented, not accidental.
- The phase ends with migration notes, not just code intent.

## Out Of Scope

- painter visual redesign
- performance tuning
- new control styles
- sample gallery work beyond minimal contract validation

## GitHub Project Mapping

Epic: `Phase 1 - Contracts, State, and API`  
Suggested labels: `area:checkbox`, `phase:1`, `type:architecture`, `type:api`, `priority:p0`

Recommended issue split:

- one issue for state/value contract
- one issue for compatibility-policy decisions
- one issue for serialization/designer metadata cleanup
- one issue for migration notes and consumer guidance

## Exit Evidence

- source audit committed and linked from the phase doc
- API review summary attached to the phase epic
- migration note committed to docs
- issue links added to the tracker
- at least one demo scenario proving wrapper and generic behavior