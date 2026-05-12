namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void ApplyToolbarLabelMode(CalendarToolbarLabelMode requestedMode)
        {
            switch (requestedMode)
            {
                case CalendarToolbarLabelMode.Compact:
                    _todayButton.Text = "T";
                    _undoButton.Text = "U";
                    _redoButton.Text = "R";
                    _monthViewButton.Text = "M";
                    _weekViewButton.Text = "W";
                    _workWeekViewButton.Text = "WW";
                    _dayViewButton.Text = "D";
                    _agendaViewButton.Text = "A";
                    _timelineViewButton.Text = "T";
                    _listViewButton.Text = "L";
                    _createEventButton.Text = "+";
                    _duplicateEventButton.Text = "Dupe";
                    _editEventButton.Text = "E";
                    _deleteEventButton.Text = "X";
                    break;

                case CalendarToolbarLabelMode.Medium:
                    _todayButton.Text = "Today";
                    _undoButton.Text = "Undo";
                    _redoButton.Text = "Redo";
                    _monthViewButton.Text = "Mon";
                    _weekViewButton.Text = "Week";
                    _workWeekViewButton.Text = "Work";
                    _dayViewButton.Text = "Day";
                    _agendaViewButton.Text = "Agenda";
                    _timelineViewButton.Text = "Timeline";
                    _listViewButton.Text = "List";
                    _createEventButton.Text = "+ Event";
                    _duplicateEventButton.Text = "Duplicate";
                    _editEventButton.Text = "Edit";
                    _deleteEventButton.Text = "Delete";
                    break;

                default:
                    _todayButton.Text = "Today";
                    _undoButton.Text = "Undo";
                    _redoButton.Text = "Redo";
                    _monthViewButton.Text = "Month";
                    _weekViewButton.Text = "Week";
                    _workWeekViewButton.Text = "Work Week";
                    _dayViewButton.Text = "Day";
                    _agendaViewButton.Text = "Agenda";
                    _timelineViewButton.Text = "Timeline";
                    _listViewButton.Text = "List";
                    _createEventButton.Text = "+ Create Event";
                    _duplicateEventButton.Text = "Duplicate";
                    _editEventButton.Text = "Edit";
                    _deleteEventButton.Text = "Delete";
                    break;
            }
        }
    }
}