using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveTimelineInteraction(Point location)
        {
            var grid = _rects.CalendarGridRect;
            int laneHeaderWidth = Math.Min(ScaleMetric(140), Math.Max(ScaleMetric(80), grid.Width / 3));
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int laneHeight = ScaleMetric(72);
            int laneSpacing = ScaleMetric(8);
            int dayCount = 7;

            var startOfWeek = GetTimelineStartOfWeek(_state.VisibleRangeStart ?? _state.CurrentDate.Date);
            var resources = ResolveTimelineResourceLanes().ToList();
            var contentArea = new Rectangle(grid.X + laneHeaderWidth, grid.Y, Math.Max(0, grid.Width - laneHeaderWidth), grid.Height);
            if (contentArea.Width <= 0 || grid.Height <= dayHeaderHeight)
            {
                return CreateEmptyTimelineHit(location);
            }

            if (location.Y <= grid.Y + dayHeaderHeight)
            {
                int day = CalendarLayoutGeometry.GetColumnIndex(contentArea, location.X, dayCount);
                if (day >= 0)
                {
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.DateCell,
                        RequestedMode = CalendarInteractionMode.SelectDate,
                        Location = location,
                        Date = startOfWeek.AddDays(day).Date,
                        Bounds = CalendarLayoutGeometry.GetColumnRect(
                            new Rectangle(contentArea.X, contentArea.Y, contentArea.Width, dayHeaderHeight),
                            day,
                            dayCount)
                    };
                }

                return CreateEmptyTimelineHit(location);
            }

            GetTimelineVisibleLaneRange(grid, dayHeaderHeight, laneHeight, laneSpacing, resources.Count, out int firstVisibleLane, out int lastVisibleLane);
            var eventsByLane = BuildTimelineLaneEventLookup(resources, startOfWeek, startOfWeek.AddDays(dayCount - 1));

            for (int laneIndex = firstVisibleLane; laneIndex <= lastVisibleLane; laneIndex++)
            {
                var lane = resources[laneIndex];
                int laneTop = grid.Y + dayHeaderHeight + laneIndex * (laneHeight + laneSpacing) + laneSpacing;
                var laneRect = new Rectangle(contentArea.X, laneTop, contentArea.Width, laneHeight);
                if (!laneRect.Contains(location))
                {
                    continue;
                }

                if (eventsByLane.TryGetValue(lane.Id ?? string.Empty, out var laneEvents))
                {
                    foreach (var evt in laneEvents.OrderByDescending(e => e.StartTime))
                    {
                        var eventRect = GetTimelineEventRect(evt, startOfWeek, dayCount, laneRect, laneTop, laneHeight);
                        if (eventRect.Contains(location))
                        {
                            return new CalendarInteractionHitTestResult
                            {
                                TargetKind = CalendarInteractionTargetKind.EventBlock,
                                RequestedMode = CalendarInteractionMode.SelectEvent,
                                Location = location,
                                Date = evt.StartTime.Date,
                                Event = evt,
                                Bounds = eventRect
                            };
                        }
                    }
                }

                int selectedDay = CalendarLayoutGeometry.GetColumnIndex(contentArea, location.X, dayCount);
                if (selectedDay >= 0)
                {
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.DateCell,
                        RequestedMode = CalendarInteractionMode.CreateEvent,
                        Location = location,
                        Date = startOfWeek.AddDays(selectedDay).Date,
                        Bounds = CalendarLayoutGeometry.GetColumnRect(laneRect, selectedDay, dayCount)
                    };
                }

                return CreateEmptyTimelineHit(location);
            }

            return CreateEmptyTimelineHit(location);
        }

        private IEnumerable<CalendarResource> ResolveTimelineResourceLanes()
        {
            var visible = Resources?
                .Where(r => r != null && r.IsVisible)
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.Name)
                .ToList() ?? new List<CalendarResource>();

            if (visible.Count > 0)
            {
                return visible;
            }

            var fromEvents = (_eventService?.GetEventsForMonth(_state.CurrentDate) ?? new List<CalendarEvent>())
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

        private Dictionary<string, List<CalendarEvent>> BuildTimelineLaneEventLookup(IReadOnlyList<CalendarResource> resources, DateTime windowStart, DateTime windowEnd)
        {
            var lookup = new Dictionary<string, List<CalendarEvent>>(StringComparer.OrdinalIgnoreCase);
            foreach (var resource in resources)
            {
                lookup[resource.Id ?? string.Empty] = new List<CalendarEvent>();
            }

            var events = (_eventService?.GetEventsForDateRange(windowStart, windowEnd) ?? new List<CalendarEvent>()).OrderBy(evt => evt.StartTime);
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

        private Rectangle GetTimelineEventRect(CalendarEvent evt, DateTime startOfWeek, int dayCount, Rectangle laneRect, int laneTop, int laneHeight)
        {
            int startOffset = Math.Max(0, (evt.StartTime.Date - startOfWeek).Days);
            int endOffset = Math.Min(dayCount - 1, (evt.EndTime.Date - startOfWeek).Days);
            if (endOffset < startOffset)
            {
                endOffset = startOffset;
            }

            var firstDayRect = CalendarLayoutGeometry.GetColumnRect(laneRect, startOffset, dayCount);
            var lastDayRect = CalendarLayoutGeometry.GetColumnRect(laneRect, endOffset, dayCount);
            int insetX = ScaleMetric(CalendarLayoutMetrics.EventInsetX);
            int insetY = ScaleMetric(CalendarLayoutMetrics.EventInsetY);
            int x = firstDayRect.X + Math.Min(insetX, Math.Max(0, (firstDayRect.Width - 1) / 2));
            int width = Math.Max(24, lastDayRect.Right - firstDayRect.X - (insetX * 2));
            return new Rectangle(x, laneTop + insetY, width, Math.Max(1, laneHeight - (insetY * 2)));
        }

        private static void GetTimelineVisibleLaneRange(Rectangle grid, int dayHeaderHeight, int laneHeight, int laneSpacing, int laneCount, out int firstVisibleLane, out int lastVisibleLane)
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

        private static DateTime GetTimelineStartOfWeek(DateTime date)
        {
            return date.Date.AddDays(-(int)date.DayOfWeek);
        }

        private CalendarInteractionHitTestResult CreateEmptyTimelineHit(Point location)
        {
            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.EmptySurface,
                RequestedMode = CalendarInteractionMode.SelectDate,
                Location = location,
                Date = _state.CurrentDate.Date
            };
        }
    }
}
