# Phase 3 — Text Rectangle, Truncation & Content Width

**Goal:** labels render in full or ellipsise deliberately; horizontal extent is honest.

---

## The finding

Node text is visibly clipped mid-word in the probe render — `Root (paren`, `L1 paren`, `L3 lea` —
with no ellipsis. The label just stops.

The text rectangle is built as:

```csharp
nodeInfo.TextRectContent = new Rectangle(
    currentX,
    y + (nodeInfo.RowHeight - nodeInfo.TextSize.Height) / 2,
    nodeInfo.TextSize.Width + 10,
    nodeInfo.TextSize.Height);
```

so the rect is measured-width + 10 and should fit.

## What it actually was — RESOLVED

The rect was never the problem. Measured directly (same font and flags the control uses), every
rect had 10px to spare:

```
Root (parent)   rectW= 78 neededW= 68  fits
L1 parent       rectW= 58 neededW= 48  fits
L2 leaf B ...   rectW=273 neededW=263  fits
```

**The painter was drawing with a different font than the layout measured with.**

- Layout measures with the *themed* font — `BeepTree.GetNodeFont()` → `Segoe UI 8pt`.
- `AntDesignTreePainter` drew with `_regularFont`, snapshotted in its constructor as
  `owner?.TextFont` → `Arial 10pt`.

`UseThemeFont` defaults to **true**, so the two always diverged. A 10pt Arial string drawn into a
rectangle sized for 8pt Segoe UI overflows and is clipped mid-word — and because the painters also
omitted `EndEllipsis`, it was a hard cut with no indication anything was missing.

Two things made this take longer than it should have:

1. **The default `TreeStyle` is `AntDesign`, not `Standard`.** The first fix went into
   `StandardTreePainter`, which was not the active painter — the render did not change, which was
   confusing until the probe was asked to simply print `GetCurrentPainter().GetType().Name`. Ask the
   object what it is before theorising about it.
2. The flag mismatch (`NoPadding` at measure, omitted at draw) was real and worth fixing, but it was
   not the visible cause. Fixing a real defect that is not *the* defect still leaves the symptom.

### Fixed

- `BaseTreePainter.NodeTextFlags` — one constant used by measurement and drawing, including
  `NoPadding` (matching) and `EndEllipsis` (graceful truncation).
- `BaseTreePainter.DrawNodeLabel(...)` — draws a node label with the font the layout measured it
  with. Every painter should render node text through this.
- `BeepTree.GetNodeFont()` — single font resolution. Layout used `ToFont`, `BaseTreePainter` used
  `ToFontForControl` (DPI-scaled), painters used their own snapshot; now all one method.
- `AntDesignTreePainter` (the default) and `StandardTreePainter` converted.
- Painter and font hoisted out of the per-node layout loop — see
  [Phase 4](phase-4-paint-efficiency.md), item 4.1, now done.

### The painter sweep — DONE

All 25 painters that draw node text now route through `DrawNodeLabel`. Node-label calls were
identified by their text argument (`node.Item.Text` and the `text` parameter of `PaintText`
overrides) rather than by rewriting every `DrawText`, leaving badge, count and subtitle draws alone.

**The fix is not "one font for everything".** Three styles deliberately use a distinctive label
font, and flattening them onto the tree font would have erased their identity:

| Style | Label font | Handling |
|---|---|---|
| `VercelClean` | `_monoFont` | overrides `GetNodeFont` |
| `FileBrowser` | `_compactFont` | overrides `GetNodeFont` |
| `PillRail` | bold when selected | overrides `GetNodeFont` to report the **bold** (widest) variant, and draws through the font-taking `DrawNodeLabel` overload |

So `ITreePainter` gained `GetNodeFont(BeepTree owner)`: the painter declares the font it will draw
labels with, and the layout measures the text rect with *that* font. Measurement follows the
painter instead of every painter being forced to follow the measurement.

`PillRail` also stopped allocating and disposing a bold `Font` on every selected row, every frame;
it is cached now.

