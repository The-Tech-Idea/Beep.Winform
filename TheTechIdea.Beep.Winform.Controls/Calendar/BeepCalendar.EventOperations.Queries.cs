using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Gets events for a date range
        /// </summary>
        public List<CalendarEvent> GetEventsForDateRange(DateTime startDate, DateTime endDate)
        {
            return _eventService?.GetEventsForDateRange(startDate, endDate) ?? new List<CalendarEvent>();
        }

        public List<CalendarEvent> SearchEvents(CalendarEventFilter filter)
        {
            return _eventService?.GetFilteredEvents(filter) ?? new List<CalendarEvent>();
        }

        public CalendarConflictResult AnalyzeConflicts(CalendarEvent candidate)
        {
            _conflictPolicy ??= new CalendarConflictPolicy(_conflictPolicyMode);
            _conflictPolicy.CanSchedule(candidate, _events, out var result);
            return result;
        }
    }
}