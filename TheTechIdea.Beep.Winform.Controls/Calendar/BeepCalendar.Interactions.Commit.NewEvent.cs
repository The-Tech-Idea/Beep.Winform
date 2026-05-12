using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private bool CommitNewEventMutation(Point location, Point delta)
        {
            var proposedStart = BuildProposedStart(location, delta) ?? _state.SelectedDate.Date;
            var proposedEnd = BuildProposedEnd(location, delta) ?? proposedStart.AddHours(1);

            var created = new CalendarEvent
            {
                Id = NextEventId(),
                Title = "New Event",
                Description = string.Empty,
                StartTime = proposedStart,
                EndTime = proposedEnd,
                IsAllDay = false,
                CategoryId = 0,
                Status = CalendarEventStatus.Tentative
            };

            NormalizeEventDuration(created);
            if (!TryAddEvent(created))
            {
                return false;
            }

            _state.SelectedEvent = created;
            _state.SelectedDate = created.StartTime.Date;
            _state.CurrentDate = created.StartTime.Date;
            _focusedDate = created.StartTime.Date;
            _state.FocusedDate = _focusedDate;
            return true;
        }
    }
}