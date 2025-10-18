using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// BeepDateTimePicker - Methods partial: Public methods for date/time operations
    /// NO DROPDOWN - Direct painting like BeepDatePickerView
    /// </summary>
    public partial class BeepDateTimePicker
    {
        #region Date Selection Methods

        public void SetDate(DateTime date)
        {
            if (date < _minDate || date > _maxDate) return;

            _selectedDate = date;
            _displayMonth = new DateTime(date.Year, date.Month, 1);
            OnDateChanged(date);
            Invalidate();
        }

        public void SetTime(TimeSpan time)
        {
            if (time < _minTime || time > _maxTime) return;

            _selectedTime = time;
            OnTimeChanged(time);
            Invalidate();
        }

        public void SetRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            if (startDate < _minDate || endDate > _maxDate) return;

            _rangeStartDate = startDate;
            _rangeEndDate = endDate;
            _rangeStartTime = null;
            _rangeEndTime = null;
            _displayMonth = new DateTime(startDate.Year, startDate.Month, 1);
            OnRangeChanged(startDate, endDate);
            Invalidate();
        }

        public void ClearSelection()
        {
            _selectedDate = null;
            _selectedTime = null;
            _rangeStartDate = null;
            _rangeEndDate = null;
            _rangeStartTime = null;
            _rangeEndTime = null;
            _selectedDates?.Clear();
            
            OnDateChanged(null);
            ClearClicked?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        public void SetToday()
        {
            var today = DateTime.Today;
            if (today >= _minDate && today <= _maxDate)
            {
                SetDate(today);
            }
        }

        public void NavigateDate(int days)
        {
            if (!_selectedDate.HasValue) return;

            var newDate = _selectedDate.Value.AddDays(days);
            if (newDate >= _minDate && newDate <= _maxDate)
            {
                SetDate(newDate);
            }
        }

        #endregion

        #region Month Navigation

        public void NavigateToPreviousMonth()
        {
            _displayMonth = _displayMonth.AddMonths(-1);
            if (_displayMonth < _minDate)
            {
                _displayMonth = new DateTime(_minDate.Year, _minDate.Month, 1);
            }
            Invalidate();
        }

        public void NavigateToNextMonth()
        {
            _displayMonth = _displayMonth.AddMonths(1);
            if (_displayMonth > _maxDate)
            {
                _displayMonth = new DateTime(_maxDate.Year, _maxDate.Month, 1);
            }
            Invalidate();
        }

        public void NavigateToMonth(int year, int month)
        {
            if (month < 1 || month > 12) return;

            var newMonth = new DateTime(year, month, 1);
            if (newMonth >= _minDate && newMonth <= _maxDate)
            {
                _displayMonth = newMonth;
                Invalidate();
            }
        }

        public void NavigateToPreviousYear()
        {
            _displayMonth = _displayMonth.AddYears(-1);
            if (_displayMonth < _minDate)
            {
                _displayMonth = new DateTime(_minDate.Year, _minDate.Month, 1);
            }
            Invalidate();
        }

        public void NavigateToNextYear()
        {
            _displayMonth = _displayMonth.AddYears(1);
            if (_displayMonth > _maxDate)
            {
                _displayMonth = new DateTime(_maxDate.Year, _maxDate.Month, 1);
            }
            Invalidate();
        }

        #endregion

        #region Quick Selection Methods

        public void SelectToday()
        {
            SetToday();
            QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(DateTime.Today, null, null, _mode));
        }

        public void SelectTomorrow()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            if (tomorrow <= _maxDate)
            {
                SetDate(tomorrow);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(tomorrow, null, null, _mode));
            }
        }

        public void SelectYesterday()
        {
            var yesterday = DateTime.Today.AddDays(-1);
            if (yesterday >= _minDate)
            {
                SetDate(yesterday);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(yesterday, null, null, _mode));
            }
        }

        public void SelectThisWeek()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            if (_mode == DatePickerMode.Range)
            {
                SetRange(startOfWeek, endOfWeek);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(startOfWeek, null, endOfWeek, _mode));
            }
            else
            {
                SetDate(startOfWeek);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(startOfWeek, null, null, _mode));
            }
        }

        public void SelectThisMonth()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            if (_mode == DatePickerMode.Range)
            {
                SetRange(startOfMonth, endOfMonth);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(startOfMonth, null, endOfMonth, _mode));
            }
            else
            {
                SetDate(startOfMonth);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(startOfMonth, null, null, _mode));
            }
        }

        #endregion

        #region Helper Methods

        public bool IsDateInRange(DateTime date)
        {
            return date >= _minDate && date <= _maxDate;
        }

        public bool IsTimeInRange(TimeSpan time)
        {
            return time >= _minTime && time <= _maxTime;
        }

        public bool IsDateDisabled(DateTime date)
        {
            return !IsDateInRange(date);
        }

        #endregion

        #region Helper Methods - Multiple Date Selection

        public void ToggleMultipleDateSelection(DateTime date)
        {
            if (date < _minDate || date > _maxDate) return;
            date = date.Date;

            int idx = _selectedDates.FindIndex(d => d == date);
            if (idx >= 0)
            {
                _selectedDates.RemoveAt(idx);
            }
            else
            {
                _selectedDates.Add(date);
                _selectedDates.Sort();
            }
            OnDateChanged(date);
            Invalidate();
        }

        public void HandleRangeDateSelection(DateTime date)
        {
            if (date < _minDate || date > _maxDate) return;
            date = date.Date;

            // If no start date, or both start and end are set, start a new range
            if (!_rangeStartDate.HasValue || (_rangeStartDate.HasValue && _rangeEndDate.HasValue))
            {
                _rangeStartDate = date;
                _rangeEndDate = null;
            }
            // If start date is set but no end date, set the end date
            else if (_rangeStartDate.HasValue && !_rangeEndDate.HasValue)
            {
                if (date >= _rangeStartDate.Value)
                {
                    _rangeEndDate = date;
                }
                else
                {
                    // Swap if end is before start
                    _rangeEndDate = _rangeStartDate;
                    _rangeStartDate = date;
                }
                OnRangeChanged(_rangeStartDate, _rangeEndDate);
            }
            
            Invalidate();
        }

        private void ToggleMultipleDate(DateTime date)
        {
            if (date < _minDate || date > _maxDate) return;
            date = date.Date;

            int idx = _selectedDates.FindIndex(d => d == date);
            if (idx >= 0)
            {
                _selectedDates.RemoveAt(idx);
            }
            else
            {
                _selectedDates.Add(date);
                _selectedDates.Sort();
            }
            Invalidate();
        }

        private bool IsMultipleDateSelected(DateTime date)
        {
            return _selectedDates?.Any(d => d.Date == date.Date) == true;
        }

        #endregion

        #region Time Scrolling Methods (Appointment Mode)

        /// <summary>
        /// Scroll the time list up (used by AppointmentHitHandler)
        /// </summary>
        public void ScrollTimeListUp()
        {
            // This would typically adjust a scroll offset in the painter
            // For now, just trigger a repaint
            Invalidate();
        }

        /// <summary>
        /// Scroll the time list down (used by AppointmentHitHandler)
        /// </summary>
        public void ScrollTimeListDown()
        {
            // This would typically adjust a scroll offset in the painter
            // For now, just trigger a repaint
            Invalidate();
        }

        #endregion

        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            // Apply calendar-specific theme properties to the control
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;
            
            // CRITICAL: Reinitialize the painter with the updated theme
            // so it gets the new calendar color scheme
            if (_currentPainter != null)
            {
                _currentPainter = DateTimePickerPainterFactory.CreatePainter(_mode, this, _currentTheme);
            }
            
            // The painters now use these calendar theme colors:
            // - CalendarTitleForColor (headers, navigation buttons)
            // - CalendarDaysHeaderForColor (day names like Mon, Tue, Wed)
            // - CalendarSelectedDateBackColor (selected date backgrounds, accent colors)
            // - CalendarSelectedDateForColor (text color on selected dates)
            // - CalendarTodayForeColor (today indicator ring/outline)
            // - CalendarForeColor (regular unselected date text)
            // - CalendarBorderColor (borders around calendar elements)
            // - CalendarHoverBackColor (hover state backgrounds)

            Invalidate();
        }
        #endregion
    }
}
