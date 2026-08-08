# TextFields — review & enhancement plan (2026-08)

23 files, ~10,300 lines. `BeepTextBox : BaseControl` split across 12 partials (Core, Properties,
Input, Events, Drawing, Search, Effects, IME, Theme, Accessibility, Methods) + 7 helpers
(drawing, effects, advanced editing, validation, search, autocomplete, simple-helper) + models
(TextEffect presets, SearchResult/Options). Seventh folder in the series — and the healthiest
theme-wise: **no useThemeColors flag anywhere**, ApplyTheme wires the TextBox* slot family
properly (placeholder, focus border, hover, disabled all themed).

## Findings (static pass)

### F1 — 9 swallows + 2 Debug.WriteLine, 0 BeepLog

- `BeepTextBox.Accessibility.cs:20` notify catch; `Drawing.cs:111` measure catch;
  `Events.cs:179` catch; `Input.cs:337,387` clipboard ExternalException ×2 (typed, but silent);
  `TextBoxAdvancedEditingHelper.cs:531` JSON parse; `TextEffect.cs:354` font fallback;
  `TextBoxSearchHelper.cs:623` measurement; `SmartAutoCompleteHelper.cs:554` filesystem.
- `TextBoxSearchHelper.cs:188,308` — Debug.WriteLine for search/regex errors (rule 1: BeepLog).

### F2 — the real literal gaps (~15 of 103; the rest are feature palettes, see F4)

| site | now | fix |
|---|---|---|
| `Drawing.cs:51-52` char counter | red/gray literals | `ErrorColor` / `SecondaryTextColor` |
| `Drawing.cs:62` typing indicator | `Color.Green` | `SuccessColor` |
| `Properties.cs:929/944` line numbers | gray/248-gray fields, never themed | theme in ApplyTheme: `SecondaryTextColor` / `PanelBackColor` |
| `TextBoxDrawingHelper:711/716` text fallbacks | grays | `DisabledForeColor` / `TextBoxForeColor` |
| `TextBoxDrawingHelper:431-434` selection ink | White/Black brightness pick | theme HAS the pair: `TextBoxSelectedForeColor` |
| `TextBoxSearchHelper:56/61` search highlights | Yellow/Orange | `HighlightBackColor` / `AccentColor`, themed in ApplyTheme |

Local pattern note: this control themes fields in ApplyTheme (placeholder/focus already do) —
follow it rather than introducing the Empty-override scheme mid-folder.

### F3 — 1 reflection probe

`TextBoxDrawingHelper:755` probes own control for `WordWrap` by reflection — check the typed
surface and replace with a cast or interface member.

### F4 — deliberate feature palettes, KEEP (documented)

`Models/TextEffect.cs` (76 literals): Terminal/Matrix/VS-Code/retro effect PRESETS — the palette
IS the feature, consumer opts in via `TextEffectMode`. `TextBoxAdvancedEditingHelper:85-109`:
syntax-highlight token defaults (no token slots exist in the theme). `TextBoxEffectsHelper`
scanline/glow internals: alpha veils of effect-config colours.

### F5 — no probe

Planned (TextProbe): render with text + placeholder + focus border; char counter near-limit
turns error-red; line numbers themed; selection render uses the slot pair; search highlight
render; theme responsiveness; typed input through real key handlers changes Text + raises
TextChanged (single Apply path — the Lovs lesson lives nearby); every render eyeballed.

## Order

1. F1 swallows + Debug.WriteLine → BeepLog — build + commit
2. F2 literal/theming fixes + F3 reflection — build + commit
3. F5 probe + eyeball — commit per fix batch

## Batch 1 done — all 10 swallows report (commit 33822482)

Clipboard failures are user actions that did NOT happen and now say so (Failure); regex/search/
measure/UIA/JSON/font/filesystem sites report once; both Debug.WriteLine sites replaced.

## Batch 2 done — theming gaps closed (commit 39608dca)

