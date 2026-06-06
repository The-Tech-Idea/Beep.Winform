using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsDesignModeSafe)
            {
                return;
            }

            if (!_pointerDown)
            {
                UpdateHoverState(e.Location);
            }

            HandleInteractionMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (IsDesignModeSafe || _pointerDown)
            {
                return;
            }

            ClearHoverState();
        }

        internal void HandleInteractionMouseMove(MouseEventArgs e)
        {
            if (IsDesignModeSafe || !_pointerDown)
            {
                return;
            }

            if (HasActiveCellComponent())
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

        private void UpdateHoverState(Point location)
        {
            if (HasActiveCellComponent())
            {
                return;
            }

            var hit = ResolveInteractionTarget(location);
            CalendarEvent hoveredEvent = hit?.TargetKind == CalendarInteractionTargetKind.EventBlock ? hit.Event : null;
            DateTime? hoveredDate = hit?.TargetKind == CalendarInteractionTargetKind.DateCell || hit?.TargetKind == CalendarInteractionTargetKind.EventBlock
                ? hit.Date?.Date
                : null;
            int? hoveredEventId = hoveredEvent?.Id;

            bool changed = _state.HoveredDate?.Date != hoveredDate?.Date
                || _state.HoveredEventId != hoveredEventId
                || _hoveredEvent?.Id != hoveredEventId;

            _hoveredDate = hoveredDate;
            _hoveredEvent = hoveredEvent;
            _state.HoveredDate = hoveredDate;
            _state.HoveredEventId = hoveredEventId;
            Cursor = ResolveHoverCursor(hit);

            if (changed)
            {
                Invalidate();
            }
        }

        private void ClearHoverState()
        {
            if (!_state.HoveredDate.HasValue && !_state.HoveredEventId.HasValue && _hoveredEvent == null && !_hoveredDate.HasValue)
            {
                Cursor = Cursors.Default;
                return;
            }

            _hoveredDate = null;
            _hoveredEvent = null;
            _state.HoveredDate = null;
            _state.HoveredEventId = null;
            Cursor = Cursors.Default;
            Invalidate();
        }

        private static Cursor ResolveHoverCursor(CalendarInteractionHitTestResult hit)
        {
            if (hit == null)
            {
                return Cursors.Default;
            }

            if (hit.ResizeEdge == CalendarEventResizeEdge.Start || hit.ResizeEdge == CalendarEventResizeEdge.End)
            {
                return Cursors.SizeNS;
            }

            if (hit.TargetKind == CalendarInteractionTargetKind.EventBlock || hit.TargetKind == CalendarInteractionTargetKind.DateCell)
            {
                return Cursors.Hand;
            }

            return Cursors.Default;
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

            // W2-Redo-6 GAP 2 - release the mouse capture that was set in
            // OnMouseDown so a cancel (e.g. Esc) doesn't leave the
            // calendar holding the mouse. Otherwise subsequent clicks
            // outside the control would still route to the calendar.
            Capture = false;
        }

        // W2-Redo-18 GAP A - when the calendar loses mouse capture
        // (Alt+Tab, system menu opens, window deactivation), Windows
        // releases the capture but does NOT send a MouseUp. Without this
        // handler the drag state (_pointerDown, _dragInProgress,
        // _state.InteractionMode) stays live. The next click reuses the
        // stale state: OnMouseMove sees _dragInProgress=true and fires
        // InteractionUpdated with the old interaction mode for the NEW
        // hit location, causing a spurious drag of whatever was
        // previously selected. CancelInteraction resets everything and
        // raises InteractionCancelled so consumers can clean up tentative
        // visuals.
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            if (!Capture && _pointerDown)
            {
                CancelInteraction();
                Invalidate();
            }
        }
    }
}
