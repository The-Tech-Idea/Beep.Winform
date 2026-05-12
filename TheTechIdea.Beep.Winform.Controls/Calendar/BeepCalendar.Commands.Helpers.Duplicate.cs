using System;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool TryDuplicateSelectedEvent()
        {
            if (_state.SelectedEvent == null)
            {
                return false;
            }

            var source = _events.FirstOrDefault(e => e.Id == _state.SelectedEvent.Id);
            if (source == null)
            {
                _state.SelectedEvent = null;
                return false;
            }

            var duplicated = CloneEvent(source);
            duplicated.Id = NextEventId();
            duplicated.StartTime = duplicated.StartTime.AddDays(1);
            duplicated.EndTime = duplicated.EndTime.AddDays(1);
            duplicated.ParentEventId = source.Id.ToString();
            duplicated.SeriesId = string.IsNullOrWhiteSpace(source.SeriesId) ? source.Id.ToString() : source.SeriesId;
            NormalizeEventDuration(duplicated);

            if (RaiseMutating(CalendarEventMutationKind.Copy, source, duplicated, duplicated, true, out var canceled) && canceled)
            {
                return false;
            }

            _conflictPolicy ??= new CalendarConflictPolicy(_conflictPolicyMode);
            CalendarConflictResult conflictResult = null;
            if (_conflictPolicy.CanSchedule(duplicated, _events, out conflictResult) == false)
            {
                ConflictDetected?.Invoke(this, new CalendarConflictEventArgs(conflictResult));
                return false;
            }

            _events.Add(duplicated);
            _state.SelectedEvent = duplicated;
            _state.SelectedDate = duplicated.StartTime.Date;
            _state.CurrentDate = duplicated.StartTime.Date;
            _focusedDate = duplicated.StartTime.Date;
            _state.FocusedDate = _focusedDate;
            _eventService?.InvalidateCache();
            Invalidate();
            RecordMutationHistory(CalendarEventMutationKind.Copy, source, duplicated);
            RaiseMutated(CalendarEventMutationKind.Copy, source, duplicated, duplicated, true, conflictResult?.Conflicts ?? Array.Empty<CalendarEvent>());
            return true;
        }
    }
}