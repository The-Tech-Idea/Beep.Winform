using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected virtual void OnCreateEventRequested(DateTime date)
        {
            DeactivateAllCellComponents();

            // W2-Redo-7 GAP 2 - if no EventEditor is registered and no host
            // is subscribed to CreateEventRequested, the create request is
            // silently dropped. Fall back to the default "create a 1-hour
            // New Event at the requested date and commit it via the standard
            // TryAddEvent path" so the event actually lands in _events and
            // the calendar updates. Developers can still override by
            // subscribing to CreateEventRequested (and optionally by setting
            // EventEditor to a custom ICalendarEventEditor).
            //
            // W2-Redo-8 GAP 3 - select the newly added event so the user
            // sees it highlighted and can immediately edit it. Previously
            // the event landed in _events but the calendar didn't reflect
            // that it was selected.
            //
            // W2-Redo-9 GAP 2 - the previous version allocated a
            // `proposedEvent` at the top of the method and then a separate
            // `fallback` inside the no-editor/no-subscriber branch. The
            // proposedEvent was never used by the fallback (which built
            // its own `fallback` with NextEventId()). The redundant
            // allocation is now created only in the path that actually
            // consumes it.
            if (EventEditor == null && CreateEventRequested == null)
            {
                _focusedDate = date;
                _state.FocusedDate = date;
                var fallback = new CalendarEvent
                {
                    Id = NextEventId(),
                    StartTime = date,
                    EndTime = date.AddHours(1),
                    Title = "New Event"
                };
                NormalizeEventDuration(fallback);
                if (TryAddEvent(fallback))
                {
                    _state.SelectedEvent = fallback;
                    _state.SelectedDate = fallback.StartTime.Date;
                    _state.CurrentDate = fallback.StartTime.Date;
                    EventSelected?.Invoke(this, new CalendarEventArgs(fallback));
                }
                return;
            }

            // Editor or subscriber path: build the proposed event with
            // the time-of-day the caller supplied (often `_focusedDate`
            // from OnKeyDown.Enter which includes the time). Use a
            // default 1-hour duration and let the editor / subscriber
            // adjust as needed.
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