# Stage 02 — three interface members nothing calls

**Kind:** structural. Nothing misbehaves today; the control cannot do what its own interface says.
**Status:** ☑ done, Option A taken. Four checks green; 17 styles byte-identical, `AppleDock`
deliberately different. See *Outcome*.

## What the survey found

`IDockPainter` has seven members (`Painters/IDockPainter.cs`):

| member | called by |
|---|---|
| `PaintDockBackground` | `BeepDock.Drawing.cs:24` |
| `PaintDockItem` | `BeepDock.Drawing.cs:33` |
| `PaintIndicator` | `BeepDock.Drawing.cs:36` |
| `PaintSeparator` | `BeepDock.Drawing.cs` (separator branch) |
| `CalculateItemBounds` | **nothing** |
| `CalculateDockBounds` | **nothing** |
| `HitTest` | **nothing** |

All real geometry goes to the static helpers instead:

```csharp
var bounds       = DockLayoutHelper.CalculateItemBounds(...);   // BeepDock.Items.cs:69
int index        = DockHitTestHelper.HitTest(point, _itemStates);// BeepDock.Methods.cs:54
int hoveredIndex = DockHitTestHelper.HitTest(e.Location, ...);   // BeepDock.DragDrop.cs:69
int clickedIndex = DockHitTestHelper.HitTest(e.Location, ...);   // BeepDock.Mouse.cs:41
```

The three geometry members are implemented once in `DockPainterBase` (lines 128, 151) and
**overridden by no painter**. So they are not merely uncalled — no style has ever expressed a
different layout through them, which is why nothing has noticed.

The consequence: **a painter can change pixels but not geometry.** An Apple dock magnifies items on
hover with neighbours displaced; a Windows 11 dock centres fixed-size icons; a Plasma panel is a
full-width bar. Those are layout differences, and the interface offers a way to express them that
the control does not use. Every style is laid out identically and then painted differently.

## The decision this stage exists to make

Two coherent answers. Picking neither is what produced the current state.

**Option A — the painter owns geometry.**
`BeepDock` asks the active painter for item bounds and hit-testing; `DockLayoutHelper` and
`DockHitTestHelper` become the implementation `DockPainterBase` uses, so the default behaviour is
unchanged and a style can override. This is what the interface already promises, and it is what
made the `GridX` header work once the painters were given the layout.

**Option B — the helpers own geometry.**
Delete the three members from `IDockPainter` and `DockPainterBase`. The interface then describes
exactly what painters do: paint. Styles that need different geometry get it by configuring
`DockConfig`, not by overriding.

**Recommendation: A.** The signatures were designed for it (`CalculateItemBounds` already takes the
index, all states, config and dock bounds — everything a magnifying dock needs), the layout helper
already computes magnification scales, and the difference between an Apple dock and a taskbar is
mostly geometry. Option B is cheaper and honest, but it permanently forecloses the thing that makes
19 styles worth having.

Whichever is chosen, the outcome is the same shape: **one implementation, reachable, with no second
copy left behind.**

## Work if A is taken

1. `DockPainterBase.CalculateItemBounds` / `CalculateDockBounds` / `HitTest` delegate to
   `DockLayoutHelper` / `DockHitTestHelper`. Behaviour is unchanged for all 19 styles at this point.
2. `BeepDock.Items.cs:69`, `BeepDock.Methods.cs:54`, `BeepDock.DragDrop.cs:69`, `BeepDock.Mouse.cs:41`
   and `BeepDockPopup.cs:355` call the painter instead of the helper directly. `BeepDock.Items.cs:124`
   (`DockLayoutHelper.CalculateDockSize`) is a third layout entry point with no interface member at
   all — it either joins `CalculateDockBounds` or is explicitly left to the control. Leaving it
   undecided is how the folder got here.
3. The signature mismatch is resolved rather than bridged: `DockLayoutHelper.CalculateItemBounds`
   returns `Rectangle[]` for all items, `IDockPainter.CalculateItemBounds` returns one `Rectangle`
   for an index. Computing all bounds per item is O(n²) per layout pass. The interface gains a
   whole-set method and loses the per-index one — no adapter, no both.
