# 01 — Anchor Rect & Placement Engine

**Priority P0.** Everything else in this program sits on top of placement.

## Current behaviour

### The anchor is one pixel

`CustomToolTip.Positioning.cs`:

```csharp
var targetRect = new Rectangle(targetPosition, new Size(1, 1));
var placement = ToolTipPositioningHelpers.CalculateOptimalPlacement(
    targetRect, tooltipSize, _config.Placement, _config.Offset);
```

Both the `Auto` and the explicit-placement paths build a 1×1 rectangle from a *point*.

The alignment variants are defined against the anchor's edges — `CalculateBoundsForPlacement` uses
`targetRect.Left` for `TopStart`, `targetRect.Right - tooltipSize.Width` for `TopEnd`, and
`targetRect.Left + (targetRect.Width - tooltipSize.Width) / 2` for `Top`. With `Width = 1` those
three expressions differ by at most one pixel from each other's centre term. **`TopStart`, `Top` and
`TopEnd` are effectively the same placement**, and the same holds for every other family.

It also means the tooltip is positioned relative to the cursor or an arbitrary point rather than to
the control, so it can overlap the control it describes.

### Two implementations of the same maths

| | `ToolTipPositioningHelpers.CalculateBoundsForPlacement` | `CustomToolTip.AdjustPositionForPlacement` |
|---|---|---|
| Input | target **rect** | target **point** |
| Offset | `offset` | `offset` **+ `arrowSize`** |
| Centring | `targetRect.Left + (targetRect.Width - w)/2` | `targetPosition.X - Width/2` |
| Used by | `CalculateOptimalPlacement`, `FindBestPlacement` | the actual show path |

So placement is *chosen* by one implementation and *applied* by another that offsets differently.
The chosen placement can therefore be validated as fitting and then applied at a position that does
not fit. This is the duplicate-engine defect already fixed in `BeepTree` — see
`Trees/plans/correctness/phase-2-single-layout-engine.md`.

### Placement selection scores all twelve candidates

`CalculateOptimalPlacement` loops all twelve placements and scores each by visible area, distance
from target centre, and a +50 bonus for being below the target. A requested placement is honoured
only if it is *fully* visible; otherwise the scoring may pick any side.

That is not how the reference systems behave. A tooltip requested `Top` that is clipped by 3px
should slide 3px, not jump to `Right`.

## What the reference systems do

Floating UI (used by Radix, Tippy, MUI's Popper successor) composes ordered middleware:

| Middleware | Behaviour |
|---|---|
| `offset` | gap between anchor and floating element |
| `flip` | if the preferred side does not fit, try the **opposite** side — not all sides |
| `shift` | slide *along* the placement axis to stay in view, keeping the side |
| `arrow` | report how far the anchor centre is from the floating element centre |
| `size` | expose available width/height so the element can clamp and scroll |
| `hide` | report when the anchor is clipped or off-screen |

Order matters: `flip` before `shift` means a tooltip prefers the opposite side over sliding, but
slides rather than flipping to an unrelated side.

## Work

1. **Take a `Rectangle` anchor through the whole path.** `ToolTipConfig` gains an
   `AnchorRect` (screen coordinates) alongside the existing `Position`; when a tooltip is attached
   to a control, the anchor is `control.RectangleToScreen(control.ClientRectangle)`. `Position`
   remains for cursor-anchored and `FollowCursor` cases, expressed as a degenerate rect so there is
   one code path.
2. **Delete `CustomToolTip.AdjustPositionForPlacement`.** The helper becomes the single
   implementation. Fold the arrow-size term into the helper's `offset` so the geometry it validates
   is the geometry that gets applied.
3. **Replace scoring with ordered middleware**: `offset → flip → shift → arrow → size → hide`,
   each a small pure function over `(anchorRect, floatingSize, screenBounds)`. Keep the scorer only
   for `Placement.Auto`, where "pick the side with the most room" is genuinely the right rule.
4. **Respect an explicit placement.** An explicit request flips only to its opposite, then shifts;
   it never silently becomes an unrelated side.
5. **`ScreenEdgePadding` becomes configurable** (`ToolTipConfig.ViewportPadding`, default 8) and
   DPI-scaled — it is currently a hard-coded `const int` and is not scaled.

## Verification

- Anchor a tooltip to a 200×40 button and assert `TopStart`, `Top`, `TopEnd` produce three
  *different* x positions that align to the button's left edge, centre and right edge.
- Place the anchor 3px from each screen edge, in turn, and assert the tooltip keeps the requested
  side and shifts along it rather than flipping to another side.
- Place the anchor in a corner with no room on the requested side and assert it flips to the
  opposite side.
- Render each case and look at it. Rect assertions alone missed the BeepTree clipping bug for
  months.
