# 02 — Header metrics: alignment and hit targets

All line references are `Helpers/TabHeaderMetrics.cs` unless stated. Every defect below was read
from the code and traced to the call site that consumes it.

## Finding 1 — the close button is not centred in its slot

```csharp
public static Rectangle GetCloseButtonBounds(Rectangle bounds, Control ownerControl)
{
    int closeSlot = CloseButtonSlotWidth(ownerControl);   // 22
    int closeSize = CloseButtonSize(ownerControl);        // 13

    return new Rectangle(
        bounds.Right - closeSlot,                          // x: LEFT edge of the slot
        bounds.Y + (bounds.Height - closeSize) / 2,        // y: centred
        closeSize, closeSize);
}
```

The glyph is centred **vertically** and not **horizontally**. A 13px glyph is placed at the left edge
of a 22px slot, leaving 9px of dead space between the glyph and the tab's right edge. The glyph sits
visibly left of where the reserved slot implies.

The returned rectangle is used unmodified in both places, so nothing downstream corrects it:
- `Helpers/TabPaintHelper.cs:275` → `DrawCloseButton(g, closeRect, …)`
- `Helpers/TabLayoutHelper.cs:379` → returned as the **hit rectangle**

Fix: `bounds.Right - closeSlot + (closeSlot - closeSize) / 2`.

## Finding 2 — the close hit target is 13x13

Because layout returns the *glyph* rect as the hit rect, the clickable area is `CloseButtonSize`
(13px), not `CloseButtonSlotWidth` (22px). 13px is well under the ~24px comfortable minimum, and the
same defect was already fixed in the grid header this cycle by padding hit targets to a
`MinTouchTarget`.

Fix: paint the glyph rect, hit-test the slot rect. They are different rectangles and should be
returned separately rather than one standing in for the other.

## Finding 3 — the badge is drawn inside the close button

```csharp
public static Rectangle GetBadgeBounds(Rectangle tabBounds, int badgeTextWidth, Control ownerControl)
{
    …
    int x = tabBounds.Right - w - DpiScalingHelper.ScaleValue(4, ownerControl);
    int y = tabBounds.Y + DpiScalingHelper.ScaleValue(2, ownerControl);
```

The badge is anchored to `tabBounds.Right`, which is exactly where the close slot lives (the
rightmost 22px). On any closable tab with a badge the two overlap. Nothing offsets the badge by the
close slot, and nothing suppresses one when the other is present.

Fix: anchor the badge to `tabBounds.Right - (CanClose ? CloseButtonSlotWidth : 0)`, and reserve its
width in measurement (see [03](03-measure-draw-contract.md)).

## Finding 4 — `GetTextBounds` reserves the close slot but not the badge

```csharp
Math.Max(0, bounds.Width - (hPad * 2) - closeSlot - iconSlot)
```

Icon and close are reserved; the badge is not. A badge therefore paints over the caption's tail. The
signature has already grown once (`showCloseButton`, then `hasIcon` via a
"backward-compatible overload" at `:62`) — a third boolean is the wrong shape.

Fix: replace the boolean-accretion overloads with one `TabSlotLayout` result type that returns
`IconRect`, `TextRect`, `BadgeRect`, `CloseGlyphRect` and `CloseHitRect` computed together from a
single left-to-right / right-to-left cursor. This is the same shape as `HeaderCellLayout` in the grid
program, which removed exactly this class of disagreement.

The "backward-compatible overload" at `:62` is a legacy shim and goes with it — no fallback.

## Work

- [ ] Introduce `TabSlotLayout` and one method that computes every slot from one cursor
- [ ] Centre the close glyph in its slot; return glyph and hit rects separately
- [ ] Anchor the badge inboard of the close slot
- [ ] Reserve the badge in the text rect
- [ ] Delete the `GetTextBounds(bounds, showCloseButton, ownerControl)` compatibility overload

## Verification

Asserted through the phase-08 harness, each against a controlled baseline:

- close glyph centre-x == close slot centre-x (currently off by `(22-13)/2` = 4–5px scaled)
- close hit rect width >= 24 logical px
- `BadgeRect` does not intersect `CloseHitRect` on a closable tab carrying a badge
- `TextRect` does not intersect `BadgeRect`
- a tab with icon + long caption + badge + close renders all four without overlap, at 100%/150%/200%
