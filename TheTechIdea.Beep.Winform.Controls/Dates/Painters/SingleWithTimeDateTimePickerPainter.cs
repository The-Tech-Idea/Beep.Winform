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
    /// Single Date With Time Painter
    /// Calendar with integrated time picker section
    /// Visual styling follows BeepTheme
    /// </summary>
    public class SingleWithTimeDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.SingleWithTime;

        public SingleWithTimeDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
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

            // Paint calendar section
            PaintHeader(g, layout.HeaderRect, displayMonth.ToString("MMMM yyyy"), true, false);
            
            PaintNavigationButton(g, layout.PreviousButtonRect, false, 
                hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));
            
            PaintNavigationButton(g, layout.NextButtonRect, true,
                hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));

            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);

            // Paint time picker section with spinners (bottom)
            PaintTimePickerWithSpinners(g, layout, _owner.SelectedTime ?? TimeSpan.Zero, hoverState);
        }

        private void PaintBackground(Graphics g, Rectangle bounds)
        {
            // BaseControl (Minimalist) handles outer background/border; fill interior only
            var bgColor = _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        private void PaintTimePickerWithSpinners(Graphics g, DateTimePickerLayout layout, TimeSpan selectedTime, DateTimePickerHoverState hoverState)
        {
            if (layout.TimePickerRect == Rectangle.Empty)
                return;

            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(220, 220, 220);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);

            // Draw separator line
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, layout.TimePickerRect.X, layout.TimePickerRect.Y, 
                    layout.TimePickerRect.Right, layout.TimePickerRect.Y);
            }

            // Draw "Time:" label
            var labelRect = new Rectangle(layout.TimePickerRect.X, layout.TimePickerRect.Y + 12, 60, 24);
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                g.DrawString("Time:", font, brush, labelRect);
            }

            // Get hour and minute
            int hour = selectedTime.Hours;
            int minute = selectedTime.Minutes;

            // Draw hour spinner
            if (layout.TimeHourRect != Rectangle.Empty)
            {
                bool hourUpHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartHourUpButton) == true;
                bool hourUpPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartHourUpButton) == true;
                bool hourDownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartHourDownButton) == true;
                bool hourDownPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartHourDownButton) == true;

                PaintTimeSpinner(g, layout.TimeHourRect, layout.TimeHourUpRect, layout.TimeHourDownRect,
                    hour.ToString("D2"), hourUpHovered, hourUpPressed, hourDownHovered, hourDownPressed);
            }

            // Draw colon separator
            if (layout.TimeColonRect != Rectangle.Empty)
            {
                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 14f, FontStyle.Bold);
                    g.DrawString(":", boldFont, brush, layout.TimeColonRect, format);
                }
            }

            // Draw minute spinner
            if (layout.TimeMinuteRect != Rectangle.Empty)
            {
                bool minuteUpHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartMinuteUpButton) == true;
                bool minuteUpPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartMinuteUpButton) == true;
                bool minuteDownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartMinuteDownButton) == true;
                bool minuteDownPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartMinuteDownButton) == true;

                PaintTimeSpinner(g, layout.TimeMinuteRect, layout.TimeMinuteUpRect, layout.TimeMinuteDownRect,
                    minute.ToString("D2"), minuteUpHovered, minuteUpPressed, minuteDownHovered, minuteDownPressed);
            }
        }

        private void PaintTimeSpinner(Graphics g, Rectangle bounds, Rectangle upRect, Rectangle downRect, 
            string value, bool upHovered, bool upPressed, bool downHovered, bool downPressed)
        {
            if (bounds == Rectangle.Empty)
                return;

            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(120, 120, 120);
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var pressedColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 12f);

            // Draw spinner border
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Draw up button background
            if (!upRect.IsEmpty && (upHovered || upPressed))
            {
                using (var brush = new SolidBrush(upPressed ? pressedColor : hoverColor))
                {
                    g.FillRectangle(brush, upRect);
                }
            }

            // Draw down button background
            if (!downRect.IsEmpty && (downHovered || downPressed))
            {
                using (var brush = new SolidBrush(downPressed ? pressedColor : hoverColor))
                {
                    g.FillRectangle(brush, downRect);
                }
            }

            // Draw up/down arrow icons
            using (var pen = new Pen(upPressed || downPressed ? Color.White : secondaryTextColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Up arrow
                if (!upRect.IsEmpty)
                {
                    int cx = upRect.X + upRect.Width / 2;
                    int cy = upRect.Y + upRect.Height / 2;
                    g.DrawLine(pen, cx - 4, cy + 2, cx, cy - 2);
                    g.DrawLine(pen, cx, cy - 2, cx + 4, cy + 2);
                }

                // Down arrow
                if (!downRect.IsEmpty)
                {
                    int cx = downRect.X + downRect.Width / 2;
                    int cy = downRect.Y + downRect.Height / 2;
                    g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2);
                    g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2);
                }
            }

            // Draw value text (centered between up and down buttons)
            var valueRect = new Rectangle(bounds.X, bounds.Y + 18, bounds.Width, 18);
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(value, font, brush, valueRect, format);
            }
        }

        public void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState)
        {
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);

            // Draw separator line
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, timePickerBounds.X, timePickerBounds.Y, timePickerBounds.Right, timePickerBounds.Y);
            }

            // Draw "Time:" label
            var labelRect = new Rectangle(timePickerBounds.X, timePickerBounds.Y + 8, 60, 24);
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString("Time:", font, brush, labelRect);
            }

            // Draw time slots horizontally
            int slotWidth = 70;
            int slotHeight = 32;
            int startX = timePickerBounds.X + 70;
            int currentX = startX;
            int currentY = timePickerBounds.Y + 8;
            int maxWidth = timePickerBounds.Width - 70;

            for (int i = 0; i < Math.Min(timeSlots.Count, 8); i++)
            {
                var time = timeSlots[i * (timeSlots.Count / 8)];
                var slotRect = new Rectangle(currentX, currentY, slotWidth, slotHeight);
                
                bool isSelected = selectedTime.HasValue && selectedTime.Value == time;
                bool isHovered = hoverState.IsAreaHovered(DateTimePickerHitArea.TimeSlot);
                
                PaintTimeSlot(g, slotRect, time, isSelected, isHovered, false);
                
                currentX += slotWidth + 4;
                if (currentX + slotWidth > timePickerBounds.Right)
                {
                    currentX = startX;
                    currentY += slotHeight + 4;
                }
            }
        }

        public void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(200, 200, 200);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);

            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillRectangle(brush, slotBounds);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, slotBounds);
                }
            }

            using (var pen = new Pen(isSelected ? accentColor : borderColor, 1))
            {
                g.DrawRectangle(pen, slotBounds.X, slotBounds.Y, slotBounds.Width - 1, slotBounds.Height - 1);
            }

            var timeText = time.ToString(@"hh\:mm");
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(timeText, font, brush, slotBounds, format);
            }
        }

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);

            cellBounds.Inflate(-2, -2);

            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
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

            if (isToday && !isSelected)
            {
                using (var pen = new Pen(todayColor, 2))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

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
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 13f, FontStyle.Bold);

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
            var iconColor = _theme?.CalendarTitleForColor ?? Color.Black;
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
            var textColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(128, 128, 128);
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
            var dayCells = layout.GetDayCellMatrixOrDefault();

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
                var cellRect = dayCells[row, col];
                PaintDayCell(g, cellRect, date, false, false, true, false, false, false);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = dayCells[row, col];

                bool isSelected = _owner.SelectedDate.HasValue && _owner.SelectedDate.Value.Date == date.Date;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);

                PaintDayCell(g, cellRect, date, isSelected, isToday, isDisabled, isHovered, isPressed, false);

                col++;
                if (col >= 7) { col = 0; row++; }
            }

            var nextMonth = displayMonth.AddMonths(1);
            while (row < 6)
            {
                for (int day = 1; col < 7; day++)
                {
                    var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
                    var cellRect = dayCells[row, col];
                    PaintDayCell(g, cellRect, date, false, false, true, false, false, false);
                    col++;
                }
                col = 0;
                row++;
            }
        }

        // Stub implementations
        public void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed) { }
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }
        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            // Treat bounds as content area from container
            int padding = 10;
            int currentY = bounds.Y + padding;

            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2 - 72, 36);
            
            int navButtonSize = 28;
            layout.PreviousButtonRect = new Rectangle(bounds.Right - padding - navButtonSize * 2 - 8, currentY + 2, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.Right - padding - navButtonSize, currentY + 2, navButtonSize, navButtonSize);

            currentY += 38;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 22);
            currentY += 26;

            int gridWidth = bounds.Width - padding * 2;
            // Fit calendar grid and leave room for time picker and bottom padding
            int roomForTime = 100; // approx time picker block
            int availableHeight = Math.Max(60, bounds.Bottom - currentY - roomForTime - padding);
            int gridHeight = Math.Max(132, Math.Min(availableHeight, 210));
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);

            layout.CellWidth = gridWidth / 7;
            layout.CellHeight = gridHeight / 6;

            var dayCells = new Rectangle[6, 7];

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    dayCells[row, col] = new Rectangle(
                        layout.CalendarGridRect.X + col * layout.CellWidth,
                        layout.CalendarGridRect.Y + row * layout.CellHeight,
                        layout.CellWidth,
                        layout.CellHeight
                    );
                }
            }

            layout.DayCellMatrix = dayCells;

            // Calculate time picker area with spinners
            currentY = layout.CalendarGridRect.Bottom + 16;
            int timePickerHeight = 80;
            layout.TimePickerRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, timePickerHeight);

            // Time spinner controls (hour : minute)
            int labelWidth = 60;
            int spinnerWidth = 60;
            int spinnerHeight = 54;
            int colonWidth = 20;
            int startX = layout.TimePickerRect.X + labelWidth + 10;
            int spinnerY = layout.TimePickerRect.Y + 12;

            // Hour spinner
            layout.TimeHourRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight);
            layout.TimeHourUpRect = new Rectangle(layout.TimeHourRect.X, layout.TimeHourRect.Y, spinnerWidth, 16);
            layout.TimeHourDownRect = new Rectangle(layout.TimeHourRect.X, layout.TimeHourRect.Bottom - 16, spinnerWidth, 16);

            // Colon separator
            layout.TimeColonRect = new Rectangle(startX + spinnerWidth + 4, spinnerY, colonWidth, spinnerHeight);

            // Minute spinner
            startX += spinnerWidth + colonWidth + 8;
            layout.TimeMinuteRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight);
            layout.TimeMinuteUpRect = new Rectangle(layout.TimeMinuteRect.X, layout.TimeMinuteRect.Y, spinnerWidth, 16);
            layout.TimeMinuteDownRect = new Rectangle(layout.TimeMinuteRect.X, layout.TimeMinuteRect.Bottom - 16, spinnerWidth, 16);

            // Register all hit areas with BaseControl's hit test system
            _owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);

            return layout;
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
                var dayCells = layout.GetDayCellMatrixOrDefault();
                for (int row = 0; row < dayCells.GetLength(0); row++)
                {
                    for (int col = 0; col < dayCells.GetLength(1); col++)
                    {
                        var cell = dayCells[row, col];
                        if (cell.Contains(location))
                        {
                            var date = GetDateFromCell(row, col, displayMonth, _owner.FirstDayOfWeek);
                            result.HitArea = DateTimePickerHitArea.DayCell;
                            result.Date = date;
                            result.HitBounds = cell;
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

        private List<TimeSpan> GenerateTimeSlots(TimeSpan interval, TimeSpan minTime, TimeSpan maxTime)
        {
            var slots = new List<TimeSpan>();
            var current = minTime;
            while (current <= maxTime)
            {
                slots.Add(current);
                current = current.Add(interval);
            }
            return slots;
        }

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Padding(10*2=20) + Header(38) + DayNames(26) + Grid(160) + TimePicker(100) = 344px
            int width = 350;
            int height = 360; // Calendar + time picker block

            if (properties.ShowCustomQuickDates)
            {
                height += 60;
            }

            return new Size(width, height);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Padding(10*2=20) + Header(38) + DayNames(26) + MinGrid(132) + TimePicker(100) = 316px
            int padding = 10;
            int headerHeight = 38;
            int dayNamesHeight = 26;
            int minGridHeight = 132;
            int timePickerHeight = 100;
            int minHeight = padding * 2 + headerHeight + dayNamesHeight + minGridHeight + timePickerHeight;
            
            int minWidth = 7 * 35 + padding * 2; // 7 cells * 35px + padding = 265px
            
            return new Size(Math.Max(minWidth, 300), Math.Max(minHeight, 320));
        }
    }
}
