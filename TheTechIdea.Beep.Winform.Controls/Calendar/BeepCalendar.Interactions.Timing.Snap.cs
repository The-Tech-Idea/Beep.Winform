using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private TimeSpan CalculateTimedDelta(Point delta)
        {
            if (_state.ViewMode == CalendarViewMode.Timeline)
            {
                var timelineGrid = _rects.CalendarGridRect;
                int laneHeaderWidth = Math.Min(ScaleMetric(140), Math.Max(ScaleMetric(80), timelineGrid.Width / 3));
                int contentWidth = Math.Max(1, timelineGrid.Width - laneHeaderWidth);
                double dayWidth = contentWidth / 7d;
                int dayDelta = (int)Math.Round(delta.X / dayWidth, MidpointRounding.AwayFromZero);
                return TimeSpan.FromDays(dayDelta);
            }

            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            if (timedArea.Height <= 0)
            {
                return TimeSpan.Zero;
            }

            double minutesPerPixel = CalendarLayoutGeometry.MinutesPerDay / (double)timedArea.Height;
            int minuteDelta = SnapMinutes((int)Math.Round(delta.Y * minutesPerPixel, MidpointRounding.AwayFromZero));
            return TimeSpan.FromMinutes(minuteDelta);
        }

        private int SnapMinutes(int minutes)
        {
            int interval = Math.Max(1, InteractionSnapIntervalMinutes);
            return (int)Math.Round(minutes / (double)interval, MidpointRounding.AwayFromZero) * interval;
        }

        private DateTime SnapDateTime(DateTime dateTime)
        {
            int snappedMinutes = SnapMinutes(dateTime.Hour * 60 + dateTime.Minute);
            if (snappedMinutes < 0)
            {
                snappedMinutes = 0;
            }

            return dateTime.Date.AddMinutes(snappedMinutes);
        }

        private DateTime? GetSnappedStartFromLocation(Point location)
        {
            var baseDate = _activeInteractionHit?.Date?.Date ?? _state.SelectedDate.Date;

            switch (_state.ViewMode)
            {
                case CalendarViewMode.Week:
                case CalendarViewMode.WorkWeek:
                    return GetTimedViewDateFromLocation(location)?.AddHours(0) ?? baseDate.AddHours(9);
                case CalendarViewMode.Day:
                    return GetTimedViewDateFromLocation(location)?.AddHours(0) ?? baseDate.AddHours(9);
                case CalendarViewMode.Month:
                case CalendarViewMode.List:
                default:
                    return baseDate.AddHours(9);
            }
        }

    }
}
