# BeepCalendar Planning Index

This folder contains the BeepCalendar modernization and commercialization roadmap.

## Plan Documents

- 00-overview-gap-matrix.md
- 01-phase1-foundation-and-core.md
- 02-phase2-event-model-and-interactions.md
- 03-phase3-views-resources-performance.md
- 04-phase4-integrations-enterprise-accessibility.md
- 05-external-benchmark-and-ux-standards.md
- 06-pipeline-consolidation-and-editor-layer.md
- phase1-validation-checklist.md
- phase2-validation-matrix.md
- TODO-TRACKER.md

## Scope

The plan aligns BeepCalendar with modern scheduler expectations from:

- Open-source calendars: FullCalendar, React Big Calendar, TOAST UI Calendar
- Commercial schedulers: DevExpress WinForms Scheduler, Telerik WinForms Scheduler, Syncfusion WinForms Scheduler
- Design systems: Material Design 3 date/time UX principles and Figma-first spacing/state discipline

## Current Baseline

Based on current code in Calendar (2026-06-04):

- Core control: `BeepCalendar.cs` (~95 partial files)
- State and layout: `Helpers/CalendarState.cs`, `Helpers/CalendarLayoutManager.cs`, `Helpers/CalendarSurfaceModel.cs` (immutable per-`UpdateLayout` snapshot)
- Event querying: `Helpers/CalendarEventService.cs` (paint-cycle caching + performance counters)
- Per-view painter: `Rendering/ICalendarViewPainter.cs` + `Rendering/ViewPaintArgs.cs` + `Rendering/ViewPainterFactory.cs`; one painter per `CalendarViewMode` in `Rendering/ViewPainters/` (Month / Week / WorkWeek / Day / Agenda / Timeline / List)
- Chrome (header text / view selector / toolbar / sidebar): painted by `BeepCalendar` partials (`BeepCalendar.Painting.Pipeline.cs`, `BeepCalendar.Toolbar.cs`, `BeepCalendar.Painting.Sidebar.cs`)

## Execution Rule

When implementing any phase, update TODO-TRACKER.md in the same commit/change wave.

## Current Execution Snapshot (2026-06-04)

- Phase 0: Complete
- Phase 1: In progress (core contracts implemented; baseline screenshot capture pending)
- Phase 2: In progress (event-domain/conflict/filter foundation contracts implemented)
- Phase 3: In progress (work-week / agenda / timeline renderers + initial virtualization + performance counters implemented)
- Phase 4: Planned
- Phase 6: In progress (W1 done; W2 in progress; build BROKEN during view painter installation. See `06-pipeline-consolidation-and-editor-layer.md`)
