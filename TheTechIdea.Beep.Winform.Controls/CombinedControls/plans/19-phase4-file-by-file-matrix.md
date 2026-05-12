# Phase 4 - File-By-File Implementation Matrix

This matrix converts the Phase 4 checklist into file-specific documentation and productization work.

## Execution Order

1. [BeepChipListBox.cs](../BeepChipListBox.cs)
2. [BeepChipListBox.Core.cs](../BeepChipListBox.Core.cs)
3. [BeepChipListBox.Properties.cs](../BeepChipListBox.Properties.cs)
4. [BeepChipListBox.Methods.cs](../BeepChipListBox.Methods.cs)
5. [BeepRadioListBox.cs](../BeepRadioListBox.cs)
6. [BeepRadioListBox.Core.cs](../BeepRadioListBox.Core.cs)
7. [BeepRadioListBox.Properties.cs](../BeepRadioListBox.Properties.cs)
8. [ChipListBoxStyleCoordinator.cs](../ChipListBoxStyleCoordinator.cs)

## Matrix

| File | Phase 4 responsibility | Specific work | Notes |
|---|---|---|---|
| [BeepChipListBox.cs](../BeepChipListBox.cs) | Product family overview | Document the class-level role and ensure the main type summary is aligned with the final product story. | This is the best place to keep the public overview concise and accurate. |
| [BeepChipListBox.Core.cs](../BeepChipListBox.Core.cs) | Design-time / initialization guidance | Confirm the core initialization story is suitable for docs and sample guidance. | The core file should support the user-facing story without adding noise. |
| [BeepChipListBox.Properties.cs](../BeepChipListBox.Properties.cs) | Usage recipes and property guidance | Review the public properties and identify recommended combinations, constraints, and common scenarios to highlight in docs. | This file is the main source for recipe documentation. |
| [BeepChipListBox.Methods.cs](../BeepChipListBox.Methods.cs) | Selection and sync usage guidance | Document the public methods that users should call for selection, sync, and style workflows. | Keep the method guidance oriented toward adoption, not internals. |
| [BeepRadioListBox.cs](../BeepRadioListBox.cs) | Product family overview | Document the class-level role and ensure the main type summary matches the final product story. | This should read like the sibling of `BeepChipListBox`. |
| [BeepRadioListBox.Core.cs](../BeepRadioListBox.Core.cs) | Design-time / initialization guidance | Confirm the core initialization story is suitable for docs and runtime samples. | The radio/list composition should be easy to explain to adopters. |
| [BeepRadioListBox.Properties.cs](../BeepRadioListBox.Properties.cs) | Usage recipes and property guidance | Review the public properties and identify recommended combinations, constraints, and common scenarios to highlight in docs. | This file is the main source for radio-list recipes. |
| [ChipListBoxStyleCoordinator.cs](../ChipListBoxStyleCoordinator.cs) | Showcase and variant mapping | Document the official style presets and the control-family variants they represent. | This file drives the sample showcase and release-ready style guidance. |

## Phase 4 Deliverables By File

- `BeepChipListBox.cs`: concise class-level product story.
- `BeepChipListBox.Properties.cs`: public recipe and usage guidance.
- `BeepChipListBox.Methods.cs`: adoption-friendly method guidance.
- `BeepRadioListBox.cs`: concise class-level product story.
- `BeepRadioListBox.Properties.cs`: public recipe and usage guidance.
- `ChipListBoxStyleCoordinator.cs`: variant and showcase guidance.
