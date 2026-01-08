using System;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens
{
    /// <summary>
    /// Material Design 3 elevation system
    /// Maps elevation levels to shadow parameters for consistent depth perception
    /// </summary>
    public enum ElevationLevel
    {
        /// <summary>
        /// 0dp - No elevation (flat)
        /// </summary>
        Level0 = 0,

        /// <summary>
        /// 1dp - Subtle elevation (dividers, app bars)
        /// </summary>
        Level1 = 1,

        /// <summary>
        /// 2dp - Standard elevation (cards, buttons)
        /// </summary>
        Level2 = 2,

        /// <summary>
        /// 4dp - Medium elevation (raised buttons, FABs)
        /// </summary>
        Level4 = 4,

        /// <summary>
        /// 8dp - High elevation (dialogs, menus)
        /// </summary>
        Level8 = 8,

        /// <summary>
        /// 12dp - Very high elevation (modals, dropdowns)
        /// </summary>
        Level12 = 12,

        /// <summary>
        /// 16dp - Maximum elevation (tooltips, popovers)
        /// </summary>
        Level16 = 16,

        /// <summary>
        /// 24dp - Extreme elevation (rare use cases)
        /// </summary>
        Level24 = 24
    }

    /// <summary>
    /// Helper class for Material Design 3 elevation system
    /// Provides shadow parameter mapping and elevation transitions
    /// </summary>
    public static class ElevationSystem
    {
        /// <summary>
        /// Get shadow parameters for a given elevation level
        /// </summary>
        /// <param name="elevation">Elevation level</param>
        /// <returns>Shadow parameters (ambient alpha, key alpha, offset Y, spread)</returns>
        public static (int ambientAlpha, int keyAlpha, int offsetY, int spread) GetShadowParameters(ElevationLevel elevation)
        {
            int level = (int)elevation;
            
            return level switch
            {
                0 => (0, 0, 0, 0),
                1 => (25, 20, 1, 1),
                2 => (30, 30, 2, 2),
                4 => (35, 40, 4, 3),
                8 => (40, 50, 6, 4),
                12 => (45, 60, 8, 5),
                16 => (50, 70, 10, 6),
                24 => (60, 80, 12, 8),
                _ => GetShadowParametersForCustom(level)
            };
        }

        /// <summary>
        /// Get shadow parameters for custom elevation values
        /// </summary>
        private static (int ambientAlpha, int keyAlpha, int offsetY, int spread) GetShadowParametersForCustom(int level)
        {
            // Interpolate between known levels
            int ambientAlpha = Math.Min(80, 20 + (level * 2));
            int keyAlpha = Math.Min(100, 20 + (level * 3));
            int offsetY = Math.Min(12, level);
            int spread = Math.Min(8, level / 2);
            
            return (ambientAlpha, keyAlpha, offsetY, spread);
        }

        /// <summary>
        /// Get elevation for a control state (hover, pressed, focused)
        /// </summary>
        /// <param name="baseElevation">Base elevation level</param>
        /// <param name="state">Current control state</param>
        /// <returns>Adjusted elevation level</returns>
        public static ElevationLevel GetElevationForState(ElevationLevel baseElevation, ControlState state)
        {
            int baseLevel = (int)baseElevation;

            return state switch
            {
                ControlState.Hovered => (ElevationLevel)Math.Min(24, baseLevel + 2),
                ControlState.Pressed => (ElevationLevel)Math.Max(0, baseLevel - 1),
                ControlState.Focused => (ElevationLevel)Math.Min(24, baseLevel + 1),
                ControlState.Disabled => ElevationLevel.Level0,
                _ => baseElevation
            };
        }

        /// <summary>
        /// Get elevation transition duration in milliseconds
        /// </summary>
        public static int GetTransitionDuration(ElevationLevel from, ElevationLevel to)
        {
            int levelDiff = Math.Abs((int)from - (int)to);
            
            // Longer transitions for larger elevation changes
            return levelDiff switch
            {
                0 => 0,
                <= 2 => 150,   // Small changes: 150ms
                <= 4 => 200,   // Medium changes: 200ms
                <= 8 => 250,   // Large changes: 250ms
                _ => 300       // Very large changes: 300ms
            };
        }

        /// <summary>
        /// Get recommended elevation for control type
        /// </summary>
        public static ElevationLevel GetRecommendedElevation(ControlElevationType controlType)
        {
            return controlType switch
            {
                ControlElevationType.Button => ElevationLevel.Level2,
                ControlElevationType.Card => ElevationLevel.Level2,
                ControlElevationType.RaisedButton => ElevationLevel.Level4,
                ControlElevationType.FAB => ElevationLevel.Level8,
                ControlElevationType.Dialog => ElevationLevel.Level12,
                ControlElevationType.Modal => ElevationLevel.Level16,
                ControlElevationType.Menu => ElevationLevel.Level8,
                ControlElevationType.Dropdown => ElevationLevel.Level8,
                ControlElevationType.Tooltip => ElevationLevel.Level16,
                ControlElevationType.Divider => ElevationLevel.Level1,
                ControlElevationType.AppBar => ElevationLevel.Level1,
                _ => ElevationLevel.Level2
            };
        }
    }

    /// <summary>
    /// Control types for elevation recommendations
    /// </summary>
    public enum ControlElevationType
    {
        Button,
        Card,
        RaisedButton,
        FAB,
        Dialog,
        Modal,
        Menu,
        Dropdown,
        Tooltip,
        Divider,
        AppBar
    }
}
