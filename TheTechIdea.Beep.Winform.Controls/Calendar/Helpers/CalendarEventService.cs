using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    /// <summary>
    /// Service for querying calendar events with per-paint-cycle caching
    /// </summary>
    internal class CalendarEventService
    {
        private readonly List<CalendarEvent> _events;
        
        // Cache for event queries - invalidated each paint cycle
        private Dictionary<DateTime, List<CalendarEvent>> _dateCache;
        private Dictionary<(DateTime, DateTime), List<CalendarEvent>> _monthCache;
        private int _cacheVersion;
        private int _lastCacheVersion;
        internal Action<TimeSpan> QueryTimingSink { get; set; }

        public CalendarEventService(List<CalendarEvent> events)
        {
            _events = events ?? new List<CalendarEvent>();
            _dateCache = new Dictionary<DateTime, List<CalendarEvent>>();
            _monthCache = new Dictionary<(DateTime, DateTime), List<CalendarEvent>>();
            _cacheVersion = 0;
            _lastCacheVersion = -1;
        }

        /// <summary>
        /// Call at the start of each paint cycle to reset caches
        /// </summary>
        public void BeginPaintCycle()
        {
            _cacheVersion++;
        }

        /// <summary>
        /// Invalidates all caches (call when events change)
        /// </summary>
        public void InvalidateCache()
        {
            _dateCache.Clear();
            _monthCache.Clear();
            _cacheVersion++;
        }

        /// <summary>
        /// Gets events for a specific date with caching
        /// </summary>
        public List<CalendarEvent> GetEventsForDate(DateTime date)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                CheckCacheValidity();

                var dateKey = date.Date;
                if (_dateCache.TryGetValue(dateKey, out var cached))
                {
                    return cached;
                }

                var dayStart = dateKey;
                var dayEnd = dayStart.AddDays(1).AddTicks(-1);
                var result = _events.Where(e => EventOverlapsRange(e, dayStart, dayEnd)).ToList();
                _dateCache[dateKey] = result;
                return result;
            }
            finally
            {
                stopwatch.Stop();
                QueryTimingSink?.Invoke(stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Gets events for a month with caching
        /// </summary>
        public List<CalendarEvent> GetEventsForMonth(DateTime date)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                CheckCacheValidity();

                var firstDay = new DateTime(date.Year, date.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var key = (firstDay, lastDay);

                if (_monthCache.TryGetValue(key, out var cached))
                {
                    return cached;
                }

                var monthStart = firstDay.Date;
                var monthEnd = lastDay.Date.AddDays(1).AddTicks(-1);
                var result = _events.Where(e => EventOverlapsRange(e, monthStart, monthEnd)).ToList();
                _monthCache[key] = result;
                return result;
            }
            finally
            {
                stopwatch.Stop();
                QueryTimingSink?.Invoke(stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Gets events for a date range with caching
        /// </summary>
        public List<CalendarEvent> GetEventsForDateRange(DateTime startDate, DateTime endDate)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                CheckCacheValidity();

                var key = (startDate.Date, endDate.Date);

                if (_monthCache.TryGetValue(key, out var cached))
                {
                    return cached;
                }

                var rangeStart = startDate.Date;
                var rangeEnd = endDate.Date.AddDays(1).AddTicks(-1);
                var result = _events.Where(e => EventOverlapsRange(e, rangeStart, rangeEnd)).ToList();
                _monthCache[key] = result;
                return result;
            }
            finally
            {
                stopwatch.Stop();
                QueryTimingSink?.Invoke(stopwatch.Elapsed);
            }
        }

        public List<CalendarEvent> GetFilteredEvents(CalendarEventFilter filter)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                CheckCacheValidity();

                if (filter == null)
                {
                    return _events.ToList();
                }

                IEnumerable<CalendarEvent> query = _events;

                if (filter.RangeStart.HasValue || filter.RangeEnd.HasValue)
                {
                    var start = filter.RangeStart?.Date ?? DateTime.MinValue;
                    var end = (filter.RangeEnd?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);
                    query = query.Where(e => EventOverlapsRange(e, start, end));
                }

                if (filter.CategoryIds != null && filter.CategoryIds.Count > 0)
                {
                    query = query.Where(e => filter.CategoryIds.Contains(e.CategoryId));
                }

                if (filter.Statuses != null && filter.Statuses.Count > 0)
                {
                    query = query.Where(e => filter.Statuses.Contains(e.Status));
                }

                if (!filter.IncludeAllDayEvents)
                {
                    query = query.Where(e => !e.IsAllDay);
                }

                if (filter.Tags != null && filter.Tags.Count > 0)
                {
                    query = query.Where(e => e.Tags != null && e.Tags.Any(t => filter.Tags.Contains(t, StringComparer.OrdinalIgnoreCase)));
                }

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    var term = filter.SearchText.Trim();
                    query = query.Where(e =>
                        (!string.IsNullOrWhiteSpace(e.Title) && e.Title.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(e.Description) && e.Description.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(e.Location) && e.Location.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                        (e.Tags != null && e.Tags.Any(t => t != null && t.Contains(term, StringComparison.OrdinalIgnoreCase))));
                }

                return query.ToList();
            }
            finally
            {
                stopwatch.Stop();
                QueryTimingSink?.Invoke(stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Gets the count of events for a date (optimized, no list allocation)
        /// </summary>
        public int GetEventCountForDate(DateTime date)
        {
            // Use cached list if available
            var dateKey = date.Date;
            if (_dateCache.TryGetValue(dateKey, out var cached))
            {
                return cached.Count;
            }
            
            // Otherwise count directly
            var dayStart = dateKey;
            var dayEnd = dayStart.AddDays(1).AddTicks(-1);
            return _events.Count(e => EventOverlapsRange(e, dayStart, dayEnd));
        }

        private static bool EventOverlapsRange(CalendarEvent calendarEvent, DateTime rangeStart, DateTime rangeEnd)
        {
            if (calendarEvent == null)
            {
                return false;
            }

            return calendarEvent.StartTime <= rangeEnd && calendarEvent.EndTime >= rangeStart;
        }

        private void CheckCacheValidity()
        {
            if (_lastCacheVersion != _cacheVersion)
            {
                _dateCache.Clear();
                _monthCache.Clear();
                _lastCacheVersion = _cacheVersion;
            }
        }
    }
}
