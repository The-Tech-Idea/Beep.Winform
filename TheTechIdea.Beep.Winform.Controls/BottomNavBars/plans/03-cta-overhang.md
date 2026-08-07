# Stage 03 — CTA shapes were clipped by the control's top edge

**Kind:** bug · **Status: done.**

The floating-CTA family centres a circle on the bar's top edge, so about half of it falls outside.
The control's bounds ended 8px above the band, so the circle rendered flat-topped and the diamond
rendered as a triangle. Reference designs 2, 3 and 4 all show the shape whole, protruding.

## Two paths, chosen by the parent

**WinForms has no real transparency**, which rules out simply drawing outside the bounds.

| parent | what happens |
|---|---|
| implements `IExternalDrawingProvider` (`BaseControl` containers, `BeepiFormPro`) | the protruding part is drawn on the **parent's own surface**; the control stays exactly `BarHeight` |
| anything else (plain `Panel`, `Form`) | the control reserves headroom above the band, and `IsChild` fills it with the parent's back colour |

The external path is preferred and is the one that is correct over a gradient, an image, or another
control. `IsChild` samples a **single** colour, so the fallback's headroom is a flat rectangle - fine
over a flat parent, wrong over anything else.

**The fallback cannot be dropped.** `UpdateExternalDrawing` early-returns unless the parent implements
the provider, and only two types do. On a plain `Panel` the alternative to reserved headroom is not a
clipped CTA, it is no CTA at all.

## The overhang is derived, not guessed

Each painter reports what it needs from its own geometry:

```csharp
int GetTopOverhang(int contentHeight);   // 0 for flat styles
```

`radius = (contentHeight / 2 + 6) * scale`, centre `contentHeight / 2 - 10` below the band top, so the
overhang is `radius - (contentHeight / 2 - 10)`. The halo styles use the 1.35x ring factor. It stays
correct when the bar is resized, which a constant would not.

Measured at `BarHeight = 64`: FloatingCTA and OutlineFloatingCTA 90px, Diamond and MovableNotch 81px,
the other six unchanged at 64.

## The external half draws with the same code

The handler calls the control's own `PaintBar`, translated onto the parent and **clipped to the strip
above the control**. The bottom half is painted on the control and the top half on the parent, by
identical code - so the two cannot drift. The alternative, a separate "draw just the CTA" routine in
each of the four painters, would have been four more places to keep in step.

## What I got wrong first

The first attempt took the headroom **out of** the band rather than growing the control. At any
ordinary bar height the CTA wants ~27px and a 48px band cannot spare it, so a `MinBandHeight` guard
silently skipped the overhang and nothing changed at all.

I also described the reserved headroom as "transparent". It is not - `BaseControl._isChild` defaults
to `true`, so the bar resolves the parent's back colour on every paint and the constructor's
`BackColor = Color.White` is overwritten. That is why the headroom renders as the parent colour rather
than a white block, and it is a colour *copy*, not transparency.

## Verification

Same style, same `BarHeight`, two parents - the check discriminates rather than asserting one number:

```
PASS  plain parent reserves headroom:        height 90 vs bar 64
PASS  provider parent draws outside instead: height 64 vs bar 64
```

Plus the rendered shapes: the circle is round and the diamond is a diamond, on a deliberately
strong-coloured parent so a mis-filled headroom would be obvious.
