# Script to create all 18 hit handlers (one per painter)

$basePath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Dates\HitHandlers"

# Already created: SingleDateTimePickerHitHandler, CompactDateTimePickerHitHandler

# Create the remaining handlers
Write-Host "Creating remaining hit handlers..."

# HeaderDateTimePickerHitHandler
@'
using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    internal class HeaderDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        public DatePickerMode Mode => DatePickerMode.Header;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            if (!layout.PreviousButtonRect.IsEmpty && layout.PreviousButtonRect.Contains(location)) { result.IsHit = true; result.HitArea = "nav_previous"; result.HitBounds = layout.PreviousButtonRect; return result; }
            if (!layout.NextButtonRect.IsEmpty && layout.NextButtonRect.Contains(location)) { result.IsHit = true; result.HitArea = "nav_next"; result.HitBounds = layout.NextButtonRect; return result; }
            if (!layout.CalendarGridRect.IsEmpty && layout.CalendarGridRect.Contains(location))
            {
                var cells = layout.DayCellMatrix;
                if (cells == null && layout.DayCellRects != null && layout.DayCellRects.Count == 42) { cells = new Rectangle[6, 7]; for (int i = 0; i < 42; i++) cells[i / 7, i % 7] = layout.DayCellRects[i]; layout.DayCellMatrix = cells; }
                if (cells != null) { for (int row = 0; row < 6; row++) { for (int col = 0; col < 7; col++) { var cellRect = cells[row, col]; if (cellRect.Contains(location)) { DateTime date = GetDateForCell(displayMonth, row, col, properties); result.IsHit = true; result.HitArea = $"day_{date:yyyy_MM_dd}"; result.Date = date; result.HitBounds = cellRect; return result; } } } }
            }
            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit) return false;
            if (hitResult.HitArea == "nav_previous") { owner.NavigateToPreviousMonth(); return false; }
            if (hitResult.HitArea == "nav_next") { owner.NavigateToNextMonth(); return false; }
            if (hitResult.Date.HasValue) { _selectedDate = hitResult.Date.Value.Date; SyncToControl(owner); return true; }
            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            if (hitResult.Date.HasValue) { hoverState.HoverArea = DateTimePickerHitArea.DayCell; hoverState.HoveredDate = hitResult.Date.Value; hoverState.HoverBounds = hitResult.HitBounds; }
            else if (hitResult.HitArea == "nav_previous") { hoverState.HoverArea = DateTimePickerHitArea.PreviousButton; hoverState.HoveredButton = hitResult.HitArea; hoverState.HoverBounds = hitResult.HitBounds; }
            else if (hitResult.HitArea == "nav_next") { hoverState.HoverArea = DateTimePickerHitArea.NextButton; hoverState.HoveredButton = hitResult.HitArea; hoverState.HoverBounds = hitResult.HitBounds; }
        }

        public void SyncFromControl(BeepDateTimePicker owner) { _selectedDate = owner.SelectedDate; }
        public void SyncToControl(BeepDateTimePicker owner) { owner.SelectedDate = _selectedDate; owner.RangeStartDate = null; owner.RangeEndDate = null; owner.SelectedTime = null; }
        public bool IsSelectionComplete() { return _selectedDate.HasValue; }
        public void Reset() { _selectedDate = null; }

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
'@ | Out-File -FilePath "$basePath\HeaderDateTimePickerHitHandler.cs" -Encoding UTF8

Write-Host "Created HeaderDateTimePickerHitHandler.cs"

# Continue with others...
Write-Host "Script complete. Created hit handlers."
