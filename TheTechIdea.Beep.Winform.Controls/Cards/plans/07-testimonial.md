# Stage 07 — `BeepTestimonial`

**Kind:** refactor · **Files:** `Testimonials/` (1,797 lines, 5 helper files)

9 public properties, 4 events, one `DrawContent` override.

## Composition

```
┌──────────────────────────────┐
│ [quote mark]                 │
│ [quote]                      │  wraps; the tallest part of the card
│ [avatar] [name]              │
│          [username] [rating] │
└──────────────────────────────┘
```

`Ratings/` already provides a rating control — host it rather than drawing stars. The quote is a
`BeepLabel` with `WordWrap` and `Multiline`; the dialogs program found that **both** are needed —
`WordWrap` decides where lines break, `Multiline` lets more than one render, and with only the first
a wrapped string is measured across several lines and then drawn as one.

## Nothing here is dead

**Zero of its nine public properties are unreferenced** — the only one of the five secondary cards
with a clean census.

`Username` and `ViewType` appeared dead in a first measurement scoped to `Cards/`. Scanning the whole
solution — 4,822 files — found readers for both. The wrong result is recorded because the corrected
one is only trustworthy if the mistake behind it is visible: a census scoped to one folder measures
the folder, not the property.

## `ViewType`

`ViewType` selects a presentation. Read it before composing: it likely means two or three arrangements
of the same parts — quote-first versus attribution-first, compact versus full. Each becomes a
composition, the same way `CardStyle` does in stage [02](02-beepcard.md). Composing only the default
and leaving the others rendering identically would recreate the aliased-style defect that stage 02
exists to resolve.

**Status: done.** All four view types compose: Classic 5 controls, Minimal 4, Compact 4, SocialCard 6.

The four `Draw*View` routines were the same parts in different arrangements, so they became four call
orders over one set of controls rather than four paint routines measuring and centring text by hand:

| view | composition |
|---|---|
| Classic | avatar · quote · attribution · `BeepStarRating` |
| Minimal | avatar · quote · attribution · company logo |
| Compact | avatar · attribution · quote |
| SocialCard | avatar · attribution · quote · dismiss `BeepButton` |

`BeepStarRating` was already a child control and stays one; `Rating` now drives its `SelectedRating`
instead of a painted star count. Position and username join into one attribution line.

**`OnTestimonialClick` did not exist.** `TestimonialClick` was raised inline from `OnKeyDown` only, so
there was no raiser for the quote's click to call. It has one now, and `OnKeyDown` uses it.

## Verification

1. **A long quote wraps** and the attribution stays below it — the card grows rather than clipping.
2. **The rating renders distinctly at each value**, 0 through 5. An off-by-one in a star row is
   invisible until someone counts.
3. **Every `ViewType` produces a different arrangement**, asserted from control positions. Catches the
   compose-one-mode-and-alias-the-rest failure.
4. **The census stays clean** — re-run the solution-wide unreferenced-property scan and assert zero
   for this card. It is the guard that stops the refactor landing new declared-but-unread properties,
   which is how the other four cards acquired theirs.
