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

## Probe pass 1 complete — 18/18, three defects found and fixed

1. **Enter never reached a CreateEventRequested subscriber** — the constructor registers a default
   `CalendarEventEditor`, so the "editor first" order silently swallowed every host subscription,
   contradicting the documented contract. Subscriber first now.
2. **Every timed view rendered an empty grid** while Month showed the same events. Day, Week1, Week
   and WorkWeek painted event blocks and then let the hour loop fill each slot opaquely over them.
   Slots first, events second.
3. **Week2/3/7 had the same bug with the slot fill inline** — a name-based search for PaintTimeSlot
   missed them; only per-view renders exposed it. Same reorder applied (a first patch for Week2 used
   a DayCount symbol that does not exist there; caught by the build, fixed to the view's literal 7).

Verified by renders: Day shows Standup/Design review at 9/11 AM; Week2 shows all five events in the
right day columns including the overlapping pair. Aliased-style check: all 10 view modes distinct.

Timeline looked: it renders events (so no slot-fill bug), but only the LAST event of each day is
visible - same-day events share one lane at identical bounds, so Design review covers Standup and
Overlap B covers Overlap A. Needs lane stacking (or per-day row height growth) in
TimelineViewPainter; deferred as its own work item, not a one-line reorder.

Still open from the plan: Timeline lane stacking (above), theme
responsiveness / 93 literal colours, editor lifecycle probe, drag interactions, and undisposed
brush/pen allocations in the slot loops (new SolidBrush/Pen per slot per paint).

## Probe pass 2 — 19/19

- **Timeline lane stacking done** (commit 25e3b1a6): greedy sub-rows, one rect computation shared by
  Paint and HitTest, dead GetTimelineEventRect deleted. All five probe events visible.
- **14 undisposed brush/pen sites wrapped in using** across all 7 timed painters - each slot was
  allocating a SolidBrush and Pen per paint, 24x7 per frame, GC-finalized at best.
- **Theme responsiveness verified**: Month renders differently under ArcLinuxTheme vs ZenTheme, so
  the 93 Color.FromArgb literals are fallbacks, not the live palette. F2 closed.

Remaining, both needing input injection the probe cannot fake cheaply: the editor lifecycle probe
(BeginEdit/EndEdit/Escape/pool reuse) and drag interactions (move/resize with snap). Honest status:
not attempted rather than half-done.

## Probe pass 3 — editor lifecycle CLOSED (6/6), drag is a real open defect

Editor lifecycle, all state-level, no injection needed after all: BeginEdit enters editing and returns
the hosted editor; Escape through ProcessCmdKey cancels without touching the title; a second session
reuses the pool without a dead control; EndEdit(commit) closes. 6/6.

Drag: the probe locates the event block from the RENDER, injects OnMouseDown / six OnMouseMove steps
past the threshold / OnMouseUp two slots lower - and the event does not move (28/29 checks). The
pipeline is fully wired (Down sets _pointerDown + _activeInteractionHit; Move escalates to
_dragInProgress + ResolveDragMode; Up gates CommitInteractionMutation on InteractionMode ==
MoveEvent), so the break is inside one of ResolveInteractionTarget / ResolveDragMode /
CommitExistingEventMutation. Needs instrumentation (log the hit TargetKind and InteractionMode at
each stage) - next session's first calendar task. The failing check stays in the probe as the
regression tripwire.

## Drag FIXED — three defects deep (probe 29/29)

1. Commit gated on _state.SelectedEvent, so dragging an unselected event did nothing - and with A
   selected, dragging B would have moved A. Drag target is the hit now.
2. BuildProposedStart/End read only the selection too - same fix (InteractionEvent).
3. The killer: OnMouseUp set Capture=false BEFORE clearing _pointerDown; OnMouseCaptureChanged fired
   synchronously, its (!Capture && _pointerDown) guard passed, and CancelInteraction wiped the drag -
   EVERY drag was cancelled by its own mouse-up since the W2-Redo-18 capture-loss handler was added.
   _pointerDown clears first now; genuine capture loss (Alt+Tab) still cancels.

Instrumentation route: hit/mode/delta all printed correct (56px -> 02:00), which pinned the break to
the up path and exposed the capture self-cancel. Verified: 9:00 -> 11:00, snap honoured, duration
preserved. One probe-side correction along the way: commits REPLACE the event with a clone, so
assertions must re-query Events, not hold the old reference.

## Theme colour assignment revised — slots only, no literals, one open defect

Per user direction: ViewPaintArgs colour properties have NO initialisers (the Badges rule) and
ResolveThemeColors assigns THEME SLOTS VERBATIM - CalendarHover/Selected/Today/Title/DaysHeader all
wired to their dedicated BeepTheme.Calendar slots, last-resort theme = BeepThemesManager.CurrentTheme,
the literal fallback palette deleted. Event fills: category colour, else the theme's hue slots
verbatim (Primary/Secondary/Accent/Success/Warning/Error) hashed stably per event; ink picks between
theme fore/back by contrast (GetContrastingForeground). Two earlier wrong turns, both corrected on
user review: a hue-rotation palette (invented colours) and softening blends (also invented).
Or(slot, otherSlot) guards handle instance-unpopulated slots by falling back to ANOTHER slot of the
same theme.

Verified: probe 29/29 (drag block-finder updated: it hunted GRAY blocks and the fills are now theme
hues - the failure proved the change); Week2 render shows theme-blue events with legible ink.

**OPEN DEFECT:** title text and the day-header row render invisible under the default theme, and one
of five events (Overlap A) does not render - consistent with some resolved slots still coming out
transparent (unpopulated Calendar* slots in the default theme instance, or the `is BeepTheme` gate
not running so the extended block is skipped). Next step: print the resolved args palette values at
paint time and check which slots are A=0, then extend Or() coverage or fix the gate. A probe check
asserting "title text is visible" should be added and MUST fail before the fix.

## Final colour rule (user directive): there is always a theme - just assign

All guard/fallback machinery removed: no `is BeepTheme` cast, no Or()/Slot() per-slot fallbacks, no
null-conditional theme access. ResolveThemeColors and the cell-template helpers assign the theme's
slots directly; a wrong-looking colour is the THEME's bug, fixed in the theme, not silently
substituted by the control. OutOfMonth uses the calendar surface with DisabledForeColor numbers -
the dark slabs in Month were DisabledBackColor standing in as a cell surface. Probe 33/33; Month
render verified clean.
