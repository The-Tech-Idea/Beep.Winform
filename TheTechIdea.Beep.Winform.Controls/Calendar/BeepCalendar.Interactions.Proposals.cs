using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private DateTime? BuildProposedStart(Point location, Point delta)
        {
            if (_state.SelectedEvent == null)
            {
                if (_state.InteractionMode == CalendarInteractionMode.RangeSelect || _state.InteractionMode == CalendarInteractionMode.CreateEvent)
                {
                    return BuildCreationRange(location).Start;
                }

                return null;
            }

            if (_state.InteractionMode == CalendarInteractionMode.MoveEvent)
            {
                return SnapDateTime(_state.SelectedEvent.StartTime.Add(CalculateTimedDelta(delta)));
            }

            if (_state.InteractionMode == CalendarInteractionMode.ResizeStart)
            {
                return SnapDateTime(_state.SelectedEvent.StartTime.Add(CalculateTimedDelta(delta)));
            }

            if (_state.InteractionMode == CalendarInteractionMode.RangeSelect)
            {
                return GetSnappedStartFromLocation(location);
            }

            return _state.SelectedEvent.StartTime;
        }

        private DateTime? BuildProposedEnd(Point location, Point delta)
        {
            if (_state.SelectedEvent == null)
            {
                if (_state.InteractionMode == CalendarInteractionMode.RangeSelect || _state.InteractionMode == CalendarInteractionMode.CreateEvent)
                {
                    return BuildCreationRange(location).End;
                }

                return null;
            }

            if (_state.InteractionMode == CalendarInteractionMode.MoveEvent)
            {
                return SnapDateTime(_state.SelectedEvent.EndTime.Add(CalculateTimedDelta(delta)));
            }

            if (_state.InteractionMode == CalendarInteractionMode.ResizeEnd)
            {
                return SnapDateTime(_state.SelectedEvent.EndTime.Add(CalculateTimedDelta(delta)));
            }

            if (_state.InteractionMode == CalendarInteractionMode.RangeSelect || _state.InteractionMode == CalendarInteractionMode.CreateEvent)
            {
                var start = GetSnappedStartFromLocation(location);
                return start?.AddMinutes(Math.Max(InteractionSnapIntervalMinutes, 60));
            }

            return _state.SelectedEvent.EndTime;
        }

    }
}