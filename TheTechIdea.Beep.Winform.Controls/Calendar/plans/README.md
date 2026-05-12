# BeepCalendar Planning Index

This folder contains the BeepCalendar modernization and commercialization roadmap.

## Plan Documents

- 00-overview-gap-matrix.md
- 01-phase1-foundation-and-core.md
- 02-phase2-event-model-and-interactions.md
- 03-phase3-views-resources-performance.md
- 04-phase4-integrations-enterprise-accessibility.md
- 05-external-benchmark-and-ux-standards.md
- phase1-validation-checklist.md
- phase2-validation-matrix.md
- TODO-TRACKER.md

## Scope

The plan aligns BeepCalendar with modern scheduler expectations from:

- Open-source calendars: FullCalendar, React Big Calendar, TOAST UI Calendar
- Commercial schedulers: DevExpress WinForms Scheduler, Telerik WinForms Scheduler, Syncfusion WinForms Scheduler
- Design systems: Material Design 3 date/time UX principles and Figma-first spacing/state discipline

## Current Baseline

Based on current code in Calendar:

- Core control: BeepCalendar.cs
- State and layout: Helpers/CalendarState.cs, Helpers/CalendarLayoutManager.cs
- Event querying: Helpers/CalendarEventService.cs
- Rendering strategy: Rendering/CalendarRenderer.cs, Rendering/ICalendarViewRenderer.cs
- Style strategy: Rendering/ICalendarStylePainter.cs and StylePainters/*

## Execution Rule

When implementing any phase, update TODO-TRACKER.md in the same commit/change wave.

## Current Execution Snapshot (2026-05-12)

- Phase 0: Complete
- Phase 1: In progress (core contracts implemented; baseline screenshot capture pending)
- Phase 2: In progress (event-domain/conflict/filter foundation contracts implemented)
- Phase 3: Planned
- Phase 4: Planned
