using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gradient background painter - vertical primary to secondary gradient
    /// Generic gradient painter with state awareness
    /// </summary>
    public static class GradientBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, 
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            Color primary = BackgroundPainterHelpers.GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            Color secondary = BackgroundPainterHelpers.GetColor(style, StyleColors.GetSecondary, "Secondary", theme, useThemeColors);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Apply state adjustment to both colors
            primary = BackgroundPainterHelpers.GetStateAdjustedColor(
                primary, state, BackgroundPainterHelpers.StateIntensity.Normal);
            secondary = BackgroundPainterHelpers.GetStateAdjustedColor(
                secondary, state, BackgroundPainterHelpers.StateIntensity.Normal);

            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, primary, secondary, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }
    }
}
