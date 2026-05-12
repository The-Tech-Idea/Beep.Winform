using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveDayInteraction(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            int slotHeight = Math.Max(ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight), (grid.Height - dayHeaderHeight) / 24);
            if (location.X < grid.X + timeColumnWidth || location.Y < grid.Y + dayHeaderHeight)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.EmptySurface,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = _state.CurrentDate.Date
                };
            }

            var dayDate = _state.CurrentDate.Date;
            var events = _eventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
            var targetEvent = ResolveEventInTimedView(location, events, grid, dayHeaderHeight, timeColumnWidth, grid.Width - timeColumnWidth, slotHeight, dayDate);

            return new CalendarInteractionHitTestResult
            {
                TargetKind = targetEvent != null ? CalendarInteractionTargetKind.EventBlock : CalendarInteractionTargetKind.DateCell,
                RequestedMode = targetEvent != null ? CalendarInteractionMode.SelectEvent : CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = dayDate,
                Event = targetEvent
            };
        }
    }
}