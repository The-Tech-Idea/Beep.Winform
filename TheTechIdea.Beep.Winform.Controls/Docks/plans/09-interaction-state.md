# Stage 09 — eight interaction states, one resolver, three painters that use it

**Kind:** enhancement. Pressing an item, focusing an item and disabling an item look like nothing.
**Status:** ☑ done. **75 state collisions → 0.** The last one turned out to be a graphics-state bug
in three painters, not a state bug. See *Outcome*.

## The contract exists

`DockItemState` carries seven flags (`DockConfig.cs:87-93`). `DockEnums.cs:340` declares the
canonical eight-value `DockInteractionState`. `DockPainterBase.GetInteractionState` (`:257-272`)
resolves flags to state in a fixed precedence — dragging, disabled, pressed, focused, hovered,
selected, running, normal. `BeepDock.InteractionState.cs` keeps the flags correct across mouse,
keyboard and drag. The plumbing is complete and correct.

## Almost nothing consumes it

`GetInteractionState` has one caller outside its own file:

```csharp
if (interactionState == DockInteractionState.Hovered ||
    interactionState == DockInteractionState.Selected ||
    interactionState == DockInteractionState.Focused ||
    interactionState == DockInteractionState.Pressed)   // ClassicTaskbarDockPainter.cs:72-75
```

— which collapses four of the eight states into one branch. So in the three painters that inherit it
(`Cyberpunk`, `Dracula`, `Terminal`), hovered, selected, focused and pressed render **identically**.

The other 16 painters ignore the resolver and read flags directly. Counting reads across all painters:

| flag | reads |
|---|---|
| `IsSelected` | 51 |
| `IsHovered` | 18 |
| `IsRunning` | 17 |
| `IsDragging` | 3 |
| `IsFocused` | 2 |
| `IsDisabled` | 2 |
| `IsPressed` | 1 |

`IsPressed`'s single read is the resolver itself. `IsFocused` and `IsDisabled` have one real read
each. Three of the eight states have no renderer in any of the 19 painters.

What a user sees today: press an item — nothing moves except the scale change
`DockAnimationHelper.ApplySpringEffect` applies (`:38-42`). Disable an item — it dims via
`UpdateAnimations`' opacity rule (`:105`) but is otherwise indistinguishable, and it still looks
clickable. Focus an item by keyboard — the only feedback is the dashed ring `BeepDock` draws itself
(`BeepDock.Drawing.cs:54-57`), identical in all 19 styles. The states are tracked accurately and
thrown away at the paint boundary.

## Measured, not estimated

The probe renders one item in each of the eight states, for all 18 registered styles, and hashes the
pixels. Result: **75 collisions; no style renders more than 5 of its 8 states distinctly.**

| collapse | styles affected |
|---|---|
| `Normal` = `Pressed` = `Focused` | **18 of 18** |
| `Normal` = `Disabled` | **18 of 18** |
| `Normal` = `Dragging` | 17 of 18 (iOS is the exception) |
| `Normal` = `Hovered` | Minimal, Arc |
| `Hovered` = `Pressed` = `Focused` | Cyberpunk, Terminal, Dracula — the `ClassicTaskbarDockPainter.cs:72-75` branch, exactly as predicted |

Best case is `iOSDock` at 5 distinct states of 8. Worst is `AppleDock`, `MinimalDock`, `PillDock`
and `ArcDock` at 3. `Normal = Pressed = Focused = Disabled` holding across every single style is the
headline: three of the eight states have no renderer anywhere, and the read counts above say why.

Per-style detail is in the harness baseline at `out/baseline/state-collisions.txt`.

## The other axis: 18 styles against each other

The count above asks whether *one* style renders its eight states differently. The complementary
question — whether the 18 styles render differently from *each other* — is what "why have 18 styles"
depends on, and it has a different answer depending on state:

| render | distinct |
|---|---|
| whole dock | **18 / 18** |
| item, `Hovered` | **18 / 18** |
| item, `Selected` | **18 / 18** |
| item, `Normal` | 12 / 18 |
| item, `Disabled` | 12 / 18 |

Two groups collide, and they cross painter families rather than following inheritance:

- `Windows11Dock` = `TerminalDock` = `ArcDock`
- `Material3Dock` = `GlassmorphismDock` = `NeumorphismDock` = `PillDock` = `NeonDock`

