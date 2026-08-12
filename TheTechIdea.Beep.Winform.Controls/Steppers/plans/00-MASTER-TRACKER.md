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