Char counter (ErrorColor near limit / SecondaryTextColor idle), typing indicator
(SuccessColor), line numbers (SecondaryTextColor/PanelBackColor — never themed before), search
highlights (HighlightBackColor/AccentColor), selection ink from the theme's
`TextBoxSelectedForeColor` pair instead of a brightness guess, drawing-helper text fallbacks
from slots. `WordWrap` added to `IBeepTextBox`; the reflection probe deleted. Feature palettes
(TextEffect presets, syntax tokens) kept as designed.

## Batch 3 done — probe 9/9, renders eyeballed

TextProbe (scratchpad): text render, placeholder render, near-limit counter differs from idle
(error ink visible in render: red "95/100"), themed line-number gutter, theme responsiveness,
typed input through real OnKeyPress lands in Text + raises TextChanged, backspace through
OnKeyDown, select-all render shows the themed selection pair. All eyeballed — no geometry
defects found (this folder was the healthiest of the seven reviewed).

Not verified: IME path, autocomplete dropdown interaction, validation helper behaviours.

## Batch 4 done — search highlighting and effects finally RENDER (probe 12/12)

User question ("is auto search already built in with effects?") exposed it: both subsystems were
fully built — search engine (find/replace/incremental), 1,700 lines of effects machinery with
timers that even invalidated the control — but their two rendering hooks,
`DrawSearchHighlights` and `DrawEffects`, were **defined and never called from anywhere**.
Search found matches with nothing on screen; effects animated an invisible string. (Batch 2 had
themed highlight colours that never painted — the instrument didn't catch a dead method.)

Wiring: the paint path (Events.cs, after DrawAll) now calls both hooks;
`TextBoxDrawingHelper.GetActualText` paints the effect's frame text while one runs (typewriter
partials/scramble frames through the normal pipeline), with Terminal and FadeIn suppressing the
base text — Terminal paints its own surface, FadeIn its alpha overlay. New non-instantiating
accessors (`HasActiveEffectVisual`/`EffectFrameText`) keep the lazy helper lazy on the paint
path. Also fixed by eyeball: single-line highlight rects anchored to textRect.Y while the text
draws vertically centred — highlights floated above the words; now centred to match.

Probe extended and eyeballed: search highlights paint (yellow + orange current match, aligned),
typewriter mid-frame shows "The qu", terminal renders green-on-black on its own surface.

## Batch 5 done — geometry pass: alignment, sizing, icons, scroll, caret (probe 17/17)

User directive: "revise alignment of text and sizing and other like images and icons sizing."

- **One authoritative text rect** (`GetEffectiveTextRect`): text drew in an Inflate(-2,-2) rect
  while placeholder/selection/caret used the un-inset one — the caret sat 2px left of the first
  character, placeholder 2px off the text position. All five consumers (text, placeholder,
  selection, caret, search highlights) now share the one method; the old `GetActualTextRect`
  deleted. Image gaps and insets DPI-scaled; `MaxImageSize` clamp DPI-scaled (a fixed 20px icon
  shrank relative to text at 150%).
- **Icons never showed**: `_imageVisible` defaulted false, so `ImagePath = x` silently displayed
  nothing until `ImageVisible` was also set. A path IS the intent — default flipped; both
  ImageBeforeText and TextBeforeImage verified by render (icon centred, text offset past it).
  `HasImage`'s dynamic probe replaced with the typed `BeepImage.HasImage` property.
- **Horizontal scroll was an empty stub**: with text longer than the box, the caret walked off
  the right edge and typing continued invisibly. Single-line caret-follow implemented
  (left-aligned text; offset clamped to overflow); text/selection/caret/highlights all draw
  scroll-shifted inside a clip. OnKeyPress only called ScrollToCaret when multiline (the stub
  era); Home/End never called it — all follow the caret now. Render shows "TAIL-END" after
  typing past the edge; Home returns offset to 0.
- **Caret painted at the selection ANCHOR**, not the caret: `ClearSelection` zeroes length but
  not the anchor, so after End/click the painted caret froze at the old spot. DrawCaret reads
  the real caret position now; probe holds End-vs-Home renders differing.
