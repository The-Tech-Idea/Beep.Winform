using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public event EventHandler<CalendarCommandEventArgs> CommandInvoking;
        public event EventHandler<CalendarCommandEventArgs> CommandInvoked;

        public bool GoToToday()
        {
            return ExecuteCommand(CalendarCommandType.GoToToday);
        }

        public bool NavigatePreviousPeriod()
        {
            return ExecuteCommand(CalendarCommandType.NavigatePrevious);
        }

        public bool NavigateNextPeriod()
        {
            return ExecuteCommand(CalendarCommandType.NavigateNext);
        }

        public bool SwitchView(CalendarViewMode mode)
        {
            return ExecuteCommand(CalendarCommandType.SwitchView, targetView: mode);
        }

        public bool SetVisibleRange(DateTime start, DateTime end)
        {
            return ExecuteCommand(CalendarCommandType.SetVisibleRange, anchorDate: start, visibleRangeEnd: end);
        }

        public bool UndoMutation()
        {
            return ExecuteCommand(CalendarCommandType.UndoMutation);
        }

        public bool RedoMutation()
        {
            return ExecuteCommand(CalendarCommandType.RedoMutation);
        }

        public bool DeleteSelectedEvent()
        {
            return ExecuteCommand(CalendarCommandType.DeleteSelectedEvent);
        }

        public bool EditSelectedEvent()
        {
            return ExecuteCommand(CalendarCommandType.EditSelectedEvent);
        }

        public bool CreateEventAtFocusedDate()
        {
            return ExecuteCommand(CalendarCommandType.CreateEventAtFocusedDate, anchorDate: _focusedDate.Date);
        }

        public bool DuplicateSelectedEvent()
        {
            return ExecuteCommand(CalendarCommandType.DuplicateSelectedEvent);
        }
    }
}