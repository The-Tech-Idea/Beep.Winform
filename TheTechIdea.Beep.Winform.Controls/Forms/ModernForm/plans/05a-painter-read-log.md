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
| **05a-2** helper-based, part 2 | Metro2, Minimal, NextJS, RadixUI, Shadcn, iOS, MacOS | ☑ **6/7 read** (Minimal, Shadcn, RadixUI, NextJS, iOS, MacOS); Metro2 partial (260–426) |
| **05a-3** hand-rolled, A–C | ArcLinux, Brutalist, Cartoon, ChatBubble, Custom, Cyberpunk | ◐ 3/6 read (ArcLinux, Brutalist, Cartoon) |
| **05a-4** hand-rolled, D–G | Dracula, Fluent, GNOME, Glass, Glassmorphism, GruvBox | ◐ 1/6 read (Fluent, taken early) |
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

### Metro2 — **NOT fully read** (lines 260–426 only)

Recorded as incomplete rather than filed as a finding, because the log's rule is that nothing is a
finding unless the file was read end to end. Lines 1–259 are unread.

What *was* read includes `CalculateLayoutAndHitAreas` in full, and on that basis alone the queued
DPI fix was applied — `Font.Height + 20`, button `48`, `iconSize = 18`, `iconPadding = 12` are now
scaled, while `searchBoxWidth`/`Padding` are left alone because they come from metrics. The method
is visible in its entirety, so the "which value arrived pre-scaled" question that blocked this fix
is answered for this file.

**Still owed for Metro2:** lines 1–259 — `PaintBackground`, `PaintCaption`, the tile-button drawing,
and whatever `DrawShadow` at 260 belongs to. Its `GetCornerRadius` is hardcoded to `0` and it carries
the same duplicated HSL block, both seen in the part that was read.

**All four unscaled helper-based painters are now fixed** (Material, Metro, Minimal, Metro2), so of
the ten painters using the shared caption layout, **ten scale their own literals**.

### Shadcn (359 lines, helper-based) — read in full

| # | finding | severity |
|---|---|---|
| S1 | **Metrics are fetched and thrown away.** `PaintBackground` and `PaintBorders` both open with `var metrics = GetMetrics(owner);` and then never use it — they paint `FromArgb(250,250,250)`, `FromArgb(255,255,255)` and `FromArgb(229,229,229)` literals. `PaintNonClientBorder` does the same without even fetching. The only metric this skin honours anywhere is `metrics.CaptionTextColor` for the title. | **record — and see below** |
| S2 | `buttonSize = 18` hardcoded in the paint path — seventh occurrence. | **bug** |
| S3 | `GetCornerRadius` hardcoded to `8`. | **bug** |
| S4 | The dead `padding` parameter — fifth occurrence. | tidy |
| S5 | Its own copy of `CreateRoundedRectanglePath`. | debt |
| S6 | Does scale its layout literals. | ok |

**S1 explains something that has been puzzling this work all session.** When the skin palettes were
made reachable ("skin wins"), the contact sheet showed ten skins gaining their real identity — and
several others, Shadcn and Modern among them, not changing at all. The reason is not that the
palette failed to apply: it applied, and the painter ignores it. Shadcn asks for its metrics twice
and discards the answer both times.

So there is a **third colour source** beyond metrics and the theme: painter literals that override
everything. The painter-colour exemption permits them, but their consequence is that for these
skins, `FormPainterMetrics`, `UseThemeColors` and the skin palettes are all decorative.

**How many painters do this is not yet known and must not be guessed** — a grep for `FromArgb` will
find the legitimate cases too (alpha veils, gradients, state layers). It needs the per-file read this
stage is doing. Three of the seven read so far — Modern, Linear, Shadcn — ignore metrics for their
caption fill; four honour them.

### RadixUI (419 lines, helper-based) — read in full

