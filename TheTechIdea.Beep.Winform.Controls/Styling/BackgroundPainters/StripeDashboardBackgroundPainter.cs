using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Stripe Dashboard background painter - professional fintech aesthetic
    /// Solid background with 3% lighter top gradient for polish
    /// </summary>
    public static class StripeDashboardBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Stripe Dashboard: clean white
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.StripeDashboard);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // State-adjusted base color
            Color stateColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                baseColor, state, BackgroundPainterHelpers.StateIntensity.Subtle);

            // Stripe signature: 3% lighter at top for professional polish
            Color topColor = BackgroundPainterHelpers.Lighten(stateColor, 0.03f);

            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, topColor, stateColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }
    }
}
