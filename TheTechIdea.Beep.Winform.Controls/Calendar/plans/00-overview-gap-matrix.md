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

From current Calendar implementation (2026-06-04, mid-Phase 6 W2):

- Control orchestrator exists: `BeepCalendar.cs` (~95 partial files).
- **Per-view painter** (replaces both `ICalendarStylePainter` + `ICalendarViewRenderer`): `ICalendarViewPainter` in `Rendering/ICalendarViewPainter.cs` with one implementation per `CalendarViewMode` (Month / Week / WorkWeek / Day / Agenda / Timeline / List) under `Rendering/ViewPainters/`. Each painter owns Layout + Paint + HitTest and consumes `IBeepTheme` + `BeepControlStyle` directly via a single `ViewPaintArgs` bundle — no `ICalendarStylePainter`, no `MaterialCalendarPainter`, no `MinimalCalendarPainter`, no `CalendarPainterFactory`.
- Layout manager exists: `CalendarLayoutManager` + `CalendarSurfaceModel` (immutable, built once per `UpdateLayout`).
- Event query service exists with paint-cycle caching: `CalendarEventService`.
- Chrome (header text / view selector / toolbar / sidebar) stays in `BeepCalendar` partials; the view painter only fills the central grid for its view.

## Key Gaps

1. Event domain depth: no explicit recurrence engine contract, exceptions, reminders pipeline, attendee/resource model, timezone policy.
2. Interaction depth: drag-create, drag-move, resize, snap/grid behavior, collision policy, keyboard-first event editing are not yet formalized as shared contracts.
3. Views breadth: missing year/resource-centric views (work-week, agenda, timeline are now first-class via per-view painters but the doc trail still references the legacy renderer names).
4. Resource scheduling: resource lane model exists; no resource-aware layout pipeline integration with the per-view painter for non-timeline views.
5. Virtualization/perf: virtualization in `TimelineViewPainter` is the only one; week/day need virtual rendering for >1000 events.
6. Interop and enterprise: no explicit ICS import/export, external provider sync contract, or print/export workflows.
7. Accessibility completeness: baseline roles exist in platform patterns, but calendar-specific AT narration and keyboard map parity need formalization.
8. Design-time and documentation: limited phase-driven validation matrix and scenario cookbook for adopters.
9. **Per-view painter pipeline mid-rewrite (W2 in progress, build BROKEN)**: `ICalendarStylePainter` / `MaterialCalendarPainter` / `MinimalCalendarPainter` / `ICalendarViewRenderer` / `*ViewRenderer.cs` / `CalendarPainterFactory` / `CalendarRenderContext` / `CommonDrawing.cs` all deleted; per-view `ICalendarViewPainter` partially wired (interface + `ViewPaintArgs` + `ViewPainterFactory` created; 7 view painter files + BeepCalendar rewires pending). Resolution lives in `06-pipeline-consolidation-and-editor-layer.md` W2.
10. **Empty/stub partial files**: 12+ `BeepCalendar.*.cs` partials are zero-behavior stubs (e.g. `BeepCalendar.Painting.Views.cs`, `BeepCalendar.Invalidation.cs`, `BeepCalendar.Commands.cs`). Same doc.
11. **No hosted in-place editor surface**: toolbar is painted (`BeepCalendar.Toolbar.cs:26-203`) and event editing goes through a modal `CalendarEventEditor` dialog. No `BeepTextBox` / `BeepCheckBoxBool` / `BeepDateTimePicker` / `BeepComboBox` can be hosted inside the calendar for inline editing. Same doc — `Editor/CalendarEditorHost` + `Editor/CalendarEditorLayer` + `OnPaint`/`OnPaintBackground` clip.
12. **No developer extension surface**: there is no behavior, sidebar-panel, or toolbar-item collection; `ICalendarConflictPolicy` is internal; data provider is fixed to the in-memory `Events` list. Captured in the follow-up `07-developer-extension-surface.md` doc.

## Phase Map

- Phase 1: Foundation and core contracts
- Phase 2: Event model and interaction workflows
- Phase 3: Advanced views, resources, and performance
- Phase 4: Integrations, accessibility, localization, and enterprise readiness
- Phase 6: Pipeline consolidation and editor layer (see `06-pipeline-consolidation-and-editor-layer.md`)

## Definition Of Done For Program

- All phases complete with per-phase DoD satisfied.
- TODO tracker reflects verified status and remaining risks.
- Manual QA matrix for keyboard, DPI, RTL, HC, and high-volume event scenarios is complete.
- Plan docs and Calendar readme are aligned with implemented behavior.
