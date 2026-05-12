using System;
namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    #region Supporting Classes

    /// <summary>
    /// Calendar view modes
    /// </summary>
    public enum CalendarViewMode
    {
        Month,
        Week,
        WorkWeek,
        Day,
        Agenda,
        Timeline,
        List
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