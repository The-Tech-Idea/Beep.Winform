# PHASE 03 — Layout Engine & Live Splitters

**Goal:** One authoritative, recursive, proportional layout engine and **one** live splitter
implementation. Delete the two stubbed splitter subsystems and the conflicting 0–100 ratio.

**Depends on:** 00 · **Blocks:** 02 (target resolution), 04, 05

---

## 3.1 Existing-code disposition (this phase)

| File | Disposition | What changes |
|------|-------------|--------------|
| `Layout/DockingLayoutController.cs` | **Refactor (canonical)** | Sole layout authority. Implement `GetSplitterBounds`/`FindSplitterAtPoint` (currently `TODO`). Expose `CalculateLayout(rootBounds)` → bounds for every group/panel + splitter rects. Add caching + `Invalidate()`. |
| `Layout/LayoutCalculator.cs` | **Reuse** | Pure math (ratio **0.1–0.9**, clamp, splitter bounds). Add min-size clamping. |
| `Layout/LayoutValidator.cs` | **Reuse/Refactor** | Validate tree against canonical model; used by persistence (Phase 06). |
| `Models/DockGroup.cs` (`SplitRatio` 0.1–0.9) | **Reuse** | Canonical ratio. |
| `Runtime/BeepDockSplitter.cs` | **Reuse/Refactor** | The one live splitter. On drag, call `DockingLayoutController.DragSplitter(group, delta)` which updates `DockGroup.SplitRatio` and re-lays out. Paint via `SplitterRenderer` (Phase 01). |
| `Layout/SplitterManager.cs` | **Delete** | All-stub duplicate. |
| `Runtime/SplitterDragHandler.cs` | **Delete** | Stub + 0–100 ratio; superseded. |
| `Runtime/PositioningUtilities.cs` | **Delete** | Only the deleted handler used it; salvage clamp helpers into `LayoutCalculator` if useful. |
| `BeepDockingManager.ApplyLayoutBounds`/`ApplyDockGroupBounds` | **Replace** | Replace the manager's ad-hoc positioning with a single `BeepDockingManager.ApplyLayout()` that consumes the engine result and sets child-control bounds + creates/places `BeepDockSplitter`s. |

> Result: **one** engine (`DockingLayoutController`), **one** math helper
> (`LayoutCalculator`), **one** splitter control (`BeepDockSplitter`), **one** ratio
> convention (0.1–0.9). Three files deleted.

---

## 3.2 Engine contract

```
DockLayoutResult CalculateLayout(Rectangle rootBounds);   // bounds per group & panel + splitter rects
Rectangle        GetPanelBounds(string panelKey);
Rectangle        GetPanelContentBounds(string panelKey);  // minus caption/tabs
DockGroup        FindGroupAtPoint(Point p);
SplitterHit?     FindSplitterAtPoint(Point p);            // implement (was TODO)
void             DragSplitter(DockGroup g, int deltaPx);  // delta → ratio (0.1–0.9), clamp by min sizes
void             Invalidate();                            // mark dirty; recompute on next layout
```

- **Proportional:** each split distributes space by `SplitRatio`, honoring child **min
  sizes** (new `MinWidth/MinHeight` on `DockPanel`/group).
- **Recursive:** groups nest; the engine walks the tree once and caches results until invalidated.
- **DPI-aware:** all sizes scaled; splitter thickness from theme/DPI.

---

## 3.3 Implementation steps

1. Add `MinWidth/MinHeight` to `DockPanel`/`DockGroup`; thread into `LayoutCalculator` clamps.
2. Implement `GetSplitterBounds`/`FindSplitterAtPoint` in the controller using `LayoutCalculator`.
3. Add `DockLayoutResult` (immutable map of id→bounds + splitter rects) + caching/`Invalidate`.
4. `BeepDockingManager.ApplyLayout()`: call engine, set bounds for each `BeepDockspace`/`DockPanel`,
   create/position `BeepDockSplitter` controls between siblings.
5. Wire `BeepDockSplitter` drag → `DockingLayoutController.DragSplitter` → re-`ApplyLayout`.
6. Delete `SplitterManager`, `SplitterDragHandler`, `PositioningUtilities`; remove references.
7. Delete the manager's `ApplyLayoutBounds`/`ApplyDockGroupBounds` ad-hoc path.

---

## 3.4 TODO checklist

- [x] `MinWidth/MinHeight` on panel + group; clamps in `LayoutCalculator` (`ClampSplit`).
- [x] Implement splitter hit-testing in `DockingLayoutController` (`FindSplitterAtPoint` → `DockSplitterHit`, `GetSplitterBounds`).
- [x] `DockLayoutResult` (panel + group bounds + splitter rects) + caching + `Invalidate()`.
- [x] `BeepDockingManager.ApplyLayout()` consumes engine result; positions panels + reconciles splitters (`SyncSplitters`).
- [x] `BeepDockSplitter` drag → engine `DragSplitter(groupId, deltaPx)` (single 0.1–0.9 ratio; incremental delta).
- [x] Delete `SplitterManager`, `SplitterDragHandler`, `PositioningUtilities` (done in Phase 00).
- [x] Remove manager ad-hoc positioning (`ApplyLayoutBounds`/`ApplyDockGroupBounds` now thin shims → `ApplyLayout()`).

> **Note:** The canonical engine now implements a **dock-site edge model** (Top/Bottom span the
> width, then Left/Right span the remaining height, Fill takes the rest) that matches the runtime
> tree (`Root.Children` = one group per edge + Fill), rather than the earlier abstract binary-split
> recursion. Edge size is proportional via `DockGroup.SplitRatio` (seeded once from preferred
> sizes, then owned by splitter drags). Runtime drag-resize behavior should be validated in the
> sample app.

## 3.6 Implemented surface (this phase)

| File | Change |
|------|--------|
| `Models/DockPanel.cs` | Added `MinWidth`/`MinHeight` (designer props). |
| `Models/DockGroup.cs` | Added derived `MinWidth`/`MinHeight`; `RatioInitialized` seed flag. |
| `Layout/LayoutCalculator.cs` | Added `ClampSplit`, `RatioFromDelta`. |
| `Layout/DockLayoutResult.cs` | **New** — immutable panel/group bounds + `DockSplitterHit` list + hit-test. |
| `Layout/DockingLayoutController.cs` | **Rewritten** — dock-site edge layout → `DockLayoutResult`; `CalculateLayout(rootBounds)`, `GetSplitterBounds`, `FindSplitterAtPoint`, `DragSplitter(groupId, deltaPx)`, caching/`Invalidate`. |
| `Runtime/BeepDockSplitter.cs` | Added `Orientation` + `GroupId` for manual (engine-driven) placement; incremental drag delta. |
| `BeepDockingManager.cs` | New `ApplyLayout()` + `SeedEdgeRatios` + `SyncSplitters` + `OnEngineSplitterMoved`; `RecalculateLayout` is a single pass; legacy `ApplyLayoutBounds`/`ApplyDockGroupBounds` shim to `ApplyLayout()`; per-panel splitter creation removed. |

---

## 3.5 Verification criteria

- Resizing the dock-site keeps all groups proportional and within min sizes.
- Dragging any splitter resizes neighbors live and persists the ratio in `DockGroup.SplitRatio`.
- Only `BeepDockSplitter` + `DockingLayoutController` remain for splitter/layout (grep: no
  `SplitterManager`/`SplitterDragHandler`/`PositioningUtilities`).
- Phase 02 target resolution reads group/splitter bounds from this engine.
