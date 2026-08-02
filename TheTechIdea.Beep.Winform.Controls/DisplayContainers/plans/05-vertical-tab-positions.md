# 05 — Left/Right tab positions

`TabPosition` offers `Top`, `Bottom`, `Left`, `Right`. The first two work. The second two are laid
out but not drawable in any readable way.

## Finding 1 — the measured text width becomes the tab's height

`Helpers/TabLayoutHelper.cs:192-200`, the non-horizontal branch:

```csharp
tab.Bounds = new Rectangle(
    tabArea.X,
    currentY,
    tabArea.Width,   // width  = the strip's width
    tabWidth         // height = the measured CONTENT WIDTH
);
```

`tabWidth` came from `CalculateTabContentWidth` — a horizontal measurement of
`icon + text + close + padding`. For a Left/Right strip that value is applied as the tab's **height**.

So a tab captioned "Customer Transactions" becomes tall in proportion to how wide its caption would
have been horizontally, which is not a meaningful vertical size.

## Finding 2 — the caption is then placed against the strip width

`TabHeaderMetrics.GetTextBounds` computes the caption rect from `bounds.Width`, which for a vertical
tab is the strip's width — typically 120–200px, and reduced further by `hPad*2 + closeSlot +
iconSlot`. The caption is drawn horizontally into that narrow box and ellipsises almost immediately.

The measurement that decided the tab's size and the rect the text is drawn into are about two
different axes.

## Finding 3 — there is no rotation anywhere

`Helpers/TabPaintHelper.cs` contains no `RotateTransform` and no `Graphics.DrawString`; all captions
go through `TextRenderer.DrawText`. The `TabPosition.Left` / `TabPosition.Right` cases that do exist
(`:383`, `:423`, `:498`) handle **indicator and chrome geometry only** — they place the active-tab
indicator on the correct edge. None of them rotate a caption.

This matters for the implementation, not just the diagnosis: **`TextRenderer.DrawText` is GDI and
ignores the `Graphics` world transform.** A `RotateTransform` around a `TextRenderer` call draws
unrotated text. Rotated captions must use `Graphics.DrawString`, which is GDI+ and honours the
transform. This exact trap was hit and fixed in the BeepTabs program; the fix there
(`DrawTextInBounds` taking an `isHorizontal` flag and switching to GDI+ when rotated) is the
reference implementation.

## Decision required

Three defensible options, and the choice changes the work substantially:

1. **Rotate captions** (VS "vertical document tabs", Firefox vertical tabs). Measure along the
   vertical axis, draw with `DrawString` under a 90° transform. Most faithful, most work.
2. **Icon-only vertical tabs** (VS Code's activity bar). Vertical strips show icons with tooltips;
   captions never appear. Cheapest, and arguably the better UX at typical strip widths.
3. **Horizontal captions in a wide vertical strip** (Visual Studio's docked tool-window tabs).
   Keep horizontal text but measure the caption against the strip width and size tab height from the
   font, not from a horizontal text measurement.

Option 3 is the smallest correct fix and matches how the strip is already shaped. Option 2 is the
best UX if vertical strips are meant to be narrow. **Option 1 should not be chosen by default** —
it is the most work and the least used pattern in this codebase's reference products.

## Work

- [ ] Choose the vertical model above
- [ ] Split measurement by axis: horizontal strips measure width, vertical strips measure height
- [ ] Make `GetTextBounds` axis-aware rather than assuming `bounds.Width` is the text axis
- [ ] If rotation is chosen, route captions through `Graphics.DrawString` — never `TextRenderer`
- [ ] If icon-only is chosen, suppress caption/badge measurement for vertical strips entirely

## Verification

- Render Left and Right strips with short, long and CJK captions
- Assert the caption is legible: compare rendered text-band count against the same caption drawn in a
  Top strip. A vertical tab showing fewer glyph bands than its horizontal equivalent is clipping
- Assert tab height in a vertical strip does not vary with caption *width* once fixed
- Confirm the active indicator still lands on the inner edge for both Left and Right

---

## Outcome — option 2 (icon rail), not option 3

The plan recommended **option 3** (horizontal captions in a wide vertical strip) as the smallest
correct fix. Measuring the container before implementing changed that answer.

`BeepDisplayContainer2.Layout.cs:82-89` builds a Left/Right strip as:

```csharp
case TabPosition.Left:
    _tabArea = new Rectangle(0, 0, effectiveTabHeight, Height);
```

The strip's **width** is `effectiveTabHeight` — one line of text plus chrome, so roughly 32–40px.
Option 3 assumed a strip wide enough for a horizontal caption; there is no such strip. Choosing it
would have meant widening the rail to ~160px and reflowing `_contentArea` — a far larger change than
"smallest correct fix", and a different product decision rather than a defect repair.

At ~36px the strip already *is* an icon rail, which is what VS Code uses at that width. Option 2 was
therefore implemented:

- `GetSlotLayout`'s `isPinned` parameter became `iconOnly`, covering pinned tabs **and** every tab in
  a Left/Right strip. Caption, badge and close glyph are suppressed there rather than crammed in.
- `CalculateTabWidths` is axis-aware: horizontal strips measure caption width, vertical strips return
  a uniform square the width of the rail.
- The shrink pass applies only to horizontal strips — uniform icon rows have nothing to give up.

### Measured

| | before | after |
|---|---|---|
| tab entry, caption "A" | 36 x 36 | 36 x 36 |
| tab entry, caption "Customer Transactions Ledger" | 36 x **204** | 36 x 36 |

204px was the caption's *horizontal* width applied as a *vertical* height. Vertical tab extent no
longer tracks caption width at all.

### Still open

Icon-only tabs need tooltips to be usable — a rail of unlabelled icons with no hover text is not
navigable. That belongs with the state/affordance work in [06](06-painting-and-state.md).
