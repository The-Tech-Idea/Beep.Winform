using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool CommitCopyEventMutation(CalendarEvent source, CalendarEvent mutated)
        {
            mutated.Id = NextEventId();
            mutated.ParentEventId = source.Id.ToString();
            mutated.SeriesId = string.IsNullOrWhiteSpace(source.SeriesId) ? source.Id.ToString() : source.SeriesId;
            if (RaiseMutating(CalendarEventMutationKind.Copy, source, mutated, mutated, true, out var canceled) && canceled)
            {
                return false;
            }

            _conflictPolicy ??= new CalendarConflictPolicy(_conflictPolicyMode);
            CalendarConflictResult conflictResult = null;
            if (_conflictPolicy.CanSchedule(mutated, _events, out conflictResult) == false)
            {
                ConflictDetected?.Invoke(this, new CalendarConflictEventArgs(conflictResult));
                return false;
            }

            _events.Add(mutated);
            _eventService?.InvalidateCache();
            DeactivateAllCellComponents();
            _componentCache?.DisposeAll();
            _state.SelectedEvent = mutated;
            // W2-Redo-11 BUG C - after a copy the new event is selected
            // but the calendar's date/focus state still points at the
            // SOURCE event's day. CommitNewEventMutation does the
            // analogous sync (SelectedDate / CurrentDate / focusedDate
            // / FocusedDate). Mirror it here so a Ctrl+Shift+drag copy
            // (a) scrolls the view to the destination, (b) makes
            // keyboard navigation continue from the copy's date, and
            // (c) keeps CurrentDate consistent with SelectedDate for
            // any consumer that reads the calendar's "current" day.
            _state.SelectedDate = mutated.StartTime.Date;
            _state.CurrentDate = mutated.StartTime.Date;
            _focusedDate = mutated.StartTime.Date;
            _state.FocusedDate = _focusedDate;
            RequestLayoutAndRedraw();
            RecordMutationHistory(CalendarEventMutationKind.Copy, source, mutated);
            RaiseMutated(CalendarEventMutationKind.Copy, source, mutated, mutated, true, conflictResult?.Conflicts ?? Array.Empty<CalendarEvent>());
            return true;
        }
    }
}