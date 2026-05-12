using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void NavigateToday()
        {
            _state.CurrentDate = DateTime.Today;
            _state.SelectedDate = DateTime.Today;
            _focusedDate = DateTime.Today;
            _state.FocusedDate = _focusedDate;
            Invalidate();
            UpdateViewButtonStates();
        }

        private void NavigatePrevious()
        {
            _state.CurrentDate = _state.ViewMode switch
            {
                CalendarViewMode.Month => _state.CurrentDate.AddMonths(-1),
                CalendarViewMode.Week => _state.CurrentDate.AddDays(-7),
                CalendarViewMode.WorkWeek => _state.CurrentDate.AddDays(-5),
                CalendarViewMode.Day => _state.CurrentDate.AddDays(-1),
                CalendarViewMode.Agenda => _state.CurrentDate.AddMonths(-1),
                CalendarViewMode.Timeline => _state.CurrentDate.AddDays(-7),
                CalendarViewMode.List => _state.CurrentDate.AddMonths(-1),
                _ => _state.CurrentDate.AddMonths(-1)
            };
            Invalidate();
            UpdateViewButtonStates();
        }

        private void NavigateNext()
        {
            _state.CurrentDate = _state.ViewMode switch
            {
                CalendarViewMode.Month => _state.CurrentDate.AddMonths(1),
                CalendarViewMode.Week => _state.CurrentDate.AddDays(7),
                CalendarViewMode.WorkWeek => _state.CurrentDate.AddDays(5),
                CalendarViewMode.Day => _state.CurrentDate.AddDays(1),
                CalendarViewMode.Agenda => _state.CurrentDate.AddMonths(1),
                CalendarViewMode.Timeline => _state.CurrentDate.AddDays(7),
                CalendarViewMode.List => _state.CurrentDate.AddMonths(1),
                _ => _state.CurrentDate.AddMonths(1)
            };
            Invalidate();
            UpdateViewButtonStates();
        }

        private bool TryEditSelectedEvent(Point location)
        {
            if (_state.SelectedEvent == null)
            {
                return false;
            }

            return TryOpenEventEditor(_state.SelectedEvent, CalendarEventMutationKind.Update, location, out _);
        }
    }
}