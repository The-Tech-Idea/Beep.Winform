using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    internal class CalendarEventService
    {
        private readonly List<CalendarEvent> _events;

        public CalendarEventService(List<CalendarEvent> events)
        {
            _events = events ?? new List<CalendarEvent>();
        }

        public List<CalendarEvent> GetEventsForDate(DateTime date)
        {
            return _events.Where(e => e.StartTime.Date == date.Date).ToList();
        }

        public List<CalendarEvent> GetEventsForMonth(DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            return _events.Where(e => e.StartTime.Date >= firstDay && e.StartTime.Date <= lastDay).ToList();
        }

        public List<CalendarEvent> GetEventsForDateRange(DateTime startDate, DateTime endDate)
        {
            return _events.Where(e => e.StartTime.Date >= startDate.Date && e.StartTime.Date <= endDate.Date).ToList();
        }
    }
}
