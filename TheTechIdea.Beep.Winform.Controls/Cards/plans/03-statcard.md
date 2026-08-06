# Stage 03 — `BeepStatCard`

**Kind:** refactor · **Files:** `Statuses/` (1,941 lines, 3 partials, 4 painters)
**Do this card first.** Smallest surface, cleanest control, four kinds rather than fifty-six — the
least likely to confuse a pattern problem with a card problem.

## Composition

```
┌──────────────────────────────┐
│ [icon]   [title]             │  icon gutter + text column
│          [value]             │  the KPI — the largest type on the card
│          [trend] [delta]     │  direction glyph + change
│ [spark]                      │  optional chart/progress control
└──────────────────────────────┘
```

Four `BeepLabel`s, one `BeepImage` for the trend glyph, and — for `HeartRate`, `EnergyActivity` and
`Performance` — an existing progress or chart control hosted in the last row. The four painter kinds
become four compositions differing in which rows are present.

## The trend properties finally do something

Three properties are declared and read by nothing, measured across 4,822 files:

| property | references anywhere |
|---|---|
| `TrendText` | 0 |
| `TrendUpSvgPath` | 0 |
| `TrendDownSvgPath` | 0 |

A stat card is a number and its direction of travel; all three properties describing the direction
reach nothing today. In the composed card they are the trend row: `TrendText` is the label,
`TrendUpSvgPath`/`TrendDownSvgPath` the `BeepImage`'s path chosen by the sign of the delta.

This is the clearest case in the folder of composition turning a dead property into a wired one
almost incidentally — the row exists, so the properties have somewhere to go.

## `RegisterPainter` is public, and its shape should survive

`BeepStatCard.PainterRegistry.cs` exposes `RegisterPainter(StatCardPainterKind, IStatCardPainter)`, so
a consumer can add a kind without editing a switch. It is the best piece of design in the folder.

Removing it is a caller-visible break. **The composition equivalent has to exist before this card
migrates** — a way to register a composition for a kind — or the refactor quietly deletes an extension
point. Stage [01](01-composition-pattern.md) owns providing it; this stage is the first consumer.

## The namespace

`Statuses/` declares `namespace …Controls.StatusCards` while the other five cards are in `…Controls`.
Neither is wrong alone; both together means `using` lines differ per card for no inferable reason.
A namespace rename is caller-visible — a decision to record, not a tidy-up to slip into a refactor.

## Verification

1. **The four kinds render distinctly**, compared against the pre-refactor capture.
2. **A trend renders, and up differs from down.** *Today identical: the properties reach nothing.*
3. **The registry still extends.** Register a composition for a new kind through the public API and
   assert it is used. Guards the extension point through the migration.
4. **The value label is the largest text on the card** — asserted from font size, because a KPI that
   does not dominate is a stat card that has lost its point.
