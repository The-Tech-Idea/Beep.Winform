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
    /// Range Date Painter
    /// Select start and end dates with visual range highlighting
    /// Visual styling follows BeepTheme
    /// </summary>
    public class RangeDateTimePickerPainter : DateTimePickerPainterBase
    {
        public override DatePickerMode Mode => DatePickerMode.Range;

        public RangeDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme) : base(owner, theme) { }

        public override void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var layout = CalculateLayout(bounds, properties);

            // Paint background
            PaintBackground(g, bounds);

            // Paint range selection highlighting FIRST (behind cells)
            PaintRangeSelection(g, layout.CalendarGridRect, _owner.RangeStartDate, _owner.RangeEndDate, displayMonth);

            // Paint header
            PaintHeader(g, layout.HeaderRect, displayMonth.ToString("MMMM yyyy"), true, false);
            
            PaintNavigationButton(g, layout.PreviousButtonRect, false, 
                hoverState?.IsAreaHovered(DateTimePickerHitArea.PreviousButton) ?? false,
                hoverState?.IsAreaPressed(DateTimePickerHitArea.PreviousButton) ?? false);
            
            PaintNavigationButton(g, layout.NextButtonRect, true,
                hoverState?.IsAreaHovered(DateTimePickerHitArea.NextButton) ?? false,
                hoverState?.IsAreaPressed(DateTimePickerHitArea.NextButton) ?? false);

            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);

            // Paint range info label at bottom, only if space allows
            int bottomSpace = bounds.Bottom - (layout.CalendarGridRect.Bottom + 8);
            if (bottomSpace >= 16)
            {
                int infoHeight = Math.Min(24, bottomSpace);
                PaintRangeInfo(g, new Rectangle(bounds.X + 10, layout.CalendarGridRect.Bottom + 8, bounds.Width - 20, infoHeight));
            }
        }

        private void PaintBackground(Graphics g, Rectangle bounds)
        {
            // BaseControl (Minimalist) handles container border; fill interior content area only
            var bgColor = _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        public override void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth)
        {
            // Range bands are painted per-cell in PaintDayCell; nothing to overlay here.
        }

        private void PaintRangeInfo(Graphics g, Rectangle infoBounds)
        {
            var textColor = _theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(100, 100, 100);
            using (var font = new Font(_theme?.FontName ?? "Segoe UI", 10f))
            {
                string infoText = "";
                if (_owner.RangeStartDate.HasValue && _owner.RangeEndDate.HasValue)
                {
                    var days = (_owner.RangeEndDate.Value - _owner.RangeStartDate.Value).Days + 1;
                    infoText = $"{_owner.RangeStartDate.Value:MMM d} - {_owner.RangeEndDate.Value:MMM d} ({days} day{(days != 1 ? "s" : "")})";
                }
                else if (_owner.RangeStartDate.HasValue)
                {
                    infoText = $"Start: {_owner.RangeStartDate.Value:MMM d, yyyy} (select end date)";
                }
                else
                {
                    infoText = "Select start date";
                }

                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(infoText, font, brush, infoBounds, format);
                }
            }
        }

        public override void PaintDayCell(Graphics g, Rectangle cellBounds, DateTime date, bool isSelected, bool isToday, bool isDisabled, bool isHovered, bool isPressed, bool isInRange, bool isStartDate = false, bool isEndDate = false)
        {
            var rangeColor = Color.FromArgb(50, AccentColor);

            cellBounds.Inflate(-S(2), -S(2));

            // Paint the range band behind non-endpoint cells
            if (isInRange && !isStartDate && !isEndDate)
            {
                using (var brush = new SolidBrush(rangeColor))
                {
                    g.FillRectangle(brush, cellBounds);
                }
            }

            Color textColor = TextColor;

            // Start and end use the same accent so the range reads as one control.
            if (isStartDate || isEndDate)
            {
                using (var brush = new SolidBrush(AccentColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
            }
            else if (isSelected)
            {
                using (var brush = new SolidBrush(AccentColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
                textColor = _theme?.CalendarSelectedDateForColor ?? Color.White;
            }
            else if (isPressed)
            {
                using (var brush = new SolidBrush(ShiftLuminance(AccentColor, -0.1f)))
                {
                    g.FillEllipse(brush, cellBounds);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(HoverColor))
                {
                    g.FillEllipse(brush, cellBounds);
                }
            }

            if (isToday && !isSelected && !isStartDate && !isEndDate)
            {
                using (var pen = new Pen(TodayColor, S(2)))
                {
                    g.DrawEllipse(pen, cellBounds);
                }
            }

            if (isDisabled)
            {
                textColor = Muted(TextColor, 96);
            }

            using (var font = CellFont)
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(date.Day.ToString(), font, brush, cellBounds, format);
            }
        }

        private void PaintCalendarGrid(Graphics g, DateTimePickerLayout layout, DateTime displayMonth, DateTimePickerProperties properties, DateTimePickerHoverState hoverState)
        {
            // Use DayCellMatrix directly for consistent access with hit handler
            var cells = layout.DayCellMatrix;
            if (cells == null)
            {
                if (layout.DayCellRects == null || layout.DayCellRects.Count != 42)
                {
                    return;
                }

                cells = new Rectangle[6, 7];
                for (int i = 0; i < layout.DayCellRects.Count; i++)
                {
                    cells[i / 7, i % 7] = layout.DayCellRects[i];
                }
                layout.DayCellMatrix = cells;
            }

            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);

            int firstDayOfWeek = (int)properties.FirstDayOfWeek;
            int dayOffset = ((int)firstDayOfMonth.DayOfWeek - firstDayOfWeek + 7) % 7;

            var prevMonth = displayMonth.AddMonths(-1);
            var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            int row = 0;
            int col = 0;

            // Paint previous month days
            for (int i = 0; i < dayOffset; i++)
            {
                int day = daysInPrevMonth - dayOffset + i + 1;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, day);
                var cellRect = cells[row, col];
                bool isInRange = IsInRange(date, _owner.RangeStartDate, _owner.RangeEndDate);
                PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                col++;
                if (col >= 7)
                {
                    col = 0;
                    row++;
                }
            }

            // Paint current month days
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var cellRect = cells[row, col];

                bool isStartDate = _owner.RangeStartDate.HasValue && _owner.RangeStartDate.Value.Date == date.Date;
                bool isEndDate = _owner.RangeEndDate.HasValue && _owner.RangeEndDate.Value.Date == date.Date;
                bool isStartOrEnd = isStartDate || isEndDate;
                bool isToday = date.Date == DateTime.Today;
                bool isDisabled = date < properties.MinDate || date > properties.MaxDate;
                bool isHovered = hoverState.IsDateHovered(date);
                bool isPressed = hoverState.IsDatePressed(date);
                bool isInRange = IsInRange(date, _owner.RangeStartDate, _owner.RangeEndDate);

                PaintDayCell(g, cellRect, date, isStartOrEnd, isToday, isDisabled, isHovered, isPressed, isInRange, isStartDate, isEndDate);

                col++;
                if (col >= 7)
                {
                    col = 0;
                    row++;
                }
            }

            // Paint next month days
            var nextMonth = displayMonth.AddMonths(1);
            while (row < 6)
            {
                for (int day = 1; col < 7; day++)
                {
                    var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
                    var cellRect = cells[row, col];
                    bool isInRange = IsInRange(date, _owner.RangeStartDate, _owner.RangeEndDate);
                    PaintDayCell(g, cellRect, date, false, false, true, false, false, isInRange);
                    col++;
                }

                col = 0;
                row++;
            }
        }

        private bool IsInRange(DateTime date, DateTime? start, DateTime? end)
        {
            if (!start.HasValue || !end.HasValue) return false;
            return date >= start.Value && date <= end.Value;
        }

        // Stub implementations
        public override void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState) { }
        public override void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed) { }
        public override void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public override void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed) { }
        public override void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public override void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }
        public override void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public override void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public override DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            // Treat bounds as content area from container
            int padding = 10;
            int currentY = bounds.Y + padding;

            // Ensure minimum bounds
            int minWidth = Math.Max(bounds.Width, 300);
            int effectiveWidth = bounds.Width > 0 ? bounds.Width : minWidth;

            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, effectiveWidth - padding * 2 - 72, 40);
            
            int navButtonSize = 28;
            layout.PreviousButtonRect = new Rectangle(bounds.X + effectiveWidth - padding - navButtonSize * 2 - 8, currentY + 4, navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.X + effectiveWidth - padding - navButtonSize, currentY + 4, navButtonSize, navButtonSize);

            currentY += 40;

            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, effectiveWidth - padding * 2, 24);
            currentY += 28;

            int gridWidth = effectiveWidth - padding * 2;
            int availableHeight = bounds.Bottom - currentY - padding - 26; // leave room for range info
            int gridHeight = Math.Max(132, Math.Min(availableHeight, 252));
            layout.CalendarGridRect = new Rectangle(bounds.X + padding, currentY, gridWidth, gridHeight);

            // Calculate cell dimensions with minimum size guarantee
            layout.CellWidth = Math.Max(gridWidth / 7, 30);
            layout.CellHeight = Math.Max(gridHeight / 6, 30);

            // Create both matrix and list for compatibility
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

        public override Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Padding(10*2=20) + Header(40) + DayNames(28) + Grid(200) + RangeInfo(30) = 318px
            return new Size(350, 330);
        }

        public override Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Padding(10*2=20) + Header(40) + DayNames(28) + MinGrid(132) + RangeInfo(26) = 246px
            int padding = 10;
            int headerHeight = 40;
            int dayNamesHeight = 28;
            int minGridHeight = 132;
            int rangeInfoHeight = 26;
            int minHeight = padding * 2 + headerHeight + dayNamesHeight + minGridHeight + rangeInfoHeight;
            
            int minWidth = 7 * 35 + padding * 2; // 7 cells * 35px + padding = 265px
            
            return new Size(Math.Max(minWidth, 300), Math.Max(minHeight, 260));
        }

    }
}
