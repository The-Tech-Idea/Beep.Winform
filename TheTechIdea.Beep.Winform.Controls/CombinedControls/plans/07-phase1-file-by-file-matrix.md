# Phase 1 - File-By-File Implementation Matrix

This matrix converts the Phase 1 checklist into concrete work by file.

## Execution Order

1. [BeepChipListBox.Core.cs](../BeepChipListBox.Core.cs)
2. [BeepChipListBox.Properties.cs](../BeepChipListBox.Properties.cs)
3. [BeepChipListBox.Methods.cs](../BeepChipListBox.Methods.cs)
4. [BeepRadioListBox.Core.cs](../BeepRadioListBox.Core.cs)
5. [BeepRadioListBox.Properties.cs](../BeepRadioListBox.Properties.cs)
6. [ChipListBoxStyleCoordinator.cs](../ChipListBoxStyleCoordinator.cs)

## Matrix

| File | Phase 1 responsibility | Specific work | Notes |
|---|---|---|---|
| [BeepChipListBox.Core.cs](../BeepChipListBox.Core.cs) | Foundation and initialization | Verify child-control setup order, default size, docking hierarchy, and sync wiring. Identify where layout and visibility guards should live. | This is the best place to stabilize initialization and child composition. |
| [BeepChipListBox.Properties.cs](../BeepChipListBox.Properties.cs) | Style contract and defaults | Review `StylePreset`, `CoordinateStyles`, `StyleCoordinationMode`, `Layout`, `ListBoxType`, `ChipStyle`, and visibility properties. Document which properties are primary and which are compatibility shims. | This file owns the public contract, so it should define default UX behavior clearly. |
| [BeepChipListBox.Methods.cs](../BeepChipListBox.Methods.cs) | Explicit control behavior | Audit selection, sync, and style helper methods for consistency with the foundation rules. Confirm `SetIndependentStyles`, `GetRecommendedChipStyle`, `GetRecommendedListBoxType`, and sync helpers match the planned contract. | Phase 1 should keep behavior stable, but the method surface needs a clearer policy. |
| [BeepRadioListBox.Core.cs](../BeepRadioListBox.Core.cs) | Foundation and initialization | Verify child-control setup order, default size, docking hierarchy, and sync setup for the radio/list combination. Confirm the control roles are documented: radio group as primary selector, list as detail surface. | This file mirrors the chip control and should follow the same initialization discipline. |
| [BeepRadioListBox.Properties.cs](../BeepRadioListBox.Properties.cs) | Style contract and defaults | Review `Layout`, `ShowSearch`, `ShowRadioGroup`, `ShowListBox`, `ShowDivider`, `SearchBoxHeight`, `RadioAreaHeight`, `Spacing`, `Style`, `RadioStyle`, `RadioOrientation`, and `ListStyle`. Define recommended defaults and naming guidance. | The property contract should make the control feel like a polished preset-driven selector. |
| [ChipListBoxStyleCoordinator.cs](../ChipListBoxStyleCoordinator.cs) | Shared style coordination | Validate all list/chip mapping tables and the preset switch. Confirm which combinations are commercial defaults and which are niche variants. | This is the shared style authority for Phase 1 and should be documented as such. |

## Phase 1 Deliverables By File

- `BeepChipListBox.Core.cs`: stable control composition and initialization rules.
- `BeepChipListBox.Properties.cs`: documented defaults, style policy, and layout visibility guidance.
- `BeepChipListBox.Methods.cs`: consistent helper behavior for selection and style coordination.
- `BeepRadioListBox.Core.cs`: stable radio/list composition and initialization rules.
- `BeepRadioListBox.Properties.cs`: clear layout and radio/list style contract.
- `ChipListBoxStyleCoordinator.cs`: canonical style mapping authority.

## Phase 1 Acceptance Notes

- The chip and radio controls should feel like two variants of the same product family.
- Style defaults should be explainable without reading source internals.
- Initialization should be predictable enough that Phase 2 can focus on interaction parity.
