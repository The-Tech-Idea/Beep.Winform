using System;
using System.Drawing;
using System.Linq;

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
            var headerRect = new Rectangle(grid.X, grid.Y, grid.Width, dayHeaderHeight);
            _stylePainter.PaintWeekDayHeader(g, headerRect, _state.CurrentDate, _state.CurrentDate.Date == DateTime.Today, ctx);

            // Time slots
            int slotHeight = Math.Max(ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight), (grid.Height - dayHeaderHeight) / 24);
            int currentHour = DateTime.Now.Hour;
            var eventsByHour = _eventService.GetEventsForDate(_state.CurrentDate)
                .GroupBy(e => e.StartTime.Hour)
                .ToDictionary(group => group.Key, group => group.ToList());

            for (int hour = 0; hour < 24; hour++)
            {
                int yPos = grid.Y + dayHeaderHeight + hour * slotHeight;

                // Time label
                var timeLabelRect = new Rectangle(grid.X, yPos, timeColumnWidth, slotHeight);
                _stylePainter.PaintTimeLabel(g, timeLabelRect, hour, ctx);

                // Time slot
                var slotRect = new Rectangle(
                    grid.X + timeColumnWidth,
                    yPos,
                    grid.Width - timeColumnWidth,
                    slotHeight
                );
                _stylePainter.PaintTimeSlot(g, slotRect, hour, hour == currentHour && _state.CurrentDate.Date == DateTime.Today, ctx);

                // Draw events
                if (!eventsByHour.TryGetValue(hour, out var dayEvents))
                {
                    continue;
                }

                foreach (var evt in dayEvents)
                {
                    var eventRect = new Rectangle(
                        slotRect.X + eventInsetX,
                        yPos + eventInsetY,
                        Math.Max(20, slotRect.Width - (eventInsetX * 2)),
                        Math.Max(CalendarLayoutMetrics.MinEventHitHeight, (int)(evt.Duration.TotalHours * slotHeight) - (eventInsetY * 2))
                    );
                    bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                    bool isHovered = _hoveredEvent?.Id == evt.Id;
                    _stylePainter.PaintEventBlock(g, eventRect, evt, isSelected, isHovered, ctx);
                }
            }
        }
    }
}