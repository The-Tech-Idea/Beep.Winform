# BeepDock — enhancement and fix program

Master tracker for `TheTechIdea.Beep.Winform.Controls/Docks/`.
49 C# files, 10,351 lines, 19 concrete painters, 18 registered styles.

## What the survey found

This folder is in better shape than the two programs before it, and it is worth saying where,
because it changes what the stages should spend effort on:

- **One swallowed exception, not forty-two.** The detector that found 42 catch-alls in `GridX` finds
  exactly one here, in the tooltip teardown (`BeepDock.InteractionState.cs:136`). Stage 08 removes it
  by fixing the lifetime bug underneath rather than narrowing the catch.
- **The painters are genuinely wired in.** `DrawContent` calls the active painter for background,
  items, indicators and separators (`BeepDock.Drawing.cs:26-69`). This is not the `GridX` situation
  where 13 painters existed and one hardcoded renderer drew every pixel.
- **No child controls.** Everything is painted, which is the rule this codebase wants.
- **Painters hold no fields**, so the static singleton dictionary in the factory is safe — though
  five of them write into the config they are handed, which is stage 01.
- **`DrawContent` is overridden, not `OnPaint`**, and `ApplyTheme()` is overridden — the
  `BaseControl` contract is respected.
- **Keyboard navigation and interaction state are complete and correct.** Arrows, Home/End, reorder,
  focus tracking, hover intent, drag hysteresis. The flags are all maintained accurately.

The defects are concentrated in four shapes instead:

1. **State that outlives the operation that set it** — painters and setters writing into shared
   config (01, 03).
2. **Two or three implementations of one concept** — per-style defaults, easing, DPI scaling, metrics
   access (03, 04, 06).
3. **Complete code nothing calls** — geometry members, an entire easing library, accessibility
   helpers (02, 04, 07).
4. **A published API wider than the implementation** — nine animation styles with one behaviour, four
   icon modes with one rendering, eight interaction states with three renderers (04, 05, 09).

The last one is the largest by user impact and the least visible in a code read, which is why stage
10's render corpus is the instrument the whole program depends on.

## Severity ordering

Stage 01 is a defect a user hits by changing a property. Stage 03 is the same defect from the other
side and is listed as structural only because its consequence is confusion rather than a wrong pixel.
Stages 02 and 04 are structural: they do not misbehave today, they make the control unable to do
things its own API promises. Stages 05–09 are enhancements. Stage 10 is the standing harness every
other stage reports through.

| # | Stage | Kind | Status |
|---|---|---|---|
| [01](01-style-switching-is-one-way.md) | Painters permanently mutate the control's config | **defect** | ☑ done |
| [02](02-painter-contract.md) | Three interface members implemented, overridden by none, called by nothing | structural | ☑ done |
| [03](03-config-consolidation.md) | Three tables of per-style defaults, and a setter that overwrites the user | structural | ☑ done |
| [04](04-animation.md) | Three copies of an easing library, none reachable; `AnimationStyle` unread | structural | ☑ done |
| [05](05-dead-capability-surface.md) | Six properties that name a capability with no mechanism | enhancement | ☑ done |
| [06](06-dpi.md) | DPI scaling reaches 2 painters of 19 | enhancement | ☑ done |
| [07](07-accessibility.md) | A dock of N launchers is one control to a screen reader | enhancement | ☑ done |
| [08](08-popup-and-tooltip.md) | Two hosted surfaces, two base classes, one swallowed exception | enhancement | ☑ done |
| [09](09-interaction-state.md) | Eight interaction states, one resolver, three painters that use it | enhancement | ☑ done |
| [10](10-verification.md) | A probe harness and the first tests this folder has | verification | ◐ baseline captured |

Status marks: ☐ open · ◐ in progress · ☑ done

## Baseline

`DockProbe` is built and run. Every claim above is now a number rather than a reading, and the
survey survived: **3 passed, 19 failed, 0 unexpected.** The headline measurements —

- 5 of 18 styles mutate the config while painting (stage 01)
- 26 disagreements between the two default tables (stage 03)
- 1 distinct animation curve across 9 published `AnimationStyle` values (stage 04)
- 2 of 18 styles respond to device DPI (stage 06)
- 75 interaction-state collisions; no style renders more than 5 of 8 states distinctly (stage 09)

The accessibility check is the one worth reading twice: the dock reports `-1` accessible children,
**and so does the stock `Panel` control group**. That is the trap [10](10-verification.md) describes,
reproduced rather than argued about — any check that had counted "some children" would have been
measuring the window hierarchy.

The baseline corrected two of its own checks before they could mislead, and stage 01 turned up three
more ways the harness was measuring itself rather than the folder — no icons on the fixture items, no
opacity applied per state, and an afternoon of renders written to a directory named `--nologo`. All
five are recorded in [10](10-verification.md); the fixture is now checked before any render
comparison is trusted.

