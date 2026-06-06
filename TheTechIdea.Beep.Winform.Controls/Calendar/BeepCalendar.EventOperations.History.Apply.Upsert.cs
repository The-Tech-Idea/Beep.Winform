namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool AddOrReplaceEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                return false;
            }

            var clone = CloneEvent(calendarEvent);
            NormalizeEventDuration(clone);

            int index = _events.FindIndex(e => e.Id == clone.Id);
            if (index >= 0)
            {
                _events[index] = clone;
            }
            else
            {
                _events.Add(clone);
            }

            _state.SelectedEvent = clone;
            _state.SelectedDate = clone.StartTime.Date;
            _state.CurrentDate = clone.StartTime.Date;
            _focusedDate = clone.StartTime.Date;
            _state.FocusedDate = _focusedDate;
            _eventService?.InvalidateCache();
            DeactivateAllCellComponents();
            _componentCache?.DisposeAll();
            RequestRedraw();
            return true;
        }
    }
}