# Stage 07 — Declared and never called

**Kind:** cleanup · **Status: done.**

| removed | why |
|---|---|
| `FindHitRegion` | one occurrence repo-wide: its own declaration |
| `HitRegion`, `HitTarget` | only ever produced by `FindHitRegion` |
| `_iconRectangles`, `_labelRectangles` | written, cleared, never read once `FindHitRegion` went |
| the four-argument `UpdateItems` overload | its two extra lists fed only the fields above |

Two lists were being copied on every layout pass so they could be cleared and copied again. Hit
testing works on whole cells, which is the right touch target regardless.

`BottomBarHitTestHelper` is `internal`, so its `public` members are unreachable outside the assembly -
the grep is conclusive rather than suggestive.

## Also corrected

`ShowCTAShadow` declared `[DefaultValue(false)]` while initialising to `true`. The designer omits a
property when it equals the declared default, so setting it to `false` serialised nothing and the
shadow came back on the next load. The attribute now matches the initialiser.

## Still open

`IsOverflow` is computed by the layout helper and surfaced on the control, and nothing acts on it -
there is no overflow strategy, no "More" consolidation. That is a missing feature rather than dead
code, so it is left in place and recorded in the tracker.
