# 09 — Verification harness

`scratchpad/FilterProbe`, following the `DialogsProbe` / `GridProbe` / `ContainerProbe` pattern that
is already working.

## The rule that governs this phase

**Every check must be shown capable of failing before its pass means anything.**

Across the preceding programs, harness checks returned **eleven** confident false verdicts. Every one
was caught by putting a controlled baseline behind it. Four are worth carrying, because each
represents a class of mistake this folder can reproduce:

- **A check too weak to catch its own defect.** `caption > 0` passed under the exact bug it existed
  for, because the old behaviour also left a positive — merely squeezed — caption. The discriminating
  property was that adding a badge must widen the *tab*, not narrow the *caption*.
- **A check that fails when the design works.** An assertion that the *widest* item shrinks went red
  precisely because the widest was the active item, deliberately preserved at full width.
- **A traversal measuring itself.** An accessibility check reported "0 descendants — a screen reader
  sees an empty dialog". A stock `Form` holding a `Label` and a `Button` measured 0 too:
  `GetChildCount()` returns -1 by default and MSAA enumerates through the window hierarchy.
- **Two measurements disagreeing.** A text-fit check reported a 4px clip by comparing the control's
  height against a `TextRenderer` measurement taken with different `TextFormatFlags`. Neither was
  wrong; the comparison was meaningless. Rendered pixels settled it.

## Checks by phase

### [01] Dead configuration surface
- each `FilterDisplayMode` value renders differently from `AlwaysVisible`
- each `FilterPosition` value changes the filter region's bounds
- no `[Browsable(true)]` property whose value never reaches behaviour

### [02] Painter distinctness
- 8 styles, 28 pairs, all distinct
- the comparison reports a render against itself as **identical** — otherwise it measures noise
- per-style PNGs written for eyeball review; numerically different is necessary, not sufficient

### [05] Engine and operators — the exhaustive one
- every `FilterOperator` × every supported type × a fixed dataset, expected row set asserted
- negative cases explicit: an operator matching everything must fail this suite, not pass quietly
- identical results under a non-invariant `CurrentCulture`

### [06] Input and accessibility
- per-control accessible **names**, not a tree walk
- every action reachable by driving keys
- hit targets ≥ 24 logical px at 100% / 150% / 200%

### [07] Exception policy
- zero bare `catch { }`
- each deleted guard's path fed the inputs it was guarding against

## Deliverable

`FilterProbe` printing `=== N passed, M failed ===`, PNGs per style, and a baseline section that
demonstrates each class of check going red — including at least one reconstruction of a pre-fix
behaviour, as the previous three harnesses did.
