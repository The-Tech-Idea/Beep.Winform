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
            _state.SelectedEvent = mutated;
            Invalidate();
            RecordMutationHistory(CalendarEventMutationKind.Copy, source, mutated);
            RaiseMutated(CalendarEventMutationKind.Copy, source, mutated, mutated, true, conflictResult?.Conflicts ?? Array.Empty<CalendarEvent>());
            return true;
        }
    }
}