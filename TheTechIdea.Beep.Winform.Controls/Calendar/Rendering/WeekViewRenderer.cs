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
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            var headerBand = new Rectangle(grid.X + timeColumnWidth, grid.Y, Math.Max(0, grid.Width - timeColumnWidth), Math.Min(dayHeaderHeight, grid.Height));
            var headerBackColor = ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250);
            var headerForeColor = ctx.Theme?.CalendarForeColor ?? Color.Black;
            var primaryColor = ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244);

            // Day headers
            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = CalendarLayoutGeometry.GetColumnRect(headerBand, day, 7);
                bool isToday = dayDate.Date == DateTime.Today;
                using (var brush = new SolidBrush(isToday ? primaryColor : headerBackColor))
                    g.FillRectangle(brush, headerRect);
                using (var brush = new SolidBrush(isToday ? Color.White : headerForeColor))
                    g.DrawString($"{dayDate:ddd}\n{dayDate:dd}", ctx.DayFont, brush, headerRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            DrawTimeSlots(g, ctx, startOfWeek);
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            if (location.X <= grid.X + timeColumnWidth || location.Y <= grid.Y + dayHeaderHeight) return;

            var startOfWeek = ctx.State.CurrentDate.AddDays(-(int)ctx.State.CurrentDate.DayOfWeek);
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            int day = CalendarLayoutGeometry.GetColumnIndex(timedArea, location.X, 7);
            int minutes = CalendarLayoutGeometry.GetMinuteFromY(timedArea, location.Y);
            if (day < 0)
            {
                return;
            }

            ctx.State.SelectedDate = startOfWeek.AddDays(day).AddMinutes(minutes);
        }

        private void DrawTimeSlots(Graphics g, CalendarRenderContext ctx, DateTime startOfWeek)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeSlotCount = 24;
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            int eventInsetX = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX, ctx.DensityScale);
            int eventInsetY = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetY, ctx.DensityScale);
            int minEventHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.MinEventHitHeight, ctx.DensityScale);
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            var timeLabelColor = ctx.Theme?.CalendarDaysHeaderForColor ?? Color.Gray;
            var gridLineColor = ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224);
            var dayEvents = Enumerable.Range(0, 7)
                .Select(index => startOfWeek.AddDays(index).Date)
                .ToDictionary(day => day, day => ctx.EventService.GetEventsForDate(day));

            for (int hour = 0; hour < timeSlotCount; hour++)
            {
                var rowRect = CalendarLayoutGeometry.GetRowRect(timedArea, hour, timeSlotCount);
                var timeRect = new Rectangle(grid.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                using (var brush = new SolidBrush(timeLabelColor))
                    g.DrawString($"{hour:00}:00", ctx.TimeFont, brush, timeRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                using (var pen = new Pen(gridLineColor))
                    g.DrawLine(pen, grid.X + timeColumnWidth, rowRect.Y, grid.Right, rowRect.Y);

                for (int day = 0; day < 7; day++)
                {
                    var columnRect = CalendarLayoutGeometry.GetColumnRect(timedArea, day, 7);
                    using (var pen = new Pen(gridLineColor))
                        g.DrawLine(pen, columnRect.Left, timedArea.Top, columnRect.Left, timedArea.Bottom);
                }
            }

            using (var pen = new Pen(gridLineColor))
                g.DrawLine(pen, grid.X + timeColumnWidth, timedArea.Bottom - 1, grid.Right, timedArea.Bottom - 1);

            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day).Date;
                var dayColumn = CalendarLayoutGeometry.GetColumnRect(timedArea, day, 7);
                foreach (var evt in dayEvents[dayDate].OrderBy(e => e.StartTime))
                {
                    var eventRect = CalendarLayoutGeometry.GetTimedEventRect(dayColumn, evt, dayDate, eventInsetX, eventInsetY, minEventHeight);
                    DrawEventBlock(g, ctx, evt, eventRect);
                }
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
