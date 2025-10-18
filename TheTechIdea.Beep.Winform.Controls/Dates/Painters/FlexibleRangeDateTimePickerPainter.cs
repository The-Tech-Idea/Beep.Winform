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
    /// Flexible Range Picker Painter
    /// Toggle tabs for "Choose dates" vs "I'm flexible"
    /// Side-by-side dual calendar for range selection
    /// Quick date preset buttons at bottom (+1 day, ±2 days, etc.)
    /// Inspired by modern travel booking interfaces
    /// </summary>
    public class FlexibleRangeDateTimePickerPainter : IDateTimePickerPainter
    {
        private readonly BeepDateTimePicker _owner;
        private readonly IBeepTheme _theme;
        private bool _flexibleMode = false; // Toggle between exact/flexible

        public DatePickerMode Mode => DatePickerMode.FlexibleRange;

        public FlexibleRangeDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
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

            // Background
            var bgColor = _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            int padding = 10;
            int currentY = bounds.Y + padding;

            // Tab selector at top
            var tabBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 40);
            PaintTabSelector(g, tabBounds, hoverState);
            currentY += 50;

            // Dual calendar area
            int calendarAreaHeight = bounds.Height - currentY - 70; // Leave room for bottom buttons
            var dualCalendarBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, calendarAreaHeight);
            PaintDualCalendar(g, dualCalendarBounds, properties, displayMonth, hoverState);
            currentY += calendarAreaHeight + 10;

            // Quick date buttons at bottom
            var buttonAreaBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 50);
            PaintQuickDateButtons(g, buttonAreaBounds, hoverState);
        }

        private void PaintTabSelector(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var inactiveColor = Color.FromArgb(240, 240, 240);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var activeTextColor = textColor;

            int tabWidth = bounds.Width / 2;

            // "Choose dates" tab
            var tab1Bounds = new Rectangle(bounds.X, bounds.Y, tabWidth, bounds.Height);
            var tab2Bounds = new Rectangle(bounds.X + tabWidth, bounds.Y, tabWidth, bounds.Height);

            using (var path1 = GetRoundedRectPath(new Rectangle(tab1Bounds.X, tab1Bounds.Y, tab1Bounds.Width - 2, tab1Bounds.Height), 20))
            using (var path2 = GetRoundedRectPath(new Rectangle(tab2Bounds.X + 2, tab2Bounds.Y, tab2Bounds.Width - 2, tab2Bounds.Height), 20))
            {
                using (var brush = new SolidBrush(!_flexibleMode ? Color.White : inactiveColor))
                {
                    g.FillPath(brush, path1);
                }
                using (var brush = new SolidBrush(_flexibleMode ? Color.White : inactiveColor))
                {
                    g.FillPath(brush, path2);
                }

                // Border for active tab
                using (var pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    g.DrawPath(pen, !_flexibleMode ? path1 : path2);
                }
            }

            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Regular))
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Choose dates", font, brush, tab1Bounds, format);
                g.DrawString("I'm flexible", font, brush, tab2Bounds, format);
            }
        }

        private void PaintDualCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            int calendarWidth = (bounds.Width - 20) / 2;
            var leftCalendarBounds = new Rectangle(bounds.X, bounds.Y, calendarWidth, bounds.Height);
            var rightCalendarBounds = new Rectangle(bounds.X + calendarWidth + 20, bounds.Y, calendarWidth, bounds.Height);

            var leftLayout = CalculateSingleCalendarLayout(leftCalendarBounds, properties);
            var rightLayout = CalculateSingleCalendarLayout(rightCalendarBounds, properties);

            // Paint left calendar (current month)
            PaintSingleCalendar(g, leftLayout, displayMonth, properties, hoverState);

            // Paint right calendar (next month)
            var nextMonth = displayMonth.AddMonths(1);
            PaintSingleCalendar(g, rightLayout, nextMonth, properties, hoverState);

            // Paint range highlight across both calendars if applicable
            if (_owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue)
            {
                PaintRangeHighlight(g, leftLayout, rightLayout, displayMonth, nextMonth, _owner.RangeStartDate.Value, _owner.RangeEndDate.Value);
            }
        }

        private void PaintSingleCalendar(Graphics g, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState)
        {
            // Header with month/year and navigation buttons
            var headerText = displayMonth.ToString("MMMM yyyy");
            
            // Calculate button positions (on left calendar only for shared navigation)
            int buttonSize = 24;
            var prevButtonBounds = new Rectangle(layout.HeaderRect.X, layout.HeaderRect.Y + (layout.HeaderRect.Height - buttonSize) / 2, buttonSize, buttonSize);
            var nextButtonBounds = new Rectangle(layout.HeaderRect.Right - buttonSize, layout.HeaderRect.Y + (layout.HeaderRect.Height - buttonSize) / 2, buttonSize, buttonSize);

            // Check hover/pressed states for navigation buttons
            bool prevHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.PreviousButton) == true;
            bool prevPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.PreviousButton) == true;
            bool nextHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.NextButton) == true;
            bool nextPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.NextButton) == true;

            // Paint navigation buttons
            PaintNavigationButton(g, prevButtonBounds, false, prevHovered, prevPressed);
            PaintNavigationButton(g, nextButtonBounds, true, nextHovered, nextPressed);
            
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 11f, FontStyle.Bold))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText, font, brush, layout.HeaderRect, format);
            }

            // Day names
            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);

            // Calendar grid
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);
        }

        private void PaintRangeHighlight(Graphics g, DateTimePickerLayout leftLayout, DateTimePickerLayout rightLayout, DateTime leftMonth, DateTime rightMonth, DateTime rangeStart, DateTime rangeEnd)
        {
            var rangeColor = Color.FromArgb(50, _theme?.AccentColor ?? Color.FromArgb(0, 120, 215));

            using (var brush = new SolidBrush(rangeColor))
            {
                // Highlight days in range for left calendar
                for (int row = 0; row < 6; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        var cellDate = GetDateFromCell(row, col, leftMonth, _owner.FirstDayOfWeek);
                        if (cellDate >= rangeStart.Date && cellDate <= rangeEnd.Date)
                        {
                            var rect = leftLayout.DayCellRects[GetCellIndex(row, col)];
                            rect.Inflate(-2, -2);
                            g.FillRectangle(brush, rect);
                        }
                    }
                }

                // Highlight days in range for right calendar
                for (int row = 0; row < 6; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        var cellDate = GetDateFromCell(row, col, rightMonth, _owner.FirstDayOfWeek);
                        if (cellDate >= rangeStart.Date && cellDate <= rangeEnd.Date)
                        {
                            var rect = rightLayout.DayCellRects[GetCellIndex(row, col)];
                            rect.Inflate(-2, -2);
                            g.FillRectangle(brush, rect);
                        }
                    }
                }
            }
        }

        private void PaintQuickDateButtons(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var buttonTexts = new[] { "Exact dates", "± 1 day", "± 2 days", "± 3 days", "± 7 days" };
            int buttonCount = buttonTexts.Length;
            int buttonWidth = (bounds.Width - (buttonCount - 1) * 8) / buttonCount;

            for (int i = 0; i < buttonCount; i++)
            {
                var buttonBounds = new Rectangle(
                    bounds.X + i * (buttonWidth + 8),
                    bounds.Y,
                    buttonWidth,
                    bounds.Height
                );
                
                // Check hover/pressed for this specific FlexibleRangeButton
                bool isHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.FlexibleRangeButton) == true &&
                                 hoverState.HoveredQuickButtonText == buttonTexts[i];
                bool isPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.FlexibleRangeButton) == true &&
                                 hoverState.PressedQuickButtonText == buttonTexts[i];
                
                PaintQuickButton(g, buttonBounds, buttonTexts[i], isHovered, isPressed);
            }
        }

        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed)
        {
            var borderColor = Color.FromArgb(200, 200, 200);
            var bgColor = isHovered ? Color.FromArgb(250, 250, 250) : Color.White;
            var textColor = _theme?.ForeColor ?? Color.Black;

            using (var brush = new SolidBrush(bgColor))
            using (var path = GetRoundedRectPath(buttonBounds, 20))
            {
                g.FillPath(brush, path);
            }

            using (var pen = new Pen(borderColor, 1))
            using (var path = GetRoundedRectPath(buttonBounds, 20))
            {
                g.DrawPath(pen, path);
            }

            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, font, brush, buttonBounds, format);
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

            // Previous month days (grayed out)
            for (int i = 0; i < dayOffset; i++)
            {
                var day = daysInPrevMonth - dayOffset + i + 1;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, day);
                var cellRect = dayCells[row, col];
                PaintDayCell(g, cellRect, date, false, false, true, false, false, false);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            // Current month days
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = dayCells[row, col];

                bool isRangeStart = _owner.RangeStartDate.HasValue && _owner.RangeStartDate.Value.Date == date.Date;
                bool isRangeEnd = _owner.RangeEndDate.HasValue && _owner.RangeEndDate.Value.Date == date.Date;
                bool isSelected = isRangeStart || isRangeEnd;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInRange = _owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue &&
                                date >= _owner.RangeStartDate.Value.Date && date <= _owner.RangeEndDate.Value.Date;

                PaintDayCell(g, cellRect, date, isSelected, isToday, isDisabled, isHovered, isPressed, isInRange, isRangeStart, isRangeEnd);

                col++;
                if (col >= 7) { col = 0; row++; }
            }

            // Next month days (grayed out)
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

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);
            
            // Define distinct colors for start and end dates
            var startDateColor = Color.FromArgb(34, 139, 34);  // Forest Green
            var endDateColor = accentColor;  // Blue (accent)

            cellBounds.Inflate(-2, -2);

            // Range background (but not on start/end dates)
            if (isInRange && !isStartDate && !isEndDate)
            {
                using (var brush = new SolidBrush(Color.FromArgb(40, accentColor)))
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
            else if (isSelected)
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

            if (isToday && !isSelected && !isStartDate && !isEndDate)
            {
                using (var pen = new Pen(todayColor, 2))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

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

        public void PaintHeader(Graphics g, Rectangle headerBounds, string headerText, bool showNavigation, bool isHovered)
        {
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var font = new Font(_theme?.FontName ?? "Segoe UI", 11f, FontStyle.Bold);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(headerText, font, brush, headerBounds, format);
            }
        }

        public void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed)
        {
            // Not used in this layout - navigation via tabs
        }

        public void PaintDayNamesHeader(Graphics g, Rectangle headerBounds, DatePickerFirstDayOfWeek firstDayOfWeek)
        {
            var textColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(128, 128, 128);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 8.5f);

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
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }
        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            // Calculate layout for hit testing
            int padding = 10;
            int currentY = bounds.Y + padding;
            
            // Tab selector at top (not interactive for hit testing)
            currentY += 50;
            
            // Calendar area
            int calendarAreaHeight = bounds.Height - currentY - 70;
            int calendarWidth = (bounds.Width - padding * 3) / 2;
            var leftCalendarBounds = new Rectangle(bounds.X + padding, currentY, calendarWidth, calendarAreaHeight);
            var layout = CalculateSingleCalendarLayout(leftCalendarBounds, properties);
            
            currentY += calendarAreaHeight + 10;
            
            // Quick date buttons at bottom (tolerance presets)
            var buttonAreaBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 50);
            var buttonTexts = new[] { "Exact dates", "± 1 day", "± 2 days", "± 3 days", "± 7 days" };
            var buttonKeys = new[] { "exact_dates", "plus_minus_1_day", "plus_minus_2_days", "plus_minus_3_days", "plus_minus_7_days" };
            int buttonCount = buttonTexts.Length;
            int buttonWidth = (buttonAreaBounds.Width - (buttonCount - 1) * 8) / buttonCount;
            
            layout.QuickDateButtons = new List<(string Key, Rectangle Bounds)>();
            for (int i = 0; i < buttonCount; i++)
            {
                var buttonBounds = new Rectangle(
                    buttonAreaBounds.X + i * (buttonWidth + 8),
                    buttonAreaBounds.Y,
                    buttonWidth,
                    buttonAreaBounds.Height
                );
                layout.QuickDateButtons.Add((buttonKeys[i], buttonBounds));
            }
            
            // Register all hit areas with BaseControl's hit test system
            _owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);
            
            return layout;
        }

        private DateTimePickerLayout CalculateSingleCalendarLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            int padding = 6;
            int currentY = bounds.Y + padding;
            
            // Enforce minimum width for dual calendar layout (each calendar needs ~280px minimum)
            int minWidth = Math.Max(bounds.Width, 280);
            int effectiveWidth = bounds.Width > 0 ? bounds.Width : minWidth;

            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, effectiveWidth - padding * 2, 30);
            
            // Navigation buttons in header
            int buttonSize = 24;
            layout.PreviousButtonRect = new Rectangle(layout.HeaderRect.X, layout.HeaderRect.Y + (layout.HeaderRect.Height - buttonSize) / 2, buttonSize, buttonSize);
            layout.NextButtonRect = new Rectangle(layout.HeaderRect.Right - buttonSize, layout.HeaderRect.Y + (layout.HeaderRect.Height - buttonSize) / 2, buttonSize, buttonSize);
            
            currentY += 36;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, effectiveWidth - padding * 2, 20);
            currentY += 24;

            int gridWidth = effectiveWidth - padding * 2;
            int availableHeight = bounds.Bottom - currentY - padding;
            int gridHeight = Math.Max(100, Math.Min(availableHeight, 200));
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);

            // Calculate cell dimensions with minimum size guarantee to prevent zero-sized rectangles
            layout.CellWidth = Math.Max(gridWidth / 7, 30);
            layout.CellHeight = Math.Max(gridHeight / 6, 30);
            
            // Create both matrix and list simultaneously for consistent access in painter and hit handler
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
                        int index = GetCellIndex(row, col);
                        if (layout.DayCellRects[index].Contains(location))
                        {
                            var date = GetDateFromCell(row, col, displayMonth, _owner.FirstDayOfWeek);
                            result.HitArea = DateTimePickerHitArea.DayCell;
                            result.Date = date;
                            result.HitBounds = layout.DayCellRects[index];
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
            return new Size(640, 420);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            return new Size(560, 380);
        }
    }
}
