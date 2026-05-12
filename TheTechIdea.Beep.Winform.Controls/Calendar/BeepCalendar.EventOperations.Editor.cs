using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected virtual void OnCreateEventRequested(DateTime date)
        {
            var proposedEvent = new CalendarEvent
            {
                StartTime = date,
                EndTime = date.AddHours(1),
                Title = "New Event"
            };

            if (!TryOpenEventEditor(proposedEvent, CalendarEventMutationKind.Create, Point.Empty, out var committed) && !committed)
            {
                CreateEventRequested?.Invoke(this, new CalendarEventArgs(proposedEvent));
            }
        }

        internal bool TryOpenEventEditor(CalendarEvent calendarEvent, CalendarEventMutationKind mutationKind, Point location, out bool committed)
        {
            committed = false;

            if (calendarEvent == null)
            {
                return false;
            }

            var proposedEvent = CloneEvent(calendarEvent);
            var editor = EventEditor;
            if (editor == null)
            {
                return false;
            }

            var request = new CalendarEventEditorRequest(
                editor.Mode,
                mutationKind,
                calendarEvent,
                proposedEvent,
                calendarEvent.StartTime.Date,
                location,
                ModifierKeys,
                false,
                GetConflicts(proposedEvent, calendarEvent));

            if (!editor.TryEdit(request, out var editedEvent) || editedEvent == null)
            {
                return true;
            }

            return TryCommitEditedEvent(editedEvent, calendarEvent, mutationKind, out committed);
        }
    }
}