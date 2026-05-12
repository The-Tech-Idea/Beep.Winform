# BeepCalendar Overview And Gap Matrix

Priority: High
Status: Planning Approved
Depends on: Existing Calendar rendering architecture

## Vision

Evolve BeepCalendar from a good date/event surface into a full scheduler platform with:

- Enterprise-grade event workflows
- Multi-view and multi-resource scheduling
- High DPI and performance robustness
- Accessibility and localization parity
- Modern UI/UX quality aligned with Figma-style design systems

## Competitive Benchmark Inputs

### Open-source reference capabilities

- FullCalendar docs: month, time grid, list, timeline, resources, business hours, accessibility, timezone, localization, print
- React Big Calendar: drag/drop addons, localizers, custom styling/theming variables
- TOAST UI Calendar: multiple view modes, drag/resize, default popups, start-of-week and weekend-width customization

### Commercial reference capabilities

- DevExpress WinForms Scheduler: day/week/month/year/agenda/timeline, resource grouping, recurrence, reminders, sync, high-DPI focus, print/export
- Telerik WinForms Scheduler: day/multiday/week/workweek/month/timeline/agenda, iCal support, reminders, navigator, resource grouping, localization/RTL, holiday support
- Syncfusion WinForms Scheduler: Outlook-style UX, recurrence, drag/drop, themes, localization, performance for large datasets

### UX standards applied

- Material 3: larger hit targets, no visual noise, dynamic color compatibility
- Figma workflow standards: tokenized spacing, explicit component states, reusable primitives, predictable layout constraints

## Current Architecture Snapshot

From current Calendar implementation:

- Control orchestrator exists: BeepCalendar.cs
- View render strategy exists: Month, Week, Day, List via ICalendarViewRenderer
- Style paint strategy exists: ICalendarStylePainter and Material/Minimal painters
- Layout manager exists: CalendarLayoutManager
- Event query service exists with paint-cycle caching: CalendarEventService

## Key Gaps

1. Event domain depth: no explicit recurrence engine contract, exceptions, reminders pipeline, attendee/resource model, timezone policy.
2. Interaction depth: drag-create, drag-move, resize, snap/grid behavior, collision policy, keyboard-first event editing are not yet formalized as shared contracts.
3. Views breadth: missing timeline/agenda/year/work-week/resource-centric views as first-class renderers.
4. Resource scheduling: no resource lane/group model and no resource-aware layout pipeline.
5. Virtualization/perf: no explicit large-data virtualization pipeline, batched invalidation strategy, or perf telemetry checkpoints.
6. Interop and enterprise: no explicit ICS import/export, external provider sync contract, or print/export workflows.
7. Accessibility completeness: baseline roles exist in platform patterns, but calendar-specific AT narration and keyboard map parity need formalization.
8. Design-time and documentation: limited phase-driven validation matrix and scenario cookbook for adopters.

## Phase Map

- Phase 1: Foundation and core contracts
- Phase 2: Event model and interaction workflows
- Phase 3: Advanced views, resources, and performance
- Phase 4: Integrations, accessibility, localization, and enterprise readiness

## Definition Of Done For Program

- All phases complete with per-phase DoD satisfied.
- TODO tracker reflects verified status and remaining risks.
- Manual QA matrix for keyboard, DPI, RTL, HC, and high-volume event scenarios is complete.
- Plan docs and Calendar readme are aligned with implemented behavior.
