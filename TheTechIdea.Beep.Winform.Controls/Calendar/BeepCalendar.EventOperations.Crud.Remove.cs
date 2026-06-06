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

            // W2-Redo-9 GAP 3 - the previous version passed
            // `existingEvent` as the `appliedEvent` argument. For a
            // delete the applied (post-mutation) state is "event not
            // present" — passing the event being deleted is confusing
            // and inconsistent with the corresponding `RaiseMutated`
            // call below (which correctly passes `null`). Subscribers
            // that branch on `e.Args.AppliedEvent` to decide whether
            // to take post-mutation action would see the about-to-be-
            // deleted event as "still applied" during the mutating
            // window, which is misleading.
            if (RaiseMutating(CalendarEventMutationKind.Delete, existingEvent, null, null, false, out var canceled) && canceled)
            {
                return;
            }

            _events.Remove(existingEvent);
            if (_state.SelectedEvent != null && _state.SelectedEvent.Id == existingEvent.Id)
            {
                _state.SelectedEvent = null;
            }

            // W2-Redo-15 GAP C - if a W4 inline editor (title / date-range /
            // all-day toggle) is active for the event being removed, close it.
            // Without this, the editor stays open after the event is deleted:
            // (a) the user sees an editor for a deleted event, (b) pressing
            // Enter in the editor fires EditCommitted which records undo
            // history for a stale event reference, (c) the W2-Redo-13
            // EditCommitted handler tries to RecordMutationHistory(Update, ...)
            // for an event that is no longer in _events, producing a
            // confusing undo stack entry that re-creates the deleted event.
            // EndEdit(commit: false) silently discards the editor without
            // raising EditCommitted — the editor's Saving handler never
            // fires, the in-place mutation is skipped, and the EditCancelled
            // handler clears _editingBeforeSnapshot and calls Invalidate().
            if (_editorHost?.ActiveEditors.Count > 0)
            {
                bool editorMatchesRemoved = false;
                var active = _editorHost.ActiveEditors;
                for (int i = 0; i < active.Count; i++)
                {
                    if (active[i]?.Event?.Id == existingEvent.Id)
                    {
                        editorMatchesRemoved = true;
                        break;
                    }
                }
                if (editorMatchesRemoved)
                {
                    EndEdit(commit: false);
                }
            }

            _eventService?.InvalidateCache();
            DeactivateAllCellComponents();
            _componentCache?.DisposeAll();
            RequestRedraw();
            RecordMutationHistory(CalendarEventMutationKind.Delete, existingEvent, null);
            RaiseMutated(CalendarEventMutationKind.Delete, existingEvent, null, null, false, Array.Empty<CalendarEvent>());
        }
    }
}