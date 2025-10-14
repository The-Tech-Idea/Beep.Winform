using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Models
{
    /// <summary>
    /// Maps each DatePickerMode to its corresponding ReturnDateTimeType
    /// This defines what type of value each painter/mode returns
    /// </summary>
    public static class DatePickerModeMapping
    {
        /// <summary>
        /// Dictionary mapping each mode to its return type
        /// </summary>
        private static readonly Dictionary<DatePickerMode, ReturnDateTimeType> ModeToReturnType = new Dictionary<DatePickerMode, ReturnDateTimeType>
        {
            // Single Date Modes
            { DatePickerMode.Single, ReturnDateTimeType.Date },
            { DatePickerMode.Compact, ReturnDateTimeType.Date },
            { DatePickerMode.ModernCard, ReturnDateTimeType.Date },
            { DatePickerMode.WeekView, ReturnDateTimeType.Date },
            { DatePickerMode.MonthView, ReturnDateTimeType.Date },
            { DatePickerMode.YearView, ReturnDateTimeType.Date },
            { DatePickerMode.Header, ReturnDateTimeType.Date },
            { DatePickerMode.SidebarEvent, ReturnDateTimeType.Date },
            
            // Single Date with Time Modes
            { DatePickerMode.SingleWithTime, ReturnDateTimeType.DateTime },
            { DatePickerMode.Appointment, ReturnDateTimeType.DateTime },
            
            // Date Range Modes (no time)
            { DatePickerMode.Range, ReturnDateTimeType.DateRange },
            { DatePickerMode.DualCalendar, ReturnDateTimeType.DateRange },
            { DatePickerMode.Quarterly, ReturnDateTimeType.DateRange },
            
            // Date-Time Range Modes
            { DatePickerMode.RangeWithTime, ReturnDateTimeType.DateTimeRange },
            { DatePickerMode.Timeline, ReturnDateTimeType.DateTimeRange },
            { DatePickerMode.FlexibleRange, ReturnDateTimeType.DateTimeRange },
            { DatePickerMode.FilteredRange, ReturnDateTimeType.DateTimeRange },
            
            // Multiple Selection Mode
            { DatePickerMode.Multiple, ReturnDateTimeType.MultipleDates }
        };

        /// <summary>
        /// Dictionary mapping return type to compatible modes
        /// </summary>
        private static readonly Dictionary<ReturnDateTimeType, List<DatePickerMode>> ReturnTypeToModes = new Dictionary<ReturnDateTimeType, List<DatePickerMode>>
        {
            {
                ReturnDateTimeType.Date,
                new List<DatePickerMode>
                {
                    DatePickerMode.Single,
                    DatePickerMode.Compact,
                    DatePickerMode.ModernCard,
                    DatePickerMode.WeekView,
                    DatePickerMode.MonthView,
                    DatePickerMode.YearView,
                    DatePickerMode.Header,
                    DatePickerMode.SidebarEvent
                }
            },
            {
                ReturnDateTimeType.DateTime,
                new List<DatePickerMode>
                {
                    DatePickerMode.SingleWithTime,
                    DatePickerMode.Appointment
                }
            },
            {
                ReturnDateTimeType.DateRange,
                new List<DatePickerMode>
                {
                    DatePickerMode.Range,
                    DatePickerMode.DualCalendar,
                    DatePickerMode.Quarterly
                }
            },
            {
                ReturnDateTimeType.DateTimeRange,
                new List<DatePickerMode>
                {
                    DatePickerMode.RangeWithTime,
                    DatePickerMode.Timeline,
                    DatePickerMode.FlexibleRange,
                    DatePickerMode.FilteredRange
                }
            },
            {
                ReturnDateTimeType.MultipleDates,
                new List<DatePickerMode>
                {
                    DatePickerMode.Multiple
                }
            }
        };

        /// <summary>
        /// Get the ReturnDateTimeType for a given DatePickerMode
        /// </summary>
        public static ReturnDateTimeType GetReturnType(DatePickerMode mode)
        {
            if (ModeToReturnType.TryGetValue(mode, out var returnType))
            {
                return returnType;
            }
            
            // Default to Date if not found
            return ReturnDateTimeType.Date;
        }

        /// <summary>
        /// Get all compatible modes for a given ReturnDateTimeType
        /// </summary>
        public static List<DatePickerMode> GetCompatibleModes(ReturnDateTimeType returnType)
        {
            if (ReturnTypeToModes.TryGetValue(returnType, out var modes))
            {
                return new List<DatePickerMode>(modes); // Return copy
            }
            
            // Default to Single mode
            return new List<DatePickerMode> { DatePickerMode.Single };
        }

        /// <summary>
        /// Get the default mode for a given ReturnDateTimeType
        /// </summary>
        public static DatePickerMode GetDefaultMode(ReturnDateTimeType returnType)
        {
            var compatibleModes = GetCompatibleModes(returnType);
            return compatibleModes.Count > 0 ? compatibleModes[0] : DatePickerMode.Single;
        }

        /// <summary>
        /// Check if a mode is compatible with a return type
        /// </summary>
        public static bool IsCompatible(DatePickerMode mode, ReturnDateTimeType returnType)
        {
            return GetReturnType(mode) == returnType;
        }

        /// <summary>
        /// Get a description of what the mode returns
        /// </summary>
        public static string GetReturnTypeDescription(DatePickerMode mode)
        {
            var returnType = GetReturnType(mode);
            
            switch (returnType)
            {
                case ReturnDateTimeType.Date:
                    return "Returns DateTime? with date only (00:00:00 time)";
                    
                case ReturnDateTimeType.DateTime:
                    return "Returns DateTime? with date and time";
                    
                case ReturnDateTimeType.DateRange:
                    return "Returns (DateTime? start, DateTime? end) with dates only";
                    
                case ReturnDateTimeType.DateTimeRange:
                    return "Returns (DateTime? start, DateTime? end) with dates and times";
                    
                case ReturnDateTimeType.MultipleDates:
                    return "Returns DateTime[] array of selected dates";
                    
                case ReturnDateTimeType.TimeOnly:
                    return "Returns TimeSpan? with time only";
                    
                default:
                    return "Unknown return type";
            }
        }
    }
}
