namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private string GetHeaderText()
        {
            var d = _state.CurrentDate;
            return _state.ViewMode switch
            {
                CalendarViewMode.Month => d.ToString("MMMM yyyy"),
                CalendarViewMode.Week => $"Week of {d.AddDays(-(int)d.DayOfWeek):MMMM dd, yyyy}",
                CalendarViewMode.WorkWeek => $"Work week of {d.AddDays(-(((int)d.DayOfWeek + 6) % 7)):MMMM dd, yyyy}",
                CalendarViewMode.Day => d.ToString("dddd, MMMM dd, yyyy"),
                CalendarViewMode.Agenda => d.ToString("MMMM yyyy") + " Agenda",
                CalendarViewMode.Timeline => $"Timeline of {d:MMMM yyyy}",
                CalendarViewMode.List => d.ToString("MMMM yyyy") + " Events",
                _ => d.ToString("MMMM yyyy")
            };
        }
    }
}