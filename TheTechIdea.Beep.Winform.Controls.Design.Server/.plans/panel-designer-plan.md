# BeepPanelDesigner Plan

Status: partially completed in this pass.

Implemented:
- Introduced a shared `BaseBeepParentControlDesigner` for parent/container controls that still need the common Beep smart-tag actions.
- Moved `BeepPanelDesigner` onto that base.
- Added the implemented shared container actions (`Arrange Children`, `Clear All Children`) plus a panel-specific smart-tag surface for header text, title styling, title icon path, and header presets; placeholder dock/flow entries were removed from the shared list instead of advertising fake behavior.

Remaining follow-up:
- Migrate forms-shelf parent designers onto the same base to remove their duplicated `SetProperty` / `GetProperty<T>` implementations.