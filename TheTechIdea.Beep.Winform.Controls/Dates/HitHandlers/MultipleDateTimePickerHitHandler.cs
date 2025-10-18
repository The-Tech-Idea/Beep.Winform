using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Hit handler for MultipleDateTimePickerPainter
    /// - Calendar with checkmarks for multiple date selection
    /// - Toggle selection model (click to add/remove dates)
    /// - HashSet for efficient date lookups and storage
    /// - Selection count display at bottom
    /// - Clear Selection button to reset all selections
    /// - Doesn't close until explicit action (no auto-close)
    /// - Day cells show checkmarks when selected
    /// - Rounded square highlighting for selected dates
    /// </summary>
    internal class MultipleDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private HashSet<DateTime> _selectedDates = new HashSet<DateTime>();
        
        public DatePickerMode Mode => DatePickerMode.Multiple;

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

            // Test Clear Selection button (below calendar)
            if (!layout.ClearButtonRect.IsEmpty && layout.ClearButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.ClearButton;
                result.HitBounds = layout.ClearButtonRect;
                return result;
            }

            // Test calendar day cells with checkboxes
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
                            if (!cellRect.IsEmpty && cellRect.Contains(location))
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

            // Handle Clear Selection button
            if (hitResult.HitArea == DateTimePickerHitArea.ClearButton)
            {
                _selectedDates.Clear();
                SyncToControl(owner);
                return false; // Don't close - let user continue selecting
            }

            // Handle day cell click - TOGGLE selection
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell && hitResult.Date.HasValue)
            {
                var clicked = hitResult.Date.Value.Date;
                
                // Check if date belongs to current display month
                // (previous/next month dates are disabled)
                if (clicked.Year != owner.DisplayMonth.Year || clicked.Month != owner.DisplayMonth.Month)
                {
                    return false; // Don't select dates from other months
                }
                
                // Check if date is disabled
                var props = owner.GetCurrentProperties();
                if (props != null)
                {
                    if (props.MinDate.HasValue && clicked < props.MinDate.Value)
                        return false;
                    if (props.MaxDate.HasValue && clicked > props.MaxDate.Value)
                        return false;
                }

                // Toggle: if already selected, remove it; otherwise add it
                if (_selectedDates.Contains(clicked))
                {
                    _selectedDates.Remove(clicked);
                }
                else
                {
                    _selectedDates.Add(clicked);
                }
                
                SyncToControl(owner);
                
                // Multiple mode doesn't auto-close - user keeps selecting
                return false;
            }

            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            
            if (!hitResult.IsHit)
                return;

            // Hover on day cell (shows checkmark on selected)
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            // Hover on Clear button
            else if (hitResult.HitArea == DateTimePickerHitArea.ClearButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.ClearButton;
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
            if (owner.SelectedDates != null)
            {
                _selectedDates = new HashSet<DateTime>(owner.SelectedDates.Select(d => d.Date));
            }
            else
            {
                _selectedDates.Clear();
            }
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Update SelectedDates list (sorted chronologically)
            owner.SelectedDates = _selectedDates.OrderBy(d => d).ToList();
            
            // Clear other selection modes
            owner.SelectedDate = null;
            owner.RangeStartDate = null;
            owner.RangeEndDate = null;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
        }

        public bool IsSelectionComplete()
        {
            // At least one date must be selected
            return _selectedDates.Count > 0;
        }

        public void Reset()
        {
            _selectedDates.Clear();
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
    }
}
