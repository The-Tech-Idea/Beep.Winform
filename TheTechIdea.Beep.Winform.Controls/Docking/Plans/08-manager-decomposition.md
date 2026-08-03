# 08 — Decompose `BeepDockingManager`

## Finding

```
3,317  BeepDockingManager.cs
  293  BeepDockingManager.DragDrop.cs
  264  BeepDockingManager.Persistence.cs
```

`BeepDockingManager.cs` is **3,317 lines** — the largest single file in this control library, and
four times the size of the next largest thing in the folder (`Models/DockPanel.cs`, 999).

It has **no `#region` markers at all**, so there is no stated internal structure to read. The partial
split that exists took out drag-drop and persistence and stopped.

## Why this comes before the features

Features [01](01-split-editor-groups.md) through [04](04-maximise-and-zen.md) all add behaviour to
the manager. Adding four features to a 3,317-line file with no internal boundaries produces a
5,000-line file with no internal boundaries, and every subsequent change gets more expensive.

The `Filtering` program hit a smaller version of this: `BeepFilter.cs` at 1,358 lines with regions
named for the increment that produced them (`Phase 1: …`). Splitting it into cohesive partials took
it to 790 and cost nothing, because the public surface did not change.

## Approach

**Establish the concerns first, by reading, before moving anything.** The `Filtering` decomposition
began with a hypothesis — that a 480-line region duplicated the model helpers — which reading
disproved: it was UI interaction, and the correct move was to split by concern rather than to
de-duplicate. The same discipline applies here, and the answer will be different.

Likely seams, to be confirmed rather than assumed:

- panel registration and lifecycle
- dock/undock/float operations
- auto-hide (145 references across the folder — likely a large share of the file)
- layout coordination, delegating to `Layout/DockingLayoutController`
- events and notification
- theming

Target: no partial much over 400 lines, each named for what it holds.

## Work

- [ ] Read the file and record what is actually in it, in order, before proposing a split
- [ ] Extract by concern into partials, one at a time, building between each
- [ ] Give every partial a summary that says what belongs in it — the boundary is only useful if the
      next person can tell where their change goes
- [ ] No public signature changes; decomposition is not an API change

## Verification

- The public surface is unchanged: the solution builds with 0 errors, **and every sibling repository
  builds**. `Beep.Winform.Data.Integrated` and `Beep.Sample` consume this library, and a search of
  this repository alone has already missed cross-repo consumers once
- The harness ([10](10-verification-harness.md)) reports the same results before and after each
  extraction — the point of doing it first

---

## Outcome — first pass

### What reading found

This document said to read the file before proposing a split, because the `Filtering` decomposition
began with a hypothesis that reading disproved. Reading found a clear order — construction, host-form
wiring, panel registration, layout application, direct panel operations, bulk/request API, event
raisers, MRU/navigator, dispose — with no duplication of the layout helpers. The problem is size and
the absence of any stated boundary, not layering.

### Extracted

| file | lines | concern |
|---|---|---|
| `BeepDockingManager.Events.cs` | 137 | the 26 event raisers |
| `BeepDockingManager.Theme.cs` | 125 | theme application across panels, strips, floats, splitters |
| `BeepDockingManager.Navigation.cs` | 189 | MRU tracking and the Ctrl+Tab navigator it drives |
| `BeepDockingManager.PanelRequests.cs` | 238 | bulk operations, store/restore, state-change requests |

`BeepDockingManager.cs`: **3,317 → 2,742**. Four partials, each named for what it holds.

The `PanelRequests` grouping is a real seam rather than a convenience: those members share a shape —
each is a request that may be refused — as distinct from the direct operations that simply act.

### An extraction error worth recording

The first attempt's end-detection walked past the last event raiser and pulled `PushMrPanel` into
`Events.cs` with it. Caught by grepping for the declaration afterwards, not by the compiler — the
code still compiled, it was simply in the wrong file. It was moved into `Navigation.cs`, where it
belongs, and the extraction helper was rewritten to track brace depth from the last member's opening
brace rather than guessing.

**A partial split that compiles is not evidence the boundary is right.** Verify by asking where each
member landed, not by asking whether it builds.

### Remaining

`BeepDockingManager.cs` is still 2,742 lines — above the ~400 target. The seams are identified and
each is a contiguous block:

- [ ] host-form wiring — `ManageControl`, `Attach`/`DetachHostFormHandlers`, `OnHostFormLayoutChanged`,
      `EnsurePanelHosted`, `DetachPanelFromParent` (~120 lines)
