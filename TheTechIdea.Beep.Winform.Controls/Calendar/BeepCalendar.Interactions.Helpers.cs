using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private int NextEventId()
        {
            return (_events?.Count ?? 0) == 0 ? 1 : _events.Max(e => e.Id) + 1;
        }

        private static CalendarEvent CloneEvent(CalendarEvent source)
        {
            if (source == null)
            {
                return null;
            }

            return new CalendarEvent
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                StartTime = source.StartTime,
                EndTime = source.EndTime,
                CategoryId = source.CategoryId,
                Location = source.Location,
                IsAllDay = source.IsAllDay,
                Organizer = source.Organizer,
                Tags = source.Tags != null ? new List<string>(source.Tags) : new List<string>(),
                Status = source.Status,
                TimeZoneId = source.TimeZoneId,
                SeriesId = source.SeriesId,
                ParentEventId = source.ParentEventId,
                ResourceId = source.ResourceId,
                ResourceIds = source.ResourceIds != null ? new List<string>(source.ResourceIds) : new List<string>(),
                RecurrenceRule = source.RecurrenceRule,
                RecurrenceFrequency = source.RecurrenceFrequency,
                RecurrenceInterval = source.RecurrenceInterval,
                RecurrenceCount = source.RecurrenceCount,
                RecurrenceUntilUtc = source.RecurrenceUntilUtc,
                RecurrenceExceptions = source.RecurrenceExceptions != null ? new List<DateTime>(source.RecurrenceExceptions) : new List<DateTime>(),
                ReminderMinutesBeforeStart = source.ReminderMinutesBeforeStart,
                Metadata = source.Metadata != null ? new Dictionary<string, string>(source.Metadata, StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };
        }

        private static void NormalizeEventDuration(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                return;
            }

            if (calendarEvent.EndTime <= calendarEvent.StartTime)
            {
                calendarEvent.EndTime = calendarEvent.StartTime.AddHours(1);
            }
        }

    }
}