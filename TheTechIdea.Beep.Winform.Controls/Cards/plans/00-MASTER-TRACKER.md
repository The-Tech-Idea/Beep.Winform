# Cards — composition refactor

Master tracker for `TheTechIdea.Beep.Winform.Controls/Cards/`.
**105 C# files, 22,362 lines, six card controls, 55 painters.**

## The decision

**Cards stop painting themselves and are composed from Beep controls.** A card becomes a
`TableLayoutPanel` holding `BeepLabel`, `BeepImage`, `BeepButton` and the other existing controls, one
per cell — the same arrangement the dialogs use. The 55 painters and the inline `DrawContent`
overrides go.

This replaces the previous plan for this folder, which was a set of fixes *to* the painters. Those
fixes are not carried over, because most of them stop existing:

| the old plan's finding | what composition does to it |
|---|---|
| 13 painters never scale for DPI | gone — the layout engine and the controls scale |
| 24 unconditional literal colours in 12 painters | gone — every control resolves its own theme colour |
| `AccessibleName` is the literal `"Card"` for all 56 styles | gone — each `BeepLabel` carries the text it displays, so the name is the content |
| `TaskCard.MoreIcon` renders but nothing can click it | gone — a `BeepButton` is hit-testable by construction |
| helper families duplicated 5× (Theme 22/27, Layout 17/21, Icon 15/17 methods byte-identical) | most delete outright; they exist to compute rectangles and colours a layout panel and a themed control already know |

Three of the four defects the survey found were consequences of hand-painting. They are not worth
fixing in painters that are about to be deleted.

## What composition buys, measured against this folder's own defects

- **Theme.** Every Beep control derives from `BaseControl`, which subscribes to
  `BeepThemesManager.ThemeChanged` and re-applies itself. A composed card follows a theme change with
  no code at all. The 24 literal colours and the 45 painters that reach for `_theme` by hand both stop
  being a category of bug.
- **Accessibility.** A `BeepLabel` showing a person's name *is* that text. `BeepCard`'s constant
  `"Card"` name and its style-enum description cannot recur, because there is no separate string to
  keep in step.
- **Interaction.** Buttons, links and overflow menus become controls with real hit-testing, focus and
  keyboard access. Painted affordances have none of that — which is why `MoreIcon` and `AvatarIndex`
  are declared and unreachable today.
- **Verification.** Assertions move from pixels to the control tree: does the card contain a label
  with this text, is the button inside its cell, is the action reachable. That removes the three traps
  stage [09](09-verification.md) had to carry — blank captures, proportional sample points, and mixed
  coordinate spaces — every one of which reported correct code as broken in the dialogs program.

## What it costs, stated plainly

- **56 layouts to build** for `BeepCard`'s 56 `CardStyle` values, plus five secondary cards. Many
  share structure — the `*Only` styles are one part each — but this is the bulk of the work.
- **Child-control count.** A card is currently one control; composed, it is five to fifteen. Cards
  appear in lists and grids, so this is the one genuine risk in the change and stage
  [09](09-verification.md) measures it rather than assuming it is fine.
- **`ICardPainter`, `IStatCardPainter` and `RegisterPainter` are public.** `BeepStatCard` exposes
  `RegisterPainter` so a consumer can add a painter kind. Removing that is a caller-visible break and
  needs the decision recorded in stage [08](08-removals.md), not made silently.

## Stages

| # | Stage | Kind | Status |
|---|---|---|---|
| [01](01-composition-pattern.md) | The card scaffold and the parts every card composes from | **foundation** | ☑ done |
| [02](02-beepcard.md) | `BeepCard` — 56 styles as compositions | refactor | ☑ done |
| [03](03-statcard.md) | `BeepStatCard` | refactor | ☑ done |
| [04](04-featurecard.md) | `BeepFeatureCard` | refactor | ☑ done |
| [05](05-metrictile.md) | `BeepMetricTile` | refactor | ☑ done |
| [06](06-taskcard.md) | `BeepTaskCard` | refactor | ☑ done |
| [07](07-testimonial.md) | `BeepTestimonial` | refactor | ☑ done |
| [08](08-removals.md) | Deleting the painters and the helpers they needed | structural | ☑ done |
| [09](09-verification.md) | The harness, rebuilt on the control tree | verification | ☑ done |

Status marks: ☐ open · ◐ in progress · ☑ done

## Progress

**`Helpers/CardScaffold.cs`** is the scaffold: two columns, an icon gutter that collapses, rows added
in order, plus `AddFullWidth` and `AddActions`. Nothing in it assigns a colour, scales a value or sets
an accessible name.

**`BeepStatCard` is migrated** and is the proof the pattern works. Its `DrawContent` is gone; nine
controls compose it, verified from the control tree rather than from pixels:

```
CardScaffold
  BeepImage        24x24 icon, gutter
  BeepLabel        'Total Revenue'
  BeepLabel        '$1,250.00'              38px tall - the KPI dominates
  BeepLabel        '+12.5%'
  TableLayoutPanel                          the trend row
    BeepImage      16x16 direction glyph
    BeepLabel      'Trending up this month'
  BeepLabel        'Visitors for the last 6 months'
```

