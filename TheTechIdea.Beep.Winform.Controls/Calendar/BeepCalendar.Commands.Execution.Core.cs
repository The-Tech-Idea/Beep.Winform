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
                        _state.VisibleRangeStart = args.AnchorDate.Value.Date;
                        _state.VisibleRangeEnd = args.VisibleRangeEnd?.Date;
                        _state.CurrentDate = args.AnchorDate.Value.Date;
                        _state.SelectedDate = args.AnchorDate.Value.Date;
                        Invalidate();
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
                    return TryEditSelectedEvent(System.Drawing.Point.Empty);

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