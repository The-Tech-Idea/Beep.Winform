using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (IsDesignModeSafe)
            {
                return;
            }

            Focus();
            _keyboardFocusVisible = false;
            _pointerDown = e.Button == MouseButtons.Left;
            _dragInProgress = false;
            _pointerDownLocation = e.Location;
            _activeInteractionHit = ResolveInteractionTarget(e.Location);

            if (_activeInteractionHit?.Date.HasValue == true)
            {
                _state.SelectedDate = _activeInteractionHit.Date.Value.Date;
                _state.CurrentDate = _activeInteractionHit.Date.Value.Date;
                _focusedDate = _state.SelectedDate;
                _state.FocusedDate = _focusedDate;
                DateSelected?.Invoke(this, new CalendarDateArgs(_state.SelectedDate));
            }

            if (_activeInteractionHit?.Event != null)
            {
                _state.SelectedEvent = _activeInteractionHit.Event;
                _state.SelectedDate = _activeInteractionHit.Event.StartTime.Date;
                _state.CurrentDate = _activeInteractionHit.Event.StartTime.Date;
                _focusedDate = _state.SelectedDate;
                _state.FocusedDate = _focusedDate;
                EventSelected?.Invoke(this, new CalendarEventArgs(_state.SelectedEvent));
            }

            _state.IsPointerDown = _pointerDown;
            _state.PointerDownLocation = _pointerDown ? (Point?)_pointerDownLocation : null;
            _state.InteractionMode = _activeInteractionHit?.RequestedMode ?? CalendarInteractionMode.None;
            _state.InteractionTargetKind = _activeInteractionHit?.TargetKind ?? CalendarInteractionTargetKind.None;
            _state.InteractionEventId = _activeInteractionHit?.Event?.Id;

            InteractionStarted?.Invoke(this, new CalendarInteractionEventArgs(
                _state.InteractionMode,
                _state.InteractionTargetKind,
                _state.SelectedDate,
                _state.SelectedEvent,
                e.Location,
                Point.Empty,
                isCommit: false,
                modifierKeys: ModifierKeys,
                isCopyOperation: IsCopyModifierDown()));

            Invalidate();
        }

    }
}