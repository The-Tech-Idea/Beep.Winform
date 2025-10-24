using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Models
{
    /// <summary>
    /// Configuration properties for BeepDateTimePicker control
    /// </summary>
    public class DateTimePickerProperties
    {
        // Return Type - SINGLE SOURCE OF TRUTH
        /// <summary>
        /// Defines what type of date/time value this control returns
        /// This drives the behavior of both BeepDateTimePicker and BeepDateDropDown
        /// </summary>
        public ReturnDateTimeType ReturnType { get; set; } = ReturnDateTimeType.Date;

        // Display Settings
        public DatePickerFormat Format { get; set; } = DatePickerFormat.Long;
        public DatePickerView DefaultView { get; set; } = DatePickerView.Days;
        public DatePickerSelectionMode SelectionMode { get; set; } = DatePickerSelectionMode.Single;
     
        public DateDropDownStyle DropDownStyle { get; set; } = DateDropDownStyle.Default;

        // Calendar Settings
        public DatePickerCalendarType CalendarType { get; set; } = DatePickerCalendarType.Gregorian;
        public DatePickerFirstDayOfWeek FirstDayOfWeek { get; set; } = DatePickerFirstDayOfWeek.Sunday;
        public DatePickerWeekNumbers ShowWeekNumbers { get; set; } = DatePickerWeekNumbers.None;
        public DatePickerMonthDisplay MonthDisplay { get; set; } = DatePickerMonthDisplay.Full;
        public DatePickerYearDisplay YearDisplay { get; set; } = DatePickerYearDisplay.Full;

        // Date Constraints
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? SelectedDate { get; set; }
        public DateTime? RangeStartDate { get; set; }
        public DateTime? RangeEndDate { get; set; }

        // Time Settings
        public bool ShowTime { get; set; } = false;
        public TimeSpan? SelectedTime { get; set; }
        public TimeSpan TimeInterval { get; set; } = TimeSpan.FromMinutes(30);
        public int TimeIntervalMinutes { get; set; } = 30;
        public int TimeStartHour { get; set; } = 0;
        public int TimeEndHour { get; set; } = 23;
        public TimeSpan MinTime { get; set; } = TimeSpan.Zero;
        public TimeSpan MaxTime { get; set; } = new TimeSpan(23, 59, 59);
        public string TimeFormat { get; set; } = "h:mm tt";

        // Visual Settings
        public DatePickerHighlightCurrentDate HighlightToday { get; set; } = DatePickerHighlightCurrentDate.Both;
        public DatePickerHighlightSelectedDate HighlightSelected { get; set; } = DatePickerHighlightSelectedDate.Both;
        public DatePickerDisabledDateStyle DisabledDateStyle { get; set; } = DatePickerDisabledDateStyle.GrayedOut;
        public DatePickerAnimationType AnimationType { get; set; } = DatePickerAnimationType.Fade;

        // Quick Selection Options (for ModernCard Style)
        public bool ShowTodayButton { get; set; } = true;
        public bool ShowTomorrowButton { get; set; } = true;
        public bool ShowCustomQuickDates { get; set; } = true;

        // Appointment Picker Settings (for AppointmentPicker Style)
        public TimeSpan AppointmentStartHour { get; set; } = TimeSpan.FromHours(8);
        public TimeSpan AppointmentEndHour { get; set; } = TimeSpan.FromHours(18);
        public TimeSpan AppointmentSlotDuration { get; set; } = TimeSpan.FromMinutes(30);

        // Quarterly Range Settings (for QuarterlyRange Style)
        public bool ShowQuarterButtons { get; set; } = true;
        public bool ShowMonthButtons { get; set; } = true;
        public bool ShowYearButtons { get; set; } = true;

        // Dual Calendar Settings (for DualCalendar Style)
        public int MonthsToShow { get; set; } = 2;
        public bool SyncMonthNavigation { get; set; } = true;

        // Behavior Settings
        public bool CloseOnSelection { get; set; } = true;
        public bool AllowClear { get; set; } = true;
        public bool ShowApplyButton { get; set; } = true;
        public bool ShowCancelButton { get; set; } = true;

        // Custom Format Strings
        public string CustomDateFormat { get; set; }
        public string CustomTimeFormat { get; set; }
        public string CustomRangeFormat { get; set; } = "{0} - {1}";

        // Validation
        public Func<DateTime, bool> DateValidator { get; set; }
        public Func<TimeSpan, bool> TimeValidator { get; set; }

        // Dropdown Dimensions
        public Size DropDownSize { get; set; } = new Size(320, 350);
        public bool AutoSizeDropDown { get; set; } = true;

        // Customizable Text Properties for Sidebar Event Painter
        /// <summary>
        /// Header text for events section in sidebar (default: "Current Events")
        /// </summary>
        public string EventsSectionHeader { get; set; } = "Current Events";

        /// <summary>
        /// First event item text (default: "See Daily CS Image")
        /// </summary>
        public string Event1Text { get; set; } = "See Daily CS Image";

        /// <summary>
        /// Second event item text (default: "See Daily Events")
        /// </summary>
        public string Event2Text { get; set; } = "See Daily Events";

        /// <summary>
        /// Text for create event button (default: "Create an Event")
        /// </summary>
        public string CreateEventButtonText { get; set; } = "Create an Event";

        // Additional customizable text properties for other painters
        /// <summary>
        /// Today button text (default: "Today")
        /// </summary>
        public string TodayButtonText { get; set; } = "Today";

        /// <summary>
        /// Tomorrow button text (default: "Tomorrow")
        /// </summary>
        public string TomorrowButtonText { get; set; } = "Tomorrow";

        /// <summary>
        /// Yesterday button text (default: "Yesterday")
        /// </summary>
        public string YesterdayButtonText { get; set; } = "Yesterday";

        /// <summary>
        /// Apply button text (default: "Apply")
        /// </summary>
        public string ApplyButtonText { get; set; } = "Apply";

        /// <summary>
        /// Cancel button text (default: "Cancel")
        /// </summary>
        public string CancelButtonText { get; set; } = "Cancel";

        /// <summary>
        /// Clear button text (default: "Clear")
        /// </summary>
        public string ClearButtonText { get; set; } = "Clear";

        /// <summary>
        /// Week text format (default: "Week")
        /// </summary>
        public string WeekText { get; set; } = "Week";

        /// <summary>
        /// Month text format (default: "Month")
        /// </summary>
        public string MonthText { get; set; } = "Month";

        /// <summary>
        /// Year text format (default: "Year")
        /// </summary>
        public string YearText { get; set; } = "Year";

        /// <summary>
        /// Quarter text format (default: "Q{0}")
        /// </summary>
        public string QuarterTextFormat { get; set; } = "Q{0}";
    }
}
