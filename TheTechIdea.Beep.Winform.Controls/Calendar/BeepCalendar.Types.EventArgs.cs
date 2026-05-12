using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    /// <summary>
    /// Event args for calendar event selection
    /// </summary>
    public class CalendarEventArgs : EventArgs
    {
        public CalendarEvent Event { get; }

        public CalendarEventArgs(CalendarEvent calendarEvent)
        {
            Event = calendarEvent;
        }
    }

    /// <summary>
    /// Event args for calendar date selection
    /// </summary>
    public class CalendarDateArgs : EventArgs
    {
        public DateTime Date { get; }

        public CalendarDateArgs(DateTime date)
        {
            Date = date;
        }
    }

    public class CalendarConflictEventArgs : EventArgs
    {
        public CalendarConflictResult Result { get; }

        public CalendarConflictEventArgs(CalendarConflictResult result)
        {
            Result = result;
        }
    }

    public class CalendarEventFilter
    {
        public string SearchText { get; set; } = string.Empty;
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<CalendarEventStatus> Statuses { get; set; } = new List<CalendarEventStatus>();
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime? RangeStart { get; set; }
        public DateTime? RangeEnd { get; set; }
        public bool IncludeAllDayEvents { get; set; } = true;
    }
}