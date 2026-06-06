using System;
namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    #region Supporting Classes

    /// <summary>
    /// Calendar view modes. The 7 canonical modes (Week1..Week7) match the
    /// design reference images in <c>sampleimages/</c>. The legacy names
    /// (Month, Week, WorkWeek, Day, Agenda, Timeline, List) are kept as
    /// stable constants for backward compatibility but should be replaced
    /// with the new <see cref="Week1"/>..<see cref="Week7"/> values in new
    /// code. Use <see cref="ResolveWeekN"/> to map a legacy name to its
    /// canonical WeekN value.
    /// </summary>
    public enum CalendarViewMode
    {
        // ── New canonical view modes (7) ──────────────────────────────────
        // 7-day timed grid + left sidebar (c1)
        Week1 = 0,
        // 7-day timed grid + right detail panel (c2)
        Week2 = 1,
        // 4-day timed grid (Mon-Thu) + filter bar (c3)
        Week3 = 2,
        // 6×7 month grid + right detail panel (c4)
        Week4 = 3,
        // 7-day columns of event cards + day-of-week tabs (c5)
        Week5 = 4,
        // 6-day columns (Mon-Sat) + events in time order (c6)
        Week6 = 5,
        // 7-day timed grid + appointment filter + status badges (c7)
        Week7 = 6,

        // ── Legacy aliases (kept for backward compatibility) ──────────────
        // Numeric values are stable (do not change!) so existing serialized
        // settings and old switch statements keep working. They map to the
        // canonical WeekN values via ResolveWeekN() — see below.
        Month    = 100,
        Week     = 101,
        WorkWeek = 102,
        Day      = 103,
        Agenda   = 104,
        Timeline = 105,
        List     = 106,
    }

    public enum CalendarDensityMode
    {
        Compact,
        Comfortable
    }

    public enum CalendarEventStatus
    {
        Tentative,
        Confirmed,
        Cancelled
    }

    public enum CalendarConflictPolicyMode
    {
        AllowOverlap,
        WarnOnOverlap,
        PreventOverlap
    }

    internal sealed class CalendarMutationRecord
    {
        public CalendarMutationRecord(CalendarEventMutationKind kind, CalendarEvent beforeEvent, CalendarEvent afterEvent)
        {
            Kind = kind;
            BeforeEvent = beforeEvent;
            AfterEvent = afterEvent;
        }

        public CalendarEventMutationKind Kind { get; }
        public CalendarEvent BeforeEvent { get; }
        public CalendarEvent AfterEvent { get; }
    }

    public enum CalendarRecurrenceFrequency
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    internal enum CalendarToolbarLabelMode
    {
        Full,
        Medium,
        Compact
    }

    #endregion
}