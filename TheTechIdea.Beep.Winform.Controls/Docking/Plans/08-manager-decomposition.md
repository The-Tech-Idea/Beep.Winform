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

## Outcome (partial)

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
