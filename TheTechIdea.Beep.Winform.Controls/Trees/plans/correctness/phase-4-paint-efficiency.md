# Phase 4 — Layout & Paint Hot-Path Efficiency

**Goal:** layout and repaint cost scales with what changed, not with the whole tree.

This phase applies the lesson learned in `BeepGridPro`: an unconditional full-control
`Invalidate()` on a mouse-move path is invisible in code review and obvious the moment you count
repaints. The grid was repainting 12 times for 12 mouse moves; scoping it took that to 2.

---

## Findings

### 4.1 Per-node work that should be per-pass

Inside the `for` loop over every visible node in `RecalculateLayoutCache`:

```csharp
var painter = GetCurrentPainter();                       // painter lookup, per node
Font font = UseThemeFont && _currentTheme != null
    ? ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont)   // allocates a Font
    : TextFont;
int preferredHeight = painter?.GetPreferredRowHeight(...);
font = UseThemeFont && _currentTheme != null
    ? ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont)   // allocates AGAIN
    : TextFont;
```

The painter is resolved once per node, and the theme font is constructed **twice per node**. Neither
varies across the loop. Both belong above it. On a 10,000-node tree that is 20,000 `Font`
allocations per layout pass, none of them disposed.

### 4.2 Allocation on every scroll

```csharp
var (start, end) = GetVirtualizationRange(_layoutCache.Select(n => n.Item).ToList());
```

`UpdateViewportLayout` runs on scroll and materialises a fresh `List<SimpleItem>` of the entire tree
just to compute a range. `GetVirtualizationRange` only needs counts and row heights, both already in
`_layoutCache` — it should take the cache directly.

### 4.3 Invalidate scope

Audit every `Invalidate()` in `BeepTree.Events.cs` / `Scrolling.cs` against the rule from the grid:

- Repaint only when state **actually changed** (compare before assigning hover/selection indices).
- Repaint only the **affected rows**, not the control, for hover and selection changes — a node's
  row rect is already known.

Hover repaints on a tree are per-mouse-move by nature, so this is the same hot path that produced
visible flicker in the grid toolbar.

### 4.4 Struct copies

`NodeInfo` is a struct held in `List<NodeInfo>`. Every `_layoutCache[i]` read copies it, and
`SyncFromVisibleNodes` copies the entire list element by element. Measure before changing anything —
this is a real cost at 10k nodes but the current shape is *correct*, and correctness beat cleverness
here before (the code carries an explicit comment about writing structs back). Only revisit after
Phase 2 has collapsed the engines.

## Sequencing

Do **not** start this phase before Phase 2. Optimising two competing layout engines means doing the
work twice and choosing between them under time pressure.

## Exit criteria

- [ ] Painter and theme font resolved once per layout pass, not per node
- [ ] Theme fonts disposed or cached rather than leaked per node
- [ ] `GetVirtualizationRange` consumes the cache without materialising a list
- [ ] Hover/selection repaints are change-gated and row-scoped
- [ ] Repaint count measured before and after on a mouse-move sweep, as was done for the grid
