# Ratings — review & enhancement plan (2026-08)

28 files, ~5,500 lines. `BeepStarRating : BaseControl` with 14 `RatingStyle` values over 17
painters (`IRatingPainter` + factory + shared base + context), helpers for
theme/font/icon/accessibility. Sixth folder in the review series.

## Findings (static pass)

### F1 — the control NEVER follows the theme

The chain: painters ← `RatingPainterContext` ← control fields (`_filledStarColor = Color.Gold`,
`_emptyStarColor = Color.Gray`) ← `RatingThemeHelpers.ApplyThemeColors` — which passes the
control's own current values as `customColor`. `HasValue` is always true for a non-nullable
`Color`, so every getter returns its input and the assignment is a no-op (the BreadCrumbs inert
shape). Result: **Gold/Gray literals forever, on every theme**. The theme even HAS a dedicated
10-slot `StarRating*` family — which the helpers probe by REFLECTION (`GetProperty(
"StarRatingFillColor")` ×4) despite the properties sitting right on `IBeepTheme`.

Fix (settled): helpers slot-direct `(theme, style, customColor)` — Empty falls through to the
slot; style-semantic mapping stays but from semantic slots (Heart→ErrorColor, Thumb/Circle→
PrimaryColor, else `StarRatingFillColor`); empty→`StarRatingBackColor`, hover→
`StarRatingHoverForeColor`, border→`StarRatingBorderColor`, label→`StarRatingForeColor`.
Control fields default to `Color.Empty` (explicit caller override = data) and the painter
context resolves per paint through the helpers. Delete the inert `ApplyThemeColors`,
`ShiftLuminance`, the flag, and the reflection.

### F2 — high-contrast stamping copy (the Steppers batch-4 disease, 4th appearance)

`RatingAccessibilityHelpers.GetHighContrastColors` tuple + `ApplyHighContrastAdjustments`
stamping into control fields from ApplyTheme. Fix: HC branch per paint inside the theme
helpers; delete the stamping machinery.

### F3 — 3 swallows, 0 BeepLog

`BeepStarRating.cs:1634` (conversion catch → ignore), `RatingFontHelpers.cs:118,162`
(font-creation fallbacks, silent).

### F4 — ~62 literals

Helpers (style-default palettes: Gold/Pink/Blue, gray 200s) die with F1; painters carry glow/
shadow/gradient locals — classify alpha-of-slot (keep) vs derived/hardcoded (fix) in batch 2.

### F5 — no probe

Planned (RatingProbe): all 14 styles render at value 3.5/5 (half-star where supported) wide +
narrow, blank-guard + cross-style distinctness; theme responsiveness (**must fail before F1,
pass after** — the check that proves the fix); click sets rating + events; keyboard; hover
state; every render eyeballed.

## Order

1. F1 + F2 + F3 (one coherent change: helpers rewrite, per-paint resolution, HC per paint,
   swallows) — build + commit
2. F4 painter literal sweep — build + commit
3. F5 probe + geometry eyeball — commit per fix batch

## Batch 1 done — the control follows the theme for the first time (build 0 errors)

The instrument was broken first, per the standing rule: a two-theme render comparison printed
**IDENTICAL across themes** before the fix and **DIFFERS** after — the check can fail and the
fix is proven.

F1: `RatingThemeHelpers` rewritten (311 → 54 lines): slot-direct from the real `StarRating*`
family (reflection deleted — the properties sit right on the interface), Heart→ErrorColor and
Thumb/Circle→PrimaryColor keep their semantic identity, HC per paint. Control colour fields
default `Color.Empty` (= the theme decides; explicit set = override) and BOTH painter-context
builds resolve per paint through the helpers — every painter themed uniformly with zero painter
colour edits. Inert `ApplyThemeColors`, `GetThemeColors` tuple, `ShiftLuminance`, and the flag
(threaded through context + 5 painters + icon helpers) deleted.

F2: a11y HC region (tuple getters + field stamping) + dead WCAG luminance chain deleted
(~6,900 chars, zero callers after F1).

F3: all 3 swallows report (conversion WarnOnce, font fallbacks FallbackOnce ×2).

Not verified yet: per-style renders (batch 3).

## Batch 2 done — literal sweep (commit bad4c815)

On-fill ink + secondary text became resolved context roles (`StarRatingSelectedForeColor`,
`SecondaryTextColor`); glows/gradients are alpha-only veils of the resolved fill (GradientStar
keeps its radial identity through alpha rings); colour grade defaults ErrorColor→SuccessColor.
Kept, documented: the white specular spot highlights — lighting, not palette.

## Batch 3 done — probe 20/20, three defects found

RateProbe (scratchpad): 14 styles rendered + eyeballed, cross-style distinctness, half-star
2.5 ≠ full 3, theme responsiveness as a regression gate, full-star and half-star click
round-trips through the real handlers.

- **Emoji rendered BLANK (1 colour)**: the draw-as-text guard required `iconPath.Length == 1
  && char.IsSurrogate(...)` — impossible; emoji are surrogate PAIRS (length 2). Every emoji
  went to the SVG painter as a fake path and silently vanished. Guard fixed; the adjacent
  icon-paint swallow now reports (FallbackOnce).
- **Half-star clicks were silent**: the half-star branch wrote `_preciseRating`/`_selectedRating`
  fields directly, bypassing the setter — no RatingChanged (data binding never notified) and no
  repaint. Now raises + invalidates; probe holds it.
- **DefaultTheme's star palette was a copy-paste bug** (fixed in the THEME, per the standing
  rule): Material blue pasted into every `StarRating*BackColor` slot — empty stars rendered
  blue and looked selected; `SelectedForeColor` was Goldenrod — gold ink on the gold fill.
  Empty is now a muted neutral, on-fill ink a dark brown, hover/selected backs light gold
  tints. (Touches `TheTechIdea.Beep.Vis.Modules2.0` DefaultTheme parts.)

Not verified: HC branch (review-only, same caveat as prior folders); hover visuals asserted
only via slots, not cursor-driven renders; other themes' StarRating palettes not audited.

## Standing constraints

There is ALWAYS a theme — slot per role, no flag, no guards, no reflection, no
blends/luminance shifts (alpha veils of a slot are fine); semantic states from semantic slots.
A check must be able to fail — the theme-responsiveness check gets run BEFORE the fix to watch
it go red. Commit to master only.
