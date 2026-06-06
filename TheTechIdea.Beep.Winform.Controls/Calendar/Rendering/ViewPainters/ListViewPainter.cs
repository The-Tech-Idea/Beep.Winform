using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
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
        public string Key => "list";
        public string DisplayLabel => "List";
        public int VisibleDayCount => 7;
        public bool IsTimedView => false;
        public bool IsMonthGrid => false;
        public bool RequiresLeftGutter => false;
        public bool HasAllDayStrip => false;
        public bool SupportsEventDrag => false;
        public bool IsHorizontalTimeAxis => false;

        public DateTime NavigatePrevious(DateTime d) => d.AddMonths(-1);
        public DateTime NavigateNext(DateTime d) => d.AddMonths(1);
        public string GetHeaderText(DateTime d) => d.ToString("MMMM yyyy") + " Events";
        public DateTime GetVisibleRangeStart(DateTime d) => new DateTime(d.Year, d.Month, 1);
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddMonths(1);

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

        public DateTime? GetDateTimeFromLocation(Point location, ViewPaintArgs args)
        {
            // List view is time-agnostic; the Y axis lists events
            // chronologically, not by date. The hit-test API is reserved for
            // painters that anchor Y/X to a time-of-day; we return null so
            // the central snap code uses its fallback range.
            return null;
        }

        private static void PaintListRow(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args)
        {
            // W8 - delegate to developer's IBeepUIComponent when registered.
            var cellKey = $"evt:{evt.Id}";
            var ctx = new CalendarCellContext(
                CalendarCellKind.EventBlock,
                evt,
                evt.StartTime.Date,
                args.State?.ViewMode ?? CalendarViewMode.Week1,
                0, 0);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

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
