# 10 — Theming & Painter Parity

## Contact sheet — BUILT

Seven painters × three themes = 21 renders, written to `scratchpad/contact-sheets/painters-*.png`,
with four assertions: no paint threw, no render is a single flat colour, no two painters produce the
same fingerprint under a theme, and every painter changes when the theme changes. **All pass.**

The order of those assertions is the point. The exception check runs first, because the sheet
initially reported 21 green renders while `CardTabPainter` was throwing on the user's machine —
`DrawToBitmap` swallows paint exceptions, so an aborted paint still yields a plausible bitmap. Under
that broken run the sheet also reported "Underline and Minimal render identically under
MaterialDesignTheme"; once the crash was fixed, all seven render distinctly under all three themes.
A pixel finding taken from a run where paint was throwing is not a finding.

## Live theme switching — verified, no defect

Switching `Theme` on a live control leaves it identical to one built with that theme from the start:
tab widths `104/95/79` either way, **0.0% pixel difference**. `ApplyTheme` re-resolves the font and
the painter's theme and then calls `RefreshHeaderLayoutState`, which re-measures the header and
invalidates.

I initially reported the opposite — that `ApplyTheme` neither invalidated nor refreshed layout — from
a truncated read of the method that cut off the `RefreshHeaderLayoutState()` call on its last line,
and was about to add a redundant `Invalidate()`. The measurement had already shown there was nothing
wrong; reading the whole method before editing is what caught it.

## Header positions — all four now covered

The style sheet rendered Top only. Left/Right and Bottom are now covered too, and Bottom turned up
one defect: **Button clipped its captions**. It draws the button inset inside the tab slot and then
lays the caption out inside the *button*, so the text had four fewer pixels than `MeasureTab` had
reserved — the measure/draw divergence again, in geometry rather than fonts. `ButtonTabPainter` now
overrides `MeasureTab` to add back what it insets. The rule generalises: whatever a painter subtracts
before drawing content, it has to add when measuring.

Nothing else was wrong at Bottom — no painter throws, every style renders a visible tab, and every
style renders its captions.

### Two more measurement mistakes, both mine

**An assertion that failed all seven styles at once.** I asserted the chrome had to touch the top edge
of a bottom-docked strip. Seven simultaneous failures is the signature of a wrong assumption rather
than seven broken painters, and it was: the 44px crop simply leaves whitespace above a ~30px tab.
Looking at the render settled it in seconds.

**A glyph metric blind to light text.** Counting *dark* pixels scored Classic and Card as having no
caption at Bottom — they fill the selected tab and draw the caption in white, so they put down no
dark ink at all, while the render plainly showed the text. The measure is now a colour-agnostic
difference between a captioned render and a caption-free one at identical geometry, shared by the
vertical and bottom checks rather than implemented twice. The vertical check had the same latent
bias and had been passing for the wrong reason — on the one unselected tab in its sample.

## Adornments — three defects found by rendering them

The badge, dirty dot and busy ring had been rewired through the colour seam but never looked at.
A state sheet — plain / dirty / four badge kinds / dot / busy, per theme — found:

**Every adornment was drawn in the same slot.** Badge, dirty dot, busy ring and close button were
all positioned at `Bounds.Right - edgeP - size`, stacked on top of one another.
`MeasureHorizontalAdornmentWidth` had always reserved room for each of them cumulatively, so the tab
was wide enough — the layout simply never used the space. A tab with a badge and a close button
showed one of them. They now lay out right-to-left along a cursor.

**Tabs were measured 6px too narrow.** Edge padding was reserved once but applied at both ends. This
was invisible only because the caption was allowed to run *underneath* the overlapping adornments;
the moment they were separated, a caption the tab had been sized to fit ellipsised. One bug had been
hiding the other.

**The dirty dot and busy ring were drawn blue-on-blue.** Both resolve to the primary colour, which is
also the selected tab's fill, so they rendered the whole time and nothing could see them — the same
defect as the Info/Count badge. All three now pass through one `SeparateFromSurface` rule.

