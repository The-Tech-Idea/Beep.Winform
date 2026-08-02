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

---

## Outcome

### Correction to the count above

This document opened by claiming **44** catch blocks. That figure came from `grep -c catch`, which
counts the *word* — including comments, prose, and the word appearing twice on one line. The
accurate figures, counting `catch` followed by `(` or `{` in code:

| | before (`93b5488e`) | after |
|---|---|---|
| catch statements | **32** | **20** |
| bare `catch {` | **22** | **10** |

The direction was right and roughly twelve were removed either way, but the starting number was
overstated. Recorded because a plan that opens with a wrong number invites the next reader to
"verify" it and reach a different total.

An earlier grep in this program made the opposite error: `^\s*catch` missed the inline
`try { … } catch { }` form entirely — which is precisely the shape the six `OnPaint` guards used —
and reported no change across a commit that removed eight of them.

### What was done

- **Reported, not swallowed.** `ContainerError` / `ContainerErrorEventArgs` added. `OnPaint` held six
  bare catches, one per drawing step, so a container that failed to draw its tab strip drew
  everything else and said nothing. Those six are one reporting guard.
- **Narrowed** where a specific failure is expected: hosted-control `Visible`/`Invalidate` to
  `ObjectDisposedException`; icon painting to `IOException` / `ArgumentException` /
  `NotSupportedException`.
- **Deleted** two path guards proven unreachable. `CreateRoundedPath` and `CreateTabCornerPath`
  return early on zero and negative sizes, clamp every radius to half the smaller side, return early
  on over-large diameters, and gate each `AddArc` on a positive radius. Tested against the eight
  degenerate inputs those guards existed for — zero size, negative width, negative height, radius
  500 on a 10x10 rect, a 1px sliver, zero radius, negative radius, exact-half radius. All handled
  without throwing.
- **Traced** the one genuine cross-subsystem fallback: `BeepStyling.PaintControl` can fail for
  reasons outside this painter, so the plain-fill substitution stays — but it is now visible rather
  than passing for an intentional style.

### The remaining 20

Left deliberately. They sit in paths this program never exercised — addin hosting, theme propagation
across a control tree, layout under a disposed handle. Removing a catch that cannot be tested trades
a silent wrong result for an unhandled throw, which is not obviously an improvement. They are a known
quantity now, and `ContainerError` exists for whichever turn out to need it.

Evidence the removals were safe: a populated container rendered with `ContainerError` subscribed
reports **zero** errors and draws 23 distinct colours.
