using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Apple background painter - clean, refined aesthetics
    /// Premium feel with subtle vertical gradient for depth
    /// </summary>
    public static class AppleBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Apple: clean white/light surface
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Apple);

            // Apple uses subtle, refined state changes
            Color stateColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                baseColor, state, BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Apple signature: subtle vertical gradient (lighter top to slightly darker bottom)
            Color topColor = stateColor;
            Color bottomColor = BackgroundPainterHelpers.Darken(stateColor, 0.02f);
            
            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, topColor, bottomColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }
    }
}
