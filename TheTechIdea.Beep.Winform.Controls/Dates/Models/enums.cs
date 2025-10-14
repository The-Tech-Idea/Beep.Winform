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
}
