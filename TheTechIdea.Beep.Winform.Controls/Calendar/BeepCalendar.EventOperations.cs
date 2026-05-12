using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        #region Public Methods

        private bool RaiseMutating(CalendarEventMutationKind mutationKind, CalendarEvent originalEvent, CalendarEvent proposedEvent, CalendarEvent appliedEvent, bool isCopyOperation, out bool canceled)
        {
            var args = new CalendarEventMutationEventArgs(mutationKind, originalEvent, proposedEvent, appliedEvent, isCopyOperation, GetConflicts(proposedEvent, originalEvent));
            EventMutating?.Invoke(this, args);
            canceled = args.Cancel;
            return true;
        }

        private void RaiseMutated(CalendarEventMutationKind mutationKind, CalendarEvent originalEvent, CalendarEvent proposedEvent, CalendarEvent appliedEvent, bool isCopyOperation, IReadOnlyList<CalendarEvent> conflicts)
        {
            EventMutated?.Invoke(this, new CalendarEventMutationEventArgs(mutationKind, originalEvent, proposedEvent, appliedEvent, isCopyOperation, conflicts));
        }

        private IReadOnlyList<CalendarEvent> GetConflicts(CalendarEvent candidate, CalendarEvent existingEvent)
        {
            if (candidate == null)
            {
                return Array.Empty<CalendarEvent>();
            }

            _conflictPolicy ??= new CalendarConflictPolicy(_conflictPolicyMode);

            var comparisonSet = existingEvent == null
                ? _events
                : _events.Where(e => e.Id != existingEvent.Id).ToList();

            _conflictPolicy.CanSchedule(candidate, comparisonSet, out var result);
            return result?.Conflicts ?? Array.Empty<CalendarEvent>();
        }

        private bool CanApplyEventChange(CalendarEvent candidate, CalendarEvent existingEvent)
        {
            _conflictPolicy ??= new CalendarConflictPolicy(_conflictPolicyMode);

            var comparisonSet = existingEvent == null
                ? _events
                : _events.Where(e => e.Id != existingEvent.Id).ToList();

            bool canSchedule = _conflictPolicy.CanSchedule(candidate, comparisonSet, out var result);
            if (result?.HasConflicts == true)
            {
                ConflictDetected?.Invoke(this, new CalendarConflictEventArgs(result));
            }

            return canSchedule;
        }

        #endregion
    }
}