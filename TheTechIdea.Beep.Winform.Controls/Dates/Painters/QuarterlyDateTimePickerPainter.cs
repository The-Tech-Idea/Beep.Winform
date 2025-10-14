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
    /// Quarterly Date Picker Painter
    /// Quick quarter selection (Q1-Q4) with fiscal year support and date range display
    /// Visual styling follows BeepTheme
    /// </summary>
    public class QuarterlyDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.Quarterly;

        public QuarterlyDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
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

            // Paint header with year selector
            var headerRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 50);
            PaintQuarterlyHeader(g, headerRect, displayMonth.Year, hoverState);
            currentY += 70;

            // Paint quarter grid (2x2)
            var quarterGridRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 240);
            PaintQuarterGrid(g, quarterGridRect, displayMonth.Year, hoverState);
            currentY += 260;

            // Paint selected range display
            var rangeRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 80);
            PaintSelectedRangeDisplay(g, rangeRect);
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

        private void PaintQuarterlyHeader(Graphics g, Rectangle bounds, int year, DateTimePickerHoverState hoverState)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(100, 100, 100);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 16f, FontStyle.Bold);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);

            // Year display
            var yearText = $"FY {year}";
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(yearText, boldFont, brush, new Rectangle(bounds.X, bounds.Y, bounds.Width, 30), format);
            }

            // Year navigation buttons
            int buttonSize = 32;
            var prevYearRect = new Rectangle(bounds.X, bounds.Y, buttonSize, buttonSize);
            var nextYearRect = new Rectangle(bounds.Right - buttonSize, bounds.Y, buttonSize, buttonSize);

            PaintYearNavButton(g, prevYearRect, false, false, false);
            PaintYearNavButton(g, nextYearRect, true, false, false);

            // Subtitle
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString("Select Quarter", font, brush, new Rectangle(bounds.X, bounds.Y + 34, bounds.Width, 16), format);
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

            using (var pen = new Pen(iconColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = bounds.X + bounds.Width / 2;
                int centerY = bounds.Y + bounds.Height / 2;
                int size = 6;

                if (isNext)
                {
                    g.DrawLine(pen, centerX - 3, centerY - size, centerX + 3, centerY);
                    g.DrawLine(pen, centerX + 3, centerY, centerX - 3, centerY + size);
                }
                else
                {
                    g.DrawLine(pen, centerX + 3, centerY - size, centerX - 3, centerY);
                    g.DrawLine(pen, centerX - 3, centerY, centerX + 3, centerY + size);
                }
            }
        }

        private void PaintQuarterGrid(Graphics g, Rectangle bounds, int year, DateTimePickerHoverState hoverState)
        {
            int gap = 16;
            int cardWidth = (bounds.Width - gap) / 2;
            int cardHeight = (bounds.Height - gap) / 2;

            // Q1
            var q1Rect = new Rectangle(bounds.X, bounds.Y, cardWidth, cardHeight);
            PaintQuarterCard(g, q1Rect, 1, year, IsQuarterSelected(1, year), false, false);

            // Q2
            var q2Rect = new Rectangle(bounds.X + cardWidth + gap, bounds.Y, cardWidth, cardHeight);
            PaintQuarterCard(g, q2Rect, 2, year, IsQuarterSelected(2, year), false, false);

            // Q3
            var q3Rect = new Rectangle(bounds.X, bounds.Y + cardHeight + gap, cardWidth, cardHeight);
            PaintQuarterCard(g, q3Rect, 3, year, IsQuarterSelected(3, year), false, false);

            // Q4
            var q4Rect = new Rectangle(bounds.X + cardWidth + gap, bounds.Y + cardHeight + gap, cardWidth, cardHeight);
            PaintQuarterCard(g, q4Rect, 4, year, IsQuarterSelected(4, year), false, false);
        }

        private void PaintQuarterCard(Graphics g, Rectangle bounds, int quarter, int year, bool isSelected, bool isHovered, bool isPressed)
        {
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(100, 100, 100);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 20f, FontStyle.Bold);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);
            var smallFont = new Font("Segoe UI", 9f);

            // Background
            if (isSelected)
            {
                using (var brush = new SolidBrush(accentColor))
                using (var path = GetRoundedRectPath(bounds, 12))
                {
                    g.FillPath(brush, path);
                }
                textColor = Color.White;
                secondaryTextColor = Color.FromArgb(230, 230, 230);
            }
            else if (isPressed || isHovered)
            {
                using (var brush = new SolidBrush(hoverColor))
                using (var path = GetRoundedRectPath(bounds, 12))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(accentColor, 2))
                using (var path = GetRoundedRectPath(bounds, 12))
                {
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                using (var pen = new Pen(borderColor, 1))
                using (var path = GetRoundedRectPath(bounds, 12))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Quarter label
            string quarterText = $"Q{quarter}";
            using (var brush = new SolidBrush(isSelected ? Color.White : accentColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(quarterText, boldFont, brush, new Rectangle(bounds.X, bounds.Y + 20, bounds.Width, 30), format);
            }

            // Get quarter date range
            var (startMonth, endMonth) = GetQuarterMonths(quarter);
            var startDate = new DateTime(year, startMonth, 1);
            var endDate = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

            // Date range
            string dateRange = $"{startDate:MMM d} - {endDate:MMM d}";
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(dateRange, font, brush, new Rectangle(bounds.X, bounds.Y + 60, bounds.Width, 20), format);
            }

            // Month names
            string months = $"{startDate:MMM}, {startDate.AddMonths(1):MMM}, {startDate.AddMonths(2):MMM}";
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(months, smallFont, brush, new Rectangle(bounds.X, bounds.Y + 85, bounds.Width, 16), format);
            }

            // Days count
            int days = (endDate - startDate).Days + 1;
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString($"{days} days", smallFont, brush, new Rectangle(bounds.X, bounds.Bottom - 26, bounds.Width, 16), format);
            }
        }

        private void PaintSelectedRangeDisplay(Graphics g, Rectangle bounds)
        {
            var textColor = _theme?.ForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.SecondaryTextColor ?? Color.FromArgb(100, 100, 100);
            var accentColor = _theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(230, 230, 230);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 12f, FontStyle.Bold);

            // Background panel
            using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
            using (var path = GetRoundedRectPath(bounds, 8))
            {
                g.FillPath(brush, path);
            }
            using (var pen = new Pen(borderColor, 1))
            using (var path = GetRoundedRectPath(bounds, 8))
            {
                g.DrawPath(pen, path);
            }

            if (_owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue)
            {
                // Title
                using (var brush = new SolidBrush(secondaryTextColor))
                {
                    g.DrawString("Selected Range", font, brush, bounds.X + 16, bounds.Y + 12);
                }

                // Date range
                string rangeText = $"{_owner.RangeStartDate.Value:MMM d, yyyy} — {_owner.RangeEndDate.Value:MMM d, yyyy}";
                using (var brush = new SolidBrush(accentColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString(rangeText, boldFont, brush, new Rectangle(bounds.X, bounds.Y + 32, bounds.Width, 24), format);
                }

                // Days count
                int days = (_owner.RangeEndDate.Value - _owner.RangeStartDate.Value).Days + 1;
                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString($"{days} days", font, brush, new Rectangle(bounds.X, bounds.Y + 58, bounds.Width, 16), format);
                }
            }
            else
            {
                // Placeholder
                using (var brush = new SolidBrush(secondaryTextColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("No quarter selected", font, brush, bounds, format);
                }
            }
        }

        private bool IsQuarterSelected(int quarter, int year)
        {
            if (!_owner.RangeStartDate.HasValue || !_owner.RangeEndDate.HasValue) return false;

            var (startMonth, endMonth) = GetQuarterMonths(quarter);
            var quarterStart = new DateTime(year, startMonth, 1);
            var quarterEnd = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

            return _owner.RangeStartDate.Value.Date == quarterStart.Date &&
                   _owner.RangeEndDate.Value.Date == quarterEnd.Date;
        }

        private (int startMonth, int endMonth) GetQuarterMonths(int quarter)
        {
            return quarter switch
            {
                1 => (1, 3),   // Jan-Mar
                2 => (4, 6),   // Apr-Jun
                3 => (7, 9),   // Jul-Sep
                4 => (10, 12), // Oct-Dec
                _ => (1, 3)
            };
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
            // Quarter buttons + month grid: Buttons(100) + Grid(300) + Padding = 420px
            return new Size(380, 420);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Minimum for quarter selector + month grid
            // Buttons(80) + Grid(240) + Padding = 340px
            return new Size(340, 340);
        }
    }
}
