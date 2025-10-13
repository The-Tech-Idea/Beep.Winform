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
    /// Week View Date Picker Painter
    /// Week-based calendar with full week selection
    /// Visual styling follows BeepTheme
    /// </summary>
    public class WeekViewDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.WeekView;

        public WeekViewDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
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

            // Paint week numbers column
            PaintWeekNumbers(g, new Rectangle(bounds.X + 16, layout.DayNamesRect.Y, 40, layout.CalendarGridRect.Height + 28), displayMonth, properties.FirstDayOfWeek, hoverState);

            // Paint day names (offset for week numbers)
            var dayNamesRect = new Rectangle(layout.DayNamesRect.X + 44, layout.DayNamesRect.Y, layout.DayNamesRect.Width - 44, layout.DayNamesRect.Height);
            PaintDayNamesHeader(g, dayNamesRect, properties.FirstDayOfWeek);

            // Paint calendar grid with week highlighting
            PaintWeekBasedCalendarGrid(g, layout, displayMonth, properties, hoverState);

            // Paint selected week info
            if (_owner.RangeStartDate.HasValue)
            {
                var weekInfoRect = new Rectangle(bounds.X + 16, layout.CalendarGridRect.Bottom + 12, bounds.Width - 32, 32);
                PaintSelectedWeekInfo(g, weekInfoRect);
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

        private void PaintSelectedWeekInfo(Graphics g, Rectangle bounds)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var bgColor = Color.FromArgb(250, 250, 250);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(230, 230, 230);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 10f, FontStyle.Bold);

            // Get week start/end
            var weekStart = _owner.RangeStartDate.Value;
            var weekEnd = _owner.RangeEndDate ?? weekStart.AddDays(6);

            // Background
            using (var brush = new SolidBrush(bgColor))
            using (var pen = new Pen(borderColor, 1))
            {
                g.FillRectangle(brush, bounds);
                g.DrawRectangle(pen, bounds);
            }

            int weekNumber = GetWeekNumber(weekStart);
            string weekText = $"Week {weekNumber}: {weekStart:MMM d} — {weekEnd:MMM d}";

            using (var brush = new SolidBrush(accentColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(weekText, boldFont, brush, bounds, format);
            }
        }

        private int GetWeekNumber(DateTime date)
        {
            var culture = CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState)
        {
            var textColor = _theme?.SecondaryTextColor ?? Color.FromArgb(120, 120, 120);
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(230, 230, 230);
            var font = new Font("Segoe UI", 8f, FontStyle.Bold);

            // Column background
            using (var brush = new SolidBrush(Color.FromArgb(248, 248, 248)))
            {
                g.FillRectangle(brush, weekColumnBounds);
            }

            // Border
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, weekColumnBounds.Right, weekColumnBounds.Y, weekColumnBounds.Right, weekColumnBounds.Bottom);
            }

            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            int dayOffset = ((int)firstDayOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

            int cellHeight = (weekColumnBounds.Height - 28) / 6;
            int currentY = weekColumnBounds.Y + 28;

            for (int week = 0; week < 6; week++)
            {
                int dayIndex = week * 7 - dayOffset + 1;
                DateTime weekDate;

                if (dayIndex < 1)
                {
                    var prevMonth = displayMonth.AddMonths(-1);
                    var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
                    weekDate = new DateTime(prevMonth.Year, prevMonth.Month, daysInPrevMonth + dayIndex);
                }
                else
                {
                    var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);
                    if (dayIndex > daysInMonth)
                    {
                        var nextMonth = displayMonth.AddMonths(1);
                        weekDate = new DateTime(nextMonth.Year, nextMonth.Month, dayIndex - daysInMonth);
                    }
                    else
                    {
                        weekDate = new DateTime(displayMonth.Year, displayMonth.Month, dayIndex);
                    }
                }

                int weekNum = GetWeekNumber(weekDate);
                var weekRect = new Rectangle(weekColumnBounds.X, currentY, weekColumnBounds.Width, cellHeight);

                PaintWeekNumber(g, weekRect, weekNum, false);
                currentY += cellHeight;
            }
        }

        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered)
        {
            var textColor = _theme?.SecondaryTextColor ?? Color.FromArgb(120, 120, 120);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var font = new Font("Segoe UI", 8f, FontStyle.Bold);

            if (isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, weekBounds);
                }
            }

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(weekNumber.ToString(), font, brush, weekBounds, format);
            }
        }

        private void PaintWeekBasedCalendarGrid(Graphics g, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState)
        {
            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);

            int firstDayOfWeek = (int)properties.FirstDayOfWeek;
            int dayOffset = ((int)firstDayOfMonth.DayOfWeek - firstDayOfWeek + 7) % 7;

            var prevMonth = displayMonth.AddMonths(-1);
            var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            int row = 0, col = 0;

            // Adjust cell rects for week numbers column
            var adjustedCellRects = new Rectangle[6, 7];
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    adjustedCellRects[r, c] = new Rectangle(
                        layout.DayCellRects[r, c].X + 44,
                        layout.DayCellRects[r, c].Y,
                        layout.DayCellRects[r, c].Width,
                        layout.DayCellRects[r, c].Height
                    );
                }
            }

            for (int i = 0; i < dayOffset; i++)
            {
                var day = daysInPrevMonth - dayOffset + i + 1;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, day);
                var cellRect = adjustedCellRects[row, col];
                bool isInWeek = IsInSelectedWeek(date);
                PaintWeekDayCell(g, cellRect, date, false, false, true, false, false, isInWeek);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = adjustedCellRects[row, col];

                bool isSelected = _owner.SelectedDate.HasValue && _owner.SelectedDate.Value.Date == date.Date;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInWeek = IsInSelectedWeek(date);

                PaintWeekDayCell(g, cellRect, date, isSelected, isToday, isDisabled, isHovered, isPressed, isInWeek);

                col++;
                if (col >= 7) { col = 0; row++; }
            }

            var nextMonth = displayMonth.AddMonths(1);
            while (row < 6)
            {
                for (int day = 1; col < 7; day++)
                {
                    var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
                    var cellRect = adjustedCellRects[row, col];
                    bool isInWeek = IsInSelectedWeek(date);
                    PaintWeekDayCell(g, cellRect, date, false, false, true, false, false, isInWeek);
                    col++;
                }
                col = 0;
                row++;
            }
        }

        private bool IsInSelectedWeek(DateTime date)
        {
            if (!_owner.RangeStartDate.HasValue) return false;

            var weekStart = _owner.RangeStartDate.Value;
            var weekEnd = _owner.RangeEndDate ?? weekStart.AddDays(6);

            return date >= weekStart && date <= weekEnd;
        }

        private void PaintWeekDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInWeek)
        {
            
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var weekColor = Color.FromArgb(40, accentColor);

            cellBounds.Inflate(-1, -1);

            // Week highlight (full row)
            if (isInWeek)
            {
                using (var brush = new SolidBrush(weekColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }

            // Hover
            if (isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }

            // Today indicator
            if (isToday)
            {
                using (var pen = new Pen(accentColor, 2))
                {
                    g.DrawRectangle(pen, cellBounds.X + 2, cellBounds.Y + 2, cellBounds.Width - 4, cellBounds.Height - 4);
                }
            }

            // Day number
            var dayText = date.Day.ToString();
            var font = new Font("Segoe UI", 9f, isInWeek ? FontStyle.Bold : FontStyle.Regular);

            if (isDisabled)
            {
                textColor = Color.FromArgb(180, 180, 180);
            }
            else if (isInWeek)
            {
                textColor = accentColor;
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

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange)
        {
            PaintWeekDayCell(g, cellBounds, date, isSelected, isToday, isDisabled, isHovered, isPressed, isInRange);
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

        // Stub implementations
        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
        public void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState) { }
        public void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed) { }
        public void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed) { }
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }

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

            int gridWidth = bounds.Width - padding * 2 - 44; // Account for week numbers
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, 252);

            layout.CellWidth = gridWidth / 7;
            layout.CellHeight = 252 / 6;
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
            return new Size(400, 450);
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

            return result;
        }
    }
}
