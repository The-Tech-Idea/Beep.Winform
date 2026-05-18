using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarEvent ResolveEventInTimedView(
            Point location,
            List<CalendarEvent> events,
            Rectangle dayColumnRect,
            DateTime dayDate)
        {
            return ResolveTimedEventHit(location, events, dayColumnRect, dayDate).Event;
        }

        private (CalendarEvent Event, Rectangle Bounds, CalendarEventResizeEdge ResizeEdge) ResolveTimedEventHit(
            Point location,
            List<CalendarEvent> events,
            Rectangle dayColumnRect,
            DateTime dayDate)
        {
            if (events == null || events.Count == 0)
            {
                return (null, Rectangle.Empty, CalendarEventResizeEdge.None);
            }

            int eventInsetX = ScaleMetric(CalendarLayoutMetrics.EventInsetX);
            int eventInsetY = ScaleMetric(CalendarLayoutMetrics.EventInsetY);
            int minEventHeight = ScaleMetric(CalendarLayoutMetrics.MinEventHitHeight);

            foreach (var evt in events.OrderByDescending(evt => evt.StartTime))
            {
                var eventRect = CalendarLayoutGeometry.GetTimedEventRect(dayColumnRect, evt, dayDate, eventInsetX, eventInsetY, minEventHeight);
                if (eventRect.Contains(location))
                {
                    return (evt, eventRect, ResolveResizeEdge(location, eventRect));
                }
            }

            return (null, Rectangle.Empty, CalendarEventResizeEdge.None);
        }
    }
}