**The trend row is new behaviour, not just relocated code.** `TrendText`, `TrendUpSvgPath` and
`TrendDownSvgPath` had no readers anywhere in the solution — a stat card that could not show its
direction of travel. They have a row now.

**`BeepMetricTile` is migrated**, and it answered the one question the plan flagged as uncertain.
The silhouette — a faded glyph *behind* the text — layers as the scaffold's `BackgroundImage` with the
labels marked `IsTransparentBackground`. A `BeepImage` in a spanning cell would not have worked:
`IsChild` gives a control its parent's colour, not transparency over what the parent painted, so the
opaque labels would have covered it. **No card needs a painted implementation.**

**The five secondary cards are migrated.** Every one composes, and the probe asserts from the control
tree that each card *contains* the text it shows rather than having painted it — the assertion this
refactor exists to make possible, and one no painted card could pass:

| card | controls | what it gained |
|---|---|---|
| `BeepStatCard` | 9 | the trend row, which had no readers at all |
| `BeepMetricTile` | 4 | the silhouette, which nothing drew |
| `BeepFeatureCard` | 10 | four dead icon paths, and actions that focus |
| `BeepTaskCard` | 10 | `MoreIcon` and the avatars as real controls; the literal pink gradient gone |
| `BeepTestimonial` | 4–6 | all four view types, and an `OnTestimonialClick` that did not exist |

**`BeepCard` is migrated too.** Its 56 styles are a `CardStyle → CardPart[]` table rather than 56
painters, so a new style is an entry rather than a class. `BeepCard.Drawing.cs` — 520 lines — is
deleted, along with the `_layoutContext` hit-testing that decided which painted rectangle the mouse
was over, and **both swallowed exceptions**, including the one this tracker recorded at
`BeepCard.cs:238`.

`PriceText` turned out to be the same defect as the stat card's trend row: a backing field commented
*"For product cards"* with **no public property**, so the seven styles built around one prominent
number could never be given one. It has a property now.

All six cards are composed, and **the painters are deleted**. `Cards/` went from **112 files / 22,578
lines to 36 files / 9,659 lines** — 76 files and 12,919 lines removed — and every card still composes:
55 of 56 `CardStyle` values carry content, `BlankCard` carries none, and the check fails in both
directions.

`RegisterPainter` was public, and the tracker asked for that removal to be a recorded decision rather
than a silent one: it is removed outright, because there is no compatibility constraint on cards.
Nothing outside `Cards/` referenced it.

**Every stage is done.** The harness asserts from the control tree — 0 failures across six cards and
all 56 styles — and the cost the tracker called this change's one genuine risk is measured rather than
assumed:

| | |
|---|---|
| 500 `BeepLabel`s constructed, never parented | 57 ms |
| 500 `BeepLabel`s in 100 panels of 5 | 1,449 ms |
| 500 controls in 100 composed cards | 1,498 ms |

**Composition costs about 3% over the same tree shape built by hand.** The cost is WinForms parenting
and layout, not the cards. 50 style changes on one card moved the handle count by +0 and +5 across two
runs, so recomposition does not leak.

The first version of that measurement put all 500 labels in one panel and reported 32.5 seconds — it
measured a 500-sibling container, which is O(n²) in the child count, not the controls. A result that
flattered the change was exactly as wrong as one that condemned it.

### A measurement note for the harness

Verifying this took three attempts. `CopyFromScreen` returned a file explorer twice: the probe window
was not foreground, so the capture was the desktop behind it. **`Control.DrawToBitmap` renders the
control tree itself** and cannot capture the wrong window — that is what stage
[09](09-verification.md) should use for card renders, keeping screen capture out of it entirely.

## Order of work

1. **[09](09-verification.md) captures the baseline first.** Every card must look the same after the
   refactor as before it; without a corpus there is nothing to compare against.
2. **[01](01-composition-pattern.md) next, and nothing else until it is right.** Six cards and 56
   styles all build on the scaffold. Getting it wrong and discovering that on the sixth card means
   redoing five.
3. **[03](03-statcard.md) as the first real card.** Four painter kinds, the smallest surface, and
   already the cleanest control here — the least likely to confuse a pattern problem with a card
   problem.
4. **[02](02-beepcard.md) last of the cards.** 56 styles; do it once the pattern has survived five
   smaller controls.
5. **[08](08-removals.md) only after every card is migrated.** A painter deleted while something still
   selects it is a crash, not a cleanup.

## Standing constraints

- **Layout is `TableLayoutPanel`, one control per cell.** Not dock stacks, not flow panels, not
  coordinates.
- **Composition belongs in the designer file**, in plain `Controls.Add(control, column, row)`
  statements. Method calls are not designer-serialisable, and a form composed at runtime shows nothing
  on the design surface.
- **Nothing assigns colours.** Controls resolve their own from `BeepThemesManager`. The single
  exception this library allows is a colour that carries meaning rather than style — an alert or
  destructive affordance.
- **`BeepImage` for every icon.** It is what renders and themes SVGs.
- No legacy paths, no stubs, no shims. Never swallow an exception — **one exists**, at
  `BeepCard.cs:238`.
- Do not modify `BaseControl`. Use it.

## The rule every stage is verified against

**A check must be able to fail for the reason it was written.** Every stage states the baseline it
measures against and what a failing run prints today.
