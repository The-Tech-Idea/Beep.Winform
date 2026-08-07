using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// The event a move/resize proposal is computed against: the one the pointer went down on,
        /// falling back to the selection.
        /// </summary>
        /// <remarks>
        /// These builders read only <c>_state.SelectedEvent</c>, which made the drag pipeline
        /// silently inert for any event that was not already selected: the commit asked for a
        /// proposed start, the builder saw no selection, returned null, and the commit fell back to
        /// "unchanged". Selection is a click outcome; the drag target is the hit — the same rule
        /// <c>CommitExistingEventMutation</c> follows.
        /// </remarks>
        private CalendarEvent InteractionEvent => _activeInteractionHit?.Event ?? _state.SelectedEvent;

        private DateTime? BuildProposedStart(Point location, Point delta)
        {
            var evt = InteractionEvent;
            if (evt == null)
            {
                if (_state.InteractionMode == CalendarInteractionMode.RangeSelect || _state.InteractionMode == CalendarInteractionMode.CreateEvent)
                {
                    return BuildCreationRange(location).Start;
                }

                return null;
            }

            if (_state.InteractionMode == CalendarInteractionMode.MoveEvent)
            {
                return SnapDateTime(evt.StartTime.Add(CalculateTimedDelta(delta)));
            }

            if (_state.InteractionMode == CalendarInteractionMode.ResizeStart)
            {
                return SnapDateTime(evt.StartTime.Add(CalculateTimedDelta(delta)));
            }

            if (_state.InteractionMode == CalendarInteractionMode.RangeSelect)
            {
                return GetSnappedStartFromLocation(location);
            }

            return evt.StartTime;
        }

        private DateTime? BuildProposedEnd(Point location, Point delta)
        {
            var evt = InteractionEvent;
            if (evt == null)
            {
                if (_state.InteractionMode == CalendarInteractionMode.RangeSelect || _state.InteractionMode == CalendarInteractionMode.CreateEvent)
                {
                    return BuildCreationRange(location).End;
                }

                return null;
            }

            if (_state.InteractionMode == CalendarInteractionMode.MoveEvent)
            {
                return SnapDateTime(evt.EndTime.Add(CalculateTimedDelta(delta)));
            }

            if (_state.InteractionMode == CalendarInteractionMode.ResizeEnd)
            {
                return SnapDateTime(evt.EndTime.Add(CalculateTimedDelta(delta)));
            }

            if (_state.InteractionMode == CalendarInteractionMode.RangeSelect || _state.InteractionMode == CalendarInteractionMode.CreateEvent)
            {
                var start = GetSnappedStartFromLocation(location);
                return start?.AddMinutes(Math.Max(InteractionSnapIntervalMinutes, 60));
            }

            return evt.EndTime;
        }

    }
}
