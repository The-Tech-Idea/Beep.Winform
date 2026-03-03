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
            var headerBackColor = ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250);
            var headerForeColor = ctx.Theme?.CalendarTitleForColor ?? Color.Black;
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            // Header
            using (var brush = new SolidBrush(headerBackColor))
                g.FillRectangle(brush, new Rectangle(grid.X, grid.Y, grid.Width, dayHeaderHeight));
            using (var brush = new SolidBrush(headerForeColor))
                g.DrawString(ctx.State.CurrentDate.ToString("dddd, MMMM dd"), ctx.HeaderFont, brush, new Rectangle(grid.X, grid.Y, grid.Width, dayHeaderHeight), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            DrawTimeSlots(g, ctx);
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            int eventInsetX = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX, ctx.DensityScale);
            if (location.X <= grid.X + timeColumnWidth + eventInsetX || location.Y <= grid.Y + dayHeaderHeight) return;

            int slotHeight = Math.Max(CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight, ctx.DensityScale), (grid.Height - dayHeaderHeight) / 24);
            int hour = (location.Y - grid.Y - dayHeaderHeight) / slotHeight;
            ctx.State.SelectedDate = ctx.State.CurrentDate.Date.AddHours(hour);
        }

        private void DrawTimeSlots(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeSlotCount = 24;
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int eventInsetX = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX, ctx.DensityScale);
            int eventInsetY = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetY, ctx.DensityScale);
            int slotHeight = Math.Max(CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight, ctx.DensityScale), (grid.Height - dayHeaderHeight) / timeSlotCount);
            var timeLabelColor = ctx.Theme?.CalendarDaysHeaderForColor ?? Color.Gray;
            var gridLineColor = ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224);
            var dayEvents = ctx.EventService.GetEventsForDate(ctx.State.CurrentDate);

            for (int hour = 0; hour < timeSlotCount; hour++)
            {
                int yPos = grid.Y + dayHeaderHeight + hour * slotHeight;
                var timeRect = new Rectangle(grid.X, yPos, timeColumnWidth, slotHeight);
                using (var brush = new SolidBrush(timeLabelColor))
                    g.DrawString($"{hour:00}:00", ctx.TimeFont, brush, timeRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                using (var pen = new Pen(gridLineColor))
                    g.DrawLine(pen, grid.X + timeColumnWidth, yPos, grid.Right, yPos);

                foreach (var evt in dayEvents.Where(e => e.StartTime.Hour == hour))
                {
                    var eventRect = new Rectangle(
                        grid.X + timeColumnWidth + eventInsetX,
                        yPos + eventInsetY,
                        Math.Max(20, grid.Width - timeColumnWidth - (eventInsetX * 2)),
                        Math.Max(CalendarLayoutMetrics.MinEventHitHeight, (int)(evt.Duration.TotalHours * slotHeight) - (eventInsetY * 2)));
                    DrawEventBlock(g, ctx, evt, eventRect);
                }
            }
        }

        private void DrawEventBlock(Graphics g, CalendarRenderContext ctx, CalendarEvent evt, Rectangle rect)
        {
            bool isSelected = ctx.State.SelectedEvent?.Id == evt.Id;
            CommonDrawing.DrawEventCard(g, ctx, evt, rect, isSelected);
        }
    }
}
