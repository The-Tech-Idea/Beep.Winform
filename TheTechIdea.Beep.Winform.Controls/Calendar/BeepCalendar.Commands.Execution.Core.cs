namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool ExecuteCommandCore(CalendarCommandEventArgs args)
        {
            switch (args.CommandType)
            {
                case CalendarCommandType.GoToToday:
                    NavigateToday();
                    return true;

                case CalendarCommandType.NavigatePrevious:
                    NavigatePrevious();
                    return true;

                case CalendarCommandType.NavigateNext:
                    NavigateNext();
                    return true;

                case CalendarCommandType.SwitchView:
                    if (args.TargetView.HasValue && ViewMode != args.TargetView.Value)
                    {
                        ViewMode = args.TargetView.Value;
                    }
                    return true;

                case CalendarCommandType.SetVisibleRange:
                    if (args.AnchorDate.HasValue)
                    {
                        // W2-Redo-11 BUG A - mirroring the W2-Redo-9
                        // CurrentDate-setter fix, SetVisibleRange must also
                        // sync _focusedDate / _state.FocusedDate and tear
                        // down any active W8 cell component. Otherwise
                        // (a) the keyboard navigation anchor stays at the
                        // old date so the next Left/Right arrow press
                        // jumps from the wrong cell, and (b) a hosted
                        // W8 Control remains visible at the previous
                        // cell's bounds even though that cell is no
                        // longer in view.
                        DateTime anchor = args.AnchorDate.Value.Date;
                        _state.VisibleRangeStart = anchor;
                        _state.VisibleRangeEnd = args.VisibleRangeEnd?.Date;
                        _state.CurrentDate = anchor;
                        _state.SelectedDate = anchor;
                        _focusedDate = anchor;
                        _state.FocusedDate = _focusedDate;
                        DeactivateAllCellComponents();
                        RequestLayoutAndRedraw();
                        return true;
                    }
                    return false;

                case CalendarCommandType.UndoMutation:
                    return UndoLastMutation();

                case CalendarCommandType.RedoMutation:
                    return RedoLastMutation();

                case CalendarCommandType.DeleteSelectedEvent:
                    return TryDeleteSelectedEvent();

                case CalendarCommandType.EditSelectedEvent:
                    // W2-Redo-9 GAP 4 - the previous version passed
                    // Point.Empty to TryEditSelectedEvent, which forwarded
                    // it to TryOpenEventEditor as the editor request's
                    // Location. The W4 sample editors ignore Location
                    // (they use ComputeEditorBounds), but custom
                    // ICalendarEventEditor implementations typically
                    // position a popup dialog at the click point. With
                    // Point.Empty the dialog would pop at (0, 0) â€” the
                    // form's top-left corner â€” which is jarring. Pass
                    // the center of the client area so a popup-style
                    // custom editor appears in a sensible default
                    // location.
                    {
                        var clientCenter = new System.Drawing.Point(
                            ClientRectangle.Width / 2,
                            ClientRectangle.Height / 2);
                        return TryEditSelectedEvent(clientCenter);
                    }

                case CalendarCommandType.CreateEventAtFocusedDate:
                    OnCreateEventRequested(args.AnchorDate ?? _focusedDate.Date);
                    return true;

                case CalendarCommandType.DuplicateSelectedEvent:
                    return TryDuplicateSelectedEvent();

                default:
                    return false;
            }
        }
    }
}