using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Ant Design background painter - Solid white background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class AntDesignBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Ant Design: Clean white solid
            Color backgroundColor = Color.White;
            backgroundColor = BackgroundPainterHelpers.ApplyState(backgroundColor, state);

            using (var brush = new SolidBrush(backgroundColor))
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
