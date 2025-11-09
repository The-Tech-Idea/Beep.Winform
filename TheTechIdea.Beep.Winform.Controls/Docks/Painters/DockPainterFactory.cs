using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Factory for creating dock painters based on DockStyle
    /// </summary>
    public static class DockPainterFactory
    {
        private static readonly Dictionary<DockStyle, IDockPainter> _painters = new Dictionary<DockStyle, IDockPainter>();

        static DockPainterFactory()
        {
            // Register all painters
            _painters[DockStyle.AppleDock] = new AppleDockPainter();
            _painters[DockStyle.Windows11Dock] = new Windows11DockPainter();
            _painters[DockStyle.Material3Dock] = new Material3DockPainter();
            _painters[DockStyle.MinimalDock] = new MinimalDockPainter();
            _painters[DockStyle.GlassmorphismDock] = new GlassmorphismDockPainter();
            _painters[DockStyle.PillDock] = new FloatingDockPainter(); // Pill and Floating are the same style
            _painters[DockStyle.NeumorphismDock] = new NeumorphicDockPainter();
            _painters[DockStyle.NeonDock] = new NeonDockPainter();
        }

        /// <summary>
        /// Gets a dock painter instance for the specified style
        /// </summary>
        /// <param name="style">The DockStyle to get a painter for</param>
        /// <returns>The appropriate IDockPainter implementation</returns>
        public static IDockPainter GetPainter(DockStyle style)
        {
            if (_painters.TryGetValue(style, out var painter))
            {
                return painter;
            }

            // Default to Apple Dock if style not found
            return _painters[DockStyle.AppleDock];
        }
    }
}
