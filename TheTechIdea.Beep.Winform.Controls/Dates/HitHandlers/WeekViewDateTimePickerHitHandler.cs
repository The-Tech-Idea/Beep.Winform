using System;
using System.Drawing;
using System.Globalization;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    /// <summary>
    /// Week View Date Picker Hit Handler
    /// 
    /// UNIQUE DESIGN:
    /// - Week number column on left (40px wide, X+16, offset +44 for calendar)
    /// - Clicking week number OR any day in row selects entire week (7 days)
    /// - Full row highlighting on hover
    /// - Selected week info displayed below calendar (weekInfoRect: Height=32, Y=CalendarBottom+12)
    /// - Week calculation uses CultureInfo.Calendar.GetWeekOfYear()
    /// - RangeStartDate/EndDate represent week span (StartDate = Monday, EndDate = Sunday)
    /// 
    /// INTERACTION MODEL:
    /// 1. Hover over week number or any day → entire row highlights
    /// 2. Click anywhere in row → select full week (7 days), show week info
    /// 3. Close on selection (IsSelectionComplete = true)
    /// 
    /// LAYOUT:
    /// - Week column: X+16, Width=40px, starts at DayNamesRect.Y
    /// - Calendar offset: +44px to account for week column
    /// - 6 rows for weeks, cellHeight = (CalendarGridRect.Height - 28) / 6
    /// </summary>
    internal class WeekViewDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _weekStartDate;  // Monday of selected week
        private DateTime? _weekEndDate;    // Sunday of selected week
        public DatePickerMode Mode => DatePickerMode.WeekView;

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

            // Week number column detection (40px wide, X+16)
            var weekColumnBounds = new Rectangle(
                layout.CalendarGridRect.X - 44 + 16,  // Account for offset
                layout.DayNamesRect.Y,
                40,
                layout.CalendarGridRect.Height + 28
            );

            if (weekColumnBounds.Contains(location))
            {
                // Determine which week row was clicked
                int cellHeight = (layout.CalendarGridRect.Height - 28) / 6;
                int currentY = layout.DayNamesRect.Y + 28;
                
                for (int week = 0; week < 6; week++)
                {
                    var weekRect = new Rectangle(weekColumnBounds.X, currentY, weekColumnBounds.Width, cellHeight);
                    if (weekRect.Contains(location))
                    {
                        // Get first date of this week row
                        DateTime weekDate = GetDateForCell(displayMonth, week, 0, properties);
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.WeekNumber;
                        result.Date = weekDate;
                        result.CellIndex = week;
                        result.HitBounds = weekRect;
                        return result;
                    }
                    currentY += cellHeight;
                }
            }

            // Calendar grid - detect entire week row
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
                    // Find which row was clicked (any cell in row selects entire week)
                    for (int row = 0; row < 6; row++)
                    {
                        for (int col = 0; col < 7; col++)
                        {
                            var cellRect = cells[row, col];
                            if (cellRect.Contains(location))
                            {
                                // Get first date of this week
                                DateTime weekStartDate = GetDateForCell(displayMonth, row, 0, properties);
                                result.IsHit = true;
                                result.HitArea = DateTimePickerHitArea.WeekRow;
                                result.Date = weekStartDate;
                                result.CellIndex = row;
                                result.HitBounds = GetWeekRowBounds(cells, row);
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
                owner.NavigateToPreviousMonth();  // Navigate by month in week view
                return false;
            }
            if (hitResult.HitArea == DateTimePickerHitArea.NextButton)
            {
                owner.NavigateToNextMonth();
                return false;
            }
            
            // Week selection (from week number column or calendar row)
            if ((hitResult.HitArea == DateTimePickerHitArea.WeekNumber || hitResult.HitArea == DateTimePickerHitArea.WeekRow) && hitResult.Date.HasValue)
            {
                // Calculate week start (find Monday of this week)
                DateTime clickedDate = hitResult.Date.Value;
                DateTime weekStart = GetWeekStart(clickedDate);
                DateTime weekEnd = weekStart.AddDays(6);
                
                // Validate against MinDate/MaxDate
                if (!owner.IsDateInRange(weekStart) || !owner.IsDateInRange(weekEnd))
                    return false;
                
                _weekStartDate = weekStart;
                _weekEndDate = weekEnd;
                SyncToControl(owner);
                return true;  // Close on selection
            }
            
            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            
            // Week row hover (highlight entire row)
            if ((hitResult.HitArea == DateTimePickerHitArea.WeekNumber || hitResult.HitArea == DateTimePickerHitArea.WeekRow) && hitResult.Date.HasValue)
            {
                hoverState.HoverArea = hitResult.HitArea;
                hoverState.HoveredDate = hitResult.Date;  // First day of week
                hoverState.HoverBounds = hitResult.HitBounds;   // Entire row bounds
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
            // Week view uses RangeStartDate/EndDate for week span
            _weekStartDate = owner.RangeStartDate;
            _weekEndDate = owner.RangeEndDate;
        }

        public void SyncToControl(BeepDateTimePicker owner)
        {
            // Set week as range (Monday to Sunday)
            owner.RangeStartDate = _weekStartDate;
            owner.RangeEndDate = _weekEndDate;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
            owner.SelectedDate = _weekStartDate;  // Set SelectedDate to week start
            owner.SelectedTime = null;
        }

        public bool IsSelectionComplete()
        {
            return _weekStartDate.HasValue && _weekEndDate.HasValue;
        }

        public void Reset()
        {
            _weekStartDate = null;
            _weekEndDate = null;
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
        /// Get the bounds for an entire week row (all 7 cells)
        /// </summary>
        private Rectangle GetWeekRowBounds(Rectangle[,] cells, int row)
        {
            if (cells == null || row < 0 || row >= 6) 
                return Rectangle.Empty;
            
            var firstCell = cells[row, 0];
            var lastCell = cells[row, 6];
            
            return new Rectangle(
                firstCell.X,
                firstCell.Y,
                lastCell.Right - firstCell.X,
                firstCell.Height
            );
        }

        /// <summary>
        /// Get the Monday (start) of the week containing the given date
        /// </summary>
        private DateTime GetWeekStart(DateTime date)
        {
            // Find the previous Monday (or same day if already Monday)
            int daysFromMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-daysFromMonday).Date;
        }

        /// <summary>
        /// Get week number for the given date using current culture
        /// </summary>
        private int GetWeekNumber(DateTime date)
        {
            var culture = CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}
