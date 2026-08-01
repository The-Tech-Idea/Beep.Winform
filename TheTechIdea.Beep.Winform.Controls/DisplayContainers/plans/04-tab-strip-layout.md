# 04 — Tab strip layout: sizing under pressure

## Finding 1 — tabs never shrink; the strip jumps straight to scrolling

`Helpers/TabLayoutHelper.cs:117`:

```csharp
int finalWidth = Math.Max(minWidth, Math.Min(maxWidth, contentWidth));
```

Each tab is clamped to `[minWidth, maxWidth]` **independently of how many tabs there are**. Then
`:58`:

```csharp
bool needsScrolling = totalTabWidth > noScrollAvailableSpace;
```

So the moment the tabs' natural widths exceed the strip, scroll buttons appear and the user must
page through them. There is no intermediate state.

Every comparable document host — VS Code, Chrome, Firefox, Visual Studio — shrinks tabs toward a
floor first, and only scrolls once even the floor will not fit. That is the behaviour that keeps all
tabs reachable without paging, and it is the single largest UX gap in the strip.

`minWidth` (80 logical px, `:129`) already exists and is exactly the floor such a pass needs.

**Work:** insert a proportional shrink pass between measurement and the scroll decision — distribute
the overflow across tabs down to `minWidth`, then scroll only if still overflowing. Preserve the
active tab at its natural width where possible, as VS Code does, so the tab being read never
collapses to an ellipsis.

## Finding 2 — the pinned/active tabs are shrunk by the same rule as the rest

Pinned tabs return a fixed `PinnedTabWidth` (38) and must not participate in shrinking. The active
tab should shrink last. Neither distinction exists today because there is no shrink pass at all;
both need to be expressed when one is added.

## Finding 3 — reserved utility width is a flat constant

`TabHeaderMetrics.cs:28,30,32`:

```csharp
UtilityButtonsReservedWidth => 140
NewTabButtonReservedWidth   => 40
ScrollAreaOffset            => 40
```

140px is reserved for the utility cluster whether or not every utility button is shown. The prior
enhancement pass already made the *choice* conditional (reserve new-tab only when not overflowing,
the full cluster when overflowing) — but the amount is still a magic constant rather than the sum of
the buttons actually laid out. If the cluster's composition ever changes, the reservation silently
stops matching, and the symptom is a gap or an overlap at the strip's right edge.

The grid toolbar solved the identical problem this cycle by computing `reservedRight` from the
buttons it was actually going to draw, in a two-pass layout. Same fix applies.

## Finding 4 — `ScrollAreaOffset` (40) and `NewTabButtonReservedWidth` (40) are coincidentally equal

Two independently-meaningful constants that happen to share a value invite a future edit to one that
silently changes the other's behaviour if someone consolidates them. Keep them distinct and name what
each reserves.

## Work

- [ ] Add a shrink pass: proportional reduction to `minWidth` before scrolling is considered
- [ ] Exclude pinned tabs from shrinking; shrink the active tab last
- [ ] Compute reserved utility width from the buttons actually laid out, not a constant
- [ ] Re-check `needsScrolling` after shrinking (it is currently decided against unshrunk widths)

## Verification

- 3 tabs in a wide strip: no scroll buttons, tabs at natural width
- 12 tabs in the same strip: tabs shrink toward the floor, still no scroll buttons
- 40 tabs: tabs at `minWidth`, scroll buttons appear
- across all three, assert the rightmost tab's right edge never crosses the utility cluster's left
  edge — the overlap this phase exists to prevent
- assert the active tab is never the narrowest tab when others could shrink instead
