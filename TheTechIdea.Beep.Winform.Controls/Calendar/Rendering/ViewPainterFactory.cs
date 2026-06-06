using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    /// <summary>
    /// Resolves the right <see cref="ICalendarViewPainter"/> for a given
    /// <see cref="CalendarViewMode"/>. One painter instance per view mode is
    /// kept cached for the lifetime of the process. Each enum value (legacy
    /// or new) resolves to its OWN self-contained painter — the legacy names
    /// are NOT aliased to the new Week1..Week7 painters, because "each view
    /// should be distinct" is a project rule. Legacy enum values remain
    /// numeric-stable for backward compatibility.
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
                // New canonical view modes (W2-Redo) — each is self-contained
                // and matches a sample image in Calendar/sampleimages/.
                CalendarViewMode.Week1  => new Week1ViewPainter(),
                CalendarViewMode.Week2  => new Week2ViewPainter(),
                CalendarViewMode.Week3  => new Week3ViewPainter(),
                CalendarViewMode.Week4  => new Week4ViewPainter(),
                CalendarViewMode.Week5  => new Week5ViewPainter(),
                CalendarViewMode.Week6  => new Week6ViewPainter(),
                CalendarViewMode.Week7  => new Week7ViewPainter(),

                // Legacy view modes (each is a distinct self-contained painter
                // inlined in W2-Redo-2 — no shared TimedWeekPaintLogic shim).
                // They are NOT aliases of Week1..Week7.
                CalendarViewMode.Month    => new MonthViewPainter(),
                CalendarViewMode.Week     => new WeekViewPainter(),
                CalendarViewMode.WorkWeek => new WorkWeekViewPainter(),
                CalendarViewMode.Day      => new DayViewPainter(),
                CalendarViewMode.Agenda   => new AgendaViewPainter(),
                CalendarViewMode.Timeline => new TimelineViewPainter(),
                CalendarViewMode.List     => new ListViewPainter(),

                _ => new WeekViewPainter()
            };
            _cache[mode] = painter;
            return painter;
        }

        /// <summary>Drop all cached painters. Test hook; not used at runtime.</summary>
        public static void Reset() => _cache.Clear();

        /// <summary>
        /// Enumerate the canonical Week1..Week7 view modes and their resolved
        /// painters in registration order. Used by the calendar's toolbar to
        /// build the view-selector buttons dynamically so adding a new view
        /// mode is a factory change only — no toolbar edit required.
        /// </summary>
        public static IEnumerable<KeyValuePair<CalendarViewMode, ICalendarViewPainter>> GetRegisteredViews()
        {
            yield return new KeyValuePair<CalendarViewMode, ICalendarViewPainter>(CalendarViewMode.Week1, GetPainter(CalendarViewMode.Week1));
            yield return new KeyValuePair<CalendarViewMode, ICalendarViewPainter>(CalendarViewMode.Week2, GetPainter(CalendarViewMode.Week2));
            yield return new KeyValuePair<CalendarViewMode, ICalendarViewPainter>(CalendarViewMode.Week3, GetPainter(CalendarViewMode.Week3));
            yield return new KeyValuePair<CalendarViewMode, ICalendarViewPainter>(CalendarViewMode.Week4, GetPainter(CalendarViewMode.Week4));
            yield return new KeyValuePair<CalendarViewMode, ICalendarViewPainter>(CalendarViewMode.Week5, GetPainter(CalendarViewMode.Week5));
            yield return new KeyValuePair<CalendarViewMode, ICalendarViewPainter>(CalendarViewMode.Week6, GetPainter(CalendarViewMode.Week6));
            yield return new KeyValuePair<CalendarViewMode, ICalendarViewPainter>(CalendarViewMode.Week7, GetPainter(CalendarViewMode.Week7));
        }
    }
}
