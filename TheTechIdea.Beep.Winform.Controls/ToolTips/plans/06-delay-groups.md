# 06 — Delay Groups (Skip-Delay)

**Priority P1.**

## Current behaviour

Delays are per-tooltip only. `ToolTipManager` exposes `DefaultShowDelay = 500` and
`DefaultHideDelay = 3000`, and `ToolTipConfig` has nullable `ShowDelay` / `HideDelay` overrides.
There is no concept of a *group*: searching for `DelayGroup`, `skipDelay` or similar returns nothing.

The effect is the interaction every toolbar suffers from: hover the first button, wait 500ms, read
the tooltip; move to the adjacent button, wait another 500ms. Sweeping a toolbar of ten buttons costs
five seconds of waiting to read ten labels.

## What the reference systems do

Every mature system special-cases the "user is already reading tooltips" state:

| System | Mechanism |
|---|---|
| Radix | `<Tooltip.Provider delayDuration={700} skipDelayDuration={300}>` — after one tooltip shows, others in the provider open instantly for 300ms after the last closes |
| Tippy | `delay` + a singleton with `overrides`, plus `moveTransition` so the tooltip slides between anchors |
| WinForms `ToolTip` | `ReshowDelay` — the delay used when moving between controls that already have tooltips |
| DevExpress | `ToolTipController.ReshowDelay`, plus `Rounded`/`AutoPopDelay` |

The shared idea: the *first* tooltip is delayed to avoid noise; subsequent ones in the same
neighbourhood are immediate, until a quiet period resets the group.

## Work

1. **`ToolTipDelayGroup`** — a named group with `ShowDelay`, `SkipDelayWindow` (default 300ms) and
   last-hidden timestamp. `ToolTipConfig.DelayGroup` (string, default `"default"`) assigns membership.
2. **Skip-delay rule**: when showing a tooltip whose group last hid a tooltip within
   `SkipDelayWindow`, show immediately; otherwise apply `ShowDelay`.
3. **Reshow within a container.** Controls sharing a parent (toolbar, ribbon, grid header) should
   default to the same group without callers configuring anything — derive the default group key
   from the anchor's parent when the caller does not specify one.
4. **Optional move transition.** With a group active, moving between adjacent anchors can animate
   the existing tooltip window to the new position instead of hide/show. This is what makes Tippy's
   singleton feel polished, and it is cheaper than destroying and recreating a window
   (see [12](12-lifecycle-and-performance.md)).
5. **Cancel semantics.** Leaving the anchor during the show delay must cancel cleanly — verify the
   current `Task.Delay`-based path cannot show a tooltip for an anchor the pointer already left.

## Verification

- Hover five adjacent toolbar buttons in sequence; assert only the first waits `ShowDelay` and the
  rest appear within one frame.
- Wait longer than `SkipDelayWindow` and assert the delay applies again.
- Flick the pointer across a toolbar faster than `ShowDelay` and assert **no** tooltip appears —
  the classic bug this feature must not introduce.
- Assert no tooltip is shown for an anchor the pointer has already left, under rapid enter/leave.
