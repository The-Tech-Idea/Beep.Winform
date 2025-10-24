using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Hit handler for ModernCardDateTimePickerPainter
    /// - Card-Style UI with shadow and rounded corners
    /// - Quick date buttons at top (Today, Tomorrow, etc.) in 2x2 grid
    /// - Uses DateTimePickerQuickButtonHelper for consistent button definitions
    /// - Calendar below separator for manual selection
    /// - Quick buttons provide immediate selection and close
    /// - Two interaction modes: quick (buttons) vs manual (calendar)
    /// </summary>
    internal class ModernCardDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        
        public DatePickerMode Mode => DatePickerMode.ModernCard;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();

            // Test quick date buttons first (top priority, at top of card)
            if (layout.QuickDateButtons != null && layout.QuickDateButtons.Count > 0)
            {
                // Get button definitions from helper to match painter's logic
                var buttonDefs = DateTimePickerQuickButtonHelper.GetQuickButtonDefinitions(properties);
                
                for (int i = 0; i < Math.Min(layout.QuickDateButtons.Count, buttonDefs.Count); i++)
                {
                    var btn = layout.QuickDateButtons[i];
                    if (btn.Bounds.Contains(location))
                    {
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.QuickButton;
                        result.Date = buttonDefs[i].TargetDate; // Store target date
                        result.HitBounds = btn.Bounds;
                        return result;
                    }
                }
            }

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

            // Handle quick date buttons - these provide immediate selection
            if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && hitResult.Date.HasValue)
            {
                _selectedDate = hitResult.Date.Value.Date;
                SyncToControl(owner);
                return true; // Quick buttons close immediately
            }
            
            // Handle day cell clicks
            if (hitResult.HitArea == DateTimePickerHitArea.DayCell && hitResult.Date.HasValue)
            {
                _selectedDate = hitResult.Date.Value.Date;
                SyncToControl(owner);
                return true;
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
            else if (hitResult.HitArea == DateTimePickerHitArea.QuickButton)
            {
                hoverState.HoverArea = DateTimePickerHitArea.QuickButton;
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
