using System;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public event EventHandler<CalendarInteractionEventArgs> InteractionStarted;
        public event EventHandler<CalendarInteractionEventArgs> InteractionUpdated;
        public event EventHandler<CalendarInteractionEventArgs> InteractionCompleted;
        public event EventHandler<CalendarInteractionEventArgs> InteractionCancelled;

        private CalendarInteractionMode ResolveDragMode(CalendarInteractionHitTestResult hit)
        {
            if (hit == null)
            {
                return CalendarInteractionMode.None;
            }

            if (hit.TargetKind == CalendarInteractionTargetKind.EventBlock)
            {
                if (_state.ViewMode == CalendarViewMode.Day ||
                    _state.ViewMode == CalendarViewMode.Week ||
                    _state.ViewMode == CalendarViewMode.WorkWeek)
                {
                    return hit.ResizeEdge == CalendarEventResizeEdge.Start
                        ? CalendarInteractionMode.ResizeStart
                        : hit.ResizeEdge == CalendarEventResizeEdge.End
                            ? CalendarInteractionMode.ResizeEnd
                            : CalendarInteractionMode.MoveEvent;
                }

                if (_state.ViewMode == CalendarViewMode.Timeline)
                {
                    return CalendarInteractionMode.MoveEvent;
                }

                return CalendarInteractionMode.SelectEvent;
            }

            if (hit.TargetKind == CalendarInteractionTargetKind.DateCell)
            {
                return _state.ViewMode == CalendarViewMode.Day ||
                       _state.ViewMode == CalendarViewMode.Week ||
                       _state.ViewMode == CalendarViewMode.WorkWeek
                    ? CalendarInteractionMode.RangeSelect
                    : CalendarInteractionMode.CreateEvent;
            }

            if (hit.TargetKind == CalendarInteractionTargetKind.EmptySurface)
            {
                return CalendarInteractionMode.CreateEvent;
            }

            return CalendarInteractionMode.None;
        }

        private bool IsCopyModifierDown()
        {
            return (ModifierKeys & (Keys.Control | Keys.Shift)) == (Keys.Control | Keys.Shift)
                || (ModifierKeys & Keys.Control) == Keys.Control;
        }

        private bool CommitInteractionMutation(Point location, Point delta)
        {
            if (_state.InteractionMode == CalendarInteractionMode.MoveEvent ||
                _state.InteractionMode == CalendarInteractionMode.ResizeStart ||
                _state.InteractionMode == CalendarInteractionMode.ResizeEnd)
            {
                return CommitExistingEventMutation(location, delta);
            }

            if (_state.InteractionMode == CalendarInteractionMode.CreateEvent ||
                _state.InteractionMode == CalendarInteractionMode.RangeSelect)
            {
                return CommitNewEventMutation(location, delta);
            }

            return false;
        }

    }
}
