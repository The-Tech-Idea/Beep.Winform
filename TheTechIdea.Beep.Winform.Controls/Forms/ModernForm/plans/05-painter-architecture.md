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

## ~~The work — skins describe, the pipeline draws~~ SUPERSEDED

> **This section proposed moving the *drawing* into a shared `FormChromePipeline`, with skins reduced
> to a `FormSkin` description. Fifteen full painter reads say that is the wrong half.** Superseded by
> the section below; the original wording is kept because "the plan said X" gets repeated downstream.
>
> The original proposal: a `FormChromePipeline` owning the paint sequence, skins becoming records of
> radii, weights and glyph sets, with override points for the bespoke ones, migrated alphabetically
> behind a parity gate.

## The work — extract the layout, leave the drawing alone

Written from `05a-painter-read-log.md` — fifteen painters read end to end and one measurement.

**The drawing is genuinely different and is the product.** Arc hexagons, Brutalist blocks, macOS and
iOS traffic lights, Cartoon's outlined circles with offset shadows, Minimal's Zen enso rings, Metro's
3D tiles. Collapsing that into a pipeline with override hooks would either flatten the skins or
recreate them as overrides — the same code in a different file.

**The layout is not different at all.** Measured, comment-stripped, line by line:
`CalculateLayoutAndHitAreas` is **91–97% identical across 17 painters** — ArcLinux and Brutalist
differ by a single redundant `GetMetrics` call. That is ~1,300 lines that are one ~79-line method,
each copy carrying its own `Show*` checks, its own `buttonX -= buttonWidth` cascade, its own hit-area
registrations.

1. **Extend `FormPainterLayoutHelper`** to cover the hand-rolled cascade (it already covers the
   right-aligned and traffic-light variants used by 12 painters).
2. **Migrate the 17 measured duplicates first**, then the remainder, one per commit, `CalculateLayoutAndHitAreas`
   only. **No painter's drawing code is touched.**
3. **Position from the layout, never from literals.** This is the single decision most worth
   centralising — see below.
4. **Delete each painter's copy** as it migrates; no legacy path kept beside the new one.

### Why layout, specifically

- **It is where the bugs were.** `ShowCustomActionButton` was dead in 33 of 37 skins and took four
  separate fixes, because 25 painters each had to remember it and four different things went wrong in
  four different places. GNOME and Custom stayed broken longest simply by omitting one block.
- **It is where DPI lives.** Four helper-based painters passed raw literals; MacOS and iOS scaled
  nothing. Every one of those was a separate fix.
- **It is where corner-safety lives.** Only NextJS, iOS, MacOS, MaterialYou and Cartoon keep the icon
  clear of the corner arc or set `SafeContentInsets`. The other ten read do not.
- **Parity is already measurable.** The `Show*` flags live in layout, so the existing per-flag pixel
  differential *is* the gate — 2/185 dead today, and it must stay there.

### The one drawing rule worth centralising

Where a drawn button's position comes from has three different answers in this folder:

| source | painters |
|---|---|
| the layout rect | MacOS, iOS *(after this session)* |
| half layout, half recomputed | ArcLinux |
| local literals | Cartoon, Brutalist, Modern, Linear, Shadcn, RadixUI, NextJS, Minimal, … |

Only the first survives DPI scaling. The third made iOS's buttons **invisible but clickable**, and
would have desynchronised them again had the layout been scaled before the paint path was converted.
Painters keep their drawing; they should not keep their *positions*.

### What is explicitly not proposed

- No `FormSkin` record, no description-driven rendering, no override hooks.
- No change to any painter's colours — the painter-colour exemption stands, and six painters that
  discard their metrics entirely are recorded as findings, not as migration targets.
- No skin retired. That remains a product decision requiring sign-off.

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


### Ruling applied — skin wins (2026-08-13)

The user's call: a named skin imposes its own palette. Implemented by lifting the per-skin colour
switch out of the `else` of `if (theme != null && theme != m.beepTheme && UseThemeColors)` so it
runs unconditionally, after the theme colours are taken as the baseline. `FormStyle.Custom` is the
single exception and keeps deferring - it exists to follow the theme, and its case would otherwise
impose a fixed grey.

~370 lines that had never executed in the default configuration now do.

**This deliberately changes how most skins look**, which is the point of the ruling. Contact sheets
before and after are archived (`05-contact-sheet-BEFORE.png`, `05-contact-sheet.png`) and were
eyeballed side by side. The ten skins that were one skin are now ten skins: Dracula dark purple,
Solarized cream, GruvBox brown, Nord slate, OneDark charcoal, Tokyo navy, ArcLinux dark slate,
Neon black-on-magenta, Holographic a rainbow gradient, Paper light. Retro turned dark purple,
Cyberpunk gained its cyan scanlines, Brutalist its heavy black frame, and **Terminal is now black
with green text** instead of a blue caption.

