using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void InvalidateDateCell(DateTime? date)
        {
            if (!date.HasValue || _state.ViewMode != CalendarViewMode.Month)
            {
                return;
            }

            var cellRect = TryGetMonthCellRect(date.Value.Date);
            if (cellRect.HasValue)
            {
                Invalidate(cellRect.Value);
            }
        }

        private Rectangle? TryGetMonthCellRect(DateTime date)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            if (grid.Width <= 0 || grid.Height <= dayHeaderHeight)
            {
                return null;
            }

            var firstDayOfMonth = new DateTime(_state.CurrentDate.Year, _state.CurrentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            int offset = (date.Date - firstDayOfCalendar.Date).Days;
            if (offset < 0 || offset > 41)
            {
                return null;
            }

            int col = offset % 7;
            int row = offset / 7;
            int cellWidth = grid.Width / 7;
            int cellHeight = (grid.Height - dayHeaderHeight) / 6;

            return new Rectangle(
                grid.X + (col * cellWidth),
                grid.Y + dayHeaderHeight + (row * cellHeight),
                cellWidth,
                cellHeight);
        }
    }
}