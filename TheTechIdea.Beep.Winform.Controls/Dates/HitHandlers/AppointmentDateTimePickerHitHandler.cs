using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Hit handler for AppointmentDateTimePickerPainter
    /// - DISTINCT LAYOUT: Split panel - Calendar (left 55%) + Time Slot List (right 45%)
    /// - Vertical separator line between panels
    /// - Calendar: Standard grid with navigation
    /// - Time Slot List: Scrollable hourly slots (8 AM - 8 PM, 44px each)
    /// - "Select Time" header above time slots
    /// - Scroll indicator if slots exceed visible area
    /// - Two separate hover zones (calendar vs time slots)
    /// - Two-step: Select date from calendar, then time from list
    /// - Appointment-specific hourly granularity (full hour slots)
    /// - Rounded rectangle styling for selected time slots
    /// </summary>
    internal class AppointmentDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        private TimeSpan? _selectedTime;
        private int _scrollOffset = 0; // For future scroll support
        
        public DatePickerMode Mode => DatePickerMode.Appointment;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();

            // Calculate split panel bounds (painter uses 55% / 45% split)
            var totalBounds = GetTotalBounds(layout);
            int calendarWidth = (int)(totalBounds.Width * 0.55f);
            int timeSlotWidth = totalBounds.Width - calendarWidth - 1;

            var calendarBounds = new Rectangle(totalBounds.X, totalBounds.Y, calendarWidth, totalBounds.Height);
            var timeSlotBounds = new Rectangle(totalBounds.X + calendarWidth + 1, totalBounds.Y, timeSlotWidth, totalBounds.Height);

            // Determine which panel the click is in
            bool isInCalendarPanel = calendarBounds.Contains(location);
            bool isInTimeSlotPanel = timeSlotBounds.Contains(location);

            // CALENDAR PANEL (left side)
            if (isInCalendarPanel)
            {
                // Test navigation buttons
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

                // Test calendar day cells
                if (!layout.CalendarGridRect.IsEmpty && layout.CalendarGridRect.Contains(location))
                {
                    var cells = layout.GetDayCellMatrixOrDefault(6, 7);

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
            }
            // TIME SLOT PANEL (right side)
            else if (isInTimeSlotPanel)
            {
                // Check time control spinners first (at bottom of time panel)
                int timeControlsHeight = 100;
                var timeControlBounds = new Rectangle(
                    timeSlotBounds.X,
                    timeSlotBounds.Bottom - timeControlsHeight,
                    timeSlotBounds.Width,
                    90
                );

                if (timeControlBounds.Contains(location))
                {
                    // Check hour/minute spinner buttons using layout rectangles
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
                }

                // Calculate time slot rectangles (hourly from 8 AM to 8 PM)
                // Painter: padding=12, header=32, slotHeight=44, slotGap=4
                int padding = 12;
                int headerHeight = 32;
                int slotHeight = 44;
                int slotGap = 4;
                int startY = timeSlotBounds.Y + padding + headerHeight;

                for (int hour = 8; hour <= 20; hour++)
                {
                    var slotBounds = new Rectangle(
                        timeSlotBounds.X + padding,
                        startY + (hour - 8) * slotHeight,
                        timeSlotBounds.Width - padding * 2,
                        slotHeight - slotGap
                    );

                    // Stop if slot exceeds visible area (leave room for time controls)
                    if (slotBounds.Bottom > timeSlotBounds.Bottom - timeControlsHeight)
                        break;

                    if (slotBounds.Contains(location))
                    {
                        var time = new TimeSpan(hour, 0, 0);
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.TimeSlot;
                        result.Time = time;
                        result.HitBounds = slotBounds;
                        result.TimeSlotIndex = hour - 8;
                        return result;
                    }
                }
            }

            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit)
                return false;

            // Handle navigation buttons (calendar panel)
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

            // Handle date selection (calendar panel) - Step 1
            if (hitResult.Date.HasValue)
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

                // Select date, keep existing time if any
                _selectedDate = date;
                SyncToControl(owner);
                
                // Don't close - wait for time slot selection
                return false;
            }

            // Handle time slot selection (time panel) - Step 2
            if (hitResult.Time.HasValue)
            {
                // Auto-select today if no date selected yet
                if (!_selectedDate.HasValue)
                {
                    _selectedDate = DateTime.Today;
                }
                
                _selectedTime = hitResult.Time.Value;
                SyncToControl(owner);
                
                // Both date and time selected - close for appointment booking
                return true;
            }

            // Handle time control spinner button clicks
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
                int interval = props?.TimeInterval.Minutes ?? 15; // Default 15 min for appointments
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
                int interval = props?.TimeInterval.Minutes ?? 15; // Default 15 min for appointments
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

            // Hover on day cell (calendar panel)
            if (hitResult.Date.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on time slot (time slot panel) - separate zone
            else if (hitResult.Time.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.TimeSlot;
                hoverState.HoveredTime = hitResult.Time.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on time control spinner buttons
            else if (hitResult.HitArea == DateTimePickerHitArea.StartHourUpButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartHourDownButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartMinuteUpButton ||
                     hitResult.HitArea == DateTimePickerHitArea.StartMinuteDownButton)
            {
                hoverState.HoverArea = hitResult.HitArea;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on navigation buttons
            else if (hitResult.HitArea == DateTimePickerHitArea.PreviousButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.PreviousButton;
                hoverState.HoveredButton = hitResult.HitArea.ToString();
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea == DateTimePickerHitArea.NextButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.NextButton;
                hoverState.HoveredButton = hitResult.HitArea.ToString();
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
            // Both date AND time must be selected for appointment
            return _selectedDate.HasValue && _selectedTime.HasValue;
        }

        public void Reset()
        {
            _selectedDate = null;
            _selectedTime = null;
            _scrollOffset = 0;
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
        /// Get total bounds from layout to calculate split panels
        /// </summary>
        private Rectangle GetTotalBounds(DateTimePickerLayout layout)
        {
            // Use calendar grid as reference for total bounds
            // The painter splits from the full control bounds
            // We'll estimate from header and grid
            if (!layout.HeaderRect.IsEmpty && !layout.CalendarGridRect.IsEmpty)
            {
                int x = Math.Min(layout.HeaderRect.X, layout.CalendarGridRect.X);
                int y = Math.Min(layout.HeaderRect.Y, layout.CalendarGridRect.Y);
                int right = Math.Max(layout.HeaderRect.Right, layout.CalendarGridRect.Right);
                int bottom = layout.CalendarGridRect.Bottom;
                
                // Extend to reasonable appointment picker size
                int width = Math.Max(right - x, 500);
                int height = Math.Max(bottom - y, 350);
                
                return new Rectangle(x, y, width, height);
            }
            
            // Fallback
            return new Rectangle(0, 0, 500, 350);
        }
    }
}