One shift worth flagging for review rather than burying: **Metro2 went from amber to blue**. Blue is
Metro2's own palette, so it is correct under this ruling, but it is the largest single change to a
skin that already looked deliberate.

All 37 skins still render; both blindness guards and every stage 01-04 check stayed green.

**Unchanged by this ruling:** `ShowCustomActionButton` is still dead in 33 of 37 skins - that is a
layout defect, not a palette one, and belongs to the migration.

### A process note, paid for twice

The first run after this change produced a contact sheet **identical to the before sheet**, and the
change looked inert. It was not: two `FormProbe` processes from an earlier timed-out run were still
alive and holding the binary, so the rebuild could not overwrite it and the probe re-ran the old
code. This is the stale-binary trap recorded in `07-verification.md`, hit again from a different
direction - a timeout that kills the shell does not kill the child process. Kill the process first,
then rebuild, then trust the render.

## Ground truth — all 38 painters audited with comments stripped (2026-08-13)

Written after three wrong conclusions in a row, every one of them caused by grepping source that
included **comments**. The audit script strips block comments, line comments and string literals
before matching, and reports per painter. Numbers below replace every earlier count in this
document and in my working notes.

### There are two architectures, not one mess

| | painters | lays out | caption height |
|---|---|---|---|
| **helper-based** | **12** — Linear, MacOS, Material, MaterialYou, Metro, Metro2, Minimal, Modern, NextJS, RadixUI, Shadcn, iOS | `FormPainterLayoutHelper` does the button row; the painter positions only the icon | `owner.Font.Height + <literal>` |
| **hand-rolled** | **25** | all nine regions itself, in its own cascade | `Math.Max(metrics.CaptionHeight, Font.Height * metrics.FontHeightMultiplier)` |

That split is the real shape of the folder, and it is cleaner than "38 near-duplicates" implies.
The duplication is 25 copies of one cascade, not 38 copies of everything.

### Corrections to claims made earlier in this work

- **`PaintBuiltInCaptionElements` is called by 2 painters, not 25.** MacOS and NeoMorphism. The other
  matches are the comment `// NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) here because we
  paint custom beveled buttons`. This single miscount produced three failed fixes for
  `ShowCustomActionButton` before the parity harness forced the issue.
- **Stage 04's caption-height figures were right; a later audit of mine was wrong.** The 21 painters
  recorded there as metrics-driven use
  `Math.Max(metrics.CaptionHeight, Font.Height * metrics.FontHeightMultiplier)` — both terms scale,
  so they are the best-behaved group. A stricter regex in the new audit failed to match the
  `Math.Max` form and briefly suggested only one painter used metrics. It does not; **stage 04
  stands unamended.**
- **"Laid out but never painted" is an artefact of the detector, not a defect.** The audit counts a
  region as painted only when a painter invokes `X?.OnPaint`. Close, Maximize, Minimize, Theme,
  Style, Search and Icon show `painted 0` because painters draw those with their own code
  (`PaintTileFlipButton` and friends) rather than through the region. They render correctly. Only
  Profile and CustomAction are drawn *through* regions, which is why those two are the ones that
  could silently go missing.

### The remaining two dead skins, precisely

`GNOME` and `Custom` lay out **every** region except `CustomAction` — visible directly in the audit,
where their laid-out list is the only pair missing it. Both now invoke the region, so they draw into
an empty rect. Neither uses the shared helper, so there is no single place to fix it: each needs the
slot added to its own cascade, between Theme and Profile, exactly as the other 23 hand-rolled
painters already do it.

### What this means for the pipeline

The migration target should be **the hand-rolled cascade**, not the painters wholesale. 25 painters
repeat the same ordered sequence — close, maximize, minimize, style, theme, custom action, profile,
search — each with its own `if (owner.ShowX)` and its own `buttonX -= buttonWidth`. That sequence is
already expressed once, correctly, in `FormPainterLayoutHelper`, and 12 painters use it. Migrating
the other 25 to the same helper is a smaller, better-defined job than inventing a `FormChromePipeline`,
and it would have made `ShowCustomActionButton` a one-line fix instead of a 35-file one.

**Revised recommendation:** the first migration step is to move the 25 hand-rolled painters onto
`FormPainterLayoutHelper` for *layout only*, leaving every painter's drawing untouched. Layout is
where the flags live, so parity is measurable by the existing per-flag differential, and no skin's
appearance should change — which the contact sheet can prove.

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
