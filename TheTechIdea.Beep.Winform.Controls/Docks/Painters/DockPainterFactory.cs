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
            // Modern OS styles
            _painters[DockStyle.AppleDock] = new AppleDockPainter();
            _painters[DockStyle.Windows11Dock] = new Windows11DockPainter();
            _painters[DockStyle.iOSDock] = new iOSDockPainter();
            
            // Linux DE styles
            _painters[DockStyle.GNOMEDock] = new GNOMEDockPainter();
            _painters[DockStyle.PlasmaPanel] = new PlasmaPanelPainter();
            _painters[DockStyle.PlankDock] = new PlankDockPainter();
            
            // Design systems
            _painters[DockStyle.Material3Dock] = new Material3DockPainter();
            _painters[DockStyle.MinimalDock] = new MinimalDockPainter();
            
            // Visual effects
            _painters[DockStyle.GlassmorphismDock] = new GlassmorphismDockPainter();
            _painters[DockStyle.PillDock] = new FloatingDockPainter();
            _painters[DockStyle.NeumorphismDock] = new NeumorphicDockPainter();
            _painters[DockStyle.NeonDock] = new NeonDockPainter();
            
            // Theme-based styles
            _painters[DockStyle.NordDock] = new NordDockPainter();
            
            // Use Classic Taskbar as fallback for unimplemented styles
            var classicPainter = new ClassicTaskbarDockPainter();
            _painters[DockStyle.CyberpunkDock] = classicPainter;
            _painters[DockStyle.TerminalDock] = classicPainter;
            _painters[DockStyle.BubbleDock] = classicPainter;
            _painters[DockStyle.ArcDock] = classicPainter;
            _painters[DockStyle.DraculaDock] = classicPainter;
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
