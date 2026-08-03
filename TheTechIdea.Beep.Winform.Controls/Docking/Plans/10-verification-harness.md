# 10 — Verification harness

`scratchpad/DockProbe`, following the `DialogsProbe` / `GridProbe` / `ContainerProbe` / `FilterProbe`
pattern that is already working across four programs.

Built **before** the features, not after. Features 01–04 all mutate the layout tree; without a way to
capture and compare a tree, "the layout came back correctly" is an opinion.

## The rule that governs this phase

**Every check must be shown capable of failing before its pass means anything.**

Across the preceding programs, harness checks returned **fourteen** confident false verdicts. Every
one was caught by putting a controlled baseline behind it. The classes that will recur here:

- **A check too weak to catch its own defect.** `caption > 0` passed under the exact bug it existed
  for, because the old behaviour also left a positive — merely squeezed — caption.
- **A check that fails when the design works.** An assertion that the *widest* tab shrinks went red
  precisely because the widest was the active one, deliberately preserved.
- **A traversal measuring itself.** "0 accessible descendants" — a stock `Form` measured 0 too,
  because `GetChildCount()` returns -1 by default and MSAA walks the window hierarchy.
- **Two measurements disagreeing.** A 4px "clip" was two `TextRenderer` calls with different
  `TextFormatFlags`. Neither was wrong; the comparison was meaningless. Pixels settled it.
- **A search crossing a boundary.** Enum values counted across three declarations in one file; a
  property name colliding with an identically-named one in another subsystem; a `timeout`
  truncating a cross-repo grep into a false "no consumers".

## The core primitive: capture and compare a layout tree

Most of this program's verification reduces to *did the arrangement come back exactly?* So the first
thing to build is a deterministic serialisation of the live layout — nodes, orientations, ratios,
active tabs, float bounds — and an exact comparison.

With that, these become one-liners:

| feature | assertion |
|---|---|
| [01](01-split-editor-groups.md) | split then collapse; tree equals the original |
| [02](02-layout-perspectives.md) | A → B → A; tree equals A |
| [04](04-maximise-and-zen.md) | maximise then restore; tree **identical**, ratios to the pixel |
| [05](05-drop-guides-and-preview.md) | drag then `Esc`; tree identical to the pre-drag capture |
| [07](07-persistence-and-migration.md) | save → load → save; serialised forms byte-identical |
| [08](08-manager-decomposition.md) | same results before and after each extraction |

## Checks by feature

### Structural — [09](09-dead-surface.md), [08](08-manager-decomposition.md)
- zero bare `catch { }`
- no `public` enum value that is never compared
- no `[Browsable]` property whose value never reaches behaviour
- no partial much over 400 lines

### Visual — [05](05-drop-guides-and-preview.md)
- every `DockPosition` guide renders differently from the others and from no-hover
- the preview rectangle equals the rectangle the drop actually produces, within 1px
- the comparison reports a render against itself as identical

### Layout — [01](01-split-editor-groups.md), [04](04-maximise-and-zen.md)
- tree round-trips through split/collapse and maximise/restore
- ratios survive a host resize within 1px
- a split that would breach the minimum group size is refused, not clamped to 3px

### Persistence — [03](03-multi-monitor-floating.md), [07](07-persistence-and-migration.md)
- byte-identical save → load → save
- unknown panel id, truncated file, future version, absent monitor: four defined outcomes
- off-screen float bounds clamped so the caption is reachable
- **the monitor set must be an input, not an ambient fact**, or none of the multi-monitor cases can
  be tested headlessly

### Accessibility — [06](06-keyboard-and-accessibility.md)
- per-panel accessible names, with an empty host compared against a populated one
- every operation reachable by driving keys

## Deliverable

`DockProbe` printing `=== N passed, M failed ===`, PNGs for the guide overlay states, and a baseline
section demonstrating each class of check going red — including at least one reconstruction of a
pre-fix behaviour, as all four previous harnesses did.

---

## Outcome

`scratchpad/DockProbe` — built before the features, as this document required.

### The capture primitive

A deterministic textual form of the live layout — panel key, state, dock position, bounds — ordered
by key so two captures of the same arrangement are byte-identical. With it, *"did the layout come
back?"* is a string comparison rather than an opinion.

Three properties are asserted about the primitive itself before anything is asserted with it:

- it captures the panels that exist
- capturing twice without touching anything produces the same string
- **a change is visible to it** — hiding a panel must change the capture, or it cannot detect a
  layout that came back wrong. Asserted as an `ExpectFail`, since a capture that ignores changes
  would otherwise pass every later test silently.

### It found a defect on its first run

Hide a panel, then show it again, and the layout does not come back:

| panel | original | after hide → show |
|---|---|---|
| explorer | 249 x **447** | 249 x **600** |
| output | 900 x **149** | **no bounds at all** |
| props | 179 x **447** | 179 x **600** |

`ShowPanel` restores the panel's `State` and `DockPosition` — the capture shows `output:Docked:Bottom`
— but the panel is given no bounds, and its siblings never yield the space back.

`HidePanel` removes the panel from its **parent control** (`ds.Controls.Remove(panel)`) and never
from its **layout group**, so `panel.Group` stays non-null. `ShowPanel`'s `if (panel.Group == null)`
guard is therefore false and the re-join branch is skipped; the panel is marked docked while the
layout has no live allocation for it.

**Not fixed here.** The mechanism above is read from the code and consistent with every observed
value, but the fix is in the layout-group lifecycle — the same area features
[01](01-split-editor-groups.md) and [04](04-maximise-and-zen.md) will rework. Fixing it in isolation
now risks doing it twice. It is recorded with a reproducing check that currently fails, which is a
better handoff than a fix nobody verified.

### Ground rules, enforced mechanically

- bare `catch` statements: **0**
- enum values **51**, unreferenced **2** — both allow-listed with the reason from
  [09](09-dead-surface.md), since deleting either would be wrong
- `[Obsolete]` shims: **0**
- manager partial sizes reported, largest **2,742** — recorded rather than asserted, because
  [08](08-manager-decomposition.md) deliberately stopped at four extractions. It becomes an assertion
  when the remaining five seams are taken.

### One harness bug worth recording

The console formatter was written three times with an escaped newline that became a literal one,
breaking the build each time. It now goes through a single `Flat()` helper. Trivial, but it is the
fourth time in this session that inline string escaping in a generated edit produced a syntax error —
one shared helper removes the whole class of it.
