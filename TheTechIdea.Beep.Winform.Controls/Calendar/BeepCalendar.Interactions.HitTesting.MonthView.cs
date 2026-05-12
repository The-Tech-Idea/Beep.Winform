using System;
using System.Drawing;

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

            int cellWidth = grid.Width / 7;
            int cellHeight = Math.Max(1, (grid.Height - dayHeaderHeight) / 6);
            int col = (location.X - grid.X) / cellWidth;
            int row = (location.Y - grid.Y - dayHeaderHeight) / cellHeight;

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

            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.DateCell,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = date
            };
        }
    }
}