using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Hit handler for SingleWithTimeDateTimePickerPainter
    /// - Split layout: Calendar section (top 70%) + Time picker section (bottom 30%)
    /// - Calendar: Standard 7x6 grid with navigation
    /// - Time picker: Horizontal time slot grid (up to 8 visible slots)
    /// - Two-step interaction: Select date first, then select time
    /// - Separator line between calendar and time sections
    /// - Time slots displayed horizontally with wrapping
    /// - Doesn't close until both date AND time are selected
    /// </summary>
    internal class SingleWithTimeDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        private TimeSpan? _selectedTime;
        
        public DatePickerMode Mode => DatePickerMode.SingleWithTime;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();

            // Test navigation buttons first
            if (!layout.PreviousButtonRect.IsEmpty && layout.PreviousButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.PreviousButton;
                result.HitBounds = layout.PreviousButtonRect;
                result.NavigationButton = DatePickerNavigationButton.Previous;
                return result;
            }

            if (!layout.NextButtonRect.IsEmpty && layout.NextButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.NextButton;
                result.HitBounds = layout.NextButtonRect;
                result.NavigationButton = DatePickerNavigationButton.Next;
                return result;
            }

            // Test calendar day cells (top section)
            if (!layout.CalendarGridRect.IsEmpty && layout.CalendarGridRect.Contains(location))
            {
                var cells = layout.DayCellMatrix ?? layout.GetDayCellMatrixOrDefault();

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
                                result.CellIndex = row * 7 + col;
                                result.HitBounds = cellRect;
                                return result;
                            }
                        }
                    }
                }
            }

            // Test time slots (bottom section)
            // Check for time spinner buttons (hour/minute up/down)
            if (!layout.TimeHourUpRect.IsEmpty && layout.TimeHourUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartHourUpButton;
                result.HitBounds = layout.TimeHourUpRect;
                return result;
            }

            if (!layout.TimeHourDownRect.IsEmpty && layout.TimeHourDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartHourDownButton;
                result.HitBounds = layout.TimeHourDownRect;
                return result;
            }

            if (!layout.TimeMinuteUpRect.IsEmpty && layout.TimeMinuteUpRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartMinuteUpButton;
                result.HitBounds = layout.TimeMinuteUpRect;
                return result;
            }

            if (!layout.TimeMinuteDownRect.IsEmpty && layout.TimeMinuteDownRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.StartMinuteDownButton;
                result.HitBounds = layout.TimeMinuteDownRect;
                return result;
            }

            // Fallback: Check if clicking on the time spinner body (for potential future interactions)
            if (!layout.TimeHourRect.IsEmpty && layout.TimeHourRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.TimeSpinner;
                result.HitBounds = layout.TimeHourRect;
                return result;
            }

            if (!layout.TimeMinuteRect.IsEmpty && layout.TimeMinuteRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.TimeSpinner;
                result.HitBounds = layout.TimeMinuteRect;
                return result;
            }

            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit)
                return false;

            // Handle navigation buttons
            if (hitResult.HitArea == DateTimePickerHitArea.PreviousButton)
            {
                owner.NavigateToPreviousMonth();
                return false; // Don't close
            }

            if (hitResult.HitArea == DateTimePickerHitArea.NextButton)
            {
                owner.NavigateToNextMonth();
                return false; // Don't close
            }

            // Handle day cell click - Step 1: Select date
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell && hitResult.Date.HasValue)
            {
                var date = hitResult.Date.Value.Date;
                
                // Check if date is disabled
                var props = owner.GetCurrentProperties();
                if (props != null)
                {
                    if (props.MinDate.HasValue && date < props.MinDate.Value)
                        return false;
                    if (props.MaxDate.HasValue && date > props.MaxDate.Value)
                        return false;
                }

                // Select date and clear time (user must reselect time)
                _selectedDate = date;
                // Don't clear time if already set - allow changing date without losing time
                SyncToControl(owner);
                
                // Don't close - wait for time selection
                return false;
            }

            // Handle time slot click - Step 2: Select time
            if (hitResult.HitArea == DateTimePickerHitArea.TimeSlot && hitResult.Time.HasValue)
            {
                // Can only select time if date is already selected
                if (!_selectedDate.HasValue)
                {
                    // Maybe select today's date automatically?
                    _selectedDate = DateTime.Today;
                }
                
                _selectedTime = hitResult.Time.Value;
                SyncToControl(owner);
                
                // Both date and time selected - close dropdown
                return true;
            }

            // Handle time spinner button clicks
            if (hitResult.HitArea == DateTimePickerHitArea.StartHourUpButton)
            {
                if (!_selectedTime.HasValue)
                    _selectedTime = TimeSpan.Zero;
                
                int newHour = (_selectedTime.Value.Hours + 1) % 24;
                _selectedTime = new TimeSpan(newHour, _selectedTime.Value.Minutes, 0);
                SyncToControl(owner);
                return false; // Don't close
            }

            if (hitResult.HitArea == DateTimePickerHitArea.StartHourDownButton)
            {
                if (!_selectedTime.HasValue)
                    _selectedTime = TimeSpan.Zero;
                
                int newHour = (_selectedTime.Value.Hours - 1 + 24) % 24;
                _selectedTime = new TimeSpan(newHour, _selectedTime.Value.Minutes, 0);
                SyncToControl(owner);
                return false; // Don't close
            }

            if (hitResult.HitArea == DateTimePickerHitArea.StartMinuteUpButton)
            {
                if (!_selectedTime.HasValue)
                    _selectedTime = TimeSpan.Zero;
                
                var props = owner.GetCurrentProperties();
                int interval = props?.TimeInterval.Minutes ?? 1;
                int newMinute = (_selectedTime.Value.Minutes + interval) % 60;
                _selectedTime = new TimeSpan(_selectedTime.Value.Hours, newMinute, 0);
                SyncToControl(owner);
                return false; // Don't close
            }

            if (hitResult.HitArea == DateTimePickerHitArea.StartMinuteDownButton)
            {
                if (!_selectedTime.HasValue)
                    _selectedTime = TimeSpan.Zero;
                
                var props = owner.GetCurrentProperties();
                int interval = props?.TimeInterval.Minutes ?? 1;
                int newMinute = (_selectedTime.Value.Minutes - interval + 60) % 60;
                _selectedTime = new TimeSpan(_selectedTime.Value.Hours, newMinute, 0);
                SyncToControl(owner);
                return false; // Don't close
            }

            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            
            if (!hitResult.IsHit)
                return;

            // Hover on day cell (calendar section)
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on time slot (time picker section)
            else if (hitResult.HitArea == DateTimePickerHitArea.TimeSlot)
            {
                hoverState.HoverArea = DateTimePickerHitArea.TimeSlot;
                hoverState.HoveredTime = hitResult.Time;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on time spinner buttons
            else if (hitResult.HitArea == DateTimePickerHitArea.StartHourUpButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartHourDownButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartMinuteUpButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartMinuteDownButton ||
                     hitResult.HitArea == DateTimePickerHitArea.TimeSpinner)
            {
                hoverState.HoverArea = hitResult.HitArea;
                hoverState.HoverBounds = hitResult.HitBounds;
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
            _selectedDate = owner.SelectedDate;
            _selectedTime = owner.SelectedTime;
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            owner.SelectedDate = _selectedDate;
            owner.SelectedTime = _selectedTime;
            
            // Clear range selection modes
            owner.RangeStartDate = null;
            owner.RangeEndDate = null;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
        }

        public bool IsSelectionComplete()
        {
            // Both date AND time must be selected
            return _selectedDate.HasValue && _selectedTime.HasValue;
        }

        public void Reset()
        {
            _selectedDate = null;
            _selectedTime = null;
        }

        /// <summary>
        /// Calculate the date for a specific cell in the calendar grid
        /// </summary>
        private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties properties)
        {
            DateTime firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            DayOfWeek firstDayOfWeek = (DayOfWeek)(int)properties.FirstDayOfWeek;
            
            // Calculate offset from start of week
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
            
            // Calculate day offset for this cell
            int dayOffset = row * 7 + col - offset;
            
            return firstDayOfMonth.AddDays(dayOffset);
        }

        /// <summary>
        /// Calculate time picker bounds based on layout
        /// Matches painter's logic: below calendar grid with padding
        /// </summary>
        private Rectangle CalculateTimePickerBounds(DateTimePickerLayout layout)
        {
            // Match painter: 16px padding on sides, 16px below calendar, 80px height
            int padding = 16;
            int timePickerHeight = 80;
            
            return new Rectangle(
                layout.CalendarGridRect.X,
                layout.CalendarGridRect.Bottom + padding,
                layout.CalendarGridRect.Width,
                timePickerHeight
            );
        }

        /// <summary>
        /// Generate time slots matching painter's logic
        /// Uses properties.TimeInterval, MinTime, MaxTime
        /// </summary>
        private List<TimeSpan> GenerateTimeSlots(DateTimePickerProperties properties)
        {
            var slots = new List<TimeSpan>();
            var interval = properties.TimeInterval;
            if (interval <= TimeSpan.Zero)
            {
                int minutes = properties.TimeIntervalMinutes > 0 ? properties.TimeIntervalMinutes : 30;
                interval = TimeSpan.FromMinutes(minutes);
            }

            var minTime = properties.MinTime;
            var maxTime = properties.MaxTime;
            if (maxTime < minTime)
            {
                var temp = minTime;
                minTime = maxTime;
                maxTime = temp;
            }
            
            var current = minTime;
            while (current <= maxTime)
            {
                slots.Add(current);
                current = current.Add(interval);
            }
            
            return slots;
        }

        /// <summary>
        /// Calculate time slot rectangles matching painter's horizontal layout
        /// Painter shows up to 8 slots horizontally with wrapping
        /// </summary>
        private List<Rectangle> CalculateTimeSlotRects(Rectangle timePickerBounds, List<TimeSpan> timeSlots, DateTimePickerProperties properties)
        {
            var rects = new List<Rectangle>();
            
            int slotWidth = 70;
            int slotHeight = 32;
            int gap = 4;
            int startX = timePickerBounds.X + 70; // After "Time:" label
            int currentX = startX;
            int currentY = timePickerBounds.Y + 8;
            int maxWidth = timePickerBounds.Width - 70;
            
            // Show up to 8 slots (painter logic)
            int visibleSlots = Math.Min(timeSlots.Count, 8);
            
            for (int i = 0; i < visibleSlots; i++)
            {
                var slotRect = new Rectangle(currentX, currentY, slotWidth, slotHeight);
                rects.Add(slotRect);
                
                currentX += slotWidth + gap;
                
                // Wrap to next row if needed
                if (currentX + slotWidth > timePickerBounds.Right)
                {
                    currentX = startX;
                    currentY += slotHeight + gap;
                }
            }
            
            return rects;
        }
    }
}