| # | finding | severity |
|---|---|---|
| R1 | **The "focus ring" is a hover ring.** The class doc sells this skin as "accessible, high-contrast" with "clear focus states" and "prominent focus rings on buttons". The ring is drawn at `focusAlpha = isHovered ? 60 : 30` — always painted, varying only with the mouse. Nothing in the file consults keyboard focus; `KeyboardFocusedCaptionArea` (added in stage 03) is never read. A keyboard user gets no focus indication from this skin's buttons at all. | **bug** |
| R2 | Metrics fetched and discarded in `PaintBackground` and `PaintBorders`; only `CaptionTextColor` is honoured. Same as Shadcn — fourth painter whose metrics are decorative. | record |
| R3 | `buttonSize = 20` hardcoded in the paint path — eighth occurrence. | **bug** |
| R4 | `GetCornerRadius` hardcoded to `6`. | **bug** |
| R5 | The dead `padding` parameter — sixth occurrence. | tidy |
| R6 | `ShiftLuminance` for the hover colour, which the standing colour rule excludes. | record |
| R7 | Own copies of `CreateRoundedRectanglePath` and the four HSL helpers. | debt |

**R1 is worth separating from the pile.** The form *does* paint a real keyboard focus indicator
centrally (`PaintKeyboardCaptionFocusIndicator`, confirmed by the stage 03 renders), so keyboard
focus is not invisible overall. But this skin advertises focus rings as its distinguishing feature
and ships a hover effect wearing that name. Anyone choosing RadixUI *for* its accessibility gets
the same focus story as every other skin, plus a decorative ring that misleads a reader of the code.

Whether to make it real — read `KeyboardFocusedCaptionArea` and key the ring off it — or to rename
it honestly is a design call, recorded rather than taken.

### NextJS (471 lines, helper-based) — read in full

The most careful painter read so far, and the first with findings that are *good*.

| # | finding | severity |
|---|---|---|
| N1 | **Keeps the icon clear of the corner arc.** Uses `FormPainterMetrics.GetCaptionLeftSafeX` to push the icon inward when the top-left radius would clip it. No other painter read so far does this — the other eight place the icon at a fixed padding and let the arc cut it. | good / **bug elsewhere** |
| N2 | **Sets `layout.SafeContentInsets`** — second of nine to do so, after MaterialYou. Seven do not, so child controls can overlap the rounded corners in those skins. | good / **bug elsewhere** |
| N3 | **The duplicated HSL helpers have drifted.** NextJS names it `HueToRgb` and writes `1.0f / 3.0f`; Modern, Metro and RadixUI name it `Hue2Rgb` with `1f / 3f`. Same algorithm, divergent copies — proof that the duplication is not merely wasteful but already inconsistent. | debt |
| N4 | Metrics fetched and discarded in `PaintBackground` and `PaintBorders`; only `CaptionTextColor` honoured. **Fifth** painter. | record |
| N5 | `buttonSize = 20` hardcoded in the paint path — ninth occurrence. | **bug** |
| N6 | `GetCornerRadius` hardcoded to `10`. | **bug** |
| N7 | The dead `padding` parameter — seventh occurrence. | tidy |
| N8 | `ShiftLuminance` for hover. | record |

**N1 and N2 invert the usual finding.** Everywhere else the pattern has been "this painter is missing
something the others have". Here two painters (NextJS, MaterialYou) do corner-safety work the other
seven omit — so the defect is in the majority, and it is only visible because a careful minority
exists to compare against. Reading the careful ones is what makes the careless ones legible.

### iOS (461 lines, helper-based) — read in full

| # | finding | severity |
|---|---|---|
| I1 ☑ | **The window buttons were invisible but clickable.** `PaintiOSButtons` draws the red/yellow/green traffic lights and was **never called** — not from `PaintCaption`, not from `PaintWithEffects`. Meanwhile `CalculateLayoutAndHitAreas` lays all three out through `TryBuildTrafficLightCaptionLayout` and registers their hit areas. So close, minimize and maximize responded to clicks while rendering nothing. Fixed by calling it. | **bug — user-visible** |
| I2 | The comment where the call belonged read *"iOS uses navigation bar style — no window buttons. System buttons painted below."* Nothing below painted them. The comment described an intention the code did not carry out, and it is why the gap survived review. | record |
| I3 | Corner-safe icon placement **and** `SafeContentInsets` — the second painter after NextJS to do both. | good |
| I4 | `HueToRgb` naming, matching NextJS and differing from Modern/Metro/RadixUI's `Hue2Rgb` — the drifted-copy finding again. | debt |
| I5 | `buttonSize = 12`, `buttonSpacing = 8`, `startX = 10` hardcoded in the paint path — tenth occurrence. They happen to match the layout's own literals, so the drawn lights land on their hit areas; nothing enforces that they stay matched. | **bug** |

