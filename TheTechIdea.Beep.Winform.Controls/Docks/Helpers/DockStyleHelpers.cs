using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Helper class for mapping DockStyle and BeepControlStyle to dock styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
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

        /// <summary>
        /// Gets recommended item size for dock style
        /// </summary>
        public static int GetRecommendedItemSize(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => 56,
                DockStyle.Windows11Dock => 48,
                DockStyle.Material3Dock => 56,
                DockStyle.MinimalDock => 44,
                DockStyle.GlassmorphismDock => 56,
                DockStyle.NeumorphismDock => 56,
                DockStyle.iOSDock => 60,
                DockStyle.GNOMEDock => 48,
                DockStyle.PlasmaPanel => 48,
                DockStyle.PlankDock => 40,
                DockStyle.NeonDock => 56,
                DockStyle.NordDock => 56,
                _ => 56
            };
        }

        /// <summary>
        /// Gets recommended dock height for dock style
        /// </summary>
        public static int GetRecommendedDockHeight(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => 72,
                DockStyle.Windows11Dock => 64,
                DockStyle.Material3Dock => 72,
                DockStyle.MinimalDock => 56,
                DockStyle.GlassmorphismDock => 72,
                DockStyle.NeumorphismDock => 72,
                DockStyle.iOSDock => 80,
                DockStyle.GNOMEDock => 64,
                DockStyle.PlasmaPanel => 64,
                DockStyle.PlankDock => 48,
                DockStyle.NeonDock => 72,
                DockStyle.NordDock => 72,
                _ => 72
            };
        }

        /// <summary>
        /// Gets recommended spacing between items
        /// </summary>
        public static int GetRecommendedSpacing(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => 8,
                DockStyle.Windows11Dock => 6,
                DockStyle.Material3Dock => 8,
                DockStyle.MinimalDock => 4,
                DockStyle.GlassmorphismDock => 8,
                DockStyle.NeumorphismDock => 8,
                DockStyle.iOSDock => 10,
                DockStyle.GNOMEDock => 6,
                DockStyle.PlasmaPanel => 6,
                DockStyle.PlankDock => 4,
                DockStyle.NeonDock => 8,
                DockStyle.NordDock => 8,
                _ => 8
            };
        }

        /// <summary>
        /// Gets recommended padding for dock
        /// </summary>
        public static int GetRecommendedPadding(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => 12,
                DockStyle.Windows11Dock => 10,
                DockStyle.Material3Dock => 12,
                DockStyle.MinimalDock => 8,
                DockStyle.GlassmorphismDock => 12,
                DockStyle.NeumorphismDock => 12,
                DockStyle.iOSDock => 14,
                DockStyle.GNOMEDock => 10,
                DockStyle.PlasmaPanel => 10,
                DockStyle.PlankDock => 8,
                DockStyle.NeonDock => 12,
                DockStyle.NordDock => 12,
                _ => 12
            };
        }

        /// <summary>
        /// Gets recommended maximum scale for hover effect
        /// </summary>
        public static float GetRecommendedMaxScale(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => 1.5f,
                DockStyle.Windows11Dock => 1.3f,
                DockStyle.Material3Dock => 1.4f,
                DockStyle.MinimalDock => 1.2f,
                DockStyle.GlassmorphismDock => 1.5f,
                DockStyle.NeumorphismDock => 1.4f,
                DockStyle.iOSDock => 1.6f,
                DockStyle.GNOMEDock => 1.3f,
                DockStyle.PlasmaPanel => 1.3f,
                DockStyle.PlankDock => 1.2f,
                DockStyle.NeonDock => 1.5f,
                DockStyle.NordDock => 1.4f,
                _ => 1.5f
            };
        }

        /// <summary>
        /// Gets icon size ratio for dock items (as percentage of item size)
        /// </summary>
        public static float GetIconSizeRatio(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => 0.8f,
                DockStyle.Windows11Dock => 0.75f,
                DockStyle.Material3Dock => 0.8f,
                DockStyle.MinimalDock => 0.85f,
                DockStyle.GlassmorphismDock => 0.8f,
                DockStyle.NeumorphismDock => 0.8f,
                DockStyle.iOSDock => 0.85f,
                DockStyle.GNOMEDock => 0.75f,
                DockStyle.PlasmaPanel => 0.75f,
                DockStyle.PlankDock => 0.9f,
                DockStyle.NeonDock => 0.8f,
                DockStyle.NordDock => 0.8f,
                _ => 0.8f
            };
        }

        /// <summary>
        /// Determines if shadow should be shown for dock
        /// </summary>
        public static bool ShouldShowShadow(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => true,
                DockStyle.Windows11Dock => true,
                DockStyle.Material3Dock => true,
                DockStyle.MinimalDock => false,
                DockStyle.GlassmorphismDock => true,
                DockStyle.NeumorphismDock => true,
                DockStyle.iOSDock => true,
                DockStyle.GNOMEDock => false,
                DockStyle.PlasmaPanel => false,
                DockStyle.PlankDock => false,
                DockStyle.NeonDock => true,
                DockStyle.NordDock => false,
                _ => true
            };
        }

        /// <summary>
        /// Gets recommended background opacity
        /// </summary>
        public static float GetRecommendedBackgroundOpacity(DockStyle dockStyle)
        {
            return dockStyle switch
            {
                DockStyle.AppleDock => 0.85f,
                DockStyle.Windows11Dock => 0.9f,
                DockStyle.Material3Dock => 0.8f,
                DockStyle.MinimalDock => 1.0f,
                DockStyle.GlassmorphismDock => 0.7f,
                DockStyle.NeumorphismDock => 0.9f,
                DockStyle.iOSDock => 0.85f,
                DockStyle.GNOMEDock => 0.9f,
                DockStyle.PlasmaPanel => 0.9f,
                DockStyle.PlankDock => 1.0f,
                DockStyle.NeonDock => 0.8f,
                DockStyle.NordDock => 0.85f,
                _ => 0.85f
            };
        }
    }
}
