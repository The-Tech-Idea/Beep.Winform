# BottomNavBars — review and enhancement

Master tracker for `TheTechIdea.Beep.Winform.Controls/BottomNavBars/`.
**18 C# files, 3,211 lines: `BottomBar` (BaseControl), 10 `BottomBarStyle` values, one painter each.**

Reference designs live in `.plans/example1..5.png`. The pre-existing `.plans/Phase1..4` and
`GapMatrix` documents are aspirations, not a record of what the code does — they are treated here as
claims to check.

## How this review was done

Two passes that disagreed with each other, which is the point.

1. **Rendering every style and looking at it.** A probe builds all 10 styles, captures each with
   `Control.DrawToBitmap`, and compares them.
2. **A 73-agent read of the folder** across lifecycle, painters, helpers and theming — every finding
   then handed to a separate agent whose job was to *refute* it from the source. Findings that
   survived are below; the ones that did not are not recorded as defects.

Neither pass found everything. The crash came only from reading; the clipped CTA and the misplaced
selection marker came only from looking.

## Fixed

| # | Defect | Where |
|---|---|---|
| 1 | **Replacing `Items` threw out of `OnPaint`, ~20×/second** | [01](01-items-and-layout.md) |
| 2 | **The selection marker was drawn on the CTA, not the selection** | [02](02-indicator-and-bounds.md) |
| 3 | **CTA shapes were clipped by the control's top edge** | [03](03-cta-overhang.md) |
| 4 | **Clicks forked: painter-owned cells never opened their child popup** | [04](04-click-routing.md) |
| 5 | **The 50ms ticker ran forever, for every style, visible or not** | [05](05-animation-cost.md) |
| 6 | **A `Font` allocated per label per paint** | [05](05-animation-cost.md) |
| 7 | **`DpiScale` was stored and never acted on** | [02](02-indicator-and-bounds.md) |
| 8 | **Six `catch { }` around code that cannot throw** | [06](06-swallows.md) |
| 9 | **Badges never followed the theme** — `== Color.Empty` on a field defaulting to `BeepColor.Red` | [07](07-dead-surface.md) |
| 10 | **Dead surface removed**, and `ShowCTAShadow`'s `[DefaultValue]` corrected | [07](07-dead-surface.md) |

## The two most serious

**Replacing `Items` crashed the control.** The setter swapped the list and called `Invalidate()`
without marking the layout dirty — unlike `Items_ListChanged`, which does both. `EnsureLayout`'s guard
tests only the dirty flag and the bounds, so the cached rectangles described the *previous* list.
Painters then walk `for (i = 0; i < rects.Count; i++) context.Items[i]`. A shorter list threw
`ArgumentOutOfRangeException` out of an `OnPaint` with no `catch`, and the ticker re-raised it about
twenty times a second. **8 of the 10 painters** had that unguarded loop.

**The selection marker was drawn on the CTA.** `GetIndicatorRect()` computed the indicator from item 0
and then overwrote it with the CTA's rectangle whenever `CTAIndex >= 0`. Every style reading
`AnimatedIndicatorX` — Classic, Bubble, Pill, MovableNotch, both FloatingCTAs — put its selection
marker in the wrong cell. Visible in the render: the bubble sat on "Add" while "Home" was selected.

## Still open

| Finding | Note |
|---|---|
| Accessible bounds/hover/tooltip use grid cells, not the painted region | on CTA styles the click target is elsewhere than the reported bounds |
| `IsOverflow` is computed and nothing acts on it | a missing overflow strategy, not dead code — left in place |
| No default selection | the bar opens with no active tab |
| Icons never rendered in the probe | the sample SVG paths did not resolve, so icon rendering is **unverified** here |

## A correction to this review's own first finding

The first render reported **"Bubble == Pill == NotionMinimal render identically"** and called them
aliased styles. That was wrong, and the cause was the probe: it never selected an item, and those
three styles express themselves *only* through the selected-item treatment. With a selection, all ten
render distinctly.

The real finding is narrower and still worth having: **the bar defaults to no selection**, so it opens
with no active tab — and for those three styles that makes them indistinguishable from one another.

## Standing constraints

Per `CLAUDE.md`: report every catch through `BeepLog`; no stubs or legacy paths; nothing assigns
colours; a check must be able to fail for the reason it was written.
