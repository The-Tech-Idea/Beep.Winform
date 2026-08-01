# 07 — 44 catch blocks

## Finding

`DisplayContainers/` contains **44** `catch` blocks:

| count | file |
|---|---|
| 13 | `BeepDisplayContainer2.Painting.cs` |
| 8 | `BeepDisplayContainer.cs` *(deleted by [01](01-container-consolidation.md))* |
| 7 | `BeepDisplayContainer2.IDisplayContainer.cs` |
| 6 | `Helpers/TabPaintHelper.cs` |
| 4 | `BeepDisplayContainer2.cs` |
| 4 | `BeepDisplayContainer2.TabManagement.cs` |
| 1 | `Helpers/TabLayoutHelper.cs` |
| 1 | `BeepDisplayContainer2.Layout.cs` |

Thirteen in the painting partial alone. A paint pass that catches its own failures cannot report a
broken container; it renders a wrong one. This is the single largest concentration of swallowed
exceptions found in any folder across these programs — the dialogs had 4, and removing them exposed
two real defects that had been hidden for as long as the catches existed.

## Two already identified elsewhere

- `TabLayoutHelper.cs:144` — replaces a failed text measurement with `title.Length * 7px`
  ([03](03-measure-draw-contract.md))
- `TabPaintHelper.cs:295` — hosts an entire alternate renderer
  ([06](06-painting-and-state.md))

## Policy

Adopted from the dialogs program, where it worked:

1. **Delete the catch** where nothing is expected to throw. Most of these guard ordinary geometry and
   drawing that cannot fail in normal operation. A genuine failure should surface.
2. **Narrow the catch** where a specific, expected failure exists (a missing icon file →
   `IOException`; a disposed handle → `ObjectDisposedException`). Catch that type, not `Exception`.
3. **Report, never swallow.** Where a container must not tear down a form's paint cycle, raise an
   error event carrying the context — the pattern used for `DialogStateStore.Error` — so the failure
   is observable rather than invisible.

`BeepDisplayContainer2` already has an events partial (`BeepDisplayContainer2.Events.cs`) and a
`ContainerEvents` payload, so route 3 has somewhere to live.

## Work

- [ ] Classify all 44 into delete / narrow / report
- [ ] Add `ContainerError` to `BeepDisplayContainer2.Events.cs` with context + exception
- [ ] Remove every `catch` that only returns or only substitutes a fabricated value
- [ ] Re-run the phase-08 harness after each file — removing a catch is exactly how hidden defects
      surface, and they should be fixed here rather than re-suppressed

## Verification

- zero bare `catch` and zero `catch (Exception)` in the folder, enforced mechanically by the harness
  (the dialogs harness has this check already and it can be pointed at this folder)
- the container still renders after an intentionally-thrown painter fault, and raises `ContainerError`
- **expect new failures.** If removing 44 catches surfaces nothing, the removal is not being tested
  hard enough
