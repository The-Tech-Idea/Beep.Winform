# 08 — Verification harness

Every claim in phases 01–07 is asserted here. Built as `scratchpad/ContainerProbe`, following the
`DialogsProbe` / `GridProbe` pattern that is already working.

## The rule that governs this phase

**Every visual check needs a controlled baseline, or it measures the wrong thing.**

Across the preceding programs, harness checks returned **eight** confident false verdicts. The
baseline caught all eight. Two worth carrying forward, because both would recur here:

- An accessibility check reported "0 accessible descendants — a screen reader sees an empty dialog."
  A stock `Form` holding a `Label` and a `Button` measured **0 too**: `GetChildCount()` returns -1 by
  default and MSAA enumerates through the *window* hierarchy. The traversal was measuring itself.
  Phase 06 adds an accessible tree — the same trap is waiting.
- A text-fit check reported a caption clipped by 4px, measured with `TextFormatFlags.WordBreak`
  against a control using its own flag set. **Two measurements disagreeing says nothing about which
  is right.** Rendered pixels against a stock control settled it: nothing was clipped. Phases 02–05
  are full of measurement claims; each needs pixels, not a second opinion.

A check that has never failed is not yet a check. Break the thing deliberately and confirm the check
goes red before trusting it green.

## Checks

### Ground rules (mechanical, from the dialogs harness)
- no bare `catch` / `catch (Exception)` in the folder — [07](07-exception-policy.md)
- no `SystemColors` in painting code — [06](06-painting-and-state.md)
- no method with an empty body lacking a justification comment
- no second implementation of a container type — [01](01-container-consolidation.md)

### Slot geometry — [02](02-header-metrics-and-alignment.md), [03](03-measure-draw-contract.md)
For a matrix of tab configurations (plain / icon / badge / close / pinned / icon+badge+close):
- close glyph centre-x == close slot centre-x
- close **hit** rect >= 24 logical px, at 100% / 150% / 200% DPI
- `BadgeRect` does not intersect `CloseHitRect`
- `TextRect` does not intersect `BadgeRect` or `IconRect`
- `sum(slot widths) <= measured tab width` — everything drawn was paid for in measurement
- `TextRect.Width > 0` in every configuration

### Strip layout — [04](04-tab-strip-layout.md)
- 3 / 12 / 40 tabs in a fixed strip: natural → shrunk → scrolling, in that order
- rightmost tab never crosses the utility cluster's left edge
- the active tab is never the narrowest when another tab could shrink instead
- pinned tabs keep `PinnedTabWidth` under pressure

### Vertical positions — [05](05-vertical-tab-positions.md)
- Left/Right captions render with the same glyph-band count as the Top equivalent (the clipping test
  that actually works — a band count, compared against a horizontal baseline)
- tab height in a vertical strip does not vary with caption width
- the active indicator lands on the inner edge for both Left and Right

### Painting states — [06](06-painting-and-state.md)
- pairwise render comparison across normal / hover / active / active+hover / pressed / dragging /
  pinned / badged / focused / disabled — **any two rendering identically is a failure**
- this is the check that exposed seven "distinct" tab painters producing identical pixels; it is the
  highest-value check in the harness
- focus indicator distinct from both hover and active
- legible under `SystemInformation.HighContrast`

### Accessibility — [06](06-painting-and-state.md)
- per-control accessible **names**, not a tree walk (see the trap above)
- container reports a tab-list role; each tab is named by its caption

## Deliverable

`scratchpad/ContainerProbe` printing `=== N passed, M failed ===`, plus PNGs per state and per
configuration written to a render directory for eyeball review. Baseline captured **before** phase 02
begins, so every later phase can be diffed against it.
