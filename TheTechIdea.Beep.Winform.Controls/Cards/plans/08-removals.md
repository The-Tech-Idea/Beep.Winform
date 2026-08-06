# Stage 08 — deleting the painters, and the helpers they needed

**Kind:** structural · **Runs last.** A painter deleted while something still selects it is a crash,
not a cleanup.

**Status: done.** `Cards/` went from **112 files / 22,578 lines to 36 files / 9,659 lines** — 76 files
and 12,919 lines removed, with every card still composing.

## What was deleted

| | |
|---|---|
| `Cards/Painters/` | the 56 `BeepCard` painters |
| `Cards/Statuses/Painters/` | the four stat-card painters |
| `Helpers/ICardPainter.cs`, `Helpers/CardPaintCache.cs` | the painter contract and its cache |
| `Helpers/ICardInteractive.cs` | hit areas as rectangles, which controls replace |
| `BeepCard.Drawing.cs` (520 lines) | including `PaintEnhancedButton`, `DrawLoadingSkeleton`, `DrawAccentBar`, `DrawFocusRing`, `DrawRippleOverlay`, `DrawAuxiliaryIcons` |
| `BeepCard.Layout.cs` | `BuildLayoutContext` and `RefreshHitAreas` |
| `BeepStatCard.PainterRegistry.cs`, `StatCardPainterKind.cs` | the registry and its enum |
| 8 helper files | the Font and Layout families, once nothing measured text or computed a rectangle |

## `RegisterPainter` was public, and it is gone

The tracker flagged this as a caller-visible break needing a recorded decision rather than a silent
one. **The decision was made explicitly:** there is no legacy or compatibility constraint on cards, so
`RegisterPainter`, `IStatCardPainter` and `StatCardPainterKind` are removed outright rather than
deprecated. Nothing outside `Cards/` referenced them — verified across the solution, and the
similarly-named `RegisterPainter` on `ProjectCards` and `Docking` is a different type in a different
namespace and is untouched.

## The Accessibility, Icon and Theme helpers stayed

Only the **Font** and **Layout** families died, and they died because nothing measures text or computes
a rectangle any more. The remaining three families still have one caller each and do work composition
does not do: resolving an icon name to a path, applying high-contrast adjustments, reading theme
colours for the card surface itself.

`FontHelpers` was checked for the five-way duplication the survey reported: **0 of 4 methods were
identical across the families**, so they were kept on that basis earlier. They are deleted now for the
different and better reason that they have no callers.

## Three public properties would have died silently

`ShowSelectionCheckbox`, `ContextMenuIcon` and `IsCollapsible` were rendered by `DrawAuxiliaryIcons`
and hit-tested against rectangles. Deleting the paint pass without composing them would have left three
live properties doing nothing — reintroducing the exact defect this refactor exists to remove. They are
composed as a `BeepCheckBoxBool` and two `BeepButton`s in a chrome row.

`ContextMenuIcon` defaults to a real glyph, so **every card carries an overflow button**. That is the
property's default behaving as declared, and it is why the verification counts *content* controls
rather than all controls — a control count cannot tell a composed card from an empty one.

## Two swallowed exceptions are gone

The tracker recorded one, at `BeepCard.cs:238`. There were two: `SafeInvalidate`'s bare `catch`, and a
`try` around the **entire** constructor body that wrote the failure to `Debug` and carried on with a
half-built card. Both are removed.


## What goes

| | files | lines |
|---|---|---|
| `Painters/` — 55 painters + rendering helpers | 56 | 10,727 |
| `Statuses/Painters/` — 4 painters + base + interface | 6 | — |
| `Helpers/CardPaintCache.cs` | 1 | — |
| `Helpers/ICardPainter.cs` | 1 | — |
| the `DrawContent` override on each of the six controls | — | — |

Roughly **half this folder's 22,362 lines**, replaced by compositions and the controls that already
exist.

## The helper families, decided by measurement

Each card carries five helper files. Measured by hashing every `public static` body:

| family | copies | methods | byte-identical across all | verdict |
|---|---|---|---|---|
| `ThemeHelpers` | 5 | 27 | 22 | **delete** — controls resolve their own theme |
| `LayoutHelpers` | 4 | 21 | 17 | **delete** — they compute rectangles a `TableLayoutPanel` computes |
| `IconHelpers` | 5 | 17 | 15 | **mostly delete** — `BeepImage` renders and themes SVGs; keep only genuine icon *selection* logic |
| `AccessibilityHelpers` | 5 | 15 | 8 | **mostly delete** — a `BeepLabel` showing text is that text; keep high-contrast and reduced-motion queries |
| `FontHelpers` | 5 | 4 | **0** | **keep all five** |

**`FontHelpers` is the exception and it is not a rounding error.** Zero of its four methods match
across cards, because a pull-quote genuinely wants different type from a KPI or a task label. Those
choices become font assignments on the composed labels — the decisions survive even though the helper
that applied them to a `Graphics` does not. Deleting them as duplication would replace five correct
behaviours with one wrong one.

`IsHighContrastMode` (`571afd52`) and `IsReducedMotionEnabled` (`1d4522d6`) are byte-identical in all
five files and are system queries, not card logic. One copy, in `Helpers/`.

## The public API break

These are public and removing them is caller-visible:

- `ICardPainter`, `IStatCardPainter`
- `BeepStatCard.RegisterPainter(StatCardPainterKind, IStatCardPainter)` — an extension point a
  consumer can use today to add a kind without editing a switch
- the 55 painter classes themselves, if any consumer constructs one directly

**Do not delete these silently.** Stage [01](01-composition-pattern.md) provides the composition
equivalent of `RegisterPainter`; stage [03](03-statcard.md) is its first consumer. The interfaces
should be marked `[Obsolete]` pointing at the replacement for one release rather than removed outright
— the same treatment `CloseOnClickOutside` got in the dialogs program, and for the same reason: a
published surface with consumers is not the compiler's to reclaim.

## Order

1. Every card migrated and verified green.
2. Assert **no construction site** remains for any painter class — this is the check that makes
   deletion safe, and it is mechanical.
3. Delete painters, `CardPaintCache`, the `DrawContent` overrides.
4. Delete the helper families above; consolidate the two system queries.
5. Mark the interfaces obsolete.

## Verification

1. **Nothing references a painter** — grep for construction sites, assert zero, before any file is
   removed.
2. **Every card still renders**, compared against the corpus captured in stage
   [09](09-verification.md) *before* stage 01 began.
3. **`FontHelpers` still has five implementations**, asserted explicitly, so a later tidy-up does not
   merge them.
4. **The build has no unreferenced-symbol warnings** in `Cards/` afterwards — the cheap check that a
   deletion pass finished rather than stopped halfway.
