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
        /// isStartDate: true if this cell is the range start date (for distinct styling)
        /// isEndDate: true if this cell is the range end date (for distinct styling)
        /// </summary>
        void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false);

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
        /// Get the preferred size for direct rendering (no dropdown)
        /// </summary>
        Size GetPreferredSize(DateTimePickerProperties properties);

        /// <summary>
        /// Get the minimum size required by this painter
        /// </summary>
        Size GetMinimumSize(DateTimePickerProperties properties);

    
    }

    /// <summary>
    /// Layout information for DateTimePicker components
    /// </summary>
    /// <summary>
    /// Layout information for a single calendar month grid.
    /// Used in multi-month layouts (e.g., dual-month range pickers).
    /// </summary>
    public class CalendarMonthGrid
    {
        public Rectangle GridRect { get; set; }
        public Rectangle TitleRect { get; set; }
        public Rectangle PreviousButtonRect { get; set; }
        public Rectangle NextButtonRect { get; set; }
        public Rectangle DayNamesRect { get; set; }
        public Rectangle YearComboBoxRect { get; set; }  // Year combo box in header
        public List<Rectangle> DayCellRects { get; set; } // Flattened list of day cells (row by row)
        public List<Rectangle> WeekNumberRects { get; set; }
        public DateTime DisplayMonth { get; set; } // The month this grid represents
    }

    /// <summary>
    /// Layout information for DateTimePicker.
    /// Supports both single-month and multi-month layouts.
    /// </summary>
    public class DateTimePickerLayout
    {
        // Legacy single-month layout (maintained for backward compatibility)
        public Rectangle HeaderRect { get; set; }
        public Rectangle TitleRect { get; set; }
        public Rectangle PreviousButtonRect { get; set; }
        public Rectangle NextButtonRect { get; set; }
        public Rectangle DayNamesRect { get; set; }
        public Rectangle CalendarGridRect { get; set; }

        private List<Rectangle> _dayCellRects;
        private Rectangle[,] _dayCellMatrix;
        public List<Rectangle> DayCellRects
        {
            get => _dayCellRects;
            set
            {
                _dayCellRects = value;
                if (value == null)
                {
                    _dayCellMatrix = null;
                }
                else
                {
                    int rows = 6;
                    int cols = 7;
                    _dayCellMatrix = new Rectangle[rows, cols];
                    for (int i = 0; i < value.Count; i++)
                    {
                        int row = i / cols;
                        int col = i % cols;
                        if (row < rows && col < cols)
                        {
                            _dayCellMatrix[row, col] = value[i];
                        }
                    }
                }
            }
        }

        public Rectangle[,] DayCellMatrix
        {
            get => _dayCellMatrix;
            set
            {
                _dayCellMatrix = value;
                if (value == null)
                {
                    _dayCellRects = null;
                }
                else
                {
                    int rows = value.GetLength(0);
                    int cols = value.GetLength(1);
                    var list = new List<Rectangle>(rows * cols);
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        {
                            list.Add(value[row, col]);
                        }
                    }
                    _dayCellRects = list;
                }
            }
        }

        public Rectangle[,] GetDayCellMatrixOrDefault(int defaultRows = 6, int defaultCols = 7)
        {
            if (_dayCellMatrix != null)
            {
                return _dayCellMatrix;
            }

            if (defaultRows <= 0)
            {
                defaultRows = 6;
            }

            if (defaultCols <= 0)
            {
                defaultCols = 7;
            }

            if (_dayCellRects != null && _dayCellRects.Count > 0)
            {
                var matrix = new Rectangle[defaultRows, defaultCols];
                int limit = Math.Min(_dayCellRects.Count, defaultRows * defaultCols);
                for (int i = 0; i < limit; i++)
                {
                    matrix[i / defaultCols, i % defaultCols] = _dayCellRects[i];
                }

                DayCellMatrix = matrix;
                return _dayCellMatrix;
            }

            DayCellMatrix = new Rectangle[defaultRows, defaultCols];
            return _dayCellMatrix;
        }

        public Rectangle GetDayCellRect(int row, int col, int defaultRows = 6, int defaultCols = 7)
        {
            var matrix = GetDayCellMatrixOrDefault(defaultRows, defaultCols);
            if (matrix == null)
            {
                return Rectangle.Empty;
            }

            if (row < 0 || row >= matrix.GetLength(0) || col < 0 || col >= matrix.GetLength(1))
            {
                return Rectangle.Empty;
            }

            return matrix[row, col];
        }

        public List<Rectangle> WeekNumberRects { get; set; }
        public Rectangle TodayButtonRect { get; set; }
        public Rectangle ClearButtonRect { get; set; }
        public Rectangle TabExactRect { get; set; }
        public Rectangle TabFlexibleRect { get; set; }
        public Dictionary<string, Rectangle> FlexibleButtons { get; set; }
        public Dictionary<string, Rectangle> FilterButtons { get; set; }
        
        // Multi-month layout support (for range pickers, dual calendar views)
        public List<CalendarMonthGrid> MonthGrids { get; set; }
        
        // MonthView and YearView picker layouts
        public Rectangle PreviousYearButtonRect { get; set; }
        public Rectangle NextYearButtonRect { get; set; }
        public List<Rectangle> MonthCellRects { get; set; }
        public Rectangle PreviousDecadeButtonRect { get; set; }
        public Rectangle NextDecadeButtonRect { get; set; }
        public List<Rectangle> YearCellRects { get; set; }
        
        // ComboBox controls for direct year/month selection
        public Rectangle YearComboBoxRect { get; set; }
        public Rectangle MonthComboBoxRect { get; set; }
        
        // FilteredRange picker layouts
        public Rectangle SidebarRect { get; set; }
        public Rectangle FilterTitleRect { get; set; }
        public List<Rectangle> FilterButtonRects { get; set; }
        public Rectangle MainContentRect { get; set; }
        public Rectangle DualCalendarContainerRect { get; set; }
        
        // Left calendar (FilteredRange)
        public Rectangle LeftYearDropdownRect { get; set; }
        public Rectangle LeftHeaderRect { get; set; }
        public Rectangle LeftDayNamesRect { get; set; }
        public Rectangle LeftCalendarGridRect { get; set; }
        public List<Rectangle> LeftDayCellRects { get; set; }
        
        // Right calendar (FilteredRange)
        public Rectangle RightYearDropdownRect { get; set; }
        public Rectangle RightHeaderRect { get; set; }
        public Rectangle RightDayNamesRect { get; set; }
        public Rectangle RightCalendarGridRect { get; set; }
        public List<Rectangle> RightDayCellRects { get; set; }
        
        // Time picker row (FilteredRange)
        public Rectangle TimePickerRowRect { get; set; }
        public Rectangle FromLabelRect { get; set; }
        public Rectangle FromTimeInputRect { get; set; }
        public Rectangle ToLabelRect { get; set; }
        public Rectangle ToTimeInputRect { get; set; }
        
        // Action buttons (FilteredRange)
        public Rectangle ActionButtonRowRect { get; set; }
        public Rectangle ResetButtonRect { get; set; }
        public Rectangle ShowResultsButtonRect { get; set; }
        
        // WeekNumber column
        public Rectangle WeekNumberColumnRect { get; set; }
        
        // Timeline
        public Rectangle TimelineRect { get; set; }
        public List<Rectangle> EventMarkerRects { get; set; }
        public List<Rectangle> TimelineSegmentRects { get; set; }
        
        // Year selector dropdown
        public Rectangle YearSelectorRect { get; set; }
        public Rectangle YearDropdownArrowRect { get; set; }
        public Rectangle SelectedRangeDisplayRect { get; set; }
        
        // Common layout elements
    public Rectangle TimePickerRect { get; set; }
    public List<Rectangle> TimeSlotRects { get; set; }

    // Single time picker layout (for SingleWithTime mode)
    public Rectangle TimeHourRect { get; set; }
    public Rectangle TimeMinuteRect { get; set; }
    public Rectangle TimeColonRect { get; set; }
    public Rectangle TimeHourUpRect { get; set; }
    public Rectangle TimeHourDownRect { get; set; }
    public Rectangle TimeMinuteUpRect { get; set; }
    public Rectangle TimeMinuteDownRect { get; set; }

    // RangeWithTime time picker layout
    public Rectangle TimeSeparatorRect { get; set; }
    public Rectangle StartTimePickerRect { get; set; }
    public Rectangle EndTimePickerRect { get; set; }
    public Rectangle StartTimeDisplayRect { get; set; }
    public Rectangle EndTimeDisplayRect { get; set; }
    public Rectangle StartTimeColonRect { get; set; }
    public Rectangle EndTimeColonRect { get; set; }
    public Rectangle StartTimeHourRect { get; set; }
    public Rectangle StartTimeMinuteRect { get; set; }
    public Rectangle EndTimeHourRect { get; set; }
    public Rectangle EndTimeMinuteRect { get; set; }
    public Rectangle StartTimeHourUpRect { get; set; }
    public Rectangle StartTimeHourDownRect { get; set; }
    public Rectangle StartTimeMinuteUpRect { get; set; }
    public Rectangle StartTimeMinuteDownRect { get; set; }
    public Rectangle EndTimeHourUpRect { get; set; }
    public Rectangle EndTimeHourDownRect { get; set; }
        public Rectangle EndTimeMinuteUpRect { get; set; }
        public Rectangle EndTimeMinuteDownRect { get; set; }
        public Rectangle QuickButtonsRect { get; set; }
        public List<Rectangle> QuickButtonRects { get; set; }
        
        // Quick date buttons (for Quarterly, FlexibleRange, ModernCard modes)
        // Stores button key (e.g., "Q1", "Q2", "exact_dates", "Today") and its bounds
        public List<(string Key, Rectangle Bounds)> QuickDateButtons { get; set; }
        
        public Rectangle ActionButtonsRect { get; set; }
        public Rectangle ApplyButtonRect { get; set; }
        public Rectangle CancelButtonRect { get; set; }
        public Rectangle TimeScrollUpRect { get; set; }
        public Rectangle TimeScrollDownRect { get; set; }
        
        // Legacy time spinner (kept for backward compatibility)
        public Rectangle TimeSpinnerRect { get; set; }
        
        // Sizing information
        public int CellWidth { get; set; }
        public int CellHeight { get; set; }
        
        // Helper to determine if this is a multi-month layout
        public bool IsMultiMonthLayout => MonthGrids != null && MonthGrids.Count > 1;

      

    }

    /// <summary>
    /// Hit test result for DateTimePicker
    /// </summary>
    public class DateTimePickerHitTestResult
    {
        // Hit detection
        public bool IsHit { get; set; }
        public DateTimePickerHitArea HitArea { get; set; }
        public Rectangle HitBounds { get; set; }
        
        // Date and time
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        
        // Cell positioning (for single month and multi-month grids)
        public int? CellIndex { get; set; }
        public int? GridIndex { get; set; }
        
        // Time picker
        public int? TimeSlotIndex { get; set; }
        
        // Flexible range mode
        public string FlexibleOption { get; set; }
        public string QuarterKey { get; set; }
        public string MonthKey { get; set; }
        public string WeekKey { get; set; }
        public string DayKey { get; set; }
        public int? YearValue { get; set; }
        public string KeyValue { get; set; }



        // Filtered range mode
        public string FilterName { get; set; }
        public bool IsFiltered { get; set; }
        public object FilterValue { get; set; }
        public object PreviousFilterValue { get; set; }
        public object CustomData { get; set; }

        // Legacy properties (kept for backwards compatibility if needed by painters)
        public DatePickerNavigationButton? NavigationButton { get; set; }
        public int? WeekNumber { get; set; }
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
              public string HoveredButton { get; set; }
              public (DateTime Start, DateTime End)? HoveredRangePreview { get; set; }
        public Rectangle HoverBounds { get; set; }
        public int HoveredGridIndex { get; set; } = -1;  // For dual calendar tracking
        
        // Press state tracking
        public bool IsPressed { get; set; }
        public DateTimePickerHitArea PressedArea { get; set; } = DateTimePickerHitArea.None;
        public DateTime? PressedDate { get; set; }
        public TimeSpan? PressedTime { get; set; }
        public string PressedQuickButtonText { get; set; }
        public int PressedGridIndex { get; set; } = -1;  // For dual calendar tracking

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
            HoveredButton = null;
            HoveredRangePreview = null;
            HoverBounds = Rectangle.Empty;
            HoveredGridIndex = -1;
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
            PressedGridIndex = -1;
        }
    }

   
}

