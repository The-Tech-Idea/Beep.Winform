# 02 — Painter distinctness

## The question this phase answers

`FilterStyle` has 8 values, `Painters/` has 8 implementations, and `FilterPainterFactory` maps every
one of them. On the structure, this is complete — unlike [01](01-dead-configuration-surface.md),
there is no missing wire here.

What is **not** established is that the eight produce eight different surfaces.

## Why it is not assumed

The BeepTabs program found seven painters whose visually distinct code produced **pixel-identical
output**. The GridX program found twelve header painters where the only style-driven difference was
the band *height* — where two styles shared a height, the headers were byte-identical. In the
DisplayContainers program, `active+hover` rendered **0.00%** different from `active` because one
branch consumed a flag without reading it.

Three folders, three times the same shape. Reading an implementation is not evidence that it renders
differently, and the structural signal here is weak in the same way it was there: every painter
overrides exactly 5 members against a base offering 6 virtuals — `TagPillsFilterPainter` is the only
one at 7.

That uniformity may mean the base does the right thing and each subclass changes what matters. It may
also mean seven painters inherit most of their appearance. **Pixels decide.**

## Work

- [ ] Render each `FilterStyle` at a fixed size with identical criteria and theme
- [ ] Compare pairwise. Any two rendering identically is a defect, not a coincidence
- [ ] For any pair that collapses, determine which of the two is wrong before changing either —
      in DisplayContainers the collapse was in the *colour resolution*, not in the painter
- [ ] Confirm `SupportsAnimations` and `SupportsDragDrop` are read by something. Two booleans on the
      interface that nothing consults would be the same defect as [01](01-dead-configuration-surface.md)
- [ ] Check `IsFullyImplemented` has a caller. If `CreatePainter` silently falls back to
      `TagPillsFilterPainter` for an unmapped style while a method exists to report that, the report
      is being ignored

## Verification

- 8 styles, 28 pairs, all distinct — asserted in [09](09-verification-harness.md)
- The comparison must be shown to report an identical pair *as* identical before its passes mean
  anything: compare a render against itself and confirm the check goes red
- Per-style PNGs written for eyeball review, since "different" is necessary but not sufficient — two
  styles can differ numerically and still both look wrong
