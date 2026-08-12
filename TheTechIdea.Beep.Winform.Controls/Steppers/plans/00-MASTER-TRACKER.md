# Steppers — review and enhancement

## Census — unusually clean

| rule | count |
|---|---|
| literal `Color.Xxx` / `FromArgb(r,g,b)` | **0** |
| `useThemeColors` flags | **0** |
| bare `catch { }` | **0** |
| literal `new Font("…")` | **0** |
| luminance shifts / blends | **0** |
| `BeepLog` calls | 5 |
| `Color.Empty` checks | 11 — all the accepted `customColor is { } c && c != Color.Empty` passthrough, not the disease |
| reflection | 3 — `StepperFontHelpers` reading theme fonts by name, reported through `BeepLog` |

Nothing to sweep. The gap was behavioural.

## The gap — the default painter drew no step labels

`CircularNodeStepperPainter` is the **default** (`[DefaultValue("CircularNode")]`) and drew only
the number inside each circle. It never rendered `step.Text`, so a stepper with named steps came
out as anonymous numbered dots — and because eight of the other painters *do* draw labels, every
caller read the omission as its own wiring fault. `ComputeLayout`'s own comment in that file
("labels touching") shows labels were expected there.

It now draws the title beside the node — under it when horizontal, right of it when vertical —
with the subtitle beneath, from `StepperThemeHelpers.GetStepLabelColor` so the state colouring
carries. No font is allocated in the paint path.

## The control had no title of its own

`BeepStepperBar` could not show a heading at all. It inherits `Control.Text`, but nothing ever
painted it — setting it did nothing — so callers had to park a separate label above the bar and
keep it aligned by hand. The wizard forms were doing exactly that with their own step-count label.

- **`Text` is the control's title**, painted in the theme's title typography.
- **`SubText`** (new) is the optional line under it, in its own smaller font. It first inherited
  `_textFont ?? titleFont`, which fell through to the bold heading face whenever `_textFont` was
  unset and read as a second title.
- **`TitleImagePath`** (new) draws an icon left of the title through `StyledImagePainter` — the
  control that renders and themes SVGs here — in a square box matched to the band height, so icon
  and heading share a baseline at any DPI.
- **`TitleAlignment`** places the heading within the band.

The band is measured and `GetStepperContentBounds` subtracts its height, so the steps can never be
laid out over the heading. Verified: first step top moves to 105px once a title is set.

### The check for it was wrong twice

- Measuring ink in a band derived from the step positions **moved that band** the moment a title
  existed (the steps shift down), so it compared two different regions and reported *less* ink with
  a title than without.
- The icon check failed on a path that does not resolve in the probe's environment. A probe that
  goes red on a **missing asset** says nothing about whether the painting code is right, so the
  icon is rendered for eyeballing instead of counted.

Both are now a whole-bitmap differential plus a saved render. 82 checks, 0 failures.

## Per-step captions: `Text` is the title there too, and every painter now draws it

There is no separate title property — `StepModel.Text` **is** the title — so every painter is
expected to show it. Two drew nothing at all and the rest each rolled their own placement:

- **`SquareDashed`** drew its square and even computed a text colour it never used for a caption.
- **`Dots`** was worse: `Initialize` **discarded the owner and all three fonts**, keeping only the
  theme, so it could not have drawn a caption even in principle.
- Of the twelve that did draw text, **one** measured against the room it had and **one** applied
  `EndEllipsis`. A title longer than its slot ran into its neighbour rather than being cut.

`Helpers/StepperLabelHelpers.cs` is now the one authority: it measures, clamps to the width the
step actually owns, ellipsises, and places by orientation — under the node when horizontal, right
of it when vertical. No font is allocated in the paint path.

Sizing took three corrections, each visible only in a render:

1. The box was sized from the **title** and then reused for the **subtitle**, so a short title
   clipped a longer one — "Profile" / "Ste…" while "Account" / "Step 1" was fine. It measures both
   lines and takes the wider.
2. A lone step fell back to the **node's** width, which is far narrower than its room. It gets the
   content rect now.
3. `Dots` captions only the **active** step, so it competes with nothing — but it was being clamped
   to the ~20px dot pitch and rendered "Pr…" / "St…". It uses the full content width.

## The instrument was wrong three times, and each time it said PASS

Worth recording, because every version looked reasonable:

1. **`inkBelowNodes >= 20`.** Break-it-first: with the new label drawing commented out it still
   reported **1096 "label pixels"** and passed. It was counting the panel border, node rings and
   connectors.
2. **Differential, band below the nodes.** Now it failed correctly for CircularNode — but flagged
   `ChevronBreadcrumb`, `CompactInline`, `SegmentedTab` and `GradientMaterial` as label-less. They
   draw their text **inside** the chevron or segment, so the band was in the wrong place. Four
   false reds.
3. **Differential over the whole bitmap.** Correct for eight painters, but `SegmentedTab` and
   `GradientMaterial` **size their segments from the text**, so blanking it changes the layout —
   SegmentedTab measured *more* ink with no text at all. The check cannot separate "drew a label"
   from "laid itself out differently" for those two.

Final form: a whole-bitmap differential over the eight painters whose layout is text-independent,
with `SegmentedTab` and `GradientMaterial` excluded by name and verified by eyeballing their
renders instead — both plainly draw the step titles inside their shapes.

**78 checks, 0 failures**, renders in `%TEMP%\StepperProbe`.

## Note for the Wizards work

`WizProbe` hangs after its first form and times out — on **unmodified** library code, verified by
reverting the wizard commit entirely and reproducing it. `StepperProbe` runs clean against the same
build, so it is that probe, not the library. Any conclusion drawn from a hung `WizProbe` run is
unsafe; the wizard forms' designer files were re-landed on that basis.
