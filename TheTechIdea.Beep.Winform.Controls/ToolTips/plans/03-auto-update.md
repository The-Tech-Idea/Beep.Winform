# 03 — Auto-Update on Anchor Movement

**Priority P0.** Depends on [01](01-anchor-and-placement.md).

## Current behaviour

A tooltip is positioned once, at show time, and never again.

Searching the whole `ToolTips/` tree for movement handling turns up only `FollowCursor`
(`ToolTipManager.OnControlMouseMove`) and `BeepPinnedTooltip`'s own drag handling. There is no
subscription to:

- the anchor control's `LocationChanged` / `SizeChanged`
- the owning form's `Move`, `Resize`, `ResizeEnd`
- container scrolling
- `DpiChangedAfterParent` or monitor changes

So a tooltip that is open while the user scrolls a panel, drags the window, maximises it, or drags
it to a second monitor stays exactly where it was — floating over unrelated content, still pointing
at nothing.

This matters more here than on the web: a WinForms tooltip is a top-level window, so it does not
move with its parent for free.

## What the reference systems do

Floating UI ships `autoUpdate(reference, floating, update, options)`, which subscribes to:

| Source | Why |
|---|---|
| ancestor scroll | anchor moves within a scrollable container |
| ancestor resize | layout reflow moves the anchor |
| `ResizeObserver` on both elements | content size changes |
| `IntersectionObserver` | anchor leaves the viewport → `hide` middleware |
| animation frame (opt-in) | anchor moved by a transform that fires no event |

Tippy and Radix enable it by default for any non-trivial popper, and explicitly document that
disabling it is a performance trade-off.

## Work

1. **`ToolTipAutoUpdate` helper** owning a subscription set for one visible tooltip:
   - anchor control `LocationChanged`, `SizeChanged`, `VisibleChanged`, `ParentChanged`
   - the anchor's `TopLevelControl` `Move`, `ResizeEnd`, `Deactivate`
   - each `ScrollableControl` ancestor's `Scroll`
   - `SystemEvents.DisplaySettingsChanged` for monitor/DPI changes
2. **Recompute through the same placement path** as the initial show — never a second, simplified
   "just move it" path. That is how the duplicate-engine problem starts.
3. **Coalesce.** Scroll and resize fire in bursts; batch to at most one reposition per frame
   (~16ms) or per `WM_PAINT`, whichever is simpler, and skip entirely when the recomputed rect
   equals the current one. This is the same change-gating rule that took BeepGridPro from 12
   repaints per mouse-move to 2.
4. **Hide when the anchor is no longer showable** — disposed, `Visible == false`, zero-size, its
   form deactivated or minimised, or the anchor rect fully clipped by an ancestor's client area.
   That last case is Floating UI's `hide` middleware and needs an explicit ancestor-clip test, since
   WinForms will happily report a control's screen rect while it is scrolled out of view.
5. **Opt-out** via `ToolTipConfig.AutoUpdate` (default `true`) for callers that place a tooltip
   deliberately and do not want it to chase anything.

## Verification

- Open a tooltip anchored inside a scrollable panel; scroll; assert the tooltip tracks the anchor
  and hides once the anchor scrolls out of the panel's client area.
- Open a tooltip, drag the form 300px; assert the tooltip follows and the arrow still points at the
  anchor.
- Drag the form to a second monitor with a different DPI; assert the tooltip re-measures and stays
  on the correct screen's working area.
- Count repositions during a 2-second scroll and assert coalescing holds it to roughly one per
  frame, not one per scroll event.