**I1 was visible in the stage 05 contact sheet from the very first run** — iOS was the one skin with
a bare centred title and no buttons — and it was not recognised as a defect for six commits. The
sheet was read as "iOS is minimal by design" because the code said so in a comment. **A render
showing an absence only becomes a finding once you know what should have been there.** Confirmed
fixed by re-rendering: the traffic lights are now present.

### MacOS (468 lines, helper-based) — read in full  **[stage 05a-2 complete]**

The most carefully written painter of the eleven read, and the one that shows what the others should
look like.

| # | finding | severity |
|---|---|---|
| Ma1 | **Draws its buttons from the layout, not from literals.** `DrawTrafficLights` reads `layout.CloseButtonRect` etc. and `DrawTrafficLightInRect` centres each circle inside the *registered hit rect*, with a comment saying exactly why: "to stay consistent with registered hit areas". This is the direct fix for the class of bug iOS had — iOS recomputed `startX = 10, buttonSize = 12` in its paint path and only coincidentally matched its own layout. | **good — the reference pattern** |
| Ma2 | Corner-safe on **both** sides (`safeLeftX`, `safeRightX`), icon placed after `leftBoundary`, plus `SafeContentInsets`. The most thorough corner handling of any painter read. | good |
| Ma3 | One of only **two** painters that call `PaintBuiltInCaptionElements`, and it documents the ordering (icon before title so the text is not painted under it). | good |
| Ma4 | **No DPI scaling** — `Font.Height + 16`, `buttonSize = 12`, `buttonSpacing = 8`, `iconSize = 16`, `iconPadding = 8`, though it does take `AuxiliaryButtonWidth`, `SearchBoxWidth` and `SearchBoxPadding` from metrics. | **bug** |
| Ma5 | `PaintBackgroundEffects` does real work (translucent overlay + gradient), which **confirms** MacOS genuinely belongs in the seven-painter "clip does real work" group. One of the five unverified classifications is now verified by reading. | confirms |
| Ma6 | A stale comment: the layout says "avoid the rounded corner arc (radius=10 for macOS)" while `GetCornerRadius` returns `14`. | tidy |
| Ma7 | `Hue2Rgb` spelling, against iOS's `HueToRgb` — the drifted copies again. | debt |

**Ma4 corrects a claim I made two commits ago.** I wrote that "all four unscaled helper-based
painters are now fixed" and "ten of ten now scale their own literals". That scan only looked at
painters calling `TryBuildStandardRightAlignedCaptionLayout`. **MacOS and iOS use
`TryBuildTrafficLightCaptionLayout` instead**, and neither scales — so the correct figure is ten of
**twelve**, with two still unscaled. Both have now been read in full, so fixing them is unblocked;
it is the next action rather than a guess.

### Cartoon (514 lines, hand-rolled) — read in full  **[stage 05a-3 begins]**

| # | finding | severity |
|---|---|---|
| Ca1 ☑ | **FIXED** — one tiled fill, using Fluent's cached-`TextureBrush` pattern. **The halftone texture is a per-paint performance bug.** `PaintBackground` runs a nested loop over the whole client area at 8px spacing, calling `g.FillEllipse(dotBrush, x, y, 1, 1)` for each. On a 1000×700 form that is **~11,000 FillEllipse calls on every repaint** — and a 1×1 `FillEllipse` costs far more than a pixel write. Nothing caches it. | **bug — performance** |
| Ca2 | **`PaintBackgroundEffects` is an empty no-op**, so Cartoon belongs in the "dead clip" group, not the seven I classified as doing real work. **Second misclassification confirmed** (Minimal was the first). | **corrects my own sweep** |
| Ca3 | `buttonSize = 24` and `padding` in `PaintCartoonButtons` are **dead locals** — `PaintCartoonButton` takes neither and hardcodes `size = 20`. So the caller computes a size the drawing ignores. | tidy |
| Ca4 | Corner-safe icon placement **and** `SafeContentInsets` — the fifth painter to do this, and the first hand-rolled one. | good |
| Ca5 | No DPI scaling (`Font.Height + 20`, button `32`, `iconSize = 16`), though the search box comes from metrics. | **bug** |
| Ca6 | `new Random(123)` allocated per paint in `DrawComicSpeedLines` (seeded, so the pattern is at least stable). | tidy |
| Ca7 | The caption is drawn with `DrawRectangle`/`FillRectangle` — square corners — while `GetCornerRadius` returns `20`. The caption's corners do not match the form's. | record |

