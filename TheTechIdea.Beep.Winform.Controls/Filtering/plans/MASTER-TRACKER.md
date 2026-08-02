# Filtering — enhancement program: master tracker

`TheTechIdea.Beep.Winform.Controls/Filtering/` — 27 files, 9,819 lines. `BeepFilter` builds a
`FilterConfiguration` and raises `FilterApplied`; consuming controls apply it. That contract is real
and works — this program is about the surface around it that does not.

## Status

| # | Phase | Scope | Status |
|---|---|---|---|
| [01](01-dead-configuration-surface.md) | Dead configuration surface | `FilterPosition`, `FilterDisplayMode` | ☑ **done** |
| [02](02-painter-distinctness.md) | Painter distinctness | the 8 `IFilterPainter` implementations | ☑ **done** |
| [03](03-beepfilter-decomposition.md) | `BeepFilter` decomposition | 1,358-line partial | ☑ **done** |
| [04](04-filter-systems-duplication.md) | Competing filter systems | `Filtering/` vs `GridX` | ☑ **done** |
| [05](05-engine-and-operators.md) | Engine and operator coverage | `FilterEngine`, `FilterOperator` | ☑ **done** |
| [06](06-input-and-accessibility.md) | Keyboard, autocomplete, a11y | popup, keyboard handler | ☐ not started |
| [07](07-exception-policy.md) | Exception policy | folder-wide | ☐ not started |
| [08](08-documentation-accuracy.md) | Documentation accuracy | `README.md` | ☐ not started |
| [09](09-verification-harness.md) | Verification harness | `scratchpad/FilterProbe` | ☐ not started |

Mark a phase done only when its verification section passes **and** the harness has been shown
capable of failing on the defect the phase fixed.

## Ground rules

Carried from the Tabs, ToolTips, DialogsManagers and DisplayContainers programs:

- **No stubs, no legacy, no fallback.** One implementation per concept.
- **No swallowed exceptions.** Absorb only where a failure must not propagate, and report it.
- **No duplication.**
- **Measure before claiming.** Every visual assertion needs a controlled baseline.

## Two rules this program adopts from mistakes made in the previous ones

**Count within the right boundary.** A first pass here reported "17 `FilterStyle` values but only 8
painters". `FilterStyle.cs` declares *three* enums — `FilterStyle` (8), `FilterDisplayMode` (5),
`FilterPosition` (5) — and the count had spanned all three. `FilterStyle` is in fact complete: 8
values, 8 painters, all registered. The real defect was elsewhere, and stating the wrong one would
have sent the work in the wrong direction.

**A grep filter can exclude the evidence.** Checking which `FilterDisplayMode` values are consulted,
an exclusion of `= FilterDisplayMode` also removed every `== FilterDisplayMode` comparison, making a
live value look dead. Both figures in [01](01-dead-configuration-surface.md) were re-derived with
comparison-only greps.

**Deletion plus a clean compile is the authoritative test for deadness.** Grep is not — receiver-less
internal calls are invisible to `\.Method(`, and API used only by a *sibling repository* is invisible
to a search of this one. Both have already caused wrong conclusions in this codebase.

## Cross-repo consumers

`Filtering` is consumed by `GridX` in this repository, and the wider solution is consumed by
`Beep.Winform.Data.Integrated` and `Beep.Sample`. Any change to a public signature must be swept
across **all** sibling repos under `source/repos/The-Tech-Idea`, not just this one. Removing four
`[Obsolete]` aliases from `BeepDialogManager` after checking only this repository broke nine call
sites in the integrated app.
