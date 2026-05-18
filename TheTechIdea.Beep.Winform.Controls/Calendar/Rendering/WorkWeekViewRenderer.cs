using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class WorkWeekViewRenderer : ICalendarViewRenderer
    {
        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            var startOfWorkWeek = GetStartOfWorkWeek(ctx.State.CurrentDate);
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            var headerBand = new Rectangle(grid.X + timeColumnWidth, grid.Y, Math.Max(0, grid.Width - timeColumnWidth), Math.Min(dayHeaderHeight, grid.Height));
            var headerBackColor = ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250);
            var headerForeColor = ctx.Theme?.CalendarForeColor ?? Color.Black;
            var primaryColor = ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244);

            for (int day = 0; day < 5; day++)
            {
                var dayDate = startOfWorkWeek.AddDays(day);
                var headerRect = CalendarLayoutGeometry.GetColumnRect(headerBand, day, 5);
                bool isToday = dayDate.Date == DateTime.Today;
                using (var brush = new SolidBrush(isToday ? primaryColor : headerBackColor))
                    g.FillRectangle(brush, headerRect);
                using (var brush = new SolidBrush(isToday ? Color.White : headerForeColor))
                    g.DrawString($"{dayDate:ddd}\n{dayDate:dd}", ctx.DayFont, brush, headerRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            DrawTimeSlots(g, ctx, startOfWorkWeek);
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            if (location.X <= grid.X + timeColumnWidth || location.Y <= grid.Y + dayHeaderHeight) return;

            var startOfWorkWeek = GetStartOfWorkWeek(ctx.State.CurrentDate);
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            int day = CalendarLayoutGeometry.GetColumnIndex(timedArea, location.X, 5);
            int minutes = CalendarLayoutGeometry.GetMinuteFromY(timedArea, location.Y);
            if (day < 0)
            {
                return;
            }

            ctx.State.SelectedDate = startOfWorkWeek.AddDays(day).AddMinutes(minutes);
        }

        private void DrawTimeSlots(Graphics g, CalendarRenderContext ctx, DateTime startOfWorkWeek)
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
            var dayEvents = Enumerable.Range(0, 5)
                .Select(index => startOfWorkWeek.AddDays(index).Date)
                .ToDictionary(day => day, day => ctx.EventService.GetEventsForDate(day));

            for (int hour = 0; hour < timeSlotCount; hour++)
            {
                var rowRect = CalendarLayoutGeometry.GetRowRect(timedArea, hour, timeSlotCount);
                var timeRect = new Rectangle(grid.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                using (var brush = new SolidBrush(timeLabelColor))
                    g.DrawString($"{hour:00}:00", ctx.TimeFont, brush, timeRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                using (var pen = new Pen(gridLineColor))
                    g.DrawLine(pen, grid.X + timeColumnWidth, rowRect.Y, grid.Right, rowRect.Y);

                for (int day = 0; day < 5; day++)
                {
                    var columnRect = CalendarLayoutGeometry.GetColumnRect(timedArea, day, 5);
                    using (var pen = new Pen(gridLineColor))
                        g.DrawLine(pen, columnRect.Left, timedArea.Top, columnRect.Left, timedArea.Bottom);
                }
            }

            using (var pen = new Pen(gridLineColor))
                g.DrawLine(pen, grid.X + timeColumnWidth, timedArea.Bottom - 1, grid.Right, timedArea.Bottom - 1);

            for (int day = 0; day < 5; day++)
            {
                var dayDate = startOfWorkWeek.AddDays(day).Date;
                var dayColumn = CalendarLayoutGeometry.GetColumnRect(timedArea, day, 5);
                foreach (var evt in dayEvents[dayDate].OrderBy(e => e.StartTime))
                {
                    var eventRect = CalendarLayoutGeometry.GetTimedEventRect(dayColumn, evt, dayDate, eventInsetX, eventInsetY, minEventHeight);
                    bool isSelected = ctx.State.SelectedEvent?.Id == evt.Id;
                    bool isHovered = ctx.HoveredEventId == evt.Id;
                    CommonDrawing.DrawEventCard(g, ctx, evt, eventRect, isSelected, isHovered);
                }
            }
        }

        private static DateTime GetStartOfWorkWeek(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var mondayOffset = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            return date.Date.AddDays(-mondayOffset);
        }
    }
}
