using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Hit handler for RangeDateTimePickerPainter
    /// - Two-step range selection (start date, then end date)
    /// - Range preview on hover (shows potential range)
    /// - State machine: WaitingForStart -> WaitingForEnd -> Complete
    /// - Auto-swap if end is before start
    /// - Visual feedback for partial selection
    /// </summary>
    internal class RangeDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _start;
        private DateTime? _end;
        private bool _selectingStart = true;
        
        public DatePickerMode Mode => DatePickerMode.Range;

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
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell && hitResult.Date.HasValue)
            {
                var clicked = hitResult.Date.Value.Date;
                
                // Validate against MinDate/MaxDate with logging
                if (!IsDateInBounds(clicked, owner))
                {
                    System.Diagnostics.Debug.WriteLine($"[Range] Date {clicked:yyyy-MM-dd} is out of bounds");
                    return false;
                }
                
                if (_selectingStart)
                {
                    // Step 1: Select start date
                    _start = clicked;
                    _end = null;
                    _selectingStart = false;
                    
                    System.Diagnostics.Debug.WriteLine($"[Range] Start date selected: {_start:yyyy-MM-dd}");
                    
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;
                }
                else
                {
                    // Step 2: Select end date
                    _end = clicked;
                    
                    // Auto-swap if end < start (normalize the range)
                    if (_start.HasValue && _end.Value < _start.Value)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Range] Swapping dates: start={_start:yyyy-MM-dd}, end={_end:yyyy-MM-dd}");
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
                            System.Diagnostics.Debug.WriteLine($"[Range] Warning: Large range selected ({days} days)");
                        }
                        System.Diagnostics.Debug.WriteLine($"[Range] Range complete: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
                    }
                    
                    _selectingStart = true;
                    SyncToControl(owner);
                    owner.Invalidate();
                    return true;
                }
            }
            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            
            if (!hitResult.IsHit)
                return;

            // Hover on day cell
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date;
                hoverState.HoverBounds = hitResult.HitBounds;

                // Show range preview when hovering after start is selected
                if (!_selectingStart && _start.HasValue && hitResult.Date.HasValue)
                {
                    DateTime rangeStart = _start.Value.Date;
                    DateTime rangeEnd = hitResult.Date.Value.Date;
                    
                    // Ensure start is always before end for preview
                    if (rangeEnd < rangeStart)
                    {
                        var temp = rangeStart;
                        rangeStart = rangeEnd;
                        rangeEnd = temp;
                    }
                    
                    // Set preview range for painter to visualize
                    hoverState.HoveredRangePreview = (rangeStart, rangeEnd);
                }
            }
            // Hover on navigation buttons
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
            _selectingStart = !(_start.HasValue && !_end.HasValue);
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Pre-sync validation
            if (_start.HasValue && _end.HasValue && !ValidateRangeSelection())
            {
                System.Diagnostics.Debug.WriteLine("[Range] Sync aborted: Invalid range");
                return;
            }
            
            owner.RangeStartDate = _start;
            owner.RangeEndDate = _end;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
            owner.SelectedDate = null;
            owner.SelectedTime = null;
            
            // Log sync operation
            if (_start.HasValue && _end.HasValue)
            {
                var days = (_end.Value - _start.Value).Days + 1;
                System.Diagnostics.Debug.WriteLine($"[Range] Synced to control: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
            }
            else if (_start.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"[Range] Synced partial: Start={_start:yyyy-MM-dd}, waiting for end date");
            }
        }

        public bool IsSelectionComplete()
        {
            return _start.HasValue && _end.HasValue;
        }

        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine("[Range] Resetting range selection");
            
            _start = null;
            _end = null;
            _selectingStart = true;
            
            // Note: Owner must be passed to clear properties via external call
            // This method only resets internal state
        }

        private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties properties)
        {
            DateTime firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            DayOfWeek firstDayOfWeek = (DayOfWeek)(int)properties.FirstDayOfWeek;
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
            int dayOffset = row * 7 + col - offset;
            return firstDayOfMonth.AddDays(dayOffset);
        }
        
        /// <summary>
        /// Validates that a date is within the control's allowed range (MinDate to MaxDate)
        /// </summary>
        private bool IsDateInBounds(DateTime date, BeepDateTimePicker owner)
        {
            if (owner == null) return true;
            
            if (date < owner.MinDate)
            {
                System.Diagnostics.Debug.WriteLine($"[Range] Date {date:yyyy-MM-dd} is before MinDate {owner.MinDate:yyyy-MM-dd}");
                return false;
            }
            
            if (date > owner.MaxDate)
            {
                System.Diagnostics.Debug.WriteLine($"[Range] Date {date:yyyy-MM-dd} is after MaxDate {owner.MaxDate:yyyy-MM-dd}");
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
                System.Diagnostics.Debug.WriteLine("[Range] Validation failed: Start or end date is null");
                return false;
            }
            
            if (_end.Value < _start.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[Range] Validation failed: End date {_end:yyyy-MM-dd} is before start date {_start:yyyy-MM-dd}");
                return false;
            }
            
            var days = (_end.Value - _start.Value).Days + 1;
            if (days > 3650) // 10 years
            {
                System.Diagnostics.Debug.WriteLine($"[Range] Warning: Very large range ({days} days, ~{days/365} years)");
            }
            
            return true;
        }
    }
}
