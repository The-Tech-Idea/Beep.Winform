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
    /// Appointment Date Time Picker Painter
    /// Calendar with scrollable hourly time slot list on the right
    /// Visual styling follows BeepTheme
    /// </summary>
    public class AppointmentDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;
        private int _scrollOffset = 0;

        public DatePickerMode Mode => DatePickerMode.Appointment;

        public AppointmentDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        // Helper method to convert row,col to flat list index
        private int GetCellIndex(int row, int col)
        {
            return row * 7 + col;
        }

        public void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Paint background
            PaintBackground(g, bounds);

            // Split layout: Calendar on left, time slots on right
            int calendarWidth = (int)(bounds.Width * 0.55f);
            int timeSlotWidth = bounds.Width - calendarWidth - 1;

            var calendarBounds = new Rectangle(bounds.X, bounds.Y, calendarWidth, bounds.Height);
            var timeSlotBounds = new Rectangle(bounds.X + calendarWidth + 1, bounds.Y, timeSlotWidth, bounds.Height);

            // Paint calendar section
            PaintCalendarSection(g, calendarBounds, properties, displayMonth, hoverState);

            // Paint vertical separator
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, bounds.X + calendarWidth, bounds.Y, bounds.X + calendarWidth, bounds.Bottom);
            }

            // Paint time slot section
            PaintTimeSlotList(g, timeSlotBounds, hoverState);
            
            // Paint time control section at bottom
            var timeControlBounds = new Rectangle(timeSlotBounds.X, timeSlotBounds.Bottom - 100, timeSlotBounds.Width, 90);
            PaintTimeControls(g, timeControlBounds, hoverState);
        }

        private void PaintBackground(Graphics g, Rectangle bounds)
        {
            var bgColor =  _theme?.CalendarBackColor ?? Color.White;
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(200, 200, 200);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }

        private void PaintCalendarSection(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            var layout = CalculateLayout(bounds, properties);

            // Paint header
            PaintHeader(g, layout.HeaderRect, displayMonth.ToString("MMM yyyy"), true, false);
            
            PaintNavigationButton(g, layout.PreviousButtonRect, false, 
                hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));
            
            PaintNavigationButton(g, layout.NextButtonRect, true,
                hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));

            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);
        }

        private void PaintTimeSlotList(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.Empty;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(220, 220, 220);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 9f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 9f, FontStyle.Bold);

            int padding = 12;
            int currentY = bounds.Y + padding;

            // Header
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                g.DrawString("Quick Select", boldFont, brush, bounds.X + padding, currentY);
            }
            currentY += 32;

            // Scrollable time slot list (hourly from 8 AM to 8 PM)
            // Reserve space for time controls at bottom (100px)
            int timeControlsHeight = 100;
            int slotHeight = 44;
            int availableHeight = bounds.Height - currentY - padding - timeControlsHeight;
            int visibleSlots = availableHeight / slotHeight;

            for (int hour = 8; hour <= 20; hour++)
            {
                var time = new TimeSpan(hour, 0, 0);
                var slotBounds = new Rectangle(
                    bounds.X + padding,
                    currentY + (hour - 8) * slotHeight,
                    bounds.Width - padding * 2,
                    slotHeight - 4
                );

                if (slotBounds.Bottom > bounds.Bottom - timeControlsHeight) break;

                bool isSelected = _owner.SelectedTime.HasValue && 
                                 _owner.SelectedTime.Value.Hours == hour &&
                                 _owner.SelectedTime.Value.Minutes == 0;

                PaintTimeSlotItem(g, slotBounds, time, isSelected, false, false);
            }

            // Scroll indicator if needed
            if (visibleSlots < 13)
            {
                PaintScrollIndicator(g, new Rectangle(bounds.Right - 16, bounds.Y + 40, 12, availableHeight - 40));
            }
        }

        private void PaintTimeSlotItem(Graphics g, Rectangle bounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed)
        {
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(230, 230, 230);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f);
            var smallFont = new Font(_theme?.FontName ?? "Segoe UI", 8f);

            // Background
            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                using (var path = GetRoundedRectPath(bounds, 6))
                {
                    g.FillPath(brush, path);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
                secondaryTextColor = Color.FromArgb(230, 230, 230);
            }
            else if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                using (var path = GetRoundedRectPath(bounds, 6))
                {
                    g.FillPath(brush, path);
                }
            }
            else
            {
                using (var pen = new Pen(borderColor, 1))
                using (var path = GetRoundedRectPath(bounds, 6))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Time text
            string timeText = DateTime.Today.Add(time).ToString("h:00 tt");
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(timeText, font, brush, bounds.X + 12, bounds.Y + 8);
            }

            // Duration indicator
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                g.DrawString("60 min", smallFont, brush, bounds.X + 12, bounds.Y + 26);
            }

            // Available indicator
            if (!isSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(46, 125, 50)))
                {
                    g.FillEllipse(brush, bounds.Right - 24, bounds.Y + 14, 8, 8);
                }
            }
        }

        private void PaintTimeControls(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(220, 220, 220);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 9f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 9f, FontStyle.Bold);

            // Draw separator line
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, bounds.X, bounds.Y, bounds.Right, bounds.Y);
            }

            int padding = 12;
            int currentY = bounds.Y + 8;

            // "Precise Time" label
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                g.DrawString("Precise Time", boldFont, brush, bounds.X + padding, currentY);
            }
            currentY += 24;

            // Get current time
            var selectedTime = _owner.SelectedTime ?? TimeSpan.Zero;
            int hour = selectedTime.Hours;
            int minute = selectedTime.Minutes;

            // Hour and minute controls layout
            int labelWidth = 40;
            int controlWidth = 60;
            int controlHeight = 44;
            int spacing = 8;

            // Hour label
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString("Hour:", font, brush, bounds.X + padding, currentY + 12);
            }

            // Hour spinner
            var hourRect = new Rectangle(bounds.X + padding + labelWidth, currentY, controlWidth, controlHeight);
            var hourUpRect = new Rectangle(hourRect.X, hourRect.Y, controlWidth, 14);
            var hourDownRect = new Rectangle(hourRect.X, hourRect.Bottom - 14, controlWidth, 14);

            bool hourUpHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartHourUpButton) == true;
            bool hourUpPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartHourUpButton) == true;
            bool hourDownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartHourDownButton) == true;
            bool hourDownPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartHourDownButton) == true;

            PaintTimeControlSpinner(g, hourRect, hourUpRect, hourDownRect, hour.ToString("D2"),
                hourUpHovered, hourUpPressed, hourDownHovered, hourDownPressed);

            // Minute label
            int minuteX = hourRect.Right + spacing + 10;
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString("Min:", font, brush, minuteX, currentY + 12);
            }

            // Minute spinner
            var minuteRect = new Rectangle(minuteX + labelWidth, currentY, controlWidth, controlHeight);
            var minuteUpRect = new Rectangle(minuteRect.X, minuteRect.Y, controlWidth, 14);
            var minuteDownRect = new Rectangle(minuteRect.X, minuteRect.Bottom - 14, controlWidth, 14);

            bool minuteUpHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartMinuteUpButton) == true;
            bool minuteUpPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartMinuteUpButton) == true;
            bool minuteDownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartMinuteDownButton) == true;
            bool minuteDownPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartMinuteDownButton) == true;

            PaintTimeControlSpinner(g, minuteRect, minuteUpRect, minuteDownRect, minute.ToString("D2"),
                minuteUpHovered, minuteUpPressed, minuteDownHovered, minuteDownPressed);
        }

        private void PaintTimeControlSpinner(Graphics g, Rectangle bounds, Rectangle upRect, Rectangle downRect,
            string value, bool upHovered, bool upPressed, bool downHovered, bool downPressed)
        {
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(120, 120, 120);
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var pressedColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 11f, FontStyle.Bold);

            // Draw spinner border
            using (var pen = new Pen(borderColor, 1))
            using (var path = GetRoundedRectPath(bounds, 4))
            {
                g.DrawPath(pen, path);
            }

            // Draw up button background
            if (upHovered || upPressed)
            {
                using (var brush = new SolidBrush(upPressed ? pressedColor : hoverColor))
                {
                    var upPath = new GraphicsPath();
                    upPath.AddArc(bounds.X, bounds.Y, 8, 8, 180, 90);
                    upPath.AddArc(bounds.Right - 8, bounds.Y, 8, 8, 270, 90);
                    upPath.AddLine(bounds.Right, upRect.Bottom, bounds.X, upRect.Bottom);
                    upPath.CloseFigure();
                    g.FillPath(brush, upPath);
                }
            }

            // Draw down button background
            if (downHovered || downPressed)
            {
                using (var brush = new SolidBrush(downPressed ? pressedColor : hoverColor))
                {
                    var downPath = new GraphicsPath();
                    downPath.AddLine(bounds.X, downRect.Y, bounds.Right, downRect.Y);
                    downPath.AddArc(bounds.Right - 8, bounds.Bottom - 8, 8, 8, 0, 90);
                    downPath.AddArc(bounds.X, bounds.Bottom - 8, 8, 8, 90, 90);
                    downPath.CloseFigure();
                    g.FillPath(brush, downPath);
                }
            }

            // Draw separator lines
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, bounds.X, upRect.Bottom, bounds.Right, upRect.Bottom);
                g.DrawLine(pen, bounds.X, downRect.Y, bounds.Right, downRect.Y);
            }

            // Draw up/down arrow icons
            var arrowColor = (upPressed || downPressed) ? Color.White : secondaryTextColor;
            using (var pen = new Pen(arrowColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Up arrow
                int cx = upRect.X + upRect.Width / 2;
                int cy = upRect.Y + upRect.Height / 2;
                g.DrawLine(pen, cx - 3, cy + 1, cx, cy - 2);
                g.DrawLine(pen, cx, cy - 2, cx + 3, cy + 1);

                // Down arrow
                cx = downRect.X + downRect.Width / 2;
                cy = downRect.Y + downRect.Height / 2;
                g.DrawLine(pen, cx - 3, cy - 1, cx, cy + 2);
                g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 1);
            }

            // Draw value text (centered between up and down buttons)
            var valueRect = new Rectangle(bounds.X, upRect.Bottom, bounds.Width, downRect.Y - upRect.Bottom);
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(value, font, brush, valueRect, format);
            }
        }

        private void PaintScrollIndicator(Graphics g, Rectangle bounds)
        {
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(200, 200, 200);
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);

            // Track
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            {
                g.FillRectangle(brush, bounds);
            }

            // Thumb
            int thumbHeight = bounds.Height / 3;
            var thumbBounds = new Rectangle(bounds.X, bounds.Y + _scrollOffset, bounds.Width, thumbHeight);
            using (var brush = new SolidBrush(accentColor))
            {
                g.FillRectangle(brush, thumbBounds);
            }
        }

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);

            cellBounds.Inflate(-1, -1);

            // Paint selection
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
                textColor = _theme?.CalendarHoverForeColor ?? textColor;
            }

            // Today indicator
            if (isToday && !isSelected)
            {
                using (var pen = new Pen(todayColor, 2))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

            // Day number
            var dayText = date.Day.ToString();
            var font = new Font(_theme?.FontName ?? "Segoe UI", 9f);
            
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

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        public void PaintHeader(Graphics g, Rectangle headerBounds, string headerText, bool showNavigation, bool isHovered)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 12f, FontStyle.Bold);

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
                int size = 4;

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
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 8f);

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
                    g.DrawString(dayNames[dayIndex].Substring(0, 1), font, brush, cellRect, format);
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
                var cellRect = layout.DayCellRects[GetCellIndex(row, col)];
                PaintDayCell(g, cellRect, date, false, false, true, false, false, false);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = layout.DayCellRects[GetCellIndex(row, col)];
                
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
                    var cellRect = layout.DayCellRects[GetCellIndex(row, col)];
                    PaintDayCell(g, cellRect, date, false, false, true, false, false, false);
                    col++;
                }
                col = 0;
                row++;
            }
        }

        // Stub implementations
        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
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
            
            // Split layout: Calendar on left (55%), time slots on right (45%)
            int calendarWidth = (int)(bounds.Width * 0.55f);
            int timeSlotWidth = bounds.Width - calendarWidth - 1;
            
            var calendarBounds = new Rectangle(bounds.X, bounds.Y, calendarWidth, bounds.Height);
            var timeSlotBounds = new Rectangle(bounds.X + calendarWidth + 1, bounds.Y, timeSlotWidth, bounds.Height);
            
            // CALENDAR SECTION LAYOUT
            int padding = 8;
            int currentY = calendarBounds.Y + padding;

            layout.HeaderRect = new Rectangle(calendarBounds.X + padding, currentY, calendarBounds.Width - padding * 2 - 60, 32);
            
            int navButtonSize = 28;
            layout.PreviousButtonRect = new Rectangle(calendarBounds.Right - padding - navButtonSize * 2 - 6, currentY + 2, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(calendarBounds.Right - padding - navButtonSize, currentY + 2, navButtonSize, navButtonSize);

            currentY += 40;

            layout.DayNamesRect = new Rectangle(calendarBounds.X + padding, currentY, calendarBounds.Width - padding * 2, 24);
            currentY += 28;

            int gridWidth = calendarBounds.Width - padding * 2;
            layout.CalendarGridRect = new Rectangle(calendarBounds.X + padding, currentY, gridWidth, calendarBounds.Height - currentY - padding);

            layout.CellWidth = gridWidth / 7;
            layout.CellHeight = (layout.CalendarGridRect.Height) / 6;
            layout.DayCellRects = new List<Rectangle>();

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    layout.DayCellRects.Add(new Rectangle(
                        layout.CalendarGridRect.X + col * layout.CellWidth,
                        layout.CalendarGridRect.Y + row * layout.CellHeight,
                        layout.CellWidth,
                        layout.CellHeight
                    ));
                }
            }

            // TIME SLOT SECTION LAYOUT (for hit handler)
            layout.TimePickerRect = timeSlotBounds;
            layout.TimeSlotRects = new List<Rectangle>();
            
            int timeSlotPadding = 12;
            int headerHeight = 32;
            int slotHeight = 44;
            int slotGap = 4;
            int timeControlsHeight = 100;
            int startY = timeSlotBounds.Y + timeSlotPadding + headerHeight;

            for (int hour = 8; hour <= 20; hour++)
            {
                var slotBounds = new Rectangle(
                    timeSlotBounds.X + timeSlotPadding,
                    startY + (hour - 8) * slotHeight,
                    timeSlotBounds.Width - timeSlotPadding * 2,
                    slotHeight - slotGap
                );

                // Stop if slot exceeds visible area (leave room for time controls)
                if (slotBounds.Bottom > timeSlotBounds.Bottom - timeControlsHeight)
                    break;

                layout.TimeSlotRects.Add(slotBounds);
            }

            // TIME CONTROLS LAYOUT (hour/minute spinners at bottom)
            var timeControlBounds = new Rectangle(
                timeSlotBounds.X,
                timeSlotBounds.Bottom - timeControlsHeight,
                timeSlotBounds.Width,
                90
            );

            int controlPadding = 12;
            int labelWidth = 40;
            int controlWidth = 60;
            int controlHeight = 44;
            int spacing = 8;
            int controlY = timeControlBounds.Y + 32; // After "Precise Time" label

            // Hour spinner
            layout.TimeHourRect = new Rectangle(
                timeControlBounds.X + controlPadding + labelWidth,
                controlY,
                controlWidth,
                controlHeight
            );
            layout.TimeHourUpRect = new Rectangle(layout.TimeHourRect.X, layout.TimeHourRect.Y, controlWidth, 14);
            layout.TimeHourDownRect = new Rectangle(layout.TimeHourRect.X, layout.TimeHourRect.Bottom - 14, controlWidth, 14);

            // Minute spinner
            int minuteX = layout.TimeHourRect.Right + spacing + 10 + labelWidth;
            layout.TimeMinuteRect = new Rectangle(
                minuteX,
                controlY,
                controlWidth,
                controlHeight
            );
            layout.TimeMinuteUpRect = new Rectangle(layout.TimeMinuteRect.X, layout.TimeMinuteRect.Y, controlWidth, 14);
            layout.TimeMinuteDownRect = new Rectangle(layout.TimeMinuteRect.X, layout.TimeMinuteRect.Bottom - 14, controlWidth, 14);

            // Register all hit areas with BaseControl's hit test system
            _owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);

            return layout;
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

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Calendar(55%) + TimeSlots(45%) split layout
            int width = 500;
            int height = 400;

            if (properties.ShowCustomQuickDates)
            {
                height += 60;
            }

            return new Size(width, height);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Minimum for readable calendar + time slots
            // Calendar needs: ~250px, TimeSlots needs: ~200px
            return new Size(420, 360);
        }
    }
}
