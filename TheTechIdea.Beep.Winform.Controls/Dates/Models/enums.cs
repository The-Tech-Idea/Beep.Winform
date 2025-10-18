using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Models
{
    public enum DateDropDownStyle
    {
        Default,
        Modern,
        Classic,
        Custom

    }
    public enum DatePickerFormat
    {
        Long,
        Short,
        Time,
        Custom,
        ShortDate
    }
    public enum DatePickerView
    {
        Days,
        Months,
        Years
    }
    public enum DatePickerSelectionMode
    {
        Single,
        Multiple,
        Range
    }
    public enum DatePickerCalendarType
    {
        Gregorian,
        Julian,
        Hebrew,
        Hijri,
        ThaiBuddhist,
        UmAlQura
    }
    public enum DatePickerFirstDayOfWeek
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }
    public enum DatePickerWeekNumbers
    {
        None,
        FirstDay,
        FirstFullWeek,
        FirstFourDayWeek,
        Left
    }
    public enum DatePickerSelectionChangedAction
    {
        Added,
        Removed,
        Cleared
    }
    public enum DatePickerNavigationButton
    {
        Previous,
        Next,
        Today
    }
    public enum DatePickerMonthDisplay
    {
        Full,
        Abbreviated,
        Numeric
    }
    public enum DatePickerYearDisplay
    {
        Full,
        Abbreviated,
        TwoDigit
    }
    public enum DatePickerHighlightCurrentDate
    {
        None,
        Background,
        Border,
        Both
    }
    public enum DatePickerHighlightSelectedDate
    {
        None,
        Background,
        Border,
        Both
    }
    public enum DatePickerDisabledDateStyle
    {
        GrayedOut,
        Hidden,
        Custom
    }   
    public enum DatePickerAnimationType
    {
        None,
        Fade,
        Slide,
        Zoom
    }

    /// <summary>
    /// Defines what type of date/time value is returned by the control
    /// This is the single source of truth for both BeepDateTimePicker and BeepDateDropDown
    /// </summary>
    public enum ReturnDateTimeType
    {
        /// <summary>
        /// Returns DateTime? with date only (time set to 00:00:00)
        /// Example: 10/15/2025 00:00:00
        /// </summary>
        Date,

        /// <summary>
        /// Returns DateTime? with date and time
        /// Example: 10/15/2025 2:30 PM
        /// </summary>
        DateTime,

        /// <summary>
        /// Returns (DateTime? start, DateTime? end) with dates only (times set to 00:00:00)
        /// Example: (10/15/2025 00:00:00, 10/20/2025 00:00:00)
        /// </summary>
        DateRange,

        /// <summary>
        /// Returns (DateTime? start, DateTime? end) with dates and times
        /// Example: (10/15/2025 9:00 AM, 10/15/2025 5:00 PM)
        /// </summary>
        DateTimeRange,

        /// <summary>
        /// Returns DateTime[] array of selected dates (for multiple selection)
        /// Example: [10/15/2025, 10/16/2025, 10/20/2025]
        /// </summary>
        MultipleDates,

        /// <summary>
        /// Returns TimeSpan? with time only (for time-only pickers)
        /// Example: 14:30:00
        /// </summary>
        TimeOnly
    }

    /// <summary>
    /// Defines the functional mode/type of the DateTimePicker
    /// Each mode has distinct layout and functionality, styled by current BeepTheme
    /// </summary>
    public enum DatePickerMode
    {
        Single,             // Standard single date selection calendar
        SingleWithTime,     // Single date with time picker section
        Range,              // Date range picker with start/end selection
        RangeWithTime,      // Date range with time selection
        Multiple,           // Multiple date selection with checkboxes
        Appointment,        // Calendar with time slot list for scheduling
        Timeline,           // Date range with visual timeline representation
        Quarterly,          // Quarterly range selector with Q1-Q4 shortcuts
        Compact,            // Compact dropdown with minimal chrome
        ModernCard,         // Modern card with quick date buttons (Today, Tomorrow, etc.)
        DualCalendar,       // Side-by-side month view for range selection
        WeekView,           // Week-based calendar view
        MonthView,          // Month picker view
        YearView,           // Year picker view
        SidebarEvent,       // Sidebar event calendar with large date + event list + mini calendar
        FlexibleRange,      // Flexible range picker with tabs and quick date options
        FilteredRange,      // Range picker with quick filter sidebar + dual calendar + time
        Header              // Prominent header calendar with large formatted date display
    }




    /// <summary>
    /// Hit test area
    /// </summary>
    public enum DateTimePickerHitArea
    {
        None,
        Header,
        HeaderTitle,
        PreviousButton,
        NextButton,
        PreviousYearButton,
        NextYearButton,
        PreviousDecadeButton,
        NextDecadeButton,
        DayCell,
        TimeSlot,
        QuickButton,
        TimeButton,
        TimeSpinner,
        SpinnerUpButton,
        SpinnerDownButton,
        StartHourUpButton,
        StartHourDownButton,
        StartMinuteUpButton,
        StartMinuteDownButton,
        EndHourUpButton,
        EndHourDownButton,
        EndMinuteUpButton,
        EndMinuteDownButton,
        ApplyButton,
        CancelButton,
        WeekNumber,
        DropdownButton,
        ClearButton,
        ActionButton,
        Handle,
        TimelineTrack,
        FilterButton,
        CreateButton,
        MonthButton,
        YearButton,
        QuarterButton,
        WeekRow,
        GridButton,
        FlexibleRangeButton,
        TodayButton,
        TabButton,
        YearDropdown,
        TimeInput,
        ResetButton,
        ShowResultsButton,
        YearCell,
        MonthCell,
        StartHandle,
        EndHandle,
        YearComboBox,
        MonthComboBox,
        HourComboBox,
        MinuteComboBox,
        DecadeComboBox,
        FiscalYearComboBox
    }
}
