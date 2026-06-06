using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void NavigateToday()
        {
            // GAP B - W8 cell components for date-bearing cells (DateCell /
            // TimeSlot kinds) are keyed by the date. When the user navigates
            // to a different date, those keys no longer match any visible
            // cell, but the hosted Control is still visible at its old
            // bounds. Deactivate the W8 layer so the user doesn't see a
            // stale editor. W4 sample editors are event-bound (not date-
            // bound) and remain valid across navigation.
            if (_state.CurrentDate.Date != DateTime.Today)
            {
                DeactivateAllCellComponents();
            }
            _state.CurrentDate = DateTime.Today;
            _state.SelectedDate = DateTime.Today;
            _focusedDate = DateTime.Today;
            _state.FocusedDate = _focusedDate;
            RequestLayoutAndRedraw();
            UpdateViewButtonStates();
        }

        private void NavigatePrevious()
        {
            if (_viewPainter != null)
            {
                var newDate = _viewPainter.NavigatePrevious(_state.CurrentDate);
                if (newDate.Date != _state.CurrentDate.Date)
                {
                    DeactivateAllCellComponents();
                }
                // W2-Redo-14 GAP B - NavigateToday syncs all five date/focus
                // fields. NavigatePrevious and NavigateNext only set
                // _state.CurrentDate (the raw field, bypassing the property
                // setter's DeactivateAllCellComponents + Invalidate +
                // SelectedDate + focusedDate sync). Without this fix,
                // pressing Left/Right arrow or clicking prev/next in the
                // toolbar would scroll the view but (a) _focusedDate stayed
                // at the old date (next arrow-key press jumped from the
                // wrong cell), (b) _state.SelectedDate didn't track the new
                // visible range, (c) any code reading _state.FocusedDate or
                // _state.SelectedDate after navigation saw stale data.
                _state.CurrentDate = newDate;
                _state.SelectedDate = newDate.Date;
                _focusedDate = newDate.Date;
                _state.FocusedDate = _focusedDate;
            }
            RequestLayoutAndRedraw();
            UpdateViewButtonStates();
        }

        private void NavigateNext()
        {
            if (_viewPainter != null)
            {
                var newDate = _viewPainter.NavigateNext(_state.CurrentDate);
                if (newDate.Date != _state.CurrentDate.Date)
                {
                    DeactivateAllCellComponents();
                }
                // W2-Redo-14 GAP B (cont.) - same sync as NavigatePrevious.
                _state.CurrentDate = newDate;
                _state.SelectedDate = newDate.Date;
                _focusedDate = newDate.Date;
                _state.FocusedDate = _focusedDate;
            }
            RequestLayoutAndRedraw();
            UpdateViewButtonStates();
        }

        private bool TryEditSelectedEvent(Point location)
        {
            if (_state.SelectedEvent == null)
            {
                return false;
            }

            return TryOpenEventEditor(_state.SelectedEvent, CalendarEventMutationKind.Update, location, out _);
        }
    }
}