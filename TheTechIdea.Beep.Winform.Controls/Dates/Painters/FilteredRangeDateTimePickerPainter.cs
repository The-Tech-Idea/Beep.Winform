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
    /// Filtered Range DateTimePicker Painter
    /// Left sidebar with quick filter buttons (Past Week/Month/3M/6M/Year/Century)
    /// Dual calendar with year dropdowns
    /// Time pickers at bottom
    /// Reset Date and Show Results buttons
    /// Inspired by analytics/reporting date range selectors
    /// </summary>
    public class FilteredRangeDateTimePickerPainter : IDateTimePickerPainter
    {
        private readonly BeepDateTimePicker _owner;
        private readonly IBeepTheme _theme;
        private string _selectedFilter = null;

        public DatePickerMode Mode => DatePickerMode.FilteredRange;

        public FilteredRangeDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bgColor = _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            int sidebarWidth = (int)(bounds.Width * 0.25f);
            var sidebarBounds = new Rectangle(bounds.X, bounds.Y, sidebarWidth, bounds.Height);
            var mainBounds = new Rectangle(bounds.X + sidebarWidth, bounds.Y, bounds.Width - sidebarWidth, bounds.Height);

            PaintFilterSidebar(g, sidebarBounds, hoverState);
            PaintMainContent(g, mainBounds, properties, displayMonth, hoverState);
        }

        private void PaintFilterSidebar(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var sidebarBg = Color.FromArgb(250, 250, 250);
            using (var brush = new SolidBrush(sidebarBg))
            {
                g.FillRectangle(brush, bounds);
            }

            int padding = 12;
            int currentY = bounds.Y + padding;

            // Title
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var titleRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 24);
                g.DrawString("Quick Filters", font, brush, titleRect);
            }
            currentY += 36;

            // Filter buttons
            string[] filters = { "Past Week", "Past Month", "Past 3 Months", "Past 6 Months", "Past Year", "Past Century" };
            int buttonHeight = 32;
            int buttonSpacing = 8;

            for (int i = 0; i < filters.Length; i++)
            {
                var buttonBounds = new Rectangle(
                    bounds.X + padding,
                    currentY,
                    bounds.Width - padding * 2,
                    buttonHeight
                );
                bool isSelected = _selectedFilter == filters[i];
                PaintFilterButton(g, buttonBounds, filters[i], isSelected, false);
                currentY += buttonHeight + buttonSpacing;
            }
        }

        private void PaintFilterButton(Graphics g, Rectangle bounds, string text, bool isSelected, bool isHovered)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var bgColor = isSelected ? accentColor : (isHovered ? Color.FromArgb(240, 240, 240) : Color.White);
            var textColor = isSelected ? Color.White : (_theme?.ForeColor ?? Color.Black);
            var borderColor = isSelected ? accentColor : Color.FromArgb(220, 220, 220);

            using (var path = GetRoundedRectPath(bounds, 6))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                var textRect = new Rectangle(bounds.X + 10, bounds.Y, bounds.Width - 20, bounds.Height);
                g.DrawString(text, font, brush, textRect, format);
            }
        }

        private void PaintMainContent(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            int padding = 12;
            int currentY = bounds.Y + padding;

            // Dual calendar with year selectors
            int calendarHeight = (int)(bounds.Height * 0.65f);
            var dualCalendarBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, calendarHeight);
            PaintDualCalendarWithYearSelector(g, dualCalendarBounds, properties, displayMonth, hoverState);
            currentY += calendarHeight + 10;

            // Time pickers row
            int timePickerHeight = 50;
            var timePickerBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, timePickerHeight);
            PaintTimePickerRow(g, timePickerBounds, hoverState);
            currentY += timePickerHeight + 10;

            // Action buttons
            int actionHeight = 40;
            var actionBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, actionHeight);
            PaintActionButtonsRow(g, actionBounds, hoverState);
        }

        private void PaintDualCalendarWithYearSelector(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            int calendarWidth = (bounds.Width - 16) / 2;
            var leftCalendarBounds = new Rectangle(bounds.X, bounds.Y, calendarWidth, bounds.Height);
            var rightCalendarBounds = new Rectangle(bounds.X + calendarWidth + 16, bounds.Y, calendarWidth, bounds.Height);

            // Paint year selector and calendar for left
            PaintSingleCalendarWithYear(g, leftCalendarBounds, displayMonth, properties, hoverState, true);

            // Paint year selector and calendar for right (next month)
            var nextMonth = displayMonth.AddMonths(1);
            PaintSingleCalendarWithYear(g, rightCalendarBounds, nextMonth, properties, hoverState, false);

            // Paint range highlight across both
            if (_owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue)
            {
                PaintRangeOverlay(g, leftCalendarBounds, rightCalendarBounds, displayMonth, nextMonth, _owner.RangeStartDate.Value, _owner.RangeEndDate.Value, properties);
            }
        }

        private void PaintSingleCalendarWithYear(Graphics g, Rectangle bounds, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState, bool isLeft)
        {
            int padding = 6;
            int currentY = bounds.Y;

            // Year selector dropdown
            var yearBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 28);
            PaintYearSelector(g, yearBounds, displayMonth.Year, false);
            currentY += 34;

            // Month/Year header
            var headerText = displayMonth.ToString("MMMM yyyy");
            var headerBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 24);
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText, font, brush, headerBounds, format);
            }
            currentY += 28;

            // Day names
            var dayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 20);
            PaintDayNamesHeader(g, dayNamesRect, properties.FirstDayOfWeek);
            currentY += 24;

            // Calendar grid
            int gridWidth = bounds.Width - padding * 2;
            int availableHeight = bounds.Bottom - currentY - padding;
            var gridBounds = new Rectangle(bounds.X + padding, currentY, gridWidth, availableHeight);
            var layout = CalculateSingleCalendarGrid(gridBounds);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);
        }

        private void PaintYearSelector(Graphics g, Rectangle bounds, int year, bool isHovered)
        {
            var bgColor = isHovered ? Color.FromArgb(240, 240, 240) : Color.White;
            var borderColor = Color.FromArgb(200, 200, 200);

            using (var path = GetRoundedRectPath(bounds, 4))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(year.ToString(), font, brush, bounds, format);
            }

            // Dropdown arrow
            var arrowPoints = new Point[]
            {
                new Point(bounds.Right - 18, bounds.Y + bounds.Height / 2 - 2),
                new Point(bounds.Right - 12, bounds.Y + bounds.Height / 2 + 4),
                new Point(bounds.Right - 6, bounds.Y + bounds.Height / 2 - 2)
            };
            using (var brush = new SolidBrush(Color.FromArgb(128, 128, 128)))
            {
                g.FillPolygon(brush, arrowPoints);
            }
        }

        private void PaintRangeOverlay(Graphics g, Rectangle leftBounds, Rectangle rightBounds, DateTime leftMonth, DateTime rightMonth, DateTime rangeStart, DateTime rangeEnd, DateTimePickerProperties properties)
        {
            var rangeColor = Color.FromArgb(40, _theme?.AccentColor ?? Color.FromArgb(0, 120, 215));
            using (var brush = new SolidBrush(rangeColor))
            {
                // Calculate grid areas
                var leftLayout = CalculateSingleCalendarGrid(new Rectangle(leftBounds.X + 6, leftBounds.Y + 86, leftBounds.Width - 12, leftBounds.Height - 92));
                var rightLayout = CalculateSingleCalendarGrid(new Rectangle(rightBounds.X + 6, rightBounds.Y + 86, rightBounds.Width - 12, rightBounds.Height - 92));

                // Highlight in left calendar
                for (int row = 0; row < 6; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        var cellDate = GetDateFromCell(row, col, leftMonth, properties.FirstDayOfWeek);
                        if (cellDate >= rangeStart.Date && cellDate <= rangeEnd.Date)
                        {
                            var rect = leftLayout.DayCellRects[row, col];
                            rect.Inflate(-2, -2);
                            g.FillRectangle(brush, rect);
                        }
                    }
                }

                // Highlight in right calendar
                for (int row = 0; row < 6; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        var cellDate = GetDateFromCell(row, col, rightMonth, properties.FirstDayOfWeek);
                        if (cellDate >= rangeStart.Date && cellDate <= rangeEnd.Date)
                        {
                            var rect = rightLayout.DayCellRects[row, col];
                            rect.Inflate(-2, -2);
                            g.FillRectangle(brush, rect);
                        }
                    }
                }
            }
        }

        private void PaintTimePickerRow(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            int labelWidth = 50;
            int timePickerWidth = (bounds.Width - labelWidth * 2 - 30) / 2;

            // From label
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var fromLabelRect = new Rectangle(bounds.X, bounds.Y, labelWidth, bounds.Height);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString("From:", font, brush, fromLabelRect, format);
            }

            // From time picker
            var fromTimeRect = new Rectangle(bounds.X + labelWidth, bounds.Y + 5, timePickerWidth, bounds.Height - 10);
            PaintTimeInput(g, fromTimeRect, _owner.RangeStartDate?.TimeOfDay ?? TimeSpan.Zero, false);

            // To label
            int toX = bounds.X + labelWidth + timePickerWidth + 15;
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var toLabelRect = new Rectangle(toX, bounds.Y, labelWidth, bounds.Height);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString("To:", font, brush, toLabelRect, format);
            }

            // To time picker
            var toTimeRect = new Rectangle(toX + labelWidth, bounds.Y + 5, timePickerWidth, bounds.Height - 10);
            PaintTimeInput(g, toTimeRect, _owner.RangeEndDate?.TimeOfDay ?? TimeSpan.Zero, false);
        }

        private void PaintTimeInput(Graphics g, Rectangle bounds, TimeSpan time, bool isHovered)
        {
            var bgColor = isHovered ? Color.FromArgb(240, 240, 240) : Color.White;
            var borderColor = Color.FromArgb(200, 200, 200);

            using (var path = GetRoundedRectPath(bounds, 4))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            var timeText = time.ToString(@"hh\:mm");
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 10f))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(timeText, font, brush, bounds, format);
            }
        }

        private void PaintActionButtonsRow(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            int buttonWidth = (bounds.Width - 12) / 2;

            // Reset Date button
            var resetBounds = new Rectangle(bounds.X, bounds.Y, buttonWidth, bounds.Height);
            PaintActionButton(g, resetBounds, "Reset Date", false, false, false);

            // Show Results button (primary)
            var resultsBounds = new Rectangle(bounds.X + buttonWidth + 12, bounds.Y, buttonWidth, bounds.Height);
            PaintActionButton(g, resultsBounds, "Show Results", true, false, false);
        }

        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var bgColor = isPrimary ? accentColor : Color.White;
            var textColor = isPrimary ? Color.White : (_theme?.ForeColor ?? Color.Black);
            var borderColor = isPrimary ? accentColor : Color.FromArgb(200, 200, 200);

            if (isHovered && !isPrimary)
            {
                bgColor = Color.FromArgb(240, 240, 240);
            }

            using (var path = GetRoundedRectPath(buttonBounds, 6))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(borderColor, isPrimary ? 2 : 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold))
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, font, brush, buttonBounds, format);
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

                bool isRangeStart = _owner.RangeStartDate.HasValue && _owner.RangeStartDate.Value.Date == date.Date;
                bool isRangeEnd = _owner.RangeEndDate.HasValue && _owner.RangeEndDate.Value.Date == date.Date;
                bool isSelected = isRangeStart || isRangeEnd;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInRange = _owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue &&
                                date >= _owner.RangeStartDate.Value.Date && date <= _owner.RangeEndDate.Value.Date;

                PaintDayCell(g, cellRect, date, isSelected, isToday, isDisabled, isHovered, isPressed, isInRange);

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
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);

            cellBounds.Inflate(-2, -2);

            // Paint range background if in range
            if (isInRange && !isSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(40, accentColor)))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }

            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
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
        public void PaintHeader(Graphics g, Rectangle headerBounds, string headerText, bool showNavigation, bool isHovered) { }
        public void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed) { }
        public void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState) { }
        public void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed) { }
        public void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed) { }
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            int sidebarWidth = (int)(bounds.Width * 0.25f);
            int mainX = bounds.X + sidebarWidth + 12;
            int mainWidth = bounds.Width - sidebarWidth - 24;
            int calendarWidth = (mainWidth - 16) / 2;
            var leftCalendarBounds = new Rectangle(mainX, bounds.Y + 98, calendarWidth, (int)(bounds.Height * 0.55f));
            return CalculateSingleCalendarGrid(leftCalendarBounds);
        }

        private DateTimePickerLayout CalculateSingleCalendarGrid(Rectangle bounds)
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
            return new Size(720, 480);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            return new Size(640, 420);
        }
    }
}
