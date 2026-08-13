# Stage 05a — Read all 38 painters, in full, in stages

## Why this document exists

Three conclusions about these painters were stated confidently and were wrong, in one session:

1. *"13 skins ignore `ShowCloseButton`/`ShowMinMaxButtons`."* They do not — those painters delegate
   to `FormPainterLayoutHelper`, which honours the flags.
2. *"`FormHitAreaNames.Icon` is registered by zero painters."* It is registered by 17, as the string
   literal `"icon"`.
3. *"25 painters call `PaintBuiltInCaptionElements`."* **Two** do. The rest of the matches are the
   comment `// NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) here…`. This one miscount
   produced three failed fixes for `ShowCustomActionButton` before the parity harness settled it.

Every one came from pattern-matching source instead of reading it. A comment-stripped audit script
(`painter-audit.py`) fixes the crudest version of that, but it still only answers questions asked in
advance — and the first painter read in full produced five findings the script could not have asked
about, including a GDI handle leaked on every paint.

**Rule for this stage: no claim about a painter goes in this log unless the file was read end to
end.** Script output may direct attention; it may not be a finding.

## Method

- Read one painter completely, top to bottom, including the parts that look boilerplate — the leaked
  `Region` in Modern is inside a method whose first eight lines look ordinary.
- Record findings against the checklist below, in this file, one section per painter.
- Do **not** fix anything mid-read. Reading and changing at once is how the stage-05 session ended up
  with three abandoned fixes. Fixes are batched after each stage, with the parity harness re-run.
- Re-run `FORMPROBE_PARITY=1` after each stage's fixes; contact sheet eyeballed before and after.

## What to look for in each painter

Derived from what the first full read actually found, not from theory:

1. **Colour sources** — does it read `metrics.*`, or paint literals that make its metrics decorative?
   (Literals are permitted by the painter exemption; a skin that *ignores its own palette* is still
   worth recording, because it explains why a palette change does nothing.)
2. **Hardcoded geometry that the layout scales** — `GetCornerRadius` returning a constant,
   `buttonSize = 20` inside a paint method whose rect was DPI-scaled.
3. **Resource ownership** — `new Region`/`new GraphicsPath`/pens created and not disposed; `Clip`
   assigned and immediately discarded; `owner.BorderShape` correctly *not* disposed.
4. **Which regions it invokes vs draws itself** — only `Profile` and `CustomAction` are drawn through
   `FormRegion`; everything else each painter draws directly. A region it forgets is silently absent.
5. **Flag handling** — every `Show*` honoured in both layout and paint, and not gated on another
   button's flag.
6. **Duplicated helpers** — local copies of `CreateRoundedRectanglePath`, HSL converters, etc.
7. **Luminance shifts / blends**, which the standing colour rule excludes.
8. **Swallowed exceptions**, per hard rule 1.

## Stages

Grouped by architecture, because the two groups fail differently. 12 helper-based painters lay out
only the icon; 25 hand-roll the whole cascade.

| stage | painters | status |
|---|---|---|
| **05a-1** helper-based, part 1 | Modern, Linear, Material, MaterialYou, Metro | ☑ **5/5 read**; leak swept across all 38 |
| **05a-2** helper-based, part 2 | Metro2, Minimal, NextJS, RadixUI, Shadcn, iOS, MacOS | ◐ 1/7 read (Minimal) |
| **05a-3** hand-rolled, A–C | ArcLinux, Brutalist, Cartoon, ChatBubble, Custom, Cyberpunk | ☐ |
| **05a-4** hand-rolled, D–G | Dracula, Fluent, GNOME, Glass, Glassmorphism, GruvBox | ☐ |
| **05a-5** hand-rolled, H–N | Holographic, KDE, NeoMorphism, Neon, Nord, Nordic | ☐ |
| **05a-6** hand-rolled, O–S | OneDark, Paper, Retro, Solarized | ☐ |
| **05a-7** hand-rolled, T–U | Terminal, Tokyo, Ubuntu | ☐ |
| **05a-8** the shared code | `IFormPainter`, `FormPainterLayoutHelper`, `FormPainterRenderHelper`, `FormPainterMetrics` | ☐ |

Stage 05a-8 is last deliberately: after 37 painters, what the shared code *should* contain will be
obvious, and reading it first would bias every earlier read toward "this painter is fine, it matches
the helper".

## Findings

### Modern (439 lines, helper-based) — read in full

