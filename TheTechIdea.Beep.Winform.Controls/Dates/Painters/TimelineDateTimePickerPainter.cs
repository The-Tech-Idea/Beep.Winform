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
    /// Timeline Date Picker Painter
    /// Visual timeline bar with draggable range handles and date labels
    /// Visual styling follows BeepTheme
    /// </summary>
    public class TimelineDateTimePickerPainter : IDateTimePickerPainter
    {
    private readonly BeepDateTimePicker _owner;
    private readonly IBeepTheme _theme;

        public DatePickerMode Mode => DatePickerMode.Timeline;

        public TimelineDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme)
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

            // Paint header
            var headerRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 40);
            PaintTimelineHeader(g, headerRect);
            currentY += 60;

            // Paint timeline bar
            var timelineRect = new Rectangle(bounds.X + padding * 2, currentY, bounds.Width - padding * 4, 80);
            PaintTimelineBar(g, timelineRect, hoverState);
            currentY += 100;

            // Paint date labels
            var labelsRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 60);
            PaintDateLabels(g, labelsRect);
            currentY += 80;

            // Paint mini calendar for reference
            var miniCalendarRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, 160);
            PaintMiniCalendar(g, miniCalendarRect, displayMonth, properties, hoverState);
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

        private void PaintTimelineHeader(Graphics g, Rectangle bounds)
        {
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 14f, FontStyle.Bold);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString("Date Range Timeline", boldFont, brush, bounds.X, bounds.Y);
            }

            using (var brush = new SolidBrush(secondaryTextColor))
            {
                string subtitle = "Drag handles to adjust range";
                g.DrawString(subtitle, font, brush, bounds.X, bounds.Y + 24);
            }
        }

        private void PaintTimelineBar(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            var trackColor = Color.FromArgb(230, 230, 230);

            // Timeline track
            var trackRect = new Rectangle(bounds.X, bounds.Y + 30, bounds.Width, 20);
            using (var brush = new SolidBrush(trackColor))
            {
                g.FillRoundedRectangle(brush, trackRect, 10);
            }

            // Calculate range positions
            DateTime startDate = _owner.RangeStartDate ?? DateTime.Today;
            DateTime endDate = _owner.RangeEndDate ?? DateTime.Today.AddDays(30);
            DateTime minDate = DateTime.Today.AddMonths(-6);
            DateTime maxDate = DateTime.Today.AddMonths(6);

            float totalDays = (float)(maxDate - minDate).TotalDays;
            float startPos = (float)(startDate - minDate).TotalDays / totalDays;
            float endPos = (float)(endDate - minDate).TotalDays / totalDays;

            int startX = bounds.X + (int)(bounds.Width * startPos);
            int endX = bounds.X + (int)(bounds.Width * endPos);

            // Range fill
            var rangeRect = new Rectangle(startX, trackRect.Y, endX - startX, trackRect.Height);
            using (var brush = new SolidBrush(accentColor))
            {
                g.FillRoundedRectangle(brush, rangeRect, 10);
            }

            // Start handle
            var startHandleRect = new Rectangle(startX - 12, trackRect.Y - 10, 24, 40);
            PaintHandle(g, startHandleRect, "Start", true, false, false);

            // End handle
            var endHandleRect = new Rectangle(endX - 12, trackRect.Y - 10, 24, 40);
            PaintHandle(g, endHandleRect, "End", false, false, false);

            // Month markers
            PaintMonthMarkers(g, new Rectangle(bounds.X, bounds.Y, bounds.Width, 25), minDate, maxDate);
        }

        private void PaintHandle(Graphics g, Rectangle bounds, string label, bool isStart, bool isHovered, bool isDragging)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var hoverColor = ControlPaint.Light(accentColor, 0.2f);

            Color handleColor = isDragging ? ControlPaint.Dark(accentColor, 0.1f) :
                               isHovered ? hoverColor : accentColor;

            // Handle shape (rounded rectangle)
            using (var brush = new SolidBrush(handleColor))
            using (var path = GetRoundedRectPath(bounds, 6))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(Color.White, 2))
            using (var path = GetRoundedRectPath(bounds, 6))
            {
                g.DrawPath(pen, path);
            }

            // Drag indicator lines
            using (var pen = new Pen(Color.White, 1.5f))
            {
                int centerX = bounds.X + bounds.Width / 2;
                int lineY1 = bounds.Y + bounds.Height / 2 - 6;
                int lineY2 = bounds.Y + bounds.Height / 2 - 2;
                int lineY3 = bounds.Y + bounds.Height / 2 + 2;
                int lineY4 = bounds.Y + bounds.Height / 2 + 6;

                g.DrawLine(pen, centerX - 4, lineY1, centerX + 4, lineY1);
                g.DrawLine(pen, centerX - 4, lineY2, centerX + 4, lineY2);
                g.DrawLine(pen, centerX - 4, lineY3, centerX + 4, lineY3);
                g.DrawLine(pen, centerX - 4, lineY4, centerX + 4, lineY4);
            }
        }

        private void PaintMonthMarkers(Graphics g, Rectangle bounds, DateTime minDate, DateTime maxDate)
        {
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(150, 150, 150);
            var borderColor = _theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            var font = new Font("Segoe UI", 7f);

            DateTime current = new DateTime(minDate.Year, minDate.Month, 1);
            float totalDays = (float)(maxDate - minDate).TotalDays;

            while (current <= maxDate)
            {
                float pos = (float)(current - minDate).TotalDays / totalDays;
                int x = bounds.X + (int)(bounds.Width * pos);

                // Tick mark
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawLine(pen, x, bounds.Y + 20, x, bounds.Y + 25);
                }

                // Month label
                if (current.Month % 2 == 1) // Show every other month to avoid crowding
                {
                    using (var brush = new SolidBrush(secondaryTextColor))
                    {
                        string label = current.ToString("MMM");
                        var size = TextUtils.MeasureText(g,label, font);
                        g.DrawString(label, font, brush, x - size.Width / 2, bounds.Y);
                    }
                }

                current = current.AddMonths(1);
            }
        }

        private void PaintDateLabels(Graphics g, Rectangle bounds)
        {
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 10f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 11f, FontStyle.Bold);

            DateTime startDate = _owner.RangeStartDate ?? DateTime.Today;
            DateTime endDate = _owner.RangeEndDate ?? DateTime.Today.AddDays(30);
            int days = (endDate - startDate).Days + 1;

            int halfWidth = bounds.Width / 2 - 10;

            // Start date
            var startRect = new Rectangle(bounds.X, bounds.Y, halfWidth, bounds.Height);
            PaintDateLabel(g, startRect, "Start Date", startDate, days);

            // Arrow
            using (var pen = new Pen(accentColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.ArrowAnchor;
                int y = bounds.Y + 30;
                g.DrawLine(pen, bounds.X + halfWidth + 10, y, bounds.X + bounds.Width - halfWidth - 10, y);
            }

            // End date
            var endRect = new Rectangle(bounds.X + bounds.Width - halfWidth, bounds.Y, halfWidth, bounds.Height);
            PaintDateLabel(g, endRect, "End Date", endDate, days);
        }

        private void PaintDateLabel(Graphics g, Rectangle bounds, string label, DateTime date, int totalDays)
        {
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var secondaryTextColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f) ?? new Font("Segoe UI", 9f);
            var boldFont = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 13f, FontStyle.Bold);

            int y = bounds.Y;

            // Label
            using (var brush = new SolidBrush(secondaryTextColor))
            {
                g.DrawString(label, font, brush, bounds.X, y);
            }
            y += 20;

            // Date
            using (var brush = new SolidBrush(accentColor))
            {
                g.DrawString(date.ToString("MMM d, yyyy"), boldFont, brush, bounds.X, y);
            }
            y += 24;

            // Day of week
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(date.ToString("dddd"), font, brush, bounds.X, y);
            }
        }

        private void PaintMiniCalendar(Graphics g, Rectangle bounds, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState)
        {
            var layout = CalculateMiniLayout(bounds, properties);

            // Compact header
            var textColor = _theme?.CalendarTitleForColor ?? Color.Black;
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 10f, FontStyle.Bold);
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(displayMonth.ToString("MMMM yyyy"), font, brush, layout.HeaderRect, format);
            }

            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);
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

        public void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange)
        {
            var accentColor = _theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(0, 120, 215);
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var todayColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(240, 240, 240);
            var rangeColor = Color.FromArgb(60, accentColor);

            cellBounds.Inflate(-1, -1);

            // Range background
            if (isInRange)
            {
                using (var brush = new SolidBrush(rangeColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }

            // Selection
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

            // Today indicator
            if (isToday && !isSelected)
            {
                using (var pen = new Pen(todayColor, 1))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

            // Day number
            var dayText = date.Day.ToString();
            var font = new Font("Segoe UI", 8f);
            
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
            var font = new Font(_theme?.FontName ?? "Segoe UI", 10f, FontStyle.Bold) ?? new Font("Segoe UI", 10f, FontStyle.Bold);

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

        public void PaintNavigationButton(Graphics g, Rectangle buttonBounds, bool isNext, bool isHovered, bool isPressed) { }

        public void PaintDayNamesHeader(Graphics g, Rectangle headerBounds, DatePickerFirstDayOfWeek firstDayOfWeek)
        {
            var textColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(128, 128, 128);
            var font = new Font("Segoe UI", 7f);

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

            for (int i = 0; i < dayOffset; i++)
            {
                var day = daysInPrevMonth - dayOffset + i + 1;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, day);
                var cellRect = layout.DayCellRects[row, col];
                bool isInRange = IsInRange(date);
                PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = layout.DayCellRects[row, col];
                
                bool isStartOrEnd = (_owner.RangeStartDate.HasValue && _owner.RangeStartDate.Value.Date == date.Date) ||
                                   (_owner.RangeEndDate.HasValue && _owner.RangeEndDate.Value.Date == date.Date);
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInRange = IsInRange(date);
                
                PaintDayCell(g, cellRect, date, isStartOrEnd, isToday, isDisabled, isHovered, isPressed, isInRange);
                
                col++;
                if (col >= 7) { col = 0; row++; }
            }

            var nextMonth = displayMonth.AddMonths(1);
            while (row < 6)
            {
                for (int day = 1; col < 7; day++)
                {
                    var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
                    var cellRect = layout.DayCellRects[row, col];
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
            return CalculateMiniLayout(bounds, properties);
        }

        private DateTimePickerLayout CalculateMiniLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            int padding = 8;

            layout.HeaderRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, 24);
            layout.DayNamesRect = new Rectangle(bounds.X, bounds.Y + 28, bounds.Width, 18);

            int gridWidth = bounds.Width;
            int gridHeight = bounds.Height - 50;
            layout.CalendarGridRect = new Rectangle(bounds.X, bounds.Y + 50, gridWidth, gridHeight);

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
            // Simplified hit testing for mini calendar
            return result;
        }

        public Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Timeline visual + calendar: Timeline(80) + Calendar(280) + Padding = 380px
            return new Size(480, 380);
        }

        public Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Minimum for timeline + compact calendar
            // Timeline(60) + Calendar(240) + Padding = 320px
            return new Size(400, 320);
        }
    }
}

// Extension for filled rounded rectangles
public static class FilledRoundedRectExtensions
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
    {
        using (var path = GetRoundedRectPath(rect, radius))
        {
            g.FillPath(brush, path);
        }
    }

    private static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
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
}