## Stage 01 outcome

15 mutation sites, 0 remaining. All 18 styles render pixel-identical to the pre-stage baseline, so
nothing changed how anything looks. Two things came out of it that were not in the plan:

- **The theme layer had no callers.** `DockThemeHelpers` (213 lines) documented the priority
  `Custom > Theme > Default` and no painter used it; `UseThemeColors` never reached a painter at all.
  Resolved as *theme-led styles follow the theme, named-palette styles keep their palette*, declared
  per painter via `IsNamedPalette`, with the flag riding on `DockConfig`. Proven under two themes in
  both directions. This covers **7 of 18 styles** — eight painters still open-code colour and are
  tracked by a standing harness check.
- **Cross-style distinctness is not what it looks like.** Whole-dock renders are 18/18 distinct, and
  so are `Hovered` and `Selected` items — but `Normal` and `Disabled` items are only 12/18, because
  eight painters draw no item chrome at rest. Recorded in [09](09-interaction-state.md).

## Stage 03 outcome

There were **four** sources of per-style defaults, not three. The fourth surfaced only under
measurement: `DockConfig.CornerRadius` was a flat `16` for every style, in neither helper table nor
setter, silently overriding the per-style radii the metrics table had always declared.

One `Dictionary<DockStyle, StyleDefaults>` now covers all 18 styles plus `Custom`.
`DockStyleHelpers`' eight switches are deleted. `DockConfig`'s dimensions became nullable-backed with
getters that resolve from that table, so unset values follow the style and set values survive — and
the ~40 existing readers needed no change at all.

The plan's assumption that metrics should win the merge was **wrong**: `DockLayoutHelper` reads
`config`, so the helper numbers are what the control actually lays out with, and taking the metrics
set would have resized 13 of 17 styles. The helper values won.

One intended rendering change, isolated rather than assumed: 42 of 54 corpus rows changed hash while
every dock size and item rectangle stayed identical. A `--pin-legacy-radius` probe switch that
restores the flat 16 brings **all 54 rows back to an exact match** — proving per-style corner radius
is the sole visual difference and everything else is pixel-neutral. Fifteen styles now get the radius
their own row always declared (`Terminal` → 4, `Pill`/`Bubble` → 28, and so on).

## Stage 06 outcome

**The stage's premise was wrong, and so was the baseline that "confirmed" it.** It was written as
"DPI reaches 2 painters of 19". In fact it reached **none of them at runtime**, and the plan to
convert the other 17 would have spread the broken mechanism to all 19.

`GetScaledMetrics` scaled from `DpiScalingHelper.GetDpiScaleFactor(Graphics)` — the overload that
helper explicitly warns against, and the only one a painter could reach, because the painter contract
supplies a `Graphics` and no `Control`. In a WinForms paint handler `Graphics.DpiX` commonly reports
96 whatever the monitor is doing. The baseline check simulated DPI with `Bitmap.SetResolution`, which
drives that same path — so it reported "2 of 18 respond" and confirmed a mechanism that never runs in
the application. **A bitmap-rendering harness will validate this bug every time.**

`Control.DeviceDpi` now travels to the painters on `DockConfig.DpiScale` — the pattern already
approved for `UseThemeColors`, for the same reason. Scaling happens at **one boundary**, `DockConfig`'s
dimension getters, so the layout helper and all 19 painters scale together and cannot drift apart.
Published properties stay logical so the designer round-trips what the user typed.

| check | result |
|---|---|
| geometry doubles at 200% | **18/18** styles |
| fractional scale at 150% | 18/18 within 1px |
| 100% geometry vs pre-stage | 0 differences |

`AppleDock` 336×72 → 504×108 → 672×144, where before all three DPIs gave 336×72.

## Stage 02 outcome

Option A. The deletion test passed twice — the three interface members compile away, and so does
their 110-line implementation in `DockPainterBase`. Dead, confirmed rather than inferred.

The signature mismatches were resolved rather than bridged: `CalculateItemBounds` returns the whole
set (the per-index form was O(n²) per layout pass), `CalculateDockBounds` became
`CalculateDockSize`, and `HitTest` returns the index all four call sites wanted. The base delegates
to the existing helpers, so **the wiring alone changed 0 of 54 corpus rows** — verified before any
style was given geometry, which is what makes the next part attributable.

`AppleDockPainter` then took its own layout: a cosine bulge over four neighbours either side, against
the shared default's linear falloff over two. Corpus: **only `AppleDock` changed**, hovered bounds
`125;-6;84;84` → `183;-6;84;84`. Seventeen styles byte-identical, hit-testing verified across 18
styles × 9 items.