- **The bottom-right green dot** (user question): the typing indicator — on by default and
  surprising. Now opt-in (`EnableTypingIndicator = true` to get it back).

(Resolved in batch 7 — see below.)

## Batch 6 done — designer-selectable built-in icons via enum (probe 20/20)

User directive: textbox internal icons via enum backed by `IconsManagement/SvgsUIcons.cs`.

`TextBoxIconKind` (30 curated values: Search/User/Password/Email/Phone/Calendar/…) +
`TextBoxIconRegistry.GetPath` mapping each to the validated `SvgsUIcons` registry (which
reports once when a constant names a missing resource). `BeepTextBox.IconKind` property is
designer-browsable with a dropdown; setting a manual `ImagePath` resets IconKind to None so the
property never lies about what the image shows (re-entry guarded both directions).

The probe verifies every enum value resolves to an actually-embedded resource — and that check
FAILED on first run: the guessed `fi-tr-user.svg` is not embedded (→ `fi-tr-circle-user.svg`).
Render verified: IconKind=Search paints the icon with text offset past it; manual-path reset
verified.

## Standing constraints

There is ALWAYS a theme — slots direct; feature palettes (effect presets, syntax tokens) are
deliberate and stay. A check must be able to fail. Commit to master only.

## Batch 7 done — vertical scrolling for real, RTL, alignment-aware overflow (probe 26/26)

The three recorded gaps, closed:

- **Multiline vertical scrolling** (was five empty stub methods): `TextBoxScrollingHelper` now
  tracks ContentHeight (lines × line height, refreshed at the END of UpdateLines — a
  pre-rebuild call was stale by one edit), viewport height pushed by the paint path, clamped
  `ScrollOffsetY`. Mouse wheel scrolls by `SystemInformation.MouseWheelScrollLines`;
  `ScrollToCaret` follows the caret's line. Text draws Y-shifted inside a clip and the
  line-number gutter scrolls in sync (off-viewport rows skipped). Verified: wheel-down shows
  lines 13-17 with matching gutter, wheel-up clamps at 0, Ctrl+End scrolls the last line in.
- **Alignment-aware overflow** (`GetTextOriginX`, one origin model for text/selection/caret):
  when the text fits, alignment decides; when it overflows, the caret-follow offset governs
  while focused (Home shows the head even right-aligned) and the alignment's natural anchor
  governs unfocused — right-aligned long text anchors its TAIL at the right edge (verified by
  render), centred shows its middle. ScrollToCaret no longer resets for non-left alignments.
  This also fixed selection rects being misplaced for centred/right-aligned text (they assumed
  left-aligned X).
- **RTL**: the flag alone only changes reading order — Latin text rendered pixel-identical (the
  probe's check failed honestly). `EffectiveAlignment` now flips Left/Right under
  RightToLeft.Yes, matching native textbox behaviour; flags and origin model share it.

(Both resolved in batch 8 — see below.)

## Batch 8 done — one visual-line layout: wrap-aware scrolling, real multiline caret/selection (probe 30/30)

The two remaining pre-existing gaps, closed with ONE layout authority
(`TextBoxDrawingHelper.GetVisualLines`, cached per text/width/font/wrap):

- Raw lines split by scanning newline offsets (indices preserved through 

); WordWrap
  segments each raw line greedily at word boundaries (binary-search char fitting for long
  words). Every consumer draws from this layout: per-line text painting, the caret, the
  selection, the gutter, and the scroll metrics — what is measured is exactly what paints.
- **Wrap-aware scroll range**: the coordinator pushes visual-line count × line height to the
  scrolling helper before each paint (`SetContentMetrics`). A single 60-word paragraph now
  wheels through its wrapped rows (offsetY=156 where the raw count gave 0) and Ctrl+End
  follows the caret through them (`ScrollLineIntoView` + cached caret→visual-line lookup, raw
  fallback before first paint).
- **Multiline caret**: real (line, column) placement — the old math measured the whole prefix
  as one line, so the caret drifted right instead of down. Verified: two Down presses move the
  painted caret onto "charlie".
- **Multiline selection**: per-visual-line fills + selected ink, with a small tail marker when
  the line break is inside the range. Verified by zoomed render: "al|pha" boundary exact,
  middle line fully filled, line after the range untouched.
- Gutter numbers only a raw line's FIRST segment (wrapped continuations are unnumbered) and
  positions rows off the text font's line height (it previously used the gutter font's).

