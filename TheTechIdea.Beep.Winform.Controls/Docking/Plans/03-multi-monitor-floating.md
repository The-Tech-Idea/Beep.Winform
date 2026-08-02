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
