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

---

## Outcome

`scratchpad/FilterProbe` — **48 checks, 0 failing**, across seven groups:

| group | phase | what it asserts |
|---|---|---|
| display modes | 01 | each `FilterDisplayMode` distinguishable from `AlwaysVisible` |
| configuration surface | 01 | the inert enum and property are gone |
| painter distinctness | 02 | 28 style pairs, none identical |
| measurement coverage | 03 | badge and modified marker widen the tab, not the caption |
| engine operators | 05 | 21 cases, exact expected row sets, 17/17 operators |
| filter system agreement | 04 | clearing via either route leaves the grid consistent |
| accessible tree | 06 | criteria named and readable |
| keyboard focus | 06 | Tab / Shift+Tab, and the ring reaching pixels |
| hit targets | 06 | remove affordance ≥ 24px |
| ground rules | 09 | mechanical: bare catches, unreferenced enum values |

### The mechanical checks earned their place immediately

The enum-value check found two more on its first run: `FilterSuggestionType.Template`
("predefined template") and `.Smart` ("AI-suggested"), declared and never produced or consumed. The
provider advertised five suggestion kinds and generated three.

Both removed. An enum value nothing can emit is not a partial implementation of a feature — it is a
claim the provider does not honour, and a caller switching on it writes an unreachable branch.

That brings this folder's declared-but-never-read tally to **seven**: `FilterPosition`, three
`FilterDisplayMode` values, two painter capability flags, `FocusedFilterIndex`, `BeepFilter.DataSource`,
and now two suggestion types. A mechanical check finds these in a second; reading for them does not.

### Every check has been shown able to fail

Not asserted — demonstrated, and several were caught being wrong:

| check | how it was shown to discriminate |
|---|---|
| display modes | `AlwaysVisible` asserted **not** distinguishable from itself |
| painter distinctness | a render compared against itself must report identical |
| measurement coverage | the pre-fix measurement reproduced: caption squeezed to 33px vs 57px |
| filter systems | the defect demonstrated first — 5 rows visible with `IsFiltered` still true |
| accessible tree | 0 criteria compared against 2, so the count cannot be a constant |
| focus ring | first run reported **0.00%**, which is what exposed the layout-recalculation flaw |

### Checks that were wrong before they were right

Recorded because the ratio matters: of the failures this harness produced, **six were the test and
five were the code**.

- `caption > 0` — passed under the exact defect it existed for
- "12 tabs shrink rather than scroll" — miscalibrated; 12 genuinely need scrolling
- "the widest tab shrinks" — fails precisely when the design works
- `Regex`, `In`, `NotIn`, `IsNull` — four engine cases asserting behaviour the engine never claimed
- criteria added by mutating the list — never recalculated the layout, so the painters rendered
  their empty state and phase 02's first result measured nothing

Each was checked against the implementation before being called a defect. That step is the harness.
