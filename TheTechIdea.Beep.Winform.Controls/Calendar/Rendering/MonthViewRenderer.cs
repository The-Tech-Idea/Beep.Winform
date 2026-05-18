using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class MonthViewRenderer : ICalendarViewRenderer
    {
        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            var firstDayOfMonth = new DateTime(ctx.State.CurrentDate.Year, ctx.State.CurrentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            var headerBand = new Rectangle(grid.X, grid.Y, grid.Width, Math.Min(dayHeaderHeight, grid.Height));
            var monthBody = new Rectangle(grid.X, headerBand.Bottom, grid.Width, Math.Max(0, grid.Bottom - headerBand.Bottom));

            // Day headers
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < 7; i++)
            {
                var headerRect = CalendarLayoutGeometry.GetColumnRect(headerBand, i, 7);
                using (var brush = new SolidBrush(ctx.Theme?.CalendarBackColor ?? ctx.Owner.BackColor))
                    g.FillRectangle(brush, headerRect);
                using (var brush = new SolidBrush(ctx.Theme?.CalendarDaysHeaderForColor ?? ctx.Owner.ForeColor))
                    g.DrawString(dayNames[i], ctx.DaysHeaderFont, brush, headerRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // Cells
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstDayOfCalendar.AddDays(week * 7 + day);
                    var rowRect = CalendarLayoutGeometry.GetRowRect(monthBody, week, 6);
                    var cellRect = CalendarLayoutGeometry.GetColumnRect(rowRect, day, 7);
                    DrawMonthCell(g, ctx, cellDate, cellRect);
                }
            }
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            if (location.X < grid.X || location.Y < grid.Y + dayHeaderHeight) return;

            var monthBody = new Rectangle(grid.X, grid.Y + dayHeaderHeight, grid.Width, Math.Max(0, grid.Height - dayHeaderHeight));

            int col = CalendarLayoutGeometry.GetColumnIndex(monthBody, location.X, 7);
            int row = CalendarLayoutGeometry.GetRowIndex(monthBody, location.Y, 6);
            if (col < 0 || col >= 7 || row < 0 || row >= 6) return;

            var firstDayOfMonth = new DateTime(ctx.State.CurrentDate.Year, ctx.State.CurrentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            var clickedDate = firstDayOfCalendar.AddDays(row * 7 + col);

            ctx.State.SelectedDate = clickedDate;

            // Check events click
            var dayEvents = ctx.EventService.GetEventsForDate(clickedDate);
            if (dayEvents.Any())
            {
                var rowRect = CalendarLayoutGeometry.GetRowRect(monthBody, row, 6);
                var cellRect = CalendarLayoutGeometry.GetColumnRect(rowRect, col, 7);
                int eventY = CommonDrawing.ScaleMetric(25, ctx.DensityScale);
                int eventHeight = Math.Min(CalendarLayoutMetrics.MinEventHitHeight, Math.Max(16, cellRect.Height / 4));
                int eventSpacing = CommonDrawing.ScaleMetric(2, ctx.DensityScale);
                int relativeY = location.Y - cellRect.Y;
                foreach (var evt in dayEvents.Take(3))
                {
                    if (relativeY >= eventY && relativeY <= eventY + eventHeight)
                    {
                        ctx.State.SelectedEvent = evt;
                        break;
                    }
                    eventY += eventHeight + eventSpacing;
                }
            }
        }

        private void DrawMonthCell(Graphics g, CalendarRenderContext ctx, DateTime cellDate, Rectangle cellRect)
        {
            var events = ctx.EventService.GetEventsForDate(cellDate);
            var state = CalendarViewStateHelper.BuildDayCellState(
                cellDate,
                ctx.State.CurrentDate,
                ctx.State.SelectedDate,
                ctx.HoveredDate,
                ctx.FocusedDate,
                ctx.IsKeyboardFocusVisible,
                events.Count,
                ctx.MaxEventsPerCell);

            Color bgColor = state.IsSelected ? ctx.Theme?.CalendarSelectedDateBackColor ?? ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244) :
                           state.IsToday ? ctx.Theme?.CalendarHoverBackColor ?? Color.FromArgb(230, 240, 255) :
                           state.IsCurrentMonth ? ctx.Theme?.CalendarBackColor ?? Color.White :
                           Color.FromArgb(248, 249, 250);

            using (var brush = new SolidBrush(bgColor))
                g.FillRectangle(brush, cellRect);

            using (var pen = new Pen(ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
                g.DrawRectangle(pen, cellRect);

            Color textColor = state.IsSelected ? ctx.Theme?.CalendarSelectedDateForColor ?? Color.White :
                             state.IsToday ? ctx.Theme?.CalendarTodayForeColor ?? Color.FromArgb(244, 67, 54) :
                             state.IsCurrentMonth ? ctx.Theme?.CalendarForeColor ?? Color.Black : Color.Gray;

            using (var brush = new SolidBrush(textColor))
            {
                var dayRect = new Rectangle(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, 20);
                g.DrawString(cellDate.Day.ToString(), ctx.DayFont, brush, dayRect);
            }

            if (state.IsFocused)
            {
                using (var focusPen = new Pen(ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244), 2f))
                {
                    var focusRect = Rectangle.Inflate(cellRect, -2, -2);
                    g.DrawRectangle(focusPen, focusRect);
                }
            }

            // Events
            int eventStartOffset = CommonDrawing.ScaleMetric(25, ctx.DensityScale);
            int eventY = cellRect.Y + eventStartOffset;
            int eventHeight = Math.Min(CalendarLayoutMetrics.MinEventHitHeight, Math.Max(16, cellRect.Height / 4));
            int eventSpacing = CommonDrawing.ScaleMetric(2, ctx.DensityScale);
            foreach (var evt in events.Take(ctx.MaxEventsPerCell))
            {
                var rect = new Rectangle(cellRect.X + 2, eventY, cellRect.Width - 4, eventHeight);
                var color = CommonDrawing.GetCategoryColor(ctx, evt);
                bool isHovered = ctx.HoveredEventId == evt.Id;
                using (var brush = new SolidBrush(color))
                using (var path = CommonDrawing.RoundedRect(rect, 3))
                    g.FillPath(brush, path);

                if (isHovered)
                {
                    using (var pen = new Pen(CommonDrawing.GetContrastingTextColor(color), 1.5f))
                    using (var path = CommonDrawing.RoundedRect(rect, 3))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Use luminance-aware text color for event labels
                Color eventText = CommonDrawing.GetContrastingTextColor(color);
                using (var brush = new SolidBrush(eventText))
                    g.DrawString(evt.Title, ctx.EventFont, brush, rect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                eventY += eventHeight + eventSpacing;
            }

            if (state.HasMoreEvents)
            {
                using (var brush = new SolidBrush(ctx.Theme?.CalendarForeColor ?? Color.Gray))
                    g.DrawString($"+{events.Count - ctx.MaxEventsPerCell} more", ctx.EventFont, brush, new PointF(cellRect.X + 2, eventY));
            }
        }

    }
}
