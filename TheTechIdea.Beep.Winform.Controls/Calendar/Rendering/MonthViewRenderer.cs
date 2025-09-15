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

            int cellWidth = grid.Width / 7;
            int cellHeight = (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / 6;

            // Day headers
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < 7; i++)
            {
                var headerRect = new Rectangle(grid.X + i * cellWidth, grid.Y, cellWidth, CalendarLayoutMetrics.DayHeaderHeight);
                using (var brush = new SolidBrush(ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250)))
                    g.FillRectangle(brush, headerRect);
                using (var brush = new SolidBrush(ctx.Theme?.CalendarDaysHeaderForColor ?? Color.FromArgb(95, 99, 104)))
                    g.DrawString(dayNames[i], ctx.DaysHeaderFont, brush, headerRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // Cells
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstDayOfCalendar.AddDays(week * 7 + day);
                    var cellRect = new Rectangle(grid.X + day * cellWidth, grid.Y + CalendarLayoutMetrics.DayHeaderHeight + week * cellHeight, cellWidth, cellHeight);
                    DrawMonthCell(g, ctx, cellDate, cellRect);
                }
            }
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            if (location.X < grid.X || location.Y < grid.Y + CalendarLayoutMetrics.DayHeaderHeight) return;

            int cellWidth = grid.Width / 7;
            int cellHeight = (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / 6;

            int col = (location.X - grid.X) / cellWidth;
            int row = (location.Y - grid.Y - CalendarLayoutMetrics.DayHeaderHeight) / cellHeight;
            if (col < 0 || col >= 7 || row < 0 || row >= 6) return;

            var firstDayOfMonth = new DateTime(ctx.State.CurrentDate.Year, ctx.State.CurrentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            var clickedDate = firstDayOfCalendar.AddDays(row * 7 + col);

            ctx.State.SelectedDate = clickedDate;

            // Check events click
            var dayEvents = ctx.EventService.GetEventsForDate(clickedDate);
            if (dayEvents.Any())
            {
                int eventY = 25;
                int relativeY = location.Y - (grid.Y + CalendarLayoutMetrics.DayHeaderHeight + row * cellHeight);
                foreach (var evt in dayEvents.Take(3))
                {
                    if (relativeY >= eventY && relativeY <= eventY + 16)
                    {
                        ctx.State.SelectedEvent = evt;
                        break;
                    }
                    eventY += 18;
                }
            }
        }

        private void DrawMonthCell(Graphics g, CalendarRenderContext ctx, DateTime cellDate, Rectangle cellRect)
        {
            bool isCurrentMonth = cellDate.Month == ctx.State.CurrentDate.Month;
            bool isToday = cellDate.Date == DateTime.Today;
            bool isSelected = cellDate.Date == ctx.State.SelectedDate.Date;

            Color bgColor = isSelected ? ctx.Theme?.CalendarSelectedDateBackColor ?? Color.FromArgb(66, 133, 244) :
                           isToday ? ctx.Theme?.CalendarHoverBackColor ?? Color.FromArgb(230, 240, 255) :
                           isCurrentMonth ? ctx.Theme?.CalendarBackColor ?? Color.White :
                           Color.FromArgb(248, 249, 250);

            using (var brush = new SolidBrush(bgColor))
                g.FillRectangle(brush, cellRect);

            using (var pen = new Pen(ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
                g.DrawRectangle(pen, cellRect);

            Color textColor = isSelected ? ctx.Theme?.CalendarSelectedDateForColor ?? Color.White :
                             isToday ? ctx.Theme?.CalendarTodayForeColor ?? Color.FromArgb(244, 67, 54) :
                             isCurrentMonth ? ctx.Theme?.CalendarForeColor ?? Color.Black : Color.Gray;

            using (var brush = new SolidBrush(textColor))
            {
                var dayRect = new Rectangle(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, 20);
                g.DrawString(cellDate.Day.ToString(), ctx.DayFont, brush, dayRect);
            }

            // Events
            var events = ctx.EventService.GetEventsForDate(cellDate);
            int eventY = cellRect.Y + 25;
            foreach (var evt in events.Take(3))
            {
                var rect = new Rectangle(cellRect.X + 2, eventY, cellRect.Width - 4, 16);
                var color = CommonDrawing.GetCategoryColor(ctx, evt);
                using (var brush = new SolidBrush(color))
                using (var path = CommonDrawing.RoundedRect(rect, 3))
                    g.FillPath(brush, path);
                using (var brush = new SolidBrush(Color.White))
                    g.DrawString(evt.Title, ctx.EventFont, brush, rect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                eventY += 18;
            }

            if (events.Count > 3)
            {
                using (var brush = new SolidBrush(ctx.Theme?.CalendarForeColor ?? Color.Gray))
                    g.DrawString($"+{events.Count - 3} more", new Font(ctx.EventFont.FontFamily, 8), brush, new PointF(cellRect.X + 2, eventY));
            }
        }
    }
}
