using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private DateTime? GetTimedViewDateFromLocation(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            if (timedArea.Height <= 0)
            {
                return null;
            }

            if (_state.ViewMode == CalendarViewMode.Week || _state.ViewMode == CalendarViewMode.WorkWeek)
            {
                int dayCount = _state.ViewMode == CalendarViewMode.WorkWeek ? 5 : 7;
                int col = CalendarLayoutGeometry.GetColumnIndex(timedArea, location.X, dayCount);
                if (col < 0)
                {
                    col = Math.Max(0, Math.Min(dayCount - 1, (location.X - timedArea.X) * dayCount / Math.Max(1, timedArea.Width)));
                }

                DateTime startOfWeek;
                if (_state.ViewMode == CalendarViewMode.WorkWeek)
                {
                    int dayOfWeek = (int)_state.CurrentDate.DayOfWeek;
                    int mondayOffset = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
                    startOfWeek = _state.CurrentDate.Date.AddDays(-mondayOffset);
                }
                else
                {
                    startOfWeek = _state.CurrentDate.Date.AddDays(-(int)_state.CurrentDate.DayOfWeek);
                }

                var dayDate = startOfWeek.AddDays(col).Date;
                int minuteOffset = SnapMinutes(CalendarLayoutGeometry.GetMinuteFromY(timedArea, location.Y));
                return dayDate.AddMinutes(minuteOffset);
            }

            if (_state.ViewMode == CalendarViewMode.Day)
            {
                int minuteOffset = SnapMinutes(CalendarLayoutGeometry.GetMinuteFromY(timedArea, location.Y));
                return _state.CurrentDate.Date.AddMinutes(minuteOffset);
            }

            return null;
        }
    }
}
