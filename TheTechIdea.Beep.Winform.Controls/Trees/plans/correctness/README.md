# BeepTree — Layout Correctness & Architecture Plan

This plan is **not** a replacement for `Trees/plans/phase-1..6-*.md`. That series tracks *features*
(data binding, multi-column, enterprise capability) and is 50/320 complete. This series tracks
*correctness and architecture* of the layout and paint path — the things that make the control
render wrong or slowly regardless of which features exist.

It was written from a reproduction, not from reading: `scratchpad/TreeProbe` builds a tree that
mixes expandable nodes and leaves at every level, dumps the computed rectangles per node, and
renders the control to PNG.

## Why this exists

Reported: *"when I create the second level of BeepTree node it's aligning incorrectly."*

Reproduced exactly. `BeepTree.RecalculateLayoutCache` advanced the running X cursor past the
expander **only for nodes that have children**:

```csharp
if (nodeInfo.Item.Children?.Count > 0)
{
    nodeInfo.ToggleRectContent = new Rectangle(currentX, ...);
    currentX += boxSize + 4;      // parents advance
}
else
{
    nodeInfo.ToggleRectContent = Rectangle.Empty;   // leaves do not
}
```

So a leaf started its icon and text one box-width to the left of an expandable sibling on the same
level. With `IndentWidth = 16` and `BoxSize = 14`, a leaf at level N landed within 2px of a parent
at level N-1 — children rendered at their own parent's indent, so the hierarchy read wrong:

```
node             lvl  toggleX   iconX   textX   expected
Root (parent)      0        0       -      18        18
L1 parent          1       16       -      34        34
L2 parent          2       32       -      50        50
L3 leaf            3        -       -      48        66   <-- MISALIGNED
L2 leaf A          2        -       -      32        50   <-- MISALIGNED
L2 leaf B          2        -       -      32        50   <-- MISALIGNED
L1 leaf            1        -       -      16        34   <-- MISALIGNED

level 1: distinct text X = [16, 34]   <-- 2 alignments on one level
level 2: distinct text X = [32, 50]   <-- 2 alignments on one level
```

The invariant this plan defends: **every node on the same level starts its text at the same X**.

## Phases

| Phase | Title | Status |
|-------|-------|--------|
| 0 | Stability — accessibility teardown crash | **Done** |
| [1](phase-1-slot-reservation.md) | Slot reservation & per-level alignment | **Done** except DPI sweep |
| [2](phase-2-single-layout-engine.md) | Collapse the two competing layout engines | **Done** except multi-column eyeball |
| [3](phase-3-text-metrics.md) | Text rectangle, truncation & content width | **Done** except tooltips |
| [4](phase-4-paint-efficiency.md) | Layout/paint hot-path efficiency | 4.1 done; rest not started |
| [5](phase-5-verification.md) | Permanent verification harness | Assertions + contact sheet built; probe not promoted |

### Defects found along the way

| Defect | Where | Status |
|---|---|---|
| `FigmaCard` rendered completely blank — an `if (isSelected \|\| isHovered)` block never closed, making every element conditional on hover/selection | `FigmaCardTreePainter` | **fixed** |
| `StripeDashboard` truncated every label — painter shrank the text rect by 40px for a badge the layout never reserved | `StripeDashboardTreePainter` | **fixed** via `GetLabelTrailingReserve` |
| `StripeDashboard` badge drawn in content coordinates while everything else is viewport-transformed, so it drifted on scroll | `StripeDashboardTreePainter` | **fixed** |
| Multi-column geometry now produced but never eyeballed | `BeepTree.CalculateMultiColumnCells` | needs a look |
| `VirtualizeLayout` is nominal — every node laid out eagerly | `BeepTree.RecalculateLayoutCache` | implement or retire the property |

### Phase 0 — accessibility teardown crash (done)

`BeepTreeAccessibleObject` derived from plain `AccessibleObject`. WinForms stores whatever
`CreateAccessibilityInstance()` returns in a property-store slot typed `ControlAccessibleObject`,
so the tree threw `InvalidCastException` inside `Control.OnHandleDestroyed` — on form close,
re-parent, or theme change — but only once something had touched its accessibility object. Now
derives from `Control.ControlAccessibleObject`. Every other override in the assembly (Grid,
CheckBox, Panel, TextBox, MenuBar) was already correct.

## Standing rules for this area

1. **Reserve the slot, don't conditionally advance.** Any element that only *sometimes* draws
   (expander, icon, checkbox) must still reserve its width, or siblings misalign. This is the same
   defect class fixed in the grid's column header sort indicator.
2. **One layout engine.** Geometry must have exactly one implementation. See Phase 2.
3. **Verify by render.** A layout change is not done until the probe reports one distinct text X per
   level and the PNG has been looked at.
4. **A grep is not a call-site census.** Phase 2 was written around the claim that a method had no
   callers, from a `grep layoutHelper\.[A-Za-z]*` that could not match `_layoutHelper?.Method()` —
   the null-conditional breaks the pattern. There were three callers, two of them actively
   installing wrong geometry. Delete the symbol and let the compiler enumerate the truth.
5. **Ask the object what it is.** Phase 3's clipping fix first went into `StandardTreePainter` on the
   assumption that "Standard" is the default style. It is not — the default `TreeStyle` is
   `AntDesign`. One `Console.WriteLine(GetCurrentPainter().GetType().Name)` would have saved a
   round trip. Print the active implementation before editing an implementation.
6. **Measure and draw with the same font and the same flags.** Layout sizes a rect from a measured
   string; if any painter draws that string with a different font or different `TextFormatFlags`,
   the glyphs will not fit the rect that was built for them. Route node text through
   `BaseTreePainter.DrawNodeLabel`.
