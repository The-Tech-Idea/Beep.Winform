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
    /// Dual Calendar Date Picker Painter
    /// Side-by-side month calendars for easy range selection across months
    /// Visual styling follows BeepTheme
    /// </summary>
    public class DualCalendarDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.DualCalendar;

        public DualCalendarDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Paint background
            PaintBackground(g, bounds);

            int padding = 16;
            int gap = 12;
            int calendarWidth = (bounds.Width - padding * 2 - gap) / 2;

            // Left calendar (current month)
            var leftCalendarBounds = new Rectangle(bounds.X + padding, bounds.Y + padding, calendarWidth, bounds.Height - padding * 2);
            PaintSingleCalendar(g, leftCalendarBounds, displayMonth, properties, hoverState, true);

            // Separator
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(230, 230, 230);
            using (var pen = new Pen(borderColor, 1))
            {
                int separatorX = bounds.X + padding + calendarWidth + gap / 2;
                g.DrawLine(pen, separatorX, bounds.Y + padding + 10, separatorX, bounds.Bottom - padding - 10);
            }

            // Right calendar (next month)
            var rightCalendarBounds = new Rectangle(bounds.X + padding + calendarWidth + gap, bounds.Y + padding, calendarWidth, bounds.Height - padding * 2);
            PaintSingleCalendar(g, rightCalendarBounds, displayMonth.AddMonths(1), properties, hoverState, false);

            // Range info at bottom
            if (_owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue)
            {
                var rangeInfoRect = new Rectangle(bounds.X + padding, bounds.Bottom - 50, bounds.Width - padding * 2, 34);
                PaintRangeInfo(g, rangeInfoRect);
            }
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

        private void PaintSingleCalendar(Graphics g, Rectangle bounds, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState, bool showNavigation)
        {
            var layout = CalculateSingleCalendarLayout(bounds, properties);

            // Header
            PaintHeader(g, layout.HeaderRect, displayMonth.ToString("MMM yyyy"), false, false);

            // Navigation only on left calendar
            if (showNavigation)
            {
                PaintNavigationButton(g, layout.PreviousButtonRect, false,
                    hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                    hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));

                PaintNavigationButton(g, layout.NextButtonRect, true,
                    hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                    hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));
            }

            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);
        }

        private void PaintRangeInfo(Graphics g, Rectangle bounds)
        {
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var bgColor = Color.FromArgb(250, 250, 250);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(230, 230, 230);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 10f, FontStyle.Bold);

            // Background
            using (var brush = new SolidBrush(bgColor))
            using (var pen = new Pen(borderColor, 1))
            {
                g.FillRectangle(brush, bounds);
                g.DrawRectangle(pen, bounds);
            }

            if (_owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue)
            {
                int days = (_owner.RangeEndDate.Value - _owner.RangeStartDate.Value).Days + 1;
                string rangeText = $"{_owner.RangeStartDate.Value:MMM d} — {_owner.RangeEndDate.Value:MMM d} ({days} day{(days != 1 ? "s" : "")})";

                using (var brush = new SolidBrush(accentColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(rangeText, boldFont, brush, bounds, format);
                }
            }
        }

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);
            var rangeColor = Color.FromArgb(50, accentColor);

            cellBounds.Inflate(-1, -1);

            // Range background
            if (isInRange && !isSelected)
            {
                using (var brush = new SolidBrush(rangeColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }

            // Selection (start or end)
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
            var font = new Font("Segoe UI", 9f);

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
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 11f, FontStyle.Bold);

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
            var font = new Font("Segoe UI", 8f);

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
                bool isInRange = IsInRange(date);
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
                bool isInRange = IsInRange(date);

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
                    bool isInRange = IsInRange(date);
                    PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                    col++;
                }
                col = 0;
                row++;
            }
        }

        private bool IsInRange(DateTime date)
        {
            if (!_owner.RangeStartDate.HasValue || !_owner.RangeEndDate.HasValue) return false;
            return date >= _owner.RangeStartDate.Value && date <= _owner.RangeEndDate.Value;
        }

        private DateTimePickerLayout CalculateSingleCalendarLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            int padding = 8;
            int currentY = bounds.Y;

            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2 - 60, 32);

            int navButtonSize = 28;
            layout.PreviousButtonRect = new Rectangle(bounds.X + 2, currentY + 2, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.Right - navButtonSize - 2, currentY + 2, navButtonSize, navButtonSize);

            currentY += 38;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 22);
            currentY += 26;

            int gridWidth = bounds.Width - padding * 2;
            int gridHeight = bounds.Height - (currentY - bounds.Y) - padding;
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
            return new DateTimePickerLayout();
        }

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth)
        {
            return new DateTimePickerHitTestResult();
        }

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Side-by-side calendars: (Calendar_Width * 2) + Gap + Padding
            // Each calendar: ~280px, Gap: 20px, Padding: 40px = 620px
            return new Size(640, 320);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Minimum for two readable calendars side-by-side
            // Each calendar: ~240px, Gap: 16px, Padding: 32px = 528px
            return new Size(560, 280);
        }
    }
}

