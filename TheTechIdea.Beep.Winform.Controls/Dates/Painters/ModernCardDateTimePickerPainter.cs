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
    /// Modern Card Date Picker Painter
    /// Card-Style UI with quick date buttons (Today, Tomorrow, Next Week, etc.)
    /// Visual styling follows BeepTheme
    /// </summary>
    public class ModernCardDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.ModernCard;

        public ModernCardDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Modern card background
            PaintCardBackground(g, bounds);

            int padding = 16;
            int currentY = bounds.Y + padding;

            // Quick selection buttons at top
            var quickButtonsRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 100);
            PaintQuickSelectionButtons(g, quickButtonsRect, properties, hoverState);
            currentY += 116;

            // Separator
            PaintSeparator(g, new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 1));
            currentY += 12;

            // Calendar section
            var layout = CalculateLayout(new Rectangle(bounds.X, currentY, bounds.Width, bounds.Bottom - currentY), properties);
            
            PaintHeader(g, layout.HeaderRect, displayMonth.ToString("MMMM yyyy"), true, false);
            
            PaintNavigationButton(g, layout.PreviousButtonRect, false, 
                hoverState.IsAreaHovered(DateTimePickerHitArea.PreviousButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.PreviousButton));
            
            PaintNavigationButton(g, layout.NextButtonRect, true,
                hoverState.IsAreaHovered(DateTimePickerHitArea.NextButton),
                hoverState.IsAreaPressed(DateTimePickerHitArea.NextButton));

            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);
        }

        private void PaintCardBackground(Graphics g, Rectangle bounds)
        {
            var bgColor = _theme?.BackgroundColor ?? Color.White;
            var shadowColor = Color.FromArgb(20, 0, 0, 0);

            // Shadow effect
            for (int i = 0; i < 3; i++)
            {
                using (var brush = new SolidBrush(Color.FromArgb(10 - i * 3, 0, 0, 0)))
                using (var path = GetRoundedRectPath(new Rectangle(bounds.X + i, bounds.Y + i, bounds.Width, bounds.Height), 12))
                {
                    g.FillPath(brush, path);
                }
            }

            // Card background
            using (var brush = new SolidBrush(bgColor))
            using (var path = GetRoundedRectPath(bounds, 12))
            {
                g.FillPath(brush, path);
            }
        }

        private void PaintSeparator(Graphics g, Rectangle bounds)
        {
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(230, 230, 230);
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, bounds.X, bounds.Y, bounds.Right, bounds.Y);
            }
        }

        public void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState)
        {
            // Build using helper to ensure IDs/order match hit handling
            var defs = Dates.Helpers.DateTimePickerQuickButtonHelper.GetQuickButtonDefinitions(properties);

            int buttonHeight = 40;
            int gap = 8;
            int rows = 2;
            int cols = 2;
            int buttonWidth = (buttonAreaBounds.Width - gap) / cols;
            int rowHeight = (buttonAreaBounds.Height - gap) / rows;

            // Populate layout.QuickButtonRects if owner/layout available via CalculateLayout
            if (_owner != null)
            {
                if (_owner.GetCurrentProperties() != null) { /* no-op placeholder */ }
            }

            for (int i = 0; i < Math.Min(defs.Count, rows * cols); i++)
            {
                int row = i / cols;
                int col = i % cols;
                
                var buttonRect = new Rectangle(
                    buttonAreaBounds.X + col * (buttonWidth + gap),
                    buttonAreaBounds.Y + row * (rowHeight + gap),
                    buttonWidth,
                    rowHeight
                );

                bool isSelected = _owner.SelectedDate.HasValue && 
                                 _owner.SelectedDate.Value.Date == defs[i].TargetDate.Date;

                // Check if this specific quick button is hovered/pressed by matching button text
                bool isHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.QuickButton) == true &&
                                hoverState.HoveredQuickButtonText == defs[i].Label;
                bool isPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.QuickButton) == true &&
                                hoverState.PressedQuickButtonText == defs[i].Label;

                PaintQuickButton(g, buttonRect, defs[i].Label, defs[i].TargetDate, isSelected, isHovered, isPressed);
            }
        }

        public void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed)
        {
            // This overload is for interface compatibility
            PaintQuickButton(g, buttonBounds, text, DateTime.Today, false, isHovered, isPressed);
        }

        private void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, DateTime date, bool isSelected, bool isHovered, bool isPressed)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(100, 100, 100);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 9f, FontStyle.Bold);

            // Background
            if (isSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, accentColor)))
                using (var path = GetRoundedRectPath(buttonBounds, 8))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(accentColor, 2))
                using (var path = GetRoundedRectPath(buttonBounds, 8))
                {
                    g.DrawPath(pen, path);
                }
            }
            else if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                using (var path = GetRoundedRectPath(buttonBounds, 8))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(borderColor, 1))
                using (var path = GetRoundedRectPath(buttonBounds, 8))
                {
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                using (var pen = new Pen(borderColor, 1))
                using (var path = GetRoundedRectPath(buttonBounds, 8))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Button text
            using (var brush = new SolidBrush(isSelected ? accentColor : textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(text, isSelected ? boldFont : font, brush, 
                    new Rectangle(buttonBounds.X, buttonBounds.Y + 8, buttonBounds.Width, 16), format);
            }

            // Date
            string dateText = date.ToString("MMM d");
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                var smallFont = new Font("Segoe UI", 8f);
                g.DrawString(dateText, smallFont, brush, 
                    new Rectangle(buttonBounds.X, buttonBounds.Y + 28, buttonBounds.Width, 14), format);
            }
        }

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);

            cellBounds.Inflate(-2, -2);

            // Modern rounded square selection
            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                using (var path = GetRoundedRectPath(cellBounds, 8))
                {
                    g.FillPath(brush, path);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
            }
            else if (isPressed)
            {
                var pressedColor = ControlPaint.Dark(accentColor, 0.1f);
                using (var brush = new SolidBrush(pressedColor))
                using (var path = GetRoundedRectPath(cellBounds, 8))
                {
                    g.FillPath(brush, path);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                using (var path = GetRoundedRectPath(cellBounds, 8))
                {
                    g.FillPath(brush, path);
                }
            }

            // Today indicator - underline
            if (isToday && !isSelected)
            {
                using (var pen = new Pen(todayColor, 2))
                {
                    g.DrawLine(pen, cellBounds.X + 8, cellBounds.Bottom - 4, cellBounds.Right - 8, cellBounds.Bottom - 4);
                }
            }

            // Day number
            var dayText = date.Day.ToString();
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);
            
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
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 13f, FontStyle.Bold);

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
                var cellRect = layout.DayCellMatrix[row, col];
                PaintDayCell(g, cellRect, date, false, false, true, false, false, false);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = layout.DayCellMatrix[row, col];
                
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
                    var cellRect = layout.DayCellMatrix[row, col];
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
        public void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }
        public void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            int padding = 16;
            int currentY = bounds.Y + padding;

            // Quick buttons region (2x2)
            layout.QuickButtonsRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 100);
            layout.QuickButtonRects = new List<Rectangle>(4);
            layout.QuickDateButtons = new List<(string Key, Rectangle Bounds)>();
            
            var buttonKeys = new[] { "today", "tomorrow", "next_week", "next_month" };
            {
                int rows = 2, cols = 2, gap = 8;
                int buttonWidth = (layout.QuickButtonsRect.Width - gap) / cols;
                int rowHeight = (layout.QuickButtonsRect.Height - gap) / rows;
                for (int i = 0; i < rows * cols; i++)
                {
                    int r = i / cols, c = i % cols;
                    var rect = new Rectangle(
                        layout.QuickButtonsRect.X + c * (buttonWidth + gap),
                        layout.QuickButtonsRect.Y + r * (rowHeight + gap),
                        buttonWidth,
                        rowHeight);
                    layout.QuickButtonRects.Add(rect);
                    
                    // Add to QuickDateButtons with proper key
                    if (i < buttonKeys.Length)
                    {
                        layout.QuickDateButtons.Add((buttonKeys[i], rect));
                    }
                }
            }
            currentY += 116;

            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2 - 72, 36);
            
            int navButtonSize = 32;
            layout.PreviousButtonRect = new Rectangle(bounds.Right - padding - navButtonSize * 2 - 8, currentY + 2, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.Right - padding - navButtonSize, currentY + 2, navButtonSize, navButtonSize);

            currentY += 44;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 26);
            currentY += 30;

            int gridWidth = bounds.Width - padding * 2;
            int gridHeight = bounds.Bottom - currentY - padding;
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);

            layout.CellWidth = gridWidth / 7;
            layout.CellHeight = gridHeight / 6;
            layout.DayCellMatrix = new Rectangle[6, 7];

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    layout.DayCellMatrix[row, col] = new Rectangle(
                        layout.CalendarGridRect.X + col * layout.CellWidth,
                        layout.CalendarGridRect.Y + row * layout.CellHeight,
                        layout.CellWidth,
                        layout.CellHeight
                    );
                }
            }

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
            // Quick buttons + calendar: Buttons(100) + Calendar(280) + Padding = 400px
            int width = 380;
            int height = 400; // Modern card layout height

            if (properties.ShowCustomQuickDates)
            {
                height += 60;
            }

            return new Size(width, height);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Minimum for quick buttons + compact calendar
            // Buttons(80) + Calendar(240) + Padding = 340px
            return new Size(360, 340);
        }
    }
}
