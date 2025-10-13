using System;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Painters
{
    /// <summary>
    /// Factory for creating DateTimePicker painters based on mode
    /// Visual styling is provided by BeepTheme
    /// </summary>
    public static class DateTimePickerPainterFactory
    {
        /// <summary>
        /// Create painter based on DatePickerMode
        /// Each mode has distinct functionality, styled by BeepTheme
        /// </summary>
        public static IDateTimePickerPainter CreatePainter(
            DatePickerMode mode,
            BeepDateTimePicker owner,
            IBeepTheme theme)
        {
            return mode switch
            {
                DatePickerMode.Single => new SingleDateTimePickerPainter(owner, theme),
                DatePickerMode.SingleWithTime => new SingleWithTimeDateTimePickerPainter(owner, theme),
                DatePickerMode.Range => new RangeDateTimePickerPainter(owner, theme),
                DatePickerMode.RangeWithTime => new RangeWithTimeDateTimePickerPainter(owner, theme),
                DatePickerMode.Multiple => new MultipleDateTimePickerPainter(owner, theme),
                DatePickerMode.Appointment => new AppointmentDateTimePickerPainter(owner, theme),
                DatePickerMode.Timeline => new TimelineDateTimePickerPainter(owner, theme),
                DatePickerMode.Quarterly => new QuarterlyDateTimePickerPainter(owner, theme),
                DatePickerMode.Compact => new CompactDateTimePickerPainter(owner, theme),
                DatePickerMode.ModernCard => new ModernCardDateTimePickerPainter(owner, theme),
                DatePickerMode.DualCalendar => new DualCalendarDateTimePickerPainter(owner, theme),
                DatePickerMode.WeekView => new WeekViewDateTimePickerPainter(owner, theme),
                DatePickerMode.MonthView => new MonthViewDateTimePickerPainter(owner, theme),
                DatePickerMode.YearView => new YearViewDateTimePickerPainter(owner, theme),
                _ => new SingleDateTimePickerPainter(owner, theme) // Default fallback
            };
        }
    }
}
