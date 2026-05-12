namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool TryCommitEditedEvent(CalendarEvent editedEvent, CalendarEvent calendarEvent, CalendarEventMutationKind mutationKind, out bool committed)
        {
            committed = false;

            if (mutationKind == CalendarEventMutationKind.Create && editedEvent.Id <= 0)
            {
                editedEvent.Id = NextEventId();
            }

            if (mutationKind != CalendarEventMutationKind.Create)
            {
                editedEvent.Id = calendarEvent.Id;
            }

            NormalizeEventDuration(editedEvent);
            switch (mutationKind)
            {
                case CalendarEventMutationKind.Create:
                    committed = TryAddEvent(editedEvent);
                    return true;
                case CalendarEventMutationKind.Move:
                case CalendarEventMutationKind.ResizeStart:
                case CalendarEventMutationKind.ResizeEnd:
                case CalendarEventMutationKind.Update:
                    committed = TryUpdateEvent(editedEvent);
                    return true;
                case CalendarEventMutationKind.Copy:
                    committed = TryAddEvent(editedEvent);
                    return true;
                default:
                    return true;
            }
        }
    }
}