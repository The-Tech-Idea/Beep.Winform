# Stage 01 — painters permanently mutate the control's config

**Kind:** defect. A user hits this by setting a property the control publishes.
**Status:** ☑ done. All six checks green, corpus unchanged. See *Outcome* at the end, including the
part of this that is deliberately **not** finished.

## What happens

`DockConfig` is a **class** (`DockConfig.cs:10`), and `BeepDock` holds exactly one instance for the
lifetime of the control:

```csharp
private readonly DockConfig _config;          // BeepDock.cs:31
```

`DrawContent` passes that instance straight to the painter:

```csharp
_dockPainter.PaintDockBackground(g, ClientRectangle, _config, _currentTheme);   // BeepDock.Drawing.cs:24
```

Five painters then write into it:

```csharp
config.BackgroundColor ??= Color.FromArgb(40, 42, 54);      // DraculaDockPainter.cs:10
config.BorderColor     ??= Color.FromArgb(98, 114, 164);    // DraculaDockPainter.cs:11
```

Fifteen sites in total — two conditional colour writes and one unconditional opacity write each:

| file | `??=` colours | `=` opacity |
|---|---|---|
| `Painters/ArcDockPainter.cs` | 10, 11 | 12 |
| `Painters/BubbleDockPainter.cs` | 10, 11 | 12 |
| `Painters/CyberpunkDockPainter.cs` | 11, 12 | 13 |
| `Painters/DraculaDockPainter.cs` | 10, 11 | 12 |
| `Painters/TerminalDockPainter.cs` | 10, 11 | 12 |

Each of these five painters is 16–17 lines and overrides nothing but `PaintDockBackground` — the
mutation *is* the style. See [09](09-interaction-state.md) for what that means when comparing renders.

`??=` assigns only when the target is null. So the **first** style that paints writes its colours
into the control's own config, and they are never null again. Every later painter's `??=` is a no-op.

The control clearly intends style to be switchable at runtime — the setter re-resolves the painter:

```csharp
_dockPainter = DockPainterFactory.GetPainter(value);     // BeepDock.Properties.cs:41
```

So: set `Style = DraculaDock`, let it paint once, set `Style = ArcDock`. The painter changes. The
background stays Dracula's, permanently, for the life of the control.

## A second, quieter problem in the same lines

```csharp
var cyber = config;                    // CyberpunkDockPainter.cs:10
cyber.BackgroundColor ??= ...;
```

`config` is a reference. `var cyber = config` copies the reference, not the object. This reads
exactly like a defensive copy and is not one — it is the most misleading of the five, because it
looks like the author already considered the problem and handled it.

## Why `??=` was reached for

The intent is reasonable: *"use the user's colour if they set one, otherwise the style's colour."*
That is a fallback, and a fallback belongs in the read, not in a write to shared state. The bug is
not the defaulting — it is that the default is **persisted**.

## The fix

Resolve colours without mutating. The painter asks for an effective value; the config keeps holding
only what the user actually set:

```csharp
// DockPainterBase
protected static Color ResolveBackground(DockConfig config, Color styleDefault)
    => config.BackgroundColor ?? styleDefault;
```

Each of the five painters becomes a read:

```csharp
public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
{
    var background = ResolveBackground(config, Color.FromArgb(40, 42, 54));
    var border     = ResolveBorder(config, Color.FromArgb(98, 114, 164));
    ...
}
```

`BackgroundOpacity` is assigned unconditionally rather than with `??=` (`= 0.94f`), so it overwrites
a user value on every paint. It gets the same treatment: a style default that the config overrides,
never a write.

This also removes the need for `??=` to exist in a painter at all, which makes the rule easy to
enforce: **a painter never assigns to `DockConfig`.** That is a one-line grep in review.

## Verification

The check must be able to fail. The baseline is the same control, same sequence, with the assertion
inverted — so state the failing shape explicitly:

1. Construct a `BeepDock`, set `Style = DraculaDock`, paint to a bitmap.
2. Assert `_config.BackgroundColor` is **still null** — the user set no colour, so the config must
   not have acquired one. *Today this fails: it holds `40,42,54`.*
3. Set `Style = ArcDock`, paint again, sample the background pixel.
4. Assert it matches Arc's `244,245,247`, not Dracula's. *Today this fails.*
5. Set `config.BackgroundColor = Color.Red` explicitly, paint, sample. Assert red — the user's value
   must still win over the style default. This one passes today and must keep passing; without it,
   "never mutate" could be satisfied by ignoring user colours entirely.
