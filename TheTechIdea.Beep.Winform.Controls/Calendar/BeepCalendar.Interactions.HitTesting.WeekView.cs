using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveWeekInteraction(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            int dayCount = _state.ViewMode == CalendarViewMode.WorkWeek ? 5 : 7;
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
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

            int col = CalendarLayoutGeometry.GetColumnIndex(timedArea, location.X, dayCount);
            if (col < 0 || col >= dayCount)
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
            if (_state.ViewMode == CalendarViewMode.WorkWeek)
            {
                int dayOfWeek = (int)_state.CurrentDate.DayOfWeek;
                int mondayOffset = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
                startOfWeek = _state.CurrentDate.Date.AddDays(-mondayOffset);
            }

            var dayDate = startOfWeek.AddDays(col).Date;
            var events = _eventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
            var dayColumn = CalendarLayoutGeometry.GetColumnRect(timedArea, col, dayCount);
            var eventHit = ResolveTimedEventHit(location, events, dayColumn, dayDate);
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
