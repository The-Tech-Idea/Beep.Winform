# 07 — Overflow & Header Actions

## Done

**Three of the five policies were fiction.** `ScrollButtons`, `ShrinkToFit` and `Multiline` had
**zero references** anywhere in the assembly. Selecting one behaved like `None` except worse: tabs
were dropped from the run *and* no overflow menu appeared, because the menu is gated on
`OverflowMenu` — so those tabs became unreachable. The designer offered all five in the property
grid as though they worked. The enum now declares only `None` and `OverflowMenu`.

**The default was `None`,** i.e. no defined behaviour the first time tabs did not fit. It is now
`OverflowMenu`, so the out-of-the-box control degrades sensibly.

**The selected tab could be pushed into overflow.** The run was filled left-to-right until a tab did
not fit, so with twelve tabs in a narrow header and the tenth selected, clicking that tab made it
vanish from the strip. Pinning gave a tab no protection either, which is most of what pinning is
for. Space is now claimed in priority order — pinned, then selected, then positional — while tabs
still *render* in positional order. The loop also no longer stops at the first tab that does not
fit: a narrower tab later in the order can still claim space, where previously one wide tab hid
everything behind it.

Verified with 12 tabs in a 420px header, tab 10 selected and tab 11 pinned:

```
visible : 0,10,11
overflow: 1,2,3,4,5,6,7,8,9
```

Six assertions: something actually overflows, the selected tab survives, the pinned tab survives,
every tab appears exactly once across both lists, the visible run stays in positional order, and the
enum offers only what it implements.

**The overflow menu is now MRU-ordered.** Positional order is the wrong ordering for this list: the
tabs in it are exactly the ones that did not fit, so the one the user wants is far likelier to be the
one they were last in than the one that happens to sit leftmost. `BeepTabWorkspaceMruTracker` already
tracked recency and nothing here consulted it. Verified by visiting tabs 3 then 5 then settling on 0,
and asserting the menu leads with 5: `5,3,4,6,7,8,9,10,11`. Navigation mode keeps positional order,
since MRU is a document-mode capability.

**Three unreachable header actions removed.** `BeepTabHeaderActionKind` declared `AddTab`,
`ScrollBackward` and `ScrollForward`, and `BeepTabHeaderActionRouter` dispatched all three — but no
action slot was ever created with any of them, so none could fire, and all three handlers were
`return false`. `ScrollBackward`/`ScrollForward` were remnants of the `ScrollButtons` policy already
removed for never having been implemented. The enum values, the router branches and the stubs are all
gone.

**Remaining:** decide whether the visible run should be a contiguous window around the selection
rather than showing `0, 10, 11` with a positional gap — reference products keep a contiguous window.

## Original findings


**Priority P1.**

## Current behaviour

The machinery exists:

- `Helpers/BeepTabOverflowCoordinator.cs` — consumes `owner.GetDesiredHeaderTabSizes(graphics)`
- `Models/BeepTabOverflowPolicy.cs`
- `Hosts/BeepTabHeaderHost.Overflow.cs`
- `Models/BeepTabHeaderAction.cs`, `Hosts/BeepTabHeaderHost.Actions.cs` (206 lines),
  `Helpers/BeepTabHeaderActionRouter.cs`
- `BeepTabs.Appearance` exposes `HeaderOverflowPolicy`, defaulting to **`BeepTabOverflowPolicy.None`**

Two things stand out.

**The default is `None`.** Out of the box, a tab strip with more tabs than fit has no defined
behaviour — no scroll, no dropdown, no shrink. Every reference product picks a real default:
VS Code scrolls, Visual Studio shows an overflow chevron with a document list, DevExpress scrolls
with optional buttons. A default of "None" means the first time a user opens enough tabs, the
control does something unspecified.

**Overflow depends on the measure path.** `BeepTabOverflowCoordinator` calls back into
`owner.GetDesiredHeaderTabSizes(graphics)` rather than reading the snapshot, so overflow decisions
are computed from a different source than the host paints from — the seam described in
[02](02-measure-render-pipeline.md). If those two ever disagree, overflow will hide or reveal the
wrong tabs.

## What the reference products do

| Product | Overflow |
|---|---|
| VS Code | horizontal scroll, plus a chevron listing all tabs, plus Ctrl+Tab MRU switcher |
| Visual Studio | scroll buttons + document dropdown ordered by MRU, pinned tabs always visible |
| DevExpress | scroll / dropdown / shrink-to-fit as an explicit policy |
| Chrome-style | tabs shrink to a minimum, then scroll |

The common rule: pinned tabs stay visible, the selected tab is always scrolled into view, and the
overflow list is ordered usefully (MRU rather than positional).

## Work

1. **Choose a real default policy.** `None` is not a behaviour. Scroll-with-overflow-list matches
   both the product shells this control targets and the MRU machinery already present.
2. **Feed overflow from the snapshot**, not from a second call into the owner
   (see [02](02-measure-render-pipeline.md)).
3. **Guarantee the selected tab is visible.** Selecting a tab that is scrolled out must scroll it
   into view — including selection driven by Ctrl+Tab, reopen-closed and programmatic selection.
4. **Pinned tabs are exempt from overflow.** `TabIsPinned` exists on the page; overflow must honour
   it or pinning does not mean anything.
5. **Order the overflow list by MRU**, reusing `BeepTabWorkspaceMruTracker` rather than adding a
   second ordering.
6. **Header actions must not steal overflow space silently** — reserve their width in the measure
   pass, the same rule that `ToolTips` needed for its trailing badge.

## Verification

- Probe: with 40 tabs in a 600px header, assert every tab is reachable, the selected tab is visible,
  and pinned tabs are always drawn.
- Probe: select a tab that is scrolled out of view and assert it becomes visible.
- Probe: assert the overflow list order equals the MRU order.
- Probe: assert the sum of laid-out tab widths plus header actions never exceeds the header bounds.
