# 08 — Keyboard, Accessibility, RTL & Touch

**Priority P1.**

## Current behaviour

Each area has a home, but coverage is very uneven.

| Area | Files | Files referencing it |
|---|---|---|
| Accessibility | `Helpers/BeepTabAccessibleObjectFactory.cs` (228 lines), `Hosts/BeepTabHeaderHost.Accessibility.cs` | 8 |
| Keyboard | `BeepTabs.Keyboard.cs` (155), `Hosts/BeepTabHeaderHost.Keyboard.cs` | — |
| High contrast | ~~`Hosts/BeepTabHeaderHost.HighContrast.cs` (156)~~ **deleted** — now `Helpers/TabThemeHelpers.cs` | all painters |
| Touch | `Hosts/BeepTabHeaderHost.Touch.cs`, `MinTouchTargetWidth` (default 44) | 4 |
| **RTL** | `Helpers/BeepTabRtlLayoutHelper.cs` | **1** |

**High contrast never ran at all — FIXED.** `BeepTabHeaderHost.HighContrast.cs` contained a full
high-contrast paint pass, documented as *"Called from OnPaint when IsHighContrast is true"*, that
**nothing ever called**. `RenderHeader` painted through the theme painters unconditionally; only the
focus ring consulted `SystemInformation.HighContrast`. So on Windows High Contrast the tabs kept
rendering theme colours — a real accessibility failure, not a cosmetic one.

It was also a second implementation of tab geometry (its own text bounds, close glyph, dirty marker)
and a lossy one: no icons, no badges, no subtext, no header actions. Wiring it in would have traded
one accessibility bug for another. Instead high contrast is now resolved where colours are resolved —
`TabThemeHelpers` returns system colours when `IsHighContrast`, ahead of both theme colours and any
explicit custom colour, since honouring a custom colour is precisely what breaks high contrast. The
one painter pipeline is now correct in both modes and the duplicate file is deleted.

The state→system-colour mapping from the deleted file was preserved rather than reinvented
(`Highlight`/`HotTrack`/`ButtonFace` for backgrounds, `HighlightText`/`WindowText` for text,
`WindowFrame` for borders, `ControlText` for the dirty dot), and each carries a comment saying where
it came from. The harness asserts exactly one file consults `SystemInformation.HighContrast` and that
`IsHighContrast` is actually read outside its own file.

**Still unverified:** none of this has been *rendered* in high contrast. The contact sheet in
[10](10-theming-and-painters.md) must include a high-contrast column — the colours are now sourced
correctly, which is not the same as proven legible.

**The control had no accessible tree at all — FIXED.** `BeepTabs` never overrode
`CreateAccessibilityInstance`, so a screen reader saw one opaque control: the tabs could not be
enumerated, named, or activated. Meanwhile `BeepTabAccessibleObjectFactory` — 228 lines building tab
and close-button accessible objects with the right roles, states, names and Select/Close actions —
had **zero callers**, and `BeepTabHeaderHost.Accessibility.cs` was an empty partial whose entire
content was a comment promising the work "in a future update".

`BeepTabs` now reports `AccessibleRole.PageTabList` and exposes one child per tab plus one per close
button, built from the layout snapshot so the rectangle a screen reader is given is the rectangle the
user sees. Verified: three tabs produce six children, each named by its caption, the selected tab
reports `AccessibleStates.Selected`, and `Select(TakeSelection)` through the accessible object
actually changes the selection.

**The keyboard contract holds.** Ctrl+Tab walks most-recently-used order, not positional — visiting
0 → 3 → 1 and cycling lands on 3, where positional order would have said 2. Home and End jump to the
first and last tab.

**Touch: the API was dead *and* unsound.** `BeepTabHeaderHost.Touch.cs` —
`ExpandToMinTouchTarget`, `TouchHitTestTabIndex`, `MeetsTouchTarget`, `ScaleTouchTarget` — had no
callers; the live hit test uses the painted bounds directly. It could not have been wired up as
written either: tabs in a run are contiguous, so centring an expansion on each one makes neighbours
overlap, and `TouchHitTestTabIndex` returns the first match — a tab's left edge would select its
neighbour. Deleted, along with `MinTouchTargetWidth`, which had exactly one reader that copied it
into a render-context field nothing consumed.

