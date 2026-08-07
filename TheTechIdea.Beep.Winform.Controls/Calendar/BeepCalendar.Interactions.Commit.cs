using System;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool CommitExistingEventMutation(Point location, Point delta)
        {
            // The dragged event is whatever the pointer went DOWN on, not whatever was selected
            // beforehand. This gated on _state.SelectedEvent, so dragging an unselected event
            // silently did nothing - the pipeline armed, escalated and committed against a null and
            // returned false. Worse, the fallback then mutated the SELECTED event: with event A
            // selected, dragging event B would have moved A. Selection is a click outcome; the drag
            // target is the hit.
            var dragTarget = _activeInteractionHit?.Event ?? _state.SelectedEvent;
            if (dragTarget == null)
            {
                return false;
            }

            var source = _events.FirstOrDefault(e => e.Id == dragTarget.Id) ?? dragTarget;
            var proposedStart = BuildProposedStart(location, delta) ?? source.StartTime;
            var proposedEnd = BuildProposedEnd(location, delta) ?? source.EndTime;
            var copyOperation = _state.InteractionMode == CalendarInteractionMode.MoveEvent && IsCopyModifierDown();

            var mutated = CloneEvent(source);
            mutated.StartTime = proposedStart;
            mutated.EndTime = proposedEnd;
            NormalizeEventDuration(mutated);

            if (copyOperation)
            {
                return CommitCopyEventMutation(source, mutated);
            }

            var mutationKind = _state.InteractionMode == CalendarInteractionMode.ResizeStart
                ? CalendarEventMutationKind.ResizeStart
                : _state.InteractionMode == CalendarInteractionMode.ResizeEnd
                    ? CalendarEventMutationKind.ResizeEnd
                    : CalendarEventMutationKind.Move;

            if (RaiseMutating(mutationKind, source, mutated, mutated, false, out var updateCanceled) && updateCanceled)
            {
                return false;
            }

            _conflictPolicy ??= new CalendarConflictPolicy(_conflictPolicyMode);
            CalendarConflictResult updateConflicts = null;
            if (_conflictPolicy.CanSchedule(mutated, _events.Where(e => e.Id != source.Id).ToList(), out updateConflicts) == false)
            {
                ConflictDetected?.Invoke(this, new CalendarConflictEventArgs(updateConflicts));
                return false;
            }

            int index = _events.FindIndex(e => e.Id == source.Id);
            if (index < 0)
            {
                return false;
            }

            _events[index] = mutated;
            if (_state.SelectedEvent != null && _state.SelectedEvent.Id == mutated.Id)
            {
                _state.SelectedEvent = mutated;
            }

            _eventService?.InvalidateCache();
            DeactivateAllCellComponents();
            _componentCache?.DisposeAll();
            RequestRedraw();
            RecordMutationHistory(mutationKind, source, mutated);
            RaiseMutated(mutationKind, source, mutated, mutated, false, updateConflicts?.Conflicts ?? Array.Empty<CalendarEvent>());
            return true;
        }

    }
}