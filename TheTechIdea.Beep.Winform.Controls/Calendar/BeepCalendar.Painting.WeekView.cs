using System;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawWeekViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var grid = _rects.CalendarGridRect;
            var startOfWeek = _state.CurrentDate.AddDays(-(int)_state.CurrentDate.DayOfWeek);
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            int cellWidth = (grid.Width - timeColumnWidth) / 7;

            // Day headers
            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = new Rectangle(
                    grid.X + timeColumnWidth + day * cellWidth,
                    grid.Y,
                    cellWidth,
                    dayHeaderHeight
                );
                _stylePainter.PaintWeekDayHeader(g, headerRect, dayDate, dayDate.Date == DateTime.Today, ctx);
            }

            // Time slots
            int slotHeight = Math.Max(ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight), (grid.Height - dayHeaderHeight) / 24);
            int currentHour = DateTime.Now.Hour;
            var eventsByDay = Enumerable.Range(0, 7)
                .Select(index => startOfWeek.AddDays(index).Date)
                .ToDictionary(day => day, day => _eventService.GetEventsForDate(day));

            for (int hour = 0; hour < 24; hour++)
            {
                int yPos = grid.Y + dayHeaderHeight + hour * slotHeight;

                // Time label
                var timeLabelRect = new Rectangle(grid.X, yPos, timeColumnWidth, slotHeight);
                _stylePainter.PaintTimeLabel(g, timeLabelRect, hour, ctx);

                // Time slot for each day
                for (int day = 0; day < 7; day++)
                {
                    var slotRect = new Rectangle(
                        grid.X + timeColumnWidth + day * cellWidth,
                        yPos,
                        cellWidth,
                        slotHeight
                    );
                    _stylePainter.PaintTimeSlot(g, slotRect, hour, hour == currentHour && startOfWeek.AddDays(day).Date == DateTime.Today, ctx);

                    var dayDate = startOfWeek.AddDays(day).Date;
                    var dayEvents = eventsByDay[dayDate]
                        .Where(e => e.StartTime.Hour == hour)
                        .ToList();

                    DrawWeekSlotEvents(g, slotRect, yPos, slotHeight, dayEvents, ScaleMetric(CalendarLayoutMetrics.EventInsetX), ScaleMetric(CalendarLayoutMetrics.EventInsetY), ctx);
                }
            }
        }
    }
}