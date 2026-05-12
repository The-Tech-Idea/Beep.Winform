# Phase 2 - Implementation Checklist

This checklist turns the Phase 2 interaction plan into execution-ready verification steps.

## 1. Selection Sync Rules

- [ ] Verify list-to-chip selection sync in `BeepChipListBox.Events.cs`.
- [ ] Verify chip-to-list selection sync in `BeepChipListBox.Events.cs` and `BeepChipListBox.Methods.cs`.
- [ ] Verify radio-to-list selection sync in `BeepRadioListBox.Core.cs`.
- [ ] Confirm source-of-truth rules prevent recursion or duplicate updates.

## 2. Search And Discovery

- [ ] Confirm search box behavior in `BeepChipListBox.Core.cs` and `BeepRadioListBox.Core.cs`.
- [ ] Confirm `SearchText` behavior in `BeepRadioListBox.Properties.cs`.
- [ ] Define how search should behave when one or more child surfaces are hidden.
- [ ] Verify search feels like filtering, not a disruptive mode switch.

## 3. Keyboard Parity

- [ ] Review keyboard handling in `BeepChipListBox.Events.cs`.
- [ ] Review keyboard handling in `BeepRadioListBox.Properties.cs` and `BeepRadioListBox.Core.cs` if shortcut hooks exist there.
- [ ] Define canonical shortcuts for focus search, clear search, select all, activation, and escape/cancel.
- [ ] Record which shortcuts apply only to multi-select behavior.

## 4. Context And Activation Feedback

- [ ] Confirm click and checked-state events in `BeepChipListBox.Events.cs` are surfaced clearly.
- [ ] Confirm selection change events in both controls remain precise and readable for host apps.
- [ ] Define whether context menu support is required now or deferred to Phase 3.
- [ ] Confirm hover, focus, and pressed feedback remain distinguishable at small sizes.

## 5. Handoff Criteria

- [ ] Capture any interaction rules that should move to Phase 3 if they add visual-system complexity.
- [ ] Update the review notes with any selection or search edge cases.
- [ ] Ensure the Phase 2 tracker can be used to drive implementation without reopening Phase 1 scope.
