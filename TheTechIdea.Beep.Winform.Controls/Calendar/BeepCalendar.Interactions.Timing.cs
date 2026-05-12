using System;
using System.Collections.Generic;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private IReadOnlyList<CalendarEvent> GetCurrentInteractionConflicts(Point location, Point delta)
        {
            var proposedStart = BuildProposedStart(location, delta);
            var proposedEnd = BuildProposedEnd(location, delta);

            if (_state.SelectedEvent == null)
            {
                if (_state.InteractionMode != CalendarInteractionMode.CreateEvent && _state.InteractionMode != CalendarInteractionMode.RangeSelect)
                {
                    return Array.Empty<CalendarEvent>();
                }

                if (!proposedStart.HasValue || !proposedEnd.HasValue)
                {
                    return Array.Empty<CalendarEvent>();
                }

                var candidate = new CalendarEvent
                {
                    Id = 0,
                    StartTime = proposedStart.Value,
                    EndTime = proposedEnd.Value,
                    Title = "Preview",
                    Status = CalendarEventStatus.Tentative
                };
                NormalizeEventDuration(candidate);
                return AnalyzeConflicts(candidate).Conflicts;
            }

            var preview = CloneEvent(_state.SelectedEvent);
            preview.StartTime = proposedStart ?? preview.StartTime;
            preview.EndTime = proposedEnd ?? preview.EndTime;
            NormalizeEventDuration(preview);
            return AnalyzeConflicts(preview).Conflicts;
        }

    }
}