## Painter geometry was not DPI-scaled

The style work introduced insets, gaps, radii and rule thicknesses as raw constants. A 3px gap stays
3px at 200%, so on a high-DPI display the chrome shrinks to a third of its intended weight while the
text scales normally. `BaseTabPainter.Scale` now exists and every painter literal and design-pixel
constant goes through it.

## Vertical headers have never rendered a label — FIXED

Chasing the colour-seam work turned up a defect older than this program: **Left and Right header
positions drew no caption at all**, only the close glyph. Three causes stacked on top of each other,
and each hid the next:

1. **`TextRenderer` ignores the world transform.** Rotated text was drawn via
   `Graphics.RotateTransform` followed by `TextRenderer.DrawText`, which goes through GDI and
   discards the transform entirely — the rotation did nothing and the text landed off the tab. The
   deleted `DrawTabText` had exactly this bug, so this was never working; it is now drawn with
   `Graphics.DrawString`, which is GDI+ and honours the transform.
2. **The label's box was one line high.** `BeepTabAdornmentLayoutHelper` gave a vertical tab a
   `TextBounds` of `textHeight` inside a ~30px-wide tab, so any caption ellipsised to nothing. A
   rotated label needs a box that runs *down* the tab.
3. **Vertical tabs were sized for horizontal text.** `CalculateTabSizes` used `MeasureTab().Height`
   for the vertical run — one text line plus padding, about 30px — no matter how long the caption.
   Rotating swaps the axes, so the extent needed along the strip is the caption's *width*.

Also fixed: the edge padding was subtracted at both ends of the vertical text run while
`MeasureHorizontalAdornmentWidth` had already reserved it once, costing ~6px — enough to clip the
last glyph off a caption the tab had been sized to fit.

### How the check was wrong twice first

The first version of the vertical check counted dark pixels and passed at 205px — all of it chrome.
The second compared against a baseline with empty captions, which changes what `MeasureTab` reports
and therefore the tab size, so the difference included a resized tab rather than only glyphs. The
working version holds the captions fixed and switches `TabTextVisibility` off for the baseline, so
the two renders have identical geometry and only the glyphs differ. It reported 0px against the
broken code and 56px against the fixed code, and its threshold is set from that measured range
rather than guessed.

## Every painter now has its own visual — measured, not asserted

The seven styles were not seven styles. Two layers of duplication:

**1. Every painter carried two implementations of the same visual.** `ITabPainter.PaintTab` was
overridden by all seven painters and reachable only from `BaseTabPainter.PaintTabItem` — which never
runs, because all seven override *that* too. So each painter had the same colours and fill written
once against `tabRect`/`isSelected` and once against `itemLayout.Item`, and only the second was ever
displayed. Proven with a probe subclass that records whether its `PaintTab` is reached when the host's
real entry point is invoked: it is not. `PaintTab` and `DrawTabText` are deleted from the interface,
the base and all seven painters; the solution compiles clean.

This settles a question reading got wrong twice, in opposite directions — first "PaintTab is dead",
then "PaintTab is the live extension point". Both were readings. The answer came from running it.

**2. Classic, Capsule and Segmented were one painter with three radius constants,** and Minimal and
Underline had byte-identical `PaintTabItem` bodies. Each style now has its actual identity:

| Style | What makes it that style |
|---|---|
| Classic | selected tab is a sheet with an open bottom edge that merges into the content; unselected are unfilled with hairline dividers |
| Underline | full-width rule across the strip, thick accent bar on the selected tab, accent-coloured selected label |
| Capsule | inset floating pill; only the selected tab is filled, hover is a lighter pill |
| Minimal | no chrome at all; selection carried purely by contrast, unselected labels blended toward the strip |
| Segmented | one recessed bordered track spanning the run, dividers between segments, raised tile for the selected one |
| Card | every tab is a separated bordered card; the selected one is lifted and carries an accent stripe |
| Button | bordered button per tab, filled when selected |

