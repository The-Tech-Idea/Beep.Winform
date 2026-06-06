using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Timeline"/>. Renders
    /// a resource-lane × day-axis grid with virtualization. Resources come
    /// from <c>args.Resources</c> (or are derived from events when empty);
    /// the visible lane range is computed from the grid height.
    /// </summary>
    public sealed class TimelineViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Timeline;
        public string Key => "timeline";
        public string DisplayLabel => "Timeline";
        public int VisibleDayCount => 7;
        public bool IsTimedView => false;
        public bool IsMonthGrid => false;
        public bool RequiresLeftGutter => true;
        public bool HasAllDayStrip => false;
        public bool SupportsEventDrag => true;
        public bool IsHorizontalTimeAxis => true;

        public DateTime NavigatePrevious(DateTime d) => d.AddDays(-7);
        public DateTime NavigateNext(DateTime d) => d.AddDays(7);
        public string GetHeaderText(DateTime d) => $"Timeline of {d:MMMM yyyy}";
        public DateTime GetVisibleRangeStart(DateTime d) => d.AddDays(-(int)d.DayOfWeek).Date;
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddDays(7);

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            int laneHeaderWidth = Math.Min(140, Math.Max(80, grid.Width / 3));
            int dayHeaderHeight = surface.DayHeaderHeight;
            int laneHeight = 72;
            int laneSpacing = 8;
            int dayCount = 7;
            var startOfWeek = GetStartOfWeek(surface.CurrentDate);
            var resources = ResolveResources(args);
            int contentWidth = Math.Max(1, grid.Width - laneHeaderWidth);
            var contentArea = new Rectangle(grid.X + laneHeaderWidth, grid.Y, contentWidth, grid.Height);
            if (contentWidth <= 0 || grid.Height <= dayHeaderHeight) return;

            GetVisibleLaneRange(grid, dayHeaderHeight, laneHeight, laneSpacing, resources.Count,
                out int firstVisibleLane, out int lastVisibleLane);
            var eventsByLane = BuildLaneEventLookup(resources, startOfWeek, startOfWeek.AddDays(dayCount - 1), args);

            for (int day = 0; day < dayCount; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = CalendarPainterHelpers.GetColumnRect(
                    new Rectangle(contentArea.X, contentArea.Y, contentArea.Width, dayHeaderHeight),
                    day, dayCount);
                PaintDayHeader(g, headerRect, dayDate, dayDate.Date == DateTime.Today, args);
            }

            for (int laneIndex = firstVisibleLane; laneIndex <= lastVisibleLane; laneIndex++)
            {
                var lane = resources[laneIndex];
                int laneTop = grid.Y + dayHeaderHeight + laneIndex * (laneHeight + laneSpacing) + laneSpacing;
                var laneRect = new Rectangle(contentArea.X, laneTop, contentArea.Width, laneHeight);
                var headerRect = new Rectangle(grid.X, laneTop, laneHeaderWidth, laneHeight);

                using (var backBrush = new SolidBrush(args.BackgroundColor))
                using (var borderPen = new Pen(args.BorderColor))
                {
                    g.FillRectangle(backBrush, headerRect);
                    g.DrawRectangle(borderPen, headerRect);
                    CalendarPainterHelpers.DrawText(g, lane.ToString() ?? string.Empty,
                        args.DaysHeaderFont ?? args.DayFont, args.ForegroundColor,
                        headerRect, StringAlignment.Near, StringAlignment.Center);
                }

                var laneBack = (laneIndex % 2 == 0)
                    ? args.BackgroundColor
                    : Color.FromArgb(255,
                        Math.Min(255, args.BackgroundColor.R + 6),
                        Math.Min(255, args.BackgroundColor.G + 6),
                        Math.Min(255, args.BackgroundColor.B + 6));
                using (var laneBrush = new SolidBrush(laneBack))
                using (var lanePen = new Pen(args.BorderColor))
                {
                    g.FillRectangle(laneBrush, laneRect);
                    g.DrawRectangle(lanePen, laneRect);
                }

                if (lane.Color != Color.Empty)
                {
                    using var accentBrush = new SolidBrush(lane.Color);
                    g.FillRectangle(accentBrush, new Rectangle(laneRect.X, laneRect.Y, 4, laneRect.Height));
                }

                if (!eventsByLane.TryGetValue(lane.Id ?? string.Empty, out var laneEvents))
                    continue;

                foreach (var evt in laneEvents)
                {
                    int dayOffset = Math.Max(0, (evt.StartTime.Date - startOfWeek).Days);
                    int endOffset = Math.Min(dayCount - 1, (evt.EndTime.Date - startOfWeek).Days);
                    if (endOffset < 0 || dayOffset >= dayCount) continue;

                    var firstDayRect = CalendarPainterHelpers.GetColumnRect(contentArea, dayOffset, dayCount);
                    var lastDayRect = CalendarPainterHelpers.GetColumnRect(contentArea,
                        Math.Max(dayOffset, endOffset), dayCount);
                    int insetX = surface.EventInsetX;
                    int insetY = surface.EventInsetY;
                    int x = firstDayRect.X + insetX;
                    int w = Math.Max(24, lastDayRect.Right - firstDayRect.X - (insetX * 2));
                    var eventRect = new Rectangle(x, laneTop + insetY,
                        w, Math.Max(1, laneHeight - (insetY * 2)));
                    PaintEventBlock(g, eventRect, evt, args);
                }
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return EmptyHit(location, args);
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            int laneHeaderWidth = Math.Min(140, Math.Max(80, grid.Width / 3));
            int dayHeaderHeight = surface.DayHeaderHeight;
            int laneHeight = 72;
            int laneSpacing = 8;
            int dayCount = 7;

            var startOfWeek = GetStartOfWeek(surface.CurrentDate);
            var resources = ResolveResources(args);
            var contentArea = new Rectangle(grid.X + laneHeaderWidth, grid.Y,
                Math.Max(0, grid.Width - laneHeaderWidth), grid.Height);
            if (contentArea.Width <= 0 || grid.Height <= dayHeaderHeight)
                return EmptyHit(location, args);

            if (location.Y <= grid.Y + dayHeaderHeight)
            {
                int day = CalendarPainterHelpers.GetColumnIndex(contentArea, location.X, dayCount);
                if (day >= 0)
                {
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.DateCell,
                        RequestedMode = CalendarInteractionMode.SelectDate,
                        Location = location,
                        Date = startOfWeek.AddDays(day).Date,
                        Bounds = CalendarPainterHelpers.GetColumnRect(
                            new Rectangle(contentArea.X, contentArea.Y, contentArea.Width, dayHeaderHeight),
                            day, dayCount)
                    };
                }
                return EmptyHit(location, args);
            }

            GetVisibleLaneRange(grid, dayHeaderHeight, laneHeight, laneSpacing, resources.Count,
                out int firstVisibleLane, out int lastVisibleLane);
            var eventsByLane = BuildLaneEventLookup(resources, startOfWeek, startOfWeek.AddDays(dayCount - 1), args);

            for (int laneIndex = firstVisibleLane; laneIndex <= lastVisibleLane; laneIndex++)
            {
                int laneTop = grid.Y + dayHeaderHeight + laneIndex * (laneHeight + laneSpacing) + laneSpacing;
                var laneRect = new Rectangle(contentArea.X, laneTop, contentArea.Width, laneHeight);
                if (!laneRect.Contains(location)) continue;

                var lane = resources[laneIndex];
                if (eventsByLane.TryGetValue(lane.Id ?? string.Empty, out var laneEvents))
                {
                    foreach (var evt in laneEvents.OrderByDescending(e => e.StartTime))
                    {
                        var eventRect = GetTimelineEventRect(evt, startOfWeek, dayCount, laneRect, laneTop, laneHeight, surface);
                        if (eventRect.Contains(location))
                        {
                            var edge = CalendarPainterHelpers.ResolveResizeEdge(location, eventRect, 6);
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
                                Bounds = eventRect
                            };
                        }
                    }
                }

                int selectedDay = CalendarPainterHelpers.GetColumnIndex(contentArea, location.X, dayCount);
                if (selectedDay >= 0)
                {
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.DateCell,
                        RequestedMode = CalendarInteractionMode.CreateEvent,
                        Location = location,
                        Date = startOfWeek.AddDays(selectedDay).Date,
                        Bounds = CalendarPainterHelpers.GetColumnRect(laneRect, selectedDay, dayCount)
                    };
                }
                return EmptyHit(location, args);
            }

            return EmptyHit(location, args);
        }

        public DateTime? GetDateTimeFromLocation(Point location, ViewPaintArgs args) => null;

        private static DateTime GetStartOfWeek(DateTime date) => date.Date.AddDays(-(int)date.DayOfWeek);

        private static List<CalendarResource> ResolveResources(ViewPaintArgs args)
        {
            var visible = (args.Resources ?? new List<CalendarResource>())
                .Where(r => r != null && r.IsVisible)
                .OrderBy(r => r.SortOrder).ThenBy(r => r.Name)
                .ToList();
            if (visible.Count > 0) return visible;

            var fromEvents = (args.EventService?.GetEventsForMonth(args.Surface.CurrentDate) ?? new List<CalendarEvent>())
                .SelectMany(evt => CalendarResourceHelper.GetEventResourceIds(evt).DefaultIfEmpty(string.Empty))
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select((id, index) => new CalendarResource { Id = id, Name = id, SortOrder = index })
                .ToList();
            if (fromEvents.Count > 0) return fromEvents;

            return new List<CalendarResource>
            {
                new CalendarResource { Id = "all", Name = "All events", SortOrder = 0 }
            };
        }

        private static Dictionary<string, List<CalendarEvent>> BuildLaneEventLookup(
            List<CalendarResource> resources, DateTime windowStart, DateTime windowEnd, ViewPaintArgs args)
        {
            var lookup = new Dictionary<string, List<CalendarEvent>>(StringComparer.OrdinalIgnoreCase);
            foreach (var resource in resources)
                lookup[resource.Id ?? string.Empty] = new List<CalendarEvent>();

            var events = (args.EventService?.GetEventsForDateRange(windowStart, windowEnd) ?? new List<CalendarEvent>())
                .OrderBy(evt => evt.StartTime);
            foreach (var evt in events)
            {
                var ids = CalendarResourceHelper.GetEventResourceIds(evt);
                if (ids.Count == 0)
                {
                    if (lookup.TryGetValue("all", out var allLane)) allLane.Add(evt);
                    continue;
                }
                foreach (var id in ids)
                {
                    if (lookup.TryGetValue(id, out var lane)) lane.Add(evt);
                }
            }
            return lookup;
        }

        private static void GetVisibleLaneRange(Rectangle grid, int dayHeaderHeight, int laneHeight, int laneSpacing,
            int laneCount, out int firstVisibleLane, out int lastVisibleLane)
        {
            if (laneCount <= 0)
            {
                firstVisibleLane = 0;
                lastVisibleLane = -1;
                return;
            }
            int laneStride = Math.Max(1, laneHeight + laneSpacing);
            int lanesTop = grid.Y + dayHeaderHeight + laneSpacing;
            int visibleBottom = grid.Bottom - 1;
            firstVisibleLane = 0;
            lastVisibleLane = Math.Min(laneCount - 1, Math.Max(firstVisibleLane, (visibleBottom - lanesTop) / laneStride));
        }

        private static Rectangle GetTimelineEventRect(CalendarEvent evt, DateTime startOfWeek, int dayCount,
            Rectangle laneRect, int laneTop, int laneHeight, CalendarSurfaceModel surface)
        {
            int startOffset = Math.Max(0, (evt.StartTime.Date - startOfWeek).Days);
            int endOffset = Math.Min(dayCount - 1, (evt.EndTime.Date - startOfWeek).Days);
            if (endOffset < startOffset) endOffset = startOffset;
            var firstDayRect = CalendarPainterHelpers.GetColumnRect(laneRect, startOffset, dayCount);
            var lastDayRect = CalendarPainterHelpers.GetColumnRect(laneRect, endOffset, dayCount);
            int insetX = surface.EventInsetX;
            int insetY = surface.EventInsetY;
            int x = firstDayRect.X + Math.Min(insetX, Math.Max(0, (firstDayRect.Width - 1) / 2));
            int width = Math.Max(24, lastDayRect.Right - firstDayRect.X - (insetX * 2));
            return new Rectangle(x, laneTop + insetY, width, Math.Max(1, laneHeight - (insetY * 2)));
        }

        private static void PaintDayHeader(Graphics g, Rectangle rect, DateTime dayDate, bool isToday, ViewPaintArgs args)
        {
            // W8 - delegate to developer's IBeepUIComponent DateCell factory for the
            // timeline day-axis header (which is a date cell) when one is registered.
            // Falls through to the default filled header + text otherwise.
            string cellKey = $"date:{dayDate:yyyy-MM-dd}:timeline-header";
            var ctx = new CalendarCellContext(
                CalendarCellKind.DateCell, null, dayDate,
                args.Surface?.ViewMode ?? CalendarViewMode.Timeline, 0, 0);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            CalendarPainterHelpers.DrawText(g, dayDate.ToString("ddd\nd"),
                args.DaysHeaderFont ?? args.DayFont,
                isToday ? args.TodayForeColor : args.ForegroundColor,
                rect, StringAlignment.Center, StringAlignment.Center);
        }

        private static void PaintEventBlock(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
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
                CalendarPainterHelpers.FillRoundedRect(g, accent, 0, Color.FromArgb(80, 0, 0, 0));
            }
            var textRect = new Rectangle(rect.X + args.Metrics.EventAccentWidth + 4, rect.Y + 2,
                rect.Width - args.Metrics.EventAccentWidth - 6, Math.Max(0, rect.Height - 4));
            CalendarPainterHelpers.DrawText(g, evt.Title,
                args.EventFont ?? args.DayFont, args.ForegroundColor, textRect,
                StringAlignment.Near, StringAlignment.Near, centerVertically: false);
        }

        // W2-Redo-10 GAP 1 - TimelineViewPainter.EmptyHit returned
        // SelectDate which is inconsistent with all 13 other painters
        // (DayView/Week/WorkWeek/Month/Agenda/List/Week1..Week7) that all
        // return CreateEvent for EmptyHit. The RequestedMode is metadata
        // not consumed by BeepCalendar's ResolveDragMode or
        // OnMouseDoubleClick (both branch on TargetKind, not on
        // RequestedMode), so the actual user-facing behavior was the same
        // for both — but downstream consumers reading the
        // CalendarInteractionHitTestResult would see an inconsistent
        // value. Normalize to CreateEvent.
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
