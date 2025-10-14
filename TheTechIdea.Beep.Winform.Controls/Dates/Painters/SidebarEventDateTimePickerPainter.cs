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
    /// Sidebar Event Calendar Painter
    /// Prominent left sidebar with large date display, event list, and create button
    /// Compact calendar grid on the right side
    /// Inspired by modern event/scheduling apps
    /// </summary>
    public class SidebarEventDateTimePickerPainter : IDateTimePickerPainter
    {
        private readonly BeepDateTimePicker _owner;
        private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.SidebarEvent;

        public SidebarEventDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Layout: Left sidebar (40%) + Right calendar (60%)
            int sidebarWidth = (int)(bounds.Width * 0.40f);
            int calendarWidth = bounds.Width - sidebarWidth;

            var sidebarBounds = new Rectangle(bounds.X, bounds.Y, sidebarWidth, bounds.Height);
            var calendarBounds = new Rectangle(bounds.X + sidebarWidth, bounds.Y, calendarWidth, bounds.Height);

            // Paint sidebar with accent background
            PaintSidebar(g, sidebarBounds, hoverState);

            // Paint calendar section
            PaintCalendarSection(g, calendarBounds, properties, displayMonth, hoverState);
        }

        private void PaintSidebar(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(76, 175, 80); // Green accent
            var textColor = Color.White;
            var secondaryTextColor = Color.FromArgb(230, 255, 230);

            // Sidebar background
            using (var brush = new SolidBrush(accentColor))
            {
                g.FillRectangle(brush, bounds);
            }

            int padding = 20;
            int currentY = bounds.Y + padding;

            // Large date display
            var selectedDate = _owner.SelectedDate ?? DateTime.Today;
            var dayNumber = selectedDate.Day.ToString();
            var dayName = selectedDate.ToString("dddd").ToUpper();

            // Day number (large)
            using (var largeFont = new Font(_theme?.FontName ?? "Segoe UI", 48f, FontStyle.Bold))
            using (var brush = new SolidBrush(textColor))
            {
                var daySize = TextUtils.MeasureText(g,dayNumber, largeFont);
                g.DrawString(dayNumber, largeFont, brush, bounds.X + padding, currentY);
                currentY += (int)daySize.Height;
            }

            // Day name
            using (var mediumFont = new Font(_theme?.FontName ?? "Segoe UI", 13f, FontStyle.Bold))
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(dayName, mediumFont, brush, bounds.X + padding, currentY);
                currentY += 30;
            }

            // Current Events section
            currentY += 20;
            using (var smallFont = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                g.DrawString("Current Events", smallFont, brush, bounds.X + padding, currentY);
                currentY += 25;
            }

            // Sample event entries (placeholder)
            var eventFont = new Font(_theme?.FontName ?? "Segoe UI", 8.5f);
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString("See Daily CS Image", eventFont, brush, bounds.X + padding, currentY);
                currentY += 20;
                g.DrawString("See Daily Events", eventFont, brush, bounds.X + padding, currentY);
                currentY += 35;
            }

            // Create Event button
            var buttonBounds = new Rectangle(bounds.X + padding, bounds.Bottom - 60, bounds.Width - padding * 2, 40);
            PaintCreateButton(g, buttonBounds, hoverState);
        }

        private void PaintCreateButton(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var buttonColor = Color.FromArgb(100, 255, 100, 100);
            var textColor = Color.White;

            using (var brush = new SolidBrush(buttonColor))
            using (var path = GetRoundedRectPath(bounds, 20))
            {
                g.FillPath(brush, path);
            }

            using (var pen = new Pen(Color.White, 2))
            using (var path = GetRoundedRectPath(bounds, 20))
            {
                g.DrawPath(pen, path);
            }

            // Plus icon
            int centerX = bounds.X + bounds.Width - 28;
            int centerY = bounds.Y + bounds.Height / 2;
            using (var pen = new Pen(textColor, 2))
            {
                g.DrawLine(pen, centerX - 6, centerY, centerX + 6, centerY);
                g.DrawLine(pen, centerX, centerY - 6, centerX, centerY + 6);
            }

            // Text
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 10f))
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString("Create an Event", font, brush, bounds.X + 12, centerY, format);
            }
        }

        private void PaintCalendarSection(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            var bgColor = _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            var layout = CalculateCalendarLayout(bounds, properties);

            // Paint month selector
            PaintMonthSelector(g, layout.HeaderRect, displayMonth, hoverState);

            // Paint nav buttons
            PaintNavigationButton(g, layout.PreviousButtonRect, false,
                hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));

            PaintNavigationButton(g, layout.NextButtonRect, true,
                hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));

            // Paint day names
            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);

            // Paint calendar grid
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);
        }

        private void PaintMonthSelector(Graphics g, Rectangle bounds, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;

            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 8f))
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                int monthWidth = bounds.Width / 6;
                for (int i = 0; i < 12; i++)
                {
                    int row = i / 6;
                    int col = i % 6;
                    var monthRect = new Rectangle(
                        bounds.X + col * monthWidth,
                        bounds.Y + row * 18,
                        monthWidth,
                        16
                    );

                    var currentMonth = i + 1;
                    var monthColor = currentMonth == displayMonth.Month ? _theme?.AccentColor ?? Color.Green : textColor;
                    using (var monthBrush = new SolidBrush(monthColor))
                    {
                        g.DrawString(months[i], font, monthBrush, monthRect, format);
                    }
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

            // Previous month days
            for (int i = 0; i < dayOffset; i++)
            {
                var day = daysInPrevMonth - dayOffset + i + 1;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, day);
                var cellRect = layout.DayCellRects[row, col];
                PaintDayCell(g, cellRect, date, false, false, true, false, false, false);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            // Current month days
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = layout.DayCellRects[row, col];

                bool isSelected = _owner.SelectedDate.HasValue && _owner.SelectedDate.Value.Date == date.Date;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);

                PaintDayCell(g, cellRect, date, isSelected, isToday, isDisabled, isHovered, isPressed, false);

                col++;
                if (col >= 7) { col = 0; row++; }
            }

            // Next month days
            var nextMonth = displayMonth.AddMonths(1);
            while (row < 6)
            {
                for (int day = 1; col < 7; day++)
                {
                    var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
                    var cellRect = layout.DayCellRects[row, col];
                    PaintDayCell(g, cellRect, date, false, false, true, false, false, false);
                    col++;
                }
                col = 0;
                row++;
            }
        }

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(76, 175, 80);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);

            cellBounds.Inflate(-2, -2);

            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = Color.White;
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
                using (var pen = new Pen(accentColor, 2))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

            var dayText = date.Day.ToString();
            var font = new Font(_theme?.FontName ?? "Segoe UI", 8.5f);

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
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText, font, brush, headerBounds, format);
            }
        }

        public void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed)
        {
            var iconColor = _theme?.ForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);

            if (isHovered || isPressed)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillEllipse(brush, buttonBounds);
                }
            }

            using (var pen = new Pen(iconColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = buttonBounds.X + buttonBounds.Width / 2;
                int centerY = buttonBounds.Y + buttonBounds.Height / 2;
                int size = 4;

                if (isNext)
                {
                    g.DrawLine(pen, centerX - 2, centerY - size, centerX + 2, centerY);
                    g.DrawLine(pen, centerX + 2, centerY, centerX - 2, centerY + size);
                }
                else
                {
                    g.DrawLine(pen, centerX + 2, centerY - size, centerX - 2, centerY);
                    g.DrawLine(pen, centerX - 2, centerY, centerX + 2, centerY + size);
                }
            }
        }

        public void PaintDayNamesHeader(Graphics g, Rectangle headerBounds, DatePickerFirstDayOfWeek firstDayOfWeek)
        {
            var textColor = _theme?.SecondaryTextColor ?? Color.FromArgb(128, 128, 128);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 8f);

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
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(dayNames[dayIndex].Substring(0, 2), font, brush, cellRect, format);
                }
            }
        }

        // Stub implementations
        public void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState) { }
        public void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed) { }
        public void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed) { }
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }
        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            // Split layout
            int sidebarWidth = (int)(bounds.Width * 0.40f);
            int calendarWidth = bounds.Width - sidebarWidth;
            var calendarBounds = new Rectangle(bounds.X + sidebarWidth, bounds.Y, calendarWidth, bounds.Height);

            return CalculateCalendarLayout(calendarBounds, properties);
        }

        private DateTimePickerLayout CalculateCalendarLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            int padding = 8;
            int currentY = bounds.Y + padding;

            // Month selector area
            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2 - 40, 40);

            int navButtonSize = 20;
            layout.PreviousButtonRect = new Rectangle(bounds.Right - padding - navButtonSize * 2 - 4, currentY + 10, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.Right - padding - navButtonSize, currentY + 10, navButtonSize, navButtonSize);

            currentY += 48;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 20);
            currentY += 24;

            int gridWidth = bounds.Width - padding * 2;
            int availableHeight = bounds.Bottom - currentY - padding;
            int gridHeight = Math.Max(100, Math.Min(availableHeight, 180));
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);

            layout.CellWidth = gridWidth / 7;
            layout.CellHeight = gridHeight / 6;
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

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            return new Size(560, 320);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            return new Size(480, 280);
        }
    }
}
