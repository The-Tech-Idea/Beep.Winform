# Phase 1 - Task Breakdown

This document converts the Phase 1 file-by-file matrix into a practical execution backlog.

## Step 1 - Stabilize BeepChipListBox Core

- Verify child-control creation order in `BeepChipListBox.Core.cs`.
- Confirm the search, chip, divider, and list panels are always added in a predictable docking sequence.
- Document where child-control visibility toggles should affect layout versus rendering.
- Record any constructor or initialization assumptions that Phase 2 must preserve.

## Step 2 - Normalize BeepChipListBox Style Contract

- Review style preset fields and properties in `BeepChipListBox.Properties.cs`.
- Define which style properties are primary user-facing inputs.
- Define which properties are compatibility shims or derived settings.
- Capture the recommended default combinations for common scenarios:
  - searchable chip selector
  - category chips
  - checkbox-style list + chips
  - avatar/team-member selector

## Step 3 - Confirm BeepChipListBox Behavior Helpers

- Review selection helpers in `BeepChipListBox.Methods.cs`.
- Confirm `SelectItem`, `AddToSelection`, `RemoveFromSelection`, `ClearSelection`, and `SetSelection` preserve the same selection source-of-truth.
- Confirm `SetIndependentStyles` is treated as a controlled escape hatch.
- Confirm sync helper methods remain descriptive and stable.

## Step 4 - Stabilize BeepRadioListBox Core

- Verify child-control creation order in `BeepRadioListBox.Core.cs`.
- Confirm the radio group remains the primary selector surface and the list remains the supporting detail surface.
- Document layout behavior when search, radio, divider, or list sections are hidden.
- Record any initialization assumptions that Phase 2 must preserve.

## Step 5 - Normalize BeepRadioListBox Style Contract

- Review style and layout properties in `BeepRadioListBox.Properties.cs`.
- Confirm recommended defaults for search height, radio area height, spacing, and list style.
- Document when `RadioStyle` and `RadioOrientation` should be exposed as presets versus overrides.
- Capture any naming or category changes that would make the API easier to understand.

## Step 6 - Validate Shared Style Coordination

- Review every mapping in `ChipListBoxStyleCoordinator.cs`.
- Decide which mappings are canonical defaults for the phase.
- Decide which mappings are niche and should be documented as optional variants.
- Confirm preset behavior and the helper methods `ShouldShowCheckboxes`, `ShouldShowSearch`, and `ShouldShowImages` are aligned with the intended UX model.

## Step 7 - Review And Handoff

- Record any discrepancies between the current implementation and the new Phase 1 contract.
- Mark anything that should be deferred into Phase 2 or Phase 3.
- Update the CombinedControls tracker with the implementation status.

## Phase 1 Review Gate

- `BeepChipListBox` and `BeepRadioListBox` should feel like siblings, not unrelated composites.
- Style coordination should be predictable from the public properties.
- Initialization and layout decisions should be stable enough that interaction work can begin without re-litigating the foundation.
