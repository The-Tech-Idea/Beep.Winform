using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellRender
{
    public interface ICalendarCellHost
    {
        bool UpdateEvent(CalendarEvent evt);
        bool RemoveEvent(CalendarEvent evt);
        void InvalidateCell();
        CalendarEvent SelectedEvent { get; set; }
    }
}
