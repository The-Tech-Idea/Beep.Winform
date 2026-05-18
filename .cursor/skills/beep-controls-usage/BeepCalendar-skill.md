# BeepCalendar Skill

## Overview
`BeepCalendar` is a full-featured calendar control that inherits `BaseControl`. It supports 7 view modes, event CRUD with undo/redo, conflict policies, drag-and-resize interactions, style painters, and a pluggable event editor. Default size is 800×600.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Calendar;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers; // CalendarResource, CalendarConflictResult
```

## Class Declaration
```csharp
[ToolboxItem(true)]
[Category("Beep Controls")]
[DisplayName("Beep Calendar")]
public partial class BeepCalendar : BaseControl
```

---

## Enums

### `CalendarViewMode`
```
Month | Week | WorkWeek | Day | Agenda | Timeline | List
```

### `CalendarDensityMode`
```
Compact | Comfortable   (default: Comfortable)
```

### `CalendarEventStatus`
```
Tentative | Confirmed | Cancelled   (default: Confirmed)
```

### `CalendarConflictPolicyMode`
```
AllowOverlap | WarnOnOverlap | PreventOverlap   (default: AllowOverlap)
```

### `CalendarRecurrenceFrequency`
```
None | Daily | Weekly | Monthly | Yearly   (default: None)
```

### `CalendarEventMutationKind`
```
Create | Move | ResizeStart | ResizeEnd | Copy | Update | Delete
```

### `CalendarCommandType`
```
GoToToday | NavigatePrevious | NavigateNext | SwitchView | SetVisibleRange
| UndoMutation | RedoMutation | DeleteSelectedEvent | EditSelectedEvent
| CreateEventAtFocusedDate | DuplicateSelectedEvent
```

### `BeepControlStyle` (for `CalendarStyle`)
Material3 (default), Material, MaterialYou, Minimal, NotionMinimal, VercelClean

---

## Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CurrentDate` | `DateTime` | Today | The date the calendar is navigated to |
| `ViewMode` | `CalendarViewMode` | Month | Active view |
| `ShowSidebar` | `bool` | false | Show mini-calendar sidebar |
| `DensityMode` | `CalendarDensityMode` | Comfortable | Layout density |
| `Events` | `List<CalendarEvent>` | `[]` | Bound event list; setter rebuilds event service |
| `Categories` | `List<EventCategory>` | `[]` | Event categories |
| `Resources` | `List<CalendarResource>` | `[]` | Resources for timeline/resource views |
| `ConflictPolicyMode` | `CalendarConflictPolicyMode` | AllowOverlap | Conflict enforcement |
| `InteractionSnapIntervalMinutes` | `int` | 15 | Drag/resize snap granularity |
| `CalendarStyle` | `BeepControlStyle` | Material3 | Visual painter style |
| `UsePainterSystem` | `bool` | true | Use the new style-painter rendering system |
| `EventEditor` | `ICalendarEventEditor` | `CalendarEventEditor` | Pluggable editor; replaced for custom UI |
| `CanUndo` | `bool` | — | Read-only; true when undo stack is non-empty |
| `CanRedo` | `bool` | — | Read-only; true when redo stack is non-empty |
| `PerformanceMetrics` | `CalendarPerformanceMetrics` | — | Read-only render metrics |

### Font Properties

| Property | Default |
|----------|---------|
| `HeaderFont` | Segoe UI 16pt Bold |
| `DayFont` | Segoe UI 12pt |
| `EventFont` | Segoe UI 9pt |
| `TimeFont` | Segoe UI 10pt |
| `DaysHeaderFont` | Segoe UI 10pt Medium (private set) |

---

## Events

| Event | Args | Description |
|-------|------|-------------|
| `EventSelected` | `CalendarEventArgs` | User clicks an event |
| `EventDoubleClick` | `CalendarEventArgs` | User double-clicks an event |
| `DateSelected` | `CalendarDateArgs` | User clicks a date cell |
| `CreateEventRequested` | `CalendarEventArgs` | User activates create action |
| `ConflictDetected` | `CalendarConflictEventArgs` | Conflict found per policy |
| `EventMutating` | `CalendarEventMutationEventArgs` | Before mutation — set `Cancel=true` to block |
| `EventMutated` | `CalendarEventMutationEventArgs` | After successful mutation |
| `CommandInvoking` | `CalendarCommandEventArgs` | Before command executes — set `Cancel=true` to block |
| `CommandInvoked` | `CalendarCommandEventArgs` | After command completes |

