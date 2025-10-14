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
    /// Year View Date Picker Painter
    /// Year range selector with decade navigation
    /// Visual styling follows BeepTheme
    /// </summary>
    public class YearViewDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.YearView;

        public YearViewDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
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

            // Decade header with navigation
            int startYear = (displayMonth.Year / 10) * 10;
            int endYear = startYear + 9;
            
            var headerRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 50);
            PaintDecadeHeader(g, headerRect, startYear, endYear, hoverState);
            currentY += 70;

            // Year grid (4x3 = 12 years, showing start-1 to start+10)
            var yearGridRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, bounds.Height - currentY - padding);
            PaintYearGrid(g, yearGridRect, startYear, hoverState);
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

        private void PaintDecadeHeader(Graphics g, Rectangle bounds, int startYear, int endYear, DateTimePickerHoverState hoverState)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 18f, FontStyle.Bold);

            // Decade range display
            string decadeText = $"{startYear} — {endYear}";
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(decadeText, boldFont, brush, new Rectangle(bounds.X, bounds.Y, bounds.Width, 40), format);
            }

            // Navigation buttons
            int buttonSize = 36;
            var prevDecadeRect = new Rectangle(bounds.X, bounds.Y + 2, buttonSize, buttonSize);
            var nextDecadeRect = new Rectangle(bounds.Right - buttonSize, bounds.Y + 2, buttonSize, buttonSize);

            PaintDecadeNavButton(g, prevDecadeRect, false, false, false);
            PaintDecadeNavButton(g, nextDecadeRect, true, false, false);
        }

        private void PaintDecadeNavButton(Graphics g, Rectangle bounds, bool isNext, bool isHovered, bool isPressed)
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
                    // Double chevron for decade
                    g.DrawLine(pen, centerX - 6, centerY - size, centerX - 2, centerY);
                    g.DrawLine(pen, centerX - 2, centerY, centerX - 6, centerY + size);
                    g.DrawLine(pen, centerX + 0, centerY - size, centerX + 4, centerY);
                    g.DrawLine(pen, centerX + 4, centerY, centerX + 0, centerY + size);
                }
                else
                {
                    // Double chevron for decade
                    g.DrawLine(pen, centerX + 6, centerY - size, centerX + 2, centerY);
                    g.DrawLine(pen, centerX + 2, centerY, centerX + 6, centerY + size);
                    g.DrawLine(pen, centerX + 0, centerY - size, centerX - 4, centerY);
                    g.DrawLine(pen, centerX - 4, centerY, centerX + 0, centerY + size);
                }
            }
        }

        private void PaintYearGrid(Graphics g, Rectangle bounds, int startYear, DateTimePickerHoverState hoverState)
        {
            int rows = 4;
            int cols = 3;
            int gap = 10;

            int cellWidth = (bounds.Width - gap * (cols - 1)) / cols;
            int cellHeight = (bounds.Height - gap * (rows - 1)) / rows;

            // Show startYear-1 to startYear+10 (12 years total)
            for (int i = 0; i < 12; i++)
            {
                int year = startYear - 1 + i;
                int row = i / cols;
                int col = i % cols;

                var cellRect = new Rectangle(
                    bounds.X + col * (cellWidth + gap),
                    bounds.Y + row * (cellHeight + gap),
                    cellWidth,
                    cellHeight
                );

                bool isSelected = _owner.SelectedDate.HasValue && _owner.SelectedDate.Value.Year == year;
                bool isCurrent = DateTime.Today.Year == year;
                bool isOutOfDecade = (i == 0 || i == 11); // First and last are outside main decade

                PaintYearCell(g, cellRect, year, isSelected, isCurrent, isOutOfDecade, false, false);
            }
        }

        private void PaintYearCell(Graphics g, Rectangle bounds, int year, bool isSelected, bool isCurrent, bool isOutOfDecade, bool isHovered, bool isPressed)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(100, 100, 100);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 16f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 16f, FontStyle.Bold);

            // Out of decade years are slightly faded
            if (isOutOfDecade)
            {
                textColor = Color.FromArgb(160, 160, 160);
                secondaryTextColor = Color.FromArgb(180, 180, 180);
            }

            // Background
            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                using (var path = GetRoundedRectPath(bounds, 10))
                {
                    g.FillPath(brush, path);
                }
                textColor = Color.White;
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

            // Current year indicator
            if (isCurrent && !isSelected)
            {
                using (var pen = new Pen(accentColor, 2))
                using (var path = GetRoundedRectPath(bounds, 10))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Year number
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(year.ToString(), isSelected ? boldFont : font, brush, bounds, format);
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
        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange) { }
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
            return new DateTimePickerLayout();
        }

        public DateTimePickerHitTestResult HitTest(Point location, DateTimePickerLayout layout, DateTime displayMonth)
        {
            return new DateTimePickerHitTestResult();
        }

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Year grid (4x3): Header(50) + Grid(320) + Padding = 390px
            return new Size(360, 380);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Minimum for year grid: Header(40) + Grid(260) + Padding = 320px
            return new Size(320, 320);
        }
    }
}
