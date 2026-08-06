# Stage 10 — the verification harness

Every claim in stages 01–09 is asserted here. Built as `scratchpad/DockProbe`, following the
`DialogsProbe` / `GridProbe` / `ContainerProbe` pattern that is already working.

This stage is not last in time. The baseline it captures must exist **before stage 01 changes a
line**, or most of the checks below lose the thing they compare against.

## The rule that governs this stage

**A check must be able to fail for the reason it was written.**

Across the preceding programs, harness checks returned eight confident false verdicts, and the
baseline caught all eight. Two are worth carrying forward because both are waiting in this folder:

- An accessibility check reported "0 accessible descendants — a screen reader sees an empty control."
  A stock `Form` holding a `Label` and a `Button` measured **0 too**: `GetChildCount()` returns `-1`
  by default and MSAA enumerates through the *window* hierarchy. The traversal was measuring itself.
  [07](07-accessibility.md) adds an accessible tree; the same trap is set.
- A text-fit check reported a caption clipped by 4 px, measured with `TextFormatFlags.WordBreak`
  against a control using a different flag set. **Two measurements disagreeing says nothing about
  which is right.** Rendered pixels settled it: nothing was clipped. [05](05-dead-capability-surface.md)
  adds labels, and will produce exactly this argument.

A third rule, earned in this folder's survey: **deletion plus a clean compile is authoritative for
deadness; grep is not.** `ClassicTaskbarDockPainter` looked unregistered and dead, and is the base
class of three painters. Stages 02, 04 and 06 each begin with a delete-and-build step for this
reason.

A check that has never failed is not yet a check. Break the thing deliberately and confirm the check
goes red before trusting it green.

## Baseline capture — ☑ done, before stage 01

`DockProbe` is built and the baseline is captured: 198 PNGs, 18 registered styles × 3 DPI for the
dock corpus and 18 × 8 for the state corpus, plus three baseline files.

```
=== 3 passed, 19 failed ===
    19 red at baseline (a stage owns each), 0 unexpected
```

Every check carries an expectation, so a run before the fixes land reports honestly: a red marked
`RedAtBaseline` is the measurement its stage exists to move, and only an *unexpected* red sets the
exit code. When a check goes green the harness says so explicitly, because a stale expectation is
how a harness starts lying.

What the baseline measured — each of these was a survey claim, now a number:

| stage | measured at baseline |
|---|---|
| 01 | 5 of 18 styles mutate the config while painting; 15 mutation sites |
| 01 | after Dracula paints, switching to Terminal still renders Dracula — colour distance 2 from Dracula, 80 from Terminal |
| 03 | 26 disagreements between `DockStyleHelpers` and `DockPainterMetrics` on size and spacing alone |
| 03 | `ItemSize = 40` becomes 44, `MaxScale = 2.0` becomes 1.2, on a style change |
| 03 | `StyleProfile` still reports `AppleDock` after `DockStyleType = PlankDock` |
| 04 | 1 distinct animation curve across all 9 `DockAnimationStyle` values |
| 04 | 3 easing functions defined more than once (`EaseOutCubic` ×3, `EaseOutElastic` ×3, `EaseInOutCubic` ×2) |
| 05 | `Custom` renders as Apple; the factory returns Apple for an unregistered style; 1 distinct render across 4 `IconMode` values |
| 06 | 2 of 18 styles respond to device DPI — `AppleDock` and `Material3Dock`, exactly as surveyed |
| 07 | dock reports `-1` accessible children — **and so does the stock `Panel` control group** |
| 08 | 1 swallowed exception, `BeepDock.InteractionState.cs:136` |
| 09 | 75 state collisions; no style renders more than 5 of 8 states distinctly |

Artifacts: `out/baseline/corpus.csv`, `out/baseline/state-collisions.txt`,
`out/baseline/dpi-aware-styles.txt`, `out/render/baseline/**.png`.

Stages that change rendering must diff against this corpus and *justify every difference*. An
unexplained pixel change is a finding, not noise.

### Three ways the harness measured itself

Found while implementing stage 01, all three producing confident numbers about the folder that were
actually about the probe. The fixture is now checked before anything that compares renders is
trusted.

1. **Items had no `ImagePath`.** Most painters draw only an icon in the passive states, so an
   icon-less item rendered as an empty field — and empty equals empty. This reported *9 of 18*
   distinct item renders and a set of cross-family "collisions" that were styles agreeing on
   nothing. With real icons it is 12 of 18, and the remaining collisions are real.
2. **`CurrentOpacity` was left at 1.0 for every state.** The disabled dimming lives in
   `DockAnimationHelper.UpdateAnimations` (`:105`), not in the painter, so skipping the animation
   pass made "Disabled renders as Normal" look like a painter defect in all 18 styles. `ApplyState`
   now sets opacity the way the animator would. `CurrentScale` is deliberately held at 1.0, because
   scale is animator-supplied too and holding it constant is what isolates the question this stage
   asks about painters.
