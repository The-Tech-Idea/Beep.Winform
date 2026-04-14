# Tabs And Layout Container Actions

## Scope

- `BeepTabsDesigner`
- `BeepLayoutControlDesigner`

## Completed

- `BeepTabsDesigner` now provides undo-aware add, remove, clear, reorder, and select-tab actions plus selected-tab caption editing from the smart tag and designer verbs, and it synchronizes designer selection to the active `TabPage` after add/remove/navigation operations
- `BeepLayoutControlDesigner` now provides restore-template, clear-generated-children, rebuild, 2 x 2 grid, and split-workspace actions in addition to template selection

## Follow-Up

- Consider explicit slot or placeholder editors for `BeepLayoutControl` only if the runtime grows authored layout metadata beyond template regeneration