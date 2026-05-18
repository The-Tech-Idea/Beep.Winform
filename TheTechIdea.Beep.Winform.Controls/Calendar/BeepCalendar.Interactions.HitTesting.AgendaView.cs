using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveAgendaInteraction(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int padding = ScaleMetric(CalendarLayoutMetrics.SidebarPadding);
            int headerHeight = ScaleMetric(24);
            int rowHeight = ScaleMetric(CalendarLayoutMetrics.ListRowHeight);
            int rowSpacing = ScaleMetric(CalendarLayoutMetrics.ListRowSpacing);

            var groups = _eventService?.GetEventsForMonth(_state.CurrentDate)
                .OrderBy(e => e.StartTime)
                .GroupBy(e => e.StartTime.Date)
                .OrderBy(g => g.Key)
                .ToList() ?? new List<IGrouping<DateTime, CalendarEvent>>();

            int yPos = grid.Y + padding;
            foreach (var group in groups)
            {
                var headerRect = new Rectangle(grid.X + padding, yPos, Math.Max(1, grid.Width - (padding * 2)), headerHeight);
                if (headerRect.Contains(location))
                {
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.DateCell,
                        RequestedMode = CalendarInteractionMode.SelectDate,
                        Location = location,
                        Date = group.Key,
                        Bounds = headerRect
                    };
                }

                yPos += headerHeight + rowSpacing;
                foreach (var evt in group)
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
                    if (yPos > grid.Bottom)
                    {
                        return CreateEmptyAgendaHit(location);
                    }
                }

                yPos += rowSpacing;
                if (yPos > grid.Bottom)
                {
                    return CreateEmptyAgendaHit(location);
                }
            }

            return CreateEmptyAgendaHit(location);
        }

        private CalendarInteractionHitTestResult CreateEmptyAgendaHit(Point location)
        {
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
