using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Quarterly Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - 2x2 grid of quarter buttons (Q1, Q2, Q3, Q4)
    /// - Year selector with navigation buttons (FY YYYY display)
    /// - Each quarter button shows: "Q1", "Jan - Mar", date range
    /// - Optional fallback calendar below for custom date selection
    /// - Selected range display at bottom showing quarter dates
    /// 
    /// INTERACTION MODEL:
    /// 1. Click Q1-Q4 button → select full quarter range, close
    /// 2. Fallback calendar → two-step custom range selection
    /// 3. Year navigation → change year, stay in view
    /// 
    /// LAYOUT STRUCTURE:
    /// - Header: Y+20, Height=50, "FY YYYY" + nav buttons
    /// - Quarter grid: Y+90, 2x2 grid, gap=16px
    /// - Card size: (Width-gap)/2 x (Height-gap)/2
    /// - Range display: Bottom, Height=80
    /// - Optional calendar: Below quarter grid if enabled
    /// 
    /// QUARTER DEFINITIONS:
    /// - Q1: Jan 1 - Mar 31
    /// - Q2: Apr 1 - Jun 30
    /// - Q3: Jul 1 - Sep 30
    /// - Q4: Oct 1 - Dec 31
    /// </summary>
    internal class QuarterlyDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _start;  // Quarter start date
        private DateTime? _end;    // Quarter end date
        private bool _selectingStart = true;  // For fallback calendar
        public DatePickerMode Mode => DatePickerMode.Quarterly;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            
            // Year navigation buttons
            if (!layout.PreviousButtonRect.IsEmpty && layout.PreviousButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.PreviousYearButton;
                result.HitBounds = layout.PreviousButtonRect;
                return result;
            }
            if (!layout.NextButtonRect.IsEmpty && layout.NextButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.NextYearButton;
                result.HitBounds = layout.NextButtonRect;
                return result;
            }
            
            // Quarter buttons (Q1-Q4) - using QuickDateButtons layout
            if (layout.QuickDateButtons != null)
            {
                foreach (var btn in layout.QuickDateButtons)
                {
                    if (btn.Bounds.Contains(location))
                    {
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.QuarterButton;
                        result.KeyValue = btn.Key.ToLower();  // q1, q2, q3, q4
                        result.CustomData = btn.Key.ToLower();  // Keep for backwards compatibility
                        result.HitBounds = btn.Bounds;
                        return result;
                    }
                }
            }
            
            // Fallback calendar grid (optional)
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
                            if (cellRect.Contains(location))
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
            
            // Year navigation
            if (hitResult.HitArea == DateTimePickerHitArea.PreviousYearButton)
            {
                owner.NavigateToPreviousYear();
                return false;
            }
            if (hitResult.HitArea == DateTimePickerHitArea.NextYearButton)
            {
                owner.NavigateToNextYear();
                return false;
            }
            
            // Quarter button selection (Q1-Q4)
            if (hitResult.HitArea == DateTimePickerHitArea.QuarterButton && !string.IsNullOrEmpty(hitResult.KeyValue))
            {
                var quarterKey = (hitResult.KeyValue ?? string.Empty).ToLower();  // "q1", "q2", etc.
                int quarter = 0;
                
                if (quarterKey == "q1") quarter = 1;
                else if (quarterKey == "q2") quarter = 2;
                else if (quarterKey == "q3") quarter = 3;
                else if (quarterKey == "q4") quarter = 4;
                
                if (quarter > 0)
                {
                    // Convert quarter to date range
                    int year = owner.DisplayMonth.Year;
                    var quarterRange = GetQuarterDateRange(quarter, year);
                    
                    System.Diagnostics.Debug.WriteLine($"[Quarterly] Quarter {quarter} selected: {quarterRange.Start:yyyy-MM-dd} to {quarterRange.End:yyyy-MM-dd}");
                    
                    // Validate against MinDate/MaxDate with adjustment
                    bool startValid = IsDateInBounds(quarterRange.Start, owner);
                    bool endValid = IsDateInBounds(quarterRange.End, owner);
                    
                    if (!startValid || !endValid)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Quarterly] Quarter {quarter} range partially out of bounds, skipping");
                        return false;
                    }
                    
                    _start = quarterRange.Start;
                    _end = quarterRange.End;
                    _selectingStart = true;
                    
                    var days = (_end.Value - _start.Value).Days + 1;
                    System.Diagnostics.Debug.WriteLine($"[Quarterly] Range complete: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
                    
                    SyncToControl(owner);
                    owner.Invalidate();
                    return true;  // Close on quarter selection
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[Quarterly] Unknown quarter key: {quarterKey}");
                }
            }
            
            // Fallback calendar custom date range selection
            if (hitResult.Date.HasValue)
            {
                var clicked = hitResult.Date.Value.Date;
                
                // Validate against MinDate/MaxDate with logging
                if (!IsDateInBounds(clicked, owner))
                {
                    System.Diagnostics.Debug.WriteLine($"[Quarterly] Date {clicked:yyyy-MM-dd} is out of bounds");
                    return false;
                }
                
                if (_selectingStart)
                {
                    // Step 1: Select start date
                    _start = clicked;
                    _end = null;
                    _selectingStart = false;
                    
                    System.Diagnostics.Debug.WriteLine($"[Quarterly] Custom start date selected: {_start:yyyy-MM-dd}");
                    
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;  // Don't close, wait for end date
                }
                else
                {
                    // Step 2: Select end date
                    _end = clicked;
                    
                    // Auto-swap if end < start
                    if (_start.HasValue && _end.Value < _start.Value)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Quarterly] Swapping dates: start={_start:yyyy-MM-dd}, end={_end:yyyy-MM-dd}");
                        var tmp = _start;
                        _start = _end;
                        _end = tmp;
                    }
                    
                    // Validate and log range span
                    if (_start.HasValue && _end.HasValue)
                    {
                        var days = (_end.Value - _start.Value).Days + 1;
                        if (days > 365)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Quarterly] Warning: Large range selected ({days} days)");
                        }
                        System.Diagnostics.Debug.WriteLine($"[Quarterly] Custom range complete: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
                    }
                    
                    _selectingStart = true;
                    SyncToControl(owner);
                    owner.Invalidate();
                    return true;  // Close on completion
                }
            }
            
            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            
            // Quarter button hover
            if (hitResult.HitArea == DateTimePickerHitArea.QuarterButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.QuarterButton;
                hoverState.HoveredButton = hitResult.KeyValue ?? hitResult.CustomData?.ToString();
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Date hover with range preview (fallback calendar)
            else if (hitResult.Date.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
                
                // Show range preview when selecting end date
                if (!_selectingStart && _start.HasValue)
                {
                    DateTime a = _start.Value.Date;
                    DateTime b = hitResult.Date.Value.Date;
                    
                    // Normalize (ensure a <= b)
                    if (b < a)
                    {
                        var t = a;
                        a = b;
                        b = t;
                    }
                    
                    hoverState.HoveredRangePreview = (a, b);
                }
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.PreviousYearButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.PreviousYearButton;
                hoverState.HoveredButton = "nav_previous_year";
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.NextYearButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.NextYearButton;
                hoverState.HoveredButton = "nav_next_year";
                hoverState.HoverBounds = hitResult.HitBounds;
            }
        }

        public void SyncFromControl(BeepDateTimePicker owner)
        {
            _start = owner.RangeStartDate;
            _end = owner.RangeEndDate;
            _selectingStart = !(_start.HasValue && !_end.HasValue);
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Pre-sync validation
            if (_start.HasValue && _end.HasValue && !ValidateRangeSelection())
            {
                System.Diagnostics.Debug.WriteLine("[Quarterly] Sync aborted: Invalid range");
                return;
            }
            
            owner.RangeStartDate = _start;
            owner.RangeEndDate = _end;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
            owner.SelectedDate = _start;
            owner.SelectedTime = null;
            
            // Log sync operation
            if (_start.HasValue && _end.HasValue)
            {
                var days = (_end.Value - _start.Value).Days + 1;
                System.Diagnostics.Debug.WriteLine($"[Quarterly] Synced to control: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
            }
            else if (_start.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"[Quarterly] Synced partial: Start={_start:yyyy-MM-dd}, waiting for end date");
            }
        }

        public bool IsSelectionComplete()
        {
            return _start.HasValue && _end.HasValue;
        }

        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine("[Quarterly] Resetting range selection");
            
            _start = null;
            _end = null;
            _selectingStart = true;
            
          
        }

        private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties properties)
        {
            DateTime firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)properties.FirstDayOfWeek + 7) % 7;
            return firstDayOfMonth.AddDays(row * 7 + col - offset);
        }

        /// <summary>
        /// Convert quarter number (1-4) to date range
        /// Q1: Jan 1 - Mar 31, Q2: Apr 1 - Jun 30, Q3: Jul 1 - Sep 30, Q4: Oct 1 - Dec 31
        /// </summary>
        private (DateTime Start, DateTime End) GetQuarterDateRange(int quarter, int year)
        {
            if (quarter < 1 || quarter > 4)
                throw new ArgumentException("Quarter must be between 1 and 4", nameof(quarter));
            
            int startMonth = (quarter - 1) * 3 + 1;  // 1, 4, 7, 10
            DateTime start = new DateTime(year, startMonth, 1);
            DateTime end = start.AddMonths(3).AddDays(-1);
            
            return (start, end);
        }

        /// <summary>
        /// Determine which quarter a date falls into
        /// </summary>
        private int GetQuarterFromDate(DateTime date)
        {
            return ((date.Month - 1) / 3) + 1;
        }
        
        /// <summary>
        /// Validates that a date is within the control's allowed range (MinDate to MaxDate)
        /// </summary>
        private bool IsDateInBounds(DateTime date, BeepDateTimePicker owner)
        {
            if (owner == null) return true;
            
            if (date < owner.MinDate)
            {
                System.Diagnostics.Debug.WriteLine($"[Quarterly] Date {date:yyyy-MM-dd} is before MinDate {owner.MinDate:yyyy-MM-dd}");
                return false;
            }
            
            if (date > owner.MaxDate)
            {
                System.Diagnostics.Debug.WriteLine($"[Quarterly] Date {date:yyyy-MM-dd} is after MaxDate {owner.MaxDate:yyyy-MM-dd}");
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
                System.Diagnostics.Debug.WriteLine("[Quarterly] Validation failed: Start or end date is null");
                return false;
            }
            
            if (_end.Value < _start.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[Quarterly] Validation failed: End date {_end:yyyy-MM-dd} is before start date {_start:yyyy-MM-dd}");
                return false;
            }
            
            var days = (_end.Value - _start.Value).Days + 1;
            if (days > 3650) // 10 years
            {
                System.Diagnostics.Debug.WriteLine($"[Quarterly] Warning: Very large range ({days} days, ~{days/365} years)");
            }
            
            return true;
        }
    }
}
            
            