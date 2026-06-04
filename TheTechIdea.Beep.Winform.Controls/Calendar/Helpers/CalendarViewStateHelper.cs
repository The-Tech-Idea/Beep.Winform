using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    /// <summary>
    /// Visual state for a single day cell in the month view. Built once per
    /// day-cell draw and consumed by <c>MonthViewPainter</c>.
    /// </summary>
    public sealed class DayCellState
    {
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsFocused { get; set; }
        public int EventCount { get; set; }
        public bool HasMoreEvents { get; set; }
    }

    internal static class CalendarViewStateHelper
    {
        public static DayCellState BuildDayCellState(
            DateTime cellDate,
            DateTime currentMonthAnchor,
            DateTime selectedDate,
            DateTime? hoveredDate,
            DateTime focusedDate,
            bool isKeyboardFocusVisible,
            int eventCount,
            int maxEventsPerCell)
        {
            return new DayCellState
            {
                IsCurrentMonth = cellDate.Month == currentMonthAnchor.Month,
                IsToday = cellDate.Date == DateTime.Today,
                IsSelected = cellDate.Date == selectedDate.Date,
                IsHovered = hoveredDate.HasValue && hoveredDate.Value.Date == cellDate.Date,
                IsWeekend = cellDate.DayOfWeek == DayOfWeek.Saturday || cellDate.DayOfWeek == DayOfWeek.Sunday,
                IsFocused = isKeyboardFocusVisible && focusedDate.Date == cellDate.Date,
                EventCount = eventCount,
                HasMoreEvents = eventCount > Math.Max(1, maxEventsPerCell)
            };
        }
    }
}
