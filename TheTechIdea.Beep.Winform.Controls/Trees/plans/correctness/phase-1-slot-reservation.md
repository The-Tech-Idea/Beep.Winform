# Phase 1 — Slot Reservation & Per-Level Alignment

**Goal:** every node on a given level starts its text at the same X, regardless of whether that node
has children, an icon, or a checkbox.

**Primary file:** `Trees/BeepTree.Layout.cs` → `RecalculateLayoutCache()`

---

## 1.1 Expander slot — DONE

The reported bug. `currentX` advanced past the expander only for nodes with children, so leaves sat
one box-width left of their expandable siblings.

Fixed by reserving the slot for every node and drawing the glyph only when there are children:

```csharp
bool hasChildren = nodeInfo.Item.Children?.Count > 0;
nodeInfo.ToggleRectContent = hasChildren
    ? new Rectangle(currentX, y + (nodeInfo.RowHeight - boxSize) / 2, boxSize, boxSize)
    : Rectangle.Empty;
currentX += boxSize + 4;      // always
```

`ToggleRectContent` stays `Empty` for leaves, so painters draw no glyph and hit-testing still
rejects toggle clicks on leaves — only the cursor advances.

**Verified.** Probe after the fix:

```
level 0: distinct text X = [18]  ok
level 1: distinct text X = [34]  ok
level 2: distinct text X = [50]  ok
level 3: distinct text X = [66]  ok
```

Standard trees (Explorer, VS Code) reserve this slot unconditionally, which is why leaves there line
up under their siblings' labels rather than under their siblings' expanders.

---

## 1.2 Icon slot — DONE

Identical defect, still present in the same method:

```csharp
if (!string.IsNullOrEmpty(nodeInfo.Item.ImagePath))
{
    nodeInfo.IconRectContent = new Rectangle(currentX, ...);
    currentX += imageSize + 4;      // only nodes WITH an icon advance
}
else
{
    nodeInfo.IconRectContent = Rectangle.Empty;
}
```

In any tree where some nodes have icons and some do not — a folder tree with typed leaves, for
instance — text is ragged within a level by `imageSize + 4`.

This one carries a design decision, which is why it was not swept into 1.1: reserving the slot
unconditionally would indent every label in a **text-only** tree by ~20px of permanently empty
space, changing the look of trees that are currently fine.

**Recommended:** add an `IconSlotMode` property, defaulting to the "any node" behaviour:

| Mode | Behaviour |
|------|-----------|
| `WhenAnyNodeHasIcon` *(default)* | Reserve on every row if at least one visible node has an icon. Text-only trees are unaffected; mixed trees align. |
| `Always` | Reserve unconditionally. For hosts that populate icons lazily and don't want reflow when they arrive. |
| `Never` | Never reserve; icons overlap-free only if all nodes have one. Escape hatch for compact trees. |

Implemented as `IconSlotMode` (`Trees/Models/IconSlotMode.cs`) with the property on
`BeepTree.Properties.cs`, and a single `AnyVisibleNodeHasIcon()` pre-pass in
`RecalculateLayoutCache` setting one `bool reserveIconSlot` before the geometry loop. The icon
*rect* is still only produced for nodes that have an image — only the width is reserved — so
painters and hit-testing are unchanged.

**Correction to the original plan.** This document previously warned that lazily-loaded icons
(`BeepTreeAsyncImageLoader`) could flip "any node has an icon" from false to true after layout and
reflow every row. That concern is unfounded as implemented: both `AnyVisibleNodeHasIcon()` and the
per-row decision key off `SimpleItem.ImagePath`, which the host sets up front — the async loader
only fetches pixels for a path that is *already* declared, so the flag never changes as images
arrive. The real (and ordinary) case is a host assigning `ImagePath` after layout, which needs a
layout pass for the same reason changing `Text` does.

---

## 1.3 Checkbox slot — VERIFIED

`ShowCheckBox` is tree-wide and advances `currentX` unconditionally, so it was *believed* correct.
Now confirmed rather than assumed: every `checkbox=True` case in the matrix passes, with text X
shifting uniformly by the box width (`[18,34,50,66]` → `[36,52,68,84]`).

---

## 1.4 Regression assertion — DONE

The probe asserts instead of printing. Three invariants per case:

1. one distinct `TextRectContent.X` per level;
2. indent strictly increases with depth;
3. no two rects within a row (toggle / checkbox / icon / text) intersect.

Run over `{icons on, off} × {checkbox on, off} × {WhenAnyNodeHasIcon, Always, Never}` — 12 cases:

```
PASS  icons=False checkbox=False mode=WhenAnyNodeHasIcon  textX=[18,34,50,66]
PASS  icons=False checkbox=True  mode=Always              textX=[60,76,92,108]
PASS  icons=True  checkbox=False mode=WhenAnyNodeHasIcon  textX=[42,58,74,90]
PASS  icons=True  checkbox=False mode=Never               textX=[42,58,50,90]  (ragged by design)
...
ALL ALIGNMENT ASSERTIONS PASSED
```

**The invariant is per-mode, not universal.** `Never` opts out of reserving, so a tree with *some*
icons is expected to be ragged under it — the first run flagged those two cases as failures, which
was the assertion being wrong, not the control. The probe now encodes the contract each mode
actually offers rather than one blanket rule.

DPI 150%/200% is still not covered — it belongs with the rest of the DPI work in
[Phase 5](phase-5-verification.md), since `GetScaled*` rounding is where off-by-one indent appears.

---

## Exit criteria

- [x] Expander slot reserved for every node; probe reports one distinct text X per level
- [x] Icon slot policy implemented behind `IconSlotMode`, defaulting to `WhenAnyNodeHasIcon`
- [x] Async icon arrival — investigated; no action needed (see correction in 1.2)
- [x] Checkbox combination verified rather than assumed
- [x] Alignment invariant asserted across icons × checkbox × slot-mode
- [ ] Same assertions at 150% / 200% DPI
