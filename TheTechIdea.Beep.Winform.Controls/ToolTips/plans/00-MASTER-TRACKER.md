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