6. Round-trip every style: for each of the 19, set it, paint, sample, and assert the sampled colour
   differs from the previously sampled one wherever the two styles declare different defaults. This
   is the assertion that would have caught the bug in the first place.

Step 5 is the one that keeps the others honest — steps 2–4 alone could be passed by a change that
breaks user-supplied colours.

## Outcome

`DockPainterBase` gained a colour-resolution region. A painter declares `StyleBackgroundColor`,
`StyleBorderColor` and `StyleBackgroundOpacity`, and `ResolveBackground` / `ResolveBorder` decide at
read time. The five mutating painters became declarations — `DraculaDockPainter` is now four lines
and overrides no method. **15 mutation sites, 0 remaining.**

Measured: config untouched after painting all 18 styles; Dracula → Terminal now samples 1 unit from
Terminal and 81 from Dracula, where it was 2 from Dracula before; an explicitly set colour still
wins. All 18 styles render pixel-identical to the pre-stage baseline — 54 corpus rows, zero
differences.

### The theme question, and what it changed

The first cut of this stage declared style palettes as plain constants and resolved
`user → style → theme`. That preserved behaviour exactly and was wrong in shape: it made "style beats
theme, always" a structural property of the base class. The folder already had a theme layer —
`DockThemeHelpers`, 213 lines, documenting the priority `Custom color > Theme > Default` — **and no
painter called it.** Nor could they: `BaseControl.UseThemeColors` never reached a painter, and
`DockPainterBase.cs:373` guessed it as `theme != null`.

Resolved as: **theme-led styles resolve through `DockThemeHelpers`; named-palette styles keep their
palette.** A painter declares which it is via `IsNamedPalette`. Dracula, Nord, Cyberpunk, Terminal
and Arc are named — the palette is why the style was chosen, and a Dracula dock in the ambient theme
is not a Dracula dock. Everything else is theme-led, with its palette as the fallback for
`UseThemeColors = false`.

`UseThemeColors` reaches painters through `DockConfig`, written in exactly one place
(`BeepDock.ApplyTheme`). It cannot be a painter property: `DockPainterFactory` hands out **shared
singletons**, and per-control state on a shared painter is the bug class this stage removes.

The corpus being unchanged nearly hid a hole here. All 18 styles rendering identically is what a
clean refactor looks like *and* what a theme step that never executes looks like — `DefaultTheme`'s
colours coincide with the fallbacks the painters already used. Proving it needed two different
themes:

| check | measured |
|---|---|
| theme-led follows the theme | PillDock: `74,78,89` under ArcLinux vs `225,225,225` under Brutalist |
| named palette ignores the theme | DraculaDock: `45,47,59` under both, distance 0 |
| `UseThemeColors = false` falls back to the palette | PillDock: `232,232,232` vs `225,225,225` |

### Completing the conversion

When this stage first landed the mechanism covered only **7 of 18 styles** — three painters routed
through the resolvers and eight open-coded their colours, unable to honour `UseThemeColors` at all.
That was tracked as a standing red check rather than a note, and it has since been closed:
**every painter now resolves colour through `DockThemeHelpers`**, and the check is locked green.

The conversion was smaller than it looked. Seven of the eight already consulted the theme — as
`GetColor(config.X, theme?.Y ?? fallback, opacity)` — so they were not ignoring it, they were
bypassing the documented priority and could not see `UseThemeColors`. Routing them through
`ResolveBackground` / `ResolveBorder` was mostly mechanical.

`PlankDockPainter` was the exception and the most worth fixing: its background was two hardcoded
greys with no theme reference at all. It now resolves the top of its gradient and derives the bottom
by darkening, so the 3D effect survives whatever palette it lands in.

**Three styles changed rendering** — `NeonDock`, `NeumorphismDock` and `PlankDock` — and all three
are the intended consequence: they now take their background from the theme where before they took
it from a literal. The other 15 are unchanged.

### Still owed

`DockThemeHelpers.GetDockBorderColor` hardcodes alpha 100 and takes no opacity argument, so
`ResolveBorder` reads `theme.BorderColor` directly rather than going through it. Giving it the same
custom-colour and opacity parameters its background sibling already has is a small change that keeps
being deferred because it restyles borders.

`ResolveAccentColor` has a related wrinkle: it forwards `config.IndicatorColor`, which is
non-nullable with a default, so a caller can never express "no opinion" and the style's own accent
can never win. Making it nullable-backed against the style table — the shape
[03](03-config-consolidation.md) used for the dimensions — is the fix.
