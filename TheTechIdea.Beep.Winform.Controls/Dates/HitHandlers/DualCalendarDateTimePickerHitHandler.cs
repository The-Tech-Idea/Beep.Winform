using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Dual Calendar Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - Two side-by-side calendars (left: current month, right: next month)
    /// - Layout: padding=16px, gap=12px between calendars
    /// - Each calendar width: (totalWidth - 2*padding - gap) / 2
    /// - Vertical separator line between calendars at X + padding + calendarWidth + gap/2
    /// - Range info displayed at bottom (Height=34, Y=Bottom-50)
    /// - Navigation buttons only on LEFT calendar (both prev/next)
    /// 
    /// INTERACTION MODEL:
    /// 1. Two-step range selection: Start date → End date
    /// 2. Cross-calendar selection: Can select start in left, end in right
    /// 3. Range preview on hover: Shows tentative range from start to hovered date
    /// 4. Auto-swap: If end < start, automatically swaps them
    /// 5. Synchronized navigation: Both calendars move together by 1 month
    /// 
    /// LAYOUT STRUCTURE:
    /// - Left calendar: X+16, shows current displayMonth
    /// - Right calendar: X+16+calendarWidth+gap, shows displayMonth+1
    /// - Range info: Bottom-50, shows "Start — End (X days)"
    /// </summary>
    internal class DualCalendarDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _start;  // Range start date
        private DateTime? _end;    // Range end date
        private bool _selectingStart = true;  // State: true = awaiting start, false = awaiting end
        public DatePickerMode Mode => DatePickerMode.DualCalendar;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            
            // Check if we have dual calendar layout
            if (layout.MonthGrids != null && layout.MonthGrids.Count > 0)
            {
                // Iterate through both calendars (0=left, 1=right)
                for (int gridIndex = 0; gridIndex < layout.MonthGrids.Count; gridIndex++)
                {
                    var grid = layout.MonthGrids[gridIndex];
                    if (grid == null || grid.DayCellRects == null) continue;
                    
                    // Calculate month for this grid (left=displayMonth, right=displayMonth+1)
                    DateTime gridMonth = displayMonth.AddMonths(gridIndex);
                    
                    // Navigation buttons only on left calendar (gridIndex == 0)
                    if (gridIndex == 0)
                    {
                        if (!grid.PreviousButtonRect.IsEmpty && grid.PreviousButtonRect.Contains(location))
                        {
                            result.IsHit = true;
                            result.HitArea = DateTimePickerHitArea.PreviousButton;
                            result.HitBounds = grid.PreviousButtonRect;
                            result.GridIndex = gridIndex;
                            return result;
                        }
                        if (!grid.NextButtonRect.IsEmpty && grid.NextButtonRect.Contains(location))
                        {
                            result.IsHit = true;
                            result.HitArea = DateTimePickerHitArea.NextButton;
                            result.HitBounds = grid.NextButtonRect;
                            result.GridIndex = gridIndex;
                            return result;
                        }
                    }
                    
                    // Year combo box (available on both calendars)
                    if (!grid.YearComboBoxRect.IsEmpty && grid.YearComboBoxRect.Contains(location))
                    {
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.YearComboBox;
                        result.HitBounds = grid.YearComboBoxRect;
                        result.GridIndex = gridIndex;
                        return result;
                    }
                    
                    // Day cell detection in this calendar
                    DateTime firstDayOfMonth = new DateTime(gridMonth.Year, gridMonth.Month, 1);
                    int offset = ((int)firstDayOfMonth.DayOfWeek - (int)properties.FirstDayOfWeek + 7) % 7;
                    int daysInMonth = DateTime.DaysInMonth(gridMonth.Year, gridMonth.Month);
                    
                    for (int i = 0; i < grid.DayCellRects.Count; i++)
                    {
                        Rectangle cellRect = grid.DayCellRects[i];
                        if (cellRect.Contains(location))
                        {
                            int dayNum = i - offset + 1;
                            
                            // Only valid dates within the month
                            if (dayNum >= 1 && dayNum <= daysInMonth)
                            {
                                DateTime date = new DateTime(gridMonth.Year, gridMonth.Month, dayNum);
                                result.IsHit = true;
                                result.HitArea = DateTimePickerHitArea.DayCell;
                                result.Date = date;
                                result.GridIndex = gridIndex;
                                result.HitBounds = cellRect;
                                return result;
                            }
                        }
                    }
                }
            }
            
            // Check for Reset button (clears range selection)
            if (layout.ResetButtonRect != Rectangle.Empty && layout.ResetButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.ResetButton;
                result.HitBounds = layout.ResetButtonRect;
                return result;
            }
            
            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit) return false;
            
            // Navigation (synchronized - both calendars move together)
            if (hitResult.HitArea == DateTimePickerHitArea.PreviousButton)
            {
                owner.NavigateToPreviousMonth();
                return false;  // Don't close
            }
            if (hitResult.HitArea == DateTimePickerHitArea.NextButton)
            {
                owner.NavigateToNextMonth();
                return false;  // Don't close
            }
            
            // Year combo box - show year selection dropdown
            if (hitResult.HitArea == DateTimePickerHitArea.YearComboBox)
            {
                ShowYearComboBox(owner, hitResult);
                return false;  // Don't close
            }
            
            // Reset button - clear range selection
            if (hitResult.HitArea == DateTimePickerHitArea.ResetButton)
            {
                Reset();
                SyncToControl(owner);
                return false;  // Don't close
            }
            
            // Date selection (two-step range)
            if (hitResult.Date.HasValue)
            {
                var clicked = hitResult.Date.Value.Date;
                
                // Validate against MinDate/MaxDate with logging
                if (!IsDateInBounds(clicked, owner))
                {
                    System.Diagnostics.Debug.WriteLine($"[DualCalendar] Date {clicked:yyyy-MM-dd} is out of bounds");
                    return false;
                }
                
                if (_selectingStart)
                {
                    // Step 1: Select start date with default start-of-day time
                    _start = clicked;
                    _end = null;
                    _selectingStart = false;
                    
                    System.Diagnostics.Debug.WriteLine($"[DualCalendar] Start date selected: {_start:yyyy-MM-dd}");
                    
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;  // Don't close, wait for end date
                }
                else
                {
                    // Step 2: Select end date with default end-of-day time
                    _end = clicked;
                    
                    // Auto-swap if end < start (normalize the range)
                    if (_start.HasValue && _end.Value < _start.Value)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DualCalendar] Swapping dates: start={_start:yyyy-MM-dd}, end={_end:yyyy-MM-dd}");
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
                            System.Diagnostics.Debug.WriteLine($"[DualCalendar] Warning: Large range selected ({days} days)");
                        }
                        System.Diagnostics.Debug.WriteLine($"[DualCalendar] Range complete: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
                    }
                    
                    _selectingStart = true;  // Reset for next range
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
            
            // Date hover with cross-calendar range preview
            if (hitResult.Date.HasValue)
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
                    
                    // Set preview range (painter will highlight all dates between a and b)
                    hoverState.HoveredRangePreview = (a, b);
                }
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
            else if (hitResult.HitArea == DateTimePickerHitArea.YearComboBox)
            {
                hoverState.HoverArea = DateTimePickerHitArea.YearComboBox;
                hoverState.HoverBounds = hitResult.HitBounds;
                hoverState.HoveredGridIndex = hitResult.GridIndex ?? -1;  // Track which calendar's combo box
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.ResetButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.ResetButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
        }

        public void SyncFromControl(BeepDateTimePicker owner)
        {
            _start = owner.RangeStartDate;
            _end = owner.RangeEndDate;
            
            // Determine selection state:
            // - If no start: awaiting start
            // - If start but no end: awaiting end
            // - If both: awaiting start (ready for new range)
            _selectingStart = !(_start.HasValue && !_end.HasValue);
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Pre-sync validation
            if (_start.HasValue && _end.HasValue && !ValidateRangeSelection())
            {
                System.Diagnostics.Debug.WriteLine("[DualCalendar] Sync aborted: Invalid range");
                return;
            }
            
            // Update range dates
            owner.RangeStartDate = _start;
            owner.RangeEndDate = _end;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
            
            // Set SelectedDate to start for consistency
            owner.SelectedDate = _start;
            owner.SelectedTime = null;
            
            // Log sync operation
            if (_start.HasValue && _end.HasValue)
            {
                var days = (_end.Value - _start.Value).Days + 1;
                System.Diagnostics.Debug.WriteLine($"[DualCalendar] Synced to control: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
            }
            else if (_start.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"[DualCalendar] Synced partial: Start={_start:yyyy-MM-dd}, waiting for end date");
            }
        }

        public bool IsSelectionComplete()
        {
            return _start.HasValue && _end.HasValue;
        }

        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine("[DualCalendar] Resetting range selection");
            
            _start = null;
            _end = null;
            _selectingStart = true;
            
            // Note: Owner must be passed to clear properties via external call
            // This method only resets internal state
        }

        /// <summary>
        /// Calculate the total date range span in days
        /// </summary>
        private int GetRangeDays()
        {
            if (!_start.HasValue || !_end.HasValue)
                return 0;
            
            return (_end.Value - _start.Value).Days + 1;
        }

        /// <summary>
        /// Check if a date falls within the selected range
        /// </summary>
        private bool IsDateInRange(DateTime date)
        {
            if (!_start.HasValue || !_end.HasValue)
                return false;
            
            return date >= _start.Value && date <= _end.Value;
        }

        /// <summary>
        /// Show year combo box dropdown for year selection
        /// Dual calendar specific: handles independent year selection for left or right calendar
        /// Uses BeepComboBox with ShowDropdown() to display year options
        /// </summary>
        private void ShowYearComboBox(BeepDateTimePicker owner, DateTimePickerHitTestResult hitResult)
        {
            if (owner == null) return;

            // Determine which calendar was clicked (left=0, right=1)
            int gridIndex = hitResult.GridIndex ?? 0;
            
            // Calculate the actual month being displayed in the clicked calendar
            // Left calendar (gridIndex 0) shows DisplayMonth
            // Right calendar (gridIndex 1) shows DisplayMonth + 1 month
            DateTime targetMonth = owner.DisplayMonth.AddMonths(gridIndex);
            int currentYear = targetMonth.Year;

            // Get year range from MinDate/MaxDate
            int minYear = owner.MinDate.Year;
            int maxYear = owner.MaxDate.Year;

            // Create BeepComboBox using DateTimeComboBoxHelper
            var comboBox = DateTimeComboBoxHelper.CreateYearComboBox(minYear, maxYear, currentYear);
            
            // Set up selection change handler
            comboBox.SelectedIndexChanged += (s, e) =>
            {
                int? selectedYear = DateTimeComboBoxHelper.GetSelectedYear(comboBox);
                if (!selectedYear.HasValue) return;
                
                // Create new month with selected year but same month number
                DateTime newTargetMonth = new DateTime(selectedYear.Value, targetMonth.Month, 1);
                
                // Calculate how to adjust DisplayMonth to make the clicked calendar show newTargetMonth
                // If left calendar (gridIndex 0): DisplayMonth should equal newTargetMonth
                // If right calendar (gridIndex 1): DisplayMonth should equal newTargetMonth - 1 month
                DateTime desiredDisplayMonth = newTargetMonth.AddMonths(-gridIndex);
                
                // Calculate month difference from current DisplayMonth
                int monthDiff = ((desiredDisplayMonth.Year - owner.DisplayMonth.Year) * 12) + 
                               (desiredDisplayMonth.Month - owner.DisplayMonth.Month);
                
                // Navigate to adjust DisplayMonth
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
            // Convert to screen coordinates for proper positioning
            var screenPoint = owner.PointToScreen(new Point(
                hitResult.HitBounds.X, 
                hitResult.HitBounds.Y
            ));
            
            // Set combo box size to match the hit bounds
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
        /// Validates that a date is within the control's allowed range (MinDate to MaxDate)
        /// </summary>
        private bool IsDateInBounds(DateTime date, BeepDateTimePicker owner)
        {
            if (owner == null) return true;
            
            if (date < owner.MinDate)
            {
                System.Diagnostics.Debug.WriteLine($"[DualCalendar] Date {date:yyyy-MM-dd} is before MinDate {owner.MinDate:yyyy-MM-dd}");
                return false;
            }
            
            if (date > owner.MaxDate)
            {
                System.Diagnostics.Debug.WriteLine($"[DualCalendar] Date {date:yyyy-MM-dd} is after MaxDate {owner.MaxDate:yyyy-MM-dd}");
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
                System.Diagnostics.Debug.WriteLine("[DualCalendar] Validation failed: Start or end date is null");
                return false;
            }
            
            if (_end.Value < _start.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[DualCalendar] Validation failed: End date {_end:yyyy-MM-dd} is before start date {_start:yyyy-MM-dd}");
                return false;
            }
            
            var days = (_end.Value - _start.Value).Days + 1;
            if (days > 3650) // 10 years
            {
                System.Diagnostics.Debug.WriteLine($"[DualCalendar] Warning: Very large range ({days} days, ~{days/365} years)");
            }
            
            return true;
        }
    }
}
