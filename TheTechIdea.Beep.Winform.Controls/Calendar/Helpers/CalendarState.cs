using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar; // for CalendarViewMode, CalendarEvent

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    // Holds current calendar state
    internal class CalendarState
    {
        public DateTime CurrentDate { get; set; } = DateTime.Today;
        public CalendarViewMode ViewMode { get; set; } = CalendarViewMode.Month;
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public CalendarEvent SelectedEvent { get; set; }
        public bool ShowSidebar { get; set; } = false; // Default to false to avoid empty space
    }

    // Rectangles computed by layout
    internal class CalendarRects
    {
        public Rectangle HeaderRect;
        public Rectangle ViewSelectorRect;
        public Rectangle NavigationRect; // reserved in case we want separate area
        public Rectangle CalendarGridRect;
        public Rectangle SidebarRect;
    }

    // Metrics and constants
    internal static class CalendarLayoutMetrics
    {
        public const int HeaderHeight = 60;
        public const int BaseViewSelectorHeight = 40;
        public const int SelectorVerticalPadding = 8;
        public const int SectionSpacing = 4;
        public const int SidebarWidth = 300;
        public const int DayHeaderHeight = 30;
        public const int TimeSlotHeight = 60;
    }
}
