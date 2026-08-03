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

---

## Outcome — the premise was wrong

### What the audit got wrong

This document opened with *"`grep -rn "TabGroup\|SplitGroup"` returns **0** … the docking system
cannot split the workspace"*. Those are **VS Code's names, not this codebase's**. Measuring what the
code does rather than what it is called found both halves already present:

**The model is a splittable tree.** `DockGroup` carries `Parent`, `Children`, `SplitOrientation`,
`SplitRatio` and `GetAllPanelsRecursive()`. `DockLayoutTree` has `Root`, a schema version and
panel/group registries. This document's conditional — *"if it is already a node in a splittable tree,
this feature is mostly interaction"* — is the branch that applies.

**Splitting already worked by drag.** `BeepDockingManager.DragDrop.cs`'s
`IDockDragHost.CommitGroupEdge` promotes a leaf group's panels into a child group, adds the dragged
panel as a sibling child, sets `SplitOrientation` from the drop side, registers both groups and
recalculates. That is a complete split.

**Collapse-on-empty already worked**, which this document listed as work to do. Measured: removing
the last panel of a group took the tree from 3 groups to 2 and the shape from
`Fill(Left[a,b] Right[c])` to `Fill(Left[a,b])`.

### The real gap: no command surface

Splitting was reachable **only with a pointer**. `CommitGroupEdge` sat behind an explicit
`IDockDragHost` implementation, so nothing could bind it to a key, put it in a menu, or script it —
and features [02](02-layout-perspectives.md), [04](04-maximise-and-zen.md) and
[06](06-keyboard-and-accessibility.md) all need it as an operation.

Added, reusing the drag path's logic rather than reimplementing it:

- `SplitPanel(panelKey, direction)` — the primitive
- `SplitPanelRight(panelKey)` / `SplitPanelDown(panelKey)` — VS Code's and Rider's named commands

The explicit interface implementation now forwards to a private `CommitGroupEdge`, so the drag and
the command run identical code.

### Measured

```
before split: Fill(Left[a,b] Right[c])
after  split: Fill(Left(Left[a] Left[b]) Right[c])
```

A genuine nested split from a command, with the panel still reachable afterwards.

### Remaining

- [ ] Per-group tab strip with its own active tab — `DockGroup.ActivePanel` exists; whether each
      nested group renders its own strip needs checking rather than assuming
- [ ] Keyboard bindings for the new commands — belongs with [06](06-keyboard-and-accessibility.md)
      so the folder gets one key table, not four
- [ ] Minimum group size: a split that would leave either half unusably small should be refused
      rather than produce a sliver
- [ ] Overlapping-bounds validation, moved here from [09](09-dead-surface.md): `LayoutValidator`
      declared an `OverlappingBounds` error it never raised, and splitting is what creates the risk
