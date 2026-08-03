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
- [x] Drop between two tabs to choose the insertion index — done, see below
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
- [x] Drop between two tabs to choose the insertion index — done, see below
- [ ] Centre and edge targets over the hovered *group* — needs [01](01-split-editor-groups.md)
- [ ] Hover growth animation on the guide targets, respecting reduced-motion
- [ ] Guides target the monitor under the cursor — needs [03](03-multi-monitor-floating.md)

Three of the five depend on features not yet built, which is why this one stops here rather than
being carried further now.

---

## Outcome — tab-index drop

`DockDropResult.InsertIndex` already existed, was already plumbed through to `CommitCenterStack`,
and `CommitDragCenterStack` already honoured it via `MovePanelToIndex`. The only missing piece was
the computation: `DockTargetResolver` set it to **`-1` unconditionally**, so every drop appended no
matter where the user aimed. Declared, carried, honoured — and never once computed.

### Where the computation belongs

Not in the resolver. It works from the layout result, which carries group rectangles but no tab
geometry; the strip's rectangles live on the panels (`DockPanel.TabBounds`, mirrored there by the
caption layout). So the host answers instead, through a new `IDockDragHost.ResolveTabInsertIndex`
— the interface is `internal` with one implementer, so extending it costs nothing.

The comparison is against each tab's **midpoint**, not its leading edge. Using the leading edge
would make the last position unreachable: there is no tab to the right of the final one to drop in
front of.

`DockDragSession` now records the screen point its current target was resolved at. Reading
`Cursor.Position` at commit time would have answered a slightly different question — where the mouse
is *now*, rather than where the drop was resolved.

### Measured

```
tabs:  a: x -2392..-2232   b: x -2232..-2072   c: x -2072..-1912
drop before first  -> 0
drop on 2nd's left -> 1
drop on 3rd's left -> 2
drop past the last -> -1  (append)
distinct indices across four drop points: 4
order a,b,c -> moving 'c' to index 0 -> c,a,b
```

The assertion that matters is the fifth: **different drop positions produce different indices**. A
resolver that always returned `0`, or always `-1`, satisfies "an index came back" — which is exactly
what shipped. The baseline asserts the all-same case is not what happens.

`DockProbe`: **195 passed, 0 failed**. Docking suite 48/48. Solution 0 errors.

### Remaining in this feature

- [ ] Hover growth animation on the guide targets, respecting reduced-motion
- [ ] Centre and edge targets over the hovered *group* — the group-edge path exists
      ([01](01-split-editor-groups.md)); what is missing is the rosette rendering over a group
      rather than the host
