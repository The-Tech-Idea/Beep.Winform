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
    /// Compact Date Picker Painter
    /// Minimal chrome, tight spacing, smaller cells for dropdown scenarios
    /// Visual styling follows BeepTheme
    /// </summary>
    public class CompactDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.Compact;

        public CompactDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var layout = CalculateLayout(bounds, properties);

            // Minimal background - no border
            PaintBackground(g, bounds);

            // Compact header
            PaintCompactHeader(g, layout.HeaderRect, displayMonth.ToString("MMM yyyy"), hoverState);
            
            PaintNavigationButton(g, layout.PreviousButtonRect, false, 
                hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));
            
            PaintNavigationButton(g, layout.NextButtonRect, true,
                hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));

            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);

            // Compact today button (clamped to bounds)
            int bottomSpace = bounds.Bottom - (layout.CalendarGridRect.Bottom + 4);
            if (bottomSpace >= 20)
            {
                int btnHeight = Math.Min(28, bottomSpace);
                var todayButtonRect = new Rectangle(bounds.X + 8, layout.CalendarGridRect.Bottom + 4, bounds.Width - 16, btnHeight);
                PaintTodayButton(g, todayButtonRect, hoverState, properties);
            }
        }

        private void PaintBackground(Graphics g, Rectangle bounds)
        {
            var bgColor = _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }
            // No border for compact style - BaseControl painter handles outer border if any
        }

        private void PaintCompactHeader(Graphics g, Rectangle bounds, string headerText, DateTimePickerHoverState hoverState)
        {
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(headerText, font, brush, bounds, format);
            }
        }

        private void PaintTodayButton(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState, DateTimePickerProperties properties)
        {
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var accentColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 8.5f);

            bool isHovered = false; // TODO: Add hit area
            
            if (isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            using (var brush = new SolidBrush(accentColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(properties?.TodayButtonText ?? "Today", font, brush, bounds, format);
            }
        }

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);

            // Tighter cell padding for compact style
            cellBounds.Inflate(-1, -1);

            // Paint selection - simple fill, no border
            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
            }
            else if (isPressed)
            {
                var pressedColor = ControlPaint.Dark(accentColor, 0.1f);
                using (var brush = new SolidBrush(pressedColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
                textColor = _theme?.CalendarHoverForeColor ?? textColor;
            }

            // Today indicator - small dot
            if (isToday && !isSelected)
            {
                using (var brush = new SolidBrush(todayColor))
                {
                    g.FillEllipse(brush, cellBounds.X + cellBounds.Width / 2 - 2, cellBounds.Bottom - 6, 4, 4);
                }
            }

            // Day number - smaller font
            var dayText = date.Day.ToString();
            var font = new Font(_theme?.FontName ?? "Segoe UI", 8.5f);
            
            if (isDisabled)
            {
                textColor = Color.FromArgb(190, 190, 190);
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
            PaintCompactHeader(g, headerBounds, headerText, null);
        }

        public void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed)
        {
            var iconColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);

            if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, buttonBounds);
                }
            }

            // Smaller arrows for compact style
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
            var textColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(140, 140, 140);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 7.5f);

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
                    // Show only first letter for compactness
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
            // Treat bounds as content area; keep padding minimal to match container
            int padding = 6; // Minimal padding
            int currentY = bounds.Y + padding;

            // Compact header
            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2 - 52, 26);
            
            int navButtonSize = 22; // Smaller nav buttons
            layout.PreviousButtonRect = new Rectangle(bounds.Right - padding - navButtonSize * 2 - 4, currentY + 1, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.Right - padding - navButtonSize, currentY + 1, navButtonSize, navButtonSize);

            currentY += 28;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 18);
            currentY += 22;

            int gridWidth = bounds.Width - padding * 2;
            // Fit grid to available space to avoid trailing empty area that may appear as black bands
            int availableHeight = bounds.Bottom - currentY - padding;
            int gridHeight = Math.Max(108, Math.Min(availableHeight, 180)); // Compact grid
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

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Padding(6*2=12) + Header(28) + DayNames(22) + Grid(150) + TodayBtn(32) = 244px
            int width = 280;
            int height = 260;

            if (properties.ShowCustomQuickDates)
            {
                height += 40;
            }

            return new Size(width, height);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Padding(6*2=12) + Header(28) + DayNames(22) + MinGrid(108) + TodayBtn(optional 32) = 202px
            // Using 180px for compact layout without today button
            int padding = 6;
            int headerHeight = 28;
            int dayNamesHeight = 22;
            int minGridHeight = 108;
            int minHeight = padding * 2 + headerHeight + dayNamesHeight + minGridHeight;
            
            int minWidth = 7 * 30 + padding * 2; // 7 cells * 30px + padding = 222px
            
            return new Size(Math.Max(minWidth, 240), Math.Max(minHeight, 180));
        }
    }
}
