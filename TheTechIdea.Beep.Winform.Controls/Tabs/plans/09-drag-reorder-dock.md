# 09 — Drag, Reorder & Dock

## Done — the two reorder paths now share one rule

**Dragging bypassed every constraint the context menu enforced.** `BeepTabs_DragDrop` called
`TryMoveHostedSourceItem(draggedIndex, newIndex)` directly, checking nothing, while Move Left/Right
went through `CanMoveHeaderTab`. So a pinned tab could not be moved past an unpinned one with the
menu command, but could be dragged anywhere; `TabCanReorder = false` was likewise honoured only by
the menu. The constraints applied to the path users were *less* likely to take.

Both paths now call `CanReorderTabTo(fromIndex, toIndex)` — one predicate covering mode, per-item
`CanReorder`, enabled/visible state, and the pinned partition. A tab that cannot be reordered also
no longer *starts* a drag: showing drop markers for a move that will be rejected reads as a broken
control rather than as a fixed tab.

**Pinning meant "immovable" where it should have meant "confined".** The item snapshot cleared
`CanReorder` for every pinned tab, which was both redundant with the partition check and stricter
than it — pinned tabs could not be reordered *among themselves*, which VS, VS Code and Chrome all
allow. The snapshot no longer second-guesses the partition rule.

Seven assertions, with 5 tabs (0–1 pinned, 3 non-reorderable), verified by running the predicates:
the partition cannot be crossed in either direction, reorder within each partition is allowed,
`TabCanReorder = false` blocks both the move and the drag start, Navigation mode refuses reorder
entirely, and — the point of the exercise — **the menu path and the drag path agree on every
adjacent move**.

**Tear-out is still undecided**, and remains the open question for this feature.

## Original findings


**Priority P2.**

## Current behaviour

Drag and reorder have real infrastructure: 9 files reference drag, 9 reference reorder,
`Models/BeepTabHeaderDragFeedback.cs` models the visual feedback, `BeepTabItem.IsDragging` carries
the state, and `BeepTabPage.TabCanReorder` gates it per page. `Hosts/BeepTabHeaderHost.Mouse.cs`
(338 lines) is the largest host partial, which is where drag handling would live.

Docking is thinner — 5 files mention it, with no dedicated model or host partial. Whether "dock"
here means real split/dock targets or just `Control.Dock` is not clear from the file names alone and
must be established before any work is planned on it.

`BeepTabs.HostedContent.MovePage` exists and returns `bool`, so programmatic reorder has a defined
API. What is unproven is whether the *interactive* reorder path goes through it or manipulates the
collection separately — a second path would be the same duplication problem this repo has hit
repeatedly.

## What the reference products do

- **Reorder**: drag within the strip with a live insertion indicator; the dragged tab follows the
  pointer; dropping outside the strip either cancels or tears out.
- **Tear-out**: dragging a tab out of the window creates a new window hosting that document
  (VS Code, Chrome, Visual Studio).
- **Dock**: dragging over a document area shows split targets (left/right/top/bottom/centre) and
  drops create a split view.
- **Constraints**: pinned tabs reorder only among pinned tabs; `CanReorder = false` tabs cannot be
  moved and cannot be displaced.

## Work

1. **Establish what "dock" currently means here** before planning it. If it is only `Control.Dock`,
   say so and treat real docking as new work rather than a gap in existing behaviour.
2. **Confirm interactive reorder routes through `MovePage`**, so there is one reorder implementation
   with one set of events and one validation path.
3. **Honour the constraints**: `TabCanReorder`, and pinned tabs partitioned from unpinned ones. A
   pinned tab that can be dragged into the unpinned run makes pinning meaningless.
4. **Insertion feedback** — `BeepTabHeaderDragFeedback` exists; verify it renders an insertion
   indicator at the drop index, and that the index it shows is the index actually used on drop.
5. **Decide on tear-out** explicitly. It is a significant feature and the Workspace mode implies it;
   it should be scheduled or ruled out, not left ambiguous.

## Verification

- Probe: drag tab 1 to position 4 and assert the resulting order, that `MovePage` was the path
  taken, and that the insertion indicator index matched the final index.
- Probe: attempt to drag a `TabCanReorder = false` tab and assert no move occurs.
- Probe: attempt to drag an unpinned tab into the pinned run and assert it is rejected.
- Probe: cancel a drag (Escape / drop outside) and assert the order is unchanged.
