using System;
using System.Collections.Generic;
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
            // Check if cache needs reset
            CheckCacheValidity();
            
            var dateKey = date.Date;
            if (_dateCache.TryGetValue(dateKey, out var cached))
            {
                return cached;
            }

            var result = _events.Where(e => e.StartTime.Date == dateKey).ToList();
            _dateCache[dateKey] = result;
            return result;
        }

        /// <summary>
        /// Gets events for a month with caching
        /// </summary>
        public List<CalendarEvent> GetEventsForMonth(DateTime date)
        {
            CheckCacheValidity();
            
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            var key = (firstDay, lastDay);
            
            if (_monthCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var result = _events.Where(e => e.StartTime.Date >= firstDay && e.StartTime.Date <= lastDay).ToList();
            _monthCache[key] = result;
            return result;
        }

        /// <summary>
        /// Gets events for a date range with caching
        /// </summary>
        public List<CalendarEvent> GetEventsForDateRange(DateTime startDate, DateTime endDate)
        {
            CheckCacheValidity();
            
            var key = (startDate.Date, endDate.Date);
            
            if (_monthCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var result = _events.Where(e => e.StartTime.Date >= startDate.Date && e.StartTime.Date <= endDate.Date).ToList();
            _monthCache[key] = result;
            return result;
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
            return _events.Count(e => e.StartTime.Date == dateKey);
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
