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

## Standing constraints

There is ALWAYS a theme — slots direct; feature palettes (effect presets, syntax tokens) are
deliberate and stay. A check must be able to fail. Commit to master only.
