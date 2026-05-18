using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawDayViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            int eventInsetX = ScaleMetric(CalendarLayoutMetrics.EventInsetX);
            int eventInsetY = ScaleMetric(CalendarLayoutMetrics.EventInsetY);

            // Day header
            using (var cornerBrush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(cornerBrush, new Rectangle(grid.X, grid.Y, timeColumnWidth, dayHeaderHeight));
            }

            var headerRect = new Rectangle(grid.X + timeColumnWidth, grid.Y, Math.Max(0, grid.Width - timeColumnWidth), dayHeaderHeight);
            _stylePainter.PaintWeekDayHeader(g, headerRect, _state.CurrentDate, _state.CurrentDate.Date == DateTime.Today, ctx);

            // Time slots
            var timedArea = CalendarLayoutGeometry.GetTimedArea(grid, timeColumnWidth, dayHeaderHeight);
            int currentHour = DateTime.Now.Hour;
            var dayEvents = _eventService.GetEventsForDate(_state.CurrentDate)
                .OrderBy(e => e.StartTime)
                .ToList();

            for (int hour = 0; hour < 24; hour++)
            {
                var rowRect = CalendarLayoutGeometry.GetRowRect(timedArea, hour, 24);

                // Time label
                var timeLabelRect = new Rectangle(grid.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                _stylePainter.PaintTimeLabel(g, timeLabelRect, hour, ctx);

                // Time slot
                var slotRect = new Rectangle(
                    grid.X + timeColumnWidth,
                    rowRect.Y,
                    grid.Width - timeColumnWidth,
                    rowRect.Height
                );
                _stylePainter.PaintTimeSlot(g, slotRect, hour, hour == currentHour && _state.CurrentDate.Date == DateTime.Today, ctx);
            }

            int minEventHeight = ScaleMetric(CalendarLayoutMetrics.MinEventHitHeight);
            foreach (var evt in dayEvents)
            {
                var eventRect = CalendarLayoutGeometry.GetTimedEventRect(timedArea, evt, _state.CurrentDate.Date, eventInsetX, eventInsetY, minEventHeight);
                bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                bool isHovered = _hoveredEvent?.Id == evt.Id;
                _stylePainter.PaintEventBlock(g, eventRect, evt, isSelected, isHovered, ctx);
            }
        }
    }
}
