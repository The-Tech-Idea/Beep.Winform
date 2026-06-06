using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellRender
{
    /// <summary>
    /// Identifies the kind of calendar cell a developer's
    /// <see cref="TheTechIdea.Beep.Vis.Modules.IBeepUIComponent"/> should be
    /// created for.
    /// </summary>
    public enum CalendarCellKind
    {
        /// <summary>Cell holds a single <see cref="CalendarEvent"/> block.</summary>
        EventBlock,
        /// <summary>Cell is a date slot in Month / Week grid (no event bound).</summary>
        DateCell,
        /// <summary>Cell is an hour / time-slot in a timed view (no event bound).</summary>
        TimeSlot,
        /// <summary>Sidebar detail card showing the currently selected event (if any).</summary>
        SidebarDetailCard
    }

    /// <summary>
    /// Context passed to a developer's
    /// <see cref="TheTechIdea.Beep.Vis.Modules.IBeepUIComponent"/> factory
    /// when a cell needs to be drawn. Carries everything the factory needs
    /// to construct a meaningful component (the event, the date, the view
    /// mode, cell coordinates) plus a free-form <see cref="UserData"/> slot
    /// the developer can populate.
    /// </summary>
    public sealed class CalendarCellContext
    {
        public CalendarCellContext(
            CalendarCellKind kind,
            CalendarEvent evt,
            DateTime date,
            CalendarViewMode viewMode,
            int row,
            int column,
            ICalendarCellHost host = null)
        {
            Kind = kind;
            Event = evt;
            Date = date;
            ViewMode = viewMode;
            Row = row;
            Column = column;
            Host = host;
        }

        public CalendarCellKind Kind { get; set; }
        public CalendarEvent Event { get; set; }
        public DateTime Date { get; set; }
        public CalendarViewMode ViewMode { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        /// <summary>Free-form bag for developer use. Survives across paint cycles.</summary>
        public object UserData { get; set; }

        /// <summary>
        /// The calendar that owns this cell. Developers can call
        /// <see cref="ICalendarCellHost.UpdateEvent"/>,
        /// <see cref="ICalendarCellHost.RemoveEvent"/>, or
        /// <see cref="ICalendarCellHost.InvalidateCell"/> to push edits
        /// back from a W8 Control into the calendar's data model.
        /// </summary>
        public ICalendarCellHost Host { get; set; }
    }
}
