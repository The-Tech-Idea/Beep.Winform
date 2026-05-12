using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private DateTime? GetTimedViewDateFromLocation(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            int slotHeight = Math.Max(ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight), (grid.Height - dayHeaderHeight) / 24);
            if (grid.Height <= 0 || slotHeight <= 0)
            {
                return null;
            }

            if (_state.ViewMode == CalendarViewMode.Week || _state.ViewMode == CalendarViewMode.WorkWeek)
            {
                int usableWidth = Math.Max(1, grid.Width - timeColumnWidth);
                int dayCount = _state.ViewMode == CalendarViewMode.WorkWeek ? 5 : 7;
                int cellWidth = Math.Max(1, usableWidth / dayCount);
                int col = Math.Max(0, Math.Min(dayCount - 1, (location.X - grid.X - timeColumnWidth) / cellWidth));
                var startOfWeek = _state.CurrentDate.Date.AddDays(-(int)_state.CurrentDate.DayOfWeek);
                if (_state.ViewMode == CalendarViewMode.WorkWeek)
                {
                    int mondayOffset = ((int)startOfWeek.DayOfWeek == 0 ? 6 : (int)startOfWeek.DayOfWeek - 1);
                    startOfWeek = startOfWeek.AddDays(-mondayOffset);
                }
                var dayDate = startOfWeek.AddDays(col).Date;
                int minuteOffset = SnapMinutes(Math.Max(0, location.Y - grid.Y - dayHeaderHeight) * 60 / slotHeight);
                return dayDate.AddMinutes(minuteOffset);
            }

            if (_state.ViewMode == CalendarViewMode.Day)
            {
                int minuteOffset = SnapMinutes(Math.Max(0, location.Y - grid.Y - dayHeaderHeight) * 60 / slotHeight);
                return _state.CurrentDate.Date.AddMinutes(minuteOffset);
            }

            return null;
        }
    }
}