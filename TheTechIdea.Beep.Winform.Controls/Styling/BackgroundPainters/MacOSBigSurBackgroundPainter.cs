using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// macOS Big Sur background painter - Vertical gradient (5% lighter at top)
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MacOSBigSurBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // macOS Big Sur: Subtle vertical gradient
            Color baseColor = useThemeColors ? theme.BackColor : Color.FromArgb(250, 250, 250);
            baseColor = BackgroundPainterHelpers.ApplyState(baseColor, state);
            Color topColor = BackgroundPainterHelpers.Lighten(baseColor, 0.05f);
            Color bottomColor = baseColor;

            using (var brush = new LinearGradientBrush(bounds, topColor, bottomColor, 90f))
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
