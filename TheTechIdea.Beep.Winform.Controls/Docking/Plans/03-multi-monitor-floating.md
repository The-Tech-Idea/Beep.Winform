# 03 — Multi-monitor floating windows

## What is missing

`grep -rn "Screen.AllScreens"` across `Docking/` returns **0**. `Runtime/FloatWindow.cs` (372 lines)
never consults `Screen` at all — it positions from `Owner.Bounds` (`:217`) and an `initialBounds`
passed in by the caller.

A float window is therefore only ever reasoned about relative to its owner form, on whatever monitor
that form happens to be. Three consequences follow:

1. **A float dragged to a second monitor is not known to be there.** Nothing maps it to a screen.
2. **Restoring a saved layout can place a float off-screen.** If the user detached a monitor between
   sessions, the stored bounds point into nothing, and the window is unreachable — the classic
   "my tool window disappeared" bug.
3. **Per-monitor DPI is unhandled.** Moving a float from a 100% to a 200% display should rescale its
   chrome; nothing here responds to `WM_DPICHANGED`.

## Reference behaviour

| product | behaviour |
|---|---|
| Visual Studio | floats are first-class windows on any monitor; layouts store the monitor and fall back to primary when it is gone |
| JetBrains Rider | detached tool windows remember their screen; restored to primary if absent |
| Figma / Blender | multi-monitor as a first-class arrangement, not an accident |

## Design

- Store float bounds **with the monitor's device name and working area**, not just a rectangle.
  Restoring matches by device name first, then by geometry overlap, then falls back to primary.
- On restore, clamp into the target monitor's working area — a float must always be reachable,
  including its caption bar, which is what the user needs to drag it back.
- Handle `WM_DPICHANGED` on `FloatWindow`: rescale caption height, glyphs and border to the new DPI
  rather than leaving them at the old monitor's scale.
- Snapping and drop guides ([05](05-drop-guides-and-preview.md)) must consider the monitor under the
  cursor, not the owner's.

## Work

- [ ] Record `{ DeviceName, WorkingArea, Bounds }` for a float in persistence
- [ ] Restore: match by device name → geometry overlap → primary, then clamp into the working area
- [ ] `WM_DPICHANGED` handling in `FloatWindow`
- [ ] Guide/snap logic uses `Screen.FromPoint(cursor)`, not the owner
- [ ] Refuse to restore a float whose caption would be entirely off-screen

## Verification

- Save a layout with a float on a secondary monitor; restore with that monitor present — assert the
  same screen and bounds
- Restore the same layout with the monitor absent — assert the float lands on the primary, fully
  within the working area, caption visible
- Restore bounds deliberately set off-screen (`-5000, -5000`) — assert clamping brings it back
- These are testable headlessly by driving the restore logic against a synthetic screen list; the
  monitor set is an input, not an ambient fact, and should be modelled that way so it can be

---

## Outcome

The premise held: zero `Screen` references in the folder outside two unrelated local variables,
`FloatingPanelInfo` stored a bare `Rectangle`, and there was no `WM_DPICHANGED` handling anywhere.

### The monitor set is an input

`MonitorInfo` + `IMonitorProvider`, with `SystemMonitorProvider` wrapping `Screen.AllScreens` and
`BeepDockingManager.Monitors` settable. This document asked for it and it is the decision everything
else rests on: *"the display this layout was saved on has been unplugged"* is the case that matters
most, and it is untestable if the monitor set is read from the machine at the point of use.

`FloatBoundsResolver` is a pure function of (saved float, available displays) — no `Screen`, no form,
no side effects. All three verification cases become assertions rather than hardware rearrangements.

### The rule

Device name → geometry overlap → primary, then clamp into the working area.

Identity is tried first deliberately. Rearranging two monitors changes every coordinate while their
identity is unchanged, and matching on geometry alone moves a float to the wrong screen whenever the
user swaps their display order. Geometry is the fallback for layouts written before device names
existed, and for a display replaced by a different one in the same position.

Clamping moves before it resizes: a float larger than it needs to be is a far smaller problem than
one whose caption sits above the top of the screen, where there is nothing to grab.

### Measured

```
both monitors present:  {2100,200,400,300} on \.\DISPLAY2 via DeviceName
secondary unplugged:    {1520,200,400,300} on \.\DISPLAY1 via Primary (clamped)
saved at -5000,-5000:   {0,0,400,300}      on \.\DISPLAY1 via Primary (clamped)
no device name:         {2100,200,400,300} on \.\DISPLAY2 via GeometryOverlap
secondary moved left:   {-400,200,400,300} on \.\DISPLAY2 via DeviceName (clamped)
larger than the screen: {0,0,1920,1040}    on \.\DISPLAY1 via GeometryOverlap (clamped)
no monitors at all:     returned unchanged, MatchKind.NoMonitors
```

### The end-to-end check earned its place

The resolver passed every case while **never being called**. Driving a real float through save and
restore with the display set swapped underneath reported `0 relocation(s)`.

`MaterializeFromDefinition` skips a float whose panel is already `Floating` — and
`CloseAllFloatWindows`, which runs immediately before, destroyed every float window without changing
any panel's state. So the panel sat at `Floating` with nothing backing it, `FloatPanel` returned
early for the same reason, and the window was never re-placed. A float saved on a display that no
longer exists stayed exactly where it was: unreachable.

`CloseAllFloatWindows` now returns its panels to `Docked`, because a panel whose window has been
destroyed is not floating. Leaving a state nothing backs is what made three separate decisions all
read the wrong answer.

This is the third defect in this program with the same shape — **a reference or a state kept after
the thing it describes is gone** (a pruned group still referenced by its panel; a torn-down group
still referenced across a restore; now a destroyed float window still reflected in panel state).

### DPI

`FloatWindow`'s `CaptionHeight` and `ResizeMargin` were compile-time constants used in six places,
so nothing could rescale. They are now DPI-scaled properties, `WM_DPICHANGED` honours the rectangle
Windows suggests — which keeps the window the same physical size and under the cursor rather than
fighting the shell — and re-applies the metrics.

Not verified: this needs two displays at different scales, which the harness cannot synthesise the
way it can synthesise a monitor list, because the DPI change arrives as a real window message. The
code path is small and the scaling is asserted by inspection only. **Recorded as unverified rather
than counted as done.**

### Guides

`DockingGuideOverlay.ShowOver` centred the rosette on the host form with no screen awareness, so a
host straddling two displays could put part of it where the user cannot reach. It now clamps into
the working area of the display under its centre, reusing `FloatBoundsResolver.ClampInto` rather
than a second implementation that could disagree about what "on screen" means.

`DockProbe`: **166 passed, 0 failed**. Solution builds with 0 errors. Docking test suite 48/48,
with one pre-existing intermittent failure from shared static theme state under parallel test
execution (passes in isolation; unrelated to floats).

### Remaining

- [ ] `WM_DPICHANGED` behaviour verified on real mixed-DPI displays
- [ ] Snap-to-owner-edges (`FloatWindow.SnapToOwnerEdges`) still reasons only about the owner form;
      snapping to *display* edges is the other half of what this document's last work item asks for
