using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Updates an existing event
        /// </summary>
        public void UpdateEvent(CalendarEvent calendarEvent)
        {
            TryUpdateEvent(calendarEvent);
        }

        public bool TryUpdateEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                return false;
            }

            var existingEvent = _events.FirstOrDefault(e => e.Id == calendarEvent.Id);
            if (existingEvent == null)
            {
                return false;
            }

            var appliedEvent = CloneEvent(calendarEvent);
            NormalizeEventDuration(appliedEvent);

            if (RaiseMutating(CalendarEventMutationKind.Update, existingEvent, appliedEvent, appliedEvent, false, out var canceled) && canceled)
            {
                return false;
            }

            if (!CanApplyEventChange(appliedEvent, existingEvent))
            {
                return false;
            }

            int index = _events.IndexOf(existingEvent);
            _events[index] = appliedEvent;
            if (_state.SelectedEvent != null && _state.SelectedEvent.Id == appliedEvent.Id)
            {
                _state.SelectedEvent = appliedEvent;
            }
            _eventService?.InvalidateCache();
            DeactivateAllCellComponents();
            _componentCache?.DisposeAll();
            RequestRedraw();
            RecordMutationHistory(CalendarEventMutationKind.Update, existingEvent, appliedEvent);
            RaiseMutated(CalendarEventMutationKind.Update, existingEvent, appliedEvent, appliedEvent, false, GetConflicts(appliedEvent, existingEvent));
            return true;
        }
    }
}