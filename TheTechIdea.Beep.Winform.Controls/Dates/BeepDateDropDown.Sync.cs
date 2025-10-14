using System;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// Handles bidirectional synchronization between BeepDateDropDown and BeepDateTimePicker
    /// Ensures data flows correctly in both directions using DateTimePickerProperties
    /// </summary>
    public partial class BeepDateDropDown
    {
        #region "Sync with BeepDateTimePicker"
        
        /// <summary>
        /// Sync BeepDateDropDown state TO BeepDateTimePicker (calendar popup)
        /// Called when showing the dropdown calendar
        /// </summary>
        private void SyncToCalendar()
        {
            if (_calendarView == null)
                return;
            
            try
            {
                // Sync mode and constraints based on ReturnType
                switch (_returnType)
                {
                    case Models.ReturnDateTimeType.Date:
                        _calendarView.ShowTime = false;
                        _calendarView.SelectedDate = _selectedDateTime?.Date;
                        _calendarView.SelectedTime = null;
                        _calendarView.RangeStartDate = null;
                        _calendarView.RangeEndDate = null;
                        break;
                        
                    case Models.ReturnDateTimeType.DateTime:
                        _calendarView.ShowTime = true;
                        if (_selectedDateTime.HasValue)
                        {
                            _calendarView.SelectedDate = _selectedDateTime.Value.Date;
                            _calendarView.SelectedTime = _selectedDateTime.Value.TimeOfDay;
                        }
                        else
                        {
                            _calendarView.SelectedDate = null;
                            _calendarView.SelectedTime = null;
                        }
                        _calendarView.RangeStartDate = null;
                        _calendarView.RangeEndDate = null;
                        break;
                        
                    case Models.ReturnDateTimeType.DateRange:
                        _calendarView.ShowTime = false;
                        _calendarView.RangeStartDate = _startDate?.Date;
                        _calendarView.RangeEndDate = _endDate?.Date;
                        _calendarView.SelectedDate = null;
                        _calendarView.SelectedTime = null;
                        break;
                        
                    case Models.ReturnDateTimeType.DateTimeRange:
                        _calendarView.ShowTime = true;
                        _calendarView.RangeStartDate = _startDate;
                        _calendarView.RangeEndDate = _endDate;
                        _calendarView.SelectedDate = null;
                        _calendarView.SelectedTime = null;
                        break;
                }
                
                // Sync date constraints
                if (_minDate.HasValue) _calendarView.MinDate = _minDate.Value;
                if (_maxDate.HasValue) _calendarView.MaxDate = _maxDate.Value;
                
                // Force calendar to refresh
                _calendarView.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SyncToCalendar error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Sync BeepDateTimePicker (calendar popup) state back TO BeepDateDropDown
        /// Called when user selects a date in the calendar
        /// NOTE: We use OUR ReturnType, not the calendar's, to avoid circular updates
        /// </summary>
        private void SyncFromCalendar()
        {
            if (_calendarView == null)
                return;
            
            try
            {
                // Use OUR ReturnType as source of truth (not the calendar's)
                // This prevents circular updates when calendar value changes
                switch (_returnType)
                {
                    case Models.ReturnDateTimeType.Date:
                        // Get date only (strip time)
                        if (_calendarView.SelectedDate.HasValue)
                        {
                            _selectedDateTime = _calendarView.SelectedDate.Value.Date;
                            UpdateTextFromDate();
                            OnSelectedDateTimeChanged(new Models.DateTimePickerEventArgs(
                                _selectedDateTime, 
                                null, 
                                null, 
                                Models.DatePickerMode.Single));
                        }
                        break;
                        
                    case Models.ReturnDateTimeType.DateTime:
                        // Combine date and time
                        if (_calendarView.SelectedDate.HasValue)
                        {
                            DateTime date = _calendarView.SelectedDate.Value.Date;
                            TimeSpan time = _calendarView.SelectedTime ?? TimeSpan.Zero;
                            _selectedDateTime = date.Add(time);
                            UpdateTextFromDate();
                            OnSelectedDateTimeChanged(new Models.DateTimePickerEventArgs(
                                _selectedDateTime, 
                                time, 
                                null, 
                                Models.DatePickerMode.SingleWithTime));
                        }
                        break;
                        
                    case Models.ReturnDateTimeType.DateRange:
                        // Get date range (dates only)
                        if (_calendarView.RangeStartDate.HasValue && _calendarView.RangeEndDate.HasValue)
                        {
                            _startDate = _calendarView.RangeStartDate.Value.Date;
                            _endDate = _calendarView.RangeEndDate.Value.Date;
                            
                            // Ensure end >= start
                            if (_endDate < _startDate)
                            {
                                var temp = _startDate;
                                _startDate = _endDate;
                                _endDate = temp;
                            }
                            
                            UpdateTextFromDateRange();
                            OnDateRangeChanged(new DateRangeEventArgs(_startDate, _endDate));
                        }
                        break;
                        
                    case Models.ReturnDateTimeType.DateTimeRange:
                        // Get date-time range
                        if (_calendarView.RangeStartDate.HasValue && _calendarView.RangeEndDate.HasValue)
                        {
                            _startDate = _calendarView.RangeStartDate.Value;
                            _endDate = _calendarView.RangeEndDate.Value;
                            
                            // Ensure end >= start
                            if (_endDate < _startDate)
                            {
                                var temp = _startDate;
                                _startDate = _endDate;
                                _endDate = temp;
                            }
                            
                            UpdateTextFromDateRange();
                            OnDateRangeChanged(new DateRangeEventArgs(_startDate, _endDate));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SyncFromCalendar error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Enhanced calendar date changed handler with proper sync
        /// NOTE: Does NOT change ReturnType - only reads calendar values
        /// </summary>
        private void CalendarView_DateChanged_Enhanced(object sender, DateTimePickerEventArgs e)
        {
            // Sync calendar state back to dropdown (using OUR ReturnType, not changing it)
            SyncFromCalendar();
            
            // Close popup if appropriate based on OUR ReturnType
            switch (_returnType)
            {
                case Models.ReturnDateTimeType.Date:
                case Models.ReturnDateTimeType.DateTime:
                    // Close immediately for single selection
                    ClosePopup();
                    break;
                    
                case Models.ReturnDateTimeType.DateRange:
                case Models.ReturnDateTimeType.DateTimeRange:
                    // For range mode, close only when both dates are selected
                    if (_startDate.HasValue && _endDate.HasValue)
                    {
                        ClosePopup();
                    }
                    break;
            }
        }
        
        #endregion
    }
}
