using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Hit handler for SingleDateTimePickerPainter
    /// 
    /// Layout Understanding:
    /// - Standard calendar grid (7×6 cells)
    /// - Navigation buttons (prev/next month)
    /// - Optional today button
    /// 
    /// Hit Test Logic:
    /// - Priority 1: Navigation buttons
    /// - Priority 2: Day cells (with date calculation)
    /// - Priority 3: Today button (if enabled)
    /// 
    /// Click Behavior:
    /// - Navigation: Change month, stay open
    /// - Day cell: Select date, close immediately
    /// - Today button: Select today's date, close immediately
    /// 
    /// Date Calculation:
    /// - Uses FirstDayOfWeek property
    /// - Calculates dates for previous/current/next month cells
    /// - Respects MinDate/MaxDate constraints
    /// </summary>
    internal class SingleDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        
        public DatePickerMode Mode => DatePickerMode.Single;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();

            // Test navigation buttons first (top priority)
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

            // Test day cells in the calendar grid (7x6 matrix)
            if (!layout.CalendarGridRect.IsEmpty && layout.CalendarGridRect.Contains(location))
            {
                var cells = layout.DayCellMatrix;
                
                // Fallback: build matrix from list if needed
                if (cells == null && layout.DayCellRects != null && layout.DayCellRects.Count == 42)
                {
                    cells = new Rectangle[6, 7];
                    for (int i = 0; i < 42; i++)
                    {
                        cells[i / 7, i % 7] = layout.DayCellRects[i];
                    }
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
                                result.CellIndex = row * 7 + col;
                                result.HitBounds = cellRect;
                                return result;
                            }
                        }
                    }
                }
            }

            // Test today button (if enabled)
            if (properties.ShowTodayButton && !layout.TodayButtonRect.IsEmpty && layout.TodayButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.TodayButton;
                result.HitBounds = layout.TodayButtonRect;
                result.Date = DateTime.Today;
                return result;
            }

            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit)
                return false;

            // Handle navigation button clicks
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

            // Handle today button click
            if (hitResult.HitArea == DateTimePickerHitArea.TodayButton)
            {
                _selectedDate = DateTime.Today;
                SyncToControl(owner);
                return true; // Close immediately
            }

            // Handle day cell clicks
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

                // Set selected date
                _selectedDate = date;
                SyncToControl(owner);
                
                // Single mode closes immediately on selection
                return true;
            }

            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            
            if (!hitResult.IsHit)
                return;

            // Hover on day cell - show circular highlight
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on previous button
            else if (hitResult.HitArea == DateTimePickerHitArea.PreviousButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.PreviousButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on next button
            else if (hitResult.HitArea == DateTimePickerHitArea.NextButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.NextButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on today button
            else if (hitResult.HitArea == DateTimePickerHitArea.TodayButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.TodayButton;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
        }

        public void SyncFromControl(BeepDateTimePicker owner)
        {
            _selectedDate = owner.SelectedDate;
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            owner.SelectedDate = _selectedDate;
            
            // Clear other selection modes
            owner.RangeStartDate = null;
            owner.RangeEndDate = null;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
            owner.SelectedTime = null;
        }

        public bool IsSelectionComplete()
        {
            return _selectedDate.HasValue;
        }

        public void Reset()
        {
            _selectedDate = null;
        }

        /// <summary>
        /// Calculate the date for a specific cell in the grid
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
    }
}
