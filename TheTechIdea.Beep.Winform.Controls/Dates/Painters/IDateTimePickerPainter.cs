using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Painters
{
    /// <summary>
    /// Interface for DateTimePicker visual rendering strategies
    /// Each painter implements a specific DatePickerMode with distinct functionality
    /// Visual styling is handled by the BeepTheme system
    /// </summary>
    public interface IDateTimePickerPainter
    {
        /// <summary>
        /// The mode this painter implements
        /// </summary>
        DatePickerMode Mode { get; }

        /// <summary>
        /// Paint the calendar dropdown panel
        /// </summary>
        void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState);

        /// <summary>
        /// Paint individual day cells
        /// </summary>
        void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange);

        /// <summary>
        /// Paint month/year header
        /// </summary>
        void PaintHeader(Graphics g, Rectangle headerBounds, string headerText, bool showNavigation, bool isHovered);

        /// <summary>
        /// Paint navigation buttons (previous/next)
        /// </summary>
        void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed);

        /// <summary>
        /// Paint time picker section (if ShowTime is enabled)
        /// </summary>
        void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState);

        /// <summary>
        /// Paint individual time slot
        /// </summary>
        void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed);

        /// <summary>
        /// Paint quick selection buttons (Today, Tomorrow, etc.)
        /// </summary>
        void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState);

        /// <summary>
        /// Paint individual quick button
        /// </summary>
        void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed);

        /// <summary>
        /// Paint action buttons (Apply, Cancel)
        /// </summary>
        void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState);

        /// <summary>
        /// Paint individual action button
        /// </summary>
        void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed);

        /// <summary>
        /// Paint range selection overlay (for range mode)
        /// </summary>
        void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth);

        /// <summary>
        /// Paint week numbers column
        /// </summary>
        void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState);

        /// <summary>
        /// Paint individual week number
        /// </summary>
        void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered);

        /// <summary>
        /// Paint day names header (Mo, Tu, We, etc.)
        /// </summary>
        void PaintDayNamesHeader(Graphics g, Rectangle headerBounds, DatePickerFirstDayOfWeek firstDayOfWeek);

        /// <summary>
        /// Calculate layout rectangles for all visual components
        /// </summary>
        DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties);

        /// <summary>
        /// Get the preferred size for the dropdown
        /// </summary>
        Size GetPreferredDropDownSize(DateTimePickerProperties properties);

        /// <summary>
        /// Hit test to determine which component was clicked
        /// </summary>
        DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth);
    }

    /// <summary>
    /// Layout information for DateTimePicker components
    /// </summary>
    public class DateTimePickerLayout
    {
        public Rectangle HeaderRect { get; set; }
        public Rectangle PreviousButtonRect { get; set; }
        public Rectangle NextButtonRect { get; set; }
        public Rectangle DayNamesRect { get; set; }
        public Rectangle CalendarGridRect { get; set; }
        public Rectangle[,] DayCellRects { get; set; } // [row, col]
        public Rectangle WeekNumbersRect { get; set; }
        public Rectangle TimePickerRect { get; set; }
        public Rectangle QuickButtonsRect { get; set; }
        public Rectangle ActionButtonsRect { get; set; }
        public Rectangle ApplyButtonRect { get; set; }
        public Rectangle CancelButtonRect { get; set; }
        public int CellWidth { get; set; }
        public int CellHeight { get; set; }
    }

    /// <summary>
    /// Hit test result for DateTimePicker
    /// </summary>
    public class DateTimePickerHitTestResult
    {
        public DateTimePickerHitArea HitArea { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public DatePickerNavigationButton? NavigationButton { get; set; }
        public int? WeekNumber { get; set; }
        public Rectangle HitBounds { get; set; }
        public string QuickButtonText { get; set; }
    }

    /// <summary>
    /// Hover state tracker for DateTimePicker visual feedback
    /// </summary>
    public class DateTimePickerHoverState
    {
        public DateTimePickerHitArea HoverArea { get; set; } = DateTimePickerHitArea.None;
        public DateTime? HoveredDate { get; set; }
        public TimeSpan? HoveredTime { get; set; }
        public int? HoveredWeekNumber { get; set; }
        public string HoveredQuickButtonText { get; set; }
        public Rectangle HoverBounds { get; set; }
        
        // Press state tracking
        public bool IsPressed { get; set; }
        public DateTimePickerHitArea PressedArea { get; set; } = DateTimePickerHitArea.None;
        public DateTime? PressedDate { get; set; }
        public TimeSpan? PressedTime { get; set; }
        public string PressedQuickButtonText { get; set; }

        /// <summary>
        /// Check if a specific date is being hovered
        /// </summary>
        public bool IsDateHovered(DateTime date)
        {
            return HoverArea == DateTimePickerHitArea.DayCell && 
                   HoveredDate.HasValue && 
                   HoveredDate.Value.Date == date.Date;
        }

        /// <summary>
        /// Check if a specific date is being pressed
        /// </summary>
        public bool IsDatePressed(DateTime date)
        {
            return IsPressed && 
                   PressedArea == DateTimePickerHitArea.DayCell && 
                   PressedDate.HasValue && 
                   PressedDate.Value.Date == date.Date;
        }

        /// <summary>
        /// Check if a specific time slot is being hovered
        /// </summary>
        public bool IsTimeHovered(TimeSpan time)
        {
            return HoverArea == DateTimePickerHitArea.TimeSlot && 
                   HoveredTime.HasValue && 
                   HoveredTime.Value == time;
        }

        /// <summary>
        /// Check if a specific time slot is being pressed
        /// </summary>
        public bool IsTimePressed(TimeSpan time)
        {
            return IsPressed && 
                   PressedArea == DateTimePickerHitArea.TimeSlot && 
                   PressedTime.HasValue && 
                   PressedTime.Value == time;
        }

        /// <summary>
        /// Check if a specific area is being hovered
        /// </summary>
        public bool IsAreaHovered(DateTimePickerHitArea area)
        {
            return HoverArea == area;
        }

        /// <summary>
        /// Check if a specific area is being pressed
        /// </summary>
        public bool IsAreaPressed(DateTimePickerHitArea area)
        {
            return IsPressed && PressedArea == area;
        }

        /// <summary>
        /// Clear hover state
        /// </summary>
        public void ClearHover()
        {
            HoverArea = DateTimePickerHitArea.None;
            HoveredDate = null;
            HoveredTime = null;
            HoveredWeekNumber = null;
            HoveredQuickButtonText = null;
            HoverBounds = Rectangle.Empty;
        }

        /// <summary>
        /// Clear press state
        /// </summary>
        public void ClearPress()
        {
            IsPressed = false;
            PressedArea = DateTimePickerHitArea.None;
            PressedDate = null;
            PressedTime = null;
            PressedQuickButtonText = null;
        }
    }

   
}

