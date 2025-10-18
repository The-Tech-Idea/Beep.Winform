using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    internal class FilteredRangeDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _start;
        private DateTime? _end;
        private TimeSpan? _startTime;
        private TimeSpan? _endTime;
        private bool _selectingStart = true;
        public DatePickerMode Mode => DatePickerMode.FilteredRange;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            
            // ========== SECTION 1: SIDEBAR FILTER BUTTONS ==========
            if (layout.FilterButtonRects != null)
            {
                // Keys must match the HandleClick switch statement
                string[] filterKeys = { "pastweek", "pastmonth", "past3months", "past6months", "pastyear", "pastcentury" };
                for (int i = 0; i < layout.FilterButtonRects.Count && i < filterKeys.Length; i++)
                {
                    if (layout.FilterButtonRects[i].Contains(location))
                    {
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.FilterButton;
                        result.HitBounds = layout.FilterButtonRects[i];
                        result.CellIndex = i;
                        result.KeyValue = filterKeys[i];
                        result.CustomData = filterKeys[i];  // Keep for backwards compatibility
                        return result;
                    }
                }
            }
            
            // ========== SECTION 2: LEFT CALENDAR DAY CELLS ==========
            if (layout.LeftDayCellRects != null)
            {
                DateTime leftMonth = displayMonth;
                DateTime firstDayOfMonth = new DateTime(leftMonth.Year, leftMonth.Month, 1);
                int offset = ((int)firstDayOfMonth.DayOfWeek - (int)properties.FirstDayOfWeek + 7) % 7;
                int daysInMonth = DateTime.DaysInMonth(leftMonth.Year, leftMonth.Month);
                
                for (int i = 0; i < layout.LeftDayCellRects.Count; i++)
                {
                    var cellRect = layout.LeftDayCellRects[i];
                    if (!cellRect.IsEmpty && cellRect.Contains(location))
                    {
                        int dayNum = i - offset + 1;
                        if (dayNum >= 1 && dayNum <= daysInMonth)
                        {
                            DateTime date = new DateTime(leftMonth.Year, leftMonth.Month, dayNum);
                            result.IsHit = true;
                            result.HitArea = DateTimePickerHitArea.DayCell;
                            result.Date = date;
                            result.HitBounds = cellRect;
                            result.GridIndex = 0;  // Left calendar
                            result.CellIndex = i;
                            return result;
                        }
                    }
                }
            }
            
            // ========== SECTION 3: RIGHT CALENDAR DAY CELLS ==========
            if (layout.RightDayCellRects != null)
            {
                DateTime rightMonth = displayMonth.AddMonths(1);
                DateTime firstDayOfMonth = new DateTime(rightMonth.Year, rightMonth.Month, 1);
                int offset = ((int)firstDayOfMonth.DayOfWeek - (int)properties.FirstDayOfWeek + 7) % 7;
                int daysInMonth = DateTime.DaysInMonth(rightMonth.Year, rightMonth.Month);
                
                for (int i = 0; i < layout.RightDayCellRects.Count; i++)
                {
                    var cellRect = layout.RightDayCellRects[i];
                    if (!cellRect.IsEmpty && cellRect.Contains(location))
                    {
                        int dayNum = i - offset + 1;
                        if (dayNum >= 1 && dayNum <= daysInMonth)
                        {
                            DateTime date = new DateTime(rightMonth.Year, rightMonth.Month, dayNum);
                            result.IsHit = true;
                            result.HitArea = DateTimePickerHitArea.DayCell;
                            result.Date = date;
                            result.HitBounds = cellRect;
                            result.GridIndex = 1;  // Right calendar
                            result.CellIndex = i;
                            return result;
                        }
                    }
                }
            }
            
            // ========== SECTION 4: TIME SPINNER BUTTONS ==========
            // Start (From) Hour Spinner
            if (layout.TimeHourUpRect != Rectangle.Empty && layout.TimeHourUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartHourUpButton;
                result.HitBounds = layout.TimeHourUpRect;
                return result;
            }
            
            if (layout.TimeHourDownRect != Rectangle.Empty && layout.TimeHourDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartHourDownButton;
                result.HitBounds = layout.TimeHourDownRect;
                return result;
            }
            
            // Start (From) Minute Spinner
            if (layout.TimeMinuteUpRect != Rectangle.Empty && layout.TimeMinuteUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartMinuteUpButton;
                result.HitBounds = layout.TimeMinuteUpRect;
                return result;
            }
            
            if (layout.TimeMinuteDownRect != Rectangle.Empty && layout.TimeMinuteDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartMinuteDownButton;
                result.HitBounds = layout.TimeMinuteDownRect;
                return result;
            }
            
            // End (To) Hour Spinner
            if (layout.EndTimeHourUpRect != Rectangle.Empty && layout.EndTimeHourUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.EndHourUpButton;
                result.HitBounds = layout.EndTimeHourUpRect;
                return result;
            }
            
            if (layout.EndTimeHourDownRect != Rectangle.Empty && layout.EndTimeHourDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.EndHourDownButton;
                result.HitBounds = layout.EndTimeHourDownRect;
                return result;
            }
            
            // End (To) Minute Spinner
            if (layout.EndTimeMinuteUpRect != Rectangle.Empty && layout.EndTimeMinuteUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.EndMinuteUpButton;
                result.HitBounds = layout.EndTimeMinuteUpRect;
                return result;
            }
            
            if (layout.EndTimeMinuteDownRect != Rectangle.Empty && layout.EndTimeMinuteDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.EndMinuteDownButton;
                result.HitBounds = layout.EndTimeMinuteDownRect;
                return result;
            }
            
            // ========== SECTION 5: ACTION BUTTONS ==========
            if (layout.ResetButtonRect != Rectangle.Empty && layout.ResetButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.ResetButton;
                result.HitBounds = layout.ResetButtonRect;
                return result;
            }
            
            if (layout.ShowResultsButtonRect != Rectangle.Empty && layout.ShowResultsButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.ShowResultsButton;
                result.HitBounds = layout.ShowResultsButtonRect;
                return result;
            }
            
            // ========== SECTION 6: YEAR DROPDOWNS ==========
            if (layout.LeftYearDropdownRect != Rectangle.Empty && layout.LeftYearDropdownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.YearDropdown;
                result.HitBounds = layout.LeftYearDropdownRect;
                result.GridIndex = 0;  // Left calendar
                return result;
            }
            
            if (layout.RightYearDropdownRect != Rectangle.Empty && layout.RightYearDropdownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.YearDropdown;
                result.HitBounds = layout.RightYearDropdownRect;
                result.GridIndex = 1;  // Right calendar
                return result;
            }
            
            // Fallback: Check legacy layout for backward compatibility
            if (layout.QuickDateButtons != null)
            {
                foreach (var btn in layout.QuickDateButtons)
                {
                    if (btn.Bounds.Contains(location))
                    {
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.QuickButton;
                        result.HitBounds = btn.Bounds;
                        result.KeyValue = btn.Key;
                        result.CustomData = btn.Key;  // Keep for backwards compatibility
                        return result;
                    }
                }
            }
            
            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit) return false;
            
            // Navigation buttons (legacy support)
            if (hitResult.HitArea == DateTimePickerHitArea.PreviousButton)
            {
                owner.NavigateToPreviousMonth();
                return false;
            }
            if (hitResult.HitArea == DateTimePickerHitArea.NextButton)
            {
                owner.NavigateToNextMonth();
                return false;
            }
            
            // Filter button clicks (sidebar quick filters)
            if (hitResult.HitArea == DateTimePickerHitArea.FilterButton && !string.IsNullOrEmpty(hitResult.KeyValue))
            {
                var key = hitResult.QuarterKey ?? hitResult.KeyValue ?? string.Empty;
                key = key.ToLower().Replace(" ", "");
                DateTime start = DateTime.Today, end = DateTime.Today;
                
                switch (key)
                {
                    case "today": 
                        start = DateTime.Today; 
                        end = DateTime.Today; 
                        break;
                    case "yesterday": 
                        start = DateTime.Today.AddDays(-1); 
                        end = DateTime.Today.AddDays(-1); 
                        break;
                    case "last7days":
                    case "pastweek":
                        start = DateTime.Today.AddDays(-7); 
                        end = DateTime.Today; 
                        break;
                    case "last30days":
                    case "lastmonth":
                    case "pastmonth":
                        start = DateTime.Today.AddDays(-30); 
                        end = DateTime.Today; 
                        break;
                    case "past3months":
                        start = DateTime.Today.AddMonths(-3); 
                        end = DateTime.Today; 
                        break;
                    case "past6months":
                        start = DateTime.Today.AddMonths(-6); 
                        end = DateTime.Today; 
                        break;
                    case "pastyear":
                        start = DateTime.Today.AddYears(-1); 
                        end = DateTime.Today; 
                        break;
                    case "pastcentury":
                        start = DateTime.Today.AddYears(-100); 
                        end = DateTime.Today; 
                        break;
                    case "thismonth": 
                        start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); 
                        end = start.AddMonths(1).AddDays(-1); 
                        break;
                    default: 
                        return false;
                }
                
                _start = start;
                _end = end;
                _startTime = new TimeSpan(0, 0, 0);
                _endTime = new TimeSpan(23, 59, 59);
                _selectingStart = true;
                SyncToControl(owner);
                return false;  // Don't close, let user see the selection
            }
            
            // Day cell clicks (range selection)
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell && hitResult.Date.HasValue)
            {
                var clicked = hitResult.Date.Value.Date;
                
                // Validate against MinDate/MaxDate bounds
                if (!IsDateInBounds(clicked, owner))
                {
                    System.Diagnostics.Debug.WriteLine($"[FilteredRange] Date {clicked:yyyy-MM-dd} is out of bounds");
                    return false;
                }
                
                // Check if clicking on already selected start date - deselect it
                if (_start.HasValue && _start.Value.Date == clicked && !_end.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine($"[FilteredRange] Deselecting start date: {_start:yyyy-MM-dd}");
                    _start = null;
                    _startTime = null;
                    _selectingStart = true;
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;
                }
                
                // Check if clicking on already selected end date - deselect it
                if (_end.HasValue && _end.Value.Date == clicked)
                {
                    System.Diagnostics.Debug.WriteLine($"[FilteredRange] Deselecting end date: {_end:yyyy-MM-dd}");
                    _end = null;
                    _endTime = null;
                    _selectingStart = false;  // Stay in end selection mode
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;
                }
                
                // Check if clicking on already selected start date (when end is also selected) - restart selection
                if (_start.HasValue && _start.Value.Date == clicked && _end.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine($"[FilteredRange] Restarting selection from start date: {clicked:yyyy-MM-dd}");
                    _start = clicked;
                    _startTime = new TimeSpan(0, 0, 0);
                    _end = null;
                    _endTime = null;
                    _selectingStart = false;
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;
                }
                
                if (_selectingStart)
                {
                    // First click: Set start date
                    _start = clicked;
                    _startTime = new TimeSpan(0, 0, 0);  // Default to start of day
                    _end = null;
                    _endTime = null;
                    _selectingStart = false;
                    
                    System.Diagnostics.Debug.WriteLine($"[FilteredRange] Start date selected: {_start:yyyy-MM-dd}");
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;  // Don't close, need end date
                }
                else
                {
                    // Second click: Set end date
                    _end = clicked;
                    _endTime = new TimeSpan(23, 59, 59);  // Default to end of day
                    
                    // Validate and normalize: ensure start <= end
                    if (_start.HasValue && _end.Value < _start.Value)
                    {
                        System.Diagnostics.Debug.WriteLine($"[FilteredRange] Swapping dates: start={_start:yyyy-MM-dd}, end={_end:yyyy-MM-dd}");
                        var tmpDate = _start;
                        var tmpTime = _startTime;
                        _start = _end;
                        _startTime = _endTime;
                        _end = tmpDate;
                        _endTime = tmpTime;
                    }
                    
                    // Validate range span
                    if (_start.HasValue && _end.HasValue)
                    {
                        var days = (_end.Value - _start.Value).Days;
                        if (days > 365)
                        {
                            System.Diagnostics.Debug.WriteLine($"[FilteredRange] Warning: Large range selected ({days} days)");
                        }
                        System.Diagnostics.Debug.WriteLine($"[FilteredRange] Range complete: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
                    }
                    
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;  // Don't close, need time selection
                }
            }
            
            // Time spinner button clicks
            if (hitResult.HitArea == DateTimePickerHitArea.StartHourUpButton)
            {
                if (!_startTime.HasValue)
                    _startTime = TimeSpan.Zero;
                
                int newHour = (_startTime.Value.Hours + 1) % 24;
                _startTime = new TimeSpan(newHour, _startTime.Value.Minutes, 0);
                SyncToControl(owner);
                owner.Invalidate();
                return false;
            }
            
            if (hitResult.HitArea == DateTimePickerHitArea.StartHourDownButton)
            {
                if (!_startTime.HasValue)
                    _startTime = TimeSpan.Zero;
                
                int newHour = (_startTime.Value.Hours - 1 + 24) % 24;
                _startTime = new TimeSpan(newHour, _startTime.Value.Minutes, 0);
                SyncToControl(owner);
                owner.Invalidate();
                return false;
            }
            
            if (hitResult.HitArea == DateTimePickerHitArea.StartMinuteUpButton)
            {
                if (!_startTime.HasValue)
                    _startTime = TimeSpan.Zero;
                
                var props = owner.GetCurrentProperties();
                int interval = props?.TimeInterval.Minutes ?? 1;
                int newMinute = (_startTime.Value.Minutes + interval) % 60;
                _startTime = new TimeSpan(_startTime.Value.Hours, newMinute, 0);
                SyncToControl(owner);
                owner.Invalidate();
                return false;
            }
            
            if (hitResult.HitArea == DateTimePickerHitArea.StartMinuteDownButton)
            {
                if (!_startTime.HasValue)
                    _startTime = TimeSpan.Zero;
                
                var props = owner.GetCurrentProperties();
                int interval = props?.TimeInterval.Minutes ?? 1;
                int newMinute = (_startTime.Value.Minutes - interval + 60) % 60;
                _startTime = new TimeSpan(_startTime.Value.Hours, newMinute, 0);
                SyncToControl(owner);
                owner.Invalidate();
                return false;
            }
            
            if (hitResult.HitArea == DateTimePickerHitArea.EndHourUpButton)
            {
                if (!_endTime.HasValue)
                    _endTime = new TimeSpan(23, 59, 59);
                
                int newHour = (_endTime.Value.Hours + 1) % 24;
                _endTime = new TimeSpan(newHour, _endTime.Value.Minutes, 0);
                SyncToControl(owner);
                owner.Invalidate();
                return false;
            }
            
            if (hitResult.HitArea == DateTimePickerHitArea.EndHourDownButton)
            {
                if (!_endTime.HasValue)
                    _endTime = new TimeSpan(23, 59, 59);
                
                int newHour = (_endTime.Value.Hours - 1 + 24) % 24;
                _endTime = new TimeSpan(newHour, _endTime.Value.Minutes, 0);
                SyncToControl(owner);
                owner.Invalidate();
                return false;
            }
            
            if (hitResult.HitArea == DateTimePickerHitArea.EndMinuteUpButton)
            {
                if (!_endTime.HasValue)
                    _endTime = new TimeSpan(23, 59, 59);
                
                var props = owner.GetCurrentProperties();
                int interval = props?.TimeInterval.Minutes ?? 1;
                int newMinute = (_endTime.Value.Minutes + interval) % 60;
                _endTime = new TimeSpan(_endTime.Value.Hours, newMinute, 0);
                SyncToControl(owner);
                owner.Invalidate();
                return false;
            }
            
            if (hitResult.HitArea == DateTimePickerHitArea.EndMinuteDownButton)
            {
                if (!_endTime.HasValue)
                    _endTime = new TimeSpan(23, 59, 59);
                
                var props = owner.GetCurrentProperties();
                int interval = props?.TimeInterval.Minutes ?? 1;
                int newMinute = (_endTime.Value.Minutes - interval + 60) % 60;
                _endTime = new TimeSpan(_endTime.Value.Hours, newMinute, 0);
                SyncToControl(owner);
                owner.Invalidate();
                return false;
            }
            
            // Reset button
            if (hitResult.HitArea == DateTimePickerHitArea.ResetButton)
            {
                System.Diagnostics.Debug.WriteLine("[FilteredRange] Reset button clicked");
                Reset();
                // Clear owner properties explicitly
                owner.RangeStartDate = null;
                owner.RangeEndDate = null;
                owner.RangeStartTime = null;
                owner.RangeEndTime = null;
                owner.SelectedDate = null;
                owner.SelectedTime = null;
                owner.Invalidate();  // Force redraw to clear display
                return false;
            }
            
            // Show Results button - close the picker
            if (hitResult.HitArea == DateTimePickerHitArea.ShowResultsButton)
            {
                if (IsSelectionComplete())
                {
                    SyncToControl(owner);
                    return true;  // Close picker
                }
                return false;
            }
            
            // Year dropdown clicks
            if (hitResult.HitArea == DateTimePickerHitArea.YearDropdown)
            {
                ShowYearComboBox(owner, hitResult);
                return false;
            }
            
            // Legacy support for old string-based hit areas
            if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && !string.IsNullOrEmpty(hitResult.KeyValue))
            {
                // Handle legacy QuickButton hits
                var key = (hitResult.KeyValue ?? string.Empty).ToLower().Replace("filter_", "");
                // Reuse filter button logic
                var legacyResult = new DateTimePickerHitTestResult
                {
                    IsHit = true,
                    HitArea = DateTimePickerHitArea.FilterButton,
                    KeyValue = key,
                    CustomData = key  // Keep for backwards compatibility
                };
                return HandleClick(legacyResult, owner);
            }
            
            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            
            // Day cell hover with range preview
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell && hitResult.Date.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
                
                // Show range preview when selecting end date
                if (!_selectingStart && _start.HasValue)
                {
                    DateTime a = _start.Value.Date;
                    DateTime b = hitResult.Date.Value.Date;
                    
                    // Ensure a <= b
                    if (b < a)
                    {
                        var t = a;
                        a = b;
                        b = t;
                    }
                    
                    hoverState.HoveredRangePreview = (a, b);
                }
            }
            // Filter button hover
            else if (hitResult.HitArea == DateTimePickerHitArea.FilterButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.FilterButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Time spinner button hovers
            else if (hitResult.HitArea == DateTimePickerHitArea.StartHourUpButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartHourDownButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartMinuteUpButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartMinuteDownButton ||
                     hitResult.HitArea == DateTimePickerHitArea.EndHourUpButton ||
                     hitResult.HitArea == DateTimePickerHitArea.EndHourDownButton ||
                     hitResult.HitArea == DateTimePickerHitArea.EndMinuteUpButton ||
                     hitResult.HitArea == DateTimePickerHitArea.EndMinuteDownButton)
            {
                hoverState.HoverArea = hitResult.HitArea;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Reset button hover
            else if (hitResult.HitArea == DateTimePickerHitArea.ResetButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.ResetButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Show Results button hover
            else if (hitResult.HitArea == DateTimePickerHitArea.ShowResultsButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.ShowResultsButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Year dropdown hover
            else if (hitResult.HitArea == DateTimePickerHitArea.YearDropdown)
            {
                hoverState.HoverArea = DateTimePickerHitArea.YearDropdown;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Navigation button hover
            else if (hitResult.HitArea == DateTimePickerHitArea.PreviousButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.PreviousButton;
                hoverState.HoveredButton = "nav_previous";
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.NextButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.NextButton;
                hoverState.HoveredButton = "nav_next";
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Legacy QuickButton support
            else if (hitResult.HitArea == DateTimePickerHitArea.QuickButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.QuickButton;
                hoverState.HoveredButton = hitResult.KeyValue ?? hitResult.CustomData?.ToString();
                hoverState.HoverBounds = hitResult.HitBounds;
            }
        }

        public void SyncFromControl(BeepDateTimePicker owner)
        {
            _start = owner.RangeStartDate;
            _end = owner.RangeEndDate;
            _startTime = owner.RangeStartTime;
            _endTime = owner.RangeEndTime;
            _selectingStart = !(_start.HasValue && !_end.HasValue);
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Validate before syncing
            if (_start.HasValue && _end.HasValue && !ValidateRangeSelection())
            {
                System.Diagnostics.Debug.WriteLine("[FilteredRange] Sync aborted: Invalid range");
                return;
            }
            
            owner.RangeStartDate = _start;
            owner.RangeEndDate = _end;
            owner.RangeStartTime = _startTime;
            owner.RangeEndTime = _endTime;
            owner.SelectedDate = null;
            owner.SelectedTime = null;
            
            if (_start.HasValue && _end.HasValue)
            {
                var days = (_end.Value - _start.Value).Days;
                System.Diagnostics.Debug.WriteLine($"[FilteredRange] Synced to control: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
            }
            else if (_start.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"[FilteredRange] Synced partial: Start={_start:yyyy-MM-dd}, waiting for end date");
            }
        }

        public bool IsSelectionComplete()
        {
            return _start.HasValue && _end.HasValue && _startTime.HasValue && _endTime.HasValue;
        }

        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine("[FilteredRange] Resetting range selection");
            
            _start = null;
            _end = null;
            _startTime = null;
            _endTime = null;
            _selectingStart = true;
            
            // Note: Owner must be passed to clear properties via external call
            // This method only resets internal state
        }

        /// <summary>
        /// Show year combo box dropdown for year selection
        /// Uses BeepComboBox with ShowDropdown() to display year options
        /// </summary>
        private void ShowYearComboBox(BeepDateTimePicker owner, DateTimePickerHitTestResult hitResult)
        {
            if (owner == null) return;

            int currentYear = owner.DisplayMonth.Year;
            int minYear = owner.MinDate.Year;
            int maxYear = owner.MaxDate.Year;

            // Create BeepComboBox using DateTimeComboBoxHelper
            var comboBox = DateTimeComboBoxHelper.CreateYearComboBox(minYear, maxYear, currentYear);
            
            // Set up selection change handler
            comboBox.SelectedIndexChanged += (s, e) =>
            {
                int? selectedYear = DateTimeComboBoxHelper.GetSelectedYear(comboBox);
                if (!selectedYear.HasValue) return;
                
                // Calculate month difference to navigate to selected year
                int monthDiff = ((selectedYear.Value - owner.DisplayMonth.Year) * 12);
                
                // Navigate to adjust DisplayMonth to the selected year
                if (monthDiff > 0)
                {
                    for (int i = 0; i < monthDiff; i++)
                        owner.NavigateToNextMonth();
                }
                else if (monthDiff < 0)
                {
                    for (int i = 0; i < Math.Abs(monthDiff); i++)
                        owner.NavigateToPreviousMonth();
                }
                
                owner.Invalidate();
                comboBox.CloseDropdown();
            };

            // Position the combo box at the year combo box rect location
            var screenPoint = owner.PointToScreen(new Point(
                hitResult.HitBounds.X, 
                hitResult.HitBounds.Y
            ));
            
            comboBox.Size = hitResult.HitBounds.Size;
            comboBox.Location = owner.PointToClient(screenPoint);
            
            // Add to owner's controls temporarily
            owner.Controls.Add(comboBox);
            comboBox.BringToFront();
            
            // Show the dropdown
            comboBox.ShowDropdown();
            
            // Remove when closed
            comboBox.PopupClosed += (s, e) =>
            {
                owner.Controls.Remove(comboBox);
                comboBox.Dispose();
            };
        }
        
        /// <summary>
        /// Validates if a date is within the allowed bounds (MinDate/MaxDate)
        /// </summary>
        private bool IsDateInBounds(DateTime date, BeepDateTimePicker owner)
        {
            if (owner == null) return true;
            
            if (date < owner.MinDate)
            {
                System.Diagnostics.Debug.WriteLine($"[FilteredRange] Date {date:yyyy-MM-dd} is before MinDate {owner.MinDate:yyyy-MM-dd}");
                return false;
            }
            
            if (date > owner.MaxDate)
            {
                System.Diagnostics.Debug.WriteLine($"[FilteredRange] Date {date:yyyy-MM-dd} is after MaxDate {owner.MaxDate:yyyy-MM-dd}");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Validates if the current range selection is valid
        /// </summary>
        private bool ValidateRangeSelection()
        {
            if (!_start.HasValue || !_end.HasValue)
            {
                System.Diagnostics.Debug.WriteLine("[FilteredRange] Validation failed: Start or end date is null");
                return false;
            }
            
            if (_end.Value < _start.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[FilteredRange] Validation failed: End date {_end:yyyy-MM-dd} is before start date {_start:yyyy-MM-dd}");
                return false;
            }
            
            var days = (_end.Value - _start.Value).Days;
            if (days > 3650) // 10 years
            {
                System.Diagnostics.Debug.WriteLine($"[FilteredRange] Validation warning: Very large range ({days} days, ~{days/365} years)");
            }
            
            return true;
        }
    }
}
