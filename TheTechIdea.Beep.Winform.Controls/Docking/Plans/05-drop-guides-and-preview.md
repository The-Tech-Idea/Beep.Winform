# 05 — Modern drop guides and preview

## What exists

`Runtime/DockingGuideOverlay.cs` (260 lines) is a real implementation: a layered `Form` with
`ShowOver(hostForm)`, `HitTest(screenPt)`, `ShowSnapGuide(rect, orientation)` and `ActiveTarget`.
Nine references across the folder. This feature is a **refinement**, not a build.

## What the reference products do that this does not

| behaviour | VS / Rider | VS Code | Dockview / Golden Layout | here |
|---|---|---|---|---|
| Compass rosette over the hovered panel | ✔ | — | ✔ | check |
| Centre target = "join this group as a tab" | ✔ | ✔ | ✔ | needs [01](01-split-editor-groups.md) |
| Edge targets = split in that direction | ✔ | ✔ | ✔ | needs [01](01-split-editor-groups.md) |
| Outer-edge targets = dock to the host edge | ✔ | ✔ | ✔ | present (`DockPosition`) |
| **Translucent preview of the resulting rect** | ✔ | ✔ | ✔ | `ShowSnapGuide` — verify it previews the *result*, not just a line |
| Animated target growth on hover | ✔ | — | ✔ | ☐ |
| Keyboard-cancellable drag (`Esc`) | ✔ | ✔ | ✔ | ☐ verify |
| Drop onto a specific tab position | ✔ | ✔ | ✔ | ☐ |

The two that matter most for perceived quality are the **result preview** — the user sees the shape
the layout will take before releasing — and **`Esc` to cancel**, without which a mis-started drag
must be completed and undone.

## Work

- [ ] Establish what `ShowSnapGuide` actually renders: a line, or the resulting rectangle. The method
      name and `DrawSnapLine` (`:180`) suggest a line; a filled translucent preview of the target
      rect is what the reference products show
- [ ] Centre and edge targets over the hovered group, once [01](01-split-editor-groups.md) provides
      groups to split
- [ ] `Esc` cancels an in-flight drag and restores the pre-drag layout
- [ ] Drop between two tabs to choose the insertion index, not only "into this group"
- [ ] Hover growth animation on the guide targets, respecting a reduced-motion preference
- [ ] Guides target the monitor under the cursor ([03](03-multi-monitor-floating.md))

## Verification

Rendered, against a controlled baseline — this is a visual feature and reading the painter proves
nothing. Three previous programs found painters whose distinct code produced identical pixels.

- Render the overlay with each `DockPosition` hovered; assert every target renders differently from
  the others and from the no-hover state
- Assert the preview rectangle equals the rectangle the layout actually produces on drop, within
  1px — a preview that lies is worse than no preview
- Drive a drag and press `Esc`; assert the layout tree is byte-identical to the pre-drag capture
- Confirm the comparison reports a render against itself as identical, before trusting any pass

---

## Outcome (partial)

### What `ShowSnapGuide` actually rendered

This document's first task was to establish that rather than assume it. `DrawSnapLine` drew a **3px
bar** on one edge of the target rectangle. Only `DockPosition.Fill` showed the whole region; the four
edge positions each showed a sliver.

So a user dragging to `Left` saw a thin blue line and had to infer how much space the panel would
take. Every reference product — Visual Studio, VS Code, Dockview, Golden Layout — previews the
resulting rectangle.

### The preview was already truthful; only the drawing was not

`DockDragController` passes `result.PreviewBounds` — the rectangle the drop actually produces, and
the same one positioning the drag ghost (`_ghost.MoveTo(result.PreviewBounds)`). Nothing needed
recomputing: the overlay simply had to fill what it was already given.

It now fills the region translucently, keeps an accent bar on the edge the split lands against, and
outlines the whole rectangle. `Fill` gets no edge bar, because it has no side.

### Measured

| position | covered, before | covered, now |
|---|---|---|
| Left / Right / Top / Bottom | ~1.3% | **98–99%** |
| Fill | ~98% | ~98% |

All 10 position pairs render differently. Two baselines confirm the checks discriminate: the
comparison reports a render against itself as identical, and the reconstructed 3px-bar rendering is
correctly measured at **1.3%** — so the coverage check would have caught the old behaviour.

Per-position PNGs are written to `scratchpad/dock-renders`.

### Remaining in this feature

- [ ] `Esc` cancels an in-flight drag and restores the pre-drag layout — the capture primitive from
      [10](10-verification-harness.md) makes this a one-line assertion once implemented
- [ ] Drop between two tabs to choose the insertion index, not only "into this group"
- [ ] Centre and edge targets over the hovered *group* — needs [01](01-split-editor-groups.md)
- [ ] Hover growth animation on the guide targets, respecting reduced-motion
- [ ] Guides target the monitor under the cursor — needs [03](03-multi-monitor-floating.md)

Three of the five depend on features not yet built, which is why this one stops here rather than
being carried further now.