| # | finding | severity |
|---|---|---|
| M1 ☑ | **FIXED.** **`PaintWithEffects` leaks a GDI `Region` every paint.** Lines 289–291 build `new Region(path)`, assign it to `g.Clip`, then immediately restore `originalClip`. The assignment has no effect and the Region is never disposed. | **bug** |
| M2 ☑ | **FIXED** — now `GetMetrics(owner).BorderRadius`. **`GetCornerRadius` returns a hardcoded `new CornerRadius(8)`** and never reads `metrics.BorderRadius`, so the DPI-scaled radius from stage 04 never reaches this skin's corners — and neither does any theme or style setting. | **bug** |
| M3 ☑ | **FIXED** — the 20px literal is scaled once, painter-side, which is correct for its own constant. **Bevel geometry is unscaled while its layout is scaled.** `CalculateLayoutAndHitAreas` scales `buttonWidth` through `DpiScalingHelper`, but `PaintModernBeveledButtons` hardcodes `buttonSize = 20` and derives padding from it. At 150% the hit rect grows and the drawn button does not. | **bug** |
| M4 | **`PaintCaption` never reads `metrics.CaptionColor`** — it paints `FromArgb(250,250,250)`→`(245,245,245)` with a `(230,230,230)` divider. Permitted by the painter-colour exemption, but it explains why Modern looked identical before and after the "skin wins" change: its metrics are decorative. | record |
| M5 | **`ShiftLuminance` used for hover and border states** (lines 152, 178, 185), which the standing colour rule excludes. | record |
| M6 | Local copies of `CreateRoundedRectanglePath`, `ShiftLuminance`, `ColorToHsl`, `ColorFromHsl`, `Hue2Rgb` — ~70 lines duplicated per painter. | debt |

Nothing in M1–M3 is visible to the parity harness, which compares caption-band pixels between flag
states. They would not have been found by any check written so far — only by reading.

**M1–M3 fixed and verified (2026-08-13).** Full probe green (parity guards, stages 01–04, dead
combinations still 4/185), and caption height captured for all 37 styles before and after is
byte-identical — so the corner-radius change is inert at this machine's DPI, as intended, and now
follows the monitor. M4 (metrics ignored for caption colour) and M5 (`ShiftLuminance`) are left
recorded, not fixed: both are colour decisions covered by the painter exemption and should be taken
across all 38 skins at once, not one file at a time.

### Linear (438 lines, helper-based) — read in full

Linear carries **all three of Modern's bugs, identically**. That is the finding: M1–M3 are not
Modern's, they are the folder's.

| # | finding | severity |
|---|---|---|
| L1 ☑ | Same leaked `Region` + dead clip pair as M1. | **bug** |
| L2 | `GetCornerRadius` hardcoded to `new CornerRadius(4)`, same shape as M2. | **bug** |
| L3 | `buttonSize = 16` unscaled while its own layout scales through `DpiScalingHelper`, same as M3. | **bug** |
| L4 | `var metrics = GetMetrics(owner);` assigned and unused in `PaintBackground` and `PaintBorders` — both paint literals instead. | record |
| L5 | The `PaintLinearButtons` doc comment is duplicated verbatim, twice in a row. | tidy |
| L6 | Colours branch on `owner.CurrentTheme.IsDarkTheme` with literal light/dark pairs — a **third** colour source alongside metrics and the skin palette. | record |

### Material (499 lines, helper-based) — read in full

| # | finding | severity |
|---|---|---|
| Mt1 ☑ | **A `SolidBrush` constructed inside a nested loop and never disposed** — the style button's 2×2 grid icon leaked **four GDI handles per paint**, whenever that button is visible. Fixed: one `using` brush for all four dots. | **bug** |
| Mt2 | **No DPI scaling at all.** Unlike Modern and Linear, `CalculateLayoutAndHitAreas` passes raw literals to the layout helper — `Font.Height + 32`, button `32`, search `200`, padding `8` — and never touches `DpiScalingHelper`. Its icon does scale, because that reads `metrics.IconSize`. So one helper-based painter scales its button row and its neighbour does not. | **bug** |
| Mt3 | `touchTarget = 40`, `iconArea = 24`, `iconSize = 12` hardcoded in the paint path — the M3/L3 pattern again. | **bug** |
| Mt4 | `padding` is computed as `(captionRect.Height - touchTarget) / 2`, passed to `PaintMaterial3Button`, and never used there — the button centres on its own rect. Dead parameter; also negative whenever the caption is shorter than 40px. | tidy |
| Mt5 | `PaintBorders` computes `var radius = GetCornerRadius(owner);` and never uses it. | tidy |

**Material settles the corner-radius question, and the answer is: do not sweep.** Its
`GetCornerRadius` returns `CornerRadius(16)` (commented "MD3 spec: 16dp") while
`FormPainterMetrics` gives `FormStyle.Material` a `BorderRadius` of **8**. Replacing the literal
with the metric would halve Material's corners — a visible design change, not a refactor. The
painters and the metrics genuinely disagree about this skin, and which one is right is a design
decision per skin. Recorded as blocked on that decision rather than swept.

### Metro (349 lines, helper-based) — read in full