Minimal and Underline no longer override `PaintTabItem` at all — that shared body is now the base
implementation, so the styles differ by what they *add*, not by repeating what they share.

### The measurement

Exact-equality is too weak a test: two tabs differing only by a corner radius pass it. The sheet also
measures pairwise pixel difference and requires every pair to differ by **at least 3%**.

Before: Classic/Segmented 0.4%, Classic/Capsule 1.4%, Underline/Minimal 0.7%, and under
MaterialDesignTheme ten pairs below the floor including Capsule/Minimal at **0.0%**.
After: the closest pair under any theme is Classic vs Capsule at 3.1–4.0%.

I briefly demoted this check to informational on the grounds that a pixel count cannot separate a
radius tweak from a small salient feature. That was the wrong call — the fix was to give each style a
treatment that spans the strip rather than one thin bar, which moves the real styles clear of the
floor and lets the check assert again. Weakening the measurement to fit the code had it backwards.

## What the sheet found

Four defects, none of which any code-reading or pixel-fingerprint assertion had caught:

**1. The style transition never ends.** `_styleTransitionTimer` stopped but left
`_styleTransitionProgress` at `1f` and `_transitionFrom != _transitionTo`. So after the *first*
style change the control was permanently "mid-transition":
`BeepTabHeaderRenderRequest.HasTransition` stayed true forever, meaning **every tab was painted
twice by two different painters on every paint** — one pass at alpha 0, doing all its GDI work to
produce nothing — and `PrimaryPainter` was never used again. It also made
`DrawHeaderSelectionIndicator` take its transition branch forever, so the settled-state code below
it was unreachable. Fixed by clearing the transition state, not just stopping the timer.

**2. Minimal and Underline were the same style.** `UnderlineTabPainter` and `MinimalTabPainter` are
byte-identical apart from the class name, and neither draws an underline — the accent bar is drawn
by `BeepTabs.Animation`, which drew it for `Underline` **or** `Minimal`. Combined with defect 1 that
made the two styles pixel-identical under every theme. "Minimal" now means no accent bar. The two
painter classes are still identical code, which is recorded as remaining work below.

**3. The selected tab's label was invisible in Underline and Minimal.** Neither painter fills a tab
body, but the base class handed them the *selected* text colour, which is chosen to sit on a filled
accent. Result: white text on the white header — the title vanished the moment you selected the tab.
Fixed by resolving the label colour through `ColorUtils.EnsureReadable` against a new
`GetTabSurfaceColor`, which the two non-filling painters override with the header background.

**4. Every close button was a solid dark square.** `GetCloseIconPath` returned `close.svg`, which is
not a glyph — it is a red rounded-square badge (`<rect fill="#dd4752">`) with a white cross on top.
Tinting multiplied the badge to near-black; recolouring filled badge and cross with one colour. Both
treatments are correct for a glyph and wrong for a badge. Switched to `x.svg`, a single `<polygon>`
with no fill attribute, which recolours to a clean monochrome cross. A probe now asserts the glyph
leaves its background visible rather than covering the box.

## A correction

An earlier note in this program claimed the "Underline and Minimal render identically" finding was
an artifact of the `FillPath` crash. **That was wrong.** It was real, and it was masked by a
different problem: the sheet captured mid-cross-fade because it waited a fixed ~240 ms against a
220 ms animation, so two runs of the *same binary* disagreed. The sheet now waits on the actual
timers (style transition and underline slide) and drives a real selection change, and the result is
stable across repeated runs. A flaky assertion is worse than no assertion — it produced a confident
conclusion in both directions.