### EventArgs shapes
```csharp
class CalendarEventArgs    { CalendarEvent Event; }
class CalendarDateArgs     { DateTime Date; }
class CalendarConflictEventArgs { CalendarConflictResult Result; }

class CalendarEventMutationEventArgs
{
    CalendarEventMutationKind MutationKind;
    CalendarEvent OriginalEvent, ProposedEvent, AppliedEvent;
    bool Cancel;          // set true in EventMutating to abort
    bool IsCopyOperation;
    IReadOnlyList<CalendarEvent> Conflicts;
}

class CalendarCommandEventArgs
{
    CalendarCommandType CommandType;
    DateTime?          AnchorDate;
    DateTime?          VisibleRangeEnd;
    CalendarViewMode?  TargetView;
    bool Cancel;        // set true in CommandInvoking to block
    bool Succeeded;
}
```

---

## Command Methods (Public API)

| Method | Returns | Description |
|--------|---------|-------------|
| `GoToToday()` | `bool` | Navigate to today |
| `NavigatePreviousPeriod()` | `bool` | Go back one period (period depends on ViewMode) |
| `NavigateNextPeriod()` | `bool` | Go forward one period |
| `SwitchView(CalendarViewMode mode)` | `bool` | Change view mode |
| `SetVisibleRange(DateTime start, DateTime end)` | `bool` | Explicitly set the visible date range |
| `UndoMutation()` | `bool` | Undo last event change |
| `RedoMutation()` | `bool` | Redo last undone change |
| `DeleteSelectedEvent()` | `bool` | Delete the currently selected event |
| `EditSelectedEvent()` | `bool` | Open editor for selected event |
| `CreateEventAtFocusedDate()` | `bool` | Create new event at focused/cursor date |
| `DuplicateSelectedEvent()` | `bool` | Duplicate the selected event |

Navigation period by view mode:
- `Month/Agenda/List` → ±1 month
- `Week/Timeline` → ±7 days
- `WorkWeek` → ±5 days
- `Day` → ±1 day

---

## CRUD Methods

| Method | Description |
|--------|-------------|
| `AddEvent(CalendarEvent)` | Add event; fires `EventMutating`/`EventMutated` |
| `TryAddEvent(CalendarEvent)` | Add with cancelable mutation; returns `false` if blocked |
| `UpdateEvent(CalendarEvent)` | Update by `Id`; fires mutation events |
| `TryUpdateEvent(CalendarEvent)` | Update with cancelable mutation; returns `false` if blocked |
| `RemoveEvent(CalendarEvent)` | Remove by `Id`; fires mutation events |

All mutations: clone the event, normalize duration, check conflict policy, push to undo stack, invalidate cache.

---

## Query Methods

```csharp
// Events within a date range
List<CalendarEvent> GetEventsForDateRange(DateTime startDate, DateTime endDate);

// Filtered search
List<CalendarEvent> SearchEvents(CalendarEventFilter filter);

// Conflict analysis (does not add the event)
CalendarConflictResult AnalyzeConflicts(CalendarEvent candidate);
```

### `CalendarEventFilter`
```csharp
class CalendarEventFilter
{
    string SearchText;                    // title/description text match
    List<int> CategoryIds;                // filter by category
    List<CalendarEventStatus> Statuses;   // filter by status
    List<string> Tags;                    // filter by tags
    DateTime? RangeStart, RangeEnd;       // date range
    bool IncludeAllDayEvents;             // default true
}
```

### `CalendarConflictResult`
```csharp
class CalendarConflictResult
{
    CalendarEvent              Candidate;
    IReadOnlyList<CalendarEvent> Conflicts;
    CalendarConflictPolicyMode PolicyMode;
    bool HasConflicts;
}
```

---

## History (Undo/Redo)

```csharp
bool UndoLastMutation();  // pops undo stack, pushes to redo
bool RedoLastMutation();  // pops redo stack, pushes to undo
bool CanUndo { get; }
bool CanRedo { get; }
```

---

## Data Models

### `CalendarEvent`
```csharp
class CalendarEvent
{
    int      Id;
    string   Title, Description, Location, Organizer;
    DateTime StartTime, EndTime;
    int      CategoryId;
    bool     IsAllDay;
    List<string> Tags;
    CalendarEventStatus Status;               // Tentative|Confirmed|Cancelled
    string   TimeZoneId, SeriesId, ParentEventId;
    string   ResourceId;
    List<string> ResourceIds;                 // multi-resource
    string   RecurrenceRule;
    CalendarRecurrenceFrequency RecurrenceFrequency;
    int      RecurrenceInterval;              // default 1
    int?     RecurrenceCount;
    DateTime? RecurrenceUntilUtc;
    List<DateTime> RecurrenceExceptions;
    int?     ReminderMinutesBeforeStart;
    Dictionary<string,string> Metadata;       // extensible key-value bag

    // Computed
    TimeSpan Duration            => EndTime - StartTime;
    bool     IsRecurring         => RecurrenceFrequency != None || RecurrenceRule != "";
    bool     IsDetachedInstance  => ParentEventId != "";

    bool OverlapsWith(CalendarEvent other);
}
```

### `EventCategory`
```csharp
class EventCategory { int Id; string Name, Description; Color Color; }
```

