# Stage 05 — 38 painters, one chrome pipeline

## The gap

38 skin painters at 300–700 lines each — roughly **16k lines** — and each re-implements the same
chrome: caption layout, button glyphs, borders, shadows, title text. `FormPainterRenderHelper`
(875 lines) exists precisely because the duplication already hurt, but painters use it unevenly.
Consequences:

- **Parity is unverifiable.** Whether every skin draws the search box, honours
  `ShowCustomActionButton`, or scales its caption identically can only be established by reading
  38 files. The ListBoxs audit found exactly this shape hiding real bugs (empty background
  overrides, dead second lines, inverted mappings) — 38 larger files will be hiding more.
- **Every cross-cutting fix costs ×38.** Stage 03's focus ring, stage 04's pre-scaled metrics and
  stage 06's backdrop each either land in one pipeline or get hand-ported 38 times.
- The earlier session's swallow-sweeps and font fixes already touched these painters
  individually; each sweep was O(38).

## The work — skins describe, the pipeline draws

1. **`FormChromePipeline`** (internal): owns the paint *sequence* — backdrop → caption →
   buttons → title/search → borders → overlays — with layout coming from stage 02's geometry and
   metrics from stage 04. It draws through the theme; it contains **no skin-specific branches**.
2. **A skin becomes a description**: corner radii, border weight/style, caption height class,
   button glyph set, gradient/texture hooks, font choices — a `FormSkin` record plus optional
   override points (`DrawCaptionBackground`, `DrawButtonGlyph`, …) for the genuinely bespoke skins
   (Cartoon, ChatBubble, Holographic keep their character through overrides, not forks).
3. **Migrate incrementally, alphabetically, recording each** — the ListBoxs lesson, applied from
   the start: one tracker line per painter (what it overrides, what it lost, render before/after).
   The old painter is deleted the day its skin passes parity; no legacy path is kept beside the
   new one (rule 3).
4. **Parity gate:** a painter migrates only when its before/after renders are eyeballed and its
   differential is understood (deliberate simplifications listed, accidental losses fixed).

## Verification

- **Contact sheet of all 38 skins** (the ListBoxs instrument, reused): every skin × {active,
  inactive, maximized} at two sizes — eyeballed, before and after each migration.
- Feature-parity matrix by *pixels*: for each skin, toggling each `Show*` flag must change the
  render (differential per flag per skin — the blindness guard is a flag known to change nothing).
- Fence: migrated skins may not reference `FormPainterRenderHelper`'s deprecated entry points;
  count must reach zero by stage end.
- Break-it-first: comment one pipeline step (borders) and confirm every migrated skin's
  differential goes red at once — that single-point failure is the *point* of the pipeline.

## Exit criteria

All 38 skins on the pipeline or explicitly recorded as retired (with the user's sign-off —
deleting a shipped skin is a product decision, not a refactor); net line count and the per-painter
migration table recorded; contact sheets archived; fences green.
