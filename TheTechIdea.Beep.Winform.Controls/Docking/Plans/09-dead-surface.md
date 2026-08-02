# 09 — Dead and duplicated surface

The cheapest phase, and the one that should run first: less code to carry into every feature that
follows.

## Finding 1 — a duplicated enum, one of them misspelled

`Models/DockingEnums.cs` declares both:

| line | enum | uses |
|---|---|---|
| 206 | `DockingPropagateAction` | **4** |
| 277 | `DockingPropogateAction` | **0** |

Same members (`Null`, `StartUpdate`, `EndUpdate`, `ShowPages`, …), same doc comments word for word.
The second is a misspelling — *Propogate* — and nothing uses it.

Two enums that mean the same thing, differing by a typo, is the worst possible shape: the next person
to need one has a 50% chance of picking the dead one, and it will compile.

**Work:** delete `DockingPropogateAction`, after a cross-repo sweep. It is `public`, so a sibling
repository could name it even though nothing here does.

## Finding 2 — eleven enums in one file

`Models/DockingEnums.cs` (342 lines) declares **eleven** enums: `DockAreas`, `DockPosition`,
`DockPanelState`, `SplitOrientation`, `HeaderPosition`, `TabStyle`, `DockingLocation`,
`DockingCloseRequest`, `DockingAutoHiddenShowState`, `DockingPropagateAction`,
`DockingPropogateAction`.

Not a defect in itself — but it is a measurement hazard, and it has already caused two false findings
in the `Filtering` program, where `FilterStyle.cs` held three enums and a value count spanned all
three. **Any "declared vs handled" count in this folder must be scoped to a single enum's body.**

`TabStyle` here is also worth checking against the `TabStyle` in the root controls namespace — a name
collision across subsystems produced a wrong "this has a caller" conclusion in `Filtering`.

## Finding 3 — the rest of the audit

To be run, not assumed:

- [ ] Every `public` enum value: is it ever compared, or only declared? `FilterPosition` was a whole
      enum plus a browsable property whose backing field was read only by its own accessors
- [ ] Every `[Browsable]` property on the docking types: does its value reach behaviour?
- [ ] Every interface member in `Interfaces/`: does anything call it? Two `IFilterPainter` capability
      flags were overridden by all eight painters and read by nothing
- [ ] `Examples/` — is it sample code that ships in the assembly? If so, it belongs in a sample
      project, not in the control library

## Exception policy, folded in

The folder holds **6** catch statements, **3** of them bare — counted as statements, not word
occurrences, because word-counting produced two wrong figures in the `DisplayContainers` program.

That is a small number, and the policy is the established one: delete where nothing can throw
(proving it by feeding the guarded path the inputs it guards against), narrow where a specific
failure is expected, report where a failure must be absorbed.

## Verification

- Deletion plus a clean compile — the authoritative deadness test. Grep is not: receiver-less
  internal calls are invisible to `\.Method(`
- **Every sibling repository builds**, not just this one
- Zero bare `catch { }` in the folder
