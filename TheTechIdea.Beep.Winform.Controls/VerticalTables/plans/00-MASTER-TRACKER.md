# VerticalTables — review & enhancement plan (2026-08)

27 files, ~8,500 lines. `BeepVerticalTable : BaseControl` (SimpleItem columns, children = rows),
14 style painters over `IVerticalTablePainter`, layout/hit-test helper, theme/font/icon/sparkline
helpers, token catalog. Readme states the intent: painters own layout helpers and hit areas.

## Findings

### F1 — the Calendar disease, concentrated: 253 literal colours across the 14 painters

`VerticalTableThemeHelpers` exists (7 colour getters, correct API shape) and Style1 partially uses
it — but the other painters hardcode a full Tailwind-slate palette (`250,251,252` surfaces,
`226,232,240` borders, `71,85,105` text, blue/green/amber accents) and never touch the theme.
The helpers themselves carry 12 literal fallbacks and the exact `UseThemeColors` / null-theme
branching that Calendar just had removed by user directive.

The theme declares a full `Grid*` family (`GridBackColor`, `GridForeColor`, `GridHeaderBack/Fore/
Border`, hover/selected variants, `GridRowHoverBackColor`, …) — the slots these painters should read.

**Fix shape (the Calendar end-state, applied from the start):**
1. `VerticalTableThemeHelpers`: theme = supplied ?? `BeepThemesManager.CurrentTheme`; no
   `useThemeColors` parameter, no literal fallbacks — one slot, one return.
2. Painters: every literal routed to a helper/slot. Semantic colours (success/warning/error)
   from semantic slots.
3. `_currentTheme ?? (UseThemeColors ? CurrentTheme : null)` dance in the control and painters →
   just `_currentTheme` (BaseControl guarantees it).

### F2 — 2 silent catches

`Helpers/VerticalTableLayoutHelper.cs`, `Painters/VerticalTableStyle7Painter.cs` → BeepLog per site.

### F3 — no probe exists

Planned checks: all 14 styles render distinctly (aliased-style check, with selection + hover state);
theme responsiveness (two themes → different pixels — F1 makes this the check that matters);
`ItemClicked`/`SelectedItemChanged` state round-trip; hit-test alignment (click lands on the column
the render shows).

## Order

1. F2 (mechanical), F1 helpers, F1 painter sweep — build + render per batch
2. Probe, renders eyeballed
3. Commit per batch on master

## Standing constraints

Per `CLAUDE.md` + user directives from Calendar: there is ALWAYS a theme — assign slots directly,
never guard, never blend, never literal; a wrong colour is the theme's bug. A check must be able
to fail.

## Batch 1 done - theme slots wired, swallows report

- VerticalTableThemeHelpers rewritten to the Calendar end-state: no useThemeColors flag, no Empty
  guards, no ShiftLuminance blends, no literal fallbacks - Grid* slots direct, theme = supplied ??
  current. Cur accessor for painters.
- 239+ literals swept to Grid*/semantic slots across all painters and the control (142 + 97 + hand
  fixes for context-dependent stragglers: white-as-text vs white-as-fill, separators, selection
  tints).
- **Style14 left as-is, needs a ruling**: it is a deliberate neon/cyberpunk identity style with
  NAMED palette constants (DarkBg, NeonCyan, ...). Making it theme-driven turns it into another
  generic style; keeping it makes it the one non-theme painter. User decision requested.
- Both silent catches now report (hit-list clear -> Failure; style7 image paint -> FailureOnce).

Not yet done: the probe (F3) - render all 14 styles distinct + theme responsiveness + click
round-trip; renders not yet eyeballed after the sweep.

## Batch 2 done - probe 19/19, and it found a process-killer

**Every click on this control was a StackOverflow that killed the host process.** The layout helper
registers the control itself as its hit areas' component; the pipeline forwards a hit to the
component's ReceiveMouseEvent; the override called base.ReceiveMouseEvent, which re-dispatches into
_input, which runs the same hit test and forwards to the control again. Fixed: the override is the
terminal receiver, no base call. Found by the probe's first click - uncatchable, so no BeepLog line
ever fired; only running it could expose it.

Probe: 14/14 styles paint real content and render distinctly; Style6 differs between two themes (the
239-literal sweep proven live); CellClicked verified through BOTH paths - the full OS pipeline (with
the real cursor parked, since _input.OnClick reads Cursor.Position, not event args) and the direct
ReceiveMouseEvent receiver. Style6 render eyeballed: clean themed table, alternating rows, feature
column, grid lines.

Instrument notes for the next reader: three rounds of guessed click coordinates hit (1) a column
resize hot zone that consumed the click as a resize-end, (2) empty space, before (3) reflecting the
layout helper's own first cell rect - the layout is the only authority on where cells are. And the
pipeline reads the PHYSICAL cursor: synthetic MouseEventArgs alone cannot drive it.

Still open: Style14 identity-palette ruling; hover/multi-select paths unprobed; renders of the other
13 styles not individually eyeballed.
