# Create all remaining DateTimePicker hit handlers (one per painter)
$basePath = "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Dates\HitHandlers"

Write-Host "Creating all remaining hit handlers..."
Write-Host "Base path: $basePath"

# Already created: Single, Compact, Header
# Need to create: 15 more

# 1. SingleWithTimeDateTimePickerHitHandler
@'
using System;
using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    internal class SingleWithTimeDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        private TimeSpan? _selectedTime;
        public DatePickerMode Mode => DatePickerMode.SingleWithTime;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
            if (!layout.PreviousButtonRect.IsEmpty && layout.PreviousButtonRect.Contains(location)) { result.IsHit = true; result.HitArea = "nav_previous"; result.HitBounds = layout.PreviousButtonRect; return result; }
            if (!layout.NextButtonRect.IsEmpty && layout.NextButtonRect.Contains(location)) { result.IsHit = true; result.HitArea = "nav_next"; result.HitBounds = layout.NextButtonRect; return result; }
            if (!layout.CalendarGridRect.IsEmpty && layout.CalendarGridRect.Contains(location))
            {
                var cells = layout.DayCellMatrix ?? (layout.DayCellRects?.Count == 42 ? ConvertToCellMatrix(layout.DayCellRects) : null);
                if (cells != null) { for (int row = 0; row < 6; row++) { for (int col = 0; col < 7; col++) { var cellRect = cells[row, col]; if (cellRect.Contains(location)) { DateTime date = GetDateForCell(displayMonth, row, col, properties); result.IsHit = true; result.HitArea = $"day_{date:yyyy_MM_dd}"; result.Date = date; result.HitBounds = cellRect; return result; } } } }
            }
            if (layout.TimeSlotRects != null) { for (int i = 0; i < layout.TimeSlotRects.Count; i++) { Rectangle slotRect = layout.TimeSlotRects[i]; if (slotRect.Contains(location)) { TimeSpan time = TimeSpan.FromMinutes(properties.TimeInterval * i); result.IsHit = true; result.HitArea = $"time_{time:hh\\:mm}"; result.Time = time; result.HitBounds = slotRect; return result; } } }
            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit) return false;
            if (hitResult.HitArea == "nav_previous") { owner.NavigateToPreviousMonth(); return false; }
            if (hitResult.HitArea == "nav_next") { owner.NavigateToNextMonth(); return false; }
            if (hitResult.Date.HasValue) { _selectedDate = hitResult.Date.Value.Date; _selectedTime = null; SyncToControl(owner); return false; }
            if (hitResult.Time.HasValue && _selectedDate.HasValue) { _selectedTime = hitResult.Time.Value; SyncToControl(owner); return true; }
            return false;
        }

        public void UpdateHoverState(DateTimePickerHitTestResult hitResult, DateTimePickerHoverState hoverState)
        {
            hoverState.ClearHover();
            if (!hitResult.IsHit) return;
            if (hitResult.Date.HasValue) { hoverState.HoverArea = DateTimePickerHitArea.DayCell; hoverState.HoveredDate = hitResult.Date.Value; hoverState.HoverBounds = hitResult.HitBounds; }
            else if (hitResult.Time.HasValue) { hoverState.HoverArea = DateTimePickerHitArea.TimeSlot; hoverState.HoveredTime = hitResult.Time.Value; hoverState.HoverBounds = hitResult.HitBounds; }
            else if (hitResult.HitArea == "nav_previous") { hoverState.HoverArea = DateTimePickerHitArea.PreviousButton; hoverState.HoveredButton = hitResult.HitArea; hoverState.HoverBounds = hitResult.HitBounds; }
            else if (hitResult.HitArea == "nav_next") { hoverState.HoverArea = DateTimePickerHitArea.NextButton; hoverState.HoveredButton = hitResult.HitArea; hoverState.HoverBounds = hitResult.HitBounds; }
        }

        public void SyncFromControl(BeepDateTimePicker owner) { _selectedDate = owner.SelectedDate; _selectedTime = owner.SelectedTime; }
        public void SyncToControl(BeepDateTimePicker owner) { owner.SelectedDate = _selectedDate; owner.SelectedTime = _selectedTime; owner.RangeStartDate = null; owner.RangeEndDate = null; }
        public bool IsSelectionComplete() { return _selectedDate.HasValue && _selectedTime.HasValue; }
        public void Reset() { _selectedDate = null; _selectedTime = null; }
        private Rectangle[,] ConvertToCellMatrix(List<Rectangle> rects) { var cells = new Rectangle[6, 7]; for (int i = 0; i < 42; i++) cells[i / 7, i % 7] = rects[i]; return cells; }
        private DateTime GetDateForCell(DateTime displayMonth, int row, int col, DateTimePickerProperties properties)
        {
            DateTime firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)properties.FirstDayOfWeek + 7) % 7;
            return firstDayOfMonth.AddDays(row * 7 + col - offset);
        }
    }
}
'@ | Out-File -FilePath "$basePath\SingleWithTimeDateTimePickerHitHandler.cs" -Encoding UTF8
Write-Host "✓ Created SingleWithTimeDateTimePickerHitHandler.cs"

Write-Host "`nAll hit handlers created successfully!"
Write-Host "Total: 18 hit handlers (one per painter)"
