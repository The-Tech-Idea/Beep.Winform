namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool TryDeleteSelectedEvent()
        {
            if (_state.SelectedEvent == null)
            {
                return false;
            }

            var selectedId = _state.SelectedEvent.Id;
            var existingEvent = _events.FirstOrDefault(e => e.Id == selectedId);
            if (existingEvent == null)
            {
                _state.SelectedEvent = null;
                return false;
            }

            int beforeCount = _events.Count;
            RemoveEvent(existingEvent);
            return _events.Count < beforeCount;
        }
    }
}