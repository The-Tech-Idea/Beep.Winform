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
    /// Month View Date Picker Painter
    /// 3x4 grid of month selectors for quick month selection with year combo box
    /// Visual styling follows BeepTheme
    /// </summary>
    public class MonthViewDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.MonthView;

        public MonthViewDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
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

            int padding = 20;
            int currentY = bounds.Y + padding;

            // Year header with navigation
            var headerRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 50);
            PaintYearHeader(g, headerRect, displayMonth.Year, hoverState);
            currentY += 70;

            // Month grid (3x4)
            var monthGridRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, bounds.Height - currentY - padding);
            PaintMonthGrid(g, monthGridRect, displayMonth.Year, hoverState);
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

        private void PaintYearHeader(Graphics g, Rectangle bounds, int year, DateTimePickerHoverState hoverState)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 18f, FontStyle.Bold);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 9f);

            // Navigation buttons
            int buttonSize = 36;
            var prevYearRect = new Rectangle(bounds.X, bounds.Y + 2, buttonSize, buttonSize);
            var nextYearRect = new Rectangle(bounds.Right - buttonSize, bounds.Y + 2, buttonSize, buttonSize);

            bool prevHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.PreviousYearButton) == true;
            bool prevPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.PreviousYearButton) == true;
            bool nextHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.NextYearButton) == true;
            bool nextPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.NextYearButton) == true;

            PaintYearNavButton(g, prevYearRect, false, prevHovered, prevPressed);
            PaintYearNavButton(g, nextYearRect, true, nextHovered, nextPressed);

            // Year ComboBox in center (direct year selection)
            int comboWidth = 120;
            int comboHeight = 32;
            int comboX = bounds.X + (bounds.Width - comboWidth) / 2;
            int comboY = bounds.Y + 5;
            var yearComboRect = new Rectangle(comboX, comboY, comboWidth, comboHeight);

            bool comboHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.YearComboBox) == true;
            bool comboPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.YearComboBox) == true;

            PaintYearComboBox(g, yearComboRect, year, comboHovered, comboPressed);

            // "Quick Select" label below combo
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                var labelRect = new Rectangle(bounds.X, comboY + comboHeight + 2, bounds.Width, 12);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString("or use navigation buttons", font, brush, labelRect, format);
            }
        }

        private void PaintYearComboBox(Graphics g, Rectangle bounds, int selectedYear, bool isHovered, bool isPressed)
        {
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var borderColor = _theme?.CalendarBorderColor ?? Color.FromArgb(200, 200, 200);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var bgColor = _theme?.CalendarBackColor ?? Color.White;
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold);

            // Background
            using (var brush = new SolidBrush(isPressed || isHovered ? hoverColor : bgColor))
            using (var path = GetRoundedRectPath(bounds, 6))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(isPressed ? accentColor : (isHovered ? accentColor : borderColor), isPressed || isHovered ? 2 : 1))
            using (var path = GetRoundedRectPath(bounds, 6))
            {
                g.DrawPath(pen, path);
            }

            // Selected year text
            using (var brush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(bounds.X + 10, bounds.Y, bounds.Width - 30, bounds.Height);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(selectedYear.ToString(), font, brush, textRect, format);
            }

            // Dropdown arrow icon
            var arrowColor = isPressed || isHovered ? accentColor : Color.FromArgb(120, 120, 120);
            using (var pen = new Pen(arrowColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int cx = bounds.Right - 16;
                int cy = bounds.Y + bounds.Height / 2;
                g.DrawLine(pen, cx - 3, cy - 2, cx, cy + 1);
                g.DrawLine(pen, cx, cy + 1, cx + 3, cy - 2);
            }
        }

        private void PaintYearNavButton(Graphics g, Rectangle bounds, bool isNext, bool isHovered, bool isPressed)
        {
            var iconColor = _theme?.ForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);

            if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillEllipse(brush, bounds);
                }
            }

            using (var pen = new Pen(iconColor, 2.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = bounds.X + bounds.Width / 2;
                int centerY = bounds.Y + bounds.Height / 2;
                int size = 7;

                if (isNext)
                {
                    g.DrawLine(pen, centerX - 3, centerY - size, centerX + 4, centerY);
                    g.DrawLine(pen, centerX + 4, centerY, centerX - 3, centerY + size);
                }
                else
                {
                    g.DrawLine(pen, centerX + 3, centerY - size, centerX - 4, centerY);
                    g.DrawLine(pen, centerX - 4, centerY, centerX + 3, centerY + size);
                }
            }
        }

        private void PaintMonthGrid(Graphics g, Rectangle bounds, int year, DateTimePickerHoverState hoverState)
        {
            string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            
            int rows = 4;
            int cols = 3;
            int gap = 12;
            
            int cellWidth = (bounds.Width - gap * (cols - 1)) / cols;
            int cellHeight = (bounds.Height - gap * (rows - 1)) / rows;

            for (int month = 1; month <= 12; month++)
            {
                int row = (month - 1) / cols;
                int col = (month - 1) % cols;

                var cellRect = new Rectangle(
                    bounds.X + col * (cellWidth + gap),
                    bounds.Y + row * (cellHeight + gap),
                    cellWidth,
                    cellHeight
                );

                bool isSelected = _owner.SelectedDate.HasValue && 
                                 _owner.SelectedDate.Value.Year == year && 
                                 _owner.SelectedDate.Value.Month == month;

                bool isCurrent = DateTime.Today.Year == year && DateTime.Today.Month == month;

                // Check if this specific month cell is hovered/pressed
                bool isHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.MonthCell) == true &&
                                hoverState.HoveredDate.HasValue &&
                                hoverState.HoveredDate.Value.Year == year &&
                                hoverState.HoveredDate.Value.Month == month;
                bool isPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.MonthCell) == true &&
                                hoverState.PressedDate.HasValue &&
                                hoverState.PressedDate.Value.Year == year &&
                                hoverState.PressedDate.Value.Month == month;

                PaintMonthCell(g, cellRect, month, monthNames[month - 1], isSelected, isCurrent, isHovered, isPressed);
            }
        }

        private void PaintMonthCell(Graphics g, Rectangle bounds, int month, string monthName, bool isSelected, bool isCurrent, bool isHovered, bool isPressed)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(100, 100, 100);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 12f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 12f, FontStyle.Bold);

            // Background
            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                using (var path = GetRoundedRectPath(bounds, 10))
                {
                    g.FillPath(brush, path);
                }
                textColor = Color.White;
                secondaryTextColor = Color.FromArgb(230, 230, 230);
            }
            else if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                using (var path = GetRoundedRectPath(bounds, 10))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(accentColor, 2))
                using (var path = GetRoundedRectPath(bounds, 10))
                {
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                using (var pen = new Pen(borderColor, 1))
                using (var path = GetRoundedRectPath(bounds, 10))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Current month indicator
            if (isCurrent && !isSelected)
            {
                using (var pen = new Pen(accentColor, 2))
                using (var path = GetRoundedRectPath(bounds, 10))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Month name
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(monthName, isSelected ? boldFont : font, brush, 
                    new Rectangle(bounds.X, bounds.Y + bounds.Height / 2 - 20, bounds.Width, 24), format);
            }

            // Month number
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                var smallFont = new Font("Segoe UI", 9f);
                g.DrawString($"Month {month}", smallFont, brush, 
                    new Rectangle(bounds.X, bounds.Y + bounds.Height / 2 + 8, bounds.Width, 16), format);
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

        // Stub implementations
        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false) { }
        public void PaintHeader(Graphics g, Rectangle headerBounds, string headerText, bool showNavigation, bool isHovered) { }
        public void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed) { }
        public void PaintDayNamesHeader(Graphics g, Rectangle headerBounds, DatePickerFirstDayOfWeek firstDayOfWeek) { }
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
            
            int padding = 20;
            int currentY = bounds.Y + padding;

            // Year header with navigation and year combo (60px height)
            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 60);
            
            int buttonSize = 36;
            layout.PreviousYearButtonRect = new Rectangle(layout.HeaderRect.X, layout.HeaderRect.Y + 2, buttonSize, buttonSize);
            layout.NextYearButtonRect = new Rectangle(layout.HeaderRect.Right - buttonSize, layout.HeaderRect.Y + 2, buttonSize, buttonSize);
            
            // Year ComboBox in center
            int comboWidth = 120;
            int comboHeight = 32;
            int comboX = bounds.X + (bounds.Width - comboWidth) / 2;
            int comboY = layout.HeaderRect.Y + 5;
            layout.YearComboBoxRect = new Rectangle(comboX, comboY, comboWidth, comboHeight);

            currentY += 80;

            // Month grid (3x4) - remaining height
            var monthGridRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, bounds.Height - currentY - padding);
            
            int rows = 4;
            int cols = 3;
            int gap = 12;
            
            int cellWidth = (monthGridRect.Width - gap * (cols - 1)) / cols;
            int cellHeight = (monthGridRect.Height - gap * (rows - 1)) / rows;

            // Calculate 12 month cell rectangles
            layout.MonthCellRects = new List<Rectangle>();
            for (int month = 1; month <= 12; month++)
            {
                int row = (month - 1) / cols;
                int col = (month - 1) % cols;

                var cellRect = new Rectangle(
                    monthGridRect.X + col * (cellWidth + gap),
                    monthGridRect.Y + row * (cellHeight + gap),
                    cellWidth,
                    cellHeight
                );
                
                layout.MonthCellRects.Add(cellRect);
            }
            
            // Register all hit areas with BaseControl's hit test system  
            _owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);
            
            return layout;
        }

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth)
        {
            return new DateTimePickerHitTestResult();
        }

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Month grid (3x4 or 4x3): Header(50) + Grid(320) + Padding = 390px
            return new Size(380, 390);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Minimum for month grid: Header(40) + Grid(260) + Padding = 320px
            return new Size(340, 320);
        }
    }
}
