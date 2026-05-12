using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    /// <summary>
    /// Represents a calendar event
    /// </summary>
    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CategoryId { get; set; }
        public string Location { get; set; } = "";
        public bool IsAllDay { get; set; }
        public string Organizer { get; set; } = "";
        public List<string> Tags { get; set; } = new List<string>();
        public CalendarEventStatus Status { get; set; } = CalendarEventStatus.Confirmed;
        public string TimeZoneId { get; set; } = string.Empty;
        public string SeriesId { get; set; } = string.Empty;
        public string ParentEventId { get; set; } = string.Empty;
        public string ResourceId { get; set; } = string.Empty;
        public List<string> ResourceIds { get; set; } = new List<string>();
        public string RecurrenceRule { get; set; } = string.Empty;
        public CalendarRecurrenceFrequency RecurrenceFrequency { get; set; } = CalendarRecurrenceFrequency.None;
        public int RecurrenceInterval { get; set; } = 1;
        public int? RecurrenceCount { get; set; }
        public DateTime? RecurrenceUntilUtc { get; set; }
        public List<DateTime> RecurrenceExceptions { get; set; } = new List<DateTime>();
        public int? ReminderMinutesBeforeStart { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public bool IsDetachedInstance => !string.IsNullOrWhiteSpace(ParentEventId);
        public bool IsRecurring => RecurrenceFrequency != CalendarRecurrenceFrequency.None || !string.IsNullOrWhiteSpace(RecurrenceRule);

        public TimeSpan Duration => EndTime - StartTime;

        public bool OverlapsWith(CalendarEvent other)
        {
            if (other == null)
            {
                return false;
            }

            if (Id == other.Id)
            {
                return false;
            }

            return StartTime < other.EndTime && EndTime > other.StartTime;
        }
    }

    /// <summary>
    /// Represents an event category with color
    /// </summary>
    public class EventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public Color Color { get; set; }
        public string Description { get; set; } = "";
    }

}