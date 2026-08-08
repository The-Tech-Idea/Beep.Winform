# Tabs — review & enhancement plan (2026-08)

65 files, ~10,000 lines — the largest folder in the series (tenth). `BeepTabs : BaseControl`
(17 partials) + `BeepTabPage` + `BeepTabQuickSwitch` (MRU Ctrl+Tab switcher, live via
WorkspaceMru), hosts (`BeepTabHeaderHost` 9 partials + `BeepTabContentHost`), 15 helpers,
15 models, 7 style painters over `BaseTabPainter`/`ITabPainter` keyed by `TabStyle`
(Classic/Underline/Capsule/Minimal/Segmented/Card/Button).

This folder has PRIOR doctrine work: catches report-and-rethrow with an on-surface error +
`TabError` event, `TabFontHelpers` documents its removed swallows, and high contrast is
already the per-paint seam in `TabThemeHelpers` (a deleted parallel HC paint path is
documented there). What remains is the colour anti-pattern and its workarounds.

## Findings (static pass)

### F1 — TabThemeHelpers: the anti-pattern, plus machinery that works around broken themes

`useThemeColors` ×28 — every call site passes `Theme != null`, so the flag never carried
intent; Empty-guard chains over every slot; Material-palette literal fallbacks
(`33,150,243` etc.); `ShiftLuminance` hover/selected derivations; and
`IsPerceptiblyDifferent` + strip-comparison fallback logic in `GetTabBackgroundColor` that
exists ONLY because some themes stamp `TabSelectedBackColor == TabBackColor` (see F2 —
fix the themes, delete the machinery). Same treatment for `TabIconHelpers` (Material
Blue/Gray literals) and `TabStyleHelpers.GetShadowColor`.

KEEP: the per-paint `IsHighContrast` branches (already the settled pattern), alpha veils
of resolved slots, WCAG brightness picks (`badgeFill.GetBrightness() > 0.55 ? Black :
White`).

Mapping (settled): control/page + header strip → `TabBackColor`; inactive text →
`TabForeColor`; hover → `TabHoverBackColor`/`TabHoverForeColor`/`TabHoverBorderColor`;
selected → `TabSelectedBackColor`/`TabSelectedForeColor`/`TabSelectedBorderColor`; plain
border → `TabBorderColor`; indicator/busy/dirty → `PrimaryColor`; badges → semantic slots
(already right). The theme family's `ActiveTab*`/`InactiveTab*` quartet stays unused —
the Selected/Hover triples cover every state this control has (decision recorded here).

### F2 — 11 bundled themes stamp TabSelectedBackColor == TabBackColor

Census (python, count-asserted): ArcLinux, Brutalist, ChatBubble, GNOME, iOS, KDE, Metro,
Neon, Nord, Solarized, Tokyo. A selected tab identical to its strip is the bug
`IsPerceptiblyDifferent` papered over. Fix in the themes (Switchs precedent): selected
trio → `PrimaryColor`/`OnPrimaryColor`(+`TabSelectedBorderColor = PrimaryColor`). The
ctor's `ThemeContrastHelper.ValidateTheme(autofix: true)` polices `*Fore` readability
after us — see the Switchs tracker for why fore slots are validator-owned.

### F3 — literal sweep outside the helpers

`BeepTabs.Appearance.ApplyTheme`'s `_currentTheme == null` branch assigns hardcoded
240,240,245/33,37,41 (there is always a theme — delete branch); `BeepTabQuickSwitch`
`?? Color.DodgerBlue`; `BeepTabs.Drawing` error overlay `?? Color.FromArgb(220,0,0)`;
`BeepTabHeaderHost.Painting` drag `markerPen = Pen(Color.Black)`;
`BeepTabFocusVisualHelper` white halo pen (focus idiom — audit, likely keep as the
two-tone focus ring standard).

### F4 — ReportError writes Debug.WriteLine directly

The report-and-rethrow shape is right; the transport is wrong. Route through
`BeepLog.Error` (keeps the `Reported` event for hosts) while keeping `TabError` + the
painted on-surface error. 0 swallows in the folder (2nd healthiest after Switchs).

### F5 — no probe

Planned (TabProbe): the 7 styles render distinctly; selected differs from unselected per
style; LIVE theme change re-renders (create control first — `ControlStyle` pins the
style's bundled theme, the Switchs lesson); click selects + raises `SelectedIndexChanged`;
keyboard nav; close-button hit; every render eyeballed.

## Order

1. F1 helpers rewrite + flag-less call sweep + F3 literals + F4 BeepLog — build + commit
2. F2 theme census fixes — build + commit
3. F5 probe + eyeball — commit per fix batch

## Batch 3 — probe + eyeball (TabProbe 21/21)

Probe: 7 styles render + selection visible per style + cross-style distinctness, live
theme change, click-select via the public ReceiveMouse* pipeline + SelectedIndexChanged,
ArrowRight via the keyboard command router, close click removes the tab + raises
TabRemoved. All renders eyeballed (including zoomed Zen/ArcLinux crops).

Fixes the probe forced:

1. **BeepTabs never followed global theme changes** — ContainerControl, not BaseControl,
   and it had no `ThemeChanged` subscription; `_currentTheme` also initialised from
   `GetDefaultTheme()` instead of the current theme. Now subscribes in the ctor (handler
   re-applies + invalidates), unsubscribes in Dispose, and the Theme setter falls back to
   the current theme when given an unknown name.
2. **Construction-time style morph** — the TabStyle setter unconditionally started a 220ms
   Classic→style cross-fade, so every form open replayed it (probe renders caught the
   blend: saturated fills + garbled captions). Transition now only starts when
   `IsHandleCreated`.
3. **Tab adornments remapped to the Tab family's accent line** — indicator, busy, dirty
   and the default badge kind now resolve `TabSelectedBorderColor`, not `PrimaryColor`:
   ZenTheme defines PrimaryColor as a neutral charcoal identical to its tab strip, so the
   selected-tab underline was invisible. Zen's palette is deliberate (Accent carries its
   green); the wrong assumption was ours.

Instrument notes: `TabCloseRequested` fires ONLY as the dirty-close guard (document modes,
dirty tab) — a plain close raises `TabRemoved`; the close-button check asserts removal.
`BeepTabHeaderItemLayout` has no Index — items matched by Bounds against `GetTabRect`.

**Second theme home discovered**: themes live in BOTH `Controls/Themes/` (Apply* parts,
censused and fixed in batch 2) and `Vis.Modules2.0/ThemeTypes/` (property-initialiser
parts, e.g. ZenTheme). The ThemeTypes population was NOT censused for Tab stamps — Zen's
Tab part is healthy; the rest are unaudited. Not verified: RTL header layout, overflow
dropdown, drag-reorder visuals, high-contrast branches (review only).

## Standing constraints

There is ALWAYS a theme — slot per role from the control's OWN slot family, no flag, no
guards, no luminance shifts (alpha veils of a resolved slot are fine; WCAG picks are an
accepted idiom). A wrong-looking colour is the THEME's bug, fixed in the theme parts. A
check must be able to fail; renders get eyeballed. Commit to master only.
