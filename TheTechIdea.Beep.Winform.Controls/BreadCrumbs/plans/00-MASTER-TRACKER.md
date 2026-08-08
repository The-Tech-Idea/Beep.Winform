# BreadCrumbs — review & enhancement plan (2026-08)

12 files, ~3,000 lines. `BeepBreadcrump : BaseControl` (note the class-name typo — "Breadcrump")
with 5 style painters (Classic/Modern/Pill/Chevron/Flat) over `IBreadcrumbPainter`, helpers for
theme/font/icon/accessibility. Entry motive: Steppers batch 4 exposed this folder's own copy of
the high-contrast stamping machinery.

## Findings (static pass)

### F1 — `BreadcrumbThemeHelpers` has the full anti-pattern, FOURTH folder running

`useThemeColors` flag, `!= Color.Empty` guards, literal fallbacks (hardcoded link-blue
`FromArgb(0,102,204)`, White, Gray). Worse: `customColor.HasValue` without an Empty check, and
`ApplyThemeColors` passes the control's own `BackColor`/`ForeColor` as customColor — `HasValue`
is always true, so the method returns its input and assigns it back: **completely inert**, and
callers believe theming happened. Settled fix (theme ?? CurrentTheme, one slot one return):

| getter | slot |
|---|---|
| item text, non-last | `LinkColor` (the theme HAS a link family: Link/HoverLink/VisitedLink) |
| item text, last | `ForeColor` |
| hover back | `FromArgb(40, ButtonHoverBackColor)` (alpha veil of the slot) |
| selected back | `FromArgb(80, ButtonSelectedBackColor)` — verify slot exists, else ButtonBackColor |
| separator | `FromArgb(α, LabelForeColor)` |
| background | `PanelBackColor` |
| border | hover → `ButtonHoverBorderColor`, else `BorderColor` |

`GetThemeColors` tuple — check callers, delete if dead. The `_currentTheme ?? (UseThemeColors ?
CurrentTheme : null)` themeless-mode dance in the control dies with the flag.

### F2 — high-contrast stamping copy (the Steppers batch-4 disease)

`GetHighContrastColors` (non-HC branch returns literal Black/LightGray/Gray/Black),
`AdjustColorsForHighContrast`, `ApplyHighContrastAdjustments` (stamps SystemColors into the
control's BackColor/ForeColor — frozen after HC turns off). Fix like Steppers: HC as a
resolution-time branch inside `BreadcrumbThemeHelpers`; delete the stamping trio + the control's
`ApplyAccessibilityAdjustments` wrapper (keep the MinimumSize part). Check the grayscale
converters at a11y 279/285 for the same dead-machinery family.

### F3 — 4 swallows, 0 BeepLog references

- `BeepBreadcrump.cs:891,902` — `try { oldFont.Dispose(); } catch { }` ×2
- `BreadcrumbAccessibilityHelpers.cs:38` — HC detection catch → `return false`
- `BreadcrumbIconHelpers.cs:89` — icon reflection catch → comment-only fallback

### F4 — ~22 literals

Ctor `BackColor = Color.White; ForeColor = Color.Black` (pre-theme literals), helper fallbacks
(F1/F2), `BreadcrumbIconHelpers` `Color.Gray`.

### F5 — child re-theming smell

`ApplyTheme` sets `button.Theme = Theme; button.ApplyTheme();` on drawing components — check
against rule 3 (never call ApplyTheme on a child) and whether these stamps are parented.

### F6 — no probe

Planned (CrumbProbe): all 5 styles render with real items (wide+narrow), blank-guard +
distinctness; theme responsiveness; click → `CrumbClicked` through the real hit-area path
(cursor parked); keyboard navigation (arrow + Enter raises the click); SelectedIndex round-trip;
eyeball every render (the Steppers lesson: colour counters cannot see a single-node collapse).

## Order

1. F3 swallows + F1 helpers rewrite + F2 HC resolution-time — build + commit
2. F4 literal sweep + F5 child-theming check — build + commit
3. F6 probe + per-style geometry eyeball — commit per fix batch

## Batches 1+2 done in one pass (small folder) — build 0 errors, census clean

F1: `BreadcrumbThemeHelpers` rewritten to the settled end-state (217 → 72 lines): theme ??
CurrentTheme, one slot one return, HC as a resolution-time branch (SystemColors per paint —
the Steppers batch-4 shape). Non-last items resolve the theme's Link family (LinkColor /
HoverLinkColor), last item ForeColor, veils are alpha-of-slot. The inert `ApplyThemeColors`
(returned its own input) is deleted.

