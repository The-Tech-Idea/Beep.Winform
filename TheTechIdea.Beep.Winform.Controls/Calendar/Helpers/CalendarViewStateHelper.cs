using System;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
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
