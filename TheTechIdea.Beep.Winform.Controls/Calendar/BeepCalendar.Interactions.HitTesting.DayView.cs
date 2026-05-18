using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveDayInteraction(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
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
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            var eventHit = ResolveTimedEventHit(location, events, timedArea, dayDate);
            var targetEvent = eventHit.Event;

            return new CalendarInteractionHitTestResult
            {
                TargetKind = targetEvent != null ? CalendarInteractionTargetKind.EventBlock : CalendarInteractionTargetKind.DateCell,
                RequestedMode = targetEvent != null
                    ? eventHit.ResizeEdge == CalendarEventResizeEdge.Start
                        ? CalendarInteractionMode.ResizeStart
                        : eventHit.ResizeEdge == CalendarEventResizeEdge.End
                            ? CalendarInteractionMode.ResizeEnd
                            : CalendarInteractionMode.SelectEvent
                    : CalendarInteractionMode.CreateEvent,
                ResizeEdge = eventHit.ResizeEdge,
                Location = location,
                Date = dayDate,
                Event = targetEvent,
                Bounds = eventHit.Bounds
            };
        }
    }
}
