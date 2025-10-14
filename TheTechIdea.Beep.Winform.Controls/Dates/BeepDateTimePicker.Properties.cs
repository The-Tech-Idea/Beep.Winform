using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// BeepDateTimePicker - Properties partial
    /// </summary>
    public partial class BeepDateTimePicker
    {
        #region Style Properties

        [Category("Appearance")]
        [Description("Functional mode of the date/time picker (Single, Range, Appointment, etc.). Visual styling follows current BeepTheme.")]
        [DefaultValue(DatePickerMode.Single)]
        public DatePickerMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    UpdatePainter();
                    Invalidate();
                }
            }
        }

        #endregion

        #region Date/Time Value Properties

        [Category("Data")]
        [Description("The selected date value")]
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnDateChanged(value);
                    Invalidate();
                }
            }
        }

        [Category("Data")]
        [Description("Selected dates collection for Multiple mode.")]
        public System.Collections.Generic.List<DateTime> SelectedDates
        {
            get => _selectedDates;
            set
            {
                var newList = value ?? new System.Collections.Generic.List<DateTime>();
                // Normalize to Date component only and distinct
                _selectedDates = newList
                    .Select(d => d.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();
                Invalidate();
            }
        }

        [Category("Data")]
        [Description("The selected time value")]
        public TimeSpan? SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (_selectedTime != value)
                {
                    _selectedTime = value;
                    OnTimeChanged(value);
                    Invalidate();
                }
            }
        }

        [Category("Data")]
        [Description("Start date for range selection mode")]
        public DateTime? RangeStartDate
        {
            get => _rangeStartDate;
            set
            {
                if (_rangeStartDate != value)
                {
                    _rangeStartDate = value;
                    OnRangeChanged(_rangeStartDate, _rangeEndDate);
                    Invalidate();
                }
            }
        }

        [Category("Data")]
        [Description("End date for range selection mode")]
        public DateTime? RangeEndDate
        {
            get => _rangeEndDate;
            set
            {
                if (_rangeEndDate != value)
                {
                    _rangeEndDate = value;
                    OnRangeChanged(_rangeStartDate, _rangeEndDate);
                    Invalidate();
                }
            }
        }

        #endregion

        #region Constraint Properties

        [Category("Behavior")]
        [Description("Minimum selectable date")]
        public DateTime MinDate
        {
            get => _minDate;
            set
            {
                if (_minDate != value)
                {
                    _minDate = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("Maximum selectable date")]
        public DateTime MaxDate
        {
            get => _maxDate;
            set
            {
                if (_maxDate != value)
                {
                    _maxDate = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("Minimum selectable time")]
        public TimeSpan MinTime
        {
            get => _minTime;
            set
            {
                if (_minTime != value)
                {
                    _minTime = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("Maximum selectable time")]
        public TimeSpan MaxTime
        {
            get => _maxTime;
            set
            {
                if (_maxTime != value)
                {
                    _maxTime = value;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Mode and Format Properties

     

        [Category("Appearance")]
        [Description("Date display format")]
        [DefaultValue(DatePickerFormat.ShortDate)]
        public DatePickerFormat Format
        {
            get => _format;
            set
            {
                if (_format != value)
                {
                    _format = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("First day of the week for calendar display")]
        [DefaultValue(DatePickerFirstDayOfWeek.Monday)]
        public DatePickerFirstDayOfWeek FirstDayOfWeek
        {
            get => _firstDayOfWeek;
            set
            {
                if (_firstDayOfWeek != value)
                {
                    _firstDayOfWeek = value;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Feature Toggle Properties

        [Category("Behavior")]
        [Description("Show time picker alongside date picker")]
        [DefaultValue(false)]
        public bool ShowTime
        {
            get => _showTime;
            set
            {
                if (_showTime != value)
                {
                    _showTime = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("Show quick selection buttons (Today, Tomorrow, etc.)")]
        [DefaultValue(true)]
        public bool ShowQuickButtons
        {
            get => _showQuickButtons;
            set
            {
                if (_showQuickButtons != value)
                {
                    _showQuickButtons = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("Show week numbers in calendar")]
        [DefaultValue(false)]
        public bool ShowWeekNumbers
        {
            get => _showWeekNumbers;
            set
            {
                if (_showWeekNumbers != value)
                {
                    _showWeekNumbers = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("Allow clearing the selected date")]
        [DefaultValue(true)]
        public bool AllowClear
        {
            get => _allowClear;
            set
            {
                if (_allowClear != value)
                {
                    _allowClear = value;
                    UpdateLayout();
                    Invalidate();
                }
            }
        }

        #endregion

        #region Time Picker Properties

        [Category("Behavior")]
        [Description("Time selection interval in minutes")]
        [DefaultValue(30)]
        public int TimeIntervalMinutes
        {
            get => _timeIntervalMinutes;
            set
            {
                if (_timeIntervalMinutes != value && value > 0)
                {
                    _timeIntervalMinutes = value;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Quick Button Properties
        // Legacy per-button toggles removed in direct-rendering redesign.
        // Use ShowQuickButtons + painter-specific quick buttons defined by theme/properties.
        // Note: Legacy per-button toggle 'ShowThisWeek' has been removed.
        // Quick buttons visibility is controlled by the single 'ShowQuickButtons' flag.
        // Painters decide which quick options to show via DateTimePickerProperties.ShowCustomQuickDates.

        // Legacy per-button toggle removed: 'ShowThisMonth'. Use ShowQuickButtons and theme configuration instead.

        #endregion

        #region Font Properties

        [Category("Appearance")]
        [Description("Use DPI-scaled font")]
        [DefaultValue(false)]
        public bool UseScaledFont
        {
            get => _useScaledFont;
            set
            {
                if (_useScaledFont != value)
                {
                    _useScaledFont = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Use theme font settings")]
        [DefaultValue(true)]
        public bool UseThemeFont
        {
            get => _useThemeFont;
            set
            {
                if (_useThemeFont != value)
                {
                    _useThemeFont = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Custom text font")]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont != value)
                {
                    _textFont?.Dispose();
                    _textFont = value;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Read-Only State Properties

    // Legacy dropdown state removed. Control renders directly with no dropdown.

        [Browsable(false)]
        public DateTime DisplayMonth => _displayMonth;

        #endregion
    }
}
