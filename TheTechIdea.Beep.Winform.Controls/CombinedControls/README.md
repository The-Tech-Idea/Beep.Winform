# CombinedControls

The CombinedControls folder contains composite Beep WinForms controls that combine multiple existing Beep selector surfaces into higher-level selection workflows.

## Included Controls

### BeepChipListBox

`BeepChipListBox` combines:

- `BeepListBox`
- `BeepMultiChipGroup`
- optional search via `BeepTextBox`

Primary use cases:

- tag and category selection
- filter builders
- multi-select pickers with chip feedback
- modern searchable selection panels

Key implementation files:

- `BeepChipListBox.cs`
- `BeepChipListBox.Core.cs`
- `BeepChipListBox.Properties.cs`
- `BeepChipListBox.Events.cs`
- `BeepChipListBox.Methods.cs`
- `BeepChipListBox.Drawing.cs`

### BeepRadioListBox

`BeepRadioListBox` combines:

- `BeepRadioGroup`
- `BeepListBox`
- optional search via `BeepTextBox`

Primary use cases:

- single-choice selectors with a richer detail list
- status or mode pickers
- compact settings panels
- guided choice workflows

Key implementation files:

- `BeepRadioListBox.cs`
- `BeepRadioListBox.Core.cs`
- `BeepRadioListBox.Properties.cs`

### ChipListBoxStyleCoordinator

`ChipListBoxStyleCoordinator` is the shared style-mapping helper that coordinates `ListBoxType`, `ChipStyle`, presets, and related UX defaults across the control family.

## Design Intent

This control family is intended to provide:

- coordinated visual variants
- synchronized list and chip or radio behavior
- searchable selector compositions
- commercial-grade UI/UX guidance aligned with modern design systems

## Plans

The full phased enhancement plan for this control family is in:

- [plans/README.md](plans/README.md)

That plan set includes:

- overview and gap matrix
- per-phase roadmap documents
- implementation checklists
- file-by-file matrices
- task breakdowns
- review notes
- TODO tracker

## Notes

- These controls inherit from `BaseControl` and should follow the shared Beep control authoring rules.
- The current planning package is intended to guide phased enhancement work before broad rollout.
