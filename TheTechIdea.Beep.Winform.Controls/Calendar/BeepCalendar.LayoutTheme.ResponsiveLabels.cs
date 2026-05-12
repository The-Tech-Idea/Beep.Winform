namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void ApplyResponsiveButtonLabels(int availableWidth)
        {
            if (_todayButton == null || _monthViewButton == null || _weekViewButton == null ||
                _workWeekViewButton == null || _dayViewButton == null || _agendaViewButton == null || _timelineViewButton == null || _listViewButton == null || _createEventButton == null || _duplicateEventButton == null || _editEventButton == null || _deleteEventButton == null || _undoButton == null || _redoButton == null)
            {
                return;
            }

            CalendarToolbarLabelMode requestedMode = availableWidth < 720
                ? CalendarToolbarLabelMode.Compact
                : availableWidth < 980
                    ? CalendarToolbarLabelMode.Medium
                    : CalendarToolbarLabelMode.Full;

            if (_toolbarLabelMode == requestedMode)
            {
                return;
            }

            _toolbarLabelMode = requestedMode;
            ApplyToolbarLabelMode(requestedMode);
        }
    }
}