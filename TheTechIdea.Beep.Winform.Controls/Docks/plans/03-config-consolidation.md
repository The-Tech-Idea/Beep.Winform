# Stage 03 — three tables of per-style defaults, and a setter that overwrites the user

**Kind:** structural, with one user-visible consequence. Stage 01 found painters writing colours into
the control's config; this is the same shape one level up, and it is where the colours came from.
**Status:** ☑ done. Six checks green, one intended rendering change, isolated and proven. See
*Outcome*.

## Three tables, one question

"How big is a Material 3 dock item?" has three answers in this folder, and all three are live:

| source | value | who reads it |
|---|---|---|
| `Helpers/DockStyleHelpers.cs:55` `GetRecommendedItemSize` | 56 | `BeepDock.Properties.cs:42`, into `_config.ItemSize` |
| `DockPainterMetrics.cs:136` `DefaultFor` | 52 | the 7 painters that call `GetMetrics`/`DefaultFor` |
| painter literals | varies | the 12 painters that call neither |

Neither table is a fallback for the other. `DockStyleHelpers` writes into `DockConfig`; painters that
read `config.ItemSize` get 56. Painters that ask for metrics get 52. Both are painting the same dock.

The disagreement is not occasional. Comparing the two tables across the 17 named styles:

| property | agrees | disagrees |
|---|---|---|
| item size | 4 | 13 |
| spacing | 5 | 12 |
| max scale | 5 | 7 (+5 unmapped) |
| background opacity | 3 | 9 (+5 unmapped) |

The two tables do not even cover the same styles. `DockPainterMetrics.DefaultFor` has cases for 13;
`CyberpunkDock`, `TerminalDock`, `BubbleDock`, `ArcDock`, `DraculaDock` and `Custom` fall through
`default:` to `DefaultFor(DockStyle.AppleDock, …)` (`DockPainterMetrics.cs:297-299`).
`DockStyleHelpers` has cases for 17 — a different 17: it omits `PillDock`, which the metrics table
covers (`DockPainterMetrics.cs:180`), so `PillDock` falls to `_ => 56` in every one of the helper's
eight switches. Each table has a blind spot the other does not, and each blind spot is filled by a
silent fallback rather than an error.

`ArcDockPainter.cs:12` writes `config.BackgroundOpacity = 0.95f`, which is also what
`DockStyleHelpers.GetRecommendedBackgroundOpacity(ArcDock)` returns (`DockStyleHelpers.cs:270`). A
third copy that happens to agree today is not agreement — it is two places to edit and one of them
will be missed.

There is a fourth source, and it is the one that bites. `MinimalDockPainter` declares its own
constant:

```csharp
private const float BackgroundOpacity = 0.05f;      // MinimalDockPainter.cs:23
…
var bgColor = GetColor(config.BackgroundColor, theme?.BackgroundColor ?? Color.White,
                       BackgroundOpacity);          // :32-36 — the constant, not config
```

The class constant shadows the config property by name, so the call site reads as though it uses
`config.BackgroundOpacity` and does not. `MinimalDock` and `ArcDock` therefore paint their background
at 5% alpha no matter what any of the other three tables say — and `ArcDockPainter` writes `0.95f`
into a config field that its own base class never reads, where it then poisons the value for whatever
style paints next. The probe found this the hard way: a style-switch check written against
`Arc` could not distinguish pass from fail, because every outcome renders within a few units of the
background it was drawn over.

## The setter that overwrites the user

```csharp
public Docks.DockStyle DockStyleType
{
    set
    {
        _config.Style = value;
        _dockPainter = DockPainterFactory.GetPainter(value);
        _config.ItemSize          = DockStyleHelpers.GetRecommendedItemSize(value);      // :42
        _config.DockHeight        = DockStyleHelpers.GetRecommendedDockHeight(value);    // :43
        _config.Spacing           = DockStyleHelpers.GetRecommendedSpacing(value);       // :44
        _config.Padding           = DockStyleHelpers.GetRecommendedPadding(value);       // :45
        _config.MaxScale          = DockStyleHelpers.GetRecommendedMaxScale(value);      // :46
        _config.ShowShadow        = DockStyleHelpers.ShouldShowShadow(value);            // :47
        _config.BackgroundOpacity = DockStyleHelpers.GetRecommendedBackgroundOpacity(value); // :48
    }
}
```