This also closed a harness gap: the corpus built item states from `DockLayoutHelper` directly, so it
reported "0 changed" even after the override landed — it was measuring the path the control no longer
takes. Any style with its own geometry would have been invisible in every captured render.

## Stage 04 outcome

Deletion test passed: `DockEasingHelper.cs` plus the five duplicate methods compile away with 0
errors. (The first attempt produced two CS1022 errors because a regex ate the enclosing braces —
structural errors are not evidence of use, so that run was discarded and redone with brace matching.)

The real work was what the plan predicted: the animation had **no clock**. `UpdateAnimations`
approached the target by a fixed fraction per tick, so there was no `t` to give a curve.
`DockItemState` gained from/to/elapsed, `AnimationSpeed` became `AnimationDuration` in seconds, and
`BeepDock` measures real elapsed time instead of assuming 16 ms.

`DockAnimationStyle.None` was animating — `GetEasingFunction`'s `_ =>` sent it to `EaseOutCubic`. Fixed.

**Two of the nine values turned out to belong to stage 05.** `Rotate` and `Pulse` name *effects*, not
easing shapes: `DockItemState.CurrentRotation` is written once, to zero, and read by no painter.
Giving them invented curves would have turned the check green while `Rotate` still rotated nothing —
so they moved to [05](05-dead-capability-surface.md) and the check here asserts what the stage can
deliver: **6 of 6 curve-named styles now move differently** (was 1 across all 9), `None` does not
move, every curve satisfies `f(0)=0`/`f(1)=1` including the hand-rolled `Spring` default, and
animations now terminate — the old exponential approach never arrived, so the 60 FPS timer never
idled.

## Stage 05 outcome

Implemented: `CustomDockPainter` (the style whose doc comment always promised `DockConfig` drives it,
and which silently rendered as Apple); the factory now **throws** for an unregistered style instead of
falling back to Apple, so a missing registration presents as the bug it is; `IconMode` renders, with
the label measured *out of* the item box so nothing lands outside the rectangle layout and
hit-testing agreed on — which is why it needed stage 02 first. `DockConfig.ShowGlow` and
`BlurIntensity` deleted, neither had a reader.

One check here was **asserting a bug**: it demanded four distinct renders from the four `IconMode`
values, but with nothing hovered `IconWithHoverLabel` is supposed to look like `IconOnly`. Satisfying
it would have meant labelling an idle dock. It now asserts the real contract, plus that labels stay
inside the item bounds at 100/150/200%.

**Four surfaces are left open on purpose** — `AutoHide`/`AutoHideDelay`, `HoverOffset`, and the
`Rotate` and `Pulse` animation styles handed over from stage 04. Each is either a real feature or a
deletion of published API, which is a product decision rather than a refactor. `HoverOffset` is the
sharpest: waking it would lift every hovered item by 20px in all 18 styles.

## Stage 07 outcome

`BeepDock.Accessibility.cs`, modelled on `Docking/BeepDockspace.Accessibility.cs`, so four controls
that paint rather than nest now solve this the same way. The dock publishes **8** children against a
stock `Panel` control group reporting **0** — the trap this program warned about, measured rather
than argued. Bounds come from `_itemStates[i].Bounds`, the same rectangles the painter drew, so the
tree cannot describe geometry that is not on screen; activation routes through the click path;
`Focused`/`Selected`/`Pressed`/`Unavailable` come from flags `DockItemState` already maintained and
no one read. Overflowed items are deliberately not advertised. `UpdateAccessibility` deleted — no
callers, stale count, and a variable assigned but never read.

**Two failures here were the harness, not the tree.** The probe called `CreateControl()` without
`Show()`, so the parent never laid out, the dock kept its default width, and the overflow cut fired
— the tree correctly published 3 of 8, refusing to advertise items that genuinely were off screen.
And a control on an unshown form cannot take focus, so no child could report `Focused`. Separately,
the bounds check printed a hardcoded "8 items" while looping over 3; it now prints what it checked.

## Stage 08 outcome

**The folder now has zero swallowed exceptions** and that ground rule is locked green.

The stage's own theory about the swallow was wrong, and the check that insisted on running *before*
the fix is what caught it. The write-up assumed `HideTooltip` threw because `FadeTimer_Tick` had
already `Close()`d the form. **It does not throw** — not closed, not disposed. So the `catch` was not
covering a reproducible failure, and "fix the lifetime bug underneath" was aiming at something that
could not be shown to exist. It is replaced with an `IsDisposed` check, on the honest grounds that
swallows are forbidden rather than that a specific bug was found behind it.

`BeepControlStyle.Material3` was hardcoded at six sites and the corner radius at one: an Apple dock
always got a Material tooltip, a sharp-cornered Terminal dock a rounded one. Both now follow the dock
(Apple 16px vs Terminal 4px; `iOS15` vs `Material3`), and the theme resolves through
`BeepThemesManager`.

