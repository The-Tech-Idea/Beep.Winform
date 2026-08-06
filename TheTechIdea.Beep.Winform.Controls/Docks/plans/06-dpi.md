# Stage 06 — DPI scaling reaches 2 painters of 19

**Kind:** enhancement. Correct at 100%; progressively wrong above it.
**Status:** ☑ done. 18/18 styles scale at 150% and 200%; 100% geometry unchanged. The premise was
wrong in an instructive way — see *Outcome*.

## What the survey found

`DockPainterBase` offers scaled metrics:

```csharp
protected DockPainterMetrics GetScaledMetrics(DockConfig config, IBeepTheme theme, Graphics g, bool useThemeColors = true)  // :31
```

Two concrete painters call it: `AppleDockPainter` and `Material3DockPainter`. That is the whole
population. The other 17 fall into three groups:

| group | painters | what they use |
|---|---|---|
| scaled metrics | Apple, Material3 | `GetScaledMetrics` — correct |
| unscaled metrics | GNOME, iOS, Nord, Plank, Plasma | `DockPainterMetrics.DefaultFor(…)` directly |
| no metrics at all | the remaining 12 | `config` values and literals |

The middle group has a second problem. Each hardcodes its own style rather than passing
`config.Style`:

```csharp
var metrics = DockPainterMetrics.DefaultFor(DockStyle.GNOMEDock, theme, useThemeColors);  // GNOMEDockPainter.cs:322
```

`iOSDockPainter.cs:295`, `NordDockPainter.cs:308`, `PlankDockPainter.cs:295`,
`PlasmaPanelPainter.cs:345` do the same. They bypass `GetMetrics` — so a painter subclass that
overrides `GetMetrics` to customise its style is ignored by its own base's code path. Five painters
opted out of the extension point the base class exists to provide.

The bottom group of 12 reads `config.CornerRadius`, `config.ItemSize` and literals like
`int dotSize = 4` (`DockPainterBase.cs:113`) and `new Pen(color, 1)` (`:92`). At 200% those are
half-size dots and hairline separators next to correctly scaled icons — the mismatched-scale look
that reads as a rendering bug rather than a setting.

`BeepDock` itself is fine: the focus ring, overflow affordance and badge font all scale
(`BeepDock.Drawing.cs:92,97,116`, `BeepDock.Notifications.cs:209`). The control is DPI-correct and
its painters are not, which is why this is invisible in a quick look at the main class.

## Three implementations of one scaling block

The same eleven-line `if (!AreScaleFactorsEqual(…)) { … }` body appears at:

- `DockPainterBase.cs:35-48` — live
- `DockPainterMetrics.cs:349-362` — a `DefaultFor` overload taking `dpiScale`, **no callers**
- `DockPainterMetrics.cs:383-396` — **commented out**, 30 lines, with a note that it waits on
  properties (`CurrentTheme`, `UseThemeColors`) that `BeepDock` has had all along

The commented block violates the standing "no legacy paths, no stubs" constraint outright, and its
premise is false — which is the risk of commented-out code generally: the comment is not checked
against reality by anything.

## The fix

1. `GetScaledMetrics` becomes the only way a painter obtains metrics. `GetMetrics` stays as the
   virtual customisation point; `GetScaledMetrics` calls it and scales the result. It is already
   written this way — the work is making the other 17 painters use it.
2. The five direct `DefaultFor` calls become `GetScaledMetrics(config, theme, g)`. This also fixes
   the hardcoded style: metrics follow `config.Style` like everywhere else.
3. The 12 painters that use no metrics are converted. Where a literal has no metrics field
   (`dotSize`, separator pen width), the field is added to `DockPainterMetrics` — not scaled inline,
   which would be a nineteenth place to get it wrong.
4. `DockPainterMetrics.cs:346-365` (the uncalled overload) and `:367-400` (the commented block) are
   deleted. One scaling implementation, in `DockPainterBase`.
5. Scale is taken from the `Graphics` (`DpiScalingHelper.GetDpiScaleFactor(g)`), which is what the
   live path already does and is correct for a control rendering into a provided surface.

Note the ordering dependency: after [03](03-config-consolidation.md) the painters read dimensions
through metrics rather than `config`, so most of step 3 is already done by stage 03 and this stage
inherits it. Doing 06 before 03 means converting the same 12 painters twice.

## Verification

Rendering the same control at two DPIs and comparing is the only check that can actually fail here;
asserting that a helper was called proves nothing about pixels.