4. `DockHitTestHelper.HitTest` returns `int`; `IDockPainter.HitTest` returns `DockItemState`. Pick
   one. The index is what all four call sites actually use.
5. Only then, give one style real geometry — `AppleDockPainter` magnification — as the proof the
   wiring carries a difference.

## Verification

Deletion plus a clean compile is authoritative for deadness; grep is not. Before any rewrite:

1. Delete the three members from `IDockPainter` and `DockPainterBase`, build the solution. If it
   compiles, they are dead and the survey is confirmed. Restore, then implement.
2. After wiring: set `Style = AppleDock`, hover the middle item, capture item bounds. Set
   `Style = Windows11Dock`, same hover, capture again. Assert the bounds **differ**. This is the
   assertion the whole stage exists for, and it is the one that can fail — if the painter is wired
   but its override is not reached, the two capture identical rectangles.
3. Assert every style still hit-tests to the item under the cursor: for each of the 19, place a
   point at the centre of item *k* and assert `HitTest` returns *k*. A geometry change that breaks
   hit-testing is the most likely regression, and it is silent.
4. Baseline guard: capture item bounds for all 19 styles **before** the change, and assert they are
   unchanged after step 1 of the work. Only `AppleDockPainter` may differ, and only after step 5.

## Outcome

**Option A.** The deletion test ran first, as the stage requires, and passed twice: removing the three
members from `IDockPainter` compiled with 0 errors, and removing their 110-line implementation region
from `DockPainterBase` compiled with 0 errors too. Dead, confirmed, not inferred.

### The signature mismatches were resolved, not bridged

| member | was | now |
|---|---|---|
| `CalculateItemBounds` | one `Rectangle` for an index — O(n²) per layout pass | `Rectangle[]` for the whole set, matching `DockLayoutHelper` |
| `CalculateDockBounds` | `Rectangle` from a container size, unused shape | `CalculateDockSize(int, DockConfig)`, matching the helper and the one real caller |
| `HitTest` | returned `DockItemState` | returns `int` — the index all four call sites actually wanted |

`DockPainterBase` implements all three by delegating to `DockLayoutHelper` / `DockHitTestHelper`, so
the default behaviour for all 19 styles is unchanged and the helpers remain the implementation rather
than a parallel one.

Call sites now go through the painter: `BeepDock.Items.cs` (bounds and size), `BeepDock.Methods.cs`,
`BeepDock.Mouse.cs`, `BeepDock.DragDrop.cs` and `BeepDockPopup.cs`.

**The wiring alone changed nothing: 0 of 54 corpus rows.** That was checked before any style was
given its own geometry, which is what makes the next part attributable.

### `AppleDock` owns its geometry

`AppleDockPainter.CalculateItemBounds` overrides the shared layout with a cosine bulge over four
neighbours either side, where the default is a linear falloff over two. The default's linear ramp
produces a visible kink at the edge of its influence range; the real dock swells rather than steps.

| check | result |
|---|---|
| override is reached | item0 59px vs default 56px |
| magnifies further out | 3 away: Apple 66px, default 56px |
| 17 styles unchanged | byte-identical to the helper |
| hit-testing still lands | 18 styles × 9 items |

Corpus: **only `AppleDock` changed** — hovered item bounds `125;-6;84;84` → `183;-6;84;84`. The
hovered item is the same size (max scale is unchanged); it sits further along because its neighbours
now grow too. Seventeen styles untouched.

### A gap this closed in the harness

The corpus built its item states by calling `DockLayoutHelper` directly, so it kept reporting "0
changed" even after the override landed — it was measuring the path the control no longer takes. A
style that laid itself out differently would have been invisible in every render the harness
captured. `Probe.MakeStates` now goes through the painter, which is how `AppleDock`'s change became
visible at all.

### Still owed

`DockPainterBase`'s private layout helpers (`CalculateTotalSize`, `CalculateAlignedPosition`,
`GetDockY`, `GetDockX`) went with the old region and are not needed by the delegating implementation.
`HoverOffset` — dead before this stage, since its only reader was the uncalled
`CalculateItemBounds` — is still not read by the delegating path, so [05](05-dead-capability-surface.md)
keeps it.
