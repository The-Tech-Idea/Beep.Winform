# Stage 05 — properties that name a capability with no mechanism

**Kind:** enhancement. Nothing crashes; the control's published API describes a dock it is not.
**Status:** ☑ done. All six surfaces implemented rather than deleted — eleven checks green. See *Outcome*.

## The group

Six published or configurable surfaces have no implementation behind them. They are one stage
because they are one mistake repeated, and because fixing them separately would produce six
inconsistent answers to "what does an unimplemented property do?"

| surface | declared | consumers |
|---|---|---|
| `DockStyle.Custom = 99` | `DockEnums.cs:104` | falls back to `AppleDock` |
| `DockIconMode` (4 values) | `DockConfig.cs:44`, published `BeepDock.Properties.cs:209` | **none** |
| `DockBlurIntensity` (5 values) | `DockConfig.cs:41` | **none** |
| `AutoHide` / `AutoHideDelay` | `DockConfig.cs:62-63`, published `:358` | **none** |
| `HoverOffset` | `DockConfig.cs:30` | one, in dead code |
| `ShowGlow` | `DockConfig.cs:37` | **none** |
| `DockAnimationStyle.Rotate` | `DockEnums.cs:243` | no rotation mechanism — see below |
| `DockAnimationStyle.Pulse` | `DockEnums.cs:238` | no pulsing mechanism — see below |

### `Rotate` and `Pulse` (handed over from [04](04-animation.md))

Stage 04 wired the easing library in and got six distinct curves from nine enum values. The three-way
shortfall was not a wiring failure: `None` was a genuine defect and was fixed there, but `Rotate` and
`Pulse` name **effects**, not easing shapes, and neither has a mechanism.

`DockItemState.CurrentRotation` exists (`DockConfig.cs:230`) and is written exactly once — to zero,
in `BeepDock.Notifications.cs:191`. **No painter reads it.** So `Rotate` cannot rotate anything no
matter which curve it selects, and mapping it to a distinct curve would only have made stage 04's
check go green while the dock carried on not rotating.

They need the same decision as the rest of this stage: implement the effect, or delete the enum value.
Implementing means `PaintDockItem` applying `CurrentRotation` via a transform, and the animator
driving it — which is a real feature, not a curve swap.

### `Custom` is silently `AppleDock`

```csharp
if (_painters.TryGetValue(style, out var painter)) return painter;
return _painters[DockStyle.AppleDock];      // DockPainterFactory.cs:59-60
```

`Custom` is not in the dictionary (`DockPainterFactory.cs:17-44`), so it gets Apple's painter.
`DockPainterMetrics.DefaultFor` sends it down `default:` to Apple's metrics
(`DockPainterMetrics.cs:297-299`). `DockStyleHelpers` sends it down `_ =>` to a third set of numbers.
Its doc comment says *"Custom style using DockConfig properties"* (`DockEnums.cs:102`) — which is
precisely what does not happen. A user who selects `Custom` and sets every `DockConfig` property they
can find gets an Apple dock with some of their values ignored, and no error anywhere.

That same fallback silently absorbs a genuine bug class: any style added to the enum and forgotten in
the factory renders as Apple. There is no way to tell a deliberate fallback from an omission.

### `IconMode` renders nothing

`DockIconMode` offers `IconOnly`, `IconWithLabel`, `IconWithHoverLabel`, `DetailedIcon`
(`DockEnums.cs:249-270`). No painter reads `config.IconMode`. Every style draws an icon and no label,
in all four modes. `IconOnly` is not the default that happens to be right — it is the only behaviour
that exists.

### `AutoHide`, `BlurIntensity`, `ShowGlow`

`AutoHide` and `AutoHideDelay` have no timer, no visibility logic, no hook. `BlurIntensity` is not
even published — it exists only to be assigned in `DockConfig`. `ShowGlow` on `DockConfig` has zero
readers; note that `DockPainterMetrics.ShowGlow` (`:69`) is a *different* field, set per style and
read by the painters that use metrics. Two identically-named flags, one live, one dead, is the
[03](03-config-consolidation.md) shape again.

### `HoverOffset`

Its only reader is `DockPainterBase.CalculateItemBounds:167`, which [02](02-painter-contract.md)
establishes nothing calls. It is dead today and becomes live the moment stage 02 lands — so it needs
no work here, but it does need a test, or stage 02 will silently start displacing hovered items by
20 px and that will read as a regression.

## The fix

The rule that settles all six: **a published property either does something or does not exist.**

1. **`Custom` gets a mechanism.** A `CustomDockPainter` registered in the factory that resolves every
   visual decision from `DockConfig` and the stage 01 resolvers — no per-style table, by design. That
   is what the doc comment already promises, and after [03](03-config-consolidation.md) the config is
   finally a trustworthy source for it.
2. **The factory stops guessing.** `GetPainter` throws for an unregistered style rather than
   returning Apple. A missing registration is a bug and must present as one. The control constructs
   with a valid style, so this is not reachable from a default path.
3. **`IconMode` is implemented in `DockPainterBase`**, once, using the measured-then-drawn split the
   rest of the codebase uses — item bounds must account for the label or it will be clipped, which
   makes this a [02](02-painter-contract.md) dependency, not just a paint change. Styles that must
   differ override; the other 18 inherit.
4. **`AutoHide` is implemented or deleted.** It is a genuine dock feature and a real amount of work
   (pointer proximity, a reveal animation, focus interaction). If it is not being built now, the
   property and its delay go, and the enum value is not left behind as a promise. Same test for
   `BlurIntensity` — glassmorphism already blurs without it.
