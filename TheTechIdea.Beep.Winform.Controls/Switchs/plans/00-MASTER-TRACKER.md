# Switchs — review & enhancement plan (2026-08)

24 files, ~3,100 lines. `BeepSwitch : BaseControl` (Core/Properties/Drawing/Interaction/
Animation/DataBinding partials), 4 painters (iOS/Material3/Fluent2/Minimal) behind
`SwitchPainterFactory` keyed by `BeepControlStyle` (every style maps to one of the four),
helpers for theme/style/icon/font, `SwitchMetrics` + `SwitchColorConfig` models. Eighth folder
in the series.

## Findings (static pass)

### F1 — the theme's dedicated `Switch*` family is completely ignored

`IBeepTheme` carries 9 colour slots + 3 fonts for switches (`SwitchBackColor`,
`SwitchSelectedBackColor`, `SwitchHoverBackColor`, Border/Fore variants, fonts). The helpers
never touch ONE of them — instead: the full anti-pattern (useThemeColors flag ×51, Empty-guard
chains, Material-green literal fallbacks `76,175,80`) over GENERIC slots (SuccessColor for the
on-track!), plus banned `ShiftLuminance` derivations for hover/pressed when
`SwitchHoverBackColor` exists for exactly that.

Mapping (settled): off track → `SwitchBackColor`, on track → `SwitchSelectedBackColor`,
hover/pressed → `SwitchHoverBackColor`, borders → `SwitchBorderColor`/Selected/Hover, label →
`SwitchForeColor`/`SwitchSelectedForeColor`/Disabled, thumb → `OnPrimaryColor`
(disabled → `DisabledBackColor`), shadow/focus ring → alpha veils of `ShadowColor`/
`PrimaryColor` (keep). `GetContrastColor` has zero callers — delete.

### F2 — `SwitchColorConfig`: zero consumers anywhere — dead model (the Steppers precedent)

### F3 — 0 swallows (healthiest so far), but the 2 icon reflection probes over `SvgsUI`
resolve icon names dynamically with silent-miss — misses must report once (the BreadCrumbs
registry-probe treatment).

### F4 — ~54 literals; most die with F1, remainder audited per painter in batch 2

### F5 — no probe

Planned (SwitchProbe): the 4 painters render on AND off (distinct pairs), theme
responsiveness, toggle round-trip — click via real handlers flips `Checked` + raises
`CheckedChanged`, Space/Enter via OnKeyDown, animation completes; DataBinding push/pull;
every render eyeballed.

## Order

1. F1 helpers rewrite to the `Switch*` family + flag removal (painters + control) + F2 delete
   dead config + F3 report icon misses — build + commit
2. F4 remaining literal sweep — build + commit
3. F5 probe + geometry eyeball — commit per fix batch

## Standing constraints

There is ALWAYS a theme — slot per role from the control's OWN slot family, no flag, no
guards, no luminance shifts (alpha veils of a slot are fine). A check must be able to fail;
renders get eyeballed. Commit to master only.

## Batches 1+2 done in one pass (small folder) — build 0 errors, census clean

F1: `SwitchThemeHelpers` rewritten to the `Switch*` family (253 → 86 lines): track on/off/
hover/disabled, borders, thumb, labels, shadow + focus ring as alpha veils. All four painters
now fill the TRACK slot-direct — the generic `BackgroundPainterFactory`/`BorderPainterFactory`
delegation never painted ON differently from OFF (the switch's one job); Minimal keeps its
outline-only identity, filling only when ON. Flag gone from all 51 sites; `ShiftLuminance`
hover/pressed derivations replaced by `SwitchHoverBackColor`.

F2: `SwitchColorConfig` deleted (zero consumers). Also deleted as caller-less: the two
`_Legacy` hand-drawn switch methods in BeepSwitch.cs (~190 lines carrying the gradient/White/
LightGray literals), `SwitchStyleHelpers.GetShadowColor`, and the rewritten `GetIconColor`
(nothing consumed it).

F3: both `SvgsUI` icon-name reflection probes report once on a miss instead of silently
falling through / not drawing.

F4: census clean — zero flags, zero literal colours.

## Batch 3 — probe + eyeball (SwitchProbe 12/12)

Probe: 4 painters render ON/OFF distinctly, cross-painter distinctness, theme
responsiveness, click via the real hit-area pipeline (cursor parked) flips `Checked` +
raises `CheckedChanged`, Space toggles back. All renders eyeballed.

Three findings the probe surfaced, none of them in the painters:

1. **`ControlStyle` pins the style's bundled theme** (`BaseControl.Properties`:
   `Theme = BeepStyling.GetThemeStyle(_controlstyle)`; iOS15 → iOSTheme). A control created
   AFTER `SetCurrentTheme` renders the bundled theme, not the manager's — a later global
   theme change wins (`OnGlobalThemeChanged` re-applies). The probe's theme check originally
   set the theme before creation and read identical renders; rewritten as a LIVE change
   after creation, which is both the scenario the check exists for and the sequence that
   can fail. Framework behaviour, left as-is, documented here.
2. **11 bundled themes shipped ON == OFF switch tracks** (stamp: all Switch* slots =
   surface/panel colour). Fixed in the themes: iOSTheme got the real UISwitch palette
   (systemGray5 off / SuccessColor green on / OnPrimaryColor thumb); ArcLinux, Brutalist,
   ChatBubble, GNOME, GruvBox, KDE, Metro, Neon, Nord, Tokyo got the selected trio →
   `PrimaryColor`/`PrimaryColor`/`OnPrimaryColor` (FluentTheme's healthy recipe), each
   verified to assign both slots. Off-track/hover character kept per theme.
3. **Thumb slot corrected to the planned `OnPrimaryColor`** — the implemented
   `SwitchSelectedForeColor` collided with `ThemeContrastHelper.ValidateTheme(autofix:true)`
   (runs at the end of every bundled theme ctor), which owns the `Switch*Fore` slots as
   caption ink and rewrote the white thumb to 70,70,70 against the green track.

Also confirmed not-a-bug: "renders Off while Checked=true" — `PaintLabels` always draws
both captions (Off left, On right); the thumb position and caption accent carry the state.

Observation (not fixed): at 120px control width Fluent2's 2:1 track ratio pushes the On
caption past the right edge — `CalculateLayout` does not auto-size the control to fit
labels. Remaining unaudited: the other ~15 bundled themes' Switch parts are healthy by
census (sel != back), but their hover slots were not reviewed.
