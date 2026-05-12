using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private TimeSpan CalculateTimedDelta(Point delta)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int slotHeight = Math.Max(1, ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight));
            if (grid.Height <= 0)
            {
                return TimeSpan.Zero;
            }

            double minutesPerPixel = 60d / slotHeight;
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