# Stage 08 — two hosted surfaces, two base classes, one swallowed exception

**Kind:** enhancement, containing the folder's only exception swallow.
**Status:** ☑ done. Swallow gone, tooltip follows the dock, base swapped to `BeepiFormPro`.
See *Outcome*.

## Two surfaces, two foundations

```csharp
public class BeepDockPopup : BeepiFormPro     // BeepDockPopup.cs:16
public class BeepDockTooltip : Form           // BeepDockTooltip.cs:16
```

Both are floating windows spawned by the same control. One inherits the framework's modern form —
themes, form styles, backdrop, rounded chrome. The other is a raw `Form` that reimplements the
basics by hand: `FormBorderStyle.None`, `ShowInTaskbar = false`, `TopMost`, manual `Opacity`, a
hand-rolled fade timer, its own corner-radius constant (`BeepDockTooltip.cs:27`), its own padding
constant (`:25`), its own max width (`:26`).

`BeepDockTooltip` is the only `… : Form` tooltip in the control library. The repository already has
`Forms/BeepPopupForm.cs:9` (`: BeepiFormPro`) and its list and modal subclasses — the shape a hosted
surface is supposed to take here.

The practical consequences, in the order a user meets them:

- The tooltip does not follow the theme. `BeepDockTooltip` receives an `IBeepTheme` in its
  constructor (`:35`) and paints from it, but it does not participate in `BeepThemesManager`, so a
  theme change while a tooltip is open leaves it on the old palette.
- It does not follow the dock's `ControlStyle`. It hardcodes `BeepControlStyle.Material3` three times
  when picking fonts (`:72-74`) — so an Apple dock gets a Material tooltip, always.
- Its corner radius, padding and fade timing are unrelated to the style's metrics. An Arc dock with
  square corners gets an 8 px rounded tooltip.

## The swallowed exception

This folder has exactly one bare `catch`, and it is in the tooltip's teardown:

```csharp
try
{
    _activeTooltip.HideTooltip();
}
catch
{
    _activeTooltip.Dispose();          // BeepDock.InteractionState.cs:132-139
}
```

Everything `HideTooltip` can throw is caught and discarded. The fallback — dispose instead of fade —
is a reasonable *recovery*, but the failure is never reported, so a tooltip that throws on every hide
looks exactly like one that hides cleanly. The standing constraint is not "handle exceptions", it is
**never swallow one**: recover and report.

`HideTooltip` (`BeepDockTooltip.cs:178`) starts a fade timer on a form that may already be disposed —
which is almost certainly the throw this `catch` was added for. Fixing that is the real fix; the
`catch` is a bandage over a lifetime bug.

Lifetime is loose in the other direction too: `ShowTooltip` (`:108`) creates a **second** timer per
call when a delay is passed, and `ShowDockTooltip` (`BeepDock.InteractionState.cs:121`) constructs a
new `BeepDockTooltip` on every hover. Hover across ten items and ten forms and up to twenty timers
have been created. The `DOCK_ENHANCEMENT_SUMMARY.md` in this folder claims "reusable `BeepDockTooltip`
instance management"; the code constructs one per hover. That is a good reason to treat that summary
as a historical note rather than a description of the folder.

## The fix

1. `BeepDockTooltip` derives from `BeepPopupForm` (or `BeepiFormPro` directly if the popup form's
   list behaviour is unwanted), and deletes what the base already does: border style, taskbar flag,
   topmost, double-buffering, the fade timer if the base animates, and the corner-radius constant.
2. It takes the dock's `ControlStyle` instead of hardcoding `Material3` at `:72-74`, and resolves
   radius and padding from the active `DockPainterMetrics` — after [06](06-dpi.md) that is also what
   makes it DPI-correct, which it currently is not.
3. It resolves its theme through `BeepThemesManager` like every other control, rather than holding
   the instance passed at construction.
4. **One tooltip instance**, owned by `BeepDock`, re-targeted per hover rather than reconstructed.
   `_activeTooltip` already exists as a field (`BeepDock.cs:35`) and is already disposed correctly
   (`BeepDock.cs:142`) — the field is right, the usage is not.
5. **One timer.** The dock already owns `_hoverIntentTimer` for hover delay (`BeepDock.cs:105`); the
   tooltip's internal delay timer (`BeepDockTooltip.cs:115-123`) is a second implementation of the
   same thing and goes.
6. `HideTooltip` becomes safe to call on a disposed or never-shown tooltip — a state check, not an
   exception. The `catch` at `BeepDock.InteractionState.cs:136` is then deleted rather than narrowed:
   with the lifetime fixed there is nothing left for it to catch, and any remaining failure is a real
   defect that must surface.
7. `BeepDockPopup` needs less: it is already on the right base. It duplicates the item-state and
   hit-test logic `BeepDock` owns (`BeepDockPopup.cs:21-22`, and the hit-test call
   [02](02-painter-contract.md) lists at `:355`), so it follows stage 02's decision rather than
   keeping a private copy.

