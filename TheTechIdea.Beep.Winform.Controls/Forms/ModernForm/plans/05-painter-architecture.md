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

## Progress — ◐ the parity harness exists and has been run; no skin migrated yet

The instrument was built **before** any migration, deliberately: a rewrite of 18,793 lines with no
way to tell what it broke is not a refactor. `FormProbe` stage 05 (`FORMPROBE_PARITY=1`, watchdog
raised to 9 min for the sweep) renders every skin's caption band over magenta and compares
signatures.

Both blindness guards pass first, so the differentials below mean something: *the same skin and
flags render identically twice*, and *two different skins render differently*.

### Corrections to this document's own claims

- **"Whether every skin honours `ShowCustomActionButton` can only be established by reading 38
  files."** It can be established by pixels, and now is. But my first attempt to answer it by
  grepping `Show*` counts was the wrong instrument and overstated the problem badly: it reported
  that 13 skins ignore `ShowCloseButton`/`ShowMinMaxButtons`. They do not — the 12 painters that
  never mention those flags delegate layout to `FormPainterLayoutHelper`, which honours them. Every
  painter but the interface file handles every flag, via the helper or by hand.
- Likewise `FormHitAreaNames.Icon` appeared to be registered by **zero** painters. It is registered
  by **17**, as the string literal `"icon"`; `NormalizeName` maps it to the constant. Harmless, but
  it is how a typo becomes a silently unclickable region.

### What the harness found (measured, 37 skins × 5 flags = 185 combinations)

| feature | skins where toggling it changes **nothing on screen** |
|---|---|
| `ShowCustomActionButton` | **33 / 37** |
| `ShowThemeButton` | 1 / 37 (Fluent) |
| `ShowStyleButton` | 1 / 37 (Fluent) |
| `ShowProfileButton` | 0 / 37 |
| `ShowSearchBox` | 0 / 37 |

35 dead combinations in total; the full list is archived at `05-dead-flags.txt`.
`ShowCustomActionButton` is effectively a no-op property across the suite — it agrees with the
source (only 24 painters reference it, and only 23 register its hit area), but the render is what
makes it a defect rather than a stylistic difference.

### The contact sheet finding: ten skins are the same skin

`05-contact-sheet.png` — all 37 caption bands, eyeballed. **ArcLinux, Dracula, Solarized, OneDark,
GruvBox, Nord, Tokyo, Paper, Neon and Holographic all render the same generic blue caption**,
differing only in glyph shape. Dracula is supposed to be `ARGB(40,42,54)`; Nord slate; GruvBox
retro brown.

Mechanism, traced rather than guessed. `FormPainterMetrics.DefaultFor` has **two** colour switches:

```csharp
if ((theme != null) && (theme != m.beepTheme) && UseThemeColors)   // :606
{
    switch (style) { ... case FormStyle.Dracula: /* Dark theme, keep as-is */ break; ... }   // :633
}
else
{
    switch (style) { ... case FormStyle.Dracula: m.CaptionColor = ARGB(40,42,54); ... }      // :762
}
```

`UseThemeColors` **defaults to `true`** (`Core.cs:250`), so the first branch is the one that runs —
and for every dark skin its case is a comment and a `break`. The second switch, roughly **370 lines
of carefully authored per-skin palettes, is unreachable in the default configuration.** The skins
then inherit the theme's caption colour, which is blue, which is why ten of them are indistinguishable.

**This is a product decision, not a refactor, and is left for the user.** The repo's standing rule is
that there is always a theme and a wrong-looking colour is the theme's bug — under which deferring
is correct and the 370 lines should be deleted. But a "Dracula" skin that looks like every other
skin is not a skin, which argues the opposite. The two readings cannot both be implemented, so
nothing was changed.

### Not started

No painter has been migrated to a `FormChromePipeline`. The harness is the prerequisite, not the
work. `FormPainterLayoutHelper` adoption is **12 / 38** — the other 25 hand-roll the same
right-aligned caption layout including their own copies of every flag check, which is where the
drift will be.

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
