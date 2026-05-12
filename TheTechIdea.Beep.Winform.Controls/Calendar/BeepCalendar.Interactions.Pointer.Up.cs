using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsDesignModeSafe)
            {
                return;
            }

            if (!_pointerDown)
            {
                return;
            }

            var delta = new Point(e.X - _pointerDownLocation.X, e.Y - _pointerDownLocation.Y);
            bool shouldCommit = _dragInProgress || Math.Abs(delta.X) >= InteractionDragThresholdPx || Math.Abs(delta.Y) >= InteractionDragThresholdPx;
            bool committed = false;

            if (shouldCommit)
            {
                committed = CommitInteractionMutation(e.Location, delta);
            }

            InteractionCompleted?.Invoke(this, new CalendarInteractionEventArgs(
                _state.InteractionMode,
                _state.InteractionTargetKind,
                _state.SelectedDate,
                _state.SelectedEvent,
                e.Location,
                delta,
                committed,
                modifierKeys: ModifierKeys,
                isCopyOperation: IsCopyModifierDown(),
                resizeEdge: _activeInteractionHit?.ResizeEdge ?? CalendarEventResizeEdge.None,
                proposedStart: BuildProposedStart(e.Location, delta),
                proposedEnd: BuildProposedEnd(e.Location, delta),
                conflicts: GetCurrentInteractionConflicts(e.Location, delta)));

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