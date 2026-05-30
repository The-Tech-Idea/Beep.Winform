using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.AutoHide;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Caption;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Splitter;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Factory for docking chrome painting. Vends two related things:
    /// <list type="bullet">
    /// <item><see cref="GetRenderers"/> — the per-style set of distinct element renderers used to
    /// paint captions, splitters, etc. This is the single place that maps a
    /// <see cref="BeepControlStyle"/> to concrete renderers.</item>
    /// <item><see cref="GetPainter"/> — the theme/metrics provider (<see cref="IDockingPainter"/>)
    /// that supplies resolved colors, fonts and layout metrics.</item>
    /// </list>
    /// Both are cached; call <see cref="ClearCache"/> to reset.
    /// </summary>
    public static class DockingPainterFactory
    {
        private static Dictionary<string, IDockingPainter> _painterCache = new Dictionary<string, IDockingPainter>();
        private static IDockingPainter _defaultPainter = NullDockingPainter.Instance;

        private static readonly Dictionary<BeepControlStyle, DockingRendererSet> _rendererCache
            = new Dictionary<BeepControlStyle, DockingRendererSet>();
        private static readonly object _rendererLock = new object();

        /// <summary>
        /// Gets (or creates and caches) the renderer set for the given control style. Renderers are
        /// currently style-agnostic — they paint flat themed chrome driven by the render context —
        /// so style-specific renderers can be substituted here later without touching call sites.
        /// </summary>
        internal static DockingRendererSet GetRenderers(BeepControlStyle style)
        {
            lock (_rendererLock)
            {
                if (_rendererCache.TryGetValue(style, out var set))
                    return set;

                set = CreateRendererSet(style);
                _rendererCache[style] = set;
                return set;
            }
        }

        private static DockingRendererSet CreateRendererSet(BeepControlStyle style)
        {
            // Branch on style here when style-specific renderers are introduced.
            return new DockingRendererSet(new CaptionRenderer(), new SplitterRenderer(), new AutoHideStripRenderer());
        }

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
            lock (_rendererLock)
                _rendererCache.Clear();
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
