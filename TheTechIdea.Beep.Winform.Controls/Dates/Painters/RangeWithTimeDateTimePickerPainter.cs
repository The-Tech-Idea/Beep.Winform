using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

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
            PaintDualTimePickers(g, layout, hoverState);
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

        private void PaintDualTimePickers(Graphics g, DateTimePickerLayout layout, DateTimePickerHoverState hoverState)
        {
            if (layout == null)
                return;

            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(200, 200, 200);

            if (!layout.TimeSeparatorRect.IsEmpty)
            {
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawLine(pen,
                        layout.TimeSeparatorRect.Left,
                        layout.TimeSeparatorRect.Top,
                        layout.TimeSeparatorRect.Right,
                        layout.TimeSeparatorRect.Top);
                }
            }

            PaintTimePickerSection(g, layout, "Start Time", _owner.RangeStartDate, _owner.RangeStartTime, hoverState, true);
            PaintTimePickerSection(g, layout, "End Time", _owner.RangeEndDate, _owner.RangeEndTime, hoverState, false);
        }

        private void PaintTimePickerSection(Graphics g, DateTimePickerLayout layout, string label, DateTime? date, TimeSpan? time, DateTimePickerHoverState hoverState, bool isStart)
        {
            var pickerRect = isStart ? layout.StartTimePickerRect : layout.EndTimePickerRect;
            if (pickerRect == Rectangle.Empty)
                return;

            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(220, 220, 220);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 8.5f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 9f, FontStyle.Bold);

            using (var brush = new SolidBrush(secondaryTextColor))
            {
                g.DrawString(label, font, brush, pickerRect.X, pickerRect.Y);
            }

            var displayRect = isStart ? layout.StartTimeDisplayRect : layout.EndTimeDisplayRect;
            if (displayRect == Rectangle.Empty)
                return;

            if (!date.HasValue || !time.HasValue)
            {
                using (var brush = new SolidBrush(Color.FromArgb(180, 180, 180)))
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawRectangle(pen, displayRect);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("--:--", font, brush, displayRect, format);
                }
                return;
            }

            var hourRect = isStart ? layout.StartTimeHourRect : layout.EndTimeHourRect;
            var minuteRect = isStart ? layout.StartTimeMinuteRect : layout.EndTimeMinuteRect;
            var colonRect = isStart ? layout.StartTimeColonRect : layout.EndTimeColonRect;
            var hourUpRect = isStart ? layout.StartTimeHourUpRect : layout.EndTimeHourUpRect;
            var hourDownRect = isStart ? layout.StartTimeHourDownRect : layout.EndTimeHourDownRect;
            var minuteUpRect = isStart ? layout.StartTimeMinuteUpRect : layout.EndTimeMinuteUpRect;
            var minuteDownRect = isStart ? layout.StartTimeMinuteDownRect : layout.EndTimeMinuteDownRect;

            // Check hover/pressed states for time spinner buttons using proper enum values
            bool hourUpHovered = hoverState?.IsAreaHovered(isStart ? DateTimePickerHitArea.StartHourUpButton : DateTimePickerHitArea.EndHourUpButton) == true;
            bool hourUpPressed = hoverState?.IsAreaPressed(isStart ? DateTimePickerHitArea.StartHourUpButton : DateTimePickerHitArea.EndHourUpButton) == true;
            bool hourDownHovered = hoverState?.IsAreaHovered(isStart ? DateTimePickerHitArea.StartHourDownButton : DateTimePickerHitArea.EndHourDownButton) == true;
            bool hourDownPressed = hoverState?.IsAreaPressed(isStart ? DateTimePickerHitArea.StartHourDownButton : DateTimePickerHitArea.EndHourDownButton) == true;
            bool minuteUpHovered = hoverState?.IsAreaHovered(isStart ? DateTimePickerHitArea.StartMinuteUpButton : DateTimePickerHitArea.EndMinuteUpButton) == true;
            bool minuteUpPressed = hoverState?.IsAreaPressed(isStart ? DateTimePickerHitArea.StartMinuteUpButton : DateTimePickerHitArea.EndMinuteUpButton) == true;
            bool minuteDownHovered = hoverState?.IsAreaHovered(isStart ? DateTimePickerHitArea.StartMinuteDownButton : DateTimePickerHitArea.EndMinuteDownButton) == true;
            bool minuteDownPressed = hoverState?.IsAreaPressed(isStart ? DateTimePickerHitArea.StartMinuteDownButton : DateTimePickerHitArea.EndMinuteDownButton) == true;

            PaintTimeSpinner(g, hourRect, hourUpRect, hourDownRect, time.Value.Hours.ToString("D2"), hourUpHovered, hourUpPressed, hourDownHovered, hourDownPressed);

            if (colonRect != Rectangle.Empty)
            {
                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(":", boldFont, brush, colonRect, format);
                }
            }

            PaintTimeSpinner(g, minuteRect, minuteUpRect, minuteDownRect, time.Value.Minutes.ToString("D2"), minuteUpHovered, minuteUpPressed, minuteDownHovered, minuteDownPressed);
        }

        private void PaintTimeSpinner(Graphics g, Rectangle bounds, Rectangle upRect, Rectangle downRect, string value, bool upHovered, bool upPressed, bool downHovered, bool downPressed)
        {
            if (bounds == Rectangle.Empty)
                return;

            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(120, 120, 120);
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var pressedColor = _theme?.AccentColor ?? Color.FromArgb(200, 200, 200);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);

            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            if (!upRect.IsEmpty && (upHovered || upPressed))
            {
                using (var brush = new SolidBrush(upPressed ? pressedColor : hoverColor))
                {
                    g.FillRectangle(brush, upRect);
                }
            }

            if (!downRect.IsEmpty && (downHovered || downPressed))
            {
                using (var brush = new SolidBrush(downPressed ? pressedColor : hoverColor))
                {
                    g.FillRectangle(brush, downRect);
                }
            }

            using (var pen = new Pen(secondaryTextColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                if (!upRect.IsEmpty)
                {
                    int cx = upRect.X + upRect.Width / 2;
                    int cy = upRect.Y + upRect.Height / 2;
                    g.DrawLine(pen, cx - 4, cy + 2, cx, cy - 2);
                    g.DrawLine(pen, cx, cy - 2, cx + 4, cy + 2);
                }

                if (!downRect.IsEmpty)
                {
                    int cx = downRect.X + downRect.Width / 2;
                    int cy = downRect.Y + downRect.Height / 2;
                    g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2);
                    g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2);
                }
            }

            var valueRect = new Rectangle(bounds.X, bounds.Y + 18, bounds.Width, 18);
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(value, font, brush, valueRect, format);
            }
        }

        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth)
        {
            // Range highlighting handled in PaintDayCell
        }

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.Empty;
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);
            var rangeColor = PathPainterHelpers.WithAlphaIfNotEmpty(accentColor, 50);
            
            // Define distinct colors for start and end dates
            var startDateColor = Color.FromArgb(34, 139, 34);  // Forest Green
            var endDateColor = accentColor;  // Blue (accent)

            cellBounds.Inflate(-2, -2);

            // Paint range background (but not on start/end dates)
            if (isInRange && !isStartDate && !isEndDate)
            {
                using (var brush = new SolidBrush(rangeColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }

            // Paint start date with green
            if (isStartDate)
            {
                using (var brush = new SolidBrush(startDateColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = Color.White;
            }
            // Paint end date with blue
            else if (isEndDate)
            {
                using (var brush = new SolidBrush(endDateColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = Color.White;
            }
            // Paint selection (start or end of range) - fallback
            else if (isSelected)
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

            // Today indicator
            if (isToday && !isSelected && !isStartDate && !isEndDate)
            {
                using (var pen = new Pen(todayColor, 2))
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
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
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
            // Use DayCellMatrix directly for consistent access with hit handler
            var dayCells = layout.DayCellMatrix ?? layout.GetDayCellMatrixOrDefault(6, 7);
            if (dayCells == null) return;

            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);

            int firstDayOfWeek = (int)properties.FirstDayOfWeek;
            int dayOffset = ((int)firstDayOfMonth.DayOfWeek - firstDayOfWeek + 7) % 7;

            var prevMonth = displayMonth.AddMonths(-1);
            var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            int row = 0, col = 0;

            // Paint previous month days
            for (int i = 0; i < dayOffset; i++)
            {
                var day = daysInPrevMonth - dayOffset + i + 1;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, day);
                var cellRect = dayCells[row, col];
                bool isInRange = IsInRange(date, _owner.RangeStartDate, _owner.RangeEndDate);
                PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            // Paint current month days
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = dayCells[row, col];

                bool isStartDate = _owner.RangeStartDate.HasValue && _owner.RangeStartDate.Value.Date == date.Date;
                bool isEndDate = _owner.RangeEndDate.HasValue && _owner.RangeEndDate.Value.Date == date.Date;
                bool isStartOrEnd = isStartDate || isEndDate;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInRange = IsInRange(date, _owner.RangeStartDate, _owner.RangeEndDate);

                PaintDayCell(g, cellRect, date, isStartOrEnd, isToday, isDisabled, isHovered, isPressed, isInRange, isStartDate, isEndDate);

                col++;
                if (col >= 7) { col = 0; row++; }
            }

            // Paint next month days
            var nextMonth = displayMonth.AddMonths(1);
            while (row < 6)
            {
                for (int day = 1; col < 7; day++)
                {
                    var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
                    var cellRect = dayCells[row, col];
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

            // Ensure minimum bounds
            int minWidth = Math.Max(bounds.Width, 360);
            int effectiveWidth = bounds.Width > 0 ? bounds.Width : minWidth;

            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, effectiveWidth - padding * 2 - 72, 40);
            
            int navButtonSize = 32;
            layout.PreviousButtonRect = new Rectangle(bounds.X + effectiveWidth - padding - navButtonSize * 2 - 8, currentY + 4, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.X + effectiveWidth - padding - navButtonSize, currentY + 4, navButtonSize, navButtonSize);

            currentY += 48;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, effectiveWidth - padding * 2, 28);
            currentY += 32;

            int gridWidth = effectiveWidth - padding * 2;
            int gridHeight = 180; // Shorter calendar to fit dual time pickers
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);

            // Calculate cell dimensions with minimum size guarantee
            layout.CellWidth = Math.Max(gridWidth / 7, 30);
            layout.CellHeight = Math.Max(gridHeight / 6, 30);

            // Create both matrix and list for compatibility
            layout.DayCellMatrix = new Rectangle[6, 7];
            layout.DayCellRects = new List<Rectangle>(42);

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    var cellRect = new Rectangle(
                        layout.CalendarGridRect.X + col * layout.CellWidth,
                        layout.CalendarGridRect.Y + row * layout.CellHeight,
                        layout.CellWidth,
                        layout.CellHeight
                    );
                    
                    layout.DayCellMatrix[row, col] = cellRect;
                    layout.DayCellRects.Add(cellRect);
                }
            }

            // Time pickers layout
            int timeSectionTop = bounds.Bottom - 100;
            int lineLeft = bounds.X + padding;
            int lineWidth = Math.Max(0, effectiveWidth - padding * 2);
            layout.TimeSeparatorRect = new Rectangle(lineLeft, timeSectionTop, lineWidth, 0);

            int halfWidth = Math.Max(0, (effectiveWidth - padding * 3) / 2);
            int pickerHeight = 80;
            int pickerY = timeSectionTop + 12;

            layout.StartTimePickerRect = halfWidth > 0
                ? new Rectangle(lineLeft, pickerY, halfWidth, pickerHeight)
                : Rectangle.Empty;

            layout.EndTimePickerRect = halfWidth > 0
                ? new Rectangle(lineLeft + halfWidth + padding, pickerY, halfWidth, pickerHeight)
                : Rectangle.Empty;

            layout.StartTimeDisplayRect = layout.StartTimePickerRect != Rectangle.Empty
                ? new Rectangle(layout.StartTimePickerRect.X, layout.StartTimePickerRect.Y + 22, layout.StartTimePickerRect.Width, 54)
                : Rectangle.Empty;

            layout.EndTimeDisplayRect = layout.EndTimePickerRect != Rectangle.Empty
                ? new Rectangle(layout.EndTimePickerRect.X, layout.EndTimePickerRect.Y + 22, layout.EndTimePickerRect.Width, 54)
                : Rectangle.Empty;

            void ConfigureSpinnerLayout(
                Rectangle displayRect,
                out Rectangle hourRect,
                out Rectangle minuteRect,
                out Rectangle colonRect,
                out Rectangle hourUp,
                out Rectangle hourDown,
                out Rectangle minuteUp,
                out Rectangle minuteDown)
            {
                if (displayRect == Rectangle.Empty)
                {
                    hourRect = minuteRect = colonRect = hourUp = hourDown = minuteUp = minuteDown = Rectangle.Empty;
                    return;
                }

                int spinnerWidth = (displayRect.Width - 20) / 2;
                if (spinnerWidth <= 0)
                {
                    hourRect = minuteRect = colonRect = hourUp = hourDown = minuteUp = minuteDown = Rectangle.Empty;
                    return;
                }

                hourRect = new Rectangle(displayRect.X, displayRect.Y, spinnerWidth, displayRect.Height);
                colonRect = new Rectangle(hourRect.Right, displayRect.Y, 20, displayRect.Height);
                minuteRect = new Rectangle(hourRect.Right + 20, displayRect.Y, spinnerWidth, displayRect.Height);

                hourUp = new Rectangle(hourRect.X + 1, hourRect.Y + 1, hourRect.Width - 2, 16);
                hourDown = new Rectangle(hourRect.X + 1, hourRect.Bottom - 17, hourRect.Width - 2, 16);
                minuteUp = new Rectangle(minuteRect.X + 1, minuteRect.Y + 1, minuteRect.Width - 2, 16);
                minuteDown = new Rectangle(minuteRect.X + 1, minuteRect.Bottom - 17, minuteRect.Width - 2, 16);
            }

            ConfigureSpinnerLayout(
                layout.StartTimeDisplayRect,
                out var startHourRect,
                out var startMinuteRect,
                out var startColonRect,
                out var startHourUpRect,
                out var startHourDownRect,
                out var startMinuteUpRect,
                out var startMinuteDownRect);

            layout.StartTimeHourRect = startHourRect;
            layout.StartTimeMinuteRect = startMinuteRect;
            layout.StartTimeColonRect = startColonRect;
            layout.StartTimeHourUpRect = startHourUpRect;
            layout.StartTimeHourDownRect = startHourDownRect;
            layout.StartTimeMinuteUpRect = startMinuteUpRect;
            layout.StartTimeMinuteDownRect = startMinuteDownRect;

            ConfigureSpinnerLayout(
                layout.EndTimeDisplayRect,
                out var endHourRect,
                out var endMinuteRect,
                out var endColonRect,
                out var endHourUpRect,
                out var endHourDownRect,
                out var endMinuteUpRect,
                out var endMinuteDownRect);

            layout.EndTimeHourRect = endHourRect;
            layout.EndTimeMinuteRect = endMinuteRect;
            layout.EndTimeColonRect = endColonRect;
            layout.EndTimeHourUpRect = endHourUpRect;
            layout.EndTimeHourDownRect = endHourDownRect;
            layout.EndTimeMinuteUpRect = endMinuteUpRect;
            layout.EndTimeMinuteDownRect = endMinuteDownRect;

            // Register all hit areas with BaseControl's hit test system
            _owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);

            return layout;
        }

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Padding(16*2=32) + Header(48) + DayNames(32) + Grid(180) + TimePickers(80) + RangeInfo(20) = 392px
            int width = 400;
            int height = 400; // Header + calendar + time picker
            
            if (properties.ShowCustomQuickDates)
                height += 50;
            
            return new Size(width, height);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Padding(16*2=32) + Header(48) + DayNames(32) + Grid(180) + TimePickers(80) = 372px
            int padding = 16;
            int headerHeight = 48;
            int dayNamesHeight = 32;
            int gridHeight = 180;
            int timePickersHeight = 80;
            int minHeight = padding * 2 + headerHeight + dayNamesHeight + gridHeight + timePickersHeight;
            
            int minWidth = 7 * 40 + padding * 2; // 7 cells * 40px + padding = 312px
            
            return new Size(Math.Max(minWidth, 360), Math.Max(minHeight, 380));
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
                        var cellRect = dayCells[row, col];

                        if (cellRect.Contains(location))
                        {
                            var date = GetDateFromCell(row, col, displayMonth, _owner.FirstDayOfWeek);
                            result.HitArea = DateTimePickerHitArea.DayCell;
                            result.Date = date;
                            result.HitBounds = cellRect;
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
