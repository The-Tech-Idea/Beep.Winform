namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool RemoveEventById(int eventId)
        {
            int index = _events.FindIndex(e => e.Id == eventId);
            if (index < 0)
            {
                return false;
            }

            var removed = _events[index];
            _events.RemoveAt(index);
            if (_state.SelectedEvent != null && _state.SelectedEvent.Id == removed.Id)
            {
                _state.SelectedEvent = null;
            }

            _eventService?.InvalidateCache();
            Invalidate();
            return true;
        }
    }
}