# ProgressBars — review & enhancement plan (2026-08)

31 files, ~5,000 lines (fifteenth folder). `BeepProgressBar : BaseControl` (+ Core
partial), 14 painters behind `ProgressPainterRegistry`/`ProgressPainterKind` (linear,
rings, dots, steppers, chevrons, arrows…), painter context/state models, helpers for
theme/font/icon/layout/DPI/accessibility. Painters read colours from OWNER PROPERTIES
(`owner.ProgressColor` etc.), so getter-level resolution fixes every painter at once.

## Findings (static pass)

### F1 — the flag + stamping web

`UseThemeColors` (LOCAL property, 12 sites) gates between an INERT
`ApplyThemeColors` — it passes the control's own non-nullable colours as the
always-winning custom override, so it re-applies what was already there — and
`ApplyColorProfile`, which stamps a LITERAL Material-palette `ProgressBarColorConfig`
(`ColorProfile` designer property). Either way the control properties hold stamped
values and explicit caller colours cannot be told from themed ones.

Settled shape: `ProgressColor/TextColor/SecondaryProgressColor/Success/Warning/Error`
fields default `Color.Empty`; GETTERS resolve custom-else-slot per read (painters
already read the properties per paint — zero painter changes). BackColor stays the
control-surface norm. Flag property deleted; `ProgressBarColorConfig` + `ColorProfile`
+ `ApplyColorProfile` deleted (zero external consumers); ApplyTheme shrinks to
border-pen/font/style/accessibility.

### F2 — ProgressBarThemeHelpers: anti-pattern + a reflection probe for a slot that doesn't exist

Empty-guard chains, literal fallbacks, and `theme.GetType().GetProperty(
"ProgressBarWarningColor")` — no such slot exists on IBeepTheme; Warning maps to
`theme.WarningColor` directly. Rewrite slot-direct flag-less: back→`ProgressBarBackColor`,
fill→`ProgressBarForeColor`, inside text→`ProgressBarInsideTextColor`,
border→`ProgressBarBorderColor`, success/error→their ProgressBar* slots,
warning→`WarningColor`, secondary→alpha veil of `SecondaryColor`, hover triple→the
`ProgressBarHover*` slots direct (the old code alpha-wrapped dedicated hover slots).
Caller-less `GetThemeColors` 11-tuple deleted. HC per paint.

### F3 — fictional catch towers and silent reflection probes

`ProgressBarAccessibilityHelpers.IsHighContrastMode` wraps
`SystemInformation.HighContrast` in a THREE-level try/catch (none of it can throw;
"catching is not error handling if nothing throws"). `IsReducedMotionEnabled` wraps a
P/Invoke that returns bool. Both flattened. `ProgressBarIconHelpers` has two SvgsUI/Svgs
reflection probes with dead catches (GetProperty returns null for a miss, it does not
throw) and silent-miss — misses report `WarnOnce` (Switchs precedent). The two
`ArgumentException` catches in BeepProgressBar look narrow — verify they report or are
genuinely impossible.

### F4 — painter literal audit (77 census hits)

Most are white-alpha speculars/glows/sheens (accepted idiom) and `ColorConfig`/helper
fallbacks that die with F1/F2. Audit the remainder per painter in the sweep.

### F5 — RusticTheme is systemically unthemed (~330 Empty slots)

The census found `ProgressBarBackColor` etc. uninitialised — and widening it: 25 Rustic
part files declare ~330 colour slots with NO initialisers (ProgressBar, ScrollBar,
ScrollList, Switch, Tab, Grid, Dialog, List, Menu, Tree, TextBox …). Half the theme
renders `Color.Empty`. This also undermines already-probed folders under Rustic.
Dedicated batch: fill ALL of them via a role-based mapping over Rustic's palette
(sienna/peru/goldenrod/beige/tan, dark-walnut ink) — the property-initialiser format
requires literals, and no ctor contrast-autofix runs for ThemeTypes themes.

### F6 — no probe

Planned (BarProbe): linear painter renders track+fill distinct at 30% vs 70%; a custom
ProgressColor survives a live theme change (F1 proof); live theme change re-renders;
2–3 other painter kinds render distinctly; state colours (Success/Error) apply; Rustic
renders non-Empty after F5 (the census check must fail before the fix). Eyeball.

## Order

1. F1–F4 code batch — build + commit
2. F5 Rustic wholesale — build + commit
3. F6 probe + eyeball — commit fixes

## Standing constraints

There is ALWAYS a theme — slot per role from the control's OWN family, customs as
Empty-passthrough resolved in getters, no flags/guards/reflection probes, no literal
palettes (outside theme part files), HC per paint. A check must be able to fail;
renders get eyeballed. Commit to master only.
