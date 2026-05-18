using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawWeekViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var startOfWeek = _state.CurrentDate.Date.AddDays(-(int)_state.CurrentDate.DayOfWeek);
            DrawTimedWeekViewWithPainter(g, ctx, startOfWeek, 7);
        }

        private void DrawTimedWeekViewWithPainter(Graphics g, CalendarPainterContext ctx, DateTime startDate, int dayCount)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            var headerBand = new Rectangle(grid.X + timeColumnWidth, grid.Y, Math.Max(0, grid.Width - timeColumnWidth), Math.Min(dayHeaderHeight, grid.Height));
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);

            // Day headers
            using (var cornerBrush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(cornerBrush, new Rectangle(grid.X, grid.Y, timeColumnWidth, dayHeaderHeight));
            }

            for (int day = 0; day < dayCount; day++)
            {
                var dayDate = startDate.AddDays(day);
                var headerRect = CalendarLayoutGeometry.GetColumnRect(headerBand, day, dayCount);
                _stylePainter.PaintWeekDayHeader(g, headerRect, dayDate, dayDate.Date == DateTime.Today, ctx);
            }

            // Time slots
            int currentHour = DateTime.Now.Hour;
            var eventsByDay = Enumerable.Range(0, dayCount)
                .Select(index => startDate.AddDays(index).Date)
                .ToDictionary(day => day, day => _eventService.GetEventsForDate(day));

            for (int hour = 0; hour < 24; hour++)
            {
                var rowRect = CalendarLayoutGeometry.GetRowRect(timedArea, hour, 24);

                // Time label
                var timeLabelRect = new Rectangle(grid.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                _stylePainter.PaintTimeLabel(g, timeLabelRect, hour, ctx);

                // Time slot for each day
                for (int day = 0; day < dayCount; day++)
                {
                    var columnRect = CalendarLayoutGeometry.GetColumnRect(timedArea, day, dayCount);
                    var slotRect = new Rectangle(columnRect.X, rowRect.Y, columnRect.Width, rowRect.Height);
                    _stylePainter.PaintTimeSlot(g, slotRect, hour, hour == currentHour && startDate.AddDays(day).Date == DateTime.Today, ctx);
                }
            }

            int eventInsetX = ScaleMetric(CalendarLayoutMetrics.EventInsetX);
            int eventInsetY = ScaleMetric(CalendarLayoutMetrics.EventInsetY);
            int minEventHeight = ScaleMetric(CalendarLayoutMetrics.MinEventHitHeight);
            for (int day = 0; day < dayCount; day++)
            {
                var dayDate = startDate.AddDays(day).Date;
                var columnRect = CalendarLayoutGeometry.GetColumnRect(timedArea, day, dayCount);
                foreach (var evt in eventsByDay[dayDate].OrderBy(e => e.StartTime))
                {
                    var eventRect = CalendarLayoutGeometry.GetTimedEventRect(columnRect, evt, dayDate, eventInsetX, eventInsetY, minEventHeight);
                    bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                    bool isHovered = _hoveredEvent?.Id == evt.Id;
                    _stylePainter.PaintEventBlock(g, eventRect, evt, isSelected, isHovered, ctx);
                }
            }
        }
    }
}
