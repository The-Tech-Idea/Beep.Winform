using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Windows 11 Mica background painter - Vertical gradient (2% darker at bottom)
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Windows11MicaBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Windows 11 Mica: Subtle mica material gradient
            Color baseColor = useThemeColors ? theme.BackColor : Color.FromArgb(243, 243, 243);
            baseColor = BackgroundPainterHelpers.ApplyState(baseColor, state);
            Color topColor = baseColor;
            Color bottomColor = BackgroundPainterHelpers.Darken(baseColor, 0.02f);

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
