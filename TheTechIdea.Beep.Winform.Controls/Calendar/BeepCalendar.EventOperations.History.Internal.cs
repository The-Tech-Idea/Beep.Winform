namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void RecordMutationHistory(CalendarEventMutationKind mutationKind, CalendarEvent beforeEvent, CalendarEvent afterEvent)
        {
            if (_suspendHistory)
            {
                return;
            }

            _undoStack.Push(new CalendarMutationRecord(
                mutationKind,
                beforeEvent != null ? CloneEvent(beforeEvent) : null,
                afterEvent != null ? CloneEvent(afterEvent) : null));
            _redoStack.Clear();
            UpdateViewButtonStates();
        }

    }
}