- [ ] float-window and auto-hide-strip lifecycle — `CloseFloatWindowFor`, `CloseAllFloatWindows`,
      `ClearAllAutoHidePanels`, `DetachFromAutoHideStrip`, `CreateAutoHideStrips` (~220 lines)
- [ ] splitters — `SyncSplitters`, `OnEngineSplitterMoved` (~120 lines)
- [ ] panel registration — `AddPanel`, `Register`/`UnregisterExistingPanel`, the `Notify*` trio,
      `RegisterDesignerCreatedPanels`, `RemovePanel` (~280 lines)
- [ ] layout application — `ApplyLayout`, `SyncDockspaceDockStyles`, `SeedEdgeRatios`,
      `PruneEmptyRootGroups`, `ApplyLayoutBounds` and helpers (~200 lines)

Stopping here rather than continuing is deliberate: each extraction is cheap individually, but the
one error above shows they need checking one at a time, and the value of the remaining five is lower
than the value of feature 10's harness existing before feature work begins.

---

## Outcome — the remaining five seams

All five identified seams are extracted. `BeepDockingManager.cs`: **3,317 → 1,947**.

| file | lines | concern |
|---|---|---|
| `BeepDockingManager.cs` | 1,947 | construction, options, direct panel operations, queries, dispose |
| `.Persistence.cs` | 388 | definition capture and materialisation |
| `.Navigation.cs` | 360 | MRU, the Ctrl+Tab navigator, and the key-binding table |
| `.DragDrop.cs` | 349 | drag session and the split commit |
| `.Registration.cs` | 289 | membership: how panels enter and leave |
| `.Maximise.cs` | 282 | maximise and zen |
| `.PanelRequests.cs` | 238 | bulk operations and requests that may be refused |
| `.LayoutApplication.cs` | 235 | turning a computed layout into control bounds |
| `.Perspectives.cs` | 206 | named layouts |
| `.DetachedSurfaces.cs` | 171 | float-window and auto-hide-strip lifetime |
| `.Hosting.cs` | 168 | host-form wiring and control parenting |
| `.Splitters.cs` | 162 | splitter reconciliation |
| `.Events.cs` | 137 | the event raisers |
| `.Theme.cs` | 125 | theme application |
| `.Validation.cs` | 77 | consistency checking |
| `.Monitors.cs` | 73 | the display set |

### Every summary states a rule, not a label

This document asked that each partial say what belongs in it, "because the boundary is only useful if
the next person can tell where their change goes". A heading naming the members is not that. Each
summary states the invariant the file exists to keep — for example:

- `LayoutApplication`: the controller decides geometry and never touches a control; this file applies
  geometry and never computes any. **A number the result does not carry belongs in the controller.**
- `DetachedSurfaces`: when the surface goes, the panel's state goes with it. That is precisely the
  rule whose absence made a float on a removed display unrecoverable in
  [03](03-multi-monitor-floating.md).
- `Splitters`: splitters are derived from the layout result, never stored alongside it.

### Verified by placement, not by compiling

The first pass recorded the rule the hard way: an extraction pulled `PushMrPanel` into `Events.cs`
and **still compiled**, because being in the wrong file is not a compile error.

So the extraction tool locates each member's span by matching braces from its own opening brace, and
a separate audit then asserts that each of the **32** moved members is declared exactly once, in the
file it was meant to go to:

```
placement audit: 0 misplaced of 32
```

That check is what makes the split trustworthy. The build succeeding says nothing about it.

### Behaviour unchanged

- `DockProbe`: **166 passed, 0 failed** — identical to before the extraction
- Docking test suite: **48/48**
- Solution: 0 errors
- `Beep.Winform.Data.Integrated`: builds clean

`Beep.Sample.Winform` does not build, on
`Beep.Sample.Common/Services/LocalDatabaseService.cs` calling `LoadStructure(..., copydata:)`.
Unrelated and pre-existing: `LoadStructure` does not exist anywhere in this repository — it is a
data-engine API this sample has drifted from, and nothing in it touches docking.

### Remaining

`BeepDockingManager.cs` is 1,947 lines, still well above the ~400 target. What is left is the core:
construction and options, the direct panel operations (`ShowPanel`, `HidePanel`, `ClosePanel`,
`FloatPanel`, `AutoHidePanel`, `MovePanel`, `ActivatePanel`), queries, and dispose.

Those resist the same treatment for a real reason rather than a lack of appetite: they are the
manager's actual behaviour, they call each other, and splitting them further would separate
operations that must agree about state transitions — exactly the coupling the three
stale-state defects in this program came from. A further split should follow a decision about
**what a panel state transition is**, not a line count.
