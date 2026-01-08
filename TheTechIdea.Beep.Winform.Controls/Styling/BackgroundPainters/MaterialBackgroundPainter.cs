using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material Design background painter - Google's material design language
    /// Clean surface with subtle elevation hint (top highlight)
    /// </summary>
    public static class MaterialBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal,
            ElevationLevel elevation = ElevationLevel.Level2)
        {
            if (g == null || path == null) return;

            // Material: clean white/light surface
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Material);

            // Get primary color for state layers
            Color primaryColor = useThemeColors && theme != null
                ? theme.PrimaryColor
                : StyleColors.GetPrimary(BeepControlStyle.Material);

            // Use ElevationSystem for state-aware elevation
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(elevation, state);

            // Solid background with strong Material-style state feedback
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);

            // Material elevation hint: subtle top highlight
            BackgroundPainterHelpers.PaintTopHighlight(g, path, 12);

            // Material Design 2 state layer overlay (for interactive states)
            PaintStateLayerOverlay(g, path, primaryColor, state);
        }

        /// <summary>
        /// Paint Material Design 2 state layer overlay
        /// </summary>
        private static void PaintStateLayerOverlay(Graphics g, GraphicsPath path, Color primaryColor, ControlState state)
        {
            int overlayAlpha = state switch
            {
                ControlState.Hovered => 8,
                ControlState.Pressed => 12,
                ControlState.Selected => 12,
                ControlState.Focused => 12,
                _ => 0
            };

            if (overlayAlpha > 0)
            {
                var overlayBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(overlayAlpha, primaryColor));
                g.FillPath(overlayBrush, path);
            }
        }
    }
}