Read out of plan order (before MaterialYou) because it is the shortest remaining file in the batch
and context was tight — noted so the log's order is not mistaken for a finding.

| # | finding | severity |
|---|---|---|
| Me1 | **The declared shadow is never drawn.** `GetShadowEffect` returns a real shadow (alpha 30, blur 5, offsetY 2), but `PaintWithEffects` calls only `PaintBackground`, `PaintBorders`, `PaintCaption` — there is no `DrawShadow` call and no `DrawShadow` method in the file. Modern, Linear and Material all draw their own shadow in `PaintWithEffects`, so the convention is painter-drawn and Metro simply does not. The interface reports a shadow this skin never renders. | **bug** |
| Me2 | **No DPI scaling.** `Font.Height + 16`, button width `46`, `iconSize = 16`, `iconPadding = 12` — all raw literals, `DpiScalingHelper` never referenced. Same as Material. | **bug** |
| Me3 | `buttonSize = 22` hardcoded in the paint path while the layout is fixed too — the M3/L3/Mt3 pattern. | **bug** |
| Me4 | `ShiftLuminance` for hover fill and border (lines 144, 152), which the standing colour rule excludes. | record |
| Me5 | `padding` computed, passed to `Paint3DTileButton`, never used there — the same dead parameter as Material's Mt4. | tidy |
| Me6 | ~45 lines of duplicated HSL helpers (`ShiftLuminance`/`ColorToHsl`/`ColorFromHsl`/`Hue2Rgb`), byte-identical to Modern's copy. | debt |

**Me1 is not fixed.** Adding the shadow would change Metro's appearance, and a flat, shadowless
Metro is defensible as a design — Metro's whole comment block is about flatness and sharp edges. What
is not defensible is *declaring* a shadow that never draws. Either the skin should draw it or
`GetShadowEffect` should return `Inner = false, Blur = 0`. That is a design call, recorded here.

### Pattern across the four painters read so far

Every one has real resource or correctness bugs, and each has a **different** one — Modern leaked a
`Region`, Linear inherited it, Material leaks four brushes per paint, Metro declares a shadow it
never draws. None of the four is visible to the parity harness, which compares caption-band pixels
between flag states.

DPI is split down the middle of one supposedly uniform group: of four helper-based painters,
**Modern and Linear scale their layout literals; Material and Metro do not.**

### MaterialYou (411 lines, helper-based) — read in full

| # | finding | severity |
|---|---|---|
| MY1 ☑ | **Material's brush leak, byte-identical and unfixed.** The same 2×2 grid-icon loop constructing a `SolidBrush` per dot and disposing none — four GDI handles per paint. Fixing Material's copy left this twin broken, which is the ×N tax in one line. Fixed; a repo-wide check found no third copy. | **bug** |
| MY2 | `PaintBorders` **and** `PaintNonClientBorder` are both empty bodies with "intentionally left minimal" comments — Material 3 uses elevation instead of borders. Defensible as design, but it means two interface methods do nothing and only a comment says so on purpose. | record |
| MY3 | **`Color.Empty` guards** — `if (owner.CurrentTheme.SurfaceColor != Color.Empty)` and the same for `PrimaryColor`, which the standing rules forbid. | record |
| MY4 | `touchTarget = 40`, `iconArea = 24`, `iconSize = 12` hardcoded in the paint path while the layout *does* scale — the M3/L3/Mt3/Me3 pattern, fifth occurrence. | **bug** |
| MY5 | The dead `padding` parameter, third occurrence (Material, Metro, here). | tidy |
| MY6 | **Only painter of the five that sets `layout.SafeContentInsets`**, keeping child controls clear of rounded corners. The other four leave it default, so a child can overlap the corner arc in every other skin. | **bug** (elsewhere) |

## Stage 05a-1 complete — 5 of 38 read

| painter | scales layout for DPI | distinct defect found |
|---|---|---|
| Modern | yes | leaked `Region` per paint; hardcoded corner radius |
| Linear | yes | same leak inherited; same hardcoded radius |
| Material | **no** | four brushes leaked per paint; radius literal ≠ its metric (16 vs 8) |
| Metro | **no** | declares a shadow it never draws |
| MaterialYou | yes | Material's brush leak, cloned and unfixed; only painter setting `SafeContentInsets` |

Three of five scale their layout; two do not. Five of five hardcode paint-path button geometry. Every
one had a real defect, and **no two had the same list** — the closest were Material and MaterialYou,
where the shared defect existed precisely because the code was copied.

**None of these defects is visible to the parity harness**, which compares caption-band pixels
between flag states. Five for five, reading found what measurement could not.

### Swept across all 38 (2026-08-13)

Once Linear showed M1 was systemic, the leak was measured rather than assumed:

- **31 painters** carried a `g.Clip = new Region(path); g.Clip = originalClip;` pair — a GDI handle
  leaked on **every paint**, in almost every skin.