5. **`DockConfig.ShowGlow` is deleted.** The metrics flag is the live one.

## Verification

1. **`Custom` is not Apple.** Set `DockStyleType = Custom`, set `CornerRadius = 0`,
   `BackgroundColor = Red`, paint, sample. Assert red with square corners. *Today: Apple's
   translucent grey with 16 px corners* — the check fails loudly before the work and is the whole
   point of it.
2. **No silent fallback.** Ask the factory for a `DockStyle` value cast from an unregistered number.
   Assert it throws. *Today it returns Apple's painter.*
3. **`IconMode` changes pixels.** For each of the four modes, render the same item and assert the
   four renders are pairwise distinct, and that measured item bounds grow when a label is shown.
   *Today all four are identical* — that count is the baseline.
4. **Label fits.** With `IconWithLabel`, assert the drawn text rectangle is inside the item bounds at
   100%, 150% and 200% DPI. The most likely regression is a label that fits at 100% and clips at 200%,
   which is silent — see [06](06-dpi.md).
5. **`HoverOffset` guard (for stage 02).** Capture hovered-item bounds before stage 02 lands and
   after. If they move by exactly `HoverOffset`, that is the property waking up, not a regression —
   assert it deliberately so nobody has to guess later.
6. **No dead published property.** For every public property on `BeepDock`, there is at least one
   read outside `DockConfig` and the property itself. Mechanical, and it is what would have caught
   all six.

## Outcome

### Done

**`DockStyle.Custom` has a mechanism.** `CustomDockPainter` resolves every visual decision from
`DockConfig` and declares no palette, no opacity and no `IsNamedPalette` — having no opinion is the
point. Setting `BackgroundColor = 12,200,40` and `CornerRadius = 0` now renders green with square
corners; it used to render Apple's translucent grey.

**The factory stops guessing.** `GetPainter` throws `ArgumentOutOfRangeException` for an unregistered
style instead of returning `AppleDockPainter`. That fallback made a missing registration
indistinguishable from a deliberate choice — any style added to the enum and forgotten rendered as
Apple, silently and forever. Every enum value is now registered, so a miss is a bug and presents as
one.

**`IconMode` renders.** `SplitForIconMode` divides the item box between icon and label and
`PaintItemLabel` draws the caption, both in `DockPainterBase`, so 13 of 18 styles inherit it through
the shared `PaintItemIcon` path. The label is measured **out of** the item's own box rather than
drawn beside it, so nothing lands outside the rectangle the layout and hit-testing agreed on — which
is why this needed [02](02-painter-contract.md) first.

**`DockConfig.ShowGlow` and `BlurIntensity` deleted.** Neither had a reader; `ShowGlow` was also a
name collision with the live `DockPainterMetrics.ShowGlow`.

### A check that was asserting a bug

The stage's original check demanded four distinct renders from the four `IconMode` values. That is
wrong: with nothing hovered, `IconWithHoverLabel` is **supposed** to look like `IconOnly`. Demanding
otherwise would have forced a label onto an idle dock to make a number go green. The check now
asserts what the contract actually says — `IconOnly`, `IconWithLabel` and `DetailedIcon` are pairwise
distinct, and `IconWithHoverLabel` matches `IconOnly` when idle and differs when hovered — plus that
the label stays inside the item bounds at 100%, 150% and 200%.

### All four "decide later" surfaces were built, not deleted

They were held back because each was either a real feature or a deletion of published API. Deleting
published properties breaks consumers, and every one of these names a behaviour a dock plausibly
wants, so all four were implemented.

**`AutoHide` / `AutoHideDelay`** - `BeepDock.AutoHide.cs`. The dock retracts to a 4px reveal strip
after `AutoHideDelay` of pointer absence and comes back when the pointer returns. It never retracts
out from under the cursor or mid-drag. `AutoHideDelay` is now published too: it existed only on
`DockConfig`, so even once auto-hide worked the delay could not be set from the designer, and half a
feature is still not a feature.

Two things fought the retraction and both had to be fixed: `UpdateDockSize` reset the height on the
resize that retracting caused, and its `MinimumSize` pinned the dock open. `UpdateDockSize` now
returns early while retracted, and `Retract` releases the minimum first.

**`HoverOffset`** - the layout lifts the hovered item toward the dock's outer edge. It now defaults
to **0** rather than 20: waking a dormant property must not silently move every hovered item in all
18 styles. Set it and the lift happens.

`AppleDockPainter` needed the same change separately, and that is worth noting as a cost of
[02](02-painter-contract.md): a style that owns its geometry owns *all* of it, including the config
properties the shared layout honours. Overriding `CalculateItemBounds` silently dropped `HoverOffset`
until it was added there too.

**`Rotate`** - `DockItemState.CurrentRotation` is now driven by the animator and applied by the
sealed `PaintDockItem` template, so it works in all 18 styles rather than wherever a painter
remembered. Measured: 11.6 degrees under `Rotate`, 0.0 under `Scale`.

**`Pulse`** - a continuous breath around the item's target scale, which is why it could never have
been an easing curve: an ease runs once and stops, a pulse does not. Measured: scale still varying by
0.117 after the ease has completed.

None of the four changed a single corpus render, because each is inert until selected.

### Also closed here

`DockConfig.ShowGlow` and `BlurIntensity` deleted - neither had a reader, and `ShowGlow` collided by
name with the live `DockPainterMetrics.ShowGlow`.
