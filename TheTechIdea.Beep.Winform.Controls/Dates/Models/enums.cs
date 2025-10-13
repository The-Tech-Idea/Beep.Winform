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
        Custom
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
        FirstFourDayWeek
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
    public enum DatePickerTimePickerStyle
    {
        // based on image i will provide and more options later

    }
}