**5. Each painter now owns its own style.** The accent bar was drawn by a
`_tabStyle == TabStyle.Underline || _tabStyle == TabStyle.Minimal` branch inside `BeepTabs.Animation`
— a switch on style sitting outside the painters, which is what painters exist to avoid, and the
reason Minimal drew Underline's accent. `ITabPainter` now has `PaintSelectionAccent`, called once per
paint after every tab and outside the per-tab clip (the accent animates between tabs and would
otherwise be clipped mid-slide). `BaseTabPainter` implements it as a documented no-op so adding a
painter requires no thought about accents; `UnderlineTabPainter` overrides it. The style transition
cross-fade still works — it asks both the outgoing and incoming painters to draw at complementary
alpha. `UnderlineTabPainter` and `MinimalTabPainter` are finally different classes doing different
things.

Still to add: a high-contrast column (see [08](08-input-and-accessibility.md) — colours are now
sourced correctly but have never been rendered), hover/selected/dirty/badge state columns, and DPI
variants.


**Priority P2.** Depends on [01](01-painter-contract.md).

## Current behaviour

Seven painters derive from `BaseTabPainter` (354 lines):

`ButtonTabPainter`, `CapsuleTabPainter`, `CardTabPainter`, `ClassicTabPainter`, `MinimalTabPainter`,
`SegmentedTabPainter`, `UnderlineTabPainter`

**Each overrides exactly two members.** Every one of them. That uniformity is suspicious in the
specific way that matters here: a capsule, a card and an underline tab are visually very different
shapes, and it is not obvious that two override points can express all of them.

This may be fine — the base may parameterise shape through the style config. Or it may be the
`BeepTree` situation, where 28 painters existed and a contact sheet revealed four groups rendering
identically, plus one painter that drew nothing at all and one that drew solid magenta. **Nobody has
rendered these seven side by side.** Until that happens, "seven styles" is an unverified claim.

Style resolution is spread across three types:

| Type | Role |
|---|---|
| `TabStyles.cs` | `enum TabStyle`, `enum TabLabelVisibility` |
| `Models/TabStyleConfig.cs` | per-style configuration |
| `Helpers/TabStyleHelpers.cs` | maps `TabStyle` → `BeepControlStyle`, radii, etc. |

Three types for one concern is not automatically wrong — enum, config, mapper is a reasonable split.
It is listed here so the audit covers whether any two of them disagree, which is the failure mode.

`Helpers/TabThemeHelpers.cs` (261 lines) and `TabColorConfig` handle colour.

## What the reference products do

- Style is a *shape and chrome* choice; theme supplies the palette. The two are orthogonal, and
  changing the theme must restyle every painter without touching them.
- Selected, hovered, pressed, focused, disabled, pinned, dirty and preview states are each visually
  distinct, in every style.
- High contrast overrides the palette entirely with system colours.

## Work

1. **Contact sheet first.** Render all seven painters with identical tabs, then across states
   (selected / hovered / pressed / disabled / pinned / dirty / preview), then across at least three
   themes including a hostile one. Review it once, then keep as a baseline to diff against.
   This is the cheapest way to find out whether seven styles exist.
2. **Assert distinctness** the way the tooltip variants were: no two painters may produce identical
   output for identical input unless that is intended and recorded.
3. **Audit the three style types for disagreement** — e.g. a radius defined in `TabStyleConfig` and a
   different one returned by `TabStyleHelpers` for the same style.
4. **Verify live theme switching** repaints tabs and does not leave cached brushes from the previous
   palette. The grid toolbar shipped exactly that bug.
5. **Contrast**: every state/theme pair must meet 4.5:1 for label text. `ColorUtils.EnsureReadable`
   already exists in this repo for this; reuse it rather than writing a fourth implementation.

## Verification

- Contact sheet: 7 painters × 7 states, reviewed and stored as baselines.
- Assertion: no two painters render identically for identical input.
- Assertion: every state/theme label colour pair meets 4.5:1.
- Probe: switch theme with tabs visible and assert every tab repaints with the new palette.
