using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveMonthInteraction(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            if (location.Y < grid.Y + dayHeaderHeight)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.EmptySurface,
                    RequestedMode = CalendarInteractionMode.CreateEvent,
                    Location = location,
                    Date = _state.CurrentDate.Date
                };
            }

            var monthBody = new Rectangle(grid.X, grid.Y + dayHeaderHeight, grid.Width, Math.Max(0, grid.Height - dayHeaderHeight));
            int col = CalendarLayoutGeometry.GetColumnIndex(monthBody, location.X, 7);
            int row = CalendarLayoutGeometry.GetRowIndex(monthBody, location.Y, 6);

            if (col < 0 || col >= 7 || row < 0 || row >= 6)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.EmptySurface,
                    RequestedMode = CalendarInteractionMode.CreateEvent,
                    Location = location,
                    Date = _state.CurrentDate.Date
                };
            }

            var firstDayOfMonth = new DateTime(_state.CurrentDate.Year, _state.CurrentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            var date = firstDayOfCalendar.AddDays(row * 7 + col);
            var rowRect = CalendarLayoutGeometry.GetRowRect(monthBody, row, 6);
            var cellRect = CalendarLayoutGeometry.GetColumnRect(rowRect, col, 7);
            var eventHit = ResolveMonthEventHit(location, date, cellRect);
            if (eventHit.Event != null)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.EventBlock,
                    RequestedMode = CalendarInteractionMode.SelectEvent,
                    Location = location,
                    Date = eventHit.Event.StartTime.Date,
                    Event = eventHit.Event,
                    Bounds = eventHit.Bounds
                };
            }

            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.DateCell,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = date
            };
        }

        private (CalendarEvent Event, Rectangle Bounds) ResolveMonthEventHit(Point location, DateTime date, Rectangle cellRect)
        {
            var dayEvents = _eventService?.GetEventsForDate(date) ?? new List<CalendarEvent>();
            if (dayEvents.Count == 0)
            {
                return (null, Rectangle.Empty);
            }

            int eventStartOffset = _usePainterSystem ? ScaleMetric(30) : ScaleMetric(25);
            int eventHeight = _usePainterSystem
                ? ScaleMetric(16)
                : Math.Min(ScaleMetric(CalendarLayoutMetrics.MinEventHitHeight), Math.Max(ScaleMetric(16), cellRect.Height / 4));
            int eventSpacing = ScaleMetric(2);
            int eventY = cellRect.Y + eventStartOffset;

            foreach (var evt in dayEvents.Take(CalendarLayoutMetrics.MaxEventsPerCell))
            {
                var eventRect = new Rectangle(cellRect.X + 2, eventY, Math.Max(1, cellRect.Width - 4), eventHeight);
                if (eventRect.Contains(location))
                {
                    return (evt, eventRect);
                }

                eventY += eventHeight + eventSpacing;
            }

            return (null, Rectangle.Empty);
        }
    }
}
