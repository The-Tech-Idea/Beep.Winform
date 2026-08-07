# Stage 02 — The selection marker was drawn on the CTA

**Kind:** bug · **Status: done.**

```csharp
_cachedIndicatorRect = new Rectangle(_cachedItemRects[0]...);
if (ctaIndex >= 0 && ctaIndex < _cachedItemRects.Count)
{
    var ctaRect = _cachedItemRects[ctaIndex];
    _cachedIndicatorRect = new Rectangle(ctaRect...);   // overwrites, unconditionally
}
```

`GetIndicatorRect()` computed the indicator from item 0 and then replaced it with the **CTA's**
rectangle whenever a CTA was configured. `BottomBar` seeds its animated indicator from that on the
first paint, so every style reading `AnimatedIndicatorX` put its selection marker in the wrong cell -
Classic, Bubble, Pill, MovableNotch and both FloatingCTA styles.

Visible in the render: the bubble sat on "Add" while "Home" was the selected item, with Home's label
correctly accent-coloured. Three seconds of settling ruled out animation lag.

**The CTA is not a selection.** It is a fixed action button that happens to sit in the row. The
indicator now follows `selectedIndex`, which `EnsureLayout` already received and ignored.

## One rectangle, three callers

The painter band was computed independently in `OnPaint`, `SyncLayoutAndHitTest` and
`StartIndicatorAnimationToSelected` - once through the `PainterInset` constant and twice as a literal
`8`. They agreed only because the numbers matched. Paint and hit-testing disagreeing means clicking
one item and selecting another, so they now share `GetPainterBounds()`.

## `DpiScale` was stored and never acted on

`DpiScale` was an auto-property assigned in `OnPaint` one line before `EnsureLayout`. But
`SyncLayoutAndHitTest` runs first - during construction - computes the layout at the default scale of
1 and clears the dirty flag, so that `EnsureLayout` hit the unchanged-bounds guard and returned. The
74/24/12 grid was never scaled on a scaled display. The setter marks the layout dirty now.