### `CalendarResource`
```csharp
class CalendarResource
{
    string Id, Name, GroupId, Description;
    Color  Color;
    bool   IsVisible;    // default true
    int    SortOrder;
}
```

---

## Custom Event Editor

Implement `ICalendarEventEditor` to replace the built-in dialog:

```csharp
public interface ICalendarEventEditor
{
    CalendarEventEditorMode Mode { get; }  // QuickEdit | DialogEdit
    bool TryEdit(CalendarEventEditorRequest request, out CalendarEvent editedEvent);
}

class CalendarEventEditorRequest
{
    CalendarEventEditorMode   Mode;
    CalendarEventMutationKind MutationKind;
    CalendarEvent             OriginalEvent, ProposedEvent;
    DateTime                  AnchorDate;
    Point                     Location;
    Keys                      ModifierKeys;
    bool                      IsCopyOperation;
    IReadOnlyList<CalendarEvent> Conflicts;
}
```

Assign via:
```csharp
calendar.EventEditor = new MyCustomEditor();
```

The built-in `CalendarEventEditor` opens a `BeepDialogForm` with full field editing.

---

## Usage Examples

### Minimal Setup
```csharp
var calendar = new BeepCalendar
{
    ViewMode = CalendarViewMode.Month,
    CurrentDate = DateTime.Today
};
this.Controls.Add(calendar);
calendar.Dock = DockStyle.Fill;
```

### Load Events
```csharp
calendar.Categories = new List<EventCategory>
{
    new EventCategory { Id = 1, Name = "Work",     Color = Color.SteelBlue },
    new EventCategory { Id = 2, Name = "Personal", Color = Color.SeaGreen }
};

calendar.Events = new List<CalendarEvent>
{
    new CalendarEvent
    {
        Id = 1, Title = "Team Standup",
        StartTime = DateTime.Today.AddHours(9),
        EndTime   = DateTime.Today.AddHours(9.5),
        CategoryId = 1
    }
};
```

### Handle Selection and Edit
```csharp
calendar.EventSelected += (s, e) =>
    Console.WriteLine($"Selected: {e.Event.Title}");

calendar.DateSelected += (s, e) =>
    Console.WriteLine($"Date: {e.Date:d}");

calendar.EventMutating += (s, e) =>
{
    if (e.MutationKind == CalendarEventMutationKind.Delete)
        e.Cancel = MessageBox.Show("Delete?", "", MessageBoxButtons.YesNo) != DialogResult.Yes;
};
```

### Add / Update / Remove
```csharp
// Add
var ev = new CalendarEvent { Id = 10, Title = "New", StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), CategoryId = 1 };
calendar.AddEvent(ev);

// Update
ev.Title = "Updated";
calendar.UpdateEvent(ev);

// Remove
calendar.RemoveEvent(ev);
```

### Switch View Programmatically
```csharp
calendar.SwitchView(CalendarViewMode.Week);
calendar.GoToToday();
```

### Query
```csharp
var thisWeek = calendar.GetEventsForDateRange(
    DateTime.Today.StartOfWeek(DayOfWeek.Monday),
    DateTime.Today.AddDays(7));

var workEvents = calendar.SearchEvents(new CalendarEventFilter
{
    CategoryIds = new List<int> { 1 },
    RangeStart = DateTime.Today,
    RangeEnd = DateTime.Today.AddMonths(1)
});
```

### Conflict Policy
```csharp
calendar.ConflictPolicyMode = CalendarConflictPolicyMode.WarnOnOverlap;
calendar.ConflictDetected += (s, e) =>
{
    var result = e.Result;
    if (result.HasConflicts)
        MessageBox.Show($"Overlaps with {result.Conflicts[0].Title}");
};
```

### Undo/Redo
```csharp
undoButton.Click += (s, e) => calendar.UndoMutation();
redoButton.Click += (s, e) => calendar.RedoMutation();
```

### Style & Density
```csharp
calendar.CalendarStyle = BeepControlStyle.Minimal;
calendar.DensityMode   = CalendarDensityMode.Compact;
```

### Resources (Timeline View)
```csharp
calendar.Resources = new List<CalendarResource>
{
    new CalendarResource { Id = "r1", Name = "Room A", Color = Color.Coral },
    new CalendarResource { Id = "r2", Name = "Room B", Color = Color.Teal }
};
calendar.Events = events; // each event.ResourceId = "r1" or "r2"
calendar.SwitchView(CalendarViewMode.Timeline);
```

---

## Design Tokens (reference)

| Token | Value |
|-------|-------|
| Header height | 60 px |
| View selector height | 40 px |
| Time slot height | 60 px |
| Time column width | 60 px |
| Sidebar width | 300 px |
| Max events per cell | 3 |
| Event corner radius | 6 px |

---

## Related Controls
- `BeepDatePicker` — date input field with popup calendar
- `BeepTimePicker` — time selection field
