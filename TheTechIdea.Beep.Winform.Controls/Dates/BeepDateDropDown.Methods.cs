using System;
using System.Globalization;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown
    {
        /// <summary>
        /// Parse date from text input
        /// </summary>
        public DateTime? ParseDate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            // Try parsing with the configured date format
            if (DateTime.TryParseExact(text, DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            // Fallback to culture-specific parsing
            if (DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Parse date range from text input (expects format: "start_date separator end_date")
        /// </summary>
        public (DateTime? start, DateTime? end) ParseDateRange(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return (null, null);

            // Split by separator
            string[] parts = text.Split(new[] { _dateSeparator }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                return (null, null);

            DateTime? start = ParseDate(parts[0].Trim());
            DateTime? end = ParseDate(parts[1].Trim());

            return (start, end);
        }

        /// <summary>
        /// Format date for display
        /// </summary>
        public string FormatDate(DateTime? date)
        {
            if (!date.HasValue)
                return string.Empty;

            try
            {
                if (_mode == Models.DatePickerMode.SingleWithTime || _mode == Models.DatePickerMode.RangeWithTime)
                {
                    return date.Value.ToString(DateTimeFormat, CultureInfo.CurrentCulture);
                }
                else
                {
                    return date.Value.ToString(DateFormat, CultureInfo.CurrentCulture);
                }
            }
            catch
            {
                return date.Value.ToShortDateString();
            }
        }

        /// <summary>
        /// Format date range for display
        /// </summary>
        public string FormatDateRange(DateTime? start, DateTime? end)
        {
            if (!start.HasValue || !end.HasValue)
                return string.Empty;

            string startStr = FormatDate(start);
            string endStr = FormatDate(end);

            return $"{startStr}{_dateSeparator}{endStr}";
        }

        /// <summary>
        /// Validate if text is a valid date
        /// </summary>
        public bool IsValidDate(string text)
        {
            return ParseDate(text).HasValue;
        }

        /// <summary>
        /// Validate if text is a valid date range
        /// </summary>
        public bool IsValidDateRange(string text)
        {
            var (start, end) = ParseDateRange(text);
            
            if (!start.HasValue || !end.HasValue)
                return false;

            // End date must be >= start date
            if (end.Value < start.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Check if date is within min/max constraints
        /// </summary>
        public bool IsDateInRange(DateTime? date)
        {
            if (!date.HasValue)
                return _allowEmpty;

            if (_minDate.HasValue && date.Value < _minDate.Value)
                return false;

            if (_maxDate.HasValue && date.Value > _maxDate.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Update text display from selected date
        /// </summary>
        private void UpdateTextFromDate()
        {
            if (_selectedDateTime.HasValue)
            {
                Text = FormatDate(_selectedDateTime);
            }
            else
            {
                Text = string.Empty;
            }
        }

        /// <summary>
        /// Update text display from date range
        /// </summary>
        private void UpdateTextFromDateRange()
        {
            if (_startDate.HasValue && _endDate.HasValue)
            {
                Text = FormatDateRange(_startDate, _endDate);
            }
            else
            {
                Text = string.Empty;
            }
        }

        /// <summary>
        /// Parse text and update date/range based on mode
        /// </summary>
        private void ParseAndUpdateDate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                if (_allowEmpty)
                {
                    _selectedDateTime = null;
                    _startDate = null;
                    _endDate = null;
                    ClearValidationError();
                }
                else
                {
                    SetValidationError("Date is required");
                }
                return;
            }

            switch (_mode)
            {
                case Models.DatePickerMode.Single:
                case Models.DatePickerMode.SingleWithTime:
                    ParseSingleDate(text);
                    break;

                case Models.DatePickerMode.Range:
                case Models.DatePickerMode.RangeWithTime:
                    ParseRangeDate(text);
                    break;
            }
        }

        private void ParseSingleDate(string text)
        {
            var date = ParseDate(text);

            if (!date.HasValue)
            {
                SetValidationError("Invalid date format");
                return;
            }

            if (!IsDateInRange(date))
            {
                if (_minDate.HasValue && date.Value < _minDate.Value)
                {
                    SetValidationError($"Date must be on or after {FormatDate(_minDate)}");
                }
                else if (_maxDate.HasValue && date.Value > _maxDate.Value)
                {
                    SetValidationError($"Date must be on or before {FormatDate(_maxDate)}");
                }
                return;
            }

            // Valid date
            _selectedDateTime = date;
            ClearValidationError();
            
            var args = new Models.DateTimePickerEventArgs(
                date, 
                null, 
                null, 
                _mode);
            OnSelectedDateTimeChanged(args);
        }

        private void ParseRangeDate(string text)
        {
            var (start, end) = ParseDateRange(text);

            if (!start.HasValue || !end.HasValue)
            {
                SetValidationError("Invalid date range format");
                return;
            }

            if (end.Value < start.Value)
            {
                SetValidationError("End date must be on or after start date");
                return;
            }

            if (!IsDateInRange(start) || !IsDateInRange(end))
            {
                SetValidationError("Date range is outside allowed constraints");
                return;
            }

            // Valid range
            _startDate = start;
            _endDate = end;
            _selectedDateTime = start;  // Primary date for compatibility
            
            ClearValidationError();
            
            OnDateRangeChanged(new DateRangeEventArgs(start, end));
        }

        private void ClearValidationError()
        {
            try 
            { 
                SetValidationError(string.Empty); 
            } 
            catch 
            { 
                ToolTipText = string.Empty; 
            }
        }

        private void SetValidationError(string message)
        {
            try 
            { 
                base.SetValidationError(message); 
            } 
            catch 
            { 
                ToolTipText = message; 
            }
        }
    }
}
