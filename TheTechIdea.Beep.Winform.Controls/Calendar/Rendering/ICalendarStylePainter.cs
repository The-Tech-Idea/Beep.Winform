using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    /// <summary>
    /// Interface for calendar style painters that provide distinct visual appearances
    /// </summary>
    public interface ICalendarStylePainter
    {
        /// <summary>
        /// Gets the name of this style
        /// </summary>
        string StyleName { get; }
        
        /// <summary>
        /// Gets the display name for this style
        /// </summary>
        string DisplayName { get; }

        #region Background and Chrome
        
        /// <summary>
        /// Paints the overall calendar background
        /// </summary>
        void PaintBackground(Graphics g, Rectangle bounds, CalendarPainterContext ctx);
        
        /// <summary>
        /// Paints the calendar header (month/year title area)
        /// </summary>
        void PaintHeader(Graphics g, Rectangle bounds, string headerText, CalendarPainterContext ctx);
        
        /// <summary>
        /// Paints the view selector area background
        /// </summary>
        void PaintViewSelector(Graphics g, Rectangle bounds, CalendarPainterContext ctx);
        
        /// <summary>
        /// Paints the sidebar area
        /// </summary>
        void PaintSidebar(Graphics g, Rectangle bounds, CalendarPainterContext ctx);
        
        #endregion

        #region Day Cells
        
        /// <summary>
        /// Paints a day cell in month view
        /// </summary>
        void PaintDayCell(Graphics g, Rectangle bounds, DateTime date, DayCellState state, CalendarPainterContext ctx);
        
        /// <summary>
        /// Paints the day name header (Sun, Mon, Tue, etc.)
        /// </summary>
        void PaintDayHeader(Graphics g, Rectangle bounds, string dayName, bool isToday, CalendarPainterContext ctx);
        
        /// <summary>
        /// Paints the week day header in week view (with date)
        /// </summary>
        void PaintWeekDayHeader(Graphics g, Rectangle bounds, DateTime date, bool isToday, CalendarPainterContext ctx);
        
        #endregion

        #region Events
        
        /// <summary>
        /// Paints a compact event bar (used in month view cells)
        /// </summary>
        void PaintEventBar(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx);
        
        /// <summary>
        /// Paints a full event block (used in week/day time slots)
        /// </summary>
        void PaintEventBlock(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx);
        
        /// <summary>
        /// Paints an event in list view
        /// </summary>
        void PaintListViewEvent(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx);
        
        #endregion

        #region Time Slots
        
        /// <summary>
        /// Paints a time slot background in week/day view
        /// </summary>
        void PaintTimeSlot(Graphics g, Rectangle bounds, int hour, bool isCurrentHour, CalendarPainterContext ctx);
        
        /// <summary>
        /// Paints the time label (e.g., "09:00")
        /// </summary>
        void PaintTimeLabel(Graphics g, Rectangle bounds, int hour, CalendarPainterContext ctx);
        
        #endregion

        #region Mini Calendar
        
        /// <summary>
        /// Paints the mini calendar in the sidebar
        /// </summary>
        void PaintMiniCalendar(Graphics g, Rectangle bounds, DateTime displayMonth, DateTime selectedDate, CalendarPainterContext ctx);
        
        #endregion

        #region Event Details
        
        /// <summary>
        /// Paints the event details panel in the sidebar
        /// </summary>
        void PaintEventDetails(Graphics g, Rectangle bounds, CalendarEvent evt, CalendarPainterContext ctx);
        
        #endregion

        #region Metrics
        
        /// <summary>
        /// Gets the style-specific metrics for layout calculations
        /// </summary>
        CalendarStyleMetrics GetMetrics();
        
        #endregion
    }

    /// <summary>
    /// State information for a day cell
    /// </summary>
    public class DayCellState
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

    /// <summary>
    /// Style-specific metrics for calendar layout
    /// </summary>
    public class CalendarStyleMetrics
    {
        public int HeaderHeight { get; set; } = 60;
        public int ViewSelectorHeight { get; set; } = 40;
        public int DayHeaderHeight { get; set; } = 30;
        public int TimeSlotHeight { get; set; } = 60;
        public int EventBarHeight { get; set; } = 18;
        public int EventSpacing { get; set; } = 2;
        public int CornerRadius { get; set; } = 4;
        public int CellPadding { get; set; } = 4;
        public int SidebarWidth { get; set; } = 300;
        public int TimeColumnWidth { get; set; } = 60;
        public int MaxEventsPerCell { get; set; } = 3;
    }

    /// <summary>
    /// Context for calendar painting operations
    /// </summary>
    public class CalendarPainterContext
    {
        // Internal state - exposed via public accessors
        internal CalendarState State { get; set; }
        internal CalendarRects Rects { get; set; }
        internal CalendarEventService EventService { get; set; }
        
        // Public accessors for state
        public DateTime CurrentDate => State?.CurrentDate ?? DateTime.Today;
        public DateTime SelectedDate => State?.SelectedDate ?? DateTime.Today;
        public CalendarViewMode ViewMode => State?.ViewMode ?? CalendarViewMode.Month;
        public CalendarEvent SelectedEvent => State?.SelectedEvent;
        public bool ShowSidebar => State?.ShowSidebar ?? false;
        
        // Public fonts
        public Font HeaderFont { get; set; }
        public Font DayFont { get; set; }
        public Font EventFont { get; set; }
        public Font TimeFont { get; set; }
        public Font DaysHeaderFont { get; set; }
        public System.Collections.Generic.List<EventCategory> Categories { get; set; }
        
        // Theme colors (from IBeepTheme or StyleColors)
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BorderColor { get; set; }
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }
        public Color SelectedBackColor { get; set; }
        public Color SelectedForeColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color TodayBackColor { get; set; }
        public Color TodayForeColor { get; set; }
        public Color WeekendBackColor { get; set; }
        public Color OutOfMonthBackColor { get; set; }
        public Color OutOfMonthForeColor { get; set; }
        
        /// <summary>
        /// Gets the color for an event category
        /// </summary>
        public Color GetCategoryColor(int categoryId)
        {
            var category = Categories?.Find(c => c.Id == categoryId);
            return category?.Color ?? Color.Gray;
        }
    }
}