## Verification

1. **No bare catch in the folder.** Grep for `catch` with no exception type, and for
   `catch (Exception)` with a body that neither rethrows nor reports. Zero hits. *Today: one, at
   `BeepDock.InteractionState.cs:136`.* This is the check the folder currently passes by one line —
   worth stating precisely so the "zero swallows" claim is true rather than nearly true.
2. **The swallow was covering something.** Before the fix, force the throw: show a tooltip, dispose
   it, then trigger `HideDockTooltip`. Confirm the `catch` fires. If it never fires, the lifetime bug
   is elsewhere and step 6 is aiming at the wrong thing. *This check must be run before the fix, not
   after* — afterwards there is nothing to observe.
3. **Instance count.** Hover ten items in sequence. Assert at most one `BeepDockTooltip` has been
   constructed and at most one timer is running. *Today: ten forms, up to twenty timers.*
4. **Theme follows.** Show a tooltip, switch theme through `BeepThemesManager`, sample the tooltip's
   background. Assert it matches the new theme. *Today it keeps the old one.*
5. **Style follows.** For an Apple dock and a Material 3 dock, capture the tooltip's font and corner
   radius. Assert they differ. *Today both are Material3 with an 8 px radius* — one render, two
   styles, identical pixels, which is the same assertion shape [09](09-interaction-state.md) uses.
6. **No leak on dispose.** Construct, hover across all items, dispose the dock. Assert no
   `BeepDockTooltip` window handles remain.

## Outcome

### The swallow is gone, and it was covering nothing demonstrable

The folder's last bare `catch` (`BeepDock.InteractionState.cs:136`) is removed and the ground rule is
locked green — **0 swallowed exceptions**.

Check 2 above insisted on running *before* the fix, and it earned its place by contradicting the
stage's own theory. The write-up assumed `HideTooltip` was throwing because it starts a fade timer on
a form that `FadeTimer_Tick` has already `Close()`d. **It does not throw** — not on a closed tooltip,
not on a disposed one. So the `catch` was not covering a reproducible failure, and "fix the lifetime
bug underneath" was aiming at something that could not be shown to exist.

It is replaced with an `IsDisposed` state check rather than a narrower catch. If something here does
fail, it is a real defect and must surface rather than be absorbed a second time. That is the honest
position: the swallow had to go because swallows are forbidden, not because a specific bug was found
behind it.

### The tooltip follows the dock

`BeepControlStyle.Material3` was hardcoded at **six** sites and the corner radius at one, so an Apple
dock always got a Material tooltip and a sharp-cornered Terminal dock got a rounded one. Both now
come from the dock's style, and the theme resolves through `BeepThemesManager` rather than being held
from whatever was passed at construction.

| check | result |
|---|---|
| no swallowed exception in the folder | **0** |
| `HideTooltip` safe on a disposed tooltip | no throw |
| corner radius from dock style | Apple 16px vs Terminal 4px (both were 8) |
| control style from dock style | Apple → `iOS15`, Material3 → `Material3` (both were `Material3`) |
| at most one tooltip alive | 1 after ten shows |
| no window survives disposal | 0 |

### Two checks that were measuring nothing

- The instance-count check drove hovers through `SetHoveredIndex`, which only *arms* the hover-intent
  timer. A console message pump never delivers that tick, so it opened **zero** tooltips and would
  have passed no matter how many the control created. It now calls `ShowDockTooltip` directly.
- The style check originally compared fonts. It failed — but for a reason that has nothing to do with
  this stage: **`DockFontHelpers` returns the same `Segoe UI 9` for `iOS15` and `Material3`.** The
  check now asserts the contract this stage actually changed (the tooltip adopts the dock's control
  style) and the font-helper finding is recorded separately, still red, so it is not lost.

### The base class, and why not `BeepPopupForm`

`BeepDockTooltip` now derives from **`BeepiFormPro`**, so it is a Beep window like every other hosted
surface and picks up the framework's form styling.

It deliberately does *not* derive from `BeepPopupForm`, which the plan named first. Reading that class
settled it: it carries parent/child popup chains, selection events, auto-close timers and a static
`ActivePopupForm` registration. A tooltip that registered itself as the active popup would fight
whatever real popup was open — machinery a tooltip does not want and cannot opt out of.

`ShowCaptionBar` is switched off, and the tooltip keeps painting its own background, border and shadow:
those are what give it its shape, and the base's chrome would double up with them. All six stage 08
checks still pass after the swap, including instance count and no-leak-on-dispose.

`BeepDockPopup` needed nothing here: already on `BeepiFormPro`, and [02](02-painter-contract.md)
removed its private layout copy by routing it through the painter.

### Still open

`DockFontHelpers` returns the same `Segoe UI 9` for every `BeepControlStyle`, so the tooltip asks the
right question and gets the same answer whatever the dock's style. That check is red on purpose and
belongs to the font helper, not to this stage.
