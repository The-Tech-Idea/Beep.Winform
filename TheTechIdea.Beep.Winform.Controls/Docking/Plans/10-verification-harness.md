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
