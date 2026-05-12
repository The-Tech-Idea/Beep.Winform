using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void InitializeControls()
        {
            // Create buttons with proper Material Design and auto-sizing based on content
            _prevButton = MakeButton("<", (s,e)=> NavigatePreviousPeriod());
            _nextButton = MakeButton(">", (s,e)=> NavigateNextPeriod());
            _todayButton = MakeButton("Today", (s,e)=> GoToToday());
            _undoButton = MakeButton("Undo", (s,e)=> UndoMutation());
            _redoButton = MakeButton("Redo", (s,e)=> RedoMutation());

            _monthViewButton = MakeButton("Month", (s,e)=> SwitchView(CalendarViewMode.Month));
            _weekViewButton  = MakeButton("Week",  (s,e)=> SwitchView(CalendarViewMode.Week));
            _dayViewButton   = MakeButton("Day",   (s,e)=> SwitchView(CalendarViewMode.Day));
            _workWeekViewButton = MakeButton("Work Week", (s,e)=> SwitchView(CalendarViewMode.WorkWeek));
            _agendaViewButton = MakeButton("Agenda", (s,e)=> SwitchView(CalendarViewMode.Agenda));
            _timelineViewButton = MakeButton("Timeline", (s,e)=> SwitchView(CalendarViewMode.Timeline));
            _listViewButton  = MakeButton("List",  (s,e)=> SwitchView(CalendarViewMode.List));

            _createEventButton = MakeButton("+ Create Event", (s,e)=> OnCreateEventRequested(_state.SelectedDate));
            _duplicateEventButton = MakeButton("Duplicate", (s,e)=> DuplicateSelectedEvent());
            _editEventButton = MakeButton("Edit", (s,e)=> EditSelectedEvent());
            _deleteEventButton = MakeButton("Delete", (s,e)=> DeleteSelectedEvent());

            Controls.AddRange(new Control[] { _prevButton, _nextButton, _todayButton, _undoButton, _redoButton,
                _monthViewButton, _weekViewButton, _workWeekViewButton, _dayViewButton, _agendaViewButton, _timelineViewButton, _listViewButton, _createEventButton, _duplicateEventButton, _editEventButton, _deleteEventButton });
        }

        private BeepButton MakeButton(string text, EventHandler handler)
        {
            var b = new BeepButton
            {
                Text = text,
                IsChild = true,
                Theme = Theme,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                //EnableMaterialStyle = true,
               // MaterialVariant = MaterialTextFieldVariant.Outlined,
                // MaterialBorderRadius = 4,
                AutoSizeContent = true,  // Enable content-based auto-sizing

            };
            b.Click += handler;

            // Force Material Design size compensation after button creation
            b.HandleCreated += (s, e) => {
                var button = s as BeepButton;

            };

            return b;
        }

    }
}