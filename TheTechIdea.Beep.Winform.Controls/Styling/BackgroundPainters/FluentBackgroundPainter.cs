using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Fluent background painter - Microsoft Fluent Design System
    /// Acrylic-inspired materials with subtle depth through diagonal gradient
    /// </summary>
    public static class FluentBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Fluent: clean system background
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Fluent);

            // State handling with strong Fluent-style feedback
            Color stateColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                baseColor, state, BackgroundPainterHelpers.StateIntensity.Strong);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Fluent signature: subtle diagonal acrylic-like gradient
            Color secondary = BackgroundPainterHelpers.Darken(stateColor, 0.03f);

            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, stateColor, secondary, LinearGradientMode.ForwardDiagonal);
            
            // Custom color blend for acrylic feel (lighter in middle)
            try
            {
                var blend = new ColorBlend
                {
                    Colors = new[] { 
                        stateColor, 
                        BackgroundPainterHelpers.Lighten(stateColor, 0.02f), 
                        stateColor 
                    },
                    Positions = new[] { 0f, 0.5f, 1f }
                };
                brush.InterpolationColors = blend;
            }
            catch { /* Fallback to linear gradient if blend fails */ }

            g.FillPath(brush, path);
        }
    }
}
