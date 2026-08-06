# Stage 08 — icon treatments, centred layouts, inset callouts

**Kind:** conformance to `dialog1.png`, `dialog2.png`, `dialog3.png`, `dialog5.png`, `dialog6.png`.

**Status: ◐ partial — the callout is built; two items are control work, one was already done.**
5 of 5 checks green; suite **56 passed / 1 failed**.

### Built: the inset callout

`DialogCallout { Severity, Label, Text }` and `DialogConfig.Callouts`, rendered above the actions as a
two-column `TableLayoutPanel` — a narrow accent bar and a body carrying the label and text. Built from
controls, not painted.

A callout carries **its own** severity: the check asserts a *warning* callout inside an *error* dialog
renders warning-coloured, which is the case `dialog3.png` actually shows and the distinction the
pattern exists for. The accent colour comes from the severity resolver, never a literal, so it follows
a theme change — and it is the same exception the destructive button takes, where the colour *is* the
message.

### Already done: the two body layouts

Item 1 asked for `DialogBodyLayout { IconLeft, Centred }`. That is what
[stage 12](12-presentation-styles.md)'s `DialogPresentation` already decides, and adding a second enum
would have been a third way to express one thing — the defect stages 03 and 05 exist to remove.

Instead `IconAlignment` — which had **zero readers** — is now an `[Obsolete]` projection onto
`Presentation`: `Left` ↔ `TitleBar`, `Top` ↔ `Centred`, `None` ↔ `ShowIcon = false`. The icon and the
text therefore cannot disagree, which is exactly the half-implementation this stage warned about.

### Not built: control work, not dialog work

- **Icon treatments** (`Bare`, `CircleOutline`, `CircleFilled`, `Oversized`). A circular container
  around a glyph is drawing, and this folder does not draw — `BeepImage` would own it.
- **Inline emphasis.** Bolding one run inside `Message` needs a label that can render runs. That is
  `BeepLabel`'s to provide; a dialog cannot fake it without painting text itself.

Both are recorded rather than half-built, and both are blocked on the same rule that removed all the
colouring from this folder.

### A measurement note

The overlap check failed on its first run against correct code. It compared `Control.Bounds` directly
— which are relative to each control's own parent, and the callout and the buttons have different
parents, so the rectangles were never in the same coordinate space — and its ternary had two identical
branches. It compares screen rectangles now. **Fourth time this session** a check measured the wrong
thing and reported working code as broken, which is why every geometric check now converts to screen
coordinates first.

## What the references specify

### Two body layouts

- **Icon-left** — `dialog1.png`'s left column: a circular outlined icon on the left, heading and body
  text to its right, left-aligned. The dense, informational layout.
- **Centred stack** — `dialog2.png`, `dialog4.png`, `dialog5.png`: icon centred, heading centred
  below it, body centred below that. The layout for a single clear outcome.

`DialogConfig` has `IconAlignment` (`:60`) with a `DialogIconAlignment` enum, so the *icon* can move,
but a centred icon over left-aligned text is not the centred layout — text alignment has to follow.

### Four icon treatments

| treatment | reference |
|---|---|
| circular **outline**, severity-coloured glyph | `dialog1.png` |
| bare glyph, no container | `dialog2.png`'s warning triangle |
| circular **filled**, white glyph | `dialog5.png`'s green tick, `dialog4.png` |
| oversized decorative graphic, bleeding off the panel | `dialog6.png` |

`IconSize` (`:58`) and `IconSizePreset` (`:59`) size the icon; nothing expresses its *container*.

### The inset callout

`dialog3.png` has the element none of the current forms can produce: a tinted panel inside the body,
with a left accent bar, an icon, a bold label ("Warning") and body text — carrying a *secondary*
severity message inside a dialog that already has one.

This is the pattern every framework calls an alert or callout, and it is what turns "are you sure?"
into "here is specifically what else you are about to destroy" — `dialog3.png` uses it for
*"By deleting this media **8 connected hotspots** will also be deleted."* That is the whole reason
the pattern exists, and it is the most consequential missing piece in this stage.

### Inline emphasis

`dialog3.png` bolds the filename inside the sentence and `dialog4.png` bolds a phrase. `Message` is a
plain string, so the body is uniformly styled and cannot emphasise the one word that matters.

## The fix

1. **`DialogBodyLayout { IconLeft, Centred }`** driving icon position *and* text alignment together,
   so the two cannot disagree.
2. **`DialogIconTreatment { Bare, CircleOutline, CircleFilled, Oversized }`**, resolved against the
   severity from [06](06-severity-and-headers.md) — outline and glyph in the severity colour, filled
   with an on-severity glyph.
3. **A callout model** — severity, optional label, text — rendered as an inset tinted panel with a
   left accent bar, reusing the same severity resolver rather than inventing a second palette. Zero
   or more per dialog.
4. **Inline emphasis in `Message`.** The smallest thing that covers the references is a bold run;
   full rich text is not needed. A minimal inline markup or a segment list, rendered by measuring
   runs — and whichever is chosen, `Message` must keep working unchanged for plain strings, because
   every existing caller passes one.
5. Body text wraps and the dialog grows within `MinWidth`/`MaxWidth` (`:252`, `:257`) rather than
   clipping — long content is stage [09](09-async-and-long-content.md).

## Verification

1. **The two layouts differ**, and in `Centred` the text is actually centred — assert the measured
   text rectangle is horizontally centred in the body, not just that the icon moved. *The likely
   half-implementation is moving the icon and leaving the text left-aligned, which looks like a bug
   rather than a layout.*
2. **Four icon treatments render distinctly** at one severity.
3. **The callout appears, and carries its severity.** A warning callout inside an error dialog must
   render in the *callout's* severity, not the dialog's — the case `dialog3.png` shows.
4. **The callout wraps and the dialog grows.** A long callout must not clip or overlap the buttons.
5. **Inline emphasis renders.** A bolded run measures wider than the same text unbolded and does not
   overlap its neighbours; a `Message` with no markup renders identically to today. The second half
   protects every existing caller.
6. **Baseline guard.** All eight forms render byte-identical to the pre-stage corpus until a dialog
   opts into a new layout, treatment or callout.