**Ca2 matters more than it looks.** My GDI-leak sweep split 31 painters into 24 "dead clip, removed"
and 7 "clip does real work, ownership fixed". Reading has now checked four of those seven:

| painter | classified | actually |
|---|---|---|
| Material | real work | **real work** ✓ |
| MacOS | real work | **real work** ✓ |
| Minimal | real work | **no-op** ✗ |
| Cartoon | real work | **no-op** ✗ |

**Half wrong so far.** No harm was done — a `using` on a Region that wraps nothing still leaks
nothing — but the classification was pattern-matching, and it is running at 50%. ChatBubble, Fluent
and Glass remain unchecked.

**Cartoon is the first hand-rolled painter read**, and it does honour every `Show*` flag in its own
cascade, exactly as the earlier grep correction predicted. Its failure profile is different from the
helper-based group: no metrics-discarding, but a real performance problem instead.

### Fluent (464 lines, hand-rolled) — read in full  *(read out of batch order; shortest of the three unverified clip classifications)*

| # | finding | severity |
|---|---|---|
| Fl1 | **Cartoon's performance fix already exists here.** `DrawAcrylicNoise` builds a 64×64 noise bitmap **once**, behind a double-checked lock with a seeded `Random`, wraps it in a `TextureBrush { WrapMode = Tile }` and caches both statically — then fills the whole client rect in a single call. Cartoon's halftone does the same *kind* of texture with ~11,000 `FillEllipse` calls **per paint**. The solution is one file away and was never copied. | **the duplication finding, at its sharpest** |
| Fl2 | `PaintBackgroundEffects` does real work (noise + gradient), so Fluent's clip classification was **correct**. Third of seven verified. | confirms |
| Fl3 | **The only painter of thirteen that draws its caption buttons through shared code** — `FormPainterRenderHelper.DrawSystemButton`, with metrics colours for normal and hover. Comments (`SA-02: Removed over-designed acrylic button rendering`) show a deliberate earlier cleanup did this. | **good — the target state** |
| Fl4 | That cleanup left the method named `PaintFluentAcrylicButtons` and the class doc still advertising "Acrylic background with noise texture" and "Fluent reveal highlight effects" for buttons that are now plain. The name and doc outlived the code. | tidy |
| Fl5 | `PaintBackground` fetches metrics and paints a literal `FromArgb(240,243,249,253)`. Sixth painter to discard its metrics. | record |
| Fl6 | No DPI scaling; no corner-safe icon; no `SafeContentInsets`. | **bug** |

**Fl1 and Fl3 together are the argument for stage 05.** Not "these files are similar", but: *one
painter already solved the texture-caching problem and another repaints 11,000 ellipses; one painter
already draws through the shared button helper and twelve hand-roll it.* The good implementations
exist, in this folder, and have not spread. A pipeline is how they spread.

**Clip classification scoreboard — 5 of 7 checked:**

| painter | classified | actually |
|---|---|---|
| Material, MacOS, Fluent | real work | **real work** ✓ ✓ ✓ |
| Minimal, Cartoon | real work | **no-op** ✗ ✗ |

ChatBubble and Glass remain unchecked.

## Cartoon's halftone fixed with Fluent's own pattern (2026-08-13)

The first fix in this stage that came from **another painter** rather than from first principles.

`CartoonFormPainter.PaintBackground` now builds an 8×8 tile carrying one `ARGB(20,0,0,0)` dot,
caches it and a `TextureBrush { WrapMode = Tile }` behind a double-checked lock, and fills the client
rectangle in a single call — the identical shape of `FluentFormPainter.DrawAcrylicNoise`. Same pitch,
same ink, so the texture is unchanged; ~11,000 `FillEllipse` calls per repaint become one
`FillRectangle`.

Verified: probe green, dead combinations 4/185, contact sheet re-rendered and eyeballed — Cartoon's
yellow caption, comic buttons and halftone all look as before.

