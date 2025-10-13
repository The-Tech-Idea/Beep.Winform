using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Models
{
    /// <summary>
    /// Configuration properties for BeepDateTimePicker control
    /// </summary>
    public class DateTimePickerProperties
    {
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
        public string TimeFormat { get; set; } = "h:mm tt";

        // Visual Settings
        public DatePickerHighlightCurrentDate HighlightToday { get; set; } = DatePickerHighlightCurrentDate.Both;
        public DatePickerHighlightSelectedDate HighlightSelected { get; set; } = DatePickerHighlightSelectedDate.Both;
        public DatePickerDisabledDateStyle DisabledDateStyle { get; set; } = DatePickerDisabledDateStyle.GrayedOut;
        public DatePickerAnimationType AnimationType { get; set; } = DatePickerAnimationType.Fade;

        // Quick Selection Options (for ModernCard style)
        public bool ShowTodayButton { get; set; } = true;
        public bool ShowTomorrowButton { get; set; } = true;
        public bool ShowCustomQuickDates { get; set; } = true;

        // Appointment Picker Settings (for AppointmentPicker style)
        public TimeSpan AppointmentStartHour { get; set; } = TimeSpan.FromHours(8);
        public TimeSpan AppointmentEndHour { get; set; } = TimeSpan.FromHours(18);
        public TimeSpan AppointmentSlotDuration { get; set; } = TimeSpan.FromMinutes(30);

        // Quarterly Range Settings (for QuarterlyRange style)
        public bool ShowQuarterButtons { get; set; } = true;
        public bool ShowMonthButtons { get; set; } = true;
        public bool ShowYearButtons { get; set; } = true;

        // Dual Calendar Settings (for DualCalendar style)
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
    }
}
