using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Maps a <see cref="DockStyle"/> onto the wider Beep styling system.
    /// </summary>
    /// <remarks>
    /// This class used to carry eight per-style switch tables as well - recommended item size, dock
    /// height, spacing, padding, max scale, icon ratio, shadow and background opacity. They were a
    /// second set of per-style defaults that disagreed with <see cref="DockPainterMetrics"/> for 13 of
    /// 17 styles, and both were live at once: the control wrote these into <c>DockConfig</c> and laid
    /// out with them, while the painters that asked for metrics drew with the others. The tables are
    /// now merged into <see cref="DockPainterMetrics"/>, which is the single source; these values won
    /// the merge because they are the ones the layout was already using.
    ///
    /// What remains is the one thing this class is actually for: the mapping to
    /// <see cref="BeepControlStyle"/>, which is a different system with a different vocabulary.
    /// </remarks>
    public static class DockStyleHelpers
    {
        /// <summary>
        /// Maps DockStyle to BeepControlStyle
        /// </summary>
        public static BeepControlStyle GetControlStyleForDock(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => BeepControlStyle.iOS15,
                DockStyle.Windows11Dock => BeepControlStyle.Fluent2,
                DockStyle.Material3Dock => BeepControlStyle.Material3,
                DockStyle.MinimalDock => BeepControlStyle.Minimal,
                DockStyle.GlassmorphismDock => BeepControlStyle.Material3,
                DockStyle.NeumorphismDock => BeepControlStyle.Material3,
                DockStyle.iOSDock => BeepControlStyle.iOS15,
                DockStyle.GNOMEDock => BeepControlStyle.Material3,
                DockStyle.PlasmaPanel => BeepControlStyle.Material3,
                DockStyle.PlankDock => BeepControlStyle.Minimal,
                DockStyle.NeonDock => BeepControlStyle.Material3,
                DockStyle.NordDock => BeepControlStyle.Material3,
                DockStyle.CyberpunkDock => BeepControlStyle.GlassAcrylic,
                DockStyle.TerminalDock => BeepControlStyle.Minimal,
                DockStyle.BubbleDock => BeepControlStyle.iOS15,
                DockStyle.ArcDock => BeepControlStyle.Minimal,
                DockStyle.DraculaDock => BeepControlStyle.GlassAcrylic,
                _ => BeepControlStyle.Material3
            };
        }

        /// <summary>
        /// Gets border radius for dock based on control style
        /// </summary>
        public static int GetBorderRadius(BeepControlStyle controlStyle, int dockHeight)
        {
            return BeepStyling.GetRadius(controlStyle);
        }
    }
}
