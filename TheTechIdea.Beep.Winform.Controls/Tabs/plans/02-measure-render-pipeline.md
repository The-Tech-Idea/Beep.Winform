# 02 — Measure / Render Pipeline & "Legacy" Naming

**Priority P0.** Depends on [01](01-painter-contract.md).

## The snapshot is what gets painted — verified

The open question for this feature was whether the measure/render split is a real boundary or two
sources of geometry pretending to be one. That is the defect class that cost `BeepTree` two layout
engines and `ToolTips` three placement engines, and reading the code cannot settle it.

Measured: with three tabs and the middle one selected, the layout snapshot says
`{X=67, Y=0, Width=121, Height=30}` and the painted fill occupies exactly
`{X=67, Y=0, Width=121, Height=30}` — **Δ0px on both edges**.

So the snapshot is authoritative for geometry. Painting does not re-derive tab extents, and the
helpers that call back into the owner (`BeepTabLayoutHelper`, `BeepTabOverflowCoordinator`) are asking
for *sizes to build the snapshot from*, not producing a second set of rectangles behind it. The seam
is real; the concern this feature was opened for does not apply.

What remains under this heading is naming and ownership, not correctness: measurement lives on
`BeepTabs.Layout` while rendering lives on `BeepTabHeaderHost`. That is a structural preference, and
with fidelity now asserted there is no defect driving a move.

### Three attempts to measure it

Worth recording, because the failures were all in the measurement:

1. Sampling the tab's centre traced **2px** — the centre line is the caption, so it measured a glyph.
2. Sampling 3px from the top traced **3px** — that line is the focus ring.
3. Taking the modal colour of the tab's interior and finding its horizontal extent is indifferent to
   where text and chrome fall, and matched exactly.

The check now reports "that is not the tab fill" and declines to compare when the traced run is less
than half the tab, rather than reporting a disagreement the control is not responsible for.

## Current behaviour

Header geometry is produced by one subsystem and consumed by another:

```
BeepTabs.Layout.GetDesiredHeaderTabSizes(graphics)
    -> painter.MeasureTab(...)                       // sizes

BeepTabs.Layout.GetCurrentHeaderTabRects(graphics)   // rects
    -> BeepTabLayoutHelper.CreateSnapshot(...)       // BeepTabHeaderLayoutSnapshot
        -> BeepTabHeaderHost.SyncSnapshot()

BeepTabs.Drawing -> _headerHost.RenderLegacyHeader(graphics, CreateHeaderRenderRequest())
    -> painter.PaintTabItem(...)                     // paint
```

Three observations, in order of severity:

**The live render entry point was named `RenderLegacyHeader` — FIXED.** It is called from
`BeepTabs.Drawing.cs:78` and is the only header render path there is; nothing replaced it and nothing
was scheduled to. It is now `RenderHeader`, and the private `PaintLegacyTab` it calls is now
`PaintTabItemClipped`, which says what it does (clips to the tab bounds, cross-fades painters during
a style transition). The stale `Phase 2` markers on the models and adornment code are gone too.

This was not cosmetic. The matching *"legacy paint overload"* comment on `ITabPainter.PaintTab` is
what led the first version of this plan to declare a live method dead and propose deleting it from
all seven painters. The harness now fails on any identifier or comment that opens by declaring
something legacy, and that detector is itself self-tested against the three original strings.

**`BeepTabs.Layout.cs` owns measurement while `BeepTabHeaderHost` owns rendering.** The previous
plan's stated principle was "keep `BeepTabHeaderHost` authoritative for header layout, hit testing,
overflow, actions, keyboard, accessibility and pointer state" — but layout measurement still lives
on `BeepTabs`, and the host receives it as a snapshot. Whether that is a clean seam or a half-done
migration is the question this document has to answer before anything else is built on it.

**Consumers reach back across the seam.** `BeepTabLayoutHelper.CreateSnapshot` calls
`owner.GetCurrentHeaderTabRects()`, and `BeepTabOverflowCoordinator` calls
`owner.GetDesiredHeaderTabSizes(graphics)`. The helpers depend on `BeepTabs` internals rather than on
the snapshot, so the snapshot is not actually the boundary it appears to be.

## The risk this creates

`BeepGridPro`, `BeepTree` and `ToolTips` each shipped with two implementations of one geometry, and
in every case they disagreed:

- `BeepTree` — two layout engines, 4px apart on every node; the dormant one activated above 10,000
  nodes.
- `ToolTips` — three placement implementations; one validated a position the other then applied
  differently.

The structure here is the same shape: a measure path and a render path that must agree, connected by
a copied snapshot. Nothing currently proves they do.

## Work

1. **Rename `RenderLegacyHeader` to what it is** (`RenderHeader`). Naming that describes a
   migration which already completed is actively harmful.
2. **Decide the seam and make it real.** Either the host owns measurement too — in which case
   `GetDesiredHeaderTabSizes` / `GetCurrentHeaderTabRects` move into it — or `BeepTabs` owns
   measurement and the helpers consume the snapshot rather than calling back into the owner. Half of
   each is what makes drift possible.
3. **Assert the snapshot matches reality.** The rects the host paints into must equal the rects
   `BeepTabs.Layout` computed for the same graphics and state. A probe comparing them is cheap and
   would catch drift the moment it appears.
4. **One measurement of text.** Confirm `painter.MeasureTab` and whatever `PaintTabItem` draws with
   use the same font resolution path (`TabFontHelpers`), and that `BeepTabHeaderItemLayout` carries
   the font rather than each side resolving its own.

## Verification

- Probe: for a tab set with mixed label lengths, icons, badges and pinned states, assert
  `GetCurrentHeaderTabRects()` and the host's painted item rects are identical.
- Probe: assert the measured text width for each tab equals the width the painter draws at, with the
  same font instance.
- Render the header at 100% / 150% / 200% DPI and confirm no label is clipped — the symptom this
  class of defect produces.
