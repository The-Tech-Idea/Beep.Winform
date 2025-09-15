using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class DayViewRenderer : ICalendarViewRenderer
    {
        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            // Header
            using (var brush = new SolidBrush(Color.FromArgb(248, 249, 250)))
                g.FillRectangle(brush, new Rectangle(grid.X, grid.Y, grid.Width, CalendarLayoutMetrics.DayHeaderHeight));
            using (var brush = new SolidBrush(Color.Black))
                g.DrawString(ctx.State.CurrentDate.ToString("dddd, MMMM dd"), ctx.HeaderFont, brush, new Rectangle(grid.X, grid.Y, grid.Width, CalendarLayoutMetrics.DayHeaderHeight), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            DrawTimeSlots(g, ctx);
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            if (location.X <= grid.X + 80 || location.Y <= grid.Y + CalendarLayoutMetrics.DayHeaderHeight) return;

            int slotHeight = Math.Max(CalendarLayoutMetrics.TimeSlotHeight, (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / 24);
            int hour = (location.Y - grid.Y - CalendarLayoutMetrics.DayHeaderHeight) / slotHeight;
            ctx.State.SelectedDate = ctx.State.CurrentDate.Date.AddHours(hour);
        }

        private void DrawTimeSlots(Graphics g, CalendarRenderContext ctx)
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

                var dayEvents = ctx.EventService.GetEventsForDate(ctx.State.CurrentDate).Where(e => e.StartTime.Hour == hour).ToList();
                foreach (var evt in dayEvents)
                {
                    var eventRect = new Rectangle(grid.X + 80, yPos, grid.Width - 100, (int)(evt.Duration.TotalHours * slotHeight));
                    DrawEventBlock(g, ctx, evt, eventRect);
                }
            }
        }

        private void DrawEventBlock(Graphics g, CalendarRenderContext ctx, CalendarEvent evt, Rectangle rect)
        {
            var color = CommonDrawing.GetCategoryColor(ctx, evt);
            using (var brush = new SolidBrush(Color.FromArgb(200, color)))
                g.FillRectangle(brush, rect);
            using (var pen = new Pen(color, 2))
                g.DrawRectangle(pen, rect);
            using (var brush = new SolidBrush(Color.White))
            {
                var textRect = Rectangle.Inflate(rect, -5, -2);
                g.DrawString($"{evt.Title}\n{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}", ctx.EventFont, brush, textRect);
            }
        }
    }
}
