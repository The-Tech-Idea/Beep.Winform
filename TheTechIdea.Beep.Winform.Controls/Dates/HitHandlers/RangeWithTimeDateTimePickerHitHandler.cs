using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Range With Time Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - Dual calendar for date range selection (start/end dates)
    /// - Dual time pickers below calendars (start time + end time)
    /// - 4-step selection: start date → end date → start time → end time
    /// - Independent time selectors with hour/minute spinners
    /// 
    /// INTERACTION MODEL:
    /// 1. Select start date from calendar
    /// 2. Select end date from calendar (auto-normalize if reversed)
    /// 3. Adjust start time with hour/minute controls
    /// 4. Adjust end time with hour/minute controls
    /// 5. Close when all 4 values are set
    /// 
    /// LAYOUT STRUCTURE:
    /// - Header: Month/year display with navigation
    /// - Dual calendar: Side-by-side or sequential month grids
    /// - Time separator: Horizontal line dividing calendar from time
    /// - Start time picker: Left side, hour/minute spinners
    /// - End time picker: Right side, hour/minute spinners
    /// - Each time picker has: Label, display, up/down buttons
    /// 
    /// HIT AREAS:
    /// - Date cells: day_YYYY_MM_DD
    /// - Start time hour up/down: start_hour_up, start_hour_down
    /// - Start time minute up/down: start_minute_up, start_minute_down
    /// - End time hour up/down: end_hour_up, end_hour_down
    /// - End time minute up/down: end_minute_up, end_minute_down
    /// </summary>
    internal class RangeWithTimeDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _start;
        private DateTime? _end;
        private TimeSpan? _startTime;
        private TimeSpan? _endTime;
        private bool _selectingStart = true;
        
        public DatePickerMode Mode => DatePickerMode.RangeWithTime;


        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            
            // Navigation buttons
            if (!layout.PreviousButtonRect.IsEmpty && layout.PreviousButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.PreviousButton;
                result.HitBounds = layout.PreviousButtonRect;
                return result;
            }
            if (!layout.NextButtonRect.IsEmpty && layout.NextButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.NextButton;
                result.HitBounds = layout.NextButtonRect;
                return result;
            }
            
            // Time selector buttons (check these before calendar to prioritize time controls)
            // Start time hour buttons
            if (!layout.StartTimeHourUpRect.IsEmpty && layout.StartTimeHourUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartHourUpButton;
                result.HitBounds = layout.StartTimeHourUpRect;
                return result;
            }
            if (!layout.StartTimeHourDownRect.IsEmpty && layout.StartTimeHourDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartHourDownButton;
                result.HitBounds = layout.StartTimeHourDownRect;
                return result;
            }
            // Start time minute buttons
            if (!layout.StartTimeMinuteUpRect.IsEmpty && layout.StartTimeMinuteUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartMinuteUpButton;
                result.HitBounds = layout.StartTimeMinuteUpRect;
                return result;
            }
            if (!layout.StartTimeMinuteDownRect.IsEmpty && layout.StartTimeMinuteDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartMinuteDownButton;
                result.HitBounds = layout.StartTimeMinuteDownRect;
                return result;
            }
            // End time hour buttons
            if (!layout.EndTimeHourUpRect.IsEmpty && layout.EndTimeHourUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.EndHourUpButton;
                result.HitBounds = layout.EndTimeHourUpRect;
                return result;
            }
            if (!layout.EndTimeHourDownRect.IsEmpty && layout.EndTimeHourDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.EndHourDownButton;
                result.HitBounds = layout.EndTimeHourDownRect;
                return result;
            }
            // End time minute buttons
            if (!layout.EndTimeMinuteUpRect.IsEmpty && layout.EndTimeMinuteUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.EndMinuteUpButton;
                result.HitBounds = layout.EndTimeMinuteUpRect;
                return result;
            }
            if (!layout.EndTimeMinuteDownRect.IsEmpty && layout.EndTimeMinuteDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.EndMinuteDownButton;
                result.HitBounds = layout.EndTimeMinuteDownRect;
                return result;
            }
            
            // Calendar grid
            if (!layout.CalendarGridRect.IsEmpty && layout.CalendarGridRect.Contains(location))
            {
                var cells = layout.DayCellMatrix;
                if (cells == null && layout.DayCellRects != null && layout.DayCellRects.Count == 42)
                {
                    cells = new Rectangle[6, 7];
                    for (int i = 0; i < 42; i++)
                        cells[i / 7, i % 7] = layout.DayCellRects[i];
                    layout.DayCellMatrix = cells;
                }
                if (cells != null)
                {
                    for (int row = 0; row < 6; row++)
                    {
                        for (int col = 0; col < 7; col++)
                        {
                            var cellRect = cells[row, col];
                            if (!cellRect.IsEmpty && cellRect.Contains(location))
                            {
                                DateTime date = GetDateForCell(displayMonth, row, col, properties);
                                result.IsHit = true;
                                result.HitArea = DateTimePickerHitArea.DayCell;
                                result.Date = date;
                                result.HitBounds = cellRect;
                                return result;
                            }
                        }
                    }
                }
            }
            if (layout.TimeSpinnerRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.TimeSpinner;
                result.HitBounds = layout.TimeSpinnerRect;
                return result;
            }
            return result;
        }


        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit) return false;
            
            // Navigation
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
            
            // Start time hour adjustments
            if (hitResult.HitArea == DateTimePickerHitArea.StartHourUpButton)
            {
                _startTime = (_startTime ?? TimeSpan.Zero).Add(TimeSpan.FromHours(1));
                if (_startTime.Value.TotalHours >= 24) _startTime = TimeSpan.Zero;
                SyncToControl(owner);
                return false;
            }
            if (hitResult.HitArea == DateTimePickerHitArea.StartHourDownButton)
            {
                _startTime = (_startTime ?? TimeSpan.Zero).Add(TimeSpan.FromHours(-1));
                if (_startTime.Value.TotalHours < 0) _startTime = TimeSpan.FromHours(23);
                SyncToControl(owner);
                return false;
            }
            // Start time minute adjustments
            if (hitResult.HitArea == DateTimePickerHitArea.StartMinuteUpButton)
            {
                _startTime = (_startTime ?? TimeSpan.Zero).Add(TimeSpan.FromMinutes(1));
                if (_startTime.Value.TotalHours >= 24) _startTime = TimeSpan.Zero;
                SyncToControl(owner);
                return false;
            }
            if (hitResult.HitArea == DateTimePickerHitArea.StartMinuteDownButton)
            {
                _startTime = (_startTime ?? TimeSpan.Zero).Add(TimeSpan.FromMinutes(-1));
                if (_startTime.Value.TotalHours < 0) _startTime = TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59));
                SyncToControl(owner);
                return false;
            }
            
            // End time hour adjustments
            if (hitResult.HitArea == DateTimePickerHitArea.EndHourUpButton)
            {
                _endTime = (_endTime ?? TimeSpan.Zero).Add(TimeSpan.FromHours(1));
                if (_endTime.Value.TotalHours >= 24) _endTime = TimeSpan.Zero;
                SyncToControl(owner);
                return false;
            }
            if (hitResult.HitArea == DateTimePickerHitArea.EndHourDownButton)
            {
                _endTime = (_endTime ?? TimeSpan.Zero).Add(TimeSpan.FromHours(-1));
                if (_endTime.Value.TotalHours < 0) _endTime = TimeSpan.FromHours(23);
                SyncToControl(owner);
                return false;
            }
            // End time minute adjustments
            if (hitResult.HitArea == DateTimePickerHitArea.EndMinuteUpButton)
            {
                _endTime = (_endTime ?? TimeSpan.Zero).Add(TimeSpan.FromMinutes(1));
                if (_endTime.Value.TotalHours >= 24) _endTime = TimeSpan.Zero;
                SyncToControl(owner);
                return false;
            }
            if (hitResult.HitArea == DateTimePickerHitArea.EndMinuteDownButton)
            {
                _endTime = (_endTime ?? TimeSpan.Zero).Add(TimeSpan.FromMinutes(-1));
                if (_endTime.Value.TotalHours < 0) _endTime = TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59));
                SyncToControl(owner);
                return false;
            }
            
            // Date selection
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell && hitResult.Date.HasValue)
            {
                var clicked = hitResult.Date.Value.Date;
                
                // Validate against MinDate/MaxDate with logging
                if (!IsDateInBounds(clicked, owner))
                {
                    System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Date {clicked:yyyy-MM-dd} is out of bounds");
                    return false;
                }
                
                if (_selectingStart)
                {
                    // Step 1: Set start date
                    _start = clicked;
                    _startTime = _startTime ?? TimeSpan.FromHours(9);  // Default 9:00 AM
                    _end = null;
                    _endTime = null;
                    _selectingStart = false;
                    
                    System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Start date selected: {_start:yyyy-MM-dd} {_startTime}");
                    
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;  // Wait for end date
                }
                else
                {
                    // Step 2: Set end date
                    _end = clicked;
                    _endTime = _endTime ?? TimeSpan.FromHours(17);  // Default 5:00 PM
                    
                    // Auto-normalize if end < start (including times)
                    if (_start.HasValue && _end.Value < _start.Value)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Swapping dates: start={_start:yyyy-MM-dd}, end={_end:yyyy-MM-dd}");
                        
                        var tmp = _start;
                        _start = _end;
                        _end = tmp;
                        
                        var tmpTime = _startTime;
                        _startTime = _endTime;
                        _endTime = tmpTime;
                    }
                    
                    // Validate and log range span
                    if (_start.HasValue && _end.HasValue)
                    {
                        var days = (_end.Value - _start.Value).Days + 1;
                        if (days > 365)
                        {
                            System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Warning: Large range selected ({days} days)");
                        }
                        System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Range complete: {_start:yyyy-MM-dd} {_startTime} to {_end:yyyy-MM-dd} {_endTime} ({days} days)");
                    }
                    
                    _selectingStart = true;
                    SyncToControl(owner);
                    owner.Invalidate();
                    // Close if both dates and times are set
                    return IsSelectionComplete();
                }
            }
            
            return false;
        }


        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            
            // Day cell hover with range preview
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date;
                hoverState.HoverBounds = hitResult.HitBounds;
                
                // Show range preview when selecting end date
                if (!_selectingStart && _start.HasValue && hitResult.Date.HasValue)
                {
                    DateTime a = _start.Value.Date;
                    DateTime b = hitResult.Date.Value.Date;
                    if (b < a)
                    {
                        var t = a;
                        a = b;
                        b = t;
                    }
                    hoverState.HoveredRangePreview = (a, b);
                }
            }
            // Time spinner button hover
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
            else if (hitResult.HitArea == DateTimePickerHitArea.PreviousButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.PreviousButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.NextButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.NextButton;
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
            // Pre-sync validation
            if (_start.HasValue && _end.HasValue && !ValidateRangeSelection())
            {
                System.Diagnostics.Debug.WriteLine("[RangeWithTime] Sync aborted: Invalid range");
                return;
            }
            
            owner.RangeStartDate = _start;
            owner.RangeEndDate = _end;
            owner.RangeStartTime = _startTime;
            owner.RangeEndTime = _endTime;
            owner.SelectedDate = _start;
            owner.SelectedTime = _startTime;
            
            // Log sync operation
            if (_start.HasValue && _end.HasValue)
            {
                var days = (_end.Value - _start.Value).Days + 1;
                System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Synced to control: {_start:yyyy-MM-dd} {_startTime} to {_end:yyyy-MM-dd} {_endTime} ({days} days)");
            }
            else if (_start.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Synced partial: Start={_start:yyyy-MM-dd} {_startTime}, waiting for end date");
            }
        }

        public bool IsSelectionComplete()
        {
            return _start.HasValue && _end.HasValue && _startTime.HasValue && _endTime.HasValue;
        }

        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine("[RangeWithTime] Resetting range selection");
            
            _start = null;
            _end = null;
            _startTime = null;
            _endTime = null;
            _selectingStart = true;
            
           
        }

        private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties properties)
        {
            DateTime firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)properties.FirstDayOfWeek + 7) % 7;
            return firstDayOfMonth.AddDays(row * 7 + col - offset);
        }
        
        /// <summary>
        /// Validates that a date is within the control's allowed range (MinDate to MaxDate)
        /// </summary>
        private bool IsDateInBounds(DateTime date, BeepDateTimePicker owner)
        {
            if (owner == null) return true;
            
            if (date < owner.MinDate)
            {
                System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Date {date:yyyy-MM-dd} is before MinDate {owner.MinDate:yyyy-MM-dd}");
                return false;
            }
            
            if (date > owner.MaxDate)
            {
                System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Date {date:yyyy-MM-dd} is after MaxDate {owner.MaxDate:yyyy-MM-dd}");
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Validates the internal range selection logic (start <= end, reasonable span)
        /// </summary>
        private bool ValidateRangeSelection()
        {
            if (!_start.HasValue || !_end.HasValue)
            {
                System.Diagnostics.Debug.WriteLine("[RangeWithTime] Validation failed: Start or end date is null");
                return false;
            }
            
            if (_end.Value < _start.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Validation failed: End date {_end:yyyy-MM-dd} is before start date {_start:yyyy-MM-dd}");
                return false;
            }
            
            var days = (_end.Value - _start.Value).Days + 1;
            if (days > 3650) // 10 years
            {
                System.Diagnostics.Debug.WriteLine($"[RangeWithTime] Warning: Very large range ({days} days, ~{days/365} years)");
            }
            
            return true;
        }
    }
}
