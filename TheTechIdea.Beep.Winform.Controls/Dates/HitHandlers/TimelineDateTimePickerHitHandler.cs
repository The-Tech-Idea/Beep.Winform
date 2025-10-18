using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Timeline Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - Visual timeline bar with start/end date handles
    /// - Drag handles to adjust date range
    /// - Mini calendar below for reference/quick selection
    /// - Timeline track spans from MinDate to MaxDate (or reasonable default)
    /// - Handles show current start/end dates
    /// 
    /// INTERACTION MODEL:
    /// 1. Drag start handle → Move start date along timeline
    /// 2. Drag end handle → Move end date along timeline
    /// 3. Click timeline track → Jump selected handle to position
    /// 4. Click mini calendar → Set selected handle date directly
    /// 5. Range automatically normalized (start <= end)
    /// 
    /// LAYOUT STRUCTURE:
    /// - Header: Y+20, Height=40, "Date Range Timeline" title
    /// - Timeline bar: Y+80, Height=80, draggable range visualization
    ///   * Start handle: Left side (24x40 rectangle)
    ///   * End handle: Right side (24x40 rectangle)
    ///   * Track: Horizontal bar showing date range
    /// - Date labels: Y+180, Height=60, shows start/end dates
    /// - Mini calendar: Y+260, Height=160, standard month view
    /// 
    /// HANDLE INTERACTION:
    /// - Click handle to select it (for keyboard control)
    /// - Drag handle to adjust date
    /// - Release handle to commit date
    /// - Handles snap to valid dates
    /// </summary>
    internal class TimelineDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _start;
        private DateTime? _end;
        private string _activeHandle = null;  // "start", "end", or null
        private bool _isDragging = false;
        
        public DatePickerMode Mode => DatePickerMode.Timeline;

        
        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            
            // Calculate timeline bounds (matching painter layout)
            int padding = 20;
            int timelineY = layout.HeaderRect.Bottom + 60;  // After header
            int timelineWidth = layout.HeaderRect.Width - padding * 4;
            var timelineTrackRect = new Rectangle(layout.HeaderRect.X + padding * 2, timelineY, timelineWidth, 20);
            
            // Calculate handle positions
            DateTime minDate = properties.MinDate ?? DateTime.Today.AddMonths(-6);
            DateTime maxDate = properties.MaxDate ?? DateTime.Today.AddMonths(6);
            int totalDays = (maxDate - minDate).Days;
            
            if (_start.HasValue && _end.HasValue && totalDays > 0)
            {
                // Start handle position
                int startDays = (_start.Value - minDate).Days;
                int startX = timelineTrackRect.X + (int)((double)startDays / totalDays * timelineTrackRect.Width);
                var startHandleRect = new Rectangle(startX - 12, timelineTrackRect.Y - 10, 24, 40);
                
                if (startHandleRect.Contains(location))
                {
                    result.IsHit = true;
                    result.HitArea = DateTimePickerHitArea.StartHandle;
                    result.HitBounds = startHandleRect;
                    result.Date = _start;
                    return result;
                }
                
                // End handle position
                int endDays = (_end.Value - minDate).Days;
                int endX = timelineTrackRect.X + (int)((double)endDays / totalDays * timelineTrackRect.Width);
                var endHandleRect = new Rectangle(endX - 12, timelineTrackRect.Y - 10, 24, 40);
                
                if (endHandleRect.Contains(location))
                {
                    result.IsHit = true;
                    result.HitArea = DateTimePickerHitArea.EndHandle;
                    result.HitBounds = endHandleRect;
                    result.Date = _end;
                    return result;
                }
            }
            
            // Timeline track (for quick positioning)
            var expandedTrackRect = new Rectangle(timelineTrackRect.X, timelineTrackRect.Y - 10, timelineTrackRect.Width, 40);
            if (expandedTrackRect.Contains(location) && totalDays > 0)
            {
                // Calculate date at click position
                int offsetX = location.X - timelineTrackRect.X;
                double percentage = (double)offsetX / timelineTrackRect.Width;
                DateTime clickedDate = minDate.AddDays((int)(percentage * totalDays));
                
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.TimelineTrack;
                result.HitBounds = timelineTrackRect;
                result.Date = clickedDate;
                return result;
            }
            
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
            
            // Mini calendar grid at bottom
            if (!layout.CalendarGridRect.IsEmpty && layout.CalendarGridRect.Contains(location))
            {
                var cells = layout.DayCellMatrix ?? layout.GetDayCellMatrixOrDefault();

                if (cells != null)
                {
                    for (int row = 0; row < cells.GetLength(0); row++)
                    {
                        for (int col = 0; col < cells.GetLength(1); col++)
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
            
            // Handle selection/dragging
            if (hitResult.HitArea == DateTimePickerHitArea.StartHandle)
            {
                _activeHandle = "start";
                _isDragging = true;
                System.Diagnostics.Debug.WriteLine("[Timeline] Start handle grabbed");
                return false;  // Don't close, allow dragging
            }
            if (hitResult.HitArea == DateTimePickerHitArea.EndHandle)
            {
                _activeHandle = "end";
                _isDragging = true;
                System.Diagnostics.Debug.WriteLine("[Timeline] End handle grabbed");
                return false;  // Don't close, allow dragging
            }
            
            // Timeline track click → Move active handle (or start handle by default)
            if (hitResult.HitArea == DateTimePickerHitArea.TimelineTrack && hitResult.Date.HasValue)
            {
                var newDate = hitResult.Date.Value.Date;
                
                // Validate against MinDate/MaxDate with adjustment
                if (!IsDateInBounds(newDate, owner))
                {
                    var minDate = owner.MinDate.Date;
                    var maxDate = owner.MaxDate.Date;
                    
                    if (newDate < minDate)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Timeline] Track click {newDate:yyyy-MM-dd} adjusted to MinDate {minDate:yyyy-MM-dd}");
                        newDate = minDate;
                    }
                    else if (newDate > maxDate)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Timeline] Track click {newDate:yyyy-MM-dd} adjusted to MaxDate {maxDate:yyyy-MM-dd}");
                        newDate = maxDate;
                    }
                }
                
                if (_activeHandle == "end" || (_start.HasValue && !_end.HasValue))
                {
                    _end = newDate;
                    _activeHandle = "end";
                    System.Diagnostics.Debug.WriteLine($"[Timeline] End handle moved to {_end:yyyy-MM-dd} via track");
                }
                else
                {
                    _start = newDate;
                    _activeHandle = "start";
                    System.Diagnostics.Debug.WriteLine($"[Timeline] Start handle moved to {_start:yyyy-MM-dd} via track");
                }
                
                // Normalize range
                if (_start.HasValue && _end.HasValue && _end.Value < _start.Value)
                {
                    System.Diagnostics.Debug.WriteLine($"[Timeline] Swapping dates: start={_start:yyyy-MM-dd}, end={_end:yyyy-MM-dd}");
                    var tmp = _start;
                    _start = _end;
                    _end = tmp;
                }
                
                // Log range span
                if (_start.HasValue && _end.HasValue)
                {
                    var days = (_end.Value - _start.Value).Days + 1;
                    System.Diagnostics.Debug.WriteLine($"[Timeline] Current range: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
                }
                
                SyncToControl(owner);
                owner.Invalidate();
                return false;  // Keep open for more adjustments
            }
            
            // Mini calendar click → Set active handle date
            if (hitResult.Date.HasValue)
            {
                var clicked = hitResult.Date.Value.Date;
                
                // Validate against MinDate/MaxDate with logging
                if (!IsDateInBounds(clicked, owner))
                {
                    System.Diagnostics.Debug.WriteLine($"[Timeline] Calendar date {clicked:yyyy-MM-dd} is out of bounds");
                    return false;
                }
                
                // Set start if empty, otherwise set end
                if (!_start.HasValue || (_start.HasValue && _end.HasValue))
                {
                    _start = clicked;
                    _end = null;
                    _activeHandle = "start";
                    System.Diagnostics.Debug.WriteLine($"[Timeline] Start date selected via calendar: {_start:yyyy-MM-dd}");
                    SyncToControl(owner);
                    owner.Invalidate();
                    return false;  // Wait for end date
                }
                else
                {
                    _end = clicked;
                    _activeHandle = "end";
                    
                    // Normalize range
                    if (_start.HasValue && _end.Value < _start.Value)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Timeline] Swapping dates: start={_start:yyyy-MM-dd}, end={_end:yyyy-MM-dd}");
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
                            System.Diagnostics.Debug.WriteLine($"[Timeline] Warning: Large range selected ({days} days)");
                        }
                        System.Diagnostics.Debug.WriteLine($"[Timeline] Range complete: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
                    }
                    
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
            
            // Handle hover
            if (hitResult.HitArea == DateTimePickerHitArea.StartHandle || hitResult.HitArea == DateTimePickerHitArea.EndHandle)
            {
                hoverState.HoverArea = DateTimePickerHitArea.Handle;
                hoverState.HoveredButton = hitResult.HitArea == DateTimePickerHitArea.StartHandle ? "handle_start" : "handle_end";
                hoverState.HoverBounds = hitResult.HitBounds;
                if (hitResult.Date.HasValue)
                    hoverState.HoveredDate = hitResult.Date.Value;
            }
            // Timeline track hover
            else if (hitResult.HitArea == DateTimePickerHitArea.TimelineTrack && hitResult.Date.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.TimelineTrack;
                hoverState.HoveredDate = hitResult.Date.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Day cell hover
            else if (hitResult.Date.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
                
                // Show range preview when setting end date
                if (_start.HasValue && !_end.HasValue)
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
            _activeHandle = _start.HasValue && !_end.HasValue ? "start" : "end";
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Pre-sync validation
            if (_start.HasValue && _end.HasValue && !ValidateRangeSelection())
            {
                System.Diagnostics.Debug.WriteLine("[Timeline] Sync aborted: Invalid range");
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
                System.Diagnostics.Debug.WriteLine($"[Timeline] Synced to control: {_start:yyyy-MM-dd} to {_end:yyyy-MM-dd} ({days} days)");
            }
            else if (_start.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"[Timeline] Synced partial: Start={_start:yyyy-MM-dd}, waiting for end date");
            }
        }

        public bool IsSelectionComplete()
        {
            return _start.HasValue && _end.HasValue;
        }

        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine("[Timeline] Resetting range selection");
            
            _start = null;
            _end = null;
            _activeHandle = null;
            _isDragging = false;
          
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
                System.Diagnostics.Debug.WriteLine($"[Timeline] Date {date:yyyy-MM-dd} is before MinDate {owner.MinDate:yyyy-MM-dd}");
                return false;
            }
            
            if (date > owner.MaxDate)
            {
                System.Diagnostics.Debug.WriteLine($"[Timeline] Date {date:yyyy-MM-dd} is after MaxDate {owner.MaxDate:yyyy-MM-dd}");
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
                System.Diagnostics.Debug.WriteLine("[Timeline] Validation failed: Start or end date is null");
                return false;
            }
            
            if (_end.Value < _start.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[Timeline] Validation failed: End date {_end:yyyy-MM-dd} is before start date {_start:yyyy-MM-dd}");
                return false;
            }
            
            var days = (_end.Value - _start.Value).Days + 1;
            if (days > 3650) // 10 years
            {
                System.Diagnostics.Debug.WriteLine($"[Timeline] Warning: Very large range ({days} days, ~{days/365} years)");
            }
            
            return true;
        }
    }
}
