# Phase 2 - File-By-File Implementation Matrix

This matrix converts the Phase 2 checklist into file-specific interaction work.

## Execution Order

1. [BeepChipListBox.Events.cs](../BeepChipListBox.Events.cs)
2. [BeepChipListBox.Methods.cs](../BeepChipListBox.Methods.cs)
3. [BeepChipListBox.Core.cs](../BeepChipListBox.Core.cs)
4. [BeepRadioListBox.Core.cs](../BeepRadioListBox.Core.cs)
5. [BeepRadioListBox.Properties.cs](../BeepRadioListBox.Properties.cs)
6. [ChipListBoxStyleCoordinator.cs](../ChipListBoxStyleCoordinator.cs)

## Matrix

| File | Phase 2 responsibility | Specific work | Notes |
|---|---|---|---|
| [BeepChipListBox.Events.cs](../BeepChipListBox.Events.cs) | Interaction events | Validate selection, search, item-click, item-checked, focus, and keyboard behavior. Confirm event payloads are consistent and host-friendly. | This is the primary Phase 2 interaction file for chip behavior. |
| [BeepChipListBox.Methods.cs](../BeepChipListBox.Methods.cs) | Selection and sync helpers | Audit selection helper methods, search helpers, sync helpers, and style helpers for Phase 2 parity expectations. | This file contains the control-side actions that are easiest to harden first. |
| [BeepChipListBox.Core.cs](../BeepChipListBox.Core.cs) | Search and initialization behavior | Confirm the search box, chip group, list box, and sync helper are created and wired in the right order for interaction work. | Core layout and creation order should not break Phase 2 input flow. |
| [BeepRadioListBox.Core.cs](../BeepRadioListBox.Core.cs) | Search and selection behavior | Confirm the radio/list combination preserves single-select behavior, focus behavior, and synchronized item updates. | There is no separate events partial today, so the core file owns the interaction scaffolding. |
| [BeepRadioListBox.Properties.cs](../BeepRadioListBox.Properties.cs) | Search and keyboard-facing properties | Confirm `SearchText`, `ShowSearch`, `ShowRadioGroup`, `ShowListBox`, and `ListStyle` remain consistent with the new interaction rules. | This file owns the user-facing control knobs for Phase 2. |
| [ChipListBoxStyleCoordinator.cs](../ChipListBoxStyleCoordinator.cs) | Interaction-adjacent styling | Verify style mappings do not fight the interaction model for searchable, checkbox, avatar, and compact variants. | Style can reinforce or weaken interaction clarity, so keep the mapping aligned. |

## Phase 2 Deliverables By File

- `BeepChipListBox.Events.cs`: stable event semantics for selection and search.
- `BeepChipListBox.Methods.cs`: predictable selection and sync helpers.
- `BeepChipListBox.Core.cs`: reliable search and control wiring.
- `BeepRadioListBox.Core.cs`: reliable single-select wiring and search integration.
- `BeepRadioListBox.Properties.cs`: clear interaction-facing properties.
- `ChipListBoxStyleCoordinator.cs`: style mappings that support the interaction model.
