# Beep ToolTips — Enhancement Program (Master Tracker)

Target: parity with what modern web and commercial desktop tooltip systems actually do —
Floating UI / Popper, Radix, Tippy.js, Material 3, Fluent 2, Ant Design, DevExpress and Telerik.

Written from a full read of `ToolTips/` (~4,500 lines across 30 files), not from the previous
planning documents, which have been deleted. Every claim below cites the code it came from.

## How to read this

One document per feature. Each states the current behaviour with evidence, what the reference
systems do, the work, and how it will be verified. Phases are *not* the organising unit — features
are — but the tracker orders them by dependency: positioning correctness first, because arrow
tracking, auto-update and sizing all build on it.

| # | Feature | Doc | State | Priority |
|---|---------|-----|-------|----------|
| 1 | Anchor rect & placement engine | [01](01-anchor-and-placement.md) | **done** — verified | P0 |
| 2 | Arrow tracking | [02](02-arrow-tracking.md) | **done** — verified; clamp/hide still open | P0 |
| 3 | Auto-update on anchor movement | [03](03-auto-update.md) | **done** — verified | P0 |
| 4 | Interactive / hoverable tooltips | [04](04-interactive-hover.md) | **hover + triggers done**; link clicks & safe-polygon open | P0 |
| 5 | Dismissal, focus & outside click | [05](05-dismissal-and-focus.md) | **escape + filter hygiene done**; policy/focus-return open | P1 |
| 6 | Delay groups (skip-delay) | [06](06-delay-groups.md) | **done** — verified | P1 |
| 7 | Content pipeline & layout variants | [07](07-content-pipeline.md) | **variants + async done**; link clicks open | P1 |
| 8 | Sizing, overflow & scrolling | [08](08-sizing-and-overflow.md) | **sizing done**; scrolling open | P1 |
| 9 | Accessibility | [09](09-accessibility.md) | **name/description fixed**; UIA announce open | P1 |
| 10 | Pinning | [10](10-pinning.md) | **done** — one owner, verified | P2 |
| 11 | Theming & style parity | [11](11-theming-and-styles.md) | **contrast fixed**; type collapse open | P2 |
| 12 | Lifecycle & performance | [12](12-lifecycle-and-performance.md) | **leak + keys fixed**; pooling open | P2 |
| 13 | Verification harness | [13](13-verification-harness.md) | **built** — 61 checks, all passing | P0 |

## Progress

The harness (`scratchpad/ToolTipProbe`) was built **first**, reproducing the P0 defects before any
fix, so each had a failing check to turn green. It went **5 passed / 15 failed → 19 passed /
3 failed**; the three remaining failures are features not yet built (04 and 07).

### Fixed and verified

**The anchor is now a rectangle.** `ToolTipConfig.AnchorRect` / `AnchorControl` carry the control's
screen rect through positioning. Previously `CustomToolTip` built `new Rectangle(position, new Size(1,1))`
from the control's *centre point*, which had two consequences the harness caught immediately:

- `TopStart`, `Top` and `TopEnd` were computed from that centre, so they were offset by ±width from
  each other rather than aligned to the anchor's edges. Now `410 / 520 / 630` against an anchor
  spanning `410..830` — left edge, centred, right edge.
- **Every placement overlapped the anchor.** "Above the centre point" still covers the top half of
  the control, so a tooltip sat on top of the thing it described. All four placements and all four
  screen corners now pass a no-overlap assertion.

**One positioning implementation.** `CustomToolTip.AdjustPositionForPlacement` and
`ConstrainToScreen` are deleted; `CustomToolTip.Positioning.cs` now holds only a note explaining
why. Everything goes through `ToolTipPositioningHelpers.Resolve`, which runs
`offset → flip → shift → arrow`. The duplicate mattered: the helper validated placements using the
gap alone while the caller applied gap **+ arrow size**, so a placement could be certified as fully
visible and then drawn where it was not.

**Explicit placements flip, they do not scramble.** A requested side now flips only to its opposite
and otherwise shifts along its own edge; the twelve-candidate scorer is reserved for `Auto`.

**The arrow tracks the anchor.** `Resolve` returns the arrow offset, satisfying
`tooltipCentre + arrowOffset == anchorCentre`. Verified at both screen edges and centre — offsets
`+48`, `-43`, `0`, each landing exactly on the anchor centre. All four painters already consumed
`config.ArrowOffset`; nothing had ever computed it.

