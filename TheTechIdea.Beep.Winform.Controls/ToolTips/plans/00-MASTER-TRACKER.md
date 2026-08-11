# ToolTips — review & enhancement plan (2026-08)

43 files, ~10,200 lines (twelfth folder). `ToolTipManager` (singleton, active-instance map,
cleanup timer, ThemeChanged subscriber) → `ToolTipInstance` (lifecycle) → `CustomToolTip :
BeepiFormPro` (floating form, 6 partials) → 4 painters over `ToolTipPainterBase`/factory +
`ToolTipStyleAdapter` (per-paint colour seam). Also `BeepPopover : CustomToolTip`,
`BeepTourManager`/`BeepTourBuilder`/`BeepTourStep` (guided tours), 15 helpers, config
models. Prior doctrine work visible (C8 comments, markup parser, position resolver).

## Findings (static pass)

### F1 — the theme freeze: ApplyThemeColors STAMPS resolved colours into nullable config slots

`ToolTipConfig.BackColor/ForeColor/BorderColor` are `Color?` — null means "resolve from
the theme at paint", and the painters already do exactly that
(`ToolTipStyleAdapter.GetColors` → helpers with the config colours as custom-override
passthrough). `ApplyThemeColors` (3 call sites: CustomToolTip.Core, CustomToolTip.Methods,
ToolTipManager) writes resolved colours INTO the null slots, after which every tooltip
looks custom-coloured — the manager's `OnThemeChanged` → `tip.ApplyTheme` then re-stamps
NOTHING because the "only when not set" guard sees stamped values. Live theme changes were
dead on arrival despite the C8 subscription. DELETE ApplyThemeColors and its call sites;
per-paint resolution already exists.

### F2 — ToolTipThemeHelpers: the anti-pattern + Blend/Darken derivations

`useThemeColors` flag (43 sites incl. `ToolTipConfig.UseBeepThemeColors` and external
setters in BaseControl.Tooltip/BreadCrumbs/Steppers), Empty-guards, Material-palette
literal fallbacks, `BlendColors` semantic tinting (30% semantic over ToolTipBackColor),
`DarkenColor` borders, dead `LightenColor`. Rewrite slot-direct: default type →
`ToolTipBackColor`/`ToolTipForeColor`/`ToolTipBorderColor`; semantic types → the semantic
slot as fill, WCAG brightness pick for ink (accepted idiom), same slot for border; links →
`ToolTipLinkColor`/`ToolTipLinkHoverColor`; shadow → `ToolTipShadowColor`.

### F3 — literal sweep (54 census hits)

Painters/arrow/badge/markup literals audited per file in batch 1; alpha veils of resolved
slots and WCAG picks stay.

### F4 — reporting: 26 raw Debug.WriteLine + 3 bare dispose swallows

Catches DO report — but through `Debug.WriteLine` (invisible in Release, unroutable).
Route through `BeepLog.Failure`/`FailureOnce` (paint paths get the Once form).
`ToolTipInstance` has 3 bare `try {...Dispose()} catch { }` — report via Failure. The
manager ctor wraps its `ThemeChanged +=` subscription in try/catch "missing at build
time" — an event subscription cannot throw at runtime; unwrap. `OnThemeChanged`'s
per-tip bare catch reports FailureOnce.

### F5 — dead code

`ToolTipStyleConfig` (zero consumers), `VirtualToolTipHost` + `IToolTipHost` (interface +
sole implementation, zero external consumers, "Sprint 12" experiment), private
`LightenColor`. Delete; removals recorded here.

### F6 — theme census (BOTH homes, per the Tabs lesson)

