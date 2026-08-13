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
| **05a-1** helper-based, part 1 | Modern, Linear, Material, MaterialYou, Metro | ◐ Modern done |
| **05a-2** helper-based, part 2 | Metro2, Minimal, NextJS, RadixUI, Shadcn, iOS, MacOS | ☐ |
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
| M1 | **`PaintWithEffects` leaks a GDI `Region` every paint.** Lines 289–291 build `new Region(path)`, assign it to `g.Clip`, then immediately restore `originalClip`. The assignment has no effect and the Region is never disposed. | **bug** |
| M2 | **`GetCornerRadius` returns a hardcoded `new CornerRadius(8)`** and never reads `metrics.BorderRadius`, so the DPI-scaled radius from stage 04 never reaches this skin's corners — and neither does any theme or style setting. | **bug** |
| M3 | **Bevel geometry is unscaled while its layout is scaled.** `CalculateLayoutAndHitAreas` scales `buttonWidth` through `DpiScalingHelper`, but `PaintModernBeveledButtons` hardcodes `buttonSize = 20` and derives padding from it. At 150% the hit rect grows and the drawn button does not. | **bug** |
| M4 | **`PaintCaption` never reads `metrics.CaptionColor`** — it paints `FromArgb(250,250,250)`→`(245,245,245)` with a `(230,230,230)` divider. Permitted by the painter-colour exemption, but it explains why Modern looked identical before and after the "skin wins" change: its metrics are decorative. | record |
| M5 | **`ShiftLuminance` used for hover and border states** (lines 152, 178, 185), which the standing colour rule excludes. | record |
| M6 | Local copies of `CreateRoundedRectanglePath`, `ShiftLuminance`, `ColorToHsl`, `ColorFromHsl`, `Hue2Rgb` — ~70 lines duplicated per painter. | debt |

Nothing in M1–M3 is visible to the parity harness, which compares caption-band pixels between flag
states. They would not have been found by any check written so far — only by reading.

## Exit criteria

All 38 files read end to end and recorded here; findings triaged into fix batches; the parity harness
and contact sheet re-run after each batch; and this log's per-painter sections complete enough that
the pipeline design in `05-painter-architecture.md` is written from evidence rather than from the
shape the folder appears to have.