## Findings from the contact sheet — both fixed

Rendering all 25 styles at once (Phase 5's contact sheet, pulled forward to verify this sweep)
confirmed labels render in full everywhere, and surfaced two unrelated defects. Both are now fixed.

### `FigmaCard` rendered completely blank

A misplaced closing brace. `PaintNode` opened `if (isSelected || isHovered)` to draw the card
background, and that block never closed until the end of the method — so the toggle, checkbox, icon,
eye and label were **all** conditional on the node being selected or hovered. An idle FigmaCard tree
drew nothing at all.

The tell that this was a brace slip rather than intent: STEP 2 immediately re-tests the *same*
condition. The drag handle is meant to appear only on hover/selection, which is only meaningful if
the surrounding block does not. The block now closes after the card background; everything else is
unconditional.

### `StripeDashboard` truncated every label

The same defect class this whole plan is about — a painter making room by shrinking a rectangle the
layout sized without knowing about it:

```csharp
Rectangle adjustedTextRect = new Rectangle(
    textRect.X, textRect.Y,
    Math.Max(0, textRect.Width - MetricBadgeWidth),   // 40px gone
    textRect.Height);
```

The layout sizes `TextRectContent` as measured-text + 10, so subtracting 40 left almost nothing and
labels ellipsised to `Root ...`. The grey blobs were the `99+` metric badge itself.

Fixed the same way as `GetNodeFont`: the painter *declares* what it needs and the layout provides
it. `ITreePainter.GetLabelTrailingReserve()` returns 0 by default; `StripeDashboard` returns
`MetricBadgeWidth`, the layout adds it to the text rect, and the painter's subtraction then leaves
the label exactly its measured width.

While in there: the badge rect was built from raw **content** coordinates while every other element
in that method is transformed to viewport, so the badge ignored scroll position and drifted away
from its row. Now transformed like everything else.

## Work items

1. Reproduce at several control widths and font sizes to establish whether clipping is
   width-dependent or constant. Constant ⇒ measure/draw flag mismatch.
2. Make measurement and drawing share one flags constant. Any place that measures must use the same
   `TextFormatFlags` the painter draws with; today the flags are chosen independently in
   `BeepTree.Layout.cs`, `BeepTreeLayoutHelper.MeasureText`, and each painter.
3. Add deliberate truncation: `TextFormatFlags.EndEllipsis` when the row is narrower than the text,
   so a long label degrades to `Long node na…` rather than a hard cut.
4. Add a tooltip (or reuse the existing tooltip infrastructure) for nodes whose text is ellipsised,
   which is what every commercial tree does.
5. Revisit content width. `RowWidth` is content-only by deliberate choice:
   ```
   // Forcing row width to viewport width causes virtual width inflation and
   // can incorrectly force a horizontal scrollbar when vertical is visible.
   ```
   That comment records a real past bug — preserve the intent. But selection/hover backgrounds that
   should span the full row need a *separate* "row band" rect; do not widen `RowWidth` to get it.

## Exit criteria

- [x] Cause of clipping identified by experiment, not assumption — rects measured as fitting, so
      the fault was on the draw side; confirmed by printing the active painter
- [x] One shared `TextFormatFlags` constant (`BaseTreePainter.NodeTextFlags`) used by measurement
      and by the converted painters
- [x] One shared font resolution (`BeepTree.GetNodeFont`) used by measurement and drawing
- [x] Long labels ellipsise rather than cut
- [x] Verified by render: every label renders in full, including a deliberately over-long one
- [x] All 25 painters converted to `DrawNodeLabel`, with `ITreePainter.GetNodeFont` so styles with a
      distinctive label font keep it and the layout measures with it
- [x] All 25 styles rendered to a contact sheet and reviewed — labels render in full everywhere
- [ ] Tooltip on truncated labels
- [ ] Horizontal scrollbar still does not appear spuriously when the vertical one is visible
