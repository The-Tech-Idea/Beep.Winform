using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Header DateTimePicker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - Large prominent colored header (Height=80) displaying selected date
    /// - Header shows "Friday, April 12" and year in white text on accent color
    /// - Clean compact calendar grid below header
    /// - Minimal calendar styling (subtle month/year header, simple grid)
    /// - Inspired by modern mobile date picker designs (Material Design Style)
    /// 
    /// INTERACTION MODEL:
    /// 1. Single-click date selection, immediate close
    /// 2. Header is non-interactive (display only)
    /// 3. Calendar grid navigation and selection below header
    /// 
    /// LAYOUT STRUCTURE:
    /// - Header: Y=0, Height=80, full width, accent color background
    /// - Calendar section: Y=80+padding, compact layout below
    /// - Padding: 8px between sections
    /// </summary>
    internal class HeaderDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        
        public DatePickerMode Mode => DatePickerMode.Header;
        
        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            
            // Navigation buttons
            if (!layout.PreviousButtonRect.IsEmpty && layout.PreviousButtonRect.Contains(location))
            {
                result.IsHit = true;
                result.HitArea =  DateTimePickerHitArea.PreviousButton;
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
            
            // Calendar grid (below header)
            if (!layout.CalendarGridRect.IsEmpty && layout.CalendarGridRect.Contains(location))
            {
                var cells = layout.DayCellMatrix ?? (layout.DayCellRects?.Count == 42 ? ConvertToCellMatrix(layout.DayCellRects) : null);
                
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
                                result.HitArea =  DateTimePickerHitArea.TimeSlot;
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
            
            // Date selection
            if (hitResult.Date.HasValue)
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
            
            if (hitResult.Date.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.DayCell;
                hoverState.HoveredDate = hitResult.Date.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
            else if (hitResult.HitArea ==  DateTimePickerHitArea.PreviousButton)
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
        
        private Rectangle[,] ConvertToCellMatrix(System.Collections.Generic.List<Rectangle> rects)
        {
            var cells = new Rectangle[6, 7];
            for (int i = 0; i < 42; i++)
                cells[i / 7, i % 7] = rects[i];
            return cells;
        }
        
        private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties properties)
        {
            DateTime firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)properties.FirstDayOfWeek + 7) % 7;
            return firstDayOfMonth.AddDays(row * 7 + col - offset);
        }
    }
}
