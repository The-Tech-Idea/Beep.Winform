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
            int cellWidth = Math.Max(1, (grid.Width - timeColumnWidth) / 5);
            var headerBackColor = ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250);
            var headerForeColor = ctx.Theme?.CalendarForeColor ?? Color.Black;
            var primaryColor = ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244);

            for (int day = 0; day < 5; day++)
            {
                var dayDate = startOfWorkWeek.AddDays(day);
                var headerRect = new Rectangle(
                    grid.X + timeColumnWidth + day * cellWidth,
                    grid.Y,
                    cellWidth,
                    dayHeaderHeight);
                bool isToday = dayDate.Date == DateTime.Today;
                using (var brush = new SolidBrush(isToday ? primaryColor : headerBackColor))
                    g.FillRectangle(brush, headerRect);
                using (var brush = new SolidBrush(isToday ? Color.White : headerForeColor))
                    g.DrawString($"{dayDate:ddd}\n{dayDate:dd}", ctx.DayFont, brush, headerRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            DrawTimeSlots(g, ctx, startOfWorkWeek, cellWidth);
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            if (location.X <= grid.X + timeColumnWidth || location.Y <= grid.Y + dayHeaderHeight) return;

            int slotHeight = Math.Max(
                CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight, ctx.DensityScale),
                (grid.Height - dayHeaderHeight) / 24);
            int hour = (location.Y - grid.Y - dayHeaderHeight) / slotHeight;

            var startOfWorkWeek = GetStartOfWorkWeek(ctx.State.CurrentDate);
            int cellWidth = Math.Max(1, (grid.Width - timeColumnWidth) / 5);
            int day = Math.Max(0, Math.Min(4, (location.X - grid.X - timeColumnWidth) / cellWidth));
            ctx.State.SelectedDate = startOfWorkWeek.AddDays(day).AddHours(hour);
        }

        private void DrawTimeSlots(Graphics g, CalendarRenderContext ctx, DateTime startOfWorkWeek, int cellWidth)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int timeSlotCount = 24;
            int timeColumnWidth = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            int eventInsetX = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX, ctx.DensityScale);
            int eventInsetY = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetY, ctx.DensityScale);
            int slotHeight = Math.Max(
                CommonDrawing.ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight, ctx.DensityScale),
                (grid.Height - dayHeaderHeight) / timeSlotCount);
            var timeLabelColor = ctx.Theme?.CalendarDaysHeaderForColor ?? Color.Gray;
            var gridLineColor = ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224);
            var dayEvents = Enumerable.Range(0, 5)
                .Select(index => startOfWorkWeek.AddDays(index).Date)
                .ToDictionary(day => day, day => ctx.EventService.GetEventsForDate(day));

            for (int hour = 0; hour < timeSlotCount; hour++)
            {
                int yPos = grid.Y + dayHeaderHeight + hour * slotHeight;
                var timeRect = new Rectangle(grid.X, yPos, timeColumnWidth, slotHeight);
                using (var brush = new SolidBrush(timeLabelColor))
                    g.DrawString($"{hour:00}:00", ctx.TimeFont, brush, timeRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                using (var pen = new Pen(gridLineColor))
                    g.DrawLine(pen, grid.X + timeColumnWidth, yPos, grid.Right, yPos);

                for (int day = 0; day < 5; day++)
                {
                    var dayDate = startOfWorkWeek.AddDays(day).Date;
                    foreach (var evt in dayEvents[dayDate].Where(e => e.StartTime.Hour == hour))
                    {
                        var eventRect = new Rectangle(
                            grid.X + timeColumnWidth + day * cellWidth + eventInsetX,
                            yPos + eventInsetY,
                            Math.Max(20, cellWidth - (eventInsetX * 2)),
                            Math.Max(
                                CalendarLayoutMetrics.MinEventHitHeight,
                                (int)(evt.Duration.TotalHours * slotHeight) - (eventInsetY * 2)));
                        bool isSelected = ctx.State.SelectedEvent?.Id == evt.Id;
                        CommonDrawing.DrawEventCard(g, ctx, evt, eventRect, isSelected);
                    }
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