(Resolved in batch 9 — see below.)

## Batch 9 done — icons themed the NORMAL way, PaintWithTint itself fixed, click-to-caret and IME on the layout (probe 33/33)

User direction: do not tint at the textbox layer — paint the SVG normally and fix PaintWithTint.

- **Textbox icons**: DrawImage draws through BeepImage again (no tint override). `IconKind`
  turns `ApplyThemeOnImage` on, so the BeepImage themes its own SVG — the Beep-normal path.
  Probe holds 0 pure-black pixels in the icon zone (the glyph follows theme ink). Custom
  `ImagePath` SVGs stay un-themed unless the consumer opts in (a coloured logo must not be
  silhouetted). The unscaled MaxImageSize re-clamp in DrawImage stays deleted (it shrank the
  icon below its DPI-scaled layout slot).
- **StyledImagePainter.PaintWithTint fixed at the source** (repo-wide painter):
  (1) SVGs now rasterize AT THE REQUESTED SIZE — `svg.Draw()` rendered at the document's
  native size and one cached bitmap was scaled into every caller's bounds, blurring small
  icons and washing out thin strokes; (2) the ImagePainter fallback set `ApplyThemeOnImage =
  true` BEFORE `FillColor = tint` — the false→true transition applies the fill, so the
  requested tint never landed (the CLAUDE.md FillColor trap, found live); (3) `LoadImage`'s
  silent catch reports once.
- **Click-to-caret consults the layout**: clicks in wrapped text used single-line prefix math
  and landed on the wrong line. `GetCaretIndexFromPoint` maps the click row to its visual
  line (scroll-aware) and the nearest character within it; the coordinator routes clicks
  through it. Probe: a click on wrapped row 2 lands the caret on visual line 2.
- **IME composition underline anchors at the caret's real pixel position**
  (`GetCaretPixelPosition` from the layout) — the old math measured the whole prefix as one
  line and pinned the underline to the control's bottom edge. Probe simulates composition and
  the render shows the dashed underline exactly under "second"'s line.

## Batch 10 done — textbox icons: no ApplyThemeToSvg, muted PaintWithTint (probe 33/33)

User reported icons still black-filled, then directed: no ApplyThemeToSvg for textbox icons.
Root cause confirmed by slot dump: `ApplyThemeToSvg` floods every SVG node's Fill with the
ImageEmbededin-mapped slot — `TextBoxForeColor`, which is pure BLACK in DefaultTheme. Built-in
IconKind glyphs now paint through the FIXED `StyledImagePainter.PaintWithTint` (sized
rasterization, replace-RGB tint) in `SecondaryTextColor` (`DisabledForeColor` when disabled) —
the muted leading-icon ink. Custom `ImagePath` images draw raw through BeepImage; their colours
belong to the consumer. Probe tightened: the icon zone must contain the muted-gray ink
(59 px) and near-zero visually-black pixels — verified by zoomed render: crisp gray outline.

## Batch 11 done — textbox icons: JUST PAINTED (probe 33/33)

Final user directive: no ApplyTheme on the image, no tint, no fill — just paint. `DrawImage`
is now three lines: `StyledImagePainter.Paint(g, imageRect, ImagePath)` — the SVG renders
with its own artwork colours, rasterized at the target size by the ImagePainter vector path.
Probe asserts the glyph is present AND an outline (39/960 dark pixels — not blank, not a
filled block); zoomed render eyeballed. StyledImagePainter's five copy-pasted
`Debug.WriteLine("Unable to resolve image")` sites now report once through BeepLog (a missing
icon says so instead of silently painting nothing).
