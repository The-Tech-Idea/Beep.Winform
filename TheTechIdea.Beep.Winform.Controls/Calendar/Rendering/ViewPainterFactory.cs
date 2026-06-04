using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    /// <summary>
    /// Resolves the right <see cref="ICalendarViewPainter"/> for a given
    /// <see cref="CalendarViewMode"/>. One painter instance per view mode is
    /// kept cached for the lifetime of the process.
    /// </summary>
    public static class ViewPainterFactory
    {
        private static readonly Dictionary<CalendarViewMode, ICalendarViewPainter> _cache
            = new Dictionary<CalendarViewMode, ICalendarViewPainter>();

        /// <summary>
        /// Get (or create and cache) the view painter for the supplied mode.
        /// </summary>
        public static ICalendarViewPainter GetPainter(CalendarViewMode mode)
        {
            if (_cache.TryGetValue(mode, out var painter) && painter != null)
            {
                return painter;
            }

            painter = mode switch
            {
                CalendarViewMode.Month     => new MonthViewPainter(),
                CalendarViewMode.Week      => new WeekViewPainter(),
                CalendarViewMode.WorkWeek  => new WorkWeekViewPainter(),
                CalendarViewMode.Day       => new DayViewPainter(),
                CalendarViewMode.Agenda    => new AgendaViewPainter(),
                CalendarViewMode.Timeline  => new TimelineViewPainter(),
                CalendarViewMode.List      => new ListViewPainter(),
                _                          => new MonthViewPainter()
            };
            _cache[mode] = painter;
            return painter;
        }

        /// <summary>Drop all cached painters. Test hook; not used at runtime.</summary>
        public static void Reset() => _cache.Clear();
    }
}
