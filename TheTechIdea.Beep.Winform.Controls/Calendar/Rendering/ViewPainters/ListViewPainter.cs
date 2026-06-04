using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.List"/>. Vertical
    /// list of event rows for the current month, ordered by start time.
    /// </summary>
    public sealed class ListViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.List;

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            var monthEvents = (args.EventService?.GetEventsForMonth(surface.CurrentDate) ?? new List<CalendarEvent>())
                .OrderBy(e => e.StartTime).ToList();

            int rowIndex = 0;
            for (int i = 0; i < monthEvents.Count; i++)
            {
                var evt = monthEvents[i];
                var rowRect = surface.GetListRowRect(rowIndex);
                if (rowRect.Bottom > grid.Bottom) break;
                PaintListRow(g, rowRect, evt, args);
                rowIndex++;
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null)
                return EmptyHit(location, args);
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            var monthEvents = (args.EventService?.GetEventsForMonth(surface.CurrentDate) ?? new List<CalendarEvent>())
                .OrderBy(e => e.StartTime).ToList();
            int rowIndex = 0;
            for (int i = 0; i < monthEvents.Count; i++)
            {
                var evt = monthEvents[i];
                var rowRect = surface.GetListRowRect(rowIndex);
                if (rowRect.Bottom > grid.Bottom) break;
                if (rowRect.Contains(location))
                {
                    var edge = CalendarPainterHelpers.ResolveResizeEdge(location, rowRect, 6);
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.EventBlock,
                        RequestedMode = edge == CalendarEventResizeEdge.Start
                            ? CalendarInteractionMode.ResizeStart
                            : edge == CalendarEventResizeEdge.End
                                ? CalendarInteractionMode.ResizeEnd
                                : CalendarInteractionMode.SelectEvent,
                        ResizeEdge = edge,
                        Location = location,
                        Date = evt.StartTime.Date,
                        Event = evt,
                        Bounds = rowRect
                    };
                }
                rowIndex++;
            }
            return EmptyHit(location, args);
        }

        private static void PaintListRow(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args)
        {
            Color fill = args.GetCategoryColor(evt.CategoryId);
            if (args.SelectedEvent?.Id == evt.Id) fill = args.SelectedBackColor;
            if (args.HoveredEventId == evt.Id) fill = args.HoverBackColor;
            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.EventCornerRadius, fill);

            if (args.Metrics.ShowEventAccentStripe)
            {
                var accent = new Rectangle(rect.X, rect.Y, args.Metrics.EventAccentWidth, rect.Height);
                CalendarPainterHelpers.FillRoundedRect(g, accent, 0, fill);
            }

            int textLeft = rect.X + args.Metrics.EventAccentWidth + 8;
            int timeWidth = 96;
            CalendarPainterHelpers.DrawText(g, evt.StartTime.ToString("ddd, MMM d • h:mm tt"),
                args.EventFont ?? args.DayFont, args.ForegroundColor,
                new Rectangle(textLeft, rect.Y, timeWidth, rect.Height),
                StringAlignment.Near, StringAlignment.Center);

            CalendarPainterHelpers.DrawText(g, evt.Title,
                args.EventFont ?? args.DayFont, args.ForegroundColor,
                new Rectangle(textLeft + timeWidth, rect.Y,
                    rect.Width - timeWidth - args.Metrics.EventAccentWidth - 12, rect.Height),
                StringAlignment.Near, StringAlignment.Center);
        }

        private static CalendarInteractionHitTestResult EmptyHit(Point location, ViewPaintArgs args) =>
            new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.EmptySurface,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = args.Surface?.CurrentDate.Date ?? DateTime.Today
            };
    }
}
