# Stage 03 — A badge cannot be wider than tall

**Kind:** bug · **Files:** `BeepFloatingBadge.cs`, `Builtin/BeepTextBadge.cs`, `Builtin/BeepCounterBadge.cs`

Measured: **`new BeepTextBadge("NEW")` is 18×18 pixels, with `Shape = BadgeShape.Pill`.**

A pill that is exactly as wide as it is tall is a circle. The shape exists to hold a word, and the
control cannot be that shape.

**Status: done.** `BeepTextBadge("NEW")` is now **50×18**; `"IN PROGRESS"` is **113px** wide.

`BadgeDiameter` is the height. Width is `max(height, measuredContent + padding)` for `Pill`,
`RoundedSquare` and `Rectangle`; `Circle` and `Diamond` stay square by definition — verified, a circle
with `"999"` is still 22×22, because silently turning one shape into another is worse than clipping.
`MaximumSize` caps height only.

Measuring lives in the two badges that own text, through a `MeasureContentWidth` override, because only
they know what they are about to draw.

**`BeepCounterBadge`'s font stepping is now a taper, not a defence.** It dropped to 35% of badge height
for 3-character labels — about 6pt on an 18px badge — because the badge could not grow. The floor is
45% now and the badge widens instead.

**`ApplyCornerOverlap` was rewritten to read the anchor.** It had been inferring the corner from where
the computed bounds landed relative to the target's centre, which was **wrong for the middle anchors**:
a `MiddleLeft` badge sits above the target's vertical centre, so the inference read "top" and pulled it
to the top edge. The two empty `if` blocks with comments claiming middle anchors are not shifted were
the previous attempt at this. Verified: `MiddleLeft` now stays vertically centred (both centres at
y=170) and overhangs the left edge.

## Why

Three things conspire, all in the base class:

```csharp
public int BadgeDiameter
{
    set
    {
        _badgeDiameter = Math.Max(8, Math.Min(48, value));
        Size = new Size(_badgeDiameter, _badgeDiameter);   // forces square
        ...
    }
}
```

- **`BadgeDiameter` forces `Size` square.** One number drives both axes, so width can never differ from
  height. The name says "diameter", which is honest about the assumption — a circle — and wrong for
  four of the six shapes the enum offers.
- **`MaximumSize = new Size(48, 48)`** in the constructor caps both axes at 48px. Even if a caller sets
  `Width` directly, WinForms clamps it back.
- **Nothing measures the text.** `BeepTextBadge.DrawBadgeContent` picks a font size from
  `contentBounds.Height * 0.5f` and draws into the square it was given. Long text is simply squeezed —
  `DrawString` with a centred `StringFormat` into a box too small for it.

`BeepCounterBadge` half-acknowledges the problem: it steps the font down for 2- and 3-character labels
(`0.55f`, `0.45f`, `0.35f` of height) instead of widening. So "99+" renders at 35% of an 18px badge —
about 6pt — rather than in a pill wide enough to hold it. `MaxDisplay`/`ShowPlus` exist precisely
because the badge cannot grow, which is a workaround for this defect rather than a feature.

## What it should be

**Height stays driven by one number; width is measured from the content, floored at height.**

- Keep `BadgeDiameter` as the *height* and the width of circular badges, and deprecate the name in
  favour of something axis-neutral (`BadgeSize` or `BadgeHeight`), keeping the old one delegating.
- For `Pill`, `RoundedSquare` and `Rectangle`, compute width as
  `max(height, measuredTextWidth + horizontalPadding)`.
- Circle and Diamond stay square by definition — a circle with a long label should either grow as a
  circle or the caller should have chosen a pill. Do not silently change a caller's shape.
- Raise or remove `MaximumSize`. A cap of 48 on the *height* of a decoration is defensible; the same
  cap on width makes "IN PROGRESS" impossible. If a cap stays, it belongs on the axis it makes sense on.

Measuring belongs where the text is known — in the two badges that own text — so the base class exposes
the ability to be non-square and does not itself measure.

## `CornerOverlap` interacts with this

`ApplyCornerOverlap` centres the badge on the target's corner using `currentBounds.Width / 2`. It
already reads width and height separately, so a wider badge will overhang correctly. **No change
needed — but it is the reason to verify with a wide badge**, since every existing test of that code
path used a square one.

## `ApplyCornerOverlap` has two blocks that do nothing

```csharp
if (currentBounds.Left + halfW == targetBounds.Left + targetBounds.Width / 2)
{
    // Centered horizontally — don't shift X.
}
```

Both `if` bodies are empty. The comments describe an intent — that middle anchors should not be shifted
— that the code does not carry out, because the shift already happened in the lines above and nothing
undoes it. So `MiddleLeft`, `MiddleRight` and `MiddleCenter` **are** shifted by half the badge, and the
comment says they are not.

Decide which is right and make the code say it. Either the middle anchors should be exempt from corner
overlap (then the exemption has to actually run, before the shift) or they should not (then delete both
blocks). Leaving a comment that contradicts the behaviour is the worst of the three.

## Verification

1. **`new BeepTextBadge("NEW")` is wider than it is tall.** *Today it is 18×18* — that is the failing
   run, and it is the whole stage in one assertion.
2. **The text is not clipped.** Measure the string at the rendered font and assert it fits inside the
   content bounds with padding to spare. A badge that grew but still clips has not been fixed.
3. **`BeepCounterBadge` with "99+" renders at a legible size** — assert the font is not below some
   floor relative to badge height, rather than asserting an exact size.
4. **A circle stays square.** Guards against a fix that makes every shape grow.
5. **A wide badge still overhangs its corner correctly** with `CornerOverlap = true` — the case that
   has never been exercised.
6. **Middle anchors land where the code claims.** Whichever way the decision above goes, assert it.
