# Stage 04 — Clicks forked, and painter-owned cells never opened their popup

**Kind:** wrong-behaviour · **Status: done.**

Each paint registers per-item hit areas through `BottomBarHitTestHelper.UpdateItems`:

```csharp
AddHitArea($"BottomBarItem_{i}", rect, null, () => HandleItemClick(idx, MouseButtons.Left));
```

Then `RegisterHitAreas` runs, and five painters re-add the **same key** with their own lambda, so that
a style can make a cell bigger than its grid rectangle - the CTA circle, the selected pill, the
selected bubble. `ControlHitTestHelper.AddHitArea` replaces by name, so this **destroyed** the
helper's registration instead of extending it, and the cell's click ran `OnItemClicked`:

```csharp
SelectedItem = Items[idx];
ItemClicked?.Invoke(...);
```

which skips `HandleItemClick` entirely. Consequences on that one cell:

- an item with `Children` raised no `PopupRequested`, so **its submenu never opened** - while the
  control still advertised `AccessibleStates.HasPopup` and a "(N sub-items)" tooltip for it
- `FocusedIndex` was never updated, so the accessible description kept naming the previously focused
  cell

Same control, same gesture, two behaviours depending on which cell was clicked.

## Fixed by moving the handler, not the rectangle

Painters hand over a rectangle now; the helper keeps the handler:

```csharp
context.BarHitTest?.SetItemHitArea(index, rect);
```

One click path for every cell, whichever style painted it.

## Verification

Four styles, each clicked on the cell that style takes ownership of, with `Children` set on it:

```
PASS  Pill                 painter cell opens its child popup: 1 popup(s)
PASS  Bubble               painter cell opens its child popup: 1 popup(s)
PASS  FloatingCTA          painter cell opens its child popup: 1 popup(s)
PASS  OutlineFloatingCTA   painter cell opens its child popup: 1 popup(s)
```

**Stated honestly:** these were seen green after the fix, not red before it. The pre-fix behaviour is
established by reading - `OnItemClicked` has no `Children` branch - rather than by observing a
failure, which is weaker evidence than the other stages carry.