52 themes healthy, none stamped back==fore. ONE defect: `RusticTheme` declares all 9
ToolTip slots with NO initializers — every slot `Color.Empty`, tooltips render
transparent-on-transparent. Fix with rustic-palette values (property-initialiser format
cannot reference sibling properties, so literals are the format's convention).

### F7 — no probe

Planned (TipProbe): manager Show → CustomToolTip renders (DrawToBitmap on the floating
form); semantic types render distinctly; two styles differ; LIVE theme change repaints an
open tooltip (the F1 proof — this check must fail before the F1 fix); placement lands
near the anchor; Hide/dispose leaves no active instances; popover + tour step render.
Eyeball everything.

## Order

1. F1 delete stamping + F2 helpers rewrite + flag sweep (incl. external setters) + F3
   literals + F5 dead code — build + commit
2. F4 BeepLog routing — build + commit
3. F6 Rustic fix — commit
4. F7 probe + eyeball — commit per fix batch

## Batches 1–3 landed (commits 89fb7165, 850481bf, a1b20bec)

As planned: stamping + flags + literals + dead code; BeepLog routing; Rustic ToolTip part.

## Batch 4 — probe + eyeball (TipProbe 11/11)

Probe: manager Show renders each semantic type distinctly (100% surface painted), a LIVE
theme change repaints an OPEN tooltip (guarded by asserting the themes' slots differ),
placement lands near the anchor, HideAll leaves nothing active. Renders eyeballed.

Two more real bugs the probe forced out:

1. **HideAllTooltipsAsync disposed before awaiting** — it started every HideAsync, then
   disposed all instances, then awaited: disposing kills the fade timer mid-flight, its
   completion never fires, and `Task.WhenAll` hangs forever (the probe's watchdog caught
   it). Reordered to await-then-dispose per instance, same as HideTooltipAsync.
2. **ApplyAccessibilityEnhancements stamped magenta into configs** — it seeded "current"
   colours from the FORM's BackColor, which is the magenta transparency key, ran WCAG
   "correction" on that, and stamped the result into the config's null slots. Every
   default-coloured tooltip body rendered as the transparency key = see-through on
   screen. Deleted: HC lives per-paint in the helpers, semantic ink is the WCAG pick,
   and Default-type contrast is theme-authored.

Instrument notes: `DrawToBitmap` on a TransparencyKey form captures the raw magenta
back-buffer — the first probe "passed" by comparing window SIZES; caught on the eyeball
(renders must be eyeballed). The capture now drives OnPaint directly over a
magenta-prefilled bitmap and asserts >80% painted, non-magenta. Stale-binary trap: after
a library rebuild the probe exe must be rebuilt too, or it runs its own old DLL copy.

Observations (not fixed): two-line tooltips clip the last line's descenders slightly
(pre-existing height math); BeepPopover buttons, tour flow, arrow rendering, markup
parser, auto-update and delay groups are probe-unverified (recorded, review only).

## Batch 5 — "tooltips don't take the manager's theme" (user report; TipProbe 13/13)

Root cause, two halves:

1. **`BeepThemesManager.DefaultTheme` had ZERO writers** — an auto-property with
   `internal set` that nothing ever set, so it returned null forever. It now resolves
   through `GetDefaultTheme()` (DefaultTheme → DefaultBeepTheme → ensure-fallback → any),
   so the manager genuinely always has a default theme.
2. **`CustomToolTip` initialised `_theme` from that dead property** — so any tooltip not
   shown through the manager (BeepPopover, tour tips, direct `ApplyConfig`+Show) started
   with a NULL theme, and nothing ever re-themed it: the manager's `OnThemeChanged` only
   reaches manager-TRACKED instances, and `BeepiFormPro`'s own ThemeChanged handler
   re-themes the form chrome, not the tooltip's `_currentTheme`/`_theme` fields. The ctor
   now starts from `CurrentTheme`, and the form subscribes to `ThemeChanged` itself
   (unsubscribed in the existing `Dispose(bool)` — the event is static).

Probe additions: `DefaultTheme` is never null; a DIRECTLY-constructed tooltip (no
manager) renders the manager's theme and follows a live ArcLinux → Zen change (both
eyeballed: blue-slate card → charcoal card).

## Standing constraints

There is ALWAYS a theme — slot per role from the control's OWN slot family, no flag, no
guards, no blends/luminance (alpha veils + WCAG picks are accepted idioms). A
wrong-looking colour is the THEME's bug, fixed in the theme parts (both homes). A check
must be able to fail; renders get eyeballed. Commit to master only.

## Batch 6 — UI/UX review of the tooltip forms (rendered gallery, eyeballed)

Fourteen configurations rendered through the real manager and inspected:
plain / title+text / long / the semantic types / arrow / multi-line / Rich / Card, under
Default, Zen and ArcLinux.

### Fixed: a Title set on the default variant was silently discarded

`LayoutVariant` defaults to `Simple`, and `ToolTipSectionPlan.For` sets `ShowTitle = false` for
Simple by design ("Simple means simple"). The consequence was that the natural call -
`new ToolTipConfig { Title = ..., Text = ... }` - rendered **body only**, dropping half the
content with no warning. Confirmed by render: identical configs showed the title under `Rich`
and lost it under the default.

`LayoutVariant` now tracks whether it was set **explicitly**. If a Title is present and the
caller never chose a variant, it reports `Rich`; an explicit `Simple` still suppresses the title,
so the original design intent survives when it is asked for by name. Titled tooltips grew
61px -> 86px, and the title now renders in every default-variant shot.

### Checked and NOT a defect

- **"Everything looks bold."** It is not: the painter resolves title = Roboto 11.5pt **Bold** and
  body = Roboto 10pt **Regular** (verified by reflecting `GetTitleFont`/`GetTextFont`). Roboto
  Regular at 10pt simply renders heavy at this size. Worth recording because it looked like a bug.
- **Semantic types** (Error/Success/Warning) render a full-bleed semantic card with white ink and
  read well.
- **Rich / Card variants** lay out title, divider and body correctly.

### Open findings (not fixed)

1. **One-liners are too tall.** "Save" occupies 150x61. `CalculateSize` starts at
   `DefaultPadding * 2` (24) and then adds the style's shadow blur/offset to the WINDOW size, so a
   single short word gets a 61px window. A native tooltip is ~22px. The shadow allowance is
   legitimate but should not read as vertical padding around the text.
2. **Minimum width is effectively 150** even though `DefaultMinWidth` is 100 - short tooltips are
   wider than their content needs.
3. **`ShowArrow = true` produced no visible arrow** in the client capture. It may be drawn outside
   the client rect (the capture is client-only), so this is unconfirmed rather than proven broken.
4. `BeepPopover` and the tour tooltip painters were not rendered in this pass.

## Batch 7 — tooltip sizing now matches its text

User: "sizing of tooltips is not correct compared to text" (width AND height). Four separate
inflators, each measured on a rendered gallery:

| | "Save" | titled | long |
|---|---|---|---|
| before | 150x61 | 242x86 | 360x103 |
| after  | **74x43** | 238x68 | 360x85 |

1. **A shadow allowance for a shadow nobody paints.** `CalculateSize` added the style's
   `shadowBlur + offset` to the tooltip's SIZE. But the card is painted edge to edge across the
   whole bounds, and `PaintBackground` was rewritten in batch 1 to a plain fill - so no shadow is
   drawn at all. The reservation simply inflated the card by ~20px in both directions. Removed,
   with a note that the allowance and the painting must return together if a drop shadow is ever
   reintroduced.
2. **`GetRecommendedMinWidth` was a slab, not a floor** - 150 for every standard style, applied as
   a hard minimum. A one-word tooltip measuring ~58px was padded to 150. Now 56/60/64, which only
   guards against a degenerate sliver; a tooltip's shape is governed by its MAX width (wrapping).
3. **Measurement and painting used different padding.** `CalculateSize` padded with the constant
   `DefaultPadding = 12`; `GetContentRectangle` padded with the style's own `StyleSpacing`
   value. For any style whose padding is not 12, the box reserved and the box drawn into were
   different sizes - the frame could not match its text. Both now call one authority,
   `GetPaddingX`/`GetPaddingY`.
4. **Equal padding on all four sides.** A tooltip is one band of text; `GetPaddingY` is now
   `padX - 5` (min 5), so one-line hints stop looking like dialogs. `DefaultMinWidth` 100 -> 56
   and the form's hardcoded 40px height floor -> 24.

TipProbe 13/13, NoteProbe 7/7, renders eyeballed under Default/Zen/ArcLinux.

Still open from batch 6: the arrow was not visible in a client-only capture (unconfirmed), and
`BeepPopover` plus the tour painters remain unrendered.

## Batch 8 — BeepPopover and the remaining painters rendered

Finishes the review: Tour, Preview, Glass and `BeepPopover` all rendered and eyeballed.

### Fixed: BeepPopover drew its buttons ON TOP of its message

`CustomToolTip` sizes itself from title + text only - it knows nothing about buttons a subclass
mounts afterwards - and `PositionButtons` anchors them to `Height - btnH - margin`. With no room
reserved, a "Discard changes? / This cannot be undone." popover rendered its message *underneath*
the Cancel button (client 187x66, buttons at Y=32 where the body text sits).

`ReserveButtonRow` now grows the popover by the button row after mounting, and widens it when the
two buttons need more than the text does (respecting `MaxPopoverWidth`). Row geometry moved to
shared constants - the reservation and the positioning previously hardcoded 28/8 independently,
which is how they drifted. Client 187x66 -> 187x110, buttons Y=32 -> Y=76, message clear.

**Capture note:** neither capture alone shows this control. Driving `OnPaint` renders the painter's
card but no child controls; `DrawToBitmap` renders the children but not the painted card. The
gallery now composites both, which is the only way the overlap was visible.

### Rendered and sound

- **Tour** - step counter, title, body, progress dots, Skip / Back / Next all laid out correctly.
- **Glass** - renders, but see below.

### Open findings (not fixed)

1. **Preview variant reserves a large empty image area when no image is supplied** (280x199, most
   of it blank, filename stranded at the bottom). It should collapse to the text when there is
   nothing to preview.
2. **Glass variant has very low text contrast** - the title is barely legible against the acrylic
   fill. It needs the WCAG ink pick the semantic types already use.
3. The arrow is still unconfirmed (client-only captures cannot show it if it is drawn outside).

---

## Batch: window shape, and the painters that disagreed with it

All three findings above are now **closed**, plus the shape work they sat behind.

### The shape defect, in four parts

1. **`CustomToolTip` derived from `BeepiFormPro`, which owns the window shape in two places** -
   `UpdateFormRegion` (managed `Region`) and `UpdateWindowRegion` (`SetWindowRgn`) - and rebuilds
   **both** as a rounded *rectangle* from the active form painter's corner radius on every size and
   style change. Any silhouette the tooltip set was overwritten before it was shown. It is now a
   plain `Form`; it used nothing else from that base (the rebase compiled with zero errors).
2. **The caret was never inside the window.** `CalculateTipPoint` put the tip at
   `bounds.Bottom + arrowSize`, so every arrow was clipped away and `ShowArrow` did nothing.
   `CalculateSize` now reserves a caret strip and `GetCardRectangle` insets the card into it.
3. **Card and caret were filled separately while the region was their union**, so the pixels
   between the two fills showed through as a hairline. Background, border and region now all come
   from one `GetSilhouettePath`.
4. **`TransparencyKey` is gone.** A colour key removes only EXACT matches: the shadow pass turned
   it from (255,0,255) into (161,0,161) so nothing keyed out, and even without that, every
   antialiased card edge is a blend toward the key - tracing a halo around the whole outline. The
   surface under the card is now the card's own colour.

### Closed findings

- **Preview reserved a 160px image band with no image.** `ImageBandHeight` is now one authority
  used by measure and paint, and returns 0 when the config names no path, no loader and holds no
  resolved image. A skeleton means "still loading"; there was nothing to wait for. 280x199 -> 280x87.
- **Preview never rendered `config.Text`** - it read only `PreviewSubtitle`, so a tooltip built the
  ordinary way showed its title and silently dropped its body. `BodyText` falls back to `Text`.
- **Preview measured and drew with literal `new Font("Segoe UI", ...)`** rebuilt on every paint,
  ignoring the theme. Both paths now share `TextFonts()` from theme typography (cache-owned).
- **Glass text contrast.** The ink was the theme's tooltip *foreground* - chosen for the DARK card -
  while the frost is that background composited against a near-white base, i.e. light. It is now a
  WCAG contrast pick between two theme slots (`ReadableOn`), and the black offset text-shadow that
  had been papering over it is gone.
- **The arrow is confirmed**, by region hit-test and by eyeballed renders of all four shapes.

### Public members removed

| member | why |
|---|---|
| `ToolTipConfig.ShowShadow`, `ToolTipConfig.EnableShadow` | nothing reads them; the shadow pass is gone |
| `IToolTipPainter.PaintShadow` + `ToolTipPainterBase` abstract + all 4 overrides | dead: the styled painter's scrim covered the whole window (it is what poisoned the colour key), and Glass's glow inflated OUTSIDE the bounds so every layer was clipped - it had never been visible |
| `ToolTipBuilder.WithShadow(bool)` | a fluent call that set only the above |
| `BaseControl.TooltipShowShadow` | a designer-visible checkbox on every Beep control that drove only the above |
| `CustomToolTip : BeepiFormPro` -> `: Form` | see 1 |

A real drop shadow is still possible and is **not** implemented: it must be drawn *outside* the
silhouette with matching space reserved in `CalculateSize`, not as a scrim over the window.

### Verified

- Region hit-tests: corner and both caret flanks outside; card centre and caret tip inside.
- **Break-it-first**: forcing `Region = new Region(new Rectangle(0,0,Width,Height))` turns the
  corner and both flank checks red - exactly the reported "it's just a rectangle" defect. Restored
  and re-run green.
- Eyeballed: Rounded / Soft / Pill / Sharp composited over a backdrop through the region, plus
  Glass, Preview and Tour - the latter three had never been rendered at all.

### Not verified / still open

- **`BaseControl.UseRichToolTip` is never read.** It is assigned 12 times across
  `BeepPopupListForm.Designer.cs` and `BeepGridFilterFlyout.designer.cs` and has no consumer, so
  setting it false changes nothing. Left in place this pass - deciding between wiring it and
  deleting it needs a call on whether it should mean anything distinct from `EnableTooltip`.
- Placements other than `Top` are unrendered: the caret strip is reserved per placement in
  `GetCardRectangle`, but only Top has been eyeballed.
- `BeepPopover` still unrendered.
