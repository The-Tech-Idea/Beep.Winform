# Phase 2 — Collapse the Two Competing Layout Engines

**Goal:** one implementation of node geometry. Today there are two, they disagree, and the dormant
one wakes up on large trees.

---

## The finding

Node rectangles are computed in **two** places:

| | `BeepTree.RecalculateLayoutCache` (`BeepTree.Layout.cs`) | `BeepTreeLayoutHelper.CalculateNodeLayout` (`Helpers/BeepTreeLayoutHelper.cs`) |
|---|---|---|
| Drives rendering | **yes** — writes `_visibleNodes`, which painters consume | no |
| Expander slot | reserved for every node *(after Phase 1)* | always reserved |
| Icon slot | only when `ImagePath` is set — **ragged** | always reserved |
| Text X | `currentX` after real elements | `iconRect.Right + 8` |
| Multi-column | not handled | `CalculateMultiColumnLayout` |
| Virtualization | not handled | `GetVirtualizationRange` + placeholder heights |

`SyncFromVisibleNodes` then **overwrites** the helper's `_layoutCache` with `_visibleNodes`, so the
helper's own geometry is discarded even when it runs.

> **Correction (this document originally got this wrong).** An earlier revision claimed
> `RecalculateLayout()` was "never called by the control", based on a call-site census that grepped
> for `layoutHelper\.[A-Za-z]*`. That pattern silently misses `_layoutHelper?.RecalculateLayout()` —
> the null-conditional `?.` breaks the match. The compiler found the truth when the method was
> deleted: **three live callers**, two of which were actively harmful.
>
> | Caller | What it did |
> |---|---|
> | `BeepTree.Properties.cs` — `ControlStyle` setter | called `RecalculateLayoutCache()` and then **immediately** `_layoutHelper?.RecalculateLayout()`, replacing the geometry just computed with the divergent one |
> | `BeepTree.Properties.cs` — `UseFormStylePaint` setter | identical pattern |
> | `BaseTreePainter.cs` | paint-time fallback when the cache is empty — so a frame arriving with an empty cache drew the tree 4px differently from the next frame |
>
> So the divergence was not latent at all: changing `ControlStyle` or `UseFormStylePaint` at runtime
> installed the wrong geometry, every time. The lesson is the one this repo keeps relearning —
> a grep is not a call-site census; deleting the symbol and letting the compiler answer is.

**Measured divergence.** Running both engines over the same tree and comparing text X per node:

```
Root (parent): control textX=42 vs helper textX=46
L1 parent:     control textX=58 vs helper textX=62
L2 parent:     control textX=74 vs helper textX=78
...            (all 7 nodes, consistently 4px apart)
```

## Why this is worse than ordinary dead code

`RecalculateLayoutAsync()` also calls it, and it is triggered automatically:

```csharp
if (EnableBackgroundLayout && _visibleNodes.Count > 10000)
    _layoutHelper.RecalculateLayoutAsync();
```

So a tree that crosses 10,000 visible nodes runs a **different geometry engine** with different
indent rules on a background thread. Alignment can change based on node count. It also mutates the
layout cache off the UI thread while paint reads it.

Multi-column has the same shape: `IsMultiColumn` is a supported mode, but the only implementation of
`CalculateMultiColumnLayout` lives on the dead path, so column rectangles are never produced for
rendering.

## Work items

1. **Choose the survivor.** `RecalculateLayoutCache` is the live one and now has the correct slot
   rules; make it the single engine.
2. **Move it into the helper** so the control keeps orchestration and the helper keeps geometry,
   then have `RecalculateLayoutCache` delegate. Do not leave two bodies behind.
3. **Port the two capabilities that only exist on the dead path** — virtualization range and
   multi-column cell rects — into the survivor. Do not delete them; they are the only
   implementations.
4. **Fix the multi-column first-column rect** while porting. It currently shrinks the column instead
   of insetting content:
   ```csharp
   cellRect = new Rectangle(baseIndent, nodeInfo.Y, column.Width - baseIndent, rowHeight);
   ```
   A deep node gets a narrower first column than a shallow one, so column edges do not line up.
   The cell should stay `column.Width` wide with the *content* indented inside it.
5. **Delete `RecalculateLayoutAsync` or make it safe.** Either drop it, or have it compute into a
   detached list and publish to the UI thread — never mutate the shared cache from a worker.
6. **`SyncFromVisibleNodes` becomes unnecessary** once there is one engine writing one cache; remove
   the copy rather than keeping an alias.

## Risks

- Multi-column and virtualization are untested on the live path — porting will surface latent bugs.
  Land Phase 1's probe assertions first so regressions are visible.
- `NodeInfo` is a **struct**; the existing code is careful to write modified copies back
  (`_visibleNodes[i] = nodeInfo`). Any refactor must preserve that or changes silently vanish.

## Exit criteria

- [x] Exactly one method computes node rectangles; the duplicate is deleted, not commented out.
      `BeepTree.RecalculateLayoutCache` survives; `BeepTreeLayoutHelper` keeps the cache, viewport
      range, transforms and measurement. `RecalculateLayout()` remains as a thin delegate because
      three callers legitimately need "recompute now and hand me the layout"
- [x] Multi-column layout ported to the surviving path as `BeepTree.CalculateMultiColumnCells`.
      This was not just a tidy-up: `BaseTreePainter` and `BeepTreeCellEditor` both read
      `GetCellRect(colIndex)`, and the only code that ever called `SetCellRect` was on the dead
      path — so **every cell rect was `Rectangle.Empty` and multi-column mode rendered no columns**
- [x] First-column rect keeps full column width with content indented. The old
      `column.Width - baseIndent` shrank column 0 by each node's own depth, so column edges did not
      line up down the tree
- [x] No layout mutation off the UI thread — the `> 10000` node `RecalculateLayoutAsync` trigger is
      gone along with the method
- [x] `GetVirtualizationRange` takes a count instead of a freshly materialised
      `List<SimpleItem>` of the whole tree on every scroll
- [x] Probe confirms "engines agree on geometry" and all Phase 1 alignment assertions still pass
- [ ] Multi-column rendering verified by eye now that cell rects exist — porting made the geometry
      real, but nothing has yet *looked* at a multi-column tree
- [ ] `VirtualizeLayout` is nominal: the control lays out every visible node eagerly, so the
      viewport range is bookkeeping and nothing is actually virtualized. Decide whether to implement
      it or retire the property