**Tooltips follow their anchor.** New `ToolTipAutoUpdate` subscribes to the anchor, its scrollable
ancestors and its form, coalesces bursts to ~16ms, skips no-op moves, and hides when the anchor is
disposed, hidden, minimised or scrolled out of an ancestor's client area. Moving the form 250px now
moves the tooltip 250px; it previously moved 0.

### Duplication and redundancy removed

An explicit audit for duplicate code, dead code and parallel controls found:

| Removed | Size | Evidence |
|---|---|---|
| `Helpers/ToolTipAnimator.cs` | 172 lines | **zero references** anywhere in the assembly |
| `CustomToolTip.Positioning.cs` | whole file | reduced to a comment once its maths was deleted; the note lives with the surviving engine |
| `ToolTipPositioningHelpers.AdjustForScreenEdges`, `.DetectCollisions`, `.CalculatePositionWithArrow` | 3 methods | dead after the `Resolve` refactor; the last one's own comment admitted it did not adjust the arrow |
| `ToolTipHelpers.CalculateOptimalPosition`, `.CalculatePositionForPlacement`, `.MeasureContentSize` | 3 methods | a **third** placement implementation, with no callers |

**There were three implementations of tooltip placement**, not two: `ToolTipPositioningHelpers`,
`CustomToolTip.Positioning`, and `ToolTipHelpers`. None agreed with the others. One remains.

**Two easing systems.** `ToolTipAnimator.ApplyEasing` keyed off the `EasingFunction` enum;
`ToolTipAnimationHelpers.GetEasingFunction` keys off `ToolTipAnimation`. The live paint path uses
the latter, so `ToolTipConfig.AnimationEasing` — which is typed as `EasingFunction` — feeds the
system that was deleted. The property is effectively inert; wiring it is part of a future item
rather than something to fake now.

**Two shadow properties.** `ShowShadow` and `EnableShadow` both defaulted to `true` and every
painter tested `ShowShadow || EnableShadow`, so setting either to `false` did nothing unless you
knew to set both. `ShowShadow` is now the single source of truth; `EnableShadow` forwards to it and
is marked `[Obsolete]`.

**Two pinning implementations — resolved by deletion.** `BeepPinnedTooltip` (200 lines, deriving
from `BeepPopover`) had **zero references anywhere in the solution**, exactly like the animator. The
integrated path already owns placement, arrow tracking, auto-update, theming and accessibility; a
standalone control duplicated all of that and received none of this program's fixes. The control is
deleted and `ToolTipConfig.Pinnable` is now implemented for real.

### Triggers and hoverability (feature 04)

**`TriggerMode` was the biggest single gap.** Declared with four values — Hover, Focus, Click,
Manual — and read by nothing, so *every* tooltip was hover-only regardless of configuration.
Keyboard users never saw a tooltip at all. All four modes now subscribe their own events, verified
by inspecting what the manager actually attached:

```
Hover   -> [Enter, Leave]        Click   -> [Click]
Focus   -> [GotFocus, LostFocus] Manual  -> []
```

and every mode detaches cleanly on `RemoveTooltip`, which is asserted too — handler leaks on
reassignment were a real bug here once already.

**`KeyboardTriggerable`** now adds focus triggers *on top of* hover, because a hover-only tooltip is
unreachable from the keyboard — the accessibility half of WCAG 1.4.13.

**`PersistOnHover` is implemented.** After the anchor is left, the pending hide waits out the close
delay and is then cancelled while the pointer is over the tooltip, so a user can move onto it to
read, scroll or click it. `ToolTipInstance.IsPointerOver()` is the hit test.

**`HideDelay` is now read.** It was declared and ignored; the close delay was a hard-coded 200ms.
It doubles as the travel grace period between anchor and tooltip.

A related fix fell out of this: the post-delay guard required the pointer to be over the control,
which is correct for hover and wrong for everything else. A focus-triggered tooltip shows precisely
when the pointer is elsewhere, so honouring `TriggerMode` without fixing that guard would have left
Focus mode silently broken.

### Dismissal and message-filter hygiene (feature 05)

**Escape now dismisses from the trigger.** `CustomToolTip.ProcessCmdKey` only fires when the tooltip
window has focus — which a hover-triggered tooltip never takes — so Escape did nothing in the case
WCAG 1.4.13 "dismissible" is actually about. New `ToolTipEscapeFilter` sees the key wherever focus
is, and deliberately does **not** consume it: Escape may also mean something to the focused control.

