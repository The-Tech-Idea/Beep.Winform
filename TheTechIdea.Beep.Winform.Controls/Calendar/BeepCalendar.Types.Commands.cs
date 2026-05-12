using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public enum CalendarCommandType
    {
        GoToToday,
        NavigatePrevious,
        NavigateNext,
        SwitchView,
        SetVisibleRange,
        UndoMutation,
        RedoMutation,
        DeleteSelectedEvent,
        EditSelectedEvent,
        CreateEventAtFocusedDate,
        DuplicateSelectedEvent
    }

    public sealed class CalendarCommandEventArgs : EventArgs
    {
        public CalendarCommandEventArgs(CalendarCommandType commandType)
        {
            CommandType = commandType;
        }

        public CalendarCommandType CommandType { get; }
        public DateTime? AnchorDate { get; set; }
        public DateTime? VisibleRangeEnd { get; set; }
        public CalendarViewMode? TargetView { get; set; }
        public bool Cancel { get; set; }
        public bool Succeeded { get; set; }
    }
}