# 13 — Verification Harness

**Priority P0.** Nothing else in this program can be called done without it.

## Why this is first, not last

None of the defects catalogued in this program are subtle in a *rendered* tooltip — an arrow
pointing at empty space, a tooltip stranded over another application, a `LayoutVariant` that changes
nothing. They are invisible in a build, and they survived a previous planning cycle precisely
because nothing rendered them.

The same lesson was learned twice already in this repo:

- `BeepGridPro` — a toolbar bug was "fixed" three times against rect coordinates before rendering to
  PNG revealed a null-brush exception silently aborting half the paint.
- `BeepTree` — 25 painters clipped every label; a contact sheet found it in one look, and the same
  sheet immediately surfaced two further defects (a blank painter and a truncating one).

## What the harness must do

### 1. Render, don't just assert

A probe that hosts a real `BeepTree`-style form, shows tooltips, and captures PNGs via
`DrawToBitmap` — plus magnified crops, because arrow-tip alignment is a few pixels.

### 2. Geometry assertions

Machine-checked invariants, failing rather than printing:

| Invariant | Covers |
|---|---|
| Tooltip rect lies fully within the monitor's working area (minus padding) | [01](01-anchor-and-placement.md) |
| Tooltip does not overlap its anchor rect | [01](01-anchor-and-placement.md) |
| Arrow tip x/y is within 1px of the anchor centre, or the arrow is hidden | [02](02-arrow-tracking.md) |
| `TopStart`/`Top`/`TopEnd` produce three distinct positions | [01](01-anchor-and-placement.md) |
| Explicit placement flips only to its opposite | [01](01-anchor-and-placement.md) |
| Resolved fore/back contrast ≥ `MinContrastRatio` | [09](09-accessibility.md), [11](11-theming-and-styles.md) |
| No two `ToolTipType` values resolve to identical colours in a theme | [11](11-theming-and-styles.md) |
| Every `LayoutVariant` renders distinctly | [07](07-content-pipeline.md) |

### 3. The edge sweep

Walk an anchor around the perimeter of each monitor — all four edges and four corners — and assert
the invariants at every step. This is where flip/shift/arrow bugs live, and it is entirely
mechanical.

### 4. A "declared but never read" check

Three config properties (`PersistOnHover`, `Pinnable`, `LoadPreviewAsync`) are documented and never
consumed. A reflection-based test can enumerate `ToolTipConfig`'s public properties and flag any
that no code in the assembly reads. That check would have caught all three, and it prevents a fourth.

### 5. Lifecycle counters

Handle counts, window counts and repositions-per-scroll, per [12](12-lifecycle-and-performance.md).

### 6. Contact sheets

One sheet per axis, reviewed by eye at least once and then stored as a baseline to diff against:

- 7 `LayoutVariant` values
- 21 `ToolTipType` values × 3 themes
- every `BeepControlStyle`
- 100% / 150% / 200% DPI

## Work

1. Build `scratchpad/ToolTipProbe` first, reproducing the P0 defects **before** fixing them, so each
   fix has a failing check to turn green.
2. Promote it out of the scratchpad once stable, next to the control, with a single-command run.
3. Store baseline images so later changes diff rather than needing re-review.

## Verification

The harness is itself verified by reproducing today's known defects: a 1px-anchor placement failure,
an arrow that misses its anchor near a screen edge, and a tooltip that stays put when its form
moves. If a fresh harness does not fail on all three, it is not measuring the right things.
