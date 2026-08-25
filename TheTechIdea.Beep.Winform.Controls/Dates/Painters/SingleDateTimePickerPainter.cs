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
    /// Single Date Selection Painter
    /// Standard calendar for selecting a single date
    /// Visual styling follows BeepTheme
    /// 
    /// Layout Structure:
    /// - Header with month/year title and navigation buttons
    /// - Day names row (Mo, Tu, We, etc.)
    /// - 7×6 calendar grid for day cells
    /// - Optional today button at bottom
    /// 
    /// Supported Hit Areas:
    /// - Header (title text area)
    /// - PreviousButton (navigate to previous month)
    /// - NextButton (navigate to next month)
    /// - DayCell (individual day selection)
    /// - TodayButton (quick select today, if ShowTodayButton is enabled)
    /// </summary>
    public class SingleDateTimePickerPainter : DateTimePickerPainterBase
    {
        public override DatePickerMode Mode => DatePickerMode.Single;

        public SingleDateTimePickerPainter(BeepDateTimePicker owner, IBeepTheme theme) : base(owner, theme) { }

        public override void PaintCalendar(Graphics g, Rectangle bounds, DateTimePickerProperties properties, DateTime displayMonth, DateTimePickerHoverState hoverState)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var layout = CalculateLayout(bounds, properties);

            // Paint background
            PaintBackground(g, bounds);

            // Paint header
            PaintHeader(g, layout.HeaderRect, displayMonth.ToString("MMMM yyyy"), true, false);

            // Paint navigation buttons
            PaintNavigationButton(g, layout.PreviousButtonRect, false, 
                hoverState?.IsAreaHovered(DateTimePickerHitArea.PreviousButton) == true,
                hoverState?.IsAreaPressed(DateTimePickerHitArea.PreviousButton) == true);
            
            PaintNavigationButton(g, layout.NextButtonRect, true,
                hoverState?.IsAreaHovered(DateTimePickerHitArea.NextButton) == true,
                hoverState?.IsAreaPressed(DateTimePickerHitArea.NextButton) == true);

            // Paint day names header
            PaintDayNamesHeader(g, layout.DayNamesRect, properties.FirstDayOfWeek);

            // Paint calendar grid
            PaintCalendarGrid(g, layout, displayMonth, properties, hoverState);

            // Paint today button (if enabled)
            if (properties.ShowTodayButton && !layout.TodayButtonRect.IsEmpty)
            {
                PaintTodayButton(g, layout.TodayButtonRect, hoverState, properties);
            }
        }

        private void PaintBackground(Graphics g, Rectangle bounds)
        {
            // Let BaseControl (Minimalist) handle outer background/border.
            // Only fill interior content area subtly when needed.
            var bgColor = _theme?.CalendarBackColor ?? _theme?.BackgroundColor ?? Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        private void PaintTodayButton(Graphics g, Rectangle bounds, DateTimePickerHoverState hoverState, DateTimePickerProperties properties)
        {
            var textColor = _theme?.CalendarForeColor ?? Color.Black;
            var accentColor = _theme?.CalendarTodayForeColor ?? Color.FromArgb(0, 120, 215);
            var hoverColor = _theme?.CalendarHoverBackColor ?? Color.FromArgb(245, 245, 245);
            var pressedColor = ShiftLuminance(accentColor, -0.1f);
            using (var font = ResolveFont(_theme?.CalendarUnSelectedFont, 9f, FontStyle.Regular))
            {
                bool isHovered = hoverState?.IsAreaHovered(DateTimePickerHitArea.TodayButton) == true;
                bool isPressed = hoverState?.IsAreaPressed(DateTimePickerHitArea.TodayButton) == true;

                // Draw hover/pressed background
                if (isPressed)
                {
                    using (var brush = new SolidBrush(pressedColor))
                    {
                        g.FillRectangle(brush, bounds);
                    }
                }
                else if (isHovered)
                {
                    using (var brush = new SolidBrush(hoverColor))
                    {
                        g.FillRectangle(brush, bounds);
                    }
                }

                // Draw button text
                string buttonText = properties?.TodayButtonText ?? "Today";
                using (var brush = new SolidBrush(accentColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(buttonText, font, brush, bounds, format);
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
                
                bool isDisabled = (properties.MinDate.HasValue && date < properties.MinDate.Value) ||
                                   (properties.MaxDate.HasValue && date > properties.MaxDate.Value);
                bool isSelected = !isDisabled &&
                    _owner.SelectedDate.HasValue && _owner.SelectedDate.Value.Date == date.Date;
                bool isToday = date.Date == DateTime.Today;
                bool isHovered = hoverState != null && hoverState.IsDateHovered(date);
                bool isPressed = hoverState != null && hoverState.IsDatePressed(date);
                bool isInRange = false;
                
                PaintDayCell(g, cellRect, date, isSelected, isToday, isDisabled, isHovered, isPressed, isInRange);
                
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
        public override void PaintTimePicker(Graphics g, Rectangle timePickerBounds, TimeSpan? selectedTime, List<TimeSpan> timeSlots, bool isEnabled, DateTimePickerHoverState hoverState) { }
        public override void PaintTimeSlot(Graphics g, Rectangle slotBounds, TimeSpan time, bool isSelected, bool isHovered, bool isPressed) { }
        public override void PaintQuickSelectionButtons(Graphics g, Rectangle buttonAreaBounds, DateTimePickerProperties properties, DateTimePickerHoverState hoverState) { }
        public override void PaintQuickButton(Graphics g, Rectangle buttonBounds, string text, bool isHovered, bool isPressed) { }
        public override void PaintActionButtons(Graphics g, Rectangle actionAreaBounds, bool showApply, bool showCancel, DateTimePickerHoverState hoverState) { }
        public override void PaintActionButton(Graphics g, Rectangle buttonBounds, string text, bool isPrimary, bool isHovered, bool isPressed) { }
        public override void PaintRangeSelection(Graphics g, Rectangle calendarBounds, DateTime? rangeStart, DateTime? rangeEnd, DateTime displayMonth) { }
        public override void PaintWeekNumbers(Graphics g, Rectangle weekColumnBounds, DateTime displayMonth, DatePickerFirstDayOfWeek firstDayOfWeek, DateTimePickerHoverState hoverState) { }
        public override void PaintWeekNumber(Graphics g, Rectangle weekBounds, int weekNumber, bool isHovered) { }

        public override DateTimePickerLayout CalculateLayout(Rectangle bounds, DateTimePickerProperties properties)
        {
            var layout = new DateTimePickerLayout();
            // Treat incoming bounds as content area from BaseControl painter; keep internal spacing minimal
            int padding = S(8);
            int currentY = bounds.Y + padding;

            // Header with navigation buttons
            int navButtonSize = S(28);
            int navGap = S(8);
            int headerHeight = S(40);
            int navButtonsWidth = navButtonSize * 2 + navGap; // Two buttons plus gap
            
            layout.HeaderRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, headerHeight);
            layout.TitleRect = new Rectangle(bounds.X + padding, currentY + S(4), bounds.Width - padding * 2 - navButtonsWidth, headerHeight - S(8));
            layout.PreviousButtonRect = new Rectangle(bounds.Right - padding - navButtonSize * 2 - navGap, currentY + S(4), navButtonSize, navButtonSize);
            layout.NextButtonRect = new Rectangle(bounds.Right - padding - navButtonSize, currentY + S(4), navButtonSize, navButtonSize);

            currentY += headerHeight;

            // Day names header
            int dayNamesHeight = S(28);
            layout.DayNamesRect = new Rectangle(bounds.X + padding, currentY, bounds.Width - padding * 2, dayNamesHeight);
            currentY += dayNamesHeight;

            // Calendar grid (7×6 cells)
            int gridWidth = bounds.Width - padding * 2;
            int availableHeight = bounds.Bottom - currentY - padding;
            
            // Reserve space for today button if enabled
            if (properties.ShowTodayButton)
            {
                availableHeight -= S(45); // Today button height + spacing
            }
            
            int gridHeight = Math.Max(S(120), Math.Min(availableHeight, S(252)));
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

            currentY += gridHeight;

            // Today button (optional)
            if (properties.ShowTodayButton)
            {
                currentY += S(10); // Spacing above button
                layout.TodayButtonRect = new Rectangle(bounds.X + padding, currentY, S(100), S(30));
            }

            // Register all hit areas with BaseControl's hit test system
            _owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);

            return layout;
        }

        public override Size GetPreferredSize(DateTimePickerProperties properties)
        {
            // Padding(8*2=16) + Header(40) + DayNames(28) + Grid(200) = 284px
            int width = S(350);
            int height = S(300); // Header + day names + 6 weeks of cells
            
            if (properties.ShowCustomQuickDates)
                height += S(50);
            
            return new Size(width, height);
        }

        public override Size GetMinimumSize(DateTimePickerProperties properties)
        {
            // Padding(8*2=16) + Header(40) + DayNames(28) + MinGrid(120) = 204px
            int padding = S(8);
            int headerHeight = S(40);
            int dayNamesHeight = S(28);
            int minGridHeight = S(120);
            int todayButtonHeight = properties.ShowTodayButton ? S(40) : 0;
            int minHeight = padding * 2 + headerHeight + dayNamesHeight + minGridHeight + todayButtonHeight;
            
            int minWidth = 7 * S(35) + padding * 2; // 7 cells * 35px + padding = 261px
            
            return new Size(Math.Max(minWidth, S(280)), Math.Max(minHeight, S(220)));
        }

    }
}
