# 08 — Sizing, Overflow & Scrolling

**Priority P1.** Depends on [01](01-anchor-and-placement.md).

## Current behaviour

`ToolTipPositioningHelpers.CalculateResponsiveSize` clamps to the smaller of `MaxSize` and 80% of
the screen, with a 120×40 floor:

```csharp
var maxWidth  = Math.Min(maxSize.Width  > 0 ? maxSize.Width  : int.MaxValue, (int)(screenBounds.Width  * 0.8));
var maxHeight = Math.Min(maxSize.Height > 0 ? maxSize.Height : int.MaxValue, (int)(screenBounds.Height * 0.8));
```

Two problems:

1. **It clamps against the whole screen, not the space available on the chosen side.** A tooltip
   placed above an anchor near the top of the screen may have 60px of room and still be sized to 80%
   of the screen height, so it is pushed off-screen or shifted somewhere unhelpful.
2. **Clamping truncates.** Nothing scrolls. Content taller than the clamp is simply cut — there is
   no scrollable body, no "…" affordance, no indication content was lost.

`MaxSize` is also nullable and unset by default, so the practical limit for long text is 80% of the
screen — a full-height tooltip for a long description.

## What the reference systems do

Floating UI's `size` middleware runs after `flip`/`shift` and reports `availableWidth` /
`availableHeight` **for the placement actually chosen**, so the element can:

```js
size({ apply({ availableHeight, elements }) {
  elements.floating.style.maxHeight = `${availableHeight}px`;
}})
```

and the content becomes scrollable within that. Radix pairs this with CSS custom properties
(`--radix-tooltip-content-available-height`).

Desktop suites do the same: DevExpress `SuperToolTip` has `MaxWidth` with word wrap and will scroll
long content rather than clip it.

## Work

1. **Size against available space on the resolved side**, not the screen. After flip/shift, compute
   the room between the anchor edge and the viewport edge, minus offset, arrow and viewport padding,
   and clamp to that.
2. **Sensible default max width.** A tooltip should be readable, so cap around 320–420 logical px
   (Material uses ~320, GitHub hover cards ~360) rather than 80% of a 4K monitor. DPI-scaled.
3. **Scroll instead of clip.** When content exceeds the available height, make the body scrollable
   and paint a scroll affordance. This interacts with [04](04-interactive-hover.md) — a scrollable
   tooltip must be hoverable or it cannot be scrolled.
4. **Re-measure on content change** (async load, text update) and re-run placement + sizing.
5. **Word wrap and minimum width.** Confirm the 120px floor does not force awkward padding on short
   labels; a one-word tooltip should hug its text.

## Verification

- Anchor near the top edge with 60px of space above and a 400px-tall body; assert the tooltip is
  clamped to the available height with a scrollable body, not pushed off-screen.
- Long single-line text; assert it wraps at the max width rather than producing a tooltip wider than
  the monitor.
- Short text; assert the tooltip hugs the content and does not pad out to a 120px minimum that looks
  wrong.
- Repeat at 150%/200% DPI and on a 4K monitor, where the 80%-of-screen rule is most visibly wrong.
