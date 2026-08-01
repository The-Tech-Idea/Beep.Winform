# 04 — Interactive / Hoverable Tooltips

**Priority P0.** This is an accessibility conformance gap, not only a feature gap.

## Current behaviour

`ToolTipConfig.PersistOnHover` is declared:

```csharp
/// <summary>
/// When true, tooltip stays open while the mouse is over it (WCAG 1.4.13).
/// </summary>
public bool PersistOnHover { get; set; } = true;
```

Searching the entire assembly for reads of it returns **nothing**. The only other occurrence is
`BeepPopover.cs:56` *assigning* `cfg.PersistOnHover = true`. No code branches on it.

So the documented WCAG 1.4.13 behaviour does not exist, and the property's default of `true`
makes the API actively misleading: a caller reading the config would reasonably conclude hoverable
tooltips are on by default.

Consequences today:

- A tooltip containing a link, button or copyable text cannot be reached — moving the mouse toward
  it leaves the anchor, which hides it.
- `ToolTipType.Interactive` ("Contains interactive elements like buttons, links, or forms") and the
  `IsLink` / `LinkTarget` fields on `ToolTipContentItem` describe content the user cannot click.

## WCAG 1.4.13 (Content on Hover or Focus)

Three requirements. Additional content triggered by hover or focus must be:

| Requirement | Status |
|---|---|
| **Dismissible** without moving the pointer (Escape) | partially — `CustomToolTip.Accessibility` handles Escape |
| **Hoverable** — the pointer can move over the content without it disappearing | **not implemented** |
| **Persistent** — stays until dismissed, the trigger is lost, or the info becomes invalid | not implemented; `Duration` auto-hides after 3s by default |

The default `Duration = 3000` also conflicts with "persistent": content vanishing on a timer fails
1.4.13 for users who read slowly.

## What the reference systems do

The hard part is the gap between anchor and tooltip. Naively, `MouseLeave` on the anchor hides the
tooltip before the pointer arrives. Three established solutions:

1. **Close delay** (Tippy `interactiveDelay`, Radix `delayDuration` on close) — a grace period of
   ~100–300ms during which leaving the anchor does not hide.
2. **Bridge / invisible padding** — extend the tooltip's hit area toward the anchor by the offset
   distance so the two are contiguous.
3. **Safe polygon** (Floating UI `safePolygon`) — compute the triangle between the pointer's exit
   point and the tooltip's near edge, and treat the pointer as still "inside" while it stays within
   that triangle. Best behaviour, and it does not keep the tooltip alive when the pointer moves away
   in an unrelated direction.

## Work

1. **Implement `PersistOnHover`.** When true, subscribe to the tooltip window's own `MouseEnter` /
   `MouseLeave` and cancel the pending hide while the pointer is over it.
2. **Bridge the gap** with a close delay plus a safe-polygon test. Start with the close delay
   (simple, covers most cases), then add the polygon for diagonal travel.
3. **Rework `Duration` semantics.** Auto-hide should be off by default for tooltips carrying
   interactive content or a title; keep timed auto-hide for transient notification types. A property
   that silently violates a WCAG criterion should not be the default.
4. **Make link content actually clickable** — hit-test `ToolTipContentItem.IsLink` spans and raise
   a `LinkClicked` event with `LinkTarget`. The markup parser already produces `SpanKind.Link` spans
   with targets; nothing consumes the click.
5. **Keyboard parity**: when a tooltip is interactive and shown by focus, Tab must move into it and
   Escape must return focus to the anchor. See [05](05-dismissal-and-focus.md).

## Verification

- Show an interactive tooltip, move the pointer from anchor to tooltip along a diagonal path, and
  assert it stays open for the whole traversal.
- Move the pointer from the anchor *away* from the tooltip and assert it hides after the close delay.
- Assert a tooltip with a title and a link does not auto-hide.
- Click a link span and assert `LinkClicked` fires with the right target.
