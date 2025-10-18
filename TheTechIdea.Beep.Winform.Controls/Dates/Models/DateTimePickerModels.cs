using System;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Models
{
    /// <summary>
    /// Event arguments for DateTimePicker events
    /// </summary>
    public class DateTimePickerEventArgs : EventArgs
    {
        public DateTime? Date { get; }
        public TimeSpan? Time { get; }
        public DateTime? RangeEnd { get; }
        public DatePickerMode Mode { get; }

        public DateTimePickerEventArgs(DateTime? date, TimeSpan? time, DateTime? rangeEnd, DatePickerMode mode)
        {
            Date = date;
            Time = time;
            RangeEnd = rangeEnd;
            Mode = mode;
        }
    }

    /// <summary>
    
    // Enums moved to enums.cs for better organization
    // DatePickerMode - defines functional behavior (Single, Range, Appointment, etc.)
    // DatePickerTheme - defines visual styling (Material3, iOS15, Fluent2, etc.)
    // DatePickerTimePickerStyle - DEPRECATED, kept for backward compatibility

    /// <summary>
    /// First day of week - MOVED to enums.cs
    /// </summary>

  
}
