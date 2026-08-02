# 06 — Keyboard, autocomplete and accessibility

## Scope

`FilterKeyboardHandler.cs` (353), `FilterAutocompletePopup.cs` (339),
`FilterSuggestionProvider.cs` (361), `FilterIconProvider.cs` (315).

## Work

- [ ] **Accessible tree.** `BeepFilter` paints its surface rather than hosting child controls, so
      MSAA has nothing to enumerate on its own — the same situation as `BeepDisplayContainer2`, where
      a screen reader saw one anonymous client area until `CreateAccessibilityInstance` was added.
      Expose the criteria as named children with roles, values and bounds.
- [ ] **Autocomplete popup semantics.** A suggestion list needs `Escape` to dismiss without
      committing, `Enter` to commit, arrows to move, and focus returned to the field on close. Check
      also that dismissing does not leave a partially-applied criterion.
- [ ] **Focus indicator.** Confirm the focused criterion is visibly marked (WCAG 2.4.7). Check
      whether one already exists before adding: in `DisplayContainers` a complete, correct focus ring
      was already implemented and never drew, because the flag driving it was read once and assigned
      nowhere. A second implementation was written before that was noticed.
- [ ] **Hit targets.** The painters draw pills and chips with remove/edit affordances; measure them
      against a 24 logical px minimum. In both the grid header and the container tab strip, the
      affordance rect was being used as *both* the glyph and the hit area, giving 13px targets.
- [ ] **High contrast.** Criteria and their affordances remain legible with
      `SystemInformation.HighContrast`.

## Verification

- Per-control accessible **names**, not a tree walk. A traversal via `GetChildCount`/`GetChild` on a
  control that has not overridden them returns nothing and proves nothing — a stock `Form` holding a
  `Label` and a `Button` measures 0 descendants, which is how that check produced a false "screen
  reader sees an empty dialog" verdict in the DialogsManagers program.
- Keyboard reachability asserted by **driving keys**, not by reading the handler.
- Hit targets measured from the rectangles the painter publishes, at 100% / 150% / 200%.

---

## Outcome (partial)

### Done: the accessible tree

`BeepFilter` already declared `AccessibleRole.Grouping` — announcing a group that contained nothing,
because the criteria are painted rather than hosted as child controls and MSAA had nothing to walk.

`BeepFilter.Accessibility.cs` now overrides `CreateAccessibilityInstance`. Each criterion is a
`AccessibleRole.Row` named for its column, with its condition as the accessible value:

```
role=Grouping   children with 0 criteria=0
                children with 2 criteria=2
names  = [Country, Amount]
values = [Equals Norway | GreaterThan 1000]
```

The 0-vs-2 comparison is what makes the count meaningful — a check that only ever sees a populated
control cannot tell a real count from a constant. This is the same trap that produced a false
"screen reader sees an empty dialog" verdict in the DialogsManagers program, where a stock `Form`
measured 0 descendants too.

### Found, surfaced, but not finished: keyboard focus

`FilterKeyboardHandler.FocusedFilterIndex` is **never set and never read**. Its setter calls
`_filter.Invalidate()`, so it plainly existed to drive a focus indicator — and no painter has ever
known about focus at all. Alt+Up and Alt+Down reorder criteria *relative to this index*, which means
they reorder relative to a position the user can neither establish nor see.

Third instance of this shape in this program, after `FilterPosition` and the painter capability
flags.

`BeepFilter.FocusedCriterionIndex` now surfaces it, and the accessible tree reports
`AccessibleStates.Focused` for that criterion — so assistive technology can at least say which is
current.

**The painted indicator is deliberately not built.** Each style locates criteria through a different
rect collection (`layout.TagRects` for TagPills, others elsewhere), so a correct focus ring means
touching all eight painters and verifying each — a phase-sized piece of work, not a tail-end
addition. Building half of it would leave a focus ring that appears in some styles and not others,
which is worse than none.

### Remaining in this phase

- [ ] Paint the focus indicator, and let Tab/arrows establish focus rather than only Alt+Up/Down
      moving it
- [ ] Autocomplete popup semantics: `Escape` dismisses without committing, `Enter` commits, arrows
      move, focus returns to the field on close
