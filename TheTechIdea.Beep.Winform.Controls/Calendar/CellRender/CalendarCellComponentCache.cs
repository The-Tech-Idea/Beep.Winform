using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellRender
{
    /// <summary>
    /// Per-calendar cache of developer-supplied
    /// <see cref="IBeepUIComponent"/> instances, one per cell key. A cell key
    /// is formed by the painter (e.g. <c>"evt:42"</c> for the event with
    /// id 42, <c>"date:2026-06-04:cell:3,2"</c> for a date cell). The cache
    /// is keyed by string; the painter owns the key naming convention.
    ///
    /// Lifecycle: <b>create once, keep forever</b>. The cache is created on
    /// first use and never disposes its components. Switching the view mode
    /// does not invalidate the cache; switching to a different BeepCalendar
    /// instance does (since the cache is per-instance).
    /// </summary>
    public sealed class CalendarCellComponentCache
    {
        private readonly Dictionary<string, IBeepUIComponent> _cache
            = new Dictionary<string, IBeepUIComponent>(StringComparer.OrdinalIgnoreCase);

        private Func<CalendarCellContext, IBeepUIComponent> _factory;

        public void SetFactory(Func<CalendarCellContext, IBeepUIComponent> factory)
        {
            _factory = factory;
        }

        public Func<CalendarCellContext, IBeepUIComponent> GetFactory()
        {
            return _factory;
        }

        public IBeepUIComponent GetOrCreate(string cellKey, CalendarCellContext ctx)
        {
            if (string.IsNullOrEmpty(cellKey)) return null;
            if (_cache.TryGetValue(cellKey, out var existing) && existing != null)
            {
                return existing;
            }
            if (_factory == null) return null;
            var created = _factory(ctx);
            if (created == null) return null;
            _cache[cellKey] = created;
            return created;
        }

        /// <summary>True if a component has been cached for the given key.</summary>
        public bool Contains(string cellKey)
        {
            return !string.IsNullOrEmpty(cellKey) && _cache.ContainsKey(cellKey);
        }

        /// <summary>Remove a single component from the cache. The component is NOT disposed.</summary>
        public bool Remove(string cellKey)
        {
            return !string.IsNullOrEmpty(cellKey) && _cache.Remove(cellKey);
        }

        /// <summary>Dispose and remove a single cached component by key.</summary>
        public bool InvalidateKey(string cellKey)
        {
            if (string.IsNullOrEmpty(cellKey)) return false;
            if (_cache.TryGetValue(cellKey, out var comp) && comp is IDisposable d)
            {
                try { d.Dispose(); } catch { }
            }
            return _cache.Remove(cellKey);
        }

        /// <summary>Remove all cached components. Components are NOT disposed.</summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// W2-Redo-8 GAP 2 - dispose every cached component and clear the
        /// cache. Called from <c>BeepCalendar.Dispose</c> so W8 components
        /// (which may be WinForms Controls or arbitrary IBeepUIComponent
        /// instances) don't leak when the calendar is destroyed. Disposes
        /// any <see cref="IDisposable"/> component (Controls implement
        /// IDisposable). Safe to call multiple times.
        /// </summary>
        public void DisposeAll()
        {
            foreach (var kvp in _cache)
            {
                if (kvp.Value is IDisposable d)
                {
                    try { d.Dispose(); } catch { /* swallow on dispose */ }
                }
            }
            _cache.Clear();
        }

        /// <summary>Number of cached components.</summary>
        public int Count => _cache.Count;
    }
}
