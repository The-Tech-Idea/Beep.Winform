# Stage 09 — the harness, rebuilt on the control tree

**Kind:** verification · **Status: done.**

## Results

The harness asserts from the control tree, not from pixels:

1. **Each card contains its text as a control** — six cards against known strings. The assertion no
   painted card could pass.
2. **Each card renders** — `DrawToBitmap`, counting distinct colours; under 3 is blank.
3. **Every `CardStyle` composes** — all 56, constructed, parented and counted.
4. **Cost is measured, not assumed.**

| card | controls | render |
|---|---|---|
| `BeepStatCard` | 9 | 45 colours |
| `BeepMetricTile` | 4 | 71 colours |
| `BeepFeatureCard` | 10 | 76 colours |
| `BeepTaskCard` | 10 | 74 colours |
| `BeepTestimonial` | 4–6 per view | 38–99 colours |

**55 of 56 `CardStyle` values carry content; `BlankCard` carries none.** 0 failures.

Every check was broken deliberately before being trusted. A sentinel string no control carries prints
`FAIL: no control carries 'NOT-PRESENT-SENTINEL'` and returns exit 1, and `BlankCard` is asserted to
have *no* content — so the style check fails in both directions rather than only one.

### Counting controls could not tell a composed card from an empty one

The first style check counted controls and passed everything, because `ContextMenuIcon` defaults to a
real glyph — every card carries an overflow button whether or not it has content. It counts **content**
controls now: labels, images, ratings, and buttons with text.

## Cost — the one genuine risk, measured

100 `BasicCard`s: 5 child controls each, 600 controls total, ~1,500 ms to build and lay out, ~43 ms to
repaint all 100, +17 process handles.

Against a baseline of the same tree shape with no cards in it:

| | |
|---|---|
| 500 `BeepLabel`s constructed, never parented | **57 ms** |
| 500 `BeepLabel`s in 100 panels of 5 | **1,449 ms** |
| 500 controls in 100 composed cards | **1,498 ms** |

Construction is nearly free at 0.11 ms per control. The cost is WinForms parenting and layout, and
**composition adds about 3% on top of it** — a composed card is as cheap as the equivalent bare tree.
The number to act on is ~15 ms per card at build time, which argues for virtualising a long list, not
for painting cards.

### The first cost measurement was wrong

The first baseline put all 500 labels in **one** panel and reported **32.5 seconds**, which would have
made composed cards look 20× faster than raw controls. It measured WinForms' cost for a 500-sibling
container — O(n²) in the child count — not the cost of the controls.

Same failure this program keeps hitting: **the instrument was wrong, not the code.** A result that
flatters the change is exactly as suspect as one that condemns it.

## Recomposition does not leak

`CardScaffold.Clear` disposes what it removes, which matters because a card recomposes on every
property that changes its arrangement. 50 style changes on one card moved the handle count by **+0 and
+5 across two runs** — noise. A card that kept its displaced controls would have shown roughly 250.

## Screen capture is not used

`CopyFromScreen` returned a file explorer twice: the probe window was not foreground, so the capture
was the desktop behind it. `Control.DrawToBitmap` renders the control tree itself and cannot capture
the wrong window. No check in this folder reads the screen.

---

## Original plan

**Captures the baseline before stage 01 begins**

There are no tests for 22,362 lines. Every claim in this program is "the card looks the same and
behaves better", and without a corpus captured *before* anything changes there is nothing to compare
against.

## Composition changes what a check is

This is the largest single benefit of the refactor, and it is worth stating precisely.

**Painted cards can only be checked by pixels.** There are no child controls to query, so every
assertion samples a bitmap — and the dialogs program lost most of a session to exactly that:

- a proportional sample point landed on a form border and reported `59,59,63` for every severity
- the same point, after a dialog grew, landed on a label and reported `248,250,255`
- a capture fired before the window painted and came back as one flat colour, three separate runs
- two controls' `Bounds` were compared across different parents, so the rectangles were never in the
  same coordinate space

Four checks reported correct code as broken. Each cost a round of changes to code that was fine.

**Composed cards are checked by asking the control tree.** Does the card contain a label whose text is
the title. Is the action focusable. Is the icon a `BeepImage` with this path. Do the title and body
share a left edge. None of those can be defeated by occlusion, paint timing, or a sample landing a few
pixels off.

Pixel comparison survives for exactly one job: **the before/after corpus**, proving the refactor did
not change how a card looks. That comparison is whole-image, so it has no sample point to get wrong.

## The baseline to capture, before stage 01

Renders: 56 `CardStyle` values, 4 `StatCardPainterKind` values, and the five secondary cards. One PNG
each plus a CSV of measured facts.

Source facts, so a later run can reproduce rather than trust them:

| measurement | today |
|---|---|
| `CardStyle` members / mapped / distinct painters / painter classes | 56 / 56 / 53 / 55 |
| painter classes never constructed | 2 — `CommunicationCardPainter`, `ProductCompactCardPainter` |
| styles sharing a painter with another style | 3 — `ImageCard`, `DownloadCard`, `ContactCard` |
| `BeepCard.AccessibleName` | the literal `"Card"`, all 56 styles |
| bare catches | 1 — `BeepCard.cs:238` |
| unreferenced public properties | StatCard 3, FeatureCard 4, MetricTile 2, TaskCard 2, Testimonial 0 |
| helper methods byte-identical across copies | Theme 22/27, Layout 17/21, Icon 15/17, Accessibility 8/15, **Font 0/4** |

## The one risk this stage must measure, not assume

**Child-control count.** A card is one control today; composed, it is five to fifteen. Cards appear in
lists and grids, so a list of forty cards goes from forty controls to several hundred. That is the
only place this refactor can lose to the painters, and the plan should not assert it is fine.

Measure, on the same machine, before and after:

1. **Control count per card**, per style.
2. **Time to create and lay out 100 cards** in a scrolling container.
3. **Repaint time** when that container scrolls.
4. **Handle count** for the same 100 cards — the resource that runs out first in WinForms.

If the numbers are bad, the answer is not to abandon composition: it is to virtualise the container,
or to compose lazily on first paint. But that decision needs the measurement in front of it.

## Verification of the verification

1. **The control group.** A bare `BaseControl` run through every assertion. Anything that passes
   against it is measuring the framework, not the cards.
2. **Deliberate breakage.** Each check is made to fail once, for the reason it was written, before it
   is trusted. A check that has never been red is not evidence.
3. **The corpus fixture renders ink**, asserted before any image comparison is believed — the guard
   that caught three blank captures elsewhere.