- [ ] Hit targets ≥ 24 logical px on the remove/edit affordances — `FilterPainterMetrics` has no
      minimum-target concept at all
- [ ] Legibility under `SystemInformation.HighContrast`

---

## Outcome — focus ring and reverse navigation

### My estimate in this document was wrong twice

It said a focus indicator meant "touching all eight painters and verifying each — a phase-sized piece
of work". Both halves were wrong:

1. **`FilterLayoutInfo` already reconciles criterion bounds.** Seven of eight painters populate either
   `TagRects` (TagPills) or `RowRects` (the six row styles). One `GetCriterionRect(index)` helper plus
   **one call site** after the active painter runs gives every style the same indicator. The eighth,
   `DropdownMultiSelect`, positions no per-criterion rect — its criteria live inside a dropdown — so
   it correctly gets nothing rather than a guess.
2. **Focus establishment already worked.** `NavigateToNextFilter` exists and `Tab` already called it.
   Only the drawing was missing.

Deferring on a wrong size estimate is still deferring. Worth recording alongside the finding.

### What was found while wiring it

`NavigateToPreviousFilter` was declared and **never called from anywhere** — `Shift+Tab` was not
bound. Navigation only ever went forwards, so overshooting meant tabbing all the way round. Now
bound.

### Verified by driving keys and comparing pixels

```
focus before any key: -1
after Tab, Tab:        0 -> 1     Tab advances
after Shift+Tab:       0          Shift+Tab walks back
focus on criterion 0 vs 2: 0.13% of pixels differ
```

The pixel comparison is the part that matters: a draw call added is not a ring rendered. Its first
run reported **0.00%** — the ring was not reaching the pixels — which led to the layout-recalculation
discovery recorded in [02](02-painter-distinctness.md).

### Still remaining

- [ ] Autocomplete popup semantics: `Escape` dismisses without committing, `Enter` commits, arrows
      move, focus returns to the field on close
- [ ] Hit targets ≥ 24 logical px — `FilterPainterMetrics` still has no minimum-target concept
- [ ] Legibility under `SystemInformation.HighContrast`

---

## Hit targets — a real defect, one painter fixed

Measuring the rectangles the painters publish (not the constants in their source) found that **four
painters compute the remove affordance twice**: once at paint time from DPI-scaled values, and again
inside `HitTest` from raw literals. The two disagree.

| painter | draws | hit-tests | at 100% |
|---|---|---|---|
| `GroupedRows` | `Right - s36`, `s28²` | `Right - 30`, `24²` | different rect |
| `InlineRow` | `currentX`, `sButtonSize²` | `Right - 16`, **`14²`** | different rect, **below 24px** |
| `QueryBuilder` | — | `Right - 28`, **`20²`** | **below 24px** |
| `AdvancedDialog` | `currentX`, `s24²` | `Right - 32`, `24²` | different rect |

The glyph the user aims at and the region that responds are **different rectangles**, and they
diverge further with DPI because one side scales and the other does not. This is the same
measure-here / draw-there family as the grid header and the container tab strip, where the glyph rect
was being used as the hit rect and produced 13px targets.

### `GroupedRows` — fixed

One expression, `RemoveRectFor(rowRect, owner)`. The layout publishes it into
`layout.RemoveButtonRects`, the paint reads it, and `HitTest` reads the published array — so the
three cannot drift. Measured: **28×28**, above the minimum.

The `HitTest` fallback that recomputed the rect was dropped rather than kept: a fallback that
computes geometry a second way is the defect this fix removes.

### Remaining — same pattern, three painters

- [ ] `InlineRowFilterPainter` — draw `:152`, hit `:284`. **14×14** target
- [ ] `QueryBuilderFilterPainter` — hit `:461`. **20×20** target
- [ ] `AdvancedDialogFilterPainter` — draw `:352`, hit `:615`

Each is the same change: extract the draw expression into a `RemoveRectFor` helper, publish it from
the layout pass into `RemoveButtonRects`, and have `HitTest` read the published array. The harness
check already covers them — they currently report "publishes no remove rects", and will assert a
≥24px target as soon as they do.

`DropdownMultiSelect` and `SidebarPanel` register no remove hit area at all; whether that is
intentional needs deciding rather than assuming.
