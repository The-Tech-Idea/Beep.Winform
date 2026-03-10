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
            
            // Style-specific painters for previously fallback styles
            _painters[DockStyle.CyberpunkDock] = new CyberpunkDockPainter();
            _painters[DockStyle.TerminalDock] = new TerminalDockPainter();
            _painters[DockStyle.BubbleDock] = new BubbleDockPainter();
            _painters[DockStyle.ArcDock] = new ArcDockPainter();
            _painters[DockStyle.DraculaDock] = new DraculaDockPainter();
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
