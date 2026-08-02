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

---

## Outcome

### The suspicion was not supported

All 8 styles rendered at 520x220 with identical criteria and theme, compared pairwise:

**28 pairs, 0 identical.**

The Filtering painters genuinely differ. The uniform "5 overrides each" signal that prompted the
suspicion turned out to mean the base does the right thing and each subclass changes what matters —
not that seven inherit their appearance. Recorded plainly, because the pattern held in three previous
folders and it would be easy for a later reader to assume it holds here too.

Per-style PNGs are written to `scratchpad/filter-renders` for eyeball review; numerically distinct is
necessary but not sufficient.

### What the phase did find: four pieces of dead surface

| member | shape | disposition |
|---|---|---|
| `IFilterPainter.SupportsAnimations` | declared, defaulted, overridden — **no reader** | removed |
| `IFilterPainter.SupportsDragDrop` | declared, defaulted, overridden by all 8 — **no reader** | removed |
| `FilterPainterFactory.IsFullyImplemented` | `public static`, **0 callers**, and vacuous once every style is mapped | removed |
| `FilterPainterFactory.GetStyleDescription` | `public static`, **0 callers** | removed |

Twelve declarations in total, plus two factory methods. Cross-repo sweep confirmed no consumer in
`Beep.Winform.Data.Integrated` or `Beep.Sample` before removing anything public.

**Intent preserved here in case drag-drop is implemented later:** the painters that declared
`SupportsDragDrop => true` were `AdvancedDialogFilterPainter` and `GroupedRowsFilterPainter`; the
other six declared `false`. Reordering criteria by drag is a reasonable feature, but a capability
flag with no consumer is not a partial implementation of it — it is eight painters answering a
question nobody asks.

### A measurement error worth recording

`SupportsAnimations` first appeared to have one real caller, at
`Forms/ModernForm/BeepiFormPro.Events.cs:92`. That is a **different** `SupportsAnimations` — the form
painters declare an identically-named property, and ~37 of them do. Scoping the search to files that
reference `IFilterPainter` showed the Filtering one has no reader at all.

Fourth instance in this program of a search matching across a boundary and producing a wrong count.
The others: counting enum values across three declarations in one file, an exclusion pattern that
also stripped the evidence, and a `timeout` truncating a cross-repo sweep into a false "no consumers".

---

## Correction — what the first comparison actually rendered

The phase-06 work exposed a flaw in this phase's evidence.

`FilterLayoutInfo.TagRects` and `.RowRects` are populated by each painter's **layout** pass, which
`BeepFilter.RecalculateLayout()` drives — and that method is `private`. The probe had built its test
data with `f.ActiveFilter.Criteria.Add(...)`, mutating the list in place. That never triggers a
recalculation, so the layout still described **zero criteria** and every style rendered its empty
"Add Filter" state.

So the original "28 pairs, 0 identical" measured the styles' *empty-state chrome*, not their
rendering of criteria. The claim was right; the evidence behind it was much weaker than stated.

Re-run correctly — assigning through the `ActiveFilter` **setter**, which calls
`RecalculateLayout()` — with criteria genuinely laid out (`TagRects.Length = 3`, criterion 0 at
`{8,8,161,32}`):

**28 pairs, 0 identical.** The conclusion holds, now on evidence that supports it.

The lesson generalises past this folder: *a render test proves nothing about content the control was
never given.* A populated-looking API call that skips the recalculation path produces a confident
green result about an empty control.
