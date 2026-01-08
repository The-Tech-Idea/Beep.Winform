using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
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

            // Ensure accessibility compliance for text containers
            if (useThemeColors && theme != null && theme.ForeColor != Color.Empty)
            {
                Color accessibleColor = ColorAccessibilityHelper.EnsureContrastRatio(
                    baseColor, theme.ForeColor, 
                    ColorAccessibilityHelper.WCAG_AA_Normal);
                if (accessibleColor != baseColor)
                {
                    baseColor = accessibleColor;
                }
            }

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // State-adjusted base color
            Color stateColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                baseColor, state, BackgroundPainterHelpers.StateIntensity.Subtle);

            // Stripe signature: 3% lighter at top for professional polish - use HSL for more natural results
            Color topColor = ColorAccessibilityHelper.LightenColor(stateColor, 0.03f);

            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, topColor, stateColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }
    }
}
