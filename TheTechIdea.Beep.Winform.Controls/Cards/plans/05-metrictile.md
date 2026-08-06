# Stage 05 — `BeepMetricTile`

**Kind:** refactor · **Files:** `Metrices/` (1,575 lines, 5 helper files)

6 public properties, 2 events, one `DrawContent` override. The smallest surface of the six.

## Composition

```
┌──────────────────────────────┐
│ [silhouette — behind]        │
│ [icon]   [title]             │
│          [value]             │
│          [delta]             │
└──────────────────────────────┘
```

Three labels, one icon, and the silhouette.

**Status: done.** The tile composes from four controls and the silhouette layers correctly.

## The silhouette question is answered: option 2 works

The plan gave two options and said to prove one before composing the other cards. **Option 2 works.**

The silhouette is the scaffold's `BackgroundImage`, faded once into a bitmap rather than drawn per
paint, with `ImageLayout.Center`. The labels are marked `IsTransparentBackground = true`, so the glyph
reads *through* the text rows instead of being punched out in label-shaped rectangles.

Option 1 — a `BeepImage` in a spanning cell — would not have worked, for the reason the plan
predicted: `IsChild` gives a control its parent's **colour**, not transparency over whatever the
parent painted, so the opaque labels would have covered it.

Rendered and inspected: 4 controls, 71 distinct colours, the faded circle visible behind "Views",
"31" and "+3 last day". No painted implementation was needed.

## The silhouette is the one part composition does not hand you

`BackgroundSilhouette` is a large faded glyph *behind* the number — the standard metric-tile treatment,
and what makes a tile readable at a glance across a dashboard. Declared, and nothing draws it.

A `TableLayoutPanel` puts controls in cells; it does not layer them. Two options, and this is a real
decision rather than a detail:

1. **A `BeepImage` in a cell spanning the tile, added first so the labels draw over it.** Z-order in
   WinForms is child order, so this works — but the labels must be `IsChild` and genuinely transparent
   over it, and *that* is the thing the dialogs program found does not hold: `IsChild` gives a control
   its parent's **colour**, not transparency over whatever the parent painted. A label over an image
   will show the parent's flat colour, not the image.
2. **The silhouette as the tile's own background image**, with the labels transparent over the panel
   rather than over a sibling control.

Option 2 is the one likely to work, for the same reason option 1's equivalent failed for dialog header
bands. **Prove it with one tile before composing the other five cards** — if neither works, the honest
answer is that this single element keeps a painted implementation and the stage records why.

## `IconImage`

`IconImage` has no references and overlaps whatever the tile already uses for its icon. Check which
before wiring: two icon inputs on a six-property control is duplication. **Deletion is as likely to be
the right answer as implementation** — do not wire a second path property just because it exists.

## The name is misspelled twice

`Metrices/BeepMetericTile.cs` — folder and file both misspelled, class correctly `BeepMetricTile`. A
file rename is safe; a folder rename touches project references. Worth doing, worth doing separately
from a behavioural change.

## Verification

1. **The silhouette renders behind the text**, and the value's pixels are unchanged by its presence.
   A background treatment that costs legibility is worse than none.
2. **The silhouette follows the theme**, not a literal grey.
3. **If neither layering approach works**, the check records that and the stage says so — a documented
   painted element beats an undocumented broken one.
4. **`IconImage` renders, or is gone.** Assert whichever is decided.
