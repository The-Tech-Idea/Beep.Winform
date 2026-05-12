using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveWeekInteraction(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            int cellWidth = Math.Max(1, (grid.Width - timeColumnWidth) / 7);
            int slotHeight = Math.Max(ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight), (grid.Height - dayHeaderHeight) / 24);
            if (location.X < grid.X + timeColumnWidth || location.Y < grid.Y + dayHeaderHeight)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.EmptySurface,
                    RequestedMode = CalendarInteractionMode.CreateEvent,
                    Location = location,
                    Date = _state.CurrentDate.Date
                };
            }

            int col = (location.X - grid.X - timeColumnWidth) / cellWidth;
            int hour = (location.Y - grid.Y - dayHeaderHeight) / slotHeight;
            if (col < 0 || col >= 7 || hour < 0 || hour >= 24)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.EmptySurface,
                    RequestedMode = CalendarInteractionMode.CreateEvent,
                    Location = location,
                    Date = _state.CurrentDate.Date
                };
            }

            var startOfWeek = _state.CurrentDate.Date.AddDays(-(int)_state.CurrentDate.DayOfWeek);
            var dayDate = startOfWeek.AddDays(col).Date;
            var events = _eventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
            var targetEvent = ResolveEventInTimedView(location, events, grid, dayHeaderHeight, timeColumnWidth, cellWidth, slotHeight, dayDate);

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