For a tab strip the mechanism that works is header height. The default is 30px against WCAG 2.5.5's
44dip guidance; `HeaderHeight` is public and raising it does reach the hit target (verified: 48 gives
a 48px tab). Tab *width* was never the problem — `MinTabWidth` is 60.

**RTL now works — FIXED.** `BeepTabRtlLayoutHelper` was complete and correct and referenced only by
itself, so `RightToLeft` did nothing measurable. It is now called from
`BeepTabHeaderHost.SyncSnapshot`, before anything consumes the snapshot.

Mirroring the snapshot is sufficient on its own: painting and hit-testing both read these bounds, so
the mirrored rectangles are simultaneously where tabs are drawn and where clicks land. That makes the
helper's `FlipPoint` actively wrong — flipping the pointer *as well* would mirror twice and cancel
out — so it was deleted rather than left as a trap.

RTL is resolved from the owning `BeepTabs` rather than the host, because `RightToLeft.Inherit` on the
host reports the framework default instead of what the application set.

Verified by measurement, not by reading — three tabs, 520px control:

| | rects |
|---|---|
| LTR | `[0..60] [60..120] [120..180]` |
| RTL | `[444..504] [384..444] [324..384]` |

Five assertions hold: the layout changes at all, the first logical tab moves to the right-hand side,
nothing escapes the control, nothing overlaps, and every width is preserved (mirroring moves boxes,
it must not resize them).

**Original finding, kept for the record:** A right-to-left layout helper that nothing calls
is either dead code or an unfinished feature; either way `RightToLeft` on this control currently does
nothing measurable. This is the same pattern as the tooltip properties that were declared and never
read.

`MinTouchTargetWidth` defaults to 44px, which matches the WCAG 2.5.5 / platform guidance — good, and
worth keeping. Note its clamp lives in `BeepTabHeaderHost.Touch.cs`, inside one of the bare `catch`
blocks flagged in [03](03-exception-policy.md).

## What the reference products do

- **Keyboard**: Left/Right (or Up/Down when vertical) move selection; Home/End jump; Ctrl+Tab is MRU
  order, not positional; Ctrl+W closes; Ctrl+Shift+T reopens; Ctrl+1..9 selects by position. The
  header is a single tab stop and arrow keys move within it — not one tab stop per tab.
- **Accessibility**: the strip exposes a tab-list role, each tab a tab-item with selected state,
  name, and position ("3 of 7"); selection changes raise a UIA selection event so a screen reader
  announces the newly-selected tab.
- **RTL**: tab order mirrors, the first tab sits at the right, close buttons move to the leading
  edge, and overflow scrolls in the mirrored direction.
- **Touch**: minimum 44px targets, and close buttons that do not become unhittable when tabs shrink.

## Work

1. **Decide RTL: implement or delete.** If `BeepTabRtlLayoutHelper` is to live, it must be wired
   into the layout path and verified with a mirrored render. If not, delete it — an unused helper
   implies support that does not exist.
2. **Verify the keyboard contract**, especially that Ctrl+Tab uses MRU order (the tracker exists) and
   that the header is one tab stop with arrow-key navigation inside it.
3. **Verify the accessibility tree** with a real client: role, name, selected state, position-in-set,
   and a selection-changed event on switch. `BeepTabAccessibleObjectFactory` is substantial; what is
   missing is proof it produces the right tree.
4. **High contrast**: render every painter under high contrast and confirm each uses system colours
   — the tooltip program found a painter that produced solid magenta because it blended against a
   transparency key.
5. **Touch**: assert the close button remains at least the minimum target size when tabs are at
   their narrowest, and that shrinking never makes it overlap the label.

## Verification

- Probe: drive every documented keyboard binding and assert the resulting selection.
- Probe: dump the accessible tree for a 5-tab strip and assert roles, names, selected state and
  set positions.
- Render at `RightToLeft.Yes` and assert tab order, close-button side and overflow direction mirror.
- Render every painter under high contrast; review once, then keep as baselines.
- Probe: shrink tabs to minimum and assert close-button hit rects meet `MinTouchTargetWidth`.
