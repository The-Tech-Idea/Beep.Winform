using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal sealed class TimelineViewRenderer : ICalendarViewRenderer
    {
        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int laneHeaderWidth = CommonDrawing.ScaleMetric(140, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            int laneHeight = CommonDrawing.ScaleMetric(72, ctx.DensityScale);
            int laneSpacing = CommonDrawing.ScaleMetric(8, ctx.DensityScale);
            int dayCount = 7;

            var startOfWeek = GetStartOfWeek(ctx.State.VisibleRangeStart ?? ctx.State.CurrentDate.Date);
            var resources = BuildResourceLanes(ctx).ToList();
            int contentWidth = Math.Max(1, grid.Width - laneHeaderWidth);
            int dayWidth = Math.Max(1, contentWidth / dayCount);
            GetVisibleLaneRange(grid, dayHeaderHeight, laneHeight, laneSpacing, resources.Count, out int firstVisibleLane, out int lastVisibleLane);
            var eventsByLane = BuildLaneEventLookup(ctx, resources, startOfWeek, startOfWeek.AddDays(dayCount - 1));

            DrawDayAxis(g, ctx, grid, startOfWeek, laneHeaderWidth, dayWidth, dayHeaderHeight);
            DrawResourceAxis(g, ctx, grid, resources, laneHeaderWidth, laneHeight, laneSpacing, dayHeaderHeight, firstVisibleLane, lastVisibleLane);

            for (int laneIndex = firstVisibleLane; laneIndex <= lastVisibleLane; laneIndex++)
            {
                var lane = resources[laneIndex];
                int laneTop = grid.Y + dayHeaderHeight + laneIndex * (laneHeight + laneSpacing) + laneSpacing;
                var laneRect = new Rectangle(grid.X + laneHeaderWidth, laneTop, contentWidth, laneHeight);
                DrawLaneBackground(g, ctx, laneRect, lane, laneIndex);

                if (!eventsByLane.TryGetValue(lane.Id ?? string.Empty, out var laneEvents))
                {
                    continue;
                }

                foreach (var evt in laneEvents)
                {
                    int dayOffset = (evt.StartTime.Date - startOfWeek).Days;
                    if (dayOffset < 0 || dayOffset >= dayCount)
                    {
                        continue;
                    }

                    int spanDays = Math.Max(1, (int)Math.Ceiling((evt.EndTime.Date - evt.StartTime.Date).TotalDays) + 1);
                    int x = grid.X + laneHeaderWidth + dayOffset * dayWidth + CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX, ctx.DensityScale);
                    int w = Math.Max(24, spanDays * dayWidth - CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX * 2, ctx.DensityScale));
                    var eventRect = new Rectangle(x, laneTop + CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetY, ctx.DensityScale), w, laneHeight - CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetY * 2, ctx.DensityScale));
                    bool isSelected = ctx.State.SelectedEvent?.Id == evt.Id;
                    CommonDrawing.DrawEventCard(g, ctx, evt, eventRect, isSelected, includeDescription: true);
                }
            }
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int laneHeaderWidth = CommonDrawing.ScaleMetric(140, ctx.DensityScale);
            int dayHeaderHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight, ctx.DensityScale);
            int laneHeight = CommonDrawing.ScaleMetric(72, ctx.DensityScale);
            int laneSpacing = CommonDrawing.ScaleMetric(8, ctx.DensityScale);
            int dayCount = 7;

            var resources = BuildResourceLanes(ctx).ToList();
            var startOfWeek = GetStartOfWeek(ctx.State.VisibleRangeStart ?? ctx.State.CurrentDate.Date);
            int contentWidth = Math.Max(1, grid.Width - laneHeaderWidth);
            int dayWidth = Math.Max(1, contentWidth / dayCount);
            GetVisibleLaneRange(grid, dayHeaderHeight, laneHeight, laneSpacing, resources.Count, out int firstVisibleLane, out int lastVisibleLane);
            var eventsByLane = BuildLaneEventLookup(ctx, resources, startOfWeek, startOfWeek.AddDays(dayCount - 1));

            if (location.Y <= grid.Y + dayHeaderHeight)
            {
                int day = Math.Max(0, Math.Min(dayCount - 1, (location.X - grid.X - laneHeaderWidth) / dayWidth));
                ctx.State.SelectedDate = startOfWeek.AddDays(day).Date;
                ctx.State.CurrentDate = ctx.State.SelectedDate;
                return;
            }

            for (int laneIndex = firstVisibleLane; laneIndex <= lastVisibleLane; laneIndex++)
            {
                var lane = resources[laneIndex];
                int laneTop = grid.Y + dayHeaderHeight + laneIndex * (laneHeight + laneSpacing) + laneSpacing;
                var laneRect = new Rectangle(grid.X + laneHeaderWidth, laneTop, contentWidth, laneHeight);
                if (!laneRect.Contains(location))
                {
                    continue;
                }

                if (!eventsByLane.TryGetValue(lane.Id ?? string.Empty, out var laneEvents))
                {
                    laneEvents = new List<CalendarEvent>();
                }

                foreach (var evt in laneEvents)
                {
                    int dayOffset = (evt.StartTime.Date - startOfWeek).Days;
                    if (dayOffset < 0 || dayOffset >= dayCount)
                    {
                        continue;
                    }

                    int spanDays = Math.Max(1, (int)Math.Ceiling((evt.EndTime.Date - evt.StartTime.Date).TotalDays) + 1);
                    int x = grid.X + laneHeaderWidth + dayOffset * dayWidth + CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX, ctx.DensityScale);
                    int w = Math.Max(24, spanDays * dayWidth - CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetX * 2, ctx.DensityScale));
                    var eventRect = new Rectangle(x, laneTop + CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetY, ctx.DensityScale), w, laneHeight - CommonDrawing.ScaleMetric(CalendarLayoutMetrics.EventInsetY * 2, ctx.DensityScale));
                    if (eventRect.Contains(location))
                    {
                        ctx.State.SelectedEvent = evt;
                        ctx.State.SelectedDate = evt.StartTime.Date;
                        ctx.State.CurrentDate = evt.StartTime.Date;
                        return;
                    }
                }

                ctx.State.SelectedDate = startOfWeek.AddDays(Math.Max(0, Math.Min(dayCount - 1, (location.X - grid.X - laneHeaderWidth) / dayWidth))).Date;
                ctx.State.CurrentDate = ctx.State.SelectedDate;
                return;
            }
        }

        private static IEnumerable<CalendarResource> BuildResourceLanes(CalendarRenderContext ctx)
        {
            var visible = ctx.Resources?.Where(r => r != null && r.IsVisible).OrderBy(r => r.SortOrder).ThenBy(r => r.Name).ToList() ?? new List<CalendarResource>();
            if (visible.Count > 0)
            {
                return visible;
            }

            var fromEvents = ctx.EventService.GetEventsForMonth(ctx.State.CurrentDate)
                .SelectMany(evt => CalendarResourceHelper.GetEventResourceIds(evt).DefaultIfEmpty(string.Empty))
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select((id, index) => new CalendarResource { Id = id, Name = id, SortOrder = index })
                .ToList();

            if (fromEvents.Count > 0)
            {
                return fromEvents;
            }

            return new[] { new CalendarResource { Id = "all", Name = "All events", SortOrder = 0 } };
        }

        private static IEnumerable<CalendarEvent> GetLaneEvents(CalendarRenderContext ctx, CalendarResource lane)
        {
            var events = ctx.EventService.GetEventsForMonth(ctx.State.CurrentDate);
            return events.Where(evt =>
            {
                var eventResourceIds = CalendarResourceHelper.GetEventResourceIds(evt);
                return eventResourceIds.Count == 0
                    ? string.Equals(lane.Id, "all", StringComparison.OrdinalIgnoreCase)
                    : eventResourceIds.Any(id => string.Equals(id, lane.Id, StringComparison.OrdinalIgnoreCase));
            }).OrderBy(evt => evt.StartTime);
        }

        private static Dictionary<string, List<CalendarEvent>> BuildLaneEventLookup(CalendarRenderContext ctx, IReadOnlyList<CalendarResource> resources, DateTime windowStart, DateTime windowEnd)
        {
            var lookup = new Dictionary<string, List<CalendarEvent>>(StringComparer.OrdinalIgnoreCase);
            foreach (var resource in resources)
            {
                lookup[resource.Id ?? string.Empty] = new List<CalendarEvent>();
            }

            var events = ctx.EventService.GetEventsForDateRange(windowStart, windowEnd).OrderBy(evt => evt.StartTime);
            foreach (var evt in events)
            {
                var eventResourceIds = CalendarResourceHelper.GetEventResourceIds(evt);
                if (eventResourceIds.Count == 0)
                {
                    if (lookup.TryGetValue("all", out var allLane))
                    {
                        allLane.Add(evt);
                    }

                    continue;
                }

                foreach (var resourceId in eventResourceIds)
                {
                    if (lookup.TryGetValue(resourceId, out var laneEvents))
                    {
                        laneEvents.Add(evt);
                    }
                }
            }

            return lookup;
        }

        private static void GetVisibleLaneRange(Rectangle grid, int dayHeaderHeight, int laneHeight, int laneSpacing, int laneCount, out int firstVisibleLane, out int lastVisibleLane)
        {
            if (laneCount <= 0)
            {
                firstVisibleLane = 0;
                lastVisibleLane = -1;
                return;
            }

            int laneStride = Math.Max(1, laneHeight + laneSpacing);
            int lanesTop = grid.Y + dayHeaderHeight + laneSpacing;
            int visibleTop = lanesTop;
            int visibleBottom = grid.Bottom - 1;

            firstVisibleLane = Math.Max(0, (visibleTop - lanesTop) / laneStride);
            lastVisibleLane = Math.Min(laneCount - 1, Math.Max(firstVisibleLane, (visibleBottom - lanesTop) / laneStride));
        }

        private static void DrawDayAxis(Graphics g, CalendarRenderContext ctx, Rectangle grid, DateTime startOfWeek, int laneHeaderWidth, int dayWidth, int dayHeaderHeight)
        {
            var headerBackColor = ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250);
            var headerForeColor = ctx.Theme?.CalendarForeColor ?? Color.Black;
            var todayColor = ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244);

            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = new Rectangle(grid.X + laneHeaderWidth + day * dayWidth, grid.Y, dayWidth, dayHeaderHeight);
                bool isToday = dayDate.Date == DateTime.Today;
                using (var brush = new SolidBrush(isToday ? todayColor : headerBackColor))
                    g.FillRectangle(brush, headerRect);
                using (var brush = new SolidBrush(isToday ? Color.White : headerForeColor))
                    g.DrawString($"{dayDate:ddd}\n{dayDate:dd}", ctx.DayFont, brush, headerRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private static void DrawResourceAxis(Graphics g, CalendarRenderContext ctx, Rectangle grid, IReadOnlyList<CalendarResource> resources, int laneHeaderWidth, int laneHeight, int laneSpacing, int dayHeaderHeight, int firstVisibleLane, int lastVisibleLane)
        {
            var headerBackColor = ctx.Theme?.CardBackColor ?? Color.White;
            var borderColor = ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224);
            var titleColor = ctx.Theme?.CalendarForeColor ?? Color.Black;

            using var backBrush = new SolidBrush(headerBackColor);
            using var borderPen = new Pen(borderColor);
            using var titleBrush = new SolidBrush(titleColor);

            for (int i = firstVisibleLane; i <= lastVisibleLane; i++)
            {
                int top = grid.Y + dayHeaderHeight + i * (laneHeight + laneSpacing) + laneSpacing;
                var rect = new Rectangle(grid.X, top, laneHeaderWidth, laneHeight);
                g.FillRectangle(backBrush, rect);
                g.DrawRectangle(borderPen, rect);
                g.DrawString(resources[i].ToString(), ctx.DaysHeaderFont, titleBrush, rect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
            }
        }

        private static void DrawLaneBackground(Graphics g, CalendarRenderContext ctx, Rectangle laneRect, CalendarResource lane, int laneIndex)
        {
            var laneBack = laneIndex % 2 == 0
                ? (ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250))
                : (ctx.Theme?.CardBackColor ?? Color.White);
            var borderColor = ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224);
            using (var backBrush = new SolidBrush(laneBack))
            using (var borderPen = new Pen(borderColor))
            {
                g.FillRectangle(backBrush, laneRect);
                g.DrawRectangle(borderPen, laneRect);
            }

            if (lane.Color != Color.Empty)
            {
                using var accentBrush = new SolidBrush(lane.Color);
                g.FillRectangle(accentBrush, new Rectangle(laneRect.X, laneRect.Y, 4, laneRect.Height));
            }
        }

        private static DateTime GetStartOfWeek(DateTime date)
        {
            return date.Date.AddDays(-(int)date.DayOfWeek);
        }
    }
}