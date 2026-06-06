using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void InvalidateDateCell(DateTime? date)
        {
            if (!date.HasValue || _viewPainter == null || !_viewPainter.IsMonthGrid)
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
            if (_surfaceModel != null)
            {
                var rect = _surfaceModel.GetMonthCellRect(date);
                return rect.IsEmpty ? (Rectangle?)null : rect;
            }

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
            var monthBody = new Rectangle(grid.X, grid.Y + dayHeaderHeight, grid.Width, Math.Max(0, grid.Height - dayHeaderHeight));
            var rowRect = CalendarLayoutGeometry.GetRowRect(monthBody, row, 6);

            return CalendarLayoutGeometry.GetColumnRect(rowRect, col, 7);
        }
    }
}