- **24** had the dead form, where the second line discards the first before anything draws under it.
  The clip never took effect, so those lines were removed outright.
- **7** — Cartoon, ChatBubble, Fluent, Glass, MacOS, Material, Minimal — are genuinely different:
  the clip is set, `PaintBackgroundEffects` draws under it, then the clip is reset. There the fix is
  ownership, not removal: `using var clipRegion = new Region(path);`. `Graphics.Clip` copies the
  region, so the caller owns the original.

**A blanket regex would have deleted the clip in those 7 and broken their rendering.** The pattern
only looked uniform; reading two files and then checking the residue is what separated them.

- **35 painters hardcode `GetCornerRadius`**; exactly one now reads `metrics.BorderRadius` (Modern).
  Not swept: unlike the leak, changing a painter's radius literal to its style's metric **can change
  appearance** wherever the two disagree, so it needs a per-style comparison first. Recorded, not done.

Verified after the sweep: probe green, dead combinations still 4/185, caption heights byte-identical
across all 37 styles.

**A fourth instrument error, same root cause as the other three.** The post-sweep check reported the
pattern still present in 31 files — it was matching the replacement *comment*, which quotes the code
it replaced. Comment-stripped, the true residue was 7, and those 7 turned out to be the legitimate
cases. Grepping source that includes comments has now produced four wrong readings in one session.

### Minimal (400 lines, helper-based) — read in full  [stage 05a-2 begins]

| # | finding | severity |
|---|---|---|
| Mi1 ☑ | No DPI scaling — `Font.Height + 16`, button `32`, `iconSize = 16`, `iconPadding = 8`. Fixed now that the file has been read; `searchBoxWidth`/`Padding` come from metrics and were deliberately left alone. | **bug** |
| Mi2 | **`PaintBackgroundEffects` is an empty no-op**, and `PaintWithEffects` builds a path, a `Region` and a clip purely to call it. The whole clip block wraps nothing. | **corrects an earlier claim — see below** |
| Mi3 | `PaintCaption` never fills a caption background; it draws only a 2px underline, so the caption shows whatever `PaintBackground` left. Deliberate for this skin and consistent with the contact sheet. | record |
| Mi4 | `buttonSize = 18` hardcoded in the paint path — sixth occurrence of the pattern. | **bug** |
| Mi5 | `padding` computed, passed to `PaintZenButton`, unused there — fourth occurrence. | tidy |
| Mi6 | `PaintBorders` computes `var radius = GetCornerRadius(owner);` and never uses it — same as Material's Mt5. | tidy |
| Mi7 | `GetCornerRadius` hardcoded to `6`. | **bug** |

**Mi2 corrects something I wrote two commits ago.** The GDI-leak sweep split 31 painters into "24
dead clips, removed" and "7 where the clip does real work, so fix ownership instead". Minimal was
put in the second group on the strength of its structure — `using var clipRegion`, then
`PaintBackgroundEffects`, then reset. Reading the file shows `PaintBackgroundEffects` is
`// No-op for Minimal Style` and nothing else. **Minimal belongs in the first group**; its clip
wraps a method that does nothing.

The `using` I added is still correct — it leaks nothing either way — so no harm was done, but the
classification was reached by pattern and not by reading, which is the mistake this whole stage
exists to stop making. The other six in that group have not been re-checked; each needs its
`PaintBackgroundEffects` read before its classification is trusted.

## DPI sweep, bounded by what has been read (2026-08-13)

Of the ten painters that use `TryBuildStandardRightAlignedCaptionLayout`, **four pass raw literals**
and never touch `DpiScalingHelper`: Material, Metro, Metro2, Minimal. The other six scale.

**Only Material and Metro were fixed** — the two of the four that have been read end to end. Metro2
and Minimal are left for stage 05a-2, because this sweep cannot be done blind:

> Metro passes `metrics.SearchBoxWidth` and `metrics.SearchBoxPadding` straight into the helper.
> Those come from metrics, which have been DPI-scaled since stage 04. Wrapping them in `ScaleValue`
> alongside the literals would apply the factor **twice** — precisely the defect the double-scale
> fence exists to catch, introduced by the fix meant to prevent it.

Distinguishing "this painter's own constant" from "a value that arrived pre-scaled" requires knowing
which is which, and that requires reading the file. A regex over the four would have double-scaled
Metro's search box.

Verified: probe green, dead combinations still 4/185, caption heights byte-identical across all 37
styles — the change is inert at this machine's DPI by construction and now follows the monitor.

## Exit criteria

All 38 files read end to end and recorded here; findings triaged into fix batches; the parity harness
and contact sheet re-run after each batch; and this log's per-painter sections complete enough that
the pipeline design in `05-painter-architecture.md` is written from evidence rather than from the
shape the folder appears to have.
