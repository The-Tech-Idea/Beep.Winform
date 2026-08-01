# 02 — Arrow Tracking

**Priority P0.** Depends on [01](01-anchor-and-placement.md).

## Current behaviour

The arrow is drawn at the centre of the tooltip edge. When the tooltip is shifted to stay on screen,
the arrow does not move with the target — it keeps pointing at wherever the tooltip's own centre
now is, which may be nowhere near the control.

The code says so itself, in `ToolTipPositioningHelpers.CalculatePositionWithArrow`:

```csharp
// If we had to adjust horizontally, we might need to adjust arrow position
// For now, just ensure tooltip stays on screen
```

`ToolTipConfig.ArrowOffset` exists for exactly this purpose —

> *Pixel offset of arrow tip from the center of the tooltip edge. Positive moves toward End
> alignment; negative toward Start.*

— and nothing computes it. It is only ever read by the painter, so a caller can set it manually, but
the positioning code never derives it.

The visible result: a tooltip on a control near the right edge of the screen slides left to fit, and
its arrow ends up pointing at empty space several tens of pixels away from the control.

## What the reference systems do

Floating UI's `arrow` middleware runs *after* `shift` and reports the offset needed to keep the
arrow centred on the anchor:

```
arrowX = anchorCenterX - floatingX - arrowWidth / 2
```

clamped so the arrow stays within the floating element's rounded corners — an arrow must never
overlap a corner radius or it renders as a notch in the border.

## Work

1. **Compute `ArrowOffset` as part of positioning**, not as a caller-supplied value. After the shift
   step, derive the offset from the anchor centre and the final tooltip rect, and write it into the
   resolved layout that the painter reads.
2. **Clamp to the safe span.** The arrow's centre must stay within
   `[cornerRadius + arrowHalfWidth, edgeLength - cornerRadius - arrowHalfWidth]`. Beyond that the
   tooltip is so far from its anchor that the arrow should be **hidden** entirely rather than pinned
   to a corner — this is what Tippy does, and it reads better than a misleading arrow.
3. **DPI-scale `ArrowSize`.** The config XML comment claims *"DPI-scaled at paint time"*; confirm
   that against `ToolTipArrowPainter` and make it true if it is not.
4. **Arrow colour must follow the resolved surface**, including the glass painter's translucent
   fill and any border — an arrow drawn in the base background over a gradient surface shows as a
   visible seam.

## Verification

- Anchor a tooltip to a control 10px from the right screen edge. Assert the arrow tip's screen x is
  within 1px of the anchor's centre x, and confirm by render.
- Walk an anchor across the full width of a monitor in 20px steps and assert the arrow tip tracks
  the anchor centre until the clamp engages, then that the arrow is hidden rather than parked in a
  corner.
- Repeat at 150% and 200% DPI — arrow size and clamp are both in pixels and are the kind of thing
  that silently breaks when scaled.
