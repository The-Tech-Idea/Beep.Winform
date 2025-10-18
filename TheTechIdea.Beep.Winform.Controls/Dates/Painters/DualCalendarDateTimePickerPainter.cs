using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;

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

            // Calculate master layout once
            var layout = CalculateLayout(bounds, properties);
            
            if (layout.MonthGrids == null || layout.MonthGrids.Count < 2)
                return;

            var leftGrid = layout.MonthGrids[0];
            var rightGrid = layout.MonthGrids[1];

            // Paint left calendar (current month) - gridIndex 0
            PaintSingleCalendarFromGrid(g, leftGrid, displayMonth, properties, hoverState, true, 0);

            // Separator
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(230, 230, 230);
            using (var pen = new Pen(borderColor, 1))
            {
                int padding = 16;
                int gap = 12;
                int calendarWidth = (bounds.Width - padding * 2 - gap) / 2;
                int separatorX = bounds.X + padding + calendarWidth + gap / 2;
                g.DrawLine(pen, separatorX, bounds.Y + padding + 10, separatorX, bounds.Bottom - padding - 10);
            }

            // Paint right calendar (next month) - gridIndex 1
            PaintSingleCalendarFromGrid(g, rightGrid, displayMonth.AddMonths(1), properties, hoverState, false, 1);

            // Range info at bottom
            if (_owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue)
            {
                int padding = 16;
                var rangeInfoRect = new Rectangle(bounds.X + padding, bounds.Bottom - 90, bounds.Width - padding * 2, 34);
                PaintRangeInfo(g, rangeInfoRect);
            }

            // Reset button (below range info or centered if no range)
            if (!layout.ResetButtonRect.IsEmpty)
            {
                bool resetHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.ResetButton) == true;
                bool resetPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.ResetButton) == true;
                PaintActionButton(g, layout.ResetButtonRect, "Reset", false, resetHovered, resetPressed);
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

        private void PaintSingleCalendar(Graphics g, Rectangle bounds, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState, bool showNavigation, int gridIndex)
        {
            // Get the grid from the master layout
            var masterLayout = CalculateLayout(new Rectangle(bounds.X - (gridIndex == 0 ? 16 : bounds.Width + 12), bounds.Y - 16, bounds.Width * 2 + 44, bounds.Height + 32), properties);
            if (masterLayout.MonthGrids == null || gridIndex >= masterLayout.MonthGrids.Count)
                return;
            
            var grid = masterLayout.MonthGrids[gridIndex];

            // Navigation only on left calendar
            if (showNavigation && gridIndex == 0)
            {
                PaintNavigationButton(g, grid.PreviousButtonRect, false,
                    hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                    hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));

                PaintNavigationButton(g, grid.NextButtonRect, true,
                    hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                    hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));
            }

            // Year combo box in header - check if THIS specific combo box is hovered/pressed
            PaintYearComboBox(g, grid.YearComboBoxRect, displayMonth.Year, hoverState, gridIndex);

            PaintDayNamesHeader(g, grid.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGridFromGrid(g, grid, displayMonth, properties, hoverState);
        }

        private void PaintSingleCalendarFromGrid(Graphics g, CalendarMonthGrid grid, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState, bool showNavigation, int gridIndex)
        {
            if (grid == null)
                return;

            // Navigation only on left calendar
            if (showNavigation && gridIndex == 0)
            {
                if (!grid.PreviousButtonRect.IsEmpty)
                {
                    PaintNavigationButton(g, grid.PreviousButtonRect, false,
                        hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                        hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));
                }

                if (!grid.NextButtonRect.IsEmpty)
                {
                    PaintNavigationButton(g, grid.NextButtonRect, true,
                        hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                        hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));
                }
            }

            // Year combo box in header - check if THIS specific combo box is hovered/pressed
            if (!grid.YearComboBoxRect.IsEmpty)
            {
                PaintYearComboBox(g, grid.YearComboBoxRect, displayMonth.Year, hoverState, gridIndex);
            }

            // Day names header
            if (!grid.DayNamesRect.IsEmpty)
            {
                PaintDayNamesHeader(g, grid.DayNamesRect, properties.FirstDayOfWeek);
            }

            // Calendar grid with day cells
            PaintCalendarGridFromGrid(g, grid, displayMonth, properties, hoverState);
        }

        private void PaintYearComboBox(Graphics g, Rectangle bounds, int selectedYear, DateTimePickerHoverState hoverState, int gridIndex)
        {
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold);

            // Check if THIS specific combo box (by gridIndex) is hovered/pressed
            bool isHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.YearComboBox) == true && 
                           hoverState?.HoveredGridIndex == gridIndex;
            bool isPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.YearComboBox) == true && 
                           hoverState?.PressedGridIndex == gridIndex;

            // Background
            if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRoundedRectangle(brush, bounds, 4);
                }
            }

            // Border
            using (var pen = new Pen(borderColor, 1))
            {
                DrawRoundedRectangle(g, pen, bounds, 4);
            }

            // Year text
            using (var brush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(bounds.X + 8, bounds.Y, bounds.Width - 24, bounds.Height);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(selectedYear.ToString(), font, brush, textRect, format);
            }

            // Dropdown arrow
            using (var pen = new Pen(textColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int arrowX = bounds.Right - 14;
                int arrowY = bounds.Y + bounds.Height / 2;
                int arrowSize = 3;

                g.DrawLine(pen, arrowX - arrowSize, arrowY - 2, arrowX, arrowY + 2);
                g.DrawLine(pen, arrowX, arrowY + 2, arrowX + arrowSize, arrowY - 2);
            }
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

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);
            var rangeColor = Color.FromArgb(50, accentColor);
            
            // Define distinct colors for start and end dates
            var startDateColor = Color.FromArgb(34, 139, 34);  // Forest Green
            var endDateColor = accentColor;  // Blue (accent)

            cellBounds.Inflate(-1, -1);

            // Range background
            if (isInRange && !isSelected && !isStartDate && !isEndDate)
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
            // Selection (start or end) - fallback for old code paths
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
                var cellRect = layout.DayCellRects[GetCellIndex(row, col)];
                bool isInRange = IsInRange(date);
                PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = layout.DayCellRects[GetCellIndex(row, col)];

                bool isStartDate = _owner.RangeStartDate.HasValue && _owner.RangeStartDate.Value.Date == date.Date;
                bool isEndDate = _owner.RangeEndDate.HasValue && _owner.RangeEndDate.Value.Date == date.Date;
                bool isStartOrEnd = isStartDate || isEndDate;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInRange = IsInRange(date);

                PaintDayCell(g, cellRect, date, isStartOrEnd, isToday, isDisabled, isHovered, isPressed, isInRange, isStartDate, isEndDate);

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
                    bool isInRange = IsInRange(date);
                    PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                    col++;
                }
                col = 0;
                row++;
            }
        }

        private void PaintCalendarGridFromGrid(Graphics g, CalendarMonthGrid grid, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState)
        {
            if (grid.DayCellRects == null || grid.DayCellRects.Count == 0)
                return;

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
                var cellRect = grid.DayCellRects[GetCellIndex(row, col)];
                bool isInRange = IsInRange(date);
                PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            // Current month days
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = grid.DayCellRects[GetCellIndex(row, col)];

                bool isStartDate = _owner.RangeStartDate.HasValue && _owner.RangeStartDate.Value.Date == date.Date;
                bool isEndDate = _owner.RangeEndDate.HasValue && _owner.RangeEndDate.Value.Date == date.Date;
                bool isStartOrEnd = isStartDate || isEndDate;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInRange = IsInRange(date);

                PaintDayCell(g, cellRect, date, isStartOrEnd, isToday, isDisabled, isHovered, isPressed, isInRange, isStartDate, isEndDate);

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
                    var cellRect = grid.DayCellRects[GetCellIndex(row, col)];
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

        private DateTimePickerLayout CalculateSingleCalendarLayout(Rectangle bounds, DateTimePickerProperties properties, bool showNavigation)
        {
            var layout = new DateTimePickerLayout();
            int padding = 8;
            int currentY = bounds.Y;

            // Year combo box in center of header
            int comboWidth = 100;
            int comboHeight = 28;
            int comboX = bounds.X + (bounds.Width - comboWidth) / 2;
            layout.YearComboBoxRect = new Rectangle(comboX, currentY + 2, comboWidth, comboHeight);

            int navButtonSize = 28;
            if (showNavigation)
            {
                layout.PreviousButtonRect = new Rectangle(bounds.X + 2, currentY + 2, navButtonSize, navButtonSize);
                layout.NextButtonRect = new Rectangle(bounds.Right - navButtonSize - 2, currentY + 2, navButtonSize, navButtonSize);
            }

            currentY += 38;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 22);
            currentY += 26;

            int gridWidth = bounds.Width - padding * 2;
            int gridHeight = bounds.Height - (currentY - bounds.Y) - padding;
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);

            layout.CellWidth = gridWidth / 7;
            layout.CellHeight = gridHeight / 6;
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

            return layout;
        }

        // Stub implementations
        public void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
        public void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState) { }
        public void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed) { }
        public void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed) { }
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        
        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed)
        {
            if (buttonBounds.IsEmpty || buttonBounds.Width <= 0 || buttonBounds.Height <= 0)
                return;

            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = isPrimary ? Color.White : (_theme?.ForeColor ?? Color.Black);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            var bgColor = isPrimary ? accentColor : (_theme?.BackgroundColor ?? Color.White);
            var hoverColor = isPrimary ? ControlPaint.Light(accentColor, 0.1f) : Color.FromArgb(240, 240, 240);
            var pressedColor = isPrimary ? ControlPaint.Dark(accentColor, 0.1f) : Color.FromArgb(230, 230, 230);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 9f);

            // Background
            Color currentBgColor = bgColor;
            if (isPressed)
                currentBgColor = pressedColor;
            else if (isHovered)
                currentBgColor = hoverColor;

            using (var brush = new SolidBrush(currentBgColor))
            {
                g.FillRoundedRectangle(brush, buttonBounds, 4);
            }

            // Border
            using (var pen = new Pen(isPrimary && !isHovered && !isPressed ? currentBgColor : borderColor, 1))
            {
                DrawRoundedRectangle(g, pen, buttonBounds, 4);
            }

            // Text
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, font, brush, buttonBounds, format);
            }
        }
        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            
            int padding = 16;
            int gap = 12;
            int calendarWidth = (bounds.Width - padding * 2 - gap) / 2;
            
            // Create MonthGrids for dual calendar display
            layout.MonthGrids = new List<CalendarMonthGrid>();
            
            // Left calendar (Month 0)
            var leftCalendarBounds = new Rectangle(bounds.X + padding, bounds.Y + padding, calendarWidth, bounds.Height - padding * 2 - 90);
            var leftGrid = CreateCalendarGrid(leftCalendarBounds, properties, true);
            layout.MonthGrids.Add(leftGrid);
            
            // Right calendar (Month 1)
            var rightCalendarBounds = new Rectangle(bounds.X + padding + calendarWidth + gap, bounds.Y + padding, calendarWidth, bounds.Height - padding * 2 - 90);
            var rightGrid = CreateCalendarGrid(rightCalendarBounds, properties, false);
            layout.MonthGrids.Add(rightGrid);
            
            // Reset button at bottom (centered)
            int resetButtonWidth = 100;
            int resetButtonHeight = 32;
            layout.ResetButtonRect = new Rectangle(
                bounds.X + (bounds.Width - resetButtonWidth) / 2,  // Centered horizontally
                bounds.Bottom - 50,                                 // Near bottom
                resetButtonWidth,
                resetButtonHeight
            );
            
            // Register all hit areas with BaseControl's hit test system
            _owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);
            
            return layout;
        }

        private CalendarMonthGrid CreateCalendarGrid(Rectangle bounds, DateTimePickerProperties properties, bool showNavigation)
        {
            var grid = new CalendarMonthGrid();
            
            int padding = 4;
            int currentY = bounds.Y;
            
            // Year combo box in center of header
            int comboWidth = 100;
            int comboHeight = 28;
            int comboX = bounds.X + (bounds.Width - comboWidth) / 2;
            grid.YearComboBoxRect = new Rectangle(comboX, currentY + 2, comboWidth, comboHeight);
            
            // Navigation buttons (only on left calendar)
            if (showNavigation)
            {
                int navButtonSize = 28;
                grid.PreviousButtonRect = new Rectangle(bounds.X + 2, currentY + 2, navButtonSize, navButtonSize);
                grid.NextButtonRect = new Rectangle(bounds.Right - navButtonSize - 2, currentY + 2, navButtonSize, navButtonSize);
            }
            
            currentY += 38;
            
            // Day names
            grid.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 22);
            currentY += 26;
            
            // Calendar grid
            int gridWidth = bounds.Width - padding * 2;
            int gridHeight = bounds.Height - (currentY - bounds.Y) - padding;
            grid.GridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);
            
            // Calculate cells
            int cellWidth = gridWidth / 7;
            int cellHeight = gridHeight / 6;
            grid.DayCellRects = new List<Rectangle>();
            
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    grid.DayCellRects.Add(new Rectangle(
                        grid.GridRect.X + col * cellWidth,
                        grid.GridRect.Y + row * cellHeight,
                        cellWidth,
                        cellHeight
                    ));
                }
            }
            
            return grid;
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
    

        private          void DrawRoundedRectangle(Graphics g, Pen pen, Rectangle bounds, int radius)
        {
            using (var path = GetRoundedRectanglePath(bounds, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        private  GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // Top left
            path.AddArc(arc, 180, 90);

            // Top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}

