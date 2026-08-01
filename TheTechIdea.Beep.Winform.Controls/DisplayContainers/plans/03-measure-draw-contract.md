# 03 — One geometry source: measure what you draw

## The rule this phase enforces

A tab's width is decided in `TabLayoutHelper.CalculateTabContentWidth`; its contents are placed by
`TabHeaderMetrics`; its pixels are drawn by `TabPaintHelper`. Whenever those three disagree about
what occupies a tab, the result is clipped text, premature ellipsis, or overlapping glyphs.

This is the defect family that cost the most across the preceding programs — measure-with-one-font /
draw-with-another in the tabs painters, and measure-at-one-width / draw-at-another in the grid header.

## Finding 1 — the badge is drawn but never measured

`TabLayoutHelper.cs:154-161`:

```csharp
int closeButtonWidth = tab.CanClose ? TabHeaderMetrics.CloseButtonSlotWidth(OwnerControl) : 0;
int internalPadding = TabHeaderMetrics.TextContentPadding(OwnerControl);
int iconWidth = !string.IsNullOrEmpty(tab.IconPath) ? TabHeaderMetrics.IconSlotWidth(OwnerControl) : 0;

int contentWidth = iconWidth + textWidth + closeButtonWidth + internalPadding;
```

Icon, close and padding are measured. **The badge is not.** `TabPaintHelper` draws one anyway
(`:356-368`), so a badged tab is drawn wider than it was measured — the badge takes space the layout
never granted, over the caption and the close button.

## Finding 2 — text flags agree on the live path; they do not on the fallback

Measurement (`TabLayoutHelper.cs:142`) uses `NoPadding | SingleLine`.

The live draw (`TabPaintHelper.cs:695-699`) uses `Left | VerticalCenter | EndEllipsis | SingleLine |
NoPadding`. **These agree** — `NoPadding` on both sides. This was checked specifically and is *not* a
defect; it is recorded so a later reader does not "fix" it.

The fallback draw inside a `catch` (`TabPaintHelper.cs:311-313`) uses `HorizontalCenter |
VerticalCenter | EndEllipsis | SingleLine` — no `NoPadding`, and centred rather than left-aligned. So
the moment the real painter throws, tabs silently change alignment *and* start ellipsising early.
That path is dealt with in [06](06-painting-and-state.md); it is named here because it is the second
half of the measure/draw contract.

## Finding 3 — measurement swallows its own failure

`TabLayoutHelper.cs:144-151`:

```csharp
catch
{
    textWidth = tab.Title.Length * DpiScalingHelper.ScaleValue(7, OwnerControl);
}
```

A bare `catch` replacing a real measurement with `length x 7px`. If the font is disposed or invalid,
every tab silently gets a fabricated width instead of a diagnosable error — and 7px/char is wrong for
any non-monospace font, so the strip quietly mis-sizes. Covered by the ground rule in
[07](07-exception-policy.md).

## Finding 4 — pinned tabs bypass the content contract

`TabLayoutHelper.cs:134-135`:

```csharp
if (tab.IsPinned)
    return TabHeaderMetrics.PinnedTabWidth(OwnerControl) + chromeWidth;   // 38 + chrome
```

A pinned tab returns a fixed 38px regardless of whether it carries a badge or a close button. If
pinned tabs are icon-only by design that is correct — but `TabPaintHelper` is not told, so it may
still draw a caption, badge or close glyph into 38px. Decide the contract once: pinned means
icon-only, and the painter must honour it.

## Work

- [ ] Add badge width to `CalculateTabContentWidth`, measured with the same font and flags the
      painter uses
- [ ] Have measurement and placement share one `TabSlotLayout` (see [02](02-header-metrics-and-alignment.md))
      so width and placement cannot drift apart
- [ ] Make the pinned-tab contract explicit and enforce it in the painter
- [ ] Remove the fabricated `length x 7px` estimate

## Verification

- For a matrix of tabs (plain / icon / badge / close / pinned / all), assert
  `sum(slot widths) <= measured tab width` — the measured width must contain everything drawn
- Assert `TextRect.Width > 0` for every case: a tab whose reserved slots exceed its width currently
  yields a zero-width caption rather than a wider tab
- Render a badged tab and confirm, against a baseline of the same tab without a badge, that the
  caption is not overdrawn
