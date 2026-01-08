using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Caching
{
    /// <summary>
    /// LRU cache for GraphicsPath objects to improve rendering performance
    /// Caches rounded rectangle paths by (bounds, radius) key
    /// </summary>
    public class PathCache
    {
        private readonly Dictionary<string, CachedPath> _cache;
        private readonly int _maxSize;
        private readonly object _lock = new object();

        /// <summary>
        /// Initialize path cache with maximum size
        /// </summary>
        /// <param name="maxSize">Maximum number of cached paths (default 100)</param>
        public PathCache(int maxSize = 100)
        {
            _maxSize = maxSize;
            _cache = new Dictionary<string, CachedPath>();
        }

        /// <summary>
        /// Get or create a rounded rectangle path
        /// </summary>
        /// <param name="bounds">Rectangle bounds</param>
        /// <param name="radius">Corner radius</param>
        /// <returns>Cached or newly created GraphicsPath</returns>
        public GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            string key = CreateKey(bounds, radius);

            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var cached))
                {
                    // Update access time for LRU
                    cached.LastAccessed = DateTime.UtcNow;
                    cached.AccessCount++;
                    return (GraphicsPath)cached.Path.Clone(); // Return clone to avoid disposal issues
                }

                // Create new path
                var path = CreateRoundedRectanglePath(bounds, radius);
                
                // Add to cache (evict if needed)
                if (_cache.Count >= _maxSize)
                {
                    EvictLeastRecentlyUsed();
                }

                _cache[key] = new CachedPath
                {
                    Path = path,
                    LastAccessed = DateTime.UtcNow,
                    AccessCount = 1
                };

                return (GraphicsPath)path.Clone();
            }
        }

        /// <summary>
        /// Create a cache key from bounds and radius
        /// </summary>
        private string CreateKey(Rectangle bounds, int radius)
        {
            return $"{bounds.X},{bounds.Y},{bounds.Width},{bounds.Height},{radius}";
        }

        /// <summary>
        /// Create a rounded rectangle GraphicsPath
        /// </summary>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Evict least recently used path from cache
        /// </summary>
        private void EvictLeastRecentlyUsed()
        {
            var lru = _cache.OrderBy(kvp => kvp.Value.LastAccessed)
                           .ThenBy(kvp => kvp.Value.AccessCount)
                           .First();

            lru.Value.Path?.Dispose();
            _cache.Remove(lru.Key);
        }

        /// <summary>
        /// Clear all cached paths
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                foreach (var cached in _cache.Values)
                {
                    cached.Path?.Dispose();
                }
                _cache.Clear();
            }
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public PathCacheStatistics GetStatistics()
        {
            lock (_lock)
            {
                return new PathCacheStatistics
                {
                    CacheSize = _cache.Count,
                    MaxSize = _maxSize,
                    TotalAccesses = _cache.Values.Sum(c => c.AccessCount),
                    HitRate = _cache.Count > 0 
                        ? _cache.Values.Sum(c => c.AccessCount) / (double)_cache.Count 
                        : 0
                };
            }
        }

        /// <summary>
        /// Cached path entry
        /// </summary>
        private class CachedPath
        {
            public GraphicsPath Path { get; set; }
            public DateTime LastAccessed { get; set; }
            public int AccessCount { get; set; }
        }
    }

    /// <summary>
    /// Path cache statistics
    /// </summary>
    public class PathCacheStatistics
    {
        public int CacheSize { get; set; }
        public int MaxSize { get; set; }
        public int TotalAccesses { get; set; }
        public double HitRate { get; set; }
    }
}