Two more checks were measuring nothing: the instance count drove hovers through `SetHoveredIndex`,
which only *arms* the hover-intent timer that a console pump never delivers — it opened zero tooltips
and would have passed regardless. And the style check compared fonts, failing for an unrelated
reason: **`DockFontHelpers` returns the same `Segoe UI 9` for `iOS15` and `Material3`.** That finding
is now tracked separately, still red, rather than absorbed.

**Deferred:** `BeepDockTooltip` still derives from `Form`. The theming consequences are fixed without
the swap; the swap itself changes chrome, padding and shadow on a control that paints its own
background, border and shadow, and that is a visual change no headless probe can verify.

## Where the program stands

**63 passed, 3 failed, 0 unexpected** — from 3/19 at baseline. **All nine work stages are done**;
stage 10's harness is the standing instrument they all report through.

Every remaining red is a deliberate, documented measurement rather than an unfinished stage:

- `DockFontHelpers` returns the same `Segoe UI 9` for every `BeepControlStyle` — a font-helper
  finding, surfaced by [08](08-popup-and-tooltip.md) but not owned by it.
- Item renders in the two passive states are 12/18 and 15/18 distinct across styles, because eight
  painters draw no item chrome at rest ([09](09-interaction-state.md)). Whether that is minimalism or
  a gap is a design decision, not a defect — and it is measured rather than assumed.

The final round closed the last five items: all four "decide later" capabilities were **implemented
rather than deleted** (`AutoHide`, `HoverOffset`, `Rotate`, `Pulse`), the tooltip moved to
`BeepiFormPro`, high contrast reaches the painters through `Docks.Helpers.HighContrast`,
`GetDockBorderColor` gained the parameters its background sibling had, and `DockConfig.IndicatorColor`
became nullable-backed so a style's own accent can finally win. None of it changed a corpus render.

One harness rule was **narrowed rather than satisfied**. "No painter reads `itemState.Is*` directly"
would have forced every read through `GetInteractionState`, which resolves to a *single* state by
precedence — so an item that is both hovered and selected reports only `Hovered`, and painters would
have stopped drawing a selection ring under a hovered item. That is a real regression in service of a
tidier grep. The rule now reads "no painter treats 3+ interaction states as one branch", which is what
actually produced identical pixels, and `ClassicTaskbarDockPainter`'s four-way collapse was fixed to
satisfy it.

## Order of work

Not the same as the numbering.

1. **[10](10-verification.md) baseline capture runs first**, before stage 01 changes a line. Most
   checks in this program are "did this render change, and was the change intended" — without the
   corpus there is nothing to compare against and the stages become unverifiable.
2. **01 → 03** in that order. Stage 03's `ApplyColorProfile` fix is what stops stage 01's colour
   fallback from being silently defeated; a stage 01 test that passes before 03 can regress after it.
3. **03 → 06.** After 03 the painters read dimensions through metrics rather than `config`, which is
   most of stage 06's conversion work. Doing 06 first means converting the same 12 painters twice.
4. **02 before 05.** `IconMode` needs item bounds that account for a label, and bounds are stage 02.
   Stage 02 also wakes `HoverOffset` — declare that in the harness so it does not read as a
   regression.
5. **09 late.** It touches every painter, so it wants 01, 03 and 06 settled first.

## Standing constraints

Carried from the docking and grid programs, and from `CLAUDE.md`:

- No legacy paths, no stubs, no shims. Production-ready code or nothing.
- No duplication or redundancy — this program exists largely because of it.
- Never swallow an exception. Recover and report; one site to clear (stage 08), then keep it clean.
- Do not modify `BaseControl` or `BeepiForm`. Use them.
- Resolve themes through `BeepThemesManager`.
- No control flow in `InitializeComponent`.
- `master` branch only.

## The rule every stage is verified against

Established across the previous two programs, after eight checks passed or failed for the wrong
reason: **a check must be able to fail for the reason it was written.** Every stage below states the
baseline it measures against, not just the assertion, and names what a failing run prints *today*. A
stage that cannot describe what a failing run would look like is not ready to implement.

Deletion plus a clean compile is authoritative for deadness. Grep is not — `ClassicTaskbarDockPainter`
looked unregistered and dead during this survey, and is in fact the base class of three painters.

## A note on `DOCK_ENHANCEMENT_SUMMARY.md`

The 295-line summary in this folder describes a refresh pass and is a useful record of intent, but it
is not a description of the code. It claims "reusable `BeepDockTooltip` instance management"; the
code constructs a new tooltip form per hover (stage 08). Treat it as history. When this program
completes, it is replaced or deleted rather than appended to.
