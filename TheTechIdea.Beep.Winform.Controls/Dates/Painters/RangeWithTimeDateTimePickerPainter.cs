using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Painters
{
    /// <summary>
    /// Range With Time Date Picker Painter
    /// Select start and end dates with separate time pickers for each
    /// Visual styling follows BeepTheme
    /// </summary>
    public class RangeWithTimeDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.RangeWithTime;

        public RangeWithTimeDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var layout = CalculateLayout(bounds, properties);

            // Paint background
            PaintBackground(g, bounds);

            // Paint header
            PaintHeader(g, layout.HeaderRect, displayMonth.ToString("MMMM yyyy"), true, false);
            
            PaintNavigationButton(g, layout.PreviousButtonRect, false, 
                hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));
            
            PaintNavigationButton(g, layout.NextButtonRect, true,
                hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));

            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);

            // Paint dual time pickers at bottom
            PaintDualTimePickers(g, bounds, hoverState);
        }

        private void PaintBackground(Graphics g, Rectangle bounds)
        {
            var bgColor = _theme?.BackgroundColor ?? Color.White;
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(200, 200, 200);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }

        private void PaintDualTimePickers(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(100, 100, 100);
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 9f, FontStyle.Bold);

            int timePickerTop = bounds.Bottom - 100;
            int padding = 16;
            int halfWidth = (bounds.Width - padding * 3) / 2;

            // Separator line
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, bounds.X + padding, timePickerTop, bounds.Right - padding, timePickerTop);
            }

            // Start time picker (left)
            var startRect = new Rectangle(bounds.X + padding, timePickerTop + 12, halfWidth, 80);
            PaintTimePickerSection(g, startRect, "Start Time", _owner.RangeStartDate, hoverState, true);

            // End time picker (right)
            var endRect = new Rectangle(bounds.X + padding * 2 + halfWidth, timePickerTop + 12, halfWidth, 80);
            PaintTimePickerSection(g, endRect, "End Time", _owner.RangeEndDate, hoverState, false);
        }

        private void PaintTimePickerSection(Graphics g, Rectangle bounds, string label, DateTime? date, DateTimePickerHoverState hoverState, bool isStart)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(100, 100, 100);
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 8.5f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 9f, FontStyle.Bold);

            // Label
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                g.DrawString(label, font, brush, bounds.X, bounds.Y);
            }

            // Time display/selector
            var timeRect = new Rectangle(bounds.X, bounds.Y + 22, bounds.Width, 54);
            
            if (!date.HasValue)
            {
                // Placeholder
                using (var brush = new SolidBrush(Color.FromArgb(180, 180, 180)))
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawRectangle(pen, timeRect);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("--:--", font, brush, timeRect, format);
                }
            }
            else
            {
                // Draw time with hour/minute spinners
                int spinnerWidth = (timeRect.Width - 20) / 2;
                
                // Hour spinner
                var hourRect = new Rectangle(timeRect.X, timeRect.Y, spinnerWidth, timeRect.Height);
                PaintTimeSpinner(g, hourRect, date.Value.Hour.ToString("D2"), "HH", isStart, true);

                // Colon separator
                using (var brush = new SolidBrush(textColor))
                {
                    var colonRect = new Rectangle(hourRect.Right, timeRect.Y, 20, timeRect.Height);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(":", boldFont, brush, colonRect, format);
                }

                // Minute spinner
                var minuteRect = new Rectangle(hourRect.Right + 20, timeRect.Y, spinnerWidth, timeRect.Height);
                PaintTimeSpinner(g, minuteRect, date.Value.Minute.ToString("D2"), "MM", isStart, false);
            }
        }

        private void PaintTimeSpinner(Graphics g, Rectangle bounds, string value, string label, bool isStart, bool isHour)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(120, 120, 120);
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);
            var smallFont = new Font("Segoe UI", 7f);

            // Border
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Up arrow button
            var upRect = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, 16);
            using (var pen = new Pen(secondaryTextColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int cx = upRect.X + upRect.Width / 2;
                int cy = upRect.Y + upRect.Height / 2;
                g.DrawLine(pen, cx - 4, cy + 2, cx, cy - 2);
                g.DrawLine(pen, cx, cy - 2, cx + 4, cy + 2);
            }

            // Value
            var valueRect = new Rectangle(bounds.X, bounds.Y + 18, bounds.Width, 18);
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(value, font, brush, valueRect, format);
            }

            // Down arrow button
            var downRect = new Rectangle(bounds.X + 1, bounds.Bottom - 17, bounds.Width - 2, 16);
            using (var pen = new Pen(secondaryTextColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int cx = downRect.X + downRect.Width / 2;
                int cy = downRect.Y + downRect.Height / 2;
                g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2);
                g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2);
            }
        }

        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth)
        {
            // Range highlighting handled in PaintDayCell
        }

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var rangeColor = Color.FromArgb(50, accentColor);

            cellBounds.Inflate(-2, -2);

            // Paint range background
            if (isInRange && !isSelected)
            {
                using (var brush = new SolidBrush(rangeColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }

            // Paint selection (start or end of range)
            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = Color.White;
            }
            else if (isPressed)
            {
                var pressedColor = ControlPaint.Dark(accentColor, 0.1f);
                using (var brush = new SolidBrush(pressedColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
            }

            // Today indicator
            if (isToday && !isSelected)
            {
                using (var pen = new Pen(accentColor, 2))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

            // Day number
            var dayText = date.Day.ToString();
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);
            
            if (isDisabled)
            {
                textColor = Color.FromArgb(180, 180, 180);
            }

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(dayText, font, brush, cellBounds, format);
            }
        }

        public void PaintHeader(Graphics g, Rectangle headerBounds, string headerText, bool showNavigation, bool isHovered)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 14f, FontStyle.Bold);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(headerText, font, brush, headerBounds, format);
            }
        }

        public void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed)
        {
            var iconColor = _theme?.ForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);

            if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillEllipse(brush, buttonBounds);
                }
            }

            using (var pen = new Pen(iconColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = buttonBounds.X + buttonBounds.Width / 2;
                int centerY = buttonBounds.Y + buttonBounds.Height / 2;
                int size = 5;

                if (isNext)
                {
                    g.DrawLine(pen, centerX - 2, centerY - size, centerX + 3, centerY);
                    g.DrawLine(pen, centerX + 3, centerY, centerX - 2, centerY + size);
                }
                else
                {
                    g.DrawLine(pen, centerX + 2, centerY - size, centerX - 3, centerY);
                    g.DrawLine(pen, centerX - 3, centerY, centerX + 2, centerY + size);
                }
            }
        }

        public void PaintDayNamesHeader(Graphics g, Rectangle headerBounds, DatePickerFirstDayOfWeek firstDayOfWeek)
        {
            var textColor = _theme?.SecondaryTextColor ?? Color.FromArgb(128, 128, 128);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);

            string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
            int startDay = (int)firstDayOfWeek;
            int cellWidth = headerBounds.Width / 7;
            
            for (int i = 0; i < 7; i++)
            {
                int dayIndex = (startDay + i) % 7;
                var cellRect = new Rectangle(
                    headerBounds.X + i * cellWidth,
                    headerBounds.Y,
                    cellWidth,
                    headerBounds.Height
                );

                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(dayNames[dayIndex].Substring(0, 2), font, brush, cellRect, format);
                }
            }
        }

        private void PaintCalendarGrid(Graphics g, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState)
        {
            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);
            
            int firstDayOfWeek = (int)properties.FirstDayOfWeek;
            int dayOffset = ((int)firstDayOfMonth.DayOfWeek - firstDayOfWeek + 7) % 7;

            var prevMonth = displayMonth.AddMonths(-1);
            var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            int row = 0, col = 0;

            for (int i = 0; i < dayOffset; i++)
            {
                var day = daysInPrevMonth - dayOffset + i + 1;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, day);
                var cellRect = layout.DayCellRects[row, col];
                bool isInRange = IsInRange(date, _owner.RangeStartDate, _owner.RangeEndDate);
                PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = layout.DayCellRects[row, col];
                
                bool isStartOrEnd = (_owner.RangeStartDate.HasValue && _owner.RangeStartDate.Value.Date == date.Date) ||
                                   (_owner.RangeEndDate.HasValue && _owner.RangeEndDate.Value.Date == date.Date);
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInRange = IsInRange(date, _owner.RangeStartDate, _owner.RangeEndDate);
                
                PaintDayCell(g, cellRect, date, isStartOrEnd, isToday, isDisabled, isHovered, isPressed, isInRange);
                
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            var nextMonth = displayMonth.AddMonths(1);
            while (row < 6)
            {
                for (int day = 1; col < 7; day++)
                {
                    var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
                    var cellRect = layout.DayCellRects[row, col];
                    bool isInRange = IsInRange(date, _owner.RangeStartDate, _owner.RangeEndDate);
                    PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                    col++;
                }
                col = 0;
                row++;
            }
        }

        private bool IsInRange(DateTime date, DateTime? start, DateTime? end)
        {
            if (!start.HasValue || !end.HasValue) return false;
            return date >= start.Value && date <= end.Value;
        }

        // Stub implementations
        public void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState) { }
        public void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed) { }
        public void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed) { }
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }
        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            int padding = 16;
            int currentY = bounds.Y + padding;

            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2 - 72, 40);
            
            int navButtonSize = 32;
            layout.PreviousButtonRect = new Rectangle(bounds.Right - padding - navButtonSize * 2 - 8, currentY + 4, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.Right - padding - navButtonSize, currentY + 4, navButtonSize, navButtonSize);

            currentY += 48;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 28);
            currentY += 32;

            int gridWidth = bounds.Width - padding * 2;
            // Shorter calendar to fit dual time pickers
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, 180);

            layout.CellWidth = gridWidth / 7;
            layout.CellHeight = 180 / 6;
            layout.DayCellRects = new Rectangle[6, 7];

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    layout.DayCellRects[row, col] = new Rectangle(
                        layout.CalendarGridRect.X + col * layout.CellWidth,
                        layout.CalendarGridRect.Y + row * layout.CellHeight,
                        layout.CellWidth,
                        layout.CellHeight
                    );
                }
            }

            return layout;
        }

        public Size GetPreferredDropDownSize(DateTimePickerProperties properties)
        {
            return new Size(400, 480);
        }

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth)
        {
            var result = new DateTimePickerHitTestResult();

            if (layout.PreviousButtonRect.Contains(location))
            {
                result.HitArea = DateTimePickerHitArea.PreviousButton;
                result.HitBounds = layout.PreviousButtonRect;
                return result;
            }

            if (layout.NextButtonRect.Contains(location))
            {
                result.HitArea = DateTimePickerHitArea.NextButton;
                result.HitBounds = layout.NextButtonRect;
                return result;
            }

            if (layout.CalendarGridRect.Contains(location))
            {
                for (int row = 0; row < 6; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        if (layout.DayCellRects[row, col].Contains(location))
                        {
                            var date = GetDateFromCell(row, col, displayMonth, _owner.FirstDayOfWeek);
                            result.HitArea = DateTimePickerHitArea.DayCell;
                            result.Date = date;
                            result.HitBounds = layout.DayCellRects[row, col];
                            return result;
                        }
                    }
                }
            }

            return result;
        }

        private DateTime GetDateFromCell(int row, int col, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek)
        {
            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            int firstDayOfWeekNum = (int)firstDayOfWeek;
            int dayOffset = ((int)firstDayOfMonth.DayOfWeek - firstDayOfWeekNum + 7) % 7;
            
            int dayIndex = row * 7 + col - dayOffset + 1;
            
            if (dayIndex < 1)
            {
                var prevMonth = displayMonth.AddMonths(-1);
                var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
                return new DateTime(prevMonth.Year, prevMonth.Month, daysInPrevMonth + dayIndex);
            }
            
            var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);
            if (dayIndex > daysInMonth)
            {
                var nextMonth = displayMonth.AddMonths(1);
                return new DateTime(nextMonth.Year, nextMonth.Month, dayIndex - daysInMonth);
            }
            
            return new DateTime(displayMonth.Year, displayMonth.Month, dayIndex);
        }
    }
}