**Worth noting for the pipeline design:** this fix needed no invention and no design decision. The
correct implementation was already in the folder, written by whoever did the `SA-02` cleanup, and it
had simply never reached the painter that needed it. That is what a shared pipeline buys — not
elegance, but *propagation*.

### Brutalist (429 lines, hand-rolled) — read in full

| # | finding | severity |
|---|---|---|
| Br1 ☑ | **A `SolidBrush` allocated inside a ternary and never disposed.** `g.FillRectangle(isHovered ? Brushes.White : new SolidBrush(iconColor), …)` — the false branch leaks a GDI handle on **every paint whenever the style button is visible and not hovered**, which is the common case. `Brushes.White` is a stock object that must *not* be disposed, so the two cannot share a `using`; the branches are now separated. A repo-wide scan found no second site. | **bug** |
| Br2 | **The most metrics-respecting painter read so far.** `BackgroundColor`, `CaptionColor`, `BorderColor`, `CaptionTextColor`, `ButtonWidth`, `IconSize`, `IconLeftPadding`, `TitleLeftPadding`, `ButtonSpacing`, `SearchBoxWidth`/`Padding` — all read from metrics, none hardcoded. | **good** |
| Br3 | `captionHeight = Math.Max(metrics.CaptionHeight, Font.Height * FontHeightMultiplier)` — both terms scale, so it is DPI-correct without ever calling `DpiScalingHelper`. | good |
| Br4 | `buttonSize = 20` and `padding` are dead locals; `PaintBrutalistButton` takes neither. Eighth dead `padding`. | tidy |
| Br5 | `PaintBorders` uses hardcoded `8`/`17` insets for its inner accent rectangle — unscaled. | tidy |

**Br2 and Br3 are the clearest evidence yet that the two architectures differ in quality, not just
in shape.** Of the helper-based painters read, **six of nine discard their metrics** and paint
literals. Of the hand-rolled painters read, Brutalist reads *everything* from metrics and Cartoon
reads most. The group that hand-rolls its layout is the group that respects the metrics system.

That inverts the assumption behind stage 05's plan, which treats the hand-rolled 25 as the debt and
the helper-based 12 as the healthy example to migrate toward. On this evidence the reverse is closer
to true: the helper-based painters delegate their *layout* and then ignore everything else.

### ArcLinux (517 lines, hand-rolled) — read in full

*(Read in two passes; the earlier note recording it as incomplete is superseded.)*

| # | finding | severity |
|---|---|---|
| Ar1 | **Half from the layout, half from its own arithmetic.** Horizontal centres come from the layout (`closeRect.X + closeRect.Width / 2`), but the vertical position is recomputed locally as `hexY = (captionRect.Height - hexSize) / 2`. This sits between MacOS (everything from the layout) and iOS (everything from literals) — a third variant of the same decision, in the same folder. | record |
| Ar2 | Reads `BackgroundColor`, `CaptionColor` and `CaptionTextColor` from metrics — consistent with Brutalist and the hand-rolled group generally. | good |
| Ar3 | `hexSize = 20`, `iconSize = 7`, `sqSize = 6` hardcoded in the paint path. | **bug** |
| Ar4 | The `PaintArcHexButtons` doc comment is duplicated verbatim, twice in a row — the same defect as Linear's L5, in an unrelated file. | tidy |
| Ar5 | `Color.FromArgb(Math.Max(0, metrics.BorderColor.A - 50), metrics.BorderColor)` — an alpha veil over a resolved slot, which the standing colour rules permit. | ok |

**Ar1 completes a set worth naming.** Across the painters read, "where does a button's drawn position
come from?" has three different answers:

| answer | painters |
|---|---|
| entirely from the layout | MacOS, iOS *(after this session's fix)* |
| horizontal from layout, vertical recomputed | ArcLinux |
| entirely from local literals | iOS *(before)*, Cartoon, Brutalist, Modern, Linear, … |

Only the first is safe under DPI scaling. The third is what made iOS's buttons invisible and what
would have desynchronised them again had the layout been scaled first. **This single decision — and
nothing about colour, style or line count — is the strongest candidate for what a pipeline should
take away from painters.**

## ShowCustomActionButton reaches zero (2026-08-13)

```
ShowCustomActionButton   33/37 skins dead  ->  0/37
TOTAL dead skin-feature combinations   35/185  ->  2/185
```

GNOME and Custom were the last two. Both honour `ShowCustomActionButton` in their *paint* path — the
sweep added that — and neither allocated a rect for it, so the paint call had an empty rectangle to
draw into. The block was missing from their layout cascade and present in 23 siblings, between the
Theme and Profile blocks. Added in the identical form.

**This is the fourth distinct cause found for one property not working**, and worth listing because
each masked the next:

1. No painter invoked the region at all (the accessor existed, unused) — 35 painters fixed.
2. The form's paint gate keyed off `!ShowThemeButton && !ShowStyleButton`, never its own flag.
3. Metro and Metro2 passed `includeCustomAction: false` — a second gate that let a painter veto the
   user's setting.
4. GNOME and Custom never allocated the slot.

Only the parity harness could show that (1) had not fixed it, that (2) changed nothing on its own,
and that (3) left exactly two skins behind. **Three of my four proposed fixes were wrong about the
cause, and the measurement caught each one.**

### The last two dead combinations, explained

Both are Fluent: `ShowThemeButton` and `ShowStyleButton`. From the full read of that file,
`PaintFluentAcrylicButtons` calls `FormPainterRenderHelper.DrawSystemButton` for **close, maximize
and minimize only** — it never draws the theme or style buttons, although its layout allocates and
registers them. The same shape as the iOS defect: laid out, hit-tested, never painted.

Not fixed here: adding two visible buttons to a skin is an appearance change, and Fluent is the one
painter that routes through the shared button renderer, so the right fix is to extend that call — a
change better made as part of the shared-layout extraction than as a one-off.

### Read in full during the migration pass

| painter | lines | notes |
|---|---|---|
| **Solarized** | 485 | Diamond buttons. Mixed `_hits.Register` and `_hits.RegisterHitArea` in one method — equivalent calls, the kind of drift that survives only in copies. |
| **GruvBox** | 503 | Win95 beveled buttons via `ControlPaint.DrawBorder3D`, Raised/Sunken on hover — the most distinctive drawing of any skin read. Warm grain texture is a `DrawLine` every 3px (~233 calls/paint): the same uncached-texture shape as Cartoon's halftone, two orders of magnitude cheaper, and fixable with Fluent's cached `TextureBrush` if it ever matters. |
| **OneDark** | 511 | Octagon buttons, and it **does** draw its theme and style octagons — unlike Fluent, every button its layout allocates is painted. Gutter grid is a nested loop of 1×1 `DrawRectangle` every 40px (~425 calls/paint), same family as the above. |

All three carried the identical layout cascade and were migrated in the same pass. Twenty of
thirty-eight painters read end to end.

**One instrument question raised and resolved while reading OneDark.** The per-flag differential
detects *"the render changed"*, and enabling a button also shifts its neighbours — so a painter could
in principle fail to draw the new button and still pass. It does not apply here: the cascade
allocates **right to left**, so enabling theme or style only moves items further left, and with
profile and search off nothing else moves. The two Fluent results are therefore genuine, and the
check is sound as written.

### KDE and Paper — dead button implementations, found by reading

Both files carry a **second, superseded button-painting method that nothing calls**:

| painter | live method | dead method | lines |
|---|---|---|---|
| KDE | `PaintKDEPlasmaButtons` | `PaintKDEButtons` | 67 |
| Paper | `PaintPaperTexturedButtons` | `PaintPaperCircleButtons` | 73 |

Paper's states the case itself, in its own doc comment:

```
/// Paint Material Design Paper double-border circle buttons (ORIGINAL - KEPT FOR COMPATIBILITY)
```

Both deleted. They are **private** methods, so nothing outside their file could ever have depended on
them, and this library carries no back-compatibility burden on internals. A second implementation
kept beside the live one is precisely what the no-legacy-paths rule exists to prevent: it compiles,
it reads as maintained, and it is never reached. Verified after deletion — 0/185 dead combinations,
and all 37 skins' rects match the archived baseline exactly.

**A method to note about how these were found.** A scan first flagged three candidates, and it was
wrong about one of them in the way that matters: it could not tell a superseded implementation from a
live one, nor from Ubuntu's `CreatePillPath`. Only reading KDE showed `PaintKDEPlasmaButtons` is the live
path; only reading Paper found the comment that justifies the deletion outright.

**Also seen in Paper while reading, recorded not fixed:** three `new Random(...)` allocations per
paint — `PaintBackground` (seeded on the client-rect hash), `DrawPaperTexture` and `DrawFiberPattern`
(seeded on each button's rect hash) — and `DrawPaperTexture` issues 50 `DrawRectangle` calls **per
button**, so a five-button caption costs ~250 plus fibers. Same uncached-texture family as Cartoon's
halftone, and the same cached-`TextureBrush` fix applies.

### Ubuntu (read in full) — a dead helper, and a comment of mine that was wrong

`CreatePillPath` (19 lines) is never called: `PaintUbuntuButtons` draws circles with `FillEllipse`,
and its own doc says **"(REDESIGNED - cleaner look)"** — the pill helper is the leftover from the
design it replaced. Deleted, on the same basis as KDE's and Paper's.

**Reading also caught a mistake I made.** The migration comment I templated into this file claimed it
"failed the earlier automated pass only because it used literals where the other seventeen used
metrics". That is false for Ubuntu: it already used `metrics.ButtonWidth` and the `Math.Max` caption
height. It was held back for a different reason — corner-safe icon placement and `SafeContentInsets`.
The comment came from the literals batch and was copied without being true here. Corrected.

That is worth recording as its own finding: **a batched edit wrote a false statement into a source
file, and only reading the file found it.** The build was clean, every check was green, and the
comment would have misled the next reader indefinitely. Three files got that same templated comment;
the other two were in the literals group, so it is accurate there — checked individually, not assumed.

### ChatBubble (544 lines) — read in full

| # | finding | severity |
|---|---|---|
| Cb1 | **`PaintBackgroundEffects` is an empty no-op**, so ChatBubble belongs in the dead-clip group, not the seven I classified as doing real work. **Third misclassification.** | corrects my own sweep |
| Cb2 | The splice I made left `var metrics = GetMetrics(owner);` mis-indented under a stale "Set caption rectangle" comment, and the templated migration note was vague about which literals held it back. Both corrected. | mine |
| Cb3 | `AddRoundedRect` **is** called — from `CreateSpeechBubblePath` at line 411. Not dead. A name-only scan would have flagged it. | — |
| Cb4 | `PaintBackground` draws diagonal stripes with a loop from `r.Top - r.Width` to `r.Bottom` stepping 24 — for a 740×200 caption-band capture that is ~39 `DrawLine` calls, but the loop is bounded by **width**, so it scales with the wider dimension regardless of height. | record |

**Clip classification scoreboard — 6 of 7 checked:**

| painter | classified | actually |
|---|---|---|
| Material, MacOS, Fluent | real work | **real work** ✓ ✓ ✓ |
| Minimal, Cartoon, ChatBubble | real work | **no-op** ✗ ✗ ✗ |

**Three right, three wrong.** Only Glass remains unchecked. The `using var clipRegion` stays in all
of them — it leaks nothing either way — but as a piece of classification it was a coin flip, which is
what pattern-matching buys.

## Measured: 17 painters share one layout method (2026-08-13)

ArcLinux's `CalculateLayoutAndHitAreas` looked identical to Brutalist's, so it was **measured rather
than asserted** — comment-stripped, whitespace-normalised, compared line by line:

```
ArcLinux layout: 80 lines, Brutalist layout: 79 lines, similarity 96.9%
the only difference:
   A-  var searchMetrics = GetMetrics(owner);            <- a second, redundant metrics fetch
   A-  int searchBoxWidth = searchMetrics.SearchBoxWidth;
   B+  int searchBoxWidth = metrics.SearchBoxWidth;
```

**One line.** Extending the comparison across the folder:

| similarity to Brutalist's layout | painters |
|---|---|
| 96.9% | ArcLinux, Cyberpunk, Dracula, Glassmorphism, GruvBox, NeoMorphism, OneDark, Terminal |
| 93.1% | KDE, Neon, Retro, Solarized, Tokyo |
| 91.8% | Nord, Nordic, Paper |

**17 painters — Brutalist plus 16 — carry the same ~79-line method, 91–97% identical.** Roughly
**1,300 lines that are one method.** Each contains its own copy of every `Show*` check, its own
`buttonX -= buttonWidth` cascade, its own hit-area registrations.

### This settles the migration target

The drawing in these painters is *genuinely different* — hexagons, blocks, circles, tiles, Zen rings.
That variety is the product. **The layout is not different at all.** So:

> **Extract the shared layout, leave every painter's drawing untouched.**

- It is measurable: 91–97% identical today, one method after.
- It is safe: layout is where the `Show*` flags live, so the existing per-flag differential is
  already the parity gate, and no skin's appearance should change — the contact sheet proves it.
- It is where the bugs were. `ShowCustomActionButton` was dead in 33 skins because 25 painters each
  had to remember it. GNOME and Custom are *still* dead for exactly that reason.
- It removes the ×N tax on everything that follows: corner-safe placement (7 painters lack it),
  `SafeContentInsets` (missing in most), DPI scaling.

This replaces the `FormChromePipeline` sketch in `05-painter-architecture.md`, which proposed moving
*drawing* into a shared pipeline. On the evidence, drawing is the part that should stay in the
painters.

## The traffic-light pair fixed together (2026-08-13)

Both are now read in full, so the DPI gap `Ma4` identified was unblocked — but scaling iOS's layout
**alone would have re-broken it**, in a subtler way than before.

`PaintiOSButtons` drew at its own `startX = 10, buttonSize = 12, buttonSpacing = 8` — the same
numbers `CalculateLayoutAndHitAreas` hands the traffic-light helper. They agreed **by coincidence**.
Scaling the layout would have moved the hit areas while the circles stayed put: buttons drifting
away from what they respond to, at 150% only, invisible at the DPI anyone would test at.

So iOS was converted to the MacOS pattern first — `CenterCircleIn(layout.CloseButtonRect)` and so on
— and only then scaled. MacOS already drew from its layout and needed the scaling alone.

Both verified: probe green, dead combinations 4/185, caption heights byte-identical across 37 styles,
and the contact sheet re-rendered and eyeballed — iOS and MacOS both still show their traffic lights.

**Twelve of twelve helper-based painters now scale their own literals**, and the two that draw
circular buttons both draw them from the layout.

**This is the second time in this stage that the safe-looking half of a fix would have caused the
damage** — the first was Metro's `metrics.SearchBoxWidth`, which a regex would have double-scaled.
Both were only visible from having read the file.

## Stage 05a-2 complete — 11 of 38 read (Metro2 partial)

The eleven divide cleanly by care taken, which no metric in the audit script captured:

- **Careful:** MacOS (draws from layout, corner-safe both sides), NextJS, iOS and MaterialYou (corner-safe, insets).
- **Careless:** the rest — fixed literals in paint paths, metrics fetched and discarded, corner arcs ignored.

And the two extremes are adjacent: **iOS and MacOS are both traffic-light skins**, one drawing from
the layout and one from hardcoded offsets, one shipping visible buttons and one shipping invisible
ones. Same problem, same helper, opposite outcomes — which is the whole argument for a pipeline in a
single example.

## Stage 05a-2 progress — 9 of 38 read

| painter | scales layout | metrics honoured for fill | corner-safe | distinct finding |
|---|---|---|---|---|
| Minimal | now yes | draws no fill | no | clip wrapping a no-op |
| Shadcn | yes | **no** | no | metrics fetched and discarded twice |
| RadixUI | yes | **no** | no | "focus ring" keys off hover, never focus |
| NextJS | yes | **no** | **yes** | icon kept clear of the corner arc; HSL copies have drifted |
| Metro2 | now yes | *(unread)* | *(unread)* | *(lines 1–259 unread)* |

Five of nine painters read now fetch metrics and discard them. That is no longer an oddity; it is the
majority behaviour among the helper-based skins.

## Stage 05a-2 progress — 7 of 38 read

| painter | scales layout | honours metrics for caption fill | distinct defect |
|---|---|---|---|
| Minimal | now yes | n/a (draws no fill) | clip wrapping a no-op |
| Shadcn | yes | **no** | metrics fetched and discarded, twice |
| RadixUI | yes | **no** | its advertised "focus ring" keys off hover, never focus |
| Metro2 | now yes | *(not fully read)* | *(pending)* |

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
