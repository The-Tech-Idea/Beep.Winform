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
    /// Header DateTimePicker Painter
    /// Prominent large colored header with full date display
    /// Clean compact calendar grid below with minimal styling
    /// Inspired by modern mobile date picker designs
    /// </summary>
    public class HeaderDateTimePickerPainter : IDateTimePickerPainter
    {
        private readonly BeepDateTimePicker _owner;
        private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.Header;

        public HeaderDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int padding = 8;
            int currentY = bounds.Y;

            // Large prominent header with selected date
            int headerHeight = 80;
            var headerBounds = new Rectangle(bounds.X, currentY, bounds.Width, headerHeight);
            PaintLargeHeader(g, headerBounds, _owner.SelectedDate ?? DateTime.Today);
            currentY += headerHeight;

            // Calendar section
            var calendarBounds = new Rectangle(bounds.X + padding, currentY + padding, bounds.Width - padding * 2, bounds.Height - headerHeight - padding * 2);
            PaintCalendarSection(g, calendarBounds, properties, displayMonth, hoverState);
        }

        private void PaintLargeHeader(Graphics g, Rectangle bounds, DateTime selectedDate)
        {
            // Header background with accent color
            var headerColor = _theme?.AccentColor ?? Color.FromArgb(100, 180, 220);
            using (var brush = new SolidBrush(headerColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Format date as "Friday, April 12"
            var dayOfWeekText = selectedDate.ToString("dddd, MMMM dd", CultureInfo.CurrentCulture);
            var yearText = selectedDate.ToString("yyyy");

            int padding = 16;
            int currentY = bounds.Y + padding;

            // Day of week and date (large)
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 20f, FontStyle.Regular))
            using (var brush = new SolidBrush(Color.White))
            {
                var textRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 40);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
                g.DrawString(dayOfWeekText, font, brush, textRect, format);
            }
            currentY += 42;

            // Year (medium)
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 14f, FontStyle.Regular))
            using (var brush = new SolidBrush(Color.FromArgb(230, 255, 255, 255)))
            {
                var textRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 24);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
                g.DrawString(yearText, font, brush, textRect, format);
            }
        }

        private void PaintCalendarSection(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            var bgColor = _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            int padding = 6;
            int currentY = bounds.Y;

            // Month/Year header (subtle)
            var headerText = displayMonth.ToString("MMMM yyyy");
            var headerRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 24);
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText, font, brush, headerRect, format);
            }
            currentY += 28;

            // Day names
            var dayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 20);
            PaintDayNamesHeader(g, dayNamesRect, properties.FirstDayOfWeek);
            currentY += 24;

            // Calendar grid
            int gridWidth = bounds.Width - padding * 2;
            int availableHeight = bounds.Bottom - currentY - padding;
            int gridHeight = Math.Max(120, Math.Min(availableHeight, 200));
            var gridBounds = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);
            var layout = CalculateCalendarGrid(gridBounds);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);
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
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(100, 180, 220);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);

            cellBounds.Inflate(-3, -3);

            // Background
            if (isSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(180, 180, 180)))
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

            // Today indicator (subtle)
            if (isToday && !isSelected)
            {
                using (var pen = new Pen(accentColor, 1))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

            // Day number
            var dayText = date.Day.ToString();
            var font = new Font(_theme?.FontName ?? "Segoe UI", 9f);

            if (isDisabled)
            {
                textColor = Color.FromArgb(200, 200, 200);
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

        public void PaintDayNamesHeader(Graphics g, Rectangle headerBounds, DatePickerFirstDayOfWeek firstDayOfWeek)
        {
            var textColor = _theme?.SecondaryTextColor ?? Color.FromArgb(120, 120, 120);
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
                    g.DrawString(dayNames[dayIndex].Substring(0, 3).ToUpper(), font, brush, cellRect, format);
                }
            }
        }

        // Stub implementations
        public void PaintHeader(Graphics g, Rectangle headerBounds, string headerText, bool showNavigation, bool isHovered) { }
        public void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed) { }
        public void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState) { }
        public void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed) { }
        public void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed)
        {
            // Minimal, theme-consistent quick button for header style
            var bgColor = _theme?.BackgroundColor ?? Color.White;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var pressedColor = Color.FromArgb(235, 235, 235);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var textColor = _theme?.ForeColor ?? Color.Black;

            var fill = isPressed ? pressedColor : isHovered ? hoverColor : bgColor;

            using (var brush = new SolidBrush(fill))
            {
                g.FillRectangle(brush, buttonBounds);
            }

            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, buttonBounds);
            }

            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, font, brush, buttonBounds, format);
            }
        }
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }
        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            int headerHeight = 80;
            int padding = 8;
            var calendarBounds = new Rectangle(
                bounds.X + padding, 
                bounds.Y + headerHeight + padding, 
                bounds.Width - padding * 2, 
                bounds.Height - headerHeight - padding * 2
            );
            
            var layout = new DateTimePickerLayout();
            int currentY = calendarBounds.Y + 6;

            layout.HeaderRect = new Rectangle(calendarBounds.X + 6, currentY, calendarBounds.Width - 12, 24);
            currentY += 28;

            layout.DayNamesRect = new Rectangle(calendarBounds.X + 6, currentY, calendarBounds.Width - 12, 20);
            currentY += 24;

            int gridWidth = calendarBounds.Width - 12;
            int availableHeight = calendarBounds.Bottom - currentY - 6;
            int gridHeight = Math.Max(120, Math.Min(availableHeight, 200));
            layout.CalendarGridRect = new Rectangle(calendarBounds.X + 6, currentY, gridWidth, gridHeight);

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

        private DateTimePickerLayout CalculateCalendarGrid(Rectangle bounds)
        {
            var layout = new DateTimePickerLayout();
            layout.CalendarGridRect = bounds;

            layout.CellWidth = bounds.Width / 7;
            layout.CellHeight = bounds.Height / 6;
            layout.DayCellRects = new Rectangle[6, 7];

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    layout.DayCellRects[row, col] = new Rectangle(
                        bounds.X + col * layout.CellWidth,
                        bounds.Y + row * layout.CellHeight,
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

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            return new Size(380, 400);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            return new Size(320, 350);
        }
    }
}
