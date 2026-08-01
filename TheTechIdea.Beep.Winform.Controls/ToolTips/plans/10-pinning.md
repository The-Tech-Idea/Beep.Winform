# 10 — Pinning

**Priority P2.**

## Current behaviour

`ToolTipConfig` declares the feature:

```csharp
/// <summary>
/// When true, a pin icon appears allowing the user to keep the tooltip open.
/// </summary>
public bool Pinnable { get; set; } = false;

/// <summary>Current pin state (managed by ToolTipManager).</summary>
public bool IsPinned { get; set; } = false;
```

Searching the assembly for `Pinnable` returns exactly one hit — the `#region Pinnable` comment in
the declaring file. **Nothing reads it**, including the `ToolTipManager` that the XML doc says
manages the state. No pin icon is painted and no pinned state is tracked.

Separately, `BeepPinnedTooltip.cs` (200 lines) exists as a standalone control with its own
`MouseMove` drag handling. It is not connected to `ToolTipConfig.Pinnable`, `ToolTipManager` or the
painter pipeline — a parallel implementation of the same idea.

So there are two half-features: a config flag with no implementation, and an implementation with no
route from config.

## What the reference systems do

Pinning is standard in developer and data tools:

- **Visual Studio / VS Code** — pin a debugger data tip; it becomes a floating window that survives
  navigation and can be dragged.
- **DevExpress `SuperToolTip`** — `AllowHtmlText` + pinnable flyouts.
- **Chrome DevTools** — pinned inspector tooltips.

Common behaviour: a pin affordance in the tooltip header; pinning converts it into a persistent,
draggable window exempt from hover/leave dismissal; a close button replaces the pin; pinned tooltips
survive anchor changes and are dismissed only explicitly.

## Work

1. **Decide the owner.** Either `BeepPinnedTooltip` becomes the pinned *presentation* driven by
   `ToolTipManager` when `Pinnable` is set, or it is deleted and pinning becomes a state of the
   normal tooltip window. Two implementations of one concept is the defect pattern this repo has
   already paid for twice (BeepTree layout engines, tree painter fonts) — pick one.
2. **Paint the pin affordance** in the header when `Pinnable`, next to the close button that
   `Closable` already provides.
3. **Pinned state changes the rules**: exempt from show/hide delays, from `Duration` timeout, from
   `MouseLeave` dismissal, and from [06](06-delay-groups.md) grouping. It still participates in
   [03](03-auto-update.md) so it tracks its anchor, unless the user has dragged it, at which point it
   detaches and keeps its own position.
4. **Multiple pinned tooltips** must coexist — the manager keys instances by string already
   (`ConcurrentDictionary<string, ToolTipInstance>`), so this is mostly about not hiding siblings.
5. **Dismissal** only via the close button, an explicit API call, or the anchor being destroyed.

## Verification

- Set `Pinnable`, hover, click the pin; assert the tooltip survives moving the pointer away, and
  survives longer than `Duration`.
- Pin three tooltips from three anchors; assert all three remain visible simultaneously.
- Drag a pinned tooltip; assert it detaches from auto-update and keeps its position when the anchor
  scrolls.
- Close the anchor's form; assert pinned tooltips are disposed and no window leaks.