Set `ItemSize = 40`, then set `DockStyleType = MinimalDock`. `ItemSize` is 44. The user's value is
gone and nothing said so. This is stage 01's defect in the other direction: stage 01 persists a style
default the user never asked for, this one discards a value the user did ask for. **A style default
must be a fallback in the read, never a write** — the same rule settles both.

`ShowShadow` is the sharpest case, because it is a `bool` with no "unset" state: once the setter has
run, there is no way to tell a user who wants no shadow from a style that recommends none.

## The profiles are a fourth path, and they go stale

`StyleProfile` and `ColorProfile` (`BeepDock.Properties.cs:384`, `:400`) are designer-serialized and
push into `_config` through `ApplyStyleProfile` / `ApplyColorProfile` (`BeepDock.Methods.cs:102`,
`:123`). They are built once from `_config` in the constructor (`BeepDock.cs:75-96`) and **never
refreshed**. So:

- `DockStyleType = PlankDock` updates `_config` but not `_styleProfile`. The designer grid still shows
  the Apple values. Whatever the designer serializes next is wrong.
- `ApplyColorProfile` (`BeepDock.Methods.cs:130-136`) assigns all five nullable colours
  unconditionally. After it runs, `_config.BackgroundColor` is never null again — which silently
  disables the `??` fallback stage 01 installs. **Stage 01's fix is not complete until this is fixed
  too**, and a stage 01 test that passes before this stage can regress after it.

## The fix

One table. `DockPainterMetrics` is the right home: it already carries the most properties, it is
already per-style, and it is what painters ask for.

1. `DockStyleHelpers`' eight recommendation switches (`:55`, `:83`, `:111`, `:139`, `:167`, `:195`,
   `:223`, `:251`) are deleted. Their values are merged into `DockPainterMetrics.DefaultFor`, which
   gains the six missing cases. Where the two tables disagree, the metrics value wins unless the
   painter visibly depends on the helper value — decide per style, record the decision, do not
   average. `GetControlStyleForDock` (`:19`) stays; it maps to a different system and has one caller.
2. `DockConfig` keeps only what the **user** set. Every dimension becomes nullable in the same way
   the colours already are, and painters read through the stage 01 resolver:
   `ResolveItemSize(config, metrics.ItemSize)`.
3. `DockStyleType`'s setter stops writing dimensions. It sets `Style`, re-resolves the painter,
   invalidates. Nothing else.
4. `DockStyleConfig` and `DockColorConfig` become **views**, not stores: their getters project from
   `_config` + the active metrics, so they cannot go stale. If that is not workable for the designer,
   they are refreshed at the end of every path that writes `_config` — one helper, called from each.
   Two stores that must be kept in sync by hand is the thing this stage exists to remove.
5. `ApplyColorProfile` assigns only colours the profile actually carries. A profile that was never
   edited must not defeat the fallback.

## Verification

The baseline is the folder as it stands; each check states what a failing run prints today.

1. **Table equality.** For each of the 19 styles, assert the value the painter uses for item size,
   spacing, padding, max scale and opacity equals the value `_config` reports. *Today this fails for
   13 of 17 styles on item size alone* — that count is the check's proof it can fail.
2. **User values survive a style change.** Set `ItemSize = 40`, set `DockStyleType = MinimalDock`,
   assert `ItemSize == 40`. *Today: 44.* Repeat for `Spacing`, `Padding`, `MaxScale`, `ShowShadow`,
   `BackgroundOpacity` — six assertions, all currently red.
3. **Style defaults still apply when the user set nothing.** Fresh control, `DockStyleType = PlankDock`,
   assert item size is Plank's, not Apple's. This passes today and must keep passing; without it,
   check 2 could be satisfied by ignoring styles entirely. This is the counterweight, the same role
   step 5 plays in stage 01.
4. **Profiles do not go stale.** `DockStyleType = PlankDock`, then read `StyleProfile`. Assert it
   reports Plank's values. *Today it reports the constructor's Apple values.*
