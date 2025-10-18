using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers
{
    internal class SidebarEventDateTimePickerHitHandler : IDateTimePickerHitHandler
    {
        private DateTime? _selectedDate;
        private TimeSpan? _selectedTime;
        public DatePickerMode Mode => DatePickerMode.SidebarEvent;

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties)
        {
            var result = new DateTimePickerHitTestResult();
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
            if (layout.TimeSlotRects != null)
            {
                for (int i = 0; i < layout.TimeSlotRects.Count; i++)
                {
                    Rectangle slotRect = layout.TimeSlotRects[i];
                    if (slotRect.Contains(location))
                    {
                        // Compute time from interval
                        double minutesInterval = properties.TimeInterval.TotalMinutes;
                        TimeSpan time = TimeSpan.FromMinutes(minutesInterval * i);
                        result.IsHit = true;
                        result.HitArea = DateTimePickerHitArea.TimeSlot;
                        result.Time = time;
                        result.HitBounds = slotRect;
                        result.TimeSlotIndex = i;
                        return result;
                    }
                }
            }
            return result;
        }

        public bool HandleClick(DateTimePickerHitTestResult hitResult, BeepDateTimePicker owner)
        {
            if (!hitResult.IsHit) return false;
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
            if (hitResult.Date.HasValue)
            {
                _selectedDate = hitResult.Date.Value.Date;
                _selectedTime = null;
                SyncToControl(owner);
                return false;
            }
            if (hitResult.Time.HasValue && _selectedDate.HasValue)
            {
                _selectedTime = hitResult.Time.Value;
                SyncToControl(owner);
                return true;
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
            else if (hitResult.Time.HasValue)
            {
                hoverState.HoverArea = DateTimePickerHitArea.TimeSlot;
                hoverState.HoveredTime = hitResult.Time.Value;
                hoverState.HoverBounds = hitResult.HitBounds;
            }
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
            owner.RangeStartDate = null;
            owner.RangeEndDate = null;
            owner.RangeStartTime = null;
            owner.RangeEndTime = null;
        }

        public bool IsSelectionComplete()
        {
            return _selectedDate.HasValue && _selectedTime.HasValue;
        }

        public void Reset()
        {
            _selectedDate = null;
            _selectedTime = null;
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
