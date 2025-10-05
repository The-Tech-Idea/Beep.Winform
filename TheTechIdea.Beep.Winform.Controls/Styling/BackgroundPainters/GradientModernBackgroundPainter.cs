using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gradient Modern background painter - Vertical gradient from primary to 30% darker
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class GradientModernBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Gradient Modern: Smooth vertical gradient
            Color primaryColor = useThemeColors ? theme.PrimaryColor : Color.FromArgb(103, 80, 164);
            primaryColor = BackgroundPainterHelpers.ApplyState(primaryColor, state);
            Color secondaryColor = BackgroundPainterHelpers.Darken(primaryColor, 0.3f);

            using (var brush = new LinearGradientBrush(bounds, primaryColor, secondaryColor, 90f))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // Apply state overlay
            Color stateOverlay = BackgroundPainterHelpers.GetStateOverlay(state);
            if (stateOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(stateOverlay))
                {
                    if (path != null)
                        g.FillPath(brush, path);
                    else
                        g.FillRectangle(brush, bounds);
                }
            }
        }
    }
}
