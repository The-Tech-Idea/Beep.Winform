using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        internal void HandleInteractionMouseMove(MouseEventArgs e)
        {
            if (IsDesignModeSafe || !_pointerDown)
            {
                return;
            }

            var delta = new Point(e.X - _pointerDownLocation.X, e.Y - _pointerDownLocation.Y);
            if (!_dragInProgress && (Math.Abs(delta.X) >= InteractionDragThresholdPx || Math.Abs(delta.Y) >= InteractionDragThresholdPx))
            {
                _dragInProgress = true;
                _state.InteractionMode = ResolveDragMode(_activeInteractionHit);
            }

            if (_dragInProgress)
            {
                InteractionUpdated?.Invoke(this, new CalendarInteractionEventArgs(
                    _state.InteractionMode,
                    _state.InteractionTargetKind,
                    _state.SelectedDate,
                    _state.SelectedEvent,
                    e.Location,
                    delta,
                    isCommit: false,
                    modifierKeys: ModifierKeys,
                    isCopyOperation: IsCopyModifierDown(),
                    resizeEdge: _activeInteractionHit?.ResizeEdge ?? CalendarEventResizeEdge.None,
                    proposedStart: BuildProposedStart(e.Location, delta),
                    proposedEnd: BuildProposedEnd(e.Location, delta),
                    GetCurrentInteractionConflicts(e.Location, delta)));
            }
        }

        internal void CancelInteraction()
        {
            if (!_pointerDown && !_dragInProgress)
            {
                return;
            }

            InteractionCancelled?.Invoke(this, new CalendarInteractionEventArgs(
                _state.InteractionMode,
                _state.InteractionTargetKind,
                _state.SelectedDate,
                _state.SelectedEvent,
                _pointerDownLocation,
                Point.Empty,
                isCommit: false));

            _pointerDown = false;
            _dragInProgress = false;
            _state.IsPointerDown = false;
            _state.PointerDownLocation = null;
            _state.InteractionMode = CalendarInteractionMode.None;
            _state.InteractionTargetKind = CalendarInteractionTargetKind.None;
            _state.InteractionEventId = null;
        }
    }
}