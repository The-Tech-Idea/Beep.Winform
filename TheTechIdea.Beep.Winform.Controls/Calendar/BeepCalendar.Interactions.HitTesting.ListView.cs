using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveListInteraction(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int rowHeight = ScaleMetric(CalendarLayoutMetrics.ListRowHeight);
            int rowSpacing = ScaleMetric(CalendarLayoutMetrics.ListRowSpacing);
            int padding = ScaleMetric(CalendarLayoutMetrics.SidebarPadding);
            var monthEvents = _eventService?.GetEventsForMonth(_state.CurrentDate).OrderBy(e => e.StartTime).ToList() ?? new List<CalendarEvent>();
            int yPos = grid.Y + padding;

            foreach (var evt in monthEvents)
            {
                var eventRect = new Rectangle(grid.X + padding, yPos, Math.Max(1, grid.Width - (padding * 2)), rowHeight);
                if (eventRect.Contains(location))
                {
                    var resizeEdge = ResolveResizeEdge(location, eventRect);
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.EventBlock,
                        RequestedMode = resizeEdge == CalendarEventResizeEdge.Start
                            ? CalendarInteractionMode.ResizeStart
                            : resizeEdge == CalendarEventResizeEdge.End
                                ? CalendarInteractionMode.ResizeEnd
                                : CalendarInteractionMode.SelectEvent,
                        ResizeEdge = resizeEdge,
                        Location = location,
                        Date = evt.StartTime.Date,
                        Event = evt,
                        Bounds = eventRect
                    };
                }

                yPos += rowHeight + rowSpacing;
            }

            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.EmptySurface,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = _state.CurrentDate.Date
            };
        }
    }
}
