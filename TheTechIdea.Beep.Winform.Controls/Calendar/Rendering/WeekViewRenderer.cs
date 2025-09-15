using System;
using System.Drawing;
using System.Linq;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class WeekViewRenderer : ICalendarViewRenderer
    {
        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            var startOfWeek = ctx.State.CurrentDate.AddDays(-(int)ctx.State.CurrentDate.DayOfWeek);
            int cellWidth = grid.Width / 7;

            // Day headers
            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = new Rectangle(grid.X + day * cellWidth, grid.Y, cellWidth, CalendarLayoutMetrics.DayHeaderHeight);
                bool isToday = dayDate.Date == DateTime.Today;
                using (var brush = new SolidBrush(isToday ? Color.FromArgb(66, 133, 244) : Color.FromArgb(248, 249, 250)))
                    g.FillRectangle(brush, headerRect);
                using (var brush = new SolidBrush(isToday ? Color.White : Color.Black))
                    g.DrawString($"{dayDate:ddd}\n{dayDate:dd}", ctx.DayFont, brush, headerRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            DrawTimeSlots(g, ctx, startOfWeek, cellWidth);
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            if (location.X <= grid.X + 60 || location.Y <= grid.Y + CalendarLayoutMetrics.DayHeaderHeight) return;

            int slotHeight = Math.Max(CalendarLayoutMetrics.TimeSlotHeight, (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / 24);
            int hour = (location.Y - grid.Y - CalendarLayoutMetrics.DayHeaderHeight) / slotHeight;

            var startOfWeek = ctx.State.CurrentDate.AddDays(-(int)ctx.State.CurrentDate.DayOfWeek);
            int cellWidth = (grid.Width - 60) / 7;
            int day = (location.X - grid.X - 60) / cellWidth;
            ctx.State.SelectedDate = startOfWeek.AddDays(day).AddHours(hour);
        }

        private void DrawTimeSlots(Graphics g, CalendarRenderContext ctx, DateTime startOfWeek, int cellWidth)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeSlotCount = 24;
            int slotHeight = Math.Max(CalendarLayoutMetrics.TimeSlotHeight, (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / timeSlotCount);

            for (int hour = 0; hour < timeSlotCount; hour++)
            {
                int yPos = grid.Y + CalendarLayoutMetrics.DayHeaderHeight + hour * slotHeight;
                var timeRect = new Rectangle(grid.X, yPos, 60, slotHeight);
                using (var brush = new SolidBrush(Color.Gray))
                    g.DrawString($"{hour:00}:00", ctx.TimeFont, brush, timeRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                using (var pen = new Pen(Color.FromArgb(218, 220, 224)))
                    g.DrawLine(pen, grid.X + 60, yPos, grid.Right, yPos);

                for (int day = 0; day < 7; day++)
                {
                    var dayDate = startOfWeek.AddDays(day);
                    var dayEvents = ctx.EventService.GetEventsForDate(dayDate).Where(e => e.StartTime.Hour == hour).ToList();
                    foreach (var evt in dayEvents)
                    {
                        var eventRect = new Rectangle(grid.X + 60 + day * cellWidth, yPos, cellWidth, (int)(evt.Duration.TotalHours * slotHeight));
                        DrawEventBlock(g, ctx, evt, eventRect);
                    }
                }
            }
        }

        private void DrawEventBlock(Graphics g, CalendarRenderContext ctx, CalendarEvent evt, Rectangle rect)
        {
            var color = CommonDrawing.GetCategoryColor(ctx, evt);
            using (var brush = new SolidBrush(Color.FromArgb(200, color)))
            using (var path = CommonDrawing.RoundedRect(rect, 5))
                g.FillPath(brush, path);
            using (var pen = new Pen(color, 2))
            using (var path = CommonDrawing.RoundedRect(rect, 5))
                g.DrawPath(pen, path);
            using (var brush = new SolidBrush(Color.White))
            {
                var textRect = Rectangle.Inflate(rect, -5, -2);
                g.DrawString($"{evt.Title}\n{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}", ctx.EventFont, brush, textRect);
            }
        }
    }
}
