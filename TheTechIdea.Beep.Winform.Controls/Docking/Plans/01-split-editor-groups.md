# 01 — Split editor groups

## What is missing

`grep -rn "TabGroup\|SplitGroup"` across `Docking/` returns **0**.

The docking system can place panels around a workspace, but it cannot split the workspace itself into
independent tab groups. Every reference product can:

| product | behaviour |
|---|---|
| VS Code | drag a tab to any edge of the editor area to split; each group has its own tab strip and active tab; groups nest horizontally and vertically |
| Visual Studio | vertical/horizontal document group split, drag between groups |
| JetBrains Rider | `Split Right` / `Split Down`, unlimited nesting |
| Dockview / FlexLayout / Golden Layout | the entire model is a tree of split groups |

This is the single most visible gap. A user comparing two files side by side, or keeping a reference
document open while editing, cannot do it.

## What already exists to build on

- `Models/DockLayoutTree.cs` (230 lines) — a tree model is already the persistence shape
- `Models/DockGroup.cs` (289 lines) — grouping exists at some level
- `Layout/LayoutCalculator.cs`, `Layout/LayoutValidator.cs`, `Layout/DockingLayoutController.cs`
- `Runtime/TabInteractionHandler.cs` (399 lines) — tab drag already handled

The first task is to establish **what `DockGroup` currently models**. If it is already a node in a
splittable tree, this feature is mostly interaction and painting. If it is a flat collection, the
model work comes first.

## Design

Follow the tree model every reference product converged on:

```
DockSplitNode  (orientation: horizontal | vertical, ratio: 0..1)
├── DockSplitNode
│   ├── DockTabGroupNode  (panels[], activeIndex)
│   └── DockTabGroupNode
└── DockTabGroupNode
```

- Dragging a tab onto the **centre** of a group joins that group
- Dragging onto an **edge** splits it, creating a `DockSplitNode` with the dragged panel in the new
  half — the interaction feature 05 covers
- Closing the last tab in a group collapses the group and its parent split, promoting the sibling
- Ratios survive resize proportionally, and are what persistence stores

## Work

- [ ] Establish what `DockGroup` and `DockLayoutTree` model today, before designing anything
- [ ] Split/join operations on the tree, with the collapse-on-empty rule
- [ ] `SplitRight` / `SplitDown` commands, keyboard-invocable (feature 06)
- [ ] Per-group tab strip with its own active tab
- [ ] Move-to-group and move-between-groups by drag and by keyboard
- [ ] Ratio-preserving resize, and a minimum group size below which a split is refused

## Verification

- Split a group; assert two groups exist and each has its own active panel
- Close the last panel in a group; assert the group collapses and its sibling takes the full space
- Nest three levels deep; assert ratios survive a host resize within 1px
- Round-trip through persistence; assert the tree is identical (feature 07)
- Refuse a split that would put either half below the minimum, rather than producing a 3px group