F2: HC region + WCAG luminance-adjust region deleted from the a11y helper — every caller was
either the inert stamping path or the painters' per-paint HC/contrast juggling, all removed with
the helpers rewrite. `ApplyAccessibilityAdjustments` wrapper deleted from the control (MinimumSize
part moved into ApplyTheme).

F3: all 4 swallows report (2× font-dispose WarnOnce, reduced-motion query WarnOnce, icon
reflection FallbackOnce) + a 5th found in `BreadcrumbPainterBase` font cache (FallbackOnce).

F4: zero literals remain — ctor White/Black now resolve from the theme; separator no longer
passes ForeColor as the always-winning custom override (LabelForeColor slot had NEVER resolved).

F5 resolved: `BaseControl.Theme`'s setter calls ApplyTheme itself, so the explicit
`button.ApplyTheme()` after `button.Theme = ...` themed every stamp TWICE — dropped, assignment
only. Also: the two `UseThemeFont` branches in ApplyTheme ran IDENTICAL code (flag decided
nothing) — collapsed; Paint no longer assigns BackColor mid-paint; tooltips always themed.

Not verified yet: renders (batch 3's probe). HC branch verified by build + review only (system
HC not toggleable from a probe).

## Batch 3 done — probe 16/16, icon/text geometry fixed (the user's complaint), capture bug

User report mid-batch: "still breadcrums aligmnents and sizing has problme with icons and text
for each crumb" — confirmed by render: the folder icon painted OVER the "D" of "Documents",
"Home" started inside its icon. Root causes, all five painters:

- `CalculateItemRect` reserved a flat 20px for the icon while `GetIconSize` paints at 65% of
  item height (24px at h=37) — the reservation understated the real icon.
- `DrawItem` centred the button text across the FULL rect (icon zone included), so text pushed
  left into the icon on every crumb.
- Classic drew with a hardcoded 10pt font while the rect was measured with the painter's
  TextFont — measured width ≠ drawn width.

Fix: one authority in the painter base — `IconZone(item, height)` = scaled lead + real icon
width + scaled gap; `CalculateItemRect` reserves it, `DrawItem` draws the button in
`TextRect(rect, item)` (right of the zone); every painter draws with the same font MeasureText
sized the rect with; paddings DPI-scaled.

**Mouse clicks reported the wrong index for every crumb**: the hit-area callback captured the
shared `for` variable — `() => OnItemClicked(item, i)` gave each crumb the post-loop index
(idx=4 with 4 items, out of range). Only the probe's click round-trip exposed it; the keyboard
path passed the index correctly. Fixed with a per-iteration copy.

**Classic == Modern pixel-identical at rest** (distinctness check): Classic's old "distinct
look" was the accidental 10pt font; once removed, nothing separated them. Modern's rounded chip
(its hover identity) now also marks the last/selected crumb at rest.

Probe (CrumbProbe, scratchpad): 5 styles × wide+narrow render + blank-guard, cross-style
distinctness, theme responsiveness, hit-area click round-trip (real hit-list rects — the
size-only `_itemRectCache` X=0 fooled the first probe run), keyboard Right+Enter, SelectedIndex
round-trip. All renders eyeballed including a pixel zoom of the icon/text seam.

Open (not defects, recorded): narrow widths clip trailing crumbs at the control edge — no
middle-collapse/ellipsis behaviour exists; `BeepBreadcrump` class-name typo ("Breadcrump")
would be a public-API rename — user's call.

## Standing constraints

There is ALWAYS a theme — assign slots directly, never guard, never blend palettes, never literal
(semantic states from semantic slots; alpha veils of a slot are fine). Text rects sized from
fonts. A check must be able to fail — and renders get eyeballed, not just counted. Commit to
master only.
