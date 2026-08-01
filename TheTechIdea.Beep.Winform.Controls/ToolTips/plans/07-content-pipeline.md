# 07 — Content Pipeline & Layout Variants

**Priority P1.**

## Current behaviour

### `ToolTipLayoutVariant` is largely decorative

Seven values are declared — `Simple`, `Rich`, `Card`, `Preview`, `Tour`, `Shortcut`, `Glass`.
`ToolTipPainterFactory` maps three:

```csharp
ToolTipLayoutVariant.Preview  => new PreviewToolTipPainter(),
ToolTipLayoutVariant.Tour     => new TourToolTipPainter(),
ToolTipLayoutVariant.Glass    => new GlassToolTipPainter(),
_                             => new BeepStyledToolTipPainter()
```

`Simple`, `Rich`, `Card` and `Shortcut` all fall to `BeepStyledToolTipPainter`, and that painter —
731 lines — never references `LayoutVariant` at all. Layout is therefore driven implicitly by which
fields happen to be populated (a `Title` makes it look "Rich", a `Shortcuts` list makes it look
"Shortcut"), and setting the enum has no effect.

Either the enum should drive layout or it should not exist; today it misleads.

### Async content is declared and unimplemented

`ToolTipConfig.LoadPreviewAsync`:

> *Optional async delegate for lazy-loading the preview image. If supplied, a skeleton placeholder
> is shown until the task completes.*

Zero references anywhere in the assembly. There is no skeleton and no await.

This matters because async content is the main reason modern tooltips exist at all — GitHub hover
cards, Slack profile previews, IDE symbol documentation. A synchronous-only tooltip cannot show
anything that needs fetching.

### Markup

`ToolTipMarkupParser` is real and reasonable — it parses `**bold**`, `*italic*`, `` `code` `` and
`[label](target)` into typed `MarkupSpan`s. Gated behind `UseMarkup` (default `false`). Links
produce `SpanKind.Link` with a target, but nothing hit-tests or raises a click
(see [04](04-interactive-hover.md)).

## What the reference systems do

- **Explicit layout templates.** MUI's rich tooltip, Ant Design's `Popover` with `title` + `content`
  slots, DevExpress `SuperToolTip` with typed `ToolTipTitleItem` / `ToolTipItem` / `ToolTipSeparator`.
  The structure is declared, not inferred from which properties are non-null.
- **Async with a loading state.** Radix/Tippy render a spinner or skeleton and reposition once the
  real content measures — because the size changes, which is why this is coupled to
  [03](03-auto-update.md) and [08](08-sizing-and-overflow.md).
- **Content as a component.** Any element can be the tooltip body.

## Work

1. **Make `LayoutVariant` authoritative.** Either give `Rich`, `Card` and `Shortcut` their own
   painters, or have `BeepStyledToolTipPainter` branch on the variant explicitly. Keep field-driven
   inference only as the resolution for `Simple`.
2. **Implement `LoadPreviewAsync`** with a real skeleton state: show the tooltip immediately at a
   placeholder size, await the delegate, then swap content and **re-run positioning** because the
   size changed. Cancel the await if the tooltip hides first, and never marshal the result onto a
   disposed window.
3. **Generalise it** beyond preview images — `Func<CancellationToken, Task<ToolTipContent>>` so any
   tooltip can populate asynchronously, with the preview case as one use.
4. **Wire link clicks** — hit-test `SpanKind.Link` spans, raise `LinkClicked(target)`, and show a
   hand cursor over them.
5. **Custom content host.** An escape hatch to place an arbitrary `Control` inside the tooltip body
   for cases the painters do not cover — this is what makes a tooltip framework general rather than
   a fixed set of templates.
6. **Measurement lives in one place.** Confirm every painter measures with the same font and flags
   it draws with; the identical defect cost the tree control clipped labels in all 25 painters
   (`Trees/plans/correctness/phase-3-text-metrics.md`).

## Verification

- Set each of the 7 `LayoutVariant` values with identical content and render all 7 — assert they are
  visually distinct, in one contact sheet.
- Supply a `LoadPreviewAsync` that delays 500ms; assert a skeleton shows, then real content, and that
  the tooltip repositions after the size change.
- Hide the tooltip while the async load is in flight; assert no exception and no ghost window.
- Click a link span; assert `LinkClicked` fires with the correct target.
