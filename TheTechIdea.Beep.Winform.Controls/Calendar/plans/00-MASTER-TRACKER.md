# Calendar — review & enhancement plan (2026-08)

113 files, ~12,500 lines. `BeepCalendar : BaseControl` in partials (Core.PublicApi, Core.Lifecycle,
Toolbar, Interactions), a per-view painter set (`Rendering/ViewPainters/`: Day, Month, Timeline,
`week`, `workweek`, `week1`…`week7`), `CellRender/` component cache, `CellTemplates/`, `Editor/`
(pooled inline editors), `Helpers/CalendarSurfaceModel`. Events flow through `CalendarEventService`
with a conflict policy. `sampleimages/c1..c7.png` are the reference designs — each `WeekN` painter
declares which image it implements.

## Read so far (this pass)

- `BeepCalendar.cs` — paint clipping around the editor layer, Esc handling, keyboard nav (arrows /
  Page / Home / End / Enter-to-create), Ctrl+wheel view switching, a thorough Dispose. Heavily
  annotated with W-numbered gap fixes; quality is high.
- `Core.PublicApi.cs` (head) — property setters consistently deactivate cell components and request
  layout; `CurrentDate` keeps `_focusedDate` in sync (documented bug fix).
- `Week2ViewPainter` head + a structural diff across Week1/2/3/7/week: **the eight week painters are
  genuinely distinct** (~190 of ~280 lines differ after name normalization), each matching its
  reference image. Not copy-paste; no dedup warranted.

## Findings

### F1 — 11 silent catches (fix now)

| where | class | fix |
|---|---|---|
| `BeepCalendar.cs` Dispose ×5, `CalendarCellComponentCache:68`, `CalendarEditorPool:62` | dispose-path swallows — a failed dispose is a leak | `BeepLog.Failure` |
| `Core.Lifecycle.cs:164,255` | `BeginEdit` "editor not registered → swallow" — double-click to edit silently does nothing | `WarnOnce` |
| `Core.PublicApi.cs:360` | cell component `Draw` — paint path | `FailureOnce` per cell key |
| `CellTemplates/BeepCellTemplateHelpers.cs:82`, `Toolbar.cs:174` | `StyledImagePainter.Paint` — returns quietly for unresolvable paths, so these only see corrupt images | `FailureOnce` keyed by path |

### F2 — 93 literal `Color.FromArgb`, 0 BeepLog references

To classify against rule 3: event-category colours are data (legitimate), chrome colours must come
from the theme. Probe check: render Month view under two themes — different pixels required.

### F3 — Material3 default fallout (from the BaseControl change `01c97390`)

Calendar renders were never taken at all, let alone under the new default. The probe below is the
first look.

## Enhancement plan (order of value)

1. **CalendarProbe** (scratchpad): build, add events, render Day/Week/Month/Timeline + all week
   variants; aliased-style check across the 9 week-family painters (distinct code ≠ distinct pixels);
   compare against `sampleimages/c1..c7`; keyboard nav state checks (arrow moves focus, Enter raises
   CreateEventRequested); event add/select/conflict-policy round-trip; theme responsiveness (F2).
2. **Fix F1** (11 catches) — mechanical, this pass.
3. **Per-render fixes** from whatever the probe/renders expose (this folder has never been rendered).
4. **Inline editor + editor pool lifecycle probe** — BeginEdit/EndEdit/Escape, pool reuse without
   disposed-control resurrection.
5. **Drag interactions** (SupportsEventDrag views): move/resize with snap — needs mouse injection;
   record honestly if not reachable, as with ComboBox keyboard nav.

## Not examined yet

Interactions partials, CalendarSurfaceModel internals, CellTemplates beyond the helper, Editor
sample editors, the toolbar hit-testing, undo/redo stacks. The per-painter geometry beyond Week2.

## Standing constraints

Per `CLAUDE.md`: every catch reports through BeepLog; no stubs/legacy; nothing assigns colours
(semantic event colours excepted); compose from Beep controls; a check must be able to fail; commit
to master only.