The cause is not shared code — these have three different base classes. It is that in the passive
states those eight painters draw **no item chrome at all**, so the item reduces to the icon, and one
icon looks like another. An idle Material 3 launcher and an idle Neon launcher are the same pixels.
The styles differ in their background and in what they do on hover and selection; at rest, eight of
them do not differ.

That is arguably intended minimalism rather than a defect, which is why it is recorded as a measured
fact rather than folded into the collision count. What it does mean concretely: the fix in this stage
must add passive-state chrome, or accept that `Normal` and `Disabled` will stay at 12/18 and say so.

Both these checks live in the harness; `Hovered` and `Selected` are already locked green so a
regression there is caught immediately.

## Five painters are recolours, not renderers

Worth stating alongside, because it changes what "19 styles" means when reviewing render output:

| painter | base | lines | overrides |
|---|---|---|---|
| `ArcDockPainter` | `MinimalDockPainter` | 16 | `PaintDockBackground` only |
| `BubbleDockPainter` | `FloatingDockPainter` | 16 | `PaintDockBackground` only |
| `CyberpunkDockPainter` | `ClassicTaskbarDockPainter` | 17 | `PaintDockBackground` only |
| `DraculaDockPainter` | `ClassicTaskbarDockPainter` | 16 | `PaintDockBackground` only |
| `TerminalDockPainter` | `ClassicTaskbarDockPainter` | 16 | `PaintDockBackground` only |

Each sets two colours and an opacity and calls base. Their items and indicators are pixel-identical
to their parent's. That is a legitimate way to build a theme variant — but it means a pairwise
render check will find five expected collisions on item rendering, and those need to be declared up
front rather than discovered and waved through. (These are also the five painters
[01](01-style-switching-is-one-way.md) is about; after stage 01 they become pure reads, which is what
they should have been.)

## The fix

1. Every painter obtains state through `GetInteractionState`. Direct flag reads in painters go —
   one grep in review, the same enforcement shape stage 01 uses.
2. `DockPainterBase` gains a default rendering for each of the eight states — a pressed inset, a
   focus treatment, a disabled desaturation — so that a painter inherits distinct states without
   writing any code, and overrides only where its style genuinely differs. This is the only way 19
   painters get 8 states each without 152 hand-written branches.
3. `ClassicTaskbarDockPainter.cs:72-75` stops collapsing four states into one.
4. Focus is the exception to "the painter decides": `BeepDock.Drawing.cs:54-57` draws the focus ring
   centrally and should keep doing so, because a focus indicator that varies by style is an
   accessibility problem, not a feature. Painters may add to it; they may not replace it.
   [07](07-accessibility.md) depends on this staying consistent.
5. Disabled items must be visibly non-interactive, not merely dimmer. The opacity rule at
   `DockAnimationHelper.cs:105` is an animation concern that currently doubles as the only disabled
   styling — separate the two.

## Verification

This is the check that found seven "distinct" painters producing identical pixels in the
`DisplayContainers` program, and it is the highest-value check in this program too.

1. **Pairwise state distinctness.** For each of the 19 styles, render one item in all eight states
   and assert the eight renders are pairwise distinct. *Today `Pressed`, `Focused` and `Normal` are
   identical in 16 styles, and `Hovered`/`Selected`/`Focused`/`Pressed` are identical in the three
   ClassicTaskbar painters.* Record the current collision count per style before any work — that
   number is the baseline, and it must go to zero.
2. **Declared exceptions only.** The five recolour painters are expected to match their base on item
   and indicator rendering. List those five pairs explicitly in the harness as known-equal. Any
   *other* cross-style collision is a failure. A check that silently tolerates collisions measures
   nothing.
3. **Backgrounds still differ.** For the same five, assert the *background* render differs from the
   base — that is the one thing they override, and if stage 01 breaks it they become exact duplicates
   with nothing to distinguish them.
