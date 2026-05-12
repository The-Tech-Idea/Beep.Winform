namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Adds an event to the calendar
        /// </summary>
        public void AddEvent(CalendarEvent calendarEvent)
        {
            TryAddEvent(calendarEvent);
        }

        public bool TryAddEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                return false;
            }

            var appliedEvent = CloneEvent(calendarEvent);
            NormalizeEventDuration(appliedEvent);

            if (RaiseMutating(CalendarEventMutationKind.Create, null, appliedEvent, appliedEvent, false, out var canceled) && canceled)
            {
                return false;
            }

            if (!CanApplyEventChange(appliedEvent, null))
            {
                return false;
            }

            _events.Add(appliedEvent);
            _eventService?.InvalidateCache();
            Invalidate();
            RecordMutationHistory(CalendarEventMutationKind.Create, null, appliedEvent);
            RaiseMutated(CalendarEventMutationKind.Create, null, appliedEvent, appliedEvent, false, GetConflicts(appliedEvent, null));
            return true;
        }
    }
}