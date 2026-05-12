using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarEvent ResolveEventInTimedView(
            Point location,
            List<CalendarEvent> events,
            Rectangle grid,
            int dayHeaderHeight,
            int timeColumnWidth,
            int usableWidth,
            int slotHeight,
            DateTime dayDate)
        {
            if (events == null || events.Count == 0)
            {
                return null;
            }

            int eventInsetX = ScaleMetric(CalendarLayoutMetrics.EventInsetX);
            int eventInsetY = ScaleMetric(CalendarLayoutMetrics.EventInsetY);
            int columnWidth = Math.Max(1, usableWidth);
            var eventsByHour = events.GroupBy(e => e.StartTime.Hour).ToDictionary(group => group.Key, group => group.ToList());

            for (int hour = 0; hour < 24; hour++)
            {
                int yPos = grid.Y + dayHeaderHeight + hour * slotHeight;
                var slotRect = new Rectangle(grid.X + timeColumnWidth, yPos, columnWidth, slotHeight);
                if (!slotRect.Contains(location) && !eventsByHour.ContainsKey(hour))
                {
                    continue;
                }

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
                        Math.Max(CalendarLayoutMetrics.MinEventHitHeight, (int)(evt.Duration.TotalHours * slotHeight) - (eventInsetY * 2)));

                    if (eventRect.Contains(location))
                    {
                        return evt;
                    }
                }
            }

            return null;
        }
    }
}