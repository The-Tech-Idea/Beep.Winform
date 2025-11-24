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

        // Helper method to convert row,col to flat list index
        private int GetCellIndex(int row, int col)
        {
            return row * 7 + col;
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

            // Filter buttons - must match the labels in CalculateLayout and keys in hit handler
            string[] filters = { "Past Week", "Past Month", "Past 3 Months", "Past 6 Months", "Past Year", "Past Century" };
            string[] filterKeys = { "last7days", "lastmonth", "past3months", "past6months", "pastyear", "pastcentury" };
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
                
                // Check hover for this specific filter button - compare with filter text, not key
                bool isHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.FilterButton) == true &&
                                 hoverState.HoveredQuickButtonText == filters[i];
                
                PaintFilterButton(g, buttonBounds, filters[i], isSelected, isHovered);
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

            // Year selector dropdown - check hover state
            var yearBounds = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 28);
            bool yearDropdownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.YearDropdown) == true;
            PaintYearSelector(g, yearBounds, displayMonth.Year, yearDropdownHovered);
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
                            var rect = leftLayout.DayCellRects[GetCellIndex(row, col)];
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
                            var rect = rightLayout.DayCellRects[GetCellIndex(row, col)];
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
            int spinnerWidth = 60;
            int spinnerHeight = 54;
            int colonWidth = 20;
            int gap = 15;

            // From label
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var fromLabelRect = new Rectangle(bounds.X, bounds.Y, labelWidth, bounds.Height);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString("From:", font, brush, fromLabelRect, format);
            }

            // From time spinners (hour : minute)
            int startX = bounds.X + labelWidth;
            int spinnerY = bounds.Y + (bounds.Height - spinnerHeight) / 2;
            var startTime = _owner.RangeStartTime ?? TimeSpan.Zero;
            
            // From hour spinner
            var fromHourRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight);
            var fromHourUpRect = new Rectangle(fromHourRect.X, fromHourRect.Y, spinnerWidth, 16);
            var fromHourDownRect = new Rectangle(fromHourRect.X, fromHourRect.Bottom - 16, spinnerWidth, 16);
            
            bool startHourUpHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartHourUpButton) == true;
            bool startHourUpPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartHourUpButton) == true;
            bool startHourDownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartHourDownButton) == true;
            bool startHourDownPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartHourDownButton) == true;
            
            PaintTimeSpinner(g, fromHourRect, fromHourUpRect, fromHourDownRect,
                startTime.Hours.ToString("D2"), startHourUpHovered, startHourUpPressed, startHourDownHovered, startHourDownPressed);

            // Colon
            startX += spinnerWidth + 4;
            var colonRect = new Rectangle(startX, spinnerY, colonWidth, spinnerHeight);
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 14f, FontStyle.Bold);
                g.DrawString(":", boldFont, brush, colonRect, format);
            }

            // From minute spinner
            startX += colonWidth + 4;
            var fromMinuteRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight);
            var fromMinuteUpRect = new Rectangle(fromMinuteRect.X, fromMinuteRect.Y, spinnerWidth, 16);
            var fromMinuteDownRect = new Rectangle(fromMinuteRect.X, fromMinuteRect.Bottom - 16, spinnerWidth, 16);
            
            bool startMinuteUpHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartMinuteUpButton) == true;
            bool startMinuteUpPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartMinuteUpButton) == true;
            bool startMinuteDownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.StartMinuteDownButton) == true;
            bool startMinuteDownPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.StartMinuteDownButton) == true;
            
            PaintTimeSpinner(g, fromMinuteRect, fromMinuteUpRect, fromMinuteDownRect,
                startTime.Minutes.ToString("D2"), startMinuteUpHovered, startMinuteUpPressed, startMinuteDownHovered, startMinuteDownPressed);

            // To label
            startX += spinnerWidth + gap;
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 9f))
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var toLabelRect = new Rectangle(startX, bounds.Y, labelWidth, bounds.Height);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString("To:", font, brush, toLabelRect, format);
            }

            // To time spinners (hour : minute)
            startX += labelWidth;
            var endTime = _owner.RangeEndTime ?? new TimeSpan(23, 59, 59);
            
            // To hour spinner
            var toHourRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight);
            var toHourUpRect = new Rectangle(toHourRect.X, toHourRect.Y, spinnerWidth, 16);
            var toHourDownRect = new Rectangle(toHourRect.X, toHourRect.Bottom - 16, spinnerWidth, 16);
            
            bool endHourUpHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.EndHourUpButton) == true;
            bool endHourUpPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.EndHourUpButton) == true;
            bool endHourDownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.EndHourDownButton) == true;
            bool endHourDownPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.EndHourDownButton) == true;
            
            PaintTimeSpinner(g, toHourRect, toHourUpRect, toHourDownRect,
                endTime.Hours.ToString("D2"), endHourUpHovered, endHourUpPressed, endHourDownHovered, endHourDownPressed);

            // Colon
            startX += spinnerWidth + 4;
            var colonRect2 = new Rectangle(startX, spinnerY, colonWidth, spinnerHeight);
            using (var brush = new SolidBrush(_theme?.ForeColor ?? Color.Black))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 14f, FontStyle.Bold);
                g.DrawString(":", boldFont, brush, colonRect2, format);
            }

            // To minute spinner
            startX += colonWidth + 4;
            var toMinuteRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight);
            var toMinuteUpRect = new Rectangle(toMinuteRect.X, toMinuteRect.Y, spinnerWidth, 16);
            var toMinuteDownRect = new Rectangle(toMinuteRect.X, toMinuteRect.Bottom - 16, spinnerWidth, 16);
            
            bool endMinuteUpHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.EndMinuteUpButton) == true;
            bool endMinuteUpPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.EndMinuteUpButton) == true;
            bool endMinuteDownHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.EndMinuteDownButton) == true;
            bool endMinuteDownPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.EndMinuteDownButton) == true;
            
            PaintTimeSpinner(g, toMinuteRect, toMinuteUpRect, toMinuteDownRect,
                endTime.Minutes.ToString("D2"), endMinuteUpHovered, endMinuteUpPressed, endMinuteDownHovered, endMinuteDownPressed);
        }

        private void PaintTimeSpinner(Graphics g, Rectangle bounds, Rectangle upRect, Rectangle downRect,
            string value, bool upHovered, bool upPressed, bool downHovered, bool downPressed)
        {
            if (bounds == Rectangle.Empty)
                return;

            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(120, 120, 120);
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var pressedColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 12f);

            // Draw spinner border
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Draw up button background
            if (!upRect.IsEmpty && (upHovered || upPressed))
            {
                using (var brush = new SolidBrush(upPressed ? pressedColor : hoverColor))
                {
                    g.FillRectangle(brush, upRect);
                }
            }

            // Draw down button background
            if (!downRect.IsEmpty && (downHovered || downPressed))
            {
                using (var brush = new SolidBrush(downPressed ? pressedColor : hoverColor))
                {
                    g.FillRectangle(brush, downRect);
                }
            }

            // Draw separator lines
            using (var pen = new Pen(borderColor, 1))
            {
                // Line below up button
                g.DrawLine(pen, bounds.X, upRect.Bottom, bounds.Right, upRect.Bottom);
                // Line above down button
                g.DrawLine(pen, bounds.X, downRect.Y, bounds.Right, downRect.Y);
            }

            // Draw up/down arrow icons
            using (var pen = new Pen(upPressed || downPressed ? Color.White : secondaryTextColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Up arrow
                if (!upRect.IsEmpty)
                {
                    int cx = upRect.X + upRect.Width / 2;
                    int cy = upRect.Y + upRect.Height / 2;
                    g.DrawLine(pen, cx - 4, cy + 2, cx, cy - 2);
                    g.DrawLine(pen, cx, cy - 2, cx + 4, cy + 2);
                }

                // Down arrow
                if (!downRect.IsEmpty)
                {
                    int cx = downRect.X + downRect.Width / 2;
                    int cy = downRect.Y + downRect.Height / 2;
                    g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2);
                    g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2);
                }
            }

            // Draw value in the middle section
            var middleRect = new Rectangle(bounds.X, upRect.Bottom, bounds.Width, downRect.Y - upRect.Bottom);
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(value, font, brush, middleRect, format);
            }
        }

        private void PaintActionButtonsRow(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            int buttonWidth = (bounds.Width - 12) / 2;

            // Reset Date button
            var resetBounds = new Rectangle(bounds.X, bounds.Y, buttonWidth, bounds.Height);
            bool resetHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.ResetButton) == true;
            bool resetPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.ResetButton) == true;
            PaintActionButton(g, resetBounds, "Reset Date", false, resetHovered, resetPressed);

            // Show Results button (primary)
            var resultsBounds = new Rectangle(bounds.X + buttonWidth + 12, bounds.Y, buttonWidth, bounds.Height);
            bool resultsHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.ShowResultsButton) == true;
            bool resultsPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.ShowResultsButton) == true;
            PaintActionButton(g, resultsBounds, "Show Results", true, resultsHovered, resultsPressed);
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

            // Paint range background if in range
            if (isInRange && !isSelected && !isStartDate && !isEndDate)
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
            var layout = new DateTimePickerLayout();
            
            int padding = 12;
            int sidebarWidth = (int)(bounds.Width * 0.25f); // 25% for sidebar
            
            layout.SidebarRect = new Rectangle(bounds.X, bounds.Y, sidebarWidth, bounds.Height);
            
            int sidebarPadding = 16;
            int currentY = layout.SidebarRect.Y + sidebarPadding;
            
            // Filter title
            layout.FilterTitleRect = new Rectangle(
                layout.SidebarRect.X + sidebarPadding,
                currentY,
                layout.SidebarRect.Width - sidebarPadding * 2,
                32
            );
            currentY += 48;
            
            // 6 Filter buttons - must match PaintFilterSidebar
            layout.FilterButtonRects = new List<Rectangle>();
            string[] filterLabels = { "Past Week", "Past Month", "Past 3 Months", "Past 6 Months", "Past Year", "Past Century" };
            int buttonHeight = 36;
            int buttonGap = 8;
            
            for (int i = 0; i < filterLabels.Length; i++)
            {
                var buttonRect = new Rectangle(
                    layout.SidebarRect.X + sidebarPadding,
                    currentY,
                    layout.SidebarRect.Width - sidebarPadding * 2,
                    buttonHeight
                );
                layout.FilterButtonRects.Add(buttonRect);
                currentY += buttonHeight + buttonGap;
            }
            
            int mainX = bounds.X + sidebarWidth + padding;
            int mainWidth = bounds.Width - sidebarWidth - padding * 2;
            
            layout.MainContentRect = new Rectangle(mainX, bounds.Y, mainWidth, bounds.Height);
            
            currentY = layout.MainContentRect.Y + padding;
            
            int calendarHeight = (int)(bounds.Height * 0.55f);
            layout.DualCalendarContainerRect = new Rectangle(
                mainX,
                currentY,
                mainWidth,
                calendarHeight
            );
            
            int calendarWidth = (mainWidth - padding) / 2;
            int calendarPadding = 8;
            
            // LEFT CALENDAR
            int leftCalendarX = layout.DualCalendarContainerRect.X;
            int leftCalendarY = layout.DualCalendarContainerRect.Y;
            
            // Left calendar year dropdown
            layout.LeftYearDropdownRect = new Rectangle(
                leftCalendarX + calendarPadding,
                leftCalendarY,
                calendarWidth - calendarPadding * 2,
                32
            );
            leftCalendarY += 40;
            
            // Left calendar header (month name + nav buttons)
            layout.LeftHeaderRect = new Rectangle(
                leftCalendarX + calendarPadding,
                leftCalendarY,
                calendarWidth - calendarPadding * 2,
                36
            );
            leftCalendarY += 44;
            
            // Left calendar day names row
            layout.LeftDayNamesRect = new Rectangle(
                leftCalendarX + calendarPadding,
                leftCalendarY,
                calendarWidth - calendarPadding * 2,
                24
            );
            leftCalendarY += 28;
            
            // Left calendar grid
            int gridHeight = layout.DualCalendarContainerRect.Bottom - leftCalendarY - calendarPadding;
            layout.LeftCalendarGridRect = new Rectangle(
                leftCalendarX + calendarPadding,
                leftCalendarY,
                calendarWidth - calendarPadding * 2,
                gridHeight
            );
            
            // Left calendar day cells (42 cells: 6 rows × 7 cols)
            layout.LeftDayCellRects = new List<Rectangle>(42);
            int leftCellWidth = Math.Max(layout.LeftCalendarGridRect.Width / 7, 25);
            int leftCellHeight = Math.Max(layout.LeftCalendarGridRect.Height / 6, 20);
            
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    layout.LeftDayCellRects.Add(new Rectangle(
                        layout.LeftCalendarGridRect.X + col * leftCellWidth,
                        layout.LeftCalendarGridRect.Y + row * leftCellHeight,
                        leftCellWidth,
                        leftCellHeight
                    ));
                }
            }
            
            // RIGHT CALENDAR
            int rightCalendarX = leftCalendarX + calendarWidth + padding;
            int rightCalendarY = layout.DualCalendarContainerRect.Y;
            
            // Right calendar year dropdown
            layout.RightYearDropdownRect = new Rectangle(
                rightCalendarX + calendarPadding,
                rightCalendarY,
                calendarWidth - calendarPadding * 2,
                32
            );
            rightCalendarY += 40;
            
            // Right calendar header (month name + nav buttons)
            layout.RightHeaderRect = new Rectangle(
                rightCalendarX + calendarPadding,
                rightCalendarY,
                calendarWidth - calendarPadding * 2,
                36
            );
            rightCalendarY += 44;
            
            // Right calendar day names row
            layout.RightDayNamesRect = new Rectangle(
                rightCalendarX + calendarPadding,
                rightCalendarY,
                calendarWidth - calendarPadding * 2,
                24
            );
            rightCalendarY += 28;
            
            // Right calendar grid
            layout.RightCalendarGridRect = new Rectangle(
                rightCalendarX + calendarPadding,
                rightCalendarY,
                calendarWidth - calendarPadding * 2,
                gridHeight
            );
            
            // Right calendar day cells (42 cells: 6 rows × 7 cols)
            layout.RightDayCellRects = new List<Rectangle>(42);
            int rightCellWidth = Math.Max(layout.RightCalendarGridRect.Width / 7, 25);
            int rightCellHeight = Math.Max(layout.RightCalendarGridRect.Height / 6, 20);
            
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    layout.RightDayCellRects.Add(new Rectangle(
                        layout.RightCalendarGridRect.X + col * rightCellWidth,
                        layout.RightCalendarGridRect.Y + row * rightCellHeight,
                        rightCellWidth,
                        rightCellHeight
                    ));
                }
            }
            
            currentY = layout.DualCalendarContainerRect.Bottom + padding * 2;
            int timeRowHeight = 70; // Increased for spinners
            
            layout.TimePickerRowRect = new Rectangle(
                mainX,
                currentY,
                mainWidth,
                timeRowHeight
            );
            
            // Spinner dimensions
            int labelWidth = 50;
            int spinnerWidth = 60;
            int spinnerHeight = 54;
            int colonWidth = 20;
            int gap = 15;
            
            int startX = layout.TimePickerRowRect.X;
            int spinnerY = currentY + (timeRowHeight - spinnerHeight) / 2;
            
            // "From:" label
            layout.FromLabelRect = new Rectangle(startX, currentY, labelWidth, timeRowHeight);
            startX += labelWidth;
            
            // FROM Hour Spinner
            layout.TimeHourRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight); // Start hour
            layout.TimeHourUpRect = new Rectangle(layout.TimeHourRect.X, layout.TimeHourRect.Y, spinnerWidth, 16);
            layout.TimeHourDownRect = new Rectangle(layout.TimeHourRect.X, layout.TimeHourRect.Bottom - 16, spinnerWidth, 16);
            startX += spinnerWidth + 4;
            
            // Colon 1
            layout.TimeColonRect = new Rectangle(startX, spinnerY, colonWidth, spinnerHeight);
            startX += colonWidth + 4;
            
            // FROM Minute Spinner
            layout.TimeMinuteRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight); // Start minute
            layout.TimeMinuteUpRect = new Rectangle(layout.TimeMinuteRect.X, layout.TimeMinuteRect.Y, spinnerWidth, 16);
            layout.TimeMinuteDownRect = new Rectangle(layout.TimeMinuteRect.X, layout.TimeMinuteRect.Bottom - 16, spinnerWidth, 16);
            startX += spinnerWidth + gap;
            
            // "To:" label
            layout.ToLabelRect = new Rectangle(startX, currentY, labelWidth, timeRowHeight);
            startX += labelWidth;
            
            // TO Hour Spinner
            layout.EndTimeHourRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight); // End hour
            layout.EndTimeHourUpRect = new Rectangle(layout.EndTimeHourRect.X, layout.EndTimeHourRect.Y, spinnerWidth, 16);
            layout.EndTimeHourDownRect = new Rectangle(layout.EndTimeHourRect.X, layout.EndTimeHourRect.Bottom - 16, spinnerWidth, 16);
            startX += spinnerWidth + 4;
            
            // Colon 2
            layout.EndTimeColonRect = new Rectangle(startX, spinnerY, colonWidth, spinnerHeight);
            startX += colonWidth + 4;
            
            // TO Minute Spinner
            layout.EndTimeMinuteRect = new Rectangle(startX, spinnerY, spinnerWidth, spinnerHeight); // End minute
            layout.EndTimeMinuteUpRect = new Rectangle(layout.EndTimeMinuteRect.X, layout.EndTimeMinuteRect.Y, spinnerWidth, 16);
            layout.EndTimeMinuteDownRect = new Rectangle(layout.EndTimeMinuteRect.X, layout.EndTimeMinuteRect.Bottom - 16, spinnerWidth, 16);
            
            currentY = layout.TimePickerRowRect.Bottom + padding * 2;
            int actionRowHeight = 44;
            
            layout.ActionButtonRowRect = new Rectangle(
                mainX,
                currentY,
                mainWidth,
                actionRowHeight
            );
            
            int buttonWidth = 120;
            int actionButtonGap = 12;
            int buttonsX = layout.ActionButtonRowRect.Right - (buttonWidth * 2 + actionButtonGap);
            
            // Reset button
            layout.ResetButtonRect = new Rectangle(
                buttonsX,
                currentY + 6,
                buttonWidth,
                32
            );
            
            // Show Results button
            layout.ShowResultsButtonRect = new Rectangle(
                buttonsX + buttonWidth + actionButtonGap,
                currentY + 6,
                buttonWidth,
                32
            );
            
            // Register all hit areas with BaseControl's hit test system
            _owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);
            
            return layout;
        }

        private DateTimePickerLayout CalculateSingleCalendarGrid(Rectangle bounds)
        {
            var layout = new DateTimePickerLayout();
            
            // Enforce minimum width for filtered range dual calendar
            int minWidth = Math.Max(bounds.Width, 200);
            int effectiveWidth = bounds.Width > 0 ? bounds.Width : minWidth;
            int minHeight = Math.Max(bounds.Height, 150);
            int effectiveHeight = bounds.Height > 0 ? bounds.Height : minHeight;
            
            layout.CalendarGridRect = new Rectangle(bounds.X, bounds.Y, effectiveWidth, effectiveHeight);

            // Calculate cell dimensions with minimum size guarantee to prevent zero-sized rectangles
            layout.CellWidth = Math.Max(effectiveWidth / 7, 25);
            layout.CellHeight = Math.Max(effectiveHeight / 6, 20);
            
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
            return new Size(720, 480);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            return new Size(640, 420);
        }
    }
}