5. **Colour fallback survives a profile.** Assign a default-constructed `ColorProfile`, then set
   `DockStyleType = DraculaDock` and sample the background. Assert Dracula's colour, not the
   profile's grey. *Today the profile wins* — and this is exactly the interaction that would let
   stage 01 look fixed while still being broken.
6. **One table.** Grep for a second per-style switch on `DockStyle` returning sizes. Zero hits, or
   the stage is not done.

## Outcome

There were **four** sources, not three. The fourth only surfaced under measurement:
`DockConfig.CornerRadius` was a flat `16` for every style, present in neither the helper tables nor
the style setter, quietly overriding the per-style radii the metrics table had always declared.

`DockPainterMetrics` now holds one literal `Dictionary<DockStyle, StyleDefaults>` covering all 18
styles plus `Custom` — no switch, no `default:` falling through to AppleDock, and the six rows that
never existed (`Cyberpunk`, `Terminal`, `Bubble`, `Arc`, `Dracula`, `Custom`) are filled in.
`DockStyleHelpers` keeps only `GetControlStyleForDock`; its eight recommendation switches are gone.

### The decision that shaped the merge

The plan assumed the metrics values should win. **They should not.** `DockLayoutHelper` reads
`config.ItemSize`, `config.Spacing` and `config.Padding` — so the *helper* numbers are what the
control has actually been laying out with. Taking the metrics set would have resized 13 of 17 styles
as a side effect of a consolidation. The helper values won; metrics was reconciled to them.

### `DockConfig` holds only what the user set

Each dimension became a nullable backing field with a non-nullable getter that resolves from the
table by `Style`:

```csharp
private int? _itemSize;
public int ItemSize
{
    get => _itemSize ?? DockPainterMetrics.DimensionsFor(Style).ItemSize;
    set => _itemSize = value;
}
```

This is why the ~40 existing readers across the painters and the layout helper needed no changes at
all, while "the user set this" finally became distinguishable from "this equals the default".
`DockStyleType`'s setter now sets `Style`, re-resolves the painter and syncs the profiles — it writes
no dimensions. `ShowShadow` was the case that could not be fixed any other way: as a plain `bool`
there was no value meaning "the user chose this".

### Measured

| check | result |
|---|---|
| layout dimensions == painter metrics | 18 styles agree on size, spacing, padding, height, scale |
| `ItemSize = 40`, change style | stays 40 (was 44) |
| `MaxScale = 2.0`, change style | stays 2.0 (was 1.2) |
| unset dimension still follows the style | Plank 40 → iOS 60 |
| `StyleProfile` after `DockStyleType = PlankDock` | reports PlankDock (was AppleDock) |

### The one rendering change, and how it was isolated

42 of 54 corpus rows changed hash — but **every dock size and every item rectangle was identical**,
so no geometry moved. Rather than assume the difference was the corner radius, the probe was given a
`--pin-legacy-radius` switch that forces `CornerRadius = 16` as before. With it pinned, **all 54 rows
match the pre-stage baseline exactly.** Per-style corner radius is therefore the sole visual
difference, and everything else in this stage is pixel-neutral.

Fifteen styles now get the radius their own metrics row always declared: `TerminalDock` 16 → 4,
`MinimalDock`/`ArcDock`/`PlasmaPanel` → 8, `Windows11`/`GNOME`/`Plank`/`Neon`/`Nord`/`Cyberpunk`/`Dracula` → 12,
`Glassmorphism` → 20, `iOS` → 24, `PillDock`/`BubbleDock` → 28. `AppleDock` and `Material3Dock` stay at 16.

A side effect worth noting: cross-style item distinctness in the `Disabled` state improved from 12/18
to 13/18, because differing corner radii now separate two styles that previously matched. See
[09](09-interaction-state.md).

### Still owed

`DockThemeHelpers.GetDockBorderColor` still hardcodes alpha 100 and takes no opacity argument, so
[01](01-style-switching-is-one-way.md)'s `ResolveBorder` reads `theme.BorderColor` directly rather
than going through it. Giving it the parameters its background sibling already has is a small change
that was deliberately not bundled here, because it restyles borders and this stage was meant to be
geometry-neutral.