**Two real bugs in `OutsideClickMessageFilter`:**

1. **Coordinate-space mismatch.** `WM_LBUTTONDOWN` carries *client* coordinates of the receiving
   window; the code unpacked `LParam` and compared it straight against the popover's *screen*
   rectangle. The "did the click land inside the popover?" test was comparing two different spaces
   and only worked when the clicked window happened to sit near the origin. Now uses
   `Control.MousePosition`, which is already screen space.
2. **It never self-removed.** Its own comment said *"If the target has been disposed … unregister
   and stop filtering"* — but the code only returned `false`. A popover disposed without closing
   left the filter in `Application`'s process-wide list for the life of the process, holding a
   reference to a dead control. It now removes itself, and `BeepPopover` unhooks in `Dispose` as
   well as `OnClosing` (a `Form` disposed directly never raises `Closing`).

Verified: 100 show/hide cycles leak **zero** message filters, and Escape dismisses with focus still
on the anchor.

### Async content (feature 07, partial)

**`LoadPreviewAsync` is implemented.** Documented as showing "a skeleton placeholder until the task
completes" and invoked by nothing. `ToolTipInstance.ResolvePreviewAsync` now runs it fire-and-forget
after the tooltip is visible (awaiting would delay the show by the caller's fetch time), then
**re-measures and repositions** — async content changes the size, so a repaint alone would leave a
skeleton-sized window around a full-size image at a placement resolved for the old size.

**A performance bug fell out of the same code.** `PreviewToolTipPainter` called `Image.FromFile` and
disposed the result **on every paint** — a visible preview tooltip re-read its image from disk on
every repaint. The image is now resolved once into `ToolTipConfig.ResolvedPreviewImage`, owned and
disposed by the instance, and the painter just draws it.

Hiding a tooltip while its load is in flight is handled explicitly: the resolved image is disposed
rather than leaked, and the disposed window is never touched. Both are asserted.

**`StepTitle` was orphaned in both directions** — nothing set it, nothing read it, and
`TourToolTipPainter` renders `Title` as the step heading. It now forwards to `Title` and is
`[Obsolete]`, the same treatment as `EnableShadow`.

### Layout variants (feature 07) — and what wiring them exposed

`LayoutVariant` is now authoritative. `ToolTipSectionPlan.For(config)` gives each variant a stated
contract, and both `PaintContent` and `CalculateSize` consult it — they must agree, or the window is
sized for content it does not draw:

| Variant | Contract | Rendered |
|---|---|---|
| `Simple` | body only; a Title is deliberately ignored | 201×59 |
| `Rich` | title + body | 201×80 |
| `Card` | title + rule + body + footer badges | 201×113 |
| `Shortcut` | one row: label left, key caps inline right | 286×64 |

Previously all four produced identical output, and setting a `Title` silently "upgraded" a Simple
tooltip to a Rich one.

**`ToolTipPainterFactory` had zero call sites.** `CustomToolTip.ApplyConfig` hard-coded
`_painter ??= new BeepStyledToolTipPainter()`, so the factory was never called and
`PreviewToolTipPainter`, `TourToolTipPainter` and `GlassToolTipPainter` — about 700 lines between
them — had **never executed**. That is why Preview, Tour and Glass rendered identically to each
other in the first contact sheet: all three were being drawn by the default painter.

The factory is now wired (an explicitly assigned `Painter` still wins). Running those painters for
the first time immediately showed that two of the three are broken:

| Painter | State when first run | Now |
|---|---|---|
| `TourToolTipPainter` | worked — step badge, title, body, Skip/Done | unchanged |
| `PreviewToolTipPainter` | **threw on every paint**; WinForms drew its red-X error box over the whole tooltip | **fixed** |
| `GlassToolTipPainter` | **rendered solid magenta** with the body text clipped away | **fixed** |

`CustomToolTip.OnPaint` also guards the painter call now: one that throws falls back to the default
painter and logs the cause rather than replacing the tooltip with a red X.

**Preview — `WrapMode.Clamp` on a `LinearGradientBrush`.** GDI+ accepts only the Tile modes there;
Clamp makes the setter throw `ArgumentException("Parameter is not valid")`. One line in
`PaintSkeleton` destroyed every paint. The default (Tile) is correct anyway, because the fill
rectangle and the gradient rectangle are identical so the gradient never repeats.

**Glass — alpha over a colour key.** `CustomToolTip` sets `TransparencyKey = Color.Magenta` and
paints its form background magenta, so those pixels are punched out. The painter filled with an
*alpha* brush over that base, producing a magenta-tinted colour that is not exactly the key — so it
was not punched out and rendered as a solid magenta box. Colour-key transparency and alpha blending
cannot be combined; true per-pixel glass needs a layered window. The frosted look is now composited
against a light base and filled opaquely, so layers drawn on top may still use alpha safely.

**Glass — measured with one font, painted with another.** It draws using the theme's `TitleStyle`
and `BodyStyle` but inherited a `CalculateSize` that measures with the base painter's much smaller
fonts, so the window was sized for one font and filled with another and the body text was clipped
off the bottom. This is the same defect that clipped every label in `BeepTree`. Its `CalculateSize`
override now measures with the fonts it draws with — in **two passes**, because height depends on
the width the text actually wraps at: measuring against the maximum width while painting at the
narrower final width let a line wrap at paint time that had not wrapped at measure time.

### Delay groups (feature 06) and sizing (feature 08)

**Skip-delay.** Sweeping a ten-button toolbar cost ten × `ShowDelay` — five seconds to read ten
labels. A group's first tooltip still waits the full delay; siblings then open immediately for
`SkipDelayWindow` (300ms) after the last one closes. Groups derive from the anchor's parent when
unspecified, so a toolbar, ribbon or grid header behaves correctly with no configuration.

The regression this feature must not introduce is a fast flick across a toolbar popping tooltips.
A group is armed only once a tooltip has genuinely been *visible*, so a cold toolbar still makes
every button wait — asserted directly (`[500,500,500,500,500]ms`).

**Sizing against the resolved side.** `CalculateResponsiveSize` clamped only against 80% of the
whole screen, so a tooltip above an anchor near the top of the display could be sized far taller
than the gap it had to live in. New `AvailableSpaceFor` reports what fits on the side actually
chosen — Floating UI's `size` middleware — and the tooltip clamps to it, re-resolving placement if
the clamp changed its size, since the original placement was computed for the old height.

The default maximum width is now a readable 360px (DPI-scaled) rather than a fraction of the
monitor: 80% of a 4K display is not a tooltip. Verified — long text caps at 360px on a 3440px-wide
screen, short text hugs its content at 150px, and a tooltip requested above a top-edge anchor flips
below rather than escaping the working area.

### Accessibility (feature 09)

**A tooltip described a control by renaming it.** `SetTooltip` wrote `config.Title` into
`control.AccessibleName`, which *replaces* the control's own name — a button labelled "Save" with a
tooltip titled "Save document" began announcing as "Save document". The tooltip describes the
control; it does not rename it, the same distinction as `aria-describedby` versus `aria-label`.
Title and text now go to `AccessibleDescription` only.

**Removing a tooltip destroyed the host's accessibility text.** `RemoveTooltip` ended with
`control.AccessibleDescription = string.Empty`, so a control that had its own description before a
tooltip was attached lost it permanently. The prior name and description are now captured on attach
and restored on detach — verified end to end:

```
before : name='Save' desc='Saves the current document'
during : name='Save' desc='Save document. Writes the file to disk.'
after  : name='Save' desc='Saves the current document'
```

**Two corrections to this document's original audit.** It claimed reduced motion was unimplemented
and that `MinContrastRatio` needed enforcing. Both were wrong: `ToolTipAccessibilityHelpers`
already queries `SPI_GETCLIENTAREAANIMATION` and `ShowAsync`/`HideAsync` consult it, and
`CustomToolTip.EnforceContrastIfNeeded` already adjusts the foreground to meet the configured
ratio. Reduced motion is now asserted rather than assumed.

Still open here: a UIA notification so screen readers announce a tooltip when it appears, and a
high-contrast render pass across the painters.

### Theming (feature 11)

**Contrast "enforcement" could not enforce anything.** `AdjustForContrast` computed a target
luminance and handed it to `AdjustLuminance`, which multiplies the colour by 1.2 or 0.8 **once** and
returns. A single 20% nudge cannot rescue a near-black foreground on a near-black background —
`0 x 1.2` is still `0`. Under `MaterialYouTheme` (`#080808` text on a `#101010` surface) the result
was `#090909`, and **all 21 tooltip types measured below 4.5:1 after "enforcement"**.

The nudge is kept for colours that only just miss, so they retain their hue; when it fails, the
fallback is now `ColorUtils.EnsureReadable` — the same helper the grid uses for this exact problem,
rather than a second implementation. Every type on every tested theme now meets 4.5:1.

**Measuring the right stage mattered.** The first version of this check read the raw theme
resolution and reported 21 failures under two more themes — failures the product already fixed at
paint time. Measuring pre-enforcement colours would have been a false alarm; the check now runs the
same two stages the tooltip does.

**14 of 21 `ToolTipType` values share one colour pair** — `Default`, `Help`, `Validation`,
`Interactive`, `Descriptive`, `Notification`, `Tutorial`, `Shortcut`, `Badge`, `Preview`,
`ContextMenu`, `Status`, `Hint` and `Custom` are indistinguishable, because the resolver only
branches on seven. `Accent` and `Info` are also identical in every theme (both map to
`theme.AccentColor`), and under `MaterialYouTheme` `Primary` joins them.

The semantic set that matters most — Success / Warning / Error / Info / Default — *is* distinct on
every theme, so this is a "the enum promises more than it delivers" problem rather than a broken
one. Left open: either give the remaining types their own tokens or reduce the enum to what it
actually expresses.

### Pinning (feature 10)

`Pinnable` was the last property declared and never read. It is now implemented in the main
pipeline:

- The painter draws a pin toggle in the header — outline when unpinned, filled when pinned — so the
  state is legible without a tooltip on a tooltip.
- `ToolTipHeaderButtons` computes the pin and close rectangles **once**, and both the painter and
  `CustomToolTip`'s hit-test read from it. Computing them independently is how a control ends up
  drawing a button somewhere the click handler is not looking.
- A pinned tooltip is exempt from the `Duration` timer and from hide-on-leave: pinning means "keep
  this until I dismiss it", so a timer closing it would defeat the feature.

Verified end to end: the pin rect lands inside the tooltip, clicking it sets `IsPinned`, and a
tooltip with a 250ms `Duration` is still on screen 600ms after being pinned.

### Lifecycle (feature 12)

**Measured before changing anything, and one of the plan's assumptions was wrong.** Tooltip windows
do *not* accumulate — 40 show/hide cycles left zero live `CustomToolTip` windows, so the
create-a-window-per-show behaviour is a performance cost, not a leak. Pooling is still worth doing,
but it is an optimisation rather than the bug the plan implied.

**The real leak was the anchor map.** `_controlTooltips` and `_attachedHandlers` are keyed by
`Control`, and nothing released them when a control was disposed without `RemoveTooltip` — which is
the normal case when a form closes. Measured: **20 disposed anchors retained** after 20
create-and-dispose cycles. The manager now subscribes to `Control.Disposed` and releases itself;
the same cycle now retains none.

**Identity keys.** The tooltip key was `$"control_{control.GetHashCode()}_{DateTime.Now.Ticks}"`.
`GetHashCode` is not an identity — two live controls can share one, and it is not stable for a given
control — so the ticks suffix was papering over collisions, leaving a key that was neither
unique-by-construction nor stable. It is a GUID now; lookups by control go through `_controlTooltips`
regardless. The delay-group key uses `RuntimeHelpers.GetHashCode` so a container that overrides
`GetHashCode` by value cannot merge two unrelated toolbars into one delay group.

### Config surface

**Every `ToolTipConfig` property is now read by something — down from six that were not.**
`HideDelay`, `TriggerMode`, `PersistOnHover`, `LoadPreviewAsync`, `StepTitle` and `Pinnable` were all
declared, documented, and consumed by nothing. The reflection check that found them (four more than
the manual audit did) now passes clean, so a seventh cannot be added silently.

**`ToolTipLayoutVariant` has 7 values; the painter factory maps 3.** `Simple`, `Rich`, `Card` and
`Shortcut` all resolve to `BeepStyledToolTipPainter`, and that painter (731 lines) never reads
`LayoutVariant` — layout is implicitly driven by which fields happen to be populated.

## Standing rules for this area

1. **Anchor to rectangles, never to points.** A point cannot express alignment or edge avoidance.
2. **Middleware order is flip → shift → arrow → size → hide**, as in Floating UI. Scoring all twelve
   placements at once, as the current code does, cannot express "keep the requested side unless it
   genuinely does not fit".
3. **One positioning implementation.** If a second appears, delete it — see the BeepTree program for
   how expensive two engines get.
4. **A declared config property must be read by something.** Three currently are not; a test should
   fail when a fourth is added.
5. **Verify by render.** Screenshot the tooltip against each edge of each monitor, not just assert
   rectangles.