4. **State survives the resolver's precedence.** An item that is both selected and hovered must
   render as hovered (the resolver's order at `:266-270`). Assert selected+hovered renders equal to
   hovered-only and differ from selected-only. Catches a painter that ORs flags instead of using the
   resolved state.
5. **Disabled is not just dim.** Render a disabled item and a normal item at the same opacity.
   Assert they still differ. Without the opacity clamp, this passes today for the wrong reason.
6. No painter reads `itemState.Is*` directly. Grep, zero hits outside `DockPainterBase`.

## Outcome

**75 state collisions → 1.** No style renders fewer than 7 of its 8 states distinctly, where before
none managed more than 5 and `Normal = Pressed = Focused = Disabled` held in all 18.

### The mechanism: a template, not an overlay

`PaintDockItem` is now sealed on `DockPainterBase`. It calls the style's own
`PaintDockItemCore` — all 15 overriding painters were renamed to it — and then `PaintStateChrome`.
Making every painter remember the states would have been 18 chances to forget; making the base
guarantee it is one.

The first design for this was a single uniform overlay, and that was **wrong for the reason this
stage exists**: it would have given all 18 docks the same pressed and focused look, trading
per-state uniformity for per-style uniformity. Instead the chrome takes every colour from the theme
resolvers (`ResolveSelectedColor`, `ResolveAccentColor`, `DockThemeHelpers.GetDockBackgroundColor`)
and every dimension from the style's own metrics, so a Terminal item (4px corners, terminal palette)
and a Pill item (28px corners, surface palette) get visibly different chrome from the same code. Any
painter can override `PaintStateChrome` outright.

`DockPainterBase` gained `ResolveHoverColor`, `ResolveSelectedColor`, `ResolveAccentColor` and
`ResolveForegroundColor`, all routed through `DockThemeHelpers` with the named-palette exception from
[01](01-style-switching-is-one-way.md). Hover, selection and running are deliberately left alone —
chrome there would paint over the very thing that makes a style look like itself.

### Two real defects surfaced by the measurement

- **`AppleDockPainter` drew its indicator dot unconditionally.** Every item showed a "running" dot
  whether or not it was running, and `config.ShowRunningIndicator` was ignored entirely — which is
  precisely why `Normal == Running` for that style. Now gated on running-or-selected.
- **`MinimalDockPainter`'s hover was an icon opacity change only**, which neither the measurement nor
  a user could distinguish from Normal. It now draws a hairline underline in the theme's hover
  colour — the smallest mark that still reads as feedback, which is what the style is for. `ArcDock`
  inherits it.

### The last collision was a graphics-state bug in three painters

`AppleDock` reported `Normal == Selected`, byte-identical, while `Running` rendered correctly through
the same `PaintDotIndicator` call. Isolating `PaintIndicator` on its own canvas showed it drawing
**more** ink for selected than for running — so the indicator was right and something before it was
wrong.

`AppleDockPainter.PaintReflection` ended with:

```csharp
g.ResetTransform();
g.ResetClip();
```

**Reset, not restore.** `ResetTransform` sets the transform to identity and `ResetClip` drops the
clip entirely, rather than putting back whatever the caller had. The reflection only runs when an
item is hovered or selected — so a selected Apple item silently destroyed the graphics state for
everything painted after it: its own indicator, its badge, its progress ring, and every later item in
the same pass.

`iOSDockPainter` and `Windows11DockPainter` had the same `SetClip` / `ResetClip` shape. All three now
use `g.Save()` / `g.Restore()` with `CombineMode.Intersect`, so a painter narrows the caller's clip
instead of replacing it.

This was invisible in the control only because `BeepDock` happens to paint in untransformed client
coordinates today. Any composed surface — a print preview, a scaled render, a parent that transformed
the Graphics, or a harness rendering into a translated bitmap — hits it immediately. The measurement
found a real defect that no amount of reading the state code would have surfaced, because the state
handling was never the problem.

**Result: 0 state collisions across all 18 styles.** Every style renders all eight states distinctly,
and the check is locked green.

### A probe change worth knowing about

Indicators are drawn *below* the item box (Apple at `Bottom + 8`, Floating at `Bottom + 12`), and the
render crop was exactly `Padding` — so the dots were clipped and the indicator states compared equal
for the wrong reason. The crop was widened by 12px, which then dropped every style's ink fraction
below the 5% "did anything draw" threshold; that threshold moved to 2% to track the canvas it
measures. Both are the same lesson this harness keeps teaching: a check has to be read together with
what it is measuring.

### Cross-style, after the change

`Hovered` and `Selected` remain 18/18 distinct and are locked. `Disabled` improved from 12/18 to
15/18. `Normal` moved from 13/18 to 12/18 — and that is a *correct* change: removing Apple's spurious
always-on dot made its idle item look like the other idle items, which is the honest state of the
design. Eight painters still draw no item chrome at rest, so at idle they reduce to the bare icon.
Whether that is minimalism or a gap is a design decision, not a defect the chrome should paper over.
