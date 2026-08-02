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
