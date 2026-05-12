using System.Drawing;
namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void UpdateViewButtonStates()
        {
            if (_monthViewButton == null) return;

            Color selectedColor = _currentTheme?.CalendarSelectedDateBackColor ?? _currentTheme?.PrimaryColor ?? _currentTheme?.AccentColor ?? Color.FromArgb(66, 133, 244);
            Color normalColor = _currentTheme?.CalendarBackColor ?? _currentTheme?.SurfaceColor ?? _currentTheme?.BackColor ?? BackColor;
            Color selectedForeColor = _currentTheme?.CalendarSelectedDateForColor ?? _currentTheme?.OnPrimaryColor ?? Color.White;
            Color normalForeColor = _currentTheme?.CalendarForeColor ?? _currentTheme?.ForeColor ?? ForeColor;

            _monthViewButton.BackColor = _state.ViewMode == CalendarViewMode.Month ? selectedColor : normalColor;
            _monthViewButton.ForeColor = _state.ViewMode == CalendarViewMode.Month ? selectedForeColor : normalForeColor;
            _weekViewButton.BackColor  = _state.ViewMode == CalendarViewMode.Week  ? selectedColor : normalColor;
            _weekViewButton.ForeColor  = _state.ViewMode == CalendarViewMode.Week  ? selectedForeColor : normalForeColor;
            _workWeekViewButton.BackColor = _state.ViewMode == CalendarViewMode.WorkWeek ? selectedColor : normalColor;
            _workWeekViewButton.ForeColor = _state.ViewMode == CalendarViewMode.WorkWeek ? selectedForeColor : normalForeColor;
            _dayViewButton.BackColor   = _state.ViewMode == CalendarViewMode.Day   ? selectedColor : normalColor;
            _dayViewButton.ForeColor   = _state.ViewMode == CalendarViewMode.Day   ? selectedForeColor : normalForeColor;
            _agendaViewButton.BackColor = _state.ViewMode == CalendarViewMode.Agenda ? selectedColor : normalColor;
            _agendaViewButton.ForeColor = _state.ViewMode == CalendarViewMode.Agenda ? selectedForeColor : normalForeColor;
            _timelineViewButton.BackColor = _state.ViewMode == CalendarViewMode.Timeline ? selectedColor : normalColor;
            _timelineViewButton.ForeColor = _state.ViewMode == CalendarViewMode.Timeline ? selectedForeColor : normalForeColor;
            _listViewButton.BackColor  = _state.ViewMode == CalendarViewMode.List  ? selectedColor : normalColor;
            _listViewButton.ForeColor  = _state.ViewMode == CalendarViewMode.List  ? selectedForeColor : normalForeColor;

            if (_undoButton != null)
            {
                _undoButton.Enabled = CanUndo;
            }

            if (_redoButton != null)
            {
                _redoButton.Enabled = CanRedo;
            }

            if (_duplicateEventButton != null)
            {
                _duplicateEventButton.Enabled = _state.SelectedEvent != null;
            }

            if (_editEventButton != null)
            {
                _editEventButton.Enabled = _state.SelectedEvent != null;
            }

            if (_deleteEventButton != null)
            {
                _deleteEventButton.Enabled = _state.SelectedEvent != null;
            }
        }

    }
}