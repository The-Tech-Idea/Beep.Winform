# W9 — All-Views Painter Coverage Plan

**User request (2026-06-04)**: "go online and LOOK FOR ALL TYPES OF CALENDAR AND STANDARD ONE AND CREATE APLAN FOR EACH WITH ITS PAINTER".

**Core rule (2026-06-04)**: "EACH VIEW SHOULD DISTINCT THATS A RULE" — every view gets its own self-contained `ICalendarViewPainter` (no shared shim files like the old `TimedWeekPaintLogic`). View-specific geometry, paint, hit-test, drag-resize, navigation, header text, and W8 cell-component wiring all live in the painter.

**Goal**: catalog every standard calendar view type found in the industry research, group them into families, and design the `ICalendarViewPainter` for each — including the new `ICalendarViewPainter` members required to express the differences.

## 1. Source Bibliography

Web research consulted (2026-06-04):

- **FullCalendar** ([fullcalendar.io](https://fullcalendar.io)) — de facto JS scheduler standard. Plugin families: `dayGrid` (Year/Month/Week/Day), `timeGrid` (Week/Day), `list` (Year/Month/Week/Day), `multiMonth` (Year), `timeline` (Year/Month/Week/Day), `resource` (Timeline / TimeGrid / DayGrid variants), `scrollGrid`, `ical`, `interaction`, `luxon3`.
- **Teamup** ([teamup.com](https://teamup.com)) — 12 named views: Day, Week, Month, Multi-Day, Multi-Week, Year, Timeline, Scheduler, List, Agenda, Tiles, Classic.
- **DevExtreme Scheduler** ([devexpress.com](https://js.devexpress.com/jQuery/Documentation/ApiReference/UI_Components/dxScheduler/Views/)) — Day, Week, WorkWeek, Month, TimelineDay/Week/Month/WorkWeek, Agenda. Recurrence editor + time-zone-aware.
- **SVAR React Calendar** ([svar.dev](https://svar.dev/react-calendar/)) — 7 built-ins (day, week, month, agenda, year, resources, timeline) with subclasses for grid/timeline/tile variants.
- **Kendo UI Scheduler** ([telerik.com](https://docs.telerik.com/kendo-ui/api/javascript/ui/scheduler/configuration/views)) — Day, Week, WorkWeek, Month, TimelineDay/Week/Month/WorkWeek, Agenda. Resource grouping. Vertical resource view.
- **Syncfusion WinForms Scheduler** — Day, Week, WorkWeek, Month, TimelineDay/Week/Month. Strong-print support.
- **Telerik WinForms Scheduler** — Day, Week, WorkWeek, Month, Timeline. Appointment reminder service.
- **Material Design 3 Date Pickers** ([m3.material.io](https://m3.material.io/components/date-pickers)) — Picker, Range Picker, Text Input, Year grid, Month grid.
- **SAP Fiori Planning Calendar** ([sapui5.hana.ondemand.com](https://sapui5.hana.ondemand.com)) — Hours, Days, 1 Week, 1 Month, Months (year). Interval headers configurable.
- **Mantine Calendar / DatePicker** ([mantine.dev](https://mantine.dev/dates/getting-started/)) — Month, Year, Date, DateRange variants.
- **shadcn/ui Calendar** — Day picker, Month picker, Year picker (all read-only browse + select).
- **Preline Calendar** — Year (12-mini-month) view.
- **Gantt suites**: Kendo Gantt, DevExtreme Gantt, bluemill, SVAR Gantt, Telerik Gantt. Task hierarchy + dependencies (start-to-start / start-to-end / end-to-start / end-to-end) + milestones + baselines + critical path.
- **Notion Database views** — Board, Table, Calendar, Gallery, Timeline, List. (Calendar + Timeline + List = the standard office-app trio.)

## 2. View Taxonomy (6 Families)

The 30+ view types collapse into 6 families that share painter architecture:

### Family A — TimeGrid (vertical time axis, day columns)
- **A1. Day** — 1 day column, 24 hour rows, time label gutter
- **A2. Work Week** — 5 day columns (Mon-Fri)
- **A3. Week** — 7 day columns (Sun-Sat or Mon-Sun)
- **A4. Multi-Day (1-14)** — N day columns, configurable count
- **A5. Resource TimeGrid** — resources as rows, time as columns (deviation from A1-A4)
- **A6. Resource DayGrid** — resources as rows, all-day events as blocks

### Family B — DayGrid (day cells, no time)
- **B1. Month** — 6×7 day cells
- **B2. DayGrid Week** — 1×7 full-day event strips
- **B3. DayGrid Day** — 1 full-day block (no time)
- **B4. Multi-Month (Year/Month)** — 2×2, 2×3, 3×4, 4×3 of full months
- **B5. Quarter** — 3 mini-months in a row
- **B6. Year** — 3×4 or 4×3 of mini-months (read-only browse)
- **B7. Resource DayGrid Month** — resources as rows, month cells as columns
- **B8. Resource DayGrid Week** — resources as rows, days as columns

### Family C — List / Agenda
- **C1. Agenda (single day)** — events for one day, grouped by hour
- **C2. Agenda (multi-day)** — events for N days, grouped by day (the classic "agenda" view)
- **C3. List Day** — vertical list of all events on a single day
- **C4. List Week** — vertical list of events for the week, grouped by day
- **C5. List Month** — vertical list for the month
- **C6. List Year** — vertical list for the year (or month-grouped)
- **C7. Tiles** — card grid, responsive (events as cards, no time alignment)
- **C8. Day Card** — single day, events as large cards

### Family D — Timeline (horizontal time axis, resource rows)
- **D1. Timeline Day** — N resources × 1 day (horizontal)
- **D2. Timeline Week** — N resources × 7 days
- **D3. Timeline Month** — N resources × ~31 days
- **D4. Timeline Year** — N resources × 12 months
- **D5. Resource Timeline Day/Week/Month/Year** — same as D1-D4 with explicit resource grouping + headers
- **D6. Scheduler** — resources as columns, events as time bars (variant of D1)

### Family E — Gantt (project scheduling)
- **E1. Gantt Hours** — hour-row scale, task bars in hours
- **E2. Gantt Days** — day-row scale
- **E3. Gantt Weeks** — week-row scale
- **E4. Gantt Months** — month-row scale
- **E5. Gantt Quarters** — quarter-row scale
- **E6. Gantt Years** — year-row scale
- **E7. Gantt With Dependencies** — any of E1-E6 + dependency arrows (FS / SS / FF / SF)
- **E8. Gantt With Hierarchy** — any of E1-E6 + parent/child task indentation + collapse

### Family F — Specialized
- **F1. Mini Month** — small month grid for sidebar (read-only browse, navigate on click)
- **F2. Heatmap Year** — 365 colored cells, intensity = event count (GitHub contribution style)
- **F3. Empty / No Events** — friendly empty-state with a "create" prompt
- **F4. Print** — daily/weekly/monthly/list layouts (Phase 4 work)

## 3. `ICalendarViewPainter` Interface Extensions

The current interface (14 members) needs ~10 more members to express the differences between families:

```csharp
public interface ICalendarViewPainter
{
    // ── Identity (existing) ──
    CalendarViewMode ViewMode { get; }
    string Key { get; }
    string DisplayLabel { get; }

    // ── Layout classification (existing) ──
    int VisibleDayCount { get; }
    bool IsTimedView { get; }
    bool IsMonthGrid { get; }
    bool RequiresLeftGutter { get; }
    bool HasAllDayStrip { get; }
    bool SupportsEventDrag { get; }

    // ── Layout classification (NEW for W9) ──
    /// <summary>True when the time axis is horizontal (Timeline / Gantt / Scheduler). Default false.</summary>
    bool IsTimelineView { get; }

    /// <summary>True when rows/columns are resources (Timeline, Gantt, Scheduler, Resource TimeGrid/DayGrid). Default false.</summary>
    bool IsResourceView { get; }

    /// <summary>True for read-only views (Year browse, Mini Month, Heatmap). Default false.</summary>
    bool IsReadOnly { get; }

    /// <summary>True for Gantt with parent/child task indentation + collapse. Default false.</summary>
    bool SupportsTaskHierarchy { get; }

    /// <summary>True for Gantt with dependency arrows. Default false.</summary>
    bool SupportsDependencies { get; }

    /// <summary>True for Year Heatmap (intensity colors). Default false.</summary>
    bool ShowHeatmap { get; }

    /// <summary>True for ISO week numbers in left gutter (Material 3 / FullCalendar). Default false.</summary>
    bool ShowWeekNumbers { get; }

    /// <summary>True for multi-cell select (Multi-Month, Multi-Week). Default false.</summary>
    bool AllowMultiSelection { get; }

    /// <summary>Total days visible (1, 7, 14, 31, 90, 365, etc.). Default VisibleDayCount.</summary>
    int MaxVisibleDays { get; }

    /// <summary>Width of the row-header column for resource/timeline views (0 if none). Default 0.</summary>
    int RowHeaderWidth { get; }

    // ── Date / navigation (existing) ──
    DateTime NavigatePrevious(DateTime currentDate);
    DateTime NavigateNext(DateTime currentDate);
    string GetHeaderText(DateTime currentDate);
    DateTime GetVisibleRangeStart(DateTime currentDate);
    DateTime GetVisibleRangeEnd(DateTime currentDate);

    // ── Layout / paint / hit-test (existing) ──
    void Layout(ViewPaintArgs args);
    void Paint(Graphics g, ViewPaintArgs args);
    CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args);
    DateTime? GetDateTimeFromLocation(Point location, ViewPaintArgs args);
}
```

Defaults for non-overridden members live on a `CalendarViewPainterBase` (new base class) so each new painter only declares what's distinct.

## 4. Per-View Plan

### A1. Day (DISTINCT — own painter)

- **Source**: FullCalendar `timeGridDay`, DevExtreme Day, Kendo Day, Material 3 Date Picker (Hour view).
- **Sample**: c1 (left sidebar variant); this is the no-sidebar pure variant.
- **Layout**: 1 day column filling the time grid, wide day header (`{DayOfWeek}, {Month} {Day}`), 24 hour rows, 60-minute time label gutter.
- **Painter properties**:
  - `ViewMode = Day` (or new `Day1` enum if we want to be canonical-only)
  - `VisibleDayCount = 1`, `IsTimedView = true`, `RequiresLeftGutter = true`, `SupportsEventDrag = true`
  - `IsTimelineView = false`, `IsResourceView = false`, `IsReadOnly = false`
  - `MaxVisibleDays = 1`
- **Surface helpers needed**: `surface.CurrentDate.Date` for the day; `surface.GetTimeRowRect(hour)` for hour rows; new `GetDayColumnRect()` for the 1-wide column (delegate to `CalendarPainterHelpers.GetColumnRect`).
- **Helpers needed**: existing `FillRoundedRect`, `DrawText`, `GetTimedEventRect`, `ResolveResizeEdge`, `TryDrawCellComponent`.
- **W8 cell-component integration**: event blocks try `CalendarPainterHelpers.TryDrawCellComponent` with key `"evt:{id}"`; time slots may also try `"slot:{yyyy-MM-dd}:{hour}"` for `TimeSlotComponentFactory`.
- **Navigation**: `NavigatePrevious(d) = d.AddDays(-1)`, `NavigateNext(d) = d.AddDays(1)`.
- **Hit-test**: event blocks via `GetTimedEventRect`; time slots via `GetTimeRowRect`; header = `DateCell`.
- **Drag/resize**: left/right edges = resize start/end; middle = move (since `SupportsEventDrag = true`).
- **Read-only**: false.
- **Build order**: W2-Redo-2 (DONE). Keep `DayViewPainter` as-is.

### A2. Work Week (DISTINCT — own painter)

- **Source**: FullCalendar `timeGridWeek` (with `businessHours`), DevExtreme WorkWeek, Kendo WorkWeek, SAP Fiori `1 Week`.
- **Sample**: c2 (right detail panel variant), c3 (4-day + filter bar variant), c7 (status badges variant).
- **Layout**: 5 day columns (Mon-Fri), 24 hour rows, time label gutter, weekend greyed (none, since 5-day = no weekend).
- **Painter properties**:
  - `VisibleDayCount = 5`, `IsTimedView = true`, `RequiresLeftGutter = true`
  - `MaxVisibleDays = 5`
- **Surface helpers needed**: `startOfWorkWeek` computed by `(int)d.DayOfWeek == 0 ? 6 : (int)d.DayOfWeek - 1`; `CalendarPainterHelpers.GetColumnRect(timedArea, day, 5)`.
- **W8 cell-component integration**: event blocks per day; time slots per (day, hour) with key `"slot:{yyyy-MM-dd}:{hour}"`.
- **Navigation**: `±7 days` (one work-week).
- **Hit-test**: same as A1 but with 5 columns.
- **Build order**: W2-Redo-2 (DONE). Keep `WorkWeekViewPainter` as-is.

### A3. Week (DISTINCT — own painter)

- **Source**: FullCalendar `timeGridWeek`, DevExtreme Week, Kendo Week, Teamup Week.
- **Sample**: c1, c2, c7 (all 7-day variants).
- **Layout**: 7 day columns (Sun-Sat or Mon-Sun), 24 hour rows, time label gutter, weekend columns shaded with `WeekendBackColor`.
- **Painter properties**:
  - `VisibleDayCount = 7`, `IsTimedView = true`, `RequiresLeftGutter = true`
- **Surface helpers needed**: `surface.GetWeekDayDate(day)`, `surface.GetWeekDayColumnRect(day)`, `surface.GetWeekDayHeaderRect(day)`, `surface.GetTimeRowRect(hour)`.
- **W8 cell-component integration**: event blocks per day; time slots per (day, hour).
- **Build order**: W2-Redo-2 (DONE). Keep `WeekViewPainter` as-is.

### A4. Multi-Day (1-14) (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `customDayCount` (1-14 configurable), Teamup Multi-Day.
- **Sample**: c3 (4-day variant) generalized.
- **Layout**: N day columns (1-14, configurable via `_state.MultiDayCount` or via a new `CalendarViewMode.MultiDay{N}`), 24 hour rows, time label gutter, weekend columns shaded.
- **Painter properties**:
  - `VisibleDayCount = N` (dynamic, exposed via `MaxVisibleDays`)
  - `IsTimedView = true`, `RequiresLeftGutter = true`
  - `MaxVisibleDays = 14` (cap)
- **Surface helpers needed**: new `GetWeekDayColumnRectFor(dayIndex, count)` (or reuse `CalendarPainterHelpers.GetColumnRect(timedArea, day, count)`); new `GetDayDateFor(dayIndex, count)` on the painter.
- **W8 cell-component integration**: event blocks per day; time slots per (day, hour).
- **Navigation**: `±N days`.
- **Hit-test**: same as A3 but with N columns.
- **Build order**: W10 (high priority).

### A5. Resource TimeGrid (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `resourceTimeGridDay`, Kendo Scheduler (vertical resource view), DevExtreme Scheduler with `groupOrientation = "vertical"`.
- **Sample**: not provided; designed from spec.
- **Layout**: resources as rows, time as horizontal axis (24 hours or 7 days wide). Each row is one resource. Event bars span the time axis horizontally. Optional resource group headers.
- **Painter properties**:
  - `IsTimedView = true` (still hour-anchored), `IsTimelineView = true` (time axis is horizontal!), `IsResourceView = true`
  - `RequiresLeftGutter = false` (row header IS the gutter)
  - `RowHeaderWidth = 160` (default for resource name column)
- **Surface helpers needed (NEW)**:
  - `GetResourceRowRect(resourceIndex, count)` — rect for the row lane
  - `GetResourceHeaderRect()` — rect for the row header column
  - `GetTimelineColumnRect(timeIndex)` — rect for the time-axis column
- **Helpers needed (NEW)**:
  - `PaintResourceHeader(g, rect, resource, args)` — draw resource name + color
  - `PaintResourceGroupHeader(g, rect, groupName, args)` — collapsible group header
  - `PaintHorizontalTimeGrid(g, bounds, hourCount, args)` — vertical grid lines + hour labels
  - `PaintHorizontalEventBar(g, rect, evt, args)` — event bar with left/right resize edges (NOT top/bottom)
- **W8 cell-component integration**: event bars try `TryDrawCellComponent` with key `"evt:{id}"`; resource rows try with key `"res:{resourceId}:{hourIndex}"`.
- **Navigation**: `±1 day` (default) or `±N hours` for sub-day scales.
- **Hit-test**: event bars check `rect.Contains`; resource row header checks against `GetResourceRowRect(i)`; resize edges = left (start) and right (end) — DIFFERENT from vertical time grid (top/bottom).
- **Drag/resize**: horizontal move (event drag along time axis); horizontal resize (left = start, right = end).
- **Data model**: events need `ResourceId` (FK to `CalendarResource.Id`).
- **Build order**: W11 (medium priority — common in team scheduling apps).

### A6. Resource DayGrid (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `resourceDayGridDay`/`resourceDayGridWeek`, Kendo `resources` (day grid mode).
- **Layout**: resources as rows, days as columns (7 or N), all-day events as blocks. Time-agnostic.
- **Painter properties**:
  - `IsTimedView = false`, `IsTimelineView = false`, `IsResourceView = true`, `IsMonthGrid = false`
  - `RequiresLeftGutter = false` (row header), `RowHeaderWidth = 160`
  - `VisibleDayCount = 7` (default for week), `MaxVisibleDays = 14`
- **Surface helpers needed**: same as A5 (`GetResourceRowRect`, `GetResourceHeaderRect`).
- **Helpers needed**: `PaintResourceHeader`, `PaintDayGridCell`, `PaintAllDayEventBlock`.
- **W8 cell-component integration**: event blocks per (resource, day); date cells per day.
- **Navigation**: `±7 days` (week) or `±N days`.
- **Hit-test**: event blocks; resource row headers.
- **Build order**: W11 (medium priority).

### B1. Month (DISTINCT — own painter, EXISTING)

- **Source**: FullCalendar `dayGridMonth`, DevExtreme Month, Teamup Month, Kendo Month.
- **Sample**: c4.
- **Layout**: 6 rows × 7 columns of date cells, day number top-left, event bars bottom-stacked, weekend cells shaded, out-of-month cells greyed, today highlighted.
- **Painter properties**:
  - `IsMonthGrid = true`, `VisibleDayCount = 7`, `IsTimedView = false`, `RequiresLeftGutter = false`
  - `ShowWeekNumbers = false` (optional)
- **Surface helpers needed**: `surface.GetMonthCellRect(date)`, `surface.GetMonthCellEventRect(date, eventIndex)`.
- **W8 cell-component integration**: event bars per (cell, event); date cells per (cell).
- **Build order**: W2 (DONE). Keep `MonthViewPainter` as-is.

### B2. DayGrid Week (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `dayGridWeek`, Teamup Week (all-day variant), Material Design 3 (week view).
- **Layout**: 1×7 day columns, each column is a tall block (no time rows). All-day events stack vertically inside each column. No time labels.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 7`
  - `RequiresLeftGutter = false` (no time gutter)
  - `HasAllDayStrip = true` (the whole view IS the all-day strip)
- **Surface helpers needed**: `GetWeekDayColumnRect(day)`, new `GetAllDayEventRect(column, eventIndex)`.
- **W8 cell-component integration**: event blocks per (day, event).
- **Navigation**: `±7 days`.
- **Hit-test**: event blocks; column = `DateCell`.
- **Build order**: W10 (high priority — common for "show me the week without times").

### B3. DayGrid Day (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `dayGridDay`, Google Calendar "Day" without times.
- **Layout**: 1 single day block (full height), all-day events stacked vertically.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 1`
  - `MaxVisibleDays = 1`
- **W8 cell-component integration**: event blocks per (day, event).
- **Build order**: W10 (high priority).

### B4. Multi-Month (Year / 2×2 / 3×4) (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `multiMonthYear` (default 2×6 mini-months), Material 3 Year picker (3×4 mini-months).
- **Layout**: K months laid out in a grid (2×2, 2×3, 3×4, 4×3, 2×6, 1×12). Each mini-month is a tiny 6×7 grid showing the day numbers + a small "has events" indicator (dot or count). Read-only browse.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 31` (the max month length)
  - `MaxVisibleDays = 366` (full year, if 1×12)
  - `IsReadOnly = true` (no event edit on the mini-month; click to navigate to that month)
  - `ShowWeekNumbers = false`
- **Surface helpers needed (NEW)**: `GetMultiMonthCellRect(monthIndex, weekIndex, dayIndex)`, `GetMultiMonthTitleRect(monthIndex)`, `GetMultiMonthBounds(monthIndex)`.
- **Helpers needed (NEW)**: `PaintMiniMonth(g, bounds, monthDate, eventsByDate, args)`, `PaintMonthGridLine(g, bounds, args)`.
- **W8 cell-component integration**: not relevant (read-only).
- **Navigation**: `±1 year` (default 2×6), `±2 years` (1×12), `±N months` (configurable).
- **Hit-test**: click on a mini-month day = `DateCell`; on the title = `MiniMonthHeader`.
- **Build order**: W10 (high priority — common for date pickers).

### B5. Quarter (DISTINCT — own painter, NEW)

- **Source**: Teamup (3 mini-months), SAP Fiori `Months` (3 months), accounting/finance apps.
- **Layout**: 3 mini-months in a single horizontal row showing the current quarter. Read-only browse; click to navigate to that month.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 31`
  - `MaxVisibleDays = 92` (full quarter)
  - `IsReadOnly = true`
- **Surface helpers needed**: same as B4.
- **Build order**: W10 (high priority for finance/business apps).

### B6. Year (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `dayGridYear` (3×4 mini-months), Material 3 Year Picker, Preline Calendar Year, Teamup Year, SVAR year view.
- **Layout**: 12 months as mini-months in a 3×4 (default) or 4×3 grid. Each mini-month shows day numbers + event-count badge. Read-only browse; click to navigate to that month.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 31`
  - `MaxVisibleDays = 366`
  - `IsReadOnly = true`
  - `Key = "year"`, `DisplayLabel = "Year"`
- **Surface helpers needed**: same as B4.
- **W8 cell-component integration**: not relevant (read-only).
- **Navigation**: `±1 year`.
- **Build order**: W10 (high priority — completes the standard "Day / Week / Month / Year" quad).

### B7. Resource DayGrid Month (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `resourceDayGridMonth`.
- **Layout**: resources as rows, days as columns (full month = ~31 columns). Very wide; horizontal scroll. All-day events as blocks. Optional resource group headers.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `IsResourceView = true`
  - `RequiresLeftGutter = false`, `RowHeaderWidth = 160`
  - `MaxVisibleDays = 31`
- **Surface helpers needed**: `GetResourceRowRect`, `GetResourceHeaderRect`, new `GetMonthDayColumnRect(dayIndex)` (31 columns, narrow).
- **Build order**: W11 (medium).

### B8. Resource DayGrid Week (DISTINCT — own painter, NEW)

- **Source**: FullCalendar `resourceDayGridWeek`.
- **Layout**: resources as rows, 7 days as columns.
- **Painter properties**:
  - `IsResourceView = true`, `VisibleDayCount = 7`
  - `MaxVisibleDays = 7`
- **Build order**: W11 (medium).

### C1. Agenda (single day) (DISTINCT — own painter, NEW)

- **Source**: DevExtreme Agenda (single-day variant), Google Calendar "Schedule" view.
- **Layout**: one day, all events listed vertically grouped by hour. No time grid lines.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 1`
- **W8 cell-component integration**: event rows per (hour, event).
- **Build order**: W10 (high priority — common for "today's schedule" view).

### C2. Agenda (multi-day) (DISTINCT — own painter, EXISTING)

- **Source**: FullCalendar `listWeek`, Teamup Agenda, Material 3 "Schedule".
- **Sample**: not provided; designed from spec.
- **Layout**: events for N days (default 30), grouped by day, vertical scroll. Day header for each day; events as rows beneath.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 30`
  - `MaxVisibleDays = 365`
  - `AllowMultiSelection = false`
- **Build order**: W2 (DONE). Keep `AgendaViewPainter` as-is.

### C3-C6. List Day / Week / Month / Year (DISTINCT — own painters, NEW)

- **Source**: FullCalendar `listDay`, `listWeek`, `listMonth`, `listYear`.
- **Layout**: same as C2 (multi-day agenda) but with a more literal "list" presentation: no time grouping, just chronological rows with date-time + title + description. Optionally grouped by day.
- **Painter properties**:
  - `IsTimedView = false`, `VisibleDayCount = 1 / 7 / 31 / 365`
  - `MaxVisibleDays = 365`
- **Build order**: W10 (high priority — standard office-app requirement).

### C7. Tiles (DISTINCT — own painter, NEW)

- **Source**: Teamup Tiles.
- **Layout**: events as a responsive card grid (CSS-like wrapping). Each card shows title + time + category color. No time grid. Read-only or with click-to-edit.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 0` (no day axis)
  - `MaxVisibleDays = 365`
  - `IsReadOnly = true` (default)
- **Surface helpers needed (NEW)**: `GetTileRect(tileIndex)`, `GetTilesPerRow(width)`.
- **Helpers needed (NEW)**: `PaintEventCard(g, rect, evt, args)`.
- **Build order**: W12 (low priority — niche; Teamup-specific).

### C8. Day Card (DISTINCT — own painter, NEW)

- **Source**: Things 3, Fantastical daily view.
- **Layout**: 1 day, events as large vertical cards (time + title + notes). Minimalist.
- **Painter properties**:
  - `IsTimedView = false`, `VisibleDayCount = 1`
- **Build order**: W12 (low priority — personal-app aesthetic).

### D1-D4. Timeline Day / Week / Month / Year (DISTINCT — own painters, NEW)

- **Source**: FullCalendar `timelineDay`, `timelineWeek`, `timelineMonth`, `timelineYear`. DevExtreme Timeline. SVAR Timeline.
- **Layout**: resources as rows (configurable; default = categories or all events grouped by category), time as horizontal axis (1 day / 7 days / ~31 days / 12 months). Event bars span the time axis horizontally. Optional resource group headers. Optional sticky first column.
- **Painter properties**:
  - `IsTimedView = true` (events still have times), `IsTimelineView = true` (time axis is horizontal), `IsResourceView = true`
  - `RequiresLeftGutter = false`, `RowHeaderWidth = 160`
  - `VisibleDayCount = 1 / 7 / 31 / 365`, `MaxVisibleDays = 365`
- **Surface helpers needed (NEW)**:
  - `GetResourceRowRect(resourceIndex)` — rect for the resource row lane
  - `GetResourceHeaderRect()` — sticky left column for resource names
  - `GetTimelineColumnRect(timeIndex)` — rect for the time-axis column
  - `GetTimelineHeaderRect()` — sticky top row for time axis labels
  - `GetTimelineEventBarRect(resourceIndex, evt)` — horizontal event bar rect
- **Helpers needed (NEW)**:
  - `PaintResourceHeader(g, rect, resource, args)`
  - `PaintTimelineAxis(g, rect, scale, args)` — hour / day / month / year tick marks
  - `PaintHorizontalEventBar(g, rect, evt, args)` — event bar with left/right resize edges
  - `PaintResourceGroupHeader(g, rect, groupName, args)`
- **W8 cell-component integration**: event bars try `TryDrawCellComponent` with key `"evt:{id}"`; resource rows try with key `"res:{resourceId}"`; time-axis columns try with key `"tcol:{timeIndex}"`.
- **Navigation**: `±1 day` (D1), `±7 days` (D2), `±1 month` (D3), `±1 year` (D4).
- **Hit-test**: event bars check `rect.Contains`; resource row headers = `ResourceRow`; timeline columns = `DateCell`.
- **Drag/resize**: horizontal move + horizontal resize (left/right).
- **Data model**: events need `ResourceId`.
- **Build order**: W11 (medium — common for project / resource scheduling).

### D5. Resource Timeline Day/Week/Month/Year (DISTINCT — own painters, NEW)

- **Source**: FullCalendar `resourceTimelineDay`, `resourceTimelineWeek`, `resourceTimelineMonth`, `resourceTimelineYear`.
- **Layout**: same as D1-D4 but with explicit resource grouping (collapse/expand groups, group headers, multiple groups).
- **Painter properties**:
  - `IsResourceView = true`, `IsTimelineView = true`
  - adds `GroupBy` (resource field name) — extends `ICalendarViewPainter` with `ResourceGroupField` (NEW) returning the property name to group by.
- **Build order**: W11 (medium).

### D6. Scheduler (DISTINCT — own painter, NEW)

- **Source**: Teamup Scheduler, Kendo Scheduler (column-resource mode).
- **Layout**: resources as columns, time as horizontal axis (1 day, 24 hours). Event bars are vertical per-column stacks. Often used for room/equipment scheduling.
- **Painter properties**:
  - `IsResourceView = true`, `IsTimelineView = true`
  - same as D1 but rotated 90° (resource column instead of resource row).
- **Build order**: W12 (low priority — Teamup-specific).

### E1-E6. Gantt Hours / Days / Weeks / Months / Quarters / Years (DISTINCT — own painters, NEW)

- **Source**: Kendo Gantt, DevExtreme Gantt, bluemill Gantt, SVAR Gantt, Telerik Gantt.
- **Layout**: tasks as rows, time as horizontal axis at a configurable scale (hours, days, weeks, months, quarters, years). Task bars span start-to-end. Sub-tasks indent under parents. Milestones = diamond. Progress = filled bar. Critical path = red bar. Baselines = ghost bar.
- **Painter properties**:
  - `IsResourceView = false` (rows are TASKS, not resources), `IsTimelineView = true` (horizontal time axis)
  - `IsTimedView = false` (no hour-level snapping; just time range)
  - `RequiresLeftGutter = false`, `RowHeaderWidth = 200` (wider for task name + indent)
  - `SupportsTaskHierarchy = true` (parent/child indent + collapse)
  - `SupportsDependencies = true` (FS / SS / FF / SF arrows)
  - `MaxVisibleDays = 365` (or 3650 for years scale)
- **Surface helpers needed (NEW)**:
  - `GetTaskRowRect(taskIndex, isCollapsed)` — vertical rect for a task row
  - `GetTaskBarRect(task, timeStart, timeEnd)` — horizontal bar
  - `GetDependencyArrowPath(sourceTask, targetTask, dependencyType)` — Bezier path between two task bars
  - `GetMilestoneRect(task)` — diamond rect for milestone
  - `GetProgressFillRect(taskBar)` — filled portion for progress
  - `GetBaselineRect(task, baseline)` — ghost bar for baseline
  - `GetIndentLevel(task)` — visual indent (e.g. 16px per level)
- **Helpers needed (NEW)**:
  - `PaintTaskBar(g, rect, task, args)`
  - `PaintTaskHierarchy(g, bounds, tasks, args)` — with collapse carets
  - `PaintMilestone(g, rect, task, args)`
  - `PaintProgress(g, barRect, percent, args)`
  - `PaintDependencyArrow(g, path, args)` — arrow with arrowhead
  - `PaintBaseline(g, rect, baseline, args)` — semi-transparent
  - `PaintCriticalPath(g, tasks, args)` — red overlay for critical path
  - `PaintGanttAxis(g, rect, scale, args)` — hour / day / week / month / quarter / year ticks
- **W8 cell-component integration**: task bars try `TryDrawCellComponent` with key `"task:{id}"`; dependency arrows try `"dep:{sourceId}:{targetId}"`.
- **Navigation**: `±1 hour` (E1), `±1 day` (E2), `±1 week` (E3), `±1 month` (E4), `±1 quarter` (E5), `±1 year` (E6).
- **Hit-test**: task bars = `TaskBar`; milestones = `Milestone`; dependency arrows = `DependencyArrow` (hot region = path stroke width + 4px).
- **Drag/resize**: horizontal move (start-to-end); horizontal resize (left/right); vertical reorder (move task up/down).
- **Hierarchy interactions**: click collapse caret = toggle children; right-click = context menu.
- **Read-only**: false.
- **Data model (NEW)**:
  - `CalendarEvent.ResourceId` — task parent (optional; for resource Gantt)
  - `CalendarEvent.ParentId` — task parent (for hierarchy)
  - `CalendarEvent.Progress` (0-100) — progress fill
  - `CalendarEvent.TaskType` (Task / Milestone / Summary) — new `CalendarTaskType` enum
  - `CalendarEvent.IsCollapsed` — for hierarchy rendering
  - `CalendarEvent.BaselineStart`, `CalendarEvent.BaselineEnd` — for baseline overlay
  - `CalendarEvent.IsCritical` — for critical path
  - `CalendarEvent.Dependencies` — `List<CalendarTaskDependency>` (SourceId, TargetId, Type)
  - `CalendarEvent.StartTime`, `CalendarEvent.EndTime` — the task bar range
  - **NEW**: `CalendarTaskDependency` class
  - **NEW**: `CalendarTaskDependencyType` enum (FS, SS, FF, SF)
- **Build order**: W12 (low priority — specialized; project management apps only).

### F1. Mini Month (DISTINCT — own painter, NEW)

- **Source**: Sidebar in Google Calendar / Outlook / Apple Calendar.
- **Layout**: small 6×7 month grid (~200×200 px). Day numbers + small "has events" dot. Read-only browse; click to navigate the main view to that date.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = true` (it IS a month grid, just smaller)
  - `VisibleDayCount = 7`
  - `IsReadOnly = true`
- **Surface helpers needed**: existing `GetMonthCellRect` works at smaller scale.
- **Build order**: W12 (low priority — sidebar widget).

### F2. Heatmap Year (DISTINCT — own painter, NEW)

- **Source**: GitHub contribution graph, calendar heatmaps in analytics dashboards.
- **Layout**: 365 colored cells (52 weeks × 7 days) for one year. Color intensity = number of events on that day. Hover = tooltip with date + count.
- **Painter properties**:
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 7` (one row = one week)
  - `MaxVisibleDays = 366`
  - `IsReadOnly = true`
  - `ShowHeatmap = true`
- **Surface helpers needed (NEW)**: `GetHeatmapCellRect(weekIndex, dayOfWeek)`, `GetHeatmapIntensity(count, maxCount) → Color` (gradient from light to primary).
- **Helpers needed (NEW)**: `PaintHeatmapCell(g, rect, intensity, args)`, `PaintHeatmapMonthLabel(g, rect, monthName, args)`, `PaintHeatmapDayOfWeekLabel(g, rect, dayName, args)`.
- **W8 cell-component integration**: not relevant (read-only).
- **Build order**: W12 (low priority — niche but distinctive).

### F3. Empty / No Events (DISTINCT — own painter, NEW)

- **Source**: standard empty-state pattern.
- **Layout**: blank area with a friendly illustration + "No events. Click + to create one" message + a centered "+" button.
- **Painter properties**:
  - `IsReadOnly = false`
  - `IsTimedView = false`, `IsMonthGrid = false`, `VisibleDayCount = 0`
- **Build order**: W12 (low priority — UX polish).

## 5. Surface Model Extensions (NEW for W9)

`CalendarSurfaceModel` will need new helper methods to support the new families. Each is a method, not a property:

```csharp
// Resource views (A5, A6, B7, B8, D1-D6)
Rectangle GetResourceRowRect(int resourceIndex, int totalResources);
Rectangle GetResourceHeaderRect();
int GetResourceIndexFromY(int y);

// Timeline views (A5, D1-D6, E1-E6)
Rectangle GetTimelineColumnRect(int timeIndex, int totalColumns);
Rectangle GetTimelineHeaderRect();
int GetTimeIndexFromX(int x);

// Gantt views (E1-E6)
Rectangle GetTaskRowRect(int taskIndex, int totalTasks);
Rectangle GetTaskBarRect(CalendarEvent task, DateTime rangeStart, DateTime rangeEnd);
Rectangle GetMilestoneRect(CalendarEvent task, DateTime rangeStart, DateTime rangeEnd);
GraphicsPath GetDependencyArrowPath(CalendarEvent source, CalendarEvent target, DateTime rangeStart, DateTime rangeEnd);
int GetIndentLevel(CalendarEvent task);

// Multi-month (B4-B6)
Rectangle GetMultiMonthCellRect(int monthIndex, int weekIndex, int dayIndex, int monthColumns, int monthRows);
Rectangle GetMultiMonthTitleRect(int monthIndex, int monthColumns, int monthRows);

// Heatmap (F2)
Rectangle GetHeatmapCellRect(int weekIndex, DayOfWeek dayOfWeek);
Color GetHeatmapIntensity(int eventCount, int maxCount);

// Tile (C7)
Rectangle GetTileRect(int tileIndex, int tilesPerRow);
```

## 6. Helper Extensions (NEW for W9)

`CalendarPainterHelpers` will need new static methods:

```csharp
// Resource headers (A5, A6, B7, B8, D1-D6)
public static void PaintResourceHeader(Graphics g, Rectangle rect, CalendarResource resource, ViewPaintArgs args);
public static void PaintResourceGroupHeader(Graphics g, Rectangle rect, string groupName, bool isCollapsed, ViewPaintArgs args);

// Timeline + Gantt
public static void PaintTimelineAxis(Graphics g, Rectangle rect, TimelineScale scale, ViewPaintArgs args);
public static void PaintHorizontalEventBar(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args);
public static void PaintHorizontalTimeGrid(Graphics g, Rectangle bounds, int hourCount, ViewPaintArgs args);

// Gantt
public static void PaintTaskBar(Graphics g, Rectangle rect, CalendarEvent task, ViewPaintArgs args);
public static void PaintMilestone(Graphics g, Rectangle rect, CalendarEvent task, ViewPaintArgs args);
public static void PaintProgress(Graphics g, Rectangle barRect, int percent, ViewPaintArgs args);
public static void PaintDependencyArrow(Graphics g, GraphicsPath path, ViewPaintArgs args);
public static void PaintBaseline(Graphics g, Rectangle rect, ViewPaintArgs args);
public static void PaintCriticalPath(Graphics g, IEnumerable<CalendarEvent> criticalTasks, ViewPaintArgs args);

// Multi-month
public static void PaintMiniMonth(Graphics g, Rectangle bounds, DateTime monthDate, IReadOnlyDictionary<DateTime, int> eventCounts, ViewPaintArgs args);

// Heatmap
public static void PaintHeatmapCell(Graphics g, Rectangle rect, Color intensity, ViewPaintArgs args);

// Tile
public static void PaintEventCard(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args);
```

## 7. Data Model Extensions (NEW for W9)

`CalendarEvent` (already in `Helpers/CalendarEvent.cs`) needs:

```csharp
// New properties (with sensible defaults so existing code keeps working)
public int? ResourceId { get; set; }              // for resource views (A5, A6, B7, B8, D1-D6)
public int? ParentId { get; set; }                // for Gantt hierarchy (E1-E6)
public int Progress { get; set; }                 // 0..100, for Gantt (E1-E6)
public CalendarTaskType TaskType { get; set; }    // for Gantt (E1-E6) — Task / Milestone / Summary
public bool IsCollapsed { get; set; }             // for Gantt hierarchy (E1-E6)
public DateTime? BaselineStart { get; set; }      // for Gantt baseline (E1-E6)
public DateTime? BaselineEnd { get; set; }        // for Gantt baseline (E1-E6)
public bool IsCritical { get; set; }              // for Gantt critical path (E1-E6)
public List<CalendarTaskDependency> Dependencies { get; set; } = new(); // for Gantt (E1-E6)
```

**NEW classes:**

```csharp
// Helpers/CalendarTaskDependency.cs
public sealed class CalendarTaskDependency
{
    public int SourceId { get; set; }
    public int TargetId { get; set; }
    public CalendarTaskDependencyType Type { get; set; }
}

// Helpers/CalendarTaskType.cs
public enum CalendarTaskType { Task, Milestone, Summary }

// Helpers/CalendarTaskDependencyType.cs
public enum CalendarTaskDependencyType { FinishToStart, StartToStart, FinishToFinish, StartToFinish }
```

`CalendarResource` already exists (per `ViewPaintArgs.Resources`).

## 8. ViewMode Enum Extensions (NEW for W9)

Add new enum values to `CalendarViewMode` (keep legacy aliases per W2-Redo follow-up decision; do not break the existing numeric values):

```csharp
public enum CalendarViewMode
{
    // Existing canonical (W2-Redo)
    Week1 = 0, Week2 = 1, Week3 = 2, Week4 = 3, Week5 = 4, Week6 = 5, Week7 = 6,

    // Family A additions (W10)
    DayGridWeek = 10,        // A3.1 / B2 — full-day 7-day grid
    DayGridDay = 11,         // B3
    MultiDay = 12,           // A4 — 1-14 day columns

    // Family B additions (W10)
    MultiMonth = 20,         // B4 — 2×2 / 3×4 / 4×3 / 1×12 of full months
    Quarter = 21,            // B5
    Year = 22,               // B6

    // Family C additions (W10)
    ListDay = 30,            // C3
    ListWeek = 31,           // C4
    ListMonth = 32,          // C5
    ListYear = 33,           // C6
    AgendaDay = 34,          // C1
    Tiles = 35,              // C7
    DayCard = 36,            // C8

    // Family D additions (W11)
    TimelineDay = 40,        // D1
    TimelineWeek = 41,       // D2
    TimelineMonth = 42,      // D3
    TimelineYear = 43,       // D4
    ResourceTimelineDay = 44,    // D5
    ResourceTimelineWeek = 45,
    ResourceTimelineMonth = 46,
    ResourceTimelineYear = 47,
    ResourceTimeGrid = 48,   // A5
    ResourceDayGridWeek = 49,    // A6 / B8
    ResourceDayGridMonth = 50,   // B7
    Scheduler = 51,          // D6

    // Family E additions (W12)
    GanttHours = 60,         // E1
    GanttDays = 61,          // E2
    GanttWeeks = 62,         // E3
    GanttMonths = 63,        // E4
    GanttQuarters = 64,      // E5
    GanttYears = 65,         // E6

    // Family F additions (W12)
    MiniMonth = 70,          // F1
    HeatmapYear = 71,        // F2
    Empty = 72,              // F3

    // Legacy aliases (kept for backward compat — W2-Redo-3 will remove)
    Month = 100, Week = 101, WorkWeek = 102, Day = 103,
    Agenda = 104, Timeline = 105, List = 106,
}
```

## 9. Build Order (Priorities)

### W10 (HIGH) — Standard Office-App Quad + Lists

Closes the gap on the most common office-app requirement: Day / Week / Month / Year + List variants.

1. `DayGridWeekViewPainter.cs` (B2)
2. `DayGridDayViewPainter.cs` (B3)
3. `MultiDayViewPainter.cs` (A4) — 1-14 day timed grid
4. `YearViewPainter.cs` (B6) — 12 mini-months
5. `QuarterViewPainter.cs` (B5) — 3 mini-months
6. `MultiMonthViewPainter.cs` (B4) — 2×2 / 3×4 of full months
7. `ListDayViewPainter.cs` (C3)
8. `ListWeekViewPainter.cs` (C4)
9. `ListMonthViewPainter.cs` (C5)
10. `ListYearViewPainter.cs` (C6)
11. `AgendaDayViewPainter.cs` (C1)

### W11 (MEDIUM) — Resource + Timeline

Closes the gap on team-scheduling apps: project boards, room schedulers, equipment booking.

12. `TimelineDayViewPainter.cs` (D1)
13. `TimelineWeekViewPainter.cs` (D2)
14. `TimelineMonthViewPainter.cs` (D3)
15. `TimelineYearViewPainter.cs` (D4)
16. `ResourceTimelineDayViewPainter.cs` (D5a)
17. `ResourceTimelineWeekViewPainter.cs` (D5b)
18. `ResourceTimelineMonthViewPainter.cs` (D5c)
19. `ResourceTimeGridViewPainter.cs` (A5)
20. `ResourceDayGridWeekViewPainter.cs` (A6)
21. `ResourceDayGridMonthViewPainter.cs` (B7)

### W12 (LOW) — Gantt + Specialized

Specialized for project-management apps and niche UX patterns.

22. `GanttWeeksViewPainter.cs` (E3) — start with weeks scale (most common)
23. `GanttDaysViewPainter.cs` (E2)
24. `GanttMonthsViewPainter.cs` (E4)
25. `GanttQuartersViewPainter.cs` (E5)
26. `GanttYearsViewPainter.cs` (E6)
27. `GanttHoursViewPainter.cs` (E1)
28. `TilesViewPainter.cs` (C7)
29. `SchedulerViewPainter.cs` (D6)
30. `MiniMonthViewPainter.cs` (F1)
31. `HeatmapYearViewPainter.cs` (F2)
32. `DayCardViewPainter.cs` (C8)
33. `EmptyViewPainter.cs` (F3)

## 10. Migration Plan (when starting W10)

1. **Add interface members** to `ICalendarViewPainter` (Section 3). Add a new `CalendarViewPainterBase` with default implementations so new painters can override only what's distinct.
2. **Add new enum values** to `CalendarViewMode` (Section 8). Keep legacy aliases.
3. **Extend `CalendarSurfaceModel`** with the new geometry helpers (Section 5). Each helper is a method on the model.
4. **Extend `CalendarPainterHelpers`** with the new draw helpers (Section 6). Each helper is a public static method.
5. **Extend `CalendarEvent`** with the new properties (Section 7). Add the new supporting classes.
6. **Update `ViewPainterFactory`** to map new enum values to new painters. Legacy aliases keep routing.
7. **For each painter in W10-W12**: create `Rendering/ViewPainters/{Name}ViewPainter.cs` (DISTINCT — self-contained, no shared shim files). Wire W8 `TryDrawCellComponent` for each new cell kind. Update factory + tests + sample.
8. **Build + tests** after each painter: 0 errors, 0 new warnings.

## 11. Per-Painter Build Checklist (template for each W10+ painter)

For each new view painter:

- [ ] Create `Rendering/ViewPainters/{Name}ViewPainter.cs` as a self-contained class (no shared shim files)
- [ ] Declare `ViewMode` to a new enum value
- [ ] Implement all `ICalendarViewPainter` members using the override pattern (only override what's distinct)
- [ ] Add `Layout(ViewPaintArgs args)` body (sets up surface-level state if needed; usually no-op)
- [ ] Add `Paint(Graphics g, ViewPaintArgs args)` body (uses `surface.*` + `CalendarPainterHelpers.*`)
- [ ] Add `HitTest(Point location, ViewPaintArgs args)` body (returns proper `CalendarInteractionTargetKind`)
- [ ] Add `GetDateTimeFromLocation(Point location, ViewPaintArgs args)` body (returns null if not applicable)
- [ ] Add `NavigatePrevious` / `NavigateNext` (per view family)
- [ ] Add `GetHeaderText` (per view family)
- [ ] Add `GetVisibleRangeStart` / `GetVisibleRangeEnd` (per view family)
- [ ] Add W8 integration: call `CalendarPainterHelpers.TryDrawCellComponent` for each new cell kind
- [ ] Add new enum value to `CalendarViewMode` (Section 8)
- [ ] Update `ViewPainterFactory.GetPainter` to map new enum value
- [ ] Update `ViewPainterFactory.GetRegisteredViews` to include the new view (if it should show in toolbar)
- [ ] Add new geometry helper(s) to `CalendarSurfaceModel` if needed
- [ ] Add new draw helper(s) to `CalendarPainterHelpers` if needed
- [ ] Extend `ViewPaintArgs` if new event service / interaction data is needed
- [ ] Update `BeepCalendar.Toolbar.cs` if `GetRegisteredViews` is the source
- [ ] Add an entry in `Calendar/sampleimages/` (if a sample image was provided)
- [ ] Add a section to this plan file (W9) with the actual layout sketch
- [ ] Add a row to the validation matrix in `phase2-validation-matrix.md`
- [ ] Build: 0 errors, 0 new warnings
- [ ] Update TODO-TRACKER: mark the new view's box as done

## 12. Sample Image Mapping (current)

| Sample | View mode | Painter | Status |
|--------|-----------|---------|--------|
| c1.png | Week1 (7-day timed + left sidebar) | `Week1ViewPainter.cs` | DONE (W2-Redo) |
| c2.png | Week2 (7-day timed + right detail panel) | `Week2ViewPainter.cs` | DONE (W2-Redo) |
| c3.png | Week3 (4-day timed + filter bar) | `Week3ViewPainter.cs` | DONE (W2-Redo) |
| c4.png | Week4 (6×7 month grid + right detail panel) | `Week4ViewPainter.cs` | DONE (W2-Redo) |
| c5.png | Week5 (7-day event-card columns + day-of-week tabs) | `Week5ViewPainter.cs` | DONE (W2-Redo) |
| c6.png | Week6 (6-day event-card columns in time order) | `Week6ViewPainter.cs` | DONE (W2-Redo) |
| c7.png | Week7 (7-day timed + filter bar + status badges) | `Week7ViewPainter.cs` | DONE (W2-Redo) |

No new sample images for the W9 views yet. They'll be added as each view is built.

## 13. Cross-References

- W2-Redo-2 (inline legacy painters, no shared shim) — COMPLETE 2026-06-04. This plan is the next wave.
- W5 (verification demo + tests) — adds a test case per new view family.
- Phase 3 (advanced views, resources, performance) — W9 implements most of Phase 3's view layer.
- Phase 4 (integrations, accessibility, enterprise) — F4 (Print view) lives in Phase 4.
