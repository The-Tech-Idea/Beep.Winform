# Stage 01 — the card scaffold, and the parts every card composes from

**Kind:** foundation · **Runs before every card stage**

Six cards and 56 styles build on this. It is worth getting right before anything consumes it, because
discovering a flaw on the sixth card means redoing five.

## The scaffold

A card is a `BaseControl` hosting one `TableLayoutPanel`, with one control per cell. Not a painter, not
a dock stack, not coordinates.

```
┌─────────────────────────────────────────┐
│ [media]                                 │  row 0 — image / avatar / chart, spans as needed
├──────────┬──────────────────────────────┤
│ [icon]   │ [title]                      │  row 1 — icon gutter + text column
│          │ [subtitle]                   │  row 2
│          │ [body]                       │  row 3
├──────────┴──────────────────────────────┤
│                      [actions]          │  row n — right-aligned action row
└─────────────────────────────────────────┘
```

Two columns: an icon gutter that collapses to zero width when there is no icon, and a text column the
title, subtitle and body share. **That shared column is the point** — it is what gives them one left
edge without anyone maintaining a margin. The dialogs program spent a session learning that a header
grid and a body grid with different column counts cannot be aligned by adjusting either.

Rows are `AutoSize` except the one that should absorb slack. A card that hugs its content is the
default; one row takes `Percent(100)` when the card must fill a fixed height.

## The parts

Every card is built from controls that already exist. No new drawing primitives.

| part | control | notes |
|---|---|---|
| title, subtitle, body, caption | `BeepLabel` | `IsChild = true` so it takes the card's surface; `WordWrap` + `Multiline` for anything that wraps |
| icon, avatar, media, silhouette | `BeepImage` | the only control that renders and themes SVGs |
| actions | `BeepButton` | real hit-testing, focus and keyboard access — which painted affordances do not have |
| badges, chips, status pills | existing chip/badge controls | check what `Chips/` already provides before adding anything |
| ratings | existing rating control | `Ratings/` exists; a star row is not a card concern |
| progress, charts | existing progress/chart controls | a card hosts them, it does not draw them |

**If a part does not exist as a control, that is a finding, not a licence to paint.** Record it and
decide; do not reintroduce a painter for one element.

## What the scaffold does not do

- **It does not assign colours.** Each control resolves its own from `BeepThemesManager` and re-applies
  on `ThemeChanged` without being asked. A scaffold that pushes colours down competes with that and
  loses — the assignment is overwritten by the control's own `ApplyTheme`, or covered by whatever
  paints on top.
- **It does not scale anything by hand.** The layout engine and the controls handle DPI.
- **It does not set accessible names.** A `BeepLabel` showing a name *is* that text.

Each of those is a category of defect the current painters have, removed by not having the code.

## Composition lives in the designer file

`InitializeComponent` creates the controls and parents them with plain
`Controls.Add(control, column, row)` statements. Composition done at runtime leaves the card blank on
the Visual Studio design surface, because the designer renders what `InitializeComponent` parents and
method calls are not designer-serialisable.

The `.cs` file keeps only what a designer file cannot express: wiring events, and anything conditional
on data.

## How a style selects a composition

`CardStyle` stays exactly as it is — 56 public values, unchanged, so no caller is affected. What
changes is what it selects: a composition rather than a painter.

`BeepStatCard`'s `RegisterPainter` is the extension point worth preserving in shape — a consumer can
add a kind without editing a switch. The composition equivalent needs to exist before stage
[03](03-statcard.md) migrates that card, or the refactor silently removes an extension point.

## Verification

1. **The scaffold composes and lays out** with an icon, without an icon, with and without media, and
   with zero, one and three actions — the shapes every card needs.
2. **The gutter collapses.** With no icon, the text column starts at the card's padding, not indented
   by an empty column.
3. **Title, subtitle and body share one left edge**, asserted from control bounds. This is the
   invariant the two-column scaffold exists for.
4. **A theme change moves the card** with no code in the scaffold — assert every part's colour differs
   before and after `BeepThemesManager.CurrentThemeName` changes.
5. **DPI scales the card** at 96 and 144 with no scaling code in the scaffold.
6. **Actions are reachable** — focusable, keyboard-activatable, and hit-testable at their centre.
   Painted affordances fail all three, which is the difference this refactor is for.