3. **`dotnet run -v q --nologo` forwarded `--nologo` to the probe**, which took `args[0]` as its
   output directory. Every render for an afternoon went to a folder named `--nologo` while the stale
   ones in `out/` kept being read as current — including the blank pre-icon renders that made fix 1
   look like it had not worked. The runner now ignores switch-shaped arguments.

The first two are the trap this stage already warned about, in a new costume: a check that cannot
fail for the reason it was written. The third is worse, because nothing was wrong with the check —
the artifacts were simply not the ones being produced. **A render check must verify its own output is
fresh**, or it is reading history.

### Two checks the baseline already corrected

Both were written wrong and the baseline caught them, which is the argument for capturing it first:

- The background sampler read the bitmap centre — where item 2 is drawn. Whether it measured
  background or item depended on the style, so a background assertion was silently an item assertion
  for some styles. It now samples the left edge at mid-height.
- The style-switch check was written as Dracula → Arc. `MinimalDockPainter` (Arc's base) fills at a
  hardcoded `0.05f` alpha (`MinimalDockPainter.cs:23`), so every possible outcome renders within a
  few units of the field it was drawn over — the check could not distinguish pass from fail and would
  have read as a clean pass after any change. Rewritten as Dracula → Terminal, both on
  `ClassicTaskbarDockPainter`, which honours config colour and opacity.

## Ground rules (mechanical, cheap, run every time)

- no bare `catch` / `catch (Exception)` whose body neither rethrows nor reports —
  [08](08-popup-and-tooltip.md). *Current count: 1, at `BeepDock.InteractionState.cs:136`.*
- no painter assigns to `DockConfig` — [01](01-style-switching-is-one-way.md)
- no second per-style switch on `DockStyle` returning sizes — [03](03-config-consolidation.md)
- no easing function defined twice — [04](04-animation.md)
- no public property on `BeepDock` without a reader outside `DockConfig` —
  [05](05-dead-capability-surface.md)
- no painter reads `itemState.Is*` directly — [09](09-interaction-state.md)
- no commented-out code block — [06](06-dpi.md), `DockPainterMetrics.cs:367-400`
- no control flow in `InitializeComponent`

## Checks by stage

### Config immutability — [01](01-style-switching-is-one-way.md), [03](03-config-consolidation.md)
- paint every style in sequence; assert `_config`'s five nullable colours are still null
- assert the sampled background matches the *current* style, not the first one painted
- assert an explicitly set `BackgroundColor` still wins over every style default
- assert `ItemSize`, `Spacing`, `Padding`, `MaxScale`, `ShowShadow`, `BackgroundOpacity` survive a
  `DockStyleType` change
- assert `StyleProfile` reports the active style's values after a style change, not the constructor's

### Geometry — [02](02-painter-contract.md)
- `AppleDock` and `Windows11Dock`, same hover, must produce **different** item bounds
- for all 19: a point at the centre of item *k* hit-tests to *k*
- all 19 unchanged against the baseline until the Apple magnification step

### Animation — [04](04-animation.md)
- the nine `DockAnimationStyle` values produce nine pairwise-distinct scale curves
- every curve satisfies `f(0)=0`, `f(1)=1`, and stays within `[-0.5, 1.5]`
- animations terminate: after 2 s, current equals target and no redraw is requested

### Capability surface — [05](05-dead-capability-surface.md)
- `Custom` honours `DockConfig` and does not render as Apple
- the factory throws for an unregistered style instead of falling back
- the four `IconMode` values render pairwise distinct, and labels fit at all three DPIs

### DPI — [06](06-dpi.md)
- every measured feature at 200% is within 1 px of double its 100% value, for all 19 styles
- within one render, the indicator dot and item size scale by the same factor
- 150% included; a `* 2` shortcut must not pass

### Accessibility — [07](07-accessibility.md)
- `GetChildCount()` returns the visible item count — **verified against the stock-form control group
  first**, per the trap above
- each child's `Name` is its item's `Text`; each child's bounds centre hit-tests to that item
- overflowed items are not published
- keyboard focus moves the `Focused` child
- 4.5:1 contrast under `SystemInformation.HighContrast`, for every style

### Hosted surfaces — [08](08-popup-and-tooltip.md)
- ten hovers construct at most one tooltip and leave at most one timer running
- the tooltip follows a theme change and differs in font and radius between two dock styles
- no window handles survive disposal

### Interaction states — [09](09-interaction-state.md)
- 19 styles × 8 states, pairwise distinct within each style
- the five recolour painters are declared known-equal to their base on items and indicators, and
  **not** equal on background
- selected+hovered renders as hovered
- disabled differs from normal at matched opacity

## Deliverable

`scratchpad/DockProbe`, printing `=== N passed, M failed ===`, plus the PNG corpus written to a
render directory for eyeball review. Baseline captured before stage 01 begins; every stage reports
through it.
