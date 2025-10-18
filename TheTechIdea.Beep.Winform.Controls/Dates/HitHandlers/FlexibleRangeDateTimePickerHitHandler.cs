using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Flexible Range Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - Select a center date from calendar
    /// - Choose tolerance from preset buttons: Exact, ±1 day, ±2 days, ±3 days, ±7 days
    /// - Automatically calculates start/end range based on tolerance
    /// - Dual-step selection: first date, then tolerance
    /// 
    /// INTERACTION MODEL:
    /// 1. Click calendar date → Set center date
    /// 2. Click tolerance button → Calculate range (center ± days)
    /// 3. "Exact dates" button → Single date (start = end)
    /// 4. "± N days" button → Range (center - N to center + N)
    /// 5. Close on tolerance selection
    /// 
    /// LAYOUT STRUCTURE:
    /// - Header: Standard month/year display with navigation
    /// - Calendar: Standard month grid for center date selection
    /// - Tolerance buttons: Bottom row, 5 buttons
    ///   * "Exact dates" (0 days)
    ///   * "± 1 day" (center - 1 to center + 1)
    ///   * "± 2 days" (center - 2 to center + 2)
    ///   * "± 3 days" (center - 3 to center + 3)
    ///   * "± 7 days" (center - 7 to center + 7)
    /// 
    /// QUICK BUTTON KEYS:
    /// - exact_dates → 0 days tolerance
    /// - plus_minus_1_day → ±1 day
    /// - plus_minus_2_days → ±2 days
    /// - plus_minus_3_days → ±3 days
    /// - plus_minus_7_days → ±7 days
    /// </summary>
    internal class FlexibleRangeDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _centerDate;  // User-selected center date
        private DateTime? _start;       // Calculated start (center - tolerance)
        private DateTime? _end;         // Calculated end (center + tolerance)
        private TimeSpan? _startTime;
        private TimeSpan? _endTime;

        public DatePickerMode Mode => DatePickerMode.FlexibleRange;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
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
            if (layout.QuickDateButtons != null)
            {
                foreach (var btn in layout.QuickDateButtons)
                {
                    if (btn.Bounds.Contains(location))
                    {
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.FlexibleRangeButton;
                        result.FlexibleOption = btn.Key;
                        result.HitBounds = btn.Bounds;
                        return result;
                    }
                }
            }
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
            return result;
        }


        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit) return false;
            
            // Navigation buttons
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
            
            // Tolerance preset buttons
            if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && !string.IsNullOrEmpty(hitResult.KeyValue))
            {
                if (!_centerDate.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("[FlexibleRange] Tolerance button clicked without center date");
                    return false;  // Need center date first
                }
                
                var key = (hitResult.KeyValue ?? string.Empty).ToLower().Replace("_", "");
                int toleranceDays = 0;
                
                // Parse tolerance from button key
                if (key == "exactdates") toleranceDays = 0;
                else if (key.Contains("1day")) toleranceDays = 1;
                else if (key.Contains("2days")) toleranceDays = 2;
                else if (key.Contains("3days")) toleranceDays = 3;
                else if (key.Contains("7days")) toleranceDays = 7;
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Unknown tolerance key: {key}");
                    return false;
                }
                
                // Calculate range from center date ± tolerance
                DateTime centerDate = _centerDate.Value.Date;
                _start = centerDate.AddDays(-toleranceDays);
                _end = centerDate.AddDays(toleranceDays);
                
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Tolerance applied: {toleranceDays} days around {centerDate:yyyy-MM-dd}");
                
                // Validate against MinDate/MaxDate
                if (_start.HasValue && !IsDateInBounds(_start.Value, owner))
                {
                    var minDate = owner.MinDate.Date;
                    System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Start date {_start:yyyy-MM-dd} adjusted to MinDate {minDate:yyyy-MM-dd}");
                    _start = minDate;
                }
                if (_end.HasValue && !IsDateInBounds(_end.Value, owner))
                {
                    var maxDate = owner.MaxDate.Date;
                    System.Diagnostics.Debug.WriteLine($"[FlexibleRange] End date {_end:yyyy-MM-dd} adjusted to MaxDate {maxDate:yyyy-MM-dd}");
                    _end = maxDate;
                }
                
                var days = (_end.Value - _start.Value).Days + 1;
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Range complete: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
                
                SyncToControl(owner);
                owner.Invalidate();
                return true;  // Close on tolerance selection
            }
            
            // Calendar date selection (sets center date)
            if (hitResult.Date.HasValue)
            {
                var clicked = hitResult.Date.Value.Date;
                
                // Validate against MinDate/MaxDate with logging
                if (!IsDateInBounds(clicked, owner))
                {
                    System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Center date {clicked:yyyy-MM-dd} is out of bounds");
                    return false;
                }
                
                _centerDate = clicked;
                _start = null;
                _end = null;
                
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Center date selected: {_centerDate:yyyy-MM-dd}, awaiting tolerance selection");
                
                // Don't sync yet - wait for tolerance selection
                // But update display to show selected center date
                owner.Invalidate();
                return false;  // Keep open for tolerance selection
            }
            
            return false;
        }


        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            
            // Day cell hover
            if (hitResult.Date.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
                
                // Show range preview based on center date (if set) and hover
                if (_centerDate.HasValue)
                {
                    // Could show potential range if tolerance was applied
                    // For now, just highlight the date
                }
            }
            // Tolerance button hover
            else if (hitResult.HitArea == DateTimePickerHitArea.QuickButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.QuickButton;
                hoverState.HoveredButton = hitResult.KeyValue ?? hitResult.CustomData?.ToString();
                hoverState.HoverBounds = hitResult.HitBounds;
            }
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
        }

        public void SyncFromControl(BeepDateTimePicker owner)
        {
            _start = owner.RangeStartDate;
            _end = owner.RangeEndDate;
            _startTime = owner.RangeStartTime;
            _endTime = owner.RangeEndTime;
            
            // Try to infer center date from existing range
            if (_start.HasValue && _end.HasValue)
            {
                int totalDays = (_end.Value - _start.Value).Days;
                _centerDate = _start.Value.AddDays(totalDays / 2.0);
            }
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Pre-sync validation
            if (_start.HasValue && _end.HasValue && !ValidateRangeSelection())
            {
                System.Diagnostics.Debug.WriteLine("[FlexibleRange] Sync aborted: Invalid range");
                return;
            }
            
            owner.RangeStartDate = _start;
            owner.RangeEndDate = _end;
            owner.RangeStartTime = _startTime;
            owner.RangeEndTime = _endTime;
            owner.SelectedDate = _centerDate ?? _start;
            owner.SelectedTime = null;
            
            // Log sync operation
            if (_start.HasValue && _end.HasValue)
            {
                var days = (_end.Value - _start.Value).Days + 1;
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Synced to control: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
            }
            else if (_centerDate.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Synced partial: Center={_centerDate:yyyy-MM-dd}, awaiting tolerance");
            }
        }

        public bool IsSelectionComplete()
        {
            return _start.HasValue && _end.HasValue;
        }

        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine("[FlexibleRange] Resetting range selection");
            
            _centerDate = null;
            _start = null;
            _end = null;
            _startTime = null;
            _endTime = null;
            
            // Note: Owner must be passed to clear properties via external call
            // This method only resets internal state
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
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Date {date:yyyy-MM-dd} is before MinDate {owner.MinDate:yyyy-MM-dd}");
                return false;
            }
            
            if (date > owner.MaxDate)
            {
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Date {date:yyyy-MM-dd} is after MaxDate {owner.MaxDate:yyyy-MM-dd}");
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
                System.Diagnostics.Debug.WriteLine("[FlexibleRange] Validation failed: Start or end date is null");
                return false;
            }
            
            if (_end.Value < _start.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Validation failed: End date {_end:yyyy-MM-dd} is before start date {_start:yyyy-MM-dd}");
                return false;
            }
            
            var days = (_end.Value - _start.Value).Days + 1;
            if (days > 3650) // 10 years
            {
                System.Diagnostics.Debug.WriteLine($"[FlexibleRange] Warning: Very large range ({days} days, ~{days/365} years)");
            }
            
            return true;
        }
    }
}
