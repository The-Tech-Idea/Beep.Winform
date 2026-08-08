# Steppers — review & enhancement plan (2026-08)

43 files, ~8,000 lines. `BeepStepperBar : BaseControl` (in `BeepSteppperBar.cs` — note the
long-standing filename typo) + `BeepStepperBreadCrumb`, 14 style painters + NoOp over
`IStepperPainter` with a registry, helpers for theme/font/icon/style/accessibility, a
`StepperColorConfig` custom-override model.

Entry motive: the user reports **alignment issues in stepper styles** seen via the wizard forms —
this folder's painters are the likely source, so the probe leads with per-painter renders and
alignment checks, not just distinctness.

## Findings (static pass)

### F1 — `StepperThemeHelpers` has the full anti-pattern, third folder running

`useThemeColors` flag, `!= Color.Empty` guards, literal fallbacks — and worse than the previous two:
**reflection-based theme property lookup** (`GetProperty(...).GetValue(theme)`) per colour, per
paint. Same fix as Calendar/VerticalTables (the settled end-state): theme = supplied ??
`BeepThemesManager.CurrentTheme`, one slot one return, no flag, no guards, no reflection.

### F2 — ~46 literal colours

Helpers (17), painters (~23 across 10 files), `BeepSteppperBar` (3), `StepperColorConfig` (3 — check
whether these are custom-override *defaults*, which should be `Color.Empty`, not a palette).

### F3 — 4 empty swallows

`StepperAccessibilityHelpers` ×2, `StepperFontHelpers` ×1, `StepperThemeHelpers` ×1 → BeepLog per
site.

### F4 — no probe; alignment unverified (the user's actual complaint)

Planned probe (StepperProbe):
1. Render ALL 14 painters at two widths (wide + narrow) with 4 steps, one completed/current/pending/
   error; blank-guard + aliased-style distinctness.
2. **Alignment checks, mechanical**: for each painter, every drawn text/glyph pixel must lie inside
   the control bounds (no off-edge bleed — the wizard Cards/Horizontal lesson); node centres evenly
   spaced (max deviation ≤2px); label column x-alignment for vertical painters.
3. Theme responsiveness: one painter under two themes → different pixels.
4. State round-trip: CurrentStep set → StepChanged raised once; click a node → navigation (if the
   control supports it) via the real pipeline (cursor parked — the VerticalTables lesson).
5. Eyeball EVERY painter render individually — this is the complaint; distinctness checks alone
   don't see misalignment.

## Order

1. F3 swallows + F1 helpers rewrite (mechanical, settled pattern) — build + commit
2. F2 painter sweep to slots — build + commit
3. F4 probe; fix alignment defects it exposes painter by painter; render-eyeball all 14 — commit per
   fix batch
4. Filename typo `BeepSteppperBar.cs` → rename only if the user wants the churn (git mv, all refs)

## Standing constraints

There is ALWAYS a theme — assign slots directly, never guard, never blend, never literal (semantic
states from semantic slots). Text rects sized from fonts, not constants. A check must be able to
fail. Commit to master only.
