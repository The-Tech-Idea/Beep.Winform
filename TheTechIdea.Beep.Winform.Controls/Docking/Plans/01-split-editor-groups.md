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

---

## Outcome — the remaining items

### Minimum group size

`CommitGroupEdge` now refuses a split before mutating anything, via
`DockingLayoutController.CanAcceptAdditionalChild`, and reports it through `DockingError`.

The failure being guarded against is worse than this document assumed. It says a bad split would
"produce a 3px group"; it does not. `AssignPanelsRecursive` computes
`available = extent - splitters` and, when that reaches zero, **returns having assigned no bounds at
all** — so the group and everything in it disappears rather than becoming thin. A sliver would at
least be visible.

Checked before the mutation because afterwards the tree has already been rearranged and there is
nothing left to refuse.

Measured as a **pair**, which is the only way the result means anything:

| scenario | splits accepted |
|---|---|
| 249px Left edge (halves are 68px; 50+50+4 does not fit) | **1 of 19** |
| 1600x900 Fill area | **7 of 7** |

A guard that refused everything would also produce the first row. Without the second, "the guard
works" and "the guard is broken" look identical.

### Overlapping bounds

`ErrorType.OverlappingBounds` could not be raised because overlap is a property of the **computed
result**, not of the tree, and `LayoutValidator` only ever saw the tree. It now has
`Validate(DockLayoutResult)`.

Verified against a deliberately overlapping result built by hand — the layout engine is not supposed
to be able to produce one, which is precisely why a check that silently never fires would go
unnoticed:

```
High - OverlappingBounds: Panels 'editor' {200,200,400,400} and 'explorer' {0,0,400,400}
                          overlap over 200x200 [editor,explorer]
```

and against a disjoint arrangement of adjacent panels, which reports nothing.

### The validator now runs

`LayoutValidator` implements real checks — unreachable groups, circular parents, panels registered to
one group while belonging to another — and **nothing in the product ever constructed one**. It was
reachable only from the test project, so a tree that drifted stayed broken silently.

`ValidateAfterStructuralChange` runs it where the tree is genuinely rearranged (split, restore),
reporting through `DockingError`, with `ValidateLayoutOnChange` to switch it off and a public
`ValidateLayout()` for on-demand use. Deliberately not on every layout pass: it walks the tree and
compares every pair of placed panels, which is wasted work when nothing structural changed.

### Wiring it in immediately found two defects

**The validator was wrong.** `RatioWithoutSplit` fired on every healthy layout. Its rule assumed
`SplitRatio` only ever means "the split between my children" — but a **root edge group** uses it as
its share of the container (`BuildLayout` computes the edge size as `available * SplitRatio`), so a
Left group with one panel and no children legitimately carries one. It passed against synthetic trees
in the test project and would have reported errors on every real layout. Now excluded for root edges.

**Adding a panel to a split edge stranded it.** `GetOrCreateGroupAtPosition` returned the root edge
group even when that group had children, and all twelve callers use the result to place a panel in.
A panel added to an already-split edge therefore sat directly on a parent group — and
`AssignPanelsRecursive` allocates a group's own panels only when it has no visible children, so the
panel was docked, registered, and had no bounds:

```
Fill(Left[p5](Left[a] Left[p2] Left[p3] Left[p4]))     p5 unplaced
```

That is the `MixedContent` state the validator already names — it predicted the bug it was being
wired in to catch. `GetOrCreateGroupAtPosition` now descends to a leaf, preferring the branch holding
an active panel so a new panel joins the group the user was last working in.

Also fixed in passing: `AssignPanelsRecursive` clamped both axes with `MIN_PANEL_WIDTH`, enforcing a
width rule on a vertical split's heights.

`DockProbe`: **143 passed, 0 failed**. Docking test suite: **48/48**. Solution builds with 0 errors.

### Per-group tab strips — verified, and it found a sizing defect

Each group does carry its own strip and its own active tab. Every `DockPanel` builds its strip from
its own `Group.Panels` and highlights its own `Group.ActivePanel`, and the two groups are
independent: activating a tab in one leaves the other's active panel untouched.

```
doc1  strip: [Doc 1, Doc 3]   group holds: [doc1, doc3]
doc2  strip: [Doc 2]          group holds: [doc2]
doc3  strip: [Doc 1, Doc 3]   group holds: [doc1, doc3]
```

**The check was wrong twice before it was right**, and both corrections are the point.

*First*, it read the strip of a panel that was not on screen. An inactive panel in a stack sits at
its default size, and a 200px strip genuinely cannot fit two 160px tabs — so it overflows and drops
one. That measured the overflow rule, not the grouping, and reported a defect where the design was
working. The overflow rule is now asserted directly instead (200px overflows, full width does not).

*Second*, after correcting it to activate each panel first, `doc3` **still** reported one tab — and
this time it was real:

```
doc3  control=200x100   engine=1400x398   parent=Form   state=Docked   vis=True
```

The layout engine had the right rectangle; the control never received it. An explicit `ApplyLayout()`
fixed it, which separated "geometry is wrong" from "geometry is right but never applied".

`AddPanel` called `ApplyLayout()` **before** `_layoutController.InvalidateLayout()`. So the apply
consumed a cached layout computed before the panel joined the group, found no bounds for it, and
skipped it. Adding the first panel to an edge was masked, because the host form's own layout pass
re-triggered a recalculation; joining an existing stack was not. Invalidation now happens as soon as
the tree changes.

A panel added to an occupied group was therefore left at 200x100 until something unrelated forced a
relayout — visible to any user who opens a second document in the same tab group.

### Still open

Nothing. Feature 01 is complete.
