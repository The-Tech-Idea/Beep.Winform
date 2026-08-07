# Stage 01 — Replacing `Items` threw out of `OnPaint`, twenty times a second

**Kind:** crash · **Status: done.**

The `Items` setter swapped the list, re-hooked `ListChanged`, and called `Invalidate()`. It never
marked the layout dirty - unlike `Items_ListChanged`, which calls `SyncLayoutAndHitTest` and
`ApplyPlacement`.

`EnsureLayout`'s guard is `if (!_dirty && _bounds == bounds) return;`. The item list is not part of
that test and both callers pass the same rectangle, so the cached rectangles still described the
**previous** list. Painters then walk:

```csharp
for (int i = 0; i < rects.Count; i++) { var item = context.Items[i]; ... }
```

A shorter list indexes past the end and throws `ArgumentOutOfRangeException` out of an `OnPaint` with
no `catch`. An exception escaping a paint handler leaves the region invalid, so the next `WM_PAINT`
throws again - and the 50ms ticker re-raised it about twenty times a second. **8 of the 10 painters**
had that loop; only FloatingCTA and MovableNotch escaped. A longer list failed silently instead: the
extra items were never painted and, because `UpdateItems` clamps with `Math.Min`, never hit-testable.

## Fixed at the source, and bounded as well

- The setter now calls `SyncLayoutAndHitTest()` and `ApplyPlacement()`, exactly as a change *within*
  the list does.
- Every painter loop is bounded by `PaintableCount(rects, context)` - `Math.Min` of the rectangle
  count and the item count. The cause is fixed above; this means the same class of mistake can never
  again become a repaint loop.

## Verification

Replacing a five-item list with a two-item one and repainting five times completes without throwing,
and replacing it with an eight-item list still paints. The second half matters because "no exception"
is also what a bar that has stopped painting entirely would report.

This one came from reading, not from looking - the rendering pass never replaced a list.
