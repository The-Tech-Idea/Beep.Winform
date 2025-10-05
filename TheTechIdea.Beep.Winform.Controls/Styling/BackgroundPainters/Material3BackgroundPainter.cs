using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material 3 background painter - Solid with 10% white elevation highlight
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Material3BackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            // Material 3: Solid background with subtle elevation highlight
            Color backgroundColor = useThemeColors ? theme.BackColor : Color.White;
            
            // Apply state modification
            backgroundColor = BackgroundPainterHelpers.ApplyState(backgroundColor, state);

            using (var brush = new SolidBrush(backgroundColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // Add elevation highlight (10% white overlay)
            Color elevationColor = BackgroundPainterHelpers.WithAlpha(Color.White, 25);
            using (var brush = new SolidBrush(elevationColor))
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