1. **The assertion the stage exists for.** For each of the 19 styles, render at 100% and at 200% and
   assert every measured feature — item size, corner radius, separator width, indicator dot diameter,
   border width — is within 1 px of exactly double. *Today this fails for 17 of 19.* Record the
   per-style pass list before the work; that list is the baseline.
2. **Ratio consistency within one render.** At 200%, assert the indicator dot and the item size scale
   by the *same* factor. This catches the specific defect — some things scaled, some not — that check
   1 could miss if a style scales nothing at all and is uniformly small.
3. **150% too.** Fractional scale is where integer rounding shows up; 100/200 alone can pass a
   `* 2` shortcut.
4. **The extension point works.** Subclass a painter, override `GetMetrics` to return a distinctive
   item size, render, assert the override is visible. *Today it is ignored by all five painters in
   the middle group* — this is the check that proves step 2 landed rather than merely compiling.
5. **Deletion is authoritative.** Delete `DockPainterMetrics.cs:346-400` and build. If it compiles,
   both blocks were dead as claimed.
6. No literal pixel dimension in a painter without a metrics field behind it. Grep for
   `new Pen(…, 1)` and bare integer sizes in `Painters/`; every hit is either fixed or justified in a
   comment.

## Outcome

**The premise above was wrong, and wrong in the direction that matters.** The stage was written as
"DPI reaches 2 painters of 19, convert the other 17". The real defect is that it reached **none of
them at runtime**, and the plan for converting the other 17 would have propagated the broken
mechanism to all 19.

`GetScaledMetrics` took its scale from `DpiScalingHelper.GetDpiScaleFactor(Graphics)`. That helper
documents its own overload:

> **WARNING: Graphics.DpiX can return incorrect values.** Prefer `GetDpiScaleFactor(Control)` when
> available. Only use this overload when Control reference is not available.

Painters get a `Graphics` and no `Control`, so the warned-against overload was the only one reachable.
In a WinForms paint handler `Graphics.DpiX` commonly reports 96 whatever the monitor is doing,
because WinForms scales the control's *bounds* instead. So `AppleDock` and `Material3Dock` were
DPI-aware in an offscreen bitmap and flat on screen.

The baseline check had the same flaw. It simulated DPI with `Bitmap.SetResolution`, which drives
exactly the `Graphics.DpiX` path — so it reported "2 of 18 respond", confirming a mechanism that does
not run in the application. **A harness that renders to bitmaps will validate this bug every time.**

### The fix

`Control.DeviceDpi` is authoritative and updates on `WM_DPICHANGED`. It travels to the painters on
`DockConfig.DpiScale`, the same way `UseThemeColors` does and for the same reason — painters are
shared singletons and cannot hold per-control state.

The scaling happens at **one boundary**: `DockConfig`'s dimension getters return device pixels.

```csharp
public int ItemSize      { get => ScaleUp(ItemSizeLogical); set => _itemSize = value; }
public int ItemSizeLogical => _itemSize ?? DockPainterMetrics.DimensionsFor(Style).ItemSize;
```

That is what makes this stage small instead of a 19-painter rewrite. `DockLayoutHelper` and every
painter already read those properties, so geometry and chrome scale together and **cannot drift
apart** — which is the failure the original plan's per-painter conversion would have risked, since a
painter that scales while the layout does not is worse than one that scales nowhere.

`BeepDock` publishes *logical* values through `ItemSize`, `DockHeight` and `ItemSpacing`, so the
designer round-trips what the user typed rather than what this monitor renders. `SyncDpiScale` runs
from `OnCreateControl` (DeviceDpi is meaningless before there is a handle) and from
`OnDpiChangedAfterParent`.

### Measured

| check | result |
|---|---|
| geometry doubles at 200% | **18/18** styles |
| fractional scale at 150% | 18/18 within 1px |
| 100% geometry vs pre-stage | 0 differences across 18 styles |

`AppleDock` 336×72 → 504×108 → 672×144. Before this stage all three DPIs produced 336×72.

The commented-out DPI block at `DockPainterMetrics.cs:367-400` is deleted, and the "no commented-out
code" ground rule is now locked green.

### Still owed

The uncalled `DefaultFor(style, theme, useThemeColors, dpiScale)` overload survives — it is now the
only remaining duplicate of the scaling block, and it has no callers. It should go, but deleting it
belongs with the `GetScaledMetrics` cleanup rather than here. The standing harness check
*"every painter resolves colour through DockThemeHelpers"* still names eight painters; that is
colour, not DPI, and is unaffected by this stage.
