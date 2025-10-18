using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Compact Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - Minimal chrome with tight spacing (smaller cells, reduced padding)
    /// - No outer border (BaseControl handles border if needed)
    /// - Smaller navigation buttons (24x24 vs standard 32x32)
    /// - Compact header with smaller font (10pt)
    /// - Optional "Today" button at bottom for quick selection
    /// - Optimized for dropdown scenarios and small spaces
    /// 
    /// INTERACTION MODEL:
    /// 1. Single-click date selection, immediate close
    /// 2. Today button (if visible) → select today, close
    /// 3. Smaller hit zones require more precise clicking
    /// 
    /// LAYOUT STRUCTURE:
    /// - Tighter padding: 6px (vs standard 16px)
    /// - Smaller cells: calculated for compact display
    /// - Today button: Optional, at bottom if ShowTodayButton=true
    /// </summary>
    internal class CompactDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        public DatePickerMode Mode => DatePickerMode.Compact;

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
            
            // Today button (optional)
            if (!layout.TodayButtonRect.IsEmpty && layout.TodayButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea = DateTimePickerHitArea.TodayButton;
                result.HitBounds = layout.TodayButtonRect;
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
            
            // Today button
            if (hitResult.HitArea == DateTimePickerHitArea.TodayButton)
            {
                _selectedDate = DateTime.Today;
                SyncToControl(owner);
                return true;  // Close on Today button
            }
            
            // Date selection
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell && hitResult.Date.HasValue)
            {
                if (!owner.IsDateInRange(hitResult.Date.Value.Date))
                    return false;

                _selectedDate = hitResult.Date.Value.Date;
                SyncToControl(owner);
                return true;  // Close on selection
            }
            
            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date;
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

        private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties properties)
        {
            DateTime firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            DayOfWeek firstDayOfWeek = (DayOfWeek)(int)properties.FirstDayOfWeek;
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
            int dayOffset = row * 7 + col - offset;
            return firstDayOfMonth.AddDays(dayOffset);
        }
    }
}
