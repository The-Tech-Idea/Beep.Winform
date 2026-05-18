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
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            // Header
            using (var brush = new SolidBrush(headerBackColor))
                g.FillRectangle(brush, new Rectangle(grid.X, grid.Y, timeColumnWidth, dayHeaderHeight));
            using (var brush = new SolidBrush(headerBackColor))
                g.FillRectangle(brush, new Rectangle(grid.X + timeColumnWidth, grid.Y, Math.Max(0, grid.Width - timeColumnWidth), dayHeaderHeight));
            using (var brush = new SolidBrush(headerForeColor))
                g.DrawString(ctx.State.CurrentDate.ToString("dddd, MMMM dd"), ctx.HeaderFont, brush, new Rectangle(grid.X + timeColumnWidth, grid.Y, Math.Max(0, grid.Width - timeColumnWidth), dayHeaderHeight), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            DrawTimeSlots(g, ctx);
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            int eventInsetX = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX, ctx.DensityScale);
            if (location.X <= grid.X + timeColumnWidth + eventInsetX || location.Y <= grid.Y + dayHeaderHeight) return;

            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            int minutes = CalendarLayoutGeometry.GetMinuteFromY(timedArea, location.Y);
            ctx.State.SelectedDate = ctx.State.CurrentDate.Date.AddMinutes(minutes);
        }

        private void DrawTimeSlots(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeSlotCount = 24;
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int eventInsetX = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX, ctx.DensityScale);
            int eventInsetY = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetY, ctx.DensityScale);
            int minEventHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.MinEventHitHeight, ctx.DensityScale);
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            var timeLabelColor = ctx.Theme?.CalendarDaysHeaderForColor ?? Color.Gray;
            var gridLineColor = ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224);
            var dayEvents = ctx.EventService.GetEventsForDate(ctx.State.CurrentDate);

            for (int hour = 0; hour < timeSlotCount; hour++)
            {
                var rowRect = CalendarLayoutGeometry.GetRowRect(timedArea, hour, timeSlotCount);
                var timeRect = new Rectangle(grid.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                using (var brush = new SolidBrush(timeLabelColor))
                    g.DrawString($"{hour:00}:00", ctx.TimeFont, brush, timeRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                using (var pen = new Pen(gridLineColor))
                    g.DrawLine(pen, grid.X + timeColumnWidth, rowRect.Y, grid.Right, rowRect.Y);
            }

            using (var pen = new Pen(gridLineColor))
            {
                g.DrawLine(pen, grid.X + timeColumnWidth, timedArea.Top, grid.X + timeColumnWidth, timedArea.Bottom);
                g.DrawLine(pen, grid.X + timeColumnWidth, timedArea.Bottom - 1, grid.Right, timedArea.Bottom - 1);
            }

            foreach (var evt in dayEvents.OrderBy(e => e.StartTime))
            {
                var eventRect = CalendarLayoutGeometry.GetTimedEventRect(timedArea, evt, ctx.State.CurrentDate.Date, eventInsetX, eventInsetY, minEventHeight);
                DrawEventBlock(g, ctx, evt, eventRect);
            }
        }

        private void DrawEventBlock(Graphics g, CalendarRenderContext ctx, CalendarEvent evt, Rectangle rect)
        {
            bool isSelected = ctx.State.SelectedEvent?.Id == evt.Id;
            bool isHovered = ctx.HoveredEventId == evt.Id;
            CommonDrawing.DrawEventCard(g, ctx, evt, rect, isSelected, isHovered);
        }
    }
}
