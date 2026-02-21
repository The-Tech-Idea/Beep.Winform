using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown
    {
        private Models.DatePickerMode _mode = Models.DatePickerMode.Single;

        /// <summary>
        /// SINGLE SOURCE OF TRUTH - Defines what type of value this control returns
        /// Setting this automatically configures masks, validation, and display format
        /// </summary>
        [Browsable(true)]
        [Category("Date")]
        [Description("Defines what type of date/time value this control returns (Date, DateTime, DateRange, DateTimeRange).")]
        [DefaultValue(Models.ReturnDateTimeType.Date)]
        public Models.ReturnDateTimeType ReturnType
        {
            get => _returnType;
            set
            {
                if (_returnType != value)
                {
                    _returnType = value;
                    
                    // Automatically select a compatible mode for this return type
                    _mode = Models.DatePickerModeMapping.GetDefaultMode(_returnType);
                    
                    // Update mask format for the new type
                    UpdateMaskForMode();
                    
                    // Sync to calendar if open
                    if (_calendarView != null)
                    {
                        SyncToCalendar();
                    }
                    
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Date")]
        [Description("Functional mode of the date picker (Single, Range, SingleWithTime, RangeWithTime, etc.). Use ReturnType for simpler configuration.")]
        [DefaultValue(Models.DatePickerMode.Single)]
        public Models.DatePickerMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    
                    // Automatically sync ReturnType based on the mode's mapping
                    _returnType = Models.DatePickerModeMapping.GetReturnType(_mode);
                    
                    // Update mask format for the new mode
                    UpdateMaskForMode();
                    
                    // If popup picker exists, update its mode immediately
                    if (_calendarView != null)
                    {
                        try { _calendarView.Mode = value; } catch { }
                    }
                    
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Date")]
        [Description("Selected date/time. Null if not set.")]
        public DateTime? SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                if (_selectedDateTime != value)
                {
                    var old = _selectedDateTime;
                    _selectedDateTime = value;
                    
                    // Update text display
                    UpdateTextFromDate();
                    
                    // Fire event
                    var args = new Models.DateTimePickerEventArgs(
                        value, 
                        value?.TimeOfDay, 
                        null, 
                        _mode);
                    OnSelectedDateTimeChanged(args);
                    
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Returns the selected DateTime value for use by the grid editor.
        /// Overrides BeepTextBox.GetValue() which would return the text string.
        /// </summary>
        public override object GetValue() => _selectedDateTime;

        /// <summary>
        /// Sets the date value from the grid editor (accepts DateTime, DateTime?, or parseable string).
        /// </summary>
        public override void SetValue(object value)
        {
            if (value is DateTime dt)
                SelectedDateTime = dt;
            else if (value != null && DateTime.TryParse(value.ToString(), out var parsed))
                SelectedDateTime = parsed;
            else
                SelectedDateTime = null;
        }

        [Browsable(true)]
        [Category("Date")]
        [Description("Start date for range mode.")]
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    
                    // Update text display for range
                    if (_mode == Models.DatePickerMode.Range || _mode == Models.DatePickerMode.RangeWithTime)
                    {
                        UpdateTextFromDateRange();
                        OnDateRangeChanged(new DateRangeEventArgs(_startDate, _endDate));
                    }
                    
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Date")]
        [Description("End date for range mode.")]
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    
                    // Update text display for range
                    if (_mode == Models.DatePickerMode.Range || _mode == Models.DatePickerMode.RangeWithTime)
                    {
                        UpdateTextFromDateRange();
                        OnDateRangeChanged(new DateRangeEventArgs(_startDate, _endDate));
                    }
                    
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Whether the dropdown button is shown.")]
        public bool ShowDropDown 
        { 
            get => _showDropDown; 
            set 
            { 
                _showDropDown = value;
                
                // Update image visibility (icon is shown in place of dropdown)
                ImageVisible = value;
                
                Invalidate(); 
            } 
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow inline text editing in addition to dropdown selection.")]
        public bool AllowInlineEdit 
        { 
            get => _allowInlineEdit; 
            set 
            { 
                _allowInlineEdit = value;
                
                // Set ReadOnly based on this property
                base.ReadOnly = !value;
            } 
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow empty date values (null).")]
        public bool AllowEmpty 
        { 
            get => _allowEmpty; 
            set => _allowEmpty = value; 
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Minimum allowed date for selection.")]
        public DateTime? MinDate 
        { 
            get => _minDate; 
            set 
            {
                _minDate = value;
                
                // Update calendar if open
                if (_calendarView != null && value.HasValue)
                {
                    try { _calendarView.MinDate = value.Value; } catch { }
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Maximum allowed date for selection.")]
        public DateTime? MaxDate 
        { 
            get => _maxDate; 
            set 
            {
                _maxDate = value;
                
                // Update calendar if open
                if (_calendarView != null && value.HasValue)
                {
                    try { _calendarView.MaxDate = value.Value; } catch { }
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(" - ")]
        [Description("Separator string used between dates in range mode.")]
        public string DateSeparator
        {
            get => _dateSeparator;
            set
            {
                if (_dateSeparator != value)
                {
                    _dateSeparator = value ?? " - ";
                    
                    // Update mask if in range mode
                    if (_mode == Models.DatePickerMode.Range || _mode == Models.DatePickerMode.RangeWithTime)
                    {
                        UpdateMaskForMode();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Show calendar icon in the control.")]
        public bool ShowCalendarIcon
        {
            get => ImageVisible;
            set => ImageVisible = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Path to custom calendar icon. If empty, uses default icon.")]
        public string CalendarIconPath
        {
            get => _calendarIconPath;
            set
            {
                if (_calendarIconPath != value)
                {
                    _calendarIconPath = value;
                    
                    if (!string.IsNullOrEmpty(value))
                    {
                        ImagePath = value;
                    }
                    
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Validate date input on each keystroke.")]
        public bool ValidateOnKeyPress { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Validate date input when control loses focus.")]
        public bool ValidateOnLostFocus { get; set; } = true;
    }
}
