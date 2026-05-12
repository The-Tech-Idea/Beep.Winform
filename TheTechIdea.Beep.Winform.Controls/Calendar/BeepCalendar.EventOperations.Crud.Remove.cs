using System;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Removes an event from the calendar
        /// </summary>
        public void RemoveEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                return;
            }

            var existingEvent = _events.FirstOrDefault(e => e.Id == calendarEvent.Id);
            if (existingEvent == null && _events.Contains(calendarEvent))
            {
                existingEvent = calendarEvent;
            }

            if (existingEvent == null)
            {
                return;
            }

            if (RaiseMutating(CalendarEventMutationKind.Delete, existingEvent, null, existingEvent, false, out var canceled) && canceled)
            {
                return;
            }

            _events.Remove(existingEvent);
            if (_state.SelectedEvent != null && _state.SelectedEvent.Id == existingEvent.Id)
            {
                _state.SelectedEvent = null;
            }

            _eventService?.InvalidateCache();
            Invalidate();
            RecordMutationHistory(CalendarEventMutationKind.Delete, existingEvent, null);
            RaiseMutated(CalendarEventMutationKind.Delete, existingEvent, null, null, false, Array.Empty<CalendarEvent>());
        }
    }
}