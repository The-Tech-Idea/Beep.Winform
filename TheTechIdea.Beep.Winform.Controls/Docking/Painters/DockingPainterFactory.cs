using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Factory for creating and caching docking painter instances based on theme.
    /// </summary>
    public static class DockingPainterFactory
    {
        private static Dictionary<string, IDockingPainter> _painterCache = new Dictionary<string, IDockingPainter>();
        private static IDockingPainter _defaultPainter = NullDockingPainter.Instance;

        /// <summary>
        /// Gets or creates a painter for the given theme.
        /// </summary>
        public static IDockingPainter GetPainter(string themeName = "Default")
        {
            if (string.IsNullOrEmpty(themeName))
                themeName = "Default";

            // Return cached painter if available
            if (_painterCache.TryGetValue(themeName, out var painter))
                return painter;

            // Return default painter if available
            if (_defaultPainter != null)
            {
                _painterCache[themeName] = _defaultPainter;
                return _defaultPainter;
            }

            // Keep designer and runtime construction safe even before themes register.
            _painterCache[themeName] = NullDockingPainter.Instance;
            return NullDockingPainter.Instance;
        }

        /// <summary>
        /// Registers a custom painter for a theme name.
        /// </summary>
        public static void RegisterPainter(string themeName, IDockingPainter painter)
        {
            if (string.IsNullOrEmpty(themeName))
                throw new ArgumentException("Theme name cannot be empty", nameof(themeName));

            _painterCache[themeName] = painter ?? throw new ArgumentNullException(nameof(painter));
        }

        /// <summary>
        /// Clears all cached painters.
        /// </summary>
        public static void ClearCache()
        {
            _painterCache.Clear();
        }

        /// <summary>
        /// Sets the default painter to use when theme is not found.
        /// </summary>
        public static void SetDefaultPainter(IDockingPainter painter)
        {
            _defaultPainter = painter;
            if (painter != null)
            {
                _painterCache["Default"] = painter;
            }
        }
    